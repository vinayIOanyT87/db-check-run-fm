/******************************************************************************

	FILE NAME:		Contrec1010StationManagerClass.cs


	PURPOSE:			Contrec1010StationManagerClass


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
    using System.Runtime.InteropServices;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;

    using Opc;
    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Server = Opc.Da.Server;

    public class Contrec1010StationManagerClass : StationManagerClass
	{
		protected ProcessVariableClass ResetCardReaderDataPV;
		protected ProcessVariableClass CardReaderDataPV;
		protected int SelectedCompartment;
		protected int iSelectTankMenuPage = 0;
		protected bool bNextPageAvailable = false;
		protected bool bPrevPageAvailable = false;
		public Contrec1010StationManagerClass(EventLog EventLog,
			LoadRackManagerClass LoadRackManager,
			StationClass Station,
			SiteManagerClass SiteManager,
			SecurityClass Security)
			: base(EventLog, LoadRackManager, Station, SiteManager, Security)
		{
			try
			{
				if (this.StationPv.URL != ""
				&& (Station.CardReader
				|| Station.TouchKeyReader))
				{
				    this.ResetCardReaderDataPV = new ProcessVariableClass(
						PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
						UNIT_TYPE.STATION_UNIT,
						VarEnum.VT_EMPTY,
						false,
						this.StationPv.OPCItemID + ".Issue Get Touch Key Prompt",
						this.StationPv.URL,
						this.StationPv.ProgID);

				    this.CardReaderDataPV = new ProcessVariableClass(
						PROCESS_VARIABLE_TYPE.CARDREADER_PV,
						UNIT_TYPE.STATION_UNIT,
						VarEnum.VT_BSTR,
						true,
						this.StationPv.OPCItemID + ".Touch Key Value",
						this.StationPv.URL,
						this.StationPv.ProgID);

				    this.OPCServerManager.AddProcessVariable(this.CardReaderDataPV);
				}

				if (this.StationState != StationState.TRANSACTION_IN_PROGRESS)
				{
				    this.SendEndTransaction();

					if (Station.CardReader
						|| Station.TouchKeyReader) this.ResetCardReaderData();
					else
					{
						// new code
					    this.IssueDriverIDPrompt();
						// end new code
					}
				}
			}

			catch (Exception e)
			{
				EventLog.WriteEntry("Contrec 1010 StationManager : " + e.Message, EventLogEntryType.Error);
			    this.StationState = StationState.IDLE;
			}
		}

		public void IssueDriverCardPrompt()
		{
			this.InRecircMode = false;
			if (this.Station.TouchKeyReader) this.IssueTouchKeyPleaseCardIn();
			else this.IssuePleaseCardIn();
		}

		public override void IssueTouchKeyPleaseCardIn()
		{
			string Prompt = "[LoadRack|Scan Driver Key]";
			int MessageTimeout = 300;

		    this.Driver = null;
		    this.StationState = StationState.IDLE;
			this.InRecircMode = false;

		    this.SetMessageTimeout(MessageTimeout);

		    this.ResetCardReaderDataPV.ServerValue = this.FormatContrecPrompt(Prompt);

		    this.OPCServerManager.Write(this.ResetCardReaderDataPV);
		}

		public override void IssuePleaseCardIn()
		{
			string Prompt = "[LoadRack|Please Card In]";
			int MessageTimeout = 300;

		    this.Driver = null;
		    this.StationState = StationState.IDLE;
			this.InRecircMode = false;

		    this.SetMessageTimeout(MessageTimeout);

		    this.ResetCardReaderDataPV.ServerValue = this.FormatContrecPrompt(Prompt);

		    this.OPCServerManager.Write(this.ResetCardReaderDataPV);
		}

		protected override void ResetCardReaderData()
		{
			if (this.Station.TouchKeyReader) this.IssueTouchKeyPleaseCardIn();
			else this.IssuePleaseCardIn();
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();
		    this.LoadArmManagerCollection.ReleaseKeyPad(this);
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
					this.eventLog.WriteEntry("Contrec 1010 StationManager CancelUnauthorizedTransaciton : " + e.Message, EventLogEntryType.Error);
				}
			}
		}

		public override bool SendEndOfDayOrMonthWarningMessagesDuringLoading { get { return false; } }

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool SaveForCancelProcessing)
		{
			int iLoadArmNumber = 0;
			int iLoadArmInUse = 0;
			string MessageToSend;

			if (this.AvailableLoadArmManagers == 0)
				return 0;

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

			if (this.StationState == StationState.ENTER_TRACTOR_CARDIN_PROMPT)
			{
			    this.SetMessageTimeout(messageTimeout);
			    this.ResetCardReaderDataPV.ServerValue = this.FormatContrecPrompt(MessageToSend);

			    this.OPCServerManager.Write(this.ResetCardReaderDataPV);
			}
			else
			{
				// Output Station Message to only the arm that has the focus 
				foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this != LoadArmManager.GetStationManager())
						continue;

					iLoadArmNumber = LoadArmManager.GetArmNumber(this);
					iLoadArmInUse = LoadArmManager.GetLoadArmInUse();
					if (iLoadArmNumber != iLoadArmInUse)
						continue;

					try
					{
						LoadArmManager.DisplayMessage(MessageToSend, responseLength, messageTimeout);
					}
					catch (Exception e)
					{
						this.eventLog.WriteEntry("Contrec 1010 StationManager DisplayMessage : " + e.Message, EventLogEntryType.Error);
					}
				}
			}

			if (SaveForCancelProcessing) this.SaveMessageValues(stockMessage, responseLength, messageTimeout);

			return 0;
		}

		public string FormatContrecPrompt(string StockMessage)
		{
			string Prompt = "";
			int iLoop;
			int iNumberofCharacters = 0;
			bool bStart = false;
			bool bLookForSpace = false;
			bool bLastCharacterWasSpace = false;

			// we need to reformat the message. The Contrec supports eight lines at 30 characters and we use the '|' as the line seperator
			// we need to remove the [] from the message
			Prompt = "|Varec Terminal Automation||";
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

		public override string AcknowledgementMessage { get { return "[LoadRack|Press Enter to Acknowledge]"; } }

		public override int AcknowledgementResponseLength { get { return 29; } }

		public override bool NumericMenuSelection { get { return true; } }

		protected virtual int NumberOfDisplayLines { get { return 4; } }

		protected override void PromptForPin(string stockMessage, int responseLength, int messageTimeout)
		{
			if (this.AvailableLoadArmManagers == 0)
				throw new OpcException("No Load Arms Available");

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				try
				{
					LoadArmManager.PromptForPIN(stockMessage, responseLength, messageTimeout);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Contrec 1010 StationManager PromptForPIN : " + e.Message, EventLogEntryType.Error);
				}
				break;
			}

		    this.SaveMessageValues(stockMessage, responseLength, messageTimeout);
		}

		public override void DisplayMenu(DisplayMenuParameters Parameters)
		{
			int iLoadArmNumber = 0;
			int iLoadArmInUse = 0;

			if (this.AvailableLoadArmManagers == 0)
				return;

			foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				iLoadArmNumber = LoadArmManager.GetArmNumber(this);
				iLoadArmInUse = LoadArmManager.GetLoadArmInUse();
				if (iLoadArmNumber != iLoadArmInUse)
					continue;

				try
				{
					LoadArmManager.DisplayMenu(Parameters);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Contrec StationManager DisplayMenu : " + e.Message, EventLogEntryType.Error);
				}
			}

			if (Parameters.SaveForCancelProcessing) this.CurrentMenuParameters = Parameters;
		}

	    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
	    public override void ReadLineItemData(
			LineItemDO lineItem,
			Server server,
			LoadArmManagerClass loadArmManager)
		{
			Contrec1010LoadArmManagerClass contrec1010LoadArmManager = loadArmManager as Contrec1010LoadArmManagerClass;
			if (contrec1010LoadArmManager == null)
				throw new Exception("ReadLineItemData : Invalid LoadArmManager");

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			ItemValueResult grossVolume;
			ItemValueResult netVolume;
			ItemValueResult averageTemperature;
			ItemValueResult averageDensity;

			ItemValueResult[] nonResettableTotal;

			contrec1010LoadArmManager.ReadNonResettableTotals(
				server,
				out nonResettableTotal);


			SiteClass site = this.SiteManager.Site;

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
						double density;
						contrec1010LoadArmManager.GetOffLoadDensity(out density);
						pv.ServerValue = density;
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

					if (lineItem.MeterReading.MeterStop != System.Convert.ToDouble(nonResettableTotal[0].Value))
					{
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}
					lineItem.MeterReading.MeterStop_BadQualityLogged = false;

					if (component.MeterValue != lineItem.MeterReading.MeterStop)
					{
						component.MeterValue = lineItem.MeterReading.MeterStop.Value;
					    this.LastActivityDateTime = siteTimeNow;
					}
				}
			}

			// this code will be used if we ever enable multiple arms loading at the same time
			//			ItemValueResult CompartmentNumber;
			//			Contrec1010LoadArmManager.GetCompartmentNumberMultipleArms(Server, out CompartmentNumber);
			//			if (CompartmentNumber.Quality == Quality.Good)
			//			{
			//				LineItem.DestinationCompartmentID = System.Convert.ToString(CompartmentNumber.Value);
			//			}

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

						if (subLineItem.MeterReading.MeterStop != System.Convert.ToDouble(nonResettableTotal[0].Value))
						{
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[0].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (component.MeterValue != subLineItem.MeterReading.MeterStop)
						{
							component.MeterValue = subLineItem.MeterReading.MeterStop.Value;
						    this.LastActivityDateTime = siteTimeNow;
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

						if (subLineItem.MeterReading.MeterStop != System.Convert.ToDouble(nonResettableTotal[additiveInjector.PresetNumber].Value))
						{
							subLineItem.MeterReading.MeterStop = System.Convert.ToDouble(nonResettableTotal[additiveInjector.PresetNumber].Value);
							subLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						subLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (additiveInjector.MeterValue != subLineItem.MeterReading.MeterStop)
						{
							additiveInjector.MeterValue = subLineItem.MeterReading.MeterStop.Value;
						    this.LastActivityDateTime = siteTimeNow;
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
		} //end routine

		public void SetSelectedCompartMentNumber(int iSelected)
		{
		    this.SelectedCompartment = iSelected;
		}

		public int GetSelectedCompartMentNumber()
		{
			return (this.SelectedCompartment);
		}

		public override bool ProcessPromptForReturns(string Response)
		{
			int iLoadArmNumber = 0;
			int iLoadArmInUse = 0;

			if (Response == "1")
				Response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0])
																);
			if (Response == "2")
				Response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1])
																);

			if (FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Yes")) != Response &&
			FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|No")) != Response)
			{
			    this.DisplayMessage("[LoadRack|Invalid Selection]", null, 0, this.MESSAGE_TIMEOUT);
			    this.StationState = StationState.PROMPT_FOR_RETURNS;
				return true;
			}

			if (this.AvailableLoadArmManagers == 0)
				return false;

			// Output Station Message to only the arm that has the focus 
			foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				iLoadArmNumber = LoadArmManager.GetArmNumber(this);
				iLoadArmInUse = LoadArmManager.GetLoadArmInUse();
				if (iLoadArmNumber != iLoadArmInUse)
					continue;

				LoadArmManager.IssueLoadNumberTransactiont();
			}
			return true;
		}

		protected override void ProcessUnloadDensity(string Response)
		{
			if (Response == EscapeString)
			{
			    this.PromptForSupplyOrderNumber();
				return;
			}
			else if (Response == "" ||
						Response.ToUpper() == "YES" ||
						Response.ToUpper() == "NO")
			{
				PromptForOffLoadDensity();
				return;
			}

			if (this.SetDensityInUnit(Response) == false)
			{
			    this.StationState = StationState.RESET_ON_TIMEOUT;
			    this.DisplayMessage("[LoadRack|Failed To Set Density]", null, 0, this.MESSAGE_TIMEOUT);
			    this.ConsecutivePrompts = 0;
				return;
			}
		    this.StationState = StationState.ENTER_UNLOAD_AMOUNT;
		    this.DisplayMessage("[LoadRack|Enter] [LoadRack|Qty On BOL]", null, 10, this.PROMPT_TIMEOUT);
		}

		public override bool SetDensityInUnit(string Density)
		{
			if (this.AvailableLoadArmManagers == 0)
				return false;

			// Output Station Message to all arms since we do not know which one will be used 
			foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				try
				{
					LoadArmManager.SetDensityInUnit(Density);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Contrec 1010 StationManager SetDensityInUnit : " + e.Message, EventLogEntryType.Error);
				}
			}

			return true;
		}

		public override void SetUnloadPresetAmount(string Response)
		{
			if (this.AvailableLoadArmManagers == 0)
				return;

			// Output Station Message to all arms since we do not know which one will be used 
			foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				try
				{
					LoadArmManager.SetUnloadPresetAmount(Response);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Contrec 1010 StationManager SetUnloadPresetAmount : " + e.Message, EventLogEntryType.Error);
				}
			}

			return;
		}

		protected override void LoadRackProcessing(ProcessVariableClass PV)
		{
			switch (PV.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
					{
						string KeypadData = (string)PV.ServerValue;
						// the cancel will send the value NO so we need to remap
						if (KeypadData.ToString() == "NO")
						{
							KeypadData = EscapeString;
						}
						if (PV.IsQualityGood)
						{
							if ((string)PV.ServerValue != "")
							{
								// Driver Card
								if (this.StationState == StationState.IDLE)
								{
								    this.ProcessDriverID((string)KeypadData);
								}

								// LoadID Card
								else if (this.StationState == StationState.LOADID_CARD_PROMPT
								|| this.StationState == StationState.LOADID_PROMPT)
								{
								    this.ProcessLoadIDCard((string)KeypadData);
								}
								else if (this.StationState == StationState.ENTER_TRACTOR_CARDIN_PROMPT)
								{
								    this.ProcessTractorCardIn((string)KeypadData);
								}

								// If we got a card scan and we are not in a state where we expect one,
								// reset the station.
								else if (this.StationState != StationState.AUTHORIZING
								&& this.StationState != StationState.AUTHORIZED
								&& this.StationState != StationState.TRANSACTION_IN_PROGRESS) this.StationState = StationState.IDLE;

								// If any scan card processing fails, the station state will be set to IDLE
								// indicating that we need to reset the station.
								if (this.StationState == StationState.IDLE) this.ResetStationDevice();
							}
						}

						break;
					}


				default:
					base.LoadRackProcessing(PV);
					break;
			}
		}

		public void SetMessageTimeout(int MessageTimeoutValue)
		{
			ProcessVariableClass MessageTimeout;
			MessageTimeout = new ProcessVariableClass(
				PROCESS_VARIABLE_TYPE.RESET_CARDREADER_DATA_PV,
				UNIT_TYPE.STATION_UNIT,
				VarEnum.VT_EMPTY,
				false,
				this.StationPv.OPCItemID + ".Set Message Time-out",
				this.StationPv.URL,
				this.StationPv.ProgID);

			MessageTimeout.ServerValue = MessageTimeoutValue;
		    this.OPCServerManager.Write(MessageTimeout);
		}

		protected override void PromptForSupplyOrderNumber()
		{
		    this.ConsecutivePrompts = 0;
		    this.StationState = StationState.ENTER_SUPPLY_ORDER_NUMBER;
		    this.DisplayMessage("[LoadRack|Enter] [LoadRack|Release Number]", null, PromptLength, this.PROMPT_TIMEOUT);
		}

		protected override void ProcessEnterSupplyOrderNumber(string response)
		{
			if (response == EscapeString)
			{
				if (this.Station.CardReader) this.IssuePleaseCardIn();
				else if (this.Station.TouchKeyReader) this.IssueTouchKeyPleaseCardIn();
				else this.IssueDriverIDPrompt();

				return;
			}
			else
			{
				if (response == "" || response.ToUpper() == "YES")
				{
				    this.ConsecutivePrompts++;
					if (this.ConsecutivePrompts >= this.SiteManager.Site._MaximumPrompts)
					{
					    this.StationState = StationState.RESET_ON_TIMEOUT;
					    this.DisplayMessage("LoadRack|Max Retries Exceeded", null, 0, this.MESSAGE_TIMEOUT);
					    this.ConsecutivePrompts = 0;
						return;
					}

				    this.StationState = StationState.ENTER_SUPPLY_ORDER_NUMBER;
				    this.DisplayMessage("[LoadRack|Invalid], [LoadRack|Enter] [LoadRack|Order Number]", null, 10, this.PROMPT_TIMEOUT);
				}
				else
				{
				    this.PromptForBOLNumber();
				}
			}
		}

		protected override void ProcessUnloadAmount(string response)
		{
			if (response == EscapeString)
			{
				PromptForBOLNumber();
				return;
			}
			else
			{
			    this.StartDateTime = this.LastActivityDateTime = TimeConverter.Now(this.SiteManager.Site);
			    this.SetUnloadPresetAmount(response);
			    this.StationState = StationState.AUTHORIZED;
			    this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.UNLOAD_VOLUME_PROMPT);
			}
		}

		protected override void ProcessDriverID(string response)
		{
			if (response == "")
			{
				foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
				{
					if (this != LoadArmManager.GetStationManager())
						continue;

					int iLoadArmNumber = LoadArmManager.GetArmNumber(this);
					int iLoadArmInUse = LoadArmManager.GetLoadArmInUse();
					if (iLoadArmNumber != iLoadArmInUse)
						continue;
					LoadArmManager.ResponsePending = true;
				}
				return;
			}
			// everytime someone logs in we will reset the date and time in the Contrec just to make sure it is valid
		    this.CheckandResetContrecTimeDate();

			// check if this is a recirc entry
			if (response.ToUpper() == this.Station.MeterRecircCardNumber.ToUpper() && this.Station.MeterRecircCardNumber.ToString() != "")
			{
			    this.StationState = StationState.PROMPT_FOR_RECIRC_CONFIMATION;
			    this.DisplayMenu(new DisplayMenuParameters("LoadRack|Perform Meter Recirc?", new string[] { "LoadRack|Yes", "LoadRack|No" },
					true,
					-1, this.PROMPT_TIMEOUT));
			}
			else
				base.ProcessDriverID(response);
		}

		public override bool ProcessMeterRecircConfirmation(string Response)
		{
			if (Response.ToUpper() == "YES")
				Response = "1";
			else if (Response.ToUpper() == "NO")
				Response = "2";

			if (Response == "1")
				Response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[0])
																);
			if (Response == "2")
				Response = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.SiteManager.Site.SiteGuid, this.CurrentMenuParameters.Menu[1])
																);

			if (FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(this.SiteManager.Site.SiteGuid, "LoadRack|Yes") ) != Response)
			{
				if (this.Station.CardReader) this.IssueDriverCardPrompt();
				else if (this.Station.TouchKeyReader) this.IssueTouchKeyPleaseCardIn();
				else this.IssueDriverIDPrompt();
				return true;
			}
			this.InRecircMode = true;

			// check if this is a single manager and single owner site and set these values
			if (!this.CheckAndSetSingleOwnerManager())
			{
			    this.StationState = StationState.RESET_ON_TIMEOUT;
			    this.DisplayMessage("[LoadRack|Single Owner/Manager Only]", null, 0, this.MESSAGE_TIMEOUT);
			    this.ConsecutivePrompts = 0;
				return false;
			}
			// set the transaction time value
		    this.TimeIn = TimeConverter.Now(this.SiteManager.Site);

		    this.StationState = StationState.PROMPT_FOR_SOURCE_TANK;
		    this.iSelectTankMenuPage = 1;
		    this.MenuPromptForTank();

			return true;
		}
		public void MenuPromptForTank()
		{
			int iCurrentTank = 0;
			int iMenuItemsAdded = 0;
			int iNumberOfTanksPerMenuPage = 3;
			ArrayList Menu = new ArrayList();

			TankCollectionClass tankCollection;
			tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			foreach (TankClass tank in tankCollection)
			{
				++iCurrentTank;
				if (iCurrentTank <= (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
					continue;
				Menu.Add("|" + tank.ID);
				++iMenuItemsAdded;
				if (iCurrentTank >= iNumberOfTanksPerMenuPage + (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
					break;
			}

		    this.bNextPageAvailable = false;
		    this.bPrevPageAvailable = false;
			DisplayMenuParameters Parameters = new DisplayMenuParameters();

			Parameters.ApplyDataDictionary = true;
			Parameters.DefaultItem = 0;
			Parameters.MenuTimeout = 30;

			if (this.StationState == StationState.PROMPT_FOR_SOURCE_TANK)
				Parameters.Caption = "[LoadRack|Select Source Tank";
			else
				Parameters.Caption = "[LoadRack|Select Dest Tank";

			if (iMenuItemsAdded == iNumberOfTanksPerMenuPage)
			{
				Menu.Add("|Next Page");
			    this.bNextPageAvailable = true;
				if (this.iSelectTankMenuPage > 1)
				{
					Menu.Add("|Prev Page");
				    this.bPrevPageAvailable = true;
				}
			}
			else
			{
				// always add next at position 4 and prev at position 5 so we can track it
				for (int iLoop = 1; iLoop < 5; iLoop++)
				{
					if (iLoop > iMenuItemsAdded)
						Menu.Add("|");
				}
				if (this.iSelectTankMenuPage > 1)
				{
					Menu.Add("|Prev Page");
				    this.bPrevPageAvailable = true;
				}
			}


			Parameters.Menu = (string[])Menu.ToArray(typeof(string));

		    this.DisplayMenu(Parameters);
		}

		public override void ProcessPromptForSourceTank(string response)
		{
			if (response.ToUpper() == EscapeString.ToUpper())
			{
				if (this.Station.CardReader) this.IssueDriverCardPrompt();
				else if (this.Station.TouchKeyReader) this.IssueTouchKeyPleaseCardIn();
				else this.IssueDriverIDPrompt();
				return;
			}
			else if (response == "4" ||
				response == "5")
			{
			    this.ProcessPageChange(response);
			}
			else if (response == "1" ||
				response == "2" ||
				response == "3")
			{
				// determine the tank that was selected and save the id and index
				int iCurrentTank = 0;
				int iNumberOfTanksPerMenuPage = 3;
				int iSelectedTank = 0;
				TankCollectionClass tankCollection;

				tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);
			    this.FromStorageLocationID = null;

				foreach (TankClass tank in tankCollection)
				{
					++iCurrentTank;
					if (iCurrentTank <= (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
						continue;
					++iSelectedTank;
					if (iSelectedTank == System.Convert.ToInt32(response))
					{
					    this.FromStorageLocationID = tank.ID;
					    this.FromStorageLocationTankGuid = tank.IdentityGuid;
					}
					if (iCurrentTank >= iNumberOfTanksPerMenuPage + (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
						break;
				}
				// verify that the tank selected is valid
				if (this.FromStorageLocationID == null) this.MenuPromptForTank();
				else
				{
					// set up prompt for destination tank
				    this.StationState = StationState.PROMPT_FOR_DESTINATION_TANK;
				    this.iSelectTankMenuPage = 1;
				    this.MenuPromptForTank();
				}
			}
			else
			{
				// user entered something completely off the wall so just redraw the menu
			    this.MenuPromptForTank();
			}
		}

		public override void ProcessPromptForDestinationTank(string Response)
		{
			if (Response.ToUpper() == EscapeString.ToUpper())
			{
			    this.StationState = StationState.PROMPT_FOR_SOURCE_TANK;
			    this.iSelectTankMenuPage = 1;
			    this.MenuPromptForTank();
				return;
			}
			else if (Response == "4" ||
				Response == "5")
			{
			    this.ProcessPageChange(Response);
			}
			else if (Response == "1" ||
				Response == "2" ||
				Response == "3")
			{
				// determine the tank that was selected and save the id and index
				int iCurrentTank = 0;
				int iNumberOfTanksPerMenuPage = 3;
				int iSelectedTank = 0;
				TankCollectionClass tankCollection;
				tankCollection = FMChannelHelper.MakeCall<ITanks, TankCollectionClass>(
																	 x =>
																	 x.Enumerate(this.Security)
																);

			    this.ToStorageLocationID = null;

				foreach (TankClass tank in tankCollection)
				{
					++iCurrentTank;
					if (iCurrentTank <= (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
						continue;
					++iSelectedTank;
					if (iSelectedTank == System.Convert.ToInt32(Response))
					{
					    this.ToStorageLocationID = tank.ID;
					    this.ToStorageLocationTankGuid = tank.IdentityGuid;
					}
					if (iCurrentTank >= iNumberOfTanksPerMenuPage + (iNumberOfTanksPerMenuPage * (this.iSelectTankMenuPage - 1)))
						break;
				}
				// verify that the tank selected is valid
				if (this.ToStorageLocationID == null) this.MenuPromptForTank();
				else
				{
				    this.StationState = StationState.ENTER_METER_RECIRC_AMOUNT;
				    this.DisplayMessage("[LoadRack|Enter] [LoadRack|Recirc Amount]", null, 10, this.PROMPT_TIMEOUT);
				}
			}
			else
			{
				// user entered something completely off the wall so just redraw the menu
			    this.MenuPromptForTank();
			}
		}

		public void ProcessPageChange(string Response)
		{
			if (Response == "4" && this.bNextPageAvailable == true)
			{
				++this.iSelectTankMenuPage;
			}
			else if (Response == "5" && this.iSelectTankMenuPage > 1 && this.bPrevPageAvailable == true)
			{
				--this.iSelectTankMenuPage;
			}
		    this.MenuPromptForTank();
		}

		public override void ProcessMeterRecircAmount(string Response)
		{
			if (Response == EscapeString)
			{
			    this.StationState = StationState.PROMPT_FOR_DESTINATION_TANK;
			    this.iSelectTankMenuPage = 1;
			    this.MenuPromptForTank();
			}
			else
			{
			    this.StartDateTime = this.LastActivityDateTime = TimeConverter.Now(this.SiteManager.Site);
			    this.SetMeterRecircPresetAmount(Response);
			    this.StationState = StationState.AUTHORIZED;
			    this.LoadArmManagerCollection.SetState(this, LOADARM_STATE.METERRECIRC_VOLUME_PROMPT);
			}
		}

		public override void SetMeterRecircPresetAmount(string Response)
		{
			if (this.AvailableLoadArmManagers == 0)
				return;

			// Output Station Message to all arms since we do not know which one will be used 
			foreach (Contrec1010LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				try
				{
					LoadArmManager.SetMeterRecircPresetAmount(Response);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("Contrec 1010 StationManager SetMeterRecircAmount : " + e.Message, EventLogEntryType.Error);
				}
			}

			return;
		}

		public void CheckandResetContrecTimeDate()
		{
			DateTimeOffset ContrecDateTime;
			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);
			TimeSpan ContrecTimeDifference;
			ProcessVariableClass ContrecWriteTimePV;
			string ContrecDateTimeString = string.Empty;

			Item[] Items = { new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".1010 Date Time")), };

			ItemValueResult[] Values = this.OPCServerManager.Read(new URL(this.StationPv.URL), Items);
			if (Values[0].Quality == Quality.Good)
			{
				try
				{
					ContrecDateTime = DateTimeOffset.Parse(Values[0].Value.ToString());
				}
				catch
				{
					ContrecDateTime = TimeConverter.MinFMDate;
				}
			}
			else
			{
				ContrecDateTime = TimeConverter.MinFMDate;
			}

			ContrecTimeDifference = siteTimeNow - ContrecDateTime;

			if (ContrecTimeDifference.Ticks > 30000)	// only reset the time if it is greater then five minutes off
			{
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

				ContrecDateTimeString = siteTimeNow.Month.ToString("D2") + siteTimeNow.Day.ToString("D2") + siteTimeNow.Year.ToString("D4");

				ContrecDateTimeString += siteTimeNow.Hour.ToString("D2") + siteTimeNow.Minute.ToString("D2") + siteTimeNow.Second.ToString("D2");

				ContrecWriteTimePV.ServerValue = ContrecDateTimeString;
				try
				{
				    this.OPCServerManager.Write(ContrecWriteTimePV);
				}
				catch
				{
				}
			}
		}

	}
}
