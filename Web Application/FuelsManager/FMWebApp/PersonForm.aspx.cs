// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the PersonForm type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Sockets;
    using System.Web;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FMControls;

    using FMEnterpriseManagementBusinessObjects.BusinessInterfaces;
    using FMEnterpriseManagementBusinessObjects.ChannelFactories;

    using FMSynchronizationCommon;

    /// <summary>
    ///    Summary description for PersonForm.
    /// </summary>
    public partial class PersonForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields
		public PersonClass Person;
		public List<string> VersionSpecificFields;

        private bool remote;

        private bool isEnterprise;

		#endregion

		#region Public Methods and Operators
		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the person form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			var personArrayList = this.Session["PersonArrayList"] as ArrayList;

			if (personArrayList != null)
			{
                var personTuple = personArrayList[personArrayList.Count - 1] as Tuple<PersonClass, bool>;
                if (personTuple != null)
                {
                    this.Person = personTuple.Item1;
                    this.remote = personTuple.Item2;
                }
                else
                {
                    this.Person = personArrayList[personArrayList.Count - 1] as PersonClass;
                    this.remote = false;
                }

			    var person = this.Person;
                if (person != null && (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
				                       && (this.Security.SiteGuid == person.SiteGuid || person.SiteGuid == Guid.Empty)))
				{
					this.OK.Enabled = enable;
					this.New.Enabled = enable && !this.IsFromDispatch;
				}

				this.Cancel.Enabled = enable;

				this.CardOutButton.Enabled = enable && this.IsPermittedToCardInOrOut && person != null && person.CardedIn;
				this.CardInButton.Enabled = enable && this.IsPermittedToCardInOrOut && person != null && !person.CardedIn;
			}

			this.tcPersonTabs.HeaderEnabled = enable;
		}

		public void UpdateData()
		{
			this.PersonGeneralPage.UpdateData();
			this.PersonDriverPage.UpdateData();
			this.PersonLicensesPage.UpdateData();
			this.PersonLoadRackPage.UpdateData();
			this.PersonAdditionalDataPage.UpdateData();
			this.PersonUserDataPage.UpdateData();
		}
        #endregion

        #region Methods
        private void GetRecordVersioningFields()
        {
            this.VersionSpecificFields = new List<string>();
            bool currentSiteOwnsRecordVersion = (this.Person.SiteGuid == this.Security.SiteGuid);
            if ((this.Person.IdentityGuid.Equals(Guid.Empty))
                 || (currentSiteOwnsRecordVersion && this.Person.IdentityGuid.Equals(this.Person.MasterRecordGuid)))
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
                                                                x.GetRecordVersioningFields(this.Security, this.Person.EntityType, this.Person.MasterRecordGuid, flcMode)
                                                          );

                this.Session["PersonnelVersionSpecificFields"] = this.VersionSpecificFields;
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

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						this.LoadPerson(this.QueryEntityGuid);
						this.New.Enabled = false;
						this.CardInButton.Enabled = false;
						this.CardOutButton.Enabled = false;
					}
					else if (this.IsFromDispatch)
					{
						this.LoadPerson(this.DispatchEntityGuid);
						this.New.Enabled = false;
						this.CardInButton.Enabled = false;
						this.CardOutButton.Enabled = false;
					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.Session.Remove("Status");
                this.Session.Remove("UserSelectSearchString");

			    this.isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMultipleSiteKey());

				var personArrayList = this.Session["PersonArrayList"] as ArrayList;
				if (personArrayList == null && this.IsFromDispatch == false && this.IsFromQueryWriter == false)
				{
					return;
				}

				if (personArrayList != null && personArrayList.Count > 0)
				{
                    var equipmentTuple = personArrayList[personArrayList.Count - 1] as Tuple<PersonClass, bool>;
                    if (equipmentTuple != null)
                    {
                        this.Person = equipmentTuple.Item1;
                        this.remote = equipmentTuple.Item2;
                    }
                    else
                    {
                        this.Person = personArrayList[personArrayList.Count - 1] as PersonClass;
                        this.remote = false;
                    }
                }

                this.VersionSpecificFields = this.Session["PersonnelVersionSpecificFields"] as List<string>;

                if (!this.Page.IsPostBack)
                {
                    this.GetRecordVersioningFields();
                    if (this.Person != null)
                    {
                        if (!this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA)
                                || (this.Person.SiteGuid != Guid.Empty
                                    && this.Security.SiteGuid != this.Person.SiteGuid &&
                                    (this.VersionSpecificFields == null || this.VersionSpecificFields.Count == 0))
                           )
                        {
                            this.OK.Enabled = false;
                            this.New.Enabled = false;
                        }
                        //Set the title label with a key field from the bound object appended
                        this.PersonnelTitleLabel.Text = this.GetTitleLabelText(this.PersonnelTitleLabel.Text, this.Person.ID);
                    }

                    this.CardInButton.Enabled = this.IsPermittedToCardInOrOut && this.Person != null && !this.Person.CardedIn && !this.remote;
                    this.CardOutButton.Enabled = this.IsPermittedToCardInOrOut && this.Person != null && this.Person.CardedIn && !this.remote;

                    if (this.remote)
                    {
                        string applyTranslated = "Apply";
                        if (this.Page.Session["UseDataDictionary"] == null || (bool)this.Page.Session["UseDataDictionary"])
                        {
                            if (this.Security.SiteGuid == Guid.Empty)
                            {
                                var siteGuid = this.Security.SiteGuid;

                                applyTranslated = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                                             x =>
                                                                             x.Get(siteGuid, applyTranslated)
                                                                        );
                            }
                        }

                        if (this.Security.HasRight(RIGHT.MODIFY_PERSONNEL_DATA))
                        {
                            this.OK.Enabled = true;
                        }

                        this.OK.Text = applyTranslated;
                        this.New.Enabled = false;
                        this.New.Visible = false;
                    }
				}

				// Set up the TabStrip based upon Roles
				this.tpDriverPage.Visible = false;

				this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");

				if (this.Person != null)
				{
					foreach (PersonRoleMapClass roleMap in this.Person.RoleCollection)
					{
						switch (roleMap.Role)
						{
							case PERSON_ROLE.LOADER_ROLE:
                     case PERSON_ROLE.OFFLOADER_ROLE:
								this.tpDriverPage.Visible = true;
   							break;
						}
					}
				}

				this.tpLoadRackPage.HeaderText = this.GetTranslatedText("Load Rack");
				this.tpAccessSchedulePage.HeaderText = this.GetTranslatedText("Access Schedule");
				this.tpQualificationsPage.HeaderText = this.GetTranslatedText("Qualifications");
				this.tpTrainingPage.HeaderText = this.GetTranslatedText("Training");
				this.tpLicensesPage.HeaderText = this.GetTranslatedText("Licenses");
				this.tpAdditionalDataPage.HeaderText = this.GetTranslatedText("Additional Data");
				this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");

				if (this.IsFromDispatch && this.IsPostBack == false)
				{
					this.tcPersonTabs.ActiveTab = this.tpQualificationsPage;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

        /// <summary>
        /// In order to use the card in/out buttons, you can't be coming from dispatch, you must have rights to modify load rack data,
        /// and you must be editing an existing personnel record.
        /// </summary>
	    private bool IsPermittedToCardInOrOut => this.Person != null && this.Person.IdentityGuid != Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) && !this.IsFromDispatch;

        private void CancelCommand(object sender, CommandEventArgs e)
		{
			this.TransferToOriginatingForm();
		}

		private void CardInButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
													x =>
													x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
											);
				this.Person.CardedIn = true;
				this.Person._LastActivityDate.Value = TimeConverter.Now(site);
				FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Person)
																);

                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                                                    x =>
                                                    x.Add(this.Security, this.Person.CardInWebAppEvent(this.Security.UserID))
                                            );
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("PersonForm.aspx");
		}

		private void CardOutButtonCommand(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
													x =>
													x.GetByMemberAndProcessVariables(this.Security, this.Security.SiteGuid, false, false)
											);

				this.Person.CardedIn = false;
				this.Person._LastActivityDate.Value = TimeConverter.Now(site);
				FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Person)
																);
                
                FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
                                                    x =>
                                                    {
                                                        x.Add(this.Security, this.Person.CardOutWebAppEvent(this.Security.UserID));
                                                    }
                                                    
                                            );

            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("PersonForm.aspx");
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.OK.Command				+= this.OkCommand;
			this.CardOutButton.Command	+= this.CardOutButtonCommand;
			this.CardInButton.Command	+= this.CardInButtonCommand;
			this.New.Command			+= this.NewCommand;
			this.Cancel.Command			+= this.CancelCommand;
		}

		private void LoadPerson(Guid entityGuid)
		{
			// If evoked from PersonGeneralPage Redirect already loaded
			if (this.Session["PersonArrayList"] != null)
			{
				return;
			}

			PersonClass person = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, entityGuid)
																);

			var list = new ArrayList { person };
			this.Session["PersonArrayList"] = list;
		}

		private void NewCommand(object sender, CommandEventArgs e)
		{
			try
			{
				if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
				{
					return;
				}

				this.UpdateData();

				if (!this.Person.IdentityGuid.IsEmpty())
				{
					FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.CONFIG, this.Person)
																);
				}
				else
				{
					FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.Add(this.Security, this.Person)
																);
				}

				this.Person.ID = string.Empty;
				this.Person.IdentityGuid = Guid.Empty;
				this.Person.MasterRecordGuid = Guid.Empty;
				this.Person.FirstName = string.Empty;
				this.Person.MiddleName = string.Empty;
				this.Person.LastName = string.Empty;
				this.Person.CardNumber = string.Empty;
				this.Person.PINNumber = string.Empty;
				this.Person.OnFileSignature = null;

				this.Person.ShortCardNumber = FMChannelHelper.MakeCall<IPersonnel, string>(
																	 x =>
																	 x.GetNextShortCardNumber(this.Security)
																);
				Guid twicGuid = FMChannelHelper.MakeCall<IQualifications, Guid>(
																	 x =>
																	 x.GetIdentityGuid(this.Security, QUALIFICATION_TYPE.PERSON_LICENSE, "TWIC")
																);
				int qualificationIndex = 0;

				foreach (QualificationMapClass licensemap in this.Person.LicenseCollection)
				{
					// when found update this information with the newly read information
					if (licensemap.AssignedGuid == twicGuid)
					{
						this.Person.LicenseCollection.RemoveAt(qualificationIndex);
						break;
					}

					qualificationIndex++;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Redirect("PersonForm.aspx");
		}

		private void OkCommand(object sender, CommandEventArgs e)
		{
			try
			{
                if (this.Session["Status"] != null && (string)this.Session["Status"] == "Error")
                {
                    return;
                }

                this.UpdateData();

                if (this.remote)
                {
                    // this is for requesting the assignment of enterprise equipment down to this site.
                    // Synchronization would then pull the newly assigned equipment down to the terminal.
                    var masterRecordGuid = this.Person.MasterRecordGuid;

                    this.SelfAssignFromEnterprise(masterRecordGuid);
                }
                else
                {
                    var button = sender as FMButton;
                    if (button?.ID == "EquipYes")
                    {
                        // Should only get here if we had an local/enterprise equipment ID collision and we
                        // chose to self-assign the enterprise equipment.
                        var priorPerson = this.GetPriorEnterprisePerson();

                        // We're reasonably sure that we'll have a prior equipment here; else we shouldn't have gotten here.
                        this.SelfAssignFromEnterprise(priorPerson);
                    }
                    else
                    {
                        Guid personGuid = this.Person.IdentityGuid;
                        if (this.Person.IdentityGuid != Guid.Empty)
                        {
                            FMChannelHelper.MakeCall<IPersonnel>(x => x.Modify(this.Security, DATA_TYPE.CONFIG, this.Person));
                        }
                        else
                        {
                            // For new equipment, we have to check against enterprise.
                            var priorPerson = this.GetPriorEnterprisePerson();
                            if (priorPerson != Guid.Empty)
                            {
                                this.Page.ClientScript.RegisterStartupScript(
                                    this.GetType(),
                                    "Person Already Exists",
                                    "<script type='text/javascript'>\r\n" + "<!--\r\n" + "if(window.confirm(\""
                                    + HttpUtility.JavaScriptStringEncode($"Person {this.Person.ID} already exists at the Enterprise.") + "\\r\\n"
                                    + HttpUtility.JavaScriptStringEncode("") + "\\r\\n"
                                    + HttpUtility.JavaScriptStringEncode(
                                        this.GetTranslatedText("Click OK to assign this person down from Enterprise.")) + "\\r\\n"
                                    + HttpUtility.JavaScriptStringEncode(
                                        this.GetTranslatedText("Press Cancel to change the local person.")) + "\"))\r\n"
                                    + "   document.getElementById('PersonYes').click();\r\n" + "else\r\n"
                                    + "   document.getElementById('PersonNo').click();\r\n" + "\r\n-->\r\n</script>");

                                return;
                            }

                            this.Person.IdentityGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(x => x.Add(this.Security, this.Person));
                            // re-get the person
                            this.Person = FMChannelHelper.MakeCall<IPersonnel,PersonClass>(
                                    x => x.Get(this.Security, this.Person.IdentityGuid));
                            this.Session["PersonAdd"] = this.Person.IdentityGuid;

                            this.ShiftOwnershipToEnterpriseSite(this.Person);
                        }

                        try
                        {
                            if (UsingLoadRack)
                            {
                                ILoadRackManager loadRackManager = this.GetLoadRackManager();
                                if (personGuid != Guid.Empty)
                                {
                                    loadRackManager.Modify(this.Security, typeof(PersonClass), this.Person.IdentityGuid);
                                }
                                else
                                {
                                    loadRackManager.Add(this.Security, typeof(PersonClass), this.Person.IdentityGuid);
                                }
                            }
                        }
                        catch (SocketException socketExcept)
                        {
                            if (socketExcept.ErrorCode != 10061)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.TransferToOriginatingForm();
		}

        private void ShiftOwnershipToEnterpriseSite(PersonClass person)
        {
            // Automatic ownership shift should only happen at terminal.
            // At enterprise, user is able to create personnel at site group directly
            if (this.isEnterprise)
            {
                return;
            }

            // We only upwardly change ownership if the enterprise relationship is specified
            SiteClass site =
                FMChannelHelper.MakeCall<ISites, SiteClass>(
                    x =>
                        x.Get(
                            this.Security,
                            this.Security.SiteGuid,
                            getMemberSites: false,
                            getSchedulesAndProcessVariables: false,
                            bGetAssociatedAliases: true));

            if (string.IsNullOrEmpty(site.EnterpriseSite))
            {
                // no enterprise site; nothing to do
                return;
            }

            // Get site guid for the enterprise site
            Guid enterpriseSiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(
                x => x.GetIdentityGuid(this.Security, site.EnterpriseSite));

            if (enterpriseSiteGuid == Guid.Empty)
            {
                return;
            }

            // upwardly change the ownership
			// explicit set of person.MasterSiteGuid is required here to ensure proper encryption of the PIN
            person.SiteGuid = enterpriseSiteGuid;
			person.MasterSiteGuid = enterpriseSiteGuid;

            FMChannelHelper.MakeCall<IPersonnel>(personnel => personnel.Modify(this.Security, DATA_TYPE.CONFIG, person));

            // Now entity assign back to self
            var map = new EntityToSiteMapClass
            {
                ID = person.ID,
                IdentityGuid = person.IdentityGuid,
                SiteGuid = this.Security.SiteGuid,
                TypeID = ENTITY_TYPE.PERSONNEL,
                AssignedFromSiteGuid = enterpriseSiteGuid
            };

            FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.Security, map, typeof(IPersonnel).GUID));

            this.ForceAdHocSync();
        }

        private void SelfAssignFromEnterprise(Guid masterRecordGuid)
        {
            // This function should only be executed at terminal
            if (this.isEnterprise)
            {
                return;
            }

            SiteClass site =
                FMChannelHelper.MakeCall<ISites, SiteClass>(
                    x =>
                        x.Get(
                            this.Security,
                            this.Security.SiteGuid,
                            getMemberSites: false,
                            getSchedulesAndProcessVariables: false,
                            bGetAssociatedAliases: true));

            if (!string.IsNullOrEmpty(site.EnterpriseUserId))
            {
                SecurityLoginRequest sr = new SecurityLoginRequest
                {
                    UserID = site.EnterpriseUserId,
                    Password = site.EnterprisePassword,
                    SiteID = site.EnterpriseSite
                };

                SecurityLoginResponse enterpriseSecurityResponse =
                    EnterpriseManagementChannelHelper
                        .MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(x => x.Login(sr));

                EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                    x =>
                        x.RequestEnterprisePersonAssignment(
                            enterpriseSecurityResponse.Security,
                            this.Security.SiteGuid,
                            masterRecordGuid));

                EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                    x => x.Logout(enterpriseSecurityResponse.Security));

                this.ForceAdHocSync();
            }
        }

        public Guid GetPriorEnterprisePerson()
        {
            // At enterprise, we already have access to the sitegroup records.  Skip this check
            if (this.isEnterprise)
            {
                return Guid.Empty;
            }

            // First determine if this site has a parent enterprise management relation
            SiteClass site =
                FMChannelHelper.MakeCall<ISites, SiteClass>(
                    x =>
                        x.Get(
                            this.Security,
                            this.Security.SiteGuid,
                            getMemberSites: false,
                            getSchedulesAndProcessVariables: false,
                            bGetAssociatedAliases: true));

            if (string.IsNullOrEmpty(site.EnterpriseUserId)
                || string.IsNullOrEmpty(site.EnterprisePassword)
                || string.IsNullOrEmpty(site.EnterpriseSite))
            {
                // We don't have a parent enterprise management relation; this equipment by definition does not exist
                // at enterprise
                return Guid.Empty;
            }

            SecurityLoginRequest sr = new SecurityLoginRequest
            {
                UserID = site.EnterpriseUserId,
                Password = site.EnterprisePassword,
                SiteID = site.EnterpriseSite
            };

            SecurityLoginResponse enterpriseSecurityResponse =
                EnterpriseManagementChannelHelper
                    .MakeCall<IClientEnterpriseManagementService, SecurityLoginResponse>(x => x.Login(sr));

            Guid enterpriseMasterGuid = EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService, Guid>(
                x => x.GetPersonMasterGuid(enterpriseSecurityResponse.Security, this.Person.ID));

            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                x => x.Logout(enterpriseSecurityResponse.Security));

            return enterpriseMasterGuid;
        }

        private void TransferToOriginatingForm()
		{
			var personArrayList = this.Session["PersonArrayList"] as ArrayList;

			if (personArrayList != null && personArrayList.Count > 0)
			{
				personArrayList.RemoveAt(personArrayList.Count - 1);

				if (personArrayList.Count == 0)
				{
					this.Session.Remove("PersonArrayList");
				}
			}

			if (this.IsFromQueryWriter)
			{
				this.Redirect("..\\QueryWriterWebApp\\QueryResultsForm.aspx?Mode=Returning");
			}
			else if (this.IsFromClientDispatch) //If this window was opened from Client Dispatch, it should be closed once the current item has been saved or canceled
			{
				string redirectString;
				string javascriptString = "<script language=\"JavaScript\">{0}</script>";
				HttpBrowserCapabilities myBrowserCaps = this.Request.Browser;
				var version = double.Parse(myBrowserCaps.Version, CultureInfo.InvariantCulture);
				if (version >= 7)
				{
					redirectString = "window.open('', '_self', '');";
				}
				else if (version >= 6)
				{
					redirectString = "window.opener = null;";
				}
				else
				{
					redirectString = "window.opener = '';";
				}
				redirectString += "window.close();";
				javascriptString = string.Format(javascriptString, redirectString);
				this.Response.Write(javascriptString);
			}
			else if (this.IsFromDispatch)
			{
				// Return to dispatching view
				this.Redirect("../DispatchWebApp/DispatchingView.aspx");
			}
			else if (this.Session["PersonSelectContextArrayList"] == null)
			{
				this.Redirect("PersonnelForm.aspx");
			}
			else
			{
				var personSelectContextArrayList = this.Session["PersonSelectContextArrayList"] as ArrayList;

				if (personSelectContextArrayList != null)
				{
					var personSelectContext =
						personSelectContextArrayList[personSelectContextArrayList.Count - 1] as PersonSelectContextClass;
					personSelectContextArrayList.RemoveAt(personSelectContextArrayList.Count - 1);

					if (personSelectContextArrayList.Count == 0)
					{
						this.Session.Remove("PersonSelectContextArrayList");
					}

					string transferString = "PersonSelectForm.aspx?";

					if (personSelectContext != null && personSelectContext.Role != PERSON_ROLE.MAX_PERSON_ROLE)
					{
						transferString += "Role=" + personSelectContext.Role + "&";
					}

					transferString += "Unassigned=" + (personSelectContext != null && personSelectContext.Unassigned) + "&";

					if (personSelectContext?.IDCarrierLink != null)
					{
						transferString += "IDCarrierLink=" + personSelectContext.IDCarrierLink + "&";
					}

					if (personSelectContext?.Mode != null)
					{
						transferString += "Mode=" + personSelectContext.Mode + "&";
					}

					if (personSelectContext?.SearchString != null)
					{
						transferString += "SearchString=" + personSelectContext.SearchString + "&";
					}

                    if (personSelectContext != null && personSelectContext.ExcludeGuid != Guid.Empty)
                    {
                        transferString += "ExcludeGuid=" + personSelectContext.ExcludeGuid + "&";
                    }

                    if (personSelectContext != null && personSelectContext.HideHidden)
                    {
                        transferString += "HideHidden=" + personSelectContext.HideHidden + "&";
                    }

					this.Redirect(transferString);
				}
			}
		}

        private void ForceAdHocSync()
        {
            // Ad-hoc synchronization should not be triggered at the enterprise
            if (this.isEnterprise)
            {
                return;
            }

            // Determine which SiteId to use when performing the synchronization request.  
            // This is all dependent on the determine request type.
            var selectedSyncSite =
                FMChannelHelper.MakeCall<ISyncControllerProcessor, SyncSelectedSiteDO>(
                    syncControllerProcessorChannel =>
                    syncControllerProcessorChannel.GetSynchronizationSiteId(
                        this.Security, SYNCREQUESTTYPE.MANUAL));

            FMSyncServiceChannelHelper tmpHelper = new FMSyncServiceChannelHelper();
            var tmpConfig = tmpHelper.CreateChannelFactoryConfigInfo<ISynchronizationServices>();

            FMChannelFactory<ISynchronizationServices> syncServiceFactory =
                new FMChannelFactory<ISynchronizationServices>(tmpConfig);

            Func<ISynchronizationServices, SecurityClass, SyncSelectedSiteDO, byte[], SYNCREQUESTTYPE, bool> callback = (proxy,
                                                                                                             security,
                                                                                                             selectedSite,
                                                                                                             clientCert,
                                                                                                             requestType)
                                                                                                            => proxy.ManuallyInitiate(security, selectedSite, clientCert, requestType);

            // ReSharper disable once RedundantTypeArgumentsOfMethod
            FMChannelHelper.MakeCall<ISynchronizationServices, bool>(syncServiceFactory, channelProxy => callback(channelProxy, this.Security, selectedSyncSite, this.Request.ClientCertificate.Certificate, SYNCREQUESTTYPE.MANUAL));

        }
        #endregion
    }

    public class PersonPageBase : FMUserControlBase
	{
		#region Properties

		protected PersonClass Person => ((PersonForm)this.Page).Person;

	    protected List<string> VersionSpecificFields => ((PersonForm)this.Page).VersionSpecificFields;

	    #endregion
	}
}