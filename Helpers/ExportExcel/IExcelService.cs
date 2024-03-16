using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Taipla.Webservice.Helpers.ExportExcel
{
    public interface IExcelService
    {
        ExportConfig Config(string configFile);

        ExportExcelResult Export(string configFile
            , Action<ExportConfig> action
            , DataTable table
            , Action<ExcelRange, string, object> overrideValue);
    }
}
