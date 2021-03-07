namespace BotsCore.Moduls.GetSetting.Interface
{
    public interface IObjectSetting
    {
        void SetDataObjectSetting<T>(object e);
        string GetValue(string key);
    }
}
