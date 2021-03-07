using BotsCore.Moduls;
using System;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons.Command
{
    public class CommandList
    {
        private readonly ObjectCommand[] commands;

        public CommandList(params ObjectCommand[] commands) => this.commands = commands;

        public bool CommandInvoke(ObjectDataMessageInBot inBotData, object dataEvent = null)
        {
            try
            {
                var resulSearh = commands.FirstOrDefault(x => x.IsSame(inBotData));
                if (resulSearh != default)
                {
                    resulSearh.invokeMethod?.Invoke(inBotData, dataEvent);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                EchoLog.Print($"При обработке команды произошел сбой Инфо: [{e.Message}]", EchoLog.PrivilegeLog.Error);
                return false;
            }
        }
    }
}