-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableComponentOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableComponentOutputPermissive]
@ProcessVariableProductToPresetComponentTankOrTankGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableComponentOutputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableComponentOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableComponentOutputPermissive].[ProductToPresetComponentTankOrTankGroupGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableComponentOutputPermissive].[DataType],[dbo].[tblProcessVariableComponentOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableComponentOutputPermissive].[Quality],[dbo].[tblProcessVariableComponentOutputPermissive].[SIValue],[dbo].[tblProcessVariableComponentOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableComponentOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableComponentOutputPermissive].[Maximum],[dbo].[tblProcessVariableComponentOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableComponentOutputPermissive].[Minimum],[dbo].[tblProcessVariableComponentOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableComponentOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableComponentOutputPermissive].[Input],[dbo].[tblProcessVariableComponentOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableComponentOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableComponentOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableComponentOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableComponentOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableComponentOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableComponentOutputPermissive]
            INNER JOIN [track].[tblProcessVariableComponentOutputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = [dbo].[tblProcessVariableComponentOutputPermissive].[ProcessVariableProductToPresetComponentTankOrTankGroupGuid]
        WHERE CT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = @ProcessVariableProductToPresetComponentTankOrTankGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
