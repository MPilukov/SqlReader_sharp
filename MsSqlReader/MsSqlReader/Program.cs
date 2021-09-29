using MsSqlReader.Services;
using System;
using System.IO;

namespace MsSqlReader
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("MS SQL reader started");

            var currentDirectory = Directory.GetCurrentDirectory();
            var configManager = new ConfigManager(currentDirectory);

            var storageFile = configManager.Get("Storage:FilePath");
            var storageService = new FileStorage(storageFile);

            var reader = new Reader(storageService);
            reader.Start();
        }        
    }
}
