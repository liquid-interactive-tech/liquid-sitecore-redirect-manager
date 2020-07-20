using Sitecore.Data;

namespace LiquidSC.Foundation.RedirectManager
{
	public class Templates
	{
		public Templates()
		{
		}
		public struct SiteRedirectsFolder
		{
			public static ID ID;

			static SiteRedirectsFolder()
			{
				ID = ID.Parse("{A338AD2E-89FE-49FE-832A-3B7F0889A1F1}");
			}
		}
		public struct SiteRedirectsSettings
        {
            public static ID ID;

            static SiteRedirectsSettings()
            {
                ID = ID.Parse("{A929258B-A9B9-4DEB-8544-1E4F1FF568CF}");
            }
			public struct Fields
			{
				public readonly static ID RedirectToHttps;

				public readonly static ID RedirectToWWW;

				public readonly static ID RedirectWithTrailingSlash;

				public readonly static ID RedirectToLowercase;

				static Fields()
				{
					RedirectToHttps = new ID("{5330B523-F7C4-4B1B-B6EC-D5D51667EE46}");
					RedirectToWWW = new ID("{57F23934-7BE1-4447-8CF2-F424423F44ED}");
					RedirectWithTrailingSlash = new ID("{0555B4E8-CDCC-42A7-BA1F-0D108FD7BE27}");
					RedirectToLowercase = new ID("{472A462F-D03A-4DFD-A3A2-681AAB47EA65}");
				}
			}
		}
		public struct RedirectSettings
		{
			public static ID ID;

			static RedirectSettings()
			{
				ID = ID.Parse("{1D207731-CCA3-4757-B2F8-F7C9BC4E540A}");
			}

			public struct Fields
			{
				public readonly static ID RedirectType;

				public readonly static ID PreserveQueryString;

				public readonly static ID IgnoreCase;

				static Fields()
				{
					RedirectType = new ID("{57A41BCA-DF6E-45CD-80B1-A840DF5CE724}");
					PreserveQueryString = new ID("{A15D77B0-F075-4B6E-8D9F-D406C69F1A8D}");
					IgnoreCase = new ID("{3CA15495-0285-4F43-A665-9133256C12A5}");
				}
			}
		}

		public struct ItemRedirect
		{
			public static ID ID;

			static ItemRedirect()
			{
				ID = ID.Parse("{E50CB564-88F7-44B4-BCF0-60E27E3055E8}");
			}

			public struct Fields
			{
				public readonly static ID TargetItem;

				public readonly static ID SourceItem;

				static Fields()
				{
					SourceItem = new ID("{7AC02733-7BF2-44C4-B4FB-6FA065C6FDC6}");
					TargetItem = new ID("{CA3C20D0-3313-45CB-98BF-321752DC3E63}");
				}
			}
		}

		public struct PathRedirect
		{
			public static ID ID;

			static PathRedirect()
			{
				ID = ID.Parse("{5B22C5B8-4513-4D25-B87F-2E0D24322BDD}");
			}

			public struct Fields
			{
				public readonly static ID Source;

				public readonly static ID Target;

				static Fields()
				{
					Source = new ID("{01ED9875-E7A2-457D-BBAF-E8C0FB46ADB1}");
					Target = new ID("{6840F1B4-FE2F-4790-948B-032CF40FB1FB}");
				}
			}
		}

		public struct RedirectMap
		{
			public static ID ID;

			static RedirectMap()
			{
				ID = ID.Parse("{4F554D94-F449-429C-9DA0-187F316BC95E}");
			}

			public struct Fields
			{
				public readonly static ID UrlMapping;

				static Fields()
				{
					UrlMapping = new ID("{3A32FF07-C588-4696-B512-3A553D1AD6A8}");
				}
			}
		}

		//placeholder for future state
		public struct AdvancedRedirect
		{
			public static ID ID;

			static AdvancedRedirect()
			{
				ID = ID.Parse("{F51C5C3B-55EF-46B9-8CC0-288DFBABF598}");
			}

			public struct Fields
			{
				public readonly static ID Source;

				public readonly static ID Target;

				static Fields()
				{
					Source = new ID("{6E4B308E-F6FB-462D-A867-3C02FA7E1EF8}");
					Target = new ID("{6A00EE7C-67CF-4978-8712-2756D7D08964}");
				}
			}
		}
	}
}