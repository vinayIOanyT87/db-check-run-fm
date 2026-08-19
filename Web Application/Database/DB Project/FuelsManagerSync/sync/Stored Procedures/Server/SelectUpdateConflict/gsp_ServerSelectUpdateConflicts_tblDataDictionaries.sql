-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDataDictionaries
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblDataDictionaries]
@DataDictionaryGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblDataDictionaries].[Key],[dbo].[tblDataDictionaries].[Value],[dbo].[tblDataDictionaries].[CreatedDate],[dbo].[tblDataDictionaries].[CreatedBy],[dbo].[tblDataDictionaries].[UpdatedDate],[dbo].[tblDataDictionaries].[UpdatedBy],[dbo].[tblDataDictionaries].[DataDictionaryGuid],[dbo].[tblDataDictionaries].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblDataDictionaries]
            INNER JOIN [track].[tblDataDictionaries] CT
                ON CT.PK_DataDictionaryGuid = [dbo].[tblDataDictionaries].[DataDictionaryGuid]
        WHERE CT.PK_DataDictionaryGuid = @DataDictionaryGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
