-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCloseoutInventory
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblCloseoutInventory]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Site nvarchar(30),
@CloseoutDate date,
@ProductName nvarchar(30),
@ManagerName nvarchar(100),
@GrossBookInventory float,
@NetBookInventory float,
@GrossPhysicalInventory float,
@NetPhysicalInventory float,
@GrossVariance float,
@NetVariance float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@GrossBookPrice float,
@NetBookPrice float,
@GrossPhysicalPrice float,
@NetPhysicalPrice float,
@TransVersion bigint,
@MassBookInventory float,
@MassPhysicalInventory float,
@MassVariance float,
@MassBookPrice float,
@MassPhysicalPrice float,
@CloseoutInventoryGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblCloseoutInventory varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblCloseoutInventory] CT
                        WHERE CT.PK_CloseoutInventoryGuid = @CloseoutInventoryGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblCloseoutInventory].[Site],[dbo].[tblCloseoutInventory].[CloseoutDate],[dbo].[tblCloseoutInventory].[ProductName],[dbo].[tblCloseoutInventory].[ManagerName],[dbo].[tblCloseoutInventory].[GrossBookInventory],[dbo].[tblCloseoutInventory].[NetBookInventory],[dbo].[tblCloseoutInventory].[GrossPhysicalInventory],[dbo].[tblCloseoutInventory].[NetPhysicalInventory],[dbo].[tblCloseoutInventory].[GrossVariance],[dbo].[tblCloseoutInventory].[NetVariance],[dbo].[tblCloseoutInventory].[CreatedDate],[dbo].[tblCloseoutInventory].[CreatedBy],[dbo].[tblCloseoutInventory].[UpdatedDate],[dbo].[tblCloseoutInventory].[UpdatedBy],[dbo].[tblCloseoutInventory].[GrossBookPrice],[dbo].[tblCloseoutInventory].[NetBookPrice],[dbo].[tblCloseoutInventory].[GrossPhysicalPrice],[dbo].[tblCloseoutInventory].[NetPhysicalPrice],[dbo].[tblCloseoutInventory].[TransVersion],[dbo].[tblCloseoutInventory].[MassBookInventory],[dbo].[tblCloseoutInventory].[MassPhysicalInventory],[dbo].[tblCloseoutInventory].[MassVariance],[dbo].[tblCloseoutInventory].[MassBookPrice],[dbo].[tblCloseoutInventory].[MassPhysicalPrice],[dbo].[tblCloseoutInventory].[CloseoutInventoryGuid],[dbo].[tblCloseoutInventory].[SiteGuid],[dbo].[tblCloseoutInventory].[ManagerCompanyGuid],[dbo].[tblCloseoutInventory].[ProductGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblCloseoutInventory]
                        INNER JOIN [track].[tblCloseoutInventory] CT
                            ON CT.PK_CloseoutInventoryGuid = [dbo].[tblCloseoutInventory].[CloseoutInventoryGuid] 
                    WHERE CT.PK_CloseoutInventoryGuid = @CloseoutInventoryGuid
            ) MERGE existingData
            USING (SELECT @Site,@CloseoutDate,@ProductName,@ManagerName,@GrossBookInventory,@NetBookInventory,@GrossPhysicalInventory,@NetPhysicalInventory,@GrossVariance,@NetVariance,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@GrossBookPrice,@NetBookPrice,@GrossPhysicalPrice,@NetPhysicalPrice,@TransVersion,@MassBookInventory,@MassPhysicalInventory,@MassVariance,@MassBookPrice,@MassPhysicalPrice,@CloseoutInventoryGuid,@SiteGuid,@ManagerCompanyGuid,@ProductGuid
                    ) AS remoteChanges ([Site],[CloseoutDate],[ProductName],[ManagerName],[GrossBookInventory],[NetBookInventory],[GrossPhysicalInventory],[NetPhysicalInventory],[GrossVariance],[NetVariance],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[GrossPhysicalPrice],[NetPhysicalPrice],[TransVersion],[MassBookInventory],[MassPhysicalInventory],[MassVariance],[MassBookPrice],[MassPhysicalPrice],[CloseoutInventoryGuid],[SiteGuid],[ManagerCompanyGuid],[ProductGuid])
            ON (existingData.[CloseoutInventoryGuid] = remoteChanges.[CloseoutInventoryGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [Site] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Site'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[Site] ELSE remoteChanges.[Site] END
                       ,[CloseoutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CloseoutDate'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[CloseoutDate] ELSE remoteChanges.[CloseoutDate] END
                       ,[ProductName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductName'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[ProductName] ELSE remoteChanges.[ProductName] END
                       ,[ManagerName] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerName'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[ManagerName] ELSE remoteChanges.[ManagerName] END
                       ,[GrossBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[GrossBookInventory] ELSE remoteChanges.[GrossBookInventory] END
                       ,[NetBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[NetBookInventory] ELSE remoteChanges.[NetBookInventory] END
                       ,[GrossPhysicalInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[GrossPhysicalInventory] ELSE remoteChanges.[GrossPhysicalInventory] END
                       ,[NetPhysicalInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[NetPhysicalInventory] ELSE remoteChanges.[NetPhysicalInventory] END
                       ,[GrossVariance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[GrossVariance] ELSE remoteChanges.[GrossVariance] END
                       ,[NetVariance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[NetVariance] ELSE remoteChanges.[NetVariance] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[GrossBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[GrossBookPrice] ELSE remoteChanges.[GrossBookPrice] END
                       ,[NetBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[NetBookPrice] ELSE remoteChanges.[NetBookPrice] END
                       ,[GrossPhysicalPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[GrossPhysicalPrice] ELSE remoteChanges.[GrossPhysicalPrice] END
                       ,[NetPhysicalPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[NetPhysicalPrice] ELSE remoteChanges.[NetPhysicalPrice] END
                       ,[TransVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[TransVersion] ELSE remoteChanges.[TransVersion] END
                       ,[MassBookInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[MassBookInventory] ELSE remoteChanges.[MassBookInventory] END
                       ,[MassPhysicalInventory] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[MassPhysicalInventory] ELSE remoteChanges.[MassPhysicalInventory] END
                       ,[MassVariance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[MassVariance] ELSE remoteChanges.[MassVariance] END
                       ,[MassBookPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[MassBookPrice] ELSE remoteChanges.[MassBookPrice] END
                       ,[MassPhysicalPrice] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[MassPhysicalPrice] ELSE remoteChanges.[MassPhysicalPrice] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ManagerCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[ManagerCompanyGuid] ELSE remoteChanges.[ManagerCompanyGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END

            WHEN NOT MATCHED THEN
                INSERT ([Site],[CloseoutDate],[ProductName],[ManagerName],[GrossBookInventory],[NetBookInventory],[GrossPhysicalInventory],[NetPhysicalInventory],[GrossVariance],[NetVariance],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[GrossPhysicalPrice],[NetPhysicalPrice],[TransVersion],[MassBookInventory],[MassPhysicalInventory],[MassVariance],[MassBookPrice],[MassPhysicalPrice],[CloseoutInventoryGuid],[SiteGuid],[ManagerCompanyGuid],[ProductGuid])
                    VALUES ((CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Site'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @Site END),@CloseoutDate,@ProductName,@ManagerName,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @GrossBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @NetBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @GrossPhysicalInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @NetPhysicalInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @GrossVariance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @NetVariance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @GrossBookPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @NetBookPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @GrossPhysicalPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @NetPhysicalPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @TransVersion END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @MassBookInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPhysicalInventory'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @MassPhysicalInventory END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassVariance'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @MassVariance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassBookPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @MassBookPrice END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPhysicalPrice'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @MassPhysicalPrice END),@CloseoutInventoryGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @SiteGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagerCompanyGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @ManagerCompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblCloseoutInventory)) WHEN 0 THEN NULL ELSE @ProductGuid END))
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblCloseoutInventory] WHERE CloseoutInventoryGuid = @CloseoutInventoryGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
