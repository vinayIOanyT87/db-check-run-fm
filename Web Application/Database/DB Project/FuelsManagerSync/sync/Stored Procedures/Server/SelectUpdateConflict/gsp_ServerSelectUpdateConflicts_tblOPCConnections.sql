-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOPCConnections
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblOPCConnections]
@OPCConnectionGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblOPCConnections]
            INNER JOIN [track].[tblOPCConnections] CT
                ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid]
        WHERE CT.PK_OPCConnectionGuid = @OPCConnectionGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
