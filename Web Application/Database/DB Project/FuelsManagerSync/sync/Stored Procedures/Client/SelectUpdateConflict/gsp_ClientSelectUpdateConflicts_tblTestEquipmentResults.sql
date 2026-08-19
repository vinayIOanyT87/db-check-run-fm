-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTestEquipmentResults
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTestEquipmentResults]
@TestEquipmentResultGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTestEquipmentResults].[TestName],[dbo].[tblTestEquipmentResults].[Measurement],[dbo].[tblTestEquipmentResults].[TestDate],[dbo].[tblTestEquipmentResults].[DeleteFlag],[dbo].[tblTestEquipmentResults].[CreatedDate],[dbo].[tblTestEquipmentResults].[CreatedBy],[dbo].[tblTestEquipmentResults].[UpdatedDate],[dbo].[tblTestEquipmentResults].[UpdatedBy],[dbo].[tblTestEquipmentResults].[PerformedBy],[dbo].[tblTestEquipmentResults].[Supervisor],[dbo].[tblTestEquipmentResults].[Flag01],[dbo].[tblTestEquipmentResults].[Flag02],[dbo].[tblTestEquipmentResults].[TestCode],[dbo].[tblTestEquipmentResults].[TestEquipmentResultGuid],[dbo].[tblTestEquipmentResults].[LookupTestSetStatusIndex],[dbo].[tblTestEquipmentResults].[TestSetEquipmentResultGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTestEquipmentResults]
            INNER JOIN [track].[tblTestEquipmentResults] CT
                ON CT.PK_TestEquipmentResultGuid = [dbo].[tblTestEquipmentResults].[TestEquipmentResultGuid]
        WHERE CT.PK_TestEquipmentResultGuid = @TestEquipmentResultGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
