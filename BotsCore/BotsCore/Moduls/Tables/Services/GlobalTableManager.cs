using BotsCore.Moduls.Tables.Interface;
using System.Collections.Generic;

namespace BotsCore.Moduls.Tables.Services
{
    public static class GlobalTableManager
    {
        private static readonly Dictionary<string, Dictionary<string, ITable>> globalTables = new();

        /// <summary>
        /// Загрузить таблицы сервиса
        /// </summary>
        public static void AddTables(string serviceName, params ITable[] tables)
        {
            if (!globalTables.ContainsKey(serviceName))
                globalTables.Add(serviceName, new Dictionary<string, ITable>());
            foreach (var table in tables)
                globalTables[serviceName].Add(table.GetNameTable(), table);
        }
        public static ITable GetTable(string serviceName, string tableName) => globalTables[serviceName][tableName];
    }
}
