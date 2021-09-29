using MsSqlReader.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MsSqlReader.Services
{
    internal class FileStorage : IStorage
    {
        private readonly string _filePath;
        private Dictionary<string, string> _cache = null;

        public FileStorage(string filePath)
        {
            _filePath = filePath;
        }

        private Dictionary<string, string> GetData()
        {
            if (_cache != null)
            {
                return _cache;
            }

            var response = new Dictionary<string, string>();

            if (!File.Exists(_filePath))
            {
                return response;
            }            

            var data = File.ReadAllText(_filePath);
            var items = JsonConvert.DeserializeObject<DataStorage>(data);


            foreach (var item in items.Values)
            {
                if (response.ContainsKey(item.Key))
                {
                    throw new FileLoadException($"Duplicate key in file : {_filePath}.");
                }

                response.Add(item.Key, item.Value);
            }

            _cache = response;
            return response;
        }

        private void SetData(Dictionary<string, string> data)
        {
            var dataForSave = new DataStorage
            {
                Values = data.Select(x => new ItemDataStorage
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

            _cache = null;
        }
    }
}
