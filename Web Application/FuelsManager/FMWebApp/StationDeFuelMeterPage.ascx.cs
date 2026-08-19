namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;

	using FMBusinessObjects.DataObjects;

	using global::FMWebApp;

	public partial class StationDeFuelMeterPage : FMUserControlBase
	{
		private void UpdateProcessVariablesView()
		{
			this.ProcessVariablesDataGrid.DataSource = this.ProcessVariablesView();
			this.ProcessVariablesDataGrid.DataBind();
		}

		private ICollection ProcessVariablesView()
		{
			DataTable PVDataTable = new DataTable();
			DataRow PVDataRow;

			PVDataTable.Columns.Add("Index", typeof(Int32));
			PVDataTable.Columns.Add("OPCServerID", typeof(string));
			PVDataTable.Columns.Add("OPCItemID", typeof(string));

			StationClass Station = (StationClass)this.Session["Station"];
			int Item = 0;
			foreach (ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
			{
				if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
				{
					Item++;
					continue;
				}

				if (this.Session["ProcessVariable"] is ProcessVariableClass
				&& (this.Session["ProcessVariable"] as ProcessVariableClass).ProcessVariableType == ProcessVariable.ProcessVariableType
				&& (this.Session["ProcessVariable"] as ProcessVariableClass).InstanceNumber == ProcessVariable.InstanceNumber)
				{
					var editedProcessVariable = this.Session["ProcessVariable"] as ProcessVariableClass;
					ProcessVariable.Load(editedProcessVariable);
					this.Session.Remove("ProcessVariable");
				}

				PVDataRow = PVDataTable.NewRow();

				PVDataRow["Index"] = Item;
				PVDataRow["OPCServerID"] = ProcessVariable.ProgID;
				PVDataRow["OPCItemID"] = ProcessVariable.OPCItemID;
				PVDataTable.Rows.Add(PVDataRow);
				Item++;
			}

			DataView PVDataView = new DataView(PVDataTable);
			return PVDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				StationClass Station = (StationClass)this.Session["Station"];
				if (Station.Type != STATION_TYPE.OFF_LOADING)
					return;

				if (!this.Page.IsPostBack)
				{
					this.UpdateProcessVariablesView();
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
			this.ProcessVariablesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ProcessVariablesDataGrid_EditCommand);

		}
		#endregion

		private void ProcessVariablesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			((StationForm)this.Page).UpdateData();
			this.Session["UnitForm"] = "StationForm.aspx";
			StationClass Station = (StationClass)this.Session["Station"];
			this.Session["TabIndex"] = 1;
			this.Session["ProcessVariable"] = Station.ProcessVariableCollection[System.Convert.ToInt32(e.Item.Cells[1].Text)];
			this.Redirect("OPCConnectionForm.aspx");
		}
	}
}