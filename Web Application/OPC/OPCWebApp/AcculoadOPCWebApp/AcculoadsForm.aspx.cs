/******************************************************************************

	FILE NAME:		AcculoadsForm.aspx.cs


	PURPOSE:			Implementation of AcculoadsForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaAccuload.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		2009-03-17  G.Kendall   WI# 1416 - Get OPC allowable from SecurityClass
 
*******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Web.UI.WebControls;
using AcculoadOPCObjectsLib;
using AcculoadOPCServerLib;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMControls;
using FuelsManager.FMWebApp;
using OpcCom;

// ReSharper disable once CheckNamespace
namespace AcculoadOPCWebApp
{
    /// <summary>
	/// Summary description for AcculoadsForm.
	/// </summary>
	public class AcculoadsForm :	AcculoadFormBase, IMenuDiscovery
	{
		protected FMDataGrid AcculoadsDataGrid;
		protected FMButton AddButton;
		protected Image Image1;
		protected FMLabel Label3;
		protected DropDownList SystemDropDownList;
		protected FMButton AddButton2;
		protected FMPageSizeDropDown AcculoadPresetsFormPageSizeDropDown;
		protected FMDropDownList SelectSystemModeDropDownList;
		protected TextBox SystemTextBox;
		protected FMLabel Label2;


        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="word1"></param>
        /// <param name="word2"></param>
        /// <returns>
        /// List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
            // Depends upon Load Rack Service
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			// Depends On Accuload OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x =>x.GetOPCAllowedFunctions() ) & 0x10) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
				{
					MenuItemType = FMMenuItemType.CONFIG_SMITH_METER_OPC_PRESETS,
					RootMenuName = "Configuration",
					CategoryName = "Smith Meter OPC",
					ItemName = "Presets",
					NavigateUrl = "../AcculoadOPCWebApp/AcculoadsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
					DataDictGroupPrefix = "SmithMeter"
				};

			menuItems.Add(menuItem);

			return menuItems;
		}

		protected override void UpdateView()
		{
			ICollection acculoads = this.EnumerateAcculoads();

			this.AcculoadPresetsFormPageSizeDropDown.SetPageSize(this.AcculoadsDataGrid, acculoads.Count );

		    this.AcculoadsDataGrid.DataSource = acculoads;
		    this.AcculoadsDataGrid.DataBind();
		}

		private ICollection EnumerateAcculoads()
		{
			AcculoadCollectionClass	acculoadCollection;

			try
			{
				IAcculoads acculoads=(IAcculoads) OpcCom.Interop.CreateInstance(
					new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"), this.Session["SmithMeterSystem"] as string,
					new NetworkCredential());

				acculoadCollection=(AcculoadCollectionClass) acculoads.Enumerate();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
				acculoadCollection=new AcculoadCollectionClass();
			}

			DataTable	acculoadDataTable=new DataTable();

		    acculoadDataTable.Columns.Add("Index",typeof(Int32));
			acculoadDataTable.Columns.Add("ID",typeof(string));
			acculoadDataTable.Columns.Add("Type",typeof(string));
			acculoadDataTable.Columns.Add("NetworkCommunications",typeof(bool));
			acculoadDataTable.Columns.Add("Port",typeof(string));
			acculoadDataTable.Columns.Add("IPAddress",typeof(string));

			for(int iItem=0;iItem < acculoadCollection.Count;iItem++)
			{
				var accuload = (AcculoadClass)acculoadCollection.Item(iItem);
				if (accuload.Type == ACCULOAD_TYPE.RCU_II_OPEN
				|| accuload.Type == ACCULOAD_TYPE.RCU_II_RCU
				|| accuload.Type == ACCULOAD_TYPE.SMITH_PROXIMITY)
					continue;

				var acculoadDataRow = acculoadDataTable.NewRow();

				acculoadDataRow["Index"] = accuload.Index;
				acculoadDataRow["ID"] = accuload.ID;
				acculoadDataRow["Type"] = accuload.TypeID(accuload.Type);
				acculoadDataRow["NetworkCommunications"] = accuload.NetworkCommunications;	

				string htmlCompatibleText=accuload.PortID.Replace("&","&amp");
				htmlCompatibleText=htmlCompatibleText.Replace(">","&gt");
				htmlCompatibleText=htmlCompatibleText.Replace("<","&lt");
				htmlCompatibleText=htmlCompatibleText.Replace("'","&apos");
				htmlCompatibleText=htmlCompatibleText.Replace("\"","&quot");


				acculoadDataRow["Port"] = htmlCompatibleText;
				acculoadDataRow["IPAddress"] = accuload.IPAddress;

				acculoadDataTable.Rows.Add(acculoadDataRow);
			}
			DataView		acculoadDataView=new DataView(acculoadDataTable);
			return acculoadDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
			    this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
				    this.SystemTextBox.Text=@"localhost";
				    this.Session["SmithMeterSystem"]= this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem=new ListItem("List","0");
				    this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem=new ListItem("Text","1");
				    this.SelectSystemModeDropDownList.Items.Add(newItem);
				    this.SelectSystemModeDropDownList.SelectedIndex=1;
				    this.SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(this.Session["AcculoadsPage"] != null)
					{
					    this.AcculoadsDataGrid.CurrentPageIndex=(int)this.Session["AcculoadsPage"];
					    this.Session.Remove("AcculoadsPage");
					}

				    this.UpdateView();

					if(!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
					    this.AddButton.Enabled=false;
					    this.AddButton2.Enabled=false;
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

			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.AcculoadPresetsFormPageSizeDropDown.SelectedIndexChanged += new System.EventHandler( this.PageSizeDropDown_SelectedIndexChanged );
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.AcculoadsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AcculoadsDataGrid_EditCommand);
			this.AcculoadsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.AcculoadsDataGrid_PageIndexChanged);
			this.AcculoadsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.AcculoadsDataGrid_DeleteCommand);
			this.AcculoadsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.AcculoadsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.SystemTextBox.TextChanged += new System.EventHandler(this.SystemTextBox_TextChanged);
			this.SelectSystemModeDropDownList.SelectedIndexChanged += new System.EventHandler(this.SelectSystemModeDropDownList_SelectedIndexChanged);
			this.SystemDropDownList.SelectedIndexChanged += new System.EventHandler(this.SystemDropDownList_SelectedIndexChanged);
		}
		#endregion

	    // ReSharper disable once InconsistentNaming
		private void AcculoadsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
		    this.Session.Remove("Accuload");
			TableCell indexCell = e.Item.Cells[1];//bds
		    this.Session["Index"]=indexCell.Text;
		    this.Session["AcculoadsPage"]= this.AcculoadsDataGrid.CurrentPageIndex;
			this.Redirect("AcculoadForm.aspx");
		}

	    // ReSharper disable once InconsistentNaming
		private void AcculoadsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				
				IAcculoads acculoads=(IAcculoads) OpcCom.Interop.CreateInstance(
					new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"), this.Session["SmithMeterSystem"] as string,
					new NetworkCredential());

				acculoads.Purge(Convert.ToInt32(indexCell.Text));

			    this.AcculoadsDataGrid.SelectedIndex=-1;
			    this.Session.Remove("Index");
				if(this.AcculoadsDataGrid.Items.Count == 1
				&& this.AcculoadsDataGrid.CurrentPageIndex > 0) this.AcculoadsDataGrid.CurrentPageIndex--;
			    this.UpdateView();

			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void AddButton_Command(object sender, CommandEventArgs e)
		{
		    this.Session.Remove("Accuload");
		    this.Session.Remove("Index");
		    this.Session["AcculoadsPage"]= this.AcculoadsDataGrid.CurrentPageIndex;
			this.Redirect("AcculoadForm.aspx");
		}

	    // ReSharper disable once InconsistentNaming
		private void AcculoadsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.AcculoadsDataGrid.EditItemIndex > -1)
					return;
			    this.AcculoadsDataGrid.CurrentPageIndex = e.NewPageIndex;
			    this.UpdateView();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
			    this.Session["SmithMeterSystem"]= this.SystemDropDownList.SelectedItem.Text;
			    this.AcculoadsDataGrid.CurrentPageIndex=0;
			    this.UpdateView();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}


	    // ReSharper disable once InconsistentNaming
		private void AcculoadsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
		    this.UpdateDeleteButton( (LinkButton) e.Item.FindControl("DeleteButton") );
		}

	    // ReSharper disable once InconsistentNaming
		private void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(this.SystemDropDownList.Visible
				&& this.SystemDropDownList.SelectedIndex != -1) this.SystemTextBox.Text= this.SystemDropDownList.SelectedItem.Text;

			// Populate SystemDropDownList
			if(this.SelectSystemModeDropDownList.SelectedIndex == 0)
			{
			    this.SystemDropDownList.Items.Clear();
				ListItem newItem=new ListItem("localhost","0");
			    this.SystemDropDownList.Items.Add(newItem);
				ServerEnumerator enumerator=new ServerEnumerator();
				string [] systems=enumerator.EnumerateHosts();
				int item=1;

				foreach(string system in systems)
				{
					newItem=new ListItem(system,item.ToString());
				    this.SystemDropDownList.Items.Add(newItem);
                    if (this.Session["SmithMeterSystem"] != null
                        && (string)this.Session["SmithMeterSystem"] == system) this.SystemDropDownList.SelectedIndex= this.SystemDropDownList.Items.Count-1;
					item++;
				}

			    this.Session["SmithMeterSystem"]= this.SystemDropDownList.SelectedItem.Text;
			}

		    this.SystemDropDownList.Visible=(this.SelectSystemModeDropDownList.SelectedIndex != 1);
		    this.SystemTextBox.Visible=!this.SystemDropDownList.Visible;
		}

	    // ReSharper disable once InconsistentNaming
		private void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
			    this.Session["SmithMeterSystem"]= this.SystemTextBox.Text;
			    this.AcculoadsDataGrid.CurrentPageIndex=0;
			    this.UpdateView();
			}
			catch (Exception except)
			{
			    this.ErrorHandler(except);
			}
		}

	}

}
