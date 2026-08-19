/******************************************************************************

	FILE NAME:		MicroloadNetLoadArmManager.cs


	PURPOSE:			Danload6000LoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

		11/24/2008	W.Gray		7.4.6.1 - Revised UpdateReferenceDensity to evalute Site.UseLastKnownGood (CSI 6251)

		03/02/2009	W.Gray		7.4.6.9 - Revised to read CTL in ReadComponentBatchData (CSI 1794)

*******************************************************************************/

namespace LoadRackLibrary
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Threading;

    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.LogClient;

    using Opc;
    using Opc.Da;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    using Convert = System.Convert;
    using Factory = OpcCom.Factory;
    using Server = Opc.Da.Server;

    /// <summary>
	/// Summary description for MicroloadNetLoadArmManagerClass.
	/// </summary>
	public class MicroloadNetLoadArmManagerClass : AcculoadIIILoadArmManagerClass
	{
		public MicroloadNetLoadArmManagerClass(
			EventLog eventLog,
			SiteManagerClass siteManager,
			StationManagerClass stationManager,
			LoadArmClass loadArm,
			SecurityClass security )
			: base (eventLog, siteManager, stationManager, loadArm, security)
		{
            /* Code commented out as it effectively does nothing here, but left in the source as it becomes important if functionality is added here
            if (LoadArm.Enabled == false)
            {
                // If the load arm is disabled, we don't want to set up any process variables for it.
                return;
            }
             * */
        }

		public override int NumberOfOffsets => 3;

	    protected override int MaxDisplayLineSize => 21;

	    protected override string [] MenuWriteTags
		{
			get 
			{
				string [] val = {		".Write First Line With Prompt",
										 ".Write Second Line",
										 ".Write Third Line",
										 ".Write Fourth Line",
										 ".Write Fifth Line",
										 ".Write Sixth Line" };
				
				return val;

			}
		}

        /// <summary>
		/// Returns the physical arm number from the preset that this load arm manager communicates with
		/// 
		/// Queries the preset to determine the physical arm number the first time this is called.
		/// For Microload, this is expected to be 1
		/// </summary>
		/// <param name="stationManager">Current station manager controlling this load arm manager</param>
		/// <returns>Physical Arm number from the device</returns>
		protected internal override int GetPresetArmNumber(StationManagerClass stationManager)
        {
            return 1;
        }

        public override void Start()
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string opcPath = loadArmPv.OPCItemID;
			int nLastIndex = loadArmPv.OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
			if ( nLastIndex > 0 )
			{
				opcPath = loadArmPv.OPCItemID.Substring(0, nLastIndex );
			}

		    ProcessVariableClass endTransaction = new ProcessVariableClass
		                                          {
		                                              URL = loadArmPv.URL,
		                                              OPCItemID = opcPath + ".Start"
		                                          };
		    this.OpcServerManager.Write( endTransaction );
		}

		public override void Stop()
		{
		    this.SetState(LOADARM_STATE.STOPPING);

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string opcPath = loadArmPv.OPCItemID;
			int nLastIndex = loadArmPv.OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
			if ( nLastIndex > 0 )
			{
				opcPath = loadArmPv.OPCItemID.Substring(0, nLastIndex );
			}

		    ProcessVariableClass endTransaction = new ProcessVariableClass
		                                          {
		                                              URL = loadArmPv.URL,
		                                              OPCItemID = opcPath + ".Stop"
		                                          };
		    this.OpcServerManager.Write( endTransaction );

			for(int iterator=0;iterator < MaxDelayForNoflowSeconds;iterator++)
			{
			    this.OpcServerManager.Read(this.FlowingPV);

				if(this.FlowingPV.IsQualityGood
				&& (bool)this.FlowingPV.ServerValue == false)
					break;

				Thread.Sleep(1000);
			}
		}

		public override void ReadProductNonResettableTotal(	int productNumber,
																				Server server,
																				out ItemValueResult	nonResettableGrossVolume )
		{
			ProcessVariableClass	loadArmPv= this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string opcPath = loadArmPv.OPCItemID;
			
			Item [] subItems={ new Item(new ItemIdentifier(opcPath+".Non-Resettable Totals.Product.Gross Volume")) };

			ItemValueResult [] values=server.Read(subItems);

			nonResettableGrossVolume=values[0];
		}

		public override void SetFocus()
		{
		}

		public override string GetBatchNumber(StationManagerClass stationManager)
		{
			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string opcPath = loadArmPv.OPCItemID;
			int nLastIndex = loadArmPv.OPCItemID.LastIndexOf(".", StringComparison.Ordinal);
			if ( nLastIndex > 0 )
			{
				opcPath = loadArmPv.OPCItemID.Substring(0, nLastIndex );
			}

			Server	server=new Server(new Factory(),new URL(loadArmPv.URL));
			server.Connect();

			Item [] subItems=	{ new Item(new ItemIdentifier(opcPath+".Status.Get Batch Number"))};

			ItemValueResult [] values=server.Read(subItems);

			server.Disconnect();
			server.Dispose();

			return ( Convert.ToString(values[0].Value) );

		}

		internal override void ReadComponentBatchData(
			int						productNumber,
			Server					server,
			out ItemValueResult	grossVolume,
			out ItemValueResult	standardDensity,
			out ItemValueResult	temperature,
			out ItemValueResult	netVolume,
			out ItemValueResult	ctpl,
			out ItemValueResult	pressure)
		{
			ProcessVariableClass	loadArmPv= this.LoadArm.ProcessVariableCollection[0];

			// Strip off the last part of the path
			string opcPath = loadArmPv.OPCItemID;

			string tagPrefix=opcPath+".Dynamic Values.Current Batch.";

			Item [] subItems={	new Item(new ItemIdentifier(tagPrefix+"Gross Volume")),
										new Item(new ItemIdentifier(tagPrefix+"Average Reference Density")),
										new Item(new ItemIdentifier(tagPrefix+"Average Temperature")),
										new Item(new ItemIdentifier(tagPrefix+"GST Volume")),
										new Item(new ItemIdentifier(tagPrefix+"Average CTL")),
										new Item(new ItemIdentifier(tagPrefix+"Average Pressure"))
								  };

			ItemValueResult [] values=server.Read(subItems);

			grossVolume=values[0];
			standardDensity=values[1];
			temperature=values[2];
			netVolume=values[3];
			ctpl=values[4];
			pressure = values[5];
		}

		protected override bool IsActiveStopKey ( string value )
		{
			return value == "S1";
		}

		public override bool UpdateReferenceDensity(StationManagerClass stationManager)
		{
			ProcessVariableClass	loadArmPv= this.LoadArm.ProcessVariableCollection[0];
	
			ArrayList itemValues=new ArrayList();
	
			ProductMapClass component= this.LoadArm.ComponentCollection[0];

			TankClass tank= this.SiteManager.GetTank(component, stationManager.Manager);
			if(tank == null)
			{
            // Tank Group may not have a market tank in which case no recipes will be enabled
            // for the product.
            return component.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANKGROUP_MAP;
         }

         ProcessVariableClass densityPv=tank.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STANDARD_DENSITY_PV];
			if(densityPv == null
			|| (!densityPv.IsQualityGood
			&& !this.SiteManager.Site.UseLastKnownGoodTankData))
				return false;

			EngineeringUnit units=(stationManager.CurrentTransactionAlias.DensityUnits != 0) ? stationManager.CurrentTransactionAlias.DensityUnits : stationManager.SiteManager.Site.DensityUnits;
			byte decimalPlaces=(stationManager.CurrentTransactionAlias.DensityUnits != 0) ? stationManager.CurrentTransactionAlias._DensityDecimalPlaces : stationManager.SiteManager.Site._DensityDecimalPlaces;

			double density;
			try
			{
				density=Convert.ToDouble(densityPv.GetValue(units,decimalPlaces));
			}
			catch
			{
				return false;
			}

		    ItemValue writeDensityItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change")
		                                 {
		                                     Value = "SY 413 " + density.ToString("F")
		                                 };

		    itemValues.Add(writeDensityItem);

			itemValues.Add(new ItemValue(loadArmPv.OPCItemID+".Log Out of Program Mode"));

			try
			{
			    this.OpcServerManager.Write(new URL(loadArmPv.URL),(ItemValue []) itemValues.ToArray(typeof(ItemValue)));
			}
			catch ( Exception e )
			{
			    this.eventLog.WriteEntry( e.ToString(), EventLogEntryType.Error );
				return false;
			}

			return true;
		}

        public override bool UpdateMaximumPreset(StationManagerClass stationManager)
        {
            var itemValues = new ArrayList();

            EngineeringUnit volumeUnit = (stationManager.CurrentTransactionAlias.VolumeUnits != 0) ? stationManager.CurrentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;
            var maxPreset = new SIDouble { Units = volumeUnit, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };
            ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

            var writeMaximumPresetItem = new ItemValue(loadArmPv.OPCItemID + ".Program Code Change")
            {
                Value = "SY 323 " + maxPreset.Value.ToString("F")
            };

            itemValues.Add(writeMaximumPresetItem);

            try
            {
                this.OpcServerManager.Write(new URL(loadArmPv.URL), (ItemValue[])itemValues.ToArray(typeof(ItemValue)));
            }
            catch (Exception e)
            {
                this.eventLog.WriteEntry(e.ToString(), EventLogEntryType.Error);
                return false;
            }

            return true;
        }

        // Update the Amount/Cycle and Rate parameters for the Recipe
        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
        /// </summary>
        /// <param name="name">
        /// Recipe name to be displayed
        /// </param>
        /// <param name="recipe">
        /// Recipe to be updated
        /// </param>
        /// <param name="product">
        /// Blend or Component definition for the recipe
        /// </param>
        /// <param name="additiveProfile">
        /// Additive Profile to apply to this recipe for the current ShipTo
        /// </param>
        /// <param name="deviceRecipeNumber">
        /// Internal recipe identifier on the preset.  Currently ignored for Microload
        /// </param>
        /// <returns>
        /// True is successful; false on error/failure
        /// </returns>
        public override bool UpdateRecipe(
			string name,
			ProductMapClass recipe,
            ProductClass product,
			AdditiveProfileClass additiveProfile,
			int deviceRecipeNumber)
		{
			ProcessVariableClass	loadArmPv= this.LoadArm.ProcessVariableCollection[0];
	
			ItemValue [] items=new ItemValue[this.LoadArm.AdditiveInjectorCollection.Count*2+2];
	
			items[0]=new ItemValue(	loadArmPv.OPCItemID+".Program Code Change");

		    if (name.Length > 9)
		    {
		        name = name.Substring( 0, 9 );
		    }

			items[0].Value=recipe.PresetNumber.ToString("D2")+" 002 "+name;

			int itemIndex=1;
			foreach(ProductMapClass additiveInjector in this.LoadArm.AdditiveInjectorCollection)
			{
				items[itemIndex]=new ItemValue(loadArmPv.OPCItemID+".Program Code Change");

				items[itemIndex+1]=new ItemValue(loadArmPv.OPCItemID+".Program Code Change");


				items[itemIndex].Value=recipe.PresetNumber.ToString("D2")+((additiveInjector.PresetNumber-1)*2+11).ToString("D3")+" 0.0";
				items[itemIndex+1].Value=recipe.PresetNumber.ToString("D2")+((additiveInjector.PresetNumber-1)*2+12).ToString("D3")+" 0.0";

				if(additiveProfile != null)
				{
					foreach(ProductMapClass additive in additiveProfile.AdditiveCollection)
					{
						if(additiveInjector.AssignedGuid == additive.AssignedGuid)
						{
							items[itemIndex].Value=recipe.PresetNumber.ToString("D2")+" "+((additiveInjector.PresetNumber-1)*2+11).ToString("D3")+" "+additive._AdditiveCycleVolume.Value.ToString("F");
							items[itemIndex+1].Value=recipe.PresetNumber.ToString("D2")+" "+((additiveInjector.PresetNumber-1)*2+12).ToString("D3")+" "+additive._AdditiveRate.Value.ToString("F");
							break;
						}
					} 
				}

				itemIndex+=2;
			}

			items[this.LoadArm.AdditiveInjectorCollection.Count*2+1]=new ItemValue(	loadArmPv.OPCItemID + ".Log Out of Program Mode");
			try
			{
			    this.OpcServerManager.Write(new URL(loadArmPv.URL),items);
			}
			catch ( Exception e )
			{
			    this.eventLog.WriteEntry( e.ToString(), EventLogEntryType.Error );
				return false;
			}

			return true;
		}

		public override void IssuePermissiveMessage(StationManagerClass stationManager)
		{
			if (stationManager == null)
				return;

			string message = this.GetPermissiveMessage(stationManager);

			if (message != "" &&
				(this.LoadArmState == LOADARM_STATE.AUTHORIZED || this.LoadArmState == LOADARM_STATE.INPROGRESS))
			{
				if (this.LoadArmState == LOADARM_STATE.AUTHORIZED) this.SetState(LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT);
				else if (this.LoadArmState == LOADARM_STATE.INPROGRESS)
				{
				    this.Unauthorize();
				    this.SetState(LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT);
				}

			    this.DisplayMessage(message + " " + stationManager.AcknowledgementMessage, stationManager.AcknowledgementResponseLength, 999);

			}
		}

		public override void ProcessPermissiveMessageAcknowledge(StationManagerClass stationManager, string response)
		{
			foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
			{
				if (loadArmManager.IsInAlarm)
					continue;

				if (loadArmManager.GetStationManager() != stationManager)
					continue;

				if (loadArmManager.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT)
				{
					loadArmManager.SetState(LOADARM_STATE.INPROGRESS);
					loadArmManager.ReleaseKeyPad();
				}

				else if (loadArmManager.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT)
				{
					loadArmManager.CancelPresetting(stationManager);
					loadArmManager.EndBatch();
					loadArmManager.IssueSelectPrompt(stationManager);
				}
			}
		}

		public override bool Authorize(StationManagerClass stationManager, double preset)
		{
			// since the microload operates different from the accuload. We need to override this function for the loadarm
			// permissive checks

		    this.SetState(LOADARM_STATE.AUTHORIZED);
			// check the permissives for the load arm.
			// since this is asyncronous and once the authorized bit is set the microload goes under it own control
			// we need to check them here and not authorize the microload if they are not set to true
			if (!this.CheckLoadArmPermissives(stationManager))
				return false;

			ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];
		    ProcessVariableClass authorizeTransaction = new ProcessVariableClass
		                                                {
		                                                    URL = loadArmPv.URL,
		                                                    OPCItemID = loadArmPv.OPCItemID+ ".Authorize And Set Batch Amount"
		                                                };


			if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				double offLoadPresetAmount = stationManager.OffLoadPresetAmount;
				authorizeTransaction.ServerValue = offLoadPresetAmount.ToString();
			}
			else if (stationManager.Station.SetDefaultPresetToZero)
			{
				authorizeTransaction.ServerValue = "0.0";
			}
			else
			{
				authorizeTransaction.ServerValue = preset.ToString();
			}

			StopWatch timer = new StopWatch(StopWatch.Appnames.LoadRackService, "Authorize : " + loadArmPv.OPCItemID);
			try
			{
			    this.OpcServerManager.Write(authorizeTransaction);
			}
			catch (Exception e)
			{
			    this.eventLog.WriteEntry("Microload LoadArmManager Authorize : " + e.Message, EventLogEntryType.Error);

				// Typical Errors are excessive preset or max batches.	
				try
				{
				    this.SetState(LOADARM_STATE.SELECT_PROMPT);
				    this.DisplayMessage("LoadRack|Authorize To Preset Error", 0, this.MessageTimeout);
					return true;
				}
				catch (Exception e1)
				{
				    this.eventLog.WriteEntry("Microload LoadArmManager Authorize : " + e1.Message, EventLogEntryType.Error);
				}
			}
			finally
			{
				timer.Stop();
			}

		    this.SetState(LOADARM_STATE.AUTHORIZED);
			return true;
		}

		protected bool CheckLoadArmPermissives(StationManagerClass stationManager)
		{
			foreach (ProcessVariableClass pv in this.LoadArm.LoadArmPermissives.Inputs)
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass permissives = pv.Parent;
							if (permissives == null)
								break;

							permissives.Update();

						    this.OpcServerManager.Update(true);

							if (!pv.IsQualityGood
							|| !((bool)pv.ServerValue))
							{
							    this.IssuePermissiveMessage(stationManager);
								return false;
							}

							break;
						}

					default:
				        this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + pv.OPCItemID);
						break;
				}
			}
			// check the station input permissives
			foreach (ProcessVariableClass pv in stationManager.Station.StationPermissives.Inputs)
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass permissives = pv.Parent;
							if (permissives == null)
								break;

							permissives.Update();

						    this.OpcServerManager.Update(true);

							if (!pv.IsQualityGood
							|| !((bool)pv.ServerValue))
							{
							    this.IssuePermissiveMessage(stationManager);
								return false;
							}

							break;
						}

					default:
				        this.eventLog.WriteEntry("StationManager OnInvoke : Unknown PV : " + pv.OPCItemID);
						break;
				}
			}
			return true;
		}

		/// <summary>
        /// This function clears all products on the physical preset assigned to the arm,
        /// as determined by the RecipeMap.  It also removes the recipe from the specified stations 
        /// recipe map
        /// </summary>
        /// <param name="stationManager">StationManager to clear recipe mapping from</param>
        protected override void ClearArmProducts(StationManagerClass stationManager)
        {
            // do nothing
        }
    }
}
