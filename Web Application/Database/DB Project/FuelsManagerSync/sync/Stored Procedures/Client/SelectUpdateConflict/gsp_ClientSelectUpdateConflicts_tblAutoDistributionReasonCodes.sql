-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAutoDistributionReasonCodes
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAutoDistributionReasonCodes]
@AutoDistributionReasonCodeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAutoDistributionReasonCodes].[AutoDistributionReasonCodeGuid],[dbo].[tblAutoDistributionReasonCodes].[SiteGuid],[dbo].[tblAutoDistributionReasonCodes].[ReasonCode],[dbo].[tblAutoDistributionReasonCodes].[Description],[dbo].[tblAutoDistributionReasonCodes].[CreatedDate],[dbo].[tblAutoDistributionReasonCodes].[CreatedBy],[dbo].[tblAutoDistributionReasonCodes].[UpdatedDate],[dbo].[tblAutoDistributionReasonCodes].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAutoDistributionReasonCodes]
            INNER JOIN [track].[tblAutoDistributionReasonCodes] CT
                ON CT.PK_AutoDistributionReasonCodeGuid = [dbo].[tblAutoDistributionReasonCodes].[AutoDistributionReasonCodeGuid]
        WHERE CT.PK_AutoDistributionReasonCodeGuid = @AutoDistributionReasonCodeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
