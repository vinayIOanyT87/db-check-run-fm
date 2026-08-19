namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;
	using FMBusinessObjects.DataObjects;
	using System.Data;

	[ServiceContract]
	public interface IApplicationStrings
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, ApplicationStringClass applicationString );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, ApplicationStringClass applicationString );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid applicationStringGuid );

		[OperationContract]
		ApplicationStringClass Get ( SecurityClass security, Guid guid );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, STRING_TYPE type, string inString );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyCollection(SecurityClass security, Guid siteGuid, STRING_TYPE type, ApplicationStringCollectionClass newApplicationStringCollection, ApplicationStringCollectionClass existingApplicationStringCollection);

		[OperationContract]
		ApplicationStringCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		ApplicationStringCollectionClass EnumerateByType ( SecurityClass security, STRING_TYPE type );

		[OperationContract]
		ApplicationStringCollectionClass EnumerateByTypeAndSite(SecurityClass security, STRING_TYPE type, Guid? siteGuid);

		[OperationContract]
		Dictionary<Guid, ApplicationStringClass> EnumerateByApplicationStringGuids(SecurityClass security, List<Guid> applicationStringGuidList);
		[OperationContract]
		void Import(SecurityClass security, ApplicationStringClass ApplicationString);

		[OperationContract]
		DataSet EnumerateAllCompanyTypes(SecurityClass security);
	}
}
