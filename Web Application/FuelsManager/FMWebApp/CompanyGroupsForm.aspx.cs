// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyGroupsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyGroupsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for CompanyGroupsForm.
	/// </summary>
	public partial class CompanyGroupsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
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
				return typeof(ICompanyGroups);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.COMPANY_GROUP;
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
                if ((options & 0x80100) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_COMPANY_DATA) && !security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.ACCOUNTING_COMPANIES_GROUPS,
					RootMenuName = "Accounting",
					CategoryName = "Companies",
					ItemName = "Company Groups",
					NavigateUrl = "CompanyGroupsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply,
					SortOrder = 2
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			CompanyGroupCollectionClass CompanyGroupCollection;
			CompanyGroupCollection = FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupCollectionClass>(
																	 x =>
																	 x.Enumerate(Security)
																);

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (CompanyGroupClass CompanyGroup in CompanyGroupCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == CompanyGroup.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != CompanyGroup.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != CompanyGroup.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(CompanyGroup);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<ICompanyGroups, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security,ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			CompanyGroupClass CompanyGroup = FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupClass>(
																	 x =>
																	 x.Get(security,guid)
																);

			CompanyGroup.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<ICompanyGroups>(
																	 x =>
																	 x.Modify(security,CompanyGroup)
																);
		}

		#endregion

		#region Methods

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
					if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["CompanyGroupsPage"] != null)
					{
						this.CompanyGroupsDataGrid.CurrentPageIndex = (int)this.Session["CompanyGroupsPage"];
						this.Session.Remove("CompanyGroupsPage");
					}
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("IdentityGuid");
			this.Session["CompanyGroupsPage"] = this.CompanyGroupsDataGrid.CurrentPageIndex;
			this.Redirect("CompanyGroupForm.aspx");
		}

		private void CompanyGroupsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Identity Guid
				TableCell guidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<ICompanyGroups>(
																	 x =>
																	 x.Purge(this.Security, Guid.Parse(guidCell.Text))
																);

				this.CompanyGroupsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.CompanyGroupsDataGrid.Items.Count == 1 && this.CompanyGroupsDataGrid.CurrentPageIndex > 0)
				{
					this.CompanyGroupsDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CompanyGroupsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell guidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = guidCell.Text;
			this.Session["CompanyGroupsPage"] = this.CompanyGroupsDataGrid.CurrentPageIndex;
			this.Redirect("CompanyGroupForm.aspx");
		}

		private void CompanyGroupsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (DeleteButton != null)
			{
				TableCell SiteGuidCell = e.Item.Cells[1];//bds

				bool bEnabled = this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
				                && (this.Security.SiteGuid == Guid.Parse(SiteGuidCell.Text));

				DeleteButton.Enabled = bEnabled;
			}
		}

		private void CompanyGroupsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.CompanyGroupsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.CompanyGroupsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private ICollection EnumerateCompanyGroups()
		{
			CompanyGroupCollectionClass CompanyGroupCollection = FMChannelHelper.MakeCall<ICompanyGroups, CompanyGroupCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			var CompanyGroupDataTable = new DataTable();
			DataRow CompanyGroupDataRow;

			CompanyGroupDataTable.Columns.Add("SiteGuid", typeof(Guid));
			CompanyGroupDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			CompanyGroupDataTable.Columns.Add("ID", typeof(string));

			foreach (CompanyGroupClass CompanyGroup in CompanyGroupCollection)
			{
				CompanyGroupDataRow = CompanyGroupDataTable.NewRow();

				CompanyGroupDataRow["SiteGuid"] = CompanyGroup.SiteGuid;
				CompanyGroupDataRow["IdentityGuid"] = CompanyGroup.IdentityGuid;
				CompanyGroupDataRow["ID"] = CompanyGroup.ID;

				CompanyGroupDataTable.Rows.Add(CompanyGroupDataRow);
			}
			var CompanyGroupDataView = new DataView(CompanyGroupDataTable);
			return CompanyGroupDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += this.AddButton_Command;
			this.CompanyGroupsDataGrid.EditCommand += this.CompanyGroupsDataGrid_EditCommand;
			this.CompanyGroupsDataGrid.PageIndexChanged += this.CompanyGroupsDataGrid_PageIndexChanged;
			this.CompanyGroupsDataGrid.DeleteCommand += this.CompanyGroupsDataGrid_DeleteCommand;
			this.CompanyGroupsDataGrid.ItemDataBound += this.CompanyGroupsDataGrid_ItemDataBound;
			this.AddButton.Command += this.AddButton_Command;
		}

		private void UpdateView()
		{
			ICollection Groups = this.EnumerateCompanyGroups();

			this.CompanyGroupsFormPageSizeDropDown.SetPageSize(this.CompanyGroupsDataGrid, Groups.Count);

			this.CompanyGroupsDataGrid.DataSource = Groups;
			this.CompanyGroupsDataGrid.DataBind();
		}

		#endregion
	}
}