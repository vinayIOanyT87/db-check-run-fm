-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTanks
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTanks]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTanks] AS existingData
        USING (SELECT @TankID 'TankID',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@TankGuid 'TankGuid',@SiteGuid 'SiteGuid',@LookupVesselTypeIndex 'LookupVesselTypeIndex',@ManagerCompanyGuid 'ManagerCompanyGuid',@ProductGuid 'ProductGuid',@HiddenDate 'HiddenDate',@AssetTrackingDeviceGuid 'AssetTrackingDeviceGuid',@LookupDeviceTankTypeIndex 'LookupDeviceTankTypeIndex',@Latitude 'Latitude',@Longitude 'Longitude',@TankConfigurationNumber 'TankConfigurationNumber',@Zoom 'Zoom',@OwnerCompanyGuid 'OwnerCompanyGuid'
                ) AS remoteChanges ([TankID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TankGuid],[SiteGuid],[LookupVesselTypeIndex],[ManagerCompanyGuid],[ProductGuid],[HiddenDate],[AssetTrackingDeviceGuid],[LookupDeviceTankTypeIndex],[Latitude],[Longitude],[TankConfigurationNumber],[Zoom],[OwnerCompanyGuid])
        ON (existingData.[TankGuid] = remoteChanges.[TankGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [TankID] = remoteChanges.[TankID]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupVesselTypeIndex] = remoteChanges.[LookupVesselTypeIndex]
                       ,[ManagerCompanyGuid] = remoteChanges.[ManagerCompanyGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]
                       ,[AssetTrackingDeviceGuid] = remoteChanges.[AssetTrackingDeviceGuid]
                       ,[LookupDeviceTankTypeIndex] = remoteChanges.[LookupDeviceTankTypeIndex]
                       ,[Latitude] = remoteChanges.[Latitude]
                       ,[Longitude] = remoteChanges.[Longitude]
                       ,[TankConfigurationNumber] = remoteChanges.[TankConfigurationNumber]
                       ,[Zoom] = remoteChanges.[Zoom]
                       ,[OwnerCompanyGuid] = remoteChanges.[OwnerCompanyGuid]

        WHEN NOT MATCHED THEN
            INSERT ([TankID],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[TankGuid],[SiteGuid],[LookupVesselTypeIndex],[ManagerCompanyGuid],[ProductGuid],[HiddenDate],[AssetTrackingDeviceGuid],[LookupDeviceTankTypeIndex],[Latitude],[Longitude],[TankConfigurationNumber],[Zoom],[OwnerCompanyGuid])
                VALUES (@TankID,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@TankGuid,@SiteGuid,@LookupVesselTypeIndex,@ManagerCompanyGuid,@ProductGuid,@HiddenDate,@AssetTrackingDeviceGuid,@LookupDeviceTankTypeIndex,@Latitude,@Longitude,@TankConfigurationNumber,@Zoom,@OwnerCompanyGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
