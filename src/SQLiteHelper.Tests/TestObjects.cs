using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteHelper.Tests
{
    public static class TestObjects
    {

        public static SQLiteConnection OpenNewDatabase()
        {
            string connectionString = $"DataSource=:memory:;Version=3;New=True;";
            var db = new SQLiteConnection(connectionString);
            db.Open();
            return db;
        }

        public static SQLiteTable NewPeopleTableSpec()
        {
            var table = new SQLiteTable("Person");
            table.Columns.Add(SQLiteColumn.CreateIntegerPrimaryKey("Id"));
            table.Columns.Add(SQLiteColumn.CreateRequired("FirstName", ColumnType.Text));
            table.Columns.Add(SQLiteColumn.CreateRequired("LastName", ColumnType.Text));
            table.Columns.Add(SQLiteColumn.CreateRequired("Age", ColumnType.Integer));
            return table;
        }

    }
}
