namespace FMBusinessObjects.BusinessInterfaces
{
	using System;
	using System.Collections.Generic;
	using System.ServiceModel;

	using FMBusinessObjects.DataObjects;

	[ServiceContract]
	public interface IPointTags
	{
		[OperationContract]
		[TransactionFlow( TransactionFlowOption.Allowed )]
		void AddTags( SecurityClass security, Dictionary<Guid, PointTag> tags );

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void DeleteTags(SecurityClass security, Guid pointGuid);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void Modify(SecurityClass security, PointTag tag);

		[OperationContract]
		Dictionary<Guid, PointTag> EnumerateByPointGuid( SecurityClass security, Guid pointGuid, bool enforcePointAccess = false);

		[OperationContract]
		Dictionary<Guid, string> EnumerateIdByPointGuid(SecurityClass security, Guid pointGuid);


		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateForSimulator(SecurityClass security, string opcUaEndPoint);

		[OperationContract]
		Dictionary<Guid,Dictionary<Guid, PointTag>> EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);

		[OperationContract]
		Dictionary<Guid, Dictionary<Guid, PointTag>> EnumerateByPointList(SecurityClass security, List<Guid> pointGuidList, List<string> tagIDFilter = null);

		[OperationContract]
		PointTag Get(SecurityClass security, Guid tagGuid);

		[OperationContract]
		Dictionary<Guid, PointTag> EnumerateByTagList(SecurityClass security, List<Guid> tagGuidList);

		[OperationContract]
		List<Guid> EnumerateTagListByPointAccess(SecurityClass security, List<Guid> tagGuidList);

		[OperationContract]
      Dictionary<String, Guid> EnumerateTagListByOpcUaNodeId(SecurityClass security, List<String> OpcUaNodeIds);

      [OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyTagValues(SecurityClass security, List<PointTag> pointTags, bool enterpriseVisibility);

		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
		void ModifyPointValues(SecurityClass security, List<PointValue> pointValues, bool enterpriseVisibility);

		/// <summary>
		/// Gets the maximum pointTag row version.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <returns></returns>
		[OperationContract]
		Int64? GetMaxPointTagRowVersion(SecurityClass security);

		[OperationContract]
		List<Guid> EnumerateArchivedPointTagGuidsBySite(SecurityClass security, Guid siteGuid);

		[OperationContract]
		Guid GetIdentityGuid(SecurityClass security, string pointTagID, Guid pointGuid);

		[OperationContract]
		List<PointTag> EnumerateTagsByPointList(SecurityClass security, List<Guid> points, string tagID);

		[OperationContract]
		List<PointValueIdentifier> EnumeratePointValueIdentifersByPointAndTagLists(SecurityClass security, List<Guid> pointGuids, List<Guid> tagGuids);

		[OperationContract]
		Dictionary<Guid, PointTag> EnumerateTagsAssociatedWithDeviceAlarmMapBySiteGuid(SecurityClass security, Guid siteGuid);
	}
}
