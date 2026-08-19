/******************************************************************************

	FILE NAME:		AcculoadIIIStationManager.cs


	PURPOSE:			AcculoadIIIStationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		16-Mar-2005	W.Gray		7.1.0.1 - Removed Comm Fail processing for Keypad Data

		01-Jan-2007	W.Gray		7.1.0.2 - Revised to use Force Full Screen View
										rather than Autofocus (CSI-4079)

		10-Mar-2008	W.Gray		7.4.0.0 - Revised to Force Full Screen View on PromptForPIN
										(CSI 5639)
										
		11-Mar-08	B. Schaal	7.4.0.0 - CSI 5556 - Added protected override void ProcessShipTo( string Response )

		15-May-08	W.Gray		7.4.3.0 - Added Additive Meter Transactions
		
		10-Jun-08	I.Orndorff	7.4.5.0 - Modified "ReadAdditiveBatchData() to use
										  SiteManager.Site.AdditiveVolumeUnits instead
										  of SiteManager.Site.VolumeUnits for PV.ServerUnits.

		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

		08/19/2008	W.Gray		7.4.5.1 - Changed CreateMeterReadingTransactions to create
										transactions regardless of LoadArm.Enabled (CSI 6099)

		9/09/2008	W.Gray		7.4.6.0 - Revised to support external components (CSI 5581)

		12/15/2008  W.Gray		7.4.6.1 - Revised to store values in maximum precision (CSI 6239)

		12/24/2008	W.Gray		7.4.6.2 - Added Support for Internal Additive Meter Totalizers (CSI 6341)

		01/28/2009	W.Gray		7.4.6.3 - Revised to revert back to TAS precision

		03/02/2009	W.Gray		7.4.6.9 - Revised to read CTL in ReadComponentBatchData (CSI 1794)
									  
*******************************************************************************/

