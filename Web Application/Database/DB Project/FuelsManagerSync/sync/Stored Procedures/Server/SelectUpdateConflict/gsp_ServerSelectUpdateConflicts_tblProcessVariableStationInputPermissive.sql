-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableStationInputPermissive
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableStationInputPermissive]
@ProcessVariableStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableStationInputPermissive].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStationInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableStationInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableStationInputPermissive].[StationGuid],[dbo].[tblProcessVariableStationInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableStationInputPermissive].[OPCItemID],[dbo].[tblProcessVariableStationInputPermissive].[DataType],[dbo].[tblProcessVariableStationInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableStationInputPermissive].[Quality],[dbo].[tblProcessVariableStationInputPermissive].[SIValue],[dbo].[tblProcessVariableStationInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableStationInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableStationInputPermissive].[Maximum],[dbo].[tblProcessVariableStationInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableStationInputPermissive].[Minimum],[dbo].[tblProcessVariableStationInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableStationInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableStationInputPermissive].[Input],[dbo].[tblProcessVariableStationInputPermissive].[InputEnabled],[dbo].[tblProcessVariableStationInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableStationInputPermissive].[CreatedDate],[dbo].[tblProcessVariableStationInputPermissive].[CreatedBy],[dbo].[tblProcessVariableStationInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableStationInputPermissive].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableStationInputPermissive]
            INNER JOIN [track].[tblProcessVariableStationInputPermissive] CT
                ON CT.PK_ProcessVariableStationGuid = [dbo].[tblProcessVariableStationInputPermissive].[ProcessVariableStationGuid]
        WHERE CT.PK_ProcessVariableStationGuid = @ProcessVariableStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
