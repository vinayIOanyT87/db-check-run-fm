// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoadArmForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LoadArmForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.UtilityObjects;

	using FMControls;

	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///    Summary description for LoadArmForm.
	/// </summary>
	public partial class LoadArmForm : FMFormBase
	{
		#region Constants and Fields

		protected FMLabel Label3;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will enable and disable controls.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)) //can not be configured at SiteGroup
			{
				this.OK.Enabled = enable;
			}
			this.Cancel.Enabled = enable;

			this.tcLoadArmTabs.HeaderEnabled = enable;
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
				this.Session.Remove("Status");

				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					var station = (StationClass)this.Session["Station"];

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS) || !this.Security.HasRight(RIGHT.MODIFY_METERS))
					{
						this.OK.Enabled = false;
					}

					// Copy the Load Arm
					var originalLoadArm = new LoadArmClass();
					originalLoadArm.Load(station.LoadArmCollection[(int)this.Session["LoadArmIndex"]]);
					this.Session["OriginalLoadArm"] = originalLoadArm;

					var loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

					string stationText = "Station:";
					string armText = "Arm:";
					string configurationText = "Configuration";

					if (this.Session["UseDataDictionary"] == null || (bool)this.Session["UseDataDictionary"])
					{
                        // ReSharper disable once AccessToModifiedClosure
                        stationText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.LoginSiteGuid, stationText));


                        // ReSharper disable once AccessToModifiedClosure
                        armText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.LoginSiteGuid, armText));


                        // ReSharper disable once AccessToModifiedClosure
                        configurationText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.LoginSiteGuid, configurationText));
					}

					this.ArmLabel.Text = stationText + " " + station.ID + " " + armText + " ";
					if (station.SwingArmPosition == "A")
					{
						this.ArmLabel.Text += loadArm.BayAArmNumber.ToString();
					}
					else
					{
						this.ArmLabel.Text += loadArm.BayBArmNumber.ToString();
					}

					this.ArmLabel.Text += " " + configurationText;
				}
				else
				{
					if (this.Session["LoadArmIndex"] == null)
					{
						throw new Exception("Load Arm not in Session");
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void Cancel_Command(object sender, CommandEventArgs e)
		{
			// Reload the Original Load Arm
			StationClass station = this.Session["Station"] as StationClass;
			LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
			var originalLoadArm = (LoadArmClass)this.Session["OriginalLoadArm"];
			loadArm?.Load(originalLoadArm);

			// If preset type is MAX_PRESET_TYPE this must be an add so remove from collection
			if (loadArm != null && loadArm.PresetType == PRESET_TYPE.MAX_PRESET_TYPE)
			{
				station.LoadArmCollection.RemoveAt(station.LoadArmCollection.Count - 1);
			}

			this.Redirect("StationForm.aspx");
		}

		/// <summary>
		///    Check all meters assigned to this load arm's components to see if there is one with a matching meter ID
		///    but a different definition (number of digits, etc). This method will throw if an error is detected.
		/// </summary>
		/// <param name="componentIndex">
		///    The index of the component we're checking for duplicates of so we
		///    know not to detect itself as a duplicate
		/// </param>
		/// <param name="meterID">The meter ID to check for conflicting definitions of</param>
		/// <param name="numberOfDigits">The number of digits assigned to a meter with the provided meter ID</param>
		/// <param name="rotatesBackwards">The rotates backwards flag assigned to a meter with the provided meter ID</param>
		/// <param name="receiptMeter">The receipt meter flag assigned to a meter with the provided meter ID</param>
		private void CheckForConflictingMeterDefinition(
			int componentIndex, string meterID, int numberOfDigits, bool rotatesBackwards, bool receiptMeter)
		{
			StationClass station = this.Session["Station"] as StationClass;
			LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

			if (loadArm != null)
			{
				// The user is allowed to assign the same meter to multiple product maps.
				// However, the meter information must match. Check to make sure that it does
				for (int i = 0; i < loadArm.ComponentCollection.Count; i++)
				{
					ProductMapClass existingMap = loadArm.ComponentCollection[i];

					if (!string.IsNullOrEmpty(existingMap.Meter?.ID))
					{
						if (existingMap.Meter.ID == meterID && i != componentIndex)
						{
							if (existingMap.Meter.RotatesBackwardsFlag != rotatesBackwards
							    || existingMap.Meter.ReceiptMeterFlag != receiptMeter || existingMap.Meter.NumberOfDigits != numberOfDigits)
							{
								throw new ApplicationException("Meter definitions are different");
							}
						}
					}
				}
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command += this.OK_Command;
			this.Cancel.Command += this.Cancel_Command;
		}

	    // ReSharper disable once InconsistentNaming
		private void OK_Command(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				var station = (StationClass)this.Session["Station"];
				var loadArm = station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				var originalLoadArm = (LoadArmClass)this.Session["OriginalLoadArm"];

				Guid originalAssociatedStationGuid = Guid.Empty;

				// Determine if Assocated Station in the original load arm configuration
				if (originalLoadArm.SwingArm
				    && (!loadArm.SwingArm || originalLoadArm.BayAStationGuid != loadArm.BayAStationGuid
				        || originalLoadArm.BayBStationGuid != loadArm.BayBStationGuid))
				{
					if (originalLoadArm.BayAStationGuid != station.IdentityGuid && originalLoadArm.BayAStationGuid != Guid.Empty)
					{
						originalAssociatedStationGuid = originalLoadArm.BayAStationGuid;
					}

					if (originalLoadArm.BayBStationGuid != station.IdentityGuid && originalLoadArm.BayBStationGuid != Guid.Empty)
					{
						originalAssociatedStationGuid = originalLoadArm.BayBStationGuid;
					}
				}

				// Determine if the Association still exists with other arms
				if (originalAssociatedStationGuid != Guid.Empty)
				{
					foreach (LoadArmClass otherLoadArm in station.LoadArmCollection)
					{
						if (otherLoadArm.BayAStationGuid == originalAssociatedStationGuid
						    || otherLoadArm.BayBStationGuid == originalAssociatedStationGuid)
						{
							originalAssociatedStationGuid = Guid.Empty;
							break;
						}
					}
				}

				this.LoadArmGeneralPage.UpdateData();

				// If the load arm is a swing arm, we need to check to make sure we set a bay number 
				// that makes sense for the sister station.  
				if (loadArm.SwingArm)
				{
					if (station.SwingArmPosition == "A")
					{
						if (loadArm.BayBArmNumber == 0)
						{
							// Get the other station and determine a number
							StationClass sisterStation = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(this.Security, loadArm.BayBStationGuid)
																);

							if (sisterStation != null)
							{
								loadArm.BayBArmNumber = sisterStation.LoadArmCollection.Count + 1;
							}
						}
					}
					else
					{
						if (loadArm.BayAArmNumber == 0)
						{
							// Get the other station and determine a number
							StationClass sisterStation = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(this.Security, loadArm.BayAStationGuid)
																);

							if (sisterStation != null)
							{
								loadArm.BayAArmNumber = sisterStation.LoadArmCollection.Count + 1;
							}
						}
					}
				}

				// Component meters can be shared by different components. 
				// Make sure that there are no meters with the same ID but different data values
				for (int i = 0; i < loadArm.ComponentCollection.Count; i++)
				{
					ProductMapClass component = loadArm.ComponentCollection[i];

					if (!string.IsNullOrEmpty(component?.Meter?.ID))
					{
						this.CheckForConflictingMeterDefinition(
							i,
							component.Meter.ID,
							component.Meter.NumberOfDigits,
							component.Meter.RotatesBackwardsFlag,
							component.Meter.ReceiptMeterFlag);
					}
					else
					{
						// meter is required for component so inform the user
						throw new ApplicationException("Meter definitions are required for Components");
					}
				}

				// Flow Controlled Additive meters can be shared by different components as well.
				// (FCAs are components, not additives)
				// Make sure that there are no meters with the same ID but different data values
				for (int i = 0; i < loadArm.FlowControlledAdditiveCollection.Count; i++)
				{
					ProductMapClass flowControlledAdditive = loadArm.FlowControlledAdditiveCollection[i];

					if (!string.IsNullOrEmpty(flowControlledAdditive?.Meter?.ID))
					{
						this.CheckForConflictingMeterDefinition(
							i,
							flowControlledAdditive.Meter.ID,
							flowControlledAdditive.Meter.NumberOfDigits,
							flowControlledAdditive.Meter.RotatesBackwardsFlag,
							flowControlledAdditive.Meter.ReceiptMeterFlag);
					}
					else
					{
						// meter is required for component so inform the user
						throw new ApplicationException("Meter definitions are required for Flow Controlled Additives");
					}
				}

				// CSI 4757 - Only save if the station has an ID; otherwise, we are saving during initial configuration
				if (!string.IsNullOrEmpty(station.ID))
				{
					if (!station.IdentityGuid.IsEmpty())
					{
						FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Modify(this.Security, station)
																);
					}
					else
					{
						station.IdentityGuid = FMChannelHelper.MakeCall<IStations, Guid>(
																	 x =>
																	 x.Add(this.Security, station)
																);
					}

					this.Session["Station"] = FMChannelHelper.MakeCall<IStations, StationClass>(
																	 x =>
																	 x.Get(this.Security, station.IdentityGuid)
																);


					// For existing Station try to purge from Load Rack Manager
					// to insure that no operation is started on the station
					// while being modified
					if (!site.DeferStationChanges && !(station.IdentityGuid.IsEmpty()) && station.Enabled)
					{
						try
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Purge(this.Security, typeof(StationClass), station.IdentityGuid);
						}
						catch (SocketException socketExcept)
						{
							if (socketExcept.ErrorCode != 10061)
							{
								throw;
							}
						}
					}

					if (!site.DeferStationChanges && station.Enabled)
					{
						try
						{
							ILoadRackManager loadRackManager = this.GetLoadRackManager();
							loadRackManager.Add(this.Security, typeof(StationClass), station.IdentityGuid);

							// Have to re-add any station that is no longer part of the swing arm configuration
							if (originalAssociatedStationGuid != Guid.Empty)
							{
								loadRackManager.Add(this.Security, typeof(StationClass), originalAssociatedStationGuid);
							}
						}
						catch (SocketException socketExcept)
						{
							if (socketExcept.ErrorCode != 10061)
							{
								throw;
							}
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("StationForm.aspx");
		}

		#endregion
	}

	public abstract class LoadArmPageBase : FMUserControlBase
	{
		#region Constants and Fields

		private bool Internal;

		private Guid locationGuid;

		private string meterID;

		private string presetNumber;

		private Guid productGuid;

		private string rollOver;

		private string type;

		#endregion

		#region Properties

		protected virtual DataGrid MapGrid => null;

	    protected abstract PRODUCT_MAP_TYPE PageMapType { get; }

		protected virtual ProductMapCollectionClass PageMaps
		{
			get
			{
				return null;
			}
		    // ReSharper disable once ValueParameterNotUsed
			set
			{
			}
		}

		#endregion

		#region Public Methods and Operators

	    // ReSharper disable once InconsistentNaming
		public void ProductDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
		{
			var productsDropDownList = (DropDownList)sender;
			this.productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productsDropDownList.SelectedValue)));
			this.locationGuid = Guid.Empty;
			DataGridItem item = this.MapGrid.Items[this.MapGrid.EditItemIndex];
			var internalCheckBox = (CheckBox)item.FindControl("EditInternalCheckBox");
			if (internalCheckBox != null)
			{
				this.Internal = internalCheckBox.Checked;
			}

			var rollOverTextBox = (TextBox)item.FindControl("RollOverTextBox");
			if (rollOverTextBox != null)
			{
				this.rollOver = rollOverTextBox.Text;
			}

			var presetNumberTextBox = (TextBox)item.FindControl("PresetNumberTextBox");
			if (presetNumberTextBox != null)
			{
				this.presetNumber = presetNumberTextBox.Text;
			}

			var meterIDComboBox = (FMComboBox)item.FindControl("MeterIDComboBox");
			var meterIDTextBox = (FMTextBox)item.FindControl("MeterIDTextBox");

			if (meterIDComboBox != null)
			{
				this.meterID = meterIDComboBox.Text;
			}
			else if (meterIDTextBox != null)
			{
				this.meterID = meterIDTextBox.Text;
			}

			var typeDropDownList = (FMDropDownList)item.FindControl("TypeDropDownList");
			if (typeDropDownList != null)
			{
				this.type = typeDropDownList.SelectedValue;
			}
			else
			{
				this.type = "Tank";
			}

			this.UpdatePresetConfigurationView();
		}

	    // ReSharper disable once InconsistentNaming
		public void TypeDropDownList_SelectedIndexChanged(Object sender, EventArgs e)
		{
			var typeDropDownList = (DropDownList)sender;
			this.type = typeDropDownList.SelectedValue;
			this.locationGuid = Guid.Empty;
			DataGridItem item = this.MapGrid.Items[this.MapGrid.EditItemIndex];

			var productsDropDownList = (DropDownList)item.FindControl("ProductsDropDownList");
			if (productsDropDownList != null)
			{
				this.productGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productsDropDownList.SelectedValue)));
			}

			var presetNumberTextBox = (TextBox)item.FindControl("PresetNumberTextBox");
			if (presetNumberTextBox != null)
			{
				this.presetNumber = presetNumberTextBox.Text;
			}

			var meterIDComboBox = (FMComboBox)item.FindControl("MeterIDComboBox");
			var meterIDTextBox = (FMTextBox)item.FindControl("MeterIDTextBox");

			if (meterIDComboBox != null)
			{
				this.meterID = meterIDComboBox.Text;
			}
			else if (meterIDTextBox != null)
			{
				this.meterID = meterIDTextBox.Text;
			}

			this.UpdatePresetConfigurationView();
		}

		#endregion

		#region Methods

	    // ReSharper disable once InconsistentNaming
		protected void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.productGuid = Guid.Empty;
			this.locationGuid = Guid.Empty;
			this.type = "Tank";
			this.meterID = "";
		    var maps = this.PageMaps;
			if (maps == null)
			{
				maps = new ProductMapCollectionClass();
				this.PageMaps = maps;
			}

		    var map = new ProductMapClass { Type = this.PageMapType };
		    if (maps.Count != 0)
			{
				var station = (StationClass)this.Session["Station"];
				if (station.Type == STATION_TYPE.LOAD_RACK && e.CommandName == "AddRecipe")
				{
					// Get Station
					int preset = FMChannelHelper.MakeCall<IStations, int>(
																x =>
																x.GetTheNextPresetNumber(this.Security, (Guid)this.Session["IdentityGuid"])
														);
					map.PresetNumber = preset;
				}
				else
				{
					map.PresetNumber = maps[maps.Count - 1].PresetNumber + 1;
				}
			}
			else
			{
				map.PresetNumber = 1;
			}
			maps.Add(map);
			this.presetNumber = map.PresetNumber.ToString();
			this.MapGrid.CurrentPageIndex = (maps.Count - 1) / this.MapGrid.PageSize;
			this.MapGrid.EditItemIndex = (maps.Count - 1) % this.MapGrid.PageSize;

			this.EnableControls(false);

			try
			{
				this.UpdatePresetConfigurationView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				maps.RemoveAt(maps.Count - 1);
				if (this.MapGrid.CurrentPageIndex > 0 && this.MapGrid.EditItemIndex == 0)
				{
					this.MapGrid.CurrentPageIndex--;
				}
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdatePresetConfigurationView();
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void DataGrid_CancelCommand(object source, DataGridCommandEventArgs e)
		{
		    var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				ProductMapCollectionClass maps = this.PageMaps;
			    var map = maps[Convert.ToInt32(indexLabel.Text)];
				if (map.AssignedID == "")
				{
					maps.RemoveAt(Convert.ToInt32(indexLabel.Text));
					if (this.MapGrid.Items.Count == 1 && this.MapGrid.CurrentPageIndex > 0)
					{
						this.MapGrid.CurrentPageIndex--;
					}
				}
				this.MapGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdatePresetConfigurationView();
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void DataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			var linksDataGrid = (DataGrid)source;

			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				if (linksDataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					linksDataGrid.EditItemIndex = -1;
					this.EnableControls(true);
				}
				else if (linksDataGrid.EditItemIndex > e.Item.ItemIndex)
				{
					linksDataGrid.EditItemIndex--;
				}

				ProductMapCollectionClass maps = this.PageMaps;
				maps.RemoveAt(Convert.ToInt32(indexLabel.Text));

				if (this.PageMapType == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
				{
					int tempPresetNumber = 0;
					foreach (ProductMapClass map in maps)
					{
						map.PresetNumber = ++tempPresetNumber;
					}
				}

				if (this.MapGrid.Items.Count == 1 && this.MapGrid.CurrentPageIndex > 0)
				{
					this.MapGrid.CurrentPageIndex--;
				}

				this.UpdatePresetConfigurationView();
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void DataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			var indexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (indexLabel != null)
			{
				var dataGrid = (DataGrid)source;
				dataGrid.EditItemIndex = e.Item.ItemIndex;
				this.EnableControls(false);
				ProductMapCollectionClass maps = this.PageMaps;
			    var map = maps[Convert.ToInt32(indexLabel.Text)];
				this.productGuid = map.AssignedGuid;
				this.locationGuid = map.TankOrGroupGuid;
				this.presetNumber = map.PresetNumber.ToString();

				if (map.Meter != null)
				{
					this.meterID = map.Meter.ID;

					var meterIDComboBox = (FMComboBox)e.Item.FindControl("MeterIDComboBox");

				    meterIDComboBox?.SelectByText(map.Meter.ID);
				}

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				ProcessVariableClass internalMeterPv =
					map.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
				this.Internal = (internalMeterPv != null);
				this.rollOver = (internalMeterPv == null)
					                ? ""
					                : internalMeterPv.Encode(
						                internalMeterPv.GetMaximum(EngineeringUnit.FmvMeter3, site._AdditiveVolumeDecimalPlaces),
						                Quality.Good,
						                site.AdditiveVolumeUnits,
						                site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME));

				if (map.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP || map.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
				{
					this.type = "Group";
				}
				else
				{
					this.type = "Tank";
				}

				try
				{
					this.UpdatePresetConfigurationView();
				}
				catch (Exception except)
				{
					this.ErrorHandler(except);
					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdatePresetConfigurationView();
				}
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			var linksDataGrid = (DataGrid)sender;

			if (linksDataGrid.EditItemIndex == -1 && e.Item.ItemIndex != linksDataGrid.EditItemIndex)
			{
				return;
			}

			var indexLabel = (Label)e.Item.FindControl("IndexLabel");
			if (indexLabel != null)
			{
				var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
				if (productsDropDownList != null)
				{
				    if (this.productGuid != Guid.Empty)
					{
						ListItemCollection items = productsDropDownList.Items;
						int index = items.IndexOf(items.FindByValue(this.productGuid.ToString()));
						productsDropDownList.SelectedIndex = index;
					}

					if (productsDropDownList.Items.Count == 0)
					{
						productsDropDownList.Visible = false;
					}

					var locationDropDownList = (DropDownList)e.Item.FindControl("LocationDropDownList");
					if (locationDropDownList != null)
					{
						if (locationDropDownList.Items.Count == 0)
						{
							locationDropDownList.Visible = false;
						}
						else if (this.locationGuid != Guid.Empty)
						{
							ListItemCollection items = locationDropDownList.Items;
							int index = items.IndexOf(items.FindByValue(this.locationGuid.ToString()));
							locationDropDownList.SelectedIndex = index;
						}
					}

					var internalCheckBox = (CheckBox)e.Item.FindControl("EditInternalCheckBox");
					if (internalCheckBox != null)
					{
						internalCheckBox.Checked = this.Internal;
					}

					var rollOverTextBox = (TextBox)e.Item.FindControl("RollOverTextBox");
					if (rollOverTextBox != null)
					{
						rollOverTextBox.Text = this.rollOver;
					}

					var presetNumberTextBox = (TextBox)e.Item.FindControl("PresetNumberTextBox");
					if (presetNumberTextBox != null)
					{
						presetNumberTextBox.Text = this.presetNumber;
					}

					var meterIDComboBox = (FMComboBox)e.Item.FindControl("MeterIDComboBox");
					var meterIDTextBox = (FMTextBox)e.Item.FindControl("MeterIDTextBox");

					if (meterIDComboBox != null)
					{
						meterIDComboBox.Text = this.meterID;
					}
					else if (meterIDTextBox != null)
					{
						meterIDTextBox.Text = this.meterID;
					}

					var typeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");
					if (typeDropDownList != null)
					{
						typeDropDownList.SelectedValue = this.type;
					}
				}
			}
		}

	    // ReSharper disable once InconsistentNaming
		protected void DataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			var dataGrid = (DataGrid)source;
			// if we are editing do not allow a page change
			if (dataGrid.EditItemIndex > -1)
			{
				return;
			}
			dataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdatePresetConfigurationView();
		}

		/// <summary>
		///    This method handles the update event for a row that has been edited.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		// ReSharper disable once InconsistentNaming
		protected void DataGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var indexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (indexLabel != null)
				{
					ProductMapCollectionClass maps = this.PageMaps;
					ProductMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

					if (map.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
					{
						ProcessVariableClass internalPv =
							map.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];

						var internalCheckBox = (CheckBox)e.Item.FindControl("EditInternalCheckBox");
						if (internalCheckBox != null && internalCheckBox.Checked)
						{
							// The ServerUnits are set to SI so no unit conversion will be performed
							// on Meter Totalizer values.
							if (internalPv == null)
							{
							    internalPv = new ProcessVariableClass(
							        PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV,
							        UNIT_TYPE.PRODUCT_MAP_PRESET_INJECTOR,
							        VarEnum.VT_R8,
							        true,
							        "",
							        "",
							        "") { ServerUnits = EngineeringUnit.FmvMeter3, ServerValue = 0.0, DateTimeStamp = DateTimeOffset.Now };
							    map.ProcessVariableCollection.Add(internalPv);
							}

							var rollOverTextBox = (TextBox)e.Item.FindControl("RollOverTextBox");
							if (rollOverTextBox != null)
							{
								internalPv.SetMaximum(Convert.ToDouble(rollOverTextBox.Text), EngineeringUnit.FmvMeter3);
							}
						}

						else
						{
							if (internalPv != null)
							{
								map.ProcessVariableCollection.Remove(internalPv);
							}
						}
					}

					// Set the Map Type for Components which can be either Tank or Tank Group
					if (map.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
					    || map.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
					{
						var typeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");

						if (typeDropDownList != null)
						{
							this.type = typeDropDownList.SelectedValue;

							if (this.type == "Tank")
							{
								map.Type = PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP;
							}
							else
							{
								map.Type = PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP;
							}
						}
					}

                    // Set the Map Type for ExternalComponents which can be either Tank or Tank Group
                    if (map.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
                        || map.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
                    {
                        var typeDropDownList = (FMDropDownList)e.Item.FindControl("TypeDropDownList");

                        if (typeDropDownList != null)
                        {
                            this.type = typeDropDownList.SelectedValue;

                            if (this.type == "Tank")
                            {
                                map.Type = PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP;
                            }
                            else
                            {
                                map.Type = PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP;
                            }
                        }
                    }

               try
					{
						var station = (StationClass)this.Session["Station"];

						var presetNumberTextBox = (TextBox)e.Item.FindControl("PresetNumberTextBox");
						if (presetNumberTextBox != null)
						{
							if (!(presetNumberTextBox.Text == "Dynamic" && station.Type == STATION_TYPE.LOAD_RACK && station.EnableDynamicRecipes))
							{ 
								map.PresetNumber = Convert.ToInt32(presetNumberTextBox.Text);
							}

							var enableRecipeCheckbox = (CheckBox)e.Item.FindControl("EnableRecipeCheckbox");
							if (enableRecipeCheckbox != null)
							{
								bool enableRecipe = Convert.ToBoolean(enableRecipeCheckbox.Checked);
								map.EnableRecipe = enableRecipe;
							}
						}
					}
					catch (Exception)
					{
						this.ErrorHandler(new Exception("Injector must be numeric"));
					}

				    var meterIDComboBox = (FMComboBox)e.Item.FindControl("MeterIDComboBox");
					var meterIDTextBox = (FMTextBox)e.Item.FindControl("MeterIDTextBox");

					if (meterIDComboBox != null || meterIDTextBox != null)
					{
					    string inputMeterID;
					    if (meterIDComboBox != null)
						{
							inputMeterID = meterIDComboBox.Text;
						}
						else
						{
						    inputMeterID = meterIDTextBox.Text;
						}

					    // If the user entered a meter ID, add the meter information to the product map.
						if (!string.IsNullOrEmpty(inputMeterID))
						{
							if (map.Meter == null)
							{
								map.Meter = new MeterClass();
							}

							string id = inputMeterID;
							int numberOfDigits = 0;
							bool rotatesBackwards = false;
							bool receiptMeter = false;

							var numberOfDigitsTextBox = (FMTextBox)e.Item.FindControl("NumberOfDigitsTextBox");

							if (numberOfDigitsTextBox != null)
							{
								numberOfDigits = MeterClass.ValidateNumberOfDigits(numberOfDigitsTextBox.Text);
							}

							var rotatesBackwardsCheckBox = (FMCheckBox)e.Item.FindControl("RotatesBackwardsEditCheckBox");

							if (rotatesBackwardsCheckBox != null)
							{
								rotatesBackwards = rotatesBackwardsCheckBox.Checked;
							}

							var receiptMeterCheckBox = (FMCheckBox)e.Item.FindControl("ReceiptMeterEditCheckBox");

							if (receiptMeterCheckBox != null)
							{
								receiptMeter = receiptMeterCheckBox.Checked;
							}

							map.Meter.ID = id;
							map.Meter.ReceiptMeterFlag = receiptMeter;
							map.Meter.RotatesBackwardsFlag = rotatesBackwards;
							map.Meter.NumberOfDigits = numberOfDigits;
						}
						else
						{
							// the user has erased the meter
							map.Meter = null;
						}
					}

					var productsDropDownList = (DropDownList)e.Item.FindControl("ProductsDropDownList");
					if (productsDropDownList.SelectedIndex != -1)
					{
						map.AssignedGuid = FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, Guid.Parse(productsDropDownList.SelectedValue)));
						map.AssignedID = productsDropDownList.SelectedItem.Text;

						var locationDropDownList = (DropDownList)e.Item.FindControl("LocationDropDownList");
						if (locationDropDownList != null && locationDropDownList.SelectedIndex != -1)
						{
							map.TankOrGroupGuid = Guid.Parse(locationDropDownList.SelectedValue);
							map.TankOrGroupID = locationDropDownList.SelectedItem.Text;
						}
						else
						{
							map.TankOrGroupGuid = Guid.Empty;
							map.TankOrGroupID = "";
						}
					}

					this.MapGrid.EditItemIndex = -1;
					this.EnableControls(true);
					this.UpdatePresetConfigurationView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected abstract void EnableControls(bool enable);

		/// <summary>
		///    This method will enable/disable the OK and Cancel buttons along with
		///    the tabs.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableLoadArmFormControls(bool enable)
		{
			var loadArmForm = (LoadArmForm)this.Page;
			loadArmForm.EnableControls(enable);
		}

		protected ListItemCollection EnumerateLocationTypes()
		{
		    var typeItems = new ListItemCollection { new ListItem("Tank", "Tank"), new ListItem("Group", "Group") };
		    return typeItems;
		}

		protected ListItemCollection EnumerateLocations()
		{
			var locationItems = new ListItemCollection();

			try
			{
                // Tank enumeration functions expect a product master record guid, as that is the key used for mapping
			    Guid productMasterGuid =
			        FMChannelHelper.MakeCall<IProducts, Guid>(x => x.GetMasterRecordGuid(this.Security, this.productGuid));

				if (this.type == "Tank")
				{
					List<TankClass> collection =
						FMChannelHelper.MakeCall<ITanks, List<TankClass>>(x => x.EnumerateByProduct(this.Security, productMasterGuid));

					foreach (TankClass o in collection)
					{
						BaseDataObject cdo = o;

						var newLocationItem = new ListItem(cdo.ID, cdo.IdentityGuid.ToString());
						foreach (ListItem existingLocationItem in locationItems)
						{
							if (string.Compare(existingLocationItem.Text, newLocationItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = locationItems.IndexOf(existingLocationItem);
								locationItems.Insert(index, newLocationItem);
								newLocationItem = null;
								break;
							}
						}

						if (newLocationItem != null)
						{
							locationItems.Add(newLocationItem);
						}
					}
				}
				else
				{
					List<TankGroupClass> collection =
						FMChannelHelper.MakeCall<ITankGroups, List<TankGroupClass>>(
							x => x.EnumerateByProduct(this.Security, productMasterGuid));

					foreach (TankGroupClass o in collection)
					{
						BaseDataObject cdo = o;

						var newLocationItem = new ListItem(cdo.ID, cdo.IdentityGuid.ToString());
						foreach (ListItem existingLocationItem in locationItems)
						{
							if (String.Compare(existingLocationItem.Text, newLocationItem.Text, StringComparison.Ordinal) > 0)
							{
								int index = locationItems.IndexOf(existingLocationItem);
								locationItems.Insert(index, newLocationItem);
								newLocationItem = null;
								break;
							}
						}

						if (newLocationItem != null)
						{
							locationItems.Add(newLocationItem);
						}
					}
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			if (locationItems.Count == 0)
			{
				if (this.type == "Tank")
				{
					this.ErrorHandler(new Exception("No Tanks Available."));
				}
				else
				{
					this.ErrorHandler(new Exception("No Groups Available."));
				}
			}

			return locationItems;
		}

		/// <summary>
		///    This method is used to populate the Meter ID combo box. It lists all meters assigned on the component tab
		/// </summary>
		/// <returns>An arraylist containing the meters defined for this load arm</returns>
		protected ICollection EnumerateMeters()
		{
		    var meters = new ArrayList { string.Empty };


		    //do not allow the injector tab to share meter information
			if (this.PageMapType != PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				StationClass station = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

				if (loadArm != null)
				{
					foreach (ProductMapClass map in loadArm.ComponentCollection)
					{
						if (!string.IsNullOrEmpty(map.Meter?.ID))
						{
							if (!meters.Contains(map.Meter.ID))
							{
								meters.Add(map.Meter.ID);
							}
						}
					}
				}
			}

			return meters;
		}

		protected abstract ICollection EnumeratePresetConfiguration();

		protected ListItemCollection EnumerateProducts()
		{
			var productItems = new ListItemCollection();

			try
			{
				int iIndex = this.MapGrid.CurrentPageIndex * this.MapGrid.PageSize + this.MapGrid.EditItemIndex;
				var mapDataView = (DataView)this.MapGrid.DataSource;

			    var maps = this.PageMaps;

				ProductCollectionClass productCollection =
					FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));

			    // ReSharper disable once ForCanBeConvertedToForeach
				for (int iItem = 0; iItem < productCollection.Count; iItem++)
				{
					ProductClass product = productCollection[iItem];

					switch (this.PageMapType)
					{
					    case PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP:
					        if (product.ProductType == ProductType.AdditiveProduct)
					        {
					            continue;
					        }
					        break;
					    case PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP:
					        {
					            if (product.ProductType != ProductType.AdditiveProduct)
					            {
					                continue;
					            }

					            TankCollectionClass tankCollection = this.EnumerateByProduct(this.Security, product.MasterRecordGuid);
					            if (tankCollection.Count == 0)
					            {
					                continue;
					            }
					        }
					        break;
					    case PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP:
                        case PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP:
                        case PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP:
                            {
					            if (product.ProductType != ProductType.ComponentProduct)
					            {
					                continue;
					            }

					            TankCollectionClass tankCollection = this.EnumerateByProduct(this.Security, product.MasterRecordGuid);
					            if (tankCollection.Count == 0)
					            {
					                continue;
					            }
					        }
					        break;
                    }

					for (int iExistingItem = 0; iExistingItem < maps.Count; iExistingItem++)
					{
						ProductMapClass map = maps[iExistingItem];

						if (this.MapGrid.EditItemIndex != -1 && iExistingItem == (int)mapDataView[iIndex][0])
						{
							continue;
						}

						if (product.MasterRecordGuid == map.AssignedGuid)
						{
							product = null;
							break;
						}
					}

					if (product == null)
					{
						continue;
					}

					var newProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString());
					foreach (ListItem existingProductItem in productItems)
					{
						if (string.Compare(existingProductItem.Text, newProductItem.Text, StringComparison.Ordinal) > 0)
						{
							int index = productItems.IndexOf(existingProductItem);
							productItems.Insert(index, newProductItem);
							newProductItem = null;
							break;
						}
					}

					if (newProductItem != null)
					{
						productItems.Add(newProductItem);
					}
				}

				// Set ProductGuid to first Product
				if (this.productGuid == Guid.Empty && productItems.Count != 0)
				{
					this.productGuid = Guid.Parse(productItems[0].Value);
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}

			if (productItems.Count == 0)
			{
				throw (new Exception("No Products Available."));
			}

			return productItems;
		}

		private TankCollectionClass EnumerateByProduct(SecurityClass securityClass, Guid guid)
		{
			return FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.EnumerateByProduct(securityClass, guid)
																);
		}

		/// <summary>
		///    Fires when the user changes the Meter ID combo box. Attempts to find a meter ID matching
		///    the one the user input, and if one is found, copies the information from that row
		/// </summary>
		/// <param name="sender">The combo box control</param>
		/// <param name="e">not used</param>
		// ReSharper disable once InconsistentNaming
		protected void MeterIDComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			var comboBox = (FMComboBox)sender;

			DataGridItem item = this.MapGrid.Items[this.MapGrid.EditItemIndex];

			if (comboBox != null && item != null)
			{
				string inputMeterID = comboBox.SelectedItem.ToString();

				ProductMapClass matchingMeterMap = this.FindMatchingMeterInformation(inputMeterID);

				if (matchingMeterMap?.Meter != null)
				{
					var rotatesBackwardsCheckBox = (FMCheckBox)item.FindControl("RotatesBackwardsEditCheckBox");

					if (rotatesBackwardsCheckBox != null)
					{
						rotatesBackwardsCheckBox.Checked = matchingMeterMap.Meter.RotatesBackwardsFlag;
					}

					var receiptMeterCheckBox = (FMCheckBox)item.FindControl("ReceiptMeterEditCheckBox");

					if (receiptMeterCheckBox != null)
					{
						receiptMeterCheckBox.Checked = matchingMeterMap.Meter.ReceiptMeterFlag;
					}

					var numberOfDigitsTextBox = (FMTextBox)item.FindControl("NumberOfDigitsTextBox");

					if (numberOfDigitsTextBox != null)
					{
						numberOfDigitsTextBox.Text = matchingMeterMap.Meter.NumberOfDigits.ToString();
					}
				}
			}
		}

		protected void UpdatePresetConfigurationView()
		{
			this.MapGrid.DataSource = this.EnumeratePresetConfiguration();
			this.MapGrid.DataBind();
		}

		/// <summary>
		///    Find a meter assigned to the load arm with an ID matching the input parameter,
		///    Ignoring the one that's currently being edited
		/// </summary>
		/// <param name="meterId">The meter ID to search for</param>
		/// <returns>A product map with a matching meter ID</returns>
		private ProductMapClass FindMatchingMeterInformation(string meterId)
		{
			// Go through all of the product maps in the current page and look for one that has a meter ID
			// matching the value the user picked in the combo box.
			for (int i = 0; i < this.PageMaps.Count; i++)
			{
				ProductMapClass existingMap = this.PageMaps[i];

				// If we found a matching meter ID and it's not the same row as the row the user is editing,
				// return the product map
				if (existingMap.Meter != null && existingMap.Meter.ID == meterId && i != this.MapGrid.EditItemIndex)
				{
					return existingMap;
				}
			}

			return null;
		}

		#endregion
	}
}