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
	public interface ICurrencies
	{
		[OperationContract]
		CurrencyDOCollectionClass GetForSite ( SecurityClass security, Guid siteGuid );

		[OperationContract]
		CurrencyDOCollectionClass GetCurrencies ( SecurityClass security );

		[OperationContract]
		CurrencyUnitDOCollectionClass GetCurrencyUnits ( SecurityClass security );

		[OperationContract]
		CurrencyDO Get ( SecurityClass security, Guid currencyGuid );

		[OperationContract]
		CurrencyDO GetByUnitIndex ( SecurityClass security, int unitIndex );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Add ( SecurityClass security, CurrencyDO currency );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( SecurityClass security, CurrencyDO currency );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Remove( SecurityClass security, Guid currencyGuid );
	}
}
