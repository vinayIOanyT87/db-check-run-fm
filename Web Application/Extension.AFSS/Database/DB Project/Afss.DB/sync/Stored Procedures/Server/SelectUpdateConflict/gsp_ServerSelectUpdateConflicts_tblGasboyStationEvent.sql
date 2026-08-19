-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyStationEvent
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGasboyStationEvent]
@GasboyStationEventGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGasboyStationEvent].[GasboyStationEventGuid],[dbo].[tblGasboyStationEvent].[ExternalStationLogGuid],[dbo].[tblGasboyStationEvent].[EventID],[dbo].[tblGasboyStationEvent].[LookupGasboyEventErrorClassCodeIndex],[dbo].[tblGasboyStationEvent].[ErrorCode],[dbo].[tblGasboyStationEvent].[FleetID],[dbo].[tblGasboyStationEvent].[ObjectID],[dbo].[tblGasboyStationEvent].[LookupGasboyEventObjectTypeIndex],[dbo].[tblGasboyStationEvent].[DeviceName],[dbo].[tblGasboyStationEvent].[Field1],[dbo].[tblGasboyStationEvent].[Field2],[dbo].[tblGasboyStationEvent].[Field3],[dbo].[tblGasboyStationEvent].[Field4],[dbo].[tblGasboyStationEvent].[Field5],[dbo].[tblGasboyStationEvent].[Field6],[dbo].[tblGasboyStationEvent].[Field7],[dbo].[tblGasboyStationEvent].[Field8],[dbo].[tblGasboyStationEvent].[CreatedBy],[dbo].[tblGasboyStationEvent].[CreatedDate],[dbo].[tblGasboyStationEvent].[UpdatedBy],[dbo].[tblGasboyStationEvent].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGasboyStationEvent]
            INNER JOIN [track].[tblGasboyStationEvent] CT
                ON CT.PK_GasboyStationEventGuid = [dbo].[tblGasboyStationEvent].[GasboyStationEventGuid]
        WHERE CT.PK_GasboyStationEventGuid = @GasboyStationEventGuid
    ORDER BY CT.UpdatedRowVersion ASC
END