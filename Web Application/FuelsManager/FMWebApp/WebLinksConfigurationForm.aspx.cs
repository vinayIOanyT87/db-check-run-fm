/******************************************************************************
	FILE NAME:		WebLinksConfigurationForm.aspx.cs
	PURPOSE:		Implementation of WebLinksConfigurationForm

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	/// Summary description for WebLinksConfigurationForm.
	/// </summary>
	public partial class WebLinksConfigurationForm : FMAutoSubmitFormBase, IMenuDiscovery
	{
		private int priorEditItemIndex = -2;

		private const string SortExpression = "WebLinksConfigurationForm.SortExpression";
		private const string SortDirection = "WebLinksConfigurationForm.SortDirection";

		#region Menu interface
		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {

            }
            else
            {
                // Depends Upon Shared Components Config
                if ((options & 0x4000) == 0)
                {
                    return null;
                }
            }
            var items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.CONFIGURE_WEB_LINKS))
			{
				return null;
			}

			items.Add(new FMMenuItem 
							{
								MenuItemType		= FMMenuItemType.ADMIN_SYSTEM_WEB_LINKS_CONFIGURATION,
								RootMenuName		= "Administration",
								CategoryName		= "System",
								ItemName			= "Web Links Configuration",
								NavigateUrl			= "WebLinksConfigurationForm.aspx",
								ApplyDataDictionary = ApplyDataDictionary.Apply
							});

			return items;
		}
		#endregion


		protected void EnableControls(bool bEnable)
		{
			this.AddButton.Enabled = bEnable;
			this.TopAddButton.Enabled = bEnable;
			this.PageSizeDropDown.Enabled = bEnable;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!Page.IsPostBack)
				{
					// Enumerate
					WebLinkCollectionClass webLinkCollection = 
								FMChannelHelper.MakeCall<IWebLinks, WebLinkCollectionClass>(x => x.Enumerate(this.Security));

					Session["WebLinkCollection"] = webLinkCollection;

					this.UpdateView();
					this.SetPageFocus();
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
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.TopAddButton.Command		+= new CommandEventHandler(this.AddButtonCommand);
			this.DataGrid.EditCommand		+= new DataGridCommandEventHandler(this.DataGridEditCommand);
			this.DataGrid.PageIndexChanged	+= new DataGridPageChangedEventHandler(this.DataGridPageIndexChanged);
			this.DataGrid.CancelCommand		+= new DataGridCommandEventHandler(this.DataGridCancelCommand);
			this.DataGrid.UpdateCommand		+= new DataGridCommandEventHandler(this.DataGridUpdateCommand);
			this.DataGrid.DeleteCommand		+= new DataGridCommandEventHandler(this.DataGridDeleteCommand);
			this.DataGrid.ItemDataBound		+= new DataGridItemEventHandler(this.DataGridItemDataBound);
			this.AddButton.Command			+= new CommandEventHandler(this.AddButtonCommand);
		}
		#endregion

		protected void PageSizeDropDownSelectedIndexChanged(object source, EventArgs e)
		{
			this.UpdateView();
		}

		protected void UpdateView()
		{
			this.UpdateView(this.PageSizeDropDown);
		}

		protected void SetPageFocus()
		{
			const string Script = "<script language=\"jscript\">\n" +
			                      "var AddButton=document.getElementById(\"TopAddButton\");\n" +
			                      "if(!AddButton.disabled)\n" +
			                      "AddButton.focus();\n" +
			                      "</script>\n";

			Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", Script);
		}

		/// <summary>
		/// This method updates the view in the summary page. It supports sorting if the derived
		/// class has the sort command implemented.
		/// </summary>
		/// <param name="pageSizeDropDown"></param>
		protected void UpdateView(FMPageSizeDropDown pageSizeDropDown)
		{
			if (Session[SortExpression] != null && Session[SortDirection] != null)
			{
				ICollection data = this.EnumerateWebLinks();
				var dataView = data as DataView;

				if (dataView != null)
				{
					dataView.Sort = String.Format("{0} {1}", this.Session[SortExpression], this.Session[SortDirection]);

					pageSizeDropDown.SetPageSize(this.DataGrid, dataView.Count);
					this.DataGrid.DataSource = dataView;
				}

				this.DataGrid.DataBind();
			}
			else
			{
				ICollection data = this.EnumerateWebLinks();

				if (pageSizeDropDown != null)
				{
					pageSizeDropDown.SetPageSize(this.DataGrid, data.Count);
				}

				this.DataGrid.DataSource = data;
				this.DataGrid.DataBind();
			}
		}

		/// <summary>
		/// This method will create a data view for the web link items.
		/// </summary>
		/// <returns></returns>
		private ICollection EnumerateWebLinks()
		{
			var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("LinkGuid", typeof(Guid));
			mapDataTable.Columns.Add("LinkName", typeof(string));
			mapDataTable.Columns.Add("LinkDescription", typeof(string));
			mapDataTable.Columns.Add("LinkAddress", typeof(string));

			foreach(WebLink webLink in webLinkCollection)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				mapDataRow[0] = webLink.IdentityGuid;
				mapDataRow[1] = webLink.LinkName;
				mapDataRow[2] = webLink.LinkDescription;
				mapDataRow[3] = webLink.LinkAddress;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var webLinkDataView = new DataView(mapDataTable);
			return webLinkDataView;
		}

		/// <summary>
		/// This method will add a new web link item to the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void AddButtonCommand(object sender, CommandEventArgs e)
		{
			var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];
			var webLink = new WebLink();

			webLinkCollection.Add(webLink);

			this.DataGrid.CurrentPageIndex = (webLinkCollection.Count - 1) / this.DataGrid.PageSize;
			this.DataGrid.EditItemIndex = (webLinkCollection.Count - 1) % this.DataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateView();
		}

		/// <summary>
		/// This method will sort the grid based on the sort expression.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <param name="pageSizeDropDown"></param>
		protected void DataGridSortCommand(object source, DataGridSortCommandEventArgs e, FMPageSizeDropDown pageSizeDropDown)
		{
			try
			{
				var sortExpression = Session[SortExpression] as string;
				var sortDirection = Session[SortDirection] as string;

				if (e.SortExpression != sortExpression)
				{
					Session[SortExpression] = e.SortExpression;
					Session[SortDirection] = "ASC";
				}
				else
				{
					if (sortDirection == "DESC")
					{
						Session[SortDirection] = "ASC";
					}
					else
					{
						Session[SortDirection] = "DESC";
					}
				}

				this.DataGrid.CurrentPageIndex = 0;
				this.UpdateView(pageSizeDropDown);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method the cancel process of a web link item being edited
		/// in the grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridCancelCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var guidLabel = (Label)e.Item.FindControl("GuidLabel");

				if (guidLabel != null)
				{
					Guid webLinkGuid;

					if(Guid.TryParseExact(guidLabel.Text, "D", out webLinkGuid) == false)
					{
						webLinkGuid = Guid.Empty;
					}

					var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];

					WebLink webLink = webLinkCollection.Find(x => x.IdentityGuid == webLinkGuid);
					
					if (webLink.IdentityGuid == Guid.Empty)
					{
						webLinkCollection.Remove(webLink);

						if (this.DataGrid.Items.Count == 1 && this.DataGrid.CurrentPageIndex > 0)
						{
							this.DataGrid.CurrentPageIndex--;
						}
					}
					else
					{
						WebLink originalWebLink = 
								FMChannelHelper.MakeCall<IWebLinks, WebLink>(x => x.Get(this.Security, webLinkGuid));
						
						webLink.LinkName		= originalWebLink.LinkName;
						webLink.LinkDescription = originalWebLink.LinkDescription;
						webLink.LinkAddress		= originalWebLink.LinkAddress;
						webLink.IdentityGuid	= originalWebLink.IdentityGuid;
					}

					this.EnableControls(true);
					this.priorEditItemIndex = this.DataGrid.EditItemIndex;
					this.DataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will delete a web link item from the grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var guidLabel = (Label)e.Item.FindControl("GuidLabel");
				
				if (guidLabel != null)
				{
					Guid webLinkGuid;

					if(Guid.TryParseExact(guidLabel.Text, "D", out webLinkGuid) == false)
					{
						webLinkGuid = Guid.Empty;
					}

					var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];

					WebLink webLink = webLinkCollection.Find(x => x.IdentityGuid == webLinkGuid);

					// Non Zero Index indicates WebLink has been committed to database
					if (webLink.IdentityGuid != Guid.Empty)
					{
						this.GetSecurity();

						FMChannelHelper.MakeCall<IWebLinks>(x => x.Purge(this.Security, webLinkGuid));
					}

					if (this.DataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.DataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.DataGrid.EditItemIndex > e.Item.ItemIndex)
					{
						this.DataGrid.EditItemIndex--;
					}

					webLinkCollection.Remove(webLink);

					if (this.DataGrid.CurrentPageIndex > 0
						&& this.DataGrid.CurrentPageIndex * this.DataGrid.PageSize >= webLinkCollection.Count)
					{
						this.DataGrid.CurrentPageIndex--;
					}

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will set the web link item in the grid to be in edit
		/// mode.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridEditCommand(object source, DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.DataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateView();
		}

		/// <summary>
		/// This method will update the grid to the specified page.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.DataGrid.EditItemIndex > -1)
			{
				return;
			}

			this.DataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// This method will update the web link item being edited in the 
		/// grid.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridUpdateCommand(object source, DataGridCommandEventArgs e)
		{
			try
			{
				var guidLabel = (Label)e.Item.FindControl("GuidLabel");
				
				if (guidLabel != null)
				{
					Guid webLinkGuid;

					if(Guid.TryParseExact(guidLabel.Text, "D", out webLinkGuid) == false)
					{
						webLinkGuid = Guid.Empty;
					}

					var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];

					var linkNameTextBox			= (TextBox)e.Item.FindControl("LinkNameTextBox");
					var linkDescriptionTextBox	= (TextBox)e.Item.FindControl("LinkDescriptionTextBox");
					var linkAddressTextBox		= (TextBox)e.Item.FindControl("LinkAddressTextBox");

					if (linkNameTextBox.Text.Length == 0)
					{
						throw new Exception("Link Name is required");
					}


					if (linkDescriptionTextBox.Text.Length == 0)
					{
						throw new Exception("Link Description is required");
					}


					if (linkAddressTextBox.Text.Length == 0)
					{
						throw new Exception("Link Address is required");
					}

					var webLink				= webLinkCollection.Find(x => x.IdentityGuid == webLinkGuid);
					webLink.LinkName		= linkNameTextBox.Text;
					webLink.LinkDescription = linkDescriptionTextBox.Text;
					webLink.LinkAddress		= linkAddressTextBox.Text;

					this.GetSecurity();

					if (webLink.IdentityGuid == Guid.Empty)
					{
						Guid newWebLinkGuid = FMChannelHelper.MakeCall<IWebLinks, Guid>(x => x.Add(this.Security, webLink));
						webLink.IdentityGuid = newWebLinkGuid;
					}
					else
					{
						FMChannelHelper.MakeCall<IWebLinks>(x => x.Modify(this.Security, webLink));
					}

					this.EnableControls(true);
					this.priorEditItemIndex = this.DataGrid.EditItemIndex;
					this.DataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				ErrorHandler(except);
				this.UpdateView();
			}
		}

		/// <summary>
		/// This method will setup the items in the grid during the 
		/// binding process.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (this.DataGrid == null)
			{
				return;
			}

			// Disable the Delete link button, Link Name text box, and Link Description text box
			// for the Contact Us and Support data grid items
			var rowView = e.Item.DataItem as DataRowView;
			
			if (rowView != null && rowView.Row != null && rowView.Row.ItemArray.Length > 1)
			{
				var linkName = rowView.Row.ItemArray[1] as string;
				
				if (linkName == WebLink.ContactUsLinkName ||
					linkName == WebLink.SupportLinkName)
				{
					// Disable the Delete link button
					var deleteButton = e.Item.FindControl("DeleteButton") as LinkButton;

					if (deleteButton != null)
					{
						deleteButton.Enabled = false;
					}

					// Disable the Link Name text box
					var linkNameTextBox = e.Item.FindControl("LinkNameTextBox") as TextBox;

					if (linkNameTextBox != null)
					{
						linkNameTextBox.Enabled = false;
					}

					// Disable the Link Description text box
					var linkDescriptionTextBox = e.Item.FindControl("LinkDescriptionTextBox") as TextBox;

					if (linkDescriptionTextBox != null)
					{
						linkDescriptionTextBox.Enabled = false;
					}
				}
			}

			if (this.DataGrid.EditItemIndex == e.Item.ItemIndex ||
				this.priorEditItemIndex == e.Item.ItemIndex)
			{
				// Now set the focus to the edit control
				Control ctrl;

				if (this.DataGrid.EditItemIndex == e.Item.ItemIndex)
				{
					ctrl = e.Item.FindControl("LinkNameTextBox");
				}
				else
				{
					ctrl = e.Item.FindControl("EditButton");
				}

				if (ctrl != null)
				{
					const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", string.Format(Script, ctrl.ClientID));
				}
			}
		}
	}
}
