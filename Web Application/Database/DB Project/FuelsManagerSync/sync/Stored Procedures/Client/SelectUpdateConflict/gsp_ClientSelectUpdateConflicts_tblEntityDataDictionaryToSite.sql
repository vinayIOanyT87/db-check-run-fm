-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityDataDictionaryToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityDataDictionaryToSite]
@DataDictionaryToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityDataDictionaryToSite].[DataDictionaryToSiteGuid],[map].[tblEntityDataDictionaryToSite].[OwnerSiteGuid],[map].[tblEntityDataDictionaryToSite].[MapToSiteGuid],[map].[tblEntityDataDictionaryToSite].[CreatedDate],[map].[tblEntityDataDictionaryToSite].[CreatedBy],[map].[tblEntityDataDictionaryToSite].[UpdatedDate],[map].[tblEntityDataDictionaryToSite].[UpdatedBy],[map].[tblEntityDataDictionaryToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityDataDictionaryToSite]
            INNER JOIN [track].[tblEntityDataDictionaryToSite] CT
                ON CT.PK_DataDictionaryToSiteGuid = [map].[tblEntityDataDictionaryToSite].[DataDictionaryToSiteGuid]
        WHERE CT.PK_DataDictionaryToSiteGuid = @DataDictionaryToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
