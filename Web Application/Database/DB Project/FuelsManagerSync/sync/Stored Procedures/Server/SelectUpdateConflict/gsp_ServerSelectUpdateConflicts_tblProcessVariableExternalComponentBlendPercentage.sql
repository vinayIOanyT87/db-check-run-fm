-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalComponentBlendPercentage
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableExternalComponentBlendPercentage]
@ProcessVariableProductToPresetExternalComponentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCItemID],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataType],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Quality],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[SIValue],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Maximum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Minimum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Input],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InputEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedBy],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalComponentBlendPercentage]
            INNER JOIN [track].[tblProcessVariableExternalComponentBlendPercentage] CT
                ON CT.PK_ProcessVariableProductToPresetExternalComponentGuid = [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid]
        WHERE CT.PK_ProcessVariableProductToPresetExternalComponentGuid = @ProcessVariableProductToPresetExternalComponentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
