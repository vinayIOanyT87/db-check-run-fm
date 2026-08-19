/******************************************************************************

	FILE NAME:		Danload6000StationManager.cs


	PURPOSE:			Danload6000StationManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		11-Mar-08	B. Schaal	7.4.0.0 - CSI 5556 - Added protected override void ProcessShipTo( string Response )

		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

		06/26/2008	W.Gray		7.4.5.1 - Revised to not issue Display requests
										to disabled LoadArms (CSI 5990)

		08/01/2008	W.Gray		7.4.5.2 - Revised to apply scale to Meter Closeout (CSI 6049)

		08/19/2008	W.Gray		7.4.5.1 - Changed CreateMeterReadingTransactions to create
										transactions regardless of LoadArm.Enabled (CSI 6099)

		9/09/2008	W.Gray		7.4.6.0 - Revised to support external components (CSI 5581)

		11/07/2008	W.Gray		7.4.6.0 - Revised to set DataType in ProcessVariable (CSI 6278)

		12/15/2008  W.Gray		7.4.6.1 - Revised to store values in maximum precision (CSI 6239)

		12/24/2008	W.Gray		7.4.6.2 - Added Support for Internal Additive Meter Totalizers (CSI 6341)
										
		01/28/2009	W.Gray		7.4.6.3 - Revised to revert back to TAS precision
*******************************************************************************/

