namespace LiquidSC.Foundation.RedirectManager.Pipelines.HttpRequest
{
    public class SiteLevelRedirectMapping
    {
        public bool ForceHttps { get; set; }
        public bool IncludeWWW { get; set; }
        public bool ForceTrailingSlash { get; set; }
        public bool ForceLowerCase { get; set; }

        public SiteLevelRedirectMapping()
        {
            ForceHttps = false;
            IncludeWWW = false;
            ForceTrailingSlash = false;
            ForceLowerCase = false;
        }
    }
}
