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
	public interface IReportConfigurationGroupProcessor
	{
		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Delete ( ReportConfigurationGroupSR sr );

		[OperationContract]
		ReportConfigurationGroupDO GetConfiguration ( ReportConfigurationGroupSR reportGroupSR );

		[OperationContract]
		ReportConfigurationGroupDO GetByName ( ReportConfigurationGroupSR reportGroupSR );

		[OperationContract]
		ReportConfigurationGroupListDO GetAll ( ReportConfigurationGroupSR reportGroupSR );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void Save ( ReportConfigurationGroupSR reportGroupSR );

		[OperationContract]
		[TransactionFlow ( TransactionFlowOption.Allowed )]
		void UpdateOrder ( ReportConfigurationGroupSR reportGroupSR );
	}
}
