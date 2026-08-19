-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOpcUaServer
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblOpcUaServer]
@OpcUaServerGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblOpcUaServer].[OpcUaServerGuid],[dbo].[tblOpcUaServer].[ServerEndPoint],[dbo].[tblOpcUaServer].[SecurityMode],[dbo].[tblOpcUaServer].[SecurityPolicy],[dbo].[tblOpcUaServer].[MessageEncoding],[dbo].[tblOpcUaServer].[UserIdentityMethod],[dbo].[tblOpcUaServer].[UserId],[dbo].[tblOpcUaServer].[UserPassword],[dbo].[tblOpcUaServer].[UserCertificatePath],[dbo].[tblOpcUaServer].[SiteGuid],[dbo].[tblOpcUaServer].[CreatedDate],[dbo].[tblOpcUaServer].[CreatedBy],[dbo].[tblOpcUaServer].[UpdatedDate],[dbo].[tblOpcUaServer].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblOpcUaServer]
            INNER JOIN [track].[tblOpcUaServer] CT
                ON CT.PK_OpcUaServerGuid = [dbo].[tblOpcUaServer].[OpcUaServerGuid]
        WHERE CT.PK_OpcUaServerGuid = @OpcUaServerGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
