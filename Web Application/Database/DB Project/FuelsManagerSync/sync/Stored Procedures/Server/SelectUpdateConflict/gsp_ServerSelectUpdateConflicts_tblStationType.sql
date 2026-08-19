-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblStationType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblStationType]
@StationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblStationType].[StationTypeIndex],[lookup].[tblStationType].[StationTypeCode],[lookup].[tblStationType].[StationTypeName],[lookup].[tblStationType].[StationTypeGuid],[lookup].[tblStationType].[CreatedDate],[lookup].[tblStationType].[CreatedBy],[lookup].[tblStationType].[UpdatedDate],[lookup].[tblStationType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblStationType]
            INNER JOIN [track].[tblStationType] CT
                ON CT.PK_StationTypeIndex = [lookup].[tblStationType].[StationTypeIndex]
        WHERE CT.PK_StationTypeIndex = @StationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
