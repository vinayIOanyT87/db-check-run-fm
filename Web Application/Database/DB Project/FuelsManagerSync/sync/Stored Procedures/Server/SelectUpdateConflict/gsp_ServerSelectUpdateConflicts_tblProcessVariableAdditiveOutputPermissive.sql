-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableAdditiveOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableAdditiveOutputPermissive]
@ProcessVariableProductToPresetInjectorGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableAdditiveOutputPermissive].[ProcessVariableProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableAdditiveOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableAdditiveOutputPermissive].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableAdditiveOutputPermissive].[DataType],[dbo].[tblProcessVariableAdditiveOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableAdditiveOutputPermissive].[Quality],[dbo].[tblProcessVariableAdditiveOutputPermissive].[SIValue],[dbo].[tblProcessVariableAdditiveOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableAdditiveOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableAdditiveOutputPermissive].[Maximum],[dbo].[tblProcessVariableAdditiveOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableAdditiveOutputPermissive].[Minimum],[dbo].[tblProcessVariableAdditiveOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableAdditiveOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableAdditiveOutputPermissive].[Input],[dbo].[tblProcessVariableAdditiveOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableAdditiveOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableAdditiveOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableAdditiveOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableAdditiveOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableAdditiveOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableAdditiveOutputPermissive]
            INNER JOIN [track].[tblProcessVariableAdditiveOutputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetInjectorGuid = [dbo].[tblProcessVariableAdditiveOutputPermissive].[ProcessVariableProductToPresetInjectorGuid]
        WHERE CT.PK_ProcessVariableProductToPresetInjectorGuid = @ProcessVariableProductToPresetInjectorGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
