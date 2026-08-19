/******************************************************************************

	FILE NAME:		OffLoadingExternalProductInputForm.aspx.cs


	PURPOSE:			Implementation of OffLoadingExternalProductInputForm


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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.DataObjects;

    using Opc;

    /// <summary>
	/// Support for specifying input for an external meter for offload products.
	/// </summary>
    public partial class OffLoadingExternalProductInputForm : FMFormBase
    {
         private void UpdateInputView()
        {
            var productMap = Session["ProductMap"] as ProductMapClass;
            if (productMap != null)
            {
                this.InputDataGrid.DataSource = this.InputView(productMap.ProcessVariableCollection);
            }

            this.InputDataGrid.DataBind();
		}

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private ICollection InputView(ProcessVariableCollectionClass processVariableCollection)
		{
		    var pvDataTable = new DataTable();

            pvDataTable.Columns.Add("Index", typeof(int));
			pvDataTable.Columns.Add("TypeID", typeof(string));
			pvDataTable.Columns.Add("Host", typeof(string));
			pvDataTable.Columns.Add("OPCServerID", typeof(string));
			pvDataTable.Columns.Add("OPCItemID", typeof(string));

            int item = 0;
            foreach (ProcessVariableClass processVariable in processVariableCollection)
            {
				DataRow pvDataRow = pvDataTable.NewRow();

                pvDataRow[0] = item;
                pvDataRow[1] =
                    this.GetTranslatedText(
                        ProcessVariableClass.ProcessVariableTypeID(processVariable.ProcessVariableType));
                var url = new URL(processVariable.URL);
                pvDataRow[2] = url.HostName;
                pvDataRow[3] = processVariable.ProgID;
                pvDataRow[4] = processVariable.OPCItemID;
                pvDataTable.Rows.Add(pvDataRow);
				item++;
			}

            var pvDataView = new DataView(pvDataTable);
            return pvDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
                this.GetSecurity();

			    if (!Page.IsPostBack)
			    {
// ReSharper disable RedundantCast
			        var index = Request.Params["Index"] as string;
// ReSharper restore RedundantCast
			        if (index != null)
			        {
                        StationClass station = Session["Station"] as StationClass;
                        LoadArmClass loadArm = station.LoadArmCollection[(int)Session["LoadArmIndex"]];
                        if (loadArm == null)
			            {
			                throw new Exception("No Load Arm in Session");
			            }

			            if (System.Convert.ToInt32(index, CultureInfo.InvariantCulture)
			                > loadArm.OffloadExternalProductCollection.Count - 1)
			            {
			                throw new Exception("Index out of range");
			            }

			            ProductMapClass productMap = loadArm.OffloadExternalProductCollection[System.Convert.ToInt32(index)];

			            this.ConfigurationLabel.Text += " : " + productMap.AssignedID;

			            this.Session["ProductMap"] = productMap;

			            this.Session["OffloadingExternalProductConfigurationLabel"] = this.ConfigurationLabel.Text;
			        }
			        else
			        {
			            if (this.Session["ProductMap"] == null || this.Session["OffLoadingExternalProductConfigurationLabel"] == null)
			            {
			                throw new Exception("No ProductMap in session");
			            }

			            this.ConfigurationLabel.Text = this.Session["OffLoadingExternalProductConfigurationLabel"] as string;
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
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.InputDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.InputDataGridEditCommand);
		}
		#endregion

		private void InputDataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
		    var indexLabel = (Label)e.Item.FindControl("InputIndexLabel");
		    if (indexLabel != null)
		    {
                this.Session["UnitForm"] = "OffLoadingExternalProductInputForm.aspx";
		        var productMap = Session["ProductMap"] as ProductMapClass;
		        if (productMap != null)
		        {
		            this.Session["ProcessVariable"] =
		                productMap.ProcessVariableCollection[System.Convert.ToInt32(indexLabel.Text, CultureInfo.InvariantCulture)];
		        }

		        Response.Redirect("OPCConnectionForm.aspx", false);
		        Context.ApplicationInstance.CompleteRequest();
		    }
		}
	}
}
