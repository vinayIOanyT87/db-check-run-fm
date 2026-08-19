-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToBlendComponent
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblProductToBlendComponent]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToBlendComponentGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToProductGuid uniqueidentifier,
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
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblProductToBlendComponent] CT
                        WHERE CT.PK_ProductToBlendComponentGuid = @ProductToBlendComponentGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [map].[tblProductToBlendComponent].[ProductToBlendComponentGuid],[map].[tblProductToBlendComponent].[ProductGuid],[map].[tblProductToBlendComponent].[AssignedToProductGuid],[map].[tblProductToBlendComponent].[Sequence],[map].[tblProductToBlendComponent].[BlendPercentage],[map].[tblProductToBlendComponent].[AdditiveRate],[map].[tblProductToBlendComponent].[Ratio],[map].[tblProductToBlendComponent].[AdditiveCycleVolume],[map].[tblProductToBlendComponent].[Tolerance],[map].[tblProductToBlendComponent].[PresetNumber],[map].[tblProductToBlendComponent].[AdditiveProfileGuid],[map].[tblProductToBlendComponent].[TankGuid],[map].[tblProductToBlendComponent].[MeterID],[map].[tblProductToBlendComponent].[ShipToProductID],[map].[tblProductToBlendComponent].[ShipToProductCode],[map].[tblProductToBlendComponent].[ShipToLoadRackDisplayText],[map].[tblProductToBlendComponent].[UnavailableInventoryGross],[map].[tblProductToBlendComponent].[UnavailableInventoryNet],[map].[tblProductToBlendComponent].[CreatedDate],[map].[tblProductToBlendComponent].[CreatedBy],[map].[tblProductToBlendComponent].[UpdatedDate],[map].[tblProductToBlendComponent].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [map].[tblProductToBlendComponent]
                        INNER JOIN [track].[tblProductToBlendComponent] CT
                            ON CT.PK_ProductToBlendComponentGuid = [map].[tblProductToBlendComponent].[ProductToBlendComponentGuid] 
                    WHERE CT.PK_ProductToBlendComponentGuid = @ProductToBlendComponentGuid
            ) MERGE existingData
            USING (SELECT @ProductToBlendComponentGuid,@ProductGuid,@AssignedToProductGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([ProductToBlendComponentGuid],[ProductGuid],[AssignedToProductGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[ProductToBlendComponentGuid] = remoteChanges.[ProductToBlendComponentGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ProductGuid] = remoteChanges.[ProductGuid]
                       ,[AssignedToProductGuid] = remoteChanges.[AssignedToProductGuid]
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

            WHEN NOT MATCHED THEN
                INSERT ([ProductToBlendComponentGuid],[ProductGuid],[AssignedToProductGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@ProductToBlendComponentGuid,@ProductGuid,@AssignedToProductGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToBlendComponentGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToBlendComponentGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToBlendComponentGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblProductToBlendComponent] WHERE ProductToBlendComponentGuid = @ProductToBlendComponentGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
