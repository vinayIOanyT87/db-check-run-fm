-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblActivationStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblActivationStatus]
@ActivationStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblActivationStatus].[ActivationStatusIndex],[lookup].[tblActivationStatus].[ActivationStatusCode],[lookup].[tblActivationStatus].[ActivationStatusName],[lookup].[tblActivationStatus].[ActivationStatusGuid],[lookup].[tblActivationStatus].[CreatedDate],[lookup].[tblActivationStatus].[CreatedBy],[lookup].[tblActivationStatus].[UpdatedDate],[lookup].[tblActivationStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblActivationStatus]
            INNER JOIN [track].[tblActivationStatus] CT
                ON CT.PK_ActivationStatusIndex = [lookup].[tblActivationStatus].[ActivationStatusIndex]
        WHERE CT.PK_ActivationStatusIndex = @ActivationStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
