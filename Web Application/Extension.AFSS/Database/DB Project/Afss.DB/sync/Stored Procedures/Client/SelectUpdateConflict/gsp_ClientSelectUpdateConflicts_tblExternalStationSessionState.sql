-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationSessionState
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblExternalStationSessionState]
@ExternalStationSessionStateIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationSessionState].[ExternalStationSessionStateIndex],[lookup].[tblExternalStationSessionState].[ExternalStationSessionStateCode],[lookup].[tblExternalStationSessionState].[ExternalStationSessionStateName],[lookup].[tblExternalStationSessionState].[ExternalStationSessionStateGuid],[lookup].[tblExternalStationSessionState].[LongDescription],[lookup].[tblExternalStationSessionState].[CreatedBy],[lookup].[tblExternalStationSessionState].[CreatedDate],[lookup].[tblExternalStationSessionState].[UpdatedBy],[lookup].[tblExternalStationSessionState].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationSessionState]
            INNER JOIN [track].[tblExternalStationSessionState] CT
                ON CT.PK_ExternalStationSessionStateIndex = [lookup].[tblExternalStationSessionState].[ExternalStationSessionStateIndex]
        WHERE CT.PK_ExternalStationSessionStateIndex = @ExternalStationSessionStateIndex
    ORDER BY CT.UpdatedRowVersion ASC
END