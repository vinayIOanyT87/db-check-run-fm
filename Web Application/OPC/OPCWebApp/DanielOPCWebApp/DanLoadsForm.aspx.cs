/******************************************************************************

	FILE NAME:		DanLoadsForm.aspx.cs


	PURPOSE:			Implementation of DanLoadsForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


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

using DanielOPCObjectsLib;

using DanielOPCServerLib;

using FMBusinessObjects.DataObjects;

using FuelsManager.FMWebApp;

using OpcCom;

namespace OPCWebApp.DanielOPCWebApp
{
   /// <summary>
   /// Summary description for DanLoadsForm.
   /// </summary>
   public partial class DanLoadsForm :	DanielFormBase, IMenuDiscovery
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

		    //// Depends On DanLoad OPC Server
		    //if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x =>x.GetOPCAllowedFunctions()) & 0x10) == 0)
		    //{
		    //	return null;
		    //}

		    //var menuItems = new List<FMMenuItem>();

		    //var menuItem = new FMMenuItem
		    //{
		    //	MenuItemType = FMMenuItemType.CONFIG_DANIEL_OPC_PRESETS,
		    //	RootMenuName = "Configuration",
		    //	CategoryName = "Daniel OPC",
		    //	ItemName = "Presets",
		    //	NavigateUrl = "../DanielOPCWebApp/DanLoadsForm.aspx",
		    //	ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
		    //	DataDictGroupPrefix = "Daniel"
		    //};

		    //menuItems.Add(menuItem);

		    //return menuItems;
		}

		protected override void UpdateView()
		{
			ICollection danLoads = this.EnumerateDanLoads();

			this.DanLoadPresetsFormPageSizeDropDown.SetPageSize( this.DanLoadsDataGrid, danLoads.Count );
			
			this.DanLoadsDataGrid.DataSource = danLoads;
			this.DanLoadsDataGrid.DataBind();
		}

		private ICollection EnumerateDanLoads()
		{
			DanLoadCollectionClass	danLoadCollection;

			try
			{
				IDanLoads danLoads=(IDanLoads) OpcCom.Interop.CreateInstance(
					new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}"),
					this.Session["DanielSystem"] as string,
					new NetworkCredential());

				danLoadCollection=(DanLoadCollectionClass) danLoads.Enumerate();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				danLoadCollection=new DanLoadCollectionClass();
			}

			DataTable	danLoadDataTable=new DataTable();

		    danLoadDataTable.Columns.Add("Index",typeof(int));
			danLoadDataTable.Columns.Add("ID",typeof(string));
			danLoadDataTable.Columns.Add("Type",typeof(string));
			danLoadDataTable.Columns.Add("Port",typeof(string));
			danLoadDataTable.Columns.Add("Address",typeof(string));

			for(int iItem=0;iItem < danLoadCollection.Count;iItem++)
			{
				var		danLoadDataRow = danLoadDataTable.NewRow();

				var	danLoad = (DanLoadClass) danLoadCollection.Item(iItem);
				danLoadDataRow["Index"] = danLoad.Index;
				danLoadDataRow["ID"] = danLoad.ID;
				danLoadDataRow["Type"] = danLoad.TypeID(danLoad.Type);

				string htmlCompatibleText=danLoad.PortID.Replace("&","&amp");
				htmlCompatibleText=htmlCompatibleText.Replace(">","&gt");
				htmlCompatibleText=htmlCompatibleText.Replace("<","&lt");
				htmlCompatibleText=htmlCompatibleText.Replace("'","&apos");
				htmlCompatibleText=htmlCompatibleText.Replace("\"","&quot");


				danLoadDataRow["Port"] = htmlCompatibleText;
				danLoadDataRow["Address"] = danLoad.Address;

				danLoadDataTable.Rows.Add(danLoadDataRow);
			}
			DataView		danLoadDataView=new DataView(danLoadDataTable);
			return danLoadDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (! this.Page.IsPostBack)
				{
					this.SystemTextBox.Text=@"localhost";
					this.Session["DanielSystem"]=this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem=new ListItem("List","0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem=new ListItem("Text","1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex=1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(this.Session["DanLoadsPage"] != null)
					{
						this.DanLoadsDataGrid.CurrentPageIndex=(int) this.Session["DanLoadsPage"];
						this.Session.Remove("DanLoadsPage");
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
			this.DanLoadsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DanLoadsDataGrid_EditCommand);
			this.DanLoadsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.DanLoadsDataGrid_PageIndexChanged);
			this.DanLoadsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.DanLoadsDataGrid_DeleteCommand);
			this.DanLoadsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.DanLoadsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		private void DanLoadsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("DanLoad");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"]=indexCell.Text;
			this.Session["DanLoadsPage"]=this.DanLoadsDataGrid.CurrentPageIndex;
			this.Redirect("DanLoadForm.aspx");
		}

		private void DanLoadsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				
				IDanLoads danLoads=(IDanLoads) OpcCom.Interop.CreateInstance(
					new Guid("{54F57ECB-6111-4A9A-AFA6-ABC5B3C4FF59}"),
					this.Session["DanielSystem"] as string,
					new NetworkCredential());

				danLoads.Purge(Convert.ToInt32(indexCell.Text));

				this.DanLoadsDataGrid.SelectedIndex=-1;
				this.Session.Remove("Index");
				if(this.DanLoadsDataGrid.Items.Count == 1
				&& this.DanLoadsDataGrid.CurrentPageIndex > 0)
					this.DanLoadsDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("DanLoad");
			this.Session.Remove("Index");
			this.Session["DanLoadsPage"]=this.DanLoadsDataGrid.CurrentPageIndex;
			this.Redirect("DanLoadForm.aspx");
		}

		private void DanLoadsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.DanLoadsDataGrid.EditItemIndex > -1)
					return;
				this.DanLoadsDataGrid.CurrentPageIndex = e.NewPageIndex;
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
				this.Session["DanielSystem"]=this.SystemDropDownList.SelectedItem.Text;
				this.DanLoadsDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		private void DanLoadsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
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
					if(this.Session["DanielSystem"] != null
						&& (string) this.Session["DanielSystem"] == system)
						this.SystemDropDownList.SelectedIndex=this.SystemDropDownList.Items.Count-1;
					item++;
				}

				this.Session["DanielSystem"]=this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible=(this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible=!this.SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["DanielSystem"]=this.SystemTextBox.Text;
				this.DanLoadsDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}
