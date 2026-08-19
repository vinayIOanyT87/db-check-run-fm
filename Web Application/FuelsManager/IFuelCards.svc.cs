// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFuelCards.svc.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the FuelCards type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager
{
    using System;
    using System.Data;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;

    /// <summary>
    /// The fuel cards.
    /// </summary>
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FuelCards : IFuelCards
    {
        #region Public Methods and Operators

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="fuelCard">
        /// The fuel card.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid Add(SecurityClass security, FuelCardClass fuelCard)
        {
            try
            {
                return FMChannelHelper.MakeCall<IFuelCards, Guid>((x) => x.Add(security, fuelCard));
            }
            catch (FaultException)
            {
                throw;
            }
        }

        /// <summary>
        /// The enumerate fuel cards.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="hideHiddenFuelCards">If true, only fuel cards that are not marked as hidden will be returned</param>
        /// <returns>
        /// The <see cref="FuelCardCollectionClass"/>.
        /// </returns>
        public FuelCardCollectionClass EnumerateFuelCards(SecurityClass security, bool hideHiddenFuelCards = false)
        {
            return FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>((x) => x.EnumerateFuelCards(security, hideHiddenFuelCards));
        }

        /// <summary>
        /// Enumerate all fuel cards not assigned to a fuel card limit owned or assigned to the current site.
        /// Optionally limit the fuel cards returned to those with an ID containing the provided searchFilter
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Fuel cards assigned to this limit will be returned.</param>
        /// <param name="searchFilter">If provided, limits the fuel cards returned to those containing the value provided in the ID field</param>
        /// <returns>All fuel cards not assigned to a fuel card limit owned or assigned to the current site.</returns>
        public FuelCardCollectionClass EnumerateNotAssignedToFuelCardLimit(SecurityClass security, Guid fuelCardLimitGuid, string searchFilter)
        {
            return FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(fuelCards => fuelCards.EnumerateNotAssignedToFuelCardLimit(security, fuelCardLimitGuid, searchFilter));
        }

		/// <summary>
		/// Enumerate all fuel cards for the auto complete.
		/// </summary>
		/// <param name="security">Contains security information</param>
		/// <param name="hideHiddenFuelCards">If true, only fuel cards that are not marked as hidden will be returned</param>
		/// <returns>Dataset of fuel card records.</returns>
		public DataSet EnumerateFuelCardsForAutoComplete(SecurityClass security, bool hideHiddenFuelCards = false)
		{
			return FMChannelHelper.MakeCall<IFuelCards, DataSet>((x) => x.EnumerateFuelCardsForAutoComplete(security, hideHiddenFuelCards));
		}


        /// <summary>
        /// The enumerate fuel cards by company.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="managerGuid">
        /// The manager guid.
        /// </param>
        /// <param name="ownerGuid">
        /// The owner guid.
        /// </param>
        /// <param name="shipperGuid">
        /// The shipper guid.
        /// </param>
        /// <param name="billToGuid">
        /// The bill to guid.
        /// </param>
        /// <param name="shipToGuid">
        /// The ship to guid.
        /// </param>
        /// <returns>
        /// The <see cref="FuelCardCollectionClass"/>.
        /// </returns>
        public FuelCardCollectionClass EnumerateFuelCardsByCompany(
            SecurityClass security, 
            Guid managerGuid, 
            Guid ownerGuid, 
            Guid shipperGuid, 
            Guid billToGuid, 
            Guid shipToGuid)
        {
            return
                FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(
                    (x) =>
                    x.EnumerateFuelCardsByCompany(security, managerGuid, ownerGuid, shipperGuid, billToGuid, shipToGuid));
        }

        /// <summary>
        /// The enumerate fuel cards by company and filter.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="managerGuid">
        /// The manager guid.
        /// </param>
        /// <param name="ownerGuid">
        /// The owner guid.
        /// </param>
        /// <param name="shipperGuid">
        /// The shipper guid.
        /// </param>
        /// <param name="billToGuid">
        /// The bill to guid.
        /// </param>
        /// <param name="shipToGuid">
        /// The ship to guid.
        /// </param>
        /// <param name="filterList">
        /// The filter list.
        /// </param>
        /// <param name="hideHiddenFuelCards">If true, only fuel cards that are not marked as hidden will be returned</param>
        /// <returns>
        /// The <see cref="FuelCardCollectionClass"/>.
        /// </returns>
        public FuelCardCollectionClass EnumerateFuelCardsByCompanyAndFilter(
            SecurityClass security, 
            Guid managerGuid, 
            Guid ownerGuid, 
            Guid shipperGuid, 
            Guid billToGuid, 
            Guid shipToGuid, 
            string filterList, 
            bool hideHiddenFuelCards = false)
        {
            return
                FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(
                    (x) =>
                    x.EnumerateFuelCardsByCompanyAndFilter(
                        security, 
                        managerGuid, 
                        ownerGuid, 
                        shipperGuid, 
                        billToGuid, 
                        shipToGuid, 
                        filterList,
                        hideHiddenFuelCards: hideHiddenFuelCards));
        }

        /// <summary>
        /// The enumerate fuel cards by company and filter.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="managerGuid">
        /// The manager guid.
        /// </param>
        /// <param name="ownerGuid">
        /// The owner guid.
        /// </param>
        /// <param name="shipperGuid">
        /// The shipper guid.
        /// </param>
        /// <param name="billToGuid">
        /// The bill to guid.
        /// </param>
        /// <param name="shipToGuid">
        /// The ship to guid.
        /// </param>
        /// <param name="fuelCardTypeApplicationStringGuid">
        /// Identifies the fuel card type to search for. If this is Guid.empty, show all types.</param>
        /// <param name="filterList">
        /// The filter list.
        /// </param>
        /// <param name="transientFlag">
        /// The transient flag for filtering</param>
        /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
        /// <returns>
        /// The <see cref="FuelCardCollectionClass"/>.
        /// </returns>
        public DataSet EnumerateFuelCardsForSummary(
			SecurityClass security,
			Guid managerGuid,
			Guid ownerGuid,
			Guid shipperGuid,
			Guid billToGuid,
			Guid shipToGuid,
            Guid fuelCardTypeApplicationStringGuid,
			string filterList,
			bool transientFlag, 
            bool hideHiddenFuelCards = false)
		{
			return
				FMChannelHelper.MakeCall<IFuelCards, DataSet>(
															(x) =>
															x.EnumerateFuelCardsForSummary(
																security,
																managerGuid,
																ownerGuid,
																shipperGuid,
																billToGuid,
																shipToGuid,
                                                                fuelCardTypeApplicationStringGuid,
																filterList,
																transientFlag,
                                                                hideHiddenFuelCards: hideHiddenFuelCards));
		}

		/// <summary>
		/// This method will return a list of Fuel Card objects along with the associated fuel card
		/// limit and equipment.  It is used for the entity export.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>A collection of fuel cards.</returns>
		public FuelCardCollectionClass EnumerateFuelCardsForEntityExport(SecurityClass security)
		{
			return FMChannelHelper.MakeCall<IFuelCards, FuelCardCollectionClass>(x => x.EnumerateFuelCardsForEntityExport(security));
		}

        /// <summary>
        /// The get.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="identityGuid">
        /// The identity guid.
        /// </param>
        /// <param name="getExtendedInfo">
        /// The get extended info.
        /// </param>
        /// <returns>
        /// The <see cref="FuelCardClass"/>.
        /// </returns>
        public FuelCardClass Get(SecurityClass security, Guid identityGuid, bool getExtendedInfo)
        {
            return
                FMChannelHelper.MakeCall<IFuelCards, FuelCardClass>(
                    (x) => x.Get(security, identityGuid, getExtendedInfo));
        }

        /// <summary>
        /// The get identity guid.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="fuelCardID">
        /// The fuel card id.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public Guid GetIdentityGuid(SecurityClass security, string fuelCardID)
        {
            return FMChannelHelper.MakeCall<IFuelCards, Guid>((x) => x.GetIdentityGuid(security, fuelCardID));
        }

        /// <summary>
        /// The import.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="fuelCard">
        /// The fuel card.
        /// </param>
        public void Import(SecurityClass security, FuelCardClass fuelCard)
        {
            FMChannelHelper.MakeCall<IFuelCards>((x) => x.Import(security, fuelCard));
        }

        /// <summary>
        /// The modify.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="fuelCard">
        /// The fuel card.
        /// </param>
        public void Modify(SecurityClass security, FuelCardClass fuelCard)
        {
            FMChannelHelper.MakeCall<IFuelCards>((x) => x.Modify(security, fuelCard));
        }

        /// <summary>
        /// The purge.
        /// </summary>
        /// <param name="security">
        /// The security.
        /// </param>
        /// <param name="fuelCardGuid">
        /// The fuel card guid.
        /// </param>
        public void Purge(SecurityClass security, Guid fuelCardGuid)
        {
            FMChannelHelper.MakeCall<IFuelCards>((x) => x.Purge(security, fuelCardGuid));
        }

        #endregion
    }
}