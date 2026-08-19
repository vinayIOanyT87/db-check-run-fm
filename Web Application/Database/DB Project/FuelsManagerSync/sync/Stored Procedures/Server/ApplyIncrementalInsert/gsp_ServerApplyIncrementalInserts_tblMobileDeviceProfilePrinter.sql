-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDeviceProfilePrinter
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblMobileDeviceProfilePrinter]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MobileDeviceProfilePrinterGUID uniqueidentifier,
@MobileDeviceProfileGUID uniqueidentifier,
@PrinterID nvarchar(30),
@BaudRate nvarchar(8),
@COMPort nvarchar(4),
@DataBits nvarchar(8),
@StopBits nvarchar(8),
@UseXonXoff nvarchar(8),
@XonChar nvarchar(8),
@XoffChar nvarchar(8),
@BufferSize nvarchar(8),
@Parity nvarchar(12),
@CreatedBy nvarchar(50),
@UpdatedBy nvarchar(50),
@CreatedDate datetimeoffset(7),
@UpdatedDate datetimeoffset(7),
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblMobileDeviceProfilePrinter varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblMobileDeviceProfilePrinter] AS existingData
        USING (SELECT @MobileDeviceProfilePrinterGUID 'MobileDeviceProfilePrinterGUID',@MobileDeviceProfileGUID 'MobileDeviceProfileGUID',@PrinterID 'PrinterID',@BaudRate 'BaudRate',@COMPort 'COMPort',@DataBits 'DataBits',@StopBits 'StopBits',@UseXonXoff 'UseXonXoff',@XonChar 'XonChar',@XoffChar 'XoffChar',@BufferSize 'BufferSize',@Parity 'Parity',@CreatedBy 'CreatedBy',@UpdatedBy 'UpdatedBy',@CreatedDate 'CreatedDate',@UpdatedDate 'UpdatedDate'
                ) AS remoteChanges ([MobileDeviceProfilePrinterGUID],[MobileDeviceProfileGUID],[PrinterID],[BaudRate],[COMPort],[DataBits],[StopBits],[UseXonXoff],[XonChar],[XoffChar],[BufferSize],[Parity],[CreatedBy],[UpdatedBy],[CreatedDate],[UpdatedDate])
        ON (existingData.[MobileDeviceProfilePrinterGUID] = remoteChanges.[MobileDeviceProfilePrinterGUID])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [MobileDeviceProfileGUID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MobileDeviceProfileGUID'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[MobileDeviceProfileGUID] ELSE remoteChanges.[MobileDeviceProfileGUID] END
                       ,[PrinterID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PrinterID'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[PrinterID] ELSE remoteChanges.[PrinterID] END
                       ,[BaudRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BaudRate'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[BaudRate] ELSE remoteChanges.[BaudRate] END
                       ,[COMPort] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('COMPort'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[COMPort] ELSE remoteChanges.[COMPort] END
                       ,[DataBits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataBits'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[DataBits] ELSE remoteChanges.[DataBits] END
                       ,[StopBits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StopBits'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[StopBits] ELSE remoteChanges.[StopBits] END
                       ,[UseXonXoff] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseXonXoff'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[UseXonXoff] ELSE remoteChanges.[UseXonXoff] END
                       ,[XonChar] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('XonChar'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[XonChar] ELSE remoteChanges.[XonChar] END
                       ,[XoffChar] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('XoffChar'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[XoffChar] ELSE remoteChanges.[XoffChar] END
                       ,[BufferSize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BufferSize'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[BufferSize] ELSE remoteChanges.[BufferSize] END
                       ,[Parity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Parity'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[Parity] ELSE remoteChanges.[Parity] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END

        WHEN NOT MATCHED THEN
            INSERT ([MobileDeviceProfilePrinterGUID],[MobileDeviceProfileGUID],[PrinterID],[BaudRate],[COMPort],[DataBits],[StopBits],[UseXonXoff],[XonChar],[XoffChar],[BufferSize],[Parity],[CreatedBy],[UpdatedBy],[CreatedDate],[UpdatedDate])
                VALUES (@MobileDeviceProfilePrinterGUID,@MobileDeviceProfileGUID,@PrinterID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BaudRate'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @BaudRate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('COMPort'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @COMPort END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DataBits'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @DataBits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StopBits'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @StopBits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseXonXoff'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @UseXonXoff END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('XonChar'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @XonChar END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('XoffChar'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @XoffChar END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BufferSize'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @BufferSize END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Parity'), @sync_supported_columns_tblMobileDeviceProfilePrinter)) WHEN 0 THEN NULL ELSE @Parity END),@CreatedBy,@UpdatedBy,@CreatedDate,@UpdatedDate)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfilePrinterGUID) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfilePrinterGUID))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @MobileDeviceProfilePrinterGUID)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblMobileDeviceProfilePrinter] WHERE MobileDeviceProfilePrinterGUID = @MobileDeviceProfilePrinterGUID AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