namespace LoadRackLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.ServiceRequests;
    using FMBusinessObjects.UtilityObjects;

    using Opc;
    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Factory = OpcCom.Factory;
    using Server = Opc.Da.Server;

    /// <summary>
	/// Summary description for Danload6000StationManagerClass.
	/// </summary>
	public class Danload6000StationManagerClass : StationManagerClass
	{
		protected ArrayList PowerfailAlarms = new ArrayList();

		public Danload6000StationManagerClass(EventLog EventLog,
			LoadRackManagerClass LoadRackManager,
			StationClass Station,
			SiteManagerClass SiteManager,
			SecurityClass Security)
			: base(EventLog, LoadRackManager, Station, SiteManager, Security)
		{
			try
			{
			}

			catch (Exception e)
			{
				EventLog.WriteEntry("DANLOAD 6000 StationManager : " + e.Message, EventLogEntryType.Error);
			    this.StationState = StationState.IDLE;
			}
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();

		    this.LoadArmManagerCollection.ReleaseKeyPad(this);

		    this.IssueDriverIDPrompt();
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
					this.eventLog.WriteEntry("DANLOAD 6000 StationManager CancelUnauthorizedTransaciton : " + e.Message, EventLogEntryType.Error);
				}
			}
		}



		public override bool RegisterPowerfail(string name)
		{
			foreach (string AlarmName in this.PowerfailAlarms)
			{
				if (AlarmName == name)
				{
					return false;
				}
			}

			this.PowerfailAlarms.Add(name);

			return true;

		}

		public override bool SendEndOfDayOrMonthWarningMessagesDuringLoading { get { return false; } }

		public override int DisplayMessage(string stockMessage, string defaultResponse, int responseLength, int messageTimeout, bool SaveForCancelProcessing)
		{
			if (this.AvailableLoadArmManagers == 0)
				return 0;

			// Output Station Message to all Arms 
			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				try
				{
					LoadArmManager.DisplayMessage(stockMessage, responseLength, messageTimeout);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("DANLOAD 6000 StationManager DisplayMessage : " + e.Message, EventLogEntryType.Error);
				}
			}

			if (SaveForCancelProcessing) this.SaveMessageValues(stockMessage, responseLength, messageTimeout);

			return 0;
		}

		public override string AcknowledgementMessage { get { return "[LoadRack|Press Enter to Acknowledge]"; } }

		public override int AcknowledgementResponseLength { get { return 41; } }

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
					this.eventLog.WriteEntry("DANLOAD 6000 StationManager PromptForPIN : " + e.Message, EventLogEntryType.Error);
				}
			}

		    this.SaveMessageValues(stockMessage, responseLength, messageTimeout);
		}


		protected override void ResetCardReaderData()
		{
		}

		public override void ReleaseKeyPad()
		{
		    this.LoadArmManagerCollection.ReleaseKeyPad(this);
		}

		protected override void Unauthorize()
		{
			base.Unauthorize();
		}


		public void ProcessPowerfail(ProcessVariableClass PV)
		{
			if (PV.IsQualityGood
				&& (bool)PV.ServerValue)
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
					this.IssueDriverIDPrompt();

				else
					this.LoadArmManagerCollection.ProcessMessageTimeout(this);

			}
		}

		protected override void LoadRackProcessing(ProcessVariableClass PV)
		{
			switch (PV.ProcessVariableType)
			{
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
			return false;
		}

		protected override void ProcessPreloadLoadIDSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch
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

			if (nSelection == 0)
				response = EscapeString;
			else
				response = this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadLoadIDSelection(response);
		}


		public override void ProcessPreloadOrderSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch
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

			if (nSelection == 0)
				response = EscapeString;
			else
				response = this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadOrderSelection(response);

		}

		protected override void ProcessPreloadDocumentSelection(string response)
		{
			int nSelection = -1;

			try
			{
				nSelection = System.Convert.ToInt32(response);
			}
			catch
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

			if (nSelection == 0)
				response = EscapeString;
			else
				response = this.CurrentMenuParameters.Menu[nSelection - 1];

			base.ProcessPreloadDocumentSelection(response);
		}

		public override void DisplayMenu(DisplayMenuParameters Parameters)
		{
			if (this.AvailableLoadArmManagers == 0)
				return;

			foreach (LoadArmManagerClass LoadArmManager in this.LoadArmManagerCollection)
			{
				if (this != LoadArmManager.GetStationManager())
					continue;

				try
				{
					LoadArmManager.DisplayMenu(Parameters);
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("DANLOAD 6000 StationManager DisplayMenu : " + e.Message, EventLogEntryType.Error);
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

			Danload6000LoadArmManagerClass Danload6000LoadArmManager = loadArmManager as Danload6000LoadArmManagerClass;
			if (Danload6000LoadArmManager == null)
				throw new Exception("ReadLineItemData : Invalid LoadArmManager");

			DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

			ItemValueResult GrossTotalizer;
			ItemValueResult NetTotalizer;
			ItemValueResult GrossVolume;
			ItemValueResult NetVolume;
			ItemValueResult AverageTemperature;
			ItemValueResult AverageDensity;

			ItemValueResult[] AdditiveNonResettableTotal;

			Danload6000LoadArmManager.ReadAdditiveNonResettableTotals(
				server,
				out AdditiveNonResettableTotal);

			SiteClass Site = this.SiteManager.Site;

			ProcessVariableClass PV = new ProcessVariableClass();

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				ProductMapClass Component = loadArmManager.GetComponent(lineItem.ProductGuid);
				if (Component == null)
					throw new Exception("Component not found in LoadArm Configuration");

				if (lineItem.Quantity == null)
					lineItem.Quantity = new QuantityDO();

				Danload6000LoadArmManager.ReadComponentData(
					server,
					Component.PresetNumber,
					out GrossTotalizer,
					out NetTotalizer,
					out GrossVolume,
					out NetVolume,
					out AverageTemperature,
					out AverageDensity);


				if (GrossVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadGrossQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + GrossVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadGrossQualityLogged = true;
					}
				}
				else
				{
					lineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(GrossVolume.Value), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					lineItem.Quantity.BadGrossQualityLogged = false;
				}

				if (NetVolume.Quality != Quality.Good)
				{
					if (!lineItem.Quantity.BadNetQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Net Volume OPC Quality Bad " + NetVolume.ItemName, EventLogEntryType.Error);
						lineItem.Quantity.BadNetQualityLogged = true;
					}
				}
				else
				{
					lineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(NetVolume.Value), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
					lineItem.Quantity.BadNetQualityLogged = false;
				}

				if (lineItem.Temperature == null)
					lineItem.Temperature = 0.0;

				if (AverageTemperature.Quality != Quality.Good)
				{
					if (!lineItem.Temperature_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Average Temperature OPC Quality Bad " + AverageTemperature.ItemName, EventLogEntryType.Error);
						lineItem.Temperature_BadQualityLogged = true;
					}
				}
				else
				{
					lineItem.Temperature = Math.Round(System.Convert.ToDouble(AverageTemperature.Value) / 10, lineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
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

				if (AverageDensity.Quality != Quality.Good)
				{
					if (!lineItem.Density_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadLineItemData : Component Average Density OPC Quality Bad " + AverageDensity.ItemName, EventLogEntryType.Error);
						lineItem.Density_BadQualityLogged = true;
					}
				}
				else
				{
					// Presently the system expects that the Preset Units will match the Site/Transaction Units
					// however there is a scale parameter 046 in the Daniels that is set based upon the Units
					double scale = 10;
					if (PV.ServerUnits == EngineeringUnit.FmdDegApi)
						scale = 100;
					else if (PV.ServerUnits == EngineeringUnit.FmdGcm3)
						scale = 10000;

				    SIDouble siDensity = new SIDouble
				                         {
				                             Units = lineItem.DensityUnits,
				                             Value = System.Convert.ToDouble(AverageDensity.Value) / scale
				                         };


				    // Reference Density isn't readily available from Daniels so compute it.
					if (lineItem.VCF != null
					&& lineItem.VCF.Value != 0)
					{
						siDensity.SIValue /= lineItem.VCF.Value;
						lineItem.Density = Math.Round(siDensity.Value, lineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
						lineItem.Density_BadQualityLogged = false;
					}
				}


				if (GrossTotalizer.Quality != Quality.Good)
				{
					if (lineItem.MeterReading.MeterStop == null)
						lineItem.MeterReading.MeterStop = 0.0;

					if (!lineItem.MeterReading.MeterStop_BadQualityLogged)
					{
						this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + GrossTotalizer.ItemName, EventLogEntryType.Error);
						lineItem.MeterReading.MeterStop_BadQualityLogged = true;
					}
				}
				else
				{
					if (lineItem.MeterReading.MeterStart == null)
					{
						lineItem.MeterReading.MeterStart = Component.MeterValue;
						lineItem.MeterReading.StartDateTime = siteTimeNow;
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(GrossTotalizer.Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}

					if (lineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(GrossTotalizer.Value))
					{
						lineItem.MeterReading.MeterStop = System.Convert.ToDouble(GrossTotalizer.Value);
						lineItem.MeterReading.StopDateTime = siteTimeNow;
					}
					lineItem.MeterReading.MeterStop_BadQualityLogged = false;

					if (Component.MeterValue != lineItem.MeterReading.MeterStop.Value)
					{
						Component.MeterValue = lineItem.MeterReading.MeterStop.Value;
					    this.LastActivityDateTime = DateTimeOffset.Now;
					}
				}
			}

			foreach (SubLineItemDO SubLineItem in lineItem.SubLineItems)
			{
				if (SubLineItem.Status != TransactionStatus.InProgress)
					continue;

				if (SubLineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					ProductMapClass Component = loadArmManager.GetComponent(SubLineItem.ProductGuid);
					if (Component == null)
						throw new Exception("Component not found in LoadArm Configuration");

					if (Component.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP)
						continue;

					if (SubLineItem.Quantity == null)
						SubLineItem.Quantity = new QuantityDO();

					Danload6000LoadArmManager.ReadComponentData(
						server,
						Component.PresetNumber,
						out GrossTotalizer,
						out NetTotalizer,
						out GrossVolume,
						out NetVolume,
						out AverageTemperature,
						out AverageDensity);

					if (GrossVolume.Quality != Quality.Good)
					{
						if (!SubLineItem.Quantity.BadGrossQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Gross Volume OPC Quality Bad " + GrossVolume.ItemName, EventLogEntryType.Error);
							SubLineItem.Quantity.BadGrossQualityLogged = true;
						}
					}
					else
					{
						SubLineItem.Quantity.GrossInventoryChange = -Math.Round(System.Convert.ToDouble(GrossVolume.Value), SubLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						SubLineItem.Quantity.BadGrossQualityLogged = false;
					}

					if (NetVolume.Quality != Quality.Good)
					{
						if (!SubLineItem.Quantity.BadNetQualityLogged)
						{
							this.eventLog.WriteEntry("ReadBatchData : Component Net Volume OPC Quality Bad " + NetVolume.ItemName, EventLogEntryType.Error);
							SubLineItem.Quantity.BadNetQualityLogged = true;
						}
					}
					else
					{
						SubLineItem.Quantity.NetInventoryChange = -Math.Round(System.Convert.ToDouble(NetVolume.Value), lineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
						SubLineItem.Quantity.BadNetQualityLogged = false;
					}

					if (SubLineItem.Temperature == null)
						SubLineItem.Temperature = 0.0;

					if (AverageTemperature.Quality != Quality.Good)
					{
						if (!SubLineItem.Temperature_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Temperature OPC Quality Bad " + AverageTemperature.ItemName, EventLogEntryType.Error);
							SubLineItem.Temperature_BadQualityLogged = true;
						}
					}
					else
					{
						SubLineItem.Temperature = Math.Round(System.Convert.ToDouble(AverageTemperature.Value) / 10, SubLineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
						SubLineItem.Temperature_BadQualityLogged = false;
					}

					if (!SubLineItem.Quantity.BadGrossQualityLogged
					&& SubLineItem.Quantity.GrossInventoryChange != 0
					&& !SubLineItem.Quantity.BadNetQualityLogged
					&& SubLineItem.Quantity.NetInventoryChange != 0)
						SubLineItem.VCF = Math.Round(SubLineItem.Quantity.NetInventoryChange / SubLineItem.Quantity.GrossInventoryChange, 5, MidpointRounding.AwayFromZero);
					else
						SubLineItem.VCF = null;

					if (SubLineItem.Density == null)
						SubLineItem.Density = 0.0;

					if (AverageDensity.Quality != Quality.Good)
					{
						if (!SubLineItem.Density_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Component Average Density OPC Quality Bad " + AverageDensity.ItemName, EventLogEntryType.Error);
							SubLineItem.Density_BadQualityLogged = true;
						}
					}
					else
					{
						EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0) ? this.CurrentTransactionAlias.DensityUnits : Site.DensityUnits;

					    // Presently the system expects that the Preset Units will match the Site/Transaction Units
						// however there is a scale parameter 046 in the Daniels that is set based upon the Units
						double scale = 10;
						if (PV.ServerUnits == EngineeringUnit.FmdDegApi)
							scale = 100;
						else if (PV.ServerUnits == EngineeringUnit.FmdGcm3)
							scale = 10000;

					    SIDouble siDensity = new SIDouble
					                         {
					                             Units = units,
					                             Value = System.Convert.ToDouble(AverageDensity.Value) / scale
					                         };


					    // Reference Density isn't readily available from Daniels so compute it.
						if (SubLineItem.VCF != null
						&& SubLineItem.VCF.Value != 0)
						{
							siDensity.SIValue /= SubLineItem.VCF.Value;
							SubLineItem.Density = siDensity.Value;
							SubLineItem.Density_BadQualityLogged = false;
						}
					}


					if (GrossTotalizer.Quality != Quality.Good)
					{
						if (SubLineItem.MeterReading.MeterStop == null)
							SubLineItem.MeterReading.MeterStop = 0.0;

						if (!SubLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadComponentNonResettableTotal : Product Non-Resettable Gross Volume OPC Quality Bad " + GrossTotalizer.ItemName, EventLogEntryType.Error);
							SubLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						if (SubLineItem.MeterReading.MeterStart == null)
						{
							SubLineItem.MeterReading.MeterStart = Component.MeterValue;
							SubLineItem.MeterReading.StartDateTime = siteTimeNow;
							SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(GrossTotalizer.Value);
							SubLineItem.MeterReading.StopDateTime = siteTimeNow;
						}

						if (SubLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(GrossTotalizer.Value))
						{
							SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(GrossTotalizer.Value);
							SubLineItem.MeterReading.StopDateTime = siteTimeNow;
						}
						SubLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (Component.MeterValue != SubLineItem.MeterReading.MeterStop.Value)
						{
							Component.MeterValue = SubLineItem.MeterReading.MeterStop.Value;
						    this.LastActivityDateTime = DateTimeOffset.Now;
						}
					}
				}

				else if (SubLineItem.ProductType == ProductClass.ProductTypeID(ProductType.AdditiveProduct))
				{
					ProductMapClass additiveInjector = loadArmManager.GetAdditive(SubLineItem.ProductGuid);
					if (additiveInjector == null)
						throw new Exception("Additive not found in LoadArm Configuration");

					if (SubLineItem.Quantity == null)
						SubLineItem.Quantity = new QuantityDO();

					if (AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Quality != Quality.Good)
					{
						if (SubLineItem.MeterReading.MeterStop == null)
							SubLineItem.MeterReading.MeterStop = 0.0;

						if (!SubLineItem.MeterReading.MeterStop_BadQualityLogged)
						{
							this.eventLog.WriteEntry("ReadLineItemData : Product Non-Resettable Gross Volume OPC Quality Bad " + AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].ItemName, EventLogEntryType.Error);
							SubLineItem.MeterReading.MeterStop_BadQualityLogged = true;
						}
					}
					else
					{
						double scaleFactor = 1.0;
						if (this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvCm3
						|| this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvInch3)
							scaleFactor = 100.0;

						else if (this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvUsGal
						|| this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvLitre)
							scaleFactor = 10000.0;

						ProcessVariableClass internalPv = additiveInjector.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
						if (internalPv == null)
						{

							if (SubLineItem.MeterReading.MeterStart == null)
							{
								SubLineItem.MeterReading.MeterStart = additiveInjector.MeterValue / scaleFactor;
								SubLineItem.MeterReading.StartDateTime = siteTimeNow;
								SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value) / scaleFactor;
								SubLineItem.MeterReading.StopDateTime = siteTimeNow;
							}

							if (SubLineItem.MeterReading.MeterStop.Value != System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value) / scaleFactor)
							{
								SubLineItem.MeterReading.MeterStop = System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value) / scaleFactor;
								SubLineItem.MeterReading.StopDateTime = siteTimeNow;
							}
						}

						else
						{
							double serverValue = System.Convert.ToDouble(internalPv.ServerValue);
							double rollOver = System.Convert.ToDouble(internalPv.GetMaximum(internalPv.ServerUnits, 10));
							double currentMeterValue = System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value);

							if (SubLineItem.MeterReading.MeterStart == null)
							{
								SubLineItem.MeterReading.MeterStart = serverValue;
								SubLineItem.MeterReading.StartDateTime = siteTimeNow;
								SubLineItem.MeterReading.MeterStop = serverValue;
								SubLineItem.MeterReading.StopDateTime = siteTimeNow;
							}

							serverValue += (currentMeterValue - additiveInjector.MeterValue) / scaleFactor;
							if (currentMeterValue < additiveInjector.MeterValue)
								serverValue += 999999999.0 / scaleFactor;

							if (serverValue > rollOver)
								serverValue -= rollOver;

							if (System.Convert.ToDouble(internalPv.ServerValue) != serverValue)
							{
								internalPv.ServerValue = serverValue;
								internalPv.DateTimeStamp = DateTimeOffset.Now;
								FMChannelHelper.MakeCall<IProcessVariables>(
																	 x =>
																	 x.Modify(this.Security, DATA_TYPE.DYNAMIC, internalPv)
																);
							}

							if (SubLineItem.MeterReading.MeterStop.Value != serverValue)
							{
								SubLineItem.MeterReading.MeterStop = serverValue;
								SubLineItem.MeterReading.StopDateTime = siteTimeNow;
							}
						}

						SubLineItem.MeterReading.MeterStop_BadQualityLogged = false;

						if (additiveInjector.MeterValue != System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value))
						{
							additiveInjector.MeterValue = System.Convert.ToDouble(AdditiveNonResettableTotal[additiveInjector.PresetNumber - 1].Value);
						    this.LastActivityDateTime = DateTimeOffset.Now;
						}


						SubLineItem.Quantity.GrossInventoryChange = SubLineItem.MeterReading.MeterStop.Value - SubLineItem.MeterReading.MeterStart.Value;
						if (SubLineItem.Quantity.GrossInventoryChange < 0)
							SubLineItem.Quantity.GrossInventoryChange += 999999999.0 / scaleFactor;

						SubLineItem.Quantity.GrossInventoryChange = Math.Round(SubLineItem.Quantity.GrossInventoryChange, SubLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);

						// Acquire Density, Temperature, & VCF from the 
						// tank and compute?
						TankClass Tank = this.SiteManager.GetTank(additiveInjector, this.Manager);
						if (Tank == null)
							this.eventLog.WriteEntry("ReadLineItemData : No Additive Tank", EventLogEntryType.Error);
						else
						{
							if (SubLineItem.Temperature == null)
							{
								SubLineItem.Temperature = 0.0;
							}

							PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.TEMPERATURE_PV];
							if (PV == null)
							{
								if (!SubLineItem.Temperature_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank Temperature Process Variable", EventLogEntryType.Error);
									SubLineItem.Temperature_BadQualityLogged = true;
								}
							}

							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !PV.IsQualityGood)
							|| !typeof(double).IsInstanceOfType(PV.SIValue))
							{
								if (!SubLineItem.Temperature_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Temperature OPC Quality Bad " + PV.OPCItemID, EventLogEntryType.Error);
									SubLineItem.Temperature_BadQualityLogged = true;
								}
							}
							else
							{
								SIDouble Temperature = new SIDouble();
								Temperature.Units = SubLineItem.TemperatureUnits;
								Temperature.SIValue = System.Convert.ToDouble(PV.SIValue);

								SubLineItem.Temperature = Math.Round(Temperature.Value, SubLineItem.TemperatureDecimalPlaces, MidpointRounding.AwayFromZero);
								SubLineItem.Temperature_BadQualityLogged = false;
							}

							if (SubLineItem.Density == null)
								SubLineItem.Density = 0.0;

							PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
							if (PV == null)
							{
								if (!SubLineItem.Density_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank Density Process Variable", EventLogEntryType.Error);
									SubLineItem.Density_BadQualityLogged = true;
								}
							}

							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !PV.IsQualityGood)
							|| !typeof(double).IsInstanceOfType(PV.SIValue))
							{
								if (!SubLineItem.Density_BadQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive Density OPC Quality Bad " + PV.OPCItemID, EventLogEntryType.Error);
									SubLineItem.Density_BadQualityLogged = true;
								}
							}
							else
							{
								SIDouble Density = new SIDouble();
								Density.Units = SubLineItem.DensityUnits;
								Density.SIValue = System.Convert.ToDouble(PV.SIValue);

								SubLineItem.Density = Math.Round(Density.Value, SubLineItem.DensityDecimalPlaces, MidpointRounding.AwayFromZero);
								SubLineItem.Density_BadQualityLogged = false;
							}

							if (SubLineItem.Quantity == null)
							{
								SubLineItem.Quantity = new QuantityDO();
							}

							PV = Tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.VCF_PV];
							if (PV == null)
							{
								if (!SubLineItem.Quantity.BadNetQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : No Tank VCF Process Variable", EventLogEntryType.Error);
									SubLineItem.Quantity.BadNetQualityLogged = true;
								}
							}
							else if ((!this.SiteManager.Site.UseLastKnownGoodTankData
							&& !PV.IsQualityGood)
							|| !typeof(double).IsInstanceOfType(PV.SIValue))
							{
								if (!SubLineItem.Quantity.BadNetQualityLogged)
								{
									this.eventLog.WriteEntry("ReadLineItemData : Additive VCF OPC Quality Bad " + PV.OPCItemID, EventLogEntryType.Error);
									SubLineItem.Quantity.BadNetQualityLogged = true;
								}
							}
							else
							{
								if (SubLineItem.VCF == null)
									SubLineItem.VCF = 0.0;

								SubLineItem.VCF = System.Convert.ToDouble(PV.SIValue);
								SubLineItem.Quantity.NetInventoryChange = Math.Round(SubLineItem.Quantity.GrossInventoryChange * SubLineItem.VCF.Value,
																									 SubLineItem.VolumeDecimalPlaces, MidpointRounding.AwayFromZero);
								SubLineItem.Quantity.BadNetQualityLogged = false;
							}
						}
					}
				}

				else
					continue;
			}

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.BlendProduct)) this.RollUpSplashBlendTotals(lineItem);
		}

		public override void CreateMeterReadingTransactions(
			SaveTransactionsSR saveTransactionsSR,
			TransactionAliasClass meterReadingTransactionAlias,
			DateTimeOffset inventoryDateTime)
		{
			foreach (Danload6000LoadArmManagerClass LoadArmManager in this.FullLoadArmCollection)
			{
				// Skip Load Arms that are Swing Arms on the second bay to eliminate duplicates
				if (LoadArmManager.LoadArm.SwingArm
				&& LoadArmManager.BayB.StationManager == this
				&& LoadArmManager.BayA.StationManager != null)
					continue;

				ProcessVariableClass LoadArmPV = LoadArmManager.LoadArm.ProcessVariableCollection[0];
				Server Server = new Server(new Factory(), new URL(LoadArmPV.URL));
				NetworkCredential Credentials = null;
				Server.Connect(new ConnectData(Credentials));

				DateTimeOffset siteTimeNow = TimeConverter.Now(this.SiteManager.Site);

				ItemValueResult[] ComponentNonResettableTotal;

				LoadArmManager.ReadComponentNonResettableTotals(
					Server,
					out ComponentNonResettableTotal);

				int PresetNumber = 0;
				foreach (ProductMapClass Component in LoadArmManager.LoadArm.ComponentCollection)
				{
					TransactionDO MeterReadingTransaction = this.CreateMeterReadingTransaction(
						siteTimeNow,
						LoadArmManager,
						ComponentNonResettableTotal[PresetNumber++],
						Component,
						meterReadingTransactionAlias,
						inventoryDateTime);

					if (MeterReadingTransaction != null)
						saveTransactionsSR.Transactions.Add(MeterReadingTransaction);
				}

				ItemValueResult[] additiveNonResettableTotal;

				LoadArmManager.ReadAdditiveNonResettableTotals(
					Server,
					out additiveNonResettableTotal);


				double scaleFactor = 1.0;
				if (this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvCm3
				|| this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvInch3)
					scaleFactor = 100.0;
				else if (this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvUsGal
				|| this.SiteManager.Site.AdditiveVolumeUnits == EngineeringUnit.FmvLitre)
					scaleFactor = 10000.0;


				PresetNumber = 0;
				foreach (ProductMapClass additive in LoadArmManager.LoadArm.AdditiveInjectorCollection)
				{
					ProcessVariableClass internalPv = additive.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.ADDITIVE_METER_FLOW_TOTAL_PV];
					if (internalPv != null)
					{
						additiveNonResettableTotal[PresetNumber].ItemName = this.Station.ID + " Arm " + LoadArmManager.ArmNumber(this) + " " + additive.ID;
						additiveNonResettableTotal[PresetNumber].ResultID = ResultID.S_OK;
						additiveNonResettableTotal[PresetNumber].Quality = Quality.Good;
						additiveNonResettableTotal[PresetNumber].Value = System.Convert.ToDouble(internalPv.ServerValue);
						additiveNonResettableTotal[PresetNumber].Timestamp = internalPv.DateTimeStamp.UtcDateTime;
					}

					TransactionDO meterReadingTransaction = this.CreateMeterReadingTransaction(
						siteTimeNow,
						LoadArmManager,
						additiveNonResettableTotal[PresetNumber],
						additive,
						meterReadingTransactionAlias,
						inventoryDateTime);

					PresetNumber++;

					if (meterReadingTransaction != null)
					{
						if (internalPv == null)
						{
							meterReadingTransaction.LineItems[0].MeterReading.MeterStart /= scaleFactor;
							meterReadingTransaction.LineItems[0].MeterReading.MeterStop /= scaleFactor;
						}

						saveTransactionsSR.Transactions.Add(meterReadingTransaction);
					}
				}
			}
		}
	}
}
