// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMigrationDataExportImportLog.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the IMigrationDataExportImportLog type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.BusinessInterfaces
{
    using System;
    using System.ServiceModel;

    using FMBusinessObjects.DataObjects;

    [ServiceContract]
    public interface IMigrationDataExportImportLog
    {
        [OperationContract]
        Guid Add(SecurityClass security, MigrationDataExportImportLogDO migrationImportExportHistoryDo);

        [OperationContract]
        void Modify(SecurityClass security, MigrationDataExportImportLogDO migrationImportExportHistoryDo);

        [OperationContract]
        void Purge(SecurityClass security, Guid identityGuid);

        [OperationContract]
        MigrationDataExportImportLogDO Get(SecurityClass security, Guid identityGuid);

        [OperationContract]
        MigrationDataExportImportLogCollection Enumerate(SecurityClass security);

        [OperationContract]
        MigrationDataExportImportLogCollection EnumerateBySiteGuid(SecurityClass security, Guid siteGuid);

        [OperationContract]
        MigrationDataExportImportLogCollection EnumerateExt(SecurityClass security, Guid? siteGuid, int limit = 0);
    }
}
