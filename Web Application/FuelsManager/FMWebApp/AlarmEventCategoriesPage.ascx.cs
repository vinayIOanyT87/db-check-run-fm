/******************************************************************************
	FILE NAME:		AlarmEventCategoriesPage.ascx.cs
	PURPOSE:		Implementation of AlarmEventCategoriesPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-13	Richard Panachida	Error message did not indicate the correct error. Should indicate there was
										a duplicate ID (CSI 3379).
		2007-01-22	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-02-09	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-03-15	Richard Panachida	Corrected issue with new Add button not being disable/enable (CSI 4083).
*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Interfaces;

	using global::FMWebApp;

	/// <summary>
	///		Summary description for AlarmEventCategoriesPage.
	/// </summary>
	public partial class AlarmEventCategoriesPage : FMUserControlBase, IEntityDiscovery
	{
		private const string ERROR_MSG_001 = "Duplicate ID";

		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.ALARM_EVENT_CATEGORY;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var applicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
				appStrings => appStrings.EnumerateByType(Security, STRING_TYPE.ALARM_EVENT_CATEGORY));

			EntityToSiteMapCollectionClass EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (ApplicationStringClass ApplicationString in applicationStringCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == ApplicationString.SiteGuid)
						continue;

					if (Security.LoginSiteGuid != ApplicationString.SiteGuid)
						continue;

				}
				else
				{
					if (Security.SiteGuid != ApplicationString.SiteGuid)
						continue;
				}

				EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(ApplicationString);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IApplicationStrings);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			FMChannelHelper.MakeCall<IApplicationStrings>(
				appStrings =>
				{
					ApplicationStringClass applicationString = appStrings.Get(security, guid);
					applicationString.SiteGuid = SiteGuid;
					appStrings.Modify(security, applicationString);
				});
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IApplicationStrings, Guid>(
				appStrings => appStrings.GetIdentityGuid(security, STRING_TYPE.ALARM_EVENT_CATEGORY, ID));
		}

		bool IEntityDiscovery.EntityAssignable { get { return true; } }

		protected void UpdateView()
		{
			ICollection Categories = this.EnumerateCategories();

			this.AlarmCatsPageSizeDropDown.SetPageSize(this.CategoriesDataGrid, Categories.Count);

			this.CategoriesDataGrid.DataSource = Categories;
			this.CategoriesDataGrid.DataBind();
		}

		private ICollection EnumerateCategories()
		{
			ApplicationStringCollectionClass ApplicationStringCollection;
			ApplicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

			DataTable ApplicationStringDataTable = new DataTable();
			DataRow ApplicationStringDataRow;
			ApplicationStringClass ApplicationString;

			ApplicationStringDataTable.Columns.Add("SiteGuid", typeof(Guid));
			ApplicationStringDataTable.Columns.Add("Index", typeof(Int32));
			ApplicationStringDataTable.Columns.Add("String", typeof(string));

			for (int iItem = 0; iItem < ApplicationStringCollection.Count; iItem++)
			{
				ApplicationStringDataRow = ApplicationStringDataTable.NewRow();

				ApplicationString = (ApplicationStringClass)ApplicationStringCollection[iItem];
				ApplicationStringDataRow["SiteGuid"] = ApplicationString.SiteGuid;
				ApplicationStringDataRow["Index"] = iItem;
				ApplicationStringDataRow["String"] = ApplicationString.ID;

				ApplicationStringDataTable.Rows.Add(ApplicationStringDataRow);
			}
			DataView ApplicationStringDataView = new DataView(ApplicationStringDataTable);
			return ApplicationStringDataView;
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
					{
						this.AddButton.Enabled = false;
						this.AddButton2.Enabled = false;
					}

					var applicationStringCollection = FMChannelHelper.MakeCall<IApplicationStrings, ApplicationStringCollectionClass>(
						appStrings => appStrings.EnumerateByType(this.Security, STRING_TYPE.ALARM_EVENT_CATEGORY));

					this.Session["ApplicationStringCollection"] = applicationStringCollection;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method enables and disables controls.
		/// </summary>
		/// <param name="enable"></param>
		protected void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
			this.AddButton2.Enabled = enable;
			this.AlarmCatsPageSizeDropDown.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			AlarmEventConfigurationForm alarmEventConfigurationForm = (AlarmEventConfigurationForm)this.Page;
			alarmEventConfigurationForm.EnableControls(enable);
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.CategoriesDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CategoriesDataGrid_EditCommand);
			this.CategoriesDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.CategoriesDataGrid_PageIndexChanged);
			this.CategoriesDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CategoriesDataGrid_CancelCommand);
			this.CategoriesDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CategoriesDataGrid_UpdateCommand);
			this.CategoriesDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.CategoriesDataGrid_DeleteCommand);
			this.CategoriesDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.CategoriesDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, System.EventArgs e)
		{
			this.UpdateView();
		}

		protected void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			ApplicationStringCollectionClass ApplicationStringCollection;
			ApplicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];
			ApplicationStringClass ApplicationString = new ApplicationStringClass();

			ApplicationString.Type = STRING_TYPE.ALARM_EVENT_CATEGORY;
			ApplicationStringCollection.Add(ApplicationString);
			this.CategoriesDataGrid.CurrentPageIndex = (ApplicationStringCollection.Count - 1) / this.CategoriesDataGrid.PageSize;
			this.CategoriesDataGrid.EditItemIndex = (ApplicationStringCollection.Count - 1) % this.CategoriesDataGrid.PageSize;

			this.EnableControls(false);
			this.UpdateView();
		}

		protected void CategoriesDataGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

			if (IndexLabel != null)
			{
				ApplicationStringCollectionClass ApplicationStringCollection;
				ApplicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];
				ApplicationStringClass ApplicationString;
				ApplicationString = (ApplicationStringClass)ApplicationStringCollection[System.Convert.ToInt32(IndexLabel.Text)];

				if (ApplicationString.IdentityGuid == Guid.Empty)
				{
					ApplicationStringCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));

					if ((this.CategoriesDataGrid.Items.Count == 1) && (this.CategoriesDataGrid.CurrentPageIndex > 0))
					{
						this.CategoriesDataGrid.CurrentPageIndex--;
					}
				}

				this.CategoriesDataGrid.EditItemIndex = -1;
				this.EnableControls(true);
				this.UpdateView();
			}
		}

		protected void CategoriesDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");
				if (IndexLabel != null)
				{
					ApplicationStringCollectionClass ApplicationStringCollection;
					ApplicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

					ApplicationStringClass ApplicationString;
					ApplicationString = (ApplicationStringClass)ApplicationStringCollection[System.Convert.ToInt32(IndexLabel.Text)];

					if (this.CategoriesDataGrid.EditItemIndex == e.Item.ItemIndex)
					{
						this.CategoriesDataGrid.EditItemIndex = -1;
						this.EnableControls(true);
					}
					else if (this.CategoriesDataGrid.EditItemIndex > e.Item.ItemIndex)
						this.CategoriesDataGrid.EditItemIndex--;


					// Non empty identity guid indicates ApplicationString has been committed to database
					if (ApplicationString.IdentityGuid != Guid.Empty)
					{
						FMChannelHelper.MakeCall<IApplicationStrings>(
							appStrings => appStrings.Purge(this.Security, ApplicationString.IdentityGuid));
					}

					ApplicationStringCollection.RemoveAt(System.Convert.ToInt32(IndexLabel.Text));
					if (this.CategoriesDataGrid.Items.Count == 1
					&& this.CategoriesDataGrid.CurrentPageIndex > 0)
						this.CategoriesDataGrid.CurrentPageIndex--;

					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void CategoriesDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			this.EnableControls(false);
			this.CategoriesDataGrid.EditItemIndex = e.Item.ItemIndex;
			this.UpdateView();
		}

		protected void CategoriesDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.CategoriesDataGrid.EditItemIndex > -1)
				return;
			this.CategoriesDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		protected void CategoriesDataGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				Label IndexLabel = (Label)e.Item.FindControl("IndexLabel");

				if (IndexLabel != null)
				{
					ApplicationStringCollectionClass ApplicationStringCollection;
					ApplicationStringCollection = (ApplicationStringCollectionClass)this.Session["ApplicationStringCollection"];

					TextBox StringTextBox = (TextBox)e.Item.FindControl("StringTextBox");

					var applicationString = (ApplicationStringClass)ApplicationStringCollection[System.Convert.ToInt32(IndexLabel.Text)];
					applicationString.ID = StringTextBox.Text;

					FMChannelHelper.MakeCall<IApplicationStrings>(
						appStrings =>
						{
							if (applicationString.IdentityGuid == Guid.Empty)
							{
								applicationString.IdentityGuid = appStrings.Add(this.Security, applicationString);
								applicationString.SiteGuid = this.Security.SiteGuid;
							}
							else
							{
								appStrings.Modify(this.Security, applicationString);
							}
						});

					this.EnableControls(true);
					this.CategoriesDataGrid.EditItemIndex = -1;
					this.UpdateView();
				}
			}
			catch (Exception except)
			{
				if (except.Message.ToUpper().StartsWith("APPLICATION STRING EXISTS") == true)
				{
					Exception newExcept = new Exception(AlarmEventCategoriesPage.ERROR_MSG_001);
					this.ErrorHandler(newExcept);
				}
				else
				{
					this.ErrorHandler(except);
				}
			}
		}

		protected void CategoriesDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			LinkButton EditButton = (LinkButton)e.Item.FindControl("EditButton");
			LinkButton DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
			Label SiteGuidLabel = (Label)e.Item.FindControl("SiteGuidLabel");
			if (EditButton != null
			&& DeleteButton != null
			&& SiteGuidLabel != null)
			{
				if (!this.Security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS)
				|| this.Security.SiteGuid != Guid.Parse(SiteGuidLabel.Text))
				{
					EditButton.Enabled = false;
					EditButton.Text = "<img src=Images/Edit_un.gif border=0 align=absmiddle alt='Edit this item'>";
					DeleteButton.Enabled = false;
					DeleteButton.Text = "<img src=Images/Delete_un.gif border=0 align=absmiddle alt='Delete this item'>";
				}
			}
		}
	}
}
