-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCards
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblFuelCards]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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

    ;   MERGE [dbo].[tblFuelCards] AS existingData
        USING (SELECT @ID 'ID',@Provider 'Provider',@ActivationStatus 'ActivationStatus',@InactivityPeriod 'InactivityPeriod',@Notes 'Notes',@StatusModifiedDate 'StatusModifiedDate',@StatusModifiedBy 'StatusModifiedBy',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@FuelCardGuid 'FuelCardGuid',@SiteGuid 'SiteGuid',@BillToCompanyGuid 'BillToCompanyGuid',@ManagerCompanyGuid 'ManagerCompanyGuid',@OwnerCompanyGuid 'OwnerCompanyGuid',@ShipperCompanyGuid 'ShipperCompanyGuid',@ShipToCompanyGuid 'ShipToCompanyGuid',@ExpirationDate 'ExpirationDate',@TransientCardFlag 'TransientCardFlag',@PIN 'PIN',@ProviderID 'ProviderID',@FuelCardTypeApplicationStringGuid 'FuelCardTypeApplicationStringGuid',@HiddenDate 'HiddenDate'
                ) AS remoteChanges ([ID],[Provider],[ActivationStatus],[InactivityPeriod],[Notes],[StatusModifiedDate],[StatusModifiedBy],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[FuelCardGuid],[SiteGuid],[BillToCompanyGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[ExpirationDate],[TransientCardFlag],[PIN],[ProviderID],[FuelCardTypeApplicationStringGuid],[HiddenDate])
        ON (existingData.[FuelCardGuid] = remoteChanges.[FuelCardGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
