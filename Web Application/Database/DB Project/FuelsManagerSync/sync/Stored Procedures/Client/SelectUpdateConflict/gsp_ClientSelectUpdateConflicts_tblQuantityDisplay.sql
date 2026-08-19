-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblQuantityDisplay
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQuantityDisplay]
@QuantityDisplayIndex tinyint
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblQuantityDisplay].[QuantityDisplayIndex],[lookup].[tblQuantityDisplay].[QuantityDisplayCode],[lookup].[tblQuantityDisplay].[QuantityDisplayName],[lookup].[tblQuantityDisplay].[QuantityDisplayGuid],[lookup].[tblQuantityDisplay].[CreatedDate],[lookup].[tblQuantityDisplay].[CreatedBy],[lookup].[tblQuantityDisplay].[UpdatedDate],[lookup].[tblQuantityDisplay].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblQuantityDisplay]
            INNER JOIN [track].[tblQuantityDisplay] CT
                ON CT.PK_QuantityDisplayIndex = [lookup].[tblQuantityDisplay].[QuantityDisplayIndex]
        WHERE CT.PK_QuantityDisplayIndex = @QuantityDisplayIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
