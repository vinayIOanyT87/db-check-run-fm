-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToCompanyGroup
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblProductToCompanyGroup]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToCompanyGroupGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToApplicationStringGuid uniqueidentifier,
@Sequence int,
@BlendPercentage float,
@AdditiveRate float,
@Ratio float,
@AdditiveCycleVolume float,
@Tolerance float,
@PresetNumber int,
@AdditiveProfileGuid uniqueidentifier,
@TankGuid uniqueidentifier,
@MeterID nvarchar(20),
@ShipToProductID nvarchar(30),
@ShipToProductCode nvarchar(15),
@ShipToLoadRackDisplayText nvarchar(10),
@UnavailableInventoryGross float,
@UnavailableInventoryNet float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@SpecialInstructionNote nvarchar(2000),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [map].[tblProductToCompanyGroup] AS existingData
        USING (SELECT @ProductToCompanyGroupGuid 'ProductToCompanyGroupGuid',@ProductGuid 'ProductGuid',@AssignedToApplicationStringGuid 'AssignedToApplicationStringGuid',@Sequence 'Sequence',@BlendPercentage 'BlendPercentage',@AdditiveRate 'AdditiveRate',@Ratio 'Ratio',@AdditiveCycleVolume 'AdditiveCycleVolume',@Tolerance 'Tolerance',@PresetNumber 'PresetNumber',@AdditiveProfileGuid 'AdditiveProfileGuid',@TankGuid 'TankGuid',@MeterID 'MeterID',@ShipToProductID 'ShipToProductID',@ShipToProductCode 'ShipToProductCode',@ShipToLoadRackDisplayText 'ShipToLoadRackDisplayText',@UnavailableInventoryGross 'UnavailableInventoryGross',@UnavailableInventoryNet 'UnavailableInventoryNet',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@SpecialInstructionNote 'SpecialInstructionNote'
                ) AS remoteChanges ([ProductToCompanyGroupGuid],[ProductGuid],[AssignedToApplicationStringGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SpecialInstructionNote])
        ON (existingData.[ProductToCompanyGroupGuid] = remoteChanges.[ProductToCompanyGroupGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ProductGuid] = remoteChanges.[ProductGuid]
                       ,[AssignedToApplicationStringGuid] = remoteChanges.[AssignedToApplicationStringGuid]
                       ,[Sequence] = remoteChanges.[Sequence]
                       ,[BlendPercentage] = remoteChanges.[BlendPercentage]
                       ,[AdditiveRate] = remoteChanges.[AdditiveRate]
                       ,[Ratio] = remoteChanges.[Ratio]
                       ,[AdditiveCycleVolume] = remoteChanges.[AdditiveCycleVolume]
                       ,[Tolerance] = remoteChanges.[Tolerance]
                       ,[PresetNumber] = remoteChanges.[PresetNumber]
                       ,[AdditiveProfileGuid] = remoteChanges.[AdditiveProfileGuid]
                       ,[TankGuid] = remoteChanges.[TankGuid]
                       ,[MeterID] = remoteChanges.[MeterID]
                       ,[ShipToProductID] = remoteChanges.[ShipToProductID]
                       ,[ShipToProductCode] = remoteChanges.[ShipToProductCode]
                       ,[ShipToLoadRackDisplayText] = remoteChanges.[ShipToLoadRackDisplayText]
                       ,[UnavailableInventoryGross] = remoteChanges.[UnavailableInventoryGross]
                       ,[UnavailableInventoryNet] = remoteChanges.[UnavailableInventoryNet]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SpecialInstructionNote] = remoteChanges.[SpecialInstructionNote]

        WHEN NOT MATCHED THEN
            INSERT ([ProductToCompanyGroupGuid],[ProductGuid],[AssignedToApplicationStringGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SpecialInstructionNote])
                VALUES (@ProductToCompanyGroupGuid,@ProductGuid,@AssignedToApplicationStringGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@SpecialInstructionNote)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToCompanyGroupGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToCompanyGroupGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToCompanyGroupGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblProductToCompanyGroup] WHERE ProductToCompanyGroupGuid = @ProductToCompanyGroupGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
