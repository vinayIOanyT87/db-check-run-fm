-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariablePresetInjector
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariablePresetInjector]
@ProcessVariablePresetInjectorGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariablePresetInjector].[ProcessVariablePresetInjectorGuid],[dbo].[tblProcessVariablePresetInjector].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariablePresetInjector].[InstanceNumber],[dbo].[tblProcessVariablePresetInjector].[ProductToPresetInjectorGuid],[dbo].[tblProcessVariablePresetInjector].[OPCConnectionGuid],[dbo].[tblProcessVariablePresetInjector].[OPCItemID],[dbo].[tblProcessVariablePresetInjector].[DataType],[dbo].[tblProcessVariablePresetInjector].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariablePresetInjector].[Quality],[dbo].[tblProcessVariablePresetInjector].[SIValue],[dbo].[tblProcessVariablePresetInjector].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariablePresetInjector].[DateTimeStamp],[dbo].[tblProcessVariablePresetInjector].[Maximum],[dbo].[tblProcessVariablePresetInjector].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariablePresetInjector].[Minimum],[dbo].[tblProcessVariablePresetInjector].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariablePresetInjector].[DataTypeEnabled],[dbo].[tblProcessVariablePresetInjector].[Input],[dbo].[tblProcessVariablePresetInjector].[InputEnabled],[dbo].[tblProcessVariablePresetInjector].[MessageApplicationStringGuid],[dbo].[tblProcessVariablePresetInjector].[CreatedDate],[dbo].[tblProcessVariablePresetInjector].[CreatedBy],[dbo].[tblProcessVariablePresetInjector].[UpdatedDate],[dbo].[tblProcessVariablePresetInjector].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariablePresetInjector]
            INNER JOIN [track].[tblProcessVariablePresetInjector] CT
                ON CT.PK_ProcessVariablePresetInjectorGuid = [dbo].[tblProcessVariablePresetInjector].[ProcessVariablePresetInjectorGuid]
        WHERE CT.PK_ProcessVariablePresetInjectorGuid = @ProcessVariablePresetInjectorGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
