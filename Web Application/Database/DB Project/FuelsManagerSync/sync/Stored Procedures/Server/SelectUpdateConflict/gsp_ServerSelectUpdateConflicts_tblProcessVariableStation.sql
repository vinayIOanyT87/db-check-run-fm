-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblProcessVariableStation]
@ProcessVariableStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblProcessVariableStation].[ProcessVariableStationGuid],[dbo].[tblProcessVariableStation].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableStation].[InstanceNumber],[dbo].[tblProcessVariableStation].[StationGuid],[dbo].[tblProcessVariableStation].[OPCConnectionGuid],[dbo].[tblProcessVariableStation].[OPCItemID],[dbo].[tblProcessVariableStation].[DataType],[dbo].[tblProcessVariableStation].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableStation].[Quality],[dbo].[tblProcessVariableStation].[SIValue],[dbo].[tblProcessVariableStation].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableStation].[DateTimeStamp],[dbo].[tblProcessVariableStation].[Maximum],[dbo].[tblProcessVariableStation].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableStation].[Minimum],[dbo].[tblProcessVariableStation].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableStation].[DataTypeEnabled],[dbo].[tblProcessVariableStation].[Input],[dbo].[tblProcessVariableStation].[InputEnabled],[dbo].[tblProcessVariableStation].[MessageApplicationStringGuid],[dbo].[tblProcessVariableStation].[CreatedDate],[dbo].[tblProcessVariableStation].[CreatedBy],[dbo].[tblProcessVariableStation].[UpdatedDate],[dbo].[tblProcessVariableStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblProcessVariableStation]
            INNER JOIN [track].[tblProcessVariableStation] CT
                ON CT.PK_ProcessVariableStationGuid = [dbo].[tblProcessVariableStation].[ProcessVariableStationGuid]
        WHERE CT.PK_ProcessVariableStationGuid = @ProcessVariableStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
