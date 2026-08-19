/******************************************************************************

	FILE NAME:		StationLoadArmsForm.ascx.cs


	PURPOSE:			Implementation of StationLoadArmsForm


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
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using FMBusinessObjects.DataObjects;


using FMControls;

namespace FuelsManager.FMWebApp
{
	/// <summary>
	/// Summary description for StationLoadArmsPage.
	/// </summary>
	public partial class StationLoadArmsPage : FMUserControlBase
	{

		public void UpdateView()
		{
			this.LoadArmsDataGrid.DataSource = this.EnumerateLoadArms();
			this.LoadArmsDataGrid.DataBind();

			this.DisableFirstUpButtonAndLastDownButton();
			this.SetAddButtonEnabled();
		}

		/// <summary>
		/// Disable the "up" button for the first arm in the list and the "down" button for the last arm in the list, 
		/// since it doesnt make sense to move the first arm up or the last arm down.
		/// </summary>
		private void DisableFirstUpButtonAndLastDownButton()
		{
			try
			{
				if (this.LoadArmsDataGrid == null || this.LoadArmsDataGrid.Items == null || this.LoadArmsDataGrid.Items.Count == 0)
				{
					return;
				}

				int totalRows = this.LoadArmsDataGrid.Items.Count;

				if (this.LoadArmsDataGrid.Items[0].Cells.Count >= 9 && this.LoadArmsDataGrid.Items[0].Cells[7] != null)//bds
				{
					FMButton theFirstUpButton = this.LoadArmsDataGrid.Items[0].Cells[7].FindControl("UpButton") as FMButton;//bds

					if (theFirstUpButton != null)
					{
						theFirstUpButton.Enabled = false;
					}
				}

				if (this.LoadArmsDataGrid.Items[totalRows - 1].Cells.Count >= 9 && this.LoadArmsDataGrid.Items[totalRows - 1].Cells[7] != null)//bds
				{
					FMButton theLastDownButton = this.LoadArmsDataGrid.Items[totalRows - 1].Cells[7].FindControl("DownButton") as FMButton;//bds

					if (theLastDownButton != null)
					{
						theLastDownButton.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		private ICollection EnumerateLoadArms()
		{
			// Enumerate 
			StationClass station = (StationClass)this.Session["Station"];
			DataTable loadArmDataTable = new DataTable();
			DataRow loadArmDataRow;

			loadArmDataTable.Columns.Add("Index", typeof(int));
			loadArmDataTable.Columns.Add("ArmNumber", typeof(int));
			loadArmDataTable.Columns.Add("LoadRackText", typeof(string));
			loadArmDataTable.Columns.Add("PresetTypeID", typeof(string));
			loadArmDataTable.Columns.Add("OPCServer", typeof(string));
			loadArmDataTable.Columns.Add("OPCItemID", typeof(string));

			int item = 0;
			foreach (LoadArmClass loadArm in station.LoadArmCollection)
			{
				loadArmDataRow = loadArmDataTable.NewRow();

				loadArmDataRow["Index"] = item;
				loadArmDataRow["ArmNumber"] = station.SwingArmPosition == "A" ? loadArm.BayAArmNumber : loadArm.BayBArmNumber;

				loadArmDataRow["LoadRackText"] = loadArm.LoadRackText;
				loadArmDataRow["PresetTypeID"] = LoadArmClass.PresetTypeID(loadArm.PresetType);
				ProcessVariableClass processVariable = loadArm.ProcessVariableCollection[0];
				loadArmDataRow["OPCServer"] = processVariable.ProgID;
				loadArmDataRow["OPCItemID"] = processVariable.OPCItemID;
				loadArmDataTable.Rows.Add(loadArmDataRow);
				item++;
			}
			DataView loadArmDataView = new DataView(loadArmDataTable);
			return loadArmDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					StationClass station = (StationClass)this.Session["Station"];
					if (station.Type != STATION_TYPE.LOAD_RACK &&
						station.Type != STATION_TYPE.OFF_LOADING)
					{
						return;
					}

					if (this.Session["PageIndex"] != null)
					{
						this.LoadArmsDataGrid.CurrentPageIndex = (int)this.Session["PageIndex"];
						if (this.LoadArmsDataGrid.CurrentPageIndex * this.LoadArmsDataGrid.PageSize > station.LoadArmCollection.Count)
						{
							this.LoadArmsDataGrid.CurrentPageIndex--;
						}
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SetAddButtonEnabled()
		{
			StationClass station = (StationClass)this.Session["Station"];

			switch (station.InterfaceType)
			{
				case STATION_INTERFACE_TYPE.MICROLOAD_NET:
				case STATION_INTERFACE_TYPE.MULTILOAD_II_SMP:
					this.AddButton.Enabled = station.LoadArmCollection.Count < 1;
					break;
				case STATION_INTERFACE_TYPE.CONTREC1010:
					this.AddButton.Enabled = station.LoadArmCollection.Count < 4;
					break;
				case STATION_INTERFACE_TYPE.CONTREC1010_RA:
					this.AddButton.Enabled = station.LoadArmCollection.Count < 1;
					break;
				case STATION_INTERFACE_TYPE.MULTILOAD_II:
					this.AddButton.Enabled = station.LoadArmCollection.Count < 11;
					break;
				case STATION_INTERFACE_TYPE.ACCULOADIII_Q:
					if (station.Type == STATION_TYPE.OFF_LOADING)
					{
						this.AddButton.Enabled = station.LoadArmCollection.Count < 1;
					}
					else
					{
						this.AddButton.Enabled = station.LoadArmCollection.Count < 6;
					}
					break;
				default:
					this.AddButton.Enabled = false;
					break;
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
			this.LoadArmsDataGrid.ItemCommand += new DataGridCommandEventHandler(this.LoadArmsDataGrid_ItemCommand);
			this.LoadArmsDataGrid.EditCommand += new DataGridCommandEventHandler(this.LoadArmsDataGrid_EditCommand);
			this.LoadArmsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.LoadArmsDataGrid_PageIndexChanged);
			this.LoadArmsDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.LoadArmsDataGrid_DeleteCommand);
			this.AddButton.Command += new CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		private void LoadArmsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			TableCell indexCell = e.Item.Cells[1];//bds

			((StationForm)this.Page).UpdateData();

			this.Session["LoadArmIndex"] =Convert.ToInt32(indexCell.Text);
			this.Session["TabIndex"] = 9;
			this.Session["PageIndex"] = this.LoadArmsDataGrid.CurrentPageIndex;
			this.Redirect("LoadArmForm.aspx");
		}

		private void LoadArmsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				int item = Convert.ToInt32(indexCell.Text);
				StationClass station = (StationClass)this.Session["Station"];
				station.LoadArmCollection.RemoveAt(item);

				// Resequence Arm Numbers
				for (; item < station.LoadArmCollection.Count; item++)
				{
					if (station.SwingArmPosition == "A")
					{
						station.LoadArmCollection[item].BayAArmNumber--;
					}
					else
					{
						station.LoadArmCollection[item].BayBArmNumber--;
					}
				}

				this.LoadArmsDataGrid.SelectedIndex = -1;
				if (this.LoadArmsDataGrid.Items.Count == 1
				&& this.LoadArmsDataGrid.CurrentPageIndex > 0)
				{
					this.LoadArmsDataGrid.CurrentPageIndex--;
				}

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			StationClass station = (StationClass)this.Session["Station"];
			LoadArmClass loadArm = new LoadArmClass();
			if (station.SwingArmPosition == "A")
			{
				loadArm.BayAStationGuid = station.IdentityGuid;
				loadArm.BayAArmNumber = station.LoadArmCollection.Count + 1;
			}
			else
			{
				loadArm.BayBStationGuid = station.IdentityGuid;
				loadArm.BayBArmNumber = station.LoadArmCollection.Count + 1;
			}

			station.LoadArmCollection.Add(loadArm);

			this.LoadArmsDataGrid.CurrentPageIndex = (station.LoadArmCollection.Count - 1) / this.LoadArmsDataGrid.PageSize;

			((StationForm)this.Page).UpdateData();

			this.Session["LoadArmIndex"] = station.LoadArmCollection.Count-1;
			this.Session["TabIndex"] = 9;
			this.Session["PageIndex"] = this.LoadArmsDataGrid.CurrentPageIndex;
			this.Redirect("LoadArmForm.aspx");
		}

		private void LoadArmsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.LoadArmsDataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.LoadArmsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void UpButton_Command(object sender, DataGridCommandEventArgs e)
		{
			StationClass station = (StationClass)this.Session["Station"];
			LoadArmCollectionClass loadArmCollection = station.LoadArmCollection;

			if (loadArmCollection.Count == 0)
			{
				return;
			}

			//if the user somehow tried to move the first arm in the collection up, return
			if (e.Item.DataSetIndex == 0)
			{
				return;
			}

			LoadArmClass SelectedLoadArm = loadArmCollection[e.Item.DataSetIndex];
			loadArmCollection.RemoveAt(e.Item.DataSetIndex);
			LoadArmCollectionClass ScratchLoadArmCollection = new LoadArmCollectionClass();

			int Sequence = 1;
			for (int Index = 0; Index < loadArmCollection.Count; Index++)
			{
				if (Index == e.Item.DataSetIndex - 1)
				{
					if (station.SwingArmPosition == "A")
					{
						SelectedLoadArm.BayAArmNumber = Sequence;
					}
					else
					{
						SelectedLoadArm.BayBArmNumber = Sequence;
					}

					Sequence++;
					ScratchLoadArmCollection.Add(SelectedLoadArm);
				}
				LoadArmClass LoadArm = loadArmCollection[Index];
				if (station.SwingArmPosition == "A")
				{
					LoadArm.BayAArmNumber = Sequence;
				}
				else
				{
					LoadArm.BayBArmNumber = Sequence;
				}

				ScratchLoadArmCollection.Add(LoadArm);
				Sequence++;
			}
			station.LoadArmCollection = ScratchLoadArmCollection;
			this.UpdateView();
		}

		private void DownButton_Command(object sender, DataGridCommandEventArgs e)
		{
			StationClass station = (StationClass)this.Session["Station"];
			LoadArmCollectionClass loadArmCollection = station.LoadArmCollection;

			if (loadArmCollection.Count == 0)
			{
				return;
			}

			//if the user somehow tried to move the last arm in the collection down, return
			if (e.Item.DataSetIndex >= loadArmCollection.Count - 1)
			{
				return;
			}

			LoadArmClass selectedLoadArm = loadArmCollection[e.Item.DataSetIndex];
			loadArmCollection.RemoveAt(e.Item.DataSetIndex);
			LoadArmCollectionClass scratchLoadArmCollection = new LoadArmCollectionClass();

			int sequence = 1;
			int index = 0;
			foreach (LoadArmClass loadArm in loadArmCollection)
			{
				if (station.SwingArmPosition == "A")
				{
					loadArm.BayAArmNumber = sequence;
				}
				else
				{
					loadArm.BayBArmNumber = sequence;
				}

				scratchLoadArmCollection.Add(loadArm);
				sequence++;
				if (index == e.Item.DataSetIndex)
				{
					if (station.SwingArmPosition == "A")
					{
						selectedLoadArm.BayAArmNumber = sequence;
					}
					else
					{
						selectedLoadArm.BayBArmNumber = sequence;
					}

					sequence++;
					scratchLoadArmCollection.Add(selectedLoadArm);
				}
				index++;
			}
			station.LoadArmCollection = scratchLoadArmCollection;
			this.UpdateView();
		}

		private void LoadArmsDataGrid_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.CommandName == "UpButton")
			{
				this.UpButton_Command(source, e);
			}

			else if (e.CommandName == "DownButton")
			{
				this.DownButton_Command(source, e);
			}
		}
	}
}
