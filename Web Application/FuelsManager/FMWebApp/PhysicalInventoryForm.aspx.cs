// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalInventoryForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PhysicalInventoryForm type.
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

	using FMControls;

	using Opc.Da;

	using TankClass = FMBusinessObjects.DataObjects.TankClass;

	/// <summary>
	///    Summary description for PhysicalInventoryForm.
	/// </summary>
	public partial class PhysicalInventoryForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		protected FMButton Button1;

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
                if ((word1 & 0x80) != 0x80)
				{
					return null;
				}
			}
            else
            {
                // Depends Upon WEB Inventory
                if ((options & 0x20000) == 0)
                {
                    return null;
                }
            }

            if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_INVENTORY_DATA))
			{
				return null;
			}

			var items = new List<FMMenuItem>
			{
				new FMMenuItem
				{
					MenuItemType = FMMenuItemType.OPERATIONS_INVENTORY_MANAGEMENT_PHYSICAL_INVENTORY,
					RootMenuName = "Operations",
					CategoryName = "Inventory Management",
					ItemName = "Physical Inventory",
					NavigateUrl = "PhysicalInventoryForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply
				}
			};

			return items;
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

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					//  Populate Products Drop Down List
					ProductCollectionClass ProductCollection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
					
					var NewItem = new ListItem("{All}", Guid.Empty.ToString());
					this.ProductDropDownList.Items.Add(NewItem);
					foreach (ProductClass Product in ProductCollection)
					{
						NewItem = new ListItem(Product.ID, Product.IdentityGuid.ToString());

						this.ProductDropDownList.Items.Add(NewItem);
						if (this.Session["ProductGuid"] != null && (Guid)this.Session["ProductGuid"] == Product.IdentityGuid)
						{
							this.ProductDropDownList.SelectedIndex = this.ProductDropDownList.Items.Count - 1;
						}
					}

					if (this.Session["PhysicalInventoryPage"] != null)
					{
						this.PhysicalInventoryDataGrid.CurrentPageIndex = (int)this.Session["PhysicalInventoryPage"];
						this.Session.Remove("PhysicalInventoryPage");
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ProductDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.Session["ProductGuid"] = Guid.Parse(this.ProductDropDownList.SelectedItem.Value);
			this.PhysicalInventoryDataGrid.CurrentPageIndex = 0;
			this.UpdateView();
		}

		private ICollection EnumeratePhysicalInventory()
		{
			SiteClass CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			//TankCollectionClass TankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
			//														 x =>
			//														 x.Enumerate(this.Security)
			//													);
			DataSet tankSet = FMChannelHelper.MakeCall<ITanks, DataSet>(tanks => tanks.EnumerateForPhysicalInventory(this.Security));
			var TankDataTable = new DataTable();
			DataRow TankDataRow;

			TankDataTable.Columns.Add("TankGuid", typeof(Guid));
			TankDataTable.Columns.Add("TankID", typeof(string));
			TankDataTable.Columns.Add("ProductID", typeof(string));
			TankDataTable.Columns.Add("Status", typeof(string));
			TankDataTable.Columns.Add("LevelTimeStamp", typeof(string));
			TankDataTable.Columns.Add("Level", typeof(string));
			TankDataTable.Columns.Add("GrossVolume", typeof(string));
			TankDataTable.Columns.Add("AvailableGrossVolume", typeof(string));
			TankDataTable.Columns.Add("RemainingGrossVolume", typeof(string));
			TankDataTable.Columns.Add("NetVolume", typeof(string));
			TankDataTable.Columns.Add("AvailableNetVolume", typeof(string));
			TankDataTable.Columns.Add("RemainingNetVolume", typeof(string));
			TankDataTable.Columns.Add("Market", typeof(bool));

			Guid productGuid = Guid.Empty;

			if (this.Session["ProductGuid"] != null)
			{
				productGuid = (Guid)this.Session["ProductGuid"];
				ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetBasicInfo(Security, productGuid, Security.SiteGuid));
				productGuid = Guid.Empty;
				if (product != null)
					productGuid = product.MasterRecordGuid;
			}

			if ((tankSet?.Tables.Count ?? 0) > 0)
			{
				foreach (DataRow tankRow in tankSet.Tables[0].Rows)
				{
					Guid tankGuid = DataObject.getValue(tankRow["TankGuid"], Guid.Empty);
					string tankId = DataObject.getValue(tankRow["TankId"], string.Empty);
					Guid tankProductGuid = DataObject.getValue(tankRow["ProductGuid"], Guid.Empty);
					string productId = DataObject.getValue(tankRow["ProductId"], "{None}");
					if (productGuid != Guid.Empty && productGuid != tankProductGuid)
					{
						continue;
					}

					TankDataRow = TankDataTable.NewRow();

					ProcessVariableCollectionClass PVs = FMChannelHelper.MakeCall<ITanks, ProcessVariableCollectionClass>(tanks => tanks.GetProcessVariables(this.Security, tankGuid));
					ProcessVariableClass PV;
					TankDataRow["TankGuid"] = tankGuid;
					TankDataRow["TankID"] = tankId;
					TankDataRow["ProductID"] = productId;

					PV = PVs[PROCESS_VARIABLE_TYPE.TANK_STATUS_PV];
					TankDataRow["Status"] = PV.Encode(
						PV.ServerValue,
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						0,
						null);

					PV = PVs[PROCESS_VARIABLE_TYPE.LEVEL_PV];
					TankDataRow["LevelTimeStamp"] = PV.DateTimeStamp.ToString(CurrentSite.GetDateTimeFormatInfo());
					TankDataRow["Level"] = PV.Encode(
						PV.GetValue(CurrentSite.LevelUnits, CurrentSite._LevelDecimalPlaces),
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						CurrentSite.LevelUnits,
						CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.LENGTH));

					PV = PVs[PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV];
					TankDataRow["GrossVolume"] = PV.Encode(
						PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						CurrentSite.VolumeUnits,
						CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));
					PV = PVs[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV];
					TankDataRow["AvailableGrossVolume"] =
						PV.Encode(
							PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
							(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
							CurrentSite.VolumeUnits,
							CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

					PV = PVs[PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV];
					TankDataRow["RemainingGrossVolume"] =
						PV.Encode(
							PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
							(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
							CurrentSite.VolumeUnits,
							CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

					PV = PVs[PROCESS_VARIABLE_TYPE.NET_VOLUME_PV];
					TankDataRow["NetVolume"] = PV.Encode(
						PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						CurrentSite.VolumeUnits,
						CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

					PV = PVs[PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV];
					TankDataRow["AvailableNetVolume"] = PV.Encode(
						PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						CurrentSite.VolumeUnits,
						CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

					PV = PVs[PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV];
					TankDataRow["RemainingNetVolume"] = PV.Encode(
						PV.GetValue(CurrentSite.VolumeUnits, CurrentSite._VolumeDecimalPlaces),
						(CurrentSite.UseLastKnownGoodTankData) ? new Quality(Quality.Good.GetCode()) : new Quality(PV.OPCQuality),
						CurrentSite.VolumeUnits,
						CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME));

					PV = PVs[PROCESS_VARIABLE_TYPE.TANK_OPERATION_PV];

					bool Market;
					if (PV.OPCQuality == Quality.Good.GetCode()
						&& (PV.ServerValue as string == "Market"
							|| (typeof(bool).IsInstanceOfType(PV.ServerValue) && (bool)PV.ServerValue)))
					{
						Market = true;
					}
					else
					{
						Market = false;
					}

					TankDataRow["Market"] = Market;

					TankDataTable.Rows.Add(TankDataRow);
				}
			}
			var TankDataView = new DataView(TankDataTable);
			return TankDataView;
		}

		private TankClass GetTanks(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<ITanks, TankClass>(
																	 x =>
																	 x.Get(securityClass, guid)
																);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.PhysicalInventoryDataGrid.ItemCommand +=
				new DataGridCommandEventHandler(this.PhysicalInventoryDataGrid_ItemCommand);
			this.PhysicalInventoryDataGrid.PageIndexChanged +=
				new DataGridPageChangedEventHandler(this.PhysicalInventoryDataGrid_PageIndexChanged);
		}

		private void PhysicalInventoryDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "Select")
			{
				TableCell indexCell = e.Item.Cells[1]; // This is the column in the PhysicalInventoryGrid corresponding to the TankGuid
				this.Session["IdentityGuid"] = indexCell.Text;
				this.Session["PhysicalInventoryPage"] = this.PhysicalInventoryDataGrid.CurrentPageIndex;
				this.Redirect("TankDetailForm.aspx");
			}
		}

		private void PhysicalInventoryDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.PhysicalInventoryDataGrid.EditItemIndex > -1)
			{
				return;
			}
			this.PhysicalInventoryDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpdateView()
		{
			ICollection List = this.EnumeratePhysicalInventory();

			this.PhysicalInventoryDataGrid.DataSource = List;
			this.PhysicalInventoryDataGrid.DataBind();
		}

		#endregion
	}
}