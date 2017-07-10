using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace SQLiteHelper.Tests
{
    public static class SQLitePrimitives
    {
        public static void CreateEmptyTable(SQLiteConnection connection, string name)
        {
            name = SQLiteString.QuoteIdentifier(name);
            string sql = $@"CREATE TABLE {name} (""Id""	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE);";
            int rowsAffected = connection.ExecuteNonQuery(sql);
            if (rowsAffected != 0)
                throw new InvalidOperationException("CreateEmptyTable() had an unexpected result.");
        }

        //public static bool TableExists(SQLiteConnection connection, string tableName)
        //{
        //    tableName = SQLiteString.QuoteIdentifier(tableName);
        //    var sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = {tableName}";
        //    object result = connection.ExecuteScalar(sql);
        //    if (result is long rows)
        //    {
        //        return rows == 1;
        //    }
        //    return false;
        //}

        //public static void UsingCommand(SQLiteConnection connection, string sql, Action<SQLiteCommand> func)
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = sql;
        //        func(command);
        //    }
        //}

        //public static TReturnVal UsingCommand<TReturnVal>(SQLiteConnection connection, string sql, Func<SQLiteCommand, TReturnVal> func)
        //{
        //    using (var command = connection.CreateCommand())
        //    {
        //        command.CommandText = sql;
        //        return func(command);
        //    }
        //}
    }
}
