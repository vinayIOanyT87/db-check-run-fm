-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableStationOutputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableStationOutputPermissive]
@ProcessVariableStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableStationOutputPermissive].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStationOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableStationOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableStationOutputPermissive].[StationGuid],[dbo].[tblProcessVariableStationOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableStationOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableStationOutputPermissive].[DataType],[dbo].[tblProcessVariableStationOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableStationOutputPermissive].[Quality],[dbo].[tblProcessVariableStationOutputPermissive].[SIValue],[dbo].[tblProcessVariableStationOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableStationOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableStationOutputPermissive].[Maximum],[dbo].[tblProcessVariableStationOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableStationOutputPermissive].[Minimum],[dbo].[tblProcessVariableStationOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableStationOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableStationOutputPermissive].[Input],[dbo].[tblProcessVariableStationOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableStationOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableStationOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableStationOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableStationOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableStationOutputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableStationOutputPermissive]
            INNER JOIN [track].[tblProcessVariableStationOutputPermissive] CT
                ON CT.PK_ProcessVariableStationGuid = [dbo].[tblProcessVariableStationOutputPermissive].[ProcessVariableStationGuid]
        WHERE CT.PK_ProcessVariableStationGuid = @ProcessVariableStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
