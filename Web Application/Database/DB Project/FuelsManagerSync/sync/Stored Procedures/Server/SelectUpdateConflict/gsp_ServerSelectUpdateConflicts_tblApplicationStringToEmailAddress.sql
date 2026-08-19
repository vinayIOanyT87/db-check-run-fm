-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToEmailAddress
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToEmailAddress]
@ApplicationStringToEmailAddressGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToEmailAddress].[ApplicationStringToEmailAddressGuid],[map].[tblApplicationStringToEmailAddress].[ApplicationStringGuid],[map].[tblApplicationStringToEmailAddress].[EmailGroupGuid],[map].[tblApplicationStringToEmailAddress].[Sequence],[map].[tblApplicationStringToEmailAddress].[CreatedDate],[map].[tblApplicationStringToEmailAddress].[CreatedBy],[map].[tblApplicationStringToEmailAddress].[UpdatedDate],[map].[tblApplicationStringToEmailAddress].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToEmailAddress]
            INNER JOIN [track].[tblApplicationStringToEmailAddress] CT
                ON CT.PK_ApplicationStringToEmailAddressGuid = [map].[tblApplicationStringToEmailAddress].[ApplicationStringToEmailAddressGuid]
        WHERE CT.PK_ApplicationStringToEmailAddressGuid = @ApplicationStringToEmailAddressGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
