-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionTransportLineItems
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblTransactionTransportLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@TransportOrderNumber nvarchar(50),
@TransVersion bigint,
@LocationName nvarchar(30),
@Address1 nvarchar(60),
@Address2 nvarchar(60),
@City nvarchar(60),
@State nvarchar(20),
@Zip nvarchar(11),
@POCName nvarchar(50),
@POCPhone nvarchar(20),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@TransactionTransportLineItemGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTransactionTransportLineItems varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTransactionTransportLineItems] CT
                        WHERE CT.PK_TransactionTransportLineItemGuid = @TransactionTransportLineItemGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTransactionTransportLineItems].[TransportOrderNumber],[dbo].[tblTransactionTransportLineItems].[TransVersion],[dbo].[tblTransactionTransportLineItems].[LocationName],[dbo].[tblTransactionTransportLineItems].[Address1],[dbo].[tblTransactionTransportLineItems].[Address2],[dbo].[tblTransactionTransportLineItems].[City],[dbo].[tblTransactionTransportLineItems].[State],[dbo].[tblTransactionTransportLineItems].[Zip],[dbo].[tblTransactionTransportLineItems].[POCName],[dbo].[tblTransactionTransportLineItems].[POCPhone],[dbo].[tblTransactionTransportLineItems].[CreatedBy],[dbo].[tblTransactionTransportLineItems].[CreatedDate],[dbo].[tblTransactionTransportLineItems].[UpdatedBy],[dbo].[tblTransactionTransportLineItems].[UpdatedDate],[dbo].[tblTransactionTransportLineItems].[TransactionTransportLineItemGuid],[dbo].[tblTransactionTransportLineItems].[TransactionGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTransactionTransportLineItems]
                        INNER JOIN [track].[tblTransactionTransportLineItems] CT
                            ON CT.PK_TransactionTransportLineItemGuid = [dbo].[tblTransactionTransportLineItems].[TransactionTransportLineItemGuid] 
                    WHERE CT.PK_TransactionTransportLineItemGuid = @TransactionTransportLineItemGuid
            ) MERGE existingData
            USING (SELECT @TransportOrderNumber,@TransVersion,@LocationName,@Address1,@Address2,@City,@State,@Zip,@POCName,@POCPhone,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@TransactionTransportLineItemGuid,@TransactionGuid
                    ) AS remoteChanges ([TransportOrderNumber],[TransVersion],[LocationName],[Address1],[Address2],[City],[State],[Zip],[POCName],[POCPhone],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionTransportLineItemGuid],[TransactionGuid])
            ON (existingData.[TransactionTransportLineItemGuid] = remoteChanges.[TransactionTransportLineItemGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [TransportOrderNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransportOrderNumber'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[TransportOrderNumber] ELSE remoteChanges.[TransportOrderNumber] END
                       ,[TransVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[TransVersion] ELSE remoteChanges.[TransVersion] END
                       ,[LocationName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LocationName'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[LocationName] ELSE remoteChanges.[LocationName] END
                       ,[Address1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address1'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[Address1] ELSE remoteChanges.[Address1] END
                       ,[Address2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address2'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[Address2] ELSE remoteChanges.[Address2] END
                       ,[City] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('City'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[City] ELSE remoteChanges.[City] END
                       ,[State] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('State'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[State] ELSE remoteChanges.[State] END
                       ,[Zip] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zip'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[Zip] ELSE remoteChanges.[Zip] END
                       ,[POCName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('POCName'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[POCName] ELSE remoteChanges.[POCName] END
                       ,[POCPhone] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('POCPhone'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[POCPhone] ELSE remoteChanges.[POCPhone] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[TransactionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionGuid'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN existingData.[TransactionGuid] ELSE remoteChanges.[TransactionGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([TransportOrderNumber],[TransVersion],[LocationName],[Address1],[Address2],[City],[State],[Zip],[POCName],[POCPhone],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionTransportLineItemGuid],[TransactionGuid])
                    VALUES (@TransportOrderNumber,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @TransVersion END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LocationName'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @LocationName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address1'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @Address1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Address2'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @Address2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('City'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @City END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('State'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @State END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Zip'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @Zip END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('POCName'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @POCName END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('POCPhone'), @sync_supported_columns_tblTransactionTransportLineItems)) WHEN 0 THEN NULL ELSE @POCPhone END),@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@TransactionTransportLineItemGuid,@TransactionGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionTransportLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionTransportLineItemGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionTransportLineItemGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionTransportLineItems] WHERE TransactionTransportLineItemGuid = @TransactionTransportLineItemGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
