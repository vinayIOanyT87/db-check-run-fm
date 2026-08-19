/******************************************************************************

	FILE NAME:		ProximityCardReaderStationManager.cs


	PURPOSE:			ProximitiyCardReaderStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
*******************************************************************************/
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

using Opc;
using Opc.Da;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	using FMBusinessObjects.LogClient;

	using System.Collections.Generic;

	/// <summary>
	/// Summary description for OsdpStationManagerClass.
	/// </summary>
	public class OsdpStationManagerClass : StationManagerClass
	{
		protected ProcessVariableClass CardReaderDataPv;
		protected ProcessVariableClass KeypadDataPv;

		protected ManualResetEvent CardReaderKillEvent;
		protected Thread CardReaderScanThread;
		protected int MessageTimer;

		private string PinBuffer = string.Empty;
		private bool skipInitialSwipes;

		// Beep durations are in 10ths of a second
		private const int beepDurationQuick = 2;
		private const int beepDurationError = 10;

		// LED durations are in seconds
		private const int ledDurationFlash = 1;

		public OsdpStationManagerClass(
			EventLog eventLog,
			LoadRackManagerClass loadRackManager,
			StationClass station,
			SiteManagerClass siteManager,
			SecurityClass security)
			: base(eventLog, loadRackManager, station, siteManager, security)
		{
			this.skipInitialSwipes = true;// This accomodates the OdspOPCServer preserving the last card swipe value
													// and the station picking it up and processing it during initialization
													// It will be set to false in the scan loop after .5 seconds

			// Configure the PV's associated with the Station
			if (this.StationPv.URL != "")
			{
				this.CardReaderDataPv = new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.CARDREADER_PV,
					UNIT_TYPE.STATION_UNIT,
					VarEnum.VT_BSTR,
					true,
					this.StationPv.OPCItemID + ".Card Reader Data",
					this.StationPv.URL,
					this.StationPv.ProgID
					);

				this.OPCServerManager.AddProcessVariable(this.CardReaderDataPv);

				this.CardReaderDataPv = new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV,
					UNIT_TYPE.STATION_UNIT,
					VarEnum.VT_BSTR,
					true,
					this.StationPv.OPCItemID + ".Keypad Data",
					this.StationPv.URL,
					this.StationPv.ProgID
					);

				this.OPCServerManager.AddProcessVariable(this.CardReaderDataPv);
			}


			// Launch thread to manage message/prompt timeout for PIN entry
			ThreadStart CardReaderScanStart = new ThreadStart(this.CardReaderScan);
			this.CardReaderKillEvent = new ManualResetEvent(false);
			this.CardReaderScanThread = new Thread(CardReaderScanStart);
			this.CardReaderScanThread.Start();

			this.StationState = StationState.IDLE;
			// Set the LED to "Off" as the status quo
			ArrayList itemValues = new ArrayList();

			ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".Led Off") { Value = 0 };
			itemValues.Add(writeItem);

			this.OPCServerManager.Write(new URL(this.StationPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));

			this.ResetStationDevice();
		}

		~OsdpStationManagerClass()
		{
			this.Dispose();
		}

		public override void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				base.Dispose();

				// Terminate the Scan Thread
				this.CardReaderKillEvent?.Set();
				this.CardReaderScanThread?.Join();

				GC.SuppressFinalize(this);

				this.AlreadyDisposed = true;
			}
		}

		public void CardReaderScan()
		{
			// Every 500 msec
			while (!this.CardReaderKillEvent.WaitOne(500, true))
			{
				try
				{
					Monitor.Enter(this);
					try
					{
						if (this.StationState == StationState.RESET_ON_TIMEOUT)
						{
							this.ProcessMessageTimeout();
						}

						if (this.MessageTimer > 0)
						{
							this.MessageTimer--;
							if (this.MessageTimer == 0)
							{
								this.ProcessMessageTimeout();
							}
						}

						if (this.GateTimer > 0)
						{
							this.GateTimer--;
							if (this.GateTimer == 0)
							{
								this.CloseGate();
							}
						}

						this.skipInitialSwipes = false;
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry($"{nameof(OsdpStationManagerClass)} {nameof(CardReaderScan)} : " + e.TargetSite + ":" + e.Message + " : " + this.StationState.ToString() + "\n" + e.StackTrace, EventLogEntryType.Error);
						if (this.StationState != StationState.IDLE)
						{
							this.ResetStationDevice();
						}
					}
					finally
					{
						Monitor.Exit(this);
					}
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString() + e.TargetSite + ":" + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
				}
			}
		}

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout)
		{
			// We only have an LED (red/green/amber) and a beeper
			// Because of this, we can only handle a few stock messages

			ArrayList itemValues = new ArrayList();
			switch (stockMessage)
			{
				case "[LoadRack|Invalid Card Number]":
				case "[LoadRack|Invalid ID]":
				case "[LoadRack|Invalid Carrier]":
				case "[LoadRack|Carrier] [LoadRack|Locked Out]":
				case "[LoadRack|Carrier] [LoadRack|Invalid]":
				case "[LoadRack|Carrier] [LoadRack|Inactive]":
				case "[LoadRack|Carrier] [LoadRack|Credit]":
				case "[LoadRack|Carrier Access Not Scheduled]":
				case "[LoadRack|Card Unassigned]":
				case "[LoadRack|Driver] [LoadRack|Locked Out]":
				case "[LoadRack|Driver Access Not Scheduled]":
				case "[LoadRack|Driver Timeout]":
				case "[LoadRack|Driver] [LoadRack|Not Qualified]":
				case "[LoadRack|Driver] [LoadRack|Not Trained]":
				case "[LoadRack|Driver] [LoadRack|Not Licensed]":
				case "[LoadRack|Terminal Access Not Scheduled]":
				case "[LoadRack|Qualification Expired]":
				case "[LoadRack|Training Expired]":
				case "[LoadRack|Driver License Expired]":
				case "[LoadRack|Cert/Perm Expired]":
				case "[LoadRack|Carrier License Expired]":
				case "[LoadRack|Carrier Insurance Expired]":
				case "[LoadRack|Multiple Card-in]":
				case "[LoadRack|Max Retries Exceeded]":
				case "[LoadRack|PIN] [LoadRack|Timeout]":
					{
						this.WriteLogDataToCommFile($"Red LED {messageTimeout}sec, beep{OsdpStationManagerClass.beepDurationError / 10.0}sec ({stockMessage})", CommLogDirection.Out);

						ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".Red LED") { Value = messageTimeout };
						itemValues.Add(writeItem);

						writeItem = new ItemValue(this.StationPv.OPCItemID + ".Beep") { Value = OsdpStationManagerClass.beepDurationError };
						itemValues.Add(writeItem);
						break;
					}
				case "[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|PIN]":
					{
						this.WriteLogDataToCommFile($"Amber LED {messageTimeout}sec, beep{OsdpStationManagerClass.beepDurationError/10.0}sec  ({stockMessage})", CommLogDirection.Out);

						ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".Amber LED") { Value = messageTimeout };
						itemValues.Add(writeItem);

						writeItem = new ItemValue(this.StationPv.OPCItemID + ".Beep") { Value = OsdpStationManagerClass.beepDurationError };
						itemValues.Add(writeItem);
						break;
					}
				case "[LoadRack|Enter PIN]":
					{
						this.WriteLogDataToCommFile($"Green LED {OsdpStationManagerClass.ledDurationFlash}sec, beep {OsdpStationManagerClass.beepDurationQuick/10.0}sec ({stockMessage})", CommLogDirection.Out);

						ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".Green LED") { Value = OsdpStationManagerClass.ledDurationFlash };
						itemValues.Add(writeItem);

						writeItem = new ItemValue(this.StationPv.OPCItemID + ".Beep") { Value = OsdpStationManagerClass.beepDurationQuick };
						itemValues.Add(writeItem);
						break;
					}
			}

			this.OPCServerManager.Write(new URL(this.StationPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));

			return 0;
		}

		public override void CheckDriverMessages(bool acknowledged)
		{
			// Turn on the Green LED & Contact and perform standard Open Gate Processing
			List<ItemValue> itemValues = new List<ItemValue>();

			this.WriteLogDataToCommFile($"Green LED {OsdpStationManagerClass.ledDurationFlash}sec", CommLogDirection.Out);
			ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".Green LED") { Value = OsdpStationManagerClass.ledDurationFlash };
			itemValues.Add(writeItem);

			this.OPCServerManager.Write(new URL(this.StationPv.URL), itemValues.ToArray());

			itemValues.Clear();

			switch (this.Station.Type)
			{
				case STATION_TYPE.BOL:
					this.CardIn();

					this.PrintTransactions();

					if (!this.SiteManager.AnyExitGates)
					{
						this.CardOut();
					}

					return;
				case STATION_TYPE.ENTRY_GATE:
					this.CardIn();
					this.OpenGate();
					break;
				case STATION_TYPE.EXIT_GATE:
					this.CardOut();
					this.OpenGate();
					break;
			}
		}

		protected override void EntryGateProcessing(ProcessVariableClass pv)
		{
			switch (pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						if (pv.IsQualityGood)
						{
							if (this.StationState == StationState.IDLE && !this.skipInitialSwipes)
							{
								if (pv.ServerValue is string serverValue && serverValue != "")
								{
									this.WriteLogDataToCommFile($"Read card data '{serverValue}'", CommLogDirection.In);
									this.ProcessDriverID(serverValue);
								}
							}
						}

						break;
					}

				case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV:
					{
						if (pv.IsQualityGood)
						{
							if (this.StationState == StationState.PIN_PROMPT)
							{
								if (pv.ServerValue is string serverValue && serverValue != "")
								{
									this.WriteLogDataToCommFile($"Read keypress data '{serverValue}'", CommLogDirection.In);
									this.PinBuffer += serverValue;
									this.PinBuffer = this.PinBuffer.Left(PinLength);
									if (this.PinBuffer.Length == PinLength)
									{
										this.WriteLogDataToCommFile($"Read aggregated PIN '{this.PinBuffer}'", CommLogDirection.In);
										this.ProcessPIN(this.PinBuffer);
									}
								}
							}
						}

						break;
					}

				default:
					this.eventLog.WriteEntry("Proximity Card Reader StationManager: Unknown PV Type OnInvoke");
					break;
			}
		}

		protected override void ExitGateProcessing(ProcessVariableClass pv)
		{
			switch (pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						if (pv.IsQualityGood)
						{
							if (this.StationState == StationState.IDLE && !this.skipInitialSwipes)
							{
								if (pv.ServerValue is string serverValue && serverValue != "")
								{
									this.WriteLogDataToCommFile($"Read card data '{serverValue}'", CommLogDirection.In);
									this.ProcessDriverID(serverValue);
								}
							}
						}

						break;
					}

				case PROCESS_VARIABLE_TYPE.KEYPAD_DATA_PV:
					{
						if (pv.IsQualityGood)
						{
							if (this.StationState == StationState.PIN_PROMPT)
							{
								if (pv.ServerValue is string serverValue && serverValue != "")
								{
									this.WriteLogDataToCommFile($"Read keypress data '{serverValue}'", CommLogDirection.In);
									this.PinBuffer += serverValue;
									this.PinBuffer = this.PinBuffer.Left(PinLength);
									if (this.PinBuffer.Length == PinLength)
									{
										this.WriteLogDataToCommFile($"Read aggregated PIN '{this.PinBuffer}'", CommLogDirection.In);
										this.ProcessPIN(this.PinBuffer);
									}
								}
							}
						}

						break;
					}

				default:
					this.eventLog.WriteEntry("Proximity Card Reader StationManager: Unknown PV Type OnInvoke");
					break;
			}
		}

		protected override void BolProcessing(ProcessVariableClass pv)
		{
			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("BOL Station - Called BOLProcessing");
			switch (pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						if (pv.IsQualityGood)
						{
							logger.Debug($"BoL Station received card data {pv.ServerValue}");
							if (pv.ServerValue is string serverValue && serverValue != "")
							{
								this.ProcessDriverID((string)pv.ServerValue);
							}
						}

						break;
					}

				default:
					this.eventLog.WriteEntry("Proximity Card Reader StationManager: Unknown PV Type OnInvoke");
					break;
			}
		}

		protected override void OpenGate()
		{
			if ((this.GatePV?.URL ?? string.Empty) != string.Empty)
			{
				try
				{
					this.GatePV.ServerValue = true;
					this.OPCServerManager.Write(this.GatePV);

					if (this.Station.Type != STATION_TYPE.ENTRY_GATE)
					{
						this.CardOut();
					}
					this.GateTimer = 10 * 2;
					this.StationState = StationState.OPENING_GATE;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Gate Open Failure", null, 0, this.MESSAGE_TIMEOUT);
				}
			}
		}

		protected void CloseGate()
		{
			if ((this.GatePV?.URL ?? string.Empty) != string.Empty)
			{
				try
				{
					this.GatePV.ServerValue = false;
					this.OPCServerManager.Write(this.GatePV);

					this.StationState = StationState.IDLE;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("LoadRack|Gate Open Failure", null, 0, this.MESSAGE_TIMEOUT);
				}
			}
		}

		public override void UploadStoredTransactions()
		{
			throw new NotImplementedException();
		}

		public override bool SetDownloadDensityInUnitFlag(string density)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// The Proximity Card Reader has no means to display or select from a menu.
		/// Fall back to selecting the first valid carrier in the collection.
		/// </summary>
		protected override void FinishDriverCarrierProcessing()
		{
			this.Manager = null;
			this.Owner = null;
			this.Shipper = null;
			this.BillTo = null;
			this.ShipTo = null;
			this.Carrier = null;
			this.TractorOrTanker = null;
			this.Trailer1 = null;
			this.Trailer2 = null;
			this.Trailer3 = null;
			this.Transaction = null;
			this.Order = null;
			this.PONumber = null;
			this.LoadID = null;
			this.ByWeight = false;
			this.ByWeightProduct = string.Empty;
			this.PendingTransactions.Clear();
			this.LoadArmManagerCollection.ClearRecipeMap(this);
			this.PIDXAuthorizationArray = null;
			this.PIDXProfileCompanyMapCollection = null;

			CompanyClass testedCompany = null;
			bool validCarrierFound = false;

			foreach (CompanyMapClass assignedCompany in this.Driver.AssignedCompaniesCollection)
			{
				if (assignedCompany.AssignedGuid == Guid.Empty)
				{
					continue;
				}

				testedCompany = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(x => x.Get(this.Security, assignedCompany.AssignedGuid));

				if (testedCompany == null &&
					 (this.Driver.HasRole(PERSON_ROLE.LOADER_ROLE)
					 || this.Driver.HasRole(PERSON_ROLE.OFFLOADER_ROLE)))
				{
					continue;
				}

				if (testedCompany != null)
				{
					if (testedCompany.LockedOut)
					{
						continue;
					}
				}

				validCarrierFound = true;
				break;
			}

			// If no valid carrier found, alarm based as the last entry found
			if (validCarrierFound == false)
			{
				if (testedCompany == null)
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Station.InvalidCarrierEvent(this.Driver.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.DisplayMessage("[LoadRack|Invalid Carrier]", null, 0, this.MESSAGE_TIMEOUT);
				}
				else
				{
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, testedCompany.LockedOutStationAlarm(this.Driver.FirstLastName, this.Station.ID)));
					this.LoadRackManager.EventOrAlarmEvent.Set();
					this.DisplayMessage("[LoadRack|Carrier] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
				}

				this.StationState = StationState.RESET_ON_TIMEOUT;
				return;
			}

			this.Carrier = testedCompany;
			var timeConverter = new SiteTimeConverter(this.SiteManager.Site);
			if (this.Carrier != null)
			{
				this.Carrier._LastActivityDate.Value = timeConverter.Now();
				FMChannelHelper.MakeCall<ICompanies>(x => x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Carrier));
			}

			// Check driver timeout here (device types with more functional user interface
			// check for driver timeout in CompleteDriverProcessing()).
			if (!this.Driver.InhibitInactivityLockout
				 && DateTime.UtcNow - this.Driver._LastActivityDate.UTCValue > new TimeSpan(this.SiteManager.Site._DriverTimeoutPeriod, 0, 0, 0, 0))
			{
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(x => x.Add(this.Security, this.Driver.DriverAccessTimedOutAlarm));
				this.LoadRackManager.EventOrAlarmEvent.Set();
				this.DisplayMessage("[LoadRack|Driver Timeout]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.RESET_ON_TIMEOUT;
				return;
			}

			// Prompt for PIN
			if (this.Station.InterfaceType != STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER
					&& this.PinRequired)
			{
				this.IssuePromptForPIN();
				return;
			}

			this.StationState = StationState.IDLE;
			this.CompleteDriverProcessing(false);
		}

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
			_ = responseLength;
			this.MessageTimer = messageTimeout * 2; // MessageTimer counts 500ms steps
			this.PinBuffer = string.Empty;
			this.DisplayMessage(stockMessage, null, 0, messageTimeout);
		}

		protected override void ProcessPIN(string Response)
		{
			// Check the driver PIN number against the response.  
			if (Response != this.Driver.PINNumber)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.InvalidPinEvent(this.Driver.ID, Response));

				this.ConsecutivePrompts++;
				if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					this.DisplayMessage("[LoadRack|Max Retries Exceeded]", null, 0, this.MESSAGE_TIMEOUT);
					this.Driver.LockedOut = true;
					this.Driver.LockedOutDate = TimeConverter.Now(this.SiteManager.Site).ToString("d");
					this.Driver.LockedOutReason = "Maximum number of Card In attempts was exceeded";
					this.ModifyPersonnel(this.Security, DATA_TYPE.DYNAMIC, this.Driver);
					this.ConsecutivePrompts = 0;
					return;
				}

				this.StationState = StationState.PIN_PROMPT;
				this.PromptForPin("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|PIN]", PinLength, this.PROMPT_TIMEOUT);
				return;
			}

			this.StationState = StationState.IDLE;
			this.CompleteDriverProcessing(false);
		}

		protected override void CompleteDriverProcessing(bool acknowledgement)
		{
			// Card reader doesn't offer an opportunity for acknowleging warnings.
			// Just note events and don't tell the user (that will happen at the load rack anyway)
			// For expiry, just deny the user.
			_ = acknowledgement;
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
			DateTimeOffset siteTimeToday = TimeConverter.ToDate(siteTimeNow);

			if (this.StationState == StationState.IDLE)
			{
				// verify that the driver has the training and qualifications required to operate this piece of equipment
				// first check if the driver or station is setup with required access

				// check the qualifications
				foreach (QualificationMapClass reqQualification in this.Station.ReqQualificationsCollection)
				{
					bool qualificationsOk = false;
					foreach (QualificationMapClass qualification in this.Driver.QualificationCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (reqQualification.AssignedGuid == qualification.AssignedGuid)
						{
							qualificationsOk = true;
							break;
						}
					}

					if (qualificationsOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotQualifiedEvent(this.Driver.ID, reqQualification.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Qualified]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// check the training
				foreach (QualificationMapClass reqTraining in this.Station.ReqTrainingCollection)
				{
					bool trainingOk = false;
					foreach (QualificationMapClass qualification in this.Driver.TrainingCollection)
					{
						// check eack station qualification and if not accessed inform the driver
						if (reqTraining.AssignedGuid == qualification.AssignedGuid)
						{
							trainingOk = true;
							break;
						}
					}

					if (trainingOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotTrainedEvent(this.Driver.ID, reqTraining.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Trained]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// check the license
				foreach (QualificationMapClass reqLicense in this.Station.ReqLicenseCollection)
				{
					bool licenseOk = false;
					foreach (QualificationMapClass qualification in this.Driver.LicenseCollection)
					{
						// check eack station license and if not accessed inform the driver
						if (reqLicense.AssignedGuid == qualification.AssignedGuid)
						{
							licenseOk = true;
							break;
						}
					}

					if (licenseOk == false)
					{
						this.AddAlarmAndEventLogs(this.Security, this.Station.DriverNotLicensedEvent(this.Driver.FirstLastName, reqLicense.ID));
						this.DisplayMessage("[LoadRack|Driver] [LoadRack|Not Licensed]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}
				}

				// Check for Driver Timed Out
				if (!this.Driver.InhibitInactivityLockout
					 && DateTimeOffset.Now - this.Driver._LastActivityDate.Value > new TimeSpan(this.SiteManager.Site._DriverTimeoutPeriod, 0, 0, 0, 0))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Driver.DriverAccessTimedOutAlarm);
					this.DisplayMessage("[LoadRack|Driver Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				this.Driver._LastActivityDate.Value = siteTimeNow;
				FMChannelHelper.MakeCall<IPersonnel>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, this.Driver)
																);

				// Check if Site AccessSchedule precludes access
				if (this.SiteManager.Site.InhibitAccessAfterHours && !this.Driver.HasRole(PERSON_ROLE.SUPERVISOR_ROLE))
				{
					DateTimeOffset now = siteTimeNow;
					int day = (int)now.Date.ToOADate();
					bool holiday = false;
					foreach (ScheduleClass schedule in this.SiteManager.Site.HolidayScheduleCollection)
					{
						if (schedule.HolidayDate.HasValue && schedule.HolidayDate.Value.Date.ToOADate() == day)
						{
							holiday = true;
							if (!schedule.Enabled || schedule.OpeningTime.Value.TimeOfDay > now.TimeOfDay
								 || schedule.ClosingTime.Value.TimeOfDay < now.TimeOfDay)
							{
								this.AddAlarmAndEventLogs(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID));
								this.DisplayMessage("[LoadRack|Terminal Access Not Scheduled]", null, 0, this.MESSAGE_TIMEOUT);
								this.StationState = StationState.RESET_ON_TIMEOUT;
								return;
							}
							break;
						}
					}

					if (!holiday)
					{
						int index = (int)now.DayOfWeek;
						if (!this.SiteManager.Site.OperatingScheduleCollection[index].Enabled
							 || this.SiteManager.Site.OperatingScheduleCollection[index].OpeningTime.Value.TimeOfDay > now.TimeOfDay
							 || this.SiteManager.Site.OperatingScheduleCollection[index].ClosingTime.Value.TimeOfDay < now.TimeOfDay)
						{
							this.AddAlarmAndEventLogs(this.Security, this.Station.TerminalAccessNotScheduledEvent(this.Driver.ID));
							this.DisplayMessage("[LoadRack|Terminal Access Not Scheduled]", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}
					}
				}

				if (!this.ValidateCompany(this.Carrier, COMPANY_ROLE.CARRIER))
				{
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				// Check for Expired Driver Qualifications
				foreach (QualificationMapClass qualification in this.Driver.QualificationCollection)
				{
					if (qualification.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, qualification.PersonnelQualificationExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Qualification Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (qualification.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, qualification.PersonnelQualificationWarningEvent(this.Driver.ID));
					}
				}

				// check for expired training requirements
				foreach (QualificationMapClass reqTraining in this.Driver.TrainingCollection)
				{
					if (reqTraining.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Training Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (reqTraining.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, reqTraining.PersonnelTrainingWarningEvent(this.Driver.ID));
					}
				}

				// Check for Expired Driver Licenses
				foreach (QualificationMapClass license in this.Driver.LicenseCollection)
				{
					if (license.ExpirationDate.Value <= siteTimeToday)
					{
						this.AddAlarmAndEventLogs(this.Security, license.PersonnelLicenseExpiredAlarm(this.Driver.ID));
						this.DisplayMessage("[LoadRack|Driver License Expired]", null, 0, this.MESSAGE_TIMEOUT);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						return;
					}

					if (license.ExpirationDate.Value - siteTimeToday
						 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
					{
						this.AddAlarmAndEventLogs(this.Security, license.PersonnelLicenseWarningEvent(this.Driver.ID));
					}
				}

				// Check for Expired Certificate/Permit
				if (this.Carrier != null)
				{
					foreach (QualificationMapClass certificationOrPermit in this.Carrier.CertificateAndPermitCollection)
					{
						if (certificationOrPermit.ExpirationDate.Value <= siteTimeToday)
						{
							this.AddAlarmAndEventLogs(
								this.Security, certificationOrPermit.CompanyCertificateOrPermitExpiredAlarm(this.Carrier.ID));
							this.DisplayMessage("[LoadRack|Cert/Perm Expired]", null, 0, this.MESSAGE_TIMEOUT);
							this.StationState = StationState.RESET_ON_TIMEOUT;
							return;
						}

						if (certificationOrPermit.ExpirationDate.Value - siteTimeToday
							 < new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0))
						{
							this.AddAlarmAndEventLogs(
								this.Security, certificationOrPermit.CompanyCertificateOrPermitWarningEvent(this.Carrier.ID));
						}
					}
				}

				// Check for Expired License
				if (this.Carrier != null && this.Carrier.LicenseExpired)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.LicenseExpiredAlarm);
					this.DisplayMessage("[LoadRack|Carrier License Expired]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Carrier != null
					 && this.Carrier.LicenseWarning(new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0)))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.LicenseWarningEvent);
				}

				// Check for Expired Insurance
				if (this.Carrier != null && this.Carrier.InsuranceExpired)
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.InsuranceExpiredAlarm);
					this.DisplayMessage("[LoadRack|Carrier Insurance Expired]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
					return;
				}

				if (this.Carrier != null
					 && this.Carrier.InsuranceWarning(new TimeSpan(this.SiteManager.Site._DriverWarningPeriod, 0, 0, 0, 0)))
				{
					this.AddAlarmAndEventLogs(this.Security, this.Carrier.InsuranceWarningEvent);
				}
			}

			if (this.Station.Type == STATION_TYPE.ENTRY_GATE || this.Station.Type == STATION_TYPE.EXIT_GATE)
			{
				if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
				{
					if (this.SiteManager.Site.InhibitMultipleCardIns && this.Driver.CardedIn
						 && !this.Driver.HasRole(PERSON_ROLE.SUPERVISOR_ROLE))
					{
						this.AddAlarmAndEventLogs(this.Security, this.Driver.MultipleCardInAlarm);
						this.StationState = StationState.RESET_ON_TIMEOUT;
						this.DisplayMessage("[LoadRack|Multiple Card-in]", null, 0, this.MESSAGE_TIMEOUT);
						return;
					}
				}
				else if (this.Station.Type == STATION_TYPE.EXIT_GATE)
				{
					this.CardOut();
				}
			}

			this.CheckDriverMessages(false);
		}

		public override void ProcessMessageTimeout()
		{
			switch (this.StationState)
			{
				case StationState.PIN_PROMPT:
					this.DisplayMessage("[LoadRack|PIN] [LoadRack|Timeout]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.IDLE;
					return;
			}

			base.ProcessMessageTimeout();
		}
	}
}
