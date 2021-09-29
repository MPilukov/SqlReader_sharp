using MsSqlReader.Interfaces;
using MsSqlReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsSqlReader.Services
{
    internal class Reader
    {
        private readonly IStorage _storage;
        private Dictionary<string, ISqlProvider> SqlProviders { get; set; }

        public Reader(IStorage storage)
        {
            _storage = storage;
            SqlProviders = new Dictionary<string, ISqlProvider>();
        }

        public void Start()
        {
            var mode = "";

            while (mode != "3")
            {
                Console.WriteLine($"Select mode (1 - sql, 2 - connectionManager, 3 - exit) : ");
                mode = Console.ReadLine();

                if (mode == "1")
                {
                    StartReader();
                }
                else if (mode == "2")
                {
                    StartConnectionManager();
                }
            }
        }

        private static void StartConnectionManager()
        {
            throw new NotImplementedException();
        }

        private void StartReader()
        {
            var provider = GetProvider();

            var query = "";
            while (!string.Equals(query, "exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Write query (or 'exit' for change connection) : ");
                query = Console.ReadLine();

                if (string.Equals(query, "exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                try
                {
                    provider.Execute(query, Console.WriteLine);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Failed to execute the query : {exc}");
                }
            }
        }

        private ISqlProvider GetProvider()
        {
            var existStages = GetStages();

            Console.Write(
                existStages.Any()
                ? $"Write name stage ({string.Join(',', existStages)}) : "
                : $"Write name stage : "
                );

            var stageName = Console.ReadLine();

            if (SqlProviders.TryGetValue(stageName ?? "", out var provider))
            {
                return provider;
            }

            var sqlConnection = GetSqlConnection(existStages, stageName);
            var sqlProvider = new SqlProvider(sqlConnection.Host, sqlConnection.UserName, sqlConnection.Password, sqlConnection.Database);
            SqlProviders.Add(stageName, sqlProvider);

            if (!existStages.Contains(stageName))
            {
                CreateStageConnection(stageName, sqlConnection);
            }        

            return sqlProvider;
        }

        private void CreateStageConnection(string stageName, SqlConnectionData sqlConnection)
        {
            _storage.Set($"Stages_{stageName}.Host", sqlConnection.Host);
            _storage.Set($"Stages_{stageName}.UserName", sqlConnection.UserName);
            _storage.Set($"Stages_{stageName}.Password", sqlConnection.Password);
            _storage.Set($"Stages_{stageName}.Database", sqlConnection.Database);

            var existStages = GetStages();
            existStages.Add(stageName);
            SetStages(existStages);
        }

        private SqlConnectionData GetStageConnection(string stageName)
        {
            return new SqlConnectionData
            {
                Host = _storage.Get($"Stages_{stageName}.Host"),
                UserName = _storage.Get($"Stages_{stageName}.UserName"),
                Password = _storage.Get($"Stages_{stageName}.Password"),
                Database = _storage.Get($"Stages_{stageName}.Database"),
            };
        }

        private SqlConnectionData GetSqlConnection(ICollection<string> existStages, string stageName)
        {
            if (existStages.Contains(stageName))
            {
                return GetStageConnection(stageName);
            }
            
            Console.WriteLine($"Write info for '{stageName}' : ");
            Console.Write($"Host (127.0.0.1) : ");
            var host = Console.ReadLine();
            Console.Write($"UserName : ");
            var userName = Console.ReadLine();
            Console.Write($"Password : ");
            var password = Console.ReadLine();
            Console.Write($"Database (can be empty): ");
            var database = Console.ReadLine();

            return new SqlConnectionData
            {
                Host = host,
                UserName = userName,
                Password = password,
                Database = database,
            };
        }

        private List<string> GetStages()
        {
            var existStagesStr = _storage.Get("Stages");
            var stages = (existStagesStr ?? "").Split('&');

            return stages.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        private void SetStages(IEnumerable<string> stages)
        {
            var stagesStr = string.Join('&', stages);
            _storage.Set("Stages", stagesStr);
        }
    }
}
