using System.Collections.Generic;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class FunctionLogView : LogViewBase
    {
        public CommandList commandList = null;
        public InputField inputCheatKey = null;

        private int showCommandCount = 0;
        
        public override void InitializeView()
        {
            inputCheatKey.onSubmit.AddListener(InputCheatKey);
        }

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