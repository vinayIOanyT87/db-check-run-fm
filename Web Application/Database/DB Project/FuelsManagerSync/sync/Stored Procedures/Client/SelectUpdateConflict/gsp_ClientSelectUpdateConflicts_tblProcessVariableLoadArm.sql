-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableLoadArm
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableLoadArm]
@ProcessVariableLoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableLoadArm].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableLoadArm].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableLoadArm].[InstanceNumber],[dbo].[tblProcessVariableLoadArm].[LoadArmGuid],[dbo].[tblProcessVariableLoadArm].[OPCConnectionGuid],[dbo].[tblProcessVariableLoadArm].[OPCItemID],[dbo].[tblProcessVariableLoadArm].[DataType],[dbo].[tblProcessVariableLoadArm].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableLoadArm].[Quality],[dbo].[tblProcessVariableLoadArm].[SIValue],[dbo].[tblProcessVariableLoadArm].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableLoadArm].[DateTimeStamp],[dbo].[tblProcessVariableLoadArm].[Maximum],[dbo].[tblProcessVariableLoadArm].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableLoadArm].[Minimum],[dbo].[tblProcessVariableLoadArm].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableLoadArm].[DataTypeEnabled],[dbo].[tblProcessVariableLoadArm].[Input],[dbo].[tblProcessVariableLoadArm].[InputEnabled],[dbo].[tblProcessVariableLoadArm].[MessageApplicationStringGuid],[dbo].[tblProcessVariableLoadArm].[CreatedDate],[dbo].[tblProcessVariableLoadArm].[CreatedBy],[dbo].[tblProcessVariableLoadArm].[UpdatedDate],[dbo].[tblProcessVariableLoadArm].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableLoadArm]
            INNER JOIN [track].[tblProcessVariableLoadArm] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableLoadArm].[ProcessVariableLoadArmGuid]
        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
