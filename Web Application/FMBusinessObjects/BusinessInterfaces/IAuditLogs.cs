using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IAuditLogs
	{
        [OperationContract]
        DataSet EnumerateIDs(
            SecurityClass security,
			Guid siteGuid, 
			DateTimeOffset beginning,
            DateTimeOffset ending,
            string actionID,
            string typeID,
            bool includeMemberSites);


		[OperationContract]
		DataSet EnumerateForAuditLogPage(
			SecurityClass security,
			DateTimeOffset beginning,
			DateTimeOffset ending,
			string actionID,
			string typeID,
			string id,
			string createdBy,
			string sourceFilter,
			bool useDataDictionary,
			bool includeMemberSites,
         bool queryArchiveDb,
         bool includeGlobalSites);

		[OperationContract]
		void ProcessAuditPurgeOld(SecurityClass security, Guid siteGuid, int maxDaysToRetain);

		[OperationContract]
		void PurgeShadowSiteTable(SecurityClass security, Guid siteGuid);
		
        [OperationContract]
        AuditLogCollectionClass EnumerateByBatch(
            SecurityClass security,
            DateTimeOffset? auditedDateTimeStart,
            DateTimeOffset? auditedDateTimeEnd,
            string actionID,
            string typeID,
            string id,
            string sourceNode,
            bool useDataDictionary,
            bool includeMemberSites,
            int batchSize,
            int batchNumber);

        [OperationContract]
        List<string> EnumerateAuditLogIds(
            SecurityClass security,
            Guid siteGuid,
            DateTimeOffset? auditedDateTimeStart,
            DateTimeOffset? auditedDateTimeEnd,
            string actionID,
            string typeID);

		[OperationContract]
		Dictionary<Guid, int?> GetAllSiteRetentionForShadowTable(SecurityClass security);

		[OperationContract]
        [TransactionFlow(TransactionFlowOption.Allowed)]
        void ProcessPendingAudits(SecurityClass security);
    }
}
