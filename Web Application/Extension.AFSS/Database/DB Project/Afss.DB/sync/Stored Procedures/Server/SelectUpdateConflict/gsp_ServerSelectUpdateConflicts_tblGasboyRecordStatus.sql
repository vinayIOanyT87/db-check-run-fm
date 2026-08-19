-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyRecordStatus
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyRecordStatus]
@GasboyRecordStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyRecordStatus].[GasboyRecordStatusIndex],[lookup].[tblGasboyRecordStatus].[GasboyRecordStatusCode],[lookup].[tblGasboyRecordStatus].[GasboyRecordStatusName],[lookup].[tblGasboyRecordStatus].[GasboyRecordStatusGuid],[lookup].[tblGasboyRecordStatus].[CreatedBy],[lookup].[tblGasboyRecordStatus].[CreatedDate],[lookup].[tblGasboyRecordStatus].[UpdatedBy],[lookup].[tblGasboyRecordStatus].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyRecordStatus]
            INNER JOIN [track].[tblGasboyRecordStatus] CT
                ON CT.PK_GasboyRecordStatusIndex = [lookup].[tblGasboyRecordStatus].[GasboyRecordStatusIndex]
        WHERE CT.PK_GasboyRecordStatusIndex = @GasboyRecordStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END