-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGeneralConfiguration
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblGeneralConfiguration]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Method int,
@ConsortiumFlag bit,
@ShowDeletedTrxFlag bit,
@AllowUndeleteFlag bit,
@ReverseTrxDateMode nvarchar(15),
@ForcedCloseout int,
@SecurityCode nvarchar(50),
@AuthorizationCode nvarchar(50),
@MeterTolerance float,
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@SetBeginInventoryToZeroFlag bit,
@GeneralConfigurationGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblGeneralConfiguration varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblGeneralConfiguration] CT
                        WHERE CT.PK_GeneralConfigurationGuid = @GeneralConfigurationGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblGeneralConfiguration].[Method],[dbo].[tblGeneralConfiguration].[ConsortiumFlag],[dbo].[tblGeneralConfiguration].[ShowDeletedTrxFlag],[dbo].[tblGeneralConfiguration].[AllowUndeleteFlag],[dbo].[tblGeneralConfiguration].[ReverseTrxDateMode],[dbo].[tblGeneralConfiguration].[ForcedCloseout],[dbo].[tblGeneralConfiguration].[SecurityCode],[dbo].[tblGeneralConfiguration].[AuthorizationCode],[dbo].[tblGeneralConfiguration].[MeterTolerance],[dbo].[tblGeneralConfiguration].[CreatedBy],[dbo].[tblGeneralConfiguration].[CreatedDate],[dbo].[tblGeneralConfiguration].[UpdatedBy],[dbo].[tblGeneralConfiguration].[UpdatedDate],[dbo].[tblGeneralConfiguration].[SetBeginInventoryToZeroFlag],[dbo].[tblGeneralConfiguration].[GeneralConfigurationGuid],[dbo].[tblGeneralConfiguration].[SiteGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblGeneralConfiguration]
                        INNER JOIN [track].[tblGeneralConfiguration] CT
                            ON CT.PK_GeneralConfigurationGuid = [dbo].[tblGeneralConfiguration].[GeneralConfigurationGuid] 
                    WHERE CT.PK_GeneralConfigurationGuid = @GeneralConfigurationGuid
            ) MERGE existingData
            USING (SELECT @Method,@ConsortiumFlag,@ShowDeletedTrxFlag,@AllowUndeleteFlag,@ReverseTrxDateMode,@ForcedCloseout,@SecurityCode,@AuthorizationCode,@MeterTolerance,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@SetBeginInventoryToZeroFlag,@GeneralConfigurationGuid,@SiteGuid
                    ) AS remoteChanges ([Method],[ConsortiumFlag],[ShowDeletedTrxFlag],[AllowUndeleteFlag],[ReverseTrxDateMode],[ForcedCloseout],[SecurityCode],[AuthorizationCode],[MeterTolerance],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[SetBeginInventoryToZeroFlag],[GeneralConfigurationGuid],[SiteGuid])
            ON (existingData.[GeneralConfigurationGuid] = remoteChanges.[GeneralConfigurationGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [Method] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Method'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[Method] ELSE remoteChanges.[Method] END
                       ,[ConsortiumFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ConsortiumFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[ConsortiumFlag] ELSE remoteChanges.[ConsortiumFlag] END
                       ,[ShowDeletedTrxFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShowDeletedTrxFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[ShowDeletedTrxFlag] ELSE remoteChanges.[ShowDeletedTrxFlag] END
                       ,[AllowUndeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllowUndeleteFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[AllowUndeleteFlag] ELSE remoteChanges.[AllowUndeleteFlag] END
                       ,[ReverseTrxDateMode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReverseTrxDateMode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[ReverseTrxDateMode] ELSE remoteChanges.[ReverseTrxDateMode] END
                       ,[ForcedCloseout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ForcedCloseout'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[ForcedCloseout] ELSE remoteChanges.[ForcedCloseout] END
                       ,[SecurityCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SecurityCode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[SecurityCode] ELSE remoteChanges.[SecurityCode] END
                       ,[AuthorizationCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuthorizationCode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[AuthorizationCode] ELSE remoteChanges.[AuthorizationCode] END
                       ,[MeterTolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterTolerance'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[MeterTolerance] ELSE remoteChanges.[MeterTolerance] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[SetBeginInventoryToZeroFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SetBeginInventoryToZeroFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[SetBeginInventoryToZeroFlag] ELSE remoteChanges.[SetBeginInventoryToZeroFlag] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([Method],[ConsortiumFlag],[ShowDeletedTrxFlag],[AllowUndeleteFlag],[ReverseTrxDateMode],[ForcedCloseout],[SecurityCode],[AuthorizationCode],[MeterTolerance],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[SetBeginInventoryToZeroFlag],[GeneralConfigurationGuid],[SiteGuid])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Method'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @Method END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ConsortiumFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @ConsortiumFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShowDeletedTrxFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @ShowDeletedTrxFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllowUndeleteFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @AllowUndeleteFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReverseTrxDateMode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @ReverseTrxDateMode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ForcedCloseout'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @ForcedCloseout END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SecurityCode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @SecurityCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AuthorizationCode'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @AuthorizationCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterTolerance'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @MeterTolerance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SetBeginInventoryToZeroFlag'), @sync_supported_columns_tblGeneralConfiguration)) WHEN 0 THEN NULL ELSE @SetBeginInventoryToZeroFlag END),@GeneralConfigurationGuid,@SiteGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GeneralConfigurationGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GeneralConfigurationGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @GeneralConfigurationGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblGeneralConfiguration] WHERE GeneralConfigurationGuid = @GeneralConfigurationGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
