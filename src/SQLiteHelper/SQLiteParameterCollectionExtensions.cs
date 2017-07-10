using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteHelper
{
    public static class SQLiteParameterCollectionExtensions
    {
        public static void Add(this SQLiteParameterCollection @this, KeyValuePair<string, object> keyValuePair)
        {
            @this.AddWithValue(keyValuePair.Key, keyValuePair.Value);
        }

        public static void AddRange(this SQLiteParameterCollection @this, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (parameters == null)
                return;
            foreach (var kvp in parameters)
            {
                @this.Add(kvp);
            }
        }

        public static void AddRange(this SQLiteParameterCollection @this, IEnumerable<SQLiteParameter> parameters)
        {
            if (parameters == null)
                return;
            foreach (var p in parameters)
            {
                @this.Add(p);
            }
        }
    }
}
