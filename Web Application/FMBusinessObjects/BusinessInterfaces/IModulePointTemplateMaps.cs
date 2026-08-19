namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IModulePointTemplateMaps
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void AddModuletoPointTemplateMaps( SecurityClass security, List<ModuleToPointTemplateMap> moduleToPointTemplateMaps);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, ModuleToPointTemplateMap moduleToPointTemplateMap);


		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void Purge( SecurityClass security, Guid moduleToPointTemplateMapGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		ModuleToPointTemplateMap Get(SecurityClass security, Guid pointGuid, Guid moduleToTemplateGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, ModuleToPointTemplateMap> EnumerateByTemplateGuid(SecurityClass security, Guid templateGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, ModuleToPointTemplateMap> EnumerateByPointGuid(SecurityClass security, Guid pointGuid);
	}
}

