using System;

namespace MsSqlReader.Interfaces
{
    interface ISqlProvider
    {
        public void Execute(string sql, Action<string> trace);
    }
}
