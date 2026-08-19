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
	public interface IMarkups
	{
		[OperationContract]
		MarkupDOCollection GetAll ( SecurityClass security );

		[OperationContract]
		List<TaxCompanyMapDO> GetMarkupCompanies ( MarkupDO markupDO, SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Remove ( MarkupDO markupDO, SecurityClass security );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( MarkupDO markupDO, SecurityClass security, List<TaxCompanyMapDO> companyList, List<TaxCompanyMapDO> deletedCompanyList );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		Guid Add ( MarkupDO markupDO, SecurityClass security, List<TaxCompanyMapDO> companyList );
	}
}
