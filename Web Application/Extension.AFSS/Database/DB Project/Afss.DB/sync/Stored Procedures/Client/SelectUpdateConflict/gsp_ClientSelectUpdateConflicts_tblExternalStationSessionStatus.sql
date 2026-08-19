-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationSessionStatus
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblExternalStationSessionStatus]
@ExternalStationSessionStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationSessionStatus].[ExternalStationSessionStatusIndex],[lookup].[tblExternalStationSessionStatus].[ExternalStationSessionStatusCode],[lookup].[tblExternalStationSessionStatus].[ExternalStationSessionStatusName],[lookup].[tblExternalStationSessionStatus].[ExternalStationSessionStatusGuid],[lookup].[tblExternalStationSessionStatus].[LongDescription],[lookup].[tblExternalStationSessionStatus].[CreatedBy],[lookup].[tblExternalStationSessionStatus].[CreatedDate],[lookup].[tblExternalStationSessionStatus].[UpdatedBy],[lookup].[tblExternalStationSessionStatus].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationSessionStatus]
            INNER JOIN [track].[tblExternalStationSessionStatus] CT
                ON CT.PK_ExternalStationSessionStatusIndex = [lookup].[tblExternalStationSessionStatus].[ExternalStationSessionStatusIndex]
        WHERE CT.PK_ExternalStationSessionStatusIndex = @ExternalStationSessionStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END