// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMMenuBar.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FMMenuBar type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
    using FMBusinessObjects.UtilityObjects;
    using FMCore;
    using FuelsManager.Areas.Controllers;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
	using System.Reflection;
    using System.Web;
    using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

    /// <summary>
    /// A Web User Control that displays the header information on each page, including
    /// the FuelsManager logo, the site selection combo box, the menus, and QuickLinks.
    /// </summary>
    public partial class FMMenuBar : FMUserControlBase
	{
		#region Constants
		/// <summary>
		/// The FuelsManager web application relative URL prefix.
		/// </summary>
		protected const string FmWebAppRelativeUrlPrefix = "~/FMWebApp/";

		/// <summary>
		/// The FuelsManager home page URL.
		/// </summary>
		public const string FuelsManagerHomePageUrl = FmWebAppRelativeUrlPrefix + "FuelsManagerForm.aspx";

		/// <summary>
		/// The FuelsManager logout page URL.
		/// </summary>
		public const string FuelsManagerLogoutPageUrl = FmWebAppRelativeUrlPrefix + "LogoutForm.aspx";

		/// <summary>
		/// The text for the LOGOUT BUTTON
		/// </summary>
		public const string LOGOUT_BUTTON_TEXT = "Logout";

		/// <summary>
		/// The text for the SETTINGS BUTTON
		/// </summary>
		public const string SETTINGS_BUTTON_TEXT = "Settings";

		/// <summary>
		/// The text for the CHANGE_PASSWORD BUTTON
		/// </summary>
		public const string CHANGE_PASSWORD_BUTTON_TEXT = "Change Password";

		public const string SESSION_KEY_USER_SITE_GUID = "UserSiteGuid";
		public const string MY_PROFILE_BUTTON_TEXT = "My Profile";

		/// <summary>
		/// These constants are all tightly coupled with menu.css. These values are
		/// used to properly position the drop-down menu panels, in AdjustPanelPositionAndWidth()
		/// </summary>
		protected const string COL_NARROW_CLASS = "colNarrow";
		protected const string COL_NORMAL_CLASS = "colNormal";
		protected const string COL_WIDE_CLASS = "colWide";
		protected const int COL_NARROW_WIDTH = 132;
		protected const int COL_NORMAL_WIDTH = 165;
		protected const int COL_WIDE_WIDTH = 200;
		protected const double AVG_LETTER_WIDTH = 6.64;
		protected const int SPACE_BTW_MENUS = 47;
		protected const int ALLOWED_RIGHT_MARGIN = 102;
		protected const int COLUMN_SPACING = 10;
		protected const int DOWN_ARROW_FUDGE_FACTOR = 30;

		/// <summary>
		/// The group site separator string.
		/// </summary>
		// TFS 120204 - Changed to ascii character for single horizontal line (character Alt+196)for a cleaner look
		public const string GroupSiteSeparator = "────── Site Groups ──────";

		/// <summary>
		/// The site separator string.
		/// </summary>
		// TFS 120204 - Changed to ascii character for single horizontal line (character Alt+196)for a cleaner look
		public const string SiteSeparator = "──────── Sites ────────";

      protected const string AcknowledgedLicenseExpiration = "AcknowledgedLicenseExpiration";

      #endregion


      #region Public Fields


      /// <summary>
      /// Security object
      /// </summary>
      public SecurityClass security = null;
      public string DaysLeft = string.Empty;

      #endregion

      #region Protected Fields

      //	protected string menucssurl = "~/MenuBar/css/menu.css";

      //	protected string menufocuscssurl = "";

      protected bool keyboardAccessibilityEnabled = false;

		/// <summary>
		/// The object that holds all the data for the menus
		/// </summary>
		protected FMMenuData menuData = null;

		protected string csrfToken = string.Empty;

		/// <summary>
		/// The form on which this control resides
		/// </summary>
		protected FMFormBase parentForm = null;

		/// <summary>
		/// Whether Use Data Dictionary is on
		/// </summary>
		protected bool useDataDictionary = true;

		/// <summary>
		/// Time in minutes user is warned before session expires.
		/// </summary>
		protected string sessionWarningTime = "5";
        
		protected bool licenseNotExpiredAtLogin = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether to show the menu bar when javascript detects that the browser
        /// window was opened as a dialog, with either window.showModalDialog()
        /// or window.showModelessDialog(). The default is false.
        /// </summary>
        /// <remarks>
        /// If all modal dialogs are opened using window.showModalDialog(), and
        /// window.showModelessDialog() is never used, then this property never
        /// needs to be set. If setting this property doesn't produce the desired
        /// behavior, then the visibility of the control needs to be handled
        /// explicitly in the server-side code for the page.
        /// </remarks>
        [System.ComponentModel.Browsable(true)]
		public bool ShowInDialog { get; set; }

		public string operateUrl = string.Empty;

		public bool HideEvenIfNotDialog { get; set; }

		public override string ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = "FMM";
			}
		}

		/// <summary>
		/// Gets the previous menu item URL.
		/// </summary>
		public string PreviousMenuItemUrl
		{
			get
			{
				string previousUrl = FuelsManagerHomePageUrl;
				if (this.menuData != null)
				{
					var recentList = this.menuData.RecentMenuItems;
					if (recentList.Count > 1)
					{
						previousUrl = recentList[1].NavigateUrl;
						if (previousUrl.IndexOf('/') < 0)
						{
							previousUrl = FmWebAppRelativeUrlPrefix + previousUrl;
						}
					}
				}

				return previousUrl;
			}
		}

		/// <summary>
		/// Gets the current menu item URL.
		/// </summary>
		public string CurrentMenuItemUrl
		{
			get
			{
				string currentUrl = FuelsManagerHomePageUrl;
				if (this.menuData != null)
				{
					currentUrl = this.menuData.CurrentMenuItem.NavigateUrl;
					if (currentUrl.IndexOf('/') < 0)
					{
						currentUrl = FmWebAppRelativeUrlPrefix + currentUrl;
					}
				}

				return currentUrl;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Default Constructor
		/// </summary>
		public FMMenuBar()
		{
			// Set default value
			this.ShowInDialog = false;
			this.HideEvenIfNotDialog = false;
		}

		/// <summary>
		/// Refresh the information in the control. Re-loads menu information and site information,
		/// then re-creates the ASP.NET and HTML controls for everything.
		/// </summary>
		public void Refresh()
		{
			// Get permissions for transaction aliases
			FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.RefreshTransactionSecurityRightsCache(ref security)
																);

			Session["Security"] = security;
			csrfToken = security.CSRFToken;

			// Store permissions in Session variable for later use by AccountingTreeNav and others
			this.LoadTransactionAliases();

			// Load site to get SiteGroup value
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetUsingGuid(security, security.SiteGuid)
																);

			List<KeyValuePair<string, Exception>> exceptions;
			if (Session[PageSessionKeyConstants.FM_MENU_DATA] == null)
			{
				Session[PageSessionKeyConstants.FM_MENU_DATA] = FMMenuEngine.LoadMenuData(security, site.SiteGroup, useDataDictionary, out exceptions);
			}
			else
			{
				// This method preserves the Recent list
				Session[PageSessionKeyConstants.FM_MENU_DATA] = FMMenuEngine.RefreshMenuData((FMMenuData)Session[PageSessionKeyConstants.FM_MENU_DATA], security, site.SiteGroup, useDataDictionary, out exceptions);
			}

			HandleMenuLoadErrors(exceptions);

			if (Session[PageSessionKeyConstants.FM_MENU_DATA] == null)
				throw new ApplicationException("Failed to generate menu data");

			this.menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;

			this.phMenu.Controls.Clear();
			this.phQuickLinks.Controls.Clear();
			this.LoadMenuControls();
		}

		/// <summary>
		/// Opens help page in modeless dialog using configured help URL
		/// </summary>
		/// <param name="pagePath">Relative path of help page</param>
		public void OpenHelpPage(string pagePath)
		{
			try
			{
				string url = menuData.GetHelpUrl(useDataDictionary) + "/" + pagePath;
				string jscript = "var helpWin = window.open(\"" + HttpUtility.JavaScriptStringEncode(url) + "\", \"FMHelpWin\", \"menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes,width=700,height=800\"); setTimeout(\"helpWin.focus()\", 0);";
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HelpOpener", jscript, true);
			}
			catch (Exception ex)
			{
				parentForm.ErrorHandler(ex);
			}

		}

		public bool IsAlarmCheckAvailable()
		{
			var pageName = this.Request.GetQueryOrFormValue("target");
			if (string.IsNullOrEmpty(pageName)) pageName = "";
			return security.HasRight(RIGHT.OPERATE_VIEW_ALARM_SUMMARY)
				&& pageName.IndexOf("operateindex", StringComparison.InvariantCultureIgnoreCase) < 0;
		}

		public bool IsViewOperateOnly()
		{
			var pageName = this.Request.GetQueryOrFormValue("target");
			if (string.IsNullOrEmpty(pageName)) pageName = "";

			return security.HasRight(RIGHT.VIEW_OPERATE_ONLY);
		}

		//public bool IsPingRequired()  // bds
		//{
		//return false;
		//}
		#endregion

		#region Private Methods

		/// <summary>
		/// determines the proper menu image url 
		/// </summary>
		private string GetImageURL(string fileName)
		{
			return "../FMWebApp/images/" + fileName;
		}
		/// <summary>
		/// Create menu controls and put them on the page
		/// </summary>
		protected void LoadMenuControls()
		{
			HtmlGenericControl menuBarUL = new HtmlGenericControl("ul");
			menuBarUL.ID = "menu";
			//menuBarUL.Attributes.Add("class", "mainMenu");
			bool bAddDropDownPanel = true;
			bool bAddWithTitle = true;
			string HelpScript = this.GetHelpLinkScript();


			foreach (FMMenuRootItem menuRootItem in this.menuData.MenuRootItems)
			{
				bAddDropDownPanel = true;
				bAddWithTitle = true;
				menuRootItem.NumColumnPanels = 0;

				HtmlGenericControl rootLI = new HtmlGenericControl("li");

				LinkButton menuRootItemLink = new LinkButton();
				if (keyboardAccessibilityEnabled)
				{
					rootLI.Attributes["onmouseleave"] = "OnMenuItemLeave(this, event);";
					menuRootItemLink.TabIndex = 1;
					menuRootItemLink.Attributes["onmouseover"] = "OnMenuItemHover(this, event);";
					menuRootItemLink.Attributes["onblur"] = "OnMenuItemBlur(this, event);";
					menuRootItemLink.Attributes["onkeypress"] = "OnMenuItemKeyPressed(this, event);";
					menuRootItemLink.Attributes["onfocus"] = "OnMenuItemFocus(this, event);";
				}


				switch (menuRootItem.RootItemName)
				{
					case "My Menu":
						rootLI.Attributes.Add("class", "gradient");
						menuRootItemLink.Attributes.Add("onClick", "return false;");
						break;
					case "About":
						menuRootItemLink.OnClientClick = "javascript:FMMenuBarLib.openLegalInfo()";
						bAddDropDownPanel = false;
						rootLI.Attributes.Add("class", "gradient");
						menuRootItemLink.Attributes.Add("onClick", "return false;");
						menuRootItemLink.ID = "About";

                        break;
					case "Privacy":
						menuRootItemLink.OnClientClick = "javascript:FMMenuBarLib.openPrivacyPolicy()";
						bAddDropDownPanel = false;
						rootLI.Attributes.Add("class", "gradient");
						menuRootItemLink.Attributes.Add("onClick", "return false;");
						var privacyPolicyPath = ConfigurationManager.AppSettings["PrivacyPolicyPath"];
						if (string.IsNullOrEmpty(privacyPolicyPath))
						{
							rootLI.Attributes.Add("class", "hidden");
						}
						break;
					case "Help":
						menuRootItemLink.OnClientClick = HelpScript;
						bAddDropDownPanel = false;
						rootLI.Attributes.Add("class", "gradient");
                        menuRootItemLink.ID = "Help";
                        break;

					case "Mobile": // WW-Dispatch
								   //bAddDropDownPanel = menuRootItem.IsEnabled;
								   //menuRootItemLink.Enabled = menuRootItem.IsEnabled;
						rootLI.Attributes.Add("class", "gradient");
						break;

					default:
						rootLI.Attributes.Add("class", "gradient");
						menuRootItemLink.Attributes.Add("onClick", "return false;");
						break;
				}


				if (bAddWithTitle)
				{
					//if (menuRootItem.IsEnabled)
					//{
					HtmlGenericControl divImg = new HtmlGenericControl("div");
					divImg.ID = menuRootItem.RootItemName + "_ImgDiv";
					divImg.Attributes.Add("class", "imageswap");

					Image imgOff = new Image();
					imgOff.ImageUrl = GetImageURL(menuRootItem.OffImageFileName);
					imgOff.CssClass = "off";
					divImg.Controls.Add(imgOff);

					Image imgOn = new Image();
					imgOn.ImageUrl = GetImageURL(menuRootItem.OnImageFileName);
					imgOn.CssClass = "on";
					divImg.Controls.Add(imgOn);

					menuRootItemLink.Controls.Add(divImg);

					HtmlGenericControl divText = new HtmlGenericControl("div");
					divText.ID = menuRootItem.RootItemName + "_TextDiv";
					divText.Attributes.Add("class", "menuText");

					Label span = new Label();
					span.Text = menuRootItem.GetDisplayName(useDataDictionary);
					divText.Controls.Add(span);
					menuRootItemLink.Controls.Add(divText);
					//}
					//else
					//               {
					//	Image img = new Image();
					//	img.ImageUrl = GetImageURL(menuRootItem.OffImageFileName);
					//	menuRootItemLink.Controls.Add(img);

					//	HtmlGenericControl divText = new HtmlGenericControl("div");
					//	divText.ID = menuRootItem.RootItemName + "_TextDiv";
					//	divText.Attributes.Add("class", "menuText");

					//	Label span = new Label();
					//	span.Text = menuRootItem.GetDisplayName(useDataDictionary);
					//	divText.Controls.Add(span);
					//	menuRootItemLink.Controls.Add(divText);

					//	menuRootItemLink.ToolTip = "Feature is disabled. Please call Varec sales-rep to enable it.";
					//}
				}
				else
				{
					Image img = new Image();
					img.ImageUrl = GetImageURL(menuRootItem.OnImageFileName);
					menuRootItemLink.Controls.Add(img);
				}

				rootLI.Controls.Add(menuRootItemLink);

				if (bAddDropDownPanel)
				{
					Panel dropDownPanel = new Panel();
					dropDownPanel.CssClass = menuRootItem.PanelCssClass;

					int maxNumColumnsInCategory = 0;

					foreach (FMMenuCategory menuCategory in menuRootItem.MenuCategories)
					{
						//int numColumns = 999;
						int maxItemsPerColumn = 999;

						if (menuCategory.MaxItemsPerColumn > 0)
						{
							maxItemsPerColumn = menuCategory.MaxItemsPerColumn;
						}

						Panel categoryPanel = new Panel();
						categoryPanel.CssClass = "category";
						HtmlGenericControl categoryHeader = new HtmlGenericControl("h3");
						categoryHeader.InnerText = menuCategory.GetDisplayName(useDataDictionary);
						categoryPanel.Controls.Add(categoryHeader);

						Panel columnPanel = new Panel();
						columnPanel.CssClass = menuRootItem.ColumnCssClass;

						HtmlGenericControl menuItemsUL = new HtmlGenericControl("ul");

						int numColumnsInCategory = 0;
						int iItem = 0;    // one-based index of item in current column

						foreach (FMMenuItem menuItem in menuCategory.MenuItems)
						{
							// This could become null if enough rights were removed which caused the menu item to no longer be available
							// while it was in the list of "recent items".
							if (null != menuItem)
							{
								iItem++;

								if (iItem > maxItemsPerColumn)
								{
									columnPanel.Controls.Add(menuItemsUL);
									categoryPanel.Controls.Add(columnPanel);
									menuRootItem.NumColumnPanels++;
									numColumnsInCategory++;

									columnPanel = new Panel();
									columnPanel.CssClass = menuRootItem.ColumnCssClass;

									menuItemsUL = new HtmlGenericControl("ul");

									iItem = 1;
								}

								menuItemsUL.Controls.Add(CreateMenuItemListItem(menuItem));
							}
						}

						columnPanel.Controls.Add(menuItemsUL);
						categoryPanel.Controls.Add(columnPanel);
						menuRootItem.NumColumnPanels++;
						numColumnsInCategory++;
						if (numColumnsInCategory > maxNumColumnsInCategory)
						{
							maxNumColumnsInCategory = numColumnsInCategory;
						}

						dropDownPanel.Controls.Add(categoryPanel);
					}
					AdjustPanelPositionAndWidth(dropDownPanel, menuRootItem, maxNumColumnsInCategory);
					rootLI.Controls.Add(dropDownPanel);
				}
				menuBarUL.Controls.Add(rootLI);

				phMenu.Controls.Add(menuBarUL);
			}

			foreach (FMMenuItem menuItem in menuData.QuickLinksMenuItems)
			{
				phQuickLinks.Controls.Add(CreateQuickLinkListItem(menuItem));
			}
		}

		/// <summary>
		/// Use the Page's ErrorHandler method to display errors that occurred while loading
		/// menu data
		/// </summary>
		/// <param name="exceptions">Collection of information about exceptions</param>
		protected void HandleMenuLoadErrors(List<KeyValuePair<string, Exception>> exceptions)
		{
			// There are several situations in which non-fatal exceptions can be recorded
			if (exceptions != null)
			{
				foreach (KeyValuePair<string, Exception> exInfo in exceptions)
				{
					if (exInfo.Value is ReflectionTypeLoadException)
					{
						parentForm.ErrorHandler(exInfo.Key, BuildLoadExceptionMessage(exInfo.Value as ReflectionTypeLoadException));
					}
					else if (exInfo.Key == "")
					{
						parentForm.ErrorHandler(exInfo.Value);
					}
					else
					{
						parentForm.ErrorHandler(exInfo.Key, exInfo.Value);
					}
				}
			}
		}

		/// <summary>
		/// Format the output for a ReflectionTypeLoadException
		/// </summary>
		/// <param name="reflectionException">The ReflectionTypeLoadException</param>
		/// <returns>Formatted error information</returns>
		private string BuildLoadExceptionMessage(ReflectionTypeLoadException reflectionException)
		{
			if (reflectionException == null)
			{
				throw new ArgumentNullException();
			}

			string message = reflectionException.Message;

			foreach (Exception except in reflectionException.LoaderExceptions)
			{
				message += "\n" + "===========" + "\n" + except.Message;
			}

			return message;

		}

		/// <summary>
		/// Create an LI element that holds the LinkButton for a particular
		/// menu item
		/// </summary>
		/// <param name="menuItem">The menu item</param>
		/// <returns>HTML control to add to page</returns>
		protected HtmlGenericControl CreateMenuItemListItem(FMMenuItem menuItem)
		{
			var hyperLink = new HyperLink();

			string menuText = menuItem.GetDisplayName(useDataDictionary);

			hyperLink.Text = menuText;
			var navigateUrl = menuItem.NavigateUrl;

			if (navigateUrl.IndexOf('/') < 0)
			{
				navigateUrl = FmWebAppRelativeUrlPrefix + navigateUrl;
			}

			navigateUrl = navigateUrl.Replace("\\", "/");
			navigateUrl = navigateUrl.Replace("~", "..");

			// For display purposes we populate the navigationUrl.  The onclick event will 
			// prevent this from actually be used for more than display.
			hyperLink.NavigateUrl = navigateUrl;

			var li = new HtmlGenericControl("li");

            if (menuItem.MenuItemType == FMMenuItemType.MY_MENU_ADD_FAVORITE)
			{
				hyperLink.ID = "lnkAddFavorite";
				hyperLink.ClientIDMode = ClientIDMode.Static;
				if (licenseNotExpiredAtLogin)
				{
					hyperLink.Attributes.Add("onclick", "__doPostBack('lnkAddFavorite', '');return false;");
				}
				if (keyboardAccessibilityEnabled)
				{

					hyperLink.TabIndex = 1;
				}
			}
			else
			{
				var onClick = "return FMMenu.Nav('" + navigateUrl + "', '" + ((int)menuItem.MenuItemType) + "', '" + menuItem.DynamicMenuItemGuid.ToString() + "');";
				if (menuItem.OpenInSeparateTab)
				{
                    onClick = GetScriptToOpenInSeparateTab(menuItem.NavigateUrl);
                    hyperLink.Target = "_newtab";
                }
                if (menuItem.MenuItemType != FMMenuItemType.MOBILE_LAUNCH && licenseNotExpiredAtLogin)
                    hyperLink.Attributes.Add("onclick", onClick);

				if (keyboardAccessibilityEnabled)
				{
					hyperLink.Attributes.Add("onblur", "OnMenuItemLeaveBlur(this, event);");
					hyperLink.TabIndex = 0;
				}
			}

			// Add id attribute to each menu item.
			string rootMenuName = string.IsNullOrEmpty(menuItem.RootMenuName) ? string.Empty : menuItem.RootMenuName.Trim();
			string categoryName = string.IsNullOrEmpty(menuItem.CategoryName) ? string.Empty : menuItem.CategoryName.Trim();
			string itemName = string.IsNullOrEmpty(menuItem.ItemName) ? string.Empty : menuItem.ItemName.Trim();
			string attributeId = rootMenuName;

			if (string.IsNullOrEmpty(categoryName) == false)
			{
				if (string.IsNullOrEmpty(rootMenuName))
				{
					attributeId = categoryName;
				}
				else
				{
					attributeId = attributeId + "_" + categoryName;
				}
			}

			if (string.IsNullOrEmpty(itemName) == false)
			{
				if (string.IsNullOrEmpty(rootMenuName) && string.IsNullOrEmpty(categoryName))
				{
					attributeId = itemName;
				}
				else
				{
					attributeId = attributeId + "_" + itemName;
				}
			}

			if (string.IsNullOrEmpty(attributeId) == false)
			{
				hyperLink.Attributes.Add("id", attributeId);
			}

			if (hyperLink.Text == "Operate")
			{
				operateUrl = hyperLink.NavigateUrl;
				//Redirect(hyperLink.NavigateUrl);
			}


			if (menuItem.IsEnabled)
			{
				li.Controls.Add(hyperLink);
			}
			else
			{
				li.InnerText = menuText;
			}
			return li;
		}

		/// <summary>
		/// Create an LI element that holds the LinkButton for a particular
		/// Quick Links item
		/// </summary>
		/// <param name="menuItem">The menu item</param>
		/// <returns>HTML control to add to page</returns>
		protected HtmlGenericControl CreateQuickLinkListItem(FMMenuItem menuItem)
		{
			HtmlGenericControl li = new HtmlGenericControl("li");

			if (menuItem.MenuItemType == FMMenuItemType.QUICK_LINKS_ADD_QUICK_LINK)
			{
				LinkButton ibtnAddQuickLink = new LinkButton();
				ibtnAddQuickLink.ID = "ibtnAddQuickLink";
				ibtnAddQuickLink.ClientIDMode = ClientIDMode.Static;
				ibtnAddQuickLink.ToolTip = "Add Quick Link";
				ibtnAddQuickLink.CssClass = "addQuickLink";
				ibtnAddQuickLink.Command += new CommandEventHandler(AddQuickLink_Command);
				ibtnAddQuickLink.CommandName = menuItem.MenuItemType.ToString() + ";" + menuItem.DynamicMenuItemGuid.ToString();
				ibtnAddQuickLink.CommandArgument = menuItem.NavigateUrl;
				ibtnAddQuickLink.TabIndex = 1;
				li.Controls.Add(ibtnAddQuickLink);
			}
			else
			{
				li = this.CreateMenuItemListItem(menuItem);

				if (menuItem.MenuItemType == FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS)
				{
					li.ID = "quickLinksLabelListItem";
					li.Attributes.Add("class", "quickLinksLabelListItem");
					li.ClientIDMode = ClientIDMode.Static;
				}
				else
				{
					((HyperLink)li.Controls[0]).CssClass = "quickLink";
				}

				if (keyboardAccessibilityEnabled)
				{
					((HyperLink)li.Controls[0]).TabIndex = 1;
				}
			}

			return li;
		}

		/// <summary>
		/// Position a drop-down panel so that it tends toward the center of the page,
		/// and it does not have extra blank space
		/// </summary>
		/// <param name="dropDownPanel">The drop-down panel</param>
		/// <param name="targetMenuRootItem">Root menu item of the panel</param>
		/// <param name="maxNumColumnsInCategory">The maximum number of columns that were rendered in any category
		/// in the drop-down</param>
		protected void AdjustPanelPositionAndWidth(Panel dropDownPanel, FMMenuRootItem targetMenuRootItem, int maxNumColumnsInCategory)
		{
			if (targetMenuRootItem.RootItemName == "My Menu")
				return;

			double totalMenuBarWidth = 0.0;
			double targetMenuTitleAbsLeft = 0.0;
			bool reachedTargetMenu = false;

			foreach (FMMenuRootItem menuRootItem in menuData.MenuRootItems)
			{
				if (menuRootItem.RootItemName != "My Menu")
				{
					totalMenuBarWidth += AVG_LETTER_WIDTH * menuRootItem.GetDisplayName(useDataDictionary).Length + SPACE_BTW_MENUS;

					if (!reachedTargetMenu)
					{
						if (menuRootItem == targetMenuRootItem)
						{
							reachedTargetMenu = true;
						}
						else
						{
							targetMenuTitleAbsLeft = totalMenuBarWidth;
						}
					}
				}
			}

			// Remove the extra space amount, add some more space on the right
			totalMenuBarWidth += ALLOWED_RIGHT_MARGIN - SPACE_BTW_MENUS;

			// How many columns wide will the panel be?
			int actualNumColumnsWide;
			if (maxNumColumnsInCategory > targetMenuRootItem.ExpectedNumColumns)
			{
				// In this case, one category is so wide, that it exceeds the designed width of the panel
				actualNumColumnsWide = maxNumColumnsInCategory;
			}
			else if (targetMenuRootItem.NumColumnPanels < targetMenuRootItem.ExpectedNumColumns)
			{
				// In this case, there are so few column panels, that it will be narrower than designed
				actualNumColumnsWide = targetMenuRootItem.NumColumnPanels;
			}
			else
			{
				actualNumColumnsWide = targetMenuRootItem.ExpectedNumColumns;
			}

			// Now determine proper width
			int properWidth;

			switch (targetMenuRootItem.ColumnCssClass)
			{
				case COL_NARROW_CLASS:
					properWidth = actualNumColumnsWide * (COL_NARROW_WIDTH + COLUMN_SPACING);
					break;
				case COL_WIDE_CLASS:
					properWidth = actualNumColumnsWide * (COL_WIDE_WIDTH + COLUMN_SPACING);
					break;
				default:
					properWidth = actualNumColumnsWide * (COL_NORMAL_WIDTH + COLUMN_SPACING);
					break;
			}

			// Where should it go? Figure out the LEFT value counting left edge of menu bar as zero,
			// which would put the panel in the center
			int centeredAbsLeft = (int)((totalMenuBarWidth - (double)properWidth) / 2.0);

			int actualAbsLeft;

            // Can't have it too far to the left
            var widthOfMenuItem = (int)(AVG_LETTER_WIDTH * targetMenuRootItem.GetDisplayName(useDataDictionary).Length + SPACE_BTW_MENUS);
            if (centeredAbsLeft < widthOfMenuItem)
            {
                actualAbsLeft = widthOfMenuItem;
            }
            else
            {
				// How far to the right does the menu title go?
				double targetMenuTitleAbsRight = targetMenuTitleAbsLeft + targetMenuRootItem.GetDisplayName(useDataDictionary).Length + SPACE_BTW_MENUS
					+ DOWN_ARROW_FUDGE_FACTOR;

				// How far to the right would the panel go? If not far enough, then move to the right
				if ((double)(centeredAbsLeft + properWidth) < targetMenuTitleAbsRight)
				{
					actualAbsLeft = (int)targetMenuTitleAbsRight - properWidth;
				}
				else
				{
					// Otherwise, it's just right
					actualAbsLeft = centeredAbsLeft;
				}
			}

			// Now figure out the relative left value
			int relLeft = actualAbsLeft - (int)targetMenuTitleAbsLeft;

			// Can't have it too far to the right. Somehow -1 is flush left
			if (relLeft > -30)
				relLeft = -1;

			// Hooray!
			dropDownPanel.Width = properWidth;
			dropDownPanel.Style.Add("left", relLeft.ToString(CultureInfo.InvariantCulture) + "px");
		}

		private SiteClass GetSiteClass(SecurityClass security, Guid guid, bool p1, bool p2, bool p3)
		{
			return FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(security, guid, p1, p2, p3)
																);
		}

		/// <summary>
		/// Whether a user is mapped to the current site
		/// </summary>
		/// <returns>Whether the user is mapped to the current site</returns>
		private bool UserMappedToSite()
		{
			return FMChannelHelper.MakeCall<IEntityToSiteMaps, bool>(entityToSiteChannel =>
			{
				return entityToSiteChannel.IsAssigned(security, ENTITY_TYPE.USER, security.SiteGuid, security.UserGuid);
			});
		}

		/// <summary>
		/// This method will clear necessary session items when the session changes.
		/// </summary>
		private void ClearSessionItems()
		{
			// Clear Ledger Form session values. Note that the product, owner, and manager are not cleared so that the values can be preserved
			// for use across sites
			Session.Remove(PageSessionKeyConstants.LEDGER_MONTH_SELECTION);
			Session.Remove(PageSessionKeyConstants.LEDGER_VIEW_SELECTION);
			Session.Remove(PageSessionKeyConstants.LEDGER_VIEW_COLLECTION);
			Session.Remove(PageSessionKeyConstants.LEDGER_GROSS_NET_SELECTION);
			Session.Remove(PageSessionKeyConstants.CRAF_SESSION_SITE_SELECT);
			Session.Remove(PageSessionKeyConstants.CRAF_SESSION_INCLUDE_MEMBERS);
			Session.Remove(PageSessionKeyConstants.CRAF_SESSION_COMPANY_SELECT);
			Session.Remove(PageSessionKeyConstants.CRAF_SESSION_COMPANY_ROLE_SELECT);
			Session.Remove(PageSessionKeyConstants.INVENTORY_RECONCILIATION_CONTEXT_KEY);

			// Clear the iata code collection
			Session.Remove(PageSessionKeyConstants.IATA_CODE_COLLECTION);
			Session.Remove(PageSessionKeyConstants.FM_MENU_DATA);

			//***			Session.Remove(TagViewerModel.SessionKey);
		}

		/// <summary>
		/// This method will retrieve the transaction alias collection configured for the
		/// user and put it into session for other tree views to use (accounting, order entry,
		/// supply order entry, and invoice summary).
		/// </summary>
		private void LoadTransactionAliases()
		{
			try
			{
				Session.Remove(FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION);
				if (!security.HasRight(RIGHT.VIEW_TRANSACTION_ALIASES)
					&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_ALIASES)
					&& !security.HasRight(RIGHT.EXECUTE_IMPORT_EXPORT)
					&& !security.HasRight(RIGHT.MODIFY_TRANSACTION_DATA)
					&& !security.HasRight(RIGHT.VIEW_TRANSACTION_DATA)
					&& !security.HasRight(RIGHT.VIEW_BILLS_OF_LADING)
					&& !security.HasRight(RIGHT.CONFIGURE_ACCOUNTING)
					&& !security.HasRight(RIGHT.VIEW_QUERIES)
					&& !security.HasRight(RIGHT.MODIFY_QUERIES)
					&& !security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS)
					&& !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
					&& !Security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.CREATE_ORDERS)
					&& !Security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !Security.HasRight(RIGHT.VIEW_ORDERS)
					&& !Security.HasRight(RIGHT.CREATE_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.MODIFY_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.VIEW_SUPPLY_ORDERS)
					&& !Security.HasRight(RIGHT.CREATE_ORDERS)
					&& !Security.HasRight(RIGHT.MODIFY_ORDERS)
					&& !Security.HasRight(RIGHT.VIEW_ORDERS)
						  )
					return;

				TransactionAliasNameCollectionClass aliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(transactionAliasesChannel =>
				{
					return transactionAliasesChannel.EnumerateNamesOnly(security, true);
				});

				Session.Add(FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION, aliasCollection);
			}
			catch (Exception except)
			{
				Session.Remove(FMMenuEngine.SESSION_FM_MENU_ENGINE_ALIAS_COLLECTION);
				parentForm.ErrorHandler(new Exception("Error in retrieving transaction aliases: " + except.Message));
			}
		}

		/// <summary>
		/// This method retrieves the Menu Bar Left Image URL if configured.  The value is used in the page's
		/// javascript to replace the default image.
		/// </summary>
		private void OverrideMenuBarImage()
		{
			string menuBarLeftImageUrl = ConfigurationManager.AppSettings["MenuBarPageLeftImageUrl"];
			this.MenuLeftImageUrlTB.Text = "EMPTY";

			if (string.IsNullOrEmpty(menuBarLeftImageUrl) == false)
			{
				this.MenuLeftImageUrlTB.Text = menuBarLeftImageUrl;
			}
		}
		#endregion

		#region Event Handlers

		protected override void OnInit(EventArgs e)
		{
			this.sessionWarningTime = "5";
			keyboardAccessibilityEnabled = false;
			/*
			if (Session["Accessibility"] != null)
			{
				UserAccessibilityDO ac = Session["Accessibility"] as UserAccessibilityDO;
				if (ac != null)
				{
					if (ac.Enabled)
					{
						keyboardAccessibilityEnabled = ac.Enabled && ac.EnableKeyboardForMenu;
						if (ac.OutlineFocusedControls)
						{
							menufocuscssurl = "~/MenuBar/css/menu_accessibility_focus.css";
						}
						if (ac.EnableKeyboardForMenu)
						{
							menucssurl = "~/MenuBar/css/menu_accessibility.css";
						}
					}
					this.sessionWarningTime = ac.SessionTimeoutNotificationMinute.ToString();
				}
			}
			*/

			if ((parentForm != null &&
				 parentForm.IsFromClientDispatch == true) ||
				 this.Request.GetQueryOrFormValue("ClientDispatch").DefaultIfNull(string.Empty).Equals(string.Empty) == false)
			{
				this.sessionWarningTime = "-1";
			}

			base.OnInit(e);
		}

		/// <summary>
		/// Creates and initializes controls for the page
		/// </summary>
		/// <param name="sender">unused</param>
		/// <param name="e">unused</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			security = (SecurityClass)Session["Security"];
			if (security == null)
			{
				throw new FMSessionInvalidException();
			}

			licenseNotExpiredAtLogin = (this.Session["LicenseNotExpiredAtLogin"] as string != null && this.Session["LicenseNotExpiredAtLogin"] as string == "true"); //FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());

            csrfToken = security.CSRFToken;

			useDataDictionary = (Session["UseDataDictionary"] == null) || (bool)Session["UseDataDictionary"];

			parentForm = Page as FMFormBase;
			if (parentForm == null)
			{
				throw new ApplicationException("Menu Bar cannot be placed on form that does not derive from FMFormBase");
			}

			// Retrieves the menu bar left image URL if configured.
			this.OverrideMenuBarImage();

			if (parentForm.IsFromClientDispatch)
			{
				this.HideEvenIfNotDialog = true;
			}

			lblLoginUserAndSite.Text = security.UserID;
			lblLoginUserAndSite.ToolTip = security.UserID;

			lblLoginSite.Text = security.LoginSiteID;
			lblLoginSite.ToolTip = security.LoginSiteID;

			if (Session[SESSION_KEY_USER_SITE_GUID] == null)
			{
				UserClass User = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, security.UserGuid));
				Session[SESSION_KEY_USER_SITE_GUID] = User.SiteGuid;
			}


			FMChannelHelper.MakeCall<IDataDictionariesClass>(
					dataDictionariesChannel =>
					{
						if (!security.HasRight(RIGHT.MODIFY_USERS)
						|| (Guid)Session[SESSION_KEY_USER_SITE_GUID] != security.SiteGuid)
						{
							menuSettings.InnerText = (useDataDictionary) ? dataDictionariesChannel.Get(security.SiteGuid, CHANGE_PASSWORD_BUTTON_TEXT) : CHANGE_PASSWORD_BUTTON_TEXT;
							menuSettings.Visible = !security.ActiveDirectoryUser;
						}
						else
						{
							menuSettings.InnerText = (useDataDictionary) ? dataDictionariesChannel.Get(security.SiteGuid, SETTINGS_BUTTON_TEXT) : SETTINGS_BUTTON_TEXT;
						}

						MyProfile.InnerText = (useDataDictionary) ? dataDictionariesChannel.Get(security.SiteGuid, MY_PROFILE_BUTTON_TEXT) : MY_PROFILE_BUTTON_TEXT;

						menuLogout.InnerText = (useDataDictionary) ? dataDictionariesChannel.Get(security.SiteGuid, LOGOUT_BUTTON_TEXT) : LOGOUT_BUTTON_TEXT;
					});


			if (Session[PageSessionKeyConstants.FM_MENU_DATA] == null)
			{
				// Get permissions for transaction aliases
				FMChannelHelper.MakeCall<ISites>(x => x.RefreshTransactionSecurityRightsCache(ref security));
				Session["Security"] = security;

				// Store permissions in Session variable for later use by AccountingTreeNav and others
				this.LoadTransactionAliases();

				// Check to see if the current site is a site group.
				var isSiteGroup = FMChannelHelper.MakeCall<ISites, bool>(x => x.IsSiteGroup(security, security.SiteGuid));

				// Load the menu data from assemblies and database
				List<KeyValuePair<string, Exception>> exceptions;
				Session[PageSessionKeyConstants.FM_MENU_DATA] = FMMenuEngine.LoadMenuData(security, isSiteGroup, useDataDictionary, out exceptions);

				HandleMenuLoadErrors(exceptions);

				if (Session[PageSessionKeyConstants.FM_MENU_DATA] == null)
					throw new ApplicationException("Failed to generate menu data.");
			}

			this.menuData = Session[PageSessionKeyConstants.FM_MENU_DATA] as FMMenuData;

			// For display purposes put the current site in the select.  It will get fully updated by
			// script on page startup.
			var siteSelectInit = "var sSiteSelect = document.getElementById('SiteSelect'); if ( sSiteSelect != null) sSiteSelect.add(new Option('" + security.SiteID + "'));";

			if (this.IsPostBack == false)
			{
				string licenseStatusText = string.Empty;
				string licenseStatusStyle = string.Empty;
            FMHelpers.GetLicenseStatusInfo(security, Session["LicenseDaysLeftToExpire"], Session["LicenseExpirationDate"], out licenseStatusText, out licenseStatusStyle);
            licenseStatus.Style[HtmlTextWriterStyle.Color] = licenseStatusStyle;
				licenseStatus.InnerText = licenseStatusText;

            if (Global.IsFdsIM)
				{
               this.dbVer.Value = this.Session["DatabaseVersion"] as string;
               this.fmVer.Value = this.Session["FuelsManagerVersion"] as string;
					this.bsName.Value = "FCBusinessServices"; 
               this.bsVer.Value = this.Session["FMBusinessServicesVersion"] as string;
               this.ppPath.Value = this.Session["PrivacyPolicyPath"] as string;
               this.webServerName.Value = System.Environment.MachineName.Substring(0, 1) + System.Environment.MachineName.Substring(System.Environment.MachineName.Length - 2);

            }
            else
				{
               this.dbVer.Value = this.Session["DatabaseVersion"] as string;//
               this.fmVer.Value = this.Session["FuelsManagerVersion"] as string;
               this.bsName.Value = "FMBusinessServices";
               this.bsVer.Value = this.Session["FMBusinessServicesVersion"] as string;
               this.ppPath.Value = this.Session["PrivacyPolicyPath"] as string;
               this.webServerName.Value = System.Environment.MachineName;
            }

            string welcomeTitle = ConfigurationManager.AppSettings["LoginPageWelcomeTitle"];
				this.pageTitle.Value = "EMPTY";

				if (string.IsNullOrEmpty(welcomeTitle) == false)
				{
					this.pageTitle.Value = welcomeTitle;
				}
				else
				{
               this.pageTitle.Value = "FuelsManager";
            }

				SetCuiVisability();


                var deferredExecution = siteSelectInit + @"if(typeof MenuStartup !== 'undefined') MenuStartup.LoadjQuery();
				var waitForLoad = function () {
					if (typeof jQuery != 'undefined') {
						if(typeof FMMenu !== 'undefined') {
                            FMMenu.onLoad();
                        }
					} else {
						window.setTimeout(waitForLoad, 500);
					}
				};
				window.setTimeout(waitForLoad, 500);";

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "FMMenu", deferredExecution, true);
			}
			else
			{
				var deferredExecution = siteSelectInit + @"MenuStartup.LoadjQuery();
				var waitForLoad = function () {
					if (typeof jQuery != 'undefined') {
						FMMenu.RepopulateMenu();
					} else {
						window.setTimeout(waitForLoad, 500);
					}
				};
				window.setTimeout(waitForLoad, 500);";

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "FMMenu", deferredExecution, true);
			}

			this.LoadMenuControls();

			var changePasswordscreen = this.Session["ChangePassword"];
			if (security.HasRight(RIGHT.VIEW_OPERATE_ONLY) &&
				(changePasswordscreen == null ||
				(bool)changePasswordscreen != true))
			{
				Session["ViewOperateOnly"] = "1";

				string redirectToOperateLastTime = Session["RedirectToOperate"] as string;

				if (string.IsNullOrEmpty(redirectToOperateLastTime) && operateUrl.Length > 6)
				{
					Session["RedirectToOperate"] = "1";
					Redirect(operateUrl);
				}
				else
				{
					Session["RedirectToOperate"] = string.Empty;
				}
			}
			else if (security.HasRight(RIGHT.VIEW_OPERATE_ONLY) == false) 
			{
				Session["ViewOperateOnly"] = string.Empty;
			}

			if (this.IsPostBack)
			{
				if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "SiteSelect")
				{
					this.SitesComboBox_SelectedIndexChanged(null, null);
					return;
				}

				if (this.Request.GetQueryOrFormValue("__EVENTTARGET") == "lnkAddFavorite")
				{
					this.AddFavorite_Command(null, null);
					return;
				}
			}
			AlertLicenseExpiration();

		}
      private void AlertLicenseExpiration()
      {
         if (IsPostBack)
         {
            return;
         }
         if (this.Request.UrlReferrer == null || this.Request.UrlReferrer.AbsolutePath.ToUpper().Contains("UMNYANGOFORM.ASPX") == false)
         {
            return;
         }
         security = (SecurityClass)Session["Security"];

         if (security == null)
         {
            throw new FMSessionInvalidException();
         }

         DaysLeft = string.Empty;
         this.Session.Remove(AcknowledgedLicenseExpiration);
         if (security.HasRight(RIGHT.ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING))
         {
            bool licenseExpired = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.GetLicenseExpired());
            if (licenseExpired == true)
				{
					return;
				}
            long daysLeft = (long)this.Session["LicenseDaysLeftToExpire"];
            if (daysLeft <= 90)
            {
               AlarmAndEventLogCollectionClass alarmAndEventLogs = new AlarmAndEventLogCollectionClass();
               AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor90DayWarningAck);
               DateTimeOffset current = DateTimeOffset.Now;

               DateTimeOffset beginning = new DateTimeOffset(current.Year, current.Month, current.Day, 0, 0, 0, current.Offset);
               DateTimeOffset ending = new DateTimeOffset(current.Year, current.Month, current.Day, 23, 59, 59, current.Offset);

               string categoryID = string.Empty;
               string priorityID = string.Empty;
               bool includeMemberSites = true;
               bool queryArchiveDb = false;
               bool includeGlobalSites = true;
               int days = 90;

               if (daysLeft <= 30)
               {
                  //check alarm and event log if user acknowledged 30 day license expiration alert
                  days = 30;
                  alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor30DayWarningAck);

               }
               else if (daysLeft <= 60)
               {
                  //check alarm and event log if user acknowledged 60 day license expiration alert
                  days = 60;
                  alarmAndEventLog = new AlarmAndEventLogClass(LicenseExpirationAlarmAndEventDescriptors.alarmEventDescriptorFor60DayWarningAck);
               }

               if (days == 30)
               {
                  ;
               }
               else
               {
                  beginning = beginning.AddDays(daysLeft - days);

               }
               string source = alarmAndEventLog.Source;
               string type = alarmAndEventLog.Alarm ? "Alarm" : "Event";
               string id = alarmAndEventLog.ID;

               Guid siteAdminGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
               SecurityClass innerSecurity = new SecurityClass()
               {
                  UserGuid = security.UserGuid,
                  UserID = security.UserID,
                  SiteGuid = siteAdminGuid
               };
               alarmAndEventLogs = FMChannelHelper.MakeCall<IAlarmAndEventLogs, AlarmAndEventLogCollectionClass>(
                   x => x.Enumerate(innerSecurity,
                           beginning,
                           ending,
                           source,
                           type,
                           id,
                           categoryID,
                           priorityID,
                           includeMemberSites,
                           queryArchiveDb,
                           includeGlobalSites)
                   );
               bool acknowledged = (alarmAndEventLogs.Find((x) => { if (x.CreatedBy == security.UserID && x.Acknowledged == true) return true; else return false; }) != null);
               if (acknowledged == false)
               {
                  DaysLeft = string.Format("{0}", daysLeft);
                  this.Session[AcknowledgedLicenseExpiration] = alarmAndEventLog;
               }
            }
         }
      }

      /// <summary>
      /// Show or hide the appropriate script block based on the
      /// ShowIfDialog property.
      /// </summary>
      /// <param name="e"></param>
      protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			phHideEvenIfNotDialogScript.Visible = HideEvenIfNotDialog;
			phHideIfDialogScript.Visible = !ShowInDialog && !HideEvenIfNotDialog;
			phShowEvenIfDialogScript.Visible = ShowInDialog && !HideEvenIfNotDialog;
		}



        private void SetCuiVisability()
        {
			bool displayCUIDataMark = Global.IsFdsIM || AppSettingsHelper.GetKeyValue<bool>("DisplayCUIDataMark", false);
            cuiTopDiv.Visible = displayCUIDataMark;
            cuiBottomDiv.Visible = displayCUIDataMark;
        }

        /// <summary>
        /// Add the current menu item to the Favorites list, raised by the
        /// "(Add)" item in the Favorites menu
        /// </summary>
        /// <param name="sender">object that raised the event</param>
        /// <param name="e">command arguments</param>
        protected void AddFavorite_Command(object sender, CommandEventArgs e)
		{
			FMMenuItem favoriteMenuItem = null;

			try
			{
				favoriteMenuItem = FMMenuEngine.AddToFavoritesMenu(security, menuData);
			}
			catch (Exception ex)
			{
				parentForm.ErrorHandler(ex);
			}

			if (favoriteMenuItem != null && favoriteMenuItem.MenuItemType != FMMenuItemType.QUICK_LINKS_ADD_QUICK_LINK && favoriteMenuItem.MenuItemType != FMMenuItemType.QUICK_LINKS_CONFIG_QUICK_LINKS)
			{
				// Put it in the Favorites menu!
				Control lnkAddFavorite = phMenu.FindControl("lnkAddFavorite");
				Control ulFav = lnkAddFavorite.Parent.Parent;
				ulFav.Controls.AddAt(ulFav.Controls.Count - 3, CreateMenuItemListItem(favoriteMenuItem));
			}
		}

		/// <summary>
		/// Add the current menu item to the Quick Links bar, raised by the
		/// plus-sign image button on the Quick Links bar
		/// </summary>
		/// <param name="sender">object that raised the event</param>
		/// <param name="e">command arguments</param>
		protected void AddQuickLink_Command(object sender, CommandEventArgs e)
		{
			FMMenuItem quickLinkMenuItem = null;

			try
			{
				quickLinkMenuItem = FMMenuEngine.AddToQuickLinksMenu(security, menuData);
			}
			catch (Exception ex)
			{
				parentForm.ErrorHandler(ex);
			}

			if (quickLinkMenuItem != null)
			{
				// Put it on the Quick Links bar!
				Control ibtnAddQuickLink = phMenu.FindControl("ibtnAddQuickLink");
				Control ulQL = ibtnAddQuickLink.Parent.Parent;
				ulQL.Controls.AddAt(ulQL.Controls.Count, CreateQuickLinkListItem(quickLinkMenuItem));
			}
		}

		/// <summary>
		/// Respond to a change of site by clearing session items and reloading menus
		/// </summary>
		/// <param name="sender">unused sender object</param>
		/// <param name="e">unused event arguments</param>
		protected void SitesComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				var siteGuidText = this.Request.GetQueryOrFormValue("__EVENTARGUMENT");

				var siteGuid = new Guid(siteGuidText);

				security.SiteGuid = siteGuid;

				SiteClass selectedSite = this.GetSiteClass(security, security.SiteGuid, false, false, false);
				security.SiteID = selectedSite.ID;

				// If the selected site is a site group, it goes on the site stack.
				if (selectedSite.SiteGroup)
				{
					// If the site alreay exists in the stack, remove all sites after it.
					var isMultipleSiteKey = (bool)Session["fmSiteSelectMultipleSiteKey"];
					if (isMultipleSiteKey)
					{
						var siteStack = (SiteSelectList)Session["MenuSiteStack"];

						var index = siteStack.FindIndex(x => x.SiteGuid.Equals(selectedSite.SiteGuid));

						// If the site already exists in the stack, remove all sites after it.
						if (index >= 0)
						{
							siteStack.RemoveRange(index + 1, siteStack.Count - index - 1);
						}
						else
						{
							// Otherwise, add the site to the end of the stack.
							var siteInfo = new SiteSelectInfo()
							{
								ID = selectedSite.ID,
								Number = selectedSite.Number,
								SiteGuid = selectedSite.SiteGuid,
								IsSiteGroup = selectedSite.SiteGroup
							};

							siteStack.Add(siteInfo);
						}
					}
				}

				// Make sure the user is assigned to the site
				if (UserMappedToSite())
				{
					// Reset rights collection based on groups at the selected site
					security.RightCollection = FMChannelHelper.MakeCall<IRights, RightCollectionClass>(
						rightsChannel => rightsChannel.EnumerateByUserBySite(security, security.UserGuid, security.SiteGuid));
				}
				else
				{
					security.RightCollection = new RightCollectionClass();
				}


				Session.Add("Security", security);
				FMChannelHelper.MakeCall<ISites>(
					 service =>
					 {
						 service.ModifySecurity(security.Token.ToString(), security);
					 });

				// Add the session item "ResetTabularViewSessionOperation" only if it doesn't already exist
				var operation = (string)this.Session["ResetTabularViewSessionOperation"];
				if (string.IsNullOrWhiteSpace(operation))
				{
					this.Session.Add("ResetTabularViewSessionOperation", "SiteChange");
				}

				this.ClearSessionItems();

				this.Redirect(FuelsManagerHomePageUrl);
			}
			catch (Exception except)
			{
				parentForm.ErrorHandler(except);
			}
		}

		protected string GetHelpLinkScript()
		{
			// Load help mappings if need be
			if (this.Session["HelpMappingDictionary"] == null)
			{
				this.Session["HelpMappingDictionary"] = FMChannelHelper.MakeCall<IHelpMappings, HelpMappingDictionary>(helpMappingsChannel =>
				{
					return helpMappingsChannel.GetDictionary(this.security);

				});
			}
			try
			{
				var helpMappingDict = this.Session["HelpMappingDictionary"] as HelpMappingDictionary;
				Dictionary<string, string> helpMapping = new Dictionary<string, string>();

				foreach (string key in this.parentForm.GetHelpContextKeys())
				{
					if (helpMappingDict != null)
					{
						if (!string.IsNullOrEmpty(helpMappingDict.GetHelpPage(key)))
						{

							helpMapping.Add(
								 key,
								 this.menuData.GetHelpUrl(this.useDataDictionary) + "/index.htm#t="
								 + helpMappingDict.GetHelpPage(key));
						}
					}
				}

				string startingKey = "";
				string helpMappingJs = "var HelpMapping = {}; ";
				foreach (var helpMap in helpMapping)
				{
					helpMappingJs += "HelpMapping[\"" + helpMap.Key + "\"]=\"" + helpMap.Value + "\"; ";
					if (String.IsNullOrEmpty(startingKey))
					{
						startingKey = helpMap.Key;
					}
				}

				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HelpMapping", helpMappingJs, true);
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "HelpKey", "var CurrentHelpKey =\"" + startingKey + "\";", true);
				this.Page.ClientScript.RegisterStartupScript(this.GetType(), "OverrideKey", "if (typeof OverrideKey !== 'undefined') {CurrentHelpKey = OverrideKey;};", true);

				//string HelpURL = menuData.GetHelpUrl(useDataDictionary) + "/index.htm#t=" + ((HelpMappingDictionary)Session["HelpMappingDictionary"]).GetHelpPage(parentForm.GetHelpContextKey());
				string script = "window.open(HelpMapping[CurrentHelpKey], \"FMHelpWin\", \"menubar=no,location=no,resizable=yes,scrollbars=yes,status=yes,width=700,height=800\"); return false;";
				return script;
			}

			catch
			{
				return "FMLayout.Alert('Could not load the help file for this page.', 'FMHelp Error');return false;";
			}
		}


		static public string GetRandomLengthString()
		{

			byte[] rndBytes = new byte[256 + 17];
			var rnd = new System.Security.Cryptography.RNGCryptoServiceProvider();
			rnd.GetBytes(rndBytes);
			int s = rndBytes[0] + 16;
			var rndTokenSb = new System.Text.StringBuilder(string.Empty);
			for (int i = 1; i < s; i++)
			{
				byte b = rndBytes[i];
				rndTokenSb.Append(b.ToString("X2", System.Globalization.CultureInfo.InvariantCulture));
			}
			return rndTokenSb.ToString();
		}

		protected void RedirectToUserSettings(object sender, System.EventArgs e)
		{
			this.Session[UserForm.SessionKeyUserGuid] = security.UserGuid;
			UserClass User = FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.Get(security, security.UserGuid));
			if (!security.HasRight(RIGHT.MODIFY_USERS)
					|| User.SiteGuid.IsNotEmptyAndNotEqualTo(security.SiteGuid))
			{
				this.Redirect(FmWebAppRelativeUrlPrefix + "ChangePasswordForm.aspx?FromApplication=true&FromOperate=false");
			}
			else
			{
				this.Redirect(FmWebAppRelativeUrlPrefix + "UserForm.aspx");
			}
		}
        protected void RedirectToMyProfile(object sender, System.EventArgs e)
        {
            this.Redirect(ResolveUrl("~/MenuBar/FMMenuBar.aspx?target=../UserAdministrationArea/UserConfiguration/UserConfigurationView"));
        }


        protected string GetScriptToOpenInSeparateTab(string url)
		{
			return "window.open(" + url + "); return false;";
		}

		#endregion
	}
}