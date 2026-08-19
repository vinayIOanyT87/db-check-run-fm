-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPersonnelToRole
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPersonnelToRole]
@PersonnelToRoleGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPersonnelToRole].[PersonnelToRoleGuid],[map].[tblPersonnelToRole].[PersonnelGuid],[map].[tblPersonnelToRole].[LookupPersonnelRoleIndex],[map].[tblPersonnelToRole].[CreatedDate],[map].[tblPersonnelToRole].[CreatedBy],[map].[tblPersonnelToRole].[UpdatedDate],[map].[tblPersonnelToRole].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPersonnelToRole]
            INNER JOIN [track].[tblPersonnelToRole] CT
                ON CT.PK_PersonnelToRoleGuid = [map].[tblPersonnelToRole].[PersonnelToRoleGuid]
        WHERE CT.PK_PersonnelToRoleGuid = @PersonnelToRoleGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
