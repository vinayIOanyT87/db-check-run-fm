/******************************************************************************

	FILE NAME:		MultiloadIISMPStationManager.cs


	PURPOSE:			MultiloadIISMPStationManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		06/12/2008	W.Gray		7.4.5.0 - Change to include ItemName on
										OPC Quality Bad Messages (CSI 5961)

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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Threading;

	using FMBusinessObjects.DataObjects;

	using Opc;
	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	/// <summary>
	/// Summary description for MultiloadIISMPStationManagerClass.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class MultiloadIISMPStationManagerClass : MultiloadIIStationManagerClass
	{
		public MultiloadIISMPStationManagerClass(
			 EventLog eventLog,
			 LoadRackManagerClass loadRackManager,
			 StationClass station,
			 SiteManagerClass siteManager,
			 SecurityClass security)
			 : base(eventLog, loadRackManager, station, siteManager, security)
		{
		}

		public override void ResetStationDevice()
		{
			base.ResetStationDevice();
		}

		protected override void WeightScaleProcessing(ProcessVariableClass pv)
		{
			Monitor.Enter(this);

			try
			{
				switch (pv.ProcessVariableType)
				{
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
									this.MessageTimer = 0;

									string terminatingKey = values[1].Value.ToString();
									string keypadData = string.Empty;

									// Enter Key
									if (terminatingKey == "D")
									{
										keypadData = values[0].Value.ToString();
										keypadData = keypadData.TrimEnd(new[] { ' ' });
									}
									else if (terminatingKey == "B")
									{
										// Prev Key
										keypadData = EscapeString;
									}
									else if (terminatingKey == "C")
									{
										// Exit Key
									}

									this.ProcessResponseData(keypadData);
								}
								else
								{
									this.eventLog.WriteEntry("Multiload II OnInvoke : Keypad Data OPC Quality Bad " + pv.OPCItemID, EventLogEntryType.Error);
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
								string terminatingKey = pv.ServerValue.ToString();
								switch (terminatingKey)
								{
									case "s":
									case "F":
										// "s" means the user hit the "Stop" key.  This bounces the 
										// SMP back to it's resting screen and it won't accept further input
										// We have to take action at this point.
										// I've also seen "F" as well.
										this.ProcessMessageTimeout();
										break;
								}
							}

							break;
						}

					default:
						base.WeightScaleProcessing(pv);
						break;
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		public override void IssueDriverIDPrompt()
		{
			int timeOut = 999;
			Item[] items =
			{
					 new Item(new ItemIdentifier(this.StationPv.OPCItemID + ".RCU Status"))
				};
			ItemValueResult[] values = OPCServerManager.Read(new URL(StationPv.URL), items);
			if (values[0].Quality == Quality.Good
				 && 'C' == System.Convert.ToChar(values[0].Value))
			{
				timeOut = 1;
			}

			this.Driver = null;

			// This block will be used in the future when/if HID Card reader support is ported forward from 7.5 
			//if (this.Station.EnableHidCardReader)
			//    this.DisplayMessage("[LoadRack|Please Card In", null, 10, timeOut);
			//else
			//this.DisplayMessage("[LoadRack|Enter Driver ID", null, 10, timeOut);

			this.DisplayMessage("[LoadRack|Enter Driver ID", null, 10, timeOut);
			this.PriorStationState = StationState.IDLE;
			this.StationState = StationState.ENTER_DRIVER_ID_PROMPT;

		}

		protected override void StartPreloadBatches()
		{
			this.StationState = StationState.AUTHORIZING;
			foreach (LoadArmManagerClass loadArmManager in this.LoadArmManagerCollection)
			{
				if (loadArmManager.IsInAlarm)
				{
					continue;
				}

				if (this != loadArmManager.GetStationManager())
				{
					continue;
				}

				if (!loadArmManager.ShowNoProductsMessage())
				{
					loadArmManager.EnablePreset(this, false);
				}
			}

			foreach (LineItemDO lineItem in this.Transaction.LineItems)
			{
				if (lineItem.Status != TransactionStatus.LoadPending)
				{
					continue;
				}

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

					foreach (LineItemDO preload in loadArmManager.Bay(this).PreLoads)
					{
						if (preload == lineItem)
						{
							this.StationState = StationState.AUTHORIZED;
							loadArmManager.SetFocus();
							return;
						}
					}
				}
			}

			this.Transaction = null;
			this.StationState = StationState.RESET_ON_TIMEOUT;
		}

		public override bool SetDensityInUnit(string density)
		{
			List<ItemValue> itemValues = new List<ItemValue>();

			if (this.Station.Type == STATION_TYPE.WEIGHT_SCALE)
			{
				// We're just using the SMP as an RCU; we aren't metering anything and probably
				// don't have any arms configured anyways.
				return true;
			}

			// We should have one and only one arm for an SMP
			if (this.LoadArmManagerCollection.Count < 1)
			{
				// TODO:  Log alarm for no arms
				return false;
			}

			// This is an SMP, it has only one load arm.  We'll assume the 1st item in the load arm manager collection
			// (there should not be any other)
			var loadArmManager = (MultiloadIISMPLoadArmManagerClass)this.LoadArmManagerCollection.Item(0);
			for (int item = 0; item < loadArmManager.LoadArm.ComponentCollection.Count; item++)
			{
				// Multiload specifically wants API gravity and one decimal place for register 103043
				EngineeringUnit units = (this.CurrentTransactionAlias.DensityUnits != 0)
										 ? this.CurrentTransactionAlias.DensityUnits
										 : this.SiteManager.Site.DensityUnits;
				const byte SmpDecimalPlaces = 1;
				const double Scaling = 10;

				var offLoadDensity = new SIDouble
				{
					Units = units,
					Format = this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY)
				};
				var offLoadDensityApi = new SIDouble
				{
					Units = EngineeringUnit.FmdDegApi,
					Format = this.SiteManager.Site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DENSITY)
				};
				try
				{
					offLoadDensity.Value = System.Convert.ToDouble(density);
				}
				catch
				{
					return false;
				}

				offLoadDensityApi.SIValue = offLoadDensity.SIValue;
				offLoadDensityApi.Format.NumberDecimalDigits = SmpDecimalPlaces;

				var writeDensityItem = new ItemValue(this.StationPv.OPCItemID + ".Write Register")
				{
					Value =
						  "103043"
						  + 0.ToString("D3", CultureInfo.InvariantCulture)
						  + item.ToString("D3", CultureInfo.InvariantCulture)
						  + System.Convert.ToInt32(offLoadDensityApi.Value * Scaling).ToString("000", CultureInfo.InvariantCulture)
				};

				itemValues.Add(writeDensityItem);

				try
				{
					OPCServerManager.Write(
						 new URL(this.StationPv.URL),
						 itemValues.ToArray());
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
					return false;
				}
			}

			return true;
		}
	}
}
