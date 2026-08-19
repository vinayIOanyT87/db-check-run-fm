-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblFuelCards
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblFuelCards]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblFuelCards varchar(8000)
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
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Provider] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Provider'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[Provider] ELSE remoteChanges.[Provider] END
                       ,[ActivationStatus] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActivationStatus'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ActivationStatus] ELSE remoteChanges.[ActivationStatus] END
                       ,[InactivityPeriod] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityPeriod'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[InactivityPeriod] ELSE remoteChanges.[InactivityPeriod] END
                       ,[Notes] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[Notes] ELSE remoteChanges.[Notes] END
                       ,[StatusModifiedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StatusModifiedDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[StatusModifiedDate] ELSE remoteChanges.[StatusModifiedDate] END
                       ,[StatusModifiedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StatusModifiedBy'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[StatusModifiedBy] ELSE remoteChanges.[StatusModifiedBy] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[BillToCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BillToCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[BillToCompanyGuid] ELSE remoteChanges.[BillToCompanyGuid] END
                       ,[ManagerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ManagerCompanyGuid] ELSE remoteChanges.[ManagerCompanyGuid] END
                       ,[OwnerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[OwnerCompanyGuid] ELSE remoteChanges.[OwnerCompanyGuid] END
                       ,[ShipperCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipperCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ShipperCompanyGuid] ELSE remoteChanges.[ShipperCompanyGuid] END
                       ,[ShipToCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ShipToCompanyGuid] ELSE remoteChanges.[ShipToCompanyGuid] END
                       ,[ExpirationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ExpirationDate] ELSE remoteChanges.[ExpirationDate] END
                       ,[TransientCardFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransientCardFlag'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[TransientCardFlag] ELSE remoteChanges.[TransientCardFlag] END
                       ,[PIN] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIN'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[PIN] ELSE remoteChanges.[PIN] END
                       ,[ProviderID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProviderID'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[ProviderID] ELSE remoteChanges.[ProviderID] END
                       ,[FuelCardTypeApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelCardTypeApplicationStringGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[FuelCardTypeApplicationStringGuid] ELSE remoteChanges.[FuelCardTypeApplicationStringGuid] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[Provider],[ActivationStatus],[InactivityPeriod],[Notes],[StatusModifiedDate],[StatusModifiedBy],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[FuelCardGuid],[SiteGuid],[BillToCompanyGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ShipperCompanyGuid],[ShipToCompanyGuid],[ExpirationDate],[TransientCardFlag],[PIN],[ProviderID],[FuelCardTypeApplicationStringGuid],[HiddenDate])
                    VALUES (@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Provider'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @Provider END),@ActivationStatus,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InactivityPeriod'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @InactivityPeriod END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @Notes END),@StatusModifiedDate,@StatusModifiedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @UserData8 END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@FuelCardGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BillToCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @BillToCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @ManagerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @OwnerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipperCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @ShipperCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToCompanyGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @ShipToCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExpirationDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @ExpirationDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransientCardFlag'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @TransientCardFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIN'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @PIN END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProviderID'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @ProviderID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelCardTypeApplicationStringGuid'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @FuelCardTypeApplicationStringGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblFuelCards)) WHEN 0 THEN NULL ELSE @HiddenDate END))
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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
