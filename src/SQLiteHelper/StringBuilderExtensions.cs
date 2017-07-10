using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteHelper
{
    /// <summary>
    /// Internal extension methods to make SQL string building easier.
    /// </summary>
    static class StringBuilderExtensions
    {
        public static StringBuilder AppendQuotedIdentifier(this StringBuilder @this, string value)
        {
            value = SQLiteString.QuoteIdentifier(value);
            @this.Append(value);
            return @this;
        }

        public static StringBuilder AppendQuotedLiteral(this StringBuilder @this, string value)
        {
            value = SQLiteString.QuoteLiteral(value);
            @this.Append(value);
            return @this;
        }

    }
}
