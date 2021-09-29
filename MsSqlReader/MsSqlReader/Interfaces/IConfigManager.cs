namespace MsSqlReader.Interfaces
{
    internal interface IConfigManager
    {
        string Get(string name);
        string GetConnectionString(string name);
    }
}
