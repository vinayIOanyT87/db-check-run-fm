/******************************************************************************

	FILE NAME:		AlarmEventEmailGroupsPage.ascx.cs


	PURPOSE:			Implementation of AlarmEventEmailGroupsPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		2007-06-29  A.Sang		Change EmailGroupClass(Site) to EmailGroupClass() because EmailGroup time is local time CSI4701
		2009-03-23	I.Orndorff	- Modified "EnumerateEmailGroups()" to support setting the site index. This fixes bug 2058.
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
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	///		Summary description for AlarmEventEmailGroupsPage.
	/// </summary>
	public partial class AlarmEventEmailGroupsPage : FMUserControlBase, IEntityDiscovery
	{
		ENTITY_TYPE IEntityDiscovery.EntityType
		{
			get
			{
				return ENTITY_TYPE.EMAIL_GROUP;
			}
		}

		EntityToSiteMapCollectionClass IEntityDiscovery.EnumerateEntityMaps(SecurityClass Security, ENTITY_ASSIGNMENT_TYPE Type)
		{
			var emailGroupCollection = FMChannelHelper.MakeCall<IEmailGroups, EmailGroupCollectionClass>(
				emailGroups => emailGroups.Enumerate(Security));

			EntityToSiteMapCollectionClass EntityToSiteMapCollection = new EntityToSiteMapCollectionClass();

			foreach (EmailGroupClass EmailGroup in emailGroupCollection)
			{
				if (Type == ENTITY_ASSIGNMENT_TYPE.ASSIGNED)
				{
					if (Security.SiteGuid == EmailGroup.SiteGuid)
						continue;

					if (Security.LoginSiteGuid != EmailGroup.SiteGuid)
						continue;
				}
				else
				{
					if (Security.SiteGuid != EmailGroup.SiteGuid)
						continue;
				}

				EntityToSiteMapClass EntityToSiteMap = new EntityToSiteMapClass(EmailGroup);
				EntityToSiteMapCollection.Add(EntityToSiteMap);
			}
			return EntityToSiteMapCollection;
		}

		Type IEntityDiscovery.EntityEngineType
		{
			get
			{
				return typeof(IEmailGroups);
			}
		}

		void IEntityDiscovery.SetSiteGuid(SecurityClass security, Guid guid, Guid SiteGuid)
		{
			FMChannelHelper.MakeCall<IEmailGroups>(
				emailGroups =>
				{
					EmailGroupClass emailGroup = emailGroups.Get(security, guid);
					emailGroup.SiteGuid = SiteGuid;
					emailGroups.Modify(security, emailGroup);
				});
		}

		Guid IEntityDiscovery.GetIdentityGuid(SecurityClass security, string ID)
		{
			return FMChannelHelper.MakeCall<IEmailGroups, Guid>(
				emailGroups => emailGroups.GetIdentityGuid(security, ID));
		}

		bool IEntityDiscovery.EntityAssignable { get { return true; } }

		private void UpdateView()
		{
			ICollection Groups = this.EnumerateEmailGroups();

			this.AlarmEmailPageSizeDropDown.SetPageSize(this.EmailGroupsDataGrid, Groups.Count);

			this.EmailGroupsDataGrid.DataSource = Groups;
			this.EmailGroupsDataGrid.DataBind();
		}

		private ICollection EnumerateEmailGroups()
		{
			var EmailGroupCollection = FMChannelHelper.MakeCall<IEmailGroups, EmailGroupCollectionClass>(
				emailGroups => emailGroups.Enumerate(this.Security));

			DataTable EmailGroupDataTable = new DataTable();
			DataRow EmailGroupDataRow;
			EmailGroupClass EmailGroup;

			EmailGroupDataTable.Columns.Add("SiteGuid", typeof(Guid));
			EmailGroupDataTable.Columns.Add("IdentityGuid", typeof(Guid));
			EmailGroupDataTable.Columns.Add("ID", typeof(string));
			EmailGroupDataTable.Columns.Add("Enabled", typeof(bool));

			SiteClass CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
																);

			DateTimeOffset CurrentTime = TimeConverter.Now(CurrentSite);
			DateTimeOffset SiteTimeToday = TimeConverter.ToDate(CurrentTime);

			for (int iItem = 0; iItem < EmailGroupCollection.Count; iItem++)
			{
				EmailGroupDataRow = EmailGroupDataTable.NewRow();

				EmailGroup = (EmailGroupClass)EmailGroupCollection.Item(iItem);
				EmailGroupDataRow["SiteGuid"] = EmailGroup.SiteGuid;
				EmailGroupDataRow["IdentityGuid"] = EmailGroup.IdentityGuid;
				EmailGroupDataRow["ID"] = EmailGroup.ID;
				if (EmailGroup.AlwaysEnabled
				|| (CurrentTime >= SiteTimeToday + EmailGroup.StartTime.Value.TimeOfDay
				&& CurrentTime <= SiteTimeToday + EmailGroup.EndTime.Value.TimeOfDay))
					EmailGroupDataRow["Enabled"] = true;
				else
					EmailGroupDataRow["Enabled"] = false;

				EmailGroupDataTable.Rows.Add(EmailGroupDataRow);
			}
			DataView EmailGroupDataView = new DataView(EmailGroupDataTable);
			return EmailGroupDataView;
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

					if (this.Session["PageIndex"] != null)
						this.EmailGroupsDataGrid.CurrentPageIndex = (int)this.Session["PageIndex"];

					this.UpdateView();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.EmailGroupsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.EmailGroupsDataGrid_EditCommand);
			this.EmailGroupsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.EmailGroupsDataGrid_PageIndexChanged);
			this.EmailGroupsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.EmailGroupsDataGrid_DeleteCommand);
			this.EmailGroupsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.EmailGroupsDataGrid_ItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);
			this.AddButton2.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButton_Command);

		}
		#endregion

		protected void PageSizeDropDown_SelectedIndexChanged(object source, System.EventArgs e)
		{
			this.UpdateView();
		}

		private void EmailGroupsDataGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			this.Session.Remove("EmailGroup");
			TableCell identityGuidCell = e.Item.Cells[2];//bds
			this.Session["IdentityGuid"] = identityGuidCell.Text;
			this.Session["TabIndex"] = 2;
			this.Session["PageIndex"] = this.EmailGroupsDataGrid.CurrentPageIndex;
			this.Redirect("EmailGroupForm.aspx");
		}

		private void EmailGroupsDataGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				// Get IdentityGuid
				TableCell identityGuidCell = e.Item.Cells[2];//bds

				FMChannelHelper.MakeCall<IEmailGroups>(
					emailGroups => emailGroups.Purge(this.Security, Guid.Parse(identityGuidCell.Text)));

				this.EmailGroupsDataGrid.SelectedIndex = -1;
				this.Session.Remove("IdentityGuid");
				if (this.EmailGroupsDataGrid.Items.Count == 1
				&& this.EmailGroupsDataGrid.CurrentPageIndex > 0)
					this.EmailGroupsDataGrid.CurrentPageIndex--;
				this.UpdateView();

			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void AddButton_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
		{
			this.Session.Remove("EmailGroup");
			this.Session.Remove("IdentityGuid");
			this.Session["TabIndex"] = 2;
			this.Session["PageIndex"] = this.EmailGroupsDataGrid.CurrentPageIndex;
			this.Redirect("EmailGroupForm.aspx");
		}

		private void EmailGroupsDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			// if we are editing do not allow a page change
			if (this.EmailGroupsDataGrid.EditItemIndex > -1)
				return;
			this.EmailGroupsDataGrid.CurrentPageIndex = e.NewPageIndex;
			this.UpdateView();
		}

		private void EmailGroupsDataGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
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
