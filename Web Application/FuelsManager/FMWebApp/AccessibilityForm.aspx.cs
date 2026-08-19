// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessibilityForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the AccessibilityForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Web.UI.WebControls;
    using System.Collections.Generic;

	using AjaxControlToolkit;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using global::FMWebApp;

	/// <summary>
	///    Code behind for AccessibilityForm.
	/// </summary>
	public partial class AccessibilityForm : FMAutoSubmitFormBase, IDataDictionary, IMenuDiscovery
	{

		#region Public Properties
		//public Guid userGuid = Guid.Empty;
		public AccessibilityCollectionClass Accessibilities = null;
		public List<string> VersionSpecificFields = null;
		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the company form.
		/// </summary>
		/// <param name="enable">
		///    if set to <c>true</c> [enable].
		/// </param>
		public void EnableControls(bool enable)
		{
			this.OK.Enabled = enable;
			this.Cancel.Enabled = enable;

		}

		/// <summary>
		///    Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.AccessibilityGeneralPageTab.UpdateData();
		}

		#endregion

		#region Explicit Interface Methods

		/// <summary>
		///    Identifies the data dictionary keys needed for this item.
		/// </summary>
		/// <param name="security">The current security object.</param>
		/// <returns>
		///    An array of data dictionary keys.
		/// </returns>
		string[] IDataDictionary.Keys(SecurityClass security)
		{
			string[] keys = { "Contacts", "Groups", "User Data", "Certificates & Permits", "Company Configuration", "Access Schedule" };

			return keys;
		}

		#endregion

		#region Methods
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
		{
			if (useNewLicenseKey == 1)
			{

			}
			else
			{
				// Depends Upon Shared Components Config
				if ((options & 0x4000) == 0)
				{
					return null;
				}
			}

			if ((security.HasRight(RIGHT.MODIFY_USERS) == false) && (security.UserGuid != security.UserGuid))
			{
				return null;
			}

			var items = new List<FMMenuItem>();

			items.Add(new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_OTHER_ACCESSIBILITY,
				RootMenuName = "Configuration",
				CategoryName = "Other",
				ItemName = "Accessibility",
				NavigateUrl = "AccessibilityForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.Apply
			});

			return items;
		}
		/// <summary>
		///    Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					var userGuid = this.Security.UserGuid;
					if (this.Request.Params["UserGuid"] != null)
					{
						if (!Guid.TryParse(this.Request.Params["UserGuid"], out userGuid))
						{
							userGuid = this.Security.UserGuid;

						}
					}
					Session["AccessibilityList"] = this.AccessibilityGeneralPageTab.EnumerateAccessibility(userGuid);

				}
				Accessibilities = Session["AccessibilityList"] as AccessibilityCollectionClass;
			}
			catch (FMSessionInvalidException ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		/// <exception cref="System.Exception">CompanyArrayList not in session</exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{

				// General and Contacts are always enabled
	////			this.tpGeneralPage.Visible = true;
	////			this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}


			/// <summary>
		///    Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.TransferToOriginatingForm();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
			finally
			{
				Session.Remove("AccessibilityList");
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		}



		/// <summary>
		///    Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				var userGuid = Guid.Empty;
				this.UpdateData();

				foreach (AccessibilityClass accessibility in Accessibilities)
				{
					userGuid = accessibility.UserGuid;
					AccessibilityClass oldAccessibility = FMChannelHelper.MakeCall<IAccessibilities, AccessibilityClass>(accessibilities => accessibilities.Get(this.Security, accessibility.UserGuid, accessibility.SettingKey));
					if (oldAccessibility.IdentityGuid == Guid.Empty)
					{
						FMChannelHelper.MakeCall<IAccessibilities>(accessibilities => accessibilities.Add(this.Security, accessibility));

					}
					else
					{
						FMChannelHelper.MakeCall<IAccessibilities>(accessibilities => accessibilities.Modify(this.Security, accessibility));
					}
				}
				if (this.Security.UserGuid == userGuid)
				{
					var ac = new UserAccessibilityDO(this.Security, userGuid); 
					this.Session["Accessibility"] = ac; 
				}

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}
			finally
			{
				Session.Remove("AccessibilityList");
			}

			this.TransferToOriginatingForm();
		}

		/// <summary>
		///    Transfers to originating form.
		/// </summary>
		private void TransferToOriginatingForm()
		{
			this.Session.Remove("AccessibilityList");

			this.Redirect("FuelsManagerForm.aspx");

		}

		#endregion
	}

	/// <summary>
	///    Page base for gaining access to company object
	/// </summary>
	public class AccessibilityPageBase : FMUserControlBase
	{
		#region Properties

		/// <summary>
		///    Gets the company.
		/// </summary>
		/// <value>
		///    The company.
		/// </value>
		protected AccessibilityCollectionClass Accessibilities
		{
			get
			{
				if (this.Page is AccessibilityForm)
				{
					return ((AccessibilityForm)this.Page).Accessibilities;
				}
				else
				{
					return Session["AccessibilityList"] as AccessibilityCollectionClass;
				}
			}
		}

		protected List<string> VersionSpecificFields
		{
			get
			{
				return ((AccessibilityForm)this.Page).VersionSpecificFields;
			}
		}

		#endregion
	}
}