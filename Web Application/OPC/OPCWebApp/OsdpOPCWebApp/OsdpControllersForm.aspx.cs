/******************************************************************************

	FILE NAME:		OptomuxControllersForm.aspx.cs


	PURPOSE:			Implementation of OptomuxControllersForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaOptomuxController.


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

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;

using FuelsManager.FMWebApp;

using OpcCom;

using OsdpOPCObjectsLib;

using OsdpOPCServerLib;

namespace OPCWebApp.OsdpOPCWebApp
{
	/// <summary>
	/// Summary description for OsdpControllersForm.
	/// </summary>
	public partial class OsdpControllersForm : OsdpFormBase, IMenuDiscovery
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

			// Depends On Optomux OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOPCAllowedFunctions()) & 0x200) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_OSDP_OPC_CONTROLLERS,
				RootMenuName = "Configuration",
				CategoryName = "OSDP OPC",
				ItemName = "Controllers",
				NavigateUrl = "../OsdpOPCWebApp/OsdpControllersForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
				DataDictGroupPrefix = "OSDP"
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		private void UpdateView()
		{
			ICollection controllers = this.EnumerateOsdpControllers();

			this.OsdpControllersFormPageSizeDropDown.SetPageSize(this.OsdpControllersDataGrid, controllers.Count);

			this.OsdpControllersDataGrid.DataSource = controllers;
			this.OsdpControllersDataGrid.DataBind();
		}

		private ICollection EnumerateOsdpControllers()
		{
			IOsdpControllers osdpControllers = (IOsdpControllers)OpcCom.Interop.CreateInstance(
				new Guid("{f5e1937d-316f-4a07-a31e-77f2246a1b71}"),
				this.Session["OsdpControllersSystem"] as string,
				new NetworkCredential());

			OsdpControllerCollectionClass osdpControllerCollection = (OsdpControllerCollectionClass)osdpControllers.Enumerate();
			DataTable osdpControllerDataTable = new DataTable();

			osdpControllerDataTable.Columns.Add("Index", typeof(int));
			osdpControllerDataTable.Columns.Add("ID", typeof(string));
			osdpControllerDataTable.Columns.Add("Port", typeof(string));

			for (int iItem = 0; iItem < osdpControllerCollection.Count; iItem++)
			{
				var osdpControllerDataRow = osdpControllerDataTable.NewRow();

				var osdpController = (OsdpControllerClass)osdpControllerCollection.Item(iItem);
				osdpControllerDataRow["Index"] = osdpController.Index;
				osdpControllerDataRow["ID"] = osdpController.ID;

				string htmlCompatibleText = this.GetDictionaryText(osdpController.PortID);
				htmlCompatibleText = htmlCompatibleText.Replace("&", "&amp");
				htmlCompatibleText = htmlCompatibleText.Replace(">", "&gt");
				htmlCompatibleText = htmlCompatibleText.Replace("<", "&lt");
				htmlCompatibleText = htmlCompatibleText.Replace("'", "&apos");
				htmlCompatibleText = htmlCompatibleText.Replace("\"", "&quot");

				osdpControllerDataRow["Port"] = htmlCompatibleText;

				osdpControllerDataTable.Rows.Add(osdpControllerDataRow);
			}
			DataView osdpControllerDataView = new DataView(osdpControllerDataTable);
			return osdpControllerDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.SystemTextBox.Text = @"localhost";
					this.Session["OsdpControllersSystem"] = this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem = new ListItem("List", "0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem = new ListItem("Text", "1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex = 1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);
					if (this.Session["OsdpControllersPage"] != null)
					{
						this.OsdpControllersDataGrid.CurrentPageIndex = (int)this.Session["OsdpControllersPage"];
						this.Session.Remove("OsdpControllersPage");
					}

					this.UpdateView();

					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
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
			this.AddButton2.Command += new CommandEventHandler(this.AddButton_Command);
			this.OsdpControllersDataGrid.EditCommand += new DataGridCommandEventHandler(this.OsdpControllersDataGrid_EditCommand);
			this.OsdpControllersDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.OsdpControllersDataGrid_PageIndexChanged);
			this.OsdpControllersDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.OsdpControllersDataGrid_DeleteCommand);
			this.OsdpControllersDataGrid.ItemDataBound += new DataGridItemEventHandler(this.OsdpControllersDataGrid_ItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		private void OsdpControllersDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("OsdpController");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"] = indexCell.Text;
			this.Session["OsdpControllersPage"] = this.OsdpControllersDataGrid.CurrentPageIndex;
			this.Redirect("OsdpControllerForm.aspx");
		}

		private void OsdpControllersDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds

				// ReSharper disable once RedundantNameQualifier
				IOsdpControllers osdpControllers = (IOsdpControllers)OpcCom.Interop.CreateInstance(
					new Guid("{f5e1937d-316f-4a07-a31e-77f2246a1b71}"),
					this.Session["OsdpControllersSystem"] as string,
					new NetworkCredential());

				osdpControllers.Purge(Convert.ToInt32(indexCell.Text));

				this.OsdpControllersDataGrid.SelectedIndex = -1;
				this.Session.Remove("Index");
				if (this.OsdpControllersDataGrid.Items.Count == 1
				&& this.OsdpControllersDataGrid.CurrentPageIndex > 0)
					this.OsdpControllersDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("OsdpController");
			this.Session.Remove("Index");
			this.Session["OsdpControllersPage"] = this.OsdpControllersDataGrid.CurrentPageIndex;
			this.Redirect("OsdpControllerForm.aspx");
		}

		private void OsdpControllersDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.OsdpControllersDataGrid.EditItemIndex > -1)
					return;
				this.OsdpControllersDataGrid.CurrentPageIndex = e.NewPageIndex;
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
				this.Session["OsdpControllersSystem"] = this.SystemDropDownList.SelectedItem.Text;
				this.OsdpControllersDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void OsdpControllersDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.UpdateDeleteButton((LinkButton)e.Item.FindControl("DeleteButton"));
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SystemDropDownList.Visible
				&& this.SystemDropDownList.SelectedIndex != -1)
				this.SystemTextBox.Text = this.SystemDropDownList.SelectedItem.Text;

			// Populate SystemDropDownList
			if (this.SelectSystemModeDropDownList.SelectedIndex == 0)
			{
				this.SystemDropDownList.Items.Clear();
				ListItem newItem = new ListItem("localhost", "0");
				this.SystemDropDownList.Items.Add(newItem);
				ServerEnumerator enumerator = new ServerEnumerator();
				string[] systems = enumerator.EnumerateHosts();
				int item = 1;

				foreach (string system in systems)
				{
					newItem = new ListItem(system, item.ToString());
					this.SystemDropDownList.Items.Add(newItem);
					if (this.Session["OsdpControllersSystem"] != null
						 && (string)this.Session["OsdpControllersSystem"] == system)
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					item++;
				}

				this.Session["OsdpControllersSystem"] = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["OsdpControllersSystem"] = this.SystemTextBox.Text;
				this.OsdpControllersDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}
