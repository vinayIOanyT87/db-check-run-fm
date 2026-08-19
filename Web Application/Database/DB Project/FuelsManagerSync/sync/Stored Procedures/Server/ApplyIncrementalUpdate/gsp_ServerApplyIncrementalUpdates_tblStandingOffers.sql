-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStandingOffers
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblStandingOffers]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@StandingOfferPrice float,
@EffectiveDate datetimeoffset(7),
@ExpirationDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@LowerBound int,
@UpperBound int,
@ReferenceNumber nvarchar(20),
@StandingOfferGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@SupplierCompanyGuid uniqueidentifier,
@LocationIATAGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblStandingOffers varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblStandingOffers] CT
                        WHERE CT.PK_StandingOfferGuid = @StandingOfferGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblStandingOffers].[StandingOfferPrice],[dbo].[tblStandingOffers].[EffectiveDate],[dbo].[tblStandingOffers].[ExpirationDate],[dbo].[tblStandingOffers].[CreatedBy],[dbo].[tblStandingOffers].[CreatedDate],[dbo].[tblStandingOffers].[UpdatedBy],[dbo].[tblStandingOffers].[UpdatedDate],[dbo].[tblStandingOffers].[LowerBound],[dbo].[tblStandingOffers].[UpperBound],[dbo].[tblStandingOffers].[ReferenceNumber],[dbo].[tblStandingOffers].[StandingOfferGuid],[dbo].[tblStandingOffers].[SiteGuid],[dbo].[tblStandingOffers].[ProductGuid],[dbo].[tblStandingOffers].[SupplierCompanyGuid],[dbo].[tblStandingOffers].[LocationIATAGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblStandingOffers]
                        INNER JOIN [track].[tblStandingOffers] CT
                            ON CT.PK_StandingOfferGuid = [dbo].[tblStandingOffers].[StandingOfferGuid] 
                    WHERE CT.PK_StandingOfferGuid = @StandingOfferGuid
            ) MERGE existingData
            USING (SELECT @StandingOfferPrice,@EffectiveDate,@ExpirationDate,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@LowerBound,@UpperBound,@ReferenceNumber,@StandingOfferGuid,@SiteGuid,@ProductGuid,@SupplierCompanyGuid,@LocationIATAGuid
                    ) AS remoteChanges ([StandingOfferPrice],[EffectiveDate],[ExpirationDate],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LowerBound],[UpperBound],[ReferenceNumber],[StandingOfferGuid],[SiteGuid],[ProductGuid],[SupplierCompanyGuid],[LocationIATAGuid])
            ON (existingData.[StandingOfferGuid] = remoteChanges.[StandingOfferGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [StandingOfferPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StandingOfferPrice'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[StandingOfferPrice] ELSE remoteChanges.[StandingOfferPrice] END
                       ,[EffectiveDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EffectiveDate'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[EffectiveDate] ELSE remoteChanges.[EffectiveDate] END
                       ,[ExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[ExpirationDate] ELSE remoteChanges.[ExpirationDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[LowerBound] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowerBound'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[LowerBound] ELSE remoteChanges.[LowerBound] END
                       ,[UpperBound] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpperBound'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[UpperBound] ELSE remoteChanges.[UpperBound] END
                       ,[ReferenceNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReferenceNumber'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[ReferenceNumber] ELSE remoteChanges.[ReferenceNumber] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[SupplierCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SupplierCompanyGuid'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[SupplierCompanyGuid] ELSE remoteChanges.[SupplierCompanyGuid] END
                       ,[LocationIATAGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LocationIATAGuid'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN existingData.[LocationIATAGuid] ELSE remoteChanges.[LocationIATAGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([StandingOfferPrice],[EffectiveDate],[ExpirationDate],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LowerBound],[UpperBound],[ReferenceNumber],[StandingOfferGuid],[SiteGuid],[ProductGuid],[SupplierCompanyGuid],[LocationIATAGuid])
                    VALUES (@StandingOfferPrice,@EffectiveDate,@ExpirationDate,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowerBound'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN NULL ELSE @LowerBound END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpperBound'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN NULL ELSE @UpperBound END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReferenceNumber'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN NULL ELSE @ReferenceNumber END),@StandingOfferGuid,@SiteGuid,@ProductGuid,@SupplierCompanyGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LocationIATAGuid'), @sync_supported_columns_tblStandingOffers)) WHEN 0 THEN NULL ELSE @LocationIATAGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StandingOfferGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StandingOfferGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @StandingOfferGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblStandingOffers] WHERE StandingOfferGuid = @StandingOfferGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
