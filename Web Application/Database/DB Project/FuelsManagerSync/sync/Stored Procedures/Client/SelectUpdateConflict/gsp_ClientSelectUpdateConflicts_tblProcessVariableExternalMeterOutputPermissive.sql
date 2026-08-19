-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalMeterOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableExternalMeterOutputPermissive]
@ProcessVariableProductToOffloadExternalMeterGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalMeterOutputPermissive]
            INNER JOIN [track].[tblProcessVariableExternalMeterOutputPermissive] CT
                ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid]
        WHERE CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = @ProcessVariableProductToOffloadExternalMeterGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
