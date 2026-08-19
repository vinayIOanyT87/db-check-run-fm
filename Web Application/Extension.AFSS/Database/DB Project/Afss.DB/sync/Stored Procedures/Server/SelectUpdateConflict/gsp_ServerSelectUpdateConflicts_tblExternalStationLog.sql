-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExternalStationLog
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExternalStationLog]
@ExternalStationLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExternalStationLog].[ExternalStationLogGuid],[dbo].[tblExternalStationLog].[SiteGuid],[dbo].[tblExternalStationLog].[ExternalStationGuid],[dbo].[tblExternalStationLog].[LogText],[dbo].[tblExternalStationLog].[LookupExternalStationLogTypeIndex],[dbo].[tblExternalStationLog].[LogDate],[dbo].[tblExternalStationLog].[CreatedBy],[dbo].[tblExternalStationLog].[CreatedDate],[dbo].[tblExternalStationLog].[UpdatedBy],[dbo].[tblExternalStationLog].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExternalStationLog]
            INNER JOIN [track].[tblExternalStationLog] CT
                ON CT.PK_ExternalStationLogGuid = [dbo].[tblExternalStationLog].[ExternalStationLogGuid]
        WHERE CT.PK_ExternalStationLogGuid = @ExternalStationLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END