-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblIATA
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblIATA]
@IATAGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblIATA].[IATAID],[dbo].[tblIATA].[Name],[dbo].[tblIATA].[CountryID],[dbo].[tblIATA].[CreatedDate],[dbo].[tblIATA].[CreatedBy],[dbo].[tblIATA].[UpdatedDate],[dbo].[tblIATA].[UpdatedBy],[dbo].[tblIATA].[IATAGuid],[dbo].[tblIATA].[SiteGuid],[dbo].[tblIATA].[Latitude],[dbo].[tblIATA].[Longitude],[dbo].[tblIATA].[Zoom],[dbo].[tblIATA].[TimeZone],[dbo].[tblIATA].[UserData1],[dbo].[tblIATA].[UserData2],[dbo].[tblIATA].[UserData3],[dbo].[tblIATA].[UserData4],[dbo].[tblIATA].[UserData5],[dbo].[tblIATA].[UserData6],[dbo].[tblIATA].[UserData7],[dbo].[tblIATA].[UserData8], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblIATA]
            INNER JOIN [track].[tblIATA] CT
                ON CT.PK_IATAGuid = [dbo].[tblIATA].[IATAGuid]
        WHERE CT.PK_IATAGuid = @IATAGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
