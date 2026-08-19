/******************************************************************************

	FILE NAME:		MultiloadIIStationManager.cs


	PURPOSE:			MultiloadIIStationManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2009

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec Inc.


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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Net;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using Opc;
	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	using Factory = OpcCom.Factory;
	using Server = Opc.Da.Server;

	public class MultiloadIIStationManagerClass : StationManagerClass
	{
		public static readonly string Esc = System.Convert.ToChar(27).ToString(CultureInfo.InvariantCulture);
		protected Thread ScanThread;
		protected ManualResetEvent StationKillEvent;
		protected ProcessVariableClass TerminationKeyPV;
		protected ProcessVariableClass InputDonePV;
		protected ProcessVariableClass PowerUpPV;
		protected ProcessVariableClass HostUpPV;
		protected ProcessVariableClass CardReaderDataPV;
		protected ProcessVariableClass CardReaderStatusPV;
		protected int MaxDisplayLineSize = 40;
		protected int MaxLines = 16;
		protected int MessageTimer;
		protected ArrayList ProductIndexes = new ArrayList();

		public MultiloadIIStationManagerClass(
			EventLog EventLog,
			LoadRackManagerClass LoadRackManager,
			StationClass Station,
			SiteManagerClass SiteManager,
			SecurityClass Security)
			: base(EventLog, LoadRackManager, Station, SiteManager, Security)
		{
			string stationUrl = Station.ProcessVariableCollection[0].URL;
			string stationProgID = Station.ProcessVariableCollection[0].ProgID;
			string stationOpcItemID = Station.ProcessVariableCollection[0].OPCItemID;

			this.InputDonePV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.INPUT_DONE_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BOOL,
			  true,
			  stationOpcItemID + ".Status.Input Done",
			  stationUrl,
			  stationProgID);

			this.OPCServerManager.AddProcessVariable(this.InputDonePV);

			this.TerminationKeyPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.TERMINATION_KEY,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BSTR,
			  true,
			  stationOpcItemID + ".Terminating Key",
			  stationUrl,
			  stationProgID);

			this.OPCServerManager.AddProcessVariable(this.TerminationKeyPV);

			this.PowerUpPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.LOADARM_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BOOL,
			  true,
			  stationOpcItemID + ".Status.Power Up",
			  stationUrl,
			  stationProgID);

			this.OPCServerManager.AddProcessVariable(this.PowerUpPV);

			this.HostUpPV = new ProcessVariableClass(
			  PROCESS_VARIABLE_TYPE.LOADARM_PV,
			  UNIT_TYPE.STATION_UNIT,
			  VarEnum.VT_BOOL,
			  true,
			  stationOpcItemID + ".Status.Host Up",
			  stationUrl,
			  stationProgID);

			this.OPCServerManager.AddProcessVariable(this.HostUpPV);

			this.CardReaderStatusPV = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.CARDREADER_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_BSTR,
				true,
				stationOpcItemID + ".Card Status",
				stationUrl,
				stationProgID);

			OPCServerManager.AddProcessVariable(this.CardReaderStatusPV);

			if (this.Station.CardReader)
			{
				this.CardReaderDataPV = new ProcessVariableClass(
					 PROCESS_VARIABLE_TYPE.CARDREADER_PV,
					 UNIT_TYPE.STATION_UNIT,
					 VarEnum.VT_BSTR,
					 true,
					 stationOpcItemID + ".Card Number",
					 stationUrl,
					 stationProgID);

				OPCServerManager.AddProcessVariable(this.CardReaderDataPV);
			}

			ThreadStart ScanStart = this.ScanDataThread;
			this.StationKillEvent = new ManualResetEvent(false);
			this.ScanThread = new Thread(ScanStart);
			this.ScanThread.Start();
			this.ScanThread.Priority = ThreadPriority.AboveNormal;

			try
			{
				// If the base object found a transaction in progress at the device, it will set the station
				// status to "Transaction In Progress".  In that case we do not want to reset the device as it
				// will yank the station out from under the in progress transaction
				if (this.StationState != StationState.TRANSACTION_IN_PROGRESS)
				{
					if (this.AvailableLoadArmManagers != 0 || (this.Station.Type == STATION_TYPE.WEIGHT_SCALE))
					{
						if (this.Station.CardReader)
						{
							this.IssuePleaseCardIn();
						}
						else
						{
							this.IssueDriverIDPrompt();
						}
					}
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Multiload II StationManager : " + e.Message, EventLogEntryType.Error);

				this.StationState = StationState.IDLE;
			}
		}

		public override bool PromptForTransactionCompletion
		{
			get
			{
				// TODO:  Fix Handling for transactions held in progress.
				// We don't have this working correctly for the MultiLoad yet
				// We will address this in a later sprint; for now effectively
				// disable this feature.
				return false;
			}
		}

		public override StationState StationState
		{
			get
			{
				return base.StationState;
			}

			set
			{
				string logString = this.Station.ID + ":  Changing state from " + base.StationState + " to " + value;
				var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
				logger.Debug(logString);
				base.StationState = value;
			}
		}

		public override void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				base.Dispose();

				// Terminate the Scan Thread
				this.StationKillEvent?.Set();
				this.ScanThread?.Join();

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		public void ScanDataThread()
		{
			while (!this.StationKillEvent.WaitOne(1000, true))
			{
				Monitor.Enter(this);
				try
				{
					if (this.MessageTimer > 0)
					{
						this.MessageTimer--;
						if (this.MessageTimer == 0)
						{
							// Need to try load arms first, then hit the station
							bool timeoutProcessedByLoadArm = false;
							foreach (MultiloadIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (loadArmManager.ProcessMessageTimeout(this))
								{
									timeoutProcessedByLoadArm = true;
								}
							}

							if (!timeoutProcessedByLoadArm)
							{
								this.ProcessMessageTimeout();
							}
						}
					}
				}
				catch (Exception e)
				{
					eventLog.WriteEntry("MultiloadIIStationManager ScanDataThread : " + e.Message);
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();

			if (Station.CardReader)
			{
				this.IssuePleaseCardIn();
			}
			else
			{
				this.IssueDriverIDPrompt();
			}
		}

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool SaveForCancelProcessing)
		{
			string message = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, stockMessage);
			if (responseLength > 0)
			{
				message += ":";
			}

			int retryCounter = 3;

			char[] separators = { ' ' };
			string[] strings = message.Split(separators);
			var lines = new ArrayList();

			foreach (string subMessage in strings)
			{
				if (subMessage.Length > this.MaxDisplayLineSize)
				{
					break;
				}

				if (lines.Count == 0)
				{
					lines.Add(subMessage);
				}
				else
				{
					if (((string)lines[lines.Count - 1]).Length + subMessage.Length + 1 <= this.MaxDisplayLineSize)
					{
						lines[lines.Count - 1] = ((string)lines[lines.Count - 1]) + " " + subMessage;
					}
					else
					{
						if (lines.Count >= this.MaxLines)
						{
							break;
						}

						lines.Add(subMessage);
					}
				}
			}

			var terminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
			{
				Value = Esc + "O" + Esc + "H" + Esc + "K"
			};

			string headerText = this.GetHeaderText();
			int lineNumber = 0;
			terminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + lineNumber).ToString(CultureInfo.InvariantCulture) + " " + headerText;

			++lineNumber;
			foreach (string line in lines)
			{
				terminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + lineNumber).ToString(CultureInfo.InvariantCulture) + " " + line;
				lineNumber++;
			}

			terminalCommand.Value += Esc + "@" + Esc + "L" + responseLength.ToString("D2") + Esc + "E" + Esc + "O";

			this.MessageTimer = messageTimeout;
			if (this.MessageTimer > 0)
			{
				this.MessageTimer++; // Adjust for logic in timeout countdown, which shaves off one second.
			}

			this.WriteLogDataToCommFile(terminalCommand.Value.ToString(), true);

		// we will try catch this block to ensure that we get the message to display.
		// the Multiload can cause an exception if we hit this too fast between messages
		RetryOPCComms:
			try
			{
				OPCServerManager.Write(new URL(StationPv.URL), new[] { terminalCommand });
			}
			catch (Exception e)
			{
				--retryCounter;
				if (retryCounter <= 0)
				{
					eventLog.WriteEntry("MultiloadIIStationManager SendMessage max retries reached : " + e.Message);
				}
				else
				{
					Thread.Sleep(200);
					goto RetryOPCComms;
				}
			}

			return lineNumber;
		}

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
			string message = this.GetDataDictionaryValueByKey(this.SiteManager.Site.SiteGuid, stockMessage);
			if (responseLength > 0)
			{
				message += ":";
			}

			this.WriteLogDataToCommFile(message, true);

			char[] separators = { ' ' };
			string[] strings = message.Split(separators);
			var lines = new ArrayList();

			foreach (string subMessage in strings)
			{
				if (subMessage.Length > this.MaxDisplayLineSize)
				{
					break;
				}

				if (lines.Count == 0)
				{
					lines.Add(subMessage);
				}
				else
				{
					if (((string)lines[lines.Count - 1]).Length + subMessage.Length + 1 <= this.MaxDisplayLineSize)
					{
						lines[lines.Count - 1] = ((string)lines[lines.Count - 1]) + " " + subMessage;
					}
					else
					{
						if (lines.Count >= this.MaxLines)
						{
							break;
						}

						lines.Add(subMessage);
					}
				}
			}

			var terminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
			{
				Value = Esc + "O" + Esc + "H" + Esc + "K"
			};

			string headerText = this.GetHeaderText();
			int lineNumber = 0;
			terminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + lineNumber).ToString(CultureInfo.InvariantCulture) + " " + headerText;

			++lineNumber;
			foreach (string line in lines)
			{
				terminalCommand.Value += Esc + "Y" + System.Convert.ToChar(0x20 + lineNumber).ToString(CultureInfo.InvariantCulture) + " " + line;
				lineNumber++;
			}

			terminalCommand.Value += Esc + "!" + Esc + "L" + responseLength.ToString("D2") + Esc + "E";

			OPCServerManager.Write(new URL(StationPv.URL), new[] { terminalCommand });

			this.MessageTimer = messageTimeout;
			if (this.MessageTimer > 0)
			{
				this.MessageTimer++; // Adjust for logic in timeout countdown, which shaves off one second.
			}
		}

		public override void ProcessMessageTimeout()
		{
			switch (this.StationState)
			{
				case StationState.IMPROPER_ADDITIZATION:
				case StationState.IMPROPER_ADDITIZATION_WEIGHTOUT:
					this.IssueImproperAdditizationWithAcknowledge();
					return;
			}

			base.ProcessMessageTimeout();
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			this.WriteMenuLogDataToCommFile(parameters);

			const string DefaultResponse = "None";
			int lineNumber = this.DisplayMessage(parameters.Caption, DefaultResponse, 2, PROMPT_TIMEOUT, false);

			var terminalCommand = new ItemValue(StationPv.OPCItemID + ".Terminal Command")
			{
				Value =
														 Esc
															 + "Y"
														 + System.Convert.ToChar(0x20 + lineNumber).ToString(CultureInfo.InvariantCulture)
															 + " "
			};

			for (int index = 0; index < parameters.Menu.Length && index < this.MaxLines - lineNumber; index++)
			{
				string menuItem = parameters.Menu[index];

				if (parameters.ApplyDataDictionary)
				{
					menuItem = this.GetDataDictionaryValueByKey(this.Station.SiteGuid, menuItem);
				}

				menuItem = (index + 1).ToString(CultureInfo.InvariantCulture) + ". " + menuItem;

				if (menuItem.Length > this.MaxDisplayLineSize + 4)
				{
					menuItem = menuItem.Substring(0, this.MaxDisplayLineSize);
				}

				terminalCommand.Value += menuItem + Esc + "O" + Esc + "Y" + System.Convert.ToChar(0x20 + lineNumber + index + 1).ToString(CultureInfo.InvariantCulture) + " ";
			}

			OPCServerManager.Write(new URL(StationPv.URL), new[] { terminalCommand });

			this.MessageTimer = parameters.MenuTimeout;
			if (this.MessageTimer > 0)
			{
				this.MessageTimer++; // Adjust for logic in timeout countdown, which shaves off one second.
			}

			this.CurrentMenuParameters = parameters;
		}

		protected override void ProcessPreloadDocumentSelection(string response)
		{
			if (response != EscapeString)
			{
				int selection = System.Convert.ToInt32(response);

				if (this.CurrentMenuParameters == null
					|| selection > CurrentMenuParameters.Menu.Length)
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_PRELOAD_DOCUMENT_SELECTION_MSG;
					return;
				}

				response = selection == 0 ? EscapeString : this.CurrentMenuParameters.Menu[selection - 1];
			}

			base.ProcessPreloadDocumentSelection(response);
		}

		protected override void ProcessPreloadLoadIDSelection(string response)
		{
			if (response != EscapeString)
			{
				int nSelection = System.Convert.ToInt32(response);

				if (this.CurrentMenuParameters == null
				|| nSelection > this.CurrentMenuParameters.Menu.Length)
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_PRELOAD_LOADID_SELECTION_MSG;
					return;
				}

				response = nSelection == 0 ? EscapeString : this.CurrentMenuParameters.Menu[nSelection - 1];
			}

			base.ProcessPreloadLoadIDSelection(response);
		}

		public override void EvaluateLoadArmStatus()
		{
			if (this.StationState == StationState.AUTHORIZING
				 || this.StationState == StationState.AUTHORIZED)
			{
				// We just started loading against a preload.  We aren't finishing; we're starting!
				return;
			}

			// Loop through the load arms and finish up if they are all at a good stopping point.
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS
					 || loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.END_BATCH_PROMPT
					 || loadArmManager.LoadArmState == LOADARM_STATE.BATCH_STOPPED_PROMPT)
				{
					return;
				}
			}

			bool allArmsFinished = true;
			foreach (LoadArmManagerClass lam in this.LoadArmManagerCollection)
			{
				if (this != lam.GetStationManager())
				{
					continue;
				}

				// For Multiload, we just return to the select arm screen; we never present a "Finished Loading"
				// prompt (it's not needed)
				if (lam.LoadArmState != LOADARM_STATE.FINISHED
					 && lam.LoadArmState != LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD
					 && lam.LoadArmState != LOADARM_STATE.BATCH_COMPLETE
					 && lam.LoadArmState != LOADARM_STATE.PRESET_ENABLED)
				{
					allArmsFinished = false;
					break;
				}
			}

			// When all arms are idle it may be possible to complete the transaction
			if (!allArmsFinished)
			{
				// In preload situation transaction can complete when all line items completed.
				if (this.PreloadDataSet != null
					 && this.PreloadDataSet.Tables[0].Rows.Count != 0)
				{
					foreach (LineItemDO lineItem in Transaction.LineItems)
					{
						if (lineItem.Status != TransactionStatus.Completed)
						{
							return;
						}
					}
				}
				else
				{
					// In non preload situation transaciton can complete when all compartments loaded.
					if (this.CompartmentList != null
						 && SiteManager.Site.PromptForCompartment)
					{
						foreach (CompartmentInfo compartment in this.CompartmentList)
						{
							bool found = false;
							foreach (LineItemDO lineItem in Transaction.LineItems)
							{
								if (lineItem.Status != TransactionStatus.Completed)
								{
									continue;
								}

								if (lineItem.DestinationEQ.EquipmentGuid == null)
								{
									continue;
								}

								if (lineItem.DestinationEQ.EquipmentGuid != compartment.EquipmentGuid)
								{
									continue;
								}

								int compartmentID;
								try
								{
									compartmentID = System.Convert.ToInt32(lineItem.DestinationCompartmentID);
								}
								catch
								{
									continue;
								}

								if (compartment.CompartmentNumber == compartmentID)
								{
									found = true;
									break;
								}
							}

							if (!found)
							{
								return;
							}
						}
					}
				}
			}

			this.SendEndTransaction();
			this.CompleteTransaction(!this.PromptForTransactionCompletion);
			if (this.StationState != StationState.BROKEN_BLEND
				 && this.StationState != StationState.IMPROPER_ADDITIZATION)
			{
				this.ResetStationDevice();
			}
		}

		public override void ProcessPreloadOrderSelection(string response)
		{
			if (response != EscapeString)
			{
				int selection = System.Convert.ToInt32(response);

				if (this.CurrentMenuParameters == null
				|| selection > CurrentMenuParameters.Menu.Length)
				{
					this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.INVALID_PRELOAD_ORDER_SELECTION_MSG;
					return;
				}

				response = selection == 0 ? EscapeString : CurrentMenuParameters.Menu[selection - 1];
			}

			base.ProcessPreloadOrderSelection(response);
		}

		public override void ReadLineItemData(
			LineItemDO lineItem,
			Server server,
			LoadArmManagerClass loadArmManager)
		{
			if (!(loadArmManager is MultiloadIILoadArmManagerClass multiloadIILoadArmManager))
			{
				throw new Exception("ReadLineItemData : Invalid LoadArmManager");
			}

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			ItemValueResult grossVolume;
			ItemValueResult netVolume;
			ItemValueResult averageTemperature;
			ItemValueResult averageDensity;

			ItemValueResult[] nonResettableTotal;

			multiloadIILoadArmManager.ReadNonResettableTotals(
				server,
				out nonResettableTotal);

			var pv = new ProcessVariableClass();

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				ProductMapClass component = loadArmManager.GetComponent(lineItem.ProductGuid);
				if (component == null)
				{
					throw new Exception("Component not found in LoadArm Configuration");
				}

				if (lineItem.Quantity == null)
				{
					lineItem.Quantity = new QuantityDO();
				}

				int componentPosition = multiloadIILoadArmManager.GetComponentPosition(component);

				multiloadIILoadArmManager.ReadComponentData(
					server,
					component,
					out grossVolume,
					out netVolume,
					out averageTemperature,
					out averageDensity);

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
							this.eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
							lineItem.Quantity.BadGrossQualityLogged = true;
						}
					}
					else
					{
						lineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(grossVolume.Value),
																							 lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						lineItem.Quantity.BadGrossQualityLogged = false;
					}
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
							this.eventLog.WriteEntry("ReadLineItemData : Component Net Volume OPC Quality Bad " + netVolume.ItemName, EventLogEntryType.Error);
							lineItem.Quantity.BadNetQualityLogged = true;
						}
					}
					else
					{
						lineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(netVolume.Value),
																							 lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						lineItem.Quantity.BadNetQualityLogged = false;
					}
				}

				if ((this.Station.Type == STATION_TYPE.OFF_LOADING) && this.Station.PromptForTemperature)
				{
					lineItem.Temperature = this.OffloadTemperature;
				}
				else
				{
					if (lineItem.Temperature == null)
					{
						lineItem.Temperature = 0.0;
					}

					if (averageTemperature.Quality != Quality.Good)
					{
						if (!lineItem.Temperature_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Temperature OPC Quality Bad " + averageTemperature.ItemName, EventLogEntryType.Error);
							lineItem.Temperature_BadQualityLogged = true;
						}
					}
					else
					{
						if (lineItem.TemperatureUnits == EngineeringUnit.FmtDegF)
						{
							lineItem.Temperature = Math.Round(System.Convert.ToDouble(averageTemperature.Value) / 10, lineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
						}
						else
						{
							lineItem.Temperature = Math.Round(System.Convert.ToDouble(averageTemperature.Value) / 100, lineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
						}
						lineItem.Temperature_BadQualityLogged = false;
					}
				}

				if (!lineItem.Quantity.BadGrossQualityLogged
				&& lineItem.Quantity.GrossInventoryChange != 0
				&& !lineItem.Quantity.BadNetQualityLogged
				&& lineItem.Quantity.NetInventoryChange != 0)
				{
					lineItem.VCF = Math.Round(lineItem.Quantity.NetInventoryChange / lineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
				}
				else
				{
					lineItem.VCF = null;
				}

				if ((this.Station.Type == STATION_TYPE.OFF_LOADING) && this.Station.PromptForGravity)
				{
					lineItem.Density = this.OffloadDensity;
				}
				else
				{
					if (lineItem.Density == null)
					{
						lineItem.Density = 0.0;
					}

					if (averageDensity.Quality != Quality.Good)
					{
						if (!lineItem.Density_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Density OPC Quality Bad " + averageDensity.ItemName, EventLogEntryType.Error);
							lineItem.Density_BadQualityLogged = true;
						}
					}
					else
					{
						// Presently the system expects that the Preset Units will match the Site Units
						double scale = 10;
						if (lineItem.DensityUnits == EngineeringUnit.FmdDegApi)
						{
							scale = 10;
						}
						else if (lineItem.DensityUnits == EngineeringUnit.FmdGcm3)
						{
							scale = 10000;
						}

						pv.ServerValue = System.Convert.ToDouble(averageDensity.Value) / scale;
						lineItem.Density = Math.Round(System.Convert.ToDouble(averageDensity.Value) / scale,
																		lineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
						lineItem.Density_BadQualityLogged = false;
					}
				}

				if (nonResettableTotal[componentPosition].Quality != Quality.Good)
				{
					if (lineItem.MeterReading.MeterStop == null)
					{
						lineItem.MeterReading.MeterStop = 0.0;
					}

					if (!lineItem.MeterReading.MeterStop_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[componentPosition].ItemName, EventLogEntryType.Error);
						lineItem.MeterReading.MeterStop_BadQualityLogged = true;
					}
				}
				else
				{
					if (lineItem.MeterReading.MeterStart == null)
					{
						lineItem.MeterReading.MeterStart = component.MeterValue;
						lineItem.MeterReading.StartDateTime = siteTimeNow;
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[componentPosition].Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}

					if (lineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[componentPosition].Value))
					{
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[componentPosition].Value);
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

			foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
			{
				if (subLineItem.Status != TransactionStatus.InProgress)
				{
					continue;
				}

				if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					ProductMapClass Component = loadArmManager.GetComponent(subLineItem.ProductGuid);
					if (Component == null)
						throw new Exception("Component not found in LoadArm Configuration");

					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
						continue;

					int componentPosition = multiloadIILoadArmManager.GetComponentPosition(Component);
					if (subLineItem.Quantity == null)
					{
						subLineItem.Quantity = new QuantityDO();
					}

					multiloadIILoadArmManager.ReadComponentData(
						server,
						Component,
						out grossVolume,
						out netVolume,
						out averageTemperature,
						out averageDensity);

					if (this.Station.Type == STATION_TYPE.OFF_LOADING) // for off loading we store the manually entered data instead of the actual data
					{
						subLineItem.Quantity.GrossInventoryChange = Math.Round(System.Convert.ToDouble(this.OffLoadPresetAmount), subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					}
					else
					{
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
							subLineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(grossVolume.Value),
																									  subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Quantity.BadGrossQualityLogged = false;
						}
					}

					if (this.Station.Type == STATION_TYPE.OFF_LOADING) // for off loading we store the manually entered data instead of the actual data
					{
						subLineItem.Quantity.NetInventoryChange = Math.Round(System.Convert.ToDouble(this.OffLoadPresetAmount), subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					}
					else
					{
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
							subLineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(netVolume.Value),
																									  subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Quantity.BadNetQualityLogged = false;
						}
					}

					if ((this.Station.Type == STATION_TYPE.OFF_LOADING) && this.Station.PromptForTemperature)
					{
						subLineItem.Temperature = this.OffloadTemperature;
					}
					else
					{
						if (subLineItem.Temperature == null)
						{
							subLineItem.Temperature = 0.0;
						}

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
							subLineItem.Temperature = Math.Round(System.Convert.ToDouble(averageTemperature.Value) / 10,
																					  subLineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Temperature_BadQualityLogged = false;
						}
					}

					if (!subLineItem.Quantity.BadGrossQualityLogged
					&& subLineItem.Quantity.GrossInventoryChange != 0
					&& !subLineItem.Quantity.BadNetQualityLogged
					&& subLineItem.Quantity.NetInventoryChange != 0)
					{
						subLineItem.VCF = Math.Round(subLineItem.Quantity.NetInventoryChange / subLineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
					}
					else
					{
						subLineItem.VCF = null;
					}

					if ((this.Station.Type == STATION_TYPE.OFF_LOADING) && this.Station.PromptForGravity)
					{
						subLineItem.Density = this.OffloadDensity;
					}
					else
					{
						if (subLineItem.Density == null)
						{
							subLineItem.Density = 0.0;
						}

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
							// Presently the system expects that the Preset Units will match the Site Units
							double scale = 10;
							if (subLineItem.DensityUnits == EngineeringUnit.FmdDegApi)
							{
								scale = 10;
							}
							else if (subLineItem.DensityUnits == EngineeringUnit.FmdGcm3)
							{
								scale = 10000;
							}

							subLineItem.Density = Math.Round(System.Convert.ToDouble(averageDensity.Value) / scale,
																				 subLineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
							subLineItem.Density_BadQualityLogged = false;
						}
					}

					if (nonResettableTotal[componentPosition].Quality != Quality.Good)
					{
						if (subLineItem.MeterReading.MeterStop == null)
						{
							subLineItem.MeterReading.MeterStop = 0.0;
						}

						if (!subLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[componentPosition], EventLogEntryType.Error);
							subLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						if (subLineItem.MeterReading.MeterStart == null)
						{
							subLineItem.MeterReading.MeterStart = Component.MeterValue;
							subLineItem.MeterReading.StartDateTime = siteTimeNow;
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[componentPosition].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}

						if (subLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[componentPosition].Value))
						{
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[componentPosition].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (Component.MeterValue != subLineItem.MeterReading.MeterStop.Value)
						{
							Component.MeterValue = subLineItem.MeterReading.MeterStop.Value;
							this.LastActivityDateTime = DateTimeOffset.Now;
						}
					}
				}

				else if (subLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(subLineItem.ProductGuid);
					if (additiveInjector == null)
					{
						throw new Exception("Additive not found in LoadArm Configuration");
					}

					int additiveInjectorIndex = multiloadIILoadArmManager.GetAdditivePosition(additiveInjector);
					if (additiveInjectorIndex == -1)
					{
						throw new Exception("Additive not found in LoadArm Configuration");
					}

					multiloadIILoadArmManager.ReadAdditiveData(server, additiveInjector, out grossVolume);

					if (grossVolume.Quality != Quality.Good)
					{
						if (!subLineItem.Quantity.BadGrossQualityLogged)
						{
							eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + grossVolume.ItemName, EventLogEntryType.Error);
							subLineItem.Quantity.BadGrossQualityLogged = true;
						}
					}
					else
					{
						byte decimalPlaces = (CurrentTransactionAlias._AdditiveVolumeDecimalPlaces != 0) ? CurrentTransactionAlias._AdditiveVolumeDecimalPlaces : this.SiteManager.Site._AdditiveVolumeDecimalPlaces;
						double interimQuantity = -Math.Round(System.Convert.ToDouble(grossVolume.Value) / 1000.0, decimalPlaces); // Multiload reports additive quantities in 1000ths of the delivery unit.
																																									 // Also, Multiload reports additive quantities in the delivered product units, need to convert to additive units to save
						subLineItem.Quantity.GrossInventoryChange = EngineeringUnits.Convert(interimQuantity, lineItem.VolumeUnits, subLineItem.VolumeUnits, 0);
						subLineItem.Quantity.BadGrossQualityLogged = false;
					}

					if (nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Quality != Quality.Good)
					{
						if (subLineItem.MeterReading.MeterStop == null)
						{
							subLineItem.MeterReading.MeterStop = 0.0;
						}

						if (!subLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Product Non-Resettable Gross Volume OPC Quality Bad " + nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count], EventLogEntryType.Error);
							subLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						ProcessVariableClass InternalPV = additiveInjector.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
						if (InternalPV == null)
						{
							if (subLineItem.MeterReading.MeterStart == null)
							{
								subLineItem.MeterReading.MeterStart = additiveInjector.MeterValue;
								subLineItem.MeterReading.StartDateTime = siteTimeNow;
								subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value);
								subLineItem.MeterReading.StopDateTime = siteTimeNow;
							}

							if (subLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value))
							{
								subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value);
								subLineItem.MeterReading.StopDateTime = siteTimeNow;
							}
						}
						else
						{
							double ServerValue = System.Convert.ToDouble(InternalPV.ServerValue);
							double RollOver = System.Convert.ToDouble(InternalPV.GetMaximum(InternalPV.ServerUnits, 10));
							double CurrentMeterValue = System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value);

							if (subLineItem.MeterReading.MeterStart == null)
							{
								subLineItem.MeterReading.MeterStart = ServerValue;
								subLineItem.MeterReading.StartDateTime = siteTimeNow;
								subLineItem.MeterReading.MeterStop = ServerValue;
								subLineItem.MeterReading.StopDateTime = siteTimeNow;
							}

							ServerValue += CurrentMeterValue - additiveInjector.MeterValue;
							if (CurrentMeterValue < additiveInjector.MeterValue)
							{
								ServerValue += 999999999.0;
							}

							if (ServerValue > RollOver)
							{
								ServerValue -= RollOver;
							}

							if (System.Convert.ToDouble(InternalPV.ServerValue) != ServerValue)
							{
								InternalPV.ServerValue = ServerValue;
								InternalPV.DateTimeStamp = DateTimeOffset.Now;

								this.ModifyProcessVariable(this.Security, DATA_TYPE.DYNAMIC, InternalPV);
							}

							if (subLineItem.MeterReading.MeterStop.Value != ServerValue)
							{
								subLineItem.MeterReading.MeterStop = ServerValue;
								subLineItem.MeterReading.StopDateTime = siteTimeNow;
							}
						}

						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (additiveInjector.MeterValue != System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value))
						{
							additiveInjector.MeterValue = System.Convert.ToDouble(nonResettableTotal[additiveInjectorIndex + loadArmManager.LoadArm.ComponentCollection.Count].Value);
							this.LastActivityDateTime = DateTimeOffset.Now;
						}

						// Acquire Density, Temperature, & VCF from the 
						// tank and compute?
						TankClass Tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
						if (Tank == null)
						{
							this.eventLog.WriteEntry("ReadLineItemData : No Additive Tank", EventLogEntryType.Error);
						}
						else
						{
							if (subLineItem.Temperature == null)
							{
								subLineItem.Temperature = 0.0;
							}

							pv = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV];
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
							|| !typeof(double).IsInstanceOfType(pv.SIValue))
							{
								if (!subLineItem.Temperature_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Temperature OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Temperature_BadQualityLogged = true;
								}
							}
							else
							{
								SIDouble Temperature = new SIDouble
								{
									Units = subLineItem.TemperatureUnits,
									SIValue = System.Convert.ToDouble(pv.SIValue)
								};

								subLineItem.Temperature = Math.Round(Temperature.Value, subLineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
								subLineItem.Temperature_BadQualityLogged = false;
							}

							if (subLineItem.Density == null)
							{
								subLineItem.Density = 0.0;
							}

							pv = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.DENSITY_PV];
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
							|| !typeof(double).IsInstanceOfType(pv.SIValue))
							{
								if (!subLineItem.Density_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Density OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Density_BadQualityLogged = true;
								}
							}
							else
							{
								SIDouble Density = new SIDouble
								{
									Units = subLineItem.DensityUnits,
									SIValue = System.Convert.ToDouble(pv.SIValue)
								};

								subLineItem.Density = Math.Round(Density.Value, subLineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
								subLineItem.Density_BadQualityLogged = false;
							}

							if (subLineItem.Quantity == null)
							{
								subLineItem.Quantity = new QuantityDO();
							}

							pv = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VCF_PV];
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
							|| !typeof(double).IsInstanceOfType(pv.SIValue))
							{
								if (!subLineItem.Quantity.BadNetQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive VCF OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
									subLineItem.Quantity.BadNetQualityLogged = true;
								}
							}
							else
							{
								if (subLineItem.VCF == null)
								{
									subLineItem.VCF = 0.0;
								}

								subLineItem.VCF = System.Convert.ToDouble(pv.SIValue);
								subLineItem.Quantity.NetInventoryChange = Math.Round(subLineItem.Quantity.GrossInventoryChange * subLineItem.VCF.Value,
																												subLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								subLineItem.Quantity.BadNetQualityLogged = false;
							}
						}
					}
				}
			}

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct))
			{
				this.RollUpSplashBlendTotals(lineItem);
			}
		}

		protected override void IssueImproperAdditization()
		{
			this.StationState = StationState.IMPROPER_ADDITIZATION;
			this.DisplayMessage("LoadRack|Improper Additization Detected.", null, 0, this.MESSAGE_TIMEOUT);
		}

		protected void IssueImproperAdditizationWithAcknowledge()
		{
			this.StationState = StationState.IMPROPER_ADDITIZATION;
			this.DisplayMessageWithAcknowledge("LoadRack|Improper Additization Detected.");
		}

		private void ModifyProcessVariable(SecurityClass Security, DATA_TYPE dataType, ProcessVariableClass processVariableClass)
		{
			FMChannelHelper.MakeCall<IProcessVariables>(
																	 x =>
																	 x.Modify(Security, dataType, processVariableClass)
																);
		}

		public override void CreateMeterReadingTransactions(
			SaveTransactionsSR saveTransactionsSR,
			TransactionAliasClass meterReadingTransactionAlias,
			DateTimeOffset inventoryDateTime)
		{
			foreach (MultiloadIILoadArmManagerClass LoadArmManager in this.FullLoadArmCollection)
			{
				// Skip Load Arms that are Swing Arms on the second bay to eliminate duplicates
				if (LoadArmManager.LoadArm.SwingArm
				&& LoadArmManager.BayB.StationManager == this
				&& LoadArmManager.BayA.StationManager != null)
					continue;

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

				ProcessVariableClass LoadArmPV = LoadArmManager.LoadArm.ProcessVariableCollection[0];
				Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
				NetworkCredential Credentials = null;
				Server.Connect(new ConnectData(Credentials));

				ItemValueResult[] NonResettableTotal;

				LoadArmManager.ReadNonResettableTotals(
					Server,
					out NonResettableTotal);

				if (LoadArmManager.LoadArm.ComponentCollection.Count != 0)
				{
					ProductMapClass Component = LoadArmManager.LoadArm.ComponentCollection[0];

					TransactionDO MeterReadingTransaction = this.CreateMeterReadingTransaction(
						siteTimeNow,
						LoadArmManager,
						NonResettableTotal[0],
						Component,
						meterReadingTransactionAlias,
						inventoryDateTime);

					if (MeterReadingTransaction != null)
					{
						saveTransactionsSR.Transactions.Add(MeterReadingTransaction);
					}
				}

				int ItemNumber = 1;
				foreach (ProductMapClass Additive in LoadArmManager.LoadArm.AdditiveInjectorCollection)
				{
					TransactionDO MeterReadingTransaction = this.CreateMeterReadingTransaction(
						siteTimeNow,
						LoadArmManager,
						NonResettableTotal[ItemNumber++],
						Additive,
						meterReadingTransactionAlias,
						inventoryDateTime);

					if (MeterReadingTransaction != null)
					{
						saveTransactionsSR.Transactions.Add(MeterReadingTransaction);
					}
				}
			}
		}

		protected string GetHeaderText()
		{
			string headerText = "Varec Terminal Automation";
			int numberOfSpaces = ((this.MaxDisplayLineSize - headerText.Length) / 2) + headerText.Length;
			headerText = headerText.PadLeft(numberOfSpaces, ' ');
			return headerText;
		}

		protected override void LoadRackProcessing(ProcessVariableClass pv)
		{
			Monitor.Enter(this);
			try
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
							foreach (MultiloadIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								logger.Debug("Input Permissive change - Arm " + loadArmManager.GetArmNumber(this).ToString(CultureInfo.InvariantCulture) + ", Arm State " + loadArmManager.LoadArmState.ToString());
								if (loadArmManager.LoadArmState == LOADARM_STATE.PRESET_ENABLED_PERMISSIVE_PROMPT)
								{
									// Try enabling the arm now.  EnablePreset will check permissives again for us
									logger.Debug("Calling EnablePreset for Arm " + loadArmManager.GetArmNumber(this).ToString(CultureInfo.InvariantCulture));
									loadArmManager.EnablePreset(this, true);
								}
							}

							base.LoadRackProcessing(pv);
							break;
						}

					case PROCESS_VARIABLE_TYPE.INPUT_DONE_PV:
						{
							if (pv.IsQualityGood
							&& (bool)pv.ServerValue)
							{
								Item[] items =
									 {
											new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Keypad Data")),
											new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".Terminating Key")),
													 new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".RCU Status"))
									  };
								ItemValueResult[] values = OPCServerManager.Read(new URL(StationPv.URL), items);
								if (values[0].Quality == Quality.Good && values[1].Quality == Quality.Good && values[2].Quality == Quality.Good)
								{
									bool messageProcessed = false;
									if (this.StationState != StationState.RESET_ON_TIMEOUT)
									{
										this.MessageTimer = 0;
									}

									string terminatingKey = values[1].Value.ToString();
									string keypadData = string.Empty;
									char rcuStatus = System.Convert.ToChar(values[2].Value);

									foreach (MultiloadIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
									{
										if (this != loadArmManager.GetStationManager())
										{
											continue;
										}

										if (!loadArmManager.IsStatusForThisLoadArm(rcuStatus))
										{
											continue;
										}

										// Enter Key
										if (terminatingKey == "D")
										{
											keypadData = values[0].Value.ToString();
											keypadData = keypadData.TrimEnd(new[] { ' ' });
										}
										else if (terminatingKey == "B")
										{
											// Prev Key
											if (loadArmManager.LoadArmState == LOADARM_STATE.SELECT_RECIPE_PROMPT
												 || loadArmManager.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT)
											{
												loadArmManager.CancelPresetting(this);
											}

											keypadData = EscapeString;
										}
										else if (terminatingKey == "C")
										{
											// Exit Key
											if (loadArmManager.LoadArmState == LOADARM_STATE.NORMAL)
											{
												// this.StationState = STATION_STATE.IDLE;
												// Allow the RCU Status Change dictate behavior
												break;
											}
											// ReSharper disable once RedundantIfElseBlock
											else if (loadArmManager.LoadArmState == LOADARM_STATE.COMPARTMENT_PROMPT
														|| loadArmManager.LoadArmState == LOADARM_STATE.SELECT_RECIPE_PROMPT
														|| loadArmManager.LoadArmState == LOADARM_STATE.PRESET_VOLUME_PROMPT
														|| loadArmManager.LoadArmState == LOADARM_STATE.EQUIPMENTID_PROMPT)
											{
												loadArmManager.CancelPresetting(this);
												loadArmManager.TerminalCommandAuthorize();
												break;
											}
										}

										if (loadArmManager.LoadArmProcessResponseData(this, keypadData))
										{
											messageProcessed = true;
										}
									}

									if (messageProcessed == false)
									{
										this.ProcessResponseData(keypadData);
									}
								}
								else
								{
									this.eventLog.WriteEntry("Multiload II OnInvoke : Keypad Data OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
								}
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.LOADARM_PV:
						{
							if (pv.OPCItemID.EndsWith("Status.Power Up"))
							{
								if (pv.IsQualityGood
								&& (bool)pv.ServerValue)
								{
									ItemValue FunctionPowerUp = new ItemValue(this.StationPv.OPCItemID + ".FPOWERUP");
									this.OPCServerManager.Write(new URL(this.StationPv.URL), new[] { FunctionPowerUp });
								}
							}
							else if (pv.OPCItemID.EndsWith("Status.Host Up"))
							{
								if (pv.IsQualityGood
								&& !(bool)pv.ServerValue)
								{
									ItemValue FunctionHostUp = new ItemValue(this.StationPv.OPCItemID + ".FHOSTUP");
									this.OPCServerManager.Write(new URL(this.StationPv.URL), new[] { FunctionHostUp });
								}
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
						{
							if (pv.OPCItemID.EndsWith(".Card Number"))
							{
								if (pv.IsQualityGood)
								{
									if ((string)pv.ServerValue != string.Empty &&
										Station.CardReader)
									{
										if (this.StationState == StationState.IDLE)
										{
											this.ProcessDriverID((string)pv.ServerValue);
										}
									}
								}
							}
							else if (pv.OPCItemID.EndsWith(".Card Status"))
							{
								if (pv.IsQualityGood)
								{
									if ((string)pv.ServerValue != "49" &&
										Station.CardReader)
									{
										switch (this.StationState)
										{
											case StationState.IDLE:
											case StationState.AUTHORIZED:
											case StationState.TRANSACTION_IN_PROGRESS:
												// Note that for AUTHORIZED and TRANSACTION_IN_PROGRESS, transaction end is controlled by the arm managers
												break;
											default:
												this.ResetStationDevice();
												break;
										}
									}
								}
							}

							break;
						}

					case PROCESS_VARIABLE_TYPE.TERMINATION_KEY:
						{
							if (pv.IsQualityGood)
							{
								string TerminatingKey = pv.ServerValue.ToString();
								if (TerminatingKey == "C")
								{
									this.OPCServerManager.Read(this.CardReaderStatusPV);
									if (this.CardReaderStatusPV.IsQualityGood && this.CardReaderStatusPV.ServerValue.ToString() != "49" && this.StationState != StationState.TRANSACTION_IN_PROGRESS)
									{
										foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
										{
											if (this != LoadArmManager.GetStationManager())
												continue;

											if (LoadArmManager.LoadArmState == LOADARM_STATE.BATCH_COMPLETE ||
												LoadArmManager.LoadArmState == LOADARM_STATE.PRESET_ENABLED)
											{
												// we only need to send this to one loadarm
												LoadArmManager.SendEndTransaction();
												break;
											}
										}
									}
								}
							}

							break;
						}

					default:
						base.LoadRackProcessing(pv);
						break;
				}
			}
			catch (OpcException e)
			{
				this.eventLog.WriteEntry("Multiload II LoadArmManager OnInvoke : " + e.Message, EventLogEntryType.Error);
				this.CommunicationsFailure = true;
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("Multiload II LoadArmManager OnInvoke : PV = " + pv.OPCItemID + " " + e, EventLogEntryType.Error);
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		protected override bool CheckProductsInStation()
		{
			return this.WriteProductDataToMultiLoad();
		}

		protected override void SetProductsInStation()
		{
			//this.WriteProductDataToMultiLoad();
		}

		protected bool WriteProductDataToMultiLoad()
		{
			foreach (MultiloadIILoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				int productIndex;

				// check the components
				if (loadArmManager.LoadArm.ComponentCollection != null)
				{
					foreach (ProductMapClass component in loadArmManager.LoadArm.ComponentCollection)
					{
						productIndex = component.PresetNumber;
						if (!this.CheckComponentProductDefinition(productIndex))
						{
							// TODO: Make the string aliasable
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, loadArmManager.LoadArm.ConfigurationMismatchAlarm(this.Station.ID, "Mismatch on Component preset number " + productIndex.ToString(CultureInfo.InvariantCulture))));
							return false;
						}
					}
				}

				// check the injectors
				if (loadArmManager.LoadArm.AdditiveInjectorCollection != null)
				{
					foreach (ProductMapClass injector in loadArmManager.LoadArm.AdditiveInjectorCollection)
					{
						productIndex = injector.PresetNumber;
						if (!this.CheckAdditiveProductDefinition(productIndex))
						{
							// TODO: Make the string aliasable
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, loadArmManager.LoadArm.ConfigurationMismatchAlarm(this.Station.ID, "Mismatch on Injector preset number " + productIndex.ToString(CultureInfo.InvariantCulture))));
							return false;
						}
					}
				}

				// check the recipes, and update additive quantities
				if (loadArmManager.AvailableRecipeCollection != null)
				{
					// var additiveProfiles = new AdditiveProfilesClass();
					foreach (ProductMapClass loadArmRecipe in loadArmManager.AvailableRecipeCollection)
					{
						productIndex = loadArmRecipe.PresetNumber;
						if (loadArmRecipe.AssignedProductType != ProductType.BlendProduct
									&& loadArmRecipe.AssignedProductType != ProductType.ComponentProduct)
						{
							continue;
						}

						ProductMapClass authorizedProduct = GetAuthorizedProduct(loadArmRecipe.AssignedID);

						if (authorizedProduct == null)
						{
							continue;
						}

						if (!this.CheckAndWriteBlendProductDefinition(productIndex, loadArmRecipe, loadArmManager))
						{
							// TODO: Make the string aliasable
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, loadArmManager.LoadArm.ConfigurationMismatchAlarm(this.Station.ID, "Mismatch on Recipe preset number " + productIndex.ToString(CultureInfo.InvariantCulture))));
							return false;
						}
					}
				}

				if (!loadArmManager.CheckLoadArmAuthorizedProducts())
				{
					// TODO: Make the string aliasable
					FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, loadArmManager.LoadArm.ConfigurationMismatchAlarm(this.Station.ID, "Mismatch on Arm assigned products")));
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Verifies that the product defined in register on the Multiload is configured as a component.
		/// We ONLY check that this product exists and is a component; we don't fail for any other reason currently
		/// </summary>
		/// <param name="register">The register in the Multiload to be checked.</param>
		/// <returns>True if the product definition at register is a component; false otherwise</returns>
		private bool CheckComponentProductDefinition(int register)
		{
			// Read product definition from the station
			ProcessVariableClass stationPv = this.Station.ProcessVariableCollection[0];

			string tagPrefix = stationPv.OPCItemID + ".Product.";

			ItemValueResult[] values = OPCServerManager.Read(
				 new URL(stationPv.URL),
				 new[] { new Item(new ItemIdentifier(tagPrefix + register.ToString("D", CultureInfo.InvariantCulture))) });

			if (values[0].Quality != Quality.Good)
			{
				return false;
			}

			var deviceProductDefinition = values[0].Value as string;
			if (string.IsNullOrEmpty(deviceProductDefinition))
			{
				return false;
			}

			// Build the component/additive configuration section
			// this should show a 100% composition of self as component, with zero additives
			string configurationSection = register.ToString("D3") + "10000" // components
														+ "00000000" + "00000000"
														+ "00000000" + "00000000"
														+ "00000000" + "00000000"
														+ "00000000"
														+ "00000000" + "00000000" // additives
														+ "00000000" + "00000000";

			if (string.CompareOrdinal(configurationSection, 0, deviceProductDefinition, 42, 96) != 0)
			{
				// not a component
				return false;
			}

			return true;
		}

		/// <summary>
		/// Verifies that the product defined in register on the Multiload is configured as an additive.
		/// We ONLY check that this product exists and is an additive; we don't fail for any other reason currently
		/// </summary>
		/// <param name="register">The register in the Multiload to be checked.</param>
		/// <returns>True if the product definition at register is a component; false otherwise</returns>
		private bool CheckAdditiveProductDefinition(int register)
		{
			// Read product definition from the station
			ProcessVariableClass stationPv = this.Station.ProcessVariableCollection[0];

			string tagPrefix = stationPv.OPCItemID + ".Product.";

			ItemValueResult[] values = OPCServerManager.Read(
				 new URL(stationPv.URL),
				 new[] { new Item(new ItemIdentifier(tagPrefix + register.ToString("D", CultureInfo.InvariantCulture))) });

			if (values[0].Quality != Quality.Good)
			{
				return false;
			}

			var deviceProductDefinition = values[0].Value as string;
			if (string.IsNullOrEmpty(deviceProductDefinition))
			{
				return false;
			}

			// Build the component/additive configuration section
			// this should show a 100% composition of self as component, and again
			// as an additive with a zero percent rate
			string configurationSection = register.ToString("D3") + "10000" // components
														+ "00000000" + "00000000"
														+ "00000000" + "00000000"
														+ "00000000" + "00000000"
														+ "00000000"
														+ register.ToString("D3") + "00000" + "00000000" // additives
														+ "00000000" + "00000000";

			if (string.CompareOrdinal(configurationSection, 0, deviceProductDefinition, 42, 96) != 0)
			{
				// not an additive
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks the blend product definition for being a blend and having a configuration which
		/// matches TAS.  After verifying that that is correct, update the additives to match the
		/// additive profile.
		/// </summary>
		/// <param name="register">The register address for the product definition.</param>
		/// <param name="loadArmRecipe">The load arm recipe.</param>
		/// <param name="loadArmManager">The load arm manager.</param>
		/// <returns>Success or failure</returns>
		private bool CheckAndWriteBlendProductDefinition(int register, ProductMapClass loadArmRecipe, LoadArmManagerClass loadArmManager)
		{
			string[] componentID = { "000", "000", "000", "000", "000", "000", "000", "000" };
			string[] blendPercentage = { "00000", "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
			string[] additiveID = { "000", "000", "000", "000" };
			string[] additivePercentage = { "00000", "00000", "00000", "00000" };
			int index;

			if (loadArmRecipe.AssignedProductType == ProductType.BlendProduct)
			{
				// get the recipe components
				ProductMapCollectionClass recipeComponentCollection =
					 FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(productMaps => productMaps.EnumerateByAssignedToGuidAndType(
						  this.Security,
						  loadArmRecipe.AssignedGuid,
						  PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP));

				// get the component configuration for configuration
				index = 0;
				foreach (ProductMapClass component in recipeComponentCollection)
				{
					if (!string.IsNullOrEmpty(component.AssignedID))
					{
						ProductMapClass componentMapEntry = loadArmManager.GetComponent(component.AssignedGuid, false, false);
						if (componentMapEntry != null)
						{
							int componentIndex = componentMapEntry.PresetNumber;
							if (componentIndex != -1)
							{
								componentID[index] = componentIndex.ToString("D3", CultureInfo.InvariantCulture);
								blendPercentage[index] =
									 ((int)
									  (System.Convert.ToDouble(component.BlendPercentage.ToString(CultureInfo.InvariantCulture))
										* 100.0)).ToString(CultureInfo.InvariantCulture).PadLeft(5, '0').Substring(0, 5);
								++index;
							}
						}
					}
				}
			}
			else
			{
				index = 0;
				if (!string.IsNullOrEmpty(loadArmRecipe.AssignedID))
				{
					ProductMapClass componentMapEntry = loadArmManager.GetComponent(loadArmRecipe.AssignedGuid, false, false);
					if (componentMapEntry != null)
					{
						int componentIndex = componentMapEntry.PresetNumber;
						if (componentIndex != -1)
						{
							componentID[index] = componentIndex.ToString("D3", CultureInfo.InvariantCulture);
							blendPercentage[index] = "10000"; // Straight component is 100% of itself
							++index;
						}
					}
				}
			}

			//ProductMapClass currentProduct = ShipTo.AuthorizedProductCollection.Find(x => x.AssignedGuid == loadArmRecipe.AssignedGuid);
			ProductMapClass currentProduct = null;
			foreach (ProductMapClass authorizedProduct in ShipTo.AuthorizedProductCollection)
			{
				if (authorizedProduct.AssignedGuid == loadArmRecipe.AssignedGuid)
				{
					currentProduct = authorizedProduct;
					break;
				}

				// We may have a mismatch between a product master record guid (used by load arm mappings)
				// and a product record versioned guid (used by ship-to authorized product collections)
				// Try to resolve that.
				ProductClass product = this.GetProduct(this.Security, authorizedProduct.AssignedGuid);
				if (product != null && product.MasterRecordGuid == loadArmRecipe.AssignedGuid)
				{
					currentProduct = authorizedProduct;
					break;
				}
			}

			index = 0;
			if (currentProduct.AdditiveProfileGuid != Guid.Empty)
			{
				AdditiveProfileClass additiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(additiveProfiles => additiveProfiles.Get(Security, currentProduct.AdditiveProfileGuid));

				foreach (ProductMapClass injector in loadArmManager.LoadArm.AdditiveInjectorCollection)
				{
					ProductMapClass profileAdditive = additiveProfile.AdditiveCollection.Find(x => x.AssignedGuid == injector.AssignedGuid);
					if (profileAdditive != null)
					{
						// keep the volumes in SI volume unit so that the ratio can be determined properly
						double cycleVolume = profileAdditive._AdditiveCycleVolume.SIValue;
						double rate = profileAdditive._AdditiveRate.SIValue;

						double percent = cycleVolume / (rate + cycleVolume);

						if (percent > .065535 || percent < .000001)
						{
							eventLog.WriteEntry("MultiloadIILoadArmManager Authorize : Invalid Percentage for Additive " + profileAdditive.AssignedID);
						}
						else
						{
							ProductMapClass additiveProductMap = loadArmManager.GetAdditive(profileAdditive.AssignedGuid);
							if (additiveProductMap != null)
							{
								int additiveIndex = additiveProductMap.PresetNumber;
								if (additiveIndex != -1)
								{
									additiveID[index] = additiveIndex.ToString("D3", CultureInfo.InvariantCulture);
									additivePercentage[index] = System.Convert.ToInt32(percent * 1000000).ToString("D5");
									++index;
								}
							}
						}
					}
				}
			}

			// Get the current product definition for this blend
			ProcessVariableClass stationPv = this.Station.ProcessVariableCollection[0];

			string tagPrefix = stationPv.OPCItemID + ".Product.";

			ItemValueResult[] values = OPCServerManager.Read(
				 new URL(stationPv.URL),
				 new[] { new Item(new ItemIdentifier(tagPrefix + register.ToString("D", CultureInfo.InvariantCulture))) });

			if (values[0].Quality != Quality.Good)
			{
				return false;
			}

			var deviceProductDefinition = values[0].Value as string;
			if (string.IsNullOrEmpty(deviceProductDefinition))
			{
				return false;
			}

			// build up components section to compare.
			// TODO: add handling for external components.
			// Multiload doesn't seem to do flow-controlled additives
			var componentStringBuilder = new StringBuilder();
			for (int componentEntryIndex = 0; componentEntryIndex < componentID.Length; componentEntryIndex++)
			{
				componentStringBuilder.Append(componentID[componentEntryIndex]);
				componentStringBuilder.Append(blendPercentage[componentEntryIndex]);
			}

			string componentString = componentStringBuilder.ToString();
			if (string.CompareOrdinal(componentString, 0, deviceProductDefinition, 42, 64) != 0)
			{
				return false;
			}

			if (loadArmRecipe.AssignedProductType == ProductType.ComponentProduct)
			{
				// Straight components delivered from the Multiload cannot have additives
				// Attempting to add additives changes the product from a component to a deliverable products;
				// we must not do that.
				return currentProduct.AdditiveProfileGuid == Guid.Empty;
			}

			var additiveStringBuilder = new StringBuilder();
			for (int additiveEntryIndex = 0; additiveEntryIndex < additiveID.Length; additiveEntryIndex++)
			{
				additiveStringBuilder.Append(additiveID[additiveEntryIndex]);
				additiveStringBuilder.Append(additivePercentage[additiveEntryIndex]);
			}

			string additiveString = additiveStringBuilder.ToString();

			var updateRegister = new ItemValue(StationPv.OPCItemID + ".Write Register")
			{
				// deviceProductDefinition has been checked above
				// ReSharper disable once PossibleNullReferenceException
				Value = "500" + register.ToString("D3", CultureInfo.InvariantCulture)
																	  + deviceProductDefinition.Substring(0, 106) + additiveString
			};

			// Update additive paramteters.
			OPCServerManager.Write(new URL(StationPv.URL), new[] { updateRegister });
			return true;
		}

		protected override void BuildOffloadRecipeMapForAllLoadArms(bool acknowledged)
		{
			try
			{
				if (this.StationState != StationState.BUILD_RECIPE_MAP)
				{
					this.BuildRecipeMapAuthorizedProductIndex = 0;
				}
				else
				{
					if (acknowledged)
					{
						this.BuildRecipeMapAuthorizedProductIndex++;
						this.StationState = StationState.AUTHORIZING;
					}
					else
					{
						this.CompleteTransaction(!this.PromptForTransactionCompletion);
						this.DisplayMessage("LoadRack|Message Timeout", null, 0, this.MESSAGE_TIMEOUT);
						this.ConsecutivePrompts = 0;
						this.StationState = StationState.IDLE;
						return;
					}
				}

				for (; this.BuildRecipeMapAuthorizedProductIndex < this.Supplier.AuthorizedProductCollection.Count; this.BuildRecipeMapAuthorizedProductIndex++)
				{
					ProductMapClass authorizedProduct = this.Supplier.AuthorizedProductCollection[this.BuildRecipeMapAuthorizedProductIndex];

					// LockedOut can be set for the Product or is the result of Allocation Load Denial
					if (authorizedProduct.LockedOut)
					{
						continue;
					}

					bool productServicedByStation = false;
					bool productAvailable = false;

					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						ProductMapClass recipe = loadArmManager.GetRecipe(authorizedProduct.AssignedGuid);
						if (recipe == null)
						{
							continue;
						}

						if (!recipe.EnableRecipe)
						{
							continue;
						}

						productServicedByStation = true;

						ProductClass product = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(this.Security, authorizedProduct.AssignedGuid, false));

						// enforce the component checks here
						if (product.ProductType == ProductType.AdditiveProduct)
						{
							continue;
						}

						EngineeringUnit volumeUnit = (this.CurrentTransactionAlias.VolumeUnits != 0) ? this.CurrentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
						var maximum = new SIDouble { Units = volumeUnit, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

						if (!this.IsProductAvailable(product, loadArmManager, maximum.Value, this.CurrentTransactionAlias))
						{
							continue;
						}

						productAvailable = true;

						loadArmManager.Bay(this).RecipeMap |= (ulong)0x1 << (loadArmManager.GetRecipeNumber(recipe) - 1);

						if (this == loadArmManager.GetStationManager())
						{
							string name = GetLoadRackDisplayText(authorizedProduct);

							if (!loadArmManager.UpdateRecipe(name, recipe, product, null, recipe.PresetNumber))
							{
								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}

								this.StationState = StationState.UPDATE_RECIPE_ERROR_MSG;
								this.DisplayMessageWithAcknowledge("LoadRack|Update Recipe Error");
								return;
							}
						}
					}

					if (productServicedByStation)
					{
						if (!productAvailable)
						{
							FMChannelHelper.MakeCall<IAlarmAndEventLogs>(alarmAndEventLogs => alarmAndEventLogs.Add(this.Security, this.Station.ProductUnavailableAlarm(authorizedProduct.AssignedID, this.Driver.ID)));
							this.LoadRackManager.EventOrAlarmEvent.Set();
							this.StationState = StationState.BUILD_RECIPE_MAP;

							foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
							{
								if (this != loadArmManager.GetStationManager())
								{
									continue;
								}

								if (!loadArmManager.LogOutOfProgramMode())
								{
									return;
								}
							}

							this.DisplayMessageWithAcknowledge("[LoadRack|Product is not available] : " + GetLoadRackDisplayText(authorizedProduct));
							return;
						}
					}
				}

				bool anyArmAuthorized = false;
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (this != loadArmManager.GetStationManager())
					{
						continue;
					}

					if (loadArmManager.EnablePreset(this, true))
					{
						// Focus to first arm authorized
						if (anyArmAuthorized == false && !this.HasSwingArms && !loadArmManager.IsInAlarm)
						{
							loadArmManager.SetFocus();
						}

						anyArmAuthorized = true;
					}
				}

				if (anyArmAuthorized)
				{
					if (this.Transaction == null)
					{
						this.InitializeTransaction();
						this.Transaction.DeleteFlag = true;
					}

					this.StartDateTime = DateTime.UtcNow;
					this.StationState = StationState.AUTHORIZED;
					this.LastActivityDateTime = DateTime.UtcNow;

					if (this.Transaction.RouteSchedule.FST == null)
					{
						var timeConverter = new SiteTimeConverter(this.SiteManager.Site);
						this.Transaction.RouteSchedule.FST = timeConverter.ConvertToSiteTime(DateTime.UtcNow);
					}
				}
				else
				{
					if (this.Transaction != null)
					{
						this.Transaction.Status = TransactionStatus.Cancelled;
						this.SaveTransaction();
						this.Transaction = null;
					}

					this.EndTransaction();
					this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.NORMAL);
					Thread.Sleep(this.MESSAGE_TIMEOUT * 1000);
					this.DisplayMessage("[LoadRack|No Arms Authorized]", null, 0, this.MESSAGE_TIMEOUT);
					this.StationState = StationState.RESET_ON_TIMEOUT;
				}
			}
			catch (Exception)
			{
				// if we hit this it is most likely because we tried to connect to an OPC server that was not there
				// this is a fatal error so we will restart.
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|OPC IO Error Process Terminated", null, 0, this.MESSAGE_TIMEOUT);
			}
		}

		public bool IsOffloadProductAvailable(ProductClass product, LoadArmManagerClass loadArmManager, double maximumLoadAmount, TransactionAliasClass currentTransactionAlias)
		{
			if (product.ProductType == ProductType.ComponentProduct)
			{
				ProductMapClass component = loadArmManager.GetComponent(product.MasterRecordGuid);
				if (component == null)
				{
					this.eventLog.WriteEntry("No Component for Recipe : " + product.ID, EventLogEntryType.Error);
					return false;
				}

				if (component.Type == PRODUCT_MAP_TYPE.PRESET_FLOW_CONTROLLED_ADDITIVE_MAP)
				{
					this.eventLog.WriteEntry("Flow Controlled Additive for Recipe : " + product.ID, EventLogEntryType.Error);
					return false;
				}

				if (!component.Permissives.Permitted)
				{
					this.eventLog.WriteEntry("Component Disabled by Permissives : " + component.AssignedID, EventLogEntryType.Error);
					return false;
				}

				TankClass tank = this.SiteManager.GetTank(component, this.Manager);
				if (tank == null)
				{
					this.eventLog.WriteEntry("No Tank for Component : " + component.AssignedID, EventLogEntryType.Error);
					return false;
				}
			}
			else
			{
				foreach (ProductMapClass blendComponent in product.ComponentCollection)
				{
					ProductMapClass component = loadArmManager.GetComponent(blendComponent.AssignedGuid);
					if (component == null)
					{
						this.eventLog.WriteEntry("No Component for Blend Component : " + blendComponent.AssignedID, EventLogEntryType.Error);
						return false;
					}

					if (!component.Permissives.Permitted)
					{
						this.eventLog.WriteEntry("Blend Component Disabled by Permissives : " + blendComponent.AssignedID, EventLogEntryType.Error);
						return false;
					}

					TankClass tank = this.SiteManager.GetTank(component, this.Manager);
					if (tank == null)
					{
						this.eventLog.WriteEntry("No Tank for Component : " + component.AssignedID, EventLogEntryType.Error);
						return false;
					}

					ProcessVariableClass pv = !this.SiteManager.Site.LoadByNet
						 ? tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV]
						 : tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV];
					if (pv == null)
					{
						this.eventLog.WriteEntry("No Tank Available Volume Process Variable for Tank : " + tank.ID, EventLogEntryType.Error);
						return false;
					}

					if ((!this.SiteManager.Site.UseLastKnownGoodTankData
						 && !pv.IsQualityGood)
						 || !(pv.SIValue is double))
					{
						this.eventLog.WriteEntry("Available Gross Volume OPC Quality Bad for Tank : " + tank.ID, EventLogEntryType.Error);
						return false;
					}

					EngineeringUnit units = (currentTransactionAlias.VolumeUnits != 0) ? currentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
					byte decimalPlaces = (currentTransactionAlias.VolumeUnits != 0) ? currentTransactionAlias._VolumeDecimalPlaces : this.SiteManager.Site._VolumeDecimalPlaces;

					var maximumAvailable = (double)pv.GetValue(units, decimalPlaces);

					if (maximumLoadAmount * blendComponent.BlendPercentage / 100 > maximumAvailable)
					{
						return false;
					}
				}
			}

			return true;
		}

		protected override bool LateLoadArmAuthorizedProductCheck(LoadArmManagerClass loadArmManager)
		{
			// Most stations can ignore this check; exceptions
			// are stations where TAS controls product selection after arm selection.
			return loadArmManager.CurrentRecipe != null;
		}

		public int GetComponentIndex(string ComponentID)
		{
			int Index = -1;

			if (this.ProductIndexes.Contains(ComponentID))
			{
				for (int iLoop = 0; iLoop < this.ProductIndexes.Count; iLoop++)
				{
					if (ComponentID == this.ProductIndexes[iLoop].ToString())
					{
						return iLoop;
					}
				}
			}
			return Index;
		}

		protected override void StartPreloadBatches()
		{
			bool atLeastOneArmAuthorized = false;

			// Need to set this so the load screen remains up
			// long enough for the driver to react to it and so the
			// command sequence doesn't stomp on itself in the
			// case that no arms are authorized
			this.LastActivityDateTime = DateTime.Now;

			this.StationState = StationState.AUTHORIZING;
			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (LoadArmManager.IsInAlarm)
				{
					continue;
				}

				if (this != LoadArmManager.GetStationManager())
				{
					continue;
				}

				if (!LoadArmManager.ShowNoProductsMessage())
				{
					LoadArmManager.EnablePreset(this, false);
					atLeastOneArmAuthorized = true;
				}
			}

			this.StationState = StationState.AUTHORIZED;

			foreach (LineItemDO LineItem in this.Transaction.LineItems)
			{
				if (LineItem.Status != TransactionStatus.LoadPending)
				{
					continue;
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

					foreach (LineItemDO Preload in LoadArmManager.Bay(this).PreLoads)
					{
						if (Preload == LineItem)
						{
							LoadArmManager.SetFocus();
							return;
						}
					}
				}
			}

			if (!atLeastOneArmAuthorized)
			{
				// If no arm is authorized, we need to at least go to the authorize transaction screen for the
				// transaction exit process to be available.  Find the first load arm that is enabled and authorize
				// the transaction, but not the arm.
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					if (loadArmManager is MultiloadIILoadArmManagerClass multiloadIILoadArmManager)
					{
						multiloadIILoadArmManager.TerminalCommandAuthorize();
						break;
					}
				}
			}
		}

		public override void DisplayOffLoadProductSelect()
		{
			// for the Multiload SMP we need to populate the menu with the configured arm products
			if (this.AvailableLoadArmManagers == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// check that the supplier has authorized products configured
			if (this.Supplier.SupplierAuthorizedProductCollection.Count == 0)
			{
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			// Build menu parameter set
			var parameters = new DisplayMenuParameters
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
				this.StationState = StationState.RESET_ON_TIMEOUT;
				this.DisplayMessage("LoadRack|No Products Available", null, 0, this.MESSAGE_TIMEOUT);
				return;
			}

			parameters.Menu = menu.ToArray();

			this.StationState = StationState.SELECT_OFFLOAD_PRODUCT;

			this.DisplayMenu(parameters);
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
				 UseOffLoadSupplyOrders == false)
			{
				foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
				{
					foreach (ProductMapClass productMap in loadArmManager.LoadArm.ProductRecipeCollection)
					{
						if (menuNumber == System.Convert.ToInt32(response))
						{
							loadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(Security, productMap.AssignedGuid, false));
							if (loadArmManager.CurrentLineItemProduct != null)
							{
								if (productMap.PresetNumber >= 64)
								{
									loadArmManager.Bay(this).ExtendedRecipeMap |= (ulong)1 << ((productMap.PresetNumber - 64) - 1);
								}
								else
								{
									loadArmManager.Bay(this).RecipeMap |= (ulong)1 << (productMap.PresetNumber - 1);
								}
								loadArmManager.CurrentRecipe = productMap;
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
				if (SupplyOrder.LineItems.Count > 0)
				{
					foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
					{
						// check for different products in the line items and present the user with a selection
						// ReSharper disable once ForCanBeConvertedToForeach
						for (int index = 0; index < SupplyOrder.LineItems.Count; index++)
						{
							var lineItem = SupplyOrder.LineItems[index];
							if (menuNumber == System.Convert.ToInt32(response))
							{
								loadArmManager.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(Security, lineItem.ProductGuid, false));
								ProductMapClass productMap = loadArmManager.LoadArm.ProductRecipeCollection.Find(productRecipe => productRecipe.AssignedGuid == lineItem.ProductGuid);
								if (loadArmManager.CurrentLineItemProduct != null)
								{
									if (productMap.PresetNumber >= 64)
									{
										loadArmManager.Bay(this).ExtendedRecipeMap |= (ulong)1 << ((productMap.PresetNumber - 64) - 1);
									}
									else
									{
										loadArmManager.Bay(this).RecipeMap |= (ulong)1 << (productMap.PresetNumber - 1);
									}
									loadArmManager.CurrentRecipe = productMap;
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

			this.StationState = StationState.AUTHORIZED;

			var loadArmManager = (MultiloadIILoadArmManagerClass)this.LoadArmManagerCollection.Item(0);

			loadArmManager.EnablePreset(this, false);

			if (!loadArmManager.Authorize(this, this.OffLoadPresetAmount))
			{
				// turn anything off that we may of turned on
				this.UpdatePermissives(false);
			}
		}

		protected override void DisplayPleaseWaitMessage()
		{
			// Do nothing
		}
	}
}
