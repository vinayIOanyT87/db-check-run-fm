-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableLoadArmOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableLoadArmOutputPermissive]
@ProcessVariableLoadArmGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableLoadArmOutputPermissive].[ProcessVariableLoadArmGuid],[dbo].[tblProcessVariableLoadArmOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableLoadArmOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableLoadArmOutputPermissive].[LoadArmGuid],[dbo].[tblProcessVariableLoadArmOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableLoadArmOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableLoadArmOutputPermissive].[DataType],[dbo].[tblProcessVariableLoadArmOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableLoadArmOutputPermissive].[Quality],[dbo].[tblProcessVariableLoadArmOutputPermissive].[SIValue],[dbo].[tblProcessVariableLoadArmOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableLoadArmOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableLoadArmOutputPermissive].[Maximum],[dbo].[tblProcessVariableLoadArmOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmOutputPermissive].[Minimum],[dbo].[tblProcessVariableLoadArmOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableLoadArmOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableLoadArmOutputPermissive].[Input],[dbo].[tblProcessVariableLoadArmOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableLoadArmOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableLoadArmOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableLoadArmOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableLoadArmOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableLoadArmOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableLoadArmOutputPermissive]
            INNER JOIN [track].[tblProcessVariableLoadArmOutputPermissive] CT
                ON CT.PK_ProcessVariableLoadArmGuid = [dbo].[tblProcessVariableLoadArmOutputPermissive].[ProcessVariableLoadArmGuid]
        WHERE CT.PK_ProcessVariableLoadArmGuid = @ProcessVariableLoadArmGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
