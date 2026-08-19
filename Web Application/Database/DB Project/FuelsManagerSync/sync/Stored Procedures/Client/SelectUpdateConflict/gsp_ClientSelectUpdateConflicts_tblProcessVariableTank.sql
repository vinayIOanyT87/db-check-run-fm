-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableTank
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableTank]
@ProcessVariableTankGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid],[dbo].[tblProcessVariableTank].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableTank].[InstanceNumber],[dbo].[tblProcessVariableTank].[TankGuid],[dbo].[tblProcessVariableTank].[OPCConnectionGuid],[dbo].[tblProcessVariableTank].[OPCItemID],[dbo].[tblProcessVariableTank].[DataType],[dbo].[tblProcessVariableTank].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableTank].[Quality],[dbo].[tblProcessVariableTank].[SIValue],[dbo].[tblProcessVariableTank].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableTank].[DateTimeStamp],[dbo].[tblProcessVariableTank].[Maximum],[dbo].[tblProcessVariableTank].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableTank].[Minimum],[dbo].[tblProcessVariableTank].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableTank].[DataTypeEnabled],[dbo].[tblProcessVariableTank].[Input],[dbo].[tblProcessVariableTank].[InputEnabled],[dbo].[tblProcessVariableTank].[MessageApplicationStringGuid],[dbo].[tblProcessVariableTank].[CreatedDate],[dbo].[tblProcessVariableTank].[CreatedBy],[dbo].[tblProcessVariableTank].[UpdatedDate],[dbo].[tblProcessVariableTank].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableTank]
            INNER JOIN [track].[tblProcessVariableTank] CT
                ON CT.PK_ProcessVariableTankGuid = [dbo].[tblProcessVariableTank].[ProcessVariableTankGuid]
        WHERE CT.PK_ProcessVariableTankGuid = @ProcessVariableTankGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
