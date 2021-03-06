﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteHelper.Tests
{
    static class Extensions
    {
        /// <summary>
        /// Executes a non-query SQL command.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="sql"></param>
        /// <returns>The number of rows affected.</returns>
        public static int ExecuteNonQuery(this SQLiteConnection @this, string sql)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected;
            }
        }
    }
}
