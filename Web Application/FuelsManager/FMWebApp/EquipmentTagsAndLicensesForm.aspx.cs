// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentTagsAndLicensesForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentTagsAndLicensesForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for EquipmentTagsAndLicensesForm.
	/// </summary>
	public partial class EquipmentTagsAndLicensesForm : QualificationsFormBase,
	                                                    IEntityDiscovery,
	                                                    IMenuDiscovery
	{
		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return true;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IQualifications);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.QUALIFICATION_EQUIPMENT_TAG_AND_LICENSE;
			}
		}

		#endregion

		#region Properties

		protected override DataGrid ApplicationDataGrid
		{
			get
			{
				return this.QualificationsDataGrid;
			}
		}

		protected override QUALIFICATION_TYPE QualificationType
		{
			get
			{
				return QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		///    List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Accounting
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_LICENSES,
					RootMenuName = "Assets",
					CategoryName = "Equipment",
					ItemName = "Licenses",
					NavigateUrl = "EquipmentTagsAndLicensesForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods


		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, ENTITY_ASSIGNMENT_TYPE type)
		{
			QualificationCollectionClass qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
				qualifications => qualifications.EnumerateByType(security, this.QualificationType));

			var entityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (QualificationClass qualification in qualificationCollection)
			{
				if (type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (security.SiteGuid == qualification.SiteGuid)
					{
						continue;
					}

					if (security.LoginSiteGuid != qualification.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != qualification.SiteGuid)
					{
						continue;
					}
				}

				var entityToSiteMap = new EntityToSiteMapClass(qualification);
				entityToSiteMapCollection.Add(entityToSiteMap);
			}

			return entityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IQualifications, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, this.QualificationType, ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			QualificationClass qualification = FMChannelHelper.MakeCall<IQualifications, QualificationClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			qualification.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IQualifications>(
																	 x =>
																	 x.Modify(security, qualification)
																);
		}

		#endregion

		#region Methods

		protected override void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.EquipmentTagsFormPageSizeDropDown.Enabled = enable;
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					// Enumerate 
					var qualificationCollection = FMChannelHelper.MakeCall<IQualifications, QualificationCollectionClass>(
																	 x =>
																	 x.EnumerateByType(this.Security, this.QualificationType)
																);

					this.Session["QualificationCollection"] = qualificationCollection;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected override void UpdateView()
		{
			this.UpdateView(this.EquipmentTagsFormPageSizeDropDown);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
			this.QualificationsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.QualificationsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.QualificationsDataGrid.CancelCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.QualificationsDataGrid.UpdateCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.QualificationsDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.QualificationsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
		}

		#endregion
	}
}