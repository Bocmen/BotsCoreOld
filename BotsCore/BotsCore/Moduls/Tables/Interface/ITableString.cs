using BotsCore.Moduls.Tables.Interface;

namespace BotsCore.Moduls.Tables.Interface
{
    public interface ITableString : ITable
    {
        /// <summary>
        /// Получения текста по его Id
        /// </summary>
        public string GetDataTextId(uint id);
        public (uint id, string value)[] GetAllData();
    }
}
