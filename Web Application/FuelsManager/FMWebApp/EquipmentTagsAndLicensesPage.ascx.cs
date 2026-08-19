/******************************************************************************
	FILE NAME:		EquipmentTagsAndLicensesPage.ascx.cs
	PURPOSE:		Implementation of EquipmentTagsAndLicensesPage

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
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	public partial class EquipmentTagsAndLicensesPage : QualificationPageBase
	{

		protected EquipmentClass Equipment => ((EquipmentForm)this.Page).Equipment;

	    protected override QUALIFICATION_TYPE PageQualificationType => QUALIFICATION_TYPE.EQUIPMENT_TAG_AND_LICENSE;

	    protected override QUALIFICATION_MAP_TYPE PageQualificationMapType => QUALIFICATION_MAP_TYPE.EQUIPMENT_TAG_AND_LICENSE_TO_EQUIPMENT;

	    protected override DataGrid MapGrid => this.QualificationsDataGrid;

        protected override QualificationMapCollectionClass PageMaps
		{
			get
			{
				QualificationMapCollectionClass maps = this.Equipment.TagAndLicenseCollection;
				return maps;
			}
			set
			{
				this.Equipment.TagAndLicenseCollection = value;
			}
		}


		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{

				/*System.Globalization.DateTimeFormatInfo d = ViewState["DATE_FORMAT"] as System.Globalization.DateTimeFormatInfo;
				if (d != null)
				{
					_dateFormat = d;
				}*/
				if (this.Page.IsPostBack == false)
				{
					/*if (d == null && this.Security != null)
					{
						Guid siteGuid = this.Security.SiteGuid;
						SitesClass sites = new SitesClass();
						SiteClass site = sites.Get(this.Security, siteGuid);
						if (site != null)
						{
							d = site.GetDateTimeFormatInfo();
						}
					}
					if (d != null)
					{
						_dateFormat = d;
					}
					ViewState["DATE_FORMAT"] = _dateFormat;*/

					this.UpdateQualificationsView();

					if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) == false)
					{
						this.AddButton.Enabled = false;
					}
                    this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method overrides and implements the base class enable controls.
		/// </summary>
		/// <param name="enable"></param>
		override protected void EnableControls(bool enable)
		{
			if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA) == false)
			{
				this.AddButton.Enabled = enable;
			}

			// Call the main form to disable buttons and tabs.
			EquipmentForm equipmentForm = (EquipmentForm)this.Page;
			equipmentForm.EnableControls(enable);
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
			this.QualificationsDataGrid.EditCommand += new DataGridCommandEventHandler(this.QualificationsDataGridEditCommand);
			this.QualificationsDataGrid.PageIndexChanged += new DataGridPageChangedEventHandler(this.QualificationsDataGridPageIndexChanged);
			this.QualificationsDataGrid.CancelCommand += new DataGridCommandEventHandler(this.QualificationsDataGridCancelCommand);
			this.QualificationsDataGrid.UpdateCommand += new DataGridCommandEventHandler(this.QualificationsDataGridUpdateCommand);
			this.QualificationsDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.QualificationsDataGridDeleteCommand);
			this.QualificationsDataGrid.ItemDataBound += new DataGridItemEventHandler(this.QualificationsDataGridItemDataBound);
			this.AddButton.Command += new CommandEventHandler(this.AddButtonCommand);

		}
		#endregion


        
        protected override void QualificationsDataGridItemDataBound(object sender, DataGridItemEventArgs e)
        {
            base.QualificationsDataGridItemDataBound(sender, e);
            
            //Set the availability of the Grid editing buttons for child record versions. 
            bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((EquipmentForm)this.Page).VersionSpecificFields;
            if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
                || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
            {
                return;
            }
            LinkButton editButton = (LinkButton)e.Item.FindControl("EditButton");
            LinkButton deleteButton = (LinkButton)e.Item.FindControl("DeleteButton");
            if ((editButton != null) && (deleteButton != null))
            {
                if ((versionSpecificFields == null) || !versionSpecificFields.Contains("Tags and Licences"))
                {
                    editButton.Enabled = false;
                    deleteButton.Enabled = false;
                }
            }
        }
        

        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
            System.Collections.Generic.List<string> versionSpecificFields = ((EquipmentForm)this.Page).VersionSpecificFields;
            if (this.Equipment.IdentityGuid.Equals(Guid.Empty)
                || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid))
                || (versionSpecificFields == null))
            {
                return;
            }
            this.AddButton.Enabled = (this.AddButton.Enabled && versionSpecificFields.Contains("Tags and Licences"));
        }

	}
}
