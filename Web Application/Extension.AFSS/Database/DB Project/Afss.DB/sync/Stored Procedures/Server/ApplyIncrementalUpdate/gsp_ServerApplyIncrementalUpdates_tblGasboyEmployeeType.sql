-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEmployeeType
-- Description:	Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblGasboyEmployeeType]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyEmployeeTypeIndex int,
@GasboyEmployeeTypeCode nvarchar(100),
@GasboyEmployeeTypeName nvarchar(100),
@GasboyEmployeeTypeGuid uniqueidentifier,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblGasboyEmployeeType varchar(8000)
AS
BEGIN
    DECLARE @wasDeleted int
    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [lookup].[tblGasboyEmployeeType]
                            INNER JOIN [track].[tblGasboyEmployeeType] CT
                                ON CT.PK_GasboyEmployeeTypeIndex = [lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeIndex] 
                        WHERE CT.PK_GasboyEmployeeTypeIndex = @GasboyEmployeeTypeIndex
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeIndex],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeCode],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeName],[lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeGuid],[lookup].[tblGasboyEmployeeType].[CreatedBy],[lookup].[tblGasboyEmployeeType].[CreatedDate],[lookup].[tblGasboyEmployeeType].[UpdatedBy],[lookup].[tblGasboyEmployeeType].[UpdatedDate]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [lookup].[tblGasboyEmployeeType]
                        INNER JOIN [track].[tblGasboyEmployeeType] CT
                            ON CT.PK_GasboyEmployeeTypeIndex = [lookup].[tblGasboyEmployeeType].[GasboyEmployeeTypeIndex] 
                    WHERE CT.PK_GasboyEmployeeTypeIndex = @GasboyEmployeeTypeIndex
            ) MERGE existingData
            USING (SELECT @GasboyEmployeeTypeIndex,@GasboyEmployeeTypeCode,@GasboyEmployeeTypeName,@GasboyEmployeeTypeGuid,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate
                    ) AS remoteChanges ([GasboyEmployeeTypeIndex],[GasboyEmployeeTypeCode],[GasboyEmployeeTypeName],[GasboyEmployeeTypeGuid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
            ON (existingData.[GasboyEmployeeTypeIndex] = remoteChanges.[GasboyEmployeeTypeIndex])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [GasboyEmployeeTypeCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GasboyEmployeeTypeCode'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[GasboyEmployeeTypeCode] ELSE remoteChanges.[GasboyEmployeeTypeCode] END
                       ,[GasboyEmployeeTypeName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GasboyEmployeeTypeName'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[GasboyEmployeeTypeName] ELSE remoteChanges.[GasboyEmployeeTypeName] END
                       ,[GasboyEmployeeTypeGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GasboyEmployeeTypeGuid'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[GasboyEmployeeTypeGuid] ELSE remoteChanges.[GasboyEmployeeTypeGuid] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblGasboyEmployeeType)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

            WHEN NOT MATCHED THEN
                INSERT ([GasboyEmployeeTypeIndex],[GasboyEmployeeTypeCode],[GasboyEmployeeTypeName],[GasboyEmployeeTypeGuid],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate])
                    VALUES (@GasboyEmployeeTypeIndex,@GasboyEmployeeTypeCode,@GasboyEmployeeTypeName,@GasboyEmployeeTypeGuid,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate)
            ;
    END

    SET @sync_row_count = @@rowcount; 

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyEmployeeTypeIndex) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyEmployeeTypeIndex))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GasboyEmployeeTypeIndex)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [lookup].[tblGasboyEmployeeType] WHERE GasboyEmployeeTypeIndex = @GasboyEmployeeTypeIndex AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
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
