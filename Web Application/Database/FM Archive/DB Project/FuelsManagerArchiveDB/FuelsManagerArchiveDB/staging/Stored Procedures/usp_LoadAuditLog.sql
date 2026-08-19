/*
	DROP PROCEDURE [Staging].[usp_LoadAuditLog]
 
	EXEC [staging].[usp_LoadAuditLog]
 
*/
CREATE PROCEDURE [staging].[usp_LoadAuditLog]
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [staging].[usp_LoadAuditLog]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Loads all the AuditLog records from staging into the tblAuditLog table in the Archive database.
-- Notes:
-- 1. This operation assumes that no referential integrity or dependencies are enforced on the tblAuditLog table.
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
		IF (
			(	SELECT COUNT(*) FROM staging.tblAuditLog
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
		SELECT src.[AuditLogGuid], src.[SourceClusterIdx]
		FROM staging.tblAuditLog src
		WHERE src.AuditLogGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND EXISTS
		(
			SELECT * FROM [dbo].[tblAuditLog] tgt
			WHERE tgt.AuditLogGuid = src.AuditLogGuid
		)
 
 
		INSERT INTO @tblInsertedRecords ([RecordGuid], [RecordIndex])
		SELECT src.[AuditLogGuid], src.[SourceClusterIdx]
		FROM staging.tblAuditLog src
		WHERE src.AuditLogGuid IS NOT NULL
		AND src.IsProcessed = 0
		AND src.IgnoreRecord = 0
		AND NOT EXISTS
		(
			SELECT * FROM [dbo].[tblAuditLog] tgt
			WHERE tgt.AuditLogGuid = src.AuditLogGuid
		)
 
 
		INSERT INTO [dbo].[tblAuditLog]
		(
		[SessionID]
		, [ActionID]
		, [TypeID]
		, [ID]
		, [PropertyID]
		, [NewValue]
		, [OldValue]
		, [CreatedDate]
		, [CreatedBy]
		, [ParentTypeID]
		, [AuditLogGuid]
		, [SiteGuid]
		, [AuditedDate]
		, [SourceNode]
		, [AuditContext]
		, [AuditedDateKey]
		, [ArchiveDate]
		, [ETLProcessKey]
		)
		SELECT
		src.[SessionID]
		, src.[ActionID]
		, src.[TypeID]
		, src.[ID]
		, src.[PropertyID]
		, src.[NewValue]
		, src.[OldValue]
		, src.[CreatedDate]
		, src.[CreatedBy]
		, src.[ParentTypeID]
		, src.[AuditLogGuid]
		, src.[SiteGuid]
		, src.[AuditedDate]
		, src.[SourceNode]
		, src.[AuditContext]
		, src.[AuditedDateKey]
		, src.[ArchiveDate]
		, src.[ETLProcessKey]
		FROM staging.tblAuditLog src
		INNER JOIN @tblInsertedRecords b
		ON b.RecordGuid = src.AuditLogGuid
		WHERE src.AuditLogGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE tgt
		SET tgt.[SessionID] = src.[SessionID]
		, tgt.[ActionID] = src.[ActionID]
		, tgt.[TypeID] = src.[TypeID]
		, tgt.[ID] = src.[ID]
		, tgt.[PropertyID] = src.[PropertyID]
		, tgt.[NewValue] = src.[NewValue]
		, tgt.[OldValue] = src.[OldValue]
		, tgt.[CreatedDate] = src.[CreatedDate]
		, tgt.[CreatedBy] = src.[CreatedBy]
		, tgt.[ParentTypeID] = src.[ParentTypeID]
		, tgt.[AuditLogGuid] = src.[AuditLogGuid]
		, tgt.[SiteGuid] = src.[SiteGuid]
		, tgt.[AuditedDate] = src.[AuditedDate]
		, tgt.[SourceNode] = src.[SourceNode]
		, tgt.[AuditContext] = src.[AuditContext]
		, tgt.[AuditedDateKey] = src.[AuditedDateKey]
		, tgt.[ArchiveDate] = src.[ArchiveDate]
		, tgt.[ETLProcessKey] = src.[ETLProcessKey]
		FROM [dbo].[tblAuditLog] tgt
		INNER JOIN staging.tblAuditLog src
		ON src.AuditLogGuid = tgt.AuditLogGuid
		INNER JOIN @tblUpdatedRecords b
		ON b.RecordGuid = src.AuditLogGuid
		WHERE src.AuditLogGuid IS NOT NULL
		AND src.IgnoreRecord = 0
		AND src.IsProcessed = 0
 
 
		UPDATE staging.tblAuditLog SET IsProcessed = 1
 
		INSERT INTO staging.tblInsertedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblAuditLog]', RecordGuid, RecordIndex, ParentRecordGuid
		FROM @tblInsertedRecords
 
		INSERT INTO staging.tblUpdatedRecords
		(TargetTableName, RecordGuid, RecordIndex, ParentRecordGuid)
		SELECT '[dbo].[tblAuditLog]', RecordGuid, RecordIndex, ParentRecordGuid
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
		+ 'Procedure Name: [staging].[usp_LoadAuditLog]' + CHAR(13) + CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
		RAISERROR (@_ErrMessage, 16, 1);
	END CATCH
END
