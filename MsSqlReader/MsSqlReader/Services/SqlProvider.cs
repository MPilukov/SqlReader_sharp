using MsSqlReader.Interfaces;
using System;
using System.Data.SqlClient;

namespace MsSqlReader.Services
{
    class SqlProvider : ISqlProvider
    {
        private readonly string _host;
        private readonly string _userName;
        private readonly string _password;

        public SqlProvider(string host, string userName, string password)
        {
            _host = host;
            _userName = userName;
            _password = password;
        }
        public string Execute(string sql)
        {
            var connectionString = $"server=tcp:{_host};Integrated Security=false; database=BRK_MSCRM; User ID={_userName};Password={_password};";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Connection.Open();
                var response = command.ExecuteReader();
                return response.ToString();
            }
        }
    }
}
