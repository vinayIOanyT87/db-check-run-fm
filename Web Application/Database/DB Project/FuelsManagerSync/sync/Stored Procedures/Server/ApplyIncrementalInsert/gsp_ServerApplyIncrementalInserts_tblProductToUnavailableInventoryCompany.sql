-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToUnavailableInventoryCompany
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblProductToUnavailableInventoryCompany]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductToUnavailableInventoryCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToCompanyGuid uniqueidentifier,
@Sequence int,
@BlendPercentage float,
@AdditiveRate float,
@Ratio float,
@AdditiveCycleVolume float,
@Tolerance float,
@PresetNumber int,
@AdditiveProfileGuid uniqueidentifier,
@TankGuid uniqueidentifier,
@MeterID nvarchar(20),
@ShipToProductID nvarchar(30),
@ShipToProductCode nvarchar(15),
@ShipToLoadRackDisplayText nvarchar(10),
@UnavailableInventoryGross float,
@UnavailableInventoryNet float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblProductToUnavailableInventoryCompany varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [map].[tblProductToUnavailableInventoryCompany] AS existingData
        USING (SELECT @ProductToUnavailableInventoryCompanyGuid 'ProductToUnavailableInventoryCompanyGuid',@ProductGuid 'ProductGuid',@AssignedToCompanyGuid 'AssignedToCompanyGuid',@Sequence 'Sequence',@BlendPercentage 'BlendPercentage',@AdditiveRate 'AdditiveRate',@Ratio 'Ratio',@AdditiveCycleVolume 'AdditiveCycleVolume',@Tolerance 'Tolerance',@PresetNumber 'PresetNumber',@AdditiveProfileGuid 'AdditiveProfileGuid',@TankGuid 'TankGuid',@MeterID 'MeterID',@ShipToProductID 'ShipToProductID',@ShipToProductCode 'ShipToProductCode',@ShipToLoadRackDisplayText 'ShipToLoadRackDisplayText',@UnavailableInventoryGross 'UnavailableInventoryGross',@UnavailableInventoryNet 'UnavailableInventoryNet',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy'
                ) AS remoteChanges ([ProductToUnavailableInventoryCompanyGuid],[ProductGuid],[AssignedToCompanyGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
        ON (existingData.[ProductToUnavailableInventoryCompanyGuid] = remoteChanges.[ProductToUnavailableInventoryCompanyGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[AssignedToCompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedToCompanyGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[AssignedToCompanyGuid] ELSE remoteChanges.[AssignedToCompanyGuid] END
                       ,[Sequence] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Sequence'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[Sequence] ELSE remoteChanges.[Sequence] END
                       ,[BlendPercentage] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BlendPercentage'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[BlendPercentage] ELSE remoteChanges.[BlendPercentage] END
                       ,[AdditiveRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveRate'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[AdditiveRate] ELSE remoteChanges.[AdditiveRate] END
                       ,[Ratio] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Ratio'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[Ratio] ELSE remoteChanges.[Ratio] END
                       ,[AdditiveCycleVolume] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveCycleVolume'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[AdditiveCycleVolume] ELSE remoteChanges.[AdditiveCycleVolume] END
                       ,[Tolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tolerance'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[Tolerance] ELSE remoteChanges.[Tolerance] END
                       ,[PresetNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PresetNumber'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[PresetNumber] ELSE remoteChanges.[PresetNumber] END
                       ,[AdditiveProfileGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[AdditiveProfileGuid] ELSE remoteChanges.[AdditiveProfileGuid] END
                       ,[TankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[TankGuid] ELSE remoteChanges.[TankGuid] END
                       ,[MeterID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[MeterID] ELSE remoteChanges.[MeterID] END
                       ,[ShipToProductID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductID'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[ShipToProductID] ELSE remoteChanges.[ShipToProductID] END
                       ,[ShipToProductCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductCode'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[ShipToProductCode] ELSE remoteChanges.[ShipToProductCode] END
                       ,[ShipToLoadRackDisplayText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToLoadRackDisplayText'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[ShipToLoadRackDisplayText] ELSE remoteChanges.[ShipToLoadRackDisplayText] END
                       ,[UnavailableInventoryGross] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryGross'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[UnavailableInventoryGross] ELSE remoteChanges.[UnavailableInventoryGross] END
                       ,[UnavailableInventoryNet] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryNet'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[UnavailableInventoryNet] ELSE remoteChanges.[UnavailableInventoryNet] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END

        WHEN NOT MATCHED THEN
            INSERT ([ProductToUnavailableInventoryCompanyGuid],[ProductGuid],[AssignedToCompanyGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
                VALUES (@ProductToUnavailableInventoryCompanyGuid,@ProductGuid,@AssignedToCompanyGuid,@Sequence,@BlendPercentage,@AdditiveRate,@Ratio,@AdditiveCycleVolume,@Tolerance,@PresetNumber,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AdditiveProfileGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @AdditiveProfileGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankGuid'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @TankGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @MeterID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductID'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @ShipToProductID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToProductCode'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @ShipToProductCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShipToLoadRackDisplayText'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @ShipToLoadRackDisplayText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryGross'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @UnavailableInventoryGross END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UnavailableInventoryNet'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @UnavailableInventoryNet END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProductToUnavailableInventoryCompany)) WHEN 0 THEN NULL ELSE @UpdatedBy END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToUnavailableInventoryCompanyGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToUnavailableInventoryCompanyGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductToUnavailableInventoryCompanyGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [map].[tblProductToUnavailableInventoryCompany] WHERE ProductToUnavailableInventoryCompanyGuid = @ProductToUnavailableInventoryCompanyGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

