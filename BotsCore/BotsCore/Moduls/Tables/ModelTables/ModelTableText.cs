using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;
using BotsCore.Moduls.Translate;
using System.Collections.Generic;
using static BotsCore.Moduls.Translate.Lang;
using System.Linq;

namespace BotsCore.Moduls.Tables.ModelTables
{
    public class ModelTableText : ITableText
    {
        private readonly string name;
        private ITable.UpdateData updateDataEvents;
        private readonly LangTypes[] langsStandart;
        private Dictionary<uint, Text> dataTable;

        public ModelTableText(string name, params LangTypes[] langsStandart)
        {
            this.name = name;
            this.langsStandart = langsStandart;
        }
        public string GetNameTable() => name;
        public Text GetDataTextId(uint id) => dataTable[id];
        public void Update(ModelContainerDataTable[] dataUpdate)
        {
            var LoadData = dataUpdate.Select(x => (x.IdElemTable, Text.LoadJSON(x.Data))).ToList();
            if (langsStandart != null && langsStandart.Length > 0)
            {
                List<Text> textsTranslate = LoadData.Select(x => x.Item2).ToList();
                foreach (var lang in langsStandart)
                    Text.MultiTranslate(lang, textsTranslate);
            }
            Dictionary<uint, Text> newData = new Dictionary<uint, Text>();
            foreach (var elem in LoadData)
                newData.Add(elem.IdElemTable, elem.Item2);
            dataTable = newData;
            if (updateDataEvents != null)
                updateDataEvents.Invoke();
        }
        public void AddEventUpdateData(ITable.UpdateData updateData) => updateDataEvents += updateData;

        public (uint id, Text value)[] GetAllData() => dataTable.Select(x => (x.Key, x.Value)).ToArray();
    }
}