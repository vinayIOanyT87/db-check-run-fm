-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableLoadArmInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableLoadArmInputPermissive]
@ProcessVariableLoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableLoadArmInputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableLoadArmInputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[OPCItemID],[dbo].[tblProcessVariableLoadArmInputPermissive].[DataType],[dbo].[tblProcessVariableLoadArmInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[Quality],[dbo].[tblProcessVariableLoadArmInputPermissive].[SIValue],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableLoadArmInputPermissive].[Maximum],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[Minimum],[dbo].[tblProcessVariableLoadArmInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableLoadArmInputPermissive].[Input],[dbo].[tblProcessVariableLoadArmInputPermissive].[InputEnabled],[dbo].[tblProcessVariableLoadArmInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableLoadArmInputPermissive].[CreatedDate],[dbo].[tblProcessVariableLoadArmInputPermissive].[CreatedBy],[dbo].[tblProcessVariableLoadArmInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableLoadArmInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableLoadArmInputPermissive]
            INNER JOIN [track].[tblProcessVariableLoadArmInputPermissive] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableLoadArmInputPermissive].[ProcessVariableLoadArmGuid]
        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
