using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Extensions;

namespace Taipla.Webservice.Helpers.ExportExcel
{
    public class ExcelService : IExcelService
    {
        private Random rand = new Random();
        private string builderTemp { get; set; }


        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ExcelService(IConfiguration configuration
            , IWebHostEnvironment env)
        {
            DateTimeExtension.SetDateEnv();

            _configuration = configuration;
            _env = env;

            builderTemp = _configuration.GetSection("Application:Excel:Public").Value.Replace("\\", "/");

            if (!System.IO.Directory.Exists(builderTemp))
            {
                System.IO.Directory.CreateDirectory(builderTemp);
            }
        }

        public ExportConfig Config(string configFile)
        {
            var configFileJson = string.Format("{0}/{1}.json",
              _configuration.GetSection("Application:Excel:Config").Value.Replace("\\", "/").TrimEnd('/'),
              configFile.Trim('/'));

            if (!System.IO.File.Exists(configFileJson))
            {
                throw new Exception("Cannot load config, File not found.");
            }

            var fileName = string.Format(DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-" + rand.Next(100, 999) + ".xlsx");
            var builderFile = System.IO.Path.Combine(builderTemp, fileName);

            if (System.IO.File.Exists(builderFile))
            {
                System.IO.File.Delete(fileName);
            }

            ExportConfig config = this.CreateConfig(configFileJson);


            if (string.IsNullOrEmpty(config.TEMPORARY_PATH))
            {
                throw new Exception("Cannot load config, Please check path is not empty.");
            }


            return config;
        }

        public ExportExcelResult Export(string configFile
            , Action<ExportConfig> action, DataTable table
            , Action<ExcelRange, string, object> overrideValue)
        {
            var configFileJson = string.Format("{0}/{1}.json",
                _configuration.GetSection("Application:Excel:Config").Value.Replace("\\", "/").TrimEnd('/'),
                configFile.Trim('/'));

            if (!System.IO.File.Exists(configFileJson))
            {
                throw new Exception("Cannot load config, File not found.");
            }

            var fileName = string.Format(DateTime.Now.ToString("yyyyMMddHHmmssfff") + "-" + rand.Next(100, 999) + ".xlsx");
            var builderFile = System.IO.Path.Combine(builderTemp, fileName);

            if (System.IO.File.Exists(builderFile))
            {
                System.IO.File.Delete(fileName);
            }

            ExportConfig config = this.CreateConfig(configFileJson);


            if (string.IsNullOrEmpty(config.TEMPORARY_PATH))
            {
                throw new Exception("Cannot load config, Please check path is not empty.");
            }

            ExportExcelResult result = new ExportExcelResult();
            result.success = false;
            result.FILE_NAME = fileName;
            result.EXT_INFO = System.IO.Path.GetExtension(fileName);
            result.MIME_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var fileInfo = new FileInfo(builderFile);

            var columns = config.FIELD_NAME;
            var alias = config.ALIAS == null ? columns : config.ALIAS;

            if (alias.Count != columns.Count)
            {
                throw new Exception("Cannot load config, Please check alias length equals to fields length");
            }

            //=>ตำแหน่งที่จะบันทึกไฟล์บน Server
            config.TEMPORARY_PATH = _configuration.GetSection(config.TEMPORARY_PATH).Value.Replace("\\", "/");

            try
            {
                action(config);

                using (var package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(
                        config.REPORT_NAME + "_" + DateTime.Now.ToString("dd-MM-yyyy", DateTimeExtension.format_th));

                    var row = 1;
                    var col = 1;

                    foreach (var column in alias)
                    {
                        worksheet.Cells[row, col].Value = column;
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.Font.Size = 12;
                        worksheet.Cells[row, col].AutoFitColumns();
                        col++;
                    }

                    row = 2;

                    foreach (DataRow dataRow in table.Rows)
                    {
                        col = 1;
                        columns.ForEach(colName =>
                        {

                            worksheet.Cells[row, col].Style.Font.Size = 11;
                            worksheet.Cells[row, col].AutoFitColumns();

                            string typeStr = dataRow[colName].GetType().FullName;


                            //=>State: SetFormat
                            switch (typeStr)
                            {
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0";
                                    break;
                                case "System.Double":
                                    worksheet.Cells[row, col].Style.Numberformat.Format = "#,##0.00";
                                    break;
                                case "System.DateTime":
                                    worksheet.Cells[row, col].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                                    break;
                                case "System.DBNull":
                                default:
                                    break;
                            }


                            //=>State: Convert
                            switch (typeStr)
                            {
                                case "System.DateTime":
                                    try
                                    {
                                        //=>ใช้ค่าจากต้นทาง
                                        var dt = dataRow[colName].ToString();
                                        worksheet.Cells[row, col].Value = dt;
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            //=>หากไม่สามารถ ToString ได้จะใช้ค่าเดิมก่อนแปลง
                                            worksheet.Cells[row, col].Value = dataRow[colName];
                                        }
                                        catch
                                        {
                                            //=>หากไม่สำามารถใข้ค่าเดิมก่อนแปลงจะใช้ null
                                            worksheet.Cells[row, col].Value = null;
                                        }
                                    }
                                    break;
                                default:
                                    worksheet.Cells[row, col].Value = dataRow[colName];
                                    break;
                            }

                            overrideValue(worksheet.Cells[row, col], colName, dataRow[colName]);
                            col++;
                        });

                        row++;
                    }

                    package.SaveAs(fileInfo);
                    package.Dispose();
                }

                result.SetRawBytes(System.IO.File.ReadAllBytes(builderFile));


                var fullName = System.IO.Path.Combine(config.TEMPORARY_PATH, fileName);

                if (!System.IO.Directory.Exists(config.TEMPORARY_PATH))
                {
                    System.IO.Directory.CreateDirectory(config.TEMPORARY_PATH);
                }

                if (System.IO.Directory.Exists(config.TEMPORARY_PATH))
                {
                    fileInfo.MoveTo(fullName);
                }

                if (System.IO.File.Exists(builderFile))
                {
                    System.IO.File.Delete(builderFile);
                }

                result.success = true;

                string URL = string.Format("{0}/exports/{1}",
                    PathExtension.BasePath(_env),
                    fileName);
                result.DOWNLOAD = URL;
            }
            catch (Exception e)
            {
                result.SetException(e);

                if (System.IO.File.Exists(builderFile))
                {
                    System.IO.File.Delete(builderFile);
                }
            }

            return result;

        }
        private ExportConfig CreateConfig(string configFileJson)
        {
            var config = JsonConvert.DeserializeObject<ExportConfig>(
                System.IO.File.ReadAllText(configFileJson, System.Text.Encoding.UTF8));

            if (config.ALIAS == null)
            {
                config.ALIAS = config.FIELD_NAME.ToList();
            }

            return config;
        }
    }


    public class ExportConfig
    {
        public string REPORT_NAME { get; set; }

        public string TEMPORARY_PATH { get; set; }

        public List<string> FIELD_NAME { get; set; }

        public List<string> ALIAS { get; set; }

        public List<string> DATA_TYPE { get; set; }
    }


    public class ExportExcelResult
    {
        protected Exception exception { get; set; }

        public bool success { get; set; }

        public string FILE_NAME { get; set; }

        public string EXT_INFO { get; set; }

        public string MIME_TYPE { get; set; }

        public string DOWNLOAD { get; set; }

        protected byte[] RAW_BYTES { get; set; }

        public byte[] GetRawBytes()
        {
            return this.RAW_BYTES;
        }

        public Exception GetException()
        {
            return this.exception;
        }

        public void SetRawBytes(byte[] rawBytes)
        {
            this.RAW_BYTES = rawBytes;
        }

        public void SetException(Exception exception)
        {
            this.exception = exception;
        }
    }
}
