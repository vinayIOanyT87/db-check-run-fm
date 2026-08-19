-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalGasboyStation
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalGasboyStation]
@ExternalStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalGasboyStation].[ExternalStationGuid],[dbo].[tblExternalGasboyStation].[SiteCode],[dbo].[tblExternalGasboyStation].[IPAddress],[dbo].[tblExternalGasboyStation].[UserName],[dbo].[tblExternalGasboyStation].[Password],[dbo].[tblExternalGasboyStation].[CreatedBy],[dbo].[tblExternalGasboyStation].[CreatedDate],[dbo].[tblExternalGasboyStation].[UpdatedBy],[dbo].[tblExternalGasboyStation].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalGasboyStation]
            INNER JOIN [track].[tblExternalGasboyStation] CT
                ON CT.PK_ExternalStationGuid = [dbo].[tblExternalGasboyStation].[ExternalStationGuid]
        WHERE CT.PK_ExternalStationGuid = @ExternalStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END