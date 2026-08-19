-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMailServerConnectMode
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblMailServerConnectMode]
@MailServerConnectModeIndex tinyint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMailServerConnectMode].[MailServerConnectModeIndex],[lookup].[tblMailServerConnectMode].[MailServerConnectModeCode],[lookup].[tblMailServerConnectMode].[MailServerConnectModeName],[lookup].[tblMailServerConnectMode].[MailServerConnectModeGuid],[lookup].[tblMailServerConnectMode].[CreatedDate],[lookup].[tblMailServerConnectMode].[CreatedBy],[lookup].[tblMailServerConnectMode].[UpdatedDate],[lookup].[tblMailServerConnectMode].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMailServerConnectMode]
            INNER JOIN [track].[tblMailServerConnectMode] CT
                ON CT.PK_MailServerConnectModeIndex = [lookup].[tblMailServerConnectMode].[MailServerConnectModeIndex]
        WHERE CT.PK_MailServerConnectModeIndex = @MailServerConnectModeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
