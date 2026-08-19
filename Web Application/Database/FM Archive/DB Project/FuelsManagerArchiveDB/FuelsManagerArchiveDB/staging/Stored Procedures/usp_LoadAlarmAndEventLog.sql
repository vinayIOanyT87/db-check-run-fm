/*
	DROP PROCEDURE [Staging].[usp_LoadAlarmAndEventLog]
 
	EXEC [staging].[usp_LoadAlarmAndEventLog]
 
*/
CREATE PROCEDURE [staging].[usp_LoadAlarmAndEventLog]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadAlarmAndEventLog]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the AlarmAndEventLog records from staging into the tblAlarmAndEventLog table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblAlarmAndEventLog table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblAlarmAndEventLog
				WHERE IsProcessed = 0
			) = 0)
		BEGIN
			RETURN
		END
 
	DECLARE @tblInsertedRecords TABLE
	(
		[SKey] [int] IDENTITY(1,1) NOT NULL,
		[RecordGuid] [uniqueidentifier] NOT NULL,
		[RecordIndex] [bigint] NOT NULL,
		[ParentRecordGuid] [uniqueidentifier] NULL
	)
 
	DECLARE @tblUpdatedRecords TABLE
	(
		[SKey] [int] IDENTITY(1,1) NOT NULL,
		[RecordGuid] [uniqueidentifier] NOT NULL,
		[RecordIndex] [bigint] NOT NULL,
		[ParentRecordGuid] [uniqueidentifier] NULL
	)
 
		INSERT INTO @tblUpdatedRecords ([RecordGuid], [RecordIndex])
		SELECT src.[AlarmAndEventLogGuid], src.[SequenceNumber]
		FROM staging.tblAlarmAndEventLog src
		WHERE src.AlarmAndEventLogGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblAlarmAndEventLog] tgt
			WHERE tgt.AlarmAndEventLogGuid = src.AlarmAndEventLogGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex])
		SELECT src.[AlarmAndEventLogGuid], src.[SequenceNumber]
		FROM staging.tblAlarmAndEventLog src
		WHERE src.AlarmAndEventLogGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblAlarmAndEventLog] tgt
			WHERE tgt.AlarmAndEventLogGuid = src.AlarmAndEventLogGuid
		)
 
 
		INSERT INTO [dbo].[tblAlarmAndEventLog]
		(
		[SequenceNumber]
		, [Source]
		, [Alarm]
		, [ID]
		, [AssociatedData]
		, [CategoryID]
		, [PriorityID]
		, [Acknowledged]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [AlarmAndEventLogGuid]
		, [SiteGuid]
		, [SourceNode]
		, [UpdatedDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[SequenceNumber]
		, src.[Source]
		, src.[Alarm]
		, src.[ID]
		, src.[AssociatedData]
		, src.[CategoryID]
		, src.[PriorityID]
		, src.[Acknowledged]
		, src.[CreatedDate]
		, src.[CreatedBy]
		, src.[UpdatedDate]
		, src.[UpdatedBy]
		, src.[AlarmAndEventLogGuid]
		, src.[SiteGuid]
		, src.[SourceNode]
		, src.[UpdatedDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblAlarmAndEventLog src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.AlarmAndEventLogGuid
		WHERE src.AlarmAndEventLogGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[SequenceNumber] = src.[SequenceNumber]
		, tgt.[Source] = src.[Source]
		, tgt.[Alarm] = src.[Alarm]
		, tgt.[ID] = src.[ID]
		, tgt.[AssociatedData] = src.[AssociatedData]
		, tgt.[CategoryID] = src.[CategoryID]
		, tgt.[PriorityID] = src.[PriorityID]
		, tgt.[Acknowledged] = src.[Acknowledged]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[UpdatedDate] = src.[UpdatedDate]
		, tgt.[UpdatedBy] = src.[UpdatedBy]
		, tgt.[AlarmAndEventLogGuid] = src.[AlarmAndEventLogGuid]
		, tgt.[SiteGuid] = src.[SiteGuid]
		, tgt.[SourceNode] = src.[SourceNode]
		, tgt.[UpdatedDateKey] = src.[UpdatedDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblAlarmAndEventLog] tgt
		INNER JOIN staging.tblAlarmAndEventLog src
		ON src.AlarmAndEventLogGuid = tgt.AlarmAndEventLogGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.AlarmAndEventLogGuid
		WHERE src.AlarmAndEventLogGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblAlarmAndEventLog SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblAlarmAndEventLog]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblAlarmAndEventLog]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblUpdatedRecords
 
	END TRY
	BEGIN CATCH
		DECLARE @_ErrMessage nvarchar(2048),
		@_ErrNumber int,
		@_ErrProcName nvarchar(126),
		@_ErrLineNumber int;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName = ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
		+ 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
		+ 'Procedure Name: [staging].[usp_LoadAlarmAndEventLog]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
