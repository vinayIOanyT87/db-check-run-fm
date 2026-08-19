namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IModules
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Guid Add(SecurityClass security, Module module);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, Module module);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid moduleGuid);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void LogToAlarmAndEventLog(SecurityClass security, string message);


        [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Module Get(SecurityClass security, Guid moduleGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<Guid> EnumeratePointTemplatesByAnyModuleTypeNames(SecurityClass security, string[] moduleTypeNames);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		List<Guid> EnumeratePointTemplatesByAllModuleTypeNames(SecurityClass security, string[] moduleTypeNames);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, Module> EnumerateByPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, Module> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		Dictionary<Guid, Module> EnumerateForAddToPointTemplateGuid(SecurityClass security, Guid pointTemplateGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string ID);

      [OperationContract]
      String Import(SecurityClass security, Module module);

		//[OperationContract]
		//[TransactionFlow(TransactionFlowOption.Allowed)]
		//void Import(SecurityClass security, );
	}
}

