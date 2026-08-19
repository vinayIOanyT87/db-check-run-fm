/******************************************************************************

	FILE NAME:		ManualLoadArmManager.cs


	PURPOSE:			ManualLoadArmManagerClass


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

namespace LoadRackLibrary
{
	using System.Diagnostics;
	using System.Threading;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Shell of a load arm for Varec DET.  This is a phantom load arm; all overrides should be do-nothing stubs.
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class VarecDETLoadArmManagerClass : LoadArmManagerClass
	{
		public VarecDETLoadArmManagerClass(
		 EventLog eventLog,
		 SiteManagerClass siteManager,
		 StationManagerClass stationManager,
		 LoadArmClass loadArm,
		 SecurityClass security)
			 : base(eventLog, siteManager, stationManager, loadArm, security)
		{
		}

		internal ProductMapClass CurrentLineItemRecipe
		{
			get
			{
				return this.CurrentRecipe;
			}

			set
			{
				this.CurrentRecipe = value;
			}
		}

		public override bool LoadingInProgress()
		{
			return false;
		}

		public override void Start()
		{
		}

		public override void Stop()
		{
		}

		public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap)
		{
			return false;
		}

		public override void Unauthorize()
		{
		}

        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Does nothing as DET does not support additives
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
        /// false
        /// </returns>
        public override bool UpdateRecipe(
		 string name,
		 ProductMapClass recipe,
		 ProductClass product,
		 AdditiveProfileClass additiveProfile,
		 int deviceRecipeNumber)
		{
			return false;
		}

		public override bool IsTransactionInProgress()
		{
			return false;
		}

		public override int DisplayMessage(string stockMessage, int responseLength, int messageTimeout)
		{
			return 0;
		}

		public override void DisplayMenu(DisplayMenuParameters parameters)
		{
			this.CurrentMenuParameters = parameters;
		}

		public void UpdateOffloadProductPermissives(ProductMapClass productMap, bool authorized)
		{
			if (!authorized)
			{
				foreach (ProductMapClass offloadExternalProduct in LoadArm.OffloadExternalProductCollection)
				{
					offloadExternalProduct.Permissives.Enabled = false;
				}
			}
			else
			{
				if (productMap == null)
				{
					foreach (ProductMapClass offloadExternalProduct in LoadArm.OffloadExternalProductCollection)
					{
						offloadExternalProduct.Permissives.Enabled = false;
					}
				}
				else
				{
					foreach (ProductMapClass offloadExternalProduct in LoadArm.OffloadExternalProductCollection)
					{
						offloadExternalProduct.Permissives.Enabled = offloadExternalProduct.AssignedGuid == productMap.AssignedGuid;
					}
				}
			}

			this.OpcServerManager.Update(true);
		}

		protected override void OnInvoke(ProcessVariableClass pv)
		{
		}

		internal override bool IsOffloadProductServedByLoadArm(ProductClass product)
		{
			ProductMapClass armComponent = this.GetOffloadComponent(product.IdentityGuid);
			if (armComponent != null && armComponent.Type == PRODUCT_MAP_TYPE.OFFLOAD_EXTERNAL_METER_MAP
				 && armComponent.Permissives.Permitted)
			{
				return true;
			}

			return false;
		}

		internal override ProductMapClass GetOffloadComponent(System.Guid identityGuid)
		{
			ProductMapClass component = this.LoadArm.OffloadExternalProductCollection.Find(x => x.AssignedGuid == identityGuid);

			return component;
		}

		internal string RetrievePermissiveMessage(StationManagerClass stationManager)
		{
			//// Need to force reads of the permissives before determining permissive message to get.
			//// Hit the site permissives
			//foreach (ProcessVariableClass processVariable in stationManager.SiteManager.Site.ProcessVariableCollection) 
			//{
			//   if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_PERMISSIVE_PV)
			//   {
			//      OpcServerManager.Read(processVariable);
			//   }
			//}

			//foreach (ProcessVariableClass processVariable in stationManager.Station.StationPermissives.Inputs)
			//{
			//   OpcServerManager.Read(processVariable);
			//}
			this.OpcServerManager.Update(false);
			Thread.Sleep(0); // release this timeslice so the OPC Server Manager gets a turn to process prior to the next call

			return GetPermissiveMessage(stationManager);
		}
	}
}
