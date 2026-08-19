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
	public interface IReportConfigurationDetailProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Delete ( ReportConfigurationDetailSR sr );

		[OperationContract]
		ReportConfigurationDetailDO GetConfiguration ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		ReportConfigurationDetailDO GetPrintType ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		ReportConfigurationDetailListDO GetAll ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		ReportConfigurationDetailListDO GetAllNonPrint ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		ReportConfigurationDetailListDO GetPrintAtEndOfMonth ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void UpdateOrder ( ReportConfigurationDetailSR rptDetailSR );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void CreateDefaultReportAssignments(SecurityClass security);
    }
}
