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

    /// <summary>
	/// Summary description for ProximityCardReaderStationManagerClass.
	/// </summary>
	public class ProximityCardReaderStationManagerClass :	StationManagerClass
	{
		protected	ProcessVariableClass				CardReaderDataPv;

		public ProximityCardReaderStationManagerClass(
			EventLog					eventLog,
			LoadRackManagerClass loadRackManager,
			StationClass			station,
			SiteManagerClass		siteManager,
			SecurityClass			security)
			: base(eventLog,loadRackManager,station,siteManager,security)
		{
			// Configure the PV's associated with the Station
			if(this.StationPv.URL != "")
			{
			    this.CardReaderDataPv=new ProcessVariableClass(
					PROCESS_VARIABLE_TYPE.CARDREADER_PV,
					UNIT_TYPE.STATION_UNIT,
					VarEnum.VT_BSTR,
					true,
					this.StationPv.OPCItemID+".Data",
					this.StationPv.URL,
					this.StationPv.ProgID
					);

			    this.OPCServerManager.AddProcessVariable(this.CardReaderDataPv);
			}

		    // ReSharper disable once DoNotCallOverridableMethodsInConstructor
			if(this.StationState == StationState.IDLE) this.ResetStationDevice();
		}

		public override int DisplayMessage (string stockMessage,string defaultResponse, int responseLength,int messageTimeout )
		{
			// Any display message is passed in from ProcessDriverID and must be an error
			// denote this error by displaying the Red LED and sounding the 
			ArrayList itemValues=new ArrayList();

		    ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".I/O Control") { Value = true };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Red LED") { Value = true };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Beep") { Value = true };
		    itemValues.Add(writeItem);

		    this.OPCServerManager.Write(new URL(this.StationPv.URL),(ItemValue []) itemValues.ToArray(typeof(ItemValue)));

			Thread.Sleep(this.MESSAGE_TIMEOUT*1000);

			itemValues.Clear();

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Red LED") { Value = false };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Beep") { Value = false };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".I/O Control") { Value = false };
		    itemValues.Add(writeItem);

		    this.OPCServerManager.Write(new URL(this.StationPv.URL),(ItemValue []) itemValues.ToArray(typeof(ItemValue)));

		    this.ProcessResponseData(string.Empty);

			return 0;
		}

		public override void CheckDriverMessages(bool acknowledged)
		{
			// Turn on the Green LED & Contact and perform standard Open Gate Processing
			ArrayList itemValues=new ArrayList();

		    ItemValue writeItem = new ItemValue(this.StationPv.OPCItemID + ".I/O Control") { Value = true };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Green LED") { Value = true };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Contact") { Value = true };
		    itemValues.Add(writeItem);

		    this.OPCServerManager.Write(new URL(this.StationPv.URL),(ItemValue []) itemValues.ToArray(typeof(ItemValue)));

			if(this.GatePV.URL != "")
			{
			    this.GatePV.ServerValue=true;
			    this.OPCServerManager.Write(this.GatePV);
			    this.StationState =StationState.OPENING_GATE;
			}

			Thread.Sleep(10000);

			itemValues.Clear();

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Green LED") { Value = false };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".Contact") { Value = false };
		    itemValues.Add(writeItem);

		    writeItem = new ItemValue(this.StationPv.OPCItemID + ".I/O Control") { Value = false };
		    itemValues.Add(writeItem);

		    this.OPCServerManager.Write(new URL(this.StationPv.URL),(ItemValue []) itemValues.ToArray(typeof(ItemValue)));

			if(this.GatePV.URL != "")
			{
			    this.GatePV.ServerValue=false;
			    this.OPCServerManager.Write(this.GatePV);
			    this.StationState =StationState.OPENING_GATE;
			}

		}

		protected override void EntryGateProcessing(ProcessVariableClass pv)
		{
			switch(pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
				{
					if(pv.IsQualityGood)
					{
						if((string) pv.ServerValue != "")
						{
						    this.ProcessDriverID((string) pv.ServerValue);
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
			switch(pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
				{
					if(pv.IsQualityGood)
					{
						if((string) pv.ServerValue != "")
						{
						    this.ProcessDriverID((string) pv.ServerValue);
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
			switch(pv.ProcessVariableType)
			{
				case PROCESS_VARIABLE_TYPE.CARDREADER_PV:
				{
					if(pv.IsQualityGood)
					{
                        logger.Debug($"BoL Station received card data {pv.ServerValue}");
						if((string) pv.ServerValue != "")
						{
						    this.ProcessDriverID((string) pv.ServerValue);
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
            if (this.GatePV.URL != string.Empty)
            {
                try
                {
                    this.GatePV.ServerValue = true;
                    this.OPCServerManager.Write(this.GatePV);
 
					if (this.Station.Type != STATION_TYPE.ENTRY_GATE)
					{
						this.CardOut();
					}
					this.GateTimer = 10;
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
                    this.DisplayMessage("[LoadRack|" + CompanyRoleMapClass.RoleID(COMPANY_ROLE.CARRIER) + "] [LoadRack|Locked Out]", null, 0, this.MESSAGE_TIMEOUT);
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

			if (this.Station.Type == STATION_TYPE.BOL)
			{
				this.CardIn();

				this.PrintTransactions();

				if (!this.SiteManager.AnyExitGates)
				{
					this.CardOut();
				}
				else
				{
					this.OpenGate();
				}

				return;
			}
			else if (this.Station.Type == STATION_TYPE.ENTRY_GATE)
			{
				this.CardIn();
			}

			this.StationState = StationState.IDLE;
            this.CompleteDriverProcessing(false);
        }
    }
}
