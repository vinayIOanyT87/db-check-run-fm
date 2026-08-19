// --------------------------------------------------------------------------------------------------------------------
// <copyright file="default.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the _default type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
   using System.Diagnostics;
   using System.Web;
   using System.Web.Services;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	/// Code behind for default page
	/// </summary>
	// ReSharper disable InconsistentNaming
	public partial class _default : FMFormBase
	// ReSharper restore InconsistentNaming
	{
		/// <summary>
		/// Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected void Page_Init(object sender, EventArgs e)
		{
			var security = this.Session["Security"] as SecurityClass;
			if (security != null)
			{
				// If session is active, it needs to be closed. This may happen
				// when user enters FuelsManager home URL without first logging out of 
				// the previous session.
				FMChannelHelper.MakeCall<ISites>(x => x.Logout(security));
			}

			this.Session.RemoveAll();

			string backgroundColor = ConfigurationManager.AppSettings["LoginPageBackgroundColor"];
			if (string.IsNullOrEmpty(backgroundColor) == false)
			{
				this.PageBody.Style["Background-Color"] = backgroundColor;
			}

			string warningImage = ConfigurationManager.AppSettings["WarningPageImage"];
			if (string.IsNullOrEmpty(warningImage) == false)
			{
				this.WarnTable.Style["Background-Image"] = $"url('{warningImage}')";
			}

			string titleText = ConfigurationManager.AppSettings["WarningTitle"];
			if (string.IsNullOrEmpty(titleText) == false)
			{
				this.TitleLabel.Text = titleText;
			}

			string warningText = ConfigurationManager.AppSettings["WarningText"];
			if (string.IsNullOrEmpty(warningText))
			{
				this.Session.Add("POSTWARNING", true);
				this.Redirect("PostWarning.htm");
			}
			else
			{
				warningText = warningText.Replace(@"\n", "<br>");
				warningText = warningText.Replace(@"\b", "&bull; ");
				this.WarningLabel.InnerHtml = warningText;
			}

			if (this.Session["UseDataDictionary"] != null)
			{
				this.useDataDictionary = (bool)this.Session["UseDataDictionary"];
			}

			this.Session.Add("UseDataDictionary", this.useDataDictionary);

			if (this.useDataDictionary)
			{
				this.AcceptButton.Text =
					FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(Guids.SiteAdminGuid, this.AcceptButton.Text));
			}

			this.AcceptButton.Click += this.AcceptButtonClick;
		}

		/// <summary>
		/// Handles the Click event of the AcceptButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void AcceptButtonClick(object sender, EventArgs e)
		{
			this.Session.Add("POSTWARNING", true);

			this.Redirect("PostWarning.htm");
		}

		/// <summary>
		/// Generates a list item for the site select dropdown.
		/// </summary>
		/// <param name="security">Current FM security object.</param>
		/// <param name="siteSelectInfo">Info object containing information about site to add.</param>
		/// <param name="isDescKey">Flag indicating if the system is hardware keyed as DESC system.</param>
		/// <returns>A listitem for the site select dropdown</returns>
		private static ListItem GenerateSiteSelectItem(SecurityClass security, SiteSelectInfo siteSelectInfo, bool isDescKey)
		{
			string siteName = siteSelectInfo.ID;

			if ( isDescKey && string.IsNullOrEmpty( siteSelectInfo.Number ) == false )
			{
				siteName = $"{siteName} - {siteSelectInfo.Number}";
			}

			var listItem = new ListItem( siteName, siteSelectInfo.SiteGuid.ToString() );

			if ( siteSelectInfo.IsSiteGroup )
			{
				listItem.Attributes.Add( "GroupColor", "1" );
			}

			// If the item is the currently selected site, the list item needs to be marked as such.
			if (siteSelectInfo.SiteGuid.Equals(security.SiteGuid))
			{
				listItem.Selected = true;
			}

			return listItem;
		}


		/// <summary>
		/// Checks the hardware key for DESC and MultipleSite system flags.  Caches results
		/// in session for better performance.
		/// </summary>
		/// <param name="isMultipleSiteKey">Bool var to receive multiple site key result.</param>
		/// <param name="isDescKey">Bool var to receive DESC key result.</param>
		private static void DetermineHardwareKeyConfiguration(out bool isMultipleSiteKey, out bool isDescKey )
		{
			object isMultipleTemp = HttpContext.Current.Session["fmSiteSelectMultipleSiteKey"];

			if ( isMultipleTemp == null )
			{
				bool isDescTemp = false;

				FMChannelHelper.MakeCall<IHardwareKey>(
					hardwareKeyChannel =>
					{
						isMultipleTemp = hardwareKeyChannel.IsMultipleSiteKey();
						isDescTemp = hardwareKeyChannel.IsDescKey();
					} );

				HttpContext.Current.Session["fmSiteSelectMultipleSiteKey"] = isMultipleSiteKey = (bool) isMultipleTemp;
				HttpContext.Current.Session["fmSiteSelectDescKey"] = isDescKey = isDescTemp;
			}
			else
			{
				isMultipleSiteKey = (bool) HttpContext.Current.Session["fmSiteSelectMultipleSiteKey"];
				isDescKey = (bool) HttpContext.Current.Session["fmSiteSelectDescKey"];
			}
		}

		/// <summary>
		/// Responds to request from site select dropdown for the proper list of sites to display
		/// </summary>
		/// <returns>A collection of ListItems to use for populating the site select dropdown.</returns>
		[WebMethod(EnableSession = true)]
		public static List<ListItem> GetSites()
		{
			var security = (SecurityClass)HttpContext.Current.Session["Security"];

			if (security == null)
			{
				throw new System.ServiceModel.FaultException("Invalid session");
			}

			var siteNameList = new List<ListItem>();

			bool licenseNotExpiredAtLogin = (HttpContext.Current.Session["LicenseNotExpiredAtLogin"] as string != null && HttpContext.Current.Session["LicenseNotExpiredAtLogin"] as string == "true");
			if (licenseNotExpiredAtLogin == false)
			{
				return siteNameList;
			}

			// Determine hardware key state.  Save the value so we do not have to keep
			// doing it for every page navigation.
			bool isMultipleSiteKey;
			bool isDescKey;
			DetermineHardwareKeyConfiguration(out isMultipleSiteKey, out isDescKey);

			var site = FMChannelHelper.MakeCall<ISites, SiteClass>(x => x.Get(security, security.LoginSiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false));
			var loginSiteInfo = new SiteSelectInfo
			{
				ID = site.ID,
				Number = site.Number,
				SiteGuid = site.SiteGuid,
				IsSiteGroup = site.SiteGroup
			};

			var siteStack = (SiteSelectList)HttpContext.Current.Session["MenuSiteStack"];
			if (siteStack == null)
			{
				HttpContext.Current.Session["MenuSiteStack"] = siteStack = new SiteSelectList();

				if (site.SiteGroup)
				{
					siteStack.Add(loginSiteInfo);
				}
			}

			// Start the site list with the items on the stack.
			siteStack.ForEach(siteInfo => siteNameList.Add(GenerateSiteSelectItem(security, siteInfo, isDescKey)));

			var siteSelectList = new SiteSelectList();
			if (isMultipleSiteKey)
			{
				// Get the last site in the site stack and enumerate the sites assigned to it.  
				// This forms the basis for the rest of the site list.
				if (siteStack.Count > 0)
				{
					var siteGuid = siteStack[siteStack.Count - 1].SiteGuid;
					siteSelectList = FMChannelHelper.MakeCall<ISites, SiteSelectList>(x => x.EnumerateForSiteSelect(security, siteGuid));
				}
				else
				{
					siteSelectList.Add(loginSiteInfo);
				}
			}
			else
			{
				PopulateForSingleSiteSystem(security, siteSelectList);
			}

			var addSeparator = true;
			if (siteSelectList.Count > 0 && siteSelectList[0].IsSiteGroup)
			{
				var siteSeparator = new ListItem(FMMenuBar.GroupSiteSeparator, "-1") { Enabled = false };
				siteNameList.Add(siteSeparator);
			}

			// Add the enumerated sites to the list.
			foreach (var siteInfo in siteSelectList)
			{
				// Skip sites that are already listed by virtue of being in the site stack.
				SiteSelectInfo info = siteInfo;
				if (siteInfo.IsSiteGroup
					&& siteStack.Find(x => x.SiteGuid.Equals(info.SiteGuid)) != null)
				{
					continue;
				}

				if (addSeparator
					&& siteInfo.IsSiteGroup == false
					&& siteSelectList.Count > 1)
				{
					var siteSeparator = new ListItem(FMMenuBar.SiteSeparator, "-1") { Enabled = false };
					siteNameList.Add(siteSeparator);

					addSeparator = false;
				}

				siteNameList.Add(GenerateSiteSelectItem(security, siteInfo, isDescKey));
			}

			return siteNameList;
		}

		private static void PopulateForSingleSiteSystem(SecurityClass security, SiteSelectList siteSelectList)
		{
			SiteInfoDO siteInfoDO =
				FMChannelHelper.MakeCall<ISitesInfo, SiteInfoDO>(siteInfoChannel => siteInfoChannel.RefreshSiteInfo(security));

			if (security.HasRight(RIGHT.ALLOW_SINGLE_SITE_GROUP_SELECT))
			{
				var siteCollection = siteInfoDO.EnumerateParentSites(security.LoginSiteGuid);
				siteInfoDO.SortByGroupThenId(siteCollection);

				foreach (var site in siteCollection)
				{
					var lookupSite = siteInfoDO.GetSite(site.SiteGuid);

					var siteInfo = new SiteSelectInfo
					{
						ID = lookupSite.ID,
						Number = lookupSite.Number,
						SiteGuid = lookupSite.SiteGuid,
						IsSiteGroup = lookupSite.SiteGroup
					};

					siteSelectList.Add(siteInfo);
				}
			}

			var loginSite = siteInfoDO.GetSite(security.LoginSiteGuid);
			var loginSiteInfo = new SiteSelectInfo
			{
				ID = loginSite.ID,
				Number = loginSite.Number,
				SiteGuid = loginSite.SiteGuid,
				IsSiteGroup = loginSite.SiteGroup
			};

			siteSelectList.Add(loginSiteInfo);
		}

		/// <summary>
		/// This web method is responsible for registering page visits for display on the
		/// recently visited list on the MyMenu of FuelsManager.
		/// </summary>
		/// <param name="menuItem">A string containing the numerical value of the FMMenuItemType.</param>
		/// <param name="menuGuid">The menu unique identifier.</param>
		/// <param name="securityToken"></param>
		[WebMethod(EnableSession = true)]
		public static void RegisterVisit(string menuItem, string menuGuid)
		{
			if (string.IsNullOrEmpty(menuItem) == false)
			{
				var menuValue = Convert.ToInt32(menuItem);

				if (Enum.IsDefined(typeof(FMMenuItemType), menuValue))
				{
					FMMenuEngine.AddToRecentMenu(
						(FMMenuData)HttpContext.Current.Session[PageSessionKeyConstants.FM_MENU_DATA],
						(FMMenuItemType)menuValue,
						Guid.Parse(menuGuid));
				}
			}
		}

		/// <summary>
		/// This web method returns amount of time left for session to time out.
		/// </summary>
		/// <param name="securityToken">security token associated with current session.</param>
		/// <param name="renewSession">If set to true session will be extended.</param>
		[WebMethod(EnableSession = true)]
		public static int CheckTimeout(bool renewSession)
		{
			var security = (SecurityClass)HttpContext.Current.Session["Security"];

			try
			{
				if (security != null)
				{
					if (renewSession)
					{
						FMChannelHelper.MakeCall<ISessions>(x => x.PingSession(security));

					}

					SessionClass session =
						FMChannelHelper.MakeCall<ISessions, SessionClass>(s => s.GetSessionInfo(security));
					if (session != null)
					{
						int t = (int)(DateTimeOffset.Now - session.UpdatedDate).TotalSeconds;
						int r = session.Timeout * 60 - t;
						if (r <= 0)
						{
							r = 0;
						}


						return r;
					}
				}
			}
			catch
			{
				return 0;
			}

			return 0;

		}

		[WebMethod(EnableSession = true)]
		public static void Logout()
		{
			Global.Logout(HttpContext.Current);
		}

		/// <summary>
		/// This web method is called to indicate that user has acknowledged
		/// a warning message notifying user that application license is about to expire.
		/// </summary>
		/// <exception cref="System.ServiceModel.FaultException"></exception>
		[WebMethod(EnableSession = true)]
      public static void LicenseExpirationAcknowledged()
      {
         string AcknowledgedLicenseExpiration = "AcknowledgedLicenseExpiration";
         var session = HttpContext.Current.Session;

         var security = session["Security"] as SecurityClass;

         if (security == null)
         {
            throw new System.ServiceModel.FaultException("Invalid session");
         }

         var alarmAndEventLog = session[AcknowledgedLicenseExpiration] as AlarmAndEventLogClass;
         if (alarmAndEventLog == null)
         {
            return;
         }
         session.Remove(AcknowledgedLicenseExpiration);
         long daysLeft = (long)session["LicenseDaysLeftToExpire"];
         string logmsg = string.Format("FuelsManager license will expire in {0} day{1}.{2}Contact Varec Helpdesk at the phone number below to renew license{2}Tel:800-446-4950{2}", daysLeft, daysLeft == 1 ? string.Empty : "s", Environment.NewLine);
         //string msg = string.Format("<p>FuelsManager license will expire in {0} day{1}.</p><p>Contact Varec Helpdesk at the phone number below to renew license.</p><p>Tel:800-446-4950</p><p>DSN:697-6733,34,36,37,38</p>", days, days == 1 ? string.Empty : "s");
         alarmAndEventLog.Acknowledged = true;
         alarmAndEventLog.AssociatedData = security.UserID;

         Guid siteAdminGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
         SecurityClass innerSecurity = new SecurityClass()
         {
            UserGuid = security.UserGuid,
            UserID = security.UserID,
            SiteGuid = siteAdminGuid
         };
         FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(innerSecurity, alarmAndEventLog));

         SiteCollectionClass sites = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(x => x.EnumerateByUser(innerSecurity, security.UserGuid));
         foreach (SiteClass site in sites)
         {
            if (site.SiteGuid != siteAdminGuid)
            {
               innerSecurity.SiteGuid = site.SiteGuid;
               innerSecurity.SiteID = site.ID;

               FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(innerSecurity, alarmAndEventLog));
            }

         }

         try
         {
            FMChannelHelper.MakeCall<IFMEventLog>(x => x.WriteEntry(logmsg, FMEventLogEntryType.Warning));
         }
         catch
         {
            // ignored
         }
         try
         {
            using (var eventLog = new EventLog("Application", ".", "FuelsManager"))
            {
               eventLog.WriteEntry(logmsg, EventLogEntryType.Warning);
            }
         }
         catch
         {
            // ignored
         }
      }

   }
}