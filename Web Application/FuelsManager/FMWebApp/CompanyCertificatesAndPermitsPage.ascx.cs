/******************************************************************************
	FILE NAME:		CompanyCertificatesAndPermitsPage.ascx.cs
	PURPOSE:		Implementation of CompanyCertificatesAndPermitsPage

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
		2007-03-15	Richard Panachida	Corrected view rights permission error (CSI 4231).
*******************************************************************************/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using FMControls;
using FMBusinessObjects.DataObjects;

namespace FMWebApp
{
	using FuelsManager.FMWebApp;

	public partial class CompanyCertificatesAndPermitsPage : QualificationPageBase
	{

		protected CompanyClass Company
		{
			get
			{
				return ((CompanyForm) Page).Company;
			}
		}


		protected override QUALIFICATION_TYPE PageQualificationType
		{
			get{return QUALIFICATION_TYPE.COMPANY_CERTIFICATE_AND_PERMIT;}
		}

		protected override QUALIFICATION_MAP_TYPE PageQualificationMapType
		{
			get { return QUALIFICATION_MAP_TYPE.COMPANY_CERTIFICATE_AND_PERMIT_TO_COMPANY; }
		}

		protected override DataGrid MapGrid
		{
			get{return QualificationsDataGrid;}
		}

		protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass Maps=(QualificationMapCollectionClass) Company.CertificateAndPermitCollection;
				return Maps;
			}
			set
			{
				Company.CertificateAndPermitCollection=value;
			}
		}


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (Page.IsPostBack == false)
				{
					// Check the user rights and set the controls.
					this.SetUserRights();

					UpdateQualificationsView();
                    SetFieldAccessibilityForChildRecordVersion();
				}
			}	
			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method checks the user rights and sets the controls to be disabled.
		/// </summary>
		private void SetUserRights()
		{
			if (Security.HasRight(RIGHT.MODIFY_COMPANY_DATA) == false)
			{
				this.AddButton.Enabled  = false;
			}
		}

		/// <summary>
		/// This method overrides and implements the base class enable controls.
		/// </summary>
		/// <param name="enable"></param>
		override protected void EnableControls(bool enable)
		{
			if (Security.HasRight ( RIGHT.MODIFY_COMPANY_DATA ))
			{
				this.AddButton.Enabled = enable;
			}

			// Call the main form to disable buttons and tabs.
			CompanyForm companyForm = (CompanyForm) Page;
			companyForm.EnableControls(enable);
		}

        protected override void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
        {
            base.QualificationsDataGridItemDataBound(sender, e);

            //Set the availability of the Grid editing buttons for child record versions
            bool currentSiteOwnsRecordVersion = (Company.SiteGuid == Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((CompanyForm)Page).VersionSpecificFields;
            if ((Company.IdentityGuid.Equals(Guid.Empty)) || (currentSiteOwnsRecordVersion && Company.IdentityGuid.Equals(Company.MasterRecordGuid)))
                return;
            LinkButton EditButton = (LinkButton)e.Item.FindControl("EditButton");
            LinkButton DeleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
            if ((EditButton != null) && (DeleteButton != null))
            {
                if ((versionSpecificFields == null) || !versionSpecificFields.Contains("CertificatesAndPermits"))
                {
                    EditButton.Enabled = false;
                    DeleteButton.Enabled = false;
                }
            }
        }


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((CompanyForm)Page).VersionSpecificFields;
            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (versionSpecificFields == null)))
            {
                return;
            }

            this.AddButton.Enabled = (this.AddButton.Enabled && versionSpecificFields.Contains("CertificatesAndPermits"));
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
			this.QualificationsDataGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.QualificationsDataGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.QualificationsDataGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.QualificationsDataGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.QualificationsDataGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.QualificationsDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AddButtonCommand);

		}
		#endregion

	}
}
