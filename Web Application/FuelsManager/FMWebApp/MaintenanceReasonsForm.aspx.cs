// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MaintenanceReasonsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MaintenanceReasonsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	public partial class MaintenanceReasonsForm : FMFormBase, IMenuDiscovery, IEntityDiscovery
	{
		#region Explicit Interface Properties
		bool IEntityDiscovery.EntityAssignable
		{
			get { return true; }
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get { return typeof(IMaintenanceReasons); }
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get { return ENTITY_TYPE.MAINTENANCE_REASON; }
		}
		#endregion

		#region Initialization

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.EnableControls(true);

					// Load current rows from database table into a collection, and put collection in session.
					this.Session["MaintenanceReasonCollection"] =
						FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonCollectionClass>(x => x.EnumerateBySite(this.Security));

					this.UpdateViewFromCollection();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		protected void EnableControls(bool bEnable)
		{
			this.GetSecurity();

			if (!this.Security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD))
			{
				this.AddButtonTop.Enabled = false;
				this.AddButtonBottom.Enabled = false;
			}
			else
			{
				this.AddButtonTop.Enabled = bEnable;
				this.AddButtonBottom.Enabled = bEnable;
				this.PageSizeDropDown.Enabled = bEnable;
			}
		}

		protected void UpdateViewFromCollection()
		{
			try
			{
				DataView data = this.EnumerateMaintenanceReasons();

				if (this.PageSizeDropDown != null)
				{
					this.PageSizeDropDown.SetPageSize(this.MaintenanceReasonsDataGrid, data.Count);
				}

				this.MaintenanceReasonsDataGrid.DataSource = data;
				this.MaintenanceReasonsDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		// The columns here must match as assigned in aspx file.
		// <!-- Column 0 - Edit, Update, and Cancel buttons -->
		// <!-- Column 1 - Delete button -->
		// <!-- Column 2 - SiteGuid (hidden) -->
		// <!-- Column 3 - Zero-based item number in this grid - not MaintenanceReasonGuid (hidden) -->
		// <!-- Column 4 - ID -->
		// <!-- Column 5 - Description -->
		private DataView EnumerateMaintenanceReasons()
		{
			var maintenanceReasonCollection = (MaintenanceReasonCollectionClass)this.Session["MaintenanceReasonCollection"];

			var mapDataTable = new DataTable();
			mapDataTable.Columns.Add("Index", typeof(Int32));
			mapDataTable.Columns.Add("SiteGuid", typeof(Guid));
			mapDataTable.Columns.Add("ID", typeof(string));
			mapDataTable.Columns.Add("Description", typeof(string));

			for (int iItem = 0; iItem < maintenanceReasonCollection.Count; iItem++)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				MaintenanceReasonClass maintenanceReason = maintenanceReasonCollection[iItem];
				mapDataRow["Index"] = iItem;
				mapDataRow["SiteGuid"] = maintenanceReason.SiteGuid;
				mapDataRow["ID"] = maintenanceReason.ID;
				mapDataRow["Description"] = maintenanceReason.Description;

				mapDataTable.Rows.Add(mapDataRow);
			}

			return new DataView(mapDataTable);
		}

		#region Other control message handlers
		protected void AddButtonClick(object sender, EventArgs e)
		{
			try
			{
				var oMaintenanceReason = new MaintenanceReasonClass { SiteGuid = this.Security.SiteGuid };
				var maintenanceReasonCollection = (MaintenanceReasonCollectionClass)this.Session["MaintenanceReasonCollection"];
				maintenanceReasonCollection.Add(oMaintenanceReason);

				this.MaintenanceReasonsDataGrid.CurrentPageIndex = (maintenanceReasonCollection.Count - 1)
				                                                   / this.MaintenanceReasonsDataGrid.PageSize;
				this.MaintenanceReasonsDataGrid.EditItemIndex = (maintenanceReasonCollection.Count - 1)
				                                                % this.MaintenanceReasonsDataGrid.PageSize;

				this.EnableControls(false);
				this.UpdateViewFromCollection();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			try
			{
				this.UpdateViewFromCollection();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}
		#endregion

		#region Grid Message Handlers
		protected void MaintenanceReasonsDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					var maintenanceReasonCollection = (MaintenanceReasonCollectionClass)this.Session["MaintenanceReasonCollection"];

					int nGridItemNum = Convert.ToInt32(indexLabel.Text);
					MaintenanceReasonClass maintenanceReason = maintenanceReasonCollection[nGridItemNum];

					if (this.MaintenanceReasonsDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.MaintenanceReasonsDataGrid.EditItemIndex = -1;
					}
					else if (this.MaintenanceReasonsDataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.MaintenanceReasonsDataGrid.EditItemIndex--;
					}

					bool isMaintReasonUsed = FMChannelHelper.MakeCall<IEquipmentMaintenanceLogs, bool>(
							x => x.IsMaintenanceReasonUsed(this.Security, maintenanceReason.IdentityGuid));

					if (isMaintReasonUsed)
					{
						throw new Exception(
							"Cannot Delete Maintenance Reason because it is associated with equipment maintenance records.");
					}

					isMaintReasonUsed = FMChannelHelper.MakeCall<ITankMaintenanceLogs, bool>(
							x => x.IsMaintenanceReasonUsed(this.Security, maintenanceReason.IdentityGuid));

					if (isMaintReasonUsed)
					{
						throw new Exception(
							"Cannot Delete Maintenance Reason because it is associated with tank maintenance records.");
					}

					// Non Empty IdentityGuid indicates MaintenanceReason has been committed to database.
					if (maintenanceReason.IdentityGuid != Guid.Empty)
					{
						this.GetSecurity();

						FMChannelHelper.MakeCall<IMaintenanceReasons>(x => x.Purge(this.Security, maintenanceReason.IdentityGuid));
					}

					maintenanceReasonCollection.RemoveAt(nGridItemNum);

					if (this.MaintenanceReasonsDataGrid.Items.Count == 1 && this.MaintenanceReasonsDataGrid.CurrentPageIndex > 0)
					{
						this.MaintenanceReasonsDataGrid.CurrentPageIndex--;
					}

					this.UpdateViewFromCollection();
				}
			}
			catch (SqlException)
			{
				this.ErrorHandler(new Exception("Cannot Delete Maintenance Reason Due to Dependencies"));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void MaintenanceReasonsDataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.MaintenanceReasonsDataGrid.EditItemIndex > -1)
				{
					return;
				}

				this.MaintenanceReasonsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateViewFromCollection();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void MaintenanceReasonsDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.MaintenanceReasonsDataGrid.EditItemIndex = e.Item.ItemIndex;
				this.UpdateViewFromCollection();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		protected void MaintenanceReasonsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			try
			{
				// Need to disable the edit and delete buttons when the user does not
				// have the appropriate rights.
				if (e.Item.ItemIndex != -1)
				{
					var deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
					var editButton = (LinkButton)e.Item.FindControl("EditButton");

					if (deleteButton != null)
					{
						if (!this.Security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
							|| this.Security.SiteGuid != (Guid)((DataRowView) e.Item.DataItem)["SiteGuid"])
						{
							deleteButton.Enabled = false;
						}
					}

					if (editButton != null)
					{
						if (!this.Security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
							|| this.Security.SiteGuid != (Guid) ((DataRowView) e.Item.DataItem)["SiteGuid"])
						{
							editButton.Enabled = false;
						}
					}
				}
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}

		protected void MaintenanceReasonsDataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					var maintenanceReasonCollection = (MaintenanceReasonCollectionClass)this.Session["MaintenanceReasonCollection"];

					MaintenanceReasonClass maintenanceReason = maintenanceReasonCollection[Convert.ToInt32(indexLabel.Text)];

					var idTextBox = (TextBox)e.Item.FindControl("IDTextBox");
					var descriptionTextBox = (TextBox)e.Item.FindControl("DescriptionTextBox");

					this.GetSecurity();

					maintenanceReason.SiteGuid = this.Security.SiteGuid;
					maintenanceReason.ID = idTextBox.Text;
					maintenanceReason.Description = descriptionTextBox.Text;

					if (maintenanceReason.IdentityGuid == Guid.Empty)
					{
						maintenanceReason.IdentityGuid =
							FMChannelHelper.MakeCall<IMaintenanceReasons, Guid>(x => x.Add(this.Security, maintenanceReason));
					}
					else
					{
						FMChannelHelper.MakeCall<IMaintenanceReasons>(x => x.Modify(this.Security, maintenanceReason));
					}

					var maintenanceReasonsDataGrid = (DataGrid)source;
					maintenanceReasonsDataGrid.EditItemIndex = -1;

					this.EnableControls(true);
					this.UpdateViewFromCollection();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void MaintenanceReasonsDataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var maintenanceReasonCollection = (MaintenanceReasonCollectionClass)this.Session["MaintenanceReasonCollection"];

				int index = this.MaintenanceReasonsDataGrid.CurrentPageIndex * this.MaintenanceReasonsDataGrid.PageSize
				            + e.Item.ItemIndex;
				MaintenanceReasonClass maintenanceReason = maintenanceReasonCollection[index];

				if (maintenanceReason.IdentityGuid == Guid.Empty)
				{
					maintenanceReasonCollection.RemoveAt(index);

					if (this.MaintenanceReasonsDataGrid.Items.Count == 1 && this.MaintenanceReasonsDataGrid.CurrentPageIndex > 0)
					{
						this.MaintenanceReasonsDataGrid.CurrentPageIndex--;
					}
				}
				else
				{
					this.Session["MaintenanceReasonCollection"] =
						FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonCollectionClass>(x => x.EnumerateBySite(this.Security));
				}

				this.MaintenanceReasonsDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateViewFromCollection();
			}
			catch ( Exception except )
			{
				this.ErrorHandler( except );
			}
		}
		#endregion

		#region Web Form Designer generated code
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion

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
                // Depends upon the Maintenance hardware key.
                if ((options & 0x4000000) == 0)
                {
                    return null;
                }
            }
            List<FMMenuItem> items = null;

			if (security.HasRight(RIGHT.ADD_MAINTENANCE_RECORD)
			    || security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
			    || security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD))
			{
				items = new List<FMMenuItem>
				        {
					        new FMMenuItem
					        {
						        MenuItemType		= FMMenuItemType.ASSETS_EQUIPMENT_MAINTENANCE_REASONS,
						        RootMenuName		= "Assets",
						        CategoryName		= "Equipment",
						        ItemName			= "Maintenance Reasons",
						        NavigateUrl			= "MaintenanceReasonsForm.aspx",
						        ApplyDataDictionary = ApplyDataDictionary.Apply
					        }
				        };
			}

			return items;
		}

		#region IEntityDiscovery
		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass security, 
																			ENTITY_ASSIGNMENT_TYPE entityAssignmentType)
		{
			MaintenanceReasonCollectionClass maintenanceReasonCollection =
				FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonCollectionClass>(x => x.EnumerateBySite(security));

			var reasonCodeToSiteMapList = new EntityToSiteMapCollectionClass();

			foreach (MaintenanceReasonClass maintenanceReason in maintenanceReasonCollection)
			{

				if (entityAssignmentType == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if ((security.SiteGuid == maintenanceReason.SiteGuid)
						|| (security.LoginSiteGuid != maintenanceReason.SiteGuid))
					{
						continue;
					}
				}
				else
				{
					if (security.SiteGuid != maintenanceReason.SiteGuid)
					{
						continue;
					}
				}

				reasonCodeToSiteMapList.Add(new EntityToSiteMapClass(maintenanceReason));
			}

			return reasonCodeToSiteMapList;
		}

		/// <summary>
		/// This method will return the Maintenance Reason GUID for the 
		/// given maintenance reason ID.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="id">The Maintenance Reason ID></param>
		/// <returns>Returns the Maintenance Reason GUID.</returns>
		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string id)
		{
			return FMChannelHelper.MakeCall<IMaintenanceReasons, Guid>(x => x.GetIdentityGuid(security, id));
		}

		/// <summary>
		/// This method will set the site GUID for the Maintenance Reason.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="maintenanceReasonGuid">The maintenance reason GUID.</param>
		/// <param name="siteGuid">The site GUID.</param>
		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid maintenanceReasonGuid, Guid siteGuid)
		{
			MaintenanceReasonClass maintenanceReason =
				FMChannelHelper.MakeCall<IMaintenanceReasons, MaintenanceReasonClass>(x => x.Get(security, maintenanceReasonGuid));

			maintenanceReason.SiteGuid = siteGuid;

			FMChannelHelper.MakeCall<IMaintenanceReasons>(x => x.Modify(security, maintenanceReason));
		}
		#endregion
	}
}
