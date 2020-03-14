using MsSqlReader.Interfaces;
using MsSqlReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MsSqlReader.Services
{
    class Reader
    {
        private readonly IStorage Storage;
        private Dictionary<string, ISqlProvider> SqlProviders { get; set; }

        public Reader(IStorage storage)
        {
            Storage = storage;
            SqlProviders = new Dictionary<string, ISqlProvider>();
        }

        public void Start()
        {
            var mode = "";

            while (mode != "3")
            {
                Console.WriteLine($"Введите режим приложения (1 - sql, 2 - connectionManager, 3 - exit) : ");
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

        private void StartConnectionManager()
        {

        }

        private void StartReader()
        {
            var provider = GetProvider();

            var query = "";
            while (!string.Equals(query, "exit", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine("Введите запрос (или 'exit' для смены подключения ) : ");
                query = Console.ReadLine();

                if (string.Equals(query, "exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var response = provider.Execute(query);
                    Console.WriteLine($"Reponse : {response}");
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"Не удалось выполнить запрос : {exc.ToString()}");
                }
            }
        }

        private ISqlProvider GetProvider()
        {
            var existStages = GetStages();

            Console.Write(
                existStages.Any()
                ? $"Введите название stage ({String.Join(',', existStages)}) : "
                : $"Введите название stage : "
                );

            var stageName = Console.ReadLine();

            if (SqlProviders.TryGetValue(stageName ?? "", out var provider))
            {
                return provider;
            }

            var sqlConnection = GetSqlConnection(existStages, stageName);
            var sqlProvider = new SqlProvider(sqlConnection.Host, sqlConnection.UserName, sqlConnection.Password);
            SqlProviders.Add(stageName, sqlProvider);

            if (!existStages.Contains(stageName))
            {
                CreateStageConnection(stageName, sqlConnection);
            }        

            return sqlProvider;
        }

        private void CreateStageConnection(string stageName, SqlConnectionData sqlConnection)
        {
            Storage.Set($"Stages_{stageName}.Host", sqlConnection.Host);
            Storage.Set($"Stages_{stageName}.UserName", sqlConnection.UserName);
            Storage.Set($"Stages_{stageName}.Password", sqlConnection.Password);

            var existStages = GetStages();
            existStages.Add(stageName);
            SetStages(existStages);
        }

        private SqlConnectionData GetStageConnection(string stageName)
        {
            return new SqlConnectionData
            {
                Host = Storage.Get($"Stages_{stageName}.Host"),
                UserName = Storage.Get($"Stages_{stageName}.UserName"),
                Password = Storage.Get($"Stages_{stageName}.Password"),
            };
        }

        private SqlConnectionData GetSqlConnection(List<string> existStages, string stageName)
        {
            if (existStages.Contains(stageName))
            {
                return GetStageConnection(stageName);
            }
            
            Console.WriteLine($"Введите данные для '{stageName}' : ");
            Console.Write($"Host (127.0.0.1) : ");
            var host = Console.ReadLine();
            Console.Write($"UserName : ");
            var userName = Console.ReadLine();
            Console.Write($"Password : ");
            var password = Console.ReadLine();            

            return new SqlConnectionData
            {
                Host = host,
                UserName = userName,
                Password = password,
            };
        }

        private List<string> GetStages()
        {
            var existStagesStr = Storage.Get("Stages");
            var stages = (existStagesStr ?? "").Split('&');

            return stages.Where(x => !string.IsNullOrEmpty(x)).ToList();
        }

        private void SetStages(List<string> stages)
        {
            var stagesStr = String.Join('&', stages);
            Storage.Set("Stages", stagesStr);
        }
    }
}
