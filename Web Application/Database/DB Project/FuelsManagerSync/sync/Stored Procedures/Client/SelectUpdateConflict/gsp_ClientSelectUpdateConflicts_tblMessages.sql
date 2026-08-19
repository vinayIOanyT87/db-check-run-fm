-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMessages
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMessages]
@MessageGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMessages].[ID],[dbo].[tblMessages].[CreatedDate],[dbo].[tblMessages].[CreatedBy],[dbo].[tblMessages].[UpdatedDate],[dbo].[tblMessages].[UpdatedBy],[dbo].[tblMessages].[MessageGuid],[dbo].[tblMessages].[SiteGuid],[dbo].[tblMessages].[LookupFrequencyTypeIndex],[dbo].[tblMessages].[LookupLocationTypeIndex],[dbo].[tblMessages].[CompanyGuid],[dbo].[tblMessages].[PersonnelGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMessages]
            INNER JOIN [track].[tblMessages] CT
                ON CT.PK_MessageGuid = [dbo].[tblMessages].[MessageGuid]
        WHERE CT.PK_MessageGuid = @MessageGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
