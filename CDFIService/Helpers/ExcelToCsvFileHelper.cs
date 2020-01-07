using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;

namespace CDFIService.Helpers
{
    public static class ExcelToCsvFileHelper
    {
        public static string CreateCsvFromExcelFile(string fullExcelFileName, int headerRowIndex)
        {
            var excelWorkbook = ReadExcelFile(fullExcelFileName, headerRowIndex);
            var excelSheet = excelWorkbook.Tables[0];

            var fullCsvFileName = BuildFullCsvFileName(fullExcelFileName);
            var csvContent = ConvertToCsvFormat(excelSheet);
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

        private static DataSet ReadExcelFile(string fullExcelFileName, int headerRowIndex)
        {
            var columnHeaders = new HashSet<string>();

            var dataTableConfiguration = new ExcelDataTableConfiguration()
            {
                UseHeaderRow = true,
                ReadHeaderRow = (rowReader) =>
                {
                    while (rowReader.Depth < headerRowIndex - 1)
                    {
                        rowReader.Read();
                    }
                },
                FilterColumn = (rowReader, columnIndex) =>
                {
                    var header = rowReader.GetString(columnIndex);
                    if (!columnHeaders.Contains(header))
                    {
                        columnHeaders.Add(header);
                        return true;
                    }
                    return false;
                },
                FilterRow = (rowReader) =>
                {
                    var isRowAfterHeaderRow = rowReader.Depth > headerRowIndex - 2;
                    var isFirstColumnPopulated = !string.IsNullOrWhiteSpace(rowReader.GetString(0));

                    return isRowAfterHeaderRow && isFirstColumnPopulated;
                },
            };

            var dataSetConfiguration = new ExcelDataSetConfiguration()
            {
                UseColumnDataType = true,
                ConfigureDataTable = (tableReader) => dataTableConfiguration,
            };

            using (var stream = new FileStream(fullExcelFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                return reader.AsDataSet(dataSetConfiguration);
            }
        }

        private static string ConvertToCsvFormat(DataTable tabularData)
        {
            var csvFormattedContentBuilder = new StringBuilder();

            var columnNames = tabularData.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName);

            csvFormattedContentBuilder.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in tabularData.Rows)
            {
                var fields = row.ItemArray
                    .Select(field => field.ToString().Contains(',') ? $"\"{field.ToString()}\"" : field.ToString());

                csvFormattedContentBuilder.AppendLine(string.Join(",", fields));
            }

            return csvFormattedContentBuilder.ToString();
        }

        private static void SaveFileWithContent(string fullCsvFileName, string csvFormattedContent)
        {
            File.WriteAllText(fullCsvFileName, csvFormattedContent);
        }
    }
}
