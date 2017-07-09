namespace SQLiteHelper
{
    public class SQLiteTable
    {
        public SQLiteTable()
            : this(null)
        {
        }

        public SQLiteTable(string name)
        {
            Name = name;
            Columns = new SQLiteColumnList();
        }

        public string Name { get; set; }
        public SQLiteColumnList Columns { get; }
    }
}