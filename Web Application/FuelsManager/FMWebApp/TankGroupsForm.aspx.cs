// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TankGroupsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TankGroupsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for TankGroupsForm.
	/// </summary>
	public partial class TankGroupsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
	{
		#region Explicit Interface Properties

		bool IEntityDiscovery.EntityAssignable
		{
			get
			{
				return false;
			}
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(ITankGroups);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.TANK_GROUP;
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
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                // Depends upon Load Rack Service
                if ((options & 0x8000) == 0)
                {
                    return null;
                }

                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			// Site Groups don't have Tanks
			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_TANK_DATA) && !security.HasRight(RIGHT.MODIFY_TANK_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_TANK_GROUPS,
						RootMenuName = "Assets",
						CategoryName = "Equipment",
						ItemName = "Tank Groups",
						NavigateUrl = "TankGroupsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			TankGroupCollectionClass TankGroupCollection;
			TankGroupCollection = FMChannelHelper.MakeCall<ITankGroups, TankGroupCollectionClass>(
																	 x =>
																	 x.Enumerate(Security)
																);

			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (TankGroupClass TankGroup in TankGroupCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == TankGroup.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != TankGroup.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != TankGroup.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(TankGroup);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}

			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<ITankGroups, Guid>(
																	 x =>
																	 x.GetIdentityGuid(security, ID)
																);
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			TankGroupClass TankGroup = FMChannelHelper.MakeCall<ITankGroups, TankGroupClass>(
														x =>
														x.Get(security, guid)
												);

			TankGroup.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<ITankGroups>(
																	 x =>
																	 x.Modify(security, TankGroup)
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
					if (!this.Security.HasRight(RIGHT.MODIFY_TANK_DATA))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["TankGroupsPage"] != null)
					{
						this.TankGroupsDataGrid.CurrentPageIndex = (int)this.Session["TankGroupsPage"];
						this.Session.Remove("TankGroupsPage");
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
			this.Session["TankGroupsPage"] = this.TankGroupsDataGrid.CurrentPageIndex;
			this.Redirect("TankGroupForm.aspx");
		}

		private ICollection EnumerateTankGroups()
		{
			TankGroupCollectionClass TankGroupCollection = FMChannelHelper.MakeCall<ITankGroups, TankGroupCollectionClass>(
																				 x =>
																				 x.Enumerate(this.Security)
																			);

			var TankGroupDataTable = new DataTable();
			DataRow TankGroupDataRow;

			TankGroupDataTable.Columns.Add("SiteGuid", typeof(Guid));
			TankGroupDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			TankGroupDataTable.Columns.Add("ID", typeof(string));

			foreach (TankGroupClass TankGroup in TankGroupCollection)
			{
				TankGroupDataRow = TankGroupDataTable.NewRow();

				TankGroupDataRow["SiteGuid"] = TankGroup.SiteGuid;
				TankGroupDataRow["IdentityGuid"] = TankGroup.IdentityGuid;
				TankGroupDataRow["ID"] = TankGroup.ID;

				TankGroupDataTable.Rows.Add(TankGroupDataRow);
			}
			var TankGroupDataView = new DataView(TankGroupDataTable);
			return TankGroupDataView;
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.TankGroupsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TankGroupsDataGrid_EditCommand);
			this.TankGroupsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.TankGroupsDataGrid_PageIndexChanged);
			this.TankGroupsDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.TankGroupsDataGrid_DeleteCommand);
			this.TankGroupsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.TankGroupsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}

		private void TankGroupsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				try
				{
					ILoadRackManager LoadRackManager = this.GetLoadRackManager();
					LoadRackManager.Purge(this.Security, typeof(TankGroupClass), Guid.Parse(identityGuidCell.Text));
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}

				FMChannelHelper.MakeCall<ITankGroups>(
																	 x =>
																	 x.Purge(this.Security, Guid.Parse(identityGuidCell.Text))
																);

				this.TankGroupsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.TankGroupsDataGrid.Items.Count == 1 && this.TankGroupsDataGrid.CurrentPageIndex > 0)
				{
					this.TankGroupsDataGrid.CurrentPageIndex--;
				}
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void TankGroupsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell identityGuidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["TankGroupsPage"] = this.TankGroupsDataGrid.CurrentPageIndex;
			this.Redirect("TankGroupForm.aspx");
		}

		private void TankGroupsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			if (DeleteButton != null)
			{
				TableCell siteGuidCell = e.Item.Cells[1];//bds
				if (!this.Security.HasRight(RIGHT.MODIFY_TANK_DATA)
				    || this.Security.SiteGuid != Guid.Parse(siteGuidCell.Text))
				{
					DeleteButton.Enabled = false;
					DeleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}

		private void TankGroupsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.TankGroupsDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.TankGroupsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection TankGroups = this.EnumerateTankGroups();

			this.TankGroupsFormPageSizeDropDown.SetPageSize(this.TankGroupsDataGrid, TankGroups.Count);

			this.TankGroupsDataGrid.DataSource = TankGroups;
			this.TankGroupsDataGrid.DataBind();
		}

		#endregion
	}
}