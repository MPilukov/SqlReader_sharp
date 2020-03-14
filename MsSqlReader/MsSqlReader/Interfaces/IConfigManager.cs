namespace MsSqlReader.Interfaces
{
    interface IConfigManager
    {
        string Get(string name);
        string GetConnectionString(string name);
    }
}
