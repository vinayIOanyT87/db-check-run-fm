// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonAdditionalDataPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonAdditionalDataPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	/// <summary>
	///    Summary description for PersonAdditionalDataPage.
	/// </summary>
	public partial class PersonAdditionalDataPage : PersonPageBase
	{
		#region Constants and Fields
		protected SiteClass CurrentSite;
		protected FMLabel Label20;
		protected FMLabel Label21;
		protected TextBox PersonIDTextbox;
		protected DropDownList PersonNameDropDownList;

		public string ExcludeGuid { get; set; }
		#endregion

		#region Public Methods and Operators
		public void UpdateData()
		{
			if (this.Person != null)
			{
				if (this.SupervisorIDTextBox.Text == this.GetTranslatedText("{Unassigned}"))
				{
					this.Person.SupervisorID = "{Unassigned}";
					this.Person.SupervisorGuid = Guid.Empty;
				}
				else
				{
					this.Person.SupervisorID = this.SupervisorIDTextBox.Text;
					Guid personMasterRecordGuid =
						FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.GetMasterRecordGuid(this.Security, this.Person.SupervisorID));

					this.Person.SupervisorGuid = personMasterRecordGuid;
				}

                if (this.ADCUserLoginTextBox.Text == this.GetTranslatedText("{Unassigned}"))
                {
                    this.Person.UserGuid = Guid.Empty;
                }
                else
                {
                    UserClass adcLoginUser =
                        FMChannelHelper.MakeCall<IUsers, UserClass>(x => x.GetByID(this.Security, this.ADCUserLoginTextBox.Text));
                    this.Person.UserGuid = adcLoginUser.IdentityGuid;

                }

				this.Person.ResponsibleOfficer = this.ResponsibleOfficerCheckBox.Checked;

                if(this.CurrentSite == null)
                    this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
                                                                     x =>
                                                                     x.Get(this.Security, this.Security.SiteGuid, false, false, true)
                                                                );
                DateTimeFormatInfo dateTimeFormatInfo = this.CurrentSite.GetDateTimeFormatInfo();

				this.Person.SupervisionDate =
					this.DateOfSupervisionTextbox.CurrentValue.ToString(dateTimeFormatInfo.ShortDatePattern);

				this.Person.AssignmentDate = this.DateAssignedTextbox.CurrentValue.ToString(dateTimeFormatInfo.ShortDatePattern);

				this.Person.Department = this.DepartmentTextbox.Text;

			    double laborRate;

			    if (double.TryParse(
			        this.LaborRate1Textbox.Text,
                    NumberStyles.Float | NumberStyles.AllowThousands,
			        this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
			        out laborRate))
			    {
			        this.Person.LaborRate1 = laborRate;
			    }
			    else
			    {
			        throw new Exception("Labor Rate 1 must be numeric");
			    }

                if (double.TryParse(
                    this.LaborRate2Textbox.Text,
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
                    out laborRate))
                {
                    this.Person.LaborRate2 = laborRate;
                }
                else
                {
                    throw new Exception("Labor Rate 2 must be numeric");
                }

                if (double.TryParse(
                    this.LaborRate3Textbox.Text,
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
                    out laborRate))
                {
                    this.Person.LaborRate3 = laborRate;
                }
                else
                {
                    throw new Exception("Labor Rate 3 must be numeric");
                }

                if (double.TryParse(
                    this.LaborRate4Textbox.Text,
                    NumberStyles.Float | NumberStyles.AllowThousands,
                    this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT),
                    out laborRate))
                {
                    this.Person.LaborRate4 = laborRate;
                }
                else
                {
                    throw new Exception("Labor Rate 4 must be numeric");
                }

				this.Person.Shift = (short)this.ShiftRadioButtonList.SelectedIndex;
			}
		}

		#endregion

		#region Methods
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);

			if (this.Person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid))
				  || (this.VersionSpecificFields == null))
			{
				return;
			}
			this.SupervisorIDTextBox.Enabled = (this.SupervisorIDTextBox.Enabled && this.VersionSpecificFields.Contains("SupervisorPersonnelGuid"));
			this.ResponsibleOfficerCheckBox.Enabled = (this.ResponsibleOfficerCheckBox.Enabled && this.VersionSpecificFields.Contains("ResponsibleOfficer"));
			this.DateOfSupervisionTextbox.Enabled = (this.DateOfSupervisionTextbox.Enabled && this.VersionSpecificFields.Contains("SupervisionDate"));
			this.DateAssignedTextbox.Enabled = (this.DateAssignedTextbox.Enabled && this.VersionSpecificFields.Contains("AssignmentDate"));
			this.DepartmentTextbox.Enabled = (this.DepartmentTextbox.Enabled && this.VersionSpecificFields.Contains("Department"));
			this.LaborRate1Textbox.Enabled = (this.LaborRate1Textbox.Enabled && this.VersionSpecificFields.Contains("LaborRate1"));
			this.LaborRate2Textbox.Enabled = (this.LaborRate2Textbox.Enabled && this.VersionSpecificFields.Contains("LaborRate2"));
			this.LaborRate3Textbox.Enabled = (this.LaborRate3Textbox.Enabled && this.VersionSpecificFields.Contains("LaborRate3"));
			this.LaborRate4Textbox.Enabled = (this.LaborRate4Textbox.Enabled && this.VersionSpecificFields.Contains("LaborRate4"));
			this.ShiftRadioButtonList.Enabled = (this.ShiftRadioButtonList.Enabled && this.VersionSpecificFields.Contains("Shift"));
            this.ADCUserLoginTextBox.Enabled = (this.ShiftRadioButtonList.Enabled && this.VersionSpecificFields.Contains("UserGuid"));
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.ExcludeGuid = this.Person.IdentityGuid.ToString();

				this.CurrentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);

				if (!this.Page.IsPostBack)
				{
					if (this.Person.SupervisorID == "{Unassigned}")
					{
						this.SupervisorIDTextBox.Text = this.GetTranslatedText("{Unassigned}");
					}
					else
					{
						this.SupervisorIDTextBox.Text = this.Person.SupervisorID;
					}

                    if(this.Person.UserGuid == Guid.Empty)
                    {
                        this.ADCUserLoginTextBox.Text = this.GetTranslatedText("{Unassigned}");
                    }
                    else
                    {
                        UserClass adcLoginUser = FMChannelHelper.MakeCall<IUsers, UserClass>(
                                                                     x =>
                                                                     x.Get(this.Security,this.Person.UserGuid)
                                                                );
                        this.ADCUserLoginTextBox.Text = adcLoginUser.ID;
                    }

					this.ResponsibleOfficerCheckBox.Checked = this.Person.ResponsibleOfficer;

					Guid siteGuid = this.Security.SiteGuid;
					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
													x =>
													x.Get(this.Security, siteGuid, getMemberSites: true,
													getSchedulesAndProcessVariables: true, bGetAssociatedAliases: true)
											);

					DateTimeFormatInfo dateTimeFormatInfo = site.GetDateTimeFormatInfo();
					this.DateOfSupervisionTextbox.FormatInfo = dateTimeFormatInfo;
					this.DateAssignedTextbox.FormatInfo = dateTimeFormatInfo;
					this.DateOfSupervisionTextbox.Text = this.Person.SupervisionDate;
					this.DateAssignedTextbox.Text = this.Person.AssignmentDate;
					this.DepartmentTextbox.Text = this.Person.Department;
                    this.LaborRate1Textbox.Text = this.Person.LaborRate1.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
                    this.LaborRate2Textbox.Text = this.Person.LaborRate2.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
                    this.LaborRate3Textbox.Text = this.Person.LaborRate3.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
                    this.LaborRate4Textbox.Text = this.Person.LaborRate4.ToString(this.CurrentSite.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
					this.ShiftRadioButtonList.SelectedIndex = this.Person.Shift;
					this.SetFieldAccessibilityForChildRecordVersion();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}