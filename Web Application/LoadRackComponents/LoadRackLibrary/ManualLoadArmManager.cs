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

using System.Diagnostics;

using FMBusinessObjects.DataObjects;

namespace LoadRackLibrary
{
	/// <summary>
	/// Summary description for ManualLoadArmManager.
	/// </summary>
	public class ManualLoadArmManagerClass : LoadArmManagerClass
	{
		public ManualLoadArmManagerClass(
			EventLog EventLog,
			SiteManagerClass SiteManager,
			StationManagerClass StationManager,
			LoadArmClass LoadArm,
			SecurityClass Security)
			: base (EventLog, SiteManager, StationManager, LoadArm, Security)
		{
            /* Code commented out as it effectively does nothing here, but left in the source as it becomes important if functionality is added here
            if (LoadArm.Enabled == false)
            {
                // If the load arm is disabled, we don't want to set up any process variables for it.
                return;
            }
             * */
		}

		protected override void OnInvoke(ProcessVariableClass pv) {}
		
		public override bool LoadingInProgress()
		{
			return false;
		}


		public override void Start(){}
		public override void Stop(){}
		public override bool AllocateRecipes(ulong recipeMap, ulong extendedRecipeMap){return false;}
		public override void Unauthorize(){}

        /// <summary>
        /// Applies the additive profile to the preset device and updates the display name of the recipe
		/// 
		/// Does nothing
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
			AdditiveProfileClass	additiveProfile,
			int deviceRecipeNumber)
		{
			return false;
		}
		public override bool IsTransactionInProgress() { return false; }
		public override int DisplayMessage ( string message, int responseLength, int messageTimeout ){return 0;}
		public override void DisplayMenu ( DisplayMenuParameters parameters ){}
	}

}
