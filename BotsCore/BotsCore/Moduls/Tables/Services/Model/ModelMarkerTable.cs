using BotsCore.Moduls.Tables.Interface;

namespace BotsCore.Moduls.Tables.Services.Model
{
    public class ModelMarkerTable
    {
        private ITable table;
        protected string appName;
        protected string tableName;
        protected ITable propTable
        {
            get
            {
                if (table == null)
                {
                    table = GlobalTableManager.GetTable(appName, tableName);
                    appName = null;
                    tableName = null;
                }
                return table;
            }
        }
        protected uint id;
        public ModelMarkerTable(string appName, string TableName, uint id)
        {
            this.appName = appName;
            this.tableName = TableName;
            this.id = id;
        }
        public ModelMarkerTable(ITable table, uint id)
        {
            this.table = table;
            this.id = id;
        }
    }
}