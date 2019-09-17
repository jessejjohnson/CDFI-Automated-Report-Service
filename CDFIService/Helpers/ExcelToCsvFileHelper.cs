using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace CDFIService.Helpers
{
    public static class ExcelToCsvFileHelper
    {
        public static string CreateCsvFromExcelFile(string fullExcelFileName)
        {
            var excelDataSet = ReadExcelFile(fullExcelFileName);

            var excelContent = excelDataSet.Tables[0];

            var csvContent = ConvertToCsvFormat(excelContent);

            var fullCsvFileName = BuildFullCsvFileName(fullExcelFileName);

            SaveFileWithContent(fullCsvFileName, csvContent);

            return fullCsvFileName;
        }

        private static string BuildFullCsvFileName(string fullExcelFileName)
        {
            var dirName = Path.GetDirectoryName(fullExcelFileName);
            var excelFileName = Path.GetFileName(fullExcelFileName);
            var csvFileName = Path.ChangeExtension($"{excelFileName}", "csv");

            return Path.Combine(dirName, csvFileName);
        }

        private static DataSet ReadExcelFile(string fullExcelFileName)
        {
            using (var stream = File.Open(fullExcelFileName, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                }
            }
        }

        private static string ConvertToCsvFormat(DataTable tabularData)
        {
            var csvFormattedContentBuilder = new StringBuilder();

            var columnNames = tabularData.Columns
                .Cast<DataColumn>()
                .Select(column => $"\"{column.ColumnName.ToString().Replace("\"", "\"\"")}\"");

            csvFormattedContentBuilder.Append($"{string.Join(",", columnNames)}{Environment.NewLine}");

            foreach (DataRow row in tabularData.Rows)
            {
                var fields = row.ItemArray
                    .Select(field => $"\"{field.ToString().Replace("\"", "\"\"")}\"");

                csvFormattedContentBuilder.Append($"{string.Join(",", fields)}{Environment.NewLine}");
            }

            return csvFormattedContentBuilder.ToString();
        }

        private static void SaveFileWithContent(string fullCsvFileName, string csvFormattedContent)
        {
            File.WriteAllText(fullCsvFileName, csvFormattedContent);
        }
    }
}
