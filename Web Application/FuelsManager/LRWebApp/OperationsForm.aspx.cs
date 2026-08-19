// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable once CheckNamespace
namespace LoadRackWebApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Net.Sockets;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Interfaces;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using FuelsManager.FMWebApp;

    /// <summary>
	/// Code behind for OperationsForm.
	/// </summary>
	public partial class OperationsForm : FMFormBase, IMenuDiscovery
	{
		#region Constants and Fields

		public string LastInventoryDate = string.Empty;

		public List<string> VersionSpecificFields;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">
		/// The security object of the current session
		/// </param>
		/// <param name="siteGroup">
		/// Whether the current logged-in site is a site group
		/// </param>
		/// <param name="options">
		/// Hardware key options
		/// </param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if (useNewLicenseKey == 1)
            {
                if ((word2 & 0x01) != 0x01)
                    return null;
            }
            else
            {
                // Depends Upon Load Rack
                if ((options & 0x8000) == 0)
                {
                    return null;
                }
            }

            var items = new List<FMMenuItem>();

			if (siteGroup)
			{
				return null;
			}

			if (!security.HasRight(RIGHT.VIEW_LOAD_RACK_DATA) && !security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
			{
				return null;
			}

			items.Add(
				new FMMenuItem
					{
						MenuItemType = FMMenuItemType.OPERATIONS_LOAD_RACK_OPERATIONS, 
						RootMenuName = "Operations", 
						CategoryName = "Load Rack", 
						ItemName = "Operations", 
						NavigateUrl = "..\\LRWebApp\\OperationsForm.aspx", 
						ApplyDataDictionary = ApplyDataDictionary.Apply
					});

			return items;
		}

		#endregion

		#region Methods
		private void SetFieldAccessibilityForChildRecordVersion(PersonClass person)
		{
		    this.GetRecordVersioningFields(person);
			bool currentSiteOwnsRecordVersion = (person.SiteGuid == this.Security.SiteGuid);
			if ((person.IdentityGuid.Equals(Guid.Empty)
				  || (currentSiteOwnsRecordVersion && person.IdentityGuid.Equals(person.MasterRecordGuid))
				  || (this.VersionSpecificFields == null)))
			{
				return;
			}
			this.ResetLastActivityDate.Enabled = (this.ResetLastActivityDate.Enabled && currentSiteOwnsRecordVersion && this.VersionSpecificFields.Contains("LastActivityDate"));
		}

		private void GetRecordVersioningFields(PersonClass person)
		{
			this.VersionSpecificFields = new List<string>();
            bool currentSiteOwnsRecordVersion = (person.SiteGuid == this.Security.SiteGuid);
            if ((person.IdentityGuid.Equals(Guid.Empty))
				 || (currentSiteOwnsRecordVersion && person.IdentityGuid.Equals(person.MasterRecordGuid)))
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
																x.GetRecordVersioningFields(this.Security, person.EntityType, person.MasterRecordGuid, flcMode)
														  );
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
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
		    this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					// Populate the PersonnelDropDownLists
					var personCollection =
						FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(x => x.Enumerate(this.Security));
					
					var houseCardCollection =
						FMChannelHelper.MakeCall<IHouseCards, HouseCardCollectionClass>(x => x.Enumerate(this.Security));

					foreach (PersonClass person in personCollection)
					{
						var item = new ListItem(person.ID, person.MasterRecordGuid.ToString());
						this.PersonnelDropDownList2.Items.Add(item);

						bool personFound = false;
						foreach (HouseCardClass houseCard in houseCardCollection)
						{
							if (houseCard.DriverGuid == person.MasterRecordGuid)
							{
								personFound = true;
								break;
							}
						}

						if (personFound)
						{
							continue;
						}

						this.PersonnelDropDownList1.Items.Add(item);
					}

					// Populate the CardDropDownList
					foreach (HouseCardClass houseCard in houseCardCollection)
					{
						if (!houseCard.DriverGuid.IsEmpty())
						{
							continue;
						}

						var item = new ListItem(houseCard.ID, houseCard.IdentityGuid.ToString());
						this.CardDropDownList.Items.Add(item);
					}

					this.PopulateStationDropDownList();

                    this.AssignButton.Enabled = (this.CardDropDownList.Items.Count > 0 && this.PersonnelDropDownList1.Items.Count > 0);
                    this.ResetLastActivityDate.Enabled = (this.PersonnelDropDownList2.Items.Count > 0);

					SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security, 
																			this.Security.SiteGuid, 
																			getMemberSites: false, 
																			getSchedulesAndProcessVariables: false, 
																			bGetAssociatedAliases: true)
																);

					if (site.UseLastKnownGoodTankData)
					{
					    this.TankDataButton.Text = this.GetTranslatedText("Use Current Tank Data");
					    this.CurrentTankDataMode.Text = this.GetTranslatedText("Last Known Good Tank Data");
					}
					else
					{
					    this.TankDataButton.Text = this.GetTranslatedText("Use Last Known Good Tank Data");
					    this.CurrentTankDataMode.Text = this.GetTranslatedText("Current Tank Data");
					}

					// Populate the DataExchangeDropDownList. (04-Jun-2008 IGO)
					this.PopulateDataExchangeDropDownList();

					this.CurrentInventoryDateControl.Enabled = false;

                    if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA) || this.DataExchangeDropDownList.Items.Count == 0)
                    {
                        this.SendPIDXTransButton.Enabled = false;
                    }
           
					if (!this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.InitiateEndOfDayButton.Enabled = false;
						this.AssignButton.Enabled = false;
						this.TankDataButton.Enabled = false;
						this.ResetLastActivityDate.Enabled = false;					
						this.ResetOwnerAllocationsButton.Enabled = false;
						this.DownloadAccessConfiguration.Enabled = false;
						this.StationDropdownList.Enabled = false;
					}
					else
					{
						// if there are no stations configured disable the controls
                        if (this.StationDropdownList.Items.Count <= 0)
                        {
                            this.DownloadAccessConfiguration.Enabled = false;
                            this.StationDropdownList.Enabled = false;
                        }

					    var inventoryDateSR = new InventoryDateSR
					                              {
					                                  Security = this.Security,
					                                  CurrentSiteGuid = this.Security.SiteGuid
					                              };


					    InventoryDateDO inventoryDateDO =
							FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(proc => proc.Process(inventoryDateSR));

						this.CurrentInventoryDateControl.CurrentValue = inventoryDateDO.InventoryDate;
					}

					if (this.PersonnelDropDownList2.Items.Count > 0)
					{
						var pers =
							FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
								x => x.Get(this.Security, Guid.Parse(this.PersonnelDropDownList2.SelectedItem.Value)));
					
						this.SetFieldAccessibilityForChildRecordVersion(pers);
					}


					this.ajaxloaderimg.Visible = false;
					this.LabelProgress.Visible = false;
					this.barwrapper.Visible = false;
					this.bar.Visible = false;
					this.LabelLastEOD.Visible = false;
					
					// check to see if we are alreading running the end of day process in 1 second
					this.TimerControl1.Interval = 1000;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void AssignButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    if (this.CardDropDownList.SelectedItem == null)
			    {
			        throw new Exception("You must select a card");
			    }

                if (this.PersonnelDropDownList1.SelectedItem == null)
                {
                    throw new Exception("You must select a personnel record");
                }

				FMChannelHelper.MakeCall<IHouseCards>(
					houseCards =>
						{
							HouseCardClass houseCard = houseCards.Get(this.Security, new Guid(this.CardDropDownList.SelectedValue));
							houseCard.DriverGuid = new Guid(this.PersonnelDropDownList1.SelectedValue);
							houseCard.DriverID = this.PersonnelDropDownList1.SelectedItem.Text;
							houseCards.Modify(this.Security, houseCard);
						});

				this.CardDropDownList.Items.RemoveAt(this.CardDropDownList.SelectedIndex);
				this.PersonnelDropDownList1.Items.RemoveAt(this.PersonnelDropDownList1.SelectedIndex);
				this.AssignButton.Enabled = (this.CardDropDownList.Items.Count > 0 && this.PersonnelDropDownList1.Items.Count > 0);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void DownloadAccessConfiguration_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    if (this.StationDropdownList.SelectedItem == null)
			    {
			        throw new Exception("You must select a station");
			    }

				ListItem item = this.StationDropdownList.SelectedItem;
				ILoadRackManager loadRackManager = this.GetLoadRackManager();

				loadRackManager.DownloadLocalConfigurationToStation(this.Security, Guid.Parse(item.Value));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ResetOwnerAllocationsButton.Command +=
				this.ResetOwnerAllocationsButton_Command;
			this.ResetLastActivityDate.Command +=
				this.ResetLastActivityDate_Command;
			this.TankDataButton.Command += this.TankDataButton_Command;
			this.InitiateEndOfDayButton.Command +=
				this.InitiateEndOfDayButton_Command;
			this.AssignButton.Command += this.AssignButton_Command;
			this.SendPIDXTransButton.Command +=
				this.SendPIDXTransButton_Command;
			this.DownloadAccessConfiguration.Command +=
				this.DownloadAccessConfiguration_Command;
		}

	    // ReSharper disable once InconsistentNaming
		private void InitiateEndOfDayButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
							x.Get(
								this.Security,
								this.Security.SiteGuid,
								getMemberSites: false,
								getSchedulesAndProcessVariables: false,
								bGetAssociatedAliases: false));

				DateTimeOffset currentDate = TimeConverter.Now( site );
				var controlDate = this.CurrentInventoryDateControl.CurrentValue.Date;
				if ( currentDate.Date == controlDate
					&& currentDate.TimeOfDay <= new TimeSpan( 12, 0, 0 ) )
				{
					throw new Exception("Cannot initiate End of Day processing before noon of the current date.");
				}

				if ( currentDate.Date < this.CurrentInventoryDateControl.CurrentValue.Date )
				{
					throw new Exception("Cannot initiate End of Day processing when selected date is later than the current date.");
				}

				ILoadRackManager loadRackManager = this.GetLoadRackManager();
				loadRackManager.InitiateEndOfDay(this.Security);
				// enable the processing messages
				this.LabelLastEOD.Visible = false;
				this.ajaxloaderimg.Visible = true;
				this.LabelProgress.Visible = true;
				this.LabelProgress.Text = "Starting End of Day";
				this.barwrapper.Visible = true;
				this.bar.Visible = true;
				this.TimerControl1.Interval = 1000;
				this.TimerControl1.Enabled = true;
				this.InitiateEndOfDayButton.Enabled = false;

				}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void PopulateDataExchangeDropDownList()
		{
			var pidxProfileCollection =
				FMChannelHelper.MakeCall<IPIDXProfiles, PIDXProfileCollectionClass>(x => x.Enumerate(this.Security));

			if (pidxProfileCollection.Count > 0)
			{
				var allitem = new ListItem("{All}", "{All}");
				this.DataExchangeDropDownList.Items.Add(allitem);
				foreach (PIDXProfileClass pidxProfile in pidxProfileCollection)
				{
					var item = new ListItem(pidxProfile.ID, pidxProfile.IdentityGuid.ToString());
					this.DataExchangeDropDownList.Items.Add(item);
				}
			}
		}

		private void PopulateStationDropDownList()
		{
			var stationCollection =
				FMChannelHelper.MakeCall<IStations, StationCollectionClass>(stations => stations.Enumerate(this.Security));

			this.StationDropdownList.Items.Clear();
			foreach (StationClass station in stationCollection)
			{
				// only add the Contrec ra since it is the only one supported
				if (station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA && station.Enabled)
				{
					var item = new ListItem(station.ID, station.IdentityGuid.ToString());
					this.StationDropdownList.Items.Add(item);
				}
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void ResetLastActivityDate_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    if (this.PersonnelDropDownList2.SelectedItem == null)
			    {
			        throw new Exception("You must select a personnel record");
			    }

				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(this.Security, this.Security.SiteGuid, false, false, true)
																);
				FMChannelHelper.MakeCall<IPersonnel>(
					personnel =>
						{
							PersonClass person = personnel.Get(this.Security, Guid.Parse(this.PersonnelDropDownList2.SelectedValue));
							person._LastActivityDate.Value = TimeConverter.Now(site);
							personnel.Modify(this.Security, DATA_TYPE.DYNAMIC, person);
						});
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void ResetOwnerAllocationsButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				ILoadRackManager loadRackManager = this.GetLoadRackManager();
				loadRackManager.ResetOwnerAllocations(this.Security);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void SendPIDXTransButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
			    if (this.DataExchangeDropDownList.SelectedItem == null)
			    {
			        throw new Exception("You must select a data exchange profile");
			    }

			    var getTransSR = new GetTransactionSR
			                         {
			                             Security = this.Security,
			                             Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
			                             TransTypeID = TransactionTypes.T5_PrimaryDisbursement,
			                             DocumentNumber = this.BOLNumberTextBox.Text
			                         };

			    GetTransactionDO getTransDO =
					FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(proc => proc.Process(getTransSR));

				// Throw and exception of transaction matching the document number if found.
				if (getTransDO.TransactionDataSet.Tables[0].Rows.Count == 0)
				{
					throw new Exception("No associated transaction(s) found.");
				}

				var saveTransactionsSR = new SaveTransactionsSR
					{
						Security = this.Security,
						UseAutoComplete = true,
						CurrentSiteGuid = this.Security.SiteGuid,
						ConvertUnits = false,
                        ForceNewPidxRecord = true
					};

				FMChannelHelper.MakeCall<ICompanyMaps>(
					x =>
					FMChannelHelper.MakeCall<ITransactionProcessor>(
						transProc => 
							FMChannelHelper.MakeCall<IPIDXProfiles>(
								pidxProfiles =>
									FMChannelHelper.MakeCall<IPIDXProfileCompanyMaps>( 
										maps => this.ProcessRows( x, transProc, pidxProfiles, maps, getTransDO, saveTransactionsSR ) ) ) ) );

				FMChannelHelper.MakeCall<ISaveTransactionsProcessor>( x => x.SaveTransactions(saveTransactionsSR));
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ProcessRows( ICompanyMaps companyMaps, ITransactionProcessor transProc, IPIDXProfiles pidxProfiles, IPIDXProfileCompanyMaps profileCompanyMaps, GetTransactionDO getTransDO, SaveTransactionsSR saveTransactionsSR )
		{
			// Iterate through each transaction in the dataset
			foreach (DataRow row in getTransDO.TransactionDataSet.Tables[0].Rows)
			{
			    var transSR = new TransactionSR
			                      {
			                          Security = this.Security,
			                          TransID = (string)row["TransID"],
			                          ConvertUnits = false
			                      };

			    TransactionDO trans = transProc.Process(transSR);

				trans.TransPIDXCollection = null;

				Guid companyPersonnelToShipToBillToGuid = Guid.Empty;

				if (trans.ShipToCompanyGuid != Guid.Empty && trans.ShipToCompanyGuid != Guid.Empty
				    && trans.BillToCompanyGuid != Guid.Empty && trans.BillToCompanyGuid != Guid.Empty)
				{
					CompanyMapCollectionClass shipToBillToMaps = companyMaps.EnumerateByAssignedGuidAndType(
						this.Security, trans.ShipToCompanyGuid, COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP);

					foreach (CompanyMapClass shipToBillToMap in shipToBillToMaps)
					{
						CompanyMapClass billToShipperMap = companyMaps.Get(
							this.Security, shipToBillToMap.AssignedToGuid, COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP);

						if (billToShipperMap.AssignedGuid == trans.BillToCompanyGuid)
						{
							CompanyMapCollectionClass loadIDToShipToMaps = companyMaps.EnumerateByAssignedToGuidAndType(
								this.Security, shipToBillToMap.IdentityGuid, COMPANY_MAP_TYPE.LOADID_SHIPTO_MAP);

							foreach (CompanyMapClass loadIDToShipToMap in loadIDToShipToMaps)
							{
								// {All} Drivers
								if (loadIDToShipToMap.AssignedGuid == Guid.Empty)
								{
									trans.LoadID = loadIDToShipToMap.MapID;
									companyPersonnelToShipToBillToGuid = loadIDToShipToMap.IdentityGuid;
								}
								else if (trans.OperatorPersonnelGuid != Guid.Empty && trans.OperatorPersonnelGuid == loadIDToShipToMap.AssignedGuid)
								{
									// Driver Specific LoadID
									trans.LoadID = loadIDToShipToMap.MapID;
									companyPersonnelToShipToBillToGuid = loadIDToShipToMap.IdentityGuid;
								}

								break;
							}
						}
					}
				}

				if (companyPersonnelToShipToBillToGuid != Guid.Empty)
				{
					trans.TransPIDXCollection = new List<TransactionPIDXDO>();

					PIDXProfileCollectionClass pidxProfileCollection = pidxProfiles.Enumerate(this.Security);

					PIDXProfileCompanyMapCollectionClass pidxProfileCompanyMapCollection =
						profileCompanyMaps.EnumerateSiteAndCompanyPersonnelToShipToBillToGuid(
							this.Security, companyPersonnelToShipToBillToGuid);

					foreach (PIDXProfileCompanyMapClass pidxProfileCompanyMap in pidxProfileCompanyMapCollection)
					{
						PIDXProfileClass pidxProfile = pidxProfileCollection.Find(pidxProfileCompanyMap.PIDXProfileGuid);
						if (pidxProfile == null || !pidxProfile.Enabled)
						{
							continue;
						}

						// Filter to only create for the profile selected
						if (this.DataExchangeDropDownList.SelectedValue != "{All}"
						    && Guid.Parse(this.DataExchangeDropDownList.SelectedValue) != pidxProfile.IdentityGuid)
						{
							continue;
						}

					    var transactionPidxdo = new TransactionPIDXDO
					                                {
					                                    PIDXProfileGuid = pidxProfileCompanyMap.PIDXProfileGuid,
					                                    CompanyPersonnelToShipToBillToGuid =
					                                        pidxProfileCompanyMap.CompanyPersonnelToShipToBillToGuid
					                                };


					    trans.TransPIDXCollection.Add(transactionPidxdo);
					}
				}

				saveTransactionsSR.Transactions.Add(trans);
			}
		}

	    // ReSharper disable once InconsistentNaming
		private void TankDataButton_Command(object sender, CommandEventArgs e)
		{
			try
			{
				SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(
																			this.Security, 
																			this.Security.SiteGuid, 
																			getMemberSites: false, 
																			getSchedulesAndProcessVariables: true, 
																			bGetAssociatedAliases: true)
																	);
				if (this.TankDataButton.Text == this.GetTranslatedText("Use Last Known Good Tank Data"))
				{
					site.UseLastKnownGoodTankData = true;
				}
				else
				{
					site.UseLastKnownGoodTankData = false;
				}

				FMChannelHelper.MakeCall<ISites>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, site, updateDocumentNumbers: true)
																);

				if (site.UseLastKnownGoodTankData)
				{
				    this.TankDataButton.Text = this.GetTranslatedText("Use Current Tank Data");
				    this.CurrentTankDataMode.Text = this.GetTranslatedText("Last Known Good Tank Data");
				}
				else
				{
				    this.TankDataButton.Text = this.GetTranslatedText("Use Last Known Good Tank Data");
				    this.CurrentTankDataMode.Text = this.GetTranslatedText("Current Tank Data");
				}

				try
				{
					ILoadRackManager loadRackManager = this.GetLoadRackManager();
					loadRackManager.Modify(this.Security, typeof(SiteClass), site.IdentityGuid);
				}
				catch (SocketException)
				{
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void TimerControl1_Tick(object sender, EventArgs e)
		{
			ILoadRackManager loadRackManager = null;
			try
			{
                loadRackManager = this.GetLoadRackManager();
			}
			catch (SocketException ex)
			{
				Session["PreviousUrl"]=ResolveUrl("../FMWebApp/FuelsManagerForm.aspx");

                Session["ErrorMessage"] = ex.Message;
				Redirect("../FMWebApp/RedirectOnError.aspx");
				return;
            }
			catch { }
			if (loadRackManager == null)
			{
                this.ajaxloaderimg.Visible = false;
                this.LabelProgress.Visible = false;
                this.barwrapper.Visible = false;
                this.bar.Visible = false;
                this.TimerControl1.Enabled = false;
                this.LabelLastEOD.Visible = true;
                return;
            }
            try
			{
				Hashtable results = loadRackManager.GetEndOfDayStatus(this.Security);
				string message = "";
				int progressPercentage = 0;
				string error = "";
				DateTimeOffset? lastEOD = null;
				if (results.ContainsKey("endOfDayMessage")) {
					message = results["endOfDayMessage"].ToString(); 
				}
				if (results.ContainsKey("endOfDayProcessingPercentage"))
				{
					progressPercentage = (int)results["endOfDayProcessingPercentage"];
				}
				if (results.ContainsKey("endOfDayError"))
				{
					error = results["endOfDayError"].ToString();
				}
				if (results.ContainsKey("lastSuccessfulEndOfDayTime"))
				{
					if (results["lastSuccessfulEndOfDayTime"] != null) {
						lastEOD = (DateTimeOffset?)results["lastSuccessfulEndOfDayTime"];
                    }
				}

				var isCurrentlyProcessing = (this.LabelProgress.Text != "");
				this.LabelProgress.Text = message;
				if (message == "") {
					this.ajaxloaderimg.Visible = false;
					this.LabelProgress.Visible = false;
					this.barwrapper.Visible = false;
					this.bar.Visible = false;
					this.TimerControl1.Interval = 30000;
					if (lastEOD != null){
						this.LabelProgress.Visible = true;
						SiteClass site =
							FMChannelHelper.MakeCall<ISites, SiteClass>(
								x =>
									x.Get(
										this.Security,
										this.Security.SiteGuid,
										getMemberSites: false,
										getSchedulesAndProcessVariables: false,
										bGetAssociatedAliases: false));

						this.LabelLastEOD.Visible = true;
						this.LabelLastEOD.Text = "Last successful EOD on " + site.FormatValue(lastEOD.GetValueOrDefault().DateTime, 0);
					}

					if (this.Security.HasRight(RIGHT.MODIFY_LOAD_RACK_DATA))
					{
						this.InitiateEndOfDayButton.Enabled = true;
					}
					// check if the inventory date has changed because the end of day has changed it
					var inventoryDateSR = new InventoryDateSR
					{
						Security = this.Security,
						CurrentSiteGuid = this.Security.SiteGuid
					};
					InventoryDateDO inventoryDateDO =
						FMChannelHelper.MakeCall<IInventoryDateProcessor, InventoryDateDO>(proc => proc.Process(inventoryDateSR));
					this.CurrentInventoryDateControl.CurrentValue = inventoryDateDO.InventoryDate;

					// if it was processing and now the message that we receive is empty then we have finished the closeout
					if (isCurrentlyProcessing)
					{
						ScriptManager.RegisterStartupScript(UpdatePanel1, typeof(string), "closeoutalertscript", "alert('Closeout Completed')", true);
					}

				}
				else {
					if (progressPercentage == 0)
					{
						this.ajaxloaderimg.Visible = true;
					}
					else
					{
						this.ajaxloaderimg.Visible = false;
					}
					this.LabelProgress.Visible = true;
					this.barwrapper.Visible = true;
					this.bar.Visible = true;
					this.TimerControl1.Interval = 2000;
					this.InitiateEndOfDayButton.Enabled = false;
					this.LabelLastEOD.Visible = false;
				}
				this.bar.Attributes.CssStyle.Value = "width:" + (progressPercentage * 2).ToString() + "px;";

			}
			catch (Exception)
			{
				this.ajaxloaderimg.Visible = false;
				this.LabelProgress.Visible = false;
				this.barwrapper.Visible = false;
				this.bar.Visible = false;
				this.TimerControl1.Enabled = false;
				this.LabelLastEOD.Visible = true;
			}

		}

		#endregion

		// ReSharper disable once InconsistentNaming
		protected void PersonnelDropDownList2_SelectedIndexChanged(object sender, EventArgs e)
		{
			var pers = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(x => x.Get(this.Security, Guid.Parse(this.PersonnelDropDownList2.SelectedItem.Value)));
		    this.SetFieldAccessibilityForChildRecordVersion(pers);
		}
	}
}