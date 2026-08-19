// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EquipmentForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the EquipmentForm.aspx.cs type.
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
	using FMBusinessObjects.Exceptions;
	using FMBusinessObjects.Interfaces;
	using FMBusinessObjects.ServiceRequests;

	using FMControls;

	using FMCore;

	using FMEnterpriseManagementBusinessObjects.BusinessInterfaces;
	using FMEnterpriseManagementBusinessObjects.ChannelFactories;

	using FMSynchronizationCommon;

    /// <summary>
	///    Summary description for EquipmentForm.
	/// </summary>
	public partial class EquipmentForm : FMAutoSubmitFormBase
	{
		#region Constants and Fields

		public EquipmentClass Equipment;
		public const string EquipmentQcStatus = "EquipmentQCStatus";
		public const string EquipmentAdditionalTabVisible = "EquipmentAdditionalTabVisible";

	    private bool remote;

        private bool isEnterprise;

		public List<string> VersionSpecificFields;

		#endregion

		#region Public Properties

		public int ActiveTabIndex => this.tcEquipment.ActiveTabIndex;

	    #endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will either enable or disable controls.  It is called by
		///    the individual tabs associated to the equipment form.
		/// </summary>
		/// <param name="enable"></param>
		public void EnableControls(bool enable)
		{
			var equipmentArrayList = this.Session["EquipmentArrayList"] as ArrayList;
		    var equipmentTuple = equipmentArrayList?[equipmentArrayList.Count - 1] as Tuple<EquipmentClass, bool>;
		    if (equipmentTuple != null)
		    {
		        this.Equipment = equipmentTuple.Item1;
		        this.remote = equipmentTuple.Item2;
		    }
		    else
		    {
                this.Equipment = equipmentArrayList?[equipmentArrayList.Count - 1] as EquipmentClass;
		        this.remote = false;
		    }

            var equipmentClass = this.Equipment;
		    if (equipmentClass != null && (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
			                               && (this.Security.SiteGuid == equipmentClass.SiteGuid || equipmentClass.SiteGuid == Guid.Empty)))
			{
				this.OK.Enabled = enable;
				this.New.Enabled = enable && (this.IsFromDispatch == false);
			}

			this.Cancel.Enabled = enable;

			this.tcEquipment.HeaderEnabled = enable;
		}

		public void UpdateData()
		{
			if (this.Equipment != null)
			{
				if (!FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey()) &&
                    this.Equipment.Type == EQUIPMENT_TYPE.AIRCRAFT_TYPE)
				{
					this.EquipmentAirplaneGeneralPage.UpdateData();
				}
				else
				{
					this.EquipmentGeneralPage.UpdateData();
				}
				this.EquipmentQCStatusPage.UpdateData();
				this.EquipmentAdditionalDataPage.UpdateData();
				this.EquipmentUserDataPage.UpdateData();
				this.EquipmentPipelinePage.UpdateData();
				this.EquipmentRailcarPage.UpdateData();
				this.EquipmentShipPage.UpdateData();
				this.EquipmentTractorPage.UpdateData();
				this.EquipmentTrailerPage.UpdateData();
				this.EquipmentTankerPage.UpdateData();
				this.EquipmentMeterPage.UpdateData();
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

		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (this.IsPostBack == false)
				{
					if (this.IsFromQueryWriter)
					{
						this.Session["EquipmentArrayList"] = null;
						this.LoadEquipment(this.QueryEntityGuid);
					}
					else if (this.IsFromDispatch && this.Session["FromDispatch"] as string == "Init")
					{
						this.LoadEquipment(this.DispatchEntityGuid);
						this.New.Enabled = false;
					}
					else if (this.Session["EQUIPMENT_SESSION_KEY"] is EquipmentClass)
					{
						var list = new ArrayList { this.Session["EQUIPMENT_SESSION_KEY"] };
						this.Session["EquipmentArrayList"] = list;
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
		///    The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
                bool isDesc = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
			    this.isEnterprise = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsMultipleSiteKey());
				this.Session.Remove("Status");

				var equipmentArrayList = this.Session["EquipmentArrayList"] as ArrayList;
				if (equipmentArrayList == null && this.IsFromDispatch == false && this.IsFromQueryWriter == false)
				{
					return;
				}

				if (this.Session["EQUIPMENT_SESSION_KEY"] == null)
				{
					if (equipmentArrayList != null && equipmentArrayList.Count > 0)
					{
					    var equipmentTuple = equipmentArrayList[equipmentArrayList.Count - 1] as Tuple<EquipmentClass, bool>;
					    if (equipmentTuple != null)
					    {
					        this.Equipment = equipmentTuple.Item1;
					        this.remote = equipmentTuple.Item2;
					    }
					    else
					    {
                            this.Equipment = equipmentArrayList[equipmentArrayList.Count - 1] as EquipmentClass;
					        this.remote = false;
					    }
					}
				}
				else
				{
					this.Equipment = this.Session["EQUIPMENT_SESSION_KEY"] as EquipmentClass;
					this.Session.Remove("EQUIPMENT_SESSION_KEY");
				}

			    var equipment = this.Equipment;
			    if (equipment != null && equipment.IdentityGuid == Guid.Empty)
				{
                    // New equipment starts off in service
                    equipment.InServiceFlag = true;
				}

                this.VersionSpecificFields = this.Session["EquipmentVersionSpecificFields"] as List<string>;

                if (!this.Page.IsPostBack)
                {
                    this.GetRecordVersioningFields();
                    if (this.Equipment != null)
                    {
                        if (!this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA)
                            || (this.Equipment.SiteGuid != Guid.Empty &&
                                this.Security.SiteGuid != this.Equipment.SiteGuid &&
                                (this.VersionSpecificFields == null || this.VersionSpecificFields.Count == 0))
                           )
                        {
                            this.OK.Enabled = false;
                            this.New.Enabled = false;
                        }
                        //Set the title label with a key field from the bound object appended
                        this.EquipmentTitleLabel.Text = this.GetTitleLabelText(this.EquipmentTitleLabel.Text, this.Equipment.ID);
                    }


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

                        if (this.Security.HasRight(RIGHT.MODIFY_EQUIPMENT_DATA))
                        {
                            this.OK.Enabled = true;
                        }

                        this.OK.Text = applyTranslated;
                        this.New.Enabled = false;
                        this.New.Visible = false;
                    }

					// Set up which tabs are visible and the proper text on the tab header
					// Hide these now, they will be made visible below if user has permission
				    var equipment1 = this.Equipment;
				    if (equipment1 != null && (!isDesc && equipment1.Type == EQUIPMENT_TYPE.AIRCRAFT_TYPE))
					{
                        this.tpAirplaneGeneralPage.Visible = true;
                        this.tpGeneralPage.Visible = false;
						this.tpAdditionalDataPage.Visible = false;
					    this.Session.Remove(EquipmentAdditionalTabVisible);
					}
					else
					{
						this.tpAirplaneGeneralPage.Visible = false;
						this.tpGeneralPage.Visible = true;
						this.tpAdditionalDataPage.Visible = true;
					    this.Session[EquipmentAdditionalTabVisible] = "1";
					}

                    if (!this.Security.HasRight(RIGHT.VIEW_ASSET_TRACKING_DEVICES) && !this.Security.HasRight(RIGHT.MODIFY_ASSET_TRACKING_DEVICES))
                    {
				        this.EquipmentHistoryTab.Visible = false;
				        this.HistoryTabPanel.Visible = false;
				    }

					this.tpBargePage.Visible = false;
					this.tpCompartmentsPage.Visible = false;
					this.tpMeterPage.Visible = false;
					this.tpPipelinePage.Visible = false;
					this.tpQCStatusPage.Visible = false;
					this.tpRailcarPage.Visible = false;
					this.tpShipPage.Visible = false;
					this.tpTagsAndLicensesPage.Visible = false;
					this.tpTankerPage.Visible = false;
					this.tpTestsAndInspectionsPage.Visible = false;
					this.tpTractorPage.Visible = false;
					this.tpTrailerPage.Visible = false;

					this.tpGeneralPage.HeaderText = this.GetTranslatedText("General");
					this.tpAirplaneGeneralPage.HeaderText = this.GetTranslatedText("General");

					/*  Commented out for now as there is no functionality on the pages currently
							Tab TypeTab=new Tab();
							TypeTab.Text=EquipmentClass.TypeID(Equipment.Type);
							switch(Equipment.Type)
							{
								case EQUIPMENT_TYPE.TRAILER_TYPE:
								  TypeTab.TargetID="TrailerPage";
								  break;
								case EQUIPMENT_TYPE.TRACTOR_TYPE:
								  TypeTab.TargetID="TractorPage";
								  break;
								case EQUIPMENT_TYPE.TANKER_TYPE:
								  TypeTab.TargetID="TankerPage";
								  break;
								default:
								  break;
							}

							foreach(TabItem Tab in Items)
							{
								if(Tab.Text == "General")
								  continue;

								if(TypeTab.TargetID != "")
								{
								  if(Tab.Text.CompareTo(TypeTab.Text) > 0)
								  {
									 int Index=Items.IndexOf(Tab);
									 Items.AddAt(Index,TypeTab);
									 TabSeparator TypeSeparator=new TabSeparator();
									 Items.AddAt(Index+1,TypeSeparator);
									 TypeTab=null;
									 break;
								  }
								}
							}

							if(TypeTab != null
								&& TypeTab.TargetID != "")
							{
								TabSeparator TypeSeparator=new TabSeparator();
								Items.Add(TypeSeparator);
								Items.Add(TypeTab);
							}
				*/
					ushort useNewLicenseKey = FMChannelHelper.MakeCall<IHardwareKey, ushort>(hardwareKeyChannel => hardwareKeyChannel.GetUseNewLicenseFile());

					bool hasOptionInKey = false;
					if (useNewLicenseKey == 1)
					{
						ushort word1 = FMChannelHelper.MakeCall<IHardwareKey, ushort>(hardwareKeyChannel => hardwareKeyChannel.GetWord1ValueLIN());

						if ((word1 & 0x80) != 0)
							hasOptionInKey = true;
					}
					else
					{
						uint options = FMChannelHelper.MakeCall<IHardwareKey, uint>(x => x.GetOptionsCell());
						// Depends Upon WEB Inventory
						if ((options & 0x20000) != 0)
						{
							hasOptionInKey = true;
						}
					}

					
					if (hasOptionInKey
					    && (this.Security.HasRight(RIGHT.ADD_QUALITYTAG_RECORD)
					        || this.Security.HasRight(RIGHT.MODIFY_MAINTENANCE_RECORD)
					        || this.Security.HasRight(RIGHT.MODIFY_QUALITY_TESTS)
					        || this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_LOGS)
					        || this.Security.HasRight(RIGHT.MODIFY_QUALITYTAG_RECORD)
					        || this.Security.HasRight(RIGHT.VIEW_MAINTENANCE_RECORD)
					        || this.Security.HasRight(RIGHT.VIEW_QUALITY_TESTS) || this.Security.HasRight(RIGHT.VIEW_QUALITYTAG_LOGS)
					        || this.Security.HasRight(RIGHT.VIEW_QUALITYTAG_RECORD) || this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS)))
					{
						this.tpQCStatusPage.Visible = true;
						this.tpQCStatusPage.HeaderText = this.GetTranslatedText("QC Status");
					    this.Session[EquipmentQcStatus] = "1";
					}
					else
					{
					    this.Session.Remove(EquipmentQcStatus);
					}

					if (this.Equipment.IsMultiCompartment && this.Equipment.Type.IsMultiCompartmentCapable())
					{
						this.tpCompartmentsPage.Visible = true;
						this.tpCompartmentsPage.HeaderText = this.GetTranslatedText("Compartments");
					}

					if (this.Equipment.Type != EQUIPMENT_TYPE.PIPELINE_TYPE)
					{
						this.tpTestsAndInspectionsPage.Visible = true;
						this.tpTestsAndInspectionsPage.HeaderText = this.GetTranslatedText("Tests & Inspections");

						this.tpTagsAndLicensesPage.Visible = true;
						this.tpTagsAndLicensesPage.HeaderText = this.GetTranslatedText("Tags & Licenses");
					}

					if (this.Security.HasRight(RIGHT.VIEW_METERS) || this.Security.HasRight(RIGHT.MODIFY_METERS))
					{
						// Meters don't apply to aircraft
                        if (this.Equipment.Type != EQUIPMENT_TYPE.AIRCRAFT_TYPE)
						{
							this.tpMeterPage.Visible = true;
						}
						else
						{
							this.tpMeterPage.Visible = false;
						}

						this.tpMeterPage.HeaderText = this.GetTranslatedText("Meter");
					}

					this.tpAdditionalDataPage.HeaderText = this.GetTranslatedText("Additional Data");
					this.tpUserDataPage.HeaderText = this.GetTranslatedText("User Data");

					if (this.Session["EquipmentFormTabIndex"] != null)
					{
						this.tcEquipment.ActiveTabIndex = Convert.ToInt32(this.Session["EquipmentFormTabIndex"]);
						this.Session.Remove("EquipmentFormTabIndex");
					}
					else if ((this.Request.GetQueryOrFormValue("TAB") != null && this.Request.GetQueryOrFormValue("TAB") == "GeneralTab")
					         || (this.IsPostBack == false && this.IsFromDispatch))
					{
						this.tcEquipment.ActiveTabIndex = 0; // General Tab
					}
					else
					{
						if (this.IsFromDispatch)
						{
							var fromDispatch = this.Session["FromDispatch"] as string;
							if (string.IsNullOrEmpty(fromDispatch) || fromDispatch == "Init")
							{
								if (this.tpQCStatusPage.Visible)
								{
									this.tcEquipment.ActiveTabIndex = this.tcEquipment.Tabs.IndexOf(this.tpQCStatusPage);
								}
								this.Session["FromDispatch"] = "NotInit";
							}
						}
					}
				}

                //Map EquipmentGeneralPage EquipmentTypeChanged event tp EquipmentMeterPage listener
                this.EquipmentGeneralPage.EquipmentTypeChanged += this.EquipmentMeterPage.AutoGenerateSingleMeter;

            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		/// <summary>
		///    Handles the Command event of the Cancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.Web.UI.WebControls.CommandEventArgs" /> instance containing the event data.
		/// </param>
		private void CancelCommand(object sender, CommandEventArgs e)
		{
			try
			{
				this.Session.Remove("EQUIPMENT_SESSION_KEY");
				this.Session.Remove("ReturnPageFromMaintenanceAddRecordForm");
				this.Session["EquipmentFormCancel"] = "true";
				this.TransferToOriginatingForm();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

        private void GetRecordVersioningFields()
        {
            this.VersionSpecificFields = new List<string>();
            bool currentSiteOwnsRecordVersion = (this.Equipment.SiteGuid == this.Security.SiteGuid);
            if ((this.Equipment.IdentityGuid.Equals(Guid.Empty))
                || (currentSiteOwnsRecordVersion && this.Equipment.IdentityGuid.Equals(this.Equipment.MasterRecordGuid)))
            {
                return;
            }
            string flcMode = FieldLevelConfigClass.FLCModeGSOnly;
            if (currentSiteOwnsRecordVersion)
                flcMode = FieldLevelConfigClass.FLCModeVSandGS;

            try
            {
                this.VersionSpecificFields =
                    FMChannelHelper.MakeCall<IEntityToSiteMaps, List<string>>(
                            x =>
                            x.GetRecordVersioningFields(this.Security, this.Equipment.EntityType, this.Equipment.MasterRecordGuid, flcMode)
                    );

                    this.Session["EquipmentVersionSpecificFields"] = this.VersionSpecificFields;
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
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.New.Command += this.NewCommand;
			this.OK.Command += this.OkCommand;
		    this.EquipYes.Command += this.OkCommand;
			this.Cancel.Command += this.CancelCommand;
		    this.EquipNo.Command += this.CancelCommand;

			ucFMMenuBar.Visible = (Page.Request.GetQueryOrFormValue("Modal") != null) ? false : true;
		}

		private void LoadEquipment(Guid equipmentGuid)
		{
			// note had to check if it was not from dispatch otherwise every request frm dispatch was get the equipment from a previous session.
			if (this.Session["EquipmentArrayList"] != null && !this.IsFromDispatch)
			{
				return;
			}

			EquipmentClass equipment =
				FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(x => x.Get(this.Security, equipmentGuid));

			if (equipmentGuid == Guid.Empty)
			{
				equipment.InServiceFlag = true;
				equipment.InUse = true;
			}

			var list = new ArrayList { equipment };
			this.Session["EquipmentArrayList"] = list;
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

				Guid equipmentGuid = this.Equipment.IdentityGuid;
				if (this.Equipment.IdentityGuid != Guid.Empty)
				{
					FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.Security, this.Equipment));
				}
				else
				{
					FMChannelHelper.MakeCall<IEquipments>(x => x.Add(this.Security, this.Equipment));
				}

				try
				{
					if (UsingLoadRack)
					{
						ILoadRackManager loadRackManager = this.GetLoadRackManager();
						if (equipmentGuid != Guid.Empty)
						{
							loadRackManager.Modify(this.Security, typeof(EquipmentClass), this.Equipment.IdentityGuid);
						}
						else
						{
							loadRackManager.Add(this.Security, typeof(EquipmentClass), this.Equipment.IdentityGuid);
						}
					}
				}
				catch (SocketException socketExcept)
				{
					if (socketExcept.ErrorCode != 10061)
					{
						throw ;
					}
				}

				this.Equipment.ID						= string.Empty;
				this.Equipment.IdentityGuid				= Guid.Empty;
				this.Equipment.AssetTrackingDeviceGuid	= Guid.Empty;
				this.Equipment.AssetTrackingDeviceID	= "{Unassigned}";
				this.Equipment.CompanyEquipmentID		= string.Empty;
				this.Equipment.Xref						= string.Empty;
				this.Equipment.IssPtNum					= string.Empty;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return;
			}

			this.Session.Remove("EquipmentFormTabIndex");

			this.Redirect("EquipmentForm.aspx");
		}

		/// <summary>
		///    Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">
		///    The <see cref="System.Web.UI.WebControls.CommandEventArgs" /> instance containing the event data.
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

			    if (this.remote)
			    {
                    // this is for requesting the assignment of enterprise equipment down to this site.
                    // Synchronization would then pull the newly assigned equipment down to the terminal.
                    var masterRecordGuid = this.Equipment.MasterRecordGuid;

			        this.SelfAssignFromEnterprise(masterRecordGuid);
                }
                else
			    {
                    var button = sender as FMButton;
                    if (button?.ID == "EquipYes")
                    {
                        // Should only get here if we had an local/enterprise equipment ID collision and we
                        // chose to self-assign the enterprise equipment.
                        var priorEquipment = this.GetPriorEnterpriseEquipment();

                        // We're reasonably sure that we'll have a prior equipment here; else we shouldn't have gotten here.
                        this.SelfAssignFromEnterprise(priorEquipment);
                    }
                    else
			        {
			            Guid equipmentGuid = this.Equipment.IdentityGuid;
			            if (this.Equipment.IdentityGuid != Guid.Empty)
			            {
			                FMChannelHelper.MakeCall<IEquipments>(x => x.Modify(this.Security, this.Equipment));
			            }
			            else
			            {
			                // For new equipment, we have to check against enterprise.
			                var priorEquipment = this.GetPriorEnterpriseEquipment();
			                if (priorEquipment != Guid.Empty)
			                {
			                    this.Page.ClientScript.RegisterStartupScript(
			                        this.GetType(),
			                        "Equipment Already Exists",
			                        "<script type='text/javascript'>\r\n" + "<!--\r\n" + "if(window.confirm(\""
			                        + HttpUtility.JavaScriptStringEncode($"Equipment {this.Equipment.ID} already exists at the Enterprise.") + "\\r\\n"
			                        + HttpUtility.JavaScriptStringEncode("") + "\\r\\n"
			                        + HttpUtility.JavaScriptStringEncode(
			                            this.GetTranslatedText("Click OK to assign this equipment down from Enterprise.")) + "\\r\\n"
			                        + HttpUtility.JavaScriptStringEncode(
			                            this.GetTranslatedText("Press Cancel to change the local equipment.")) + "\"))\r\n"
			                        + "   document.getElementById('EquipYes').click();\r\n" + "else\r\n"
			                        + "   document.getElementById('EquipNo').click();\r\n" + "\r\n-->\r\n</script>");

			                    return;
			                }

			                FMChannelHelper.MakeCall<IEquipments>(x => x.Add(this.Security, this.Equipment));
			                this.Equipment.IdentityGuid =
			                    FMChannelHelper.MakeCall<IEquipments, Guid>(
			                        x => x.GetIdentityGuid(this.Security, this.Equipment.ID));
                            this.Equipment.MasterRecordGuid =
                                FMChannelHelper.MakeCall<IEquipments, Guid>(
                                    x => x.GetMasterRecordGuid(this.Security, this.Equipment.ID));
                            this.Session["EquipmentAdd"] = this.Equipment.IdentityGuid;

			                this.ShiftOwnershipToEnterpriseSite(this.Equipment);
			            }

			            try
			            {
			                if (UsingLoadRack)
			                {
			                    ILoadRackManager loadRackManager = this.GetLoadRackManager();
			                    if (equipmentGuid != Guid.Empty)
			                    {
			                        loadRackManager.Modify(this.Security, typeof(EquipmentClass), this.Equipment.IdentityGuid);
			                    }
			                    else
			                    {
			                        loadRackManager.Add(this.Security, typeof(EquipmentClass), this.Equipment.IdentityGuid);
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
			this.Session.Remove("EQUIPMENT_SESSION_KEY");
			this.Session.Remove("ReturnPageFromMaintenanceAddRecordForm");
			this.TransferToOriginatingForm();
		}

        private void ShiftOwnershipToEnterpriseSite(EquipmentClass equipment)
        {
            // This only applies to Terminal systems; If this is an Enterprise system
            // just exit doing nothing
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
            equipment.SiteGuid = enterpriseSiteGuid;

            // If the equipment has a meter, change the site that the meter belongs to as well
            if (equipment.Meter.Count > 0)
            {
                foreach (MeterClass meter in equipment.Meter)
                {
                    meter.SiteGuid = enterpriseSiteGuid;
                }
            }

            FMChannelHelper.MakeCall<IEquipments>(equipments => equipments.Modify(this.Security, equipment));

            // Now entity assign back to self
            var map = new EntityToSiteMapClass
            {
                ID = equipment.ID,
                IdentityGuid = equipment.IdentityGuid,
                SiteGuid = this.Security.SiteGuid,
                TypeID = ENTITY_TYPE.EQUIPMENT,
                AssignedFromSiteGuid = enterpriseSiteGuid
            };

            FMChannelHelper.MakeCall<IEntityToSiteMaps>(x => x.Add(this.Security, map, typeof(IEquipments).GUID));

            this.ForceAdHocSync();
        }

        private void SelfAssignFromEnterprise(Guid masterRecordGuid)
        {
            // This only applies to Terminal systems; If this is an Enterprise system
            // just exit doing nothing
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
                        x.RequestEnterpriseEquipmentAssignment(
                            enterpriseSecurityResponse.Security,
                            this.Security.SiteGuid,
                            masterRecordGuid));

                EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                    x => x.Logout(enterpriseSecurityResponse.Security));

                this.ForceAdHocSync();
            }
        }

        public Guid GetPriorEnterpriseEquipment()
        {
            // This only applies to Terminal systems; If this is an Enterprise system
            // just exit doing nothing; returning an empty Guid
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
                x => x.GetEquipmentMasterGuid(enterpriseSecurityResponse.Security, this.Equipment.ID));

            EnterpriseManagementChannelHelper.MakeCall<IClientEnterpriseManagementService>(
                x => x.Logout(enterpriseSecurityResponse.Security));

            return enterpriseMasterGuid;
        }

        private void TransferToOriginatingForm()
		{
			var equipmentArrayList = this.Session["EquipmentArrayList"] as ArrayList;
			if (equipmentArrayList != null && equipmentArrayList.Count > 0)
			{
				equipmentArrayList.RemoveAt(equipmentArrayList.Count - 1);
				if (equipmentArrayList.Count == 0)
				{
					this.Session.Remove("EquipmentArrayList");
				}
			}

			if (equipmentArrayList != null && equipmentArrayList.Count > 0)
			{
				equipmentArrayList.RemoveAt(equipmentArrayList.Count - 1);
				if (equipmentArrayList.Count == 0)
				{
					this.Session.Remove("EquipmentArrayList");
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
				this.Session.Remove("FromDispatch");
				this.Redirect("../DispatchWebApp/DispatchingView.aspx");
			}
			else if (this.Session["EquipmentSelectContextArrayList"] == null)
			{
				this.Redirect("EquipmentsForm.aspx");
			}
			else
			{
				var equipmentSelectContextArrayList = this.Session["EquipmentSelectContextArrayList"] as ArrayList;
				var equipmentSelectContext =
					equipmentSelectContextArrayList?[equipmentSelectContextArrayList.Count - 1] as EquipmentSelectContextClass;

				equipmentSelectContextArrayList?.RemoveAt(equipmentSelectContextArrayList.Count - 1);
				if ((equipmentSelectContextArrayList?.Count ?? 0) == 0)
				{
					this.Session.Remove("EquipmentSelectContextArrayList");
				}

				string transferString = "EquipmentSelectForm.aspx?";

				if (equipmentSelectContext != null && equipmentSelectContext.Type != EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE)
				{
					transferString += "Type=" + equipmentSelectContext.Type + "&";
				}

				transferString += "Unassigned=" + (equipmentSelectContext != null && equipmentSelectContext.Unassigned) + "&";

				if (equipmentSelectContext?.EquipmentTextBoxID != null)
				{
					transferString += "EquipmentTextBoxID=" + equipmentSelectContext.EquipmentTextBoxID + "&";
				}

				if (equipmentSelectContext?.IDCarrierLink != null)
				{
					transferString += "IDCarrierLink=" + equipmentSelectContext.IDCarrierLink + "&";
				}

				if (equipmentSelectContext?.Mode != null)
				{
					transferString += "Mode=" + equipmentSelectContext.Mode + "&";
				}

				if (equipmentSelectContext?.SearchString != null)
				{
					transferString += "SearchString=" + equipmentSelectContext.SearchString + "&";
				}

                if (equipmentSelectContext != null && equipmentSelectContext.HideHidden)
                {
                    transferString += "HideHidden=" + equipmentSelectContext.HideHidden + "&";
                }

				this.Redirect(transferString);
			}
		}

        #endregion

        private void ForceAdHocSync()
        {
            // Forcing ad hoc synchronization should only occur at the terminal
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
    }

    public class EquipmentPageBase : FMUserControlBase
	{
		#region Properties

		protected EquipmentClass Equipment => ((EquipmentForm)this.Page).Equipment;

	    protected List<string> VersionSpecificFields => ((EquipmentForm)this.Page).VersionSpecificFields;

	    #endregion
	}
}