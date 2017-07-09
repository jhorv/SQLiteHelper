namespace SQLiteHelper
{
    public class SQLiteColumn
    {
        public static SQLiteColumn CreateIntegerPrimaryKey(string name)
        {
            return CreatePrimaryKey(name, ColumnType.Integer);
        }

        public static SQLiteColumn CreatePrimaryKey(string name, ColumnType type)
        {
            return new SQLiteColumn
            {
                Name = name,
                Type = type,
                PrimaryKey = true,
                AutoIncrement = type == ColumnType.Integer,
                NotNull = true
            };
        }

        public static SQLiteColumn Create(string name, ColumnType type, bool notNull, string defaultValue)
        {
            return new SQLiteColumn
            {
                Name = name,
                Type = type,
                NotNull = notNull,
                DefaultValue = defaultValue
            };
        }

        public static SQLiteColumn CreateRequired(string name, ColumnType type)
        {
            return new SQLiteColumn
            {
                Name = name,
                Type = type,
                NotNull = true
            };
        }

        public SQLiteColumn() { }

        public string Name { get; set; }
        public ColumnType Type { get; set; }
        public bool PrimaryKey { get; set; }
        public bool AutoIncrement { get; set; }
        public bool NotNull { get; set; }
        public string DefaultValue { get; set; }
    }
}
