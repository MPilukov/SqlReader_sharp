using Microsoft.Extensions.Configuration;
using MsSqlReader.Interfaces;

namespace MsSqlReader.Services
{
    internal class ConfigManager : IConfigManager
    {
        private readonly IConfigurationRoot _configurationRoot;
        public ConfigManager(string path)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(path)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configurationRoot = builder.Build();
        }

        public string Get(string name)
        {
            return _configurationRoot[name];
        }

        public string GetConnectionString(string name)
        {
            return _configurationRoot.GetConnectionString(name);
        }
    }
}
