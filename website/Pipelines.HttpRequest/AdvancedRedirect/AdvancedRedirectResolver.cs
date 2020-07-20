using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using LiquidSC.Foundation.RedirectManager.Extensions;
using LiquidSC.Foundation.RedirectManager.Pipelines.Base;
using Sitecore.Links;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using Sitecore.StringExtensions;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Links.UrlBuilders;
using System.Text.RegularExpressions;

namespace LiquidSC.Foundation.RedirectManager.Pipelines.HttpRequest
{
    public class AdvancedRedirectResolver : BaseClass
    {
        public AdvancedRedirectResolver()
        {
        }

        public override void ProcessRequest(HttpRequestArgs args)
        {
            //Get the site redirect settings if they exist
            var siteLevel = SiteLevelConfigMapping;

            //if site redirect settings exist
            if (siteLevel != null)
            {
                var current = HttpContext.Current;

                if (current != null && siteLevel.RedirectToHttpsAlways)
                {
                    if (!current.Request.IsSecureConnection)
                    {
                        var url = current.Request.Url.AbsoluteUri;
                        if (url.StartsWith("http:"))
                        {
                            url = "https" + url.Remove(0, 4);
                            this.Redirect301(current.Response, url);
                        }
                    }
                }
                if (siteLevel.AddWwwPrefix)
                {
                    var url = current.Request.Url.AbsoluteUri;

                    var urlVal = url.Substring(url.IndexOf("//") + 2);
                    if (!urlVal.StartsWith("www."))
                    {
                        url = (url.Substring(0, url.IndexOf("//") + 2)) + "www." + urlVal;
                        this.Redirect301(current.Response, url);
                    }

                }
            }

            //get the requested path as a sitecore path relative to the lowest known item
            string path = Context.Request.FilePath;

            //check cache for previously resolved path
            AdvancedRedirect resolvedMapping = this.GetResolvedMapping(path);

            if (resolvedMapping == null)
            {
                //if not cached, generate mapping now
                resolvedMapping = this.FindMapping(path);
            }
            if (resolvedMapping != null)
            {
                var dictionaryitem = GetCache<Dictionary<string, AdvancedRedirect>>(ResolvedAdvancedRedirectPrefix)
                                  ?? new Dictionary<string, AdvancedRedirect>();

                dictionaryitem[path] = resolvedMapping;

                SetCache(ResolvedAdvancedRedirectPrefix, dictionaryitem);
            }
            if (resolvedMapping != null && HttpContext.Current != null)
            {
                if (!resolvedMapping.Target.IsNullOrEmpty())
                {
                    if (resolvedMapping.RedirectType == RedirectType.Redirect301)
                    {
                        this.Redirect301(HttpContext.Current.Response, resolvedMapping.Target);
                    }
                    else if (resolvedMapping.RedirectType == RedirectType.Redirect302)
                    {
                        HttpContext.Current.Response.Redirect(resolvedMapping.Target, true);
                    }
                    else if (resolvedMapping.RedirectType == RedirectType.ServerTransfer)
                    {
                        HttpContext.Current.Server.TransferRequest(resolvedMapping.Target);
                    }
                    //default to 302
                    else
                    {
                        HttpContext.Current.Response.Redirect(resolvedMapping.Target, true);
                    }

                    args.AbortPipeline();
                }
            }
        }

        //TODO - share this with others
        protected virtual SiteLevelRedirectMapping SiteLevelConfigMapping
        {
            get
            {
                var siteLevelAllMapping = GetCache<Dictionary<string, SiteLevelRedirectMapping>>(SiteLevelAllMappingPrefix);

                if (siteLevelAllMapping == null || siteLevelAllMapping[SiteLevelMappingPrefix] == null)
                {
                    var siteLevelMapping = new SiteLevelRedirectMapping();

                    Item redirectSettingsItem = GetItem(GetRedirectSettingsId);

                    if (redirectSettingsItem != null && redirectSettingsItem.Fields != null)
                    {
                        CheckboxField https = redirectSettingsItem.Fields[Templates.RedirectSettings.Fields.RedirectToHttpsAlways];
                        CheckboxField www = redirectSettingsItem.Fields[Templates.RedirectSettings.Fields.AddWwwPrefix];

                        siteLevelMapping = new SiteLevelRedirectMapping()
                        {
                            AddWwwPrefix = www.Checked,
                            RedirectToHttpsAlways = https.Checked
                        };

                        if (siteLevelAllMapping == null)
                            siteLevelAllMapping = new Dictionary<string, SiteLevelRedirectMapping>();
                        siteLevelAllMapping.Add(SiteLevelMappingPrefix, siteLevelMapping);
                    }
                    if (this.CacheExpiration > 0 && siteLevelAllMapping != null)
                    {
                        SetCache(SiteLevelAllMappingPrefix, siteLevelAllMapping);
                    }
                    return siteLevelMapping;
                }

                return siteLevelAllMapping[SiteLevelMappingPrefix];
            }
        }

