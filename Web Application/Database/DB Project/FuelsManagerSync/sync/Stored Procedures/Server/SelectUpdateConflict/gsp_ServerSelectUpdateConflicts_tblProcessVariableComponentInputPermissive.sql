-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableComponentInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableComponentInputPermissive]
@ProcessVariableProductToPresetComponentTankOrTankGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableComponentInputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableComponentInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableComponentInputPermissive].[ProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableComponentInputPermissive].[OPCItemID],[dbo].[tblProcessVariableComponentInputPermissive].[DataType],[dbo].[tblProcessVariableComponentInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableComponentInputPermissive].[Quality],[dbo].[tblProcessVariableComponentInputPermissive].[SIValue],[dbo].[tblProcessVariableComponentInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableComponentInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableComponentInputPermissive].[Maximum],[dbo].[tblProcessVariableComponentInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableComponentInputPermissive].[Minimum],[dbo].[tblProcessVariableComponentInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableComponentInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableComponentInputPermissive].[Input],[dbo].[tblProcessVariableComponentInputPermissive].[InputEnabled],[dbo].[tblProcessVariableComponentInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableComponentInputPermissive].[CreatedDate],[dbo].[tblProcessVariableComponentInputPermissive].[CreatedBy],[dbo].[tblProcessVariableComponentInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableComponentInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableComponentInputPermissive]
            INNER JOIN [track].[tblProcessVariableComponentInputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = [dbo].[tblProcessVariableComponentInputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid]
        WHERE CT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = @ProcessVariableProductToPresetComponentTankOrTankGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
