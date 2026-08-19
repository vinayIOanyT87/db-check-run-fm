/******************************************************************************
	FILE NAME:		PersonLicensesPage.ascx.cs
	PURPOSE:		Implementation of PersonLicensesPage

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
		2007-06-18  A.Sang				CSI4848 Disable the Add button based on the security right

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

	public partial class PersonLicensesPage : QualificationPageBase
	{
		protected PersonClass Person => ( (PersonForm) this.Page ).Person;

	    protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.PERSON_LICENSE;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.PERSON_LICENSE_TO_PERSON;

	    protected override DataGrid MapGrid => this.QualificationsDataGrid;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				if (this.Person != null)
				{
					QualificationMapCollectionClass maps = this.Person.LicenseCollection;
					return maps;
				}

				return new QualificationMapCollectionClass ( );
			}
			set
			{
				this.Person.LicenseCollection = value;
			}
		}

		protected List<string> VersionSpecificFields => ((PersonForm)this.Page).VersionSpecificFields;

	    override protected void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
		{
			base.QualificationsDataGridItemDataBound(sender, e);
			var editButton = (LinkButton)e.Item.FindControl("Fmeditlinkbutton1");

			if (editButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				editButton.Enabled = (editButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("License"));
			}

			var deleteButton = (LinkButton)e.Item.FindControl("Fmdeletelinkbutton1");

			if (deleteButton != null)
			{
				bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

				if (this.Person.IdentityGuid.Equals(Guid.Empty)
					  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
				{
					return;
				}

				deleteButton.Enabled = (deleteButton.Enabled && (this.VersionSpecificFields != null)
                                                  && this.VersionSpecificFields.Contains("License"));
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
                                  && this.VersionSpecificFields.Contains("License"));
		}

		protected void Page_Load ( object sender, EventArgs e )
		{
			try
			{
				if (this.Page.IsPostBack == false)
				{
					this.UpdateQualificationsView ( );

					if (this.Security.HasRight ( RIGHT.MODIFY_PERSONNEL_DATA ) == false)
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
			if (this.Security.HasRight ( RIGHT.MODIFY_PERSONNEL_DATA ))
			{
				this.AddButton.Enabled = enable;
			}

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
			this.QualificationsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.QualificationsDataGridEditCommand );
			this.QualificationsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler ( this.QualificationsDataGridPageIndexChanged );
			this.QualificationsDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.QualificationsDataGridCancelCommand );
			this.QualificationsDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.QualificationsDataGridUpdateCommand );
			this.QualificationsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler ( this.QualificationsDataGridDeleteCommand );
			this.QualificationsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler ( this.QualificationsDataGridItemDataBound );
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler ( this.AddButtonCommand );

		}
		#endregion

		public void UpdateData ( )
		{
			this.UpdateQualificationsView ( );
		}
	}
}
