using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.BusinessInterfaces
{
	[ServiceContract]
	public interface IFieldLevelConfigMaps
	{
		[OperationContract]
		[TransactionFlow(TransactionFlowOption.Allowed)]
        void Update(SecurityClass security, FieldLevelConfigCollectionClass flcCollection, Guid targetSitegroupGuid);

		[OperationContract]
        FieldLevelConfigCollectionClass GetFieldLevelConfigMatrix(SecurityClass security, string entityTypeId, Guid sitegroupGuid, string filterFieldName, bool ignoreFilterValues, Guid filterValueGuid, string targetField, FieldLevelConfigClass.FIELD_CONTROL_MODE forwardControlMode, bool includeChildrenSiteGroups);

        [OperationContract]
        Hashtable GetEntityTypes(SecurityClass security);

        [OperationContract]
        SortedList GetSiteHierarchy(SecurityClass security, Guid siteGuid, int? maxHierarchyDepth, bool siteGroupsOnly);

        [OperationContract]
        Hashtable GetFilters(SecurityClass security, string entityTypeId);

        [OperationContract]
        Hashtable GetFilterValues(SecurityClass security, string entityTypeId, Guid siteGuid, string FilterFieldName);

        [OperationContract]
        Hashtable GetTargetFields(SecurityClass security, string entityTypeId);

        [OperationContract]
        bool IsFieldRecordVersionSpecific(SecurityClass security, string entityTypeId, Guid recordGuid, Guid masterRecordGuid, Guid ownerSiteGuid, string targetField);

        [OperationContract]
        Guid GetRecordVersionGuid(SecurityClass security, string entityTypeId, Guid recordGuid, Guid targetSiteGuid);

        [OperationContract]
        ERVProcessSettingsClass GetProcessSettings(SecurityClass security);

        [OperationContract]
        void SetGlobalFieldsProcessingInhibitFlag(SecurityClass security, bool inhibitFlag, string userId);

    }
}
