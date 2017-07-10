using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SQLiteHelper.Tests
{
    public class SQLitePrimitivesTests
    {
        [Theory]
        [InlineData("MyTable")]
        public void TestCreateEmptyTableWithValidName(string tableName)
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                SQLitePrimitives.CreateEmptyTable(db, tableName);
            }

        }

        [Theory]
        [InlineData("MyTable")]
        public void TestCreateEmptyTableWithValidNameAndVerifyTableExists(string tableName)
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                SQLitePrimitives.CreateEmptyTable(db, tableName);
                var exists = db.TableExists(tableName);
                Assert.True(exists);
            }
        }

        [Fact]
        public void TestCreateTableFailureByException()
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                Assert.Throws<SQLiteException>(() =>
                {
                    SQLitePrimitives.CreateEmptyTable(db, Constants.SQLiteMasterTableName);
                });
            }
        }

        [Theory]
        [InlineData("MyTable")]
        [InlineData("MyTable!")]
        [InlineData("MyTable?")]
        [InlineData("My`Table?")]
        [InlineData("My``Table?")]
        [InlineData("My```Table?")]
        [InlineData("My\"Table?")]
        [InlineData("My\"\"Table?")]
        [InlineData("My\"\"\"Table?")]
        [InlineData("My'Table?")]
        [InlineData("My''Table?")]
        [InlineData("My'''Table?")]
        public void TestTableExistsIsTrue(string tableName)
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                SQLitePrimitives.CreateEmptyTable(db, tableName);
                var exists = db.TableExists(tableName);
                Assert.True(exists);
            }
        }

        [Theory]
        [InlineData("NotCreatedYet!")]
        [InlineData("MeowMeowMeowMeow")]
        public void TestTableExistsIsFalse(string tableName)
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                var exists = db.TableExists(tableName);
                Assert.False(exists);
            }
        }

        [Fact]
        public void TestMasterTableDoesNotListItself()
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                var exists = db.TableExists(Constants.SQLiteMasterTableName);
                Assert.False(exists);
            }
        }

    }
}
