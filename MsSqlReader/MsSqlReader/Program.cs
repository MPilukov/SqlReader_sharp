using MsSqlReader.Services;
using System;
using System.IO;

namespace MsSqlReader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MS SQL reader started");

            var currentDictionart = Directory.GetCurrentDirectory();
            var configManager = new ConfigManager(currentDictionart);

            var storageFile = configManager.Get("Storage:FilePath");
            var storageService = new FileStorage(storageFile);

            var reader = new Reader(storageService);
            reader.Start();
        }        
    }
}
