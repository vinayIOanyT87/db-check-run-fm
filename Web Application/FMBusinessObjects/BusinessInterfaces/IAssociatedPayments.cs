using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IAssociatedPayments
	{
		[OperationContract]
		void GetPaymentListByFindString ( SecurityClass security, string findStr, DataSet dataSet );

		[OperationContract]
		void GetPaymentList ( SecurityClass security, DataSet dataSet );
	}
}
