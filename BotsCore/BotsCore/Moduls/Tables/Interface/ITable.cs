using BotsCore.Moduls.Tables.Services.Model;

namespace BotsCore.Moduls.Tables.Interface
{
    /// <summary>
    /// Интерфейс таблиц
    /// </summary>
    public interface ITable
    {
        /// <summary>
        /// Делегат обновления данных
        /// </summary>
        public delegate void UpdateData(object info = null);
        /// <summary>
        /// Получение название таблицы
        /// </summary>
        /// <returns></returns>
        string GetNameTable();
        /// <summary>
        /// Обновления данных таблицы
        /// </summary>
        /// <param name="JsonUpdateText"></param>
        void Update(ModelContainerDataTable[] dataUpdate);
        void AddEventUpdateData(UpdateData updateData);
    }
}