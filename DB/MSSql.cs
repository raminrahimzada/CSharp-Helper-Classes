using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace raminrahimzada
{
    public class MSSql
    {
        private static MSSql _msSql;

        public static MSSql Instance
        {
            get { return _msSql ?? (_msSql = new MSSql()); }
        }
        private static string _connectionString;

        public void Setup(string connection)
        {
            _connectionString = connection;
        }

        public DataTable Execute(string sql)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var adapter = new SqlDataAdapter(sql, sqlConnection))
                {
                    var dt = new DataTable("dt");
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
        public DataTable Execute(string sql, params object[] args)
        {
            return Execute(string.Format(sql, args));
        }

        public void ExecuteNonQuery(string sql,params object[] args)
        {
            sql = string.Format(sql, args);
            ExecuteNonQuery(sql);
        }
        public void ExecuteNonQuery(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                return;
            }
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var adapter = new SqlDataAdapter(sql, sqlConnection))
                {
                    var dt = new DataTable("dt");
                    adapter.Fill(dt);
                }
            }
        }
        public DataTable[] ExecuteMore(string query )
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                using (var adapter = new SqlDataAdapter(query, sqlConnection))
                {
                    var ds = new DataSet("ds");
                    adapter.Fill(ds);
                    return ds.Tables.Cast<DataTable>().ToArray();
                }
            }
        }
        public DataTable[] ExecuteMore(string query, params object[] args)
        {
            return ExecuteMore(string.Format(query, args));
        }
    }
    public static class SqlExtensions
    {
        public static string SafeSql(this string s)
        {
            return s.Replace("'", "''");
        }
    }
}