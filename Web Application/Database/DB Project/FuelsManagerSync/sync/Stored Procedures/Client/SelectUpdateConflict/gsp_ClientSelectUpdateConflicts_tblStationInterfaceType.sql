-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblStationInterfaceType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblStationInterfaceType]
@StationInterfaceTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblStationInterfaceType].[StationInterfaceTypeIndex],[lookup].[tblStationInterfaceType].[StationInterfaceTypeCode],[lookup].[tblStationInterfaceType].[StationInterfaceTypeName],[lookup].[tblStationInterfaceType].[StationInterfaceTypeGuid],[lookup].[tblStationInterfaceType].[CreatedDate],[lookup].[tblStationInterfaceType].[CreatedBy],[lookup].[tblStationInterfaceType].[UpdatedDate],[lookup].[tblStationInterfaceType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblStationInterfaceType]
            INNER JOIN [track].[tblStationInterfaceType] CT
                ON CT.PK_StationInterfaceTypeIndex = [lookup].[tblStationInterfaceType].[StationInterfaceTypeIndex]
        WHERE CT.PK_StationInterfaceTypeIndex = @StationInterfaceTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
