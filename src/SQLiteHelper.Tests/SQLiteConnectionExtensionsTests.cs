using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Xunit;

namespace SQLiteHelper.Tests
{
    public class SQLiteConnectionExtensionsTests
    {

        [Fact]
        public void SimpleCreateTableTest()
        {
            UsingSystemUnderTest(c =>
            {
                var tableSpec = TestObjects.NewPersonTableSpec();
                c.CreateTable(tableSpec);
                bool exists = c.TableExists(tableSpec.Name);
                Assert.True(exists);
            });
        }

        [Fact]
        public void SimpleUpdateTest()
        {
            UsingSystemUnderTest(c =>
            {
                var tableSpec = TestObjects.NewPersonTableSpec();
                c.CreateTable(tableSpec);
                c.Insert("Person", new Dictionary<string, object>
                {
                    { "FirstName", "Jim" },
                    { "LastName", "Smith" },
                    { "Age", 17 }
                });

                c.UpdateRows("Person", new Dictionary<string, object>
                {
                    { "FirstName", "James" }
                },
                "FirstName", "Jim");
            });
        }

        public void UsingSystemUnderTest(params Action<SQLiteConnection>[] tests)
        {
            using (var db = TestObjects.OpenNewDatabase())
            {
                foreach (var test in tests)
                {
                    test(db);
                }
            }
        }
    }
}
