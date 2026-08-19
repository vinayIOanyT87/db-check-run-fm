-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTag
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointTag]
@PointTagGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTag].[ID],[dbo].[tblPointTag].[EngineeringUnitsType],[dbo].[tblPointTag].[EngineeringUnitsIndex],[dbo].[tblPointTag].[DecimalPlaces],[dbo].[tblPointTag].[ServerEngineeringUnitsIndex],[dbo].[tblPointTag].[ValueType],[dbo].[tblPointTag].[Status],[dbo].[tblPointTag].[Value],[dbo].[tblPointTag].[ServerTimeStamp],[dbo].[tblPointTag].[SourceTimeStamp],[dbo].[tblPointTag].[Maximum],[dbo].[tblPointTag].[Minimum],[dbo].[tblPointTag].[PointTagInputOutputTypeIndex],[dbo].[tblPointTag].[LastPointTagInputOutputTypeIndex],[dbo].[tblPointTag].[Input],[dbo].[tblPointTag].[AlarmStatus],[dbo].[tblPointTag].[ApplyPointEngineeringUnits],[dbo].[tblPointTag].[ApplyPointDecimalPlaces],[dbo].[tblPointTag].[ApplyPointMaximum],[dbo].[tblPointTag].[ApplyPointMinimum],[dbo].[tblPointTag].[OpcUaServerGuid],[dbo].[tblPointTag].[OpcUaBrowsePath],[dbo].[tblPointTag].[OpcUaNamespaceUri],[dbo].[tblPointTag].[OpcUaPublishingInterval],[dbo].[tblPointTag].[OpcUaNodeId],[dbo].[tblPointTag].[OpcUaIsReadable],[dbo].[tblPointTag].[OpcUaServerDataType],[dbo].[tblPointTag].[OpcUaWriteHoldoffTime],[dbo].[tblPointTag].[OpcUaWritePeriodicUpdateInterval],[dbo].[tblPointTag].[CreatedDate],[dbo].[tblPointTag].[CreatedBy],[dbo].[tblPointTag].[UpdatedDate],[dbo].[tblPointTag].[UpdatedBy],[dbo].[tblPointTag].[PointTagGuid],[dbo].[tblPointTag].[PointGuid],[dbo].[tblPointTag].[PointTemplateTagGuid],[dbo].[tblPointTag].[AlarmsEnabled],[dbo].[tblPointTag].[InhibitInputOutputTypeConfiguration],[dbo].[tblPointTag].[InhibitOverride],[dbo].[tblPointTag].[Deadband],[dbo].[tblPointTag].[Holdoff],[dbo].[tblPointTag].[Archived],CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTag]
            INNER JOIN [track].[tblPointTag] CT
                ON CT.PK_PointTagGuid = [dbo].[tblPointTag].[PointTagGuid]
        WHERE CT.PK_PointTagGuid = @PointTagGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
