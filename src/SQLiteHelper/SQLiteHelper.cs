using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace SQLiteHelper
{
    public class SQLiteHelper
    {
        public static readonly string SQLiteMasterTableName = "sqlite_master";

        readonly SQLiteCommand _cmd;

        public SQLiteHelper(SQLiteCommand command)
        {
            _cmd = command;
        }

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

        #region DB Info

        public DataTable GetTableStatus()
        {
            return Select($"SELECT * FROM {SQLiteMasterTableName};");
        }

        public DataTable GetTableList()
        {
            DataTable dt = GetTableStatus();
            DataTable dt2 = new DataTable();
            dt2.Columns.Add("Tables");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string t = dt.Rows[i]["name"] + "";
                if (t != "sqlite_sequence")
                    dt2.Rows.Add(t);
            }
            return dt2;
        }

        public DataTable GetColumnStatus(string tableName)
        {
            return Select(string.Format("PRAGMA table_info(`{0}`);", tableName));
        }

        public DataTable ShowDatabase()
        {
            return Select("PRAGMA database_list;");
        }

        #endregion

        #region Query

        public void BeginTransaction()
        {
            _cmd.CommandText = "begin transaction;";
            _cmd.ExecuteNonQuery();
        }

        public void Commit()
        {
            _cmd.CommandText = "commit;";
            _cmd.ExecuteNonQuery();
        }

        public void Rollback()
        {
            _cmd.CommandText = "rollback";
            _cmd.ExecuteNonQuery();
        }

        public DataTable Select(string sql)
        {
            return Select(sql, new List<SQLiteParameter>());
        }

        public DataTable Select(string sql, Dictionary<string, object> dicParameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(dicParameters);
            return Select(sql, lst);
        }

        public DataTable Select(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            _cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    _cmd.Parameters.Add(p);
                }
            }
            var dataAdapter = new SQLiteDataAdapter(_cmd);
            var result = new DataTable();
            dataAdapter.Fill(result);
            return result;
        }

        public void Execute(string sql)
        {
            Execute(sql, default(List<SQLiteParameter>));
        }

        public void Execute(string sql, Dictionary<string, object> parameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(parameters);
            Execute(sql, lst);
        }

        public void Execute(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            _cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    _cmd.Parameters.Add(p);
                }
            }
            _cmd.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            _cmd.CommandText = sql;
            return _cmd.ExecuteScalar();
        }

        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            List<SQLiteParameter> lst = GetParametersList(parameters);
            return ExecuteScalar(sql, lst);
        }

        public object ExecuteScalar(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            _cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    _cmd.Parameters.Add(parameter);
                }
            }
            return _cmd.ExecuteScalar();
        }

        public T ExecuteScalar<T>(string sql, Dictionary<string, object> parameters = null)
        {
            List<SQLiteParameter> lst = null;
            if (parameters != null)
            {
                lst = new List<SQLiteParameter>();
                foreach (KeyValuePair<string, object> kv in parameters)
                {
                    lst.Add(new SQLiteParameter(kv.Key, kv.Value));
                }
            }
            return ExecuteScalar<T>(sql, lst);
        }

        public T ExecuteScalar<T>(string sql, IEnumerable<SQLiteParameter> parameters = null)
        {
            _cmd.CommandText = sql;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    _cmd.Parameters.Add(parameter);
                }
            }
            return (T)Convert.ChangeType(_cmd.ExecuteScalar(), typeof(T));
        }

        public T ExecuteScalar<T>(string sql)
        {
            _cmd.CommandText = sql;
            return (T)Convert.ChangeType(_cmd.ExecuteScalar(), typeof(T));
        }

        private List<SQLiteParameter> GetParametersList(Dictionary<string, object> parameters)
        {
            var list = new List<SQLiteParameter>();
            if (parameters != null)
            {
                foreach (var kvp in parameters)
                {
                    list.Add(new SQLiteParameter(kvp.Key, kvp.Value));
                }
            }
            return list;
        }

        public void Insert(string tableName, Dictionary<string, object> dic)
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

            _cmd.CommandText = sbCol.ToString() + sbVal.ToString();

            foreach (var kvp in dic)
            {
                _cmd.Parameters.AddWithValue("@v" + kvp.Key, kvp.Value);
            }

            _cmd.ExecuteNonQuery();
        }

        public void Update(string tableName, Dictionary<string, object> dicData, string colCond, object varCond)
        {
            var dic = new Dictionary<string, object>();
            dic[colCond] = varCond;
            Update(tableName, dicData, dic);
        }

        public void Update(string tableName, Dictionary<string, object> dicData, Dictionary<string, object> dicCond)
        {
            if (dicData.Count == 0)
                throw new Exception("dicData is empty.");

            var sbData = new StringBuilder();

            var _dicTypeSource = new Dictionary<string, object>();

            foreach (var kvp in dicData)
            {
                _dicTypeSource[kvp.Key] = null;
            }

            foreach (var kvp in dicCond)
            {
                if (!_dicTypeSource.ContainsKey(kvp.Key))
                    _dicTypeSource[kvp.Key] = null;
            }

            sbData.Append("update `");
            sbData.Append(tableName);
            sbData.Append("` set ");

            bool firstRecord = true;

            foreach (var kvp in dicData)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                    sbData.Append(",");

                sbData.Append("`");
                sbData.Append(kvp.Key);
                sbData.Append("` = ");

                sbData.Append("@v");
                sbData.Append(kvp.Key);
            }

            sbData.Append(" where ");

            firstRecord = true;

            foreach (var kvp in dicCond)
            {
                if (firstRecord)
                    firstRecord = false;
                else
                {
                    sbData.Append(" and ");
                }

                sbData.Append("`");
                sbData.Append(kvp.Key);
                sbData.Append("` = ");

                sbData.Append("@c");
                sbData.Append(kvp.Key);
            }

            sbData.Append(";");

            _cmd.CommandText = sbData.ToString();

            foreach (var kvp in dicData)
            {
                _cmd.Parameters.AddWithValue("@v" + kvp.Key, kvp.Value);
            }

            foreach (var kvp in dicCond)
            {
                _cmd.Parameters.AddWithValue("@c" + kvp.Key, kvp.Value);
            }

            _cmd.ExecuteNonQuery();
        }

        public long LastInsertRowId()
        {
            return ExecuteScalar<long>("select last_insert_rowid();");
        }

        #endregion

        #region Utilities

        public void CreateTable(SQLiteTable table)
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

            _cmd.CommandText = sb.ToString();
            _cmd.ExecuteNonQuery();
        }

        public void RenameTable(string tableFrom, string tableTo)
        {
            tableFrom = QuoteIdentifier(tableFrom);
            tableTo = QuoteIdentifier(tableTo);
            _cmd.CommandText = $"alter table {tableFrom} rename to {tableTo};";
            _cmd.ExecuteNonQuery();
        }

        public void CopyAllData(string tableFrom, string tableTo)
        {
            tableFrom = QuoteIdentifier(tableFrom);
            tableTo = QuoteIdentifier(tableTo);
            DataTable dt1 = Select(string.Format("select * from `{0}` where 1 = 2;", tableFrom));
            DataTable dt2 = Select(string.Format("select * from `{0}` where 1 = 2;", tableTo));

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

            _cmd.CommandText = sb2.ToString();
            _cmd.ExecuteNonQuery();
        }

        public void DropTable(string table)
        {
            _cmd.CommandText = string.Format("drop table if exists `{0}`", table);
            _cmd.ExecuteNonQuery();
        }

        public void UpdateTableStructure(string targetTable, SQLiteTable newStructure)
        {
            newStructure.Name = targetTable + "_temp";

            CreateTable(newStructure);

            CopyAllData(targetTable, newStructure.Name);

            DropTable(targetTable);

            RenameTable(newStructure.Name, targetTable);
        }

        public void AttachDatabase(string database, string alias)
        {
            Execute(string.Format("attach '{0}' as {1};", database, alias));
        }

        public void DetachDatabase(string alias)
        {
            Execute(string.Format("detach {0};", alias));
        }

        #endregion

    }
}