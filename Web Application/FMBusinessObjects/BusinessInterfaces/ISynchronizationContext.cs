using System;

using FMBusinessObjects.Constants;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
    using System.Collections;
    using System.Collections.Generic;

    using FMBusinessObjects.UtilityObjects;

    public interface ISynchronizationContext
    {
        int CurrentBatchSegment { get; set; }
        bool IsBatching { get; set;  }
        Guid ClientID { get; set; }
        Guid SyncSessionID { get; }
        int RecordsPerBatch { get; set; }
        Guid ServerID { get; set; }
        SYNCREQUESTTYPE RequestType { get; set; }
        SYNCSESSION SessionType { get; set; }
        System.Nullable<Guid> SiteGuid { get; set; }
        string SiteID { get; set; }
        SYNCTRANSFERTYPE TransferType { get; set; }
        DateTimeOffset StartDateRange { get; set; }
        DateTimeOffset EndDateRange { get; set; }
        bool UseDateRangeSynchronization { get; set; }
        SecurityClass Security { get; set; }
        SecurityClass ServerSecurity { get; set; }
        string CurrentSyncProfileID { get; set; }
        string CurrentSyncScopeID { get; set; }
        string CurrentSiteID { get; set; }
        System.Nullable<Guid> CurrentSiteGuid { get; set; }
        ArrayList SiteSynchronizationList { get; set; }
        SYNCCONTROLLERSTEP CurrentControllerStep { get; set; }
        VersionInfo ClientVersion { get; set; }
        //OFFLINESYNCACTION OfflineSyncAction { get; set; }
        DateTimeOffset ContextCreatedDate { get; set; }
        Dictionary<string, SyncTableToScopeMapColumnCollection> SupportedColumnsByTable { get; set; }
    }
}
