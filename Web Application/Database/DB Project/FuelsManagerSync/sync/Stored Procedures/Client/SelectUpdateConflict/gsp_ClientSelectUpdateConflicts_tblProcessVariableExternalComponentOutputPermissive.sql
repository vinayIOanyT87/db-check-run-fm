-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalComponentOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableExternalComponentOutputPermissive]
@ProcessVariableProductToPresetExternalComponentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[DataType],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[Quality],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[SIValue],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[Maximum],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[Minimum],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[Input],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalComponentOutputPermissive]
            INNER JOIN [track].[tblProcessVariableExternalComponentOutputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetExternalComponentGuid = [dbo].[tblProcessVariableExternalComponentOutputPermissive].[ProcessVariableProductToPresetExternalComponentGuid]
        WHERE CT.PK_ProcessVariableProductToPresetExternalComponentGuid = @ProcessVariableProductToPresetExternalComponentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
