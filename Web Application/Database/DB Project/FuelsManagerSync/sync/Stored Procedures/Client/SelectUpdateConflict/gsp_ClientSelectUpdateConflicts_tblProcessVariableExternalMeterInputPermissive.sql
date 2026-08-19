-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalMeterInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableExternalMeterInputPermissive]
@ProcessVariableProductToOffloadExternalMeterGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalMeterInputPermissive]
            INNER JOIN [track].[tblProcessVariableExternalMeterInputPermissive] CT
                ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid]
        WHERE CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = @ProcessVariableProductToOffloadExternalMeterGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
