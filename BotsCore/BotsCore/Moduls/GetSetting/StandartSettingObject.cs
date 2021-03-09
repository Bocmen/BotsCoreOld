using BotsCore.Moduls.GetSetting.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BotsCore.Moduls.GetSetting
{
    public class StandartSettingObject : IObjectSetting
    {
        public Dictionary<string, string> Data { get; protected set; } = new Dictionary<string, string>();
        public StandartSettingObject() { }
        public StandartSettingObject(Dictionary<string, string> setData) => Data = setData;
        public void SetDataObjectSetting<T>(object e)
        {
            foreach (var elemData in Data)
            {
                try
                {
                    var Field = e.GetType().GetField(elemData.Key);
                    Type typeField = Field.FieldType;
                    if (typeField == typeof(int))
                        Field.SetValue(e, int.Parse(elemData.Value));
                    else if (typeField == typeof(uint))
                        Field.SetValue(e, uint.Parse(elemData.Value));
                    else if (typeField == typeof(long))
                        Field.SetValue(e, long.Parse(elemData.Value));
                    else if (typeField == typeof(ulong))
                        Field.SetValue(e, ulong.Parse(elemData.Value));
                    else if (typeField == typeof(float))
                        Field.SetValue(e, float.Parse(elemData.Value));
                    else if (typeField == typeof(double))
                        Field.SetValue(e, double.Parse(elemData.Value));
                    else if (typeField == typeof(byte))
                        Field.SetValue(e, byte.Parse(elemData.Value));
                    else if (typeField == typeof(string))
                        Field.SetValue(e, elemData.Value);
                    else if (typeField == typeof(char) && elemData.Value != null && elemData.Value.Length == 1)
                        Field.SetValue(e, elemData.Value[0]);
                    else
                    {
                        var data = JsonConvert.DeserializeObject<T>('{' + $"\"{elemData.Key}\":{elemData.Value}" + '}');
                        Field.SetValue(e, data.GetType().GetField(elemData.Key).GetValue(data));
                    }
                }
                catch { }
            }
        }

        public string GetValue(string key) => Data[key];
    }
}