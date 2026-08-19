/******************************************************************************
	FILE NAME:		CompanyGeneralPage.ascx.cs
	PURPOSE:		Implementation of CompanyGeneralPage
	
	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-06	Richard Panachida	Corrected a data dictionary error for Assigned Role
												and Unassigned Role (CSI 3368)
 
		2007-01-17	Richard Panachida	Fixed the calendar control to close if the 
												same date is selected (CSI 3972).
 
		2007-12-12	Richard Panachida	CSI 5078 - Updated the code to remove the role assignment. It has now
												been moved to the new company role assignment page.
  
		2008-04-02	Van Thompson		CSI 5644 - Added EPA and FEIN number fields
 
		2008-12-12  A. Coker          Regional settings for date and time is based on entity's site guid. In
												case entity is new and a site guid is not assigned to it, use site guid acquired
												from page's security object.
		
		2009-04-22  G. Kendall        WI# 3335 - Locked out reason text box should be disabled 
												always on initial page load
 
*******************************************************************************/

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Constants;
    using System.ComponentModel;

    /// <summary>
    /// Summary description for CompanyGeneralPage.
    /// </summary>
    public partial class CompanyGeneralPage : CompanyPageBase
    {
        #region Protected data members
        protected TextBox IDTextbox;
        protected TextBox CardNumberTextbox;
        #endregion

        public DateTimeFormatInfo DateFormat = DateTimeFormatInfo.CurrentInfo;

         protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.ViewState["DATE_FORMAT"] != null)
                {
                    this.DateFormat = this.ViewState["DATE_FORMAT"] as DateTimeFormatInfo;
                }
                else
                {
                    if (this.Security != null)
                    {
                        Guid siteGuid = (this.Company.SiteGuid == Guid.Empty) ? this.Security.SiteGuid : this.Company.SiteGuid;

                        SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                            sites => sites.Get(this.Security, siteGuid, false, false, bGetAssociatedAliases: false));

                        if (site != null)
                        {
                            DateTimeFormatInfo d = site.GetDateTimeFormatInfo();

                            if (d != null)
                            {
                                this.DateFormat = d;
                            }
                        }
                    }
                    this.ViewState["DATE_FORMAT"] = this.DateFormat;
                }

                this.DataBind();

                if (!this.Page.IsPostBack)
                {

                    bool isDesc = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
                    if (isDesc)
                    {
                        IdentifierTextbox.MaxLength = 6;
                    }
                    else
                    {
                        IdentifierTextbox.MaxLength = 60;
                    }

                    this.IdentifierTextbox.Text             = this.Company.ID;
                    this.CodeTextbox.Text                   = this.Company.Code;
                    this.AccountNumberTextbox.Text          = this.Company.AccountNumber;
                    this.NameTextbox.Text                   = this.Company.Name;
                    this.Address1Textbox.Text               = this.Company.Address1;
                    this.Address2Textbox.Text               = this.Company.Address2;
                    this.CityTextbox.Text                   = this.Company.City;
                    this.StateTextbox.Text                  = this.Company.State;
                    this.ZipTextbox.Text                    = this.Company.Zip;
                    this.CountryTextbox.Text                = this.Company.Country;
                    this.PhoneTextbox.Text                  = this.Company.Phone;
                    this.FaxTextbox.Text                    = this.Company.Fax;
                    this.Company._LastActivityDate.Format   = this.DateFormat;
                    this.LastActivityTextbox.Text           = this.Company.LastActivityDate;
                    this.Company._EffectiveDate.Format      = this.DateFormat;
                    this.EffectiveDate.Text                 = this.Company.EffectiveDate;
                    this.Company._ExpirationDate.Format     = this.DateFormat;
                    this.ExpirationDate.Text                = this.Company.ExpirationDate;
                    this.EmergencyContactTextbox.Text       = this.Company.EmergencyContact;
                    this.EmergencyPhoneTextbox.Text         = this.Company.EmergencyPhone;
                    this.LockedOutCheckBox.Checked          = this.Company.LockedOut;
                    this.CompanyIataCodeTextbox.Text        = this.Company.CompanyIataCode;
                    this.CompanyIcaoCodeTextbox.Text        = this.Company.CompanyIcaoCode;

                    CreateConsortionDropDownListEntries();
                    if (this.Company.ConsortiumType != null)
                    {
                        this.ddlConsortiumTypes.SelectedValue =
                            Enum.GetName(typeof(ConsortiumTypes), this.Company.ConsortiumType);
                    }

                    // AssignedRolesListBox
                    this.AssignedRolesListBox.Items.Clear();
                    foreach (CompanyRoleMapClass roleMap in this.Company.RoleCollection)
                    {
                        var unassignedRoleItem = new ListItem(CompanyRoleMapClass.RoleID(roleMap.Role), ((int)roleMap.Role).ToString(CultureInfo.InvariantCulture));
                        this.AssignedRolesListBox.Items.Add(unassignedRoleItem);
                    }

                    // UnassignedRolesListBox
                    this.UnassignedRolesListBox.Items.Clear();

                    var site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                     x =>
                                                     x.Get(this.Security, this.Security.SiteGuid, getMemberSites: false,
                                                        getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false)
                                                );

                    CompanyCollectionClass managerCollection = null;
                    CompanyCollectionClass ownerCollection = null;

                    if (site.EnforceSingleOwner)
                    {
                        FMChannelHelper.MakeCall<ICompanies>(
                            x =>
                            {
                                managerCollection = x.EnumerateByRole(this.Security, COMPANY_ROLE.MANAGER, false, false);
                                ownerCollection = x.EnumerateByRole(this.Security, COMPANY_ROLE.OWNER, false, false);
                            });
                    }


                    for (COMPANY_ROLE role = COMPANY_ROLE.MANAGER; role < COMPANY_ROLE.MAX_COMPANY_ROLE; role++)
                    {
                        if (site.EnforceSingleOwner)
                        {
                            if (ownerCollection != null && (managerCollection != null && ((role == COMPANY_ROLE.MANAGER && managerCollection.Count != 0)
                                                                                          || (role == COMPANY_ROLE.OWNER && ownerCollection.Count != 0))))
                            {
                                continue;
                            }
                        }

                        if (null == this.AssignedRolesListBox.Items.FindByValue(((int)role).ToString(CultureInfo.InvariantCulture))
                            && this.UnassignedRolesListBox.Items.FindByValue(((int)role).ToString(CultureInfo.InvariantCulture)) == null)
                        {
                            var assignedRoleItem = new ListItem(CompanyRoleMapClass.RoleID(role), ((int)role).ToString(CultureInfo.InvariantCulture));
                            this.UnassignedRolesListBox.Items.Add(assignedRoleItem);
                        }
                    }

                    // WI#3335 (Kendall) - locked out reason should be disabled always on initial load
                    this.LockedOutReasonTextbox.Enabled = false;

                    this.Company._LockedOutDate.Format = this.DateFormat;
                    if (this.Company.LockedOut)
                    {
                        this.LockedOutDateTextbox.Text = this.Company.LockedOutDate;
                        this.LockedOutReasonTextbox.Text = this.Company.LockedOutReason;
                    }

                    this.TaxNumberTextbox.Text = this.Company.TaxNumber;
                    this.CreditOKCheckBox.Checked = this.Company.CreditOK;
                    this.EPANumberTextBox.Text = this.Company.EPANumber;
                    this.FEINNumberTextBox.Text = this.Company.FederalID;
                    this.LoadRackDisplayTextbox.Text = this.Company.LoadRackDisplayText;
                    this.SetFieldAccessibilityForChildRecordVersion();
                }
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }
        }

        private void CreateConsortionDropDownListEntries()
        {
            var type = typeof(ConsortiumTypes);
            this.ddlConsortiumTypes.Items.Clear();
            this.ddlConsortiumTypes.Items.Add(new ListItem("", ""));
            foreach (var consortiumTypes in Enum.GetValues(type))
            {
                var memInfo = type.GetMember(consortiumTypes.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                var description = ((DescriptionAttribute)attributes[0]).Description;
                this.ddlConsortiumTypes.Items.Add(new ListItem(description, Enum.GetName(type, consortiumTypes)));
            }
            this.ddlConsortiumTypes.DataBind();
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
            this.UnassignRolesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.UnassignRolesButtonCommand);
            this.AssignRolesButton.Command += new System.Web.UI.WebControls.CommandEventHandler(this.AssignRolesButtonCommand);
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion


        public void UpdateData()
        {
            this.Company.ExpirationDate = this.ExpirationDate.Text;

            //default back to today if its blank
            if (this.ExpirationDate.Text == string.Empty)
            {
                var tmp = new CompanyClass();

                this.Company._ExpirationDate = tmp._ExpirationDate;
            }

            this.Company.ID                     = this.IdentifierTextbox.Text;
            this.Company.Code                   = this.CodeTextbox.Text;
            this.Company.AccountNumber          = this.AccountNumberTextbox.Text;
            this.Company.Name                   = this.NameTextbox.Text;
            this.Company.Address1               = this.Address1Textbox.Text;
            this.Company.Address2               = this.Address2Textbox.Text;
            this.Company.City                   = this.CityTextbox.Text;
            this.Company.State                  = this.StateTextbox.Text;
            this.Company.Zip                    = this.ZipTextbox.Text;
            this.Company.Country                = this.CountryTextbox.Text;
            this.Company.Phone                  = this.PhoneTextbox.Text;
            this.Company.Fax                    = this.FaxTextbox.Text;
            this.Company.EffectiveDate          = this.EffectiveDate.Text;
            this.Company.EmergencyContact       = this.EmergencyContactTextbox.Text;
            this.Company.EmergencyPhone         = this.EmergencyPhoneTextbox.Text;
            this.Company.LockedOut              = this.LockedOutCheckBox.Checked;
            this.Company.LockedOutReason        = this.LockedOutReasonTextbox.Text;
            this.Company.TaxNumber              = this.TaxNumberTextbox.Text;
            this.Company.CreditOK               = this.CreditOKCheckBox.Checked;
            this.Company.EPANumber              = this.EPANumberTextBox.Text;
            this.Company.FederalID              = this.FEINNumberTextBox.Text;
            this.Company.LoadRackDisplayText    = this.LoadRackDisplayTextbox.Text;
            this.Company.CompanyIataCode        = this.CompanyIataCodeTextbox.Text;
            this.Company.CompanyIcaoCode        = this.CompanyIcaoCodeTextbox.Text;

            ConsortiumTypes parsedValue;
            if (Enum.TryParse(this.ddlConsortiumTypes.SelectedValue, true, out parsedValue))
            {
                this.Company.ConsortiumType = parsedValue;
            }
            else
            {
                this.Company.ConsortiumType = null;
            }
        }

        protected void LockedOutCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!this.LockedOutCheckBox.Checked)
            {
                this.LockedOutDateTextbox.Text = string.Empty;
                this.LockedOutReasonTextbox.Text = string.Empty;
                this.LockedOutReasonTextbox.Enabled = false;
            }
            else
            {
                if ((this.Company.IdentityGuid.Equals(Guid.Empty))
                                   || (this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid)))
                {
                    this.LockedOutReasonTextbox.Enabled = true;
                }
                else if (this.VersionSpecificFields != null)
                {
                    this.LockedOutReasonTextbox.Enabled = this.VersionSpecificFields.Contains("LockedOutReason");
                }

                var site = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                     x =>
                                                     x.Get(this.Security, this.Security.SiteGuid, getMemberSites: false,
                                                        getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false)
                                                );

                this.Company.LockedOutDate = DateTimeOffset.Now.ToString("d", site.GetDateTimeFormatInfo());
                this.LockedOutDateTextbox.Text = this.Company.LockedOutDate;
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.EffectiveDate.Visible = (!this.ExpirationDate.Calendar.Visible);
            this.ExpirationDate.Visible = (!this.EffectiveDate.Calendar.Visible);
            this.EmergencyContactTextbox.Visible = (!this.ExpirationDate.Calendar.Visible && !this.EffectiveDate.Calendar.Visible);
            this.EmergencyPhoneTextbox.Visible = (!this.ExpirationDate.Calendar.Visible && !this.EffectiveDate.Calendar.Visible);
            this.TaxNumberTextbox.Visible = (!this.ExpirationDate.Calendar.Visible && !this.EffectiveDate.Calendar.Visible);
            this.LockedOutReasonTextbox.Visible = (!this.ExpirationDate.Calendar.Visible && !this.EffectiveDate.Calendar.Visible);
        }

        private void AssignRolesButtonCommand(object sender, CommandEventArgs e)
        {
            try
            {
                ((CompanyForm)this.Page).UpdateData();

                ListItem unassignedRoleItem;

                while ((unassignedRoleItem = this.UnassignedRolesListBox.SelectedItem) != null)
                {
                    this.UnassignedRolesListBox.Items.Remove(unassignedRoleItem);
                    unassignedRoleItem.Selected = false;
                    this.AssignedRolesListBox.Items.Add(unassignedRoleItem);
                }

                this.UpdateCompanyRoles();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        private void UnassignRolesButtonCommand(object sender, CommandEventArgs e)
        {
            try
            {
                ((CompanyForm)this.Page).UpdateData();

                ListItem assignedRoleItem;

                while ((assignedRoleItem = this.AssignedRolesListBox.SelectedItem) != null)
                {
                    this.AssignedRolesListBox.Items.Remove(assignedRoleItem);
                    assignedRoleItem.Selected = false;

                    var role = (COMPANY_ROLE)Convert.ToInt32(assignedRoleItem.Value);

                    this.UnassignedRolesListBox.Items.Add(assignedRoleItem);

                    if (role == COMPANY_ROLE.CARRIER)
                    {
                        this.Company.CarrierCustomerShipToCollection.Clear();
                    }

                    if (role == COMPANY_ROLE.CUSTOMER_SHIPTO)
                    {
                        this.Company.AuthorizedCarrierCollection.Clear();
                    }
                }

                this.UpdateCompanyRoles();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        private void UpdateCompanyRoles()
        {
            try
            {
                var roles = new CompanyRoleMapCollectionClass();

                foreach (ListItem assignedRoleItem in this.AssignedRolesListBox.Items)
                {
                    var role = new CompanyRoleMapClass
                    {
                        Role = (COMPANY_ROLE)Convert.ToInt32(assignedRoleItem.Value),
                        CompanyGuid = this.Company.IdentityGuid
                    };
                    roles.Add(role);
                }

                this.Company.RoleCollection = roles;

                if (this.Company.HasRole(COMPANY_ROLE.CARRIER) || this.Company.HasRole(COMPANY_ROLE.SUPPLIER))
                {
                    if (this.Company.AccessScheduleCollection.Count == 0)
                    {
                        DAY_OF_WEEK[] dayOfWeek =
                            {
                                DAY_OF_WEEK.SUNDAY, DAY_OF_WEEK.MONDAY, DAY_OF_WEEK.TUESDAY,
                                DAY_OF_WEEK.WEDNESDAY, DAY_OF_WEEK.THURSDAY, DAY_OF_WEEK.FRIDAY,
                                DAY_OF_WEEK.SATURDAY
                            };


                        for (int item = 0; item < 7; item++)
                        {
                            var schedule = new ScheduleClass
                            {
                                Type = SCHEDULE_TYPE.COMPANY_ACCESS_TYPE,
                                Day = (int)dayOfWeek[item],
                                Enabled = true,
                                EndOfDayEnabled = false
                            };

                            this.Company.AccessScheduleCollection.Add(schedule);
                        }
                    }
                }
                else
                {
                    this.Company.AccessScheduleCollection.Clear();
                }

                var equipmentCollection = new EquipmentCollectionClass();
                foreach (EquipmentClass equipment in this.Company.EquipmentCollection)
                {
                    if (equipment.CompanyRoleAssignmentConstraint != COMPANY_ROLE.MAX_COMPANY_ROLE
                        && !this.Company.HasRole(equipment.CompanyRoleAssignmentConstraint))
                    {
                        continue;
                    }

                    equipmentCollection.Add(equipment);
                }

                this.Company.EquipmentCollection = equipmentCollection;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
                return;
            }

            this.Redirect("CompanyForm.aspx");
        }

        private void SetFieldAccessibilityForChildRecordVersion()
        {
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);

            if ((this.Company.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            this.IdentifierTextbox.Enabled          = (this.IdentifierTextbox.Enabled && this.VersionSpecificFields.Contains("ID"));
            this.CodeTextbox.Enabled                = (this.CodeTextbox.Enabled && this.VersionSpecificFields.Contains("Code"));
            this.AccountNumberTextbox.Enabled       = (this.AccountNumberTextbox.Enabled && this.VersionSpecificFields.Contains("AccountNumber"));
            this.NameTextbox.Enabled                = (this.NameTextbox.Enabled && this.VersionSpecificFields.Contains("Name"));
            this.Address1Textbox.Enabled            = (this.Address1Textbox.Enabled && this.VersionSpecificFields.Contains("Address1"));
            this.Address2Textbox.Enabled            = (this.Address2Textbox.Enabled && this.VersionSpecificFields.Contains("Address2"));
            this.CityTextbox.Enabled                = (this.CityTextbox.Enabled && this.VersionSpecificFields.Contains("City"));
            this.StateTextbox.Enabled               = (this.StateTextbox.Enabled && this.VersionSpecificFields.Contains("State"));
            this.ZipTextbox.Enabled                 = (this.ZipTextbox.Enabled && this.VersionSpecificFields.Contains("Zip"));
            this.CountryTextbox.Enabled             = (this.CountryTextbox.Enabled && this.VersionSpecificFields.Contains("Country"));
            this.PhoneTextbox.Enabled               = (this.PhoneTextbox.Enabled && this.VersionSpecificFields.Contains("Phone"));
            this.FaxTextbox.Enabled                 = (this.FaxTextbox.Enabled && this.VersionSpecificFields.Contains("FAX"));
            this.AssignRolesButton.Enabled          = (this.AssignRolesButton.Enabled && this.VersionSpecificFields.Contains("CompanyRoles"));
            this.UnassignRolesButton.Enabled        = (this.UnassignRolesButton.Enabled && this.VersionSpecificFields.Contains("CompanyRoles"));
            this.CreditOKCheckBox.Enabled           = (this.CreditOKCheckBox.Enabled && this.VersionSpecificFields.Contains("CreditOK"));
            this.LastActivityTextbox.Enabled        = (this.LastActivityTextbox.Enabled && this.VersionSpecificFields.Contains("LastActivityDate"));
            this.EffectiveDate.Enabled              = (this.EffectiveDate.Enabled && this.VersionSpecificFields.Contains("EffectiveDate"));
            this.ExpirationDate.Enabled             = (this.ExpirationDate.Enabled && this.VersionSpecificFields.Contains("ExpirationDate"));
            this.EmergencyContactTextbox.Enabled    = (this.EmergencyContactTextbox.Enabled && this.VersionSpecificFields.Contains("EmergencyContact"));
            this.EmergencyPhoneTextbox.Enabled      = (this.EmergencyPhoneTextbox.Enabled && this.VersionSpecificFields.Contains("EmergencyPhone"));
            this.TaxNumberTextbox.Enabled           = (this.TaxNumberTextbox.Enabled && this.VersionSpecificFields.Contains("TaxNumber"));
            this.EPANumberTextBox.Enabled           = (this.EPANumberTextBox.Enabled && this.VersionSpecificFields.Contains("EPANumber"));
            this.FEINNumberTextBox.Enabled          = (this.FEINNumberTextBox.Enabled && this.VersionSpecificFields.Contains("FederalID"));
            this.LoadRackDisplayTextbox.Enabled     = (this.LoadRackDisplayTextbox.Enabled && this.VersionSpecificFields.Contains("LoadRackDisplayText"));
            this.LockedOutCheckBox.Enabled          = (this.LockedOutCheckBox.Enabled && this.VersionSpecificFields.Contains("LockedOut"));
            this.LockedOutDateTextbox.Enabled       = (this.LockedOutDateTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutDate"));
            this.LockedOutReasonTextbox.Enabled     = (this.LockedOutReasonTextbox.Enabled && this.VersionSpecificFields.Contains("LockedOutReason"));
            this.HiddenCheckBox.Enabled             = this.HiddenCheckBox.Enabled && this.VersionSpecificFields.Contains("HiddenDate");
            this.ddlConsortiumTypes.Enabled         = this.ddlConsortiumTypes.Enabled && this.VersionSpecificFields.Contains("ConsortiumType");
            this.CompanyIataCodeTextbox.Enabled     = this.CompanyIataCodeTextbox.Enabled && this.VersionSpecificFields.Contains("CompanyIATACode");
            this.CompanyIcaoCodeTextbox.Enabled     = this.CompanyIcaoCodeTextbox.Enabled && this.VersionSpecificFields.Contains("CompanyICAOCode");
        }


    }
}
