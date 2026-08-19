-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMessages
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblMessages]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(120),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@MessageGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupFrequencyTypeIndex int,
@LookupLocationTypeIndex int,
@CompanyGuid uniqueidentifier,
@PersonnelGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblMessages varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblMessages] CT
                        WHERE CT.PK_MessageGuid = @MessageGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblMessages].[ID],[dbo].[tblMessages].[CreatedDate],[dbo].[tblMessages].[CreatedBy],[dbo].[tblMessages].[UpdatedDate],[dbo].[tblMessages].[UpdatedBy],[dbo].[tblMessages].[MessageGuid],[dbo].[tblMessages].[SiteGuid],[dbo].[tblMessages].[LookupFrequencyTypeIndex],[dbo].[tblMessages].[LookupLocationTypeIndex],[dbo].[tblMessages].[CompanyGuid],[dbo].[tblMessages].[PersonnelGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblMessages]
                        INNER JOIN [track].[tblMessages] CT
                            ON CT.PK_MessageGuid = [dbo].[tblMessages].[MessageGuid] 
                    WHERE CT.PK_MessageGuid = @MessageGuid
            ) MERGE existingData
            USING (SELECT @ID,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MessageGuid,@SiteGuid,@LookupFrequencyTypeIndex,@LookupLocationTypeIndex,@CompanyGuid,@PersonnelGuid
                    ) AS remoteChanges ([ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MessageGuid],[SiteGuid],[LookupFrequencyTypeIndex],[LookupLocationTypeIndex],[CompanyGuid],[PersonnelGuid])
            ON (existingData.[MessageGuid] = remoteChanges.[MessageGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupFrequencyTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupFrequencyTypeIndex'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[LookupFrequencyTypeIndex] ELSE remoteChanges.[LookupFrequencyTypeIndex] END
                       ,[LookupLocationTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupLocationTypeIndex'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[LookupLocationTypeIndex] ELSE remoteChanges.[LookupLocationTypeIndex] END
                       ,[CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[PersonnelGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelGuid'), @sync_supported_columns_tblMessages)) WHEN 0 THEN existingData.[PersonnelGuid] ELSE remoteChanges.[PersonnelGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MessageGuid],[SiteGuid],[LookupFrequencyTypeIndex],[LookupLocationTypeIndex],[CompanyGuid],[PersonnelGuid])
                    VALUES (@ID,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MessageGuid,@SiteGuid,@LookupFrequencyTypeIndex,@LookupLocationTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblMessages)) WHEN 0 THEN NULL ELSE @CompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelGuid'), @sync_supported_columns_tblMessages)) WHEN 0 THEN NULL ELSE @PersonnelGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MessageGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMessages] WHERE MessageGuid = @MessageGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
