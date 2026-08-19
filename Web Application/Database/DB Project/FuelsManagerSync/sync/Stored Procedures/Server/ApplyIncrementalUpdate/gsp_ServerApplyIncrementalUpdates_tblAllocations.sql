-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocations
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblAllocations]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblAllocations] CT
                        WHERE CT.PK_AllocationGuid = @AllocationGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblAllocations].[EffectiveDate],[dbo].[tblAllocations].[ExpirationDate],[dbo].[tblAllocations].[LoadWarning],[dbo].[tblAllocations].[LoadDenial],[dbo].[tblAllocations].[ContractNumber],[dbo].[tblAllocations].[AllocationGroupIndex],[dbo].[tblAllocations].[LastAllocationResetDate],[dbo].[tblAllocations].[CreatedDate],[dbo].[tblAllocations].[CreatedBy],[dbo].[tblAllocations].[UpdatedDate],[dbo].[tblAllocations].[UpdatedBy],[dbo].[tblAllocations].[AllocationGuid],[dbo].[tblAllocations].[CompanyBillToToShipperGuid],[dbo].[tblAllocations].[CompanyLoadOwnerToManagerGuid],[dbo].[tblAllocations].[CompanyOffLoadOwnerToManagerGuid],[dbo].[tblAllocations].[CompanyShipperToOwnerGuid],[dbo].[tblAllocations].[CompanyShipToToBillToGuid],[dbo].[tblAllocations].[CompanySupplierToOwnerGuid],[dbo].[tblAllocations].[SiteGuid],[dbo].[tblAllocations].[LookupCompanyMapTypeIndex],[dbo].[tblAllocations].[AllocationGroupApplicationStringGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblAllocations]
                        INNER JOIN [track].[tblAllocations] CT
                            ON CT.PK_AllocationGuid = [dbo].[tblAllocations].[AllocationGuid] 
                    WHERE CT.PK_AllocationGuid = @AllocationGuid
            ) MERGE existingData
            USING (SELECT @EffectiveDate,@ExpirationDate,@LoadWarning,@LoadDenial,@ContractNumber,@AllocationGroupIndex,@LastAllocationResetDate,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AllocationGuid,@CompanyBillToToShipperGuid,@CompanyLoadOwnerToManagerGuid,@CompanyOffLoadOwnerToManagerGuid,@CompanyShipperToOwnerGuid,@CompanyShipToToBillToGuid,@CompanySupplierToOwnerGuid,@SiteGuid,@LookupCompanyMapTypeIndex,@AllocationGroupApplicationStringGuid
                    ) AS remoteChanges ([EffectiveDate],[ExpirationDate],[LoadWarning],[LoadDenial],[ContractNumber],[AllocationGroupIndex],[LastAllocationResetDate],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AllocationGuid],[CompanyBillToToShipperGuid],[CompanyLoadOwnerToManagerGuid],[CompanyOffLoadOwnerToManagerGuid],[CompanyShipperToOwnerGuid],[CompanyShipToToBillToGuid],[CompanySupplierToOwnerGuid],[SiteGuid],[LookupCompanyMapTypeIndex],[AllocationGroupApplicationStringGuid])
            ON (existingData.[AllocationGuid] = remoteChanges.[AllocationGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
