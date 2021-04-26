using BotsCore.Moduls.Tables.Interface;
using System.Collections.Generic;
using System.Linq;

namespace BotsCore.Moduls.Tables.Services
{
    public static class GlobalTableManager
    {
        private static readonly Dictionary<string, Dictionary<string, object>> globalTables = new Dictionary<string, Dictionary<string, object>>();

        /// <summary>
        /// Загрузить таблицы сервиса
        /// </summary>
        public static void AddTables(string serviceName, params object[] tables)
        {
            tables = tables.Where(x => x is ITable).ToArray();
            if (!globalTables.ContainsKey(serviceName))
                globalTables.Add(serviceName, new Dictionary<string, object>());
            foreach (var table in tables)
                globalTables[serviceName].Add(((ITable)table).GetNameTable(), table);
        }
        public static object GetTableObj(string serviceName, string tableName) => globalTables[serviceName][tableName];
        public static ITable GetTable(string serviceName, string tableName) => (ITable)GetTableObj(serviceName, tableName);
    }
}
