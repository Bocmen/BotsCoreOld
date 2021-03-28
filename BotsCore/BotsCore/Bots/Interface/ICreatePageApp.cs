using BotsCore.Bots.Model;
using System;

namespace BotsCore.Bots.Interface
{
    public interface ICreatePageApp
    {
        public string GetNameApp();
        public object GetPage(string name, ObjectDataMessageInBot inBot);
        public Type GetTypePage(string name, ObjectDataMessageInBot inBot);
    }
}