namespace LoadRackLibrary
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using Opc;
	using Opc.Da;

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Net;
	using System.Runtime.InteropServices;
	using System.Threading;
	using System.Linq;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Factory = OpcCom.Factory;
	using Server = Opc.Da.Server;
    using Opc.Ua;

    /// <summary>
    /// Summary description for AcculoadIIIStationManagerClass.
    /// </summary>
    public class AcculoadIIIStationManagerClass : StationManagerClass
	{
		protected string SelectedProductID = "";

		protected ProcessVariableClass ResetCardReaderDataPv;
		protected ProcessVariableClass CardReaderDataPv;

		protected ArrayList PowerfailAlarms = new ArrayList();

		internal const int MaxRecipes = 50;

		public AcculoadIIIStationManagerClass(EventLog eventLogParam,
															LoadRackManagerClass loadRackManager,
															StationClass station,
															SiteManagerClass siteManager,
															SecurityClass security)
			: base(eventLogParam, loadRackManager, station, siteManager, security)
		{
			Monitor.Enter(this);

			try
			{

				// Configure the PV's associated with the Station
				if (this.StationPv.URL != ""
				&& station.CardReader)
				{
					this.ResetCardReaderDataPv = new ProcessVariableClass(
					  PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
					  UNIT_TYPE.STATION_UNIT,
					  VarEnum.VT_EMPTY,
					  false,
					  this.StationPv.OPCItemID + ".Card Reader.Reset Data",
					  this.StationPv.URL,
					  this.StationPv.ProgID);

					this.CardReaderDataPv = new ProcessVariableClass(
					  PROCESS_VARIABLE_TYPE.CARDREADER_PV,
					  UNIT_TYPE.STATION_UNIT,
					  VarEnum.VT_BSTR,
					  true,
					  this.StationPv.OPCItemID + ".Card Reader.Data",
					  this.StationPv.URL,
					  this.StationPv.ProgID);

					this.OPCServerManager.AddProcessVariable(this.CardReaderDataPv);
				}

				// Initial date & time sync
				this.SyncDateAndTime();

				// If the Check Device Status found a transaction in progress at the device, it will set the station
				// status to "Transaction In Progress".  In that case we do not want to reset the device as it
				// will yank the station out from under the in progress transaction
				if (this.StationState != StationState.TRANSACTION_IN_PROGRESS)
				{
					this.SendEndTransaction();

					if (station.CardReader)
					{
						this.ResetCardReaderData();
					}
					else
					{
						if (this.AvailableLoadArmManagers != 0)
						{
							this.IssueDriverIDPrompt();
						}
					}
				}
			}

			catch (Exception e)
			{
				this.CommunicationsFailure = true;
				eventLogParam.WriteEntry("Accuload III StationManager : " + e.Message + e.StackTrace, EventLogEntryType.Error);
				this.StationState = StationState.IDLE;
			}

			finally
			{
				Monitor.Exit(this);
			}
		}

        /// <summary>
        /// Returns the number of actual load arms on a preset.
        /// </summary>
        internal override int PhysicalArmsOnPreset
        {
            get 
			{
				int numberOfArms = 0;
                ProcessVariableClass stationPv = this.Station.ProcessVariableCollection[0];
                Server server = new Server(new Factory(), new URL(stationPv.URL));
                server.Connect(new ConnectData(null));

                string tag = stationPv.OPCItemID + ".Number Of Arms";

                Item[] Items ={ new Item(new ItemIdentifier(tag)),
                            };

                ItemValueResult[] Values = server.Read(Items);

                if (Values[0].Quality == Quality.Good)
                {
                    numberOfArms = System.Convert.ToInt32(Values[0].Value);
                }

				return numberOfArms;
            }
        }

        public override void ResetStationDevice()
		{
			base.ResetStationDevice();

			this.LoadArmManagerCollection.ReleaseKeyPad(this);

			if (this.Station.CardReader)
			{
				this.ResetCardReaderData();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}



		public override void CancelUnauthorizedTransaction()
		{
			if (this.IsTransactionInProgress())
			{
				try
				{
					this.eventLog.WriteEntry("Unauthorized Transaction in Progress: Station " + this.Station.ID, EventLogEntryType.Error);
					this.LoadArmManagerCollection.Stop(this);
					this.SendEndTransaction();
					this.ResetStationDevice();
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Accuload III StationManager CancelUnauthorizedTransaciton : " + e.Message + e.StackTrace, EventLogEntryType.Error);
				}
			}
		}



		public override bool RegisterPowerfail(string name)
		{
			foreach (string alarmName in this.PowerfailAlarms)
			{
				if (alarmName == name)
				{
					return false;
				}
			}

			this.PowerfailAlarms.Add(name);

			return true;

		}

		public override bool SendEndOfDayOrMonthWarningMessagesDuringLoading { get { return false; } }

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool saveForCancelProcessing)
		{
			if (this.AvailableLoadArmManagers == 0)
			{
				throw new OpcException("No Load Arms Available");
			}

			if (messageTimeout > 999)
			{
				messageTimeout = 999;
			}

			// Always use variable length input strings
			if (responseLength > 0 && responseLength < 40)
			{
				responseLength += 40;
			}

			string message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, stockMessage)
																);

			string armDisabled = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Arm Disabled")
																);

			// When Station has Swing Arms provide Bay Prefix
			if (this.HasSwingArms)
			{
				message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 // ReSharper disable once AccessToModifiedClosure
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + this.Station.SwingArmPosition + ": " + message
																);

				armDisabled = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 // ReSharper disable once AccessToModifiedClosure
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + this.Station.SwingArmPosition + ": " + armDisabled
																);

			}

			string[] writeTags = this.MenuWriteTags;

			char[] separator = { ' ' };
			string[] strings = message.Split(separator);
			string[] lines = new string[this.NumberOfDisplayLines];
			int lineIndex = 0;
			foreach (string subMessage in strings)
			{
				if (subMessage.Length > this.MaxDisplayLineSize)
				{
					break;
				}

				if (lines[lineIndex] == null)
				{
					lines[lineIndex] = subMessage;
				}
				else if (lineIndex == lines.Length - 1
					|| lines[lineIndex + 1] == null)
				{
					if (lines[lineIndex].Length + subMessage.Length + 1 < this.MaxDisplayLineSize)
					{
						lines[lineIndex] = lines[lineIndex] + " " + subMessage;
					}
					else
					{
						lineIndex++;
						if (lineIndex > lines.Length - 1)
						{
							break;
						}

						lines[lineIndex] = subMessage;
					}
				}

				else
				{
					lineIndex++;
					if (lines[lineIndex].Length + subMessage.Length + 1 < this.MaxDisplayLineSize)
					{
						lines[lineIndex] = lines[lineIndex] + " " + subMessage;
					}
					else
					{
						break;
					}
				}
			}

			// Output Station Message to all Arms 
			int itemIndex = 0;
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.IsInAlarm)
				{
					continue;
				}

				ArrayList itemValues = new ArrayList();

				ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];

				itemIndex = 0;

				foreach (string line in lines)
				{
					if (line == null)
					{
						break;
					}

					// Per Accuload III/IV documentation, last prompt command sent governs the response terminator and length. 
					// Therefore, we send the response terminator and length on ALL lines
					ItemValue writeLine = new ItemValue(loadArmPv.OPCItemID + writeTags[itemIndex])
					{
						Value = " " + messageTimeout.ToString("D3") + " " + line + "&" + responseLength.ToString("D2")
					};

					itemValues.Add(writeLine);
					itemIndex++;
				}

				this.OPCServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));

				loadArmManager.ResponsePending = true;
			}

			if (saveForCancelProcessing)
			{
				this.SaveMessageValues(stockMessage, responseLength, messageTimeout);
			}

			return itemIndex;
		}

		public override string AcknowledgementMessage { get { return "[LoadRack|Press Enter to Acknowledge]"; } }

		public override int AcknowledgementResponseLength { get { return 41; } }

		public override bool NumericMenuSelection { get { return true; } }

		protected virtual int NumberOfDisplayLines { get { return 4; } }

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
			if (this.AvailableLoadArmManagers == 0)
			{
				throw new OpcException("No Load Arms Available");
			}

			if (messageTimeout > 999)
			{
				messageTimeout = 999;
			}

			// Always use variable length input strings
			if (responseLength > 0 && responseLength < 40)
			{
				responseLength += 40;
			}

			string Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, stockMessage)
																);


			// When Station has Swing Arms provide Bay Prefix
			if (this.HasSwingArms)
			{
				Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Bay") + " " + this.Station.SwingArmPosition + ": " + Message
																);
			}

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
				{
					continue;
				}

				if (LoadArmManager.IsInAlarm)
				{
					continue;
				}

				ProcessVariableClass LoadArmPV = LoadArmManager.LoadArm.ProcessVariableCollection[0];

				ArrayList ItemValues = new ArrayList();

				ItemValue WriteLine = new ItemValue(LoadArmPV.OPCItemID + ".Write First Line With Prompt No Echo")
				{
					Value = " " + messageTimeout.ToString("D3") + " " + Message + "&" + responseLength.ToString("D2")
				};
				ItemValues.Add(WriteLine);

				this.OPCServerManager.Write(new URL(LoadArmPV.URL), (ItemValue[])ItemValues.ToArray(typeof(ItemValue)));

				LoadArmManager.ResponsePending = true;
			}

			this.SaveMessageValues(stockMessage, responseLength, messageTimeout);
		}


		protected override void ResetCardReaderData()
		{
			this.OPCServerManager.Write(this.ResetCardReaderDataPv);
		}

		public override void ReleaseKeyPad()
		{
			if (!this.HasSwingArms)
			{
				this.LoadArmManagerCollection.Item(0).ReleaseKeyPad();
			}
			else
			{
				this.LoadArmManagerCollection.ReleaseKeyPad(this);
			}
		}

		protected override void Unauthorize()
		{
			base.Unauthorize();
		}

		public override string GetDateTimeSettingCommand()
		{
			// Format the current date
			DateTimeOffset Now = DateTimeOffset.Now;
			string sValue;

			// Format the current time
			int nHour = Now.Hour;
			string sMeridian = "A";

			if (this.SiteManager.Site.TimePattern.IndexOf("H") != -1)
			{
				sValue = Now.Day.ToString("00") + Now.Month.ToString("00") + Now.Year.ToString("0000");
				sMeridian = "M";
			}

			else
			{
				sValue = Now.Month.ToString("00") + Now.Day.ToString("00") + Now.Year.ToString("0000");
				if (nHour > 12)
				{
					nHour -= 12;
					sMeridian = "P";
				}

				if (nHour == 12)
				{
					sMeridian = "P";
				}

				if (nHour == 0)
				{
					nHour = 12;
				}
			}

			sValue += " " + nHour.ToString("00") + Now.Minute.ToString("00") + " " + sMeridian;

			return sValue;

		}

		public void ProcessPowerfail(ProcessVariableClass PV)
		{
			if (PV.IsQualityGood
				&& (bool)PV.ServerValue == true)
			{

				// Log the powerfailure
				string sMessage = "Power failure alarm detected on device: " + this.Station.ID;
				sMessage += ".  This alarm is triggered when power is restored to the device.";
				this.eventLog.WriteEntry(sMessage, EventLogEntryType.Warning);

				// Issue an alarm
				FMChannelHelper.MakeCall<IAlarmAndEventLogs>(
																	 x =>
																	 x.Add(this.Security, this.Station.DevicePowerFailureAlarm)
																);

				// Attempt to set the date/time
				this.SyncDateAndTime();

				// Recover if we were loading
				if (this.Transaction != null)
				{
					// The device has some capability to persist the transaction that was in progress when power was lost.  If 
					// the device thinks there is a transaction in progress, we do not want to reset.  In that case, our internal
					// setup should still be valid.
					if (this.IsTransactionInProgress() == false)
					{
						// Log the event
						this.eventLog.WriteEntry("Aborting transaction in progress during power failure: " + this.Transaction.TransID, EventLogEntryType.Error);

						// Set up a note in the transaction
						string Message = "[LoadRack|Power failure during loading.]";
						Message = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, Message)
																);

						this.Transaction.Notes += "\n***" + Message;

						// Complete the transaction and idle the station
						this.CompleteTransaction();
						this.Transaction = null;

						this.StationState = StationState.IDLE;
					}
					else
					{
						this.eventLog.WriteEntry("Attempting to reconnect to transaction already in progress on device: " + this.Transaction.TransID,
							EventLogEntryType.Information);
					}

				}

				// Reset the power fail alarm; otherwise, we will continue to get power fail events
				this.LoadArmManagerCollection.ResetPowerFailAlarm(this);

				// CSI 4716 - Reissue station prompt if responding to a powerfail message
				if (this.Station.CardReader == false && this.Transaction == null)
				{
					this.IssueDriverIDPrompt();
				}
				else
				{
					this.LoadArmManagerCollection.ProcessMessageTimeout(this);
				}
			}
		}

		protected override void LoadRackProcessing(ProcessVariableClass PV)
		{
			switch (PV.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						if (PV.IsQualityGood)
						{
							if ((string)PV.ServerValue != "")
							{
								// Driver Card
								if (this.StationState == StationState.IDLE)
								{
									this.ProcessDriverID((string)PV.ServerValue);
								}

								// LoadID Card
								else if (this.StationState == StationState.LOADID_CARD_PROMPT
								|| this.StationState == StationState.LOADID_PROMPT)
								{
									this.ProcessLoadIDCard((string)PV.ServerValue);
								}

								// If we got a card scan and we are not in a state where we expect one,
								// reset the station.
								else if (this.StationState != StationState.AUTHORIZING
								&& this.StationState != StationState.AUTHORIZED
								&& this.StationState != StationState.TRANSACTION_IN_PROGRESS)
								{
									this.StationState = StationState.IDLE;
								}

								// If any scan card processing fails, the station state will be set to IDLE
								// indicating that we need to reset the station.
								if (this.StationState == StationState.IDLE)
								{
									this.ResetStationDevice();
								}
							}
						}

						break;
					}


				default:
					base.LoadRackProcessing(PV);
					break;
			}

		}


		protected virtual int MaxSelection
		{
			get { return 3; }
		}

		public virtual bool IsPowerFailureAlarmActive()
		{
			Server Server = new Server(new Factory(), new URL(this.StationPv.URL));
			Server.Connect();

			Item[] SubItems = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Status.Power-fail Occurred")) };

			ItemValueResult[] Values = Server.Read(SubItems);

			Server.Disconnect();
			Server.Dispose();

			return (System.Convert.ToBoolean(Values[0].Value) == true);

		}


		protected virtual string[] MenuWriteTags
		{
			get
			{
				string[] Val = { ".Write First Line With Prompt",
										".Write Second Line",
										".Write Third Line",
										".Write Fourth Line" };

				return Val;

			}

		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			if (this.AvailableLoadArmManagers == 0)
			{
				throw new OpcException("No Load Arms Available");
			}

			if (parameters.MenuTimeout > 999)
			{
				parameters.MenuTimeout = 999;
			}

			// Write the title line
			int numberOfLines = this.DisplayMessage(parameters.Caption, null, 1, this.PROMPT_TIMEOUT, false);

			string[] writeTags = this.MenuWriteTags;

			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.IsInAlarm)
				{
					continue;
				}

				ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];

				ArrayList itemValues = new ArrayList();

				for (int nLoop = 0; nLoop < parameters.Menu.Length && nLoop < writeTags.Length - numberOfLines; ++nLoop)
				{
					ItemValue writeListItem = new ItemValue(loadArmPv.OPCItemID + writeTags[nLoop + numberOfLines]);

					string value = parameters.Menu[nLoop];
					if (parameters.ApplyDataDictionary)
					{
						value = this.GetDataDictionaryValueByKey(this.Station.SiteGuid, value);
					}

					value = (nLoop + 1) + ". " + value + "                                       ";
					if (value.Length > this.MaxDisplayLineSize)
					{
						value = value.Substring(0, this.MaxDisplayLineSize);
					}

					// Add in the response & timeout
					// Per Accuload III/IV documentation, last prompt command sent governs the response terminator and length
					// Therefore, we send the response terminator and length on ALL lines
					writeListItem.Value = " " + parameters.MenuTimeout.ToString("D3") + " " + value + "&41";

					itemValues.Add(writeListItem);

				}

				this.OPCServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
			}

			if (parameters.SaveForCancelProcessing)
			{
				this.CurrentMenuParameters = parameters;
			}
		}

		protected virtual int MaxDisplayLineSize
		{
			get { return 29; }
		}

		protected string GetStationOPCPath()
		{
			ProcessVariableClass StationPV = this.Station.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string OPCPath = StationPV.OPCItemID;
			int nLastIndex = StationPV.OPCItemID.IndexOf(".");

			if (nLastIndex > 0)
			{
				OPCPath = StationPV.OPCItemID.Substring(0, nLastIndex);
			}

			return OPCPath;
		}

		private void ReadBatchData(
			LineItemDO lineItem,
			Server server,
			AcculoadIIILoadArmManagerClass loadArmManager)
		{
			if (loadArmManager == null)
			{
				throw new ArgumentNullException(nameof(loadArmManager));
			}

			ItemValueResult grossVolume;
			ItemValueResult temperature;
			ItemValueResult netVolume;
			ItemValueResult mass;
			ItemValueResult pressure;

			loadArmManager.ReadBatchData(lineItem.BatchNumber,
			server,
			out grossVolume,
			out temperature,
			out netVolume,
			out pressure);

			if (lineItem.Quantity == null)
			{
				lineItem.Quantity = new QuantityDO();
			}

			loadArmManager.ReadBatchMass(lineItem.BatchNumber, server, out mass);

			if (this.Station.Type == STATION_TYPE.OFF_LOADING) // for off loading we store the manually entered data instead of the actual data
			{
				lineItem.Quantity.GrossInventoryChange = Math.Round(System.Convert.ToDouble(this.OffLoadPresetAmount), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			}
			else
			{
				if (grossVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadGrossQualityLogged)
					{
						this.eventLog.WriteEntry("ReadBatchData : GrossVolume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadGrossQualityLogged = true;
					}
				}
				else
				{
					lineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(grossVolume.Value), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					lineItem.Quantity.BadGrossQualityLogged = false;
				}
			}

			if (mass.Quality != Quality.Good)
			{
				if (!lineItem.Quantity.BadMassQualityLogged)
				{
					this.eventLog.WriteEntry(
						 "ReadBatchData : Mass OPC Quality Bad " + mass.ItemName, EventLogEntryType.Error);
					lineItem.Quantity.BadMassQualityLogged = true;
				}
			}
			else
			{
				lineItem.Quantity.MassInventoryChange = (this.Station.Type == STATION_TYPE.OFF_LOADING ? 1.0 : -1.0) * Math.Round(System.Convert.ToDouble(mass.Value), lineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
				lineItem.Quantity.BadMassQualityLogged = false;
			}

			if (lineItem.Temperature == null)
			{
				lineItem.Temperature = 0.0;
			}

			if (temperature.Quality != Quality.Good)
			{
				if (!lineItem.Temperature_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadBatchData : Temperature OPC Quality Bad " + temperature.ItemName, EventLogEntryType.Error);
					lineItem.Temperature_BadQualityLogged = true;
				}
			}
			else
			{
				lineItem.Temperature = Math.Round(System.Convert.ToDouble(temperature.Value), lineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
				lineItem.Temperature_BadQualityLogged = false;
			}

			if (this.Station.Type == STATION_TYPE.OFF_LOADING) // for off loading we store the manually entered data instead of the actual data
			{
				lineItem.Quantity.NetInventoryChange = Math.Round(System.Convert.ToDouble(this.OffLoadPresetAmount), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
			}
			else
			{
				if (netVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadNetQualityLogged)
					{
						this.eventLog.WriteEntry("ReadBatchData : Net Volume OPC Quality Bad " + netVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadNetQualityLogged = true;
					}
				}
				else
				{
					lineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(netVolume.Value), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					lineItem.Quantity.BadNetQualityLogged = false;
				}
			}

			if (pressure.Quality != Quality.Good)
			{
				if (!lineItem.Pressure_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadBatchData : Pressure OPC Quality Bad " + pressure.ItemName, EventLogEntryType.Error);
					lineItem.Pressure_BadQualityLogged = true;
				}
			}
			else
			{
				lineItem.Pressure = Math.Round(System.Convert.ToDouble(pressure.Value), lineItem.PressureDecimalPlaces, MidpointRounding.AwayFromZero);
				lineItem.Pressure_BadQualityLogged = false;
			}




			if (lineItem.VCF == null)
			{
				lineItem.VCF = 0.0;
			}

			// ReSharper disable once CompareOfFloatsByEqualityOperator
			if (lineItem.Quantity.GrossInventoryChange != 0)
			{
				lineItem.VCF = Math.Round(lineItem.Quantity.NetInventoryChange / lineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
			}
		}

		/// <summary>
		/// Reads component batch data (subline items).
		/// </summary>
		/// <param name="component">
		/// The component for which to read data.
		/// </param>
		/// <param name="subLineItem">
		/// the subline item into which the component readings will be saved.
		/// </param>
		/// <param name="server">
		/// OPC Server reference.
		/// </param>
		/// <param name="loadArmManager">
		/// The load arm manager for the load arm the referenced batch is on.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// If the load arm manager is null.
		/// </exception>
		protected void ReadComponentBatchData(
		 ProductMapClass component,
		 SubLineItemDO subLineItem,
		 Server server,
		 AcculoadIIILoadArmManagerClass loadArmManager)
		{
			if (loadArmManager == null)
			{
				throw new ArgumentNullException(nameof(loadArmManager));
			}

			ItemValueResult grossVolume;
			ItemValueResult standardDensity;
			ItemValueResult temperature;
			ItemValueResult netVolume;
			ItemValueResult ctl;
			ItemValueResult pressure;
			ItemValueResult mass;

			loadArmManager.ReadComponentBatchData(
			component.PresetNumber,
			server,
			out grossVolume,
			out standardDensity,
			out temperature,
			out netVolume,
			out ctl,
			out pressure);

			loadArmManager.ReadComponentBatchMass(component.PresetNumber, server, out mass);

			if (subLineItem.Quantity == null)
			{
				subLineItem.Quantity = new QuantityDO();
			}

			if (grossVolume.Quality != Quality.Good)
			{
				if (!subLineItem.Quantity.BadGrossQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : GrossVolume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
					subLineItem.Quantity.BadGrossQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(grossVolume.Value), subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				subLineItem.Quantity.BadGrossQualityLogged = false;
			}

			if (subLineItem.Density == null)
			{
				subLineItem.Density = 0.0;
			}

			if (standardDensity.Quality != Quality.Good)
			{
				if (!subLineItem.Density_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : Standard Density OPC Quality Bad " + standardDensity.ItemName, EventLogEntryType.Error);
					subLineItem.Density_BadQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Density = Math.Round(System.Convert.ToDouble(standardDensity.Value), subLineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
				subLineItem.Density_BadQualityLogged = false;
			}


			if (subLineItem.Temperature == null)
			{
				subLineItem.Temperature = 0.0;
			}

			if (temperature.Quality != Quality.Good)
			{
				if (!subLineItem.Temperature_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : Temperature OPC Quality Bad " + temperature.ItemName, EventLogEntryType.Error);
					subLineItem.Temperature_BadQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Temperature = Math.Round(System.Convert.ToDouble(temperature.Value), subLineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
				subLineItem.Temperature_BadQualityLogged = false;
			}

			if (mass.Quality != Quality.Good)
			{
				if (!subLineItem.Quantity.BadMassQualityLogged)
				{
					this.eventLog.WriteEntry(
						 "ReadComponentBatchData : Temperature OPC Quality Bad " + temperature.ItemName,
						 EventLogEntryType.Error);
					subLineItem.Quantity.BadMassQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Quantity.MassInventoryChange = -Math.Round(System.Convert.ToDouble(mass.Value), subLineItem.MassDecimalPlaces, MidpointRounding.AwayFromZero);
				subLineItem.Quantity.BadMassQualityLogged = false;
			}

			if (netVolume.Quality != Quality.Good)
			{
				if (!subLineItem.Quantity.BadNetQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : Net Volume OPC Quality Bad " + netVolume.ItemName, EventLogEntryType.Error);
					subLineItem.Quantity.BadNetQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(netVolume.Value), subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
				subLineItem.Quantity.BadNetQualityLogged = false;
			}

			if (subLineItem.VCF == null)
			{
				subLineItem.VCF = 0.0;
			}

			if (ctl.Quality != Quality.Good)
			{
				if (!subLineItem.VCF_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : CTL OPC Quality Bad " + ctl.ItemName, EventLogEntryType.Error);
					subLineItem.VCF_BadQualityLogged = true;
				}
			}
			else
			{
				subLineItem.VCF = System.Convert.ToDouble(ctl.Value);
				subLineItem.VCF_BadQualityLogged = false;
			}

			if (pressure.Quality != Quality.Good)
			{
				if (!subLineItem.Pressure_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentBatchData : Pressure OPC Quality Bad " + ctl.ItemName, EventLogEntryType.Error);
					subLineItem.Pressure_BadQualityLogged = true;
				}
			}
			else
			{
				subLineItem.Pressure = System.Convert.ToDouble(pressure.Value);
				subLineItem.Pressure_BadQualityLogged = false;
			}
		}


		protected void ReadAdditiveBatchData(
			ProductMapClass additiveInjector,
			SubLineItemDO subLineItem,
			Server server,
			AcculoadIIILoadArmManagerClass loadArmManager)
		{
			if (loadArmManager == null)
			{
				throw new ArgumentNullException(nameof(loadArmManager));
			}

			SiteClass site = this.SiteManager.Site;
			ItemValueResult additiveGrossVolume;
			ProcessVariableClass pv;

			if (string.IsNullOrEmpty(subLineItem.BatchNumber))
			{
				// We might be loading from a preload but not yet be flowing, in which case 
				// there is no batch number yet.
				return;
			}

			TankClass tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
			if (tank == null)
			{
				this.eventLog.WriteEntry("ReadAdditiveBatchData : No Additive Tank", EventLogEntryType.Error);
			}

			if (additiveInjector.Type == PRODUCT_MAP_TYPE.PRESET_INJECTOR_MAP)
			{
				loadArmManager.ReadBatchAdditiveData(
					 subLineItem.BatchNumber,
					 additiveInjector.PresetNumber,
					 server,
					 out additiveGrossVolume);


				if (additiveGrossVolume.Quality != Quality.Good)
				{
					if (!subLineItem.Quantity.BadGrossQualityLogged)
					{
						this.eventLog.WriteEntry(
							 "ReadAdditiveBatchData : Additive Volume OPC Quality Bad " + additiveGrossVolume.ItemName,
							 EventLogEntryType.Error);
						subLineItem.Quantity.BadGrossQualityLogged = true;
					}
				}
				else
				{
					subLineItem.Quantity.GrossInventoryChange =
						 -Math.Round(
							  System.Convert.ToDouble(additiveGrossVolume.Value),
							  subLineItem.VolumeDecimalPlaces,
							  MidpointRounding.AwayFromZero);
					subLineItem.Quantity.BadGrossQualityLogged = false;
				}

				if (tank != null)
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
							this.eventLog.WriteEntry(
								 "ReadAdditiveBatchData : No Tank Temperature Process Variable",
								 EventLogEntryType.Error);
							subLineItem.Temperature_BadQualityLogged = true;
						}
					}
					else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
					{
						if (!subLineItem.Temperature_BadQualityLogged)
						{
							this.eventLog.WriteEntry(
								 "ReadAdditiveBatchData : Additive Temperature OPC Quality Bad " + pv.OPCItemID,
								 EventLogEntryType.Error);
							subLineItem.Temperature_BadQualityLogged = true;
						}
					}
					else
					{
						EngineeringUnit units = (this.CurrentTransactionAlias.TemperatureUnits != 0)
															  ? this.CurrentTransactionAlias.TemperatureUnits
															  : site.TemperatureUnits;
						SIDouble temperature = new SIDouble
						{
							Units = units,
							SIValue = System.Convert.ToDouble(pv.SIValue)
						};

						subLineItem.Temperature = temperature.Value;
						subLineItem.Temperature_BadQualityLogged = false;
					}


					if (subLineItem.Quantity == null)
					{
						subLineItem.Quantity = new QuantityDO();
					}

					if (subLineItem.VCF == null)
					{
						subLineItem.VCF = 0.0;
					}

					pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VCF_PV];
					if (pv == null)
					{
						if (!subLineItem.Quantity.BadNetQualityLogged)
						{
							this.eventLog.WriteEntry(
								 "ReadAdditiveBatchData : No Tank VCF Process Variable",
								 EventLogEntryType.Error);
							subLineItem.Quantity.BadNetQualityLogged = true;
						}
					}
					else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
					{
						if (!subLineItem.Quantity.BadNetQualityLogged)
						{
							this.eventLog.WriteEntry(
								 "ReadAdditiveBatchData : Additive VCF OPC Quality Bad " + pv.OPCItemID,
								 EventLogEntryType.Error);
							subLineItem.Quantity.BadNetQualityLogged = true;
						}
					}
					else
					{
						subLineItem.VCF = System.Convert.ToDouble(pv.SIValue);
						subLineItem.Quantity.NetInventoryChange =
							 Math.Round(
								  subLineItem.Quantity.GrossInventoryChange * subLineItem.VCF.Value,
								  subLineItem.VolumeDecimalPlaces,
								  MidpointRounding.AwayFromZero);
						subLineItem.Quantity.BadNetQualityLogged = false;
					}
				}
			}
			else
			{
				ItemValueResult additiveNetVolume;
				ItemValueResult averageTemperature;
				ItemValueResult averageCtl;
				ItemValueResult additiveMass;
				loadArmManager.ReadBatchFlowControlledAdditiveData(
					 additiveInjector.PresetNumber,
					 server,
					 out additiveGrossVolume,
					 out averageTemperature,
					 out additiveNetVolume,
					 out averageCtl);
				loadArmManager.ReadBatchFlowControlledAdditiveMass(
					 additiveInjector.PresetNumber,
					 server,
					 out additiveMass);

				if (additiveGrossVolume.Quality != Quality.Good)
				{
					if (!subLineItem.Quantity.BadGrossQualityLogged)
					{
						this.eventLog.WriteEntry("ReadAdditiveBatchData : Additive Volume OPC Quality Bad " + additiveGrossVolume.ItemName, EventLogEntryType.Error);
						subLineItem.Quantity.BadGrossQualityLogged = true;
					}
				}
				else
				{
					byte decimalPlaces = (this.CurrentTransactionAlias.AdditiveVolumeUnits != 0) ? this.CurrentTransactionAlias._AdditiveVolumeDecimalPlaces : site._AdditiveVolumeDecimalPlaces;
					subLineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(additiveGrossVolume.Value), decimalPlaces);
					subLineItem.Quantity.BadGrossQualityLogged = false;
				}

				if (subLineItem.Temperature == null)
				{
					subLineItem.Temperature = 0.0;
				}

				if (averageTemperature.Quality != Quality.Good)
				{
					if (!subLineItem.Temperature_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadAdditiveBatchData : Average Temperature OPC Quality Bad " + averageTemperature.ItemName, EventLogEntryType.Error);
						subLineItem.Temperature_BadQualityLogged = true;
					}
				}
				else
				{
					byte decimalPlaces = (this.CurrentTransactionAlias.TemperatureUnits != 0) ? this.CurrentTransactionAlias._TemperatureDecimalPlaces : site._TemperatureDecimalPlaces;
					subLineItem.Temperature = Math.Round(System.Convert.ToDouble(averageTemperature.Value), decimalPlaces);
					subLineItem.Temperature_BadQualityLogged = false;
				}

				if (additiveNetVolume.Quality != Quality.Good)
				{
					if (!subLineItem.Quantity.BadNetQualityLogged)
					{
						this.eventLog.WriteEntry("ReadAdditiveBatchData : Net Volume OPC Quality Bad " + additiveNetVolume.ItemName, EventLogEntryType.Error);
						subLineItem.Quantity.BadNetQualityLogged = true;
					}
				}
				else
				{
					byte decimalPlaces = (this.CurrentTransactionAlias.VolumeUnits != 0) ? this.CurrentTransactionAlias._VolumeDecimalPlaces : site._VolumeDecimalPlaces;
					subLineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(additiveNetVolume.Value), decimalPlaces);
					subLineItem.Quantity.BadNetQualityLogged = false;
				}

				if (additiveMass.Quality != Quality.Good)
				{
					if (!subLineItem.Quantity.BadMassQualityLogged)
					{
						this.eventLog.WriteEntry("ReadAdditiveBatchData : Mass OPC Quality Bad " + additiveMass.ItemName, EventLogEntryType.Error);
						subLineItem.Quantity.BadMassQualityLogged = true;
					}
				}
				else
				{
					byte decimalPlaces = (this.CurrentTransactionAlias.MassUnits != 0) ? this.CurrentTransactionAlias._MassDecimalPlaces : site._MassDecimalPlaces;
					subLineItem.Quantity.MassInventoryChange = -Math.Round(System.Convert.ToDouble(additiveMass.Value), decimalPlaces);
					subLineItem.Quantity.BadMassQualityLogged = false;
				}

				if (subLineItem.VCF == null)
				{
					subLineItem.VCF = 0.0;
				}

				if (averageCtl.Quality != Quality.Good)
				{
					if (!subLineItem.VCF_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadAdditiveBatchData : Average CTL OPC Quality Bad " + averageCtl.ItemName, EventLogEntryType.Error);
						subLineItem.VCF_BadQualityLogged = true;
					}
				}
				else
				{
					subLineItem.VCF = System.Convert.ToDouble(averageCtl.Value);
					subLineItem.VCF_BadQualityLogged = false;
				}
			}

			if (tank != null)
			{
				if (subLineItem.Density == null)
				{
					subLineItem.Density = 0.0;
				}

				pv = tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
				if (pv == null)
				{
					if (!subLineItem.Density_BadQualityLogged)
					{
						this.eventLog.WriteEntry(
							 "ReadAdditiveBatchData : No Tank Density Process Variable",
							 EventLogEntryType.Error);
						subLineItem.Density_BadQualityLogged = true;
					}
				}

				else if ((!this.SiteManager.Site.UseLastKnownGoodTankData && !pv.IsQualityGood) || !(pv.SIValue is double))
				{
					if (!subLineItem.Density_BadQualityLogged)
					{
						this.eventLog.WriteEntry(
							 "ReadAdditiveBatchData : Additive Density OPC Quality Bad " + pv.OPCItemID,
							 EventLogEntryType.Error);
						subLineItem.Density_BadQualityLogged = true;
					}
				}
				else
				{
					EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0)
														  ? this.CurrentTransactionAlias.DensityUnits
														  : site.DensityUnits;
					SIDouble density = new SIDouble { Units = units, SIValue = System.Convert.ToDouble(pv.SIValue) };

					subLineItem.Density = density.Value;
					subLineItem.Density_BadQualityLogged = false;
				}
			}
		}

		protected override byte ReadAdditiveProductsUsingInjector(
		 ProductMapClass AdditiveInjector,
		 Server Server,
		 LoadArmManagerClass LoadArmManager)
		{
			if (LoadArmManager == null)
			{
				throw new ArgumentNullException("LoadArmManager");
			}


            ((AcculoadIIILoadArmManagerClass)LoadArmManager).ReadAdditiveProductsUsingInjector(
                AdditiveInjector.PresetNumber,
                Server,
                out ItemValueResult ProductsUsingInjector);

            if (ProductsUsingInjector == null)
			{
				return 0xFF;
			}

			if (ProductsUsingInjector.ResultID != ResultID.S_OK)
			{
				throw new Exception("ReadAdditiveProductsUsingInjector : " + ProductsUsingInjector.ItemName + " " + ProductsUsingInjector.ResultID.ToString());
			}

			if (ProductsUsingInjector.Quality != Quality.Good)
			{
				throw new Exception("ReadAdditiveProductsUsingInjector : OPC Quality Bad " + ProductsUsingInjector.ItemName);
			}

			return (byte)ProductsUsingInjector.Value;
		}

		protected void ReadProductNonResettableTotal(
			ProductMapClass Component,
			LineItemDO LineItem,
			Server Server,
			AcculoadIIILoadArmManagerClass LoadArmManager)
		{
			if (LoadArmManager == null)
			{
				throw new ArgumentNullException("LoadArmManager");
			}


            LoadArmManager.ReadProductNonResettableTotal(Component.PresetNumber, Server, out ItemValueResult NonResettableGrossVolume);

            if (NonResettableGrossVolume.Quality != Quality.Good)
			{
				if (LineItem.MeterReading.MeterStop == null)
				{
					LineItem.MeterReading.MeterStop = 0.0;
				}

				if (!LineItem.MeterReading.MeterStop_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadPresetAmount : Product Non-Resettable Gross Volume OPC Quality Bad " + NonResettableGrossVolume.ItemName, EventLogEntryType.Error);
					LineItem.MeterReading.MeterStop_BadQualityLogged = true;
				}
			}

			else
			{
				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

				if (LineItem.MeterReading.MeterStart == null)
				{
					LineItem.MeterReading.MeterStart = Component.MeterValue;
					LineItem.MeterReading.StartDateTime = siteTimeNow;
					LineItem.MeterReading.MeterStop = System.Convert.ToDouble(NonResettableGrossVolume.Value);
					LineItem.MeterReading.StopDateTime = siteTimeNow;
				}

				if (LineItem.MeterReading.MeterStop.Value != (double)NonResettableGrossVolume.Value)
				{
					LineItem.MeterReading.MeterStop = (double)NonResettableGrossVolume.Value;
					LineItem.MeterReading.StopDateTime = siteTimeNow;
				}
				LineItem.MeterReading.MeterStop_BadQualityLogged = false;

				if (Component.MeterValue != LineItem.MeterReading.MeterStop.Value)
				{
					Component.MeterValue = LineItem.MeterReading.MeterStop.Value;
					this.LastActivityDateTime = DateTimeOffset.Now;
				}
			}
		}

		protected void ReadComponentNonResettableTotal(
			ProductMapClass ArmComponent,
			SubLineItemDO SubLineItem,
			Server Server,
			AcculoadIIILoadArmManagerClass LoadArmManager)
		{
			if (LoadArmManager == null)
			{
				throw new ArgumentNullException("LoadArmManager");
			}

			ItemValueResult NonResettableGrossVolume;

			LoadArmManager.ReadProductNonResettableTotal(ArmComponent.PresetNumber, Server, out NonResettableGrossVolume);

			if (NonResettableGrossVolume.Quality != Quality.Good)
			{
				if (SubLineItem.MeterReading.MeterStop == null)
				{
					SubLineItem.MeterReading.MeterStop = 0.0;
				}

				if (!SubLineItem.MeterReading.MeterStop_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + NonResettableGrossVolume.ItemName, EventLogEntryType.Error);
					SubLineItem.MeterReading.MeterStop_BadQualityLogged = true;
				}
			}
			else
			{
				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

				if (SubLineItem.MeterReading.MeterStart == null)
				{
					SubLineItem.MeterReading.MeterStart = ArmComponent.MeterValue;
					SubLineItem.MeterReading.StartDateTime = siteTimeNow;
					SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(NonResettableGrossVolume.Value);
					SubLineItem.MeterReading.StopDateTime = siteTimeNow;
				}

				if (SubLineItem.MeterReading.MeterStop.Value != (double)NonResettableGrossVolume.Value)
				{
					SubLineItem.MeterReading.MeterStop = (double)NonResettableGrossVolume.Value;
					SubLineItem.MeterReading.StopDateTime = siteTimeNow;
				}
				SubLineItem.MeterReading.MeterStop_BadQualityLogged = false;

				if (ArmComponent.MeterValue != SubLineItem.MeterReading.MeterStop.Value)
				{
					ArmComponent.MeterValue = SubLineItem.MeterReading.MeterStop.Value;
					this.LastActivityDateTime = DateTimeOffset.Now;
				}
			}
		}


		protected void ReadAdditiveNonResettableTotal(
			ProductMapClass AdditiveInjector,
			SubLineItemDO SubLineItem,
			Server Server,
			AcculoadIIILoadArmManagerClass LoadArmManager)
		{
			ItemValueResult NonResettableGrossVolume;

			LoadArmManager.ReadAdditiveNonResettableTotal(
				AdditiveInjector.PresetNumber,
				Server,
				out NonResettableGrossVolume);

			if (NonResettableGrossVolume.ResultID != ResultID.S_OK)
			{
				throw new Exception("ReadAdditiveNonResettableTotal : " + NonResettableGrossVolume.ItemName + " " + NonResettableGrossVolume.ResultID.ToString());
			}

			if (NonResettableGrossVolume.Quality != Quality.Good)
			{
				if (SubLineItem.MeterReading.MeterStop == null)
				{
					SubLineItem.MeterReading.MeterStop = 0.0;
				}

				if (!SubLineItem.MeterReading.MeterStop_BadQualityLogged)
				{
					this.eventLog.WriteEntry("ReadAdditiveNonResettableTotal : Additive Non-Resettable Volume OPC Quality Bad " + NonResettableGrossVolume, EventLogEntryType.Error);
					SubLineItem.MeterReading.MeterStop_BadQualityLogged = true;
				}
			}

			else
			{
				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

				ProcessVariableClass InternalPV = AdditiveInjector.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
				if (InternalPV == null)
				{
					if (SubLineItem.MeterReading.MeterStart == null)
					{
						SubLineItem.MeterReading.MeterStart = AdditiveInjector.MeterValue;
						SubLineItem.MeterReading.StartDateTime = siteTimeNow;
						SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(NonResettableGrossVolume.Value);
						SubLineItem.MeterReading.StopDateTime = siteTimeNow;
					}

					if (SubLineItem.MeterReading.MeterStop.Value != (double)NonResettableGrossVolume.Value)
					{
						SubLineItem.MeterReading.MeterStop = (double)NonResettableGrossVolume.Value;
						SubLineItem.MeterReading.StopDateTime = siteTimeNow;
					}
				}

				else
				{
					double ServerValue = System.Convert.ToDouble(InternalPV.ServerValue);
					double RollOver = System.Convert.ToDouble(InternalPV.GetMaximum(InternalPV.ServerUnits, 10));
					double CurrentMeterValue = System.Convert.ToDouble(NonResettableGrossVolume.Value);

					if (SubLineItem.MeterReading.MeterStart == null)
					{
						SubLineItem.MeterReading.MeterStart = ServerValue;
						SubLineItem.MeterReading.StartDateTime = siteTimeNow;
						SubLineItem.MeterReading.MeterStop = ServerValue;
						SubLineItem.MeterReading.StopDateTime = siteTimeNow;
					}

					ServerValue += CurrentMeterValue - AdditiveInjector.MeterValue;
					if (CurrentMeterValue < AdditiveInjector.MeterValue)
					{
						ServerValue += 999999.999;
					}

					if (ServerValue > RollOver)
					{
						ServerValue -= RollOver;
					}

					if (System.Convert.ToDouble(InternalPV.ServerValue) != ServerValue)
					{
						InternalPV.ServerValue = ServerValue;
						InternalPV.DateTimeStamp = DateTimeOffset.Now;

						FMChannelHelper.MakeCall<IProcessVariables>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, InternalPV)
																);
					}

					if (SubLineItem.MeterReading.MeterStop.Value != ServerValue)
					{
						SubLineItem.MeterReading.MeterStop = ServerValue;
						SubLineItem.MeterReading.StopDateTime = siteTimeNow;
					}
				}

				SubLineItem.MeterReading.MeterStop_BadQualityLogged = false;

				if (AdditiveInjector.MeterValue != System.Convert.ToDouble(NonResettableGrossVolume.Value))
				{
					AdditiveInjector.MeterValue = System.Convert.ToDouble(NonResettableGrossVolume.Value);
					this.LastActivityDateTime = DateTimeOffset.Now;
				}
			}
		}

		public override void ReadLineItemData(
		  LineItemDO lineItem,
		  Server server,
		  LoadArmManagerClass loadArmManager)
		{
			// ReSharper disable CompareOfFloatsByEqualityOperator
			AcculoadIIILoadArmManagerClass acculoadIiiLoadArmManager = loadArmManager as AcculoadIIILoadArmManagerClass;
			if (acculoadIiiLoadArmManager == null)
			{
				throw new Exception("ReadLineItemData : Invalid LoadArmManager");
			}

			if (lineItem.SplashBlendingMap == null)
			{
				this.ReadBatchData(lineItem, server, acculoadIiiLoadArmManager);
			}

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				ProductMapClass component = loadArmManager.GetComponent(lineItem.ProductGuid);
				if (component == null)
				{
					throw new Exception("Component not found in LoadArm Configuration");
				}

				if (component.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
				{
					throw new Exception("Flow Controlled Additive cannot be Recipe");
				}

				this.ReadProductNonResettableTotal(component, lineItem, server, acculoadIiiLoadArmManager);

				SubLineItemDO subLineItem = new SubLineItemDO();
				this.ReadComponentBatchData(component, subLineItem, server, acculoadIiiLoadArmManager);
				lineItem.Density = this.Station.PromptForGravity ? this.OffloadDensity : subLineItem.Density;
				lineItem.Temperature = this.Station.PromptForTemperature ? this.OffloadTemperature : subLineItem.Temperature;
				lineItem.VCF = subLineItem.VCF;
			}

			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				if (subLineItem.Status != TransactionStatus.InProgress)
				{
					continue;
				}

				if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					ProductMapClass component = loadArmManager.GetComponent(subLineItem.ProductGuid);

					if (component == null)
					{
						throw new Exception("Component not found in LoadArm Configuration");
					}

					if (component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP
						 || component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
					{
						continue;
					}

					if (component.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
					{
						this.ReadAdditiveBatchData(component, subLineItem, server, acculoadIiiLoadArmManager);
						this.ReadAdditiveNonResettableTotal(component, subLineItem, server, acculoadIiiLoadArmManager);
					}
					else
					{
						this.ReadComponentBatchData(component, subLineItem, server, acculoadIiiLoadArmManager);
						this.ReadComponentNonResettableTotal(component, subLineItem, server, acculoadIiiLoadArmManager);
					}
				}
				else if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(subLineItem.ProductGuid);
					if (additiveInjector == null)
					{
						throw new Exception("Additive not found in LoadArm Additive Configuration");
					}

					this.ReadAdditiveBatchData(additiveInjector, subLineItem, server, acculoadIiiLoadArmManager);

					this.ReadAdditiveNonResettableTotal(additiveInjector, subLineItem, server, acculoadIiiLoadArmManager);
				}
			}


			if (lineItem.SplashBlendingMap == null
			&& lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct)
			&& lineItem.Quantity.NetInventoryChange != 0.0)
			{
				EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0) ? this.CurrentTransactionAlias.DensityUnits : this.SiteManager.Site.DensityUnits;

				SIDouble lineItemDensity = new SIDouble { Units = units };
				SIDouble subLineItemDensity = new SIDouble { Units = units };

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.ProductType != ProductClass.ProductTypeID(ProductType.ComponentProduct))
					{
						continue;
					}

					if (!subLineItem.Density_BadQualityLogged
					&& subLineItem.Density != 0
					&& lineItem.Quantity.NetInventoryChange != 0
					&& subLineItem.Quantity.NetInventoryChange != 0)
					{
						subLineItemDensity.Value = subLineItem.Density ?? 0.0;
						lineItemDensity.SIValue += subLineItemDensity.SIValue * subLineItem.Quantity.NetInventoryChange / lineItem.Quantity.NetInventoryChange;
					}
				}

				if (lineItemDensity.Value != 0)
				{
					if (lineItem.Density == null)
					{
						lineItem.Density = 0.0;
					}

					lineItem.Density = lineItemDensity.Value;
				}
			}
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		protected override bool IsIncludeFlowControlledAdditivesInProductTotals(Server server)
		{
			ProcessVariableClass stationPv = this.Station.ProcessVariableCollection[0];

			// read the arm values first
			string tagPrefix = stationPv.OPCItemID + ".";

			Item[] subItems ={  new Item(new ItemIdentifier(tagPrefix+"Include Flow-controlled Additive Totals in Product Totals"))
												};

			ItemValueResult[] values = server.Read(subItems);

			if (values[0].Quality != Quality.Good)
			{
				throw new Exception("IsIncludeFlowControlledAdditivesInProductTotals : OPC Quality Bad " + values[0].ItemName);
			}

			string additiveType = System.Convert.ToString(values[0].Value);
			return additiveType == "0 Include in Prd";
		}

		public override void CreateMeterReadingTransactions(
			SaveTransactionsSR saveTransactionsSR,
			TransactionAliasClass meterReadingTransactionAlias,
			DateTimeOffset inventoryDateTime)
		{
			foreach (AcculoadIIILoadArmManagerClass LoadArmManager in this.FullLoadArmCollection)
			{
				try
				{
					// Skip Load Arms that are Swing Arms on the second bay to eliminate duplicates
					if (LoadArmManager.LoadArm.SwingArm
					&& LoadArmManager.BayB.StationManager == this
					&& LoadArmManager.BayA.StationManager != null)
					{
						continue;
					}

					ProcessVariableClass LoadArmPV = LoadArmManager.LoadArm.ProcessVariableCollection[0];
					Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
					NetworkCredential Credentials = null;
					Server.Connect(new ConnectData(Credentials));

					DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

					foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
					{
                        LoadArmManager.ReadProductNonResettableTotal(Component.PresetNumber, Server, out ItemValueResult NonResettableGrossVolume);

                        TransactionDO MeterReadingTransaction = this.CreateMeterReadingTransaction(
							siteTimeNow,
							LoadArmManager,
							NonResettableGrossVolume,
							Component,
							meterReadingTransactionAlias,
							inventoryDateTime);

						if (MeterReadingTransaction != null)
						{
							saveTransactionsSR.Transactions.Add(MeterReadingTransaction);
						}
					}

					foreach (ProductMapClass Additive in LoadArmManager.LoadArm.AdditiveInjectorCollection)
					{
						ItemValueResult NonResettableGrossVolume;

						ProcessVariableClass InternalPV = Additive.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
						if (InternalPV == null)
						{
							LoadArmManager.ReadAdditiveNonResettableTotal(Additive.PresetNumber, Server, out NonResettableGrossVolume);
						}
						else
						{
							NonResettableGrossVolume = new ItemValueResult
							{
								ItemName = this.Station.ID + " Arm " + LoadArmManager.ArmNumber(this).ToString() + " " + Additive.ID,
								Quality = Quality.Good,
								ResultID = ResultID.S_OK,
								Value = System.Convert.ToDouble(InternalPV.ServerValue),
								Timestamp = InternalPV.DateTimeStamp.UtcDateTime
							};
						}

						TransactionDO MeterReadingTransaction = this.CreateMeterReadingTransaction(
							siteTimeNow,
							LoadArmManager,
							NonResettableGrossVolume,
							Additive,
							meterReadingTransactionAlias,
							inventoryDateTime);

						if (MeterReadingTransaction != null)
						{
							saveTransactionsSR.Transactions.Add(MeterReadingTransaction);
						}
					}
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry($"Error creating meter closeout transction for station {this.Station.ID}, load arm {LoadArmManager.LoadArm.ID}: {e.Message}\n\nStack Trace:\n{e.StackTrace}", EventLogEntryType.Error);
				}
			}
		}

		protected override void ProcessPreloadLoadIDSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (nSelection == -1
				|| this.CurrentMenuParameters == null
				|| nSelection > this.CurrentMenuParameters.Menu.Length)
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_PRELOAD_LOADID_SELECTION_MSG;
				return;
			}

			response = nSelection == 0 ? EscapeString : this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadLoadIDSelection(response);
		}

		public override void ProcessPreloadOrderSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (nSelection == -1
		  || this.CurrentMenuParameters == null
		  || nSelection > this.CurrentMenuParameters.Menu.Length)
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_PRELOAD_ORDER_SELECTION_MSG;
				return;
			}

			response = nSelection == 0 ? EscapeString : this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadOrderSelection(response);
		}

		protected override void ProcessPreloadDocumentSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}

			if (nSelection == -1
			  || this.CurrentMenuParameters == null
			  || nSelection > this.CurrentMenuParameters.Menu.Length)
			{
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG;
				return;
			}

			response = nSelection == 0 ? EscapeString : this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadDocumentSelection(response);
		}

		protected override void ProcessOffLoadProductSelect(string response)
		{
			bool productFound = false;
			if (response == EscapeString)
			{
				if (this.Station.OffLoadByOffLoadID == false && this.UseOffLoadSupplyOrders)
				{
					this.PromptForSupplyOrderNumber();
				}
				else
				{
					this.StationState = StationState.OFFLOADID_PROMPT;
					this.DisplayMessage("[LoadRack|Enter] [LoadRack|Off Load ID]", null, PromptLength, this.PROMPT_TIMEOUT);
				}

				return;
			}

			int menuNumber = 1;
			if (this.Station.OffLoadByOffLoadID ||
				this.UseOffLoadSupplyOrders == false)
			{
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					foreach (ProductMapClass productMap in loadArmManager.LoadArm.ComponentCollection)
					{
						if (menuNumber == System.Convert.ToInt32(response))
						{
							this.SelectedProductID = productMap.AssignedID;
							loadArmManager.CurrentLineItemProduct = this.GetProduct(this.Security, productMap.AssignedGuid);
							if (loadArmManager.CurrentLineItemProduct != null)
							{
								productFound = true;
							}

							break;
						}

						++menuNumber;
					}
				}
			}
			else
			{
				if (this.SupplyOrder.LineItems.Count > 0)
				{
					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						// check for different products in the line items and present the user with a selection
						// ReSharper disable once ForCanBeConvertedToForeach
						for (int index = 0; index < this.SupplyOrder.LineItems.Count; index++)
						{
							var lineItem = (LineItemDO)this.SupplyOrder.LineItems[index];
							if (menuNumber == System.Convert.ToInt32(response))
							{
								this.SelectedProductID = lineItem.Product;
								loadArmManager.CurrentLineItemProduct = this.GetProduct(this.Security, lineItem.ProductGuid);
								if (loadArmManager.CurrentLineItemProduct != null)
								{
									productFound = true;
								}

								break;
							}

							++menuNumber;
						}
					}
				}
			}

			if (productFound == false)
			{
				this.DisplayOffLoadProductSelect();
				return;
			}

			this.PromptForBOLNumber();
		}

		public override void SetUnloadPresetAmount(string response)
		{
			if (response == EscapeString)
			{
				this.PromptForBOLNumber();
				return;
			}

			// set the output permissives
			this.UpdatePermissives(true);
			if (this.StationState == StationState.RESET_ON_TIMEOUT)
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);
				return;
			}

			this.StartDateTime = this.LastActivityDateTime = DateTime.UtcNow;
			this.OffLoadPresetAmount = System.Convert.ToDouble(response);

			var loadArmManager = (AcculoadIIILoadArmManagerClass)this.LoadArmManagerCollection.Item(0);

			this.StationState = StationState.AUTHORIZED;
			if (!loadArmManager.Authorize(this, System.Convert.ToDouble(response)))
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);
			}
		}

		public override void DisplayOffLoadProductSelect()
		{
			// for the preset we need to populate the menu with the configured arm products
			if (this.AvailableLoadArmManagers == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// check that the supplier has authorized products configured
			if (this.Supplier.SupplierAuthorizedProductCollection.Count == 0)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// Build menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			var menu = new List<string>();

			// Save last station state
			this.PriorStationState = this.StationState;

			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				foreach (ProductMapClass productMap in loadArmManager.LoadArm.ComponentCollection)
				{
					foreach (ProductMapClass supplierProduct in this.Supplier.SupplierAuthorizedProductCollection)
					{
						if (supplierProduct.AssignedID == productMap.AssignedID)
						{
							menu.Add(productMap.AssignedID);
						}
					}
				}
			}

			if (menu.Count == 0)
			{
				this.AddAlarmAndEventLogs(this.Security, this.Station.NoProductsAvailableEvent(this.Station.ID));
				this.LoadRackManager.EventOrAlarmEvent.Set();

				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			parameters.Menu = menu.ToArray();

			this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

			this.DisplayMenu(parameters);
		}

		public override void DisplayVerifySupplyOrderProduct()
		{
			//SelectedSupplyOrder
			SiteTimeConverter timeConverter = new SiteTimeConverter(this.SiteManager.Site);
			DateTimeOffset today = timeConverter.Today();
			string documentNumber;
			bool bProductFound = false;
			//			CardID=Response;
			// Check for preloads for the current driver
			GetTransactionSR getTransactionSR = new GetTransactionSR
			{
				Security = this.Security,
				Request = GetTransactionRequest.SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
				Site = this.SiteManager.Site.ID,
				TransTypeID = TransactionTypes.T18_SupplyOrder,
				Status = ((int)TransactionStatus.Scheduled).ToString(),
				DocumentNumber = this.SelectedSupplyOrder
			};

			GetTransactionDO getTransactionDO = FMChannelHelper.MakeCall<IGetTransactionProcessor, GetTransactionDO>(AccountingService => AccountingService.Process(getTransactionSR));

			// Build menu parameter set
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PROMPT_TIMEOUT,
				SaveForCancelProcessing = false,
				Caption = "LoadRack|Select Off Load Product"
			};

			ArrayList menu = new ArrayList();

			// Save last station state
			this.PriorStationState = this.StationState;


			if (getTransactionDO != null
				&& getTransactionDO.TransactionDataSet != null
				&& getTransactionDO.TransactionDataSet.Tables.Count != 0
				&& getTransactionDO.TransactionDataSet.Tables[0].Rows.Count != 0)
			{
				foreach (DataRow row in getTransactionDO.TransactionDataSet.Tables[0].Rows)
				{
					documentNumber = row["TransID"] as string;
					if (documentNumber != "")
					{
						this.SupplyOrder = this.GetTransaction(documentNumber);

						// check for multiple line items to very product
						if (this.SupplyOrder.LineItems.Count > 0)
						{
							// check for different products in the line items and present the user with a selection
							for (int index = 0; index < this.SupplyOrder.LineItems.Count; index++)
							{
								LineItemDO lineItem = this.SupplyOrder.LineItems[index];
								if (lineItem.Product != null && lineItem.Status == TransactionStatus.Scheduled)
								{
									// make sure this product is in the arm or the transaction will not be saved
									foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
									{
										foreach (ProductMapClass productMap in loadArmManager.LoadArm.ComponentCollection)
										{
											if (lineItem.Product == productMap.AssignedID)
											{
												menu.Add(productMap.AssignedID);
												bProductFound = true;
												parameters.Menu = (string[])menu.ToArray(typeof(string));
											}
										}
									}
								}
							}
						}

						if (bProductFound == false)
						{
							this.StationState = StationState.RESET_ON_TIMEOUT;
							this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
							return;
						}

						if (this.SupplyOrder.ShipToCompanyGuid != Guid.Empty)
						{
							this.ShipTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.ShipToCompanyGuid));
						}

						if (this.SupplyOrder.BillToCompanyGuid != Guid.Empty)
						{
							this.BillTo = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.BillToCompanyGuid));
						}

						if (this.SupplyOrder.ShipperCompanyGuid != Guid.Empty)
						{
							this.Shipper = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.ShipperCompanyGuid));
						}

						if (this.SupplyOrder.OwnerCompanyGuid != Guid.Empty)
						{
							this.Owner = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.OwnerCompanyGuid));
						}

						if (this.SupplyOrder.ManagerCompanyGuid != Guid.Empty)
						{
							this.Manager = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.ManagerCompanyGuid));
						}

						if (this.SupplyOrder.SupplierCompanyGuid != Guid.Empty)
						{
							this.Supplier = FMChannelHelper.MakeCall<ICompanies, CompanyClass>(Companies => Companies.Get(this.Security, this.SupplyOrder.SupplierCompanyGuid));
						}

						this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

						this.DisplayMenu(parameters);
						return;
					}
				}
				// default error message if any of the above does not complete
				this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
				this.StationState = StationState.INVALID_SUPPLIER_PROMPT_RESPONSE_MESSAGE;
			}
		}

		/// <summary>
		/// Writes recipes to the Accuload.
		/// 
		/// This will write only the recipes deliverable on the Accuload
		/// based on the current component/external component/flow-controlled additive configuration
		/// 
		/// Commented out now as the dynamic recipe set will be done later.  If recipes are not set dynamically, then they are
		/// already on the preset and we should still do nothing
		/// </summary>
        protected override void SetProductsInStation()
        {
    //        foreach (AcculoadIIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
    //        {
    //            ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];
                
				//foreach (ProductMapClass recipeToArmMap in loadArmManager.LoadArm.ProductRecipeCollection)
				//{
				//	// Check if the recipe is deliverable with the current component configuration
				//	// Get the recipe product definition
				//	ProductClass recipe = this.GetProduct(this.Security, recipeToArmMap.AssignedGuid);

				//	// Check the recipe's components
				//	bool recipeDeliverable = true;
				//	List<(ProductMapClass component, int armComponent)> armComponents = new List<(ProductMapClass component, int armComponent)>(); // This list will be needed later when assigning Accuload components to the recipe
				//	foreach(ProductMapClass componentToRecipeMap in recipe.ComponentCollection)
				//	{
				//		ProductMapClass componentToArmMap = loadArmManager.GetComponent(componentToRecipeMap.AssignedGuid);
				//		if (componentToArmMap == null)
				//		{
				//			recipeDeliverable = false;
				//		}
				//		else
				//		{
				//			// if the componentToArmMap represents an entry from the ComponentMap, as opposed to an external component or flow-controlled additive,
				//			// we add it to the list of arm components, along with recipe-component map to the list along with the accuload component number
				//			if (componentToArmMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP || componentToArmMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
				//			{
				//				armComponents.Add((componentToRecipeMap, componentToArmMap.PresetNumber));
    //                        }
				//		}
				//	}

				//	if (!recipeDeliverable)
				//	{
				//		continue;
				//	}

				//	int recipeNumber = loadArmManager.GetRecipeNumber(recipeToArmMap);

				//	// Start building up the OPC Items to write to build the recipe
				//	ItemValue currentItemValue;
				//	List<ItemValue> itemValues = new List<ItemValue>();

				//	// Assign to load arm
				//	currentItemValue = new ItemValue
				//	{
				//		ItemName = loadArmPv.OPCItemID + ".Program Code Change",
				//		Value = $"{recipeNumber:D2} 001 {loadArmManager.GetArmNumber(this):D1}"
				//	};
				//	itemValues.Add(currentItemValue);

    //                // Set Recipe Name
    //                currentItemValue = new ItemValue
    //                {
    //                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                    Value = $"{recipeNumber:D2} 002 {recipe.ID}"
    //                };
    //                itemValues.Add(currentItemValue);

				//	// set components
				//	// use the list of components we built above
				//	// First find the total percentage of the recipe to be supplied by the Accuload as products
				//	double totalPercentage = 0;
				//	foreach ((ProductMapClass component, int armComponent) component in armComponents)
				//	{
				//		totalPercentage += component.component.BlendPercentage;
    //                }

				//	// now set all 6 components on the recipe; setting to zero/not used those that we don't use
				//	for (int ndx = 0; ndx < 6; ndx++)
				//	{
				//		if (ndx < armComponents.Count)
				//		{
				//			var component = armComponents[ndx];
    //                        // Add the nth product delivered number
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {4 + (ndx * 2):D3} {component.armComponent}"
    //                        };
    //                        itemValues.Add(currentItemValue);

    //                        // Add the nth product delivered percentage
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {5 + (ndx * 2):D3} {component.component.BlendPercentage / totalPercentage * 100.0:F2}"
    //                        };
    //                        itemValues.Add(currentItemValue);

				//			if (ndx == 0)
				//			{
    //                            // Set the Hazardous Material class product and clean deduct products
    //                            // Hazardous Material class product is zero-indexed, as "None" is not an option
    //                            currentItemValue = new ItemValue
    //                            {
    //                                ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                                Value = $"{recipeNumber:D2} 003 {component.armComponent - 1}"
    //                            };
    //                            itemValues.Add(currentItemValue);

    //                            // Same for Clean Deduct product
    //                            currentItemValue = new ItemValue
    //                            {
    //                                ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                                Value = $"{recipeNumber:D2} 016 {component.armComponent - 1}"
    //                            };
    //                            itemValues.Add(currentItemValue);
    //                        }
    //                    }
				//		else
				//		{
    //                        // Remaining products must be set to Not Used and zero percent
    //                        // Add the nth product delivered number
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {4 + (ndx * 2):D3} 0" // zero is Not Used
    //                        };
    //                        itemValues.Add(currentItemValue);

    //                        // Add the nth product delivered percentage
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {5 + (ndx * 2):D3} 0.00"
    //                        };
    //                        itemValues.Add(currentItemValue);
    //                    }
    //                }

    //                // Set Clean Line Product to "Not Used" and Delivery Mode to "Concurrent"
    //                currentItemValue = new ItemValue
    //                {
    //                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                    Value = $"{recipeNumber:D2} 089 0"
    //                };
    //                itemValues.Add(currentItemValue);

    //                currentItemValue = new ItemValue
    //                {
    //                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                    Value = $"{recipeNumber:D2} 090 0"
    //                };
    //                itemValues.Add(currentItemValue);

				//	// enable additives that we have configured on the load arm for all components
				//	// zero out the others.
				//	// Using 1-indexed instead of 0-indexed to match additive injector numbers
				//	for (int ndx = 1; ndx <=24; ndx++)
				//	{
    //                    // Set Additive Amount/Cycle to zero (this may be adjusted later by additive profile
    //                    currentItemValue = new ItemValue
    //                    {
    //                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                        Value = $"{recipeNumber:D2} {3 * ndx + 14:D3} 0.000"
    //                    };
    //                    itemValues.Add(currentItemValue);

    //                    // Set Additive Rate to zero (this may be adjusted later by additive profile
    //                    currentItemValue = new ItemValue
    //                    {
    //                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                        Value = $"{recipeNumber:D2} {3 * ndx + 15:D3} 0.0"
    //                    };
    //                    itemValues.Add(currentItemValue);

    //                    ProductMapClass additiveMap = loadArmManager.LoadArm.AdditiveInjectorCollection.FirstOrDefault(injectorMap => injectorMap.PresetNumber == ndx);
				//		if (additiveMap != null)
				//		{
				//			// Set all products as using this additive
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {3 * ndx + 16:D3} {Math.Pow(2, armComponents.Count) - 1}"
    //                        };
    //                        itemValues.Add(currentItemValue);
    //                    }
    //                    else
				//		{
    //                        // Set no products as using this additive
    //                        currentItemValue = new ItemValue
    //                        {
    //                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
    //                            Value = $"{recipeNumber:D2} {3 * ndx + 16:D3} 0"
    //                        };
    //                        itemValues.Add(currentItemValue);
    //                    }
    //                }

				//	this.OPCServerManager.Write(new URL(loadArmPv.URL), itemValues.ToArray());
    //            }
    //        }
        }

		/// <summary>
		/// Clears recipes on the preset that were set during the loading orr all recipes
		/// 
		/// After clearing the recipes it resets the internal device recipe to configuration recipe mapping
		/// </summary>
		/// <param name="clearAll">
		/// When true, clear all recipes in the preset
		/// When false, clear only those recipes explicitly set during the loading process</param>
		protected override void ClearRecipes(bool clearAll)
		{
			if (!this.Station.EnableDynamicRecipes)
			{
                // if EnableDynamicRecipes is turned off, we don't want to actually clear anything this
                this.RecipeInternalNumberMap = new Dictionary<int, ProductMapClass>();
                this.LastDownloadedRecipe = 0;
                return;
			}

			if (clearAll)
			{
				int numberOfArms = this.PhysicalArmsOnPreset;
                this.WriteLogDataToCommFile($"Clearing all recipes from station", CommLogDirection.None);
                for (int recipeNumber = 1; recipeNumber <= MaxRecipes - numberOfArms; recipeNumber++)
				{
					if (this.RecipeBelongsToThisStation(recipeNumber))
					{
						this.ClearSingleRecipe(recipeNumber);
					}
				}

                // The Accuload requires at least one recipe assigned to each arm, else it throws errors
                // Assume that we have a LoadArmManager for each arm, as we haven't access to the Accuload's own count of arms
                for (int loadArmIndex = 0; loadArmIndex < numberOfArms; loadArmIndex++)
                {
                    AcculoadIIILoadArmManagerClass loadArmManager = this.LoadArmManagerCollection.Item(0) as AcculoadIIILoadArmManagerClass;
                    ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];

                    // Just fill in a minimal recipe, with an arm assignment and using only Product 1 at 100%
                    // Start building up the OPC Items to write to build the recipe
                    ItemValue currentItemValue;
                    List<ItemValue> itemValues = new List<ItemValue>();

                    // Assign to load arm
                    currentItemValue = new ItemValue
                    {
                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                        Value = $"{MaxRecipes - loadArmIndex:D2} 001 {loadArmIndex + 1:D1}" // This is setting an Accuload-specific arm, not our loadarm manager
                    };
                    itemValues.Add(currentItemValue);

                    // Assign 1st component
                    currentItemValue = new ItemValue
                    {
                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                        Value = $"{MaxRecipes - loadArmIndex:D2} 004 {1:D1}"
                    };
                    itemValues.Add(currentItemValue);

                    // Assign 100%
                    currentItemValue = new ItemValue
                    {
                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                        Value = $"{MaxRecipes - loadArmIndex:D2} 005 {100:D1}"
                    };
                    itemValues.Add(currentItemValue);

                    try
                    {
                        this.WriteLogDataToCommFile($"Writing placeholder recipe {MaxRecipes - loadArmIndex}", CommLogDirection.None);
                        this.OPCServerManager.Write(new URL(loadArmPv.URL), itemValues.ToArray());
                    }
                    catch (Exception e)
                    {
                        _ = e;
								
								string msg = $"Failed to write station recipe {MaxRecipes - loadArmIndex:D2}";

								this.WriteLogDataToCommFile(msg, CommLogDirection.None);
								
								this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
							}
                }
            }
            else
			{
                this.WriteLogDataToCommFile($"Clearing recipes set during loading", CommLogDirection.None);
                foreach (int recipeNumber in this.RecipeInternalNumberMap.Keys)
				{
					this.ClearSingleRecipe(recipeNumber);
				}
			}

			AcculoadIIILoadArmManagerClass firstLoadArmManager = this.LoadArmManagerCollection.Item(0) as AcculoadIIILoadArmManagerClass;
			firstLoadArmManager.LogOutOfProgramMode();

            this.RecipeInternalNumberMap = new Dictionary<int, ProductMapClass>();
			this.LastDownloadedRecipe = 0;
        }

        /// <summary>
		/// Clears a recipe out of the preset by setting all of its program values to 0
		/// </summary>
		/// <param name="recipeNumber">
		/// Te preset recipe number to clear
		/// </param>
		internal override void ClearSingleRecipe(int recipeNumber)
        {
			const int RegistersPerProduct = 90;

			if (!this.Station.EnableDynamicRecipes)
            {
                // if EnableDynamicRecipes is turned off, do nothing
                return;
            }

            ProcessVariableClass stationPv = this.StationPv;

            Server server = new Server(new Factory(), new URL(stationPv.URL));
            server.Connect(new ConnectData(null));

            this.WriteLogDataToCommFile($"Beginning recipe {recipeNumber} clear", CommLogDirection.None);

            // Get the first load arm manager to get a tag to read/write process variables
            if (this.LoadArmManagerCollection.Count == 0)
            {
                this.WriteLogDataToCommFile("No active load arms; ending recipe clear", CommLogDirection.None);
                return;
            }
            AcculoadIIILoadArmManagerClass loadArmManager = this.LoadArmManagerCollection.Item(0) as AcculoadIIILoadArmManagerClass;
            ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];

            // cycle through the recipes and find how they're assigned.
            // this becomes important when working on swingarms
            //List<Item> itemList = new List<Item>();
            //itemList.Add(new Item(new ItemIdentifier(stationPv.OPCItemID + $".Recipes.{recipeNumber:D2}.Used")));
            //ItemValueResult[] recipesUsed = server.Read(itemList.ToArray());

            // Wipe the recipe
            // Write '0' to every register for the recipe except for register 002, which gets ''
            ItemValue currentItemValue;
            List<ItemValue> itemValues = new List<ItemValue>();
            for (int registerNumber = 1; registerNumber <= RegistersPerProduct; registerNumber++)
			{
				currentItemValue = new ItemValue
				{
					ItemName = loadArmPv.OPCItemID + ".Program Code Change",
					Value = $"{recipeNumber:D2} {registerNumber:D3} {(registerNumber == 2 ? " " : "0")}"
				};
                itemValues.Add(currentItemValue);
            }
            try
            {
                this.OPCServerManager.Write(new URL(loadArmPv.URL), itemValues.ToArray());
            }
            catch (Exception e)
            {
                _ = e;
					
					string msg = $"Failed to clear station recipe {recipeNumber}";

					this.WriteLogDataToCommFile(msg, CommLogDirection.None);

					this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
			}
        }

        /// <summary>
        /// Attempt to write a recipe down to the preset if dynamic recipes are enabled
        /// 
        /// If not enabled just sets up the device recipe number to configured recipe map
        /// </summary>
        /// <param name="loadArmManager">
        /// Load Arm Manager for the load arm the recipe is on</param>
        /// <param name="recipeToArmMap">
        /// The recipe assignment to write to the preset
        /// </param>
        /// <returns>Device product number assigned to the recipe</returns>
        internal override int WriteSingleRecipe(LoadArmManagerClass loadArmManager, ProductMapClass recipeToArmMap)
        {
			int downloadedRecipeNumber = 0;
            if (!this.Station.EnableDynamicRecipes)
            {
                // if EnableDynamicRecipes is turned off, we map the configured recipe number to itself
                return loadArmManager.GetRecipeNumber(recipeToArmMap);
            }

			ProcessVariableClass loadArmPv = loadArmManager.LoadArm.ProcessVariableCollection[0];

            int recipeToWrite = this.GetNextAvailableRecipeNumber();
			this.WriteLogDataToCommFile($"Attempting to write station recipe {recipeToArmMap.AssignedID} to device recipe {recipeToWrite}", CommLogDirection.None);
			if (recipeToWrite != 0)
			{
                // Check if the recipe is deliverable with the current component configuration
                // Get the recipe product definition
                ProductClass recipe = this.GetProduct(this.Security, recipeToArmMap.AssignedGuid);

                // Check the recipe's components
                bool recipeDeliverable = true;
                List<(ProductMapClass component, int armComponent)> armComponents = new List<(ProductMapClass component, int armComponent)>(); // This list will be needed later when assigning Accuload components to the recipe
                foreach (ProductMapClass componentToRecipeMap in recipe.ComponentCollection)
                {
                    ProductMapClass componentToArmMap = loadArmManager.GetComponent(componentToRecipeMap.AssignedGuid);
                    if (componentToArmMap == null)
                    {
                        recipeDeliverable = false;
                    }
                    else
                    {
                        // if the componentToArmMap represents an entry from the ComponentMap, as opposed to an external component or flow-controlled additive,
                        // we add it to the list of arm components, along with recipe-component map to the list along with the accuload component number
                        if (componentToArmMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP || componentToArmMap.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP)
                        {
                            armComponents.Add((componentToRecipeMap, componentToArmMap.PresetNumber));
                        }
                    }
                }

                if (!recipeDeliverable)
                {
					WriteLogDataToCommFile($"Station unable to deliver configured recipe {recipeToArmMap.AssignedID}, recipe not written", CommLogDirection.None);
                    return 0; // if we can't deliver the product, don't even try writing it
                }

                // Start building up the OPC Items to write to build the recipe
                ItemValue currentItemValue;
                List<ItemValue> itemValues = new List<ItemValue>();

                // Assign to load arm
                currentItemValue = new ItemValue
                {
                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                    Value = $"{recipeToWrite:D2} 001 {loadArmManager.GetPresetArmNumber(this):D1}" // We're setting the recipe on the preset's arm number
                };
                itemValues.Add(currentItemValue);

                // Set Recipe Name
                currentItemValue = new ItemValue
                {
                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                    Value = $"{recipeToWrite:D2} 002 {recipe.ID}"
                };
                itemValues.Add(currentItemValue);

                // set components
                // use the list of components we built above
                // First find the total percentage of the recipe to be supplied by the Accuload as products
                double totalPercentage = 0;
                foreach ((ProductMapClass component, int armComponent) component in armComponents)
                {
                    totalPercentage += component.component.BlendPercentage;
                }

                // now set all 6 components on the recipe; setting to zero/not used those that we don't use
                for (int ndx = 0; ndx < 6; ndx++)
                {
                    if (ndx < armComponents.Count)
                    {
                        var component = armComponents[ndx];
                        // Add the nth product delivered number
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {4 + (ndx * 2):D3} {component.armComponent}"
                        };
                        itemValues.Add(currentItemValue);

                        // Add the nth product delivered percentage
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {5 + (ndx * 2):D3} {component.component.BlendPercentage / totalPercentage * 100.0:F2}"
                        };
                        itemValues.Add(currentItemValue);

                        if (ndx == 0)
                        {
                            // Set the Hazardous Material class product and clean deduct products
                            // Hazardous Material class product is zero-indexed, as "None" is not an option
                            currentItemValue = new ItemValue
                            {
                                ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                                Value = $"{recipeToWrite:D2} 003 {component.armComponent - 1}"
                            };
                            itemValues.Add(currentItemValue);

                            // Same for Clean Deduct product
                            currentItemValue = new ItemValue
                            {
                                ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                                Value = $"{recipeToWrite:D2} 016 {component.armComponent - 1}"
                            };
                            itemValues.Add(currentItemValue);
                        }
                    }
                    else
                    {
                        // Remaining products must be set to Not Used and zero percent
                        // Add the nth product delivered number
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {4 + (ndx * 2):D3} 0" // zero is Not Used
                        };
                        itemValues.Add(currentItemValue);

                        // Add the nth product delivered percentage
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {5 + (ndx * 2):D3} 0.00"
                        };
                        itemValues.Add(currentItemValue);
                    }
                }

                // Set Clean Line Product to "Not Used" and Delivery Mode to "Concurrent"
                currentItemValue = new ItemValue
                {
                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                    Value = $"{recipeToWrite:D2} 089 0"
                };
                itemValues.Add(currentItemValue);

                currentItemValue = new ItemValue
                {
                    ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                    Value = $"{recipeToWrite:D2} 090 0"
                };
                itemValues.Add(currentItemValue);

                // enable additives that we have configured on the load arm for all components
                // zero out the others.
                // Using 1-indexed instead of 0-indexed to match additive injector numbers
                for (int ndx = 1; ndx <= 24; ndx++)
                {
                    // Set Additive Amount/Cycle to zero (this may be adjusted later by additive profile
                    currentItemValue = new ItemValue
                    {
                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                        Value = $"{recipeToWrite:D2} {3 * ndx + 14:D3} 0.000"
                    };
                    itemValues.Add(currentItemValue);

                    // Set Additive Rate to zero (this may be adjusted later by additive profile
                    currentItemValue = new ItemValue
                    {
                        ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                        Value = $"{recipeToWrite:D2} {3 * ndx + 15:D3} 0.0"
                    };
                    itemValues.Add(currentItemValue);

                    ProductMapClass additiveMap = loadArmManager.LoadArm.AdditiveInjectorCollection.FirstOrDefault(injectorMap => injectorMap.PresetNumber == ndx);
                    if (additiveMap != null)
                    {
                        // Set all products as using this additive
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {3 * ndx + 16:D3} {Math.Pow(2, armComponents.Count) - 1}"
                        };
                        itemValues.Add(currentItemValue);
                    }
                    else
                    {
                        // Set no products as using this additive
                        currentItemValue = new ItemValue
                        {
                            ItemName = loadArmPv.OPCItemID + ".Program Code Change",
                            Value = $"{recipeToWrite:D2} {3 * ndx + 16:D3} 0"
                        };
                        itemValues.Add(currentItemValue);
                    }
                }
				try
				{
					this.OPCServerManager.Write(new URL(loadArmPv.URL), itemValues.ToArray());
					this.LastDownloadedRecipe = recipeToWrite;
					downloadedRecipeNumber = recipeToWrite;
				}
				catch (Exception e)
				{
					_ = e;
					
					string msg = $"Failed to write station recipe {recipeToArmMap.AssignedID} as {recipeToWrite}";
					
					this.WriteLogDataToCommFile(msg, CommLogDirection.None);

					this.eventLog.WriteEntry(msg, EventLogEntryType.Error);
				}
            }

			return downloadedRecipeNumber;
        }

        /// <summary>
		/// Tries to find the next available (unassigned) recipe
		/// 
		/// The Accuload III/IV has 50 recipe assignments available
		/// </summary>
		/// <returns>
		/// The recipe number of the next available recipe.
		/// 
		/// 0 on error or no recipes available
		/// </returns>
		protected override int GetNextAvailableRecipeNumber()
        {
            Server server = new Server(new Factory(), new URL(this.StationPv.URL));
            server.Connect(new ConnectData(null));

            for (int recipeNumber = this.LastDownloadedRecipe + 1; recipeNumber <= MaxRecipes - this.LoadArmManagerCollection.Count; recipeNumber++)
			{
                List<Item> itemList = new List<Item>()
				{
					new Item(new ItemIdentifier(this.StationPv.OPCItemID + $".Recipes.{recipeNumber:D2}.Used"))
				};
				ItemValueResult[] recipesUsed = server.Read(itemList.ToArray());

				if (recipesUsed.Length > 0)
				{
					ItemValueResult recipeUsed = recipesUsed[0];

					if (recipeUsed.Quality == Quality.Good)
					{
						if (System.Convert.ToInt32(recipeUsed.Value) == 0)
						{
							// Recipe was able to be read and is not currently assigned
							return recipeNumber;
						}
					}
				}
            }

			return 0;
        }

        /// <summary>
        /// Returns whether the recipce in question belongs to this FuelsManager station
        /// Comes in to play with swing arms and split bays, where two stations in FuelsManager may
        /// address the same physical preset
        /// </summary>
        /// <param name="recipeNumber">Recipe number to check</param>
        /// <returns>true</returns>
        protected override bool RecipeBelongsToThisStation(int recipeNumber)
        {
			List<int> controlledDeviceLoadArms = new List<int>();

			foreach(AcculoadIIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this == loadArmManager.GetStationManager())
				{
					controlledDeviceLoadArms.Add(loadArmManager.GetPresetArmNumber(this));
				}
			}

            Server server = new Server(new Factory(), new URL(this.StationPv.URL));
            server.Connect(new ConnectData(null));

            List<Item> itemList = new List<Item>()
                {
                    new Item(new ItemIdentifier(this.StationPv.OPCItemID + $".Recipes.{recipeNumber:D2}.Used"))
                };
            ItemValueResult[] recipesUsed = server.Read(itemList.ToArray());

            if (recipesUsed.Length > 0)
            {
                ItemValueResult recipeUsed = recipesUsed[0];

                if (recipeUsed.Quality == Quality.Good)
                {
					int recipeUsedValue = -1;
					try
					{
						recipeUsedValue = System.Convert.ToInt32(recipeUsed.Value);
					}
					catch (Exception ex)
					{
						_ = ex;
					}

					if (recipeUsedValue == 0 || controlledDeviceLoadArms.Contains(recipeUsedValue)) {
						return true;
					}
                }
            }

            return false;
        }
    }
}
