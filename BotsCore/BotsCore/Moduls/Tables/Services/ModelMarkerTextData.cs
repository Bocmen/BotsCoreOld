using BotsCore.Bots.Model.Buttons;
using BotsCore.Bots.Model.Buttons.Command.Interface;
using BotsCore.Moduls.Tables.Interface;
using BotsCore.Moduls.Tables.Services.Model;
using BotsCore.Moduls.Translate;

namespace BotsCore.Moduls.Tables.Services
{
    /// <summary>
    /// Метка на текст в таблице
    /// </summary>
    public class ModelMarkerTextData : ModelMarkerTable, IGetCommandText
    {
        public ModelMarkerTextData(string appName, string tableName, uint id) : base(appName, tableName, id) { }
        public ModelMarkerTextData(ITable table, uint id) : base(table, id) { }
        public Text GetText() => ((ITableText)propTable).GetDataTextId(id);
        public ModelMarkerTextData GetElemNewId(uint id) => propTable!= null ? new ModelMarkerTextData(propTable, id) : new ModelMarkerTextData(appName, tableName, id);

        public string GetText(Lang.LangTypes lang) => GetText().GetText(lang);

        public static implicit operator Text(ModelMarkerTextData modelMarker) => modelMarker.GetText();
    }
}