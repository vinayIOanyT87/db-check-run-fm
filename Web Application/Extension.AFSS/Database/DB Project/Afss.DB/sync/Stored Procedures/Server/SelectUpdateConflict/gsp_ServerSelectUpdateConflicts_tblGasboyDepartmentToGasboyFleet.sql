-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblGasboyDepartmentToGasboyFleet
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyDepartmentToGasboyFleet]
@GasboyDepartmentToGasboyFleetGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblGasboyDepartmentToGasboyFleet].[GasboyDepartmentToGasboyFleetGuid],[map].[tblGasboyDepartmentToGasboyFleet].[GasboyFleetGuid],[map].[tblGasboyDepartmentToGasboyFleet].[GasboyDepartmentGuid],[map].[tblGasboyDepartmentToGasboyFleet].[CreatedBy],[map].[tblGasboyDepartmentToGasboyFleet].[CreatedDate],[map].[tblGasboyDepartmentToGasboyFleet].[UpdatedBy],[map].[tblGasboyDepartmentToGasboyFleet].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblGasboyDepartmentToGasboyFleet]
            INNER JOIN [track].[tblGasboyDepartmentToGasboyFleet] CT
                ON CT.PK_GasboyDepartmentToGasboyFleetGuid = [map].[tblGasboyDepartmentToGasboyFleet].[GasboyDepartmentToGasboyFleetGuid]
        WHERE CT.PK_GasboyDepartmentToGasboyFleetGuid = @GasboyDepartmentToGasboyFleetGuid
    ORDER BY CT.UpdatedRowVersion ASC
END