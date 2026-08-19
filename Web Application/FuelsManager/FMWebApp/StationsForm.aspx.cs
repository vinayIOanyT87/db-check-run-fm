// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationsForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Net.Sockets;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    Summary description for StationsForm.
	/// </summary>
	public partial class StationsForm : FMFormBase, IEntityDiscovery, IMenuDiscovery
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
				return typeof(IStations);
			}
		}

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.STATION;
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
                // Depends Upon Load Rack
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			//// Site Groups don't have Stations
			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
			    && !security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_EQUIPMENT_DATA) && !security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
			{
				return null;
			}


			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.ASSETS_EQUIPMENT_STATION,
						RootMenuName = "Assets",
						CategoryName = "Equipment",
						ItemName = "Stations",
						NavigateUrl = "StationsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Explicit Interface Methods

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(
			SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			StationCollectionClass StationCollection;
			StationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(
																	 x =>
																	 x.Enumerate(Security)
																);


			var EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (StationClass Station in StationCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == Station.SiteGuid)
					{
						continue;
					}

					if (Security.LoginSiteGuid != Station.SiteGuid)
					{
						continue;
					}
				}
				else
				{
					if (Security.SiteGuid != Station.SiteGuid)
					{
						continue;
					}
				}

				var EntityToSiteMap = new EntityToSiteMapClass(Station);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}

			return EntityToSiteMapCollection;
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return Guid.Empty;
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			StationClass Station = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(security, guid)
																);

			Station.SiteGuid = SiteGuid;
			FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Modify(security, Station)
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
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					if (this.Session["StationsPage"] != null)
					{
						this.StationsDataGrid.CurrentPageIndex = (int)this.Session["StationsPage"];
						this.Session.Remove("StationsPage");
					}
					this.UpdateView();
					this.Session.Remove("Station");
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
			this.Session["StationsPage"] = this.StationsDataGrid.CurrentPageIndex;
			this.Redirect("StationForm.aspx");
		}

		private void EnableDisableButton_Command(object source, DataGridCommandEventArgs e)
		{
			try
			{
				Guid targetStationGuid = this.GetGuidFromGridArgument(e.Item);

				this.GetSecurity();

				// Check to make sure we can purge the Station
				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager LoadRackManager = this.GetLoadRackManager();
						LoadRackManager.Purge(this.Security, typeof(StationClass), targetStationGuid);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}

				StationClass Station = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(this.Security, targetStationGuid)
																);


				Station.Enabled = (Station.Enabled) ? false : true;

				FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Modify(this.Security, Station)
																);


				if (Station.Enabled && UsingLoadRack)
				{
					try
					{
						ILoadRackManager LoadRackManager = this.GetLoadRackManager();
						LoadRackManager.Add(this.Security, typeof(StationClass), Station.IdentityGuid);
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
						{
							throw socketExcept;
						}
					}
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private ICollection EnumerateStations()
		{
			StationCollectionClass StationCollection = FMChannelHelper.MakeCall<IStations, StationCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			var StationDataTable = new DataTable();
			DataRow StationDataRow;
			StationClass Station;

			StationDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			StationDataTable.Columns.Add("EnableDisable", typeof(string));
			StationDataTable.Columns.Add("ID", typeof(string));
			StationDataTable.Columns.Add("Enabled", typeof(bool));
			StationDataTable.Columns.Add("Type", typeof(string));
			StationDataTable.Columns.Add("VaporRecovery", typeof(bool));
			StationDataTable.Columns.Add("LoadRack", typeof(bool));
			StationDataTable.Columns.Add("MeterGuid", typeof(Guid));

			for (int iItem = 0; iItem < StationCollection.Count; iItem++)
			{
				StationDataRow = StationDataTable.NewRow();

				Station = StationCollection[iItem];
				StationDataRow["IdentityGuid"] = Station.IdentityGuid;
				StationDataRow["EnableDisable"] = (Station.Enabled) ? "Disable" : "Enable";
				StationDataRow["ID"] = Station.ID;
				StationDataRow["Enabled"] = (Station.Enabled) ? true : false;
				StationDataRow["Type"] = this.GetTranslatedText(StationClass.TypeID(Station.Type));
				StationDataRow["VaporRecovery"] = Station.VaporRecovery;
				StationDataRow["LoadRack"] = (Station.Type == STATION_TYPE.LOAD_RACK) ? true : false;
				StationDataRow["MeterGuid"] = Station.Meter.IdentityGuid;

				StationDataTable.Rows.Add(StationDataRow);
			}
			var StationDataView = new DataView(StationDataTable);
			return StationDataView;
		}

		private Guid GetGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[1].Text);
		}

		private Guid GetMeterGuidFromGridArgument(DataGridItem theDataGrid)
		{
			return new Guid(theDataGrid.Cells[7].Text);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.StationsDataGrid.ItemCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.StationsDataGrid_ItemCommand);
			this.StationsDataGrid.EditCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.StationsDataGrid_EditCommand);
			this.StationsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.StationsDataGrid_PageIndexChanged);
			this.StationsDataGrid.DeleteCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.StationsDataGrid_DeleteCommand);
			this.StationsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.StationsDataGrid_ItemDataBound);
		}

		private void StationsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				Guid targetStationGuid = this.GetGuidFromGridArgument(e.Item);

				this.GetSecurity();

				// Check to make sure we can purge the Station
				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager LoadRackManager = this.GetLoadRackManager();
						LoadRackManager.Purge(this.Security, typeof(StationClass), targetStationGuid);
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw socketExcept;
					}
				}

				FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Purge(this.Security, targetStationGuid)
																);

				Guid associatedMeterGuid = this.GetMeterGuidFromGridArgument(e.Item);
				if (associatedMeterGuid != Guid.Empty)
				{
					if (FMChannelHelper.MakeCall<IMeters, bool>(x => x.HasForeignKeyReference(this.Security, associatedMeterGuid)) == false)
					{
						FMChannelHelper.MakeCall<IMeters>(x => x.Purge(this.Security, associatedMeterGuid));
					}
				}

				this.StationsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");

				if (this.StationsDataGrid.Items.Count == 1 && this.StationsDataGrid.CurrentPageIndex > 0)
				{
					this.StationsDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void StationsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session["IdentityGuid"] = this.GetGuidFromGridArgument(e.Item);
			this.Session["StationsPage"] = this.StationsDataGrid.CurrentPageIndex;
			this.Redirect("StationForm.aspx");
		}

		private void StationsDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "EnableDisableButton")
			{
				this.EnableDisableButton_Command(source, e);
			}
		}

		private void StationsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var EditButton = (LinkButton)e.Item.FindControl("EditButton");
			var DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			var EnableDisableButton = (FMButton)e.Item.FindControl("EnableDisableButton");
			var EnabledCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckbox");

			if (DeleteButton != null && EnableDisableButton != null && EnabledCheckBox != null)
			{
				if (EnabledCheckBox.Checked)
				{
					string confirmText = HttpUtility.JavaScriptStringEncode(
						this.GetTranslatedText("Are you sure you want to disable?"));
					EnableDisableButton.Attributes.Add("onClick", "if(disabled)return false; return confirm(\"" + confirmText + "\");");
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				    && !this.Security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
				{
					EnableDisableButton.Enabled = false;
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
				{
					DeleteButton.Enabled = false;
					DeleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}

				if (!this.Security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS))
				{
					EditButton.Enabled = false;
				}
			}
		}

		private void StationsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.StationsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.StationsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection Stations = this.EnumerateStations();

			this.StationsFormPageSizeDropDown.SetPageSize(this.StationsDataGrid, Stations.Count);

			this.StationsDataGrid.DataSource = Stations;
			this.StationsDataGrid.DataBind();
		}

		#endregion
	}

	/// <summary>
	///    Summary description for StationsFormOperationsMenu.
	/// </summary>
	public class StationsFormOperationsMenu : IMenuDiscovery
	{
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
                // Depends Upon Load Rack
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();
			// Site Groups don't have Stations
			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.ENABLEDISABLE_STATIONS))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_STATIONS,
						RootMenuName = "Operations",
						CategoryName = "Load Rack",
						ItemName = "Stations",
						NavigateUrl = "StationsForm.aspx",
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion
	}
}