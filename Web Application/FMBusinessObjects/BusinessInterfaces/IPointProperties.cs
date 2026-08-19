using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using System.Data;


namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IPointProperties
	{
		[OperationContract]
		PointProperty Get(SecurityClass security, Guid pointPropertyGuid);

		[OperationContract]
		Guid GetPointPropertyGuid(SecurityClass security, Guid pointGuid, string id);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointProperty>> EnumerateByPointList(SecurityClass security, List<Guid> pointGuidList);

		[OperationContract]
		Dictionary<Guid, PointProperty> EnumerateByPoint(SecurityClass security, Guid pointGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, Guid> AddPointProperties(SecurityClass security, List<PointProperty> properties);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyPointPropertyValue(SecurityClass security, PointProperty pointProperty, Boolean bypassUpdatePointRecordVersion, Boolean bypassIsPointInSystemUse);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyPointValues(SecurityClass security, List<PointValue> pointValues);
	}
}
