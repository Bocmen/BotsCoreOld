using BotsCore.Bots.Model.Buttons.Command.Interface;
using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;
using BotsCore.Moduls.Translate;

namespace BotsCore.Moduls.Tables.Services
{
    public class ModelMarkerStringlData : ModelMarkerTable, IGetCommandText
    {
        public ModelMarkerStringlData(string appName, string TableName, uint id) : base(appName, TableName, id) { }
        public ModelMarkerStringlData(ITable table, uint id) : base(table, id) { }
        public string GetText() => ((ITableString)propTable).GetDataTextId(id);
        public ModelMarkerStringlData GetElemNewId(uint id) => propTable != null ? new ModelMarkerStringlData(propTable, id) : new ModelMarkerStringlData(appName, tableName, id);
        public string GetText(Lang.LangTypes lang) => GetText();
        public static implicit operator string(ModelMarkerStringlData modelMarker) => modelMarker.GetText();
        public override string ToString() => GetText();
    }
}