using MsSqlReader.Interfaces;
using System;
using System.Data.SqlClient;
using System.Text;

namespace MsSqlReader.Services
{
    internal class SqlProvider : ISqlProvider
    {
        private readonly string _host;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _database;

        public SqlProvider(string host, string userName, string password, string database)
        {
            _host = host;
            _userName = userName;
            _password = password;
            _database = database;
        }
        public void Execute(string sql, Action<string> trace)
        {
            var connectionString = string.IsNullOrEmpty(_database)
                ? $"server=tcp:{_host};Integrated Security=false; User ID={_userName};Password={_password};"
                : $"server=tcp:{_host};Integrated Security=false; database=master; User ID={_userName};Password={_password};";

            using (var connection = new SqlConnection(connectionString))
            {
                var command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var reader = command.ExecuteReader();

                var count = reader.VisibleFieldCount;

                var columns = new StringBuilder();
                columns.Append("Columns : ");
                for (var i = 0; i < count; i++)
                {
                    columns.Append(reader.GetName(i));

                    if (i < count - 1)
                    {
                        columns.Append(", ");
                    }
                }

                trace(columns.ToString());

                while (reader.HasRows)
                {
                    var counter = 0;
                    while (reader.Read())
                    {
                        var data = new StringBuilder();
                        data.Append(counter);
                        data.Append(" : {");

                        for (var i = 0; i < count; i++)
                        {
                            data.Append("'");
                            data.Append(reader.GetValue(i));
                            data.Append("'");

                            if (i < count - 1)
                            {
                                data.Append(", ");
                            }
                        }

                        data.Append("}");

                        trace(data.ToString());

                        counter++;
                    }

                    reader.NextResult();
                }
            }
        }
    }
}
