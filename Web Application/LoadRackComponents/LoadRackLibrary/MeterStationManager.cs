/******************************************************************************

	FILE NAME:		MeterStationManager.cs


	PURPOSE:			MeterStationManagerClass


	COMMENTS:

		Copyright (C) Varec, Inc. Norcross, GA, USA.

		This file shall not be copied or reproduced in any form without
				the express written consent of Varec, Inc.


	AUTHOR(S):	C. Knight


	VERSION:		7.4.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		21-Apr-2008	C. Knight	7.4.0.0 - CSI 5584 - Initial creation to support meters
		
		24-Apr-2008	C. Knight	7.4.0.1	- CSI 5584 - Add creation of meter transactions
		
*******************************************************************************/
using System;
using System.Diagnostics;
using System.Net;
using Opc;
using Opc.Da;
using OpcCom.Da;

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;

namespace LoadRackLibrary
{
	/// <summary>
	/// Class for handling automated reading of totalizer meters for end-of-day processing
	/// </summary>
	public class MeterStationManagerClass : StationManagerClass
	{
		private readonly ProcessVariableClass meterPV;
		private Guid associatedTankGuid;

		public MeterStationManagerClass(EventLog eventLog,
			LoadRackManagerClass loadRackManager,
			StationClass station,
			SiteManagerClass siteManager,
			SecurityClass security)
			: base(eventLog, loadRackManager, station, siteManager, security)
		{
			// Get the Meter Flow PV
			foreach (ProcessVariableClass PV in this.Station.ProcessVariableCollection)
			{
				if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
				{
					this.meterPV = PV;
				}
			}

			this.associatedTankGuid = station.AssociatedTankGuid;
		}

		public override void CreateMeterReadingTransactions(
			SaveTransactionsSR saveTransactionsSR,
			TransactionAliasClass meterReadingTransactionAlias,
			DateTimeOffset inventoryDateTime)
		{
			try
			{
				// Meter must be associated with a tank to generate a transaction
				if (this.associatedTankGuid == Guid.Empty)
				{
					return;
				}

				Opc.Da.Server Server = new Opc.Da.Server(new OpcCom.Factory(), new URL(this.meterPV.URL));
				NetworkCredential Credentials = null;
				Server.Connect(new ConnectData(Credentials));

				TankClass tank = FMChannelHelper.MakeCall<ITanks, TankClass>(x => x.Get(base.Security, this.associatedTankGuid));

				Item[] SubItems = { new Item(new ItemIdentifier(this.meterPV.OPCItemID)) };

				ItemValueResult[] Values = Server.Read(SubItems);
				ItemValueResult meterReading = Values[0];

				TransactionDO Transaction = new TransactionDO
				{
					Alias = meterReadingTransactionAlias.ID,
					TransactionAliasGuid = meterReadingTransactionAlias.MasterRecordGuid,
					TransTypeID = TransactionTypes.T12_InventoryNotAffected,
					TransactionDateTime = TimeConverter.Now(this.SiteManager.Site),
					InventoryDate = TimeConverter.ToDate(inventoryDateTime).Date,
					Site = this.SiteManager.Site.ID,
					SiteGuid = this.SiteManager.Site.IdentityGuid,
					ManagerID = tank.ManagerID,
					ManagerCompanyGuid = tank.ManagerGuid
				};

				LineItemDO LineItem = new LineItemDO
				{
					StorageLocationID = tank.ID,
					StorageLocationTankGuid = tank.IdentityGuid,
					Product = tank.ProductID,
					ProductCode = tank.ProductCode,
					ProductGuid = tank.ProductGuid,
					MeterGuid = base.Station.Meter.IdentityGuid
				};

				//-|--------------------------------------------------------------------------------------------------------
				//-| We have the link to the meter at this point. We just need the rest of the info (specifically the ID)
				//-|--------------------------------------------------------------------------------------------------------
				MeterClass meter = FMChannelHelper.MakeCall<IMeters, MeterClass>(x => x.Get(this.Security, base.Station.Meter.IdentityGuid));
				LineItem.MeterID = meter.ID;
				double tempPlaceholder = System.Convert.ToDouble(meterReading.Value);
				SiteClass decimalPlaces = this.SiteManager.Site;
				ProductClass tankProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.Get(this.Security, tank.ProductGuid));

				//-|------------------------------
				//-| Referencing SiteClass.cs
				//-|------------------------------
				switch (tankProduct.ProductType)
				{
					case ProductType.AdditiveProduct:
						LineItem.MeterReading.MeterStart = Math.Round(tempPlaceholder, decimalPlaces._AdditiveVolumeDecimalPlaces);
						LineItem.MeterReading.MeterStop = Math.Round(tempPlaceholder, decimalPlaces._AdditiveVolumeDecimalPlaces);
						break;
					default:
						LineItem.MeterReading.MeterStart = Math.Round(tempPlaceholder, decimalPlaces._VolumeDecimalPlaces);
						LineItem.MeterReading.MeterStop = Math.Round(tempPlaceholder, decimalPlaces._VolumeDecimalPlaces);
						break;
				}

				LineItem.ProductType = ProductClass.ProductTypeID(tankProduct.ProductType);
				LineItem.MeterReading.StartDateTime = inventoryDateTime;
				LineItem.MeterReading.StopDateTime = inventoryDateTime;

				Transaction.LineItems.Add(LineItem);
				saveTransactionsSR.Transactions.Add(Transaction);
			}
			catch (ConnectFailedException)
			{
				this.eventLog.WriteEntry($"Error connecting to opc point {this.meterPV.OPCItemID} on server {this.meterPV.URL} for Meter Station {this.Station.ID}.\n\nUnable to create meter closeout transaction.",
											EventLogEntryType.Warning);
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry($"Error creating meter closeout transction for station {this.Station.ID}: {e.Message}\n\nStack Trace:\n{e.StackTrace}", EventLogEntryType.Error);
			}
		}
	}
}
