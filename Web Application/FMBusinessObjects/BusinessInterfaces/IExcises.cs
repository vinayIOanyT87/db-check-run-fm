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
	public interface IExcises
	{
		[OperationContract]
		ExciseTaxDOCollection GetAll(SecurityClass security);

		[OperationContract]
		ExciseTaxDOCollection GetForProductAndCode(string productId, string exciseCode, SecurityClass security);

		[OperationContract]
		ExciseTaxDOCollection GetForProduct(string productId, SecurityClass security);

		[OperationContract]
		ExciseTaxDO GetForProductAndDate(Guid productGuid, DateTimeOffset dtDate, SecurityClass security);

		[OperationContract]
		ExciseTaxDO GetForProductCompanyAndDate(Guid productGuid, DateTimeOffset dtDate, Guid companyGuid, SecurityClass security);

		[OperationContract]
		ExciseTaxDOCollection GetForProductAndDateRange(string productId, DateTimeOffset dtStart, DateTimeOffset dtEnd, SecurityClass security);

		[OperationContract]
		DataTable GetExciseCodes(SecurityClass security);

		[OperationContract]
		List<TaxCompanyMapDO> GetExciseCompanies(ExciseTaxDO exciseDO, SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Remove(ExciseTaxDO excise, SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(ExciseTaxDO excise, SecurityClass security, List<TaxCompanyMapDO> companyList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Save(ExciseTaxDO excise, SecurityClass security, List<TaxCompanyMapDO> companyList, List<TaxCompanyMapDO> deletedCompanyList);
	}
}
