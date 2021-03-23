using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BotsCore.Moduls.Tables.ModelTables
{
    public class ModelTableString : ITableString
    {
        private readonly string name;
        private ITable.UpdateData updateDataEvents;
        private Dictionary<uint, string> dataTable;

        public ModelTableString(string name)
        {
            this.name = name;
        }
        public string GetNameTable() => name;
        public string GetDataTextId(uint id) => dataTable[id];
        public void Update(ModelContainerDataTable[] dataUpdate)
        {
            var newData = new Dictionary<uint, string>();
            foreach (var elem in dataUpdate)
                newData.Add(elem.IdElemTable, System.Text.RegularExpressions.Regex.Unescape(elem.Data));
            dataTable = newData;
            if (updateDataEvents != null)
                updateDataEvents.Invoke();
        }
        public void AddEventUpdateData(ITable.UpdateData updateData) => updateDataEvents += updateData;

        public (uint id, string value)[] GetAllData() => dataTable.Select(x => (x.Key, x.Value)).ToArray();
    }
}
