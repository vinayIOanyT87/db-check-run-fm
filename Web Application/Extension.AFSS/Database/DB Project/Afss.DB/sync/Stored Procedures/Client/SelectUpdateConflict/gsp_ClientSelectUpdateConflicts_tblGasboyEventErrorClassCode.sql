-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblGasboyEventErrorClassCode
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyEventErrorClassCode]
@GasboyEventErrorClassCodeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCode],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeName],[lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeGuid],[lookup].[tblGasboyEventErrorClassCode].[CreatedBy],[lookup].[tblGasboyEventErrorClassCode].[CreatedDate],[lookup].[tblGasboyEventErrorClassCode].[UpdatedBy],[lookup].[tblGasboyEventErrorClassCode].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblGasboyEventErrorClassCode]
            INNER JOIN [track].[tblGasboyEventErrorClassCode] CT
                ON CT.PK_GasboyEventErrorClassCodeIndex = [lookup].[tblGasboyEventErrorClassCode].[GasboyEventErrorClassCodeIndex]
        WHERE CT.PK_GasboyEventErrorClassCodeIndex = @GasboyEventErrorClassCodeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END