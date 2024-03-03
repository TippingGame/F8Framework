using System.Collections.Generic;

namespace F8Framework.Core
{
    public class FunctionLogView : LogViewBase
    {
        public CommandList commandList = null;

        private int showCommandCount = 0;

        public void InputCheatKey(string cheatKey)
        {
            Function.Instance.InvokeCheatKey(cheatKey);
        }

        private void Update()
        {
            List<Function.CommandData> commands = Function.Instance.GetCommandDatas();

            if (showCommandCount < commands.Count)
            {
                for (int index = showCommandCount; index < commands.Count; ++index)
                {
                    commandList.Insert(commands[index]);
                    ++showCommandCount;
                }
            }
        }
    }
}