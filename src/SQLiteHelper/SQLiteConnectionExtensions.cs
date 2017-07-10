using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteHelper
{
    public static class SQLiteConnectionExtensions
    {
        #region Databases / Info

        public static void AttachDatabase(this SQLiteConnection @this, string database, string alias)
        {
            @this.ExecuteNonQuery(string.Format("attach '{0}' as {1};", database, alias));
        }

        public static void DetachDatabase(this SQLiteConnection @this, string alias)
        {
            @this.ExecuteNonQuery(string.Format("detach {0};", alias));
        }

        public static DataTable ShowDatabase(this SQLiteConnection @this)
        {
            return @this.Select("PRAGMA database_list;");
        }

        #endregion

        #region Command Execution

        /// <summary>
        /// Execute the command and return the number of rows inserted/updated affected by it.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="sql"></param>
        /// <returns>The number of rows inserted/updated affected by it.</returns>
        public static int ExecuteNonQuery(this SQLiteConnection @this, string sql)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute the command and return the number of rows inserted/updated affected by it.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>The number of rows inserted/updated affected by it.</returns>
        public static int ExecuteNonQuery(this SQLiteConnection @this, string sql, Dictionary<string, object> parameters)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Execute the command and return the number of rows inserted/updated affected by it.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns>The number of rows inserted/updated affected by it.</returns>
        public static int ExecuteNonQuery(this SQLiteConnection @this, string sql, IEnumerable<SQLiteParameter> parameters)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(this SQLiteConnection @this, string sql)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                return command.ExecuteScalar();
            }
        }

        public static object ExecuteScalar(this SQLiteConnection @this, string sql, Dictionary<string, object> parameters)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return command.ExecuteScalar();
            }
        }

        public static object ExecuteScalar(this SQLiteConnection @this, string sql, IEnumerable<SQLiteParameter> parameters)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddRange(parameters);
                return command.ExecuteScalar();
            }
        }

        public static T ExecuteScalar<T>(this SQLiteConnection @this, string sql)
        {
            object resultObj = @this.ExecuteScalar(sql);
            return (T)Convert.ChangeType(resultObj, typeof(T));
        }

        public static T ExecuteScalar<T>(this SQLiteConnection @this, string sql, Dictionary<string, object> parameters)
        {
            object resultObj = @this.ExecuteScalar(sql, parameters);
            return (T)Convert.ChangeType(resultObj, typeof(T));
        }

        public static T ExecuteScalar<T>(this SQLiteConnection @this, string sql, IEnumerable<SQLiteParameter> parameters)
        {
            object resultObj = @this.ExecuteScalar(sql, parameters);
            return (T)Convert.ChangeType(resultObj, typeof(T));
        }

        #endregion

        #region Inserts / Update

        public static void Insert(this SQLiteConnection @this, string tableName, Dictionary<string, object> dic)
        {
            var sbCol = new StringBuilder();
            var sbVal = new StringBuilder();

            foreach (var kvp in dic)
            {
                if (sbCol.Length == 0)
                {
                    sbCol.Append("insert into ");
                    sbCol.Append(tableName);
                    sbCol.Append("(");
                }
                else
                {
                    sbCol.Append(",");
                }

                sbCol.Append("`");
                sbCol.Append(kvp.Key);
                sbCol.Append("`");

                if (sbVal.Length == 0)
                {
                    sbVal.Append(" values(");
                }
                else
                {
                    sbVal.Append(", ");
                }

                sbVal.Append("@v");
                sbVal.Append(kvp.Key);
            }

            sbCol.Append(") ");
            sbVal.Append(");");

            using (var command = @this.CreateCommand())
            {
                command.CommandText = sbCol.ToString() + sbVal.ToString();

                foreach (var kvp in dic)
                {
                    command.Parameters.AddWithValue("@v" + kvp.Key, kvp.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Extension method for doing a SQL UPDATE.  It couldn't simply be named "Update" because
        /// SQLiteConnection already has an event with that name.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="tableName"></param>
        /// <param name="data"></param>
        /// <param name="conditionColumn"></param>
        /// <param name="conditionValue"></param>
        public static void UpdateRows(this SQLiteConnection @this, string tableName, Dictionary<string, object> data, string conditionColumn, object conditionValue)
        {
            var conditions = new Dictionary<string, object>();
            conditions.Add(conditionColumn, conditionValue);
            @this.UpdateRows(tableName, data, conditions);
        }

        /// <summary>
        /// Extension method for doing a SQL UPDATE.  It couldn't simply be named "Update" because
        /// SQLiteConnection already has an event with that name.
        /// </summary>
        public static void UpdateRows(this SQLiteConnection @this, string tableName, Dictionary<string, object> data, Dictionary<string, object> conditions)
        {
            if (data == null || data.Count == 0)
                throw new ArgumentException("The update data cannot be empty.", nameof(data));

            var sbData = new StringBuilder();
            var dicTypeSource = new Dictionary<string, object>();

            foreach (var kvp in data)
            {
                dicTypeSource[kvp.Key] = null;
            }

            foreach (var kvp in conditions)
            {
                if (!dicTypeSource.ContainsKey(kvp.Key))
                    dicTypeSource[kvp.Key] = null;
            }

            sbData.Append("update ");
            sbData.AppendQuotedIdentifier(tableName);
            sbData.Append(" set ");

            bool firstRecord = true;

            foreach (var kvp in data)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                    sbData.Append(",");

                sbData.AppendQuotedIdentifier(kvp.Key);
                sbData.Append(" = ");

                sbData.Append("@v");
                sbData.Append(kvp.Key);
            }

            sbData.Append(" where ");

            firstRecord = true;

            foreach (var kvp in conditions)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                {
                    sbData.Append(" and ");
                }

                sbData.AppendQuotedIdentifier(kvp.Key);
                sbData.Append(" = ");

                sbData.Append("@c");
                sbData.Append(kvp.Key);
            }

            sbData.Append(";");

            using (var command = @this.CreateCommand())
            {
                command.CommandText = sbData.ToString();

                foreach (var kvp in data)
                {
                    command.Parameters.AddWithValue("@v" + kvp.Key, kvp.Value);
                }

                foreach (var kvp in conditions)
                {
                    command.Parameters.AddWithValue("@c" + kvp.Key, kvp.Value);
                }

                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region Select

        public static DataTable Select(this SQLiteConnection @this, string sql)
        {
            return @this.Select(sql, default(IEnumerable<SQLiteParameter>));
        }

        public static DataTable ExecuteSelect(this SQLiteConnection @this, string sql, Dictionary<string, object> parameters)
        {
            return @this.Select(sql, EnumerateParameterDictionary(parameters));
        }

        public static DataTable Select(this SQLiteConnection @this, string sql, IEnumerable<SQLiteParameter> parameters)
        {
            using (var command = @this.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(parameters);
                var dataAdapter = new SQLiteDataAdapter(command);
                var result = new DataTable();
                dataAdapter.Fill(result);
                return result;
            }
        }

        #endregion

        #region Tables

        public static void CopyAllData(this SQLiteConnection @this, string tableFrom, string tableTo)
        {
            tableFrom = SQLiteString.QuoteIdentifier(tableFrom);
            tableTo = SQLiteString.QuoteIdentifier(tableTo);
            DataTable dt1 = @this.Select(string.Format("select * from {0} where 1 = 2;", tableFrom));
            DataTable dt2 = @this.Select(string.Format("select * from {0} where 1 = 2;", tableTo));

            var dic = new Dictionary<string, bool>();

            foreach (DataColumn dataColumn in dt1.Columns)
            {
                if (dt2.Columns.Contains(dataColumn.ColumnName))
                {
                    if (!dic.ContainsKey(dataColumn.ColumnName))
                    {
                        dic[dataColumn.ColumnName] = true;
                    }
                }
            }

            foreach (DataColumn dataColumn in dt2.Columns)
            {
                if (dt1.Columns.Contains(dataColumn.ColumnName))
                {
                    if (!dic.ContainsKey(dataColumn.ColumnName))
                    {
                        dic[dataColumn.ColumnName] = true;
                    }
                }
            }

            var sb = new StringBuilder();

            foreach (var kvp in dic)
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append("`");
                sb.Append(kvp.Key);
                sb.Append("`");
            }

            var sb2 = new StringBuilder();
            sb2.Append("insert into `");
            sb2.Append(tableTo);
            sb2.Append("`(");
            sb2.Append(sb.ToString());
            sb2.Append(") select ");
            sb2.Append(sb.ToString());
            sb2.Append(" from `");
            sb2.Append(tableFrom);
            sb2.Append("`;");

            @this.ExecuteNonQuery(sb2.ToString());
        }

        public static void CreateTable(this SQLiteConnection @this, SQLiteTable table)
        {
            var sb = new StringBuilder();
            sb.Append("create table if not exists `");
            sb.Append(table.Name);
            sb.AppendLine("`(");

            bool firstRecord = true;

            foreach (var col in table.Columns)
            {
                if (string.IsNullOrWhiteSpace(col.Name))
                {
                    throw new Exception("Column name cannot be blank.");
                }

                if (firstRecord)
                    firstRecord = false;
                else
                    sb.AppendLine(",");

                sb.Append(col.Name);
                sb.Append(" ");

                if (col.AutoIncrement)
                {

                    sb.Append("integer primary key autoincrement");
                    continue;
                }

                switch (col.Type)
                {
                    case ColumnType.Text:
                        sb.Append("text"); break;
                    case ColumnType.Integer:
                        sb.Append("integer"); break;
                    case ColumnType.Decimal:
                        sb.Append("decimal"); break;
                    case ColumnType.DateTime:
                        sb.Append("datetime"); break;
                    case ColumnType.Blob:
                        sb.Append("blob"); break;
                }

                if (col.PrimaryKey)
                    sb.Append(" primary key");
                else if (col.NotNull)
                    sb.Append(" not null");
                else if (!string.IsNullOrEmpty(col.DefaultValue))
                {
                    sb.Append(" default ");

                    if (col.DefaultValue.Contains(" ") || col.Type == ColumnType.Text || col.Type == ColumnType.DateTime)
                    {
                        sb.Append("'");
                        sb.Append(col.DefaultValue);
                        sb.Append("'");
                    }
                    else
                    {
                        sb.Append(col.DefaultValue);
                    }
                }
            }

            sb.AppendLine(");");

            @this.ExecuteNonQuery(sb.ToString());
        }

        public static void DropTable(this SQLiteConnection @this, string tableName)
        {
            tableName = SQLiteString.QuoteIdentifier(tableName);
            @this.ExecuteNonQuery($"drop table if exists {tableName};");
        }

        public static DataTable GetTableStatus(this SQLiteConnection @this)
        {
            return @this.Select($"SELECT * FROM {Constants.SQLiteMasterTableName};");
        }

        public static DataTable GetTableList(this SQLiteConnection @this)
        {
            DataTable dt = @this.GetTableStatus();
            DataTable result = new DataTable();
            result.Columns.Add("Tables");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string t = dt.Rows[i]["name"] + "";
                if (t != "sqlite_sequence")
                    result.Rows.Add(t);
            }
            return result;
        }

        public static DataTable GetColumnStatus(this SQLiteConnection @this, string tableName)
        {
            tableName = SQLiteString.QuoteIdentifier(tableName);
            var sql = $"PRAGMA table_info({tableName});";
            return @this.Select(sql);
        }

        public static void RenameTable(this SQLiteConnection @this, string tableFrom, string tableTo)
        {
            tableFrom = SQLiteString.QuoteIdentifier(tableFrom);
            tableTo = SQLiteString.QuoteIdentifier(tableTo);
            var sql = $"alter table {tableFrom} rename to {tableTo};";
            @this.ExecuteNonQuery(sql);
        }


        public static void UpdateTableStructure(this SQLiteConnection @this, string targetTable, SQLiteTable newStructure)
        {
            newStructure.Name = targetTable + "_temp";
            @this.CreateTable(newStructure);
            @this.CopyAllData(targetTable, newStructure.Name);
            @this.DropTable(targetTable);
            @this.RenameTable(newStructure.Name, targetTable);
        }


        #endregion

        #region Transactions

        public static void BeginTransaction(this SQLiteConnection @this)
        {
            @this.ExecuteNonQuery("begin transaction;");
        }

        public static void Commit(this SQLiteConnection @this)
        {
            @this.ExecuteNonQuery("commit;");
        }

        public static void Rollback(this SQLiteConnection @this)
        {
            @this.ExecuteNonQuery("rollback;");
        }

        #endregion

        public static bool TableExists(this SQLiteConnection @this, string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
                return false;

            tableName = SQLiteString.QuoteIdentifier(tableName);
            var sql = $"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = {tableName}";
            object result = @this.ExecuteScalar(sql);
            if (result is long rows)
            {
                return rows == 1;
            }
            return false;
        }

        #region Non-Public Members

        static IEnumerable<SQLiteParameter> EnumerateParameterDictionary(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                yield break;

            foreach (var kvp in parameters)
                yield return new SQLiteParameter(kvp.Key, kvp.Value);
        }

        #endregion
    }
}
