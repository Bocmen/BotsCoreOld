using BotsCore.Moduls.Translate;

namespace BotsCore.Moduls.Tables.Interface
{
    public interface ITableText : ITable
    {
        /// <summary>
        /// Получения текста по его Id
        /// </summary>
        public Text GetDataTextId(uint id);
        public (uint id, Text value)[] GetAllData();
    }
}