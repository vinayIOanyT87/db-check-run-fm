-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOwnerCloseout
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblOwnerCloseout]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Site nvarchar(30),
@ManagerName nvarchar(100),
@ProductName nvarchar(30),
@CloseoutDate date,
@OwnerName nvarchar(100),
@GrossBookInventory float,
@NetBookInventory float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@GrossBookPrice float,
@NetBookPrice float,
@TransVersion bigint,
@MassBookInventory float,
@MassBookPrice float,
@OwnerCloseoutGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@OwnerCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblOwnerCloseout varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblOwnerCloseout] CT
                        WHERE CT.PK_OwnerCloseoutGuid = @OwnerCloseoutGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblOwnerCloseout].[Site],[dbo].[tblOwnerCloseout].[ManagerName],[dbo].[tblOwnerCloseout].[ProductName],[dbo].[tblOwnerCloseout].[CloseoutDate],[dbo].[tblOwnerCloseout].[OwnerName],[dbo].[tblOwnerCloseout].[GrossBookInventory],[dbo].[tblOwnerCloseout].[NetBookInventory],[dbo].[tblOwnerCloseout].[CreatedDate],[dbo].[tblOwnerCloseout].[CreatedBy],[dbo].[tblOwnerCloseout].[UpdatedDate],[dbo].[tblOwnerCloseout].[UpdatedBy],[dbo].[tblOwnerCloseout].[GrossBookPrice],[dbo].[tblOwnerCloseout].[NetBookPrice],[dbo].[tblOwnerCloseout].[TransVersion],[dbo].[tblOwnerCloseout].[MassBookInventory],[dbo].[tblOwnerCloseout].[MassBookPrice],[dbo].[tblOwnerCloseout].[OwnerCloseoutGuid],[dbo].[tblOwnerCloseout].[SiteGuid],[dbo].[tblOwnerCloseout].[ManagerCompanyGuid],[dbo].[tblOwnerCloseout].[OwnerCompanyGuid],[dbo].[tblOwnerCloseout].[ProductGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblOwnerCloseout]
                        INNER JOIN [track].[tblOwnerCloseout] CT
                            ON CT.PK_OwnerCloseoutGuid = [dbo].[tblOwnerCloseout].[OwnerCloseoutGuid] 
                    WHERE CT.PK_OwnerCloseoutGuid = @OwnerCloseoutGuid
            ) MERGE existingData
            USING (SELECT @Site,@ManagerName,@ProductName,@CloseoutDate,@OwnerName,@GrossBookInventory,@NetBookInventory,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@GrossBookPrice,@NetBookPrice,@TransVersion,@MassBookInventory,@MassBookPrice,@OwnerCloseoutGuid,@SiteGuid,@ManagerCompanyGuid,@OwnerCompanyGuid,@ProductGuid
                    ) AS remoteChanges ([Site],[ManagerName],[ProductName],[CloseoutDate],[OwnerName],[GrossBookInventory],[NetBookInventory],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[TransVersion],[MassBookInventory],[MassBookPrice],[OwnerCloseoutGuid],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid])
            ON (existingData.[OwnerCloseoutGuid] = remoteChanges.[OwnerCloseoutGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [Site] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Site'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[Site] ELSE remoteChanges.[Site] END
                       ,[ManagerName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerName'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[ManagerName] ELSE remoteChanges.[ManagerName] END
                       ,[ProductName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductName'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[ProductName] ELSE remoteChanges.[ProductName] END
                       ,[CloseoutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CloseoutDate'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[CloseoutDate] ELSE remoteChanges.[CloseoutDate] END
                       ,[OwnerName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerName'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[OwnerName] ELSE remoteChanges.[OwnerName] END
                       ,[GrossBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[GrossBookInventory] ELSE remoteChanges.[GrossBookInventory] END
                       ,[NetBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[NetBookInventory] ELSE remoteChanges.[NetBookInventory] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[GrossBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[GrossBookPrice] ELSE remoteChanges.[GrossBookPrice] END
                       ,[NetBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[NetBookPrice] ELSE remoteChanges.[NetBookPrice] END
                       ,[TransVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[TransVersion] ELSE remoteChanges.[TransVersion] END
                       ,[MassBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[MassBookInventory] ELSE remoteChanges.[MassBookInventory] END
                       ,[MassBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[MassBookPrice] ELSE remoteChanges.[MassBookPrice] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ManagerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[ManagerCompanyGuid] ELSE remoteChanges.[ManagerCompanyGuid] END
                       ,[OwnerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[OwnerCompanyGuid] ELSE remoteChanges.[OwnerCompanyGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([Site],[ManagerName],[ProductName],[CloseoutDate],[OwnerName],[GrossBookInventory],[NetBookInventory],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[TransVersion],[MassBookInventory],[MassBookPrice],[OwnerCloseoutGuid],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid])
                    VALUES (@Site,@ManagerName,@ProductName,@CloseoutDate,@OwnerName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @GrossBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @NetBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @GrossBookPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @NetBookPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @TransVersion END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookInventory'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @MassBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookPrice'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @MassBookPrice END),@OwnerCloseoutGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @SiteGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @ManagerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OwnerCompanyGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @OwnerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblOwnerCloseout)) WHEN 0 THEN NULL ELSE @ProductGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblOwnerCloseout] WHERE OwnerCloseoutGuid = @OwnerCloseoutGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
