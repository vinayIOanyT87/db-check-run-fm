-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTanks
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblTanks]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TankID nvarchar(50),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@TankGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupVesselTypeIndex int,
@ManagerCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@HiddenDate datetimeoffset(7),
@AssetTrackingDeviceGuid uniqueidentifier,
@LookupDeviceTankTypeIndex int,
@Latitude float,
@Longitude float,
@TankConfigurationNumber int,
@Zoom int,
@OwnerCompanyGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTanks varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTanks] CT
                        WHERE CT.PK_TankGuid = @TankGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTanks].[TankID],[dbo].[tblTanks].[CreatedDate],[dbo].[tblTanks].[CreatedBy],[dbo].[tblTanks].[UpdatedDate],[dbo].[tblTanks].[UpdatedBy],[dbo].[tblTanks].[TankGuid],[dbo].[tblTanks].[SiteGuid],[dbo].[tblTanks].[LookupVesselTypeIndex],[dbo].[tblTanks].[ManagerCompanyGuid],[dbo].[tblTanks].[ProductGuid],[dbo].[tblTanks].[HiddenDate],[dbo].[tblTanks].[AssetTrackingDeviceGuid],[dbo].[tblTanks].[LookupDeviceTankTypeIndex],[dbo].[tblTanks].[Latitude],[dbo].[tblTanks].[Longitude],[dbo].[tblTanks].[TankConfigurationNumber],[dbo].[tblTanks].[Zoom],[dbo].[tblTanks].[OwnerCompanyGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTanks]
                        INNER JOIN [track].[tblTanks] CT
                            ON CT.PK_TankGuid = [dbo].[tblTanks].[TankGuid] 
                    WHERE CT.PK_TankGuid = @TankGuid
            ) MERGE existingData
            USING (SELECT @TankID,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TankGuid,@SiteGuid,@LookupVesselTypeIndex,@ManagerCompanyGuid,@ProductGuid,@HiddenDate,@AssetTrackingDeviceGuid,@LookupDeviceTankTypeIndex,@Latitude,@Longitude,@TankConfigurationNumber,@Zoom,@OwnerCompanyGuid
                    ) AS remoteChanges ([TankID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TankGuid],[SiteGuid],[LookupVesselTypeIndex],[ManagerCompanyGuid],[ProductGuid],[HiddenDate],[AssetTrackingDeviceGuid],[LookupDeviceTankTypeIndex],[Latitude],[Longitude],[TankConfigurationNumber],[Zoom],[OwnerCompanyGuid])
            ON (existingData.[TankGuid] = remoteChanges.[TankGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [TankID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankID'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[TankID] ELSE remoteChanges.[TankID] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupVesselTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupVesselTypeIndex'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[LookupVesselTypeIndex] ELSE remoteChanges.[LookupVesselTypeIndex] END
                       ,[ManagerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[ManagerCompanyGuid] ELSE remoteChanges.[ManagerCompanyGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END
                       ,[AssetTrackingDeviceGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetTrackingDeviceGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[AssetTrackingDeviceGuid] ELSE remoteChanges.[AssetTrackingDeviceGuid] END
                       ,[LookupDeviceTankTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupDeviceTankTypeIndex'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[LookupDeviceTankTypeIndex] ELSE remoteChanges.[LookupDeviceTankTypeIndex] END
                       ,[Latitude] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Latitude'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[Latitude] ELSE remoteChanges.[Latitude] END
                       ,[Longitude] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Longitude'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[Longitude] ELSE remoteChanges.[Longitude] END
                       ,[TankConfigurationNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankConfigurationNumber'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[TankConfigurationNumber] ELSE remoteChanges.[TankConfigurationNumber] END
                       ,[Zoom] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zoom'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[Zoom] ELSE remoteChanges.[Zoom] END
                       ,[OwnerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN existingData.[OwnerCompanyGuid] ELSE remoteChanges.[OwnerCompanyGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([TankID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TankGuid],[SiteGuid],[LookupVesselTypeIndex],[ManagerCompanyGuid],[ProductGuid],[HiddenDate],[AssetTrackingDeviceGuid],[LookupDeviceTankTypeIndex],[Latitude],[Longitude],[TankConfigurationNumber],[Zoom],[OwnerCompanyGuid])
                    VALUES (@TankID,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TankGuid,@SiteGuid,@LookupVesselTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @ManagerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @ProductGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @HiddenDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetTrackingDeviceGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @AssetTrackingDeviceGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupDeviceTankTypeIndex'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @LookupDeviceTankTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Latitude'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @Latitude END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Longitude'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @Longitude END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankConfigurationNumber'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @TankConfigurationNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zoom'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @Zoom END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblTanks)) WHEN 0 THEN NULL ELSE @OwnerCompanyGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TankGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTanks] WHERE TankGuid = @TankGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
