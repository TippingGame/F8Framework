
namespace F8Framework.Core
{
    public class UILoader : BaseLoader
    {
        private bool isLoadSuccess = false;
        public override bool LoaderSuccess => isLoadSuccess;
        public string Guid;

        public void UILoadSuccess()
        {
            isLoadSuccess = true;
            base.OnComplete();
        }
    }
}
