using System.IO;
using System.Text.RegularExpressions;
using BotsCore.Moduls.GetSetting.Interface;

namespace BotsCore.Moduls.GetSetting
{
    public class ObjectSettingCostum : StandartSettingObject, IObjectSetting
    {
        private static Regex removeComment = new Regex(@"(/\*((.|\n)*)\*/)|(#(.*)\n|$)", RegexOptions.Compiled);
        private static Regex searchFiled = new Regex(@"(.*)=>(((;.*)|((.*)))*);", RegexOptions.Compiled);
        private static Regex clear = new Regex(@";\s*", RegexOptions.Compiled);
        public ObjectSettingCostum(string Patch)
        {
            string textFile = clear.Replace(removeComment.Replace(File.ReadAllText(Patch), "\n"), ";\n");
            MatchCollection matches = searchFiled.Matches(textFile);
            foreach (Match match in matches)
                Data.Add(match.Groups[1].Value, match.Groups[2].Value);
        }
    }
}