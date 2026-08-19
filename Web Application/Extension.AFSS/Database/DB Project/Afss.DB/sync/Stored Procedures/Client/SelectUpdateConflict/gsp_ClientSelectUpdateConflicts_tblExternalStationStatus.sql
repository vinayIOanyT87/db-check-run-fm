-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationStatus
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblExternalStationStatus]
@ExternalStationStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationStatus].[ExternalStationStatusIndex],[lookup].[tblExternalStationStatus].[ExternalStationStatusCode],[lookup].[tblExternalStationStatus].[ExternalStationStatusName],[lookup].[tblExternalStationStatus].[ExternalStationStatusGuid],[lookup].[tblExternalStationStatus].[CreatedBy],[lookup].[tblExternalStationStatus].[CreatedDate],[lookup].[tblExternalStationStatus].[UpdatedBy],[lookup].[tblExternalStationStatus].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationStatus]
            INNER JOIN [track].[tblExternalStationStatus] CT
                ON CT.PK_ExternalStationStatusIndex = [lookup].[tblExternalStationStatus].[ExternalStationStatusIndex]
        WHERE CT.PK_ExternalStationStatusIndex = @ExternalStationStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END