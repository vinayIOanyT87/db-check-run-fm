-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblLoadArms
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblLoadArms]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@LoadRackText nvarchar(9),
@Enabled bit,
@SwingArm bit,
@BayAArmNumber int,
@BayBArmNumber int,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@LoadArmGuid uniqueidentifier,
@LookupPresetTypeIndex int,
@BayAStationGuid uniqueidentifier,
@BayBStationGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblLoadArms varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblLoadArms] CT
                        WHERE CT.PK_LoadArmGuid = @LoadArmGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblLoadArms].[LoadRackText],[dbo].[tblLoadArms].[Enabled],[dbo].[tblLoadArms].[SwingArm],[dbo].[tblLoadArms].[BayAArmNumber],[dbo].[tblLoadArms].[BayBArmNumber],[dbo].[tblLoadArms].[CreatedDate],[dbo].[tblLoadArms].[CreatedBy],[dbo].[tblLoadArms].[UpdatedDate],[dbo].[tblLoadArms].[UpdatedBy],[dbo].[tblLoadArms].[LoadArmGuid],[dbo].[tblLoadArms].[LookupPresetTypeIndex],[dbo].[tblLoadArms].[BayAStationGuid],[dbo].[tblLoadArms].[BayBStationGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblLoadArms]
                        INNER JOIN [track].[tblLoadArms] CT
                            ON CT.PK_LoadArmGuid = [dbo].[tblLoadArms].[LoadArmGuid] 
                    WHERE CT.PK_LoadArmGuid = @LoadArmGuid
            ) MERGE existingData
            USING (SELECT @LoadRackText,@Enabled,@SwingArm,@BayAArmNumber,@BayBArmNumber,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@LoadArmGuid,@LookupPresetTypeIndex,@BayAStationGuid,@BayBStationGuid
                    ) AS remoteChanges ([LoadRackText],[Enabled],[SwingArm],[BayAArmNumber],[BayBArmNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[LoadArmGuid],[LookupPresetTypeIndex],[BayAStationGuid],[BayBStationGuid])
            ON (existingData.[LoadArmGuid] = remoteChanges.[LoadArmGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [LoadRackText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackText'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[LoadRackText] ELSE remoteChanges.[LoadRackText] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[SwingArm] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SwingArm'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[SwingArm] ELSE remoteChanges.[SwingArm] END
                       ,[BayAArmNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayAArmNumber'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[BayAArmNumber] ELSE remoteChanges.[BayAArmNumber] END
                       ,[BayBArmNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayBArmNumber'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[BayBArmNumber] ELSE remoteChanges.[BayBArmNumber] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[LookupPresetTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupPresetTypeIndex'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[LookupPresetTypeIndex] ELSE remoteChanges.[LookupPresetTypeIndex] END
                       ,[BayAStationGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayAStationGuid'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[BayAStationGuid] ELSE remoteChanges.[BayAStationGuid] END
                       ,[BayBStationGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayBStationGuid'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN existingData.[BayBStationGuid] ELSE remoteChanges.[BayBStationGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([LoadRackText],[Enabled],[SwingArm],[BayAArmNumber],[BayBArmNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[LoadArmGuid],[LookupPresetTypeIndex],[BayAStationGuid],[BayBStationGuid])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackText'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN NULL ELSE @LoadRackText END),@Enabled,@SwingArm,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayAArmNumber'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN NULL ELSE @BayAArmNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayBArmNumber'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN NULL ELSE @BayBArmNumber END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@LoadArmGuid,@LookupPresetTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayAStationGuid'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN NULL ELSE @BayAStationGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BayBStationGuid'), @sync_supported_columns_tblLoadArms)) WHEN 0 THEN NULL ELSE @BayBStationGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @LoadArmGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @LoadArmGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @LoadArmGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
