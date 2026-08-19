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
	public interface IGoodsAndServices
	{
		[OperationContract]
		GoodsAndServicesTaxDOCollection GetAll(SecurityClass security);

		[OperationContract]
		GoodsAndServicesTaxDO GetByDate(SecurityClass security, DateTimeOffset dtDate);

		[OperationContract]
		GoodsAndServicesTaxDO GetByDateAndCompany(SecurityClass security, DateTimeOffset dtDate, Guid companyGuid);

		[OperationContract]
		List<TaxCompanyMapDO> GetGSTCompanies(GoodsAndServicesTaxDO gstDO, SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Remove(GoodsAndServicesTaxDO gstDO, SecurityClass security);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Save(GoodsAndServicesTaxDO gstDO,
						SecurityClass security,
						List<TaxCompanyMapDO> companyList,
						List<TaxCompanyMapDO> deletedCompanyList,
						List<TaxCompanyMapDO> completeCompanyList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(GoodsAndServicesTaxDO gstDO, SecurityClass security, List<TaxCompanyMapDO> companyList);
	}
}
