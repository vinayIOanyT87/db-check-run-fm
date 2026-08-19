-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableRecipeOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableRecipeOutputPermissive]
@ProcessVariableProductToPresetRecipeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataType],[dbo].[tblProcessVariableRecipeOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Quality],[dbo].[tblProcessVariableRecipeOutputPermissive].[SIValue],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableRecipeOutputPermissive].[Maximum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Minimum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[Input],[dbo].[tblProcessVariableRecipeOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableRecipeOutputPermissive]
            INNER JOIN [track].[tblProcessVariableRecipeOutputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetRecipeGuid = [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid]
        WHERE CT.PK_ProcessVariableProductToPresetRecipeGuid = @ProcessVariableProductToPresetRecipeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
