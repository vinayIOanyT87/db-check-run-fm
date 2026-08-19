-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblPersonnelRole
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPersonnelRole]
@PersonnelRoleIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblPersonnelRole].[PersonnelRoleIndex],[lookup].[tblPersonnelRole].[PersonnelRoleCode],[lookup].[tblPersonnelRole].[PersonnelRoleName],[lookup].[tblPersonnelRole].[PersonnelRoleGuid],[lookup].[tblPersonnelRole].[CreatedDate],[lookup].[tblPersonnelRole].[CreatedBy],[lookup].[tblPersonnelRole].[UpdatedDate],[lookup].[tblPersonnelRole].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblPersonnelRole]
            INNER JOIN [track].[tblPersonnelRole] CT
                ON CT.PK_PersonnelRoleIndex = [lookup].[tblPersonnelRole].[PersonnelRoleIndex]
        WHERE CT.PK_PersonnelRoleIndex = @PersonnelRoleIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
