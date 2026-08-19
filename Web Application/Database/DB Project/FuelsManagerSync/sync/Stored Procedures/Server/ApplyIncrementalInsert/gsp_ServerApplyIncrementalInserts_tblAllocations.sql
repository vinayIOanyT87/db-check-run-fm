-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocations
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblAllocations]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@EffectiveDate datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@LoadWarning float,
@LoadDenial float,
@ContractNumber nvarchar(10),
@AllocationGroupIndex int,
@LastAllocationResetDate datetimeoffset(7),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@AllocationGuid uniqueidentifier,
@CompanyBillToToShipperGuid uniqueidentifier,
@CompanyLoadOwnerToManagerGuid uniqueidentifier,
@CompanyOffLoadOwnerToManagerGuid uniqueidentifier,
@CompanyShipperToOwnerGuid uniqueidentifier,
@CompanyShipToToBillToGuid uniqueidentifier,
@CompanySupplierToOwnerGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupCompanyMapTypeIndex int,
@AllocationGroupApplicationStringGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAllocations varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAllocations] AS existingData
        USING (SELECT @EffectiveDate 'EffectiveDate',@ExpirationDate 'ExpirationDate',@LoadWarning 'LoadWarning',@LoadDenial 'LoadDenial',@ContractNumber 'ContractNumber',@AllocationGroupIndex 'AllocationGroupIndex',@LastAllocationResetDate 'LastAllocationResetDate',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@AllocationGuid 'AllocationGuid',@CompanyBillToToShipperGuid 'CompanyBillToToShipperGuid',@CompanyLoadOwnerToManagerGuid 'CompanyLoadOwnerToManagerGuid',@CompanyOffLoadOwnerToManagerGuid 'CompanyOffLoadOwnerToManagerGuid',@CompanyShipperToOwnerGuid 'CompanyShipperToOwnerGuid',@CompanyShipToToBillToGuid 'CompanyShipToToBillToGuid',@CompanySupplierToOwnerGuid 'CompanySupplierToOwnerGuid',@SiteGuid 'SiteGuid',@LookupCompanyMapTypeIndex 'LookupCompanyMapTypeIndex',@AllocationGroupApplicationStringGuid 'AllocationGroupApplicationStringGuid'
                ) AS remoteChanges ([EffectiveDate],[ExpirationDate],[LoadWarning],[LoadDenial],[ContractNumber],[AllocationGroupIndex],[LastAllocationResetDate],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AllocationGuid],[CompanyBillToToShipperGuid],[CompanyLoadOwnerToManagerGuid],[CompanyOffLoadOwnerToManagerGuid],[CompanyShipperToOwnerGuid],[CompanyShipToToBillToGuid],[CompanySupplierToOwnerGuid],[SiteGuid],[LookupCompanyMapTypeIndex],[AllocationGroupApplicationStringGuid])
        ON (existingData.[AllocationGuid] = remoteChanges.[AllocationGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [EffectiveDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EffectiveDate'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[EffectiveDate] ELSE remoteChanges.[EffectiveDate] END
                       ,[ExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[ExpirationDate] ELSE remoteChanges.[ExpirationDate] END
                       ,[LoadWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadWarning'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[LoadWarning] ELSE remoteChanges.[LoadWarning] END
                       ,[LoadDenial] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadDenial'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[LoadDenial] ELSE remoteChanges.[LoadDenial] END
                       ,[ContractNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ContractNumber'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[ContractNumber] ELSE remoteChanges.[ContractNumber] END
                       ,[AllocationGroupIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllocationGroupIndex'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[AllocationGroupIndex] ELSE remoteChanges.[AllocationGroupIndex] END
                       ,[LastAllocationResetDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LastAllocationResetDate'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[LastAllocationResetDate] ELSE remoteChanges.[LastAllocationResetDate] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[CompanyBillToToShipperGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyBillToToShipperGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanyBillToToShipperGuid] ELSE remoteChanges.[CompanyBillToToShipperGuid] END
                       ,[CompanyLoadOwnerToManagerGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyLoadOwnerToManagerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanyLoadOwnerToManagerGuid] ELSE remoteChanges.[CompanyLoadOwnerToManagerGuid] END
                       ,[CompanyOffLoadOwnerToManagerGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyOffLoadOwnerToManagerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanyOffLoadOwnerToManagerGuid] ELSE remoteChanges.[CompanyOffLoadOwnerToManagerGuid] END
                       ,[CompanyShipperToOwnerGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyShipperToOwnerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanyShipperToOwnerGuid] ELSE remoteChanges.[CompanyShipperToOwnerGuid] END
                       ,[CompanyShipToToBillToGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyShipToToBillToGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanyShipToToBillToGuid] ELSE remoteChanges.[CompanyShipToToBillToGuid] END
                       ,[CompanySupplierToOwnerGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanySupplierToOwnerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[CompanySupplierToOwnerGuid] ELSE remoteChanges.[CompanySupplierToOwnerGuid] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupCompanyMapTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupCompanyMapTypeIndex'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[LookupCompanyMapTypeIndex] ELSE remoteChanges.[LookupCompanyMapTypeIndex] END
                       ,[AllocationGroupApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllocationGroupApplicationStringGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN existingData.[AllocationGroupApplicationStringGuid] ELSE remoteChanges.[AllocationGroupApplicationStringGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([EffectiveDate],[ExpirationDate],[LoadWarning],[LoadDenial],[ContractNumber],[AllocationGroupIndex],[LastAllocationResetDate],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AllocationGuid],[CompanyBillToToShipperGuid],[CompanyLoadOwnerToManagerGuid],[CompanyOffLoadOwnerToManagerGuid],[CompanyShipperToOwnerGuid],[CompanyShipToToBillToGuid],[CompanySupplierToOwnerGuid],[SiteGuid],[LookupCompanyMapTypeIndex],[AllocationGroupApplicationStringGuid])
                VALUES (@EffectiveDate,@ExpirationDate,@LoadWarning,@LoadDenial,@ContractNumber,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllocationGroupIndex'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @AllocationGroupIndex END),@LastAllocationResetDate,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AllocationGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyBillToToShipperGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanyBillToToShipperGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyLoadOwnerToManagerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanyLoadOwnerToManagerGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyOffLoadOwnerToManagerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanyOffLoadOwnerToManagerGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyShipperToOwnerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanyShipperToOwnerGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyShipToToBillToGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanyShipToToBillToGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanySupplierToOwnerGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @CompanySupplierToOwnerGuid END),@SiteGuid,@LookupCompanyMapTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllocationGroupApplicationStringGuid'), @sync_supported_columns_tblAllocations)) WHEN 0 THEN NULL ELSE @AllocationGroupApplicationStringGuid END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAllocations] WHERE AllocationGuid = @AllocationGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

