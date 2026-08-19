-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestSetEquipmentResults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTestSetEquipmentResults]
@TestSetEquipmentResultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestSetEquipmentResults].[ResultTimeStamp],[dbo].[tblTestSetEquipmentResults].[TestSetName],[dbo].[tblTestSetEquipmentResults].[Inspector],[dbo].[tblTestSetEquipmentResults].[Supervisor],[dbo].[tblTestSetEquipmentResults].[EquipmentID],[dbo].[tblTestSetEquipmentResults].[SampleNumber],[dbo].[tblTestSetEquipmentResults].[SampleSize],[dbo].[tblTestSetEquipmentResults].[IsRetest],[dbo].[tblTestSetEquipmentResults].[PreviousSampleNumber],[dbo].[tblTestSetEquipmentResults].[DocumentNumber],[dbo].[tblTestSetEquipmentResults].[Memo],[dbo].[tblTestSetEquipmentResults].[GallonsRepresented],[dbo].[tblTestSetEquipmentResults].[Override],[dbo].[tblTestSetEquipmentResults].[DeleteFlag],[dbo].[tblTestSetEquipmentResults].[CreatedDate],[dbo].[tblTestSetEquipmentResults].[CreatedBy],[dbo].[tblTestSetEquipmentResults].[UpdatedDate],[dbo].[tblTestSetEquipmentResults].[UpdatedBy],[dbo].[tblTestSetEquipmentResults].[TestSetEquipmentResultGuid],[dbo].[tblTestSetEquipmentResults].[SiteGuid],[dbo].[tblTestSetEquipmentResults].[LookupTestSetStatusIndex],[dbo].[tblTestSetEquipmentResults].[EquipmentGuid],[dbo].[tblTestSetEquipmentResults].[Flag01],[dbo].[tblTestSetEquipmentResults].[Flag02],[dbo].[tblTestSetEquipmentResults].[UserData01],[dbo].[tblTestSetEquipmentResults].[UserData02], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestSetEquipmentResults]
            INNER JOIN [track].[tblTestSetEquipmentResults] CT
                ON CT.PK_TestSetEquipmentResultGuid = [dbo].[tblTestSetEquipmentResults].[TestSetEquipmentResultGuid]
        WHERE CT.PK_TestSetEquipmentResultGuid = @TestSetEquipmentResultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
