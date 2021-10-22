using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using LiquidSC.Foundation.RedirectManager.Repositories;
using System;
using Sitecore.Data;
using System.Linq;
namespace LiquidSC.Foundation.RedirectManager.EventHandlers
{
    public class RedirectMapCacheClearer
    {
        public RedirectMapCacheClearer()
        {
        }

        public void ClearRedirectCache(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Log.Info("RedirectMapCacheClearer clearing redirect map cache.", this);
            RedirectsRepository.Reset();
            Log.Info("RedirectMapCacheClearer done.", this);
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            Item item = Event.ExtractParameter(args, 0) as Item;

            if (item == null)
                return;

            if (IsCustomRedirectItems(item.TemplateID))
                this.ClearRedirectCache(sender, args);
        }

        public void OnItemSavedRemote(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            ItemSavedRemoteEventArgs itemSavedRemoteEventArg = args as ItemSavedRemoteEventArgs;
            if (itemSavedRemoteEventArg == null || itemSavedRemoteEventArg.Item == null)
                return;

            if (IsCustomRedirectItems(itemSavedRemoteEventArg.Item.TemplateID))
                this.ClearRedirectCache(sender, args);
        }

        private bool IsCustomRedirectItems(ID templateId)
        {
            var customTempates = new ID[] {
                Templates.RedirectMap.ID,
                Templates.SiteRedirectsSettings.ID, 
                Templates.AdvancedRedirect.ID,
                Templates.PathRedirect.ID,
                Templates.ItemRedirect.ID
            };
            return customTempates.Any(a => a.Equals(templateId));
        }
    }
}