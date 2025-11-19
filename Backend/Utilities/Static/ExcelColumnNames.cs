namespace Utilities.Static
{
    public class ExcelColumnNames
    {
        public static List<TableColumn> GetColumns(IEnumerable<(string ColumnName, string PropertyName)> columnsProperties)
        {
            var columns = new List<TableColumn>();

            foreach (var (ColumnName, PropertyName) in columnsProperties)
            {
                var column = new TableColumn()
                {
                    Label = ColumnName,
                    PropertyName = PropertyName
                };

                columns.Add(column);
            }

            return columns;
        }

        #region ColumnsBrand
        public static List<(string ColumnName, string PropertyName)> GetColumnsBrands()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MARCA", "BrandName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusBrand")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsCategory
        public static List<(string ColumnName, string PropertyName)> GetColumnsCategories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CATEGORÍA", "CategoryName"),
                ("DESCRIPCIÓN", "Description"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusCategory")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsModule
        public static List<(string ColumnName, string PropertyName)> GetColumnsModules()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MÓDULO", "ModuleName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusModule")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsProduct
        public static List<(string ColumnName, string PropertyName)> GetColumnsProducts()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CÓDIGO", "Code"),
                ("DESCRIPCIÓN", "Description"),
                ("MATERIAL", "Material"),
                ("COLOR", "Color"),
                ("UNIDAD DE MEDIDA", "UnitMeasure"),
                ("CATEGORÍA", "CategoryName"),
                ("MARCA", "BrandName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusProduct")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsRole
        public static List<(string ColumnName, string PropertyName)> GetColumnsRoles()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("ROL", "RoleName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusRole")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsStore
        public static List<(string ColumnName, string PropertyName)> GetColumnsStores()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("TIENDA", "StoreName"),
                ("ENCARGADO", "Manager"),
                ("DIRECCIÓN", "Address"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusStore")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsUser
        public static List<(string ColumnName, string PropertyName)> GetColumnsUsers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("USUARIO", "UserName"),
                ("NOMBRES", "Names"),
                ("APELLIDOS", "LastNames"),
                ("TELÉFONO", "PhoneNumber"),
                ("ROL", "RoleName"),
                ("TIENDA", "StoreName"),
                ("FECHA DE CREACIÓN", "AuditCreateDate"),
                ("ESTADO", "StatusUser")
            };

            return columnsProperties;
        }
        #endregion
    }
}
