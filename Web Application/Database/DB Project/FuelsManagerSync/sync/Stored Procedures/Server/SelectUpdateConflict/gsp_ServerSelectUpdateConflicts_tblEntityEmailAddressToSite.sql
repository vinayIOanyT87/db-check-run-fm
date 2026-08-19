-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEmailAddressToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityEmailAddressToSite]
@EmailAddressToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEmailAddressToSite].[EmailAddressToSiteGuid],[map].[tblEntityEmailAddressToSite].[ApplicationStringGuid],[map].[tblEntityEmailAddressToSite].[SiteGuid],[map].[tblEntityEmailAddressToSite].[CreatedDate],[map].[tblEntityEmailAddressToSite].[CreatedBy],[map].[tblEntityEmailAddressToSite].[UpdatedDate],[map].[tblEntityEmailAddressToSite].[UpdatedBy],[map].[tblEntityEmailAddressToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEmailAddressToSite]
            INNER JOIN [track].[tblEntityEmailAddressToSite] CT
                ON CT.PK_EmailAddressToSiteGuid = [map].[tblEntityEmailAddressToSite].[EmailAddressToSiteGuid]
        WHERE CT.PK_EmailAddressToSiteGuid = @EmailAddressToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
