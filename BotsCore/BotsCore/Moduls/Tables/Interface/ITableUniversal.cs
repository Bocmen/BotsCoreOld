namespace BotsCore.Moduls.Tables.Interface
{
    public interface ITableUniversal : ITable
    {
        /// <summary>
        /// Получения данных по Id
        /// </summary>
        public object GetDataId(uint id);
        public (uint id, object value)[] GetAllData();
    }
}
