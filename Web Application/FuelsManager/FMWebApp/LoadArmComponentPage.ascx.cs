/******************************************************************************

	FILE NAME:		LoadArmComponentPage.ascx.cs


	PURPOSE:			Implementation of LoadArmComponentPage


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
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

using FMControls;

namespace FuelsManager.FMWebApp
{
	/// <summary>
	/// Summary description for LoadArmComponentPage.
	/// </summary>
	public partial class LoadArmComponentPage : LoadArmPageBase
	{
	
		protected override DataGrid MapGrid
		{
			get{return DataGrid;}
		}

		protected override PRODUCT_MAP_TYPE PageMapType
		{
			get{return PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP;}
		}

		protected override ProductMapCollectionClass PageMaps
		{
			get
			{
				StationClass Station = Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
				return LoadArm.ComponentCollection;
			}
			set
			{
				StationClass Station = Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
				LoadArm.ComponentCollection = value;
			}
		}

		protected override ICollection EnumeratePresetConfiguration()
		{
			ProductMapCollectionClass Maps;
			Maps = (ProductMapCollectionClass)PageMaps;

			DataTable MapDataTable = new DataTable();
			DataRow MapDataRow;

			MapDataTable.Columns.Add("Index", typeof(Int32));
			MapDataTable.Columns.Add("PresetNumber", typeof(Int32));
			MapDataTable.Columns.Add("MeterID", typeof(string));
			MapDataTable.Columns.Add("NumberOfDigits", typeof(Int32));
			MapDataTable.Columns.Add("RotatesBackwardsFlag", typeof(bool));
			MapDataTable.Columns.Add("ReceiptMeterFlag", typeof(bool));
			MapDataTable.Columns.Add("ProductID", typeof(string));
			MapDataTable.Columns.Add("Type", typeof(string));
			MapDataTable.Columns.Add("LocationID", typeof(string));
			MapDataTable.Columns.Add("PermissivesClick");

			if (Maps != null)
			{
				ProductMapClass Map;
				for (int iItem = 0; iItem < Maps.Count; iItem++)
				{
					Map = (ProductMapClass)Maps[iItem];

					MapDataRow = MapDataTable.NewRow();

					MapDataRow["Index"] = iItem;
					MapDataRow["PresetNumber"] = Map.PresetNumber;

					if (Map.Meter != null)
					{
						MapDataRow["MeterID"] = Map.Meter.ID;
						MapDataRow["NumberOfDigits"] = Map.Meter.NumberOfDigits;
						MapDataRow["RotatesBackwardsFlag"] = Map.Meter.RotatesBackwardsFlag;
						MapDataRow["ReceiptMeterFlag"] = Map.Meter.ReceiptMeterFlag;
					}
					else
					{
						MapDataRow["MeterID"] = string.Empty;
						MapDataRow["NumberOfDigits"] = 0;
						MapDataRow["RotatesBackwardsFlag"] = false;
						MapDataRow["ReceiptMeterFlag"] = false;
					}

					MapDataRow["ProductID"] = Map.AssignedID;

					if (Map.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP)
						MapDataRow["Type"] = this.GetTranslatedText("Tank");
					else
						MapDataRow["Type"] = GetTranslatedText("Group");

					MapDataRow["LocationID"] = Map.TankOrGroupID;
					MapDataRow["PermissivesClick"] = "PermissivesButton_Click('LoadArmComponent'," + iItem.ToString() + ")";

					int Row = 0;
					foreach (DataRow ExistingMapDataRow in MapDataTable.Rows)
					{
						if ((int)MapDataRow["PresetNumber"] < (int)ExistingMapDataRow["PresetNumber"])
						{
							MapDataTable.Rows.InsertAt(MapDataRow, Row);
							MapDataRow = null;
							break;
						}
						Row++;
					}


					if (MapDataRow != null)
						MapDataTable.Rows.Add(MapDataRow);
				}
			}
			DataView MapDataView = new DataView(MapDataTable);
			return MapDataView;
		}

		protected override void EnableControls(bool enable)
		{
			StationClass Station = Session["Station"] as StationClass;
			LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
			if ((LoadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP
			|| LoadArm.PresetType == PRESET_TYPE.MICROLOAD_NET
			|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010
			|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
			&& LoadArm.ComponentCollection.Count >= 1)
				AddButton.Enabled=false;
			else if (LoadArm.PresetType == PRESET_TYPE.MULTILOAD_II
			&& LoadArm.ComponentCollection.Count >= 8)
				AddButton.Enabled = false;
			else
				AddButton.Enabled=enable;

			EnableLoadArmFormControls(enable);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (! Page.IsPostBack) 
				{
					UpdatePresetConfigurationView();

					StationClass Station = Session["Station"] as StationClass;
					LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
					if ((LoadArm.PresetType == PRESET_TYPE.MULTILOAD_II_SMP
					|| LoadArm.PresetType == PRESET_TYPE.MICROLOAD_NET
					|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010
					|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
					&& LoadArm.ComponentCollection.Count >= 1)
						AddButton.Enabled=false;
					else
						AddButton.Enabled=true;
				}
				else
				{
				}	
			}	
			catch (Exception except)
			{
				ErrorHandler(except);
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
