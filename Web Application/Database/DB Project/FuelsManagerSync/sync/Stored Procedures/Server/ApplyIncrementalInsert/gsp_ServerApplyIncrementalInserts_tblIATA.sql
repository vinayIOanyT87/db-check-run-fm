-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblIATA
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblIATA]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@IATAID nvarchar(50),
@Name nvarchar(200),
@CountryID nvarchar(50),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@IATAGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@Latitude float,
@Longitude float,
@Zoom int,
@TimeZone nvarchar(100),
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblIATA varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblIATA] AS existingData
        USING (SELECT @IATAID 'IATAID',@Name 'Name',@CountryID 'CountryID',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@IATAGuid 'IATAGuid',@SiteGuid 'SiteGuid',@Latitude 'Latitude',@Longitude 'Longitude',@Zoom 'Zoom',@TimeZone 'TimeZone',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8'
                ) AS remoteChanges ([IATAID],[Name],[CountryID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[IATAGuid],[SiteGuid],[Latitude],[Longitude],[Zoom],[TimeZone],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8])
        ON (existingData.[IATAGuid] = remoteChanges.[IATAGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [IATAID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IATAID'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[IATAID] ELSE remoteChanges.[IATAID] END
                       ,[Name] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Name'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[Name] ELSE remoteChanges.[Name] END
                       ,[CountryID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CountryID'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[CountryID] ELSE remoteChanges.[CountryID] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[Latitude] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Latitude'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[Latitude] ELSE remoteChanges.[Latitude] END
                       ,[Longitude] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Longitude'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[Longitude] ELSE remoteChanges.[Longitude] END
                       ,[Zoom] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zoom'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[Zoom] ELSE remoteChanges.[Zoom] END
                       ,[TimeZone] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TimeZone'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[TimeZone] ELSE remoteChanges.[TimeZone] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblIATA)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END

        WHEN NOT MATCHED THEN
            INSERT ([IATAID],[Name],[CountryID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[IATAGuid],[SiteGuid],[Latitude],[Longitude],[Zoom],[TimeZone],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8])
                VALUES (@IATAID,@Name,@CountryID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @CreatedDate END),@CreatedBy,@UpdatedDate,@UpdatedBy,@IATAGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Latitude'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @Latitude END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Longitude'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @Longitude END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zoom'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @Zoom END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TimeZone'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @TimeZone END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblIATA)) WHEN 0 THEN NULL ELSE @UserData8 END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @IATAGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @IATAGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @IATAGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblIATA] WHERE IATAGuid = @IATAGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

