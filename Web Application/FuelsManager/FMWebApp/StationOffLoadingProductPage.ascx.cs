/******************************************************************************

	FILE NAME:		StationOffLoadingProductPage.ascx.cs


	PURPOSE:			Implementation of Station Offload product configuration page.


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaLoadArm.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.BusinessInterfaces;

    using FMControls;

    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	///		Code-behind for the offloading products page for DET offloading stations
	///     To simplify the model, the DET will be assumed to have a single, phantom load arm
	/// </summary>
    public partial class StationOffLoadingProductPage : FMUserControlBase
    {
        private Guid productGuid;
        private Guid locationGuid;
        private string type;
        private string meterID;
        private string presetNumber;
        private string rollOver;
        private bool internalMeter;

        protected DataGrid MapGrid => this.DataGrid;

        protected PRODUCT_MAP_TYPE PageMapType => PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP;

        protected ProductMapCollectionClass PageMaps
		{
			get
			{
			    var station = (StationClass)this.Session["Station"];
			    if (station?.LoadArmCollection == null || station.LoadArmCollection.Count < 1)
			    {
			        return null;
                }

			    var loadArm = station.LoadArmCollection[0];

			    return loadArm?.OffloadExternalProductCollection;
			}

			set
			{
			    var station = (StationClass)this.Session["Station"];
                if (station == null)
                {
                    throw new NullReferenceException("No station in session.");
                }

                if (station.LoadArmCollection == null)
                {
                    station.LoadArmCollection = new LoadArmCollectionClass();
                }

			    LoadArmClass loadArm;
                if (station.LoadArmCollection.Count == 0)
                {
                    loadArm = new LoadArmClass
                                  {
                                      BayAStationGuid = station.IdentityGuid, 
                                      BayAArmNumber = station.LoadArmCollection.Count + 1,
                                      PresetType = PRESET_TYPE.VARECDET
                                  };
                    station.LoadArmCollection.Add(loadArm);
                }
                else
                {
                    loadArm = station.LoadArmCollection[0]; // VarecDET supports only one arm for offload.
                }

                loadArm.OffloadExternalProductCollection = value;
			}
		}

		protected ICollection EnumeratePresetConfiguration()
		{
		    ProductMapCollectionClass maps = this.PageMaps;

			var mapDataTable = new DataTable();

		    mapDataTable.Columns.Add("Index", typeof(int));
            mapDataTable.Columns.Add("MeterID", typeof(string));
            mapDataTable.Columns.Add("ProductID", typeof(string));
			mapDataTable.Columns.Add("LocationID", typeof(string));
            mapDataTable.Columns.Add("InputsClick");
            mapDataTable.Columns.Add("PermissivesClick");

			if (maps != null)
			{
			    for (int item = 0; item < maps.Count; item++)
				{
					ProductMapClass map = maps[item];

					DataRow mapDataRow = mapDataTable.NewRow();

					int index = 0;
					mapDataRow[index++] = item;
                    mapDataRow[index++] = map.Meter == null ? string.Empty : map.Meter.ID;
                    mapDataRow[index++] = map.AssignedID;
					mapDataRow[index++] = map.TankOrGroupID;
					mapDataRow[index++] = "InputsButton_Click(" + item.ToString() + ")";
                    mapDataRow[index] = "PermissivesButton_Click('OffLoadExternalProduct'," + item.ToString() + ")";

					int row = 0;
					foreach (DataRow existingMapDataRow in mapDataTable.Rows)
					{
						if (!string.IsNullOrEmpty((string)mapDataRow[2]) && string.CompareOrdinal((string)mapDataRow[2], (string)existingMapDataRow[2]) < 0)
						{
							mapDataTable.Rows.InsertAt(mapDataRow, row);
							mapDataRow = null;
							break;
						}

						row++;
					}

				    if (mapDataRow != null)
				    {
				        mapDataTable.Rows.Add(mapDataRow);
				    }
				}
			}

			var mapDataView = new DataView(mapDataTable);
			return mapDataView;
		}

		protected void EnableControls(bool enable)
		{
		    this.AddButton.Enabled = enable;

            this.EnableStationFormControls(enable);
        }

        private void EnableStationFormControls(bool enable)
        {
            var stationForm = (StationForm)this.Page;
            stationForm.EnableControls(enable);
        }

        protected void UpdatePresetConfigurationView()
        {
            this.MapGrid.DataSource = this.EnumeratePresetConfiguration();
            this.MapGrid.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			    if (!this.Page.IsPostBack)
                {
                    var station = (StationClass)this.Session["Station"];
                    if (station.Type != STATION_TYPE.OFF_LOADING
                        || station.InterfaceType != STATION_INTERFACE_TYPE.VAREC_DET)
                    {
                        return;
                    }

                    if (station.LoadArmCollection == null)
                    {
                        station.LoadArmCollection = new LoadArmCollectionClass();
                    }
                    
                    // If we get here, then we're configuring a Varec DET as an offloading station
                    // (the only scenario where this page should be visible).  The DET interface
                    // inplements the product support using a hidden, or phantom, arm.  This arm
                    // is always Arm 1 (station.LoadArmCollection.Item(0).
                    //
                    // We must always force this.Session["LoadArm"] to point to this phantom arm here;
                    // even, nay _especially_, if it has been previously set.  Coming here after configuring
                    // Load Racks or more complex offloading stations with real arms will see this.Session["LoadArm"]
                    // referring to a potential load arm 2 or greater, which no longer exists.  This will cause
                    // Index Out Of Range errors being thrown by the Input and Permissive configuration popups.
                    if (station.LoadArmCollection.Count != 0)
                    {
                        this.Session["LoadArmIndex"] = 0;
                    }

			        this.UpdatePresetConfigurationView();
				}
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.DataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridEditCommand);
			this.DataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGridPageIndexChanged);
			this.DataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridCancelCommand);
			this.DataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridUpdateCommand);
			this.DataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGridDeleteCommand);
			this.DataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
		}
		#endregion

        protected void AddButtonCommand(object sender, CommandEventArgs e)
        {
            this.productGuid = Guid.Empty;
            this.locationGuid = Guid.Empty;
            this.type = "Tank";
            ProductMapCollectionClass maps = this.PageMaps;
            if (maps == null)
            {
                maps = new ProductMapCollectionClass();
                this.PageMaps = maps;
            }

            var map = new ProductMapClass { Type = this.PageMapType };
            if (maps.Count != 0)
            {
                map.PresetNumber = maps[maps.Count - 1].PresetNumber + 1;
            }
            else
            {
                map.PresetNumber = 1;
            }

            maps.Add(map);
            this.presetNumber = map.PresetNumber.ToString(CultureInfo.InvariantCulture);
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

        protected void DataGridEditCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                var dataGrid = (DataGrid)source;
                dataGrid.EditItemIndex = e.Item.ItemIndex;
                this.EnableControls(false);
                ProductMapCollectionClass maps = this.PageMaps;
                ProductMapClass map = maps[Convert.ToInt32(indexLabel.Text)];
                this.productGuid = map.AssignedGuid;
                this.locationGuid = map.TankOrGroupGuid;
                this.presetNumber = map.PresetNumber.ToString(CultureInfo.InvariantCulture);
                this.meterID = map.Meter == null ? string.Empty : map.Meter.ID;

                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                     x =>
                                                                     x.Get(this.Security, this.Security.SiteGuid, false, false, false)
                                                                );

                ProcessVariableClass internalMeterPv = map.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
                this.internalMeter = internalMeterPv != null;
                this.rollOver = (internalMeterPv == null) ? string.Empty : internalMeterPv.Encode(
                    internalMeterPv.GetMaximum(EngineeringUnit.FmvMeter3, site._AdditiveVolumeDecimalPlaces),
                    Quality.Good,
                    site.AdditiveVolumeUnits,
                    site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME));

                if (map.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
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

        protected void DataGridCancelCommand(object source, DataGridCommandEventArgs e)
        {
            var indexLabel = (Label)e.Item.FindControl("IndexLabel");
            if (indexLabel != null)
            {
                ProductMapCollectionClass maps = this.PageMaps;
                ProductMapClass map = maps[Convert.ToInt32(indexLabel.Text)];
                if (map.AssignedID == string.Empty)
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

        protected void DataGridUpdateCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                var indexLabel = (Label)e.Item.FindControl("IndexLabel");

                if (indexLabel != null)
                {
                    ProductMapCollectionClass maps = this.PageMaps;
                    ProductMapClass map = maps[Convert.ToInt32(indexLabel.Text)];

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

                    try
                    {
                        var presetNumberTextBox = (TextBox)e.Item.FindControl("PresetNumberTextBox");
                        if (presetNumberTextBox != null)
                        {
                            map.PresetNumber = Convert.ToInt32(presetNumberTextBox.Text);
                        }
                    }
                    catch (Exception)
                    {
                        this.ErrorHandler(new Exception("Injector must be numeric"));
                    }

                    var meterIDTextBox = (TextBox)e.Item.FindControl("MeterIDTextBox");
                    if (meterIDTextBox != null)
                    {
                        map.Meter = this.CreateMeterIfNull(meterIDTextBox.Text, map.Meter);
                        map.MeterID = meterIDTextBox.Text;
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
                            map.TankOrGroupID = string.Empty;
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

        protected void DataGridDeleteCommand(object source, DataGridCommandEventArgs e)
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
                    int localPresetNumber = 0;
                    foreach (ProductMapClass map in maps)
                    {
                        map.PresetNumber = ++localPresetNumber;
                    }
                }

                if (this.MapGrid.Items.Count == 1 && this.MapGrid.CurrentPageIndex > 0)
                {
                    this.MapGrid.CurrentPageIndex--;
                }

                this.UpdatePresetConfigurationView();
            }
        }

        protected void DataGridItemDataBound(object sender, DataGridItemEventArgs e)
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
                        int index = items.IndexOf(items.FindByValue(this.productGuid.ToString().ToLower()));
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
                        internalCheckBox.Checked = this.internalMeter;
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

                    var meterIDTextBox = (TextBox)e.Item.FindControl("MeterIDTextBox");
                    if (meterIDTextBox != null)
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

        protected void DataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
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
        /// This method will create a new meter if necessary and return the meter.
        /// </summary>
        /// <param name="meterId">The meter ID used to create the meter.</param>
        /// <param name="existingMeter">The existing meter from product map object.</param>
        /// <returns>Return null if meter ID is empty or a new meter.</returns>
        protected MeterClass CreateMeterIfNull(string meterId, MeterClass existingMeter)
        {
            if(string.IsNullOrEmpty(meterId))
            {
                return null;
            }

            bool receiptMeterFlag = false;
            bool rotateBackwardsFlag = false;
            int numberOfDigits = 8;

            if (existingMeter == null)
            {
                var meter = new MeterClass
                {
                    ID                      = meterId,
                    ReceiptMeterFlag        = receiptMeterFlag,
                    RotatesBackwardsFlag    = rotateBackwardsFlag,
                    NumberOfDigits          = numberOfDigits
                };

                return meter;
            }

            existingMeter.ID                    = meterId;
            existingMeter.ReceiptMeterFlag      = receiptMeterFlag;
            existingMeter.RotatesBackwardsFlag  = rotateBackwardsFlag;
            existingMeter.NumberOfDigits        = numberOfDigits;

            return existingMeter;
        }

        protected ListItemCollection EnumerateProducts()
	    {
            var productItems = new ListItemCollection();

            try
            {
                int index = (this.MapGrid.CurrentPageIndex * this.MapGrid.PageSize) + this.MapGrid.EditItemIndex;
                var mapDataView = (DataView)this.MapGrid.DataSource;

                ProductMapCollectionClass maps = this.PageMaps;

                ProductCollectionClass productCollection =
                    FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.EnumerateBySite(this.Security));

                for (int item = 0; item < productCollection.Count; item++)
                {
                    ProductClass product = productCollection[item];

                    switch (this.PageMapType)
                    {
                        case PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP:
                            {
                                if (product.ProductType != ProductType.ComponentProduct)
                                {
                                    continue;
                                }

                                var product1 = product;
                                List<TankClass> tankCollection =
                                        FMChannelHelper.MakeCall<ITanks, List<TankClass>>(x => x.EnumerateByProduct(this.Security, product1.MasterRecordGuid));
                                if (tankCollection.Count == 0)
                                {
                                    continue;
                                }
                            }

                            break;
                    }

                    for (int existingItem = 0; existingItem < maps.Count; existingItem++)
                    {
                        ProductMapClass map = maps[existingItem];

                        if (this.MapGrid.EditItemIndex != -1 && existingItem == (int)mapDataView[index][0])
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

                    var newProductItem = new ListItem(product.ID, product.MasterRecordGuid.ToString().ToLower());
                    foreach (ListItem existingProductItem in productItems)
                    {
                        if (string.Compare(existingProductItem.Text, newProductItem.Text, StringComparison.Ordinal) > 0)
                        {
                            int existingItemIndex = productItems.IndexOf(existingProductItem);
                            productItems.Insert(existingItemIndex, newProductItem);
                            newProductItem = null;
                            break;
                        }
                    }

                    if (newProductItem != null)
                    {
                        productItems.Add(newProductItem);
                    }
                }

                // Set ProductIndex to first Product
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
                throw new Exception("No Products Available.");
            }

            return productItems;
        }

	    protected void ProductDropDownListSelectedIndexChanged(object sender, EventArgs e)
	    {
            var productsDropDownList = (DropDownList)sender;
            this.productGuid = Guid.Parse(productsDropDownList.SelectedValue);
            this.locationGuid = Guid.Empty;
            DataGridItem item = this.MapGrid.Items[this.MapGrid.EditItemIndex];

	        var meterIDTextBox = (TextBox)item.FindControl("MeterIDTextBox");
	        if (meterIDTextBox != null)
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

	    protected object EnumerateLocations()
	    {
            var locationItems = new ListItemCollection();

            try
            {
                // Tanks are associated to tanks by product master record guid.
                Guid productMasterGuid =
                    FMChannelHelper.MakeCall<IProducts, Guid>(
                        x => x.GetMasterRecordGuid(this.Security, this.productGuid));

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
    }
}
