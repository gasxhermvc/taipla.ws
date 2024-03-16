using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Taipla.Webservice.Extensions
{
    public static class DataTableExtension
    {
        public static DataTable ToDataTable<T>(this List<T> lists
            , List<string> columns) where T : class
        {
            DataTable dt = new DataTable();

            CreateColumn(dt, columns);

            lists.ForEach(f =>
            {
                SetRowToTable(dt, f);
            });

            return dt;
        }
        private static DataTable CreateColumn(DataTable dt, List<string> columns)
        {
            columns.ForEach(col =>
            {
                dt.Columns.Add(col);
            });

            return dt;
        }

        private static void SetRowToTable<T>(DataTable dt, T jsonData) where T : class
        {
            DataRow row = dt.NewRow();

            foreach (DataColumn column in dt.Columns)
            {
                var prop = jsonData.GetType().GetProperty(column.ColumnName);

                try
                {
                    switch (column.DataType.FullName)
                    {
                        case "System.String":
                            var value = prop.GetValue(jsonData)?.ToString();
                            if (value != null)
                            {
                                row[column] = System.Web.HttpUtility.HtmlDecode(value);
                            }
                            break;
                        default:
                            row[column] = prop.GetValue(jsonData);
                            break;
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        row[column] = null;
                    }
                    catch
                    {
                        switch (column.DataType.FullName)
                        {
                            case "System.String":
                                row[column] = string.Empty;
                                break;
                            case "System.DBNull":
                                row[column] = DBNull.Value;
                                break;
                            case "System.Sbyte":
                            case "System.Int16":
                            case "System.Int32":
                            case "System.Int64":
                            case "System.Double":
                            case "System.Float":
                            case "System.Decimal":
                                row[column] = column.DefaultValue;
                                break;
                            default:
                                row[column] = null;
                                break;
                        }
                    }
                }
            }

            dt.Rows.Add(row);
        }
    }
}
