-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableAdditiveInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableAdditiveInputPermissive]
@ProcessVariableProductToPresetInjectorGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableAdditiveInputPermissive].[ProcessVariableProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableAdditiveInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableAdditiveInputPermissive].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[OPCItemID],[dbo].[tblProcessVariableAdditiveInputPermissive].[DataType],[dbo].[tblProcessVariableAdditiveInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableAdditiveInputPermissive].[Quality],[dbo].[tblProcessVariableAdditiveInputPermissive].[SIValue],[dbo].[tblProcessVariableAdditiveInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableAdditiveInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableAdditiveInputPermissive].[Maximum],[dbo].[tblProcessVariableAdditiveInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableAdditiveInputPermissive].[Minimum],[dbo].[tblProcessVariableAdditiveInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableAdditiveInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableAdditiveInputPermissive].[Input],[dbo].[tblProcessVariableAdditiveInputPermissive].[InputEnabled],[dbo].[tblProcessVariableAdditiveInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableAdditiveInputPermissive].[CreatedDate],[dbo].[tblProcessVariableAdditiveInputPermissive].[CreatedBy],[dbo].[tblProcessVariableAdditiveInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableAdditiveInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableAdditiveInputPermissive]
            INNER JOIN [track].[tblProcessVariableAdditiveInputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetInjectorGuid = [dbo].[tblProcessVariableAdditiveInputPermissive].[ProcessVariableProductToPresetInjectorGuid]
        WHERE CT.PK_ProcessVariableProductToPresetInjectorGuid = @ProcessVariableProductToPresetInjectorGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
