-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetComponentTankOrTankGroup
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblProductToPresetComponentTankOrTankGroup]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToPresetComponentTankOrTankGroupGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToLoadArmGuid uniqueidentifier,
@Sequence int,
@BlendPercentage float,
@AdditiveRate float,
@Ratio float,
@AdditiveCycleVolume float,
@Tolerance float,
@PresetNumber int,
@AdditiveProfileGuid uniqueidentifier,
@TankGuid uniqueidentifier,
@TankGroupApplicationStringGuid uniqueidentifier,
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
@AssignedToMeterGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblProductToPresetComponentTankOrTankGroup] CT
                        WHERE CT.PK_ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[ProductGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToLoadArmGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[Sequence],[map].[tblProductToPresetComponentTankOrTankGroup].[BlendPercentage],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveRate],[map].[tblProductToPresetComponentTankOrTankGroup].[Ratio],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveCycleVolume],[map].[tblProductToPresetComponentTankOrTankGroup].[Tolerance],[map].[tblProductToPresetComponentTankOrTankGroup].[PresetNumber],[map].[tblProductToPresetComponentTankOrTankGroup].[AdditiveProfileGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[TankGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[TankGroupApplicationStringGuid],[map].[tblProductToPresetComponentTankOrTankGroup].[MeterID],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToProductID],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToProductCode],[map].[tblProductToPresetComponentTankOrTankGroup].[ShipToLoadRackDisplayText],[map].[tblProductToPresetComponentTankOrTankGroup].[UnavailableInventoryGross],[map].[tblProductToPresetComponentTankOrTankGroup].[UnavailableInventoryNet],[map].[tblProductToPresetComponentTankOrTankGroup].[CreatedDate],[map].[tblProductToPresetComponentTankOrTankGroup].[CreatedBy],[map].[tblProductToPresetComponentTankOrTankGroup].[UpdatedDate],[map].[tblProductToPresetComponentTankOrTankGroup].[UpdatedBy],[map].[tblProductToPresetComponentTankOrTankGroup].[AssignedToMeterGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [map].[tblProductToPresetComponentTankOrTankGroup]
                        INNER JOIN [track].[tblProductToPresetComponentTankOrTankGroup] CT
                            ON CT.PK_ProductToPresetComponentTankOrTankGroupGuid = [map].[tblProductToPresetComponentTankOrTankGroup].[ProductToPresetComponentTankOrTankGroupGuid] 
                    WHERE CT.PK_ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid
            ) MERGE existingData
            USING (SELECT @ProductToPresetComponentTankOrTankGroupGuid,@ProductGuid,@AssignedToLoadArmGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@TankGroupApplicationStringGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AssignedToMeterGuid
                    ) AS remoteChanges ([ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[AssignedToLoadArmGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[TankGroupApplicationStringGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedToMeterGuid])
            ON (existingData.[ProductToPresetComponentTankOrTankGroupGuid] = remoteChanges.[ProductToPresetComponentTankOrTankGroupGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ProductGuid] = remoteChanges.[ProductGuid]
                       ,[AssignedToLoadArmGuid] = remoteChanges.[AssignedToLoadArmGuid]
                       ,[Sequence] = remoteChanges.[Sequence]
                       ,[BlendPercentage] = remoteChanges.[BlendPercentage]
                       ,[AdditiveRate] = remoteChanges.[AdditiveRate]
                       ,[Ratio] = remoteChanges.[Ratio]
                       ,[AdditiveCycleVolume] = remoteChanges.[AdditiveCycleVolume]
                       ,[Tolerance] = remoteChanges.[Tolerance]
                       ,[PresetNumber] = remoteChanges.[PresetNumber]
                       ,[AdditiveProfileGuid] = remoteChanges.[AdditiveProfileGuid]
                       ,[TankGuid] = remoteChanges.[TankGuid]
                       ,[TankGroupApplicationStringGuid] = remoteChanges.[TankGroupApplicationStringGuid]
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
                       ,[AssignedToMeterGuid] = remoteChanges.[AssignedToMeterGuid]

            WHEN NOT MATCHED THEN
                INSERT ([ProductToPresetComponentTankOrTankGroupGuid],[ProductGuid],[AssignedToLoadArmGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[TankGroupApplicationStringGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedToMeterGuid])
                    VALUES (@ProductToPresetComponentTankOrTankGroupGuid,@ProductGuid,@AssignedToLoadArmGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@TankGroupApplicationStringGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AssignedToMeterGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToPresetComponentTankOrTankGroupGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToPresetComponentTankOrTankGroupGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToPresetComponentTankOrTankGroupGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
