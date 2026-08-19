-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCards
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblFuelCards]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(50),
@Provider nvarchar(50),
@ActivationStatus int,
@InactivityPeriod int,
@Notes nvarchar(max),
@StatusModifiedDate datetimeoffset(7),
@StatusModifiedBy nvarchar(50),
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@FuelCardGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@BillToCompanyGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@OwnerCompanyGuid uniqueidentifier,
@ShipperCompanyGuid uniqueidentifier,
@ShipToCompanyGuid uniqueidentifier,
@ExpirationDate datetimeoffset(7),
@TransientCardFlag bit,
@PIN varbinary(256),
@ProviderID nvarchar(60),
@FuelCardTypeApplicationStringGuid uniqueidentifier,
@HiddenDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblFuelCards] CT
                        WHERE CT.PK_FuelCardGuid = @FuelCardGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblFuelCards].[ID],[dbo].[tblFuelCards].[Provider],[dbo].[tblFuelCards].[ActivationStatus],[dbo].[tblFuelCards].[InactivityPeriod],[dbo].[tblFuelCards].[Notes],[dbo].[tblFuelCards].[StatusModifiedDate],[dbo].[tblFuelCards].[StatusModifiedBy],[dbo].[tblFuelCards].[UserData1],[dbo].[tblFuelCards].[UserData2],[dbo].[tblFuelCards].[UserData3],[dbo].[tblFuelCards].[UserData4],[dbo].[tblFuelCards].[UserData5],[dbo].[tblFuelCards].[UserData6],[dbo].[tblFuelCards].[UserData7],[dbo].[tblFuelCards].[UserData8],[dbo].[tblFuelCards].[CreatedDate],[dbo].[tblFuelCards].[CreatedBy],[dbo].[tblFuelCards].[UpdatedDate],[dbo].[tblFuelCards].[UpdatedBy],[dbo].[tblFuelCards].[FuelCardGuid],[dbo].[tblFuelCards].[SiteGuid],[dbo].[tblFuelCards].[BillToCompanyGuid],[dbo].[tblFuelCards].[ManagerCompanyGuid],[dbo].[tblFuelCards].[OwnerCompanyGuid],[dbo].[tblFuelCards].[ShipperCompanyGuid],[dbo].[tblFuelCards].[ShipToCompanyGuid],[dbo].[tblFuelCards].[ExpirationDate],[dbo].[tblFuelCards].[TransientCardFlag],[dbo].[tblFuelCards].[PIN],[dbo].[tblFuelCards].[ProviderID],[dbo].[tblFuelCards].[FuelCardTypeApplicationStringGuid],[dbo].[tblFuelCards].[HiddenDate]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblFuelCards]
                        INNER JOIN [track].[tblFuelCards] CT
                            ON CT.PK_FuelCardGuid = [dbo].[tblFuelCards].[FuelCardGuid] 
                    WHERE CT.PK_FuelCardGuid = @FuelCardGuid
            ) MERGE existingData
            USING (SELECT @ID,@Provider,@ActivationStatus,@InactivityPeriod,@Notes,@StatusModifiedDate,@StatusModifiedBy,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@FuelCardGuid,@SiteGuid,@BillToCompanyGuid,@ManagerCompanyGuid,@OwnerCompanyGuid,@ShipperCompanyGuid,@ShipToCompanyGuid,@ExpirationDate,@TransientCardFlag,@PIN,@ProviderID,@FuelCardTypeApplicationStringGuid,@HiddenDate
                    ) AS remoteChanges ([ID],[Provider],[ActivationStatus],[InactivityPeriod],[Notes],[StatusModifiedDate],[StatusModifiedBy],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[FuelCardGuid],[SiteGuid],[BillToCompanyGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[ExpirationDate],[TransientCardFlag],[PIN],[ProviderID],[FuelCardTypeApplicationStringGuid],[HiddenDate])
            ON (existingData.[FuelCardGuid] = remoteChanges.[FuelCardGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Provider] = remoteChanges.[Provider]
                       ,[ActivationStatus] = remoteChanges.[ActivationStatus]
                       ,[InactivityPeriod] = remoteChanges.[InactivityPeriod]
                       ,[Notes] = remoteChanges.[Notes]
                       ,[StatusModifiedDate] = remoteChanges.[StatusModifiedDate]
                       ,[StatusModifiedBy] = remoteChanges.[StatusModifiedBy]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[BillToCompanyGuid] = remoteChanges.[BillToCompanyGuid]
                       ,[ManagerCompanyGuid] = remoteChanges.[ManagerCompanyGuid]
                       ,[OwnerCompanyGuid] = remoteChanges.[OwnerCompanyGuid]
                       ,[ShipperCompanyGuid] = remoteChanges.[ShipperCompanyGuid]
                       ,[ShipToCompanyGuid] = remoteChanges.[ShipToCompanyGuid]
                       ,[ExpirationDate] = remoteChanges.[ExpirationDate]
                       ,[TransientCardFlag] = remoteChanges.[TransientCardFlag]
                       ,[PIN] = remoteChanges.[PIN]
                       ,[ProviderID] = remoteChanges.[ProviderID]
                       ,[FuelCardTypeApplicationStringGuid] = remoteChanges.[FuelCardTypeApplicationStringGuid]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]

            WHEN NOT MATCHED THEN
                INSERT ([ID],[Provider],[ActivationStatus],[InactivityPeriod],[Notes],[StatusModifiedDate],[StatusModifiedBy],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[FuelCardGuid],[SiteGuid],[BillToCompanyGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[ExpirationDate],[TransientCardFlag],[PIN],[ProviderID],[FuelCardTypeApplicationStringGuid],[HiddenDate])
                    VALUES (@ID,@Provider,@ActivationStatus,@InactivityPeriod,@Notes,@StatusModifiedDate,@StatusModifiedBy,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@FuelCardGuid,@SiteGuid,@BillToCompanyGuid,@ManagerCompanyGuid,@OwnerCompanyGuid,@ShipperCompanyGuid,@ShipToCompanyGuid,@ExpirationDate,@TransientCardFlag,@PIN,@ProviderID,@FuelCardTypeApplicationStringGuid,@HiddenDate)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FuelCardGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FuelCardGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @FuelCardGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblFuelCards] WHERE FuelCardGuid = @FuelCardGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
