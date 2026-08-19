/******************************************************************************

	FILE NAME:		LoadArmExternalComponentPage.ascx.cs


	PURPOSE:			Implementation of LoadArmExternalComponentPage


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
	///		Summary description for LoadArmExternalComponentPage.
	/// </summary>
	public partial class LoadArmExternalComponentPage : LoadArmPageBase
	{

		protected override DataGrid MapGrid
		{
			get{return DataGrid;}
		}

		protected override PRODUCT_MAP_TYPE PageMapType
		{
			get{return PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP;}
		}

		protected override ProductMapCollectionClass PageMaps
		{
			get
			{
				StationClass Station = Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
				return LoadArm.ExternalComponentCollection;
			}
			set
			{
				StationClass Station = Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)Session["LoadArmIndex"]];
				LoadArm.ExternalComponentCollection = value;
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
			MapDataTable.Columns.Add("ProductID", typeof(string));
            MapDataTable.Columns.Add("Type", typeof(string));
            MapDataTable.Columns.Add("LocationID", typeof(string));
			MapDataTable.Columns.Add("PermissivesClick");
			MapDataTable.Columns.Add("InputsClick");

			if (Maps != null)
			{
				ProductMapClass Map;
				for (int iItem = 0; iItem < Maps.Count; iItem++)
				{
					Map = (ProductMapClass)Maps[iItem];

					MapDataRow = MapDataTable.NewRow();

					MapDataRow["Index"] = iItem;
					MapDataRow["PresetNumber"] = Map.PresetNumber;
					MapDataRow["MeterID"] = Map.MeterID;
					MapDataRow["ProductID"] = Map.AssignedID;
					MapDataRow["LocationID"] = Map.TankOrGroupID;
					MapDataRow["PermissivesClick"] = "PermissivesButton_Click('LoadArmExternalComponent'," + iItem.ToString() + ")";
					MapDataRow["InputsClick"] = "InputsButton_Click(" + iItem.ToString() + ")";

                    if (Map.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
                        MapDataRow["Type"] = this.GetTranslatedText("Tank");
                    else
                        MapDataRow["Type"] = GetTranslatedText("Group");

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
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
