/******************************************************************************

	FILE NAME:		ScullysForm.aspx.cs


	PURPOSE:			Implementation of ScullysForm


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+HaScully.


	AUTHOR(S):	S. Jiang


	VERSION:		1.0.0  Current version
    
 
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
using ScullyOPCObjectsLib;
using ScullyOPCServerLib;
using System.Globalization;

namespace OPCWebApp.ScullyOPCWebApp
{
	/// <summary>
	/// Summary description for ScullysForm
	/// </summary>
	public partial class ScullysForm  :	ScullyFormBase, IMenuDiscovery
    {
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

            // Depends On Scully OPC Server
            if ((FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOPCAllowedFunctions()) & 0x200) == 0)
            {
                return null;
            }

            var menuItems = new List<FMMenuItem>();

            var menuItem = new FMMenuItem
            {
                MenuItemType = FMMenuItemType.CONFIG_SCULLY_OPC_DEVICES,
                RootMenuName = "Configuration",
                CategoryName = "Scully OPC",
                ItemName = "Scullys",
                NavigateUrl = "../ScullyOPCWebApp/ScullysForm.aspx",
                ApplyDataDictionary = ApplyDataDictionary.DoNotApply,
                DataDictGroupPrefix = "Scully"
            };

            menuItems.Add(menuItem);

            return menuItems;
        }
        protected override void UpdateView()
		{
			ICollection Scales = EnumerateScullys();

			ScullysFormPageSizeDropDown.SetPageSize( ScullysDataGrid, Scales.Count );
			
			ScullysDataGrid.DataSource = Scales;
			ScullysDataGrid.DataBind();
		}

		private ICollection EnumerateScullys()
		{
			ScullyCollectionClass	ScullyCollection;

			try
			{
				IScullys Scullys=(IScullys) OpcCom.Interop.CreateInstance(
                    new Guid("{948DA86B-A687-494c-9B93-569B65499B36}"),
					Session["ScullySystem"] as string,
					new NetworkCredential());

				ScullyCollection=(ScullyCollectionClass) Scullys.Enumerate();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				ScullyCollection=new ScullyCollectionClass();
			}

			PortCollectionClass PortCollection;
			try
			{
				IPorts Ports = (IPorts)OpcCom.Interop.CreateInstance(
                    new Guid("{BF99140E-F916-49c2-9541-61BDD75E4531}"),
					Session["ScullySystem"] as string,
					new NetworkCredential());

				PortCollection = (PortCollectionClass)Ports.Enumerate();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				PortCollection = new PortCollectionClass();
			}

			DataTable ScullyDataTable = new DataTable();
			DataRow		ScullyDataRow;
			ScullyClass	Scully;
	
			ScullyDataTable.Columns.Add("Index",typeof(Int32));
			ScullyDataTable.Columns.Add("ID",typeof(string));
			ScullyDataTable.Columns.Add("Port",typeof(string));

			for(int iItem=0;iItem < ScullyCollection.Count;iItem++)
			{
				ScullyDataRow=ScullyDataTable.NewRow();

				Scully=(ScullyClass) ScullyCollection.Item(iItem);
				ScullyDataRow[0]=Scully.Index;
				ScullyDataRow[1]=Scully.ID;


				// Get Index
				bool PortFound = false;
				if (Scully.PortIndex > 0)
				{
					foreach (PortClass Port in PortCollection)
					{
						if (Port.Index == Scully.PortIndex &&
							Port.ID.Length > 0)
						{
							ScullyDataRow[2] = Port.ID;
							PortFound = true;
							break;
						}
					}
					if(PortFound == false)
						ScullyDataRow[2] = "None";
				}
				else
					ScullyDataRow[2] = "None";


				

				ScullyDataTable.Rows.Add(ScullyDataRow);
			}
			DataView		ScullyDataView=new DataView(ScullyDataTable);
			return ScullyDataView;
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				GetSecurity();

				if (! Page.IsPostBack)
				{
					SystemTextBox.Text="localhost";
					Session["ScullySystem"]=SystemTextBox.Text;

					// Populate SelectSystemModeDropDownList
					ListItem NewItem=new ListItem("List","0");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					NewItem=new ListItem("Text","1");
					SelectSystemModeDropDownList.Items.Add(NewItem);
					SelectSystemModeDropDownList.SelectedIndex=1;
					SelectSystemModeDropDownList_SelectedIndexChanged(null,null);

					if(Session["ScullysPage"] != null)
					{
						ScullysDataGrid.CurrentPageIndex=(int) Session["ScullysPage"];
						Session.Remove("ScullysPage");
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
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.ScullysDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ScullysDataGrid_EditCommand);
			this.ScullysDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.ScullysDataGrid_PageIndexChanged);
			this.ScullysDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.ScullysDataGrid_DeleteCommand);
			this.ScullysDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.ScullysDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged ( object source, System.EventArgs e )
		{
			UpdateView();
		}

		private void ScullysDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Session.Remove("Scully");
			TableCell indexCell = e.Item.Cells[1];//bds
			Session["Index"]=indexCell.Text;
			Session["ScullysPage"]=ScullysDataGrid.CurrentPageIndex;
			Response.Redirect("ScullyForm.aspx");
		}

		private void ScullysDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				// Get Index
				TableCell indexCell = e.Item.Cells[1];//bds
				
				IScullys Scullys=(IScullys) OpcCom.Interop.CreateInstance(
                    new Guid("{948DA86B-A687-494c-9B93-569B65499B36}"),
					Session["ScullySystem"] as string,
					new NetworkCredential());

                Scullys.Purge(System.Convert.ToInt32(indexCell.Text, CultureInfo.InvariantCulture));

				ScullysDataGrid.SelectedIndex=-1;
				Session.Remove("Index");
				if(ScullysDataGrid.Items.Count == 1
				&& ScullysDataGrid.CurrentPageIndex > 0)
					ScullysDataGrid.CurrentPageIndex--;
				UpdateView();

			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			Session.Remove("Scully");
			Session.Remove("Index");
			Session["ScullysPage"]=ScullysDataGrid.CurrentPageIndex;
			Response.Redirect("ScullyForm.aspx");
		}

		private void ScullysDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			try
			{
				// if we are editing do not allow a page change
				if (ScullysDataGrid.EditItemIndex > -1)
					return;
				ScullysDataGrid.CurrentPageIndex = e.NewPageIndex;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			try
			{
				Session["ScullySystem"]=SystemDropDownList.SelectedItem.Text;
				ScullysDataGrid.CurrentPageIndex=0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		private void ScullysDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			UpdateDeleteButton( (LinkButton) e.Item.FindControl("DeleteButton") );
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, System.EventArgs e)
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
                    NewItem = new ListItem(System, Item.ToString("G", CultureInfo.InvariantCulture));
					SystemDropDownList.Items.Add(NewItem);
                    if (Session["ScullySystem"] != null
                        && (string)Session["ScullySystem"] == System)
                    {
                        SystemDropDownList.SelectedIndex = SystemDropDownList.Items.Count - 1;
                    }
					Item++;
				}

				Session["ScullySystem"]=SystemDropDownList.SelectedItem.Text;
			}

			SystemDropDownList.Visible=(SelectSystemModeDropDownList.SelectedIndex == 1) ? false : true;
			SystemTextBox.Visible=!SystemDropDownList.Visible;
		}

		protected void SystemTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				Session["ScullySystem"]=SystemTextBox.Text;
				ScullysDataGrid.CurrentPageIndex=0;
				UpdateView();
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}
	}
}
