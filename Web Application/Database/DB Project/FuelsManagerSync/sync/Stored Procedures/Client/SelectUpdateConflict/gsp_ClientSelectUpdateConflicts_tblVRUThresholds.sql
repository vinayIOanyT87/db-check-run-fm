-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblVRUThresholds
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblVRUThresholds]
@VRUThresholdGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblVRUThresholds].[VRUThresholdGuid],[dbo].[tblVRUThresholds].[ID],[dbo].[tblVRUThresholds].[SiteGuid],[dbo].[tblVRUThresholds].[Interval],[dbo].[tblVRUThresholds].[IntervalType],[dbo].[tblVRUThresholds].[Limit],[dbo].[tblVRUThresholds].[Tolerance],[dbo].[tblVRUThresholds].[Enabled],[dbo].[tblVRUThresholds].[ResetDate],[dbo].[tblVRUThresholds].[CurrentValue],[dbo].[tblVRUThresholds].[CreatedDate],[dbo].[tblVRUThresholds].[CreatedBy],[dbo].[tblVRUThresholds].[UpdatedDate],[dbo].[tblVRUThresholds].[UpdatedBy],[dbo].[tblVRUThresholds].[LastCalculationDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblVRUThresholds]
            INNER JOIN [track].[tblVRUThresholds] CT
                ON CT.PK_VRUThresholdGuid = [dbo].[tblVRUThresholds].[VRUThresholdGuid]
        WHERE CT.PK_VRUThresholdGuid = @VRUThresholdGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
