/******************************************************************************

	FILE NAME:		Contrec1010RAStationManagerClass.cs


	PURPOSE:			Contrec1010RAStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	B. Schaal


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/
namespace LoadRackLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.LogClient;
    using FMBusinessObjects.UtilityObjects;

    using FMCore;

    using Opc;
    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Server = Opc.Da.Server;

    class Contrec1010RAStationManagerClass : StationManagerClass
	{
		protected ProcessVariableClass CommandFieldPV;
		protected ProcessVariableClass MessageTimeoutPV;
		protected Thread ScanThread;
		protected ManualResetEvent StationKillEvent;
		protected int MessageTimer;
		public bool ResponsePending;
		public bool ReadyToLoad = false;
		public int NumberOfArmsConfigured;
		public int FirstArmConfigured;
		protected int ContrecLastTransactionNumber;
		protected string SelectedProductID = "";

		public Contrec1010RAStationManagerClass(EventLog EventLog,
			LoadRackManagerClass LoadRackManager,
			StationClass Station,
			SiteManagerClass SiteManager,
			SecurityClass Security)
			: base(EventLog, LoadRackManager, Station, SiteManager, Security)
		{
			// in this mode the contrec handles the card in of the driver and personnel
			string StationURL = Station.ProcessVariableCollection[0].URL;
			string StationProgID = Station.ProcessVariableCollection[0].ProgID;
			string StationOPCItemID = Station.ProcessVariableCollection[0].OPCItemID;

		    this.CommandFieldPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.STATION_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				true,
				StationOPCItemID + ".Command Field",
				StationURL,
				StationProgID);

		    this.MessageTimeoutPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV,
				UNIT_TYPE.LOADARM_UNIT,
				VarEnum.VT_BOOL,
				true,
				StationOPCItemID + ".Display Message Time-out",
				StationURL,
				StationProgID);

		    this.OPCServerManager.AddProcessVariable(this.CommandFieldPV);
		    this.OPCServerManager.AddProcessVariable(this.MessageTimeoutPV);

		    this.SendEndTransaction();

		    this.SetObjectOperationalVariables();

			ThreadStart ScanStart = new ThreadStart(this.ScanDataThread);
		    this.StationKillEvent = new ManualResetEvent(false);
		    this.ScanThread = new Thread(ScanStart);
		    this.ScanThread.Start();
		    this.ScanThread.Priority = ThreadPriority.AboveNormal;

		}

		public override void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
			    this.SendEndTransaction();
				base.Dispose();

				// Terminate the Scan Thread
				if (this.StationKillEvent != null) this.StationKillEvent.Set();
				if (this.ScanThread != null) this.ScanThread.Join();

				GC.SuppressFinalize(this);
			    this.AlreadyDisposed = true;
			}
		}

		public void ScanDataThread()
		{
			DateTimeOffset LastDateTime = DateTimeOffset.Now;

		    this.CheckandResetContrecTimeDate();

			while (!this.StationKillEvent.WaitOne(1000, true))
			{

				Monitor.Enter(this);

				try
				{
					try
					{
						if (this.MessageTimer > 0)
						{
							this.MessageTimer--;
							if (this.MessageTimer == 0)
							{
								this.ProcessMessageTimeout();
							}
						}
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry("MultiloadIIStationManager Scan : " + e.Message);
					}


					// check if we should reset the clock in the Contrec
					if (DateTimeOffset.Now.Day != LastDateTime.Day)
					{
						this.CheckandResetContrecTimeDate();
						LastDateTime = DateTimeOffset.Now;
					}
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		protected override void LoadRackProcessing(ProcessVariableClass PV)
		{
			Monitor.Enter(this);
			try
			{
				if (!PV.IsQualityGood)
				{
					return;
				}

				switch (PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.STATION_PV:
						{
							if (PV.IsQualityGood)
							{
								if (PV.ServerValue.ToString() == "RA")
								{
									if (this.StationState == StationState.IDLE)
									{
										// user has entered a pin number for personnel
										string DriverEnteredValue = "";
										string TruckEnteredValue = "";
										if (this.Station.TouchKeyReader) // touch key reader used
										{
											Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Driver Touch Key")),
															new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Truck Touch Key")),};

											ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);
											if (Values[0].Quality == Quality.Good)
											{
												try
												{
													DriverEnteredValue = Values[0].Value.ToString();
												}
												catch
												{
												}
											}
											if (Values[1].Quality == Quality.Good)
											{
												try
												{
													TruckEnteredValue = Values[1].Value.ToString();
												}
												catch
												{
												}
											}
										}
										else// pin number used
										{
											Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Driver Pin Number")),
															new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Truck Pin Number")),};

											ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);
											if (Values[0].Quality == Quality.Good)
											{
												try
												{
													DriverEnteredValue = Values[0].Value.ToString();
												}
												catch
												{
												}
											}
											if (Values[1].Quality == Quality.Good)
											{
												try
												{
													TruckEnteredValue = Values[1].Value.ToString();
												}
												catch
												{
												}
											}
										}

										string ErrorMessage = "";
										bool DataProcessed = false;
										// ensure that all data supplied is present and correct

										if (DriverEnteredValue.Length == 0 || DriverEnteredValue == "Bad Pin")
										{
											ProcessVariableClass ContrecRemoteAuthorize;

											ContrecRemoteAuthorize = new ProcessVariableClass(
												PROCESS_VARIABLE_TYPE.STATION_PV,
												UNIT_TYPE.STATION_UNIT,
												VarEnum.VT_BSTR,
												false,
												this.StationPv.OPCItemID + ".Remote Auth Error",
												this.StationPv.URL,
												this.StationPv.ProgID);

											ContrecRemoteAuthorize.ServerValue = "Valid Driver Key Required";
											try
											{
											    this.OPCServerManager.Write(ContrecRemoteAuthorize);
											}
											catch
											{
											}
											break;
										}
										if (TruckEnteredValue.Length == 0 || TruckEnteredValue == "Bad Pin")
										{
											ProcessVariableClass ContrecRemoteAuthorize;

											ContrecRemoteAuthorize = new ProcessVariableClass(
												PROCESS_VARIABLE_TYPE.STATION_PV,
												UNIT_TYPE.STATION_UNIT,
												VarEnum.VT_BSTR,
												false,
												this.StationPv.OPCItemID + ".Remote Auth Error",
												this.StationPv.URL,
												this.StationPv.ProgID);

											ContrecRemoteAuthorize.ServerValue = "Valid Truck Key Required";
											try
											{
											    this.OPCServerManager.Write(ContrecRemoteAuthorize);
											}
											catch
											{
											}
											break;
										}

										if (DriverEnteredValue.Length > 0 && DriverEnteredValue != "Bad Pin")
										{
											DataProcessed = this.ProcessDriverIDContrec(DriverEnteredValue, out ErrorMessage);

											if (DataProcessed == false)
											{
												ProcessVariableClass ContrecRemoteAuthorize;

												ContrecRemoteAuthorize = new ProcessVariableClass(
													PROCESS_VARIABLE_TYPE.STATION_PV,
													UNIT_TYPE.STATION_UNIT,
													VarEnum.VT_BSTR,
													false,
													this.StationPv.OPCItemID + ".Remote Auth Error",
													this.StationPv.URL,
													this.StationPv.ProgID);

												ContrecRemoteAuthorize.ServerValue = ErrorMessage;
												try
												{
												    this.OPCServerManager.Write(ContrecRemoteAuthorize);
												}
												catch
												{
												}
												break;
											}
										}
										if (TruckEnteredValue.Length > 0 && TruckEnteredValue != "Bad Pin")
										{
											DataProcessed = this.ProcessTruckIDContrec(TruckEnteredValue, out ErrorMessage);

											if (DataProcessed == false)
											{
												ProcessVariableClass ContrecRemoteAuthorize;

												ContrecRemoteAuthorize = new ProcessVariableClass(
													PROCESS_VARIABLE_TYPE.STATION_PV,
													UNIT_TYPE.STATION_UNIT,
													VarEnum.VT_BSTR,
													false,
													this.StationPv.OPCItemID + ".Remote Auth Error",
													this.StationPv.URL,
													this.StationPv.ProgID);

												ContrecRemoteAuthorize.ServerValue = ErrorMessage;
												try
												{
												    this.OPCServerManager.Write(ContrecRemoteAuthorize);
												}
												catch
												{
												}
												break;
											}
										}
									}
									else if (this.StationState == StationState.AUTHORIZED)
									{
										ProcessVariableClass ContrecRemoteAuthorize;

										ContrecRemoteAuthorize = new ProcessVariableClass(
											PROCESS_VARIABLE_TYPE.STATION_PV,
											UNIT_TYPE.STATION_UNIT,
											VarEnum.VT_BOOL,
											false,
											this.StationPv.OPCItemID + ".Remote Authorize",
											this.StationPv.URL,
											this.StationPv.ProgID);

										ContrecRemoteAuthorize.ServerValue = true;
										try
										{
										    this.OPCServerManager.Write(ContrecRemoteAuthorize);
										}
										catch
										{
										}
									}
								}
								else if (PV.ServerValue.ToString() == "AA")
								{
									if (this.ResponsePending == true)
									{
										bool bLoadingTerminated = false;

										Item[] Items ={	new Item(new ItemIdentifier(this.StationPv.OPCItemID+".Entered Data")),
														};

										ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);
										if (Values[0].Quality == Quality.Good)
										{
											string KeypadData = Values[0].Value.ToString();
										    this.ResponsePending = false;

											// the loadrack expects the yes no to be sent as Yes or No. The contrec returns these as YES and NO.
											// change the formatting but only for these two responses
											if (KeypadData.ToString().ToUpper() == "YES")
												KeypadData = "Yes";
											else if (KeypadData.ToString().ToUpper() == "NO")
											{
												if (this.StationState == StationState.VERIFY_SHIPTO_MSG || this.StationState == StationState.PROMPT_FOR_RETURNS)
													KeypadData = "No";
												else
													KeypadData = EscapeString;
											}
											if (this.StationState == StationState.PROMPT_FOR_RETURNS) this.ProcessResponseData(KeypadData);
											else
											{
											    this.ProcessResponseData(KeypadData);
											}

											if (bLoadingTerminated == true)
											{
												bLoadingTerminated = false;
											}

										}
										else
											this.eventLog.WriteEntry("Contrec OnInvoke : Keypad Data Bad " + PV.OPCItemID, EventLogEntryType.Error);

									}

								}
							}
							break;
						}
					case PROCESS_VARIABLE_TYPE.PROMPT_TIMEOUT_PV:
						{
							if (PV.IsQualityGood
							&& (bool)PV.ServerValue)
							{
							    this.ResponsePending = false;

							    this.ProcessMessageTimeout();

							    this.StationState = StationState.IDLE;
							}
							break;
						}
					default:
						base.LoadRackProcessing(PV);
						break;
				}
			}
			catch (OpcException e)
			{
				this.eventLog.WriteEntry("Contrec 1010RA StationManager OnInvoke : " + e.Message, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected bool ProcessDriverIDContrec(string Response, out string ErrorMessage)
		{
			ErrorMessage = "Test";

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "ProcessDriverID");
			timer.Perform("ProcessDriverID");
			try
			{
			    this.Driver = null;

			    this.CardID = Response;

				Guid personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByCardNumber(this.Security, this.CardID)
																);

				if (personGuid.IsEmpty()
				&& this.SiteManager.Site.UseShortCardNumber
				&& !this.Station.CardReader)
				{
				    this.CardID = Response;
					personGuid = FMChannelHelper.MakeCall<IPersonnel, Guid>(
																	 x =>
																	 x.GetGuidByShortCardNumber(this.Security, this.CardID)
																);
				}

				if (personGuid.IsEmpty())
				{
					if (this.Station.CardReader)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidCardNumberEvent(Response))
																);

						ErrorMessage = "Invalid Card Number";
					}
					else
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidDriverIDEvent(Response))
																);

						ErrorMessage = "Invalid ID";
					}

					return false;
				}

				// Initialize the load arms
			    this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);

			    this.ConsecutivePrompts = 0;
			    this.StationState = StationState.IDLE;


				if (this.Station.Type == STATION_TYPE.LOAD_RACK || this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					if (this.SiteManager.Site.InhibitMultipleCardIns
					&& this.SiteManager.CardedInAtLoadRack(personGuid))
					{
					    this.Driver = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, personGuid)
																);

						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.MultipleCardInAlarm)
																);

						ErrorMessage = "Multiple Card-in";
						return false;
					}
				}

			    this.Driver = FMChannelHelper.MakeCall<IPersonnel, PersonClass>(
																	 x =>
																	 x.Get(this.Security, personGuid)
																);

				// Check if Driver is Locked Out
				if (this.Driver.LockedOut)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.DriverLockedOutAlarm(Response))
																);

					ErrorMessage = "Driver Locked Out]";
					return false;
				}

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
				DateTimeOffset siteTimeToday = TimeConverter.ToDate(siteTimeNow);

				// Check if Driver Access Schedule precludes access
				if (this.SiteManager.Site.InhibitAccessAfterHours)
				{
					int ScheduleIndex = (int)siteTimeNow.DayOfWeek;
					if (!this.Driver.AccessScheduleCollection[ScheduleIndex].Enabled
					|| this.Driver.AccessScheduleCollection[ScheduleIndex].OpeningTime.Value.TimeOfDay > siteTimeNow.TimeOfDay
					|| this.Driver.AccessScheduleCollection[ScheduleIndex].ClosingTime.Value.TimeOfDay < siteTimeNow.TimeOfDay)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.AccessScheduleAlarm)
																);

						ErrorMessage = "Driver Access Not Scheduled";
						return false;
					}
				}

				if (this.Station.Type == STATION_TYPE.LOAD_RACK
				|| this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					if ( (this.Station.Type == STATION_TYPE.LOAD_RACK && !this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE)) 
                      || (this.Station.Type == STATION_TYPE.OFF_LOADING && !this.Driver.HasRole(PERSON_ROLE.OFFLOADER_ROLE)))

                    {
						ErrorMessage = "Must have Driver Role";
						return false;
					}

					// When driver is not carded in and there is an entry gate station
					if (this.SiteManager.Site.AccessCardInRequired
					&& !this.Driver.CardedIn
					&& this.SiteManager.AnyEntryGates)
					{
						ErrorMessage = "Must Card In at Entry Gate";
						return false;
					}

					if (this.SiteManager.EndOfDayState != StateEndOfDay.Inactive)
					{
						ErrorMessage = "Disabled due to End Of Day";
						return false;
					}

					if (this.SiteManager.EndOfMonthState != StateEndOfMonth.Inactive)
					{
						ErrorMessage = "Disabled due to End Of Month";
						return false;
					}

					if (this.Station.Type == STATION_TYPE.LOAD_RACK)
					{
						if (this.Station.IssueByVolumeTransactionAliasGuid.IsEmpty())
						{
							ErrorMessage = "Transaction Alias Invalid";
							return false;
						}
					}
					else if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						if (this.Station.ReceiptByVolumeTransactionAliasGuid.IsEmpty())
						{
							ErrorMessage = "Transaction Alias Invalid";
							return false;
						}
					}
				}

			    this.Manager = null;
			    this.Owner = null;
			    this.Shipper = null;
			    this.BillTo = null;
			    this.ShipTo = null;
			    this.Carrier = null;
			    this.TractorOrTanker = null;
			    this.Trailer1 = null;
			    this.Trailer2 = null;
			    this.Transaction = null;
			    this.Order = null;
			    this.PONumber = null;
			    this.LoadID = null;
			    this.ByWeight = false;
			    this.ByWeightProduct = "";
			    this.PendingTransactions.Clear();
			    this.LoadArmManagerCollection.ClearRecipeMap(this);
			    this.PIDXAuthorizationArray = null;
			    this.PIDXProfileCompanyMapCollection = null;

				if (!this.Driver.CompanyGuid.IsEmpty())
				{
				    this.Carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Driver.CompanyGuid)
																);
				}

				// Drivers must be assigned to a Carrier
				if (this.Carrier == null
				&& this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidCarrierEvent(this.Driver.ID))
																);

					ErrorMessage = "Invalid Carrier";
					return false;
				}

				if (this.Carrier != null)
				{
					if (this.Carrier.LockedOut)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Carrier.LockedOutAlarm)
																);

						ErrorMessage = CompanyRoleMapClass.RoleID(COMPANY_ROLE.CARRIER) + " Locked Out";
						return false;
					}

				    this.Carrier._LastActivityDate.Value = siteTimeNow;
					FMChannelHelper.MakeCall<ICompanies>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Carrier)
																);
				}

				if (this.StationState == StationState.IDLE)
				{
					// verify that the driver has the training and qualifications required to operate this piece of equipment
					// first check if the driver or station is setup with required access
					bool bQualifaicationsOK = false;
					bool bTrainingOK = false;

					// check the qualifications
					foreach (QualificationMapClass ReqQualification in this.Station.ReqQualificationsCollection)
					{
						bQualifaicationsOK = false;
						foreach (QualificationMapClass Qualification in this.Driver.QualificationCollection)
						{
							// check eack station qualification and if not accessed inform the driver
							if (ReqQualification.AssignedGuid == Qualification.AssignedGuid)
							{
								bQualifaicationsOK = true;
								break;
							}
						}

						if (bQualifaicationsOK == false)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.DriverNotQualifiedEvent(this.Driver.ID, ReqQualification.ID))
																);

							ErrorMessage = "Driver Not Qualified";
							return false;
						}
					}

					// check the training
					foreach (QualificationMapClass ReqTraining in this.Station.ReqTrainingCollection)
					{
						bTrainingOK = false;
						foreach (QualificationMapClass Qualification in this.Driver.TrainingCollection)
						{
							// check eack station qualification and if not accessed inform the driver
							if (ReqTraining.AssignedGuid == Qualification.AssignedGuid)
							{
								bTrainingOK = true;
								break;
							}
						}

						if (bTrainingOK == false)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.DriverNotTrainedEvent(this.Driver.ID, ReqTraining.ID))
																);

							ErrorMessage = "Driver Not Trained";
							return false;
						}
					}

					// Check for Driver Timed Out
					if (DateTimeOffset.Now - this.Driver._LastActivityDate.Value > new TimeSpan(this.SiteManager.Site._DriverTimeoutPeriod, 0, 0, 0, 0))
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Driver.DriverAccessTimedOutAlarm)
																);
						;
						ErrorMessage = "Driver Timeout";
						return false;
					}

					if (this.Station.Type == STATION_TYPE.LOAD_RACK
					|| this.Station.Type == STATION_TYPE.WEIGHT_SCALE
					|| this.Station.Type == STATION_TYPE.PRELOAD
					|| this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						if (this.Driver.CardedIn) this.TimeIn = this.Driver._LastActivityDate.Value;
						else this.TimeIn = siteTimeNow;
					}

				    this.Driver._LastActivityDate.Value = siteTimeNow;
					FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Driver)
																);

					// Check if Site AccessSchedule precludes access
					if (this.SiteManager.Site.InhibitAccessAfterHours
					&& !this.Driver.HasRole(PERSON_ROLE.SUPERVISOR_ROLE))
					{
						int Day = (int)siteTimeNow.Date.ToOADate();
						bool Holiday = false;
						foreach (ScheduleClass Schedule in this.SiteManager.Site.HolidayScheduleCollection)
						{
							if (Schedule.Day == Day)
							{
								Holiday = true;
								if (!Schedule.Enabled
									|| Schedule.OpeningTime.Value.TimeOfDay > siteTimeNow.TimeOfDay
									|| Schedule.ClosingTime.Value.TimeOfDay < siteTimeNow.TimeOfDay)
								{
									FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID))
																);

									ErrorMessage = "Terminal Access Not Scheduled";
									return false;
								}
								break;
							}
						}

						if (!Holiday)
						{
							int dayIndex = (int)siteTimeNow.DayOfWeek;
							if (!this.SiteManager.Site.OperatingScheduleCollection[dayIndex].Enabled
								|| this.SiteManager.Site.OperatingScheduleCollection[dayIndex].OpeningTime.Value.TimeOfDay > siteTimeNow.TimeOfDay
								|| this.SiteManager.Site.OperatingScheduleCollection[dayIndex].ClosingTime.Value.TimeOfDay < siteTimeNow.TimeOfDay)
							{
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID))
																);

								ErrorMessage = "Terminal Access Not Scheduled";
								return false;
							}
						}
					}

					CompanyClass Company = this.Carrier;
					COMPANY_ROLE Role = COMPANY_ROLE.CARRIER;
					if (Company == null || Company.IdentityGuid == Guid.Empty)
					{
						switch (Role)
						{
							case COMPANY_ROLE.MANAGER:
							case COMPANY_ROLE.OWNER:
							case COMPANY_ROLE.CUSTOMER_SHIPTO:
								ErrorMessage = CompanyRoleMapClass.RoleID(Role) + " Invalid";
								return false;

							default:
								return true;
						}
					}

					// Check if is Locked Out
					if (Company.LockedOut)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, Company.LockedOutAlarm)
																);

						ErrorMessage = CompanyRoleMapClass.RoleID(Role) + " Locked Out";
						return false;
					}

					// Check if Inactive
					if (!Company.Active)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, Company.InactiveAlarm));
						ErrorMessage = CompanyRoleMapClass.RoleID(Role) + " Inactive";
						return false;
					}

					// Check if Credit OK
					if (!Company.CreditOK)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, Company.CreditAlarm));
						ErrorMessage = CompanyRoleMapClass.RoleID(Role) + " Credit";
						return false;
					}

					// Check if Carrier AccessSchedule precludes access
					if (Role == COMPANY_ROLE.CARRIER
						&& this.SiteManager.Site.InhibitAccessAfterHours)
					{
						int dayIndex = (int)siteTimeNow.DayOfWeek;
						if (!this.Carrier.AccessScheduleCollection[dayIndex].Enabled
							|| this.Carrier.AccessScheduleCollection[dayIndex].OpeningTime.Value.TimeOfDay > siteTimeNow.TimeOfDay
							|| this.Carrier.AccessScheduleCollection[dayIndex].ClosingTime.Value.TimeOfDay < siteTimeNow.TimeOfDay)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Carrier.AccessScheduleAlarm));
							ErrorMessage = "Carrier Access Not Scheduled";
							return false;
						}
					}

					// Check Authorized Carrier
					if (Role == COMPANY_ROLE.CUSTOMER_SHIPTO)
					{
						if (this.Carrier != null)
						{
							bool Found = false;
							foreach (CompanyMapClass AuthorizedCarrier in Company.AuthorizedCarrierCollection)
							{
								if (AuthorizedCarrier.AssignedGuid == this.Carrier.IdentityGuid)
								{
									Found = true;
									break;
								}
							}

							if (!Found)
							{
								FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, Company.UnauthorizedCarrierAlarm));
								ErrorMessage = "Unauthorized Carrier";
								return false;
							}

						}

					}

					// Check for Expired Driver Qualifications
					foreach (QualificationMapClass Qualification in this.Driver.QualificationCollection)
					{
						if (Qualification.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, Qualification.PersonnelQualificationExpiredAlarm(this.Driver.ID));
							ErrorMessage = "Qualification Expired";
							return false;
						}

					}

					// check for expired traing requirements
					foreach (QualificationMapClass ReqTraining in this.Driver.TrainingCollection)
					{
						if (ReqTraining.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, ReqTraining.PersonnelTrainingExpiredAlarm(this.Driver.ID));
							ErrorMessage = "Training Expired";
							return false;
						}

					}

				}

				if (this.StationState == StationState.IDLE
					|| this.StationState == StationState.DRIVER_QUALIFICATION_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired Driver Training
					foreach (QualificationMapClass ReqTraining in this.Driver.TrainingCollection)
					{
						if (ReqTraining.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, ReqTraining.PersonnelTrainingExpiredAlarm(this.Driver.ID));
							ErrorMessage = "Driver Training Expired";
							return false;
						}

					}

				}

				if (this.StationState == StationState.IDLE
					|| this.StationState == StationState.DRIVER_TRAINING_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired Driver Licenses
					foreach (QualificationMapClass License in this.Driver.LicenseCollection)
					{
						if (License.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, License.PersonnelLicenseExpiredAlarm(this.Driver.ID));
							ErrorMessage = "Driver License Expired";
							return false;
						}

					}

				}

				if (this.StationState == StationState.IDLE
					|| this.StationState == StationState.DRIVER_LICENSE_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired Certificate/Permit
					if (this.Carrier != null)
					{
						foreach (QualificationMapClass CertificaitonOrPermit in this.Carrier.CertificateAndPermitCollection)
						{
							if (CertificaitonOrPermit.ExpirationDate.Value <= siteTimeToday)
							{
								this.AddAlarmAndEventLog(this.Security, CertificaitonOrPermit.CompanyCertificateOrPermitExpiredAlarm(this.Carrier.ID));
								ErrorMessage = "Cert/Perm Expired";
								return false;
							}

						}
					}

				}

				if (this.StationState == StationState.IDLE
					|| this.StationState == StationState.COMPANY_CERTIFICATE_OR_PERMIT_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired License
					if (this.Carrier != null
					&& this.Carrier.LicenseExpired)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Carrier.LicenseExpiredAlarm));
						ErrorMessage = "Carrier License Expired";
						return false;
					}

				}


				if (this.StationState == StationState.IDLE
					|| this.StationState == StationState.COMPANY_LICENSE_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired Insurance
					if (this.Carrier != null
					&& this.Carrier.InsuranceExpired)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Carrier.InsuranceExpiredAlarm));
						ErrorMessage = "Carrier Insurance Expired";
						return false;
					}

				}

			    this.LoadArmManagerCollection.ResetPreloads(this);

			}

			finally
			{
				timer.Stop();
			}
			return true;
		}

		private void AddAlarmAndEventLog(SecurityClass Security, AlarmAndEventLogClass alarmAndEventLogClass)
		{
			FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(Security, alarmAndEventLogClass)
																);
		}

		public void SetMessageTimeout(int MessageTimeoutValue)
		{
			ProcessVariableClass MessageTimeout = new ProcessVariableClass();
			MessageTimeout.URL = this.StationPv.URL;
			MessageTimeout.OPCItemID = this.StationPv.OPCItemID + ".Set Message Time-out";
			MessageTimeout.ServerValue = MessageTimeoutValue;
		    this.OPCServerManager.Write(MessageTimeout);
		}

		public string FormatContrecPrompt(string StockMessage, bool bUseAnswerPrompt)
		{
			string Prompt = "";
			int iLoop;
			int iNumberofCharacters = 0;
			bool bStart = false;
			bool bLookForSpace = false;
			bool bLastCharacterWasSpace = false;

			// we need to reformat the message. The Contrec supports eight lines at 30 characters and we use the '|' as the line seperator
			// we need to remove the [] from the message
			Prompt = "|Varec Terminal Automation|";
			if (bUseAnswerPrompt)
				Prompt += "|";
			for (iLoop = 0; iLoop < StockMessage.Length; iLoop++)
			{
				if (iNumberofCharacters > 20)
				{
					bLookForSpace = true;
				}
				if (bLookForSpace && StockMessage[iLoop] == ' ')
				{
					bLookForSpace = false;
					iNumberofCharacters = 0;
					Prompt += '|';
				}
				if (StockMessage[iLoop] == '|')
					bStart = true;
				else if (StockMessage[iLoop] == ']' || StockMessage[iLoop] == '[')
				{
					bStart = false;
					if (bLastCharacterWasSpace == false)
					{
						Prompt += " ";
						++iNumberofCharacters;
					}
					bLastCharacterWasSpace = true;
				}
				else if (bStart)
				{
					if (bLastCharacterWasSpace &&
						StockMessage[iLoop] == ' ')
					{
					}
					else
					{
						bLastCharacterWasSpace = false;
						Prompt += StockMessage[iLoop];
						++iNumberofCharacters;
					}
				}
			}

			return Prompt;
		}

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
		    this.SetMessageTimeout(messageTimeout);

			string Prompt = "";
			Prompt = this.FormatContrecPrompt(stockMessage, true);

			ArrayList Items = new ArrayList();

			ItemValue DisplayMessageItemValue = new ItemValue(this.StationPv.OPCItemID + ".Hidden Message Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

		    this.OPCServerManager.Write(new URL(this.StationPv.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));

		    this.ResponsePending = true;

		    this.SaveMessageValues(stockMessage, responseLength, messageTimeout);
		}

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool SaveForCancelProcessing)
		{
			string MessageToSend;

			// the contrec uses a different command for scanning touch keys. so based on the state will determin which command we need to use
			MessageToSend = "";
			if (this.StationState == StationState.DRIVER_MESSAGE_PROMPT)
			{
				MessageToSend = "|";
				for (int iLoop = 0; iLoop < stockMessage.Length; iLoop++)
				{
					MessageToSend += stockMessage[iLoop];
				}
			}
			else
			{
				MessageToSend = stockMessage;
			}


			ArrayList Items = new ArrayList();

		    this.SetMessageTimeout(messageTimeout);

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecPrompt(MessageToSend, true);
			ItemValue DisplayMessageItemValue = new ItemValue(this.StationPv.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

			try
			{
			    this.OPCServerManager.Write(new URL(this.StationPv.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Contrec 1010 StationManager DisplayMessage : " + e.Message, EventLogEntryType.Error);
			}

		    this.ResponsePending = true;

			if (SaveForCancelProcessing) this.SaveMessageValues(stockMessage, responseLength, messageTimeout);

			return 0;
		}

		public override void DisplayMenu(DisplayMenuParameters Parameters)
		{
			ArrayList Items = new ArrayList();

			// use the get answer prompt to get the response
			string Prompt = "";
			Prompt = this.FormatContrecMenuPrompt(Parameters);
			ItemValue DisplayMessageItemValue = new ItemValue(this.StationPv.OPCItemID + ".Get Answer Prompt");
			DisplayMessageItemValue.Value = Prompt;
			Items.Add(DisplayMessageItemValue);

			try
			{
			    this.OPCServerManager.Write(new URL(this.StationPv.URL), (ItemValue[])Items.ToArray(typeof(ItemValue)));
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Contrec 1010 StationManager DisplayMessage : " + e.Message, EventLogEntryType.Error);
			}

		    this.ResponsePending = true;

			if (Parameters.SaveForCancelProcessing) this.CurrentMenuParameters = Parameters;
		}

		public string FormatContrecMenuPrompt(DisplayMenuParameters Parameters)
		{
		    this.SetMessageTimeout(Parameters.MenuTimeout);

			// the menu can consist of a header and up to 6 menu options.
			string Message = "|Varec Terminal Automation|";
			int iMaximumLength = 0;
			Message += FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Station.SiteGuid, Parameters.Caption)
																);

			int iCommandNumber;

			// first determine the maximum length of the menus
			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(this.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ > iMaximumLength)
					iMaximumLength = Value.Length /*+ 4*/;
			}

			for (int nLoop = 0; nLoop < Parameters.Menu.Length; ++nLoop)
			{
				string Value = Parameters.Menu[nLoop];

				if (Parameters.ApplyDataDictionary)
					Value = this.GetDataDictionaryValueByKey(this.Station.SiteGuid, Value);

				if (Value.Length /*+ 4*/ < iMaximumLength)
				{
					for (int nLoop1 = 0; nLoop1 < (iMaximumLength - (Value.Length /*+ 4*/)); nLoop1++)
					{
						Value += " ";
					}
				}
				iCommandNumber = nLoop + 1;
				Message += '|';
				Message += iCommandNumber.ToString() + " - ";
				Message += Value;
			}

			return Message;
		}

		//public override void SendEndTransaction()
		//{

		//    ProcessVariableClass TerminateTransaction;

		//    TerminateTransaction = new ProcessVariableClass(
		//        PROCESS_VARIABLE_TYPE.STATION_PV,
		//        UNIT_TYPE.STATION_UNIT,
		//        VarEnum.VT_BOOL,
		//        false,
		//        StationPV.OPCItemID + ".Terminate Transaction",
		//        StationPV.URL,
		//        StationPV.ProgID);

		//    TerminateTransaction.ServerValue = true;
		//    try
		//    {
		//        OPCServerManager.Write(TerminateTransaction);
		//    }
		//    catch
		//    {
		//    }
		//}

		public void SendTransactionComplete()
		{

			ProcessVariableClass TerminateTransaction;

			TerminateTransaction = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.STATION_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BOOL,
				false,
				this.StationPv.OPCItemID + ".Transaction Complete",
				this.StationPv.URL,
				this.StationPv.ProgID);

			TerminateTransaction.ServerValue = true;
			try
			{
			    this.OPCServerManager.Write(TerminateTransaction);
			}
			catch
			{
			}
		}

		protected bool ProcessTruckIDContrec(string Response, out string ErrorMessage)
		{
			ErrorMessage = "";
			{
				Guid identityGuid;
				identityGuid = FMChannelHelper.MakeCall<IEquipments, Guid>(
																	 x =>
																	 x.GetIdentityGuidByCardNumberAndEquipmentID(this.Security, this.Carrier.MasterRecordGuid, Response)
																);

				if (identityGuid != Guid.Empty)
				    this.TractorOrTanker = FMChannelHelper.MakeCall<IEquipments, EquipmentClass>(
																	 x =>
																	 x.Get(this.Security, identityGuid)
																);

				// Invalid Tractor Number
				if (identityGuid == Guid.Empty
					|| this.TractorOrTanker == null
					|| (this.TractorOrTanker.Type != EQUIPMENT_TYPE.TANKER_TYPE
					&& this.TractorOrTanker.Type != EQUIPMENT_TYPE.TRACTOR_TYPE))
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidTractorOfTankerIDEvent(Response))
																);

					ErrorMessage = "Invalid Truck Card";
					return false;
				}

				// check if this equipment can be used at this station
				bool bTestAndInspectionsOK = false;
				foreach (QualificationMapClass ReqTestOrInspection in this.Station.ReqTestsandInspectionsCollection)
				{
					bTestAndInspectionsOK = false;
					foreach (QualificationMapClass TestOrInspection in this.TractorOrTanker.TestAndInspectionCollection)
					{
						if (ReqTestOrInspection.AssignedGuid == TestOrInspection.AssignedGuid)
						{
							bTestAndInspectionsOK = true;
							break;
						}
					}

					if (bTestAndInspectionsOK == false)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.EquipmentNotAuthorizedEvent(this.TractorOrTanker.ID, ReqTestOrInspection.ID))
																);

						ErrorMessage = "Equipment Not Authorized";
						return false;
					}
				}


                //check if the station required equipment tags or licenses are attached
                if (this.IsEquipmentLicenseValidForThisStation(this.TractorOrTanker) == false)
                {
                    return false;
                }

                // check the qualifications and training required for this piece of equipment
                EquipmentTypeClass equipmentType = FMChannelHelper.MakeCall<IEquipmentTypes, EquipmentTypeClass>(
																	 x =>
																	 x.Get(this.Security, this.TractorOrTanker.EquipmentTypeGuid)
																);


				if (this.Driver.QualificationCollection.Count == 0 &&
					equipmentType.ReqQualificationsCollection.Count > 0)
				{
					ErrorMessage = "Driver Not Qualified";
					return false;
				}
				else if (this.Driver.TrainingCollection.Count == 0 &&
					equipmentType.ReqTrainingCollection.Count > 0)
				{
					ErrorMessage = "Driver Not Trained";
					return false;
				}
				else if (equipmentType.ReqQualificationsCollection.Count > 0 ||
							equipmentType.ReqTrainingCollection.Count > 0)
				{
					bool bQualifaicationsOK = false;
					bool bTrainingOK = false;
					// check the qualifications
					foreach (QualificationMapClass ReqQualification in equipmentType.ReqQualificationsCollection)
					{
						bQualifaicationsOK = false;
						foreach (QualificationMapClass Qualification in this.Driver.QualificationCollection)
						{
							// check eack station qualification and if not accessed inform the driver
							if (ReqQualification.AssignedGuid == Qualification.AssignedGuid)
							{
								bQualifaicationsOK = true;
							}
						}
						if (bQualifaicationsOK == false)
						{
							ErrorMessage = "Driver Not Qualified";
							return false;
						}
					}
					// check the training
					foreach (QualificationMapClass ReqQualification in equipmentType.ReqTrainingCollection)
					{
						bTrainingOK = false;
						foreach (QualificationMapClass Qualification in this.Driver.TrainingCollection)
						{
							// check eack station qualification and if not accessed inform the driver
							if (ReqQualification.AssignedGuid == Qualification.AssignedGuid)
							{
								bTrainingOK = true;
							}
						}
						if (bTrainingOK == false)
						{
							ErrorMessage = "Driver Not Trained";
							return false;
						}
					}
				}

				if (this.SiteManager.Site.EnforceDriverEquipmentMatch
					&& this.TractorOrTanker.CompanyGuid != this.Carrier.MasterRecordGuid)
				{
					ErrorMessage = "Invalid Truck Card";
					return false;
				}

				DateTimeOffset siteTimeToday = TimeConverter.Today(this.SiteManager.Site);

				if (this.StationState == StationState.IDLE)
				{
					// Check if Tractor/Tanker is Locked Out
					if (this.TractorOrTanker.LockedOut)
					{
						FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.TractorOrTanker.LockedOutAlarm)
																);

						ErrorMessage = "Tractor/Tanker Locked Out";
						return false;
					}

					// Check for Expired Tag/License
					foreach (QualificationMapClass TagOrLicense in this.TractorOrTanker.TagAndLicenseCollection)
					{
						if (TagOrLicense.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, TagOrLicense.EquipmentTagOrLicenseExpiredAlarm(this.TractorOrTanker.ID));
							ErrorMessage = "Tag/License Expired";
							return false;
						}

					}
                }

				if (this.StationState == StationState.IDLE
				|| this.StationState == StationState.TRACTOR_TAG_OR_LICENSE_WARNING)
				{
				    this.StationState = StationState.IDLE;

					// Check for Expired Test/Inspection
					foreach (QualificationMapClass TestOrInspection in this.TractorOrTanker.TestAndInspectionCollection)
					{
						if (TestOrInspection.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLog(this.Security, TestOrInspection.EquipmentTestOrInspectionExpiredAlarm(this.TractorOrTanker.ID));
							ErrorMessage = "Test/Insp Expired";
							return false;
						}
					}
				}

				if (this.Station.Type == STATION_TYPE.LOAD_RACK) this.IssueLoadIDPrompt();
				else this.IssueOffLoadIDPrompt();
			}
			return true;
		}

        public override void ResetStationDevice()
		{
			try
			{
				if (this.DeferredPurge
				|| this.DeferredAdd) this.SiteManager.PermissiveEvent.Set();

			    this.SendEndTransaction();
			    this.StationState = StationState.IDLE;
			    this.Driver = null;

			    this.RemoteAuthorized = false;
			    this.PreloadDataSet = null;
			    this.ConsecutivePrompts = 0;

			    this.CurrentMenuParameters = null;
			    this.PriorStockMessage = "";
			    this.PriorResponseLength = 0;
			    this.PriorMessageTimeout = 0;

			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("StationManager ResetStationDevice : " + e.Message, EventLogEntryType.Error);
			}
		}

        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public override void ReadLineItemData(
			LineItemDO lineItem,
			Server server,
			LoadArmManagerClass loadArmManager)
		{
			Contrec1010RaLoadArmManagerClass contrec1010LoadArmManager = loadArmManager as Contrec1010RaLoadArmManagerClass;
			if (contrec1010LoadArmManager == null)
				throw new Exception("ReadLineItemData : Invalid LoadArmManager");

			ItemValueResult grossVolume;
			ItemValueResult netVolume;
			ItemValueResult averageTemperature;
			ItemValueResult averageDensity;

			ItemValueResult[] nonResettableTotal;

			contrec1010LoadArmManager.ReadNonResettableTotals(
				server,
				out nonResettableTotal);


			SiteClass site = this.SiteManager.Site;
			DateTimeOffset siteTimeNow = TimeConverter.Now(site);

			ProcessVariableClass pv = new ProcessVariableClass();

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				ProductMapClass component = loadArmManager.GetComponent(lineItem.ProductGuid);
				if (component == null)
					throw new Exception("Component not found in LoadArm Configuration");

				if (lineItem.Quantity == null)
					lineItem.Quantity = new QuantityDO();

				contrec1010LoadArmManager.ReadComponentData(
					server,
					out grossVolume,
					out netVolume,
					out averageTemperature,
					out averageDensity);

				if (grossVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadGrossQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadGrossQualityLogged = true;
					}
				}
				else
				{
					pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV;
					pv.ServerUnits = this.SiteManager.Site.VolumeUnits;
					pv.ServerValue = System.Convert.ToDouble(grossVolume.Value);
					if (this.Station.Type == STATION_TYPE.OFF_LOADING)
						lineItem.Quantity.GrossInventoryChange = ((double)pv.GetValue(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces));
					else
						lineItem.Quantity.GrossInventoryChange = -((double)pv.GetValue(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces));
					lineItem.Quantity.BadGrossQualityLogged = false;
				}

				if (netVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadNetQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Net Volume OPC Quality Bad " + netVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadNetQualityLogged = true;
					}
				}
				else
				{
					pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.NET_VOLUME_PV;
					pv.ServerUnits = this.SiteManager.Site.VolumeUnits;
					pv.ServerValue = System.Convert.ToDouble(netVolume.Value);
					if (this.Station.Type == STATION_TYPE.OFF_LOADING)
						lineItem.Quantity.NetInventoryChange = ((double)pv.GetValue(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces));
					else
						lineItem.Quantity.NetInventoryChange = -((double)pv.GetValue(lineItem.VolumeUnits, lineItem.VolumeDecimalPlaces));
					lineItem.Quantity.BadNetQualityLogged = false;
				}

				if (lineItem.Temperature == null)
					lineItem.Temperature = 0.0;

				if (averageTemperature.Quality != Quality.Good)
				{
					if (!lineItem.Temperature_BadQualityLogged)
					{
						// the contrec will not set this value until the batch is complete so we need to ignore it at this point
						lineItem.Temperature_BadQualityLogged = true;
					}
				}
				else
				{
					pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.TEMPERATURE_PV;
					pv.ServerUnits = this.SiteManager.Site.TemperatureUnits;
					if (this.SiteManager.Site.TemperatureUnits == EngineeringUnit.FmtDegF)
						pv.ServerValue = System.Convert.ToDouble(averageTemperature.Value) / 10;
					else
						pv.ServerValue = System.Convert.ToDouble(averageTemperature.Value) / 100;
					lineItem.Temperature = (double)pv.GetValue(site.TemperatureUnits, site._TemperatureDecimalPlaces);
					lineItem.Temperature_BadQualityLogged = false;
				}


				if (!lineItem.Quantity.BadGrossQualityLogged
					&& lineItem.Quantity.GrossInventoryChange != 0
					&& !lineItem.Quantity.BadNetQualityLogged
					&& lineItem.Quantity.NetInventoryChange != 0)
					lineItem.VCF = Math.Round(lineItem.Quantity.NetInventoryChange / lineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
				else
					lineItem.VCF = null;

				if (lineItem.Density == null)
					lineItem.Density = 0.0;

				if (averageDensity.Quality != Quality.Good)
				{
					if (!lineItem.Density_BadQualityLogged)
					{
						// the contrec will not set this value until the batch is complete so we need to ignore it at this point
						lineItem.Density_BadQualityLogged = true;
					}
				}
				else
				{

					pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV;
					pv.ServerUnits = this.SiteManager.Site.DensityUnits;

					// Presently the system expects that the Preset Units will match the Site Units
					double scale = 10;
					if (pv.ServerUnits == EngineeringUnit.FmdDegApi)
						scale = 10;
					else if (pv.ServerUnits == EngineeringUnit.FmdGcm3)
						scale = 10000;

					if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					{
						double Density = 0.0;
						pv.ServerValue = Density;
						lineItem.Density = (double)pv.GetValue(site.DensityUnits, site._DensityDecimalPlaces);
						lineItem.Density_BadQualityLogged = false;
					}
					else
					{
						pv.ServerValue = System.Convert.ToDouble(averageDensity.Value) / scale;
						lineItem.Density = (double)pv.GetValue(site.DensityUnits, site._DensityDecimalPlaces);
						lineItem.Density_BadQualityLogged = false;
					}
				}


				if (nonResettableTotal[0].Quality != Quality.Good)
				{
					if (lineItem.MeterReading.MeterStop == null)
						lineItem.MeterReading.MeterStop = 0.0;

					if (!lineItem.MeterReading.MeterStop_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[0].ItemName, EventLogEntryType.Error);
						lineItem.MeterReading.MeterStop_BadQualityLogged = true;
					}
				}
				else
				{
					if (lineItem.MeterReading.MeterStart == null)
					{
						lineItem.MeterReading.MeterStart = component.MeterValue;
						lineItem.MeterReading.StartDateTime = siteTimeNow;
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}

					if (lineItem.MeterReading.MeterStop != null && lineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[0].Value))
					{
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}
					lineItem.MeterReading.MeterStop_BadQualityLogged = false;

					if (component.MeterValue != lineItem.MeterReading.MeterStop.Value)
					{
						component.MeterValue = lineItem.MeterReading.MeterStop.Value;
					    this.LastActivityDateTime = DateTimeOffset.Now;
					}
				}
			}

            int compartmentNumber = contrec1010LoadArmManager.GetCompartmentNumber();

			lineItem.DestinationCompartmentID = System.Convert.ToString(compartmentNumber);

			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				if (subLineItem.Status != TransactionStatus.InProgress)
					continue;

				if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					ProductMapClass component = loadArmManager.GetComponent(subLineItem.ProductGuid);
					if (component == null)
						throw new Exception("Component not found in LoadArm Configuration");

					if (subLineItem.Quantity == null)
						subLineItem.Quantity = new QuantityDO();

					contrec1010LoadArmManager.ReadComponentData(
						server,
						out grossVolume,
						out netVolume,
						out averageTemperature,
						out averageDensity);

					if (grossVolume.Quality != Quality.Good)
					{
						if (!subLineItem.Quantity.BadGrossQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
							subLineItem.Quantity.BadGrossQualityLogged = true;
						}
					}
					else
					{
						pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV;
						pv.ServerUnits = this.SiteManager.Site.VolumeUnits;
						pv.ServerValue = System.Convert.ToDouble(grossVolume.Value);
						subLineItem.Quantity.GrossInventoryChange = -((double)pv.GetValue(subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces));
						subLineItem.Quantity.BadGrossQualityLogged = false;
					}

					if (netVolume.Quality != Quality.Good)
					{
						if (!subLineItem.Quantity.BadNetQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Net Volume OPC Quality Bad " + netVolume.ItemName, EventLogEntryType.Error);
							subLineItem.Quantity.BadNetQualityLogged = true;
						}
					}
					else
					{
						pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.NET_VOLUME_PV;
						pv.ServerUnits = this.SiteManager.Site.VolumeUnits;
						pv.ServerValue = System.Convert.ToDouble(netVolume.Value);
						subLineItem.Quantity.NetInventoryChange = -((double)pv.GetValue(subLineItem.VolumeUnits, subLineItem.VolumeDecimalPlaces));
						subLineItem.Quantity.BadNetQualityLogged = false;
					}

					if (subLineItem.Temperature == null)
						subLineItem.Temperature = 0.0;

					if (averageTemperature.Quality != Quality.Good)
					{
						if (!subLineItem.Temperature_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Temperature OPC Quality Bad " + averageTemperature.ItemName, EventLogEntryType.Error);
							subLineItem.Temperature_BadQualityLogged = true;
						}
					}
					else
					{
						pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.TEMPERATURE_PV;
						pv.ServerUnits = this.SiteManager.Site.TemperatureUnits;
						pv.ServerValue = System.Convert.ToDouble(averageTemperature.Value) / 10;
						subLineItem.Temperature = (double)pv.GetValue(site.TemperatureUnits, site._TemperatureDecimalPlaces);
						subLineItem.Temperature_BadQualityLogged = false;
					}

					if (!subLineItem.Quantity.BadGrossQualityLogged
					&& subLineItem.Quantity.GrossInventoryChange != 0
					&& !subLineItem.Quantity.BadNetQualityLogged
					&& subLineItem.Quantity.NetInventoryChange != 0)
						subLineItem.VCF = Math.Round(subLineItem.Quantity.NetInventoryChange / subLineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);

					else
						subLineItem.VCF = null;

					if (subLineItem.Density == null)
						subLineItem.Density = 0.0;

					if (averageDensity.Quality != Quality.Good)
					{
						if (!subLineItem.Density_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Density OPC Quality Bad " + averageDensity.ItemName, EventLogEntryType.Error);
							subLineItem.Density_BadQualityLogged = true;
						}
					}
					else
					{
						pv.ProcessVariableType = PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV;
						pv.ServerUnits = this.SiteManager.Site.DensityUnits;

						// Presently the system expects that the Preset Units will match the Site Units
						double scale = 10;
						if (pv.ServerUnits == EngineeringUnit.FmdDegApi)
							scale = 10;
						else if (pv.ServerUnits == EngineeringUnit.FmdGcm3)
							scale = 10000;

						pv.ServerValue = System.Convert.ToDouble(averageDensity.Value) / scale;

						subLineItem.Density = (double)pv.GetValue(site.DensityUnits, site._DensityDecimalPlaces);
						subLineItem.Density_BadQualityLogged = false;
					}


					if (nonResettableTotal[0].Quality != Quality.Good)
					{
						if (subLineItem.MeterReading.MeterStop == null)
							subLineItem.MeterReading.MeterStop = 0.0;

						if (!subLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[0], EventLogEntryType.Error);
							subLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						if (subLineItem.MeterReading.MeterStart == null)
						{
							subLineItem.MeterReading.MeterStart = component.MeterValue;
							subLineItem.MeterReading.StartDateTime = siteTimeNow;
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}

						if (subLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[0].Value))
						{
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (component.MeterValue != subLineItem.MeterReading.MeterStop.Value)
						{
							component.MeterValue = subLineItem.MeterReading.MeterStop.Value;
						    this.LastActivityDateTime = DateTimeOffset.Now;
						}
					}
				}

				else if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(subLineItem.ProductGuid);
					if (additiveInjector == null)
						throw new Exception("Additive not found in LoadArm Configuration");

					if (subLineItem.Quantity == null)
						subLineItem.Quantity = new QuantityDO();

					if (nonResettableTotal[additiveInjector.PresetNumber].Quality != Quality.Good)
					{
						if (subLineItem.MeterReading.MeterStop == null)
							subLineItem.MeterReading.MeterStop = 0.0;

						if (!subLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[additiveInjector.PresetNumber], EventLogEntryType.Error);
							subLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						if (subLineItem.MeterReading.MeterStart == null)
						{
							subLineItem.MeterReading.MeterStart = additiveInjector.MeterValue;
							subLineItem.MeterReading.StartDateTime = siteTimeNow;
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[additiveInjector.PresetNumber].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}

						if (subLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[additiveInjector.PresetNumber].Value))
						{
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[additiveInjector.PresetNumber].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (additiveInjector.MeterValue != subLineItem.MeterReading.MeterStop.Value)
						{
							additiveInjector.MeterValue = subLineItem.MeterReading.MeterStop.Value;
						    this.LastActivityDateTime = DateTimeOffset.Now;
						}

						subLineItem.Quantity.GrossInventoryChange = subLineItem.MeterReading.MeterStop.Value - subLineItem.MeterReading.MeterStart.Value;
						if (subLineItem.Quantity.GrossInventoryChange < 0)
							subLineItem.Quantity.GrossInventoryChange += 999999999.0;

						// Acquire Density, Temperature, & VCF from the 
						// tank and compute?
						TankClass tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
						if (tank == null)
							this.eventLog.WriteEntry("ReadLineItemData : No Additive Tank", EventLogEntryType.Error);
						else
						{
							if (subLineItem.Temperature == null)
							{
								subLineItem.Temperature = 0.0;
							}

							pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV];
							if (pv == null)
							{
								if (!subLineItem.Temperature_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank Temperature Process Variable", EventLogEntryType.Error);
									subLineItem.Temperature_BadQualityLogged = true;
								}
							}
							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !pv.IsQualityGood)
							|| !(pv.SIValue is double))
							{
								if (!subLineItem.Temperature_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Temperature OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Temperature_BadQualityLogged = true;
								}
							}
							else
							{
								subLineItem.Temperature = (double)pv.GetValue(site.TemperatureUnits, site._TemperatureDecimalPlaces);
								subLineItem.Temperature_BadQualityLogged = false;
							}

							if (subLineItem.Density == null)
								subLineItem.Density = 0.0;

							pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.DENSITY_PV];
							if (pv == null)
							{
								if (!subLineItem.Density_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank Density Process Variable", EventLogEntryType.Error);
									subLineItem.Density_BadQualityLogged = true;
								}
							}

							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !pv.IsQualityGood)
							|| !(pv.SIValue is double))
							{
								if (!subLineItem.Density_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Density OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Density_BadQualityLogged = true;
								}
							}
							else
							{
								subLineItem.Density = (double)pv.GetValue(site.DensityUnits, site._DensityDecimalPlaces);
								subLineItem.Density_BadQualityLogged = false;
							}

							if (subLineItem.Quantity == null)
							{
								subLineItem.Quantity = new QuantityDO();
							}

							pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VCF_PV];
							if (pv == null)
							{
								if (!subLineItem.Quantity.BadNetQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank VCF Process Variable", EventLogEntryType.Error);
									subLineItem.Quantity.BadNetQualityLogged = true;
								}
							}
							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !pv.IsQualityGood)
							|| !(pv.SIValue is double))
							{
								if (!subLineItem.Quantity.BadNetQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive VCF OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Quantity.BadNetQualityLogged = true;
								}
							}
							else
							{
								subLineItem.Quantity.NetInventoryChange = subLineItem.Quantity.GrossInventoryChange * (double)pv.GetValue(EngineeringUnit.FmduPCent, 5);
								subLineItem.Quantity.BadNetQualityLogged = false;
							}
						}
					}
				}
			}

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct)) this.RollUpSplashBlendTotals(lineItem);
		}

		public new void DownloadConfigurationData()
		{

			// get the configured personnel and truck pin numbers from the card long entry
			// moxumum data to be downloaded is 500 entries
			PersonCollectionClass PersonCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			EquipmentCollectionClass EquipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			int iLoop = 0;

			ArrayList PersonPinNumbers = new ArrayList();
			ArrayList EquipmentPinNumbers = new ArrayList();

			foreach (PersonClass Person in PersonCollection)
			{
				if (Person.CardNumber.Length > 0)
					PersonPinNumbers.Add(Person.CardNumber);
				if (PersonPinNumbers.Count >= 500)
					break;
			}

			foreach (EquipmentClass Equipment in EquipmentCollection)
			{
				if (Equipment.TruckCardNumber.Length > 0)
					EquipmentPinNumbers.Add(Equipment.TruckCardNumber);
				if (EquipmentPinNumbers.Count >= 500)
					break;
			}

			// this is a four part evolution

			ProcessVariableClass ContrecSetPinNumbersPV;
			string PinNumberString = "";

			ContrecSetPinNumbersPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.STATION_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID + ".Set Pin Numbers",
				this.StationPv.URL,
				this.StationPv.ProgID);

			// first delete all of the person pin numbers
			PinNumberString = "DR/XX";

			ContrecSetPinNumbersPV.ServerValue = PinNumberString;
			try
			{
			    this.OPCServerManager.Write(ContrecSetPinNumbersPV);               
            }
			catch
			{
			}

			// now delete all of the equipment pin numbers
			PinNumberString = "TR/XX";

			ContrecSetPinNumbersPV.ServerValue = PinNumberString;
			try
			{
			    this.OPCServerManager.Write(ContrecSetPinNumbersPV);
			}
			catch
			{
			}
			// if this station is setup for a touch key then all the entries must be zero filled to a length of 12
			// if not then all entries must be zero filled to a length of 4
			// setup and download the personnel pins
			string PinNumberTemp;
			int RequiredLength = 4;
			int ValidPins = 0;
			int InvalidValidPins = 0;

			if (this.Station.TouchKeyReader == true)
				RequiredLength = 12;

			if (PersonPinNumbers.Count > 0)
			{
				for (iLoop = 0; iLoop < PersonPinNumbers.Count; iLoop++)
				{
					PinNumberTemp = PersonPinNumbers[iLoop].ToString().PadLeft(RequiredLength, '0');
					PinNumberTemp = PinNumberTemp.RemoveSpaces();

					if (PinNumberTemp.Length > RequiredLength)
					{
						++InvalidValidPins;
						continue;
					}

					PinNumberString = "DR/";
					PinNumberString += (iLoop + 1).ToString("D3");
					PinNumberString += "/";
					PinNumberString += PinNumberTemp;

					++ValidPins;

					ContrecSetPinNumbersPV.ServerValue = PinNumberString;
					try
					{
					    this.OPCServerManager.Write(ContrecSetPinNumbersPV);
					}
					catch
					{
					}
				}
			}
			// write the data to the event log for a record
			this.eventLog.WriteEntry("Downloaded " + PersonPinNumbers.Count.ToString() + " Person(s) On Station " + this.Station.ID + " Valid = " + ValidPins.ToString() + " InValid = " + InvalidValidPins.ToString());
			// do the equipment
			ValidPins = 0;
			InvalidValidPins = 0;
			if (EquipmentPinNumbers.Count > 0)
			{
				for (iLoop = 0; iLoop < EquipmentPinNumbers.Count; iLoop++)
				{
					PinNumberTemp = EquipmentPinNumbers[iLoop].ToString().PadLeft(RequiredLength, '0');
					PinNumberTemp = PinNumberTemp.RemoveSpaces();

					if (PinNumberTemp.Length > RequiredLength)
					{
						++InvalidValidPins;
						continue;
					}

					PinNumberString = "TR/";
					PinNumberString += (iLoop + 1).ToString("D3");
					PinNumberString += "/";
					PinNumberString += PinNumberTemp;

					++ValidPins;

					ContrecSetPinNumbersPV.ServerValue = PinNumberString;
					try
					{
					    this.OPCServerManager.Write(ContrecSetPinNumbersPV);
					}
					catch
					{
					}
				}
			}
			this.eventLog.WriteEntry("Downloaded " + EquipmentPinNumbers.Count.ToString() + " Equipment(s) On Station " + this.Station.ID + " Valid = " + ValidPins.ToString() + " InValid = " + InvalidValidPins.ToString());
		}

		protected new void ProcessLoadID(string Response)
		{
			if (Response == EscapeString)
			{
			    this.SendEndTransaction();
			}
			else
			{
				Guid identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(this.Security, Response)
																);

				// Invalid CompanyMap ID
				if (identityGuid == Guid.Empty)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidCustomerNumberEvent(Response))
																);

				    this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
					    this.StationState = StationState.RESET_ON_TIMEOUT;
					    this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					    this.ConsecutivePrompts = 0;
						return;
					}

				    this.StationState = StationState.LOADID_PROMPT;
				    this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.ValidateLoadID(identityGuid))
					return;

			    this.IssueShipToMenu(false);
			}
		}

		protected override void ProcessOffLoadID(string Response)
		{
			if (Response == EscapeString)
			{
			    this.SendEndTransaction();
			}
			else
			{
				Guid identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetOffLoadIdentityGuidByMapID(this.Security, Response)
																);
				// Invalid CompanyMap ID
				if (identityGuid == Guid.Empty)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.InvalidCustomerNumberEvent(Response))
																);

				    this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
					    this.StationState = StationState.RESET_ON_TIMEOUT;
					    this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					    this.ConsecutivePrompts = 0;
						return;
					}

				    this.StationState = StationState.OFFLOADID_PROMPT;
				    this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
					return;
				}

				if (!this.ValidateOffLoadID(identityGuid))
					return;

			    this.IssueVerifySupplierMenu();

			}
		}

		//protected override void ProcessShipTo(string Response)
		//{
		//    DataDictionariesClass DataDictionaries = new DataDictionariesClass();

		//    if (Response == "1")
		//        Response = dict.DataDictionariesClass_Get(SiteManager.Site.SiteIndex, CurrentMenuParameters.Menu[0]);

		//    if (Response == "2")
		//        Response = dict.DataDictionariesClass_Get(SiteManager.Site.SiteIndex, CurrentMenuParameters.Menu[1]);

		//    if (Response == EscapeString ||
		//        dict.DataDictionariesClass_Get(SiteManager.Site.SiteIndex, "LoadRack|No") == Response)
		//    {
		//        if (SiteManager.Site.PromptForShipmentNumber)
		//            IssueEnterShipmentNumberPrompt();
		//        else
		//        {
		//            if (Station.Type == STATION_TYPE.LOAD_RACK)
		//                IssueLoadIDPrompt();
		//            else
		//                IssueOffLoadIDPrompt();
		//        }
		//        return;
		//    }

		//    if (dict.DataDictionariesClass_Get(SiteManager.Site.SiteIndex, "LoadRack|Yes") == Response)
		//    {
		//        CompanyMapsClass CompanyMaps = new CompanyMapsClass();

		//        int Index = CompanyMaps.GetIndexByMapID(Security, LoadID);

		//        if (!ValidateLoadID(Index))
		//            return;

		//        if (Station.Type == STATION_TYPE.LOAD_RACK)
		//            SetProductsInStation();

		//        IssueEnterPurchaseOrderPrompt();
		//    }

		//    else if (dict.DataDictionariesClass_Get(SiteManager.Site.SiteIndex, "LoadRack|No") == Response)
		//    {
		//        if (Station.Type == STATION_TYPE.LOAD_RACK)
		//            IssueLoadIDPrompt();
		//        else
		//            IssueOffLoadIDPrompt();
		//    }
		//    else
		//    {
		//        DisplayMessage("[LoadRack|Invalid Selection]", null, 0, MESSAGE_TIMEOUT);
		//        StationState = STATION_STATE.INVALID_SHIPTO_PROMPT_RESPONSE_MESSAGE;
		//    }
		//}

		public void CheckandResetContrecTimeDate()
		{
			DateTimeOffset CurrentDateTime = DateTimeOffset.Now;
			ProcessVariableClass ContrecWriteTimePV;
			ContrecWriteTimePV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				false,
				this.StationPv.OPCItemID + ".Reset Date Time",
				this.StationPv.URL,
				this.StationPv.ProgID);

			// format the date time into the contrec format which is
			// ddmmyyyyhhmmss

			string ContrecDateTimeString = CurrentDateTime.Day.ToString("D2") + CurrentDateTime.Month.ToString("D2") + CurrentDateTime.Year.ToString("D4");
			ContrecDateTimeString += CurrentDateTime.Hour.ToString("D2") + CurrentDateTime.Minute.ToString("D2") + CurrentDateTime.Second.ToString("D2");

			ContrecWriteTimePV.ServerValue = ContrecDateTimeString;
			try
			{
			    this.OPCServerManager.Write(ContrecWriteTimePV);
			}
			catch
			{
			}
		}

		protected void SetObjectOperationalVariables()
		{
			// this routine will check the Contrec to ensure it is setup properly and set the internal variables for operation.
			// detrmine the number of arms configured
			Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Status.First Arm Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Status.Number of Arms")),
								new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Status.Last Transaction Number")),};



			ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);
			if (Values[0].Quality == Quality.Good)
			{
				try
				{
				    this.FirstArmConfigured = System.Convert.ToInt32(Values[0].Value.ToString());
				}
				catch
				{
					this.eventLog.WriteEntry("Failed to Read Station " + this.Station.ID + " First Arm. Defaulting to Arm 1");
				    this.FirstArmConfigured = 1;
				}
			}
			else
				this.eventLog.WriteEntry("Failed to Read Station " + this.Station.ID);
			if (Values[1].Quality == Quality.Good)
			{
				try
				{
				    this.NumberOfArmsConfigured = System.Convert.ToInt32(Values[1].Value.ToString());
				}
				catch
				{
					this.eventLog.WriteEntry("Failed to Read Station " + this.Station.ID + " Comfigured Arms. Defaulting to 1");
				    this.NumberOfArmsConfigured = 1;
				}
			}
			else
				this.eventLog.WriteEntry("Failed to Read Station " + this.Station.ID);

			if (this.NumberOfArmsConfigured != 1)
				this.eventLog.WriteEntry("Station " + this.Station.ID + " Comfigured Arms is > 1. Only the First Arm is Supported");

			// determine the last transaction number in the Contrec
			if (Values[2].Quality == Quality.Good)
			{
				try
				{
				    this.ContrecLastTransactionNumber = System.Convert.ToInt32(Values[2].Value.ToString());
					// check if we need to upload the data
					if (this.ContrecLastTransactionNumber > this.Station.LastTransactionNumber) this.UploadStoredTransactionsEvent.Set();
				}
				catch
				{
				}
			}
		}

		public void SetTransactionNumber()
		{
			// read the transaction number for the last batch and store it

			Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Status.Last Transaction Number")),
								};

			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);

			if (Values[0].Quality == Quality.Good)
			{
				try
				{
				    this.ContrecLastTransactionNumber = System.Convert.ToInt32(Values[0].Value.ToString());
				}
				catch
				{
				}
				// store the new value back to the station table
				this.Station.LastTransactionNumber = this.ContrecLastTransactionNumber;
				this.Station.LastTransactionNumberDateTime = DateTimeOffset.Now;
				FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Modify(this.Security, this.Station)
																);
			}
		}

		public new void CheckAndUploadStoreTransactions()
		{
			// this routine will upload a stored transaction from the Contrec and map what it can to a transaction in the database
			int iLoop = 0;
			ProcessVariableClass ContrecWriteTransNumberPV;
			string TransDriverIndex = "";
			string TransTruckIndex = "";
			string TransTransactionNumber = "";
			string TransEntryStart = "";
			string TransEntryStop = "";
			string TransStartTime = "";
			string TransStopTime = "";
			string TransDate = "";
			string TransFirstArmNumber = "";
			string TransLoadID = "";

			// ContrecLastTransactionNumber
			// check if the contrec number is greater then our database stored number
			if (this.ContrecLastTransactionNumber <= this.Station.LastTransactionNumber)
				return;

			ContrecWriteTransNumberPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_I4,
				false,
				this.StationPv.OPCItemID + ".Stored Transactions.Set Transaction Number",
				this.StationPv.URL,
				this.StationPv.ProgID);

			Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Driver Index")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Truck Index")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Transaction Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Entry Start")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Entry Stop")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Start Time")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Stop Time")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Date")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.First Arm Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Load Number")),
							};

			// we do this here so it is only done once
			PersonCollectionClass PersonCollection = FMChannelHelper.MakeCall<IPersonnel, PersonCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			EquipmentCollectionClass EquipmentCollection = FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			for (iLoop = (this.Station.LastTransactionNumber + 1); iLoop <= this.ContrecLastTransactionNumber; iLoop++)
			{
				// set the transaction number in the Contrec

				ContrecWriteTransNumberPV.ServerValue = iLoop;
				try
				{
				    this.OPCServerManager.Write(ContrecWriteTransNumberPV);
				}
				catch
				{
					this.eventLog.WriteEntry("Failed to Set Station " + this.Station.ID + " Stored Transaction Number " + iLoop.ToString() + ". Stored Transaction Cannot be Uploaded");
					return;
				}
				// read the transaction data and store it
				foreach (Item item in Items)
					item.MaxAgeSpecified = true;
				((Item)Items[0]).MaxAgeSpecified = false;

				ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);

				if (Values[0].Quality != Quality.Good ||
					Values[1].Quality != Quality.Good ||
					Values[2].Quality != Quality.Good ||
					Values[3].Quality != Quality.Good ||
					Values[4].Quality != Quality.Good ||
					Values[5].Quality != Quality.Good ||
					Values[6].Quality != Quality.Good ||
					Values[7].Quality != Quality.Good ||
					Values[8].Quality != Quality.Good ||
					Values[9].Quality != Quality.Good)
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Invalid Transaction Data for " + iLoop.ToString() + ". Stored Transaction Cannot be Uploaded");
					continue;
				}

				TransDriverIndex = Values[0].Value.ToString();
				TransTruckIndex = Values[1].Value.ToString();
				TransTransactionNumber = Values[2].Value.ToString();
				TransEntryStart = Values[3].Value.ToString();
				TransEntryStop = Values[4].Value.ToString();
				TransStartTime = Values[5].Value.ToString();
				TransStopTime = Values[6].Value.ToString();
				TransDate = Values[7].Value.ToString();
				TransFirstArmNumber = Values[8].Value.ToString();
				TransLoadID = Values[9].Value.ToString();

				// make sure we have the correct ransaction
				if (System.Convert.ToInt32(TransTransactionNumber) != iLoop)
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Failed to Send the Correct Transaction. Stored Transaction Cannot be Uploaded");
					return;
				}

				// make sure that the date on the transaction is not before our last transaction date
				// The Contrec stores the date as dd/mm/yyyy

				DateTimeFormatInfo ContrecDateInfo = new DateTimeFormatInfo();
				ContrecDateInfo.ShortDatePattern = "dd/MM/yyyy";

				string CombineStopDateTimeString = TransDate + " " + TransStopTime;
				string CombineStartDateTimeString = TransDate + " " + TransStartTime;

                DateTimeOffset ContrecStopDateTime;
                DateTimeOffset ContrecStartDateTime;
                try
                {
                    ContrecStopDateTime = DateTimeOffset.Parse(CombineStopDateTimeString, ContrecDateInfo);
                    if (ContrecStopDateTime < this.Station.LastTransactionNumberDateTime)
                        continue;
                    this.Transaction.TimeOut = ContrecStopDateTime;
                }
				catch(FormatException)
                {
                    this.AddAlarmAndEventLogs(this.Security, this.Station.StationErrorAlarm("Device transaction " + TransTransactionNumber + " reports invalid stop date " + CombineStopDateTimeString));
                }
                try
                {
                    ContrecStartDateTime = DateTimeOffset.Parse(CombineStartDateTimeString, ContrecDateInfo);
                    this.Transaction.TimeIn = ContrecStartDateTime;
                }
                catch(FormatException)
                {
                    this.AddAlarmAndEventLogs(this.Security, this.Station.StationErrorAlarm("Device transaction " + TransTransactionNumber + " reports invalid stop date " + CombineStartDateTimeString));
                }


                // get the person and the associated equipment
                // at this point all we can do is assume that the user has kept the database up to date
                PersonClass Person = null;
				try
				{
					Person = PersonCollection[System.Convert.ToInt32(TransDriverIndex)];
				}
				catch
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Driver Index " + TransDriverIndex + " Is Invalid for Trans Number " + TransTransactionNumber);
					continue;
				}

				EquipmentClass Equipment = null;
				try
				{
					Equipment = EquipmentCollection[System.Convert.ToInt32(TransTruckIndex)];
				}
				catch
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Truck Index " + TransTruckIndex + " Is Invalid for Trans Number " + TransTransactionNumber);
					continue;
				}

			    this.Driver = Person;
			    this.TractorOrTanker = Equipment;
				// use the load number for the owner manager selections since they are required
				Guid identityGuid = Guid.Empty;
				if (this.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetOffLoadIdentityGuidByMapID(this.Security, TransLoadID.Trim())
																);


					// Invalid CompanyMap ID
					if (identityGuid == Guid.Empty || !this.ValidateOffLoadID(identityGuid))
					{
                        this.AddAlarmAndEventLogs(this.Security, this.Station.OfflineOffloadIdFallbackFailureAlarm(TransLoadID, TransTransactionNumber));
						this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Off-Load ID " + TransLoadID + " Is Invalid for Trans Number " + TransTransactionNumber);
						continue;
					}
                    this.AddAlarmAndEventLogs(this.Security, this.Station.OfflineOffloadIdFallbackEvent(TransLoadID, TransTransactionNumber));
				}
				else
				{
					identityGuid = FMChannelHelper.MakeCall<ICompanyMaps, Guid>(
																	 x =>
																	 x.GetIdentityGuidByMapID(this.Security, TransLoadID.Trim())
																);

					// Invalid CompanyMap ID
					if (identityGuid == Guid.Empty || !this.ValidateLoadID(identityGuid))
					{
                        this.AddAlarmAndEventLogs(this.Security, this.Station.OfflineLoadIdFallbackFailureAlarm(TransLoadID, TransTransactionNumber));
                        this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Load ID " + TransLoadID + " Is Invalid for Trans Number " + TransTransactionNumber);
						continue;
					}
                    this.AddAlarmAndEventLogs(this.Security, this.Station.OfflineLoadIdFallbackEvent(TransLoadID, TransTransactionNumber));
                }

				if (!this.Driver.CompanyGuid.IsEmpty())
				{
				    this.Carrier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(
																	 x =>
																	 x.Get(this.Security, this.Driver.CompanyGuid)
																);

				}

				// Drivers must be assigned to a Carrier
				if (this.Carrier == null
				&& this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE))
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Invalid Carrier for Transaction " + TransLoadID + " " + TransTransactionNumber);
					continue;
				}

				// since we must have a product select the first product on the first arm
				if (this.LoadArmManagerCollection.Count < 1)
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Invalid Loadarm Configuration.");
					return;
				}
				LoadArmManagerClass LoadArmManager = (LoadArmManagerClass)this.LoadArmManagerCollection.Item(0);

				int NumberOfAuthorizedRecipes = 0;

				for (int ProdIndex = 0; ProdIndex < LoadArmManager.LoadArm.ProductRecipeCollection.Count; ProdIndex++)
				{
					NumberOfAuthorizedRecipes++;
				}

				if (NumberOfAuthorizedRecipes <= 0)
				{
					this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " No Valid Recipes Configured.");
					return;
				}

				ProductMapClass Recipe = LoadArmManager.LoadArm.ProductRecipeCollection[0];
				LoadArmManager.CurrentRecipe = Recipe;

				// if we made it this far it is time to create the transaction

			    this.CurrentTransactionAlias = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasClass>(
																	 x =>
																	 x.Get(this.Security, this.Station.IssueByVolumeTransactionAliasGuid, false)
																);

			    this.InitializeTransaction();

                ContrecStopDateTime = DateTimeOffset.Parse(CombineStopDateTimeString, ContrecDateInfo);
                ContrecStartDateTime = DateTimeOffset.Parse(CombineStartDateTimeString, ContrecDateInfo);

                for (iLoop = System.Convert.ToInt32(TransEntryStart); iLoop <= System.Convert.ToInt32(TransEntryStop); iLoop++)
				{
					// each entry is a seperate line item in the transaction
					if (this.AddLineItem(LoadArmManager.LoadArm.IdentityGuid) == null)
						continue;

				    this.SaveTransaction();

					// set the data equal to the transaction in the Contrec
					if (!this.SetUploadedDataFromContrec(LoadArmManager.LoadArm.IdentityGuid, iLoop, TransTransactionNumber, ContrecStartDateTime, ContrecStopDateTime))
					{
						this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Failed to Send the Correct Transaction Entry For Transaction Number " + TransTransactionNumber + ". Stored Transaction Cannot be Uploaded");
						continue;
					}
				}
			    this.Transaction.Status = TransactionStatus.Completed;

                this.AddAlarmAndEventLogs(this.Security, this.Station.OfflineTransactionUploadedEvent(this.Transaction.DocumentNumber));

			    this.SaveTransaction();

			}

			// store the new value back to the station table
			this.Station.LastTransactionNumber = this.ContrecLastTransactionNumber;
			this.Station.LastTransactionNumberDateTime = DateTimeOffset.Now;
			FMChannelHelper.MakeCall<IStations>(
																	 x =>
																	 x.Modify(this.Security, this.Station)
																);
		}

		protected bool SetUploadedDataFromContrec(Guid armGuid, int EntryIndex, string TransTransactionNumber, DateTimeOffset ContrecStartDateTime, DateTimeOffset ContrecStopDateTime)
		{
			LineItemDO LineItem = this.GetLineItem(armGuid);

			if (LineItem == null)
			{
				return false;
			}

			ProcessVariableClass ContrecWriteEntryNumberPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_I4,
				false,
				this.StationPv.OPCItemID + ".Stored Entries.Set Entry Number",
				this.StationPv.URL,
				this.StationPv.ProgID);

			Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Entry Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Transaction Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Arm Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Compartment Number")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Gross Total")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Gross Accum Before")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Gross Accum After")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Preset Quantity")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Net Total")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Net Accum Before")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Net Accum After")),
							new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Entries.Average Temperature")),
							};

			string StoredEntriesEntryNumber = "";
			string StoredEntriesTransactionNumber = "";
			string StoredEntriesArmNumber = "";
			string StoredEntriesCompartmentNumber = "";
			string StoredEntriesGrossTotal = "";
			string StoredEntriesGrossAccumBefore = "";
			string StoredEntriesGrossAccumAfter = "";
			string StoredEntriesPresetQuantity = "";
			string StoredEntriesNetTotal = "";
			string StoredEntriesNetAccumBefore = "";
			string StoredEntriesNetAccumAfter = "";
			string StoredEntriesAverageTemperature = "";

			ContrecWriteEntryNumberPV.ServerValue = EntryIndex;
			try
			{
			    this.OPCServerManager.Write(ContrecWriteEntryNumberPV);
			}
			catch
			{
				this.eventLog.WriteEntry("Failed to Set Station " + this.Station.ID + " Stored Entry Number " + EntryIndex.ToString() + ". Stored Transaction Cannot be Uploaded");
				return false;
			}

			foreach (Item item in Items)
				item.MaxAgeSpecified = true;
			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);

			if (Values[0].Quality != Quality.Good ||
				Values[1].Quality != Quality.Good ||
				Values[2].Quality != Quality.Good ||
				Values[3].Quality != Quality.Good ||
				Values[4].Quality != Quality.Good ||
				Values[5].Quality != Quality.Good ||
				Values[6].Quality != Quality.Good ||
				Values[7].Quality != Quality.Good ||
				Values[8].Quality != Quality.Good ||
				Values[9].Quality != Quality.Good ||
				Values[10].Quality != Quality.Good ||
				Values[11].Quality != Quality.Good)
			{
				this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Invalid Entry Data for " + EntryIndex.ToString() + ". Stored Transaction Cannot be Uploaded");
				return false;
			}

			StoredEntriesEntryNumber = Values[0].Value.ToString();
			StoredEntriesTransactionNumber = Values[1].Value.ToString();
			StoredEntriesArmNumber = Values[2].Value.ToString();
			StoredEntriesCompartmentNumber = Values[3].Value.ToString();
			StoredEntriesGrossTotal = Values[4].Value.ToString();
			StoredEntriesGrossAccumBefore = Values[5].Value.ToString();
			StoredEntriesGrossAccumAfter = Values[6].Value.ToString();
			StoredEntriesPresetQuantity = Values[7].Value.ToString();
			StoredEntriesNetTotal = Values[8].Value.ToString();
			StoredEntriesNetAccumBefore = Values[9].Value.ToString();
			StoredEntriesNetAccumAfter = Values[10].Value.ToString();
			StoredEntriesAverageTemperature = Values[11].Value.ToString();

			// the density is stored at a different level so get it here
			Item[] ItemsTransactions = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Arm 1 Density")),
										new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Arm 2 Density")),
										new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Arm 3 Density")),
										new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Stored Transactions.Arm 4 Density")),
										};
			foreach (Item item in ItemsTransactions)
				item.MaxAgeSpecified = true;
			((Item)Items[0]).MaxAgeSpecified = false;

			ItemValueResult[] TransactionsValues = this.OPCServerManager.Read(new URL(this.StationPv.URL), ItemsTransactions);

			double StoredTransactionsArmDensity = 1.0;

			if (System.Convert.ToInt32(StoredEntriesArmNumber) == 4)
			{
				if (Values[3].Quality == Quality.Good)
					StoredTransactionsArmDensity = System.Convert.ToDouble(TransactionsValues[3].Value.ToString());
			}
			else if (System.Convert.ToInt32(StoredEntriesArmNumber) == 3)
			{
				if (Values[2].Quality == Quality.Good)
					StoredTransactionsArmDensity = System.Convert.ToDouble(TransactionsValues[2].Value.ToString());
			}
			else if (System.Convert.ToInt32(StoredEntriesArmNumber) == 2)
			{
				if (Values[1].Quality == Quality.Good)
					StoredTransactionsArmDensity = System.Convert.ToDouble(TransactionsValues[1].Value.ToString());
			}
			else if (System.Convert.ToInt32(StoredEntriesArmNumber) == 1)
			{
				if (Values[0].Quality == Quality.Good)
					StoredTransactionsArmDensity = System.Convert.ToDouble(TransactionsValues[0].Value.ToString());
			}

			// make sure we have the correct transaction
			if (System.Convert.ToInt32(TransTransactionNumber) != System.Convert.ToInt32(StoredEntriesTransactionNumber))
			{
				this.eventLog.WriteEntry("Contrec Station " + this.Station.ID + " Failed to Send the Correct Transaction. Stored Transaction Cannot be Uploaded");
				return false;
			}

			LineItem.LoadingLocationID = this.Station.ID;
			LineItem.LoadingLocationStationGuid = this.Station.IdentityGuid;

			if (LineItem.PresetAmount == null)
				LineItem.PresetAmount = 0.0;
			LineItem.PresetAmount = System.Convert.ToDouble(StoredEntriesPresetQuantity);

			LoadArmManagerClass LoadArmManager = (LoadArmManagerClass)this.LoadArmManagerCollection.Item(0);

			ProcessVariableClass PV = new ProcessVariableClass();

			double GrossVolume = System.Convert.ToDouble(StoredEntriesGrossTotal);
			double NetVolume = System.Convert.ToDouble(StoredEntriesNetTotal);
			double AverageTemperature = System.Convert.ToDouble(StoredEntriesAverageTemperature);
			double AverageDensity = StoredTransactionsArmDensity;
			double MeterStart = System.Convert.ToDouble(StoredEntriesGrossAccumBefore);
			double MeterStop = System.Convert.ToDouble(StoredEntriesGrossAccumAfter);

			if (LineItem.BatchNumber == null
			|| LineItem.BatchNumber == "")
				LineItem.BatchNumber = LoadArmManager.GetBatchNumber(this);

			if (LineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				ProductMapClass Component = LoadArmManager.GetComponent(LineItem.ProductGuid);
				if (Component == null)
					throw new Exception("Component not found in LoadArm Configuration");

				if (LineItem.Quantity == null)
					LineItem.Quantity = new QuantityDO();


				PV.ProcessVariableType = PROCESS_VARIABLE_TYPE.GROSS_VOLUME_PV;
				PV.ServerUnits = this.SiteManager.Site.VolumeUnits;
				PV.ServerValue = GrossVolume;
				if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					LineItem.Quantity.GrossInventoryChange = ((double)PV.GetValue(LineItem.VolumeUnits, LineItem.VolumeDecimalPlaces));
				else
					LineItem.Quantity.GrossInventoryChange = -((double)PV.GetValue(LineItem.VolumeUnits, LineItem.VolumeDecimalPlaces));
				LineItem.Quantity.BadGrossQualityLogged = false;

				PV.ProcessVariableType = PROCESS_VARIABLE_TYPE.NET_VOLUME_PV;
				PV.ServerUnits = this.SiteManager.Site.VolumeUnits;
				PV.ServerValue = NetVolume;
				if (this.Station.Type == STATION_TYPE.OFF_LOADING)
					LineItem.Quantity.NetInventoryChange = ((double)PV.GetValue(LineItem.VolumeUnits, LineItem.VolumeDecimalPlaces));
				else
					LineItem.Quantity.NetInventoryChange = -((double)PV.GetValue(LineItem.VolumeUnits, LineItem.VolumeDecimalPlaces));
				LineItem.Quantity.BadNetQualityLogged = false;

				if (LineItem.Temperature == null)
					LineItem.Temperature = 0.0;

				PV.ProcessVariableType = PROCESS_VARIABLE_TYPE.TEMPERATURE_PV;
				PV.ServerUnits = this.SiteManager.Site.TemperatureUnits;
				if (this.SiteManager.Site.TemperatureUnits == EngineeringUnit.FmtDegF)
					PV.ServerValue = AverageTemperature / 10;
				else
					PV.ServerValue = AverageTemperature / 100;
				LineItem.Temperature = (double)PV.GetValue(this.SiteManager.Site.TemperatureUnits, this.SiteManager.Site._TemperatureDecimalPlaces);
				LineItem.Temperature_BadQualityLogged = false;


				LineItem.VCF = Math.Round(LineItem.Quantity.NetInventoryChange / LineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);

				if (LineItem.Density == null)
					LineItem.Density = 0.0;


				PV.ProcessVariableType = PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV;
				PV.ServerUnits = this.SiteManager.Site.DensityUnits;

				// Presently the system expects that the Preset Units will match the Site Units
				double scale = 10;
				if (PV.ServerUnits == EngineeringUnit.FmdDegApi)
					scale = 10;
				else if (PV.ServerUnits == EngineeringUnit.FmdGcm3)
					scale = 10000;

				PV.ServerValue = AverageDensity / scale;
				LineItem.Density = (double)PV.GetValue(this.SiteManager.Site.DensityUnits, this.SiteManager.Site._DensityDecimalPlaces);
				LineItem.Density_BadQualityLogged = false;

				SiteTimeConverter timeConverter = new SiteTimeConverter(this.SiteManager.Site);
				DateTimeOffset siteTimeStartDate = timeConverter.ConvertToSiteTime(ContrecStartDateTime);
				DateTimeOffset siteTimeStopDate = timeConverter.ConvertToSiteTime(ContrecStopDateTime);

				if (LineItem.MeterReading.MeterStart == null)
				{
					LineItem.MeterReading.MeterStart = MeterStart;
					LineItem.MeterReading.StartDateTime = siteTimeStartDate;
					LineItem.MeterReading.MeterStop = MeterStop;
					LineItem.MeterReading.StopDateTime = siteTimeStopDate;
				}
				else if (LineItem.MeterReading.MeterStart != null &&
					LineItem.MeterReading.MeterStop.Value != MeterStart)
				{
					LineItem.MeterReading.MeterStart = MeterStart;
					LineItem.MeterReading.StartDateTime = siteTimeStartDate;
					LineItem.MeterReading.MeterStop = MeterStop;
					LineItem.MeterReading.StopDateTime = siteTimeStopDate;
				}

				if (LineItem.MeterReading.MeterStop.Value != MeterStop)
				{
					LineItem.MeterReading.MeterStop = MeterStop;
					LineItem.MeterReading.StopDateTime = siteTimeStopDate;
				}
				LineItem.MeterReading.MeterStop_BadQualityLogged = false;

				if (Component.MeterValue != LineItem.MeterReading.MeterStop.Value)
				{
					Component.MeterValue = LineItem.MeterReading.MeterStop.Value;
				}
            }

			LineItem.Status = TransactionStatus.Completed;

		    this.CloseOutLineItem(LineItem);

			return true;
		}

		public override void UpdatePermissives(bool authorized)
		{
			foreach (ProcessVariableClass PV in this.Station.StationPermissives.Outputs)
			{
				switch (PV.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.OUTPUT_PERMISSIVE_PV:
						{
							PermissivesClass Permissives = PV.Parent as PermissivesClass;
							if (Permissives == null)
								break;

							PV.ServerValue = (bool)authorized;

						    this.OPCServerManager.Update(true);

							if (!PV.IsQualityGood
							|| ((bool)PV.ServerValue) != authorized)
							{
							    this.StationState = StationState.RESET_ON_TIMEOUT;
							    this.DisplayMessage("LoadRack|Error Setting Permissive" + " " + PV.OPCItemID, null, 0, this.MESSAGE_TIMEOUT);
								return;
							}

							break;
						}

					default:
						this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + PV.OPCItemID);
						break;
				}
			}
		}

		protected override void ProcessOffLoadProductSelect(string response)
		{
			bool bProductFound = false;
			if (response == EscapeString)
			{
			    this.StationState = StationState.OFFLOADID_PROMPT;
			    this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
				return;
			}

			int MenuNumber = 1;
			if (this.Station.OffLoadByOffLoadID == true)
			{
				foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					foreach (ProductMapClass ProductMap in LoadArmManager.LoadArm.ComponentCollection)
					{
						if (MenuNumber == System.Convert.ToInt32(response))
						{
						    this.SelectedProductID = ProductMap.AssignedID;
							LoadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.GetByProductAuthorizedCompanies(this.Security, ProductMap.AssignedGuid, false)
																);

							if (LoadArmManager.CurrentLineItemProduct != null)
							{
								LoadArmManager.CurrentRecipe = ProductMap;
								bProductFound = true;
							}
							break;
						}
						++MenuNumber;
					}
				}
			}

			if (bProductFound == false)
			{
			    this.DisplayOffLoadProductSelect();
				return;
			}

		    this.PromptForOffLoadDensity();
		}

		//protected override void PromptForOffLoadDensity()
		//{

		//    if (AvailableLoadArmManagers == 0)
		//        return;

		//    // Output Station Message to all arms since we do not know which one will be used 
		//    foreach (Contrec1010RALoadArmManagerClass LoadArmManager in LoadArmManagerCollection)
		//    {
		//        try
		//        {
		//            LoadArmManager.IssueLoadNumberTransactiont();
		//        }
		//        catch (Exception e)
		//        {
		//            EventLog.WriteEntry("Contrec 1010 RA StationManager IssueLoadNumberTransactiont : " + e.Message, EventLogEntryType.Error);
		//        }
		//    }

		//    return;
		//}


	}
}
