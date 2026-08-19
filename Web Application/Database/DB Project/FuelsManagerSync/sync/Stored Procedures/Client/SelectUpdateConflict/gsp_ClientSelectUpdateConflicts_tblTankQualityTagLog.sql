-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTankQualityTagLog
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTankQualityTagLog]
@TankQualityTagLogGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTankQualityTagLog].[TankID],[dbo].[tblTankQualityTagLog].[VesselType],[dbo].[tblTankQualityTagLog].[QualityTagName],[dbo].[tblTankQualityTagLog].[TaggedDate],[dbo].[tblTankQualityTagLog].[TaggedBy],[dbo].[tblTankQualityTagLog].[Memo],[dbo].[tblTankQualityTagLog].[RemovedDate],[dbo].[tblTankQualityTagLog].[RemovedBy],[dbo].[tblTankQualityTagLog].[DeleteFlag],[dbo].[tblTankQualityTagLog].[CreatedDate],[dbo].[tblTankQualityTagLog].[CreatedBy],[dbo].[tblTankQualityTagLog].[UpdatedDate],[dbo].[tblTankQualityTagLog].[UpdatedBy],[dbo].[tblTankQualityTagLog].[TagNumber],[dbo].[tblTankQualityTagLog].[TankQualityTagLogGuid],[dbo].[tblTankQualityTagLog].[SiteGuid],[dbo].[tblTankQualityTagLog].[LookupVesselTypeIndex],[dbo].[tblTankQualityTagLog].[QualityTagGuid],[dbo].[tblTankQualityTagLog].[TankGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTankQualityTagLog]
            INNER JOIN [track].[tblTankQualityTagLog] CT
                ON CT.PK_TankQualityTagLogGuid = [dbo].[tblTankQualityTagLog].[TankQualityTagLogGuid]
        WHERE CT.PK_TankQualityTagLogGuid = @TankQualityTagLogGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
