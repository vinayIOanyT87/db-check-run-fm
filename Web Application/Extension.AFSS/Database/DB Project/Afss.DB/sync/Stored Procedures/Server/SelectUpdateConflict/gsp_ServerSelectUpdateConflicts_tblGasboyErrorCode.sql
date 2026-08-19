-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyErrorCode
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyErrorCode]
@GasboyErrorCodeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyErrorCode].[GasboyErrorCodeIndex],[lookup].[tblGasboyErrorCode].[GasboyErrorCode],[lookup].[tblGasboyErrorCode].[GasboyErrorCodeName],[lookup].[tblGasboyErrorCode].[GasboyErrorCodeGuid],[lookup].[tblGasboyErrorCode].[CreatedBy],[lookup].[tblGasboyErrorCode].[CreatedDate],[lookup].[tblGasboyErrorCode].[UpdatedBy],[lookup].[tblGasboyErrorCode].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyErrorCode]
            INNER JOIN [track].[tblGasboyErrorCode] CT
                ON CT.PK_GasboyErrorCodeIndex = [lookup].[tblGasboyErrorCode].[GasboyErrorCodeIndex]
        WHERE CT.PK_GasboyErrorCodeIndex = @GasboyErrorCodeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END