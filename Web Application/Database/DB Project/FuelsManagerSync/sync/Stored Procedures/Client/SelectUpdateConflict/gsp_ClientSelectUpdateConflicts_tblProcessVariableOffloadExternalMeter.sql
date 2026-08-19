-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableOffloadExternalMeter
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableOffloadExternalMeter]
@ProcessVariableProductToOffloadExternalMeterGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableOffloadExternalMeter].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[InstanceNumber],[dbo].[tblProcessVariableOffloadExternalMeter].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCConnectionGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCItemID],[dbo].[tblProcessVariableOffloadExternalMeter].[DataType],[dbo].[tblProcessVariableOffloadExternalMeter].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Quality],[dbo].[tblProcessVariableOffloadExternalMeter].[SIValue],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DateTimeStamp],[dbo].[tblProcessVariableOffloadExternalMeter].[Maximum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Minimum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DataTypeEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[Input],[dbo].[tblProcessVariableOffloadExternalMeter].[InputEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[MessageApplicationStringGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedBy],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableOffloadExternalMeter]
            INNER JOIN [track].[tblProcessVariableOffloadExternalMeter] CT
                ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableOffloadExternalMeter].[ProcessVariableProductToOffloadExternalMeterGuid]
        WHERE CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = @ProcessVariableProductToOffloadExternalMeterGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
