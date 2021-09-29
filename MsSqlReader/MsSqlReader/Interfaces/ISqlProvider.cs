using System;

namespace MsSqlReader.Interfaces
{
    internal interface ISqlProvider
    {
        public void Execute(string sql, Action<string> trace);
    }
}
