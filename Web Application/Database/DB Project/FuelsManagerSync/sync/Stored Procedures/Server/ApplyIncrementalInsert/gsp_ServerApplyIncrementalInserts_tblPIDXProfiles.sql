-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPIDXProfiles
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblPIDXProfiles]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Type tinyint,
@ID nvarchar(30),
@IPAddress nvarchar(60),
@Port int,
@TerminalID nvarchar(30),
@UserID nvarchar(30),
@Password nvarchar(30),
@Enabled bit,
@LoggingEnabled bit,
@LogFilePath nvarchar(255),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@PIDXProfileGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@Version int,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblPIDXProfiles varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblPIDXProfiles] AS existingData
        USING (SELECT @Type 'Type',@ID 'ID',@IPAddress 'IPAddress',@Port 'Port',@TerminalID 'TerminalID',@UserID 'UserID',@Password 'Password',@Enabled 'Enabled',@LoggingEnabled 'LoggingEnabled',@LogFilePath 'LogFilePath',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@PIDXProfileGuid 'PIDXProfileGuid',@SiteGuid 'SiteGuid',@Version 'Version'
                ) AS remoteChanges ([Type],[ID],[IPAddress],[Port],[TerminalID],[UserID],[Password],[Enabled],[LoggingEnabled],[LogFilePath],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PIDXProfileGuid],[SiteGuid],[Version])
        ON (existingData.[PIDXProfileGuid] = remoteChanges.[PIDXProfileGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [Type] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Type'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[Type] ELSE remoteChanges.[Type] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[IPAddress] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IPAddress'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[IPAddress] ELSE remoteChanges.[IPAddress] END
                       ,[Port] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Port'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[Port] ELSE remoteChanges.[Port] END
                       ,[TerminalID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TerminalID'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[TerminalID] ELSE remoteChanges.[TerminalID] END
                       ,[UserID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserID'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[UserID] ELSE remoteChanges.[UserID] END
                       ,[Password] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Password'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[Password] ELSE remoteChanges.[Password] END
                       ,[Enabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Enabled'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[Enabled] ELSE remoteChanges.[Enabled] END
                       ,[LoggingEnabled] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoggingEnabled'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[LoggingEnabled] ELSE remoteChanges.[LoggingEnabled] END
                       ,[LogFilePath] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogFilePath'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[LogFilePath] ELSE remoteChanges.[LogFilePath] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[Version] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Version'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN existingData.[Version] ELSE remoteChanges.[Version] END

        WHEN NOT MATCHED THEN
            INSERT ([Type],[ID],[IPAddress],[Port],[TerminalID],[UserID],[Password],[Enabled],[LoggingEnabled],[LogFilePath],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PIDXProfileGuid],[SiteGuid],[Version])
                VALUES (@Type,@ID,@IPAddress,@Port,@TerminalID,@UserID,@Password,@Enabled,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoggingEnabled'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN NULL ELSE @LoggingEnabled END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LogFilePath'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN NULL ELSE @LogFilePath END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@PIDXProfileGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Version'), @sync_supported_columns_tblPIDXProfiles)) WHEN 0 THEN NULL ELSE @Version END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PIDXProfileGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PIDXProfileGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @PIDXProfileGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblPIDXProfiles] WHERE PIDXProfileGuid = @PIDXProfileGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

