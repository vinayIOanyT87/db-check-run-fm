-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalComponentInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableExternalComponentInputPermissive]
@ProcessVariableProductToPresetExternalComponentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataType],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Quality],[dbo].[tblProcessVariableExternalComponentInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Input],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalComponentInputPermissive]
            INNER JOIN [track].[tblProcessVariableExternalComponentInputPermissive] CT
                ON CT.PK_ProcessVariableProductToPresetExternalComponentGuid = [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid]
        WHERE CT.PK_ProcessVariableProductToPresetExternalComponentGuid = @ProcessVariableProductToPresetExternalComponentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
