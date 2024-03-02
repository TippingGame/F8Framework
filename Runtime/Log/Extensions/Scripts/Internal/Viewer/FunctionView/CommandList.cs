namespace F8Framework.Core
{
    using UnityEngine;

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
