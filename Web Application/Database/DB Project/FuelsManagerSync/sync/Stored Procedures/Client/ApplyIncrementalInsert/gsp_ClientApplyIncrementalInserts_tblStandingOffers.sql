-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblStandingOffers
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblStandingOffers]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblStandingOffers] AS existingData
        USING (SELECT @StandingOfferPrice 'StandingOfferPrice',@EffectiveDate 'EffectiveDate',@ExpirationDate 'ExpirationDate',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@LowerBound 'LowerBound',@UpperBound 'UpperBound',@ReferenceNumber 'ReferenceNumber',@StandingOfferGuid 'StandingOfferGuid',@SiteGuid 'SiteGuid',@ProductGuid 'ProductGuid',@SupplierCompanyGuid 'SupplierCompanyGuid',@LocationIATAGuid 'LocationIATAGuid'
                ) AS remoteChanges ([StandingOfferPrice],[EffectiveDate],[ExpirationDate],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LowerBound],[UpperBound],[ReferenceNumber],[StandingOfferGuid],[SiteGuid],[ProductGuid],[SupplierCompanyGuid],[LocationIATAGuid])
        ON (existingData.[StandingOfferGuid] = remoteChanges.[StandingOfferGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [StandingOfferPrice] = remoteChanges.[StandingOfferPrice]
                       ,[EffectiveDate] = remoteChanges.[EffectiveDate]
                       ,[ExpirationDate] = remoteChanges.[ExpirationDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[LowerBound] = remoteChanges.[LowerBound]
                       ,[UpperBound] = remoteChanges.[UpperBound]
                       ,[ReferenceNumber] = remoteChanges.[ReferenceNumber]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[SupplierCompanyGuid] = remoteChanges.[SupplierCompanyGuid]
                       ,[LocationIATAGuid] = remoteChanges.[LocationIATAGuid]

        WHEN NOT MATCHED THEN
            INSERT ([StandingOfferPrice],[EffectiveDate],[ExpirationDate],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[LowerBound],[UpperBound],[ReferenceNumber],[StandingOfferGuid],[SiteGuid],[ProductGuid],[SupplierCompanyGuid],[LocationIATAGuid])
                VALUES (@StandingOfferPrice,@EffectiveDate,@ExpirationDate,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@LowerBound,@UpperBound,@ReferenceNumber,@StandingOfferGuid,@SiteGuid,@ProductGuid,@SupplierCompanyGuid,@LocationIATAGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
