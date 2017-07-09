using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SQLiteHelper.Tests
{
    public class SQLiteHelperTests
    {

        [Fact]
        public void SimpleCreateTableTest()
        {
            UsingSystemUnderTest((connection, helper) =>
            {
                var tableSpec = TestObjects.NewPeopleTableSpec();
                helper.CreateTable(tableSpec);
                bool exists = SQLitePrimitives.TableExists(connection, tableSpec.Name);
                Assert.True(exists);
            });
        }

        public void UsingSystemUnderTest(params Action<SQLiteConnection, SQLiteHelper>[] tests)
        {
            using (var db = TestObjects.OpenNewDatabase())
            using (var command = db.CreateCommand())
            {
                var sut = new SQLiteHelper(command);
                foreach (var test in tests)
                {
                    test(db, sut);
                }
            }
        }
    }
}
