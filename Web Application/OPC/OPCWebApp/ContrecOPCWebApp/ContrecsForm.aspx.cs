/******************************************************************************

	FILE NAME:		ContrecsForm.aspx.cs


	PURPOSE:			Implementation of ContrecsForm


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
   /// Summary description for ContrecsForm.
   /// </summary>
   public partial class ContrecsForm : ContrecFormBase, IMenuDiscovery
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
		    /*
			// Depends upon Load Rack Service
			if ((options & 0x8000) == 0)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_SITES_AND_SITE_GROUPS) && !security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			// Depends On Contrec OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOPCAllowedFunctions()) & 0x400) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_CONTREC_OPC_PRESETS,
				RootMenuName = "Configuration",
				CategoryName = "Contrec OPC",
				ItemName = "Presets",
				NavigateUrl = "../ContrecOPCWebApp/ContrecsForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
				DataDictGroupPrefix = "Contrec"
			};

			menuItems.Add(menuItem);

			return menuItems;
            */
		}

		protected override void UpdateView()
		{
			ICollection conTrecs = this.EnumerateConTrecs();

			this.ConTrecPresetsFormPageSizeDropDown.SetPageSize(this.ConTrecsDataGrid, conTrecs.Count);

			this.ConTrecsDataGrid.DataSource = conTrecs;
			this.ConTrecsDataGrid.DataBind();
		}

		private ICollection EnumerateConTrecs()
		{
			ContrecCollectionClass conTrecCollection;

			try
			{
				IContrecs conTrecs = (IContrecs)OpcCom.Interop.CreateInstance(
					new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}"),
					this.Session["ContrecSystem"] as string,
					new NetworkCredential());

				conTrecCollection = (ContrecCollectionClass)conTrecs.Enumerate();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				conTrecCollection = new ContrecCollectionClass();
			}

			DataTable conTrecDataTable = new DataTable();

		    conTrecDataTable.Columns.Add("Index", typeof(Int32));
			conTrecDataTable.Columns.Add("ID", typeof(string));
			conTrecDataTable.Columns.Add("Type", typeof(string));
			conTrecDataTable.Columns.Add("Port", typeof(string));
			conTrecDataTable.Columns.Add("Address", typeof(string));

			for (int iItem = 0; iItem < conTrecCollection.Count; iItem++)
			{
				var conTrecDataRow = conTrecDataTable.NewRow();

				var conTrec = (ContrecClass)conTrecCollection.Item(iItem);
				conTrecDataRow["Index"] = conTrec.Index;
				conTrecDataRow["ID"] = conTrec.ID;
				conTrecDataRow["Type"] = conTrec.TypeID(conTrec.Type);

				string htmlCompatibleText = conTrec.PortID.Replace("&", "&amp");
				htmlCompatibleText = htmlCompatibleText.Replace(">", "&gt");
				htmlCompatibleText = htmlCompatibleText.Replace("<", "&lt");
				htmlCompatibleText = htmlCompatibleText.Replace("'", "&apos");
				htmlCompatibleText = htmlCompatibleText.Replace("\"", "&quot");


				conTrecDataRow["Port"] = htmlCompatibleText;
				conTrecDataRow["Address"] = conTrec.Address;

				conTrecDataTable.Rows.Add(conTrecDataRow);
			}
			DataView conTrecDataView = new DataView(conTrecDataTable);
			return conTrecDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.SystemTextBox.Text = @"localhost";
					this.Session["ContrecSystem"] = this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem = new ListItem("List", "0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem = new ListItem("Text", "1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex = 1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);

					if (this.Session["ContrecsPage"] != null)
					{
						this.ConTrecsDataGrid.CurrentPageIndex = (int)this.Session["ContrecsPage"];
						this.Session.Remove("ContrecsPage");
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
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.ConTrecsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ConTrecsDataGrid_EditCommand);
			this.ConTrecsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ConTrecsDataGrid_PageIndexChanged);
			this.ConTrecsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ConTrecsDataGrid_DeleteCommand);
			this.ConTrecsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.ConTrecsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
		}
		#endregion

		private void ConTrecsDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("Contrec");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"] = indexCell.Text;
			this.Session["ContrecsPage"] = this.ConTrecsDataGrid.CurrentPageIndex;
			this.Redirect("ContrecForm.aspx");
		}

		private void ConTrecsDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds

				IContrecs conTrecs = (IContrecs)OpcCom.Interop.CreateInstance(
					new Guid("{59DB8E98-D175-49A8-997B-8D342154B9D7}"),
					this.Session["ContrecSystem"] as string,
					new NetworkCredential());

				conTrecs.Purge(Convert.ToInt32(indexCell.Text));

				this.ConTrecsDataGrid.SelectedIndex = -1;
				this.Session.Remove("Index");
				if (this.ConTrecsDataGrid.Items.Count == 1
				&& this.ConTrecsDataGrid.CurrentPageIndex > 0)
					this.ConTrecsDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("Contrec");
			this.Session.Remove("Index");
			this.Session["ContrecsPage"] = this.ConTrecsDataGrid.CurrentPageIndex;
			this.Redirect("ContrecForm.aspx");
		}

		private void ConTrecsDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.ConTrecsDataGrid.EditItemIndex > -1)
					return;
				this.ConTrecsDataGrid.CurrentPageIndex = e.NewPageIndex;
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
				this.Session["ContrecSystem"] = this.SystemDropDownList.SelectedItem.Text;
				this.ConTrecsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ConTrecsDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
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
					if (this.Session["ContrecSystem"] != null
						&& (string)this.Session["ContrecSystem"] == system)
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					item++;
				}

				this.Session["ContrecSystem"] = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["ContrecSystem"] = this.SystemTextBox.Text;
				this.ConTrecsDataGrid.CurrentPageIndex = 0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
	}
}
