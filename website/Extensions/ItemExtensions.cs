using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Sites;

namespace LiquidSC.Foundation.RedirectManager.Extensions
{
    public static class ItemExtensions
    {
        /// <summary>
        /// Determine if the Item inherits a specific template, by ID
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="templateId">The template id</param>
        /// <returns><c>True</c> if the item inherits from <paramref name="templateId"/></returns>
        public static bool IsDerived(this Item item, ID templateId)
        {
            if (item == null)
            {
                return false;
            }

            return !templateId.IsNull && item.IsDerived(item.Database.GetItem(templateId, item.Language));
        }

        /// <summary>
        /// Determine if the Item inherits a specific template, by TemplateItem
        /// </summary>
        /// <param name="item">The item</param>
        /// <param name="inheritedTemplateItem">the templateItem</param>
        /// <returns><c>True</c> if the item inherits from <paramref name="inheritedTemplateItem"/></returns>
        public static bool IsDerived(this Item item, Item inheritedTemplateItem)
        {
            if (item == null
                || inheritedTemplateItem == null)
            {
                return false;
            }

            var itemTemplate = TemplateManager.GetTemplate(item);

            return itemTemplate != null && (itemTemplate.ID == inheritedTemplateItem.ID || itemTemplate.InheritsFrom(inheritedTemplateItem.ID, item.Database));
        }

        public static SiteContext SiteContext(this Item item)
        {
            var siteInfoList = Sitecore.Configuration.Factory.GetSiteInfoList();

            foreach (Sitecore.Web.SiteInfo siteInfo in siteInfoList)
            {
                if (string.IsNullOrWhiteSpace(siteInfo.RootPath) || string.IsNullOrWhiteSpace(siteInfo.RootPath))
                    continue;

                if (siteInfo.Domain != "sitecore" && item.Paths.FullPath.ToLower().StartsWith((siteInfo.RootPath + siteInfo.StartItem + "/").ToLower()))
                {
                    return SiteContextFactory.GetSiteContext(siteInfo.Name);
                }
            }

            return null;
        }
    }
}
