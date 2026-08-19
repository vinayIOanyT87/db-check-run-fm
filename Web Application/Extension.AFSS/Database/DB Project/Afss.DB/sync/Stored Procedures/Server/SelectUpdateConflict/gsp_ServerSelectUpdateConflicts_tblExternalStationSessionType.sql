-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationSessionType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationSessionType]
@ExternalStationSessionTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationSessionType].[ExternalStationSessionTypeIndex],[lookup].[tblExternalStationSessionType].[ExternalStationSessionTypeCode],[lookup].[tblExternalStationSessionType].[ExternalStationSessionTypeName],[lookup].[tblExternalStationSessionType].[ExternalStationSessionTypeGuid],[lookup].[tblExternalStationSessionType].[LongDescription],[lookup].[tblExternalStationSessionType].[CreatedBy],[lookup].[tblExternalStationSessionType].[CreatedDate],[lookup].[tblExternalStationSessionType].[UpdatedBy],[lookup].[tblExternalStationSessionType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationSessionType]
            INNER JOIN [track].[tblExternalStationSessionType] CT
                ON CT.PK_ExternalStationSessionTypeIndex = [lookup].[tblExternalStationSessionType].[ExternalStationSessionTypeIndex]
        WHERE CT.PK_ExternalStationSessionTypeIndex = @ExternalStationSessionTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END