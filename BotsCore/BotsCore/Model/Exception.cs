using BotsCore.Moduls;

namespace BotsCore.Model
{
    public class Exception : System.Exception
    {
        public Exception(string text) => EchoLog.Print(text, EchoLog.PrivilegeLog.Error);
    }
}
