using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteHelper
{
    public static class SQLiteString
    {
        public static string QuoteIdentifier(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (IsQuotedIdentifier(value))
                return value;
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }

        public static string QuoteLiteral(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (IsQuotedLiteral(value))
                return value;
            value = value.Replace("'", "''");
            return $"'{value}'";
        }

        public static bool IsQuotedIdentifier(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return
                (value.StartsWith("\"") && value.EndsWith("\""))
                || (value.StartsWith("[") && value.EndsWith("]"))
                || (value.StartsWith("`") && value.EndsWith("`"))
                ;
        }

        public static bool IsQuotedLiteral(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.StartsWith("'") && value.EndsWith("'");
        }

    }
}