        protected virtual AdvancedRedirect FindMapping(string requestedPath)
        {
            AdvancedRedirect redirectMapping = null;
            List<AdvancedRedirect>.Enumerator enumerator = this.MappingsMap.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    AdvancedRedirect current = enumerator.Current;

                    //if the map entry key is regex
                    if (IsValidRegex(current.Source))
                    {
                        //if the current regex does not match the requested path, skip
                        if (!Regex.IsMatch(requestedPath, current.Source))
                        {
                            continue;
                        }
                    }
                    //if the map entry key path does not equal the request path, skip
                    else if (current.Source != requestedPath)
                    {
                        continue;
                    }

                    redirectMapping = current;
                    return redirectMapping;
                }
            }
            finally
            {
                ((IDisposable)enumerator).Dispose();
            }
            return redirectMapping;
        }

        /// <summary>
        /// Gets the list of redirect items associated with the website (bound through the redirectSettingsID attribute)
        /// </summary>
        protected virtual List<AdvancedRedirect> MappingsMap
        {
            get
            {
                var AdvancedRedirects = GetCache<List<AdvancedRedirect>>(AllAdvancedRedirectMappingsPrefix);

                if (AdvancedRedirects == null)
                {
                    AdvancedRedirects = new List<AdvancedRedirect>();

                    //get the settings item
                    Item redirectSettingsItem = GetItem(GetRedirectSettingsId);

                    List<Item> redirectItems = new List<Item>();

                    if (redirectSettingsItem != null)
                    {
                        //get the descendants that inherit the Redirect template's ID
                        redirectItems.AddRange((
                            from i in (IEnumerable<Item>)redirectSettingsItem.Axes.GetDescendants()
                            where i.InheritsFrom(Templates.Redirect.ID)
                            select i).ToList<Item>());
                    }

                    redirectItems.Sort(new TreeComparer());

                    //foreach redirect item
                    foreach (var redirectItem in redirectItems)
                    {
                        //get the type
                        RedirectType redirectType;
                        Enum.TryParse<RedirectType>(redirectItem[Templates.RedirectMap.Fields.RedirectType], out redirectType);

                        //get the source raw value
                        var source = this.GetSource(redirectItem);

                        //get the resolved target url
                        var targetUrl = this.GetRedirectUrl(redirectItem);

                        if (!string.IsNullOrWhiteSpace(targetUrl)
                            && !string.IsNullOrWhiteSpace(source)
                            )
                        {
                            var redirect = new AdvancedRedirect();

                            redirect.RedirectType = redirectType;
                            redirect.Source = source;
                            redirect.Target = targetUrl;

                            AdvancedRedirects.Add(redirect);
                        }
                    }


                    //if the config is set to use cache, cache the redirects for this site
                    if (this.CacheExpiration > 0)
                    {
                        SetCache(AllAdvancedRedirectMappingsPrefix, AdvancedRedirects);
                    }
                }
                return AdvancedRedirects;
            }
        }

        //TODO - share this with RedirectItemResolver
        protected virtual string GetRedirectUrl(Item redirectItem)
        {
            LinkField redirectLinkField = redirectItem.Fields[Templates.AdvancedRedirect.Fields.Target];
            string redirectUrl = null;
            if (redirectLinkField != null)
            {
                if (!redirectLinkField.IsInternal || redirectLinkField.TargetItem == null)
                {
                    redirectUrl = (!redirectLinkField.IsMediaLink || redirectLinkField.TargetItem == null ? redirectLinkField.Url : ((MediaItem)redirectLinkField.TargetItem).GetMediaUrl(null));
                }
                else
                {
                    //TODO - get site of taret item, not context, to support cross site resolving
                    SiteInfo siteInfo = Context.Site.SiteInfo;

                    //get the base options from the link provider
                    ItemUrlBuilderOptions defaultOptions = new DefaultItemUrlBuilderOptions();

                    defaultOptions.Site = SiteContextFactory.GetSiteContext(siteInfo.Name);
                    defaultOptions.AlwaysIncludeServerUrl = true;
                    defaultOptions.SiteResolving = true;

                    //inject option for languageembedding
                    defaultOptions.LanguageEmbedding = LanguageEmbedding.Never;

                    redirectUrl = LinkManager.GetItemUrl(redirectLinkField.TargetItem, defaultOptions);
                }
            }

            return redirectUrl;
        }

        //TODO - share this with RedirectItemResolver
        protected virtual string GetTargetQueryString(Item redirectItem)
        {
            LinkField redirectLinkField = redirectItem.Fields[Templates.AdvancedRedirect.Fields.Target];
            string redirectQueryString = String.Empty;
            if (redirectLinkField != null)
            {
                redirectQueryString = redirectLinkField.QueryString.TrimStart('?');
            }

            return redirectQueryString;
        }

        protected virtual AdvancedRedirect GetResolvedMapping(string ItemId)
        {
            var item = GetCache<Dictionary<string, AdvancedRedirect>>(ResolvedAdvancedRedirectPrefix);
            if (item == null || !item.ContainsKey(ItemId))
            {
                return null;
            }
            return item[ItemId];
        }

        protected virtual string GetSource(Item sourceItem)
        {
            string str = string.Empty;

            if (sourceItem.Fields[Templates.AdvancedRedirect.Fields.Source] != null)
            {
                str = sourceItem.Fields[Templates.AdvancedRedirect.Fields.Source].Value;

                //if (IsValidRegex(sourceItem.Fields[Templates.AdvancedRedirect.Fields.Source].Value))
                //{

                //}
            }

            return str;
        }

        private static bool IsValidRegex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return false;

            //supporting only "starts with" regex at this time. alternative is to generate a "is regex" checkbox on the cms side for the user to determine if their own is regex
            if (!pattern.StartsWith("^")) return false;

            try
            {
                pattern = HttpUtility.UrlDecode(pattern.ToLower(), System.Text.Encoding.UTF8);
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}