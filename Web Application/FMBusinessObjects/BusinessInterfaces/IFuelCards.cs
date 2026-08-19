using System;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	using System.Data;

	[ServiceContract]
	public interface IFuelCards
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, FuelCardClass FuelCard );

		[OperationContract]
		FuelCardCollectionClass EnumerateFuelCards ( SecurityClass security, bool hideHiddenFuelCards = false );

		[OperationContract]
		FuelCardCollectionClass EnumerateFuelCardsByCompanyAndFilter ( SecurityClass security,
																				Guid managerGuid,
																				Guid ownerGuid,
																				Guid shipperGuid,
																				Guid billToGuid,
																				Guid shipToGuid, 
																				string filterList,
                                                                                bool hideHiddenFuelCards = false);

		[OperationContract]
        DataSet EnumerateFuelCardsForAutoComplete(SecurityClass security, bool hideHiddenFuelCards = false);

		[OperationContract]
		DataSet EnumerateFuelCardsForSummary(SecurityClass security,
											Guid managerGuid,
											Guid ownerGuid,
											Guid shipperGuid,
											Guid billToGuid,
											Guid shipToGuid,
                                            Guid fuelCardTypeApplicationStringGuid,
											string filterList,
											bool transientFlag,
                                            bool hideHiddenFuelCards = false);


		[OperationContract]
		FuelCardCollectionClass EnumerateFuelCardsByCompany ( SecurityClass security,
																		Guid managerGuid,
																		Guid ownerGuid,
																		Guid shipperGuid,
																		Guid billToGuid,
																		Guid shipToGuid);

		/// <summary>
		/// This method will return a list of Fuel Card objects along with the associated fuel card
		/// limit and equipment.  It is used for the entity export.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <returns>A collection of fuel cards.</returns>
		[OperationContract]
		FuelCardCollectionClass EnumerateFuelCardsForEntityExport(SecurityClass security);

        /// <summary>
        /// Enumerate all fuel cards not assigned to a fuel card limit owned or assigned to the current site.
        /// Optionally limit the fuel cards returned to those with an ID containing the provided searchFilter
        /// </summary>
        /// <param name="security">Contains security information</param>
        /// <param name="fuelCardLimitGuid">Fuel cards assigned to this limit will be returned.</param>
        /// <param name="searchFilter">If provided, limits the fuel cards returned to those containing the value provided in the ID field</param>
        /// <returns>All fuel cards not assigned to a fuel card limit owned or assigned to the current site.</returns>
        [OperationContract]
        FuelCardCollectionClass EnumerateNotAssignedToFuelCardLimit(SecurityClass security, Guid fuelCardLimitGuid, string searchFilter);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Import ( SecurityClass security, FuelCardClass fuelCard );

		[OperationContract]
		FuelCardClass Get(SecurityClass security, Guid identityGuid, bool GetExtendedInfo);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string FuelCardID);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, FuelCardClass FuelCard );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge(SecurityClass security, Guid fuelCardGuid);
	}
}
