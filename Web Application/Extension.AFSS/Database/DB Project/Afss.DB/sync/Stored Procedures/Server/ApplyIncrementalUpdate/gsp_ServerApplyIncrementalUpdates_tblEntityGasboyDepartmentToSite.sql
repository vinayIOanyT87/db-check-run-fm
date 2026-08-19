-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityGasboyDepartmentToSite
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblEntityGasboyDepartmentToSite]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyDepartmentToSiteGuid uniqueidentifier,
@GasboyDepartmentGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@AssignedFromSiteGuid uniqueidentifier,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEntityGasboyDepartmentToSite varchar(8000)
AS
BEGIN
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [map].[tblEntityGasboyDepartmentToSite]
                            INNER JOIN [track].[tblEntityGasboyDepartmentToSite] CT
                                ON CT.PK_GasboyDepartmentToSiteGuid = [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid] 
                        WHERE CT.PK_GasboyDepartmentToSiteGuid = @GasboyDepartmentToSiteGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid],[map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentGuid],[map].[tblEntityGasboyDepartmentToSite].[SiteGuid],[map].[tblEntityGasboyDepartmentToSite].[AssignedFromSiteGuid],[map].[tblEntityGasboyDepartmentToSite].[CreatedBy],[map].[tblEntityGasboyDepartmentToSite].[CreatedDate],[map].[tblEntityGasboyDepartmentToSite].[UpdatedBy],[map].[tblEntityGasboyDepartmentToSite].[UpdatedDate]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [map].[tblEntityGasboyDepartmentToSite]
                        INNER JOIN [track].[tblEntityGasboyDepartmentToSite] CT
                            ON CT.PK_GasboyDepartmentToSiteGuid = [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid] 
                    WHERE CT.PK_GasboyDepartmentToSiteGuid = @GasboyDepartmentToSiteGuid
            ) MERGE existingData
            USING (SELECT @GasboyDepartmentToSiteGuid,@GasboyDepartmentGuid,@SiteGuid,@AssignedFromSiteGuid,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate
                    ) AS remoteChanges ([GasboyDepartmentToSiteGuid],[GasboyDepartmentGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            ON (existingData.[GasboyDepartmentToSiteGuid] = remoteChanges.[GasboyDepartmentToSiteGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [GasboyDepartmentGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GasboyDepartmentGuid'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[GasboyDepartmentGuid] ELSE remoteChanges.[GasboyDepartmentGuid] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[AssignedFromSiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedFromSiteGuid'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[AssignedFromSiteGuid] ELSE remoteChanges.[AssignedFromSiteGuid] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEntityGasboyDepartmentToSite)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

            WHEN NOT MATCHED THEN
                INSERT ([GasboyDepartmentToSiteGuid],[GasboyDepartmentGuid],[SiteGuid],[AssignedFromSiteGuid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                    VALUES (@GasboyDepartmentToSiteGuid,@GasboyDepartmentGuid,@SiteGuid,@AssignedFromSiteGuid,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
            ;
    END

    SET @sync_row_count = @@rowcount; 

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentToSiteGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentToSiteGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyDepartmentToSiteGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblEntityGasboyDepartmentToSite] WHERE GasboyDepartmentToSiteGuid = @GasboyDepartmentToSiteGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END
    
    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
