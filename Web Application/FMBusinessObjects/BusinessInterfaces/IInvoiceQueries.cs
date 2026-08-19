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
	public interface IInvoiceQueries
	{
		[OperationContract]
		InvoiceQueryClass GetByIdentityGuid ( SecurityClass security, Guid invoiceQueryGuid );

		[OperationContract]
		InvoiceQueryCollectionClass Enumerate ( SecurityClass security );

		[OperationContract]
		InvoiceQueryCollectionClass EnumerateByKeyword ( SecurityClass security, string keyword );
	}
}
