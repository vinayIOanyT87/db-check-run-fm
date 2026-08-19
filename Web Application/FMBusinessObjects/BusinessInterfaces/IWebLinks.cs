namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IWebLinks
	{
		[OperationContract]
		WebLinkCollectionClass Enumerate(SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, WebLink webLink);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, WebLink webLink);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid webLinkGuid);

		[OperationContract]
		WebLink Get(SecurityClass security, Guid webLinkGuid);

		[OperationContract]
		WebLink GetByName(SecurityClass security, string linkName);
	}
}
