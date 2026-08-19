/******************************************************************************

	FILE NAME:		LoadArmRecipePage.ascx.cs


	PURPOSE:			Implementation of LoadArmRecipePage


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
	using FMBusinessObjects.DataObjects;
	using FMControls;
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	/// <summary>
	/// Summary description for LoadArmRecipePage.
	/// </summary>
	public partial class LoadArmRecipePage : LoadArmPageBase
	{

		protected override DataGrid MapGrid
		{
			get{return this.DataGrid;}
		}

		protected override PRODUCT_MAP_TYPE PageMapType
		{
			get{return PRODUCT_MAP_TYPE.PRESET_RECIPE_MAP;}
		}

		protected override ProductMapCollectionClass PageMaps
		{
			get
			{
				StationClass Station = this.Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				return LoadArm.ProductRecipeCollection;
			}
			set
			{
				StationClass Station = this.Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				LoadArm.ProductRecipeCollection = value;
			}
		}

		protected override ICollection EnumeratePresetConfiguration()
		{
			ProductMapCollectionClass Maps;
			Maps = (ProductMapCollectionClass)this.PageMaps;

			DataTable MapDataTable = new DataTable();
			DataRow MapDataRow;

			MapDataTable.Columns.Add("Index", typeof(Int32));
         MapDataTable.Columns.Add("EnableRecipe", typeof(bool));
			MapDataTable.Columns.Add("PresetNumber", typeof(string));
			MapDataTable.Columns.Add("ProductID", typeof(string));
			MapDataTable.Columns.Add("PermissivesClick");

			StationClass Station = this.Session["Station"] as StationClass;

			if (Maps != null)
			{
				ProductMapClass Map;
				for (int iItem = 0; iItem < Maps.Count; iItem++)
				{
					Map = (ProductMapClass)Maps[iItem];

					MapDataRow = MapDataTable.NewRow();

					MapDataRow["Index"] = iItem;
               MapDataRow["EnableRecipe"] = Map.EnableRecipe;
					MapDataRow["PresetNumber"] = Map.PresetNumber;
					MapDataRow["ProductID"] = Map.AssignedID;
					MapDataRow["PermissivesClick"] = "PermissivesButton_Click('LoadArmRecipe'," + iItem.ToString() + ")";

					if (!Station.EnableDynamicRecipes)
					{
						int Row = 0;
						int rowToAddPresetNumber = Convert.ToInt32(MapDataRow["PresetNumber"]);

						foreach (DataRow ExistingMapDataRow in MapDataTable.Rows)
						{
							int existingRowPresetNumber = Convert.ToInt32(ExistingMapDataRow["PresetNumber"]);

							if (rowToAddPresetNumber < existingRowPresetNumber)
							{
								MapDataTable.Rows.InsertAt(MapDataRow, Row);
								MapDataRow = null;
								break;
							}
							Row++;
						}
					}

					if (MapDataRow != null)
						MapDataTable.Rows.Add(MapDataRow);
				}

				if (Station != null && Station.Type == STATION_TYPE.LOAD_RACK && Station.EnableDynamicRecipes)
				{
					TemplateColumn presetColumn = (TemplateColumn)MapGrid.Columns[3];
					presetColumn.EditItemTemplate = null;

					for (int i = 0; i < MapDataTable.Rows.Count; i++)
					{
						(MapDataTable.Rows[i])["PresetNumber"] = "Dynamic";
					}
				}
			}
			DataView MapDataView = new DataView(MapDataTable);
			return MapDataView;
		}


		protected override void EnableControls(bool Enable)
		{
			StationClass Station = this.Session["Station"] as StationClass;
			LoadArmClass LoadArm = Station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
			if (Enable == false &&
				LoadArm.PresetType != PRESET_TYPE.CONTREC1010 &&
				LoadArm.PresetType != PRESET_TYPE.CONTREC1010_RA &&
				LoadArm.PresetType != PRESET_TYPE.MULTILOAD_II)
				this.AddButton.Enabled = false;
			else if ((LoadArm.PresetType == PRESET_TYPE.CONTREC1010
				|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
				&& LoadArm.ProductRecipeCollection.Count >= 1)
				this.AddButton.Enabled = false;
			else if (LoadArm.PresetType == PRESET_TYPE.MULTILOAD_II
				&& LoadArm.ProductRecipeCollection.Count >= 8)
				this.AddButton.Enabled = false;
			else
				this.AddButton.Enabled = true;
			this.EnableLoadArmFormControls(Enable);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (! this.Page.IsPostBack) 
				{
					this.UpdatePresetConfigurationView();

					StationClass Station = this.Session["Station"] as StationClass;
					LoadArmClass LoadArm = Station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
					if ((LoadArm.PresetType == PRESET_TYPE.CONTREC1010
						|| LoadArm.PresetType == PRESET_TYPE.CONTREC1010_RA)
						&& LoadArm.ProductRecipeCollection.Count >= 1)
						this.AddButton.Enabled = false;
					else if (LoadArm.PresetType == PRESET_TYPE.MULTILOAD_II
						&& LoadArm.ProductRecipeCollection.Count >= 8)
						this.AddButton.Enabled = false;
					else
						this.AddButton.Enabled = true;
				}
				else
				{
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
			this.DataGrid.ItemCreated += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DataGrid_ItemCreated);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void DataGrid_ItemCreated(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if(e.Item.ItemType == ListItemType.Header)
			{
				StationClass Station = this.Session["Station"] as StationClass;
				LoadArmClass LoadArm = Station.LoadArmCollection[(int)this.Session["LoadArmIndex"]];
				if (LoadArm.PresetType == PRESET_TYPE.DANLOAD6000)
					e.Item.Cells[2].Text=this.GetTranslatedText("Sequence/Low Proportion");//bds
				else
					e.Item.Cells[2].Text=this.GetTranslatedText("Recipe");//bds
			}
		}
	}
}
