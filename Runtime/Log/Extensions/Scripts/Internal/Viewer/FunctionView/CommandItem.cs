using System.Text;
using UnityEngine.UI;

namespace F8Framework.Core
{
    public class CommandItemData : InfiniteScrollData
    {
        public Function.CommandData commandData = null;
    }

    public class CommandItem : InfiniteScrollItem
    {
        public Text commandName = null;

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            CommandItemData data = (CommandItemData)scrollData;
            Function.CommandData commandData = data.commandData;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}.{1}", commandData.type.Name, commandData.methodName);
            if (commandData.parameters != null)
            {
                sb.Append("(");
                for (int index = 0; index < commandData.parameters.Length; ++index)
                {
                    sb.AppendFormat("{0},", commandData.parameters[index].GetType().Name);
                }

                sb.Replace(",", ")", sb.Length - 1, 1);
            }

            commandName.text = sb.ToString();
        }

        public void InvokeCommand()
        {
            CommandItemData data = (CommandItemData)scrollData;
            Function.CommandData commandData = data.commandData;

            commandData.methodInfo.Invoke(commandData.instance, commandData.parameters);
        }
    }
}