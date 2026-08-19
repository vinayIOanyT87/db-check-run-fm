using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IQualifications
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( SecurityClass security, QualificationClass Qualification );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Modify ( SecurityClass security, QualificationClass Qualification );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Purge ( SecurityClass security, Guid targetGuid);

		[OperationContract]
		QualificationClass Get ( SecurityClass security, Guid targetGiud );

		[OperationContract]
		Guid GetIdentityGuid ( SecurityClass security, QUALIFICATION_TYPE type, string ID );

		[OperationContract]
		QualificationCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		QualificationCollectionClass EnumerateByType ( SecurityClass security, QUALIFICATION_TYPE type );
	}
}
