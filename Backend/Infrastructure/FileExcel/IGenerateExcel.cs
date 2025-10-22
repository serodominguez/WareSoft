using Utilities.Static;

namespace Infrastructure.FileExcel
{
    public interface IGenerateExcel
    {
        MemoryStream GenerateToExcel<T>(IEnumerable<T> data, List<TableColumn> columns);
    }
}
