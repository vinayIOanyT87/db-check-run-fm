using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using System.ServiceModel;

using Microsoft.Synchronization.Data;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
    /// <summary>
    /// Interface that represents the standard synchronization framework proxy interface.
    /// </summary>
    public interface ISyncServerProviderFM
    {
        #region Properties
        #endregion Properties

        #region Methods
        SyncContext ApplyChanges(SyncGroupMetadata pGroupMetadata, DataSet pDataSet, SyncSession pSyncSession);
        SyncContext GetChanges(SyncGroupMetadata pGroupMetadata, SyncSession pSyncSession);
        SyncSchema GetSchema(Collection<string> pTableNames, SyncSession pSyncSession);
        SyncServerInfo GetServerInfo(SyncSession pSyncSession);
        #endregion Methods
    }
}
