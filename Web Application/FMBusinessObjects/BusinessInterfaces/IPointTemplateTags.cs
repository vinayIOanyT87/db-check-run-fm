namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointTemplateTags
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void AddTags( SecurityClass security, Dictionary<Guid, PointTemplateTag> tags );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void AddModuleTags(SecurityClass security, List<PointTemplateTag> tags);


		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void PurgeAll( SecurityClass security, Guid pointTemplateGuid );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Purge(SecurityClass security, Guid pointTemplateTagGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void PurgeByPointTemplateGuidAndNotInList(SecurityClass security, Guid pointTemplateGuid, List<Guid> tagList);


		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyTag(SecurityClass security, PointTemplateTag tag, bool deviceAlarmMappTag);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void UpdatePointTemplateTags(SecurityClass security, Guid pointTemplateGuid, Dictionary< Guid, PointTemplateTag> tagList);

		[OperationContract]
		Dictionary<Guid, PointTemplateTag> EnumerateByPointTemplateGuid( SecurityClass security, Guid pointTemplateGuid );

		[OperationContract]
		List<KeyValuePair<string, string>> EnumerateAllUniqueTagNames(SecurityClass security);

		[OperationContract]
		PointTemplateTag Get(SecurityClass security, Guid tagGuid);

		[OperationContract]
		List<KeyValuePair<string,string>> EnumerateMovementSummaryColumnNames(SecurityClass security);
	}
}
