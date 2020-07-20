using LiquidSC.Foundation.RedirectManager.Pipelines.HttpRequest;

namespace LiquidSC.Foundation.RedirectManager.Pipelines.HttpRequest
{
    public class AdvancedRedirect
    {
        public string Source
        {
            get;
            set;
        }
        public string Target
        {
            get;
            set;
        }

        public bool PreserveQueryString
        {
            get;
            set;
        }

        public RedirectType RedirectType
        {
            get;
            set;
        }
    }
}
