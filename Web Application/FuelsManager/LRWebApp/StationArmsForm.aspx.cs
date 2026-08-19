// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationArmsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace LoadRackWebApp
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

	using FMWebApp;

	using FuelsManager.FMWebApp;

	public partial class StationArmsForm : FMFormBase
	{
		#region Methods

		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
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
					if (this.Session["StationArmsPage"] != null)
					{
						this.StationArmsDataGrid.CurrentPageIndex = (int)this.Session["StationArmsPage"];
						this.Session.Remove("StationArmsPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void EnableDisableButton_Command(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get identityGuid
				TableCell stationIdentityGuidCell = e.Item.Cells[0];
				TableCell armIndexCell = e.Item.Cells[1];

				this.GetSecurity();
				var station = new StationClass();
				FMChannelHelper.MakeCall<IStations>(
					stations =>
					{
						station = stations.Get(this.Security, Guid.Parse(stationIdentityGuidCell.Text));

						if (station.Enabled)
						{
							// Check to make sure we can purge the Station
							try
							{
								ILoadRackManager LoadRackManager = this.GetLoadRackManager();
								LoadRackManager.Purge(this.Security, typeof(StationClass), Guid.Parse(stationIdentityGuidCell.Text));
							}
							catch (SocketException socketExcept)
							{
								if (socketExcept.ErrorCode != 10061)
								{
									throw socketExcept;
								}
							}
						}

						LoadArmClass loadArm = station.LoadArmCollection[Convert.ToInt32(armIndexCell.Text)];

						loadArm.Enabled = loadArm.Enabled ? false : true;

						stations.Modify(this.Security, station);
					});

				if (station.Enabled)
				{
					try
					{
						ILoadRackManager LoadRackManager = this.GetLoadRackManager();
						LoadRackManager.Add(this.Security, typeof(StationClass), station.IdentityGuid);
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
			var stationArmDataTable = new DataTable();
			FMChannelHelper.MakeCall<IStations>(
				stations =>
				{
					StationCollectionClass stationCollection = stations.Enumerate(this.Security);
					DataRow stationArmDataRow;

					stationArmDataTable.Columns.Add("StationGuid", typeof(Guid));
					stationArmDataTable.Columns.Add("ArmIndex", typeof(Int32));
					stationArmDataTable.Columns.Add("EnableDisable", typeof(string));
					stationArmDataTable.Columns.Add("ID", typeof(string));
					stationArmDataTable.Columns.Add("Enabled", typeof(bool));

					foreach (StationClass station in stationCollection)
					{
						if (station.Type != STATION_TYPE.LOAD_RACK)
						{
							continue;
						}

						station.Load(stations.Get(this.Security, station.IdentityGuid));

						int armIndex = 0;
						foreach (LoadArmClass loadArm in station.LoadArmCollection)
						{
							stationArmDataRow = stationArmDataTable.NewRow();

							stationArmDataRow["StationGuid"] = station.IdentityGuid;
							stationArmDataRow["ArmIndex"] = armIndex++;
							stationArmDataRow["EnableDisable"] = loadArm.Enabled ? "Disable" : "Enable";
							stationArmDataRow["ID"] = loadArm.ID;
							stationArmDataRow["Enabled"] = loadArm.Enabled;

							stationArmDataTable.Rows.Add(stationArmDataRow);
						}
					}
				});

			var stationDataView = new DataView(stationArmDataTable);
			return stationDataView;
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.StationArmsDataGrid.ItemCommand +=
				new System.Web.UI.WebControls.DataGridCommandEventHandler(this.StationArmsDataGrid_ItemCommand);
			this.StationArmsDataGrid.PageIndexChanged +=
				new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.StationArmsDataGrid_PageIndexChanged);
			this.StationArmsDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.StationArmsDataGrid_ItemDataBound);
		}

		private void StationArmsDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "EnableDisableButton")
			{
				this.EnableDisableButton_Command(source, e);
			}
		}

		private void StationArmsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var EnableDisableButton = (FMButton)e.Item.FindControl("EnableDisableButton");
			var EnabledCheckBox = (CheckBox)e.Item.FindControl("EnabledCheckbox");
			if (EnableDisableButton != null && EnabledCheckBox != null)
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
			}
		}

		private void StationArmsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.StationArmsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.StationArmsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection Stations = this.EnumerateStations();

			this.StationArmsFormPageSizeDropDown.SetPageSize(this.StationArmsDataGrid, Stations.Count);

			this.StationArmsDataGrid.DataSource = Stations;
			this.StationArmsDataGrid.DataBind();
		}

		#endregion
	}

	/// <summary>
	///     Summary description for StationsFormLoadRackNode.
	/// </summary>
	public class StationsFormLoadRackNode : IMenuDiscovery
	{
		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
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
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_ENABLE_STATION_ARMS, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Station Arms", 
						NavigateUrl = "..\\LRWebApp\\StationArmsForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion
	}
}