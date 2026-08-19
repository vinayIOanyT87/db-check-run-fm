-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableRecipeInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableRecipeInputPermissive]
@ProcessVariableProductToPresetRecipeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableRecipeInputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableRecipeInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableRecipeInputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[OPCItemID],[dbo].[tblProcessVariableRecipeInputPermissive].[DataType],[dbo].[tblProcessVariableRecipeInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableRecipeInputPermissive].[Quality],[dbo].[tblProcessVariableRecipeInputPermissive].[SIValue],[dbo].[tblProcessVariableRecipeInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableRecipeInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableRecipeInputPermissive].[Maximum],[dbo].[tblProcessVariableRecipeInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableRecipeInputPermissive].[Minimum],[dbo].[tblProcessVariableRecipeInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableRecipeInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableRecipeInputPermissive].[Input],[dbo].[tblProcessVariableRecipeInputPermissive].[InputEnabled],[dbo].[tblProcessVariableRecipeInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableRecipeInputPermissive].[CreatedDate],[dbo].[tblProcessVariableRecipeInputPermissive].[CreatedBy],[dbo].[tblProcessVariableRecipeInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableRecipeInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableRecipeInputPermissive]
            INNER JOIN [track].[tblProcessVariableRecipeInputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetRecipeGuid = [dbo].[tblProcessVariableRecipeInputPermissive].[ProcessVariableProductToPresetRecipeGuid]
        WHERE CT.PK_ProcessVariableProductToPresetRecipeGuid = @ProcessVariableProductToPresetRecipeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
