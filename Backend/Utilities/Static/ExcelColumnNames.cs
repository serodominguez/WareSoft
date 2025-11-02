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

        #region ColumnsBrands
        public static List<(string ColumnName, string PropertyName)> GetColumnsBrands()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MARCA", "BRAND_NAME"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_BRAND")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsCategories
        public static List<(string ColumnName, string PropertyName)> GetColumnsCategories()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("CATEGORÍA", "CATEGORY_NAME"),
                ("DESCRIPCIÓN", "DESCRIPTION"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_CATEGORY")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsModules
        public static List<(string ColumnName, string PropertyName)> GetColumnsModules()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("MÓDULO", "MODULE_NAME"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_MODULE")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsRoles
        public static List<(string ColumnName, string PropertyName)> GetColumnsRoles()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("ROL", "ROLE_NAME"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_ROLE")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsStores
        public static List<(string ColumnName, string PropertyName)> GetColumnsStores()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("TIENDA", "STORE_NAME"),
                ("ENCARGADO", "MANAGER"),
                ("DIRECCIÓN", "ADDRESS"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_STORE")
            };

            return columnsProperties;
        }
        #endregion

        #region ColumnsUsers
        public static List<(string ColumnName, string PropertyName)> GetColumnsUsers()
        {
            var columnsProperties = new List<(string ColumnName, string PropertyName)>
            {
                ("USUARIO", "USER_NAME"),
                ("NOMBRES", "NAMES"),
                ("APELLIDOS", "LAST_NAMES"),
                ("TELÉFONO", "PHONE_NUMBER"),
                ("ROL", "ROLE_NAME"),
                ("TIENDA", "STORE_NAME"),
                ("FECHA DE CREACIÓN", "AUDIT_CREATE_DATE"),
                ("ESTADO", "STATE_USER")
            };

            return columnsProperties;
        }
        #endregion
    }
}
