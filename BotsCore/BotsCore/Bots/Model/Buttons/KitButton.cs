using BotsCore.Bots.Model.Buttons.Command;
using System.Collections.Generic;
using System.Linq;

namespace BotsCore.Bots.Model.Buttons
{
    public class KitButton
    {
        private readonly CommandList commandList;
        private readonly Button[][] buttons;

        public KitButton(Button[][] buttons)
        {
            this.buttons = buttons;
            List<ObjectCommand> commands = new();
            foreach (var item in buttons)
                commands.AddRange(item.Select(x => x.ObjectCommand));
            commandList = new CommandList(commands.ToArray());
        }
        public Button[][] GetButtons(params (byte x, byte y)?[] diective)
        {
            diective = diective?.Where(x => x != null).ToArray();
            if (diective != null)
            {
                List<Button[]> resul = new();
                for (int y = 0; y < buttons.Length; y++)
                {
                    List<Button> resulStr = new();
                    for (int x = 0; x < buttons[y].Length; x++)
                    {
                        if (!diective.Where(N => N.Value.x == x && N.Value.y == y).Any())
                        {
                            resulStr.Add(buttons[y][x]);
                        }
                    }
                    if (resulStr.Count > 0)
                        resul.Add(resulStr.ToArray());
                }
                return resul.ToArray();
            }
            else
                return buttons;
        }
        public static implicit operator Button[][](KitButton kitButton) => kitButton?.buttons;
        public bool CommandInvoke(ObjectDataMessageInBot inBot, object dataEvent = null) => commandList.CommandInvoke(inBot, dataEvent);
    }
}