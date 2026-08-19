/******************************************************************************
	FILE NAME:		PersonQualificationsPage.ascx.cs
	PURPOSE:		Implementation of PersonQualificationsPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2007-01-22	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2007-02-08	Richard Panachida	Added an override method to disable/enable controls. 
										Currently, it disables/enables the Add button (CSI 4083).
		2008-12-12  A. Coker            Regional settings for date and time is based on entity's site index. In
										case entity is new and a site index is not assigned to it, use site index acquired
										from page's security object.

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	using FMControls;

	public partial class PersonQualificationsPage : QualificationPageBase
	{
		protected PersonClass Person => ( (PersonForm) this.Page ).Person;

	    protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_QUALIFICATION;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_QUALIFICATION_TO_PERSON;

	    protected override DataGrid MapGrid => this.QualificationsDataGrid;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.Person.QualificationCollection;
				return maps;
			}
			set
			{
				this.Person.QualificationCollection = value;
			}
		}

        protected List<string> VersionSpecificFields => ((PersonForm)this.Page).VersionSpecificFields;

	    override protected void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			base.QualificationsDataGridItemDataBound(sender, e);
			var editButton = (LinkButton)e.Item.FindControl("Fmeditquallinkbutton");

			if (editButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				editButton.Enabled = (editButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("Qualification"));
			}

			var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletequallinkbutton");

			if (deleteButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				deleteButton.Enabled = (deleteButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("Qualification"));
			}
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
			if (this.Person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
			{
				return;
			}

			this.AddButton.Enabled = (this.AddButton.Enabled && (this.VersionSpecificFields != null)
                                  && this.VersionSpecificFields.Contains("Qualification"));
		}

		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				if (!this.Page.IsPostBack)
				{
					this.UpdateQualificationsView ( );

					if (!this.Security.HasRight ( RIGHT.MODIFY_PERSONNEL_DATA ) ||
						!this.Security.HasRight ( RIGHT.MODIFY_PERSON_QUALIFICATIONS ))
					{
						this.AddButton.Enabled = false;
					}

					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
			}
		}

		/// <summary>
		/// This method overrides and implements the base class enable controls.
		/// </summary>
		/// <param name="enable"></param>
		override protected void EnableControls ( bool enable )
		{
			this.AddButton.Enabled = enable;

			// Call the main form to disable buttons and tabs.
			var personForm = (PersonForm) this.Page;
			personForm.EnableControls ( enable );
		}

		#region Web Form Designer generated code
		override protected void OnInit ( EventArgs e )
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent ( );
			base.OnInit ( e );
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ( )
		{
			this.QualificationsDataGrid.EditCommand += new DataGridCommandEventHandler( this.QualificationsDataGridEditCommand );
			this.QualificationsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler( this.QualificationsDataGridPageIndexChanged );
			this.QualificationsDataGrid.CancelCommand += new DataGridCommandEventHandler( this.QualificationsDataGridCancelCommand );
			this.QualificationsDataGrid.UpdateCommand += new DataGridCommandEventHandler( this.QualificationsDataGridNoDueDateEditUpdateCommand );
			this.QualificationsDataGrid.DeleteCommand += new DataGridCommandEventHandler( this.QualificationsDataGridDeleteCommand );
			this.QualificationsDataGrid.ItemDataBound += new DataGridItemEventHandler( this.QualificationsDataGridItemDataBound );
			this.QualificationsDataGrid.ItemDataBound += new DataGridItemEventHandler ( this.LocalQualificationsDataGrid_ItemDataBound );
			this.AddButton.Command += new CommandEventHandler( this.AddButtonCommand );
		}

		void LocalQualificationsDataGrid_ItemDataBound ( object sender, DataGridItemEventArgs e )
		{
			try
			{
				if (this.Security.HasRight ( RIGHT.MODIFY_PERSON_QUALIFICATIONS ) == false
				   && e.Item.ItemIndex != -1)
				{
					FMDeleteLinkButton deleteButton = (FMDeleteLinkButton) e.Item.FindControl ( "DeleteButton" );
					if (deleteButton != null)
					{
						deleteButton.Enabled = false;
					}

					FMEditLinkButton editButton = (FMEditLinkButton) e.Item.FindControl ( "EditButton" );
					if (editButton != null)
					{
						editButton.Enabled = false;
					}

				}
			}
			catch (Exception except)
			{
				this.ErrorHandler ( except );
			}
		}
		#endregion
	}
}
