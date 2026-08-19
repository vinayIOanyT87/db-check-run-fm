-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblSiteCloseoutTime
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblSiteCloseoutTime]
@SiteCloseoutTimeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblSiteCloseoutTime].[SiteCloseoutTimeGuid],[dbo].[tblSiteCloseoutTime].[EffectiveDate],[dbo].[tblSiteCloseoutTime].[ExpirationDate],[dbo].[tblSiteCloseoutTime].[CloseoutTime],[dbo].[tblSiteCloseoutTime].[PointTagRefDataAsXML],[dbo].[tblSiteCloseoutTime].[CreatedDate],[dbo].[tblSiteCloseoutTime].[CreatedBy],[dbo].[tblSiteCloseoutTime].[UpdatedDate],[dbo].[tblSiteCloseoutTime].[UpdatedBy],[dbo].[tblSiteCloseoutTime].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblSiteCloseoutTime]
            INNER JOIN [track].[tblSiteCloseoutTime] CT
                ON CT.PK_SiteCloseoutTimeGuid = [dbo].[tblSiteCloseoutTime].[SiteCloseoutTimeGuid]
        WHERE CT.PK_SiteCloseoutTimeGuid = @SiteCloseoutTimeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
