-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToUserGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointAccessGroupToUserGroup]
@PointAccessGroupToUserGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToUserGroup].[PointAccessGroupToUserGroupGuid],[map].[tblPointAccessGroupToUserGroup].[PointAccessGroupGuid],[map].[tblPointAccessGroupToUserGroup].[UserGroupGuid],[map].[tblPointAccessGroupToUserGroup].[CreatedDate],[map].[tblPointAccessGroupToUserGroup].[CreatedBy],[map].[tblPointAccessGroupToUserGroup].[UpdatedDate],[map].[tblPointAccessGroupToUserGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToUserGroup]
            INNER JOIN [track].[tblPointAccessGroupToUserGroup] CT
                ON CT.PK_PointAccessGroupToUserGroupGuid = [map].[tblPointAccessGroupToUserGroup].[PointAccessGroupToUserGroupGuid]
        WHERE CT.PK_PointAccessGroupToUserGroupGuid = @PointAccessGroupToUserGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
