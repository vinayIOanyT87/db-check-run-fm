// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the CompanyForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Web.UI.WebControls;
    using System.Collections.Generic;

	using AjaxControlToolkit;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;

	using global::FMWebApp;
    using FMCore;

    /// <summary>
    ///    Code behind for CompanyForm.
    /// </summary>
    public partial class CompanyForm : FMAutoSubmitFormBase
	{
		#region Public Properties

		/// <summary>
		///    Gets or sets the company object
		/// </summary>
		public CompanyClass Company { get; set; }

        public List<string> VersionSpecificFields { get; set; }

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the company form.
		/// </summary>
		/// <param name="enable">
		///    if set to <c>true</c> [enable].
		/// </param>
		public void EnableControls(bool enable)
		{
			var companyArrayList = this.Session["CompanyArrayList"] as ArrayList;
			if (companyArrayList != null)
			{
				this.Company = companyArrayList[companyArrayList.Count - 1] as CompanyClass;
			}

			CompanyClass companyClass = this.Company;
			if (companyClass != null
			    && (this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
			        && (this.Security.SiteGuid == companyClass.SiteGuid || companyClass.SiteGuid == Guid.Empty)))
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable;
			}

			this.Cancel.Enabled = enable;

			this.tcCompanyTabs.HeaderEnabled = enable;
		}

		/// <summary>
		///    Updates the data.
		/// </summary>
		public void UpdateData()
		{
			this.CompanyGeneralPage.UpdateData();
			this.CompanyContactsPage.UpdateData();

			// ManagerPage.UpdateData();
			this.CompanyOwnerPage.UpdateData();
			this.CompanyShipperPage.UpdateData();
			this.CompanyCustomerBillToPage.UpdateData();
			this.CompanyCustomerShipToPage.UpdateData();
			this.CompanyCarrierPage.UpdateData();
			this.CompanyUserDataPage.UpdateData();
			this.CompanyNotesPage.UpdateData();
		}

		#endregion

		#region Methods

		/// <summary>
		///    Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    Handles the Init event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						CompanyClass company =
							FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
								companies => companies.Get(this.Security, this.QueryEntityGuid));

						var list = new ArrayList { company };
						this.Session["CompanyArrayList"] = list;
					}
				}
			}
			catch (FMSessionInvalidException ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		///    Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="EventArgs" /> instance containing the event data.
		/// </param>
		/// <exception cref="System.Exception">CompanyArrayList not in session</exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");

				var companyArrayList = this.Session["CompanyArrayList"] as ArrayList;
				if (companyArrayList == null)
				{
					throw new Exception("CompanyArrayList not in session");
				}                

                this.Company = companyArrayList[companyArrayList.Count - 1] as CompanyClass;

                this.VersionSpecificFields = this.Session["CompanyVersionSpecificFields"] as List<string>;

                if (!this.Page.IsPostBack)
				{
                    this.GetRecordVersioningFields();
                    if (this.Company != null)
                    {
                        if (!this.Security.HasRight(RIGHT.MODIFY_COMPANY_DATA)
                            || (this.Company.SiteGuid != Guid.Empty &&
                                this.Security.SiteGuid != this.Company.SiteGuid &&
                                (this.VersionSpecificFields == null || this.VersionSpecificFields.Count == 0))
                            )

                        {
                            this.OK.Enabled = false;
                            this.New.Enabled = false;
                        }

                        // Set the title label with a key field from the bound object appended
                        this.CompanyTitleLabel.Text = this.GetTitleLabelText(this.CompanyTitleLabel.Text, this.Company.ID);
                    }
                }

				// Set up the TabContainer based upon Roles
				this.tpAccessSchedulePage.Visible = false;
				this.tpCarrierPage.Visible = false;
				this.tpCertificatesAndPermitsPage.Visible = false;
				this.tpCustomerBillToPage.Visible = false;
				this.tpCustomerShipToPage.Visible = false;
				this.tpManagerPage.Visible = false;
				this.tpOwnerPage.Visible = false;
				this.tpShipperPage.Visible = false;
				this.tpSupplierPage.Visible = false;

				// General and Contacts are always enabled
				this.tpGeneralPage.Visible = true;
				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				this.tpContactsPage.Visible = true;
				this.tpContactsPage.HeaderText = this.GetTranslatedText("Contacts");

				foreach (CompanyRoleMapClass roleMap in this.Company.RoleCollection)
				{
					TabPanel tabPanel = null;

					switch (roleMap.Role)
					{
							// case COMPANY_ROLE.MANAGER:
							// RoleTab.TargetID="ManagerPage";
							// break;
						case COMPANY_ROLE.OWNER:
							tabPanel = this.tpOwnerPage;
							break;
						case COMPANY_ROLE.SHIPPER:
							tabPanel = this.tpShipperPage;
							break;
						case COMPANY_ROLE.CUSTOMER_BILLTO:
							tabPanel = this.tpCustomerBillToPage;
							break;
						case COMPANY_ROLE.CUSTOMER_SHIPTO:
							tabPanel = this.tpCustomerShipToPage;
							break;
						case COMPANY_ROLE.CARRIER:
							tabPanel = this.tpCarrierPage;
							break;
						case COMPANY_ROLE.SUPPLIER:
							tabPanel = this.tpSupplierPage;
							break;
					}

					if (tabPanel != null)
					{
						tabPanel.Visible = true;
						tabPanel.HeaderText = this.GetTranslatedText(CompanyRoleMapClass.RoleID(roleMap.Role));
					}
				}

				if (this.Company.HasRole(COMPANY_ROLE.CARRIER) || this.Company.HasRole(COMPANY_ROLE.SUPPLIER))
				{
					this.tpAccessSchedulePage.Visible = true;
					this.tpAccessSchedulePage.HeaderText = this.GetTranslatedText("Access Schedule");
				}

				if (this.Company.HasRole(COMPANY_ROLE.CARRIER))
				{
					this.tpCertificatesAndPermitsPage.Visible = true;
					this.tpCertificatesAndPermitsPage.HeaderText = this.GetTranslatedText("Certificates & Permits");
				}

				// These pages are always enabled
				this.tpEquipmentPage.Visible = true;
				this.tpEquipmentPage.HeaderText = this.GetTranslatedText("Equipment");

				this.tpGroupsPage.Visible = true;
				this.tpGroupsPage.HeaderText = this.GetTranslatedText("Groups");

				UserDataFieldCollectionClass userDataFieldCollection =
					FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
						x => x.EnumerateByEntityType(this.Security, ENTITY_TYPE.COMPANY, Guid.Empty, false, false));

				if (userDataFieldCollection.Count > 0)
				{
					this.tpUserDataPage.Visible = true;
					this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");
				}

				this.tpNotesPage.Visible = true;
				this.tpNotesPage.HeaderText = this.GetTranslatedText("Notes");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}


        private void GetRecordVersioningFields()
        {
            this.VersionSpecificFields = new List<string>();
            bool currentSiteOwnsRecordVersion = (this.Company.SiteGuid == this.Security.SiteGuid);

            if ((this.Company.IdentityGuid.Equals(Guid.Empty)) 
                || (currentSiteOwnsRecordVersion && this.Company.IdentityGuid.Equals(this.Company.MasterRecordGuid)))
            {
                return;
            }

            string flcMode = FieldLevelConfigClass.FLCModeGSOnly;

            if (currentSiteOwnsRecordVersion)
                flcMode = FieldLevelConfigClass.FLCModeVSandGS;

            try
            {
                this.VersionSpecificFields = FMChannelHelper.MakeCall<IEntityToSiteMaps, List<string>>(
                                                                x =>
                                                                x.GetRecordVersioningFields(this.Security, this.Company.EntityType, this.Company.MasterRecordGuid, flcMode)
                                                           );

                this.Session["CompanyVersionSpecificFields"] = this.VersionSpecificFields;
            }
            catch (Exception except)
            {
                this.ErrorHandler(except);
            }

            if (this.VersionSpecificFields == null)
            {
                this.VersionSpecificFields = new List<string>();
            }
        }


		/// <summary>
		///    Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.TransferToOriginatingForm();
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
			this.New.Command += this.NewCommand;
			this.OK.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
			ucFMMenuBar.Visible = (Page.Request.GetQueryOrFormValue("Modal") != null) ? false : true;
		}

		/// <summary>
		///    Handles the Command event of the New control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void NewCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.Company.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ICompanies>(x => x.Modify(this.Security, DATA_TYPE.CONFIG, this.Company));
				}
				else
				{
					FMChannelHelper.MakeCall<ICompanies>(x => x.Add(this.Security, this.Company));
				}

				this.Company.ID = string.Empty;
				this.Company.IdentityGuid = Guid.Empty;
                this.Company.AssignedPersonnelCollection.Clear();
				this.Company.EquipmentCollection.Clear();
				this.Company.Note = string.Empty;


				foreach (ProductMapClass authorizedProduct in this.Company.AuthorizedProductCollection)
				{
					authorizedProduct.SpecialInstructions = string.Empty;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("CompanyForm.aspx");
		}

		/// <summary>
		///    Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (this.Company.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<ICompanies>(companies => companies.Modify(this.Security, DATA_TYPE.CONFIG, this.Company));
				}
				else
				{
					FMChannelHelper.MakeCall<ICompanies>(companies => companies.Add(this.Security, this.Company));
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.TransferToOriginatingForm();
		}

		/// <summary>
		///    Transfers to originating form.
		/// </summary>
		private void TransferToOriginatingForm()
		{
			var companyArrayList = this.Session["CompanyArrayList"] as ArrayList;
			if (companyArrayList != null)
			{
				companyArrayList.RemoveAt(companyArrayList.Count - 1);

				if (companyArrayList.Count == 0)
				{
					this.Session.Remove("CompanyArrayList");
				}
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.Session["CompanySelectContextArrayList"] == null)
			{
				this.Redirect("CompaniesForm.aspx");
			}
			else
			{
				var companySelectContextArrayList = this.Session["CompanySelectContextArrayList"] as ArrayList;
				if (companySelectContextArrayList != null)
				{
					var companySelectContext =
						(CompanySelectContextClass)companySelectContextArrayList[companySelectContextArrayList.Count - 1];

					companySelectContextArrayList.RemoveAt(companySelectContextArrayList.Count - 1);

					if (companySelectContextArrayList.Count == 0)
					{
						this.Session.Remove("CompanySelectContextArrayList");
					}

					string transferString = "CompanySelectForm.aspx?";

					if (companySelectContext.Role != COMPANY_ROLE.MAX_COMPANY_ROLE)
					{
						transferString += "Role=" + companySelectContext.Role.ToString() + "&";
					}

					if (companySelectContext.MapType == typeof(COMPANY_MAP_TYPE))
					{
						transferString += "Map=" + ((COMPANY_MAP_TYPE)companySelectContext.Map).ToString() + "&";
					}

					if (companySelectContext.MapType == typeof(PRODUCT_MAP_TYPE))
					{
						transferString += "Map=" + ((PRODUCT_MAP_TYPE)companySelectContext.Map).ToString() + "&";
					}

					if (companySelectContext.SubRole != COMPANY_SUB_ROLE.NO_SUBROLE)
					{
						transferString += "SubRole=" + companySelectContext.SubRole.ToString() + "&";
					}

					transferString += "All=" + companySelectContext.All.ToString() + "&";

					transferString += "Unassigned=" + companySelectContext.Unassigned.ToString() + "&";

					if (companySelectContext.IDLink != null)
					{
						transferString += "IDLink=" + companySelectContext.IDLink + "&";
					}

					if (companySelectContext.Mode != null)
					{
						transferString += "Mode=" + companySelectContext.Mode + "&";
					}

					if (companySelectContext.SearchString != null)
					{
						transferString += "SearchString=" + companySelectContext.SearchString + "&";
					}

                    if (companySelectContext.HideHidden)
                    {
                        transferString += "HideHidden=" + companySelectContext.HideHidden + "&";
                    }

					this.Redirect(transferString);
				}
			}
		}

		#endregion
	}

	/// <summary>
	///    Page base for gaining access to company object
	/// </summary>
	public class CompanyPageBase : FMUserControlBase
	{
		#region Properties

		/// <summary>
		///    Gets the company.
		/// </summary>
		/// <value>
		///    The company.
		/// </value>
		protected CompanyClass Company => ((CompanyForm)this.Page).Company;

	    protected List<string> VersionSpecificFields => ((CompanyForm)this.Page).VersionSpecificFields;

	    #endregion
	}
}