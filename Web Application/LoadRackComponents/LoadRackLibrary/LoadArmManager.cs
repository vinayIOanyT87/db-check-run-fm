/******************************************************************************

	FILE NAME:		LoadArmManager.cs


	PURPOSE:			LoadArmManagerClass


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------
		5/25/2008	W.Gray		7.4.4.0 - Changed UpdatePermissives to set No Additive when
										Additive Profile has no additives.

		8/29/2008	W.Gray		7.4.5.0 - Revused UpdatePermissives to check for null
										StationManager.ShipTo and null AuthorizedProduct (CSI 6120)
 
		9/09/2008	W.Gray		7.4.6.0 - Revised to support external components (CSI 5581)

		12/24/2008	W.Gray		7.4.6.1 - Added Support for Internal Additive Meter Totalizers (CSI 6341)

		9/20/2009	W.Gray		7.4.6.2 - Revised to separate IssuePermissiveMessage between
										issueing the message and processing the acknowledgement.  Also
										revised such that messages are not issued to an arm in alarm
										or disabled.

		12/1/2009	W.Gray		7.5.1.1 - Revised to handle OPCServerManager CancelSubscriptions within OPCServerManager dispose
  
		01/04/2010	W.Gray		7.5.1.2 - Revised to IssueEndBatch after acknowledge of Batch Stopped Prompt (WI 10165)

 *******************************************************************************/

