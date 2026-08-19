-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToAdditiveProfile
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblProductToAdditiveProfile]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToAdditiveProfileGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToAdditiveProfileGuid uniqueidentifier,
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
@DesiredTreatRate float,
@EnableRecipe bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblProductToAdditiveProfile varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblProductToAdditiveProfile] CT
                        WHERE CT.PK_ProductToAdditiveProfileGuid = @ProductToAdditiveProfileGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [map].[tblProductToAdditiveProfile].[ProductToAdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[ProductGuid],[map].[tblProductToAdditiveProfile].[AssignedToAdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[Sequence],[map].[tblProductToAdditiveProfile].[BlendPercentage],[map].[tblProductToAdditiveProfile].[AdditiveRate],[map].[tblProductToAdditiveProfile].[Ratio],[map].[tblProductToAdditiveProfile].[AdditiveCycleVolume],[map].[tblProductToAdditiveProfile].[Tolerance],[map].[tblProductToAdditiveProfile].[PresetNumber],[map].[tblProductToAdditiveProfile].[AdditiveProfileGuid],[map].[tblProductToAdditiveProfile].[TankGuid],[map].[tblProductToAdditiveProfile].[MeterID],[map].[tblProductToAdditiveProfile].[ShipToProductID],[map].[tblProductToAdditiveProfile].[ShipToProductCode],[map].[tblProductToAdditiveProfile].[ShipToLoadRackDisplayText],[map].[tblProductToAdditiveProfile].[UnavailableInventoryGross],[map].[tblProductToAdditiveProfile].[UnavailableInventoryNet],[map].[tblProductToAdditiveProfile].[DesiredTreatRate],[map].[tblProductToAdditiveProfile].[EnableRecipe],[map].[tblProductToAdditiveProfile].[CreatedDate],[map].[tblProductToAdditiveProfile].[CreatedBy],[map].[tblProductToAdditiveProfile].[UpdatedDate],[map].[tblProductToAdditiveProfile].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [map].[tblProductToAdditiveProfile]
                        INNER JOIN [track].[tblProductToAdditiveProfile] CT
                            ON CT.PK_ProductToAdditiveProfileGuid = [map].[tblProductToAdditiveProfile].[ProductToAdditiveProfileGuid] 
                    WHERE CT.PK_ProductToAdditiveProfileGuid = @ProductToAdditiveProfileGuid
            ) MERGE existingData
            USING (SELECT @ProductToAdditiveProfileGuid,@ProductGuid,@AssignedToAdditiveProfileGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,@AdditiveProfileGuid,@TankGuid,@MeterID,@ShipToProductID,@ShipToProductCode,@ShipToLoadRackDisplayText,@UnavailableInventoryGross,@UnavailableInventoryNet,@DesiredTreatRate,@EnableRecipe,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([ProductToAdditiveProfileGuid],[ProductGuid],[AssignedToAdditiveProfileGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[DesiredTreatRate],[EnableRecipe],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[ProductToAdditiveProfileGuid] = remoteChanges.[ProductToAdditiveProfileGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[AssignedToAdditiveProfileGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedToAdditiveProfileGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[AssignedToAdditiveProfileGuid] ELSE remoteChanges.[AssignedToAdditiveProfileGuid] END
                       ,[Sequence] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Sequence'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[Sequence] ELSE remoteChanges.[Sequence] END
                       ,[BlendPercentage] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BlendPercentage'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[BlendPercentage] ELSE remoteChanges.[BlendPercentage] END
                       ,[AdditiveRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveRate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[AdditiveRate] ELSE remoteChanges.[AdditiveRate] END
                       ,[Ratio] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Ratio'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[Ratio] ELSE remoteChanges.[Ratio] END
                       ,[AdditiveCycleVolume] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveCycleVolume'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[AdditiveCycleVolume] ELSE remoteChanges.[AdditiveCycleVolume] END
                       ,[Tolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tolerance'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[Tolerance] ELSE remoteChanges.[Tolerance] END
                       ,[PresetNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PresetNumber'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[PresetNumber] ELSE remoteChanges.[PresetNumber] END
                       ,[AdditiveProfileGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[AdditiveProfileGuid] ELSE remoteChanges.[AdditiveProfileGuid] END
                       ,[TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END
                       ,[MeterID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[MeterID] ELSE remoteChanges.[MeterID] END
                       ,[ShipToProductID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductID'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[ShipToProductID] ELSE remoteChanges.[ShipToProductID] END
                       ,[ShipToProductCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductCode'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[ShipToProductCode] ELSE remoteChanges.[ShipToProductCode] END
                       ,[ShipToLoadRackDisplayText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToLoadRackDisplayText'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[ShipToLoadRackDisplayText] ELSE remoteChanges.[ShipToLoadRackDisplayText] END
                       ,[UnavailableInventoryGross] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryGross'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[UnavailableInventoryGross] ELSE remoteChanges.[UnavailableInventoryGross] END
                       ,[UnavailableInventoryNet] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryNet'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[UnavailableInventoryNet] ELSE remoteChanges.[UnavailableInventoryNet] END
                       ,[DesiredTreatRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DesiredTreatRate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[DesiredTreatRate] ELSE remoteChanges.[DesiredTreatRate] END
                       ,[EnableRecipe] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableRecipe'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[EnableRecipe] ELSE remoteChanges.[EnableRecipe] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([ProductToAdditiveProfileGuid],[ProductGuid],[AssignedToAdditiveProfileGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[DesiredTreatRate],[EnableRecipe],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@ProductToAdditiveProfileGuid,@ProductGuid,@AssignedToAdditiveProfileGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @AdditiveProfileGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @TankGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @MeterID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductID'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @ShipToProductID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductCode'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @ShipToProductCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToLoadRackDisplayText'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @ShipToLoadRackDisplayText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryGross'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @UnavailableInventoryGross END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryNet'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @UnavailableInventoryNet END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DesiredTreatRate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @DesiredTreatRate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableRecipe'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @EnableRecipe END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProductToAdditiveProfile)) WHEN 0 THEN NULL ELSE @UpdatedBy END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToAdditiveProfileGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToAdditiveProfileGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToAdditiveProfileGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblProductToAdditiveProfile] WHERE ProductToAdditiveProfileGuid = @ProductToAdditiveProfileGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
