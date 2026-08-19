-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEventErrorClassCode
-- Description:	Apply Deletes
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalDeletes_tblGasboyEventErrorClassCode]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@GasboyEventErrorClassCodeIndex int,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DELETE [lookup].[tblGasboyEventErrorClassCode]  
        FROM [lookup].[tblGasboyEventErrorClassCode]  
            INNER JOIN [track].[tblGasboyEventErrorClassCode] CT
                ON CT.PK_GasboyEventErrorClassCodeIndex = [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex]
        WHERE GasboyEventErrorClassCodeIndex = @GasboyEventErrorClassCodeIndex
            AND (@sync_force_write = 1 
            OR (DeletedRowVersion IS NULL OR DeletedRowVersion <= @sync_last_received_anchor))

    SET @sync_row_count = @@rowcount; 
    DECLARE @minValidVersion BigInt 
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.

    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CD)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END