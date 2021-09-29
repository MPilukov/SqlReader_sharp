namespace MsSqlReader.Interfaces
{
    internal interface IStorage
    {
        void Set(string key, string value);
        string Get(string key);
    }
}
