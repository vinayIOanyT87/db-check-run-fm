/******************************************************************************

	FILE NAME:		CardReadersForm.aspx.cs


	PURPOSE:			Implementation of CardReadersForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaCardReader.


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
using AcculoadOPCWebApp;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMControls;
using FuelsManager.FMWebApp;
using OpcCom;

namespace OPCWebApp.AcculoadOPCWebApp
{
    /// <summary>
	/// Summary description for CardReadersForm.
	/// </summary>
	public class CardReadersForm : 	AcculoadFormBase, IMenuDiscovery
	{
		protected FMDataGrid CardReadersDataGrid;
		protected FMButton AddButton;
		protected Image Image1;
		protected FMLabel Label3;
		protected DropDownList SystemDropDownList;
		protected FMButton AddButton2;
		protected FMPageSizeDropDown CardReadersFormPageSizeDropDown;
		protected FMDropDownList SelectSystemModeDropDownList;
		protected TextBox SystemTextBox;
		protected FMLabel Label2;


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

			// Depends On Accuload OPC Server
			if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x =>x.GetOPCAllowedFunctions() ) & 0x10) == 0)
			{
				return null;
			}

			var menuItems = new List<FMMenuItem>();

			var menuItem = new FMMenuItem
			{
				MenuItemType = FMMenuItemType.CONFIG_SMITH_METER_OPC_CARD_READERS,
				RootMenuName = "Configuration",
				CategoryName = "Smith Meter OPC",
				ItemName = "Card Readers",
				NavigateUrl = "../AcculoadOPCWebApp/CardReadersForm.aspx",
				ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
				DataDictGroupPrefix = "SmithMeter"
			};

			menuItems.Add(menuItem);

			return menuItems;
		}

		protected override void UpdateView()
		{
			ICollection cardReaders = this.EnumerateCardReaders();

			this.CardReadersFormPageSizeDropDown.SetPageSize( this.CardReadersDataGrid, cardReaders.Count );
			
			this.CardReadersDataGrid.DataSource = cardReaders;
			this.CardReadersDataGrid.DataBind();
		}

		private ICollection EnumerateCardReaders()
		{
            IAcculoads Devices = (IAcculoads)OpcCom.Interop.CreateInstance(
                new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
                Session["SmithMeterSystem"] as string,
                new NetworkCredential());

            AcculoadCollectionClass DeviceCollection = (AcculoadCollectionClass)Devices.Enumerate();
            DataTable CardReaderDataTable = new DataTable();
            DataRow CardReaderDataRow;
            AcculoadClass Device;

            CardReaderDataTable.Columns.Add("Index", typeof(Int32));
            CardReaderDataTable.Columns.Add("ID", typeof(string));
            CardReaderDataTable.Columns.Add("Type", typeof(string));
            CardReaderDataTable.Columns.Add("NetworkCommunications", typeof(bool));
            CardReaderDataTable.Columns.Add("Port", typeof(string));
            CardReaderDataTable.Columns.Add("IPAddress", typeof(string));

            for (int iItem = 0; iItem < DeviceCollection.Count; iItem++)
            {
                Device = (AcculoadClass)DeviceCollection.Item(iItem);
                if (Device.Type != ACCULOAD_TYPE.RCU_II_OPEN
                && Device.Type != ACCULOAD_TYPE.RCU_II_RCU
                && Device.Type != ACCULOAD_TYPE.SMITH_PROXIMITY)
                    continue;

                CardReaderDataRow = CardReaderDataTable.NewRow();

                CardReaderDataRow[0] = Device.Index;
                CardReaderDataRow[1] = Device.ID;
                CardReaderDataRow[2] = Device.TypeID(Device.Type);
                CardReaderDataRow[3] = Device.NetworkCommunications;

                string HTMLCompatibleText = Device.PortID.Replace("&", "&amp");
                HTMLCompatibleText = HTMLCompatibleText.Replace(">", "&gt");
                HTMLCompatibleText = HTMLCompatibleText.Replace("<", "&lt");
                HTMLCompatibleText = HTMLCompatibleText.Replace("'", "&apos");
                HTMLCompatibleText = HTMLCompatibleText.Replace("\"", "&quot");

                CardReaderDataRow[4] = HTMLCompatibleText;
                CardReaderDataRow[5] = Device.IPAddress;

                CardReaderDataTable.Rows.Add(CardReaderDataRow);
            }
            DataView CardReaderDataView = new DataView(CardReaderDataTable);
            return CardReaderDataView;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (! this.Page.IsPostBack) 
				{
					this.SystemTextBox.Text=@"localhost";
					this.Session["SmithMeterSystem"]=this.SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem newItem=new ListItem("List","0");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					newItem=new ListItem("Text","1");
					this.SelectSystemModeDropDownList.Items.Add(newItem);
					this.SelectSystemModeDropDownList.SelectedIndex=1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(this.Session["CardReadersPage"] != null)
					{
						this.CardReadersDataGrid.CurrentPageIndex=(int) this.Session["CardReadersPage"];
						this.Session.Remove("CardReadersPage");
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
			this.CardReadersFormPageSizeDropDown.SelectedIndexChanged += new System.EventHandler(this.PageSizeDropDown_SelectedIndexChanged);
			this.CardReadersDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CardReadersDataGrid_EditCommand);
			this.CardReadersDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.CardReadersDataGrid_PageIndexChanged);
			this.CardReadersDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CardReadersDataGrid_DeleteCommand);
			this.CardReadersDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.CardReadersDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.SystemDropDownList.SelectedIndexChanged += new System.EventHandler(this.SystemDropDownList_SelectedIndexChanged);
			this.SelectSystemModeDropDownList.SelectedIndexChanged += new System.EventHandler(this.SelectSystemModeDropDownList_SelectedIndexChanged);
			this.SystemTextBox.TextChanged += new System.EventHandler(this.SystemTextBox_TextChanged);
		}
		#endregion

		private void CardReadersDataGrid_EditCommand(object source, DataGridCommandEventArgs e)
		{
			this.Session.Remove("CardReader");
			TableCell indexCell = e.Item.Cells[1];//bds
			this.Session["Index"]=indexCell.Text;
			this.Session["CardReadersPage"]=this.CardReadersDataGrid.CurrentPageIndex;
			this.Redirect("CardReaderForm.aspx");
		}

		private void CardReadersDataGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				
                IAcculoads Devices = (IAcculoads)OpcCom.Interop.CreateInstance(
                    new Guid("{41D54854-8705-400A-9B22-F58B58088BE7}"),
                    (string)Session["SmithMeterSystem"],
                    new NetworkCredential());

                Devices.Purge(System.Convert.ToInt32(indexCell.Text));

				this.CardReadersDataGrid.SelectedIndex=-1;
				this.Session.Remove("Index");
				if(this.CardReadersDataGrid.Items.Count == 1
				&& this.CardReadersDataGrid.CurrentPageIndex > 0)
					this.CardReadersDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, CommandEventArgs e)
		{
			this.Session.Remove("CardReader");
			this.Session.Remove("Index");
			this.Session["CardReadersPage"]=this.CardReadersDataGrid.CurrentPageIndex;
			this.Redirect("CardReaderForm.aspx");
		}

		private void CardReadersDataGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (this.CardReadersDataGrid.EditItemIndex > -1)
					return;
				this.CardReadersDataGrid.CurrentPageIndex = e.NewPageIndex;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["SmithMeterSystem"]=this.SystemDropDownList.SelectedItem.Text;
				this.CardReadersDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void CardReadersDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.UpdateDeleteButton( (LinkButton) e.Item.FindControl("DeleteButton") );
		}

		private void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
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
                    if (this.Session["SmithMeterSystem"] != null
                        && (string)this.Session["SmithMeterSystem"] == system)
						this.SystemDropDownList.SelectedIndex=this.SystemDropDownList.Items.Count-1;
					item++;
				}

				this.Session["SmithMeterSystem"]=this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible=(this.SelectSystemModeDropDownList.SelectedIndex != 1);
			this.SystemTextBox.Visible=!this.SystemDropDownList.Visible;
		}

		private void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			try
			{
				this.Session["SmithMeterSystem"]=this.SystemTextBox.Text;
				this.CardReadersDataGrid.CurrentPageIndex=0;
				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	}

}
