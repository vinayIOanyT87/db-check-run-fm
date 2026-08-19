/******************************************************************************

	FILE NAME:		ExternalComponentInputForm.aspx.cs


	PURPOSE:			Implementation of ExternalComponentInpuForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2008

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


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
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
    using FMCore;

	using Opc;

	/// <summary>
	/// Summary description for ExternalComponentInputForm.
	/// </summary>
	public partial class ExternalComponentInputForm : FMFormBase
	{
	
		private void UpdateInputView()
		{
			ProductMapClass ProductMap=this.Session["ProductMap"] as ProductMapClass;
			this.InputDataGrid.DataSource=this.InputView(ProductMap.ProcessVariableCollection);
			this.InputDataGrid.DataBind();
		}

		private ICollection InputView(ProcessVariableCollectionClass ProcessVariableCollection)
		{
			DataTable			PVDataTable=new DataTable();
			DataRow				PVDataRow;

			PVDataTable.Columns.Add("Index",typeof(Int32));
			PVDataTable.Columns.Add("TypeID",typeof(string));
			PVDataTable.Columns.Add("Host",typeof(string));
			PVDataTable.Columns.Add("OPCServerID",typeof(string));
			PVDataTable.Columns.Add("OPCItemID",typeof(string));

			int Item=0;
			foreach(ProcessVariableClass ProcessVariable in ProcessVariableCollection)
			{
				if (this.Session["ProcessVariable"] is ProcessVariableClass
				&& (this.Session["ProcessVariable"] as ProcessVariableClass).ProcessVariableType == ProcessVariable.ProcessVariableType
				&& (this.Session["ProcessVariable"] as ProcessVariableClass).InstanceNumber == ProcessVariable.InstanceNumber)
				{
					var editedProcessVariable = this.Session["ProcessVariable"] as ProcessVariableClass;
					ProcessVariable.Load(editedProcessVariable);
					this.Session.Remove("ProcessVariable");
				}

				PVDataRow=PVDataTable.NewRow();

				PVDataRow["Index"] = Item;
				PVDataRow["TypeID"] = this.GetTranslatedText(ProcessVariableClass.ProcessVariableTypeID(ProcessVariable.ProcessVariableType));
				URL	Url=new URL(ProcessVariable.URL);
				PVDataRow["Host"] = Url.HostName;
				PVDataRow["OPCServerID"] = ProcessVariable.ProgID;
				PVDataRow["OPCItemID"] = ProcessVariable.OPCItemID;
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
				this.GetSecurity();

				if (!this.Page.IsPostBack) 
				{
					string Index=this.Request.GetQueryOrFormValue("Index") as string;
					if(Index != null)
					{
						StationClass Station = this.Session["Station"] as StationClass;
						LoadArmClass LoadArm=Station.LoadArmCollection[(int) this.Session["LoadArmIndex"]];
						if(LoadArm == null)
							throw new Exception("No Load Arm in Session");
						
						ProductMapClass ProductMap=null;
						if (System.Convert.ToInt32(Index, CultureInfo.InvariantCulture) > LoadArm.ExternalComponentCollection.Count - 1)
							throw new Exception("Index out of range");

						ProductMap = LoadArm.ExternalComponentCollection[System.Convert.ToInt32(Index, CultureInfo.InvariantCulture)];

						this.ConfigurationLabel.Text+=" : "+ProductMap.AssignedID;

						this.Session["ProductMap"]=ProductMap;

						this.Session["ExternalComponentInputConfigurationLabel"]=this.ConfigurationLabel.Text;

					}
					else
					{
						if(this.Session["ProductMap"] == null
						|| this.Session["ExternalComponentInputConfigurationLabel"] == null)
							throw new Exception("No ProductMap in session");

						this.ConfigurationLabel.Text=this.Session["ExternalComponentInputConfigurationLabel"] as String;

					}

					this.UpdateInputView();
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
			this.InputDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.InputDataGrid_EditCommand);

		}
		#endregion

		private void InputDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Label			IndexLabel=(Label) e.Item.FindControl("InputIndexLabel");
			if(IndexLabel != null)
			{
				this.Session["UnitForm"]="ExternalComponentInputForm.aspx";
				ProductMapClass ProductMap=this.Session["ProductMap"] as ProductMapClass;
				this.Session["ProcessVariable"]=ProductMap.ProcessVariableCollection[System.Convert.ToInt32(IndexLabel.Text, CultureInfo.InvariantCulture)];
				this.Redirect("OPCConnectionForm.aspx");
			}
		}
	}
}
