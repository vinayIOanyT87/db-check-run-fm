-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetTankResults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestSetTankResults]
@TestSetTankResultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestSetTankResults].[ResultTimeStamp],[dbo].[tblTestSetTankResults].[TestSetName],[dbo].[tblTestSetTankResults].[Inspector],[dbo].[tblTestSetTankResults].[Supervisor],[dbo].[tblTestSetTankResults].[TankID],[dbo].[tblTestSetTankResults].[SampleNumber],[dbo].[tblTestSetTankResults].[SampleSize],[dbo].[tblTestSetTankResults].[IsRetest],[dbo].[tblTestSetTankResults].[PreviousSampleNumber],[dbo].[tblTestSetTankResults].[DocumentNumber],[dbo].[tblTestSetTankResults].[Memo],[dbo].[tblTestSetTankResults].[GallonsRepresented],[dbo].[tblTestSetTankResults].[Override],[dbo].[tblTestSetTankResults].[DeleteFlag],[dbo].[tblTestSetTankResults].[CreatedDate],[dbo].[tblTestSetTankResults].[CreatedBy],[dbo].[tblTestSetTankResults].[UpdatedDate],[dbo].[tblTestSetTankResults].[UpdatedBy],[dbo].[tblTestSetTankResults].[Flag01],[dbo].[tblTestSetTankResults].[Flag02],[dbo].[tblTestSetTankResults].[UserData01],[dbo].[tblTestSetTankResults].[UserData02],[dbo].[tblTestSetTankResults].[TestSetTankResultGuid],[dbo].[tblTestSetTankResults].[SiteGuid],[dbo].[tblTestSetTankResults].[LookupTestSetStatusIndex],[dbo].[tblTestSetTankResults].[TankGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestSetTankResults]
            INNER JOIN [track].[tblTestSetTankResults] CT
                ON CT.PK_TestSetTankResultGuid = [dbo].[tblTestSetTankResults].[TestSetTankResultGuid]
        WHERE CT.PK_TestSetTankResultGuid = @TestSetTankResultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
