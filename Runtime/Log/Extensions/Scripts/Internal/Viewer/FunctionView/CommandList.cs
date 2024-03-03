using UnityEngine;

namespace F8Framework.Core
{
    public class CommandList : MonoBehaviour
    {
        private InfiniteScroll infiniteScroll = null;

        private void Awake()
        {
            infiniteScroll = GetComponent<InfiniteScroll>();
        }

        public void Insert(Function.CommandData data)
        {
            infiniteScroll.InsertData(new CommandItemData() { commandData = data });
        }
    }
}