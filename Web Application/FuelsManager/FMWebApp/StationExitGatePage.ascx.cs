/******************************************************************************

	FILE NAME:		StationExitGatePage.ascx.cs


	PURPOSE:			Implementation of StationExitGatePage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


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
using System.Runtime.InteropServices;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

using FMControls;

namespace FuelsManager.FMWebApp
{
	/// <summary>
	/// Summary description for StationExitGatePage.
	/// </summary>
	public partial class StationExitGatePage : FMUserControlBase
	{
	
		private void UpdateProcessVariablesView()
		{
			ProcessVariablesDataGrid.DataSource=ProcessVariablesView();
			ProcessVariablesDataGrid.DataBind();
		}

		private ICollection ProcessVariablesView()
		{
			DataTable			PVDataTable=new DataTable();
			DataRow				PVDataRow;

            PVDataTable.Columns.Add("Index", typeof(Int32));
            PVDataTable.Columns.Add("Host", typeof(string));
            PVDataTable.Columns.Add("OPCServerID", typeof(string));
            PVDataTable.Columns.Add("OPCItemID", typeof(string));
            PVDataTable.Columns.Add("MessageID", typeof(string));

            StationClass Station=(StationClass) Session["Station"];
			int Item=0;
			foreach(ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
			{
				if(ProcessVariable.ProcessVariableType	!=	PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
				{
					Item++;
					continue;
				}

				if (Session["ProcessVariable"] is ProcessVariableClass
				&& (Session["ProcessVariable"] as ProcessVariableClass).ProcessVariableType == ProcessVariable.ProcessVariableType
				&& (Session["ProcessVariable"] as ProcessVariableClass).InstanceNumber == ProcessVariable.InstanceNumber)
				{
					var editedProcessVariable = Session["ProcessVariable"] as ProcessVariableClass;
					ProcessVariable.Load(editedProcessVariable);
					Session.Remove("ProcessVariable");
				}


				PVDataRow=PVDataTable.NewRow();

                PVDataRow["Index"] = Item;
                Opc.URL Url = new Opc.URL(ProcessVariable.URL);
                PVDataRow["Host"] = Url.HostName;
                PVDataRow["OPCServerID"] = ProcessVariable.ProgID;
                PVDataRow["OPCItemID"] = ProcessVariable.OPCItemID;
                PVDataRow["MessageID"] = ProcessVariable.MessageID;
                PVDataTable.Rows.Add(PVDataRow);
                Item++;
			}

			DataView		PVDataView=new DataView(PVDataTable);
			return PVDataView;
		}



		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				StationClass Station=(StationClass) Session["Station"];
				if(Station.Type != STATION_TYPE.EXIT_GATE && Station.Type != STATION_TYPE.BOL)
					return;

				if (! Page.IsPostBack) 
				{
					UpdateProcessVariablesView();
                    this.QueryForTrailers.Checked = Station.QueryForTrailers;
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
			this.ProcessVariablesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ProcessVariablesDataGrid_EditCommand);
            this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);
            this.ProcessVariablesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ProcessVariablesDataGrid_DeleteCommand);
            this.ProcessVariablesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ProcessVariablesDataGrid_PageIndexChanged);
        }
		#endregion

		private void ProcessVariablesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Session["UnitForm"]="StationForm.aspx";
			Session["TabIndex"]=3;
			StationClass	Station=(StationClass) Session["Station"];
			Session["ProcessVariable"]=Station.ProcessVariableCollection[System.Convert.ToInt32(e.Item.Cells[1].Text)];
			this.Redirect("OPCConnectionForm.aspx");
		}

        public void UpdateData()
        {
            var station = (StationClass)Session["Station"];
            station.QueryForTrailers = this.QueryForTrailers.Checked;
        }

        private void ProcessVariablesDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            try
            {
                // if we are editing do not allow a page change
                if (ProcessVariablesDataGrid.EditItemIndex > -1)
                    return;
                ProcessVariablesDataGrid.CurrentPageIndex = e.NewPageIndex;
                UpdateProcessVariablesView();
            }
            catch (Exception except)
            {
                ErrorHandler(except);
            }
        }

        private void AddButtonCommand(object sender, CommandEventArgs e)
        {
            SiteClass Site = (SiteClass)Session["Site"];
            int InstanceNumber = 0;
            int numberOfProcessVariables = 0;
            StationClass Station = (StationClass)Session["Station"];

            foreach (ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
            {
                if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
                {
                    continue;
                }

                if (ProcessVariable.InstanceNumber > InstanceNumber)
                {
                    InstanceNumber = ProcessVariable.InstanceNumber;
                }

                numberOfProcessVariables++;
            }

            ProcessVariableClass NewProcessVariable = new ProcessVariableClass();
            NewProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
            NewProcessVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV;
            NewProcessVariable.InstanceNumber = InstanceNumber + 1;
            NewProcessVariable.DataType = VarEnum.VT_BOOL;
            NewProcessVariable.DataTypeEnabled = false;

            Station.ProcessVariableCollection.Add(NewProcessVariable);

            ((StationForm)Page).UpdateData();

            Session["UnitForm"] = "StationForm.aspx";
            Session["TabIndex"] = 3;
            Session["ProcessVariable"] = NewProcessVariable;
            this.Redirect("OPCConnectionForm.aspx");
        }

        private void ProcessVariablesDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            StationClass Station = (StationClass)Session["Station"];
            Station.ProcessVariableCollection.Remove(System.Convert.ToInt32(e.Item.Cells[1].Text));

            if (this.ProcessVariablesDataGrid.Items.Count == 1
            && ProcessVariablesDataGrid.CurrentPageIndex > 0)
                ProcessVariablesDataGrid.CurrentPageIndex--;

            UpdateProcessVariablesView();
        }
    }
}
