-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDrawings
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblDrawings]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@DrawingGuid uniqueidentifier,
@ID nvarchar(30),
@Description nvarchar(255),
@Image nvarchar(max),
@SiteGuid uniqueidentifier,
@PanelType int,
@PointTemplateGuid uniqueidentifier,
@Published bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblDrawings varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblDrawings] CT
                        WHERE CT.PK_DrawingGuid = @DrawingGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblDrawings].[DrawingGuid],[dbo].[tblDrawings].[ID],[dbo].[tblDrawings].[Description],[dbo].[tblDrawings].[Image],[dbo].[tblDrawings].[SiteGuid],[dbo].[tblDrawings].[PanelType],[dbo].[tblDrawings].[PointTemplateGuid],[dbo].[tblDrawings].[Published],[dbo].[tblDrawings].[CreatedDate],[dbo].[tblDrawings].[CreatedBy],[dbo].[tblDrawings].[UpdatedDate],[dbo].[tblDrawings].[UpdatedBy]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblDrawings]
                        INNER JOIN [track].[tblDrawings] CT
                            ON CT.PK_DrawingGuid = [dbo].[tblDrawings].[DrawingGuid] 
                    WHERE CT.PK_DrawingGuid = @DrawingGuid
            ) MERGE existingData
            USING (SELECT @DrawingGuid,@ID,@Description,@Image,@SiteGuid,@PanelType,@PointTemplateGuid,@Published,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy
                    ) AS remoteChanges ([DrawingGuid],[ID],[Description],[Image],[SiteGuid],[PanelType],[PointTemplateGuid],[Published],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
            ON (existingData.[DrawingGuid] = remoteChanges.[DrawingGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[Image] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Image'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[Image] ELSE remoteChanges.[Image] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[PanelType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PanelType'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[PanelType] ELSE remoteChanges.[PanelType] END
                       ,[PointTemplateGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTemplateGuid'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[PointTemplateGuid] ELSE remoteChanges.[PointTemplateGuid] END
                       ,[Published] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Published'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[Published] ELSE remoteChanges.[Published] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

            WHEN NOT MATCHED THEN
                INSERT ([DrawingGuid],[ID],[Description],[Image],[SiteGuid],[PanelType],[PointTemplateGuid],[Published],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                    VALUES (@DrawingGuid,@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @Description END),@Image,@SiteGuid,@PanelType,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PointTemplateGuid'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @PointTemplateGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Published'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @Published END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblDrawings)) WHEN 0 THEN NULL ELSE @UpdatedBy END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DrawingGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DrawingGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DrawingGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblDrawings] WHERE DrawingGuid = @DrawingGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
