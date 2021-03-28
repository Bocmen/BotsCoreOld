using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;

namespace BotsCore.Moduls.Tables.Services
{
    /// <summary>
    /// Метка на данные в таблице
    /// </summary>
    /// <typeparam name="T">Тип дынных такой же как в таблице</typeparam>
    public class ModelMarkerUneversalData<T> : ModelMarkerTable
    {
        public ModelMarkerUneversalData(string appName, string TableName, uint id) : base(appName, TableName, id) { }
        public ModelMarkerUneversalData(ITable table, uint id) : base(table, id) { }
        public T GetData() => (T)((ITableUniversal)propTable).GetDataId(id);
        public ModelMarkerUneversalData<T> GetElemNewId(uint id) => propTable != null ? new(propTable, id) : new(appName, tableName, id);
        public static implicit operator T(ModelMarkerUneversalData<T> modelMarker) => modelMarker.GetData();
    }
}