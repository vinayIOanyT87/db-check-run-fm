-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplate
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblPointTemplate]
@sync_initialized bit,
@sync_last_received_anchor bigint,
@sync_new_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_server_id_binary binary(16),
@sync_context_site_guid uniqueidentifier,
@sync_context_site_id nvarchar(30),
@sync_context_site_guid_list nvarchar(1024),
@sync_context_site_id_list nvarchar(1024),
@sync_table_name nvarchar(512),
@sync_batch_size_tblPointTemplate int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblPointTemplate int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblPointTemplate].[ID],[dbo].[tblPointTemplate].[Description],[dbo].[tblPointTemplate].[Standard],[dbo].[tblPointTemplate].[ExecutionInterval],[dbo].[tblPointTemplate].[LevelUnitIndex],[dbo].[tblPointTemplate].[TemperatureUnitIndex],[dbo].[tblPointTemplate].[DensityUnitIndex],[dbo].[tblPointTemplate].[PressureUnitIndex],[dbo].[tblPointTemplate].[FlowUnitIndex],[dbo].[tblPointTemplate].[VolumeUnitIndex],[dbo].[tblPointTemplate].[MassUnitIndex],[dbo].[tblPointTemplate].[VelocityUnitIndex],[dbo].[tblPointTemplate].[MassFlowUnitIndex],[dbo].[tblPointTemplate].[LevelDecimalPlaces],[dbo].[tblPointTemplate].[TemperatureDecimalPlaces],[dbo].[tblPointTemplate].[DensityDecimalPlaces],[dbo].[tblPointTemplate].[PressureDecimalPlaces],[dbo].[tblPointTemplate].[FlowDecimalPlaces],[dbo].[tblPointTemplate].[VolumeDecimalPlaces],[dbo].[tblPointTemplate].[MassDecimalPlaces],[dbo].[tblPointTemplate].[VelocityDecimalPlaces],[dbo].[tblPointTemplate].[MassFlowDecimalPlaces],[dbo].[tblPointTemplate].[LevelMaximum],[dbo].[tblPointTemplate].[LevelMinimum],[dbo].[tblPointTemplate].[TemperatureMaximum],[dbo].[tblPointTemplate].[TemperatureMinimum],[dbo].[tblPointTemplate].[DensityMaximum],[dbo].[tblPointTemplate].[DensityMinimum],[dbo].[tblPointTemplate].[PressureMaximum],[dbo].[tblPointTemplate].[PressureMinimum],[dbo].[tblPointTemplate].[VolumetricFlowMaximum],[dbo].[tblPointTemplate].[VolumetricFlowMinimum],[dbo].[tblPointTemplate].[VolumeMaximum],[dbo].[tblPointTemplate].[VolumeMinimum],[dbo].[tblPointTemplate].[MassMaximum],[dbo].[tblPointTemplate].[MassMinimum],[dbo].[tblPointTemplate].[VelocityMaximum],[dbo].[tblPointTemplate].[VelocityMinimum],[dbo].[tblPointTemplate].[MassFlowMaximum],[dbo].[tblPointTemplate].[MassFlowMinimum],[dbo].[tblPointTemplate].[CreatedDate],[dbo].[tblPointTemplate].[CreatedBy],[dbo].[tblPointTemplate].[UpdatedDate],[dbo].[tblPointTemplate].[UpdatedBy],[dbo].[tblPointTemplate].[PointTemplateGuid],[dbo].[tblPointTemplate].[SiteGuid],[dbo].[tblPointTemplate].[PointTemplateTypeApplicationStringGuid],[dbo].[tblPointTemplate].[ProfileImageGuid],[dbo].[tblPointTemplate].[DefaultDrawingGuid],[dbo].[tblPointTemplate].[PointCommandStatus],[dbo].[tblPointTemplate].[DeviceAlarmMaps],[dbo].[tblPointTemplate].[PointLogicScript],[dbo].[tblPointTemplate].[Version], [dbo].[tblPointTemplate].[_RowVersion]
            FROM [dbo].[tblPointTemplate]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblPointTemplate IS NULL OR 
        (@sync_batch_size_tblPointTemplate IS NOT NULL AND @sync_batch_size_tblPointTemplate = 0))
    BEGIN
        SET @sync_batch_size_tblPointTemplate = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
            SELECT TOP(@sync_batch_size_tblPointTemplate) WITH TIES [ID],[Description],[Standard],[ExecutionInterval],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[VelocityUnitIndex],[MassFlowUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[VelocityDecimalPlaces],[MassFlowDecimalPlaces],[LevelMaximum],[LevelMinimum],[TemperatureMaximum],[TemperatureMinimum],[DensityMaximum],[DensityMinimum],[PressureMaximum],[PressureMinimum],[VolumetricFlowMaximum],[VolumetricFlowMinimum],[VolumeMaximum],[VolumeMinimum],[MassMaximum],[MassMinimum],[VelocityMaximum],[VelocityMinimum],[MassFlowMaximum],[MassFlowMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[PointTemplateGuid],[SiteGuid],[PointTemplateTypeApplicationStringGuid],[ProfileImageGuid],[DefaultDrawingGuid],[PointCommandStatus],[DeviceAlarmMaps],[PointLogicScript],[Version],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblPointTemplate) WITH TIES [dbo].[tblPointTemplate].[ID],[dbo].[tblPointTemplate].[Description],[dbo].[tblPointTemplate].[Standard],[dbo].[tblPointTemplate].[ExecutionInterval],[dbo].[tblPointTemplate].[LevelUnitIndex],[dbo].[tblPointTemplate].[TemperatureUnitIndex],[dbo].[tblPointTemplate].[DensityUnitIndex],[dbo].[tblPointTemplate].[PressureUnitIndex],[dbo].[tblPointTemplate].[FlowUnitIndex],[dbo].[tblPointTemplate].[VolumeUnitIndex],[dbo].[tblPointTemplate].[MassUnitIndex],[dbo].[tblPointTemplate].[VelocityUnitIndex],[dbo].[tblPointTemplate].[MassFlowUnitIndex],[dbo].[tblPointTemplate].[LevelDecimalPlaces],[dbo].[tblPointTemplate].[TemperatureDecimalPlaces],[dbo].[tblPointTemplate].[DensityDecimalPlaces],[dbo].[tblPointTemplate].[PressureDecimalPlaces],[dbo].[tblPointTemplate].[FlowDecimalPlaces],[dbo].[tblPointTemplate].[VolumeDecimalPlaces],[dbo].[tblPointTemplate].[MassDecimalPlaces],[dbo].[tblPointTemplate].[VelocityDecimalPlaces],[dbo].[tblPointTemplate].[MassFlowDecimalPlaces],[dbo].[tblPointTemplate].[LevelMaximum],[dbo].[tblPointTemplate].[LevelMinimum],[dbo].[tblPointTemplate].[TemperatureMaximum],[dbo].[tblPointTemplate].[TemperatureMinimum],[dbo].[tblPointTemplate].[DensityMaximum],[dbo].[tblPointTemplate].[DensityMinimum],[dbo].[tblPointTemplate].[PressureMaximum],[dbo].[tblPointTemplate].[PressureMinimum],[dbo].[tblPointTemplate].[VolumetricFlowMaximum],[dbo].[tblPointTemplate].[VolumetricFlowMinimum],[dbo].[tblPointTemplate].[VolumeMaximum],[dbo].[tblPointTemplate].[VolumeMinimum],[dbo].[tblPointTemplate].[MassMaximum],[dbo].[tblPointTemplate].[MassMinimum],[dbo].[tblPointTemplate].[VelocityMaximum],[dbo].[tblPointTemplate].[VelocityMinimum],[dbo].[tblPointTemplate].[MassFlowMaximum],[dbo].[tblPointTemplate].[MassFlowMinimum],[dbo].[tblPointTemplate].[CreatedDate],[dbo].[tblPointTemplate].[CreatedBy],[dbo].[tblPointTemplate].[UpdatedDate],[dbo].[tblPointTemplate].[UpdatedBy],[dbo].[tblPointTemplate].[PointTemplateGuid],[dbo].[tblPointTemplate].[SiteGuid],[dbo].[tblPointTemplate].[PointTemplateTypeApplicationStringGuid],[dbo].[tblPointTemplate].[ProfileImageGuid],[dbo].[tblPointTemplate].[DefaultDrawingGuid],[dbo].[tblPointTemplate].[PointCommandStatus],[dbo].[tblPointTemplate].[DeviceAlarmMaps],[dbo].[tblPointTemplate].[PointLogicScript],[dbo].[tblPointTemplate].[Version],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblPointTemplate]
                        INNER JOIN (SELECT [PointTemplateToSiteGuid],[PointTemplateGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedPointTemplateForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblPointTemplate].[PointTemplateGuid] = data.[PointTemplateGuid]
                        INNER JOIN [track].[tblPointTemplate] CT
                            ON CT.PK_PointTemplateGuid = [dbo].[tblPointTemplate].[PointTemplateGuid] 
                        INNER JOIN [track].[tblEntityPointTemplateToSite] MAPCT
                            ON MAPCT.PK_PointTemplateToSiteGuid = data.[PointTemplateToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
