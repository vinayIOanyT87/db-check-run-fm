/******************************************************************************

	FILE NAME:		LoadArmInjectorPage.ascx.cs


	PURPOSE:			Implementation of LoadArmInjectorPage


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
    using System.Data;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// Summary description for LoadArmInjectorPage.
	/// </summary>
	public partial class LoadArmInjectorPage : LoadArmPageBase
	{
	
		protected override DataGrid MapGrid => this.DataGrid;

	    protected override PRODUCT_MAP_TYPE PageMapType => PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP;

	    protected override ProductMapCollectionClass PageMaps
		{
			get
			{
				StationClass station = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				return loadArm?.AdditiveInjectorCollection;
			}
			set
			{
				StationClass station = this.Session["Station"] as StationClass;
				LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				loadArm.AdditiveInjectorCollection = value;
			}
		}

		protected override ICollection EnumeratePresetConfiguration()
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
			ProductMapCollectionClass maps = this.PageMaps;

			DataTable mapDataTable = new DataTable();

		    mapDataTable.Columns.Add("Index", typeof(int));
			mapDataTable.Columns.Add("PresetNumber", typeof(int));
			mapDataTable.Columns.Add("MeterID", typeof(string));
			mapDataTable.Columns.Add("NumberOfDigits", typeof(int));
			mapDataTable.Columns.Add("RotatesBackwardsFlag", typeof(bool));
			mapDataTable.Columns.Add("ReceiptMeterFlag", typeof(bool));
			mapDataTable.Columns.Add("Internal", typeof(bool));
			mapDataTable.Columns.Add("RollOver", typeof(string));
			mapDataTable.Columns.Add("ProductID", typeof(string));
			mapDataTable.Columns.Add("LocationID", typeof(string));
			mapDataTable.Columns.Add("PermissivesClick");

			if (maps != null)
			{
			    for (int iItem = 0; iItem < maps.Count; iItem++)
				{
					var map = maps[iItem];

					var mapDataRow = mapDataTable.NewRow();

					mapDataRow["Index"] = iItem;
					mapDataRow["PresetNumber"] = map.PresetNumber;

					if (map.Meter != null)
					{
						mapDataRow["MeterID"] = map.Meter.ID;
						mapDataRow["NumberOfDigits"] = map.Meter.NumberOfDigits;
						mapDataRow["RotatesBackwardsFlag"] = map.Meter.RotatesBackwardsFlag;
						mapDataRow["ReceiptMeterFlag"] = map.Meter.ReceiptMeterFlag;
					}
					else
					{
						mapDataRow["MeterID"] = string.Empty;
						mapDataRow["NumberOfDigits"] = 0;
						mapDataRow["RotatesBackwardsFlag"] = false;
						mapDataRow["ReceiptMeterFlag"] = false;
					}

					ProcessVariableClass internalMeterPv = map.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
					mapDataRow["Internal"] = (internalMeterPv != null);
					mapDataRow["RollOver"] = (internalMeterPv == null) ? "" : internalMeterPv.Encode(
						internalMeterPv.GetMaximum(EngineeringUnit.FmvMeter3, site._AdditiveVolumeDecimalPlaces),
						Quality.Good,
						site.AdditiveVolumeUnits,
						site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.ADDITIVE_VOLUME));


					mapDataRow["ProductID"] = map.AssignedID;
					mapDataRow["LocationID"] = map.TankOrGroupID;
					mapDataRow["PermissivesClick"] = "PermissivesButton_Click('LoadArmAdditive'," + iItem + ")";

					int row = 0;
					foreach (DataRow existingMapDataRow in mapDataTable.Rows)
					{
						if ((int)mapDataRow["PresetNumber"] < (int)existingMapDataRow["PresetNumber"])
						{
							mapDataTable.Rows.InsertAt(mapDataRow, row);
							mapDataRow = null;
							break;
						}
						row++;
					}


					if (mapDataRow != null)
						mapDataTable.Rows.Add(mapDataRow);
				}
			}
			DataView mapDataView = new DataView(mapDataTable);
			return mapDataView;
		}

		protected override void EnableControls(bool enable)
		{
			StationClass station = this.Session["Station"] as StationClass;
			LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
		    if (loadArm == null)
		    {
		        this.AddButton.Enabled = false;
		    }

            else
            {
                if (enable == false &&
                    loadArm.PresetType != PRESET_TYPE.MULTILOAD_II_SMP &&
                    loadArm.PresetType != PRESET_TYPE.MICROLOAD_NET &&
                    loadArm.PresetType != PRESET_TYPE.CONTREC1010 &&
                    loadArm.PresetType != PRESET_TYPE.CONTREC1010_RA &&
                    loadArm.PresetType != PRESET_TYPE.MULTILOAD_II)
                    this.AddButton.Enabled = false;
                else if (loadArm.PresetType == PRESET_TYPE.CONTREC1010)
                    this.AddButton.Enabled = false;
                else if (loadArm.PresetType == PRESET_TYPE.MICROLOAD_NET
                        && loadArm.AdditiveInjectorCollection.Count >= 4)
                    this.AddButton.Enabled = false;
                else if (loadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP
                        && loadArm.AdditiveInjectorCollection.Count >= 2)
                    this.AddButton.Enabled = false;
                else if (loadArm.PresetType == PRESET_TYPE.MULTILOAD_II
                        && loadArm.AdditiveInjectorCollection.Count >= 16)
                    this.AddButton.Enabled = false;
                else if (loadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
                    this.AddButton.Enabled = false;
                else
                    this.AddButton.Enabled = true;
            }

            this.EnableLoadArmFormControls(enable);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{

				if (! this.Page.IsPostBack) 
				{
					this.UpdatePresetConfigurationView();

					StationClass station = this.Session["Station"] as StationClass;
					LoadArmClass loadArm = station?.LoadArmCollection[(int)this.Session["LoadArmIndex"]];

					if(loadArm != null && ((loadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP
					                        && loadArm.AdditiveInjectorCollection.Count >= 2)
					                       || (loadArm.PresetType == PRESET_TYPE.MULTILOAD_II
					                           && loadArm.AdditiveInjectorCollection.Count >= 16)
					                       || (loadArm.PresetType == PRESET_TYPE.MICROLOAD_NET
					                           && loadArm.AdditiveInjectorCollection.Count >= 4)
					                       || loadArm.PresetType == PRESET_TYPE.CONTREC1010
					                       || loadArm.PresetType == PRESET_TYPE.CONTREC1010_RA))
					{
						this.AddButton.Enabled=false;
					}
					else
					{
						this.AddButton.Enabled=true;
				}
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
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.DataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid_EditCommand);
			this.DataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DataGrid_PageIndexChanged);
			this.DataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid_CancelCommand);
			this.DataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid_UpdateCommand);
			this.DataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DataGrid_DeleteCommand);
			this.DataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}

		#endregion
	}
}
