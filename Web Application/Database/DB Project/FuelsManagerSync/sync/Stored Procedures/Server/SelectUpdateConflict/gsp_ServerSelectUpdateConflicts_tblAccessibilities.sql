-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblAccessibilities
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblAccessibilities]
@AccessibilityGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblAccessibilities].[AccessibilityGuid],[lookup].[tblAccessibilities].[ValueType],[lookup].[tblAccessibilities].[ValueRange],[lookup].[tblAccessibilities].[SettingKey],[lookup].[tblAccessibilities].[DefaultSettingValue],[lookup].[tblAccessibilities].[DisplayName],[lookup].[tblAccessibilities].[Description],[lookup].[tblAccessibilities].[CreatedDate],[lookup].[tblAccessibilities].[CreatedBy],[lookup].[tblAccessibilities].[UpdatedDate],[lookup].[tblAccessibilities].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblAccessibilities]
            INNER JOIN [track].[tblAccessibilities] CT
                ON CT.PK_AccessibilityGuid = [lookup].[tblAccessibilities].[AccessibilityGuid]
        WHERE CT.PK_AccessibilityGuid = @AccessibilityGuid
    ORDER BY CT.UpdatedRowVersion ASC
END