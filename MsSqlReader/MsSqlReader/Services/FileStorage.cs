using MsSqlReader.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MsSqlReader.Services
{
    class FileStorage : IStorage
    {
        private readonly string _filePath;
        private Dictionary<string, string> Cache = null;

        class Items
        {
            public List<Item> Values { get; set; }
        }
        class Item
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public FileStorage(string filePath)
        {
            _filePath = filePath;
        }

        private Dictionary<string, string> GetData()
        {
            if (Cache != null)
            {
                return Cache;
            }

            var response = new Dictionary<string, string>();

            if (!File.Exists(_filePath))
            {
                return response;
            }            

            var data = File.ReadAllText(_filePath);
            var items = JsonConvert.DeserializeObject<Items>(data);


            foreach (var item in items.Values)
            {
                if (response.ContainsKey(item.Key))
                {
                    throw new FileLoadException($"Дублирующиеся ключи в файле : {_filePath}.");
                }

                response.Add(item.Key, item.Value);
            }

            Cache = response;
            return response;
        }

        private void SetData(Dictionary<string, string> data)
        {
            var dataForSave = new Items
            {
                Values = data.Select(x => new Item
                {
                    Key = x.Key,
                    Value = x.Value,
                }).ToList(),
            };
            var dataStr = JsonConvert.SerializeObject(dataForSave);
            File.WriteAllText(_filePath, dataStr);
        }

        public string Get(string key)
        {
            var data = GetData();
            return data.TryGetValue(key, out var value) 
                ? value 
                : null;
        }

        public void Set(string key, string value)
        {
            var data = GetData();
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }

            SetData(data);

            Cache = null;
        }
    }
}
