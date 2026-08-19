using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IBSMEVerifications
	{
		[OperationContract]
		string FindMatchShipment ( string shipmentNumber, SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		bool CheckOrUpdateSuspenseStatus ( string strNumber, string docNumber, SecurityClass security );

		[OperationContract]
		string GetNextDocumentSeqNumber ( string partialDocNumber, string aliasName, SecurityClass security );

		[OperationContract]
		bool IsShippingTransactionDuplicate ( TransactionDO transDO, SecurityClass security );

		[OperationContract]
		DataTable LoadSuspenseData ( SecurityClass security );

		[OperationContract]
		string GetShipmentDocumentNumber ( string shipment, SecurityClass security );

		[OperationContract]
		bool DuplicateDocNumber ( string docNumber, string aliasName, SecurityClass security );
	}
}
