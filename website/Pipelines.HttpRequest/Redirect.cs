using Sitecore.Data.Items;

namespace LiquidSC.Foundation.RedirectManager.Pipelines.HttpRequest
{
    public class Redirect
    {
        public Item RedirectItem { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public bool PreserveQueryString { get; set; }
        public bool IgnoreCase { get; set; }
        public RedirectType RedirectType { get; set; }
    }
}
