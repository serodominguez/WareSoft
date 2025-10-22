using ClosedXML.Excel;
using Utilities.Static;

namespace Infrastructure.FileExcel
{
    public class GenerateExcel : IGenerateExcel
    {
        public MemoryStream GenerateToExcel<T>(IEnumerable<T> data, List<TableColumn> columns)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Listado");

            for (int i = 0; i < columns.Count; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = columns[i].Label;

                cell.Style.Font.Bold = true;

            }

            var rowIndex = 2;

            foreach (var item in data)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    var propertyValue = typeof(T).GetProperty(columns[i].PropertyName!)?.GetValue(item)?.ToString();
                    worksheet.Cell(rowIndex, i + 1).Value = propertyValue;

                }

                rowIndex++;
            }
            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;
            return stream;
        }
    }
}