namespace LoadRackLibrary
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.Threading;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;

	using Opc;
	using Opc.Da;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	public enum LOADARM_STATE
	{
		NORMAL,
		AUTHORIZED,
		INPROGRESS,
		BATCH_COMPLETE_PROMPT,
		SELECT_PROMPT,
		FINISHED,
		COMPARTMENT_PROMPT,
		COMPARTMENT_ALREADY_LOADED_MSG,
		INVALID_BATCH_SELECTION_MSG,
		INVALID_SELECT_SELECTION_MSG,
		INVALID_COMPARTMENT_SELECTION_MSG,
		EQUIPMENTID_PROMPT,
		INVALID_EQUIPMENT_SELECTION_MSG,
		NO_PRODUCTS_TO_LOAD,
		FINISHED_WITH_NO_PRODUCTS_TO_LOAD,
		NON_PRELOAD_EQUIPMENT_PROMPT,
		INVALID_NON_PRELOAD_EQUIPMENT_SELECTION_MSG,
		NO_COMPARTMENTS_TO_LOAD_MSG,
		SPLASH_BLEND_COMPONENT_PROMPT,
		INVALID_SPLASH_BLEND_COMPONENT_SELECTION_MSG,
		END_BATCH_PROMPT,
		STOPPING,
		BATCH_COMPLETE_LOADING_IN_PROGRESS_MSG,
		TRANSACTION_COMPLETION_LOADING_IN_PROGRESS_MSG,
		SELECT_LOADING_IN_PROGRESS_MSG,
		SELECT_RECIPE_PROMPT,
		PRESET_VOLUME_PROMPT,
		AUTHORIZED_PERMISSIVE_PROMPT,
		INPROGRESS_PERMISSIVE_PROMPT,
		INVALID_RECIPE_SELECTION_MSG,
		INVALID_PRESET_MSG,
		PRESET_MSG,
		PRESET_ENABLED,
		BATCH_COMPLETE,
		UNLOAD_VOLUME_PROMPT,
		METERRECIRC_VOLUME_PROMPT,
		BATCH_STOPPED_PROMPT,
		TRANSATION_COMPLETION_PROMPT,
		INVALID_TRANSACTION_COMPLETION_MSG,
		NO_EQUIPMENT_TO_LOAD_MSG,
		COMPARTMENT_LOADED_PROMPT,
		PRODUCT_UNAVAILABLE,
		MAXIMUM_PRESET_LESS_THAN_OR_EQUAL_ZERO,
		COMPARTMENT_NOT_ON_ORDER_MSG,
		PRESET_ENABLED_PERMISSIVE_PROMPT
	};


	/// <summary>
	/// Summary description for LoadArmManagerCollectionClass.
	/// </summary>
	public class LoadArmManagerCollectionClass : CollectionBase
	{

		public void Add(LoadArmManagerClass LoadArmManager)
		{
			this.List.Add(LoadArmManager);
		}

		public void Add(LoadArmManagerCollectionClass loadArmCollection)
		{
			foreach (LoadArmManagerClass manager in loadArmCollection)
			{
				this.Add(manager);
			}
		}

		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new IndexOutOfRangeException("Invalid Index");
			}
			this.List.RemoveAt(index);
		}

		public void Remove(LoadArmManagerClass LoadArmManager)
		{
			int index = 0;
			foreach (LoadArmManagerClass Item in this.List)
			{
				if (Item.LoadArm.IdentityGuid == LoadArmManager.LoadArm.IdentityGuid)
				{
					this.List.RemoveAt(index);
					return;
				}
				index++;
			}
		}

		public LoadArmManagerClass Item(int Index)
		{
			return (LoadArmManagerClass)this.List[Index];
		}

		public void SetState(StationManagerClass StationManager, LOADARM_STATE NewState)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.SetState(NewState);
				}
			}
		}

		public void ResetPreloads(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.Bay(StationManager).PreLoads = new ArrayList();
				}
			}
		}

		public void ResetSplashProducts(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.Bay(StationManager).SplashProducts = new ArrayList();
				}
			}
		}


		public void ReleaseKeyPad(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.ReleaseKeyPad();
				}
			}
		}

		public void ResetPowerFailAlarm(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.ResetPowerFailAlarm();
				}
			}
		}

		public void ResetCommunicationsFailAlarm(StationManagerClass stationManager)
		{
			foreach (LoadArmManagerClass loadArmManager in this.List)
			{
				if (stationManager == loadArmManager.GetStationManager())
				{
					loadArmManager.ResetCommunicationsFailAlarm();
				}
			}
		}

		public void SendEndTransaction(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.SendEndTransaction();
				}
			}
		}

		public void SyncDateAndTime(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.SyncDateAndTime();
				}
			}
		}

		public void ClearRecipeMap(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.Bay(StationManager).RecipeMap = 0x0;
				}
			}
		}


		public void Stop(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.Stop();
				}
			}
		}

		public void StopIfInProgress(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				switch (LoadArmManager.LoadArmState)
				{
					case LOADARM_STATE.AUTHORIZED:
					case LOADARM_STATE.INPROGRESS:
						if (StationManager == LoadArmManager.GetStationManager())
						{
							LoadArmManager.Unauthorize();
						}
						break;
				}
			}
		}

		public void ProcessMessageTimeout(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.ProcessMessageTimeout(StationManager);
				}
			}
		}

		public void RefreshPermissives(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					LoadArmManager.OpcServerManager.Update(false);
				}
			}
		}

		public void ModifyProcessVariableMessage(ApplicationStringClass Message)
		{
			foreach (LoadArmManagerClass Item in this.List)
			{
				Item.ModifyProcessVariableMessage(Message);
			}
		}

		public void PurgeProcessVariableMessage(Guid identityGuid)
		{
			foreach (LoadArmManagerClass Item in this.List)
			{
				Item.PurgeProcessVariableMessage(identityGuid);
			}
		}

		public void IssuePermissiveMessage(StationManagerClass StationManager)
		{
			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager != LoadArmManager.GetStationManager())
				{
					continue;
				}

				if (LoadArmManager.IsInAlarm)
				{
					continue;
				}

				LoadArmManager.IssuePermissiveMessage(StationManager);
			}
		}

		public bool CheckAvailableVolume(StationManagerClass StationManager)
		{
			bool saveTransaction = false;

			foreach (LoadArmManagerClass LoadArmManager in this.List)
			{
				if (StationManager == LoadArmManager.GetStationManager())
				{
					if (LoadArmManager.CheckAvailableVolume(StationManager))
					{
						saveTransaction = true;
					}
				}
			}

			return saveTransaction;
		}
	}

	public class BayInfo
	{
		public BayInfo()
		{
			this.PreLoads = new ArrayList();
			this.SplashProducts = new ArrayList();
			this.StationManager = null;
			this.RecipeMap = 0;
			this.ExtendedRecipeMap = 0;
		}

		public StationManagerClass StationManager { get; set; }

		public ulong RecipeMap { get; set; }

		public ulong ExtendedRecipeMap { get; set; }

		public ArrayList PreLoads { get; set; }

		public ArrayList SplashProducts { get; set; }
	}


	/// <summary>
	/// Summary description for LoadArmManagerClass.
	/// </summary>
	public abstract class LoadArmManagerClass : IDisposable
	{
		protected const int MaxDelayForNoflowSeconds = 30;

		private LOADARM_STATE loadArmState = LOADARM_STATE.NORMAL;
		protected EventLog eventLog;
		protected bool AlreadyDisposed = false;
		public LoadArmClass LoadArm;
		public SiteManagerClass SiteManager;
		public BayInfo BayA = new BayInfo();
		public BayInfo BayB = new BayInfo();
		public OPCServerManagerClass OpcServerManager;
		public ProductClass CurrentLineItemProduct;
		public AdditiveProfileClass AdditiveProfile;
		public int MessageRetries;
		public string EquipmentID = string.Empty;
		public string NonPreloadEquipmentSelection = string.Empty;
		public int NonPreloadCompartmentSelection = -1;
		public SubLineItemDO SplashSubLineItem = null;
		protected LineItemDO CurrentLineItem = null;
		public SecurityClass Security;
		protected bool CommunicationsFailure = false;
		protected object LastSwingArmStatus = null;
		protected double MaximumPreset = 0.0;
		internal ProductMapClass CurrentRecipe = null;
		protected int CurrentPreset = 0;
		public bool ResponsePending = false;
		protected DisplayMenuParameters CurrentMenuParameters;
		public ProductMapCollectionClass AvailableRecipeCollection = null;
		public ArrayList EquipmentList = new ArrayList();

		public int PromptTimeout;
		public int MessageTimeout;

		protected LoadArmManagerClass(
			EventLog eventLog,
			SiteManagerClass siteManager,
			StationManagerClass stationManager,
			LoadArmClass loadArm,
			SecurityClass security)
		{
			this.eventLog = eventLog;
			this.SiteManager = siteManager;

			if (stationManager.Station.SwingArmPosition == "A")
			{
				this.BayA.StationManager = stationManager;
			}
			else
			{
				this.BayB.StationManager = stationManager;
			}

			this.PromptTimeout = stationManager.Station.StationPromptTimeout;
			this.MessageTimeout = stationManager.Station.StationMessageTimeout;

			this.LoadArm = loadArm;
			this.Security = security;
			this.OpcServerManager = new OPCServerManagerClass(eventLog);

			if (loadArm.Enabled == false)
			{
				// Only initialize the process variables if the load arm is enabled.  This allows us to load the arm 
				// for meter closeouts even when the arm is disabled.  Some customers need to disable arms when source
				// tanks get too low, but they still need the meters included in the meter closeout records.
				return;
			}

			// redo the recipe collection based on the components

			this.AvailableRecipeCollection = new ProductMapCollectionClass();

			this.SetAvailableProductsCollection();

			this.OpcServerManager.Invoke += this.OnInvoke;

			loadArm.LoadArmPermissives.Enabled = false;

			foreach (ProcessVariableClass pv in loadArm.LoadArmPermissives.Outputs)
			{
				this.OpcServerManager.AddProcessVariable(pv);
			}

			foreach (ProcessVariableClass permissive in loadArm.LoadArmPermissives.Inputs)
			{
				this.OpcServerManager.AddProcessVariable(permissive);
			}

			loadArm.NoAdditivePermissives.Enabled = false;

			foreach (ProcessVariableClass pv in loadArm.NoAdditivePermissives.Outputs)
			{
				this.OpcServerManager.AddProcessVariable(pv);
			}

			foreach (ProcessVariableClass permissive in loadArm.NoAdditivePermissives.Inputs)
			{
				this.OpcServerManager.AddProcessVariable(permissive);
			}

			foreach (ProductMapClass component in loadArm.ComponentCollection)
			{
				component.Permissives.Enabled = false;

				foreach (ProcessVariableClass pv in component.Permissives.Outputs)
				{
					this.OpcServerManager.AddProcessVariable(pv);
				}

				foreach (ProcessVariableClass permissive in component.Permissives.Inputs)
				{
					this.OpcServerManager.AddProcessVariable(permissive);
				}
			}

			foreach (ProductMapClass additive in loadArm.AdditiveInjectorCollection)
			{
				additive.Permissives.Enabled = false;

				foreach (ProcessVariableClass pv in additive.Permissives.Outputs)
				{
					this.OpcServerManager.AddProcessVariable(pv);
				}

				foreach (ProcessVariableClass permissive in additive.Permissives.Inputs)
				{
					this.OpcServerManager.AddProcessVariable(permissive);
				}
			}

			foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
			{
				recipe.Permissives.Enabled = false;

				foreach (ProcessVariableClass pv in recipe.Permissives.Outputs)
				{
					this.OpcServerManager.AddProcessVariable(pv);
				}

				foreach (ProcessVariableClass permissive in recipe.Permissives.Inputs)
				{
					this.OpcServerManager.AddProcessVariable(permissive);
				}
			}

			foreach (ProductMapClass externalComponent in loadArm.ExternalComponentCollection)
			{
				externalComponent.Permissives.Enabled = false;

				foreach (ProcessVariableClass pv in externalComponent.Permissives.Outputs)
				{
					this.OpcServerManager.AddProcessVariable(pv);
				}

				foreach (ProcessVariableClass permissive in externalComponent.Permissives.Inputs)
				{
					this.OpcServerManager.AddProcessVariable(permissive);
				}
			}

			foreach (ProductMapClass externalOffloadComponent in loadArm.OffloadExternalProductCollection)
			{
				externalOffloadComponent.Permissives.Enabled = false;

				foreach (ProcessVariableClass pv in externalOffloadComponent.Permissives.Outputs)
				{
					this.OpcServerManager.AddProcessVariable(pv);
				}

				foreach (ProcessVariableClass permissive in externalOffloadComponent.Permissives.Inputs)
				{
					this.OpcServerManager.AddProcessVariable(permissive);
				}
			}

			this.OpcServerManager.Update(true);
		}

		~LoadArmManagerClass()
		{
			this.Dispose();
		}

		protected internal virtual bool SuppressLoadFinishedPrompt
		{
			get { return false; }
		}

		public virtual LOADARM_STATE LoadArmState
		{
			get { return this.loadArmState; }
			set { this.loadArmState = value; }
		}

        public BayInfo Bay(StationManagerClass StationManager)
		{
			if (this.BayA.StationManager == StationManager)
			{
				return this.BayA;
			}

			if (this.BayB.StationManager == StationManager)
			{
				return this.BayB;
			}

			throw new Exception("Invalid StationManager");
		}

		public int ArmNumber(StationManagerClass StationManager)
		{
			if (this.BayA.StationManager == StationManager)
			{
				return this.LoadArm.BayAArmNumber;
			}

			if (this.BayB.StationManager == StationManager)
			{
				return this.LoadArm.BayBArmNumber;
			}

			throw new Exception("Invalid StationManager");
		}

        /// <summary>
		/// Returns the physical arm number from the preset that this load arm manager communicates with
		/// 
		/// For the general case, assume that the FuelsManager arm number matches the device arm number
		/// </summary>
		/// <param name="stationManager"></param>
		/// <returns></returns>
		protected internal virtual int GetPresetArmNumber(StationManagerClass stationManager)
        {
            return this.ArmNumber(stationManager);
        }

        public virtual void Dispose()
		{
			if (!this.AlreadyDisposed)
			{
				if (this.LoadArm.Enabled)
				{
					this.OpcServerManager.Invoke -= this.OnInvoke;
				}
				this.OpcServerManager.Dispose();

				GC.SuppressFinalize(this);
				this.AlreadyDisposed = true;
			}
		}

		public void CancelPresetting(StationManagerClass StationManager)
		{
			if (StationManager.PreloadInProgress)
			{
				if (this.CurrentLineItem != null)
				{
					this.CurrentLineItem.Status = TransactionStatus.LoadPending;
					foreach (SubLineItemDO SubLineItem in this.CurrentLineItem.SubLineItems)
					{
						if (SubLineItem.Status == TransactionStatus.InProgress)
						{
							SubLineItem.Status = TransactionStatus.LoadPending;
						}
					}
				}
			}
			else
			{
				if (this.NonPreloadEquipmentSelection != ""
					 && this.NonPreloadCompartmentSelection != -1)
				{
					CompartmentInfo Info = this.GetCompartmentIfValid(this.NonPreloadEquipmentSelection, this.NonPreloadCompartmentSelection);
					Info.Loaded = false;
				}
			}
		}

		protected void UpdatePermissives(StationManagerClass stationManager, int recipeNumber, bool authorized)
		{
			ProductMapClass currentRecipe = this.GetRecipeByRecipeNumber(recipeNumber);

			this.UpdatePermissives(stationManager, currentRecipe, authorized);
		}

		protected void UpdatePermissives(StationManagerClass stationManager, ProductMapClass currentRecipe, bool authorized)
		{
			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("Entered UpdatePermissives with authorized flag = " + authorized + ":  Stacktrace" + Environment.StackTrace);
			if (!authorized)
			{
				this.LoadArm.LoadArmPermissives.Enabled = false;
				this.LoadArm.NoAdditivePermissives.Enabled = false;

				foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
				{
					component.Permissives.Enabled = false;
				}

				foreach (ProductMapClass additive in this.LoadArm.AdditiveInjectorCollection)
				{
					additive.Permissives.Enabled = false;
				}

				foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
				{
					recipe.Permissives.Enabled = false;
				}

				foreach (ProductMapClass externalComponent in this.LoadArm.ExternalComponentCollection)
				{
					externalComponent.Permissives.Enabled = false;
				}

				foreach (ProductMapClass flowControlledAdditive in this.LoadArm.FlowControlledAdditiveCollection)
				{
					flowControlledAdditive.Permissives.Enabled = false;
				}
			}
			else
			{
				this.LoadArm.LoadArmPermissives.Enabled = true;

				if (currentRecipe == null)
				{
					foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
					{
						component.Permissives.Enabled = false;
					}

					foreach (ProductMapClass additive in this.LoadArm.AdditiveInjectorCollection)
					{
						additive.Permissives.Enabled = false;
					}

					foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
					{
						recipe.Permissives.Enabled = false;
					}

					foreach (ProductMapClass externalComponent in this.LoadArm.ExternalComponentCollection)
					{
						externalComponent.Permissives.Enabled = false;
					}

					foreach (ProductMapClass flowControlledAdditive in this.LoadArm.FlowControlledAdditiveCollection)
					{
						flowControlledAdditive.Permissives.Enabled = false;
					}
				}
				else
				{
					foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
					{
						recipe.Permissives.Enabled = recipe.AssignedGuid == currentRecipe.AssignedGuid;
					}

					if (currentRecipe.AssignedProductType == ProductType.BlendProduct)
					{
						ProductClass blend = FMChannelHelper.MakeCall<IProducts, ProductClass>(products => products.Get(this.Security, currentRecipe.AssignedGuid, false));
						foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
						{
							component.Permissives.Enabled = blend.ComponentCollection.Find(x => x.AssignedGuid == component.AssignedGuid) != null;
						}

						foreach (ProductMapClass externalComponent in this.LoadArm.ExternalComponentCollection)
						{
							externalComponent.Permissives.Enabled = blend.ComponentCollection.Find(x => x.AssignedGuid == externalComponent.AssignedGuid) != null;
						}

						foreach (ProductMapClass flowControlledAdditive in this.LoadArm.FlowControlledAdditiveCollection)
						{
							flowControlledAdditive.Permissives.Enabled = blend.ComponentCollection.Find(x => x.AssignedGuid == flowControlledAdditive.AssignedGuid) != null;
						}
					}
					else
					{
						foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
						{
							component.Permissives.Enabled = currentRecipe.AssignedGuid == component.AssignedGuid;
						}

						foreach (ProductMapClass externalComponent in this.LoadArm.ExternalComponentCollection)
						{
							externalComponent.Permissives.Enabled = false;
						}

						foreach (ProductMapClass flowControlledAdditive in this.LoadArm.FlowControlledAdditiveCollection)
						{
							flowControlledAdditive.Permissives.Enabled = false;
						}
					}

					ProductMapClass authorizedProduct = null;

					if (stationManager.ShipTo != null)
					{
						ProductClass currentRecipeProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(x => x.GetBasicInfo(Security, currentRecipe.AssignedGuid, Security.SiteGuid));
						authorizedProduct = stationManager.ShipTo.AuthorizedProductCollection.Find(x => x.AssignedGuid == currentRecipeProduct.IdentityGuid);
					}

					AdditiveProfileClass additiveProfile = null;

					if (authorizedProduct != null
					&& authorizedProduct.AdditiveProfileGuid != Guid.Empty)
					{
						additiveProfile = FMChannelHelper.MakeCall<IAdditiveProfiles, AdditiveProfileClass>(additiveProfiles => additiveProfiles.Get(this.Security, authorizedProduct.AdditiveProfileGuid));
                    }

                    if (additiveProfile != null
					&& additiveProfile.AdditiveCollection.Count != 0)
					{
						this.LoadArm.NoAdditivePermissives.Enabled = false;

						foreach (ProductMapClass additive in this.LoadArm.AdditiveInjectorCollection)
						{
							additive.Permissives.Enabled = additiveProfile.AdditiveCollection.Find(x => x.AssignedGuid == additive.AssignedGuid) != null;
						}
					}
					else
					{
						this.LoadArm.NoAdditivePermissives.Enabled = true;

						foreach (ProductMapClass additive in this.LoadArm.AdditiveInjectorCollection)
						{
							additive.Permissives.Enabled = false;
						}
					}
				}
			}

			this.OpcServerManager.Update(true);
		}

		protected virtual void OnInvoke(ProcessVariableClass pv)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager != null)
			{
				switch (pv.ProcessVariableType)
				{
					case PROCESS_VARIABLE_TYPE.INPUT_PERMISSIVE_PV:
						{
							PermissivesClass permissives = pv.Parent;
							if (permissives == null)
							{
								break;
							}

							permissives.Update();

							this.OpcServerManager.Update(true);

							if (!pv.IsQualityGood
							|| !(bool)pv.ServerValue)
							{
								this.IssuePermissiveMessage(stationManager);
							}

							break;
						}

					default:
						this.eventLog.WriteEntry("LoadArmManager OnInvoke : Unknown PV : " + pv.OPCItemID);
						break;
				}
			}
		}


		public virtual void ReadPresetAmount(
			 Opc.Da.Server Server,
			 out ItemValueResult PresetAmount)
		{
			PresetAmount = null;
		}

		public virtual void ReadBatchRecipe(
			 string BatchNumber,
			 Opc.Da.Server Server,
			 out ItemValueResult Recipe)
		{
			Recipe = null;
		}

		public virtual void ReadCleanLineParameters(
			Opc.Da.Server server,
			out double? cleanLineAmount,
			out int? cleanLineProduct,
			out int? cleanLineDeduct)
		{
			cleanLineAmount = null;
			cleanLineDeduct = null;
			cleanLineProduct = null;
		}

		public abstract void Start();

		public abstract void Stop();

		public virtual bool AllocateRecipes(ulong recipeMap)
		{
			return this.AllocateRecipes(recipeMap, 0);
		}

		public abstract bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap);

		public abstract void Unauthorize();

		public virtual int NumberOfOffsets
		{
			get
			{
				return 0;
			}
		}

		public virtual bool LogOutOfProgramMode() { return true; }

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
		/// Internal recipe identifier on the preset
		/// </param>
		/// <returns>
		/// True is successful; false on error/failure
		/// </returns>
		public abstract bool UpdateRecipe(
			 string name,
			 ProductMapClass recipe,
				 ProductClass product,
			 AdditiveProfileClass additiveProfile,
			 int deviceRecipeNumber);

		public virtual bool UpdateReferenceDensity(StationManagerClass stationManager) { return true; }

		/// <summary>
		/// Overload for setting an arbitrary reference density; used for Offloads where
		/// the BoL specifies the reference density of received product.
		/// </summary>
		/// <param name="stationManager">Station manager owning this load arm</param>
		/// <param name="density">Density to pass to the device; the density is expected to be in the device's units</param>
		/// <returns></returns>
		public virtual bool UpdateReferenceDensity(StationManagerClass stationManager, double density)
		{
			return true;
		}

		public virtual bool UpdateMaximumPreset(StationManagerClass stationManager)
		{
			return true;
		}

		public virtual void CancelUnauthorizedTransaction() { }

		public virtual void CaptureMeterValues() { }

		public int GetArmNumber(StationManagerClass StationManager)
		{
			if (!this.LoadArm.SwingArm)
			{
				if (StationManager == this.BayA.StationManager)
				{
					return this.LoadArm.BayAArmNumber;
				}

				if (StationManager == this.BayB.StationManager)
				{
					return this.LoadArm.BayBArmNumber;
				}

				return 0;
			}

			if (this.SwingArmPosition == "A")
			{
				return this.LoadArm.BayAArmNumber;
			}

			if (this.SwingArmPosition == "B")
			{
				return this.LoadArm.BayBArmNumber;
			}

			return 0;
		}

		protected virtual string SwingArmPosition
		{
			get
			{
				return "A";
			}
		}

		public virtual bool IsInAlarm
		{
			get
			{
				return false;
			}
		}

		public StationManagerClass GetStationManager()
		{
			if (!this.LoadArm.SwingArm)
			{
				return this.BayA.StationManager ?? this.BayB.StationManager;
			}

			if (this.SwingArmPosition == "A")
			{
				return this.BayA.StationManager;
			}

			if (this.SwingArmPosition == "B")
			{
				return this.BayB.StationManager;
			}

			return null;
		}

		public ProductMapClass GetComponent(Guid identityGuid)
		{
			return this.GetComponent(identityGuid, true, true);
		}

		public ProductMapClass GetComponent(Guid identityGuid, bool includeExternalComponents, bool includeFlowControlledAdditives)
		{
			ProductMapClass component = this.LoadArm.ComponentCollection.Find(x => x.AssignedGuid == identityGuid)
													?? this.LoadArm.ComponentCollection.Find(x => x.AssignedGuid == FMChannelHelper.MakeCall<IProducts, Guid>(y => y.GetMasterRecordGuid(this.Security, identityGuid)));

			if (component == null && includeExternalComponents)
			{
				component = this.LoadArm.ExternalComponentCollection.Find(x => x.AssignedGuid == identityGuid)
									?? this.LoadArm.ExternalComponentCollection.Find(x => x.AssignedGuid == FMChannelHelper.MakeCall<IProducts, Guid>(y => y.GetMasterRecordGuid(this.Security, identityGuid)));
			}

			if (component == null && includeFlowControlledAdditives)
			{
				component = this.LoadArm.FlowControlledAdditiveCollection.Find(x => x.AssignedGuid == identityGuid)
									?? this.LoadArm.FlowControlledAdditiveCollection.Find(x => x.AssignedGuid == FMChannelHelper.MakeCall<IProducts, Guid>(y => y.GetMasterRecordGuid(this.Security, identityGuid)));
			}

			return component;
		}

		public ProductMapClass GetAdditive(Guid identityGuid)
		{
			// TODO:  ad-hackery for TAS MVP.  This will need to be corrected to MRG only
			// once all product maps have been updated to use MRGs and SiteGuids.
			return this.LoadArm.AdditiveInjectorCollection.Find(x => x.AssignedGuid == identityGuid) ??
					  this.LoadArm.AdditiveInjectorCollection.Find(x => x.AssignedGuid == FMChannelHelper.MakeCall<IProducts, Guid>(y => y.GetMasterRecordGuid(this.Security, identityGuid)));
		}

		public ProductMapClass GetRecipe(Guid identityGuid)
		{
			// TODO:  ad-hackery for TAS MVP.  This will need to be corrected to MRG only
			// once all product maps have been updated to use MRGs and SiteGuids.
			return this.AvailableRecipeCollection.Find(x => x.AssignedGuid == identityGuid) ??
				 this.AvailableRecipeCollection.Find(x => x.AssignedGuid == FMChannelHelper.MakeCall<IProducts, Guid>(y => y.GetMasterRecordGuid(this.Security, identityGuid)));
		}

		public virtual int GetRecipeNumber(ProductMapClass recipe)
		{
			return recipe.PresetNumber;
		}

		public virtual ProductMapClass GetRecipeByRecipeNumber(int recipeNumber)
		{
			foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
			{
				if (recipe.PresetNumber == recipeNumber)
				{
					return recipe;
				}
			}

			return null;
		}

		public bool IsAdditiveProfileServedByLoadArm(AdditiveProfileClass additiveProfile)
		{
			return additiveProfile == null || this.LoadArm.IsAdditiveProfileAvailable(additiveProfile);
		}

		public bool IsProductServedByLoadArm(ProductClass product)
		{
			ProductMapClass armComponent = this.GetComponent(product.IdentityGuid);
			if (armComponent != null && (armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_MAP || armComponent.Type == PRODUCT_MAP_TYPE.PRESET_EXTERNAL_COMPONENT_TANKGROUP_MAP)
				 && armComponent.Permissives.Permitted)
			{
				return true;
			}

			ProductMapClass recipe = this.AvailableRecipeCollection.Find(x => x.AssignedGuid == product.MasterRecordGuid);
			return recipe != null && recipe.Permissives.Permitted;
		}

		public virtual void ReleaseKeyPad() { }

		public virtual void SetFocus() { }

		public virtual bool ProcessMessageTimeout(StationManagerClass stationManager)
		{
			switch (this.LoadArmState)
			{
				case LOADARM_STATE.COMPARTMENT_PROMPT:
					this.IssueCompartmentPrompt(stationManager);
					break;

				case LOADARM_STATE.EQUIPMENTID_PROMPT:
					this.IssueEquipmentPrompt(stationManager);
					break;

				case LOADARM_STATE.BATCH_COMPLETE_PROMPT:
					this.IssueBatchCompletePrompt();
					break;

				case LOADARM_STATE.SELECT_PROMPT:
					this.IssueSelectPrompt(stationManager);
					break;

				case LOADARM_STATE.BATCH_COMPLETE_LOADING_IN_PROGRESS_MSG:
					this.IssueBatchCompletePrompt();
					break;

				case LOADARM_STATE.TRANSACTION_COMPLETION_LOADING_IN_PROGRESS_MSG:
					this.IssueTransactionCompletionPrompt();
					break;

				case LOADARM_STATE.SELECT_LOADING_IN_PROGRESS_MSG:
					this.IssueSelectPrompt(stationManager);
					break;

				case LOADARM_STATE.COMPARTMENT_ALREADY_LOADED_MSG:
					this.PromptForNextBatch(stationManager, true);
					break;

				case LOADARM_STATE.INVALID_BATCH_SELECTION_MSG:
					this.IssueBatchCompletePrompt();
					break;

				case LOADARM_STATE.INVALID_SELECT_SELECTION_MSG:
					this.IssueSelectPrompt(stationManager);
					break;

				case LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG:
					this.IssueCompartmentPrompt(stationManager);
					break;

				case LOADARM_STATE.INVALID_EQUIPMENT_SELECTION_MSG:
					this.IssueEquipmentPrompt(stationManager);
					break;

				case LOADARM_STATE.INVALID_NON_PRELOAD_EQUIPMENT_SELECTION_MSG:
				case LOADARM_STATE.NON_PRELOAD_EQUIPMENT_PROMPT:
					this.IssueNonPreloadEquipmentPrompt(stationManager);
					break;

				case LOADARM_STATE.NO_PRODUCTS_TO_LOAD:
					this.SetFinishedLoading();
					break;

				case LOADARM_STATE.NO_EQUIPMENT_TO_LOAD_MSG:
					this.SetFinishedLoading();
					break;

				case LOADARM_STATE.NO_COMPARTMENTS_TO_LOAD_MSG:
					{
						if (stationManager.StationState == StationState.TRANSACTION_IN_PROGRESS)
						{
							this.IssueSelectPrompt(stationManager);
						}
						else
						{
							this.SetFinishedLoading();
						}

						break;
					}

				case LOADARM_STATE.FINISHED:
					this.SetFinishedLoading();
					break;

				case LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD:
					//this.IssueNoProductsToLoadMessage(true);
					this.SetFinishedLoading();
					break;

				case LOADARM_STATE.PRODUCT_UNAVAILABLE:
					this.IssueCompartmentPrompt(stationManager);
					break;

				case LOADARM_STATE.END_BATCH_PROMPT:
					this.IssueEndBatchPrompt();
					break;

				case LOADARM_STATE.SPLASH_BLEND_COMPONENT_PROMPT:
				case LOADARM_STATE.INVALID_SPLASH_BLEND_COMPONENT_SELECTION_MSG:
					this.IssueSplashBlendComponentSelectionPrompt();
					break;

				case LOADARM_STATE.SELECT_RECIPE_PROMPT:
				case LOADARM_STATE.INVALID_RECIPE_SELECTION_MSG:
					this.IssueSelectRecipePrompt(stationManager, this.MaximumPreset);
					break;

				case LOADARM_STATE.PRESET_VOLUME_PROMPT:
				case LOADARM_STATE.INVALID_PRESET_MSG:
					this.IssuePresetPrompt(stationManager, this.MaximumPreset);
					break;

				case LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT:
				case LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT:
					this.IssuePermissiveMessage(stationManager);
					break;

				case LOADARM_STATE.COMPARTMENT_LOADED_PROMPT:
				case LOADARM_STATE.COMPARTMENT_NOT_ON_ORDER_MSG:
					this.IssueCompartmentPrompt(stationManager);
					break;

				case LOADARM_STATE.MAXIMUM_PRESET_LESS_THAN_OR_EQUAL_ZERO:
					if (stationManager.SiteManager.Site.PromptForCompartment)
					{
						this.IssueCompartmentPrompt(stationManager);
					}
					else
					{
						this.IssueNonPreloadEquipmentPrompt(stationManager);
					}

					break;

				default:
					return false;
			}

			return true;
		}

		protected bool ProcessResponseData(StationManagerClass stationManager, string response)
		{
			if (response.Length > 0 && this.RequestWillBeProcessed())
			{
				stationManager.WriteLogDataToCommFile(response, StationManagerClass.CommLogDirection.In);
			}

			switch (this.LoadArmState)
			{
				case LOADARM_STATE.BATCH_COMPLETE_PROMPT:
					this.ProcessBatchCompleteResponse(stationManager, response);
					break;

				case LOADARM_STATE.SELECT_PROMPT:
					this.ProcessSelectResponse(stationManager, response);
					break;

				case LOADARM_STATE.COMPARTMENT_PROMPT:
					this.ProcessCompartmentResponse(stationManager, response);
					break;

				case LOADARM_STATE.EQUIPMENTID_PROMPT:
					this.ProcessEquipmentResponse(stationManager, response);
					break;

				case LOADARM_STATE.NON_PRELOAD_EQUIPMENT_PROMPT:
					this.ProcessNonPreloadEquipmentResponse(stationManager, response);
					break;

				case LOADARM_STATE.SPLASH_BLEND_COMPONENT_PROMPT:
					this.ProcessSplashBlendComponentPromptResponse(stationManager, response);
					break;

				case LOADARM_STATE.END_BATCH_PROMPT:
					this.ProcessEndBatchPromptResponse(stationManager, response);
					break;

				case LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT:
				case LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT:
					this.ProcessPermissiveMessageAcknowledge(stationManager, response);
					break;

				case LOADARM_STATE.SELECT_RECIPE_PROMPT:
					this.ProcessSelectRecipeResponse(stationManager, response);
					break;

				case LOADARM_STATE.PRESET_VOLUME_PROMPT:
					this.ProcessPresetResponse(stationManager, response);
					break;

				case LOADARM_STATE.BATCH_STOPPED_PROMPT:
					this.IssueEndBatchPrompt();
					break;

				case LOADARM_STATE.TRANSATION_COMPLETION_PROMPT:
					this.ProcessTransactionCompletionResponse(stationManager, response);
					break;

				case LOADARM_STATE.COMPARTMENT_LOADED_PROMPT:
					this.ProcessCompartmentLoadedResponse(stationManager, response);
					break;

				default:
					return false;
			}

			return true;
		}

		protected bool RequestWillBeProcessed()
		{
			switch (this.LoadArmState)
			{
				case LOADARM_STATE.BATCH_COMPLETE_PROMPT:
				case LOADARM_STATE.SELECT_PROMPT:
				case LOADARM_STATE.COMPARTMENT_PROMPT:
				case LOADARM_STATE.EQUIPMENTID_PROMPT:
				case LOADARM_STATE.NON_PRELOAD_EQUIPMENT_PROMPT:
				case LOADARM_STATE.SPLASH_BLEND_COMPONENT_PROMPT:
				case LOADARM_STATE.END_BATCH_PROMPT:
				case LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT:
				case LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT:
				case LOADARM_STATE.SELECT_RECIPE_PROMPT:
				case LOADARM_STATE.PRESET_VOLUME_PROMPT:
				case LOADARM_STATE.BATCH_STOPPED_PROMPT:
				case LOADARM_STATE.TRANSATION_COMPLETION_PROMPT:
					break;

				default:
					return false;
			}

			return true;
		}

		public virtual bool Authorize(StationManagerClass stationManager, double preset)
		{
			return false;
		}

		protected virtual void ProcessNonPreloadEquipmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Get the value of the response
			int selection = -1;

			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = System.Convert.ToInt32(response);
				}
				catch
				{
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Process the selection
			this.ValidateNonPreloadEquipmentResponse(selection);
		}

		protected virtual void ProcessEquipmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Get the value of the response
			int selection = -1;
			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = System.Convert.ToInt32(response);
				}
				catch
				{
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Check the validity of the selection
			if (selection < 0)
			{
				this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
				this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
				return;
			}

			// Process selection
			this.ValidateEquipmentResponse(stationManager, selection);
		}

		protected virtual void ProcessCompartmentLoadedResponse(StationManagerClass StationManager, string Response)
		{
			if (Response == StationManagerClass.EscapeString)
			{
				IssueCompartmentPrompt(StationManager);
				return;
			}

			// Get the value of the response
			int nSelection = -1;
			if (Response != null && Response != string.Empty)
			{
				try
				{
					nSelection = System.Convert.ToInt32(Response);
				}
				catch
				{
					return;
				}
			}

			// Check the validity of the selection
			if (nSelection < 1
			|| nSelection > 2)
			{
				SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
				DisplayMessage("[LoadRack|Invalid Selection]", 0);
				return;
			}

			if (nSelection == 1)
			{
				AuthorizeNonPreloadBatch(StationManager);
			}
			else
			{
				IssueCompartmentPrompt(StationManager);
			}
		}

		protected virtual void ProcessCompartmentResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Get the value of the response
			int selection = -1;
			if (!string.IsNullOrEmpty(response))
			{
				try
				{
					selection = System.Convert.ToInt32(response);
				}
				catch
				{
					this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
					this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
					return;
				}
			}

			// Zero cancels
			if (selection == 0)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;

				// Entering 0 here should be treated the same as Clear/Escape 
				this.IssueSelectPrompt(stationManager);
				return;
			}

			// Check the validity of the selection
			if (selection < 0)
			{
				this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
				this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
				return;
			}

			// Process selection
			this.ValidateCompartmentResponse(stationManager, selection);
		}

		private void ProcessTransactionCompletionResponse(StationManagerClass stationManager, string response)
		{
			// Extract the first character of the response
			char cResponse = '\0';
			if (response.Length > 0)
				cResponse = response[0];

			if (CurrentMenuParameters == null)
			{
				throw new NullReferenceException("ProcessTransactionCompleteResponse:  Current Menu Parameters not set");
			}

			int menuOption = cResponse == '\0' ? -1 : (int)cResponse - (int)'1';
			string selectedMenuText;

			try
			{
				selectedMenuText = CurrentMenuParameters.Menu[menuOption];
			}
			catch (IndexOutOfRangeException)
			{
				selectedMenuText = string.Empty;
			}

			// What was the driver's selection?
			switch (selectedMenuText)
			{
				case "LoadRack|Set Complete":
					DoFinishedLoadingProcessing(stationManager, true, LOADARM_STATE.TRANSACTION_COMPLETION_LOADING_IN_PROGRESS_MSG);
					break;

				case "LoadRack|Leave InProgress":
					DoFinishedLoadingProcessing(stationManager, false, LOADARM_STATE.TRANSACTION_COMPLETION_LOADING_IN_PROGRESS_MSG);
					break;

				default:
					SetState(LOADARM_STATE.INVALID_TRANSACTION_COMPLETION_MSG);
					DisplayMessage("[LoadRack|Invalid Selection]", 0);
					break;
			}
		}

		private void ProcessSelectResponse(StationManagerClass stationManager, string response)
		{
			// Extract the first character of the response
			char cResponse = '\0';
			if (response.Length > 0)
			{
				cResponse = response[0];
			}

			if (this.CurrentMenuParameters == null)
			{
				throw new NullReferenceException("ProcessBatchCompleteResponse:  Current Menu Parameters not set");
			}

			int menuOption = cResponse == '\0' ? -1 : cResponse - '1';
			string selectedMenuText;

			try
			{
				selectedMenuText = this.CurrentMenuParameters.Menu[menuOption];
			}
			catch (IndexOutOfRangeException)
			{
				selectedMenuText = "";
			}

			// What was the driver's selection?
			switch (selectedMenuText)
			{
				case "LoadRack|Off Load New Batch":
				case "LoadRack|Load New Batch":
					this.PromptForNextBatch(stationManager, true);
					break;

				case "LoadRack|Finished Off Loading":
					this.DoFinishedLoadingProcessing(stationManager, true, LOADARM_STATE.SELECT_LOADING_IN_PROGRESS_MSG);
					break;

				case "LoadRack|Finished Loading":
					{
						if (stationManager.PromptForTransactionCompletion)
						{
							IssueTransactionCompletionPrompt();
						}
						else
						{
							DoFinishedLoadingProcessing(stationManager, true, LOADARM_STATE.SELECT_LOADING_IN_PROGRESS_MSG);
						}
						break;
					}

				default:
					this.SetState(LOADARM_STATE.INVALID_BATCH_SELECTION_MSG);
					this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
					break;
			}
		}

		protected virtual void DoFinishedLoadingProcessing(StationManagerClass stationManager, bool transactionComplete, LOADARM_STATE state)
		{
			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			if (!stationManager.FinishLoading(transactionComplete))
				IssueLoadingInProgressMessage(state);
		}

		private void ProcessBatchCompleteResponse(StationManagerClass stationManager, string Response)
		{
			// Extract the first character of the response
			char cResponse = '\0';
			if (Response.Length > 0)
			{
				cResponse = Response[0];
			}

			if (this.CurrentMenuParameters == null)
			{
				throw new NullReferenceException("ProcessBatchCompleteResponse:  Current Menu Parameters not set");
			}

			int menuOption = cResponse == '\0' ? -1 : cResponse - '1';
			string selectedMenuText;

			try
			{
				selectedMenuText = this.CurrentMenuParameters.Menu[menuOption];
			}
			catch (IndexOutOfRangeException)
			{
				selectedMenuText = "";
			}

			// What was the driver's selection?
			switch (selectedMenuText)
			{
				case "LoadRack|Load New Batch":
					{
						if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
						{
							this.SetState(LOADARM_STATE.NORMAL);
							if (stationManager.Station.OffLoadByOffLoadID ||
								 stationManager.UseOffLoadSupplyOrders == false)
							{
								stationManager.DisplayOffLoadProductSelect();
							}
							else
							{
								stationManager.DisplayVerifySupplyOrderProduct();
							}
						}
						else
						{
							this.PromptForNextBatch(stationManager, true);
						}

						break;
					}

				case "LoadRack|Finished Off Loading":
					DoFinishedLoadingProcessing(stationManager, true, LOADARM_STATE.BATCH_COMPLETE_LOADING_IN_PROGRESS_MSG);
					break;

				case "LoadRack|Finished Loading":
					if (stationManager.PromptForTransactionCompletion)
					{
						IssueTransactionCompletionPrompt();
					}
					else
					{
						DoFinishedLoadingProcessing(stationManager, true, LOADARM_STATE.BATCH_COMPLETE_LOADING_IN_PROGRESS_MSG);
					}
					break;

				case "LoadRack|View Last Status":
					{
						this.ReleaseKeyPad();

						// Delay to allow driver to look at the display
						Monitor.Exit(stationManager);
						Thread.Sleep(10000);
						Monitor.Enter(stationManager);
						this.IssueBatchCompletePrompt();
						break;
					}

				default:
					this.SetState(LOADARM_STATE.INVALID_BATCH_SELECTION_MSG);
					this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
					break;
			}
		}


		public void SetState(LOADARM_STATE NewState)
		{
			if (NewState == LOADARM_STATE.INPROGRESS
			|| this.LoadArmState == LOADARM_STATE.INPROGRESS)
			{
				this.SiteManager.PermissiveEvent.Set();
			}

			var logger = new Logger(StopWatch.Appnames.LoadRackService.ToString());
			logger.Debug("Arm " + this.GetArmNumber(this.GetStationManager()).ToString(CultureInfo.InvariantCulture) + ": changing state from " + this.LoadArmState.ToString() + " to " + NewState.ToString());
			logger.Debug("    Got here via:  " + Environment.StackTrace);
			LoadArmState = NewState;

			if (LoadArmState == LOADARM_STATE.NORMAL)
			{
				this.NonPreloadEquipmentSelection = string.Empty;
				this.NonPreloadCompartmentSelection = -1;
			}
		}

		public bool IsSplashBlendComplete
		{
			get
			{
				foreach (ProductMapClass Component in this.CurrentLineItemProduct.ComponentCollection)
				{
					bool Found = false;

					foreach (SubLineItemDO SubLineItem in this.CurrentLineItem.SubLineItems)
					{
						if (SubLineItem.ProductGuid != Guid.Empty
							 && SubLineItem.ProductGuid == Component.AssignedGuid)
						{
							Found = true;
							break;
						}
					}

					if (!Found)
					{
						return false;
					}
				}

				return true;
			}
		}

		public ArrayList GetSplashBlendComponentList()
		{
			ArrayList List = new ArrayList();

			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
			{
				return List;
			}

			foreach (ProductMapClass Component in this.CurrentLineItemProduct.ComponentCollection)
			{
				bool Found = false;

				foreach (SubLineItemDO SubLineItem in this.CurrentLineItem.SubLineItems)
				{
					if (SubLineItem.ProductGuid != Guid.Empty
						&& SubLineItem.ProductGuid == Component.AssignedGuid)
					{
						Found = true;
						break;
					}
				}

				// Skip Components that have been loaded
				if (Found)
				{
					continue;
				}

				foreach (ProductMapClass SplashProduct in this.Bay(StationManager).SplashProducts)
				{
					if (SplashProduct.AssignedGuid == Component.AssignedGuid)
					{
						Found = true;
						break;
					}
				}

				// Skip Components not available on this arm
				if (!Found)
				{
					continue;
				}

				// User Ship To Identifiers if configured
				foreach (ProductMapClass AuthorizedProduct in StationManager.ShipTo.AuthorizedProductCollection)
				{
					if (AuthorizedProduct.AssignedGuid == Component.AssignedGuid)
					{
						Component.ShipToProductID = AuthorizedProduct.ShipToProductID;
						Component.ShipToProductCode = AuthorizedProduct.ShipToProductCode;
						Component.ShipToLoadRackDisplayText = AuthorizedProduct.ShipToLoadRackDisplayText;
						break;
					}
				}

				List.Add(Component);
			}

			return List;
		}

		protected virtual void IssueSplashBlendComponentSelectionPrompt()
		{
			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = false,
				DefaultItem = 0,
				MenuTimeout = 999,

				Caption = "[LoadRack|Component] [LoadRack|0=Cancel]"
			};

			ArrayList ComponentList = this.GetSplashBlendComponentList();

			ArrayList Menu = new ArrayList();
			foreach (ProductMapClass Component in ComponentList)
			{
				Menu.Add(StationManagerClass.GetLoadRackDisplayText(Component));
			}

			Parameters.Menu = (string[])Menu.ToArray(typeof(string));
			this.SetState(LOADARM_STATE.SPLASH_BLEND_COMPONENT_PROMPT);
			this.DisplayMenu(Parameters);
		}

		protected virtual void ProcessSelectRecipeResponse(StationManagerClass StationManager, string Response)
		{
			foreach (ProductMapClass Recipe in this.AvailableRecipeCollection)
			{
				if (Recipe.AssignedID == Response)
				{
					this.CurrentRecipe = Recipe;
					break;
				}
			}

			this.IssuePresetPrompt(StationManager, this.MaximumPreset);
		}

		protected virtual void ProcessSplashBlendComponentPromptResponse(StationManagerClass stationManager, string response)
		{
			if (response == StationManagerClass.EscapeString)
			{
				this.IssueCompartmentPrompt(stationManager);
				return;
			}

			this.ValidateSplashBlendComponentResponse(stationManager, response);
		}

		public virtual void EndBatch()
		{
		}

		protected virtual bool IsFlowing()
		{
			return false;
		}

		protected virtual void ProcessEndBatchPromptResponse(StationManagerClass stationManager, string response)
		{
			if (response == "1")
			{
				try
				{
					this.EndBatch();

					LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);
					if (lineItem != null)
					{
						// update the data before we actually terminate
						stationManager.UpdateLineItem(lineItem);
						stationManager.CloseOutLineItem(lineItem);
						stationManager.SaveTransaction();
					}

					this.IssueSelectPrompt(stationManager);
					return;
				}
				catch (Exception e)
				{
					this.eventLog.WriteEntry("LoadArmManager : ProcessEndBatchPromptResonse " + e.Message, EventLogEntryType.Error);
				}
			}

			this.ReleaseKeyPad();
			this.SetState(LOADARM_STATE.INPROGRESS);
		}

		protected virtual void ValidateSplashBlendComponentResponse(StationManagerClass stationManager, string response)
		{
			ArrayList componentList = this.GetSplashBlendComponentList();
			foreach (ProductMapClass component in componentList)
			{
				if (StationManagerClass.GetLoadRackDisplayText(component) == response)
				{
					this.CurrentLineItem.SplashBlendingMap = component;
					stationManager.AuthorizeLoadArm(this, this.CurrentLineItem);
					return;
				}
			}

			this.IssueSplashBlendComponentInvalidMessage();
		}

		protected virtual void IssueEquipmentPrompt(StationManagerClass StationManager)
		{
			// Since equipment ID is a string, we have to prompt with a menu
			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = 999
			};

			ArrayList EquipmentList = this.GetPreloadEquipmentList(StationManager);
			Parameters.Menu = new string[EquipmentList.Count];

			Parameters.Caption = "[LoadRack|Equipment] [LoadRack|0=Cancel]";

			int nItem = 0;
			foreach (string EquipmentID in EquipmentList)
			{
				Parameters.Menu[nItem++] = EquipmentID;
			}

			this.SetState(LOADARM_STATE.EQUIPMENTID_PROMPT);
			this.DisplayMenu(Parameters);
		}

		public bool ShowNoProductsMessage()
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
			{
				return false;
			}

			if (this.Bay(StationManager).PreLoads.Count == 0)
			{
				this.IssueNoProductsToLoadMessage(true);
				return true;
			}

			return false;
		}

		protected void IssueLoadingInProgressMessage(LOADARM_STATE State)
		{
			this.SetState(State);
			this.DisplayMessage("LoadRack|Loading In Progress", 0, this.MessageTimeout);
		}

		protected string GetPermissiveMessage(StationManagerClass StationManager)
		{
			string Message = "";
			bool FailedPermissive = false;

			// Check Site Permissive
			foreach (ProcessVariableClass PV in StationManager.SiteManager.Site.ProcessVariableCollection)
			{
				if (PV.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV
				&& (!PV.IsQualityGood
				|| !(bool)PV.ServerValue))
				{
					Message = PV.MessageID;
					FailedPermissive = true;
					break;
				}
			}

			// Check Station Permissives
			if (!FailedPermissive)
			{
				foreach (ProcessVariableClass PV in StationManager.Station.StationPermissives.Inputs)
				{
					if (!PV.IsQualityGood
					|| !(bool)PV.ServerValue)
					{
						Message = PV.MessageID;
						FailedPermissive = true;
						break;
					}
				}
			}

			// Check Arm Permissives
			if (!FailedPermissive)
			{
				foreach (ProcessVariableClass PV in this.LoadArm.LoadArmPermissives.Inputs)
				{
					if (!PV.IsQualityGood
					|| !(bool)PV.ServerValue)
					{
						Message = PV.MessageID;
						FailedPermissive = true;
						break;
					}
				}
			}

			return Message;
		}

		public virtual void IssuePermissiveMessage(StationManagerClass stationManager)
		{
		}

		public virtual void ProcessPermissiveMessageAcknowledge(StationManagerClass stationManager, string response)
		{
		}

		protected void IssueFinishedMessage()
		{
			this.SetState(LOADARM_STATE.FINISHED);
			this.DisplayMessageWithAcknowledge("LoadRack|Finished");
		}

		protected void DisplayMessageWithAcknowledge(string message)
		{
			this.DisplayMessage(message + " " + "[LoadRack|Press Enter to Acknowledge]", 41, this.PromptTimeout);
		}

		public virtual void SetFinishedLoading()
		{
			try
			{
				this.DoFinishedLoadingProcessing();

				this.SetState(LOADARM_STATE.FINISHED);

				StationManagerClass stationManager = this.GetStationManager();
				if (stationManager == null)
				{
					return;
				}

				stationManager.EvaluateLoadArmStatus();

				if (stationManager.StationState != StationState.IDLE
					 && stationManager.StationState != StationState.ENTER_DRIVER_ID_PROMPT)
				{
					this.IssueSelectPrompt(stationManager);
				}
			}
			catch (Exception e)
			{
				this.eventLog.WriteEntry("LoadArmManager SetFinishedLoading : " + e.Message + "\n" + e.StackTrace, EventLogEntryType.Error);
				this.SetState(LOADARM_STATE.SELECT_PROMPT);
				this.DisplayMessage("LoadRack|Finished Loading Error", 0);
			}
		}

		protected virtual void DoFinishedLoadingProcessing()
		{
		}

		public virtual void IssueEndBatchPrompt()
		{
			DisplayMenuParameters parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = 999,
				Caption = "LoadRack|End Batch?",
				Menu = new string[2]
			};


			parameters.Menu[0] = "LoadRack|Yes";
			parameters.Menu[1] = "LoadRack|No";

			this.SetState(LOADARM_STATE.END_BATCH_PROMPT);
			this.DisplayMenu(parameters);
		}

		public virtual void IssueSelectPrompt(StationManagerClass StationManager)
		{
			// If we were authorized by remote control, do not prompt for batch complete
			StationManager.UpdatePermissives(false);

			if (StationManager.IsRemoteAuthorized
				 || StationManager.TransactionSupportsMultipleLineItems == false
				 || StationManager.InRecircMode)
			{
				this.SetFinishedLoading();
				return;
			}

			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PromptTimeout,

				Caption = "LoadRack|Select"
			};

			List<string> menuList = new List<string>();
			if (StationManager.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				menuList.Add("LoadRack|Off Load New Batch");
			}
			else
			{
				menuList.Add("LoadRack|Load New Batch");
			}

			if (this.SuppressLoadFinishedPrompt == false)
			{
				menuList.Add("LoadRack|Finished Loading");
			}
			Parameters.Menu = menuList.ToArray();

			this.SetState(LOADARM_STATE.SELECT_PROMPT);
			this.DisplayMenu(Parameters);
		}

		public virtual void IssueTransactionCompletionPrompt()
		{
			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PromptTimeout,

				Caption = "LoadRack|Transaction Completion"
			};

			List<string> menuList = new List<string>
			{
				"LoadRack|Set Complete",
				"LoadRack|Leave InProgress"
			};
			Parameters.Menu = menuList.ToArray();

			SetState(LOADARM_STATE.TRANSATION_COMPLETION_PROMPT);
			DisplayMenu(Parameters);
		}

		public virtual void IssueBatchCompletePrompt()
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				return;
			}

			stationManager.UpdatePermissives(false);

			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			// If we were authorized by remote control, do not prompt for batch complete
			if (stationManager.IsRemoteAuthorized)
			{
				this.SetFinishedLoading();
				return;
			}

			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = 999,

				Caption = "LoadRack|Batch Complete"
			};

			List<string> menuList = new List<string>
			{
				"LoadRack|Load New Batch"
			};
			if (this.SuppressLoadFinishedPrompt == false)
			{
				if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
				{
					menuList.Add("LoadRack|Finished Off Loading");
				}
				else
				{
					menuList.Add("LoadRack|Finished Loading");
				}
			}
			menuList.Add("LoadRack|View Last Status");
			Parameters.Menu = menuList.ToArray();

			this.SetState(LOADARM_STATE.BATCH_COMPLETE_PROMPT);
			this.DisplayMenu(Parameters);
		}

		public virtual bool EnablePreset(StationManagerClass stationManager, bool showNoProductsMessage)
		{
			return this.PromptForNextBatch(stationManager, showNoProductsMessage);
		}

		public bool PromptForNextBatch(StationManagerClass stationManager, bool showNoProductsMessage)
		{
			if (stationManager.PreloadInProgress)
			{
				// If there are no preload items this load arm can serve, do nothing
				if (this.ItemsLeftToLoad(stationManager))
				{
					// If there is only one arm and only one preload row left to load, we do
					// not need to prompt since we can safely guess what will be loaded next.
					if (stationManager.AvailableLoadArms == 1
					&& this.Bay(stationManager).PreLoads.Count == 1)
					{
						this.StartLine(stationManager, this.Bay(stationManager).PreLoads[0] as LineItemDO);
						return true;
					}

					// If there are compartments to use for prompting, prompt by compartment
					// Otherwise, prompt by product
					this.MessageRetries = 0;
					this.EquipmentID = string.Empty;

					if (this.NeedEquipmentPrompt(stationManager))
					{
						this.IssueEquipmentPrompt(stationManager);
						return true;
					}

					if (this.IssueCompartmentPrompt(stationManager))
					{
						return true;
					}

					if (this.CurrentLineItem == null)
					{
						this.Bay(stationManager).RecipeMap = 0;
						this.IssueNoCompartmentsLeftToLoadMessage();
						return false;
					}

					return true;
				}

				if (showNoProductsMessage)
				{
					this.IssueNoProductsToLoadMessage(false);
					return false;
				}

				return false;
			}

			this.NonPreloadEquipmentSelection = string.Empty;
			this.NonPreloadCompartmentSelection = -1;

			if (this.Bay(stationManager).RecipeMap == 0)
			{
				this.IssueNoProductsToLoadMessage(true);
				return false;
			}

			if ((stationManager.SiteManager.Site.PromptForTractorOrTanker
				 && stationManager.TractorOrTanker != null
				 && stationManager.TractorOrTanker.Type == EQUIPMENT_TYPE.TANKER_TYPE)
				|| stationManager.SiteManager.Site.PromptForFirstTrailer)
			{
				if (this.IssueNonPreloadEquipmentPrompt(stationManager))
				{
					return true;
				}

				if (string.IsNullOrEmpty(this.NonPreloadEquipmentSelection))
				{
					this.Bay(stationManager).RecipeMap = 0;
					this.IssueNoEquipmentLeftToLoadMessage();
					return false;
				}
			}

			if (stationManager.SiteManager.Site.PromptForCompartment)
			{
				if (this.IssueCompartmentPrompt(stationManager))
				{
					return true;
				}

				if (this.NonPreloadCompartmentSelection == -1)
				{
					this.Bay(stationManager).RecipeMap = 0;
					this.IssueNoCompartmentsLeftToLoadMessage();
					return false;
				}
			}

			return this.AuthorizeNonPreloadBatch(stationManager);
		}

		public virtual bool IssueSelectRecipePrompt(StationManagerClass stationManager, double maximumPreset)
		{
			return false;
		}

		public virtual bool IssuePresetPrompt(StationManagerClass StationManager, double MaximumPreset)
		{
			return false;
		}

		public virtual void ProcessPresetResponse(StationManagerClass StationManager, string Response)
		{
		}

		protected virtual void IssueNoProductsToLoadMessage(bool finished)
		{
			if (this.GetStationManager().Mode == StationManagerClass.OperatingMode.Unloading)
			{
				if (finished)
				{
					this.SetState(LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD);
					this.DisplayMessage("LoadRack|No Products to Offoad on Arm", 0, this.PromptTimeout);
				}
				else
				{
					this.SetState(LOADARM_STATE.NO_PRODUCTS_TO_LOAD);
					this.DisplayMessage("LoadRack|No Products to Offoad on Arm", 0);
				}
			}
			else
			{
				if (finished)
				{
					this.SetState(LOADARM_STATE.FINISHED_WITH_NO_PRODUCTS_TO_LOAD);
					this.DisplayMessage("LoadRack|No Products to Load on Arm", 0, this.PromptTimeout);
				}
				else
				{
					this.SetState(LOADARM_STATE.NO_PRODUCTS_TO_LOAD);
					this.DisplayMessage("LoadRack|No Products to Load on Arm", 0);
				}
			}
		}

		protected void IssueProductUnavailable(string productID)
		{
			this.SetState(LOADARM_STATE.PRODUCT_UNAVAILABLE);
			this.DisplayMessage("[LoadRack|Product]" + " " + productID + " " + "[LoadRack|Unavailable]", 0);
		}

		protected void SetEquipmentInfo(StationManagerClass StationManager)
		{
			CompartmentInfo Info = this.GetFirstUnloadedCompartment(StationManager);

			if (Info != null)
			{
				this.NonPreloadEquipmentSelection = Info.EquipmentID;
			}

		}


		protected void SetCompartmentInfo(StationManagerClass StationManager)
		{
			CompartmentInfo Info = this.GetFirstUnloadedCompartment(StationManager);

			if (Info != null)
			{
				Info.Loaded = true;
				this.NonPreloadCompartmentSelection = Info.CompartmentNumber;
			}
		}


		protected CompartmentInfo GetFirstUnloadedCompartment(StationManagerClass StationManager)
		{
			ArrayList CompartmentList = StationManager.CompartmentList;

			if (CompartmentList != null && CompartmentList.Count > 0)
			{
				foreach (CompartmentInfo Info in CompartmentList)
				{
					if (Info.Loaded == false)
					{
						return Info;
					}
				}
			}

			return null;
		}

		protected virtual bool IssueNonPreloadEquipmentPrompt(StationManagerClass stationManager)
		{
			EquipmentList.Clear();

			DisplayMenuParameters Parameters = new DisplayMenuParameters
			{
				ApplyDataDictionary = true,
				DefaultItem = 0,
				MenuTimeout = this.PromptTimeout
			};

			EquipmentClass[] equipmentArray = { stationManager.TractorOrTanker,
												stationManager.Trailer1,
												stationManager.Trailer2,
												stationManager.Trailer3};

			foreach (EquipmentClass equipmentItem in equipmentArray)
			{
				if (equipmentItem == null)
				{
					continue;
				}

				if (equipmentItem.Type == EQUIPMENT_TYPE.TRACTOR_TYPE)
				{
					continue;
				}

				if (!stationManager.IsScheduledOrder
				|| stationManager.IsEquipmentAvailableOnOrderForLoadArm(equipmentItem, this))
				{
					if (stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers)
					{
						EquipmentList.Add(equipmentItem.CompanyEquipmentID);
					}
					else
					{
						EquipmentList.Add(equipmentItem.ID);
					}
				}
			}

			if (EquipmentList.Count < 2)
			{
				if (EquipmentList.Count == 1)
				{
					this.NonPreloadEquipmentSelection = EquipmentList[0] as string;
				}

				return false;
			}

			int nMenuItems = EquipmentList.Count;

			Parameters.Menu = new string[nMenuItems];

			Parameters.Caption = "[LoadRack|Equipment] [LoadRack|0=Cancel]";

			// Menu Items
			int nIndex = 0;
			foreach (string ID in EquipmentList)
			{
				Parameters.Menu[nIndex++] = ID;
			}

			SetState(LOADARM_STATE.NON_PRELOAD_EQUIPMENT_PROMPT);
			DisplayMenu(Parameters);

			return true;
		}


		protected bool NeedEquipmentPrompt(StationManagerClass StationManager)
		{
			// If there is only one preload, we do not have to worry about prompting
			if (this.Bay(StationManager).PreLoads.Count == 1)
			{
				return false;
			}

			// If two compartment numbers match but their equipment differs, we will need to 
			// prompt for equipment first
			foreach (LineItemDO LineItem in this.Bay(StationManager).PreLoads)
			{
				// Only need to process the line if it is loadpending
				if (LineItem.Status == TransactionStatus.LoadPending)
				{
					// Ignore it if the compartment is not configured
					if (LineItem.DestinationCompartmentID != null && LineItem.DestinationCompartmentID != "")
					{
						// Compare it against the other line items - I realize this loop will end up 
						// comparing each item to itself, but the performance hit should not be a big
						// deal with the anticipated number of preload lineitems to compare.  It's easier
						// just to let the comparison happen rather than try to prevent it.
						foreach (LineItemDO CompareItem in this.Bay(StationManager).PreLoads)
						{
							if (this.CheckEquipmentAmbiguity(CompareItem, LineItem))
							{
								return true;
							}
						}
					}
				}
			}

			// If we get here, we do not need to prompt for equipment
			return false;
		}

		protected bool CheckEquipmentAmbiguity(LineItemDO compareItem, LineItemDO lineItem)
		{
			if (compareItem.Status != TransactionStatus.LoadPending)
			{
				return false;
			}

			// Ignore it if the compartment is not configured
			if (!string.IsNullOrEmpty(compareItem.DestinationCompartmentID))
			{
				// If the compartment ids match, check the equipment ids
				if (compareItem.DestinationCompartmentID == lineItem.DestinationCompartmentID)
				{
					// If the compartment numbers match but they are on different equipment ids
					// we need to ask which equipment they wish to load first
					if (compareItem.DestinationEQ.RegistrationID != lineItem.DestinationEQ.RegistrationID)
					{
						return true;
					}
				}
			}

			return false;
		}

		protected bool ItemsLeftToLoad(StationManagerClass stationManager)
		{
			foreach (LineItemDO lineItem in this.Bay(stationManager).PreLoads)
			{
				if (lineItem.Status == TransactionStatus.LoadPending)
				{
					return true;
				}

				// Take splash blending into account
				if (lineItem.Status == TransactionStatus.InProgress && lineItem.SplashBlendingMap != null)
				{
					// The line item is being fulfilled with splash blending and one of the components is in progress.
					// We need to allow for continued loading after it is complete.
					return true;
				}
			}

			return false;
		}

		protected void ValidateEquipmentResponse(StationManagerClass stationManager, int nSelection)
		{
			--nSelection;

			ArrayList EquipmentList = this.GetPreloadEquipmentList(stationManager);

			if (nSelection >= 0 && nSelection < EquipmentList.Count)
			{
				this.EquipmentID = EquipmentList[nSelection] as string;
				this.IssueCompartmentPrompt(stationManager);
			}
			else
			{
				// Issue invalid response message
				this.IssueEquipmentInvalidMessage();
			}
		}

		protected bool AuthorizeNonPreloadBatch(StationManagerClass stationManager)
		{
			ulong recipeMap = this.Bay(stationManager).RecipeMap;

			EngineeringUnit volumeUnit = (stationManager.CurrentTransactionAlias.VolumeUnits != 0) ? stationManager.CurrentTransactionAlias.VolumeUnits : this.SiteManager.Site.VolumeUnits;

			EquipmentClass equipment = null;

			var maxPreset = new SIDouble { Units = volumeUnit, SIValue = this.SiteManager.Site._MaximumLoadAmount.SIValue };

			if (!string.IsNullOrEmpty(this.NonPreloadEquipmentSelection))
			{
				equipment = stationManager.GetEquipmentClass(this.NonPreloadEquipmentSelection);
			}

			if (equipment != null)
			{
				if (stationManager.SiteManager.Site.PromptForCompartment)
				{
					EquipmentClass compartment = stationManager.GetCompartment(equipment, this.NonPreloadCompartmentSelection);

					if (maxPreset.SIValue > compartment.SISafeFill.SIValue)
					{
						maxPreset.SIValue = compartment.SISafeFill.SIValue;
					}

					if (stationManager.Transaction != null)
					{
						foreach (LineItemDO lineItem in stationManager.Transaction.LineItems)
						{
							if (lineItem.DestinationEQ.EquipmentGuid == equipment.MasterRecordGuid
							&& lineItem.DestinationCompartmentID == this.NonPreloadCompartmentSelection.ToString(CultureInfo.InvariantCulture))
							{
								if (stationManager.SiteManager.Site.LoadByNet)
								{
									maxPreset.Value -= lineItem.Quantity.Net;
								}
								else
								{
									maxPreset.Value -= lineItem.Quantity.Gross;
								}
							}
						}
					}

					if (stationManager.IsScheduledOrder)
					{
						ProductMapClass recipe = null;

						foreach (LineItemDO lineItem in stationManager.Order.LineItems)
						{
							if (lineItem.Status == TransactionStatus.Completed)
							{
								continue;
							}

							if (lineItem.DestinationEQ.EquipmentGuid == equipment.MasterRecordGuid
							&& this.NonPreloadCompartmentSelection.ToString(CultureInfo.InvariantCulture) == lineItem.DestinationCompartmentID)
							{
								recipe = this.GetRecipe(lineItem.ProductGuid);
								if (recipe == null)
								{
									this.IssueProductUnavailable(lineItem.Product);
									return false;
								}

								EngineeringUnit orderVolumeUnits = (stationManager.OrderTransactionAlias.VolumeUnits != 0) ? stationManager.OrderTransactionAlias.VolumeUnits : stationManager.SiteManager.Site.VolumeUnits;

								recipeMap = (ulong)0x1 << (this.GetRecipeNumber(recipe) - 1);

								if ((this.Bay(stationManager).RecipeMap & recipeMap) == 0)
								{
									this.IssueProductUnavailable(lineItem.Product);
									return false;
								}

								if (stationManager.SiteManager.Site.LoadByNet)
								{
									var netQuantity = new SIDouble { Units = orderVolumeUnits, Value = lineItem.Quantity.Net };

									if (maxPreset.Value > netQuantity.Value)
									{
										maxPreset.Value = netQuantity.Value;
									}
								}
								else
								{
									var grossQuantity = new SIDouble { Units = orderVolumeUnits, Value = lineItem.Quantity.Gross };

									if (maxPreset.Value > grossQuantity.Value)
									{
										maxPreset.Value = grossQuantity.Value;
									}
								}
							}
						}

						if (recipe == null)
						{
							this.IssueCompartmentNotOnOrderMessage(this.NonPreloadCompartmentSelection);
							return false;
						}
					}
				}
				else
				{
					// ReSharper disable once CompareOfFloatsByEqualityOperator
					if ((maxPreset.SIValue > equipment.SISafeFill.SIValue) && (equipment.SISafeFill.SIValue != 0.0))
					{
						maxPreset.SIValue = equipment.SISafeFill.SIValue;
					}
				}
			}

			var maxFromOrder = stationManager.GetMaximumFromSalesOrderOnly();
			if (maxPreset.Value > maxFromOrder)
			{
				maxPreset.Value = maxFromOrder;
			}

			if (maxPreset.Value <= 0)
			{
				this.IssueMaximumPresetLessOrEqualZero();
				return false;
			}

			return stationManager.AuthorizeLoadArm(this, null, maxPreset.Value, recipeMap);
		}

		protected void ValidateNonPreloadEquipmentResponse(int nSelection)
		{
			--nSelection;

			StationManagerClass StationManager = GetStationManager();
			if (StationManager == null)
				return;

			if (nSelection >= 0 && nSelection < EquipmentList.Count)
			{
				// Get and save the equipment
				NonPreloadEquipmentSelection = EquipmentList[nSelection].ToString();

				if (StationManager.SiteManager.Site.PromptForCompartment)
				{
					if (IssueCompartmentPrompt(StationManager))
					{
						return;
					}

					if (NonPreloadCompartmentSelection == -1)
					{
						IssueNoCompartmentsLeftToLoadMessage();
						return;
					}

					this.AuthorizeNonPreloadBatch(StationManager);
				}
				else
				{
					this.AuthorizeNonPreloadBatch(StationManager);
				}
			}
			else
			{
				IssueNonPreloadEquipmentInvalidMessage();
			}
		}

		protected ArrayList GetPreloadEquipmentList(StationManagerClass stationManager)
		{
			ArrayList equipmentList = new ArrayList();
			foreach (LineItemDO lineItem in this.Bay(stationManager).PreLoads)
			{
				if (lineItem.DestinationEQ != null)
				{
					if (stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers)
					{
						if (!string.IsNullOrEmpty(lineItem.DestinationEQ.CompanyEquipmentID))
						{
							this.AddToEquipmentListIfNecessary(equipmentList, lineItem.DestinationEQ.CompanyEquipmentID);
						}
					}
					else
					{
						if (!string.IsNullOrEmpty(lineItem.DestinationEQ.RegistrationID))
						{
							this.AddToEquipmentListIfNecessary(equipmentList, lineItem.DestinationEQ.RegistrationID);
						}
					}
				}
			}

			return equipmentList;
		}

		protected void AddToEquipmentListIfNecessary(ArrayList equipmentList, string item)
		{
			foreach (string id in equipmentList)
			{
				if (id == item)
				{
					return;
				}
			}

			equipmentList.Add(item);

		}


		protected CompartmentInfo GetCompartmentIfValid(string EquipmentID, int Compartment)
		{
			if (this.GetStationManager().CompartmentList != null)
			{
				foreach (CompartmentInfo Info in this.GetStationManager().CompartmentList)
				{
					if (EquipmentID == "" || Info.EquipmentID == EquipmentID)
					{
						if (Info.CompartmentNumber == Compartment)
						{
							return Info;
						}
					}
				}
			}

			return null;
		}

		protected void IssueCompartmentLoadedPrompt(int compartment)
		{
			this.SetState(LOADARM_STATE.COMPARTMENT_LOADED_PROMPT);
			this.DisplayMenu(
				 new DisplayMenuParameters(
					  "[LoadRack|Compartment]" + " " + compartment.ToString(CultureInfo.InvariantCulture) + " "
					  + "[LoadRack|Loaded, continue loading?]",
					  new[] { "LoadRack|Yes", "LoadRack|No" },
					  true,
					  -1,
					  this.PromptTimeout));
		}

		protected void ValidateNonPreloadCompartmentResponse(int selection)
		{
			StationManagerClass stationManager = this.GetStationManager();
			if (stationManager == null)
			{
				return;
			}

			EquipmentClass equipment = stationManager.GetEquipmentClass(this.NonPreloadEquipmentSelection);
			if (selection > equipment.CompartmentCollection.Count)
			{
				this.IssueCompartmentInvalidMessage();
				return;
			}

			foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
			{
				if (loadArmManager.LoadArm.IdentityGuid == this.LoadArm.IdentityGuid)
				{
					continue;
				}

				if (loadArmManager.GetStationManager() != stationManager)
				{
					continue;
				}

				if (loadArmManager.NonPreloadEquipmentSelection == this.NonPreloadEquipmentSelection
				&& loadArmManager.NonPreloadCompartmentSelection == selection)
				{
					this.IssueCompartmentAlreadyLoadingMessage();
					return;
				}
			}

			this.NonPreloadCompartmentSelection = selection;

			if (stationManager.CheckCompartmentLoaded(equipment, selection))
			{
				// temp removeal of splash blending capability
				this.IssueCompartmentAlreadyLoadingMessage();
				//// IssueCompartmentLoadedPrompt(Selection);
				return;
			}

			this.AuthorizeNonPreloadBatch(stationManager);
		}

		protected void ValidateCompartmentResponse(StationManagerClass stationManager, int selection)
		{
			if (!stationManager.PreloadInProgress)
			{
				this.ValidateNonPreloadCompartmentResponse(selection);
				return;
			}

			// Check to make sure the selection is valid in terms of the Preload values
			foreach (LineItemDO lineItem in this.Bay(stationManager).PreLoads)
			{
				int nCompartmentID;
				try
				{
					nCompartmentID = System.Convert.ToInt32(lineItem.DestinationCompartmentID);
				}
				catch
				{
					continue;
				}

				if (lineItem.Status == TransactionStatus.Completed)
				{
					continue;
				}

				if (this.EquipmentID == ""
				|| (!stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers
				&& this.EquipmentID == lineItem.DestinationEQ.RegistrationID)
				|| (stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers
				&& this.EquipmentID == lineItem.DestinationEQ.CompanyEquipmentID))
				{
					if (nCompartmentID == selection)
					{
						this.StartLine(stationManager, lineItem);
						return;

					}
				}
			}

			// If got here, the compartment entered was invalid
			this.IssueCompartmentInvalidMessage();

		}


		protected bool IsSplashBlend(LineItemDO LineItem)
		{
			StationManagerClass StationManager = this.GetStationManager();
			if (StationManager == null)
			{
				return false;
			}

			// If remote authorization is in progress, this will not be a splash blend load
			if (StationManager.RemoteAuthorized)
			{
				return false;
			}

			// If the product is not a blend product, it cannot be a splash blend
			if (LineItem.ProductType != ProductClass.ProductTypeID(ProductType.BlendProduct))
			{
				return false;
			}

			foreach (ProductMapClass Recipe in this.AvailableRecipeCollection)
			{
				if (LineItem.ProductGuid != Guid.Empty
				&& Recipe.AssignedGuid == LineItem.ProductGuid)
				{
					return false;
				}
			}

			// If we did not find the item in the recipe map, it is a splash blend
			return true;

		}


		protected void StartLine(StationManagerClass stationManager, LineItemDO lineItem)
		{
			bool SplashBlend = this.IsSplashBlend(lineItem);

			// Is this particular line item already in progress somewhere else?
			if (lineItem.Status != TransactionStatus.LoadPending)
			{
				// Do not remove the item from the list if this is a splash blend
				if (SplashBlend == false)
				{
					this.Bay(stationManager).PreLoads.Remove(lineItem);
				}

				this.IssueCompartmentAlreadyLoadingMessage();

				return;
			}

			this.CurrentLineItem = lineItem;

			if (this.CurrentLineItem == null
			|| this.CurrentLineItem.ProductGuid == Guid.Empty)
			{
				return;
			}

			this.CurrentLineItemProduct = FMChannelHelper.MakeCall<IProducts, ProductClass>(
																	 x =>
																	 x.Get(this.Security, this.CurrentLineItem.ProductGuid)
																);

			this.CurrentLineItem.ArmNumber = stationManager.Station.SwingArmPosition == "A" ? this.LoadArm.BayAArmNumber : this.LoadArm.BayBArmNumber;

			// Also set the sublineitems
			if (SplashBlend)
			{
				// Determine what splash blend component product we are loading
				ArrayList ComponentList = this.GetSplashBlendComponentList();
				if (ComponentList.Count == 1)
				{
					// If there is only one we can load from the load arm, set it and go on
					this.CurrentLineItem.SplashBlendingMap = ComponentList[0] as ProductMapClass;

					// Moved this authorize call to here from the end of the function to prevent
					// double authorization when we aren't doing splash blends
					stationManager.AuthorizeLoadArm(this, this.CurrentLineItem);
				}
				else
				{
					// We need to prompt for what component they want to load
					this.IssueSplashBlendComponentSelectionPrompt();
					return;
				}
			}
			else
			{
				// Authorize the batch
				stationManager.AuthorizeLoadArm(this, this.CurrentLineItem);

				ProcessVariableClass loadArmPv = this.LoadArm.ProcessVariableCollection[0];

				var server = new Opc.Da.Server(new OpcCom.Factory(), new URL(loadArmPv.URL));
				server.Connect(new ConnectData(null));

				// Add Blend Components
				stationManager.CreateBlendSubLineItems(lineItem, this.CurrentLineItemProduct, this, server);

				if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
				{
					ProductMapClass armComponent = this.GetComponent(this.CurrentLineItemProduct.MasterRecordGuid);
					if (armComponent == null)
					{
						throw new Exception("Component not found in LoadArm Configuration");
					}

					lineItem.MeterID = armComponent.MeterID;

					TankClass tank = this.SiteManager.GetTank(armComponent, stationManager.Manager);
					if (tank != null)
					{
						lineItem.StorageLocationID = tank.ID;
						lineItem.StorageLocationTankGuid = tank.IdentityGuid;
					}
				}

				stationManager.CreateAdditiveSubLineItems(lineItem, 0, server, this);

				server.Disconnect();
				server.Dispose();

				this.CurrentLineItem.Status = TransactionStatus.InProgress;

				foreach (SubLineItemDO SubLineItem in this.CurrentLineItem.SubLineItems)
				{
					SubLineItem.Status = TransactionStatus.InProgress;
				}
			}
		}

		public int DisplayMessage(string message, int responseLength)
		{
			return responseLength == 0
					 ? this.DisplayMessage(message, responseLength, this.MessageTimeout)
					 : this.DisplayMessage(message, responseLength, this.PromptTimeout);
		}

		public abstract int DisplayMessage(string message, int responseLength, int messageTimeout);

		public virtual void PromptForPIN(string Message, int ResponseLength, int MessageTimeout) {; }

		public abstract void DisplayMenu(DisplayMenuParameters parameters);

		protected virtual void IssueCompartmentAlreadyLoadingMessage()
		{
			this.SetState(LOADARM_STATE.COMPARTMENT_ALREADY_LOADED_MSG);
			this.DisplayMessage("[LoadRack|Compartment Already Loading]", 0);
		}

		protected virtual bool IssueCompartmentPrompt(StationManagerClass stationManager)
		{
			if (!stationManager.PreloadInProgress)
			{
				EquipmentClass equipment = stationManager.GetEquipmentClass(this.NonPreloadEquipmentSelection);

				if (equipment == null)
				{
					return false;
				}

				this.NonPreloadCompartmentSelection = -1;
				int compartmentsAvailable = 0;
				int availableCompartment = 0;
				for (int compartment = 1; compartment < equipment.CompartmentCollection.Count + 1; compartment++)
				{
					if (stationManager.CheckCompartmentAvailable(equipment, compartment))
					{
						compartmentsAvailable++;
						availableCompartment = compartment;
					}
				}

				if (compartmentsAvailable == 0)
				{
					return false;
				}

				if (compartmentsAvailable == 1)
				{
					int numberOfLoadArms = 0;

					foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
					{
						if (loadArmManager.LoadArm.IdentityGuid == this.LoadArm.IdentityGuid)
						{
							continue;
						}

						if (loadArmManager.GetStationManager() != stationManager)
						{
							continue;
						}

						if (loadArmManager.Bay(stationManager).RecipeMap == 0)
						{
							continue;
						}

						numberOfLoadArms++;
					}

					if (numberOfLoadArms == 0)
					{
						this.NonPreloadCompartmentSelection = availableCompartment;
						return false;
					}
				}
			}
			else
			{
				int compartmentsAvailable = 0;
				int numberOfLoadArms = 0;

				LineItemDO currentLineItem = null;

				foreach (LineItemDO lineItem in this.Bay(stationManager).PreLoads)
				{
					if (lineItem.Status == TransactionStatus.Completed)
					{
						continue;
					}

					if (this.EquipmentID == string.Empty
					|| (!stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers
					&& this.EquipmentID == lineItem.DestinationEQ.RegistrationID)
					|| (stationManager.SiteManager.Site.UseCompanyEquipmentIdentifiers
					&& this.EquipmentID == lineItem.DestinationEQ.CompanyEquipmentID))
					{
						compartmentsAvailable++;
						currentLineItem = lineItem;

						foreach (LoadArmManagerClass loadArmManager in stationManager.LoadArmManagerCollection)
						{
							if (loadArmManager.LoadArm.IdentityGuid == this.LoadArm.IdentityGuid)
							{
								continue;
							}

							foreach (LineItemDO preload in loadArmManager.Bay(stationManager).PreLoads)
							{
								if (preload.ProductGuid == lineItem.ProductGuid
									 && preload.DestinationEQ.EquipmentGuid == lineItem.DestinationEQ.EquipmentGuid
									 && preload.DestinationCompartmentID == lineItem.DestinationCompartmentID)
								{
									numberOfLoadArms++;
								}
							}
						}
					}
				}

				if (compartmentsAvailable == 0)
				{
					return false;
				}

				if (compartmentsAvailable == 1 && numberOfLoadArms == 0)
				{
					this.StartLine(stationManager, currentLineItem);
					return false;
				}
			}

			this.SetState(LOADARM_STATE.COMPARTMENT_PROMPT);
			this.DisplayMessage("[LoadRack|Enter Compartment] [LoadRack|0=Cancel]", 2, this.PromptTimeout);

			return true;
		}

		protected virtual void IssueNoEquipmentLeftToLoadMessage()
		{
			SetState(LOADARM_STATE.NO_EQUIPMENT_TO_LOAD_MSG);
			DisplayMessage("[LoadRack|No Equipment to Load]", 0);
		}

		protected virtual void IssueNoCompartmentsLeftToLoadMessage()
		{
			this.SetState(LOADARM_STATE.NO_COMPARTMENTS_TO_LOAD_MSG);
			this.DisplayMessage("[LoadRack|No Compartments to Load]", 0);
		}

		protected void IssueCompartmentNotOnOrderMessage(int compartment)
		{
			SetState(LOADARM_STATE.COMPARTMENT_NOT_ON_ORDER_MSG);
			DisplayMessage("[LoadRack|Compartment]" + " " + compartment.ToString() + " " + "[LoadRack|Not On Order]", 0);
		}

		protected virtual void IssueCompartmentInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_COMPARTMENT_SELECTION_MSG);
			this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
		}

		protected virtual void IssueEquipmentInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_EQUIPMENT_SELECTION_MSG);
			this.DisplayMessage("[LoadRack|Invalid Selection]", 0);
		}

		protected virtual void IssueSplashBlendComponentInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_SPLASH_BLEND_COMPONENT_SELECTION_MSG);
			this.DisplayMessage("LoadRack|Invalid Selection", 0);
		}

		protected virtual void IssueRecipeInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_RECIPE_SELECTION_MSG);
			this.DisplayMessage("LoadRack|Invalid Selection", 0);
		}

		protected virtual void IssuePresetInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_PRESET_MSG);
			this.DisplayMessage("LoadRack|Invalid Preset Value", 0);
		}

		protected virtual void IssueMaximumPresetLessOrEqualZero()
		{
			this.SetState(LOADARM_STATE.MAXIMUM_PRESET_LESS_THAN_OR_EQUAL_ZERO);
			this.DisplayMessage("LoadRack|Maximum Preset Less Or Equal Zero", 0);
		}

		protected virtual void IssueNonPreloadEquipmentInvalidMessage()
		{
			this.SetState(LOADARM_STATE.INVALID_NON_PRELOAD_EQUIPMENT_SELECTION_MSG);
			this.DisplayMessage("LoadRack|Invalid Selection", 0);
		}

		public virtual bool LoadingInProgress()
		{
			return this.LoadArmState == LOADARM_STATE.INPROGRESS
							|| this.LoadArmState == LOADARM_STATE.INPROGRESS_PERMISSIVE_PROMPT
							|| this.LoadArmState == LOADARM_STATE.END_BATCH_PROMPT
							|| this.LoadArmState == LOADARM_STATE.AUTHORIZED
							|| this.LoadArmState == LOADARM_STATE.AUTHORIZED_PERMISSIVE_PROMPT;
		}

		public abstract bool IsTransactionInProgress();


		public virtual void ResetPowerFailAlarm()
		{
		}

		public virtual void ResetCommunicationsFailAlarm()
		{
		}

		public virtual void SendEndTransaction()
		{
		}

		public virtual void SyncDateAndTime()
		{
		}

		public virtual string GetBatchNumber(StationManagerClass stationManager)
		{
			stationManager.CurrentBatchNumber++;

			return stationManager.CurrentBatchNumber.ToString();
		}


		public bool IsProductAvailabilityDetermined(StationManagerClass StationManager, ProductClass Product)
		{
			foreach (LineItemDO LineItem in this.Bay(StationManager).PreLoads)
			{
				if (LineItem.ProductGuid != Guid.Empty
				&& LineItem.ProductGuid == Product.IdentityGuid)
				{
					return true;
				}
			}

			return false;
		}

		public bool IsComponentAvailabilityDetermined(StationManagerClass StationManager, ProductMapClass Component)
		{
			foreach (ProductMapClass SplashProduct in this.Bay(StationManager).SplashProducts)
			{
				if (SplashProduct.AssignedGuid == Component.AssignedGuid)
				{
					return true;
				}
			}

			return false;
		}

		public void ModifyProcessVariableMessage(ApplicationStringClass Message)
		{
			this.LoadArm.LoadArmPermissives.ModifyProcessVariableMessage(Message);
			this.LoadArm.NoAdditivePermissives.ModifyProcessVariableMessage(Message);

			foreach (ProductMapClass Component in this.LoadArm.ComponentCollection)
			{
				Component.Permissives.ModifyProcessVariableMessage(Message);
			}

			foreach (ProductMapClass Additive in this.LoadArm.AdditiveInjectorCollection)
			{
				Additive.Permissives.ModifyProcessVariableMessage(Message);
			}

			foreach (ProductMapClass Recipe in this.AvailableRecipeCollection)
			{
				Recipe.Permissives.ModifyProcessVariableMessage(Message);
			}
		}

		public void PurgeProcessVariableMessage(Guid identityGuid)
		{
			this.LoadArm.LoadArmPermissives.PurgeProcessVariableMessage(identityGuid);
			this.LoadArm.NoAdditivePermissives.PurgeProcessVariableMessage(identityGuid);

			foreach (ProductMapClass component in this.LoadArm.ComponentCollection)
			{
				component.Permissives.PurgeProcessVariableMessage(identityGuid);
			}

			foreach (ProductMapClass additive in this.LoadArm.AdditiveInjectorCollection)
			{
				additive.Permissives.PurgeProcessVariableMessage(identityGuid);
			}

			foreach (ProductMapClass recipe in this.AvailableRecipeCollection)
			{
				recipe.Permissives.PurgeProcessVariableMessage(identityGuid);
			}
		}

		public virtual void IssueBatchStoppedPrompt(StationManagerClass stationManager)
		{
			this.DisplayMessage("LoadRack|No Available Volume, Batch Stopped" + " " + stationManager.AcknowledgementMessage, stationManager.AcknowledgementResponseLength, this.PromptTimeout);
			this.LoadArmState = LOADARM_STATE.BATCH_STOPPED_PROMPT;
		}

		public bool CheckAvailableVolume(StationManagerClass stationManager)
		{
			LineItemDO lineItem = stationManager.GetLineItem(this.LoadArm.IdentityGuid);
			if (lineItem == null
			|| lineItem.Status != TransactionStatus.InProgress)
			{
				return false;
			}

			PROCESS_VARIABLE_TYPE tankVariable;
			if (stationManager.Station.Type == STATION_TYPE.OFF_LOADING)
			{
				tankVariable = stationManager.SiteManager.Site.LoadByNet
					 ? PROCESS_VARIABLE_TYPE.REMAINING_NET_VOLUME_PV
					 : PROCESS_VARIABLE_TYPE.REMAINING_GROSS_VOLUME_PV;
			}
			else
			{
				tankVariable = stationManager.SiteManager.Site.LoadByNet
					 ? PROCESS_VARIABLE_TYPE.AVAILABLE_NET_VOLUME_PV
					 : PROCESS_VARIABLE_TYPE.AVAILABLE_GROSS_VOLUME_PV;
			}

			bool shutdown = false;

			if (lineItem.ProductType == ProductClass.ProductTypeID(ProductType.ComponentProduct))
			{
				if (lineItem.StorageLocationTankGuid != Guid.Empty)
				{
					TankClass tank = this.SiteManager.GetTank(lineItem.StorageLocationTankGuid);
					ProcessVariableClass availableVolume = tank.ProcessVariableCollection[tankVariable];
					if (availableVolume == null
					|| (!availableVolume.IsQualityGood && !stationManager.SiteManager.Site.UseLastKnownGoodTankData)
					|| !(availableVolume.SIValue is double testAvailableVolume)
					|| testAvailableVolume <= 0.0)
					{
						shutdown = true;
					}
				}
			}
			else
			{
				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					if (subLineItem.StorageLocationTankGuid != Guid.Empty)
					{
						TankClass tank = this.SiteManager.GetTank(subLineItem.StorageLocationTankGuid);
						ProcessVariableClass availableVolume = tank.ProcessVariableCollection[tankVariable];
						if (availableVolume == null
						|| (!availableVolume.IsQualityGood && !stationManager.SiteManager.Site.UseLastKnownGoodTankData)
						|| !(availableVolume.SIValue is double testAvailableVolume)
						|| testAvailableVolume <= 0.0)
						{
							shutdown = true;
							break;
						}
					}
				}
			}

			if (shutdown)
			{
				switch (this.LoadArmState)
				{
					case LOADARM_STATE.BATCH_STOPPED_PROMPT:
					case LOADARM_STATE.END_BATCH_PROMPT:
					case LOADARM_STATE.BATCH_COMPLETE:
						// do nothing
						break;
					default:
						this.Stop();
						this.IssueBatchStoppedPrompt(stationManager);
						break;
				}
			}

			return shutdown;
		}

		public void SetAvailableProductsCollection()
		{
			this.AvailableRecipeCollection.Clear();

			foreach (ProductMapClass recipe in this.LoadArm.ProductRecipeCollection)
			{
				if (recipe.AssignedProductType == ProductType.AdditiveProduct)
				{
					continue;
				}
				if (recipe.AssignedProductType == ProductType.ComponentProduct)
				{
					ProductMapClass loadArmComponent = this.LoadArm.ComponentCollection.Find(x => x.AssignedGuid == recipe.AssignedGuid);
					if (loadArmComponent == null)
					{
						continue;
					}

					this.AvailableRecipeCollection.Add(recipe);
				}
				else     // PRODUCT_TYPE.BLEND_PRODUCT or PRODUCT_TYPE.ADDITIZED_PRODUCT
				{
					// Only Blends with all Components available can be in the RecipeCollection
					var productMapCollection = this.EnumerateByAssignedToGuidAndType(this.Security, recipe.AssignedGuid, PRODUCT_MAP_TYPE.BLEND_COMPONENT_MAP);
					bool available = true;

					foreach (ProductMapClass component in productMapCollection)
					{
						var loadArmComponent = this.LoadArm.ComponentCollection.Find(x => x.AssignedGuid == component.AssignedGuid)
													  ?? this.LoadArm.ExternalComponentCollection.Find(x => x.AssignedGuid == component.AssignedGuid)
															  ?? this.LoadArm.FlowControlledAdditiveCollection.Find(x => x.AssignedGuid == component.AssignedGuid);

						if (loadArmComponent == null)
						{
							available = false;
						}
					}

					if (available)
					{
						this.AvailableRecipeCollection.Add(recipe);
					}
				}
			}

		}

		private ProductMapCollectionClass EnumerateByAssignedToGuidAndType(SecurityClass security, Guid guid, PRODUCT_MAP_TYPE productMapType)
		{
			return FMChannelHelper.MakeCall<IProductMaps, ProductMapCollectionClass>(
																	 x =>
																	 x.EnumerateByAssignedToGuidAndType(security, guid, productMapType)
																);
		}

		internal virtual bool IsOffloadProductServedByLoadArm(ProductClass product)
		{
			ProductMapClass armComponent = this.GetComponent(product.IdentityGuid);
			if (armComponent != null && armComponent.Type == PRODUCT_MAP_TYPE.PRESET_COMPONENT_TANK_MAP
				 && armComponent.Permissives.Permitted)
			{
				return true;
			}

			return false;
		}
		internal virtual ProductMapClass GetOffloadComponent(Guid identityGuid)
		{
			ProductMapClass component = this.LoadArm.ComponentCollection.Find(x => x.AssignedGuid == identityGuid);

			return component;
		}

		/// <summary>
		/// This function clears all products on the physical preset assigned to the arm,
		/// as determined by the RecipeMap.  It also removes the recipe from the specified stations 
		/// recipe map
		/// </summary>
		/// <param name="stationManager">StationManager to clear recipe mapping from</param>
		protected virtual void ClearArmProducts(StationManagerClass stationManager)
		{
			// do nothing
		}
	}
}
