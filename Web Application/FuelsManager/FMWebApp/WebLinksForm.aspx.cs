/******************************************************************************
	FILE NAME:		WebLinksForm.aspx.cs
	PURPOSE:		Implementation of WebLinksForm

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
	/// Summary description for WebLinksForm.
	/// </summary>
	public partial class WebLinksForm : FMFormBase, IMenuDiscovery
	{
		private const string SortExpression = "WebLinksForm.SortExpression";
		private const string SortDirection = "WebLinksForm.SortDirection";
	
		#region Menu Interface
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

			// Web Links is a feature from Defense.  Per discussion with Jim Stevenson, the Web Links will not be supported at this time.
			// This is primarily due to the fact that it is not properly developed along the lines of the FuelsManager multi tenant architecture,
			// meaning it is not by site, entity assignable, and synchonized.  Auditing is also not supported
			if (useNewLicenseKey == 1)
            {
				return null;
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

			items.Add(new FMMenuItem
							{
								MenuItemType		= FMMenuItemType.REPORTS_WEB_LINKS,
								RootMenuName		= "Reports",
								CategoryName		= "Uncategorized",
								ItemName			= "Web Links",
								NavigateUrl			= "WebLinksForm.aspx",
								ApplyDataDictionary = ApplyDataDictionary.Apply
							});

			return items;
		}
		#endregion


		protected void EnableControls(bool bEnable)
		{
			this.PageSizeDropDown.Enabled = bEnable;
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!Page.IsPostBack)
				{
					
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
			this.DataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.DataGridPageIndexChanged);
			this.DataGrid.ItemDataBound += new DataGridItemEventHandler(this.DataGridItemDataBound);
		}
		#endregion

		protected void PageSizeDropDownSelectedIndexChanged ( object source, EventArgs e )
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
			                      "var AddButton=document.getElementById(\"PageSizeDropDown\");\n" +
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
		/// This method will built the data view from the web link items.
		/// </summary>
		/// <returns></returns>
		private ICollection EnumerateWebLinks()
		{
			var webLinkCollection = (WebLinkCollectionClass)this.Session["WebLinkCollection"];

			var mapDataTable = new DataTable();

			mapDataTable.Columns.Add("LinkName", typeof(string));
			mapDataTable.Columns.Add("LinkDescription", typeof(string));
			mapDataTable.Columns.Add("LinkAddress", typeof(string));

			foreach(WebLink webLink in webLinkCollection)
			{
				DataRow mapDataRow = mapDataTable.NewRow();

				mapDataRow[0] = webLink.LinkName;
				mapDataRow[1] = webLink.LinkDescription;
				mapDataRow[2] = webLink.LinkAddress;

				mapDataTable.Rows.Add(mapDataRow);
			}

			var webLinkDataView = new DataView(mapDataTable);
			return webLinkDataView;
		}

		/// <summary>
		/// This method handles the grid sorting based on the sort expression.
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
		/// This method handles the data grid page change.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		protected void DataGridPageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			this.DataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		/// <summary>
		/// This method will process each row for binding to the grid.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void DataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			this.GetSecurity();

			if (this.DataGrid != null)
			{
				// Now set the focus to the Link Name control
				Control ctrl = e.Item.FindControl("LinkNameTextBox");

				if (ctrl != null)
				{
					const string Script = @"<script language='javascript'> document.getElementById('{0}').focus(); </script>";
					Page.ClientScript.RegisterStartupScript(this.GetType(), "page_set_focus", string.Format(Script, ctrl.ClientID));
				}
			}
		}
	}
}
