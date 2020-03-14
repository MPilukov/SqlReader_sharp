namespace MsSqlReader.Interfaces
{
    interface ISqlProvider
    {
        public string Execute(string sql);
    }
}
