using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IStandingOffers
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, StandingOfferClass standingOffer);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, StandingOfferClass standingOffer);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid standingOfferGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ImportWithXML(SecurityClass security, string Xml);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ImportWithStandingOffer(SecurityClass security, StandingOfferClass standingOffer);

		[OperationContract]
		StandingOfferClass Get(SecurityClass security, Guid standingOfferGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string standingOfferID);

		[OperationContract]
		StandingOfferClass GetUsingProduct(SecurityClass security, Guid productGuid, DateTimeOffset currentPeriod);

		[OperationContract]
		Guid GetIdentityGuidUsingProduct(SecurityClass security, Guid supplierGuid, Guid productGuid);

		[OperationContract]
		Guid GetIdentityGuidUsingLocation(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid);

		[OperationContract]
		Guid GetIdentityGuidUsingPeriod(SecurityClass security, Guid supplierGuid, Guid productGuid, DateTimeOffset currentPeriod);

		[OperationContract]
		Guid GetIdentityGuidUsingMostRecent(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid,
															DateTimeOffset currentPeriod, double? quantity, bool mostRecent);

		[OperationContract]
		Guid GetIdentityGuidUsingQuantity(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid,
														DateTimeOffset currentPeriod, double? quantity);

		[OperationContract]
		Guid GetIdentityGuidUsingLocationPeriod(SecurityClass security, Guid supplierGuid, Guid productGuid,
																Guid locationGuid, DateTimeOffset currentPeriod);

		[OperationContract]
		StandingOfferClass GetByID(SecurityClass security, string standingOfferID);

		[OperationContract]
		bool IsStandingOfferOverlapping(SecurityClass security, StandingOfferClass standingOffer);

		[OperationContract]
		StandingOfferCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		StandingOfferCollectionClass EnumerateWithFilter(SecurityClass security, StandingOfferFilterClass filter);

		[OperationContract]
		string BuildIDUsingGuids(SecurityClass security, Guid supplierGuid, Guid productGuid, Guid locationGuid,
											DateTimeOffset effectiveDate, DateTimeOffset expirationDate, int lowerBound, int upperBound);

		[OperationContract]
		string BuildIDUsingIDs(SecurityClass security, string supplierID, string productID, string locationID,
										DateTimeOffset effectiveDate, DateTimeOffset expirationDate, int lowerBound, int upperBound);
	}
}
