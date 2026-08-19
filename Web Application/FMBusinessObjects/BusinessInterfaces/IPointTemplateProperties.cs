
namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;


	[ServiceContract]
	public interface IPointTemplateProperties
	{
		[OperationContract]
		Dictionary<Guid, PointTemplateProperty> EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		PointTemplateProperty Get(SecurityClass security, Guid pointTemplatePropertyGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddProperties(SecurityClass security, List<PointTemplateProperty> pointTemplatePropertyList);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyPointTemplatePropertyValue(SecurityClass security, PointTemplateProperty pointTemplateProperty);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdatePointTemplateProperties(SecurityClass security, Guid pointTemplateGuid, Dictionary<Guid, PointTemplateProperty> propertyList);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid pointTemplatePropertyGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> propertyList);
	}
}
