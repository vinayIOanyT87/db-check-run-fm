-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblExternalStationType
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationType]
@ExternalStationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblExternalStationType].[ExternalStationTypeIndex],[lookup].[tblExternalStationType].[ExternalStationTypeCode],[lookup].[tblExternalStationType].[ExternalStationTypeName],[lookup].[tblExternalStationType].[ExternalStationTypeGuid],[lookup].[tblExternalStationType].[CreatedBy],[lookup].[tblExternalStationType].[CreatedDate],[lookup].[tblExternalStationType].[UpdatedBy],[lookup].[tblExternalStationType].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblExternalStationType]
            INNER JOIN [track].[tblExternalStationType] CT
                ON CT.PK_ExternalStationTypeIndex = [lookup].[tblExternalStationType].[ExternalStationTypeIndex]
        WHERE CT.PK_ExternalStationTypeIndex = @ExternalStationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END