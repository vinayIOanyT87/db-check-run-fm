/******************************************************************************

	FILE NAME:		PortsForm.aspx.cs


	PURPOSE:			Implementation of PortsForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	B. Schaal


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

using ContrecOPCObjectsLib;

using ContrecOPCServerLib;

using FMBusinessObjects.DataObjects;

using FuelsManager.FMWebApp;

using OpcCom;

namespace OPCWebApp.ContrecOPCWebApp
{
   /// <summary>
   /// Summary description for PortsForm.
   /// </summary>
   public partial class PortsForm :	ContrecFormBase, IMenuDiscovery
	{
        /// <summary>
        /// Gets a list of menu items that should be displayed for the current user.
        /// </summary>
        /// <param name="security">The security object of the current session</param>
        /// <param name="siteGroup">Whether the current logged-in site is a site group</param>
        /// <param name="word1">First word of license key options</param>
        /// <param name="word2">Second word of license key options</param>
        /// <returns>
        /// List of menu items to be displayed
        /// </returns>
        public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1, ushort word2, ushort useNewLicenseKey, uint options)
        {
		    return null;

		    //// Depends upon Load Rack Service
		    //if ((options & 0x8000) == 0)
		    //{
		    //	return null;
		    //}

		    //if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
		    //{
		    //	return null;
		    //}

		    //// Depends On Contrec OPC Server
		    //if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x =>x.GetOPCAllowedFunctions()) & 0x10) == 0)
		    //{
		    //	return null;
		    //}

		    //var menuItems = new List<FMMenuItem>();

		    //var menuItem = new FMMenuItem
		    //{
		    //	MenuItemType = FMMenuItemType.CONFIG_CONTREC_OPC_PORTS,
		    //	RootMenuName = "Configuration",
		    //	CategoryName = "Contrec OPC",
		    //	ItemName = "Ports",
		    //	NavigateUrl = "../ContrecOPCWebApp/PortsForm.aspx",
		    //	ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
		    //	DataDictGroupPrefix = "Contrec"
		    //};

		    //menuItems.Add(menuItem);

		    //return menuItems;
		}

		protected override void UpdateView()
		{
			ICollection ports = this.EnumeratePorts();

			this.ContrecPortsFormPageSizeDropDown.SetPageSize( this.PortsDataGrid, ports.Count );
			
			this.PortsDataGrid.DataSource = ports;
			this.PortsDataGrid.DataBind();
		}

		private ICollection EnumeratePorts()
		{
			PortCollectionClass	portCollection;

			try
			{
				IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(
					new Guid("{2B2CCFD9-9EF7-48BB-BEF4-C58C0C43409D}"),
					this.Session["ContrecSystem"] as string,
					new NetworkCredential());

				portCollection=(PortCollectionClass) ports.Enumerate();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				portCollection=new PortCollectionClass();
			}

			DataTable	portDataTable=new DataTable();

		    portDataTable.Columns.Add("Index",typeof(Int32));
			portDataTable.Columns.Add("ID",typeof(string));

			for(int iItem=0;iItem < portCollection.Count;iItem++)
			{
				var		portDataRow = portDataTable.NewRow();

				var	port = (PortClass) portCollection.Item(iItem);
				portDataRow["Index"] = port.Index;
				portDataRow["ID"] = port.ID;

				portDataTable.Rows.Add(portDataRow);
			}
			DataView		portDataView=new DataView(portDataTable);
			return portDataView;
		}
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (! this.Page.IsPostBack)
				{
					this.SystemTextBox.Text=@"localhost";
					this.Session["ContrecSystem"]=this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem=new ListItem("List","0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem=new ListItem("Text","1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex=1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(this.Session["PortsPage"] != null)
					{
						this.PortsDataGrid.CurrentPageIndex=(int) this.Session["PortsPage"];
						this.Session.Remove("PortsPage");
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
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.PortsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PortsDataGrid_EditCommand);
			this.PortsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.PortsDataGrid_PageIndexChanged);
			this.PortsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.PortsDataGrid_DeleteCommand);
			this.PortsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.PortsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}
		#endregion

		private void PortsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Port");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"]=indexCell.Text;
			this.Session["PortsPage"]=this.PortsDataGrid.CurrentPageIndex;
			this.Redirect("PortForm.aspx");
		}

		private void PortsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				
				IPorts ports=(IPorts) OpcCom.Interop.CreateInstance(
					new Guid("{2B2CCFD9-9EF7-48BB-BEF4-C58C0C43409D}"),
					this.Session["ContrecSystem"] as string,
					new NetworkCredential());

				ports.Purge(Convert.ToInt32(indexCell.Text));

				this.PortsDataGrid.SelectedIndex=-1;
				this.Session.Remove("Index");
				if(this.PortsDataGrid.Items.Count == 1
					&& this.PortsDataGrid.CurrentPageIndex > 0)
					this.PortsDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Port");
			this.Session.Remove("Index");
			this.Session["PortsPage"]=this.PortsDataGrid.CurrentPageIndex;
			this.Redirect("PortForm.aspx");
		}

		private void PortsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.PortsDataGrid.EditItemIndex > -1)
					return;
				this.PortsDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["ContrecSystem"]=this.SystemDropDownList.SelectedItem.Text;
				this.PortsDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PortsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.UpdateDeleteButton( (LinkButton) e.Item.FindControl("DeleteButton") );
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(this.SystemDropDownList.Visible
				&& this.SystemDropDownList.SelectedIndex != -1)
				this.SystemTextBox.Text=this.SystemDropDownList.SelectedItem.Text;

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
					if(this.Session["ContrecSystem"] != null
						&& (string) this.Session["ContrecSystem"] == system)
						this.SystemDropDownList.SelectedIndex=this.SystemDropDownList.Items.Count-1;
					item++;
				}

				this.Session["ContrecSystem"]=this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible=(this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible=!this.SystemDropDownList.Visible;
		
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["ContrecSystem"]=this.SystemTextBox.Text;
				this.PortsDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}
