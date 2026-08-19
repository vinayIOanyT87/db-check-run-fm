-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblUserDataType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataType]
@UserDataTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblUserDataType].[UserDataTypeIndex],[lookup].[tblUserDataType].[UserDataTypeCode],[lookup].[tblUserDataType].[UserDataTypeName],[lookup].[tblUserDataType].[UserDataTypeGuid],[lookup].[tblUserDataType].[CreatedDate],[lookup].[tblUserDataType].[CreatedBy],[lookup].[tblUserDataType].[UpdatedDate],[lookup].[tblUserDataType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblUserDataType]
            INNER JOIN [track].[tblUserDataType] CT
                ON CT.PK_UserDataTypeIndex = [lookup].[tblUserDataType].[UserDataTypeIndex]
        WHERE CT.PK_UserDataTypeIndex = @UserDataTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
