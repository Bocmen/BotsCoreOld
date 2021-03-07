using BotsCore.Bots.Model.Buttons.Command;
using System.Collections.Generic;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons
{
    public class KitButton
    {
        private CommandList commandList;
        private Button[][] buttons;

        public KitButton(Button[][] buttons)
        {
            this.buttons = buttons;
            List<ObjectCommand> commands = new List<ObjectCommand>();
            foreach (var item in buttons)
                commands.AddRange(item.Select(x=>x.objectCommand));
            commandList = new CommandList(commands.ToArray());
        }

        public Button[][] GetButtons((byte x, byte y)[] diective)
        {
            List<Button[]> resul = new List<Button[]>();
            for (int y = 0; y < buttons.Length; y++)
            {
                List<Button> resulStr = new List<Button>();
                for (int x = 0; x < buttons[y].Length; x++)
                {
                    if (diective.Where(N => N.x == x && N.y == y).Count() == 0)
                    {
                        resulStr.Add(buttons[y][x]);
                    }
                }
                if (resulStr.Count > 0)
                    resul.Add(resulStr.ToArray());
            }
            return resul.ToArray();
        }

        public bool CommandInvoke(ObjectDataMessageInBot inBot, object dataEvent = null) => commandList.CommandInvoke(inBot, dataEvent);
    }
}