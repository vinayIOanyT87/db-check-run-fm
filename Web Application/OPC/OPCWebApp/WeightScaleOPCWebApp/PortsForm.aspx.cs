/******************************************************************************

	FILE NAME:		PortsForm.aspx.cs


	PURPOSE:			Implementation of PortsForm


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
 
*******************************************************************************/
using System;
using System.Collections;
using System.Data;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeightScaleOPCObjectsLib;
using WeightScaleOPCServerLib;
using OpcCom;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace WeightScaleOPCWebApp
{
   using FuelsManager.FMWebApp;

   using System.Collections.Generic;
   /// <summary>
   /// Summary description for PortsForm.
   /// </summary>
   public partial class PortsForm : WeightScaleFormBase,
                                        IMenuDiscovery
	{

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
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

			// Depends On Weight Scale OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOPCAllowedFunctions()) & 0x10) == 0)
            {
                return null;
            }

            var menuItems = new List<FMMenuItem>();

            var menuItem = new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_WEIGHT_SCALES_OPC_PORTS,
                RootMenuName = "Configuration",
                CategoryName = "Weight Scale OPC",
                ItemName = "Ports",
                NavigateUrl = "../WeightScaleOPCWebApp/PortsForm.aspx",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
                DataDictGroupPrefix = "Weight Scale"
            };

            menuItems.Add(menuItem);

            return menuItems;
        }

        protected override void UpdateView()
		{
			ICollection Ports = EnumeratePorts();

            this.WeightScalePortsFormPageSizeDropDown.SetPageSize(PortsDataGrid, Ports.Count);
			
			PortsDataGrid.DataSource = Ports;
			PortsDataGrid.DataBind();
		}

		private ICollection EnumeratePorts()
		{
			PortCollectionClass	PortCollection;

			try
			{
				IPorts Ports=(IPorts) OpcCom.Interop.CreateInstance(
					new Guid("{265331A0-40D0-4DEC-B614-2A21CDC5CC1F}"),
					Session["WeightScaleSystem"] as string,
					new NetworkCredential());

				PortCollection=(PortCollectionClass) Ports.Enumerate();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				PortCollection=new PortCollectionClass();
			}

			DataTable	PortDataTable=new DataTable();
			DataRow		PortDataRow;
			PortClass	Port;
	
			PortDataTable.Columns.Add("Index",typeof(Int32));
			PortDataTable.Columns.Add("ID",typeof(string));

			for(int iItem=0;iItem < PortCollection.Count;iItem++)
			{
				PortDataRow=PortDataTable.NewRow();

				Port=(PortClass) PortCollection.Item(iItem);
				PortDataRow[0]=Port.Index;
				PortDataRow[1]=Port.ID;

				PortDataTable.Rows.Add(PortDataRow);
			}
			DataView		PortDataView=new DataView(PortDataTable);
			return PortDataView;
		}
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				GetSecurity();

				if (! Page.IsPostBack)
				{
					SystemTextBox.Text="localhost";
					Session["WeightScaleSystem"] = SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem NewItem=new ListItem("List","0");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					NewItem=new ListItem("Text","1");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					SelectSystemModeDropDownList.SelectedIndex=1;
					SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(Session["PortsPage"] != null)
					{
						PortsDataGrid.CurrentPageIndex=(int) Session["PortsPage"];
						Session.Remove("PortsPage");
					}

					UpdateView();

					if(!Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						AddButton.Enabled=false;
						AddButton2.Enabled=false;
					}

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
			this.AddButton2.Command += new CommandEventHandler(this.AddButton_Command);
			this.PortsDataGrid.EditCommand += new DataGridCommandEventHandler(this.PortsDataGrid_EditCommand);
			this.PortsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.PortsDataGrid_PageIndexChanged);
			this.PortsDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.PortsDataGrid_DeleteCommand);
			this.PortsDataGrid.ItemDataBound += new DataGridItemEventHandler(this.PortsDataGrid_ItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButton_Command);

		}
		#endregion

        protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
        {
            UpdateView();
        }

		private void PortsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			Session.Remove("Port");
			TableCell indexCell = e.Item.Cells[2];
			Session["Index"]=indexCell.Text;
			Session["PortsPage"]=PortsDataGrid.CurrentPageIndex;
			Response.Redirect("PortForm.aspx");
		}

		private void PortsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[2];
				
				IPorts Ports=(IPorts) OpcCom.Interop.CreateInstance(
					new Guid("{265331A0-40D0-4DEC-B614-2A21CDC5CC1F}"),
					Session["WeightScaleSystem"] as string,
					new NetworkCredential());

				Ports.Purge(System.Convert.ToInt32(indexCell.Text));

				PortsDataGrid.SelectedIndex=-1;
				Session.Remove("Index");
				if(PortsDataGrid.Items.Count == 1
				&& PortsDataGrid.CurrentPageIndex > 0)
					PortsDataGrid.CurrentPageIndex--;
				UpdateView();

			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			Session.Remove("Port");
			Session.Remove("Index");
			Session["PortsPage"]=PortsDataGrid.CurrentPageIndex;
			Response.Redirect("PortForm.aspx");
		}

		private void PortsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (PortsDataGrid.EditItemIndex > -1)
					return;
				PortsDataGrid.CurrentPageIndex = e.NewPageIndex;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				Session["WeightScaleSystem"] = SystemDropDownList.SelectedItem.Text;
				PortsDataGrid.CurrentPageIndex=0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void PortsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			UpdateDeleteButton( (LinkButton) e.Item.FindControl("DeleteButton") );
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(SystemDropDownList.Visible
			&& SystemDropDownList.SelectedIndex != -1)
				SystemTextBox.Text=SystemDropDownList.SelectedItem.Text;

			// Populate SystemDropDownList
			if(SelectSystemModeDropDownList.SelectedIndex == 0)
			{
				SystemDropDownList.Items.Clear();
				ListItem NewItem=new ListItem("localhost","0");
				SystemDropDownList.Items.Add(NewItem);
				ServerEnumerator Enumerator=new ServerEnumerator();
				string [] Systems=Enumerator.EnumerateHosts();
				int Item=1;

				foreach(string System in Systems)
				{
					NewItem=new ListItem(System,Item.ToString());
					SystemDropDownList.Items.Add(NewItem);
					if (Session["WeightScaleSystem"] != null
					&& (string)Session["WeightScaleSystem"] == System)
						SystemDropDownList.SelectedIndex=SystemDropDownList.Items.Count-1;
					Item++;
				}

				Session["WeightScaleSystem"] = SystemDropDownList.SelectedItem.Text;
			}

			SystemDropDownList.Visible=SelectSystemModeDropDownList.SelectedIndex != 1;
			SystemTextBox.Visible=!SystemDropDownList.Visible;
		
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				Session["WeightScaleSystem"] = SystemTextBox.Text;
				PortsDataGrid.CurrentPageIndex=0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}
	}
}
