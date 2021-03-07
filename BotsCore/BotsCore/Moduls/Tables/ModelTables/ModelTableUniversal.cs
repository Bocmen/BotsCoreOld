using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace BotsCore.Moduls.Tables.ModelTables
{
    public class ModelTableUniversal<T> : ITableUniversal
    {
        private readonly string name;
        private ITable.UpdateData updateDataEvents;

        private Dictionary<uint, T> dataTable;
        public ModelTableUniversal(string name)
        {
            this.name = name;
        }
        public string GetNameTable() => name;
        public void Update(ModelContainerDataTable[] dataUpdate)
        {
            var newData = new Dictionary<uint, T>();
            foreach (var elem in dataUpdate)
                newData.Add(elem.IdElemTable, JsonConvert.DeserializeObject<T>(elem.Data));
            dataTable = newData;
            if (updateDataEvents != null)
                updateDataEvents.Invoke();
        }
        public object GetDataId(uint id) => dataTable[id];

        public void AddEventUpdateData(ITable.UpdateData updateData) => updateDataEvents += updateData;

        public (uint id, object value)[] GetAllData() => dataTable.Select(x => (x.Key, (object)x.Value)).ToArray();
    }
}