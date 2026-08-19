using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IFootNotes
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, FootNoteClass footNote );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, FootNoteClass footNote );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid identityGuid );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		FootNoteClass Get(SecurityClass security, Guid identityGuid);

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid GetIdentityGuid ( SecurityClass security, string id );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		FootNoteCollectionClass Enumerate ( SecurityClass security );
	}
}
