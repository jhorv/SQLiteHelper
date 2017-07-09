using System.Collections.ObjectModel;

namespace SQLiteHelper
{
    public class SQLiteColumnList : KeyedCollection<string, SQLiteColumn>
    {
        protected override string GetKeyForItem(SQLiteColumn item)
        {
            return item.Name;
        }
    }
}
