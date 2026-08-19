/*
	DROP PROCEDURE [archive].[usp_LogArchivingSession]

	EXEC [archive].[usp_LogArchivingSession] 85678, '2005-03-01', 0, 'OnlineMode', '[AlarmAndEvent Log].[IsArchivngOn] = True; [Audit Log].[IsArchivngOn] = True; [Transaction Tables].[IsArchivngOn] = False'

*/
CREATE PROCEDURE [archive].[usp_LogArchivingSession]
(
	@archiveETLAuditKey bigint, 
	@cutOffDate date,
	@inDebuggingMode bit,
	@archivingMode nvarchar(25),
	@scopeArchivingOnString nvarchar(250)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_LogArchivingSession]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Log details of the last archiving session.
	-- Notes:
	-- 1. @archiveETLAuditKey: Archiving ETLAuditKey for the archive session
	-- 2. @cutOffDate: Cut-off date for the archive session
	-- 3. @inDebuggingMode: Flag to indicate whether DebuggingMode is on or off
	-- 4. @archivingMode: Archiving Mode
	-- 5. ScopeArchivingOnString: Combined string capturing the IsArchivingOn flag of all the scopes
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @alarmAndEventLogCount int
		DECLARE @auditLogCount int
		DECLARE @transactionCount int
		DECLARE @logText nvarchar(1000)
		DECLARE @siteAdminKey uniqueidentifier

		SELECT @siteAdminKey = SiteGuid FROM dbo.tblSites WHERE ID = 'SiteAdmin'

		SELECT @alarmAndEventLogCount = COUNT(*) FROM archive.tblAlarmAndEventLogLastProcessedRecords

		SELECT @auditLogCount = COUNT(*) FROM archive.tblAuditLogLastProcessedRecords

		SELECT @transactionCount = COUNT(*) FROM archive.tblTransactionLastProcessedRecords
		WHERE SourceArchiveTable = '[dbo].[tblTransactions]'

		SET @logText = 'Archiving ETL Audit Key = ' + CONVERT(varchar(10), @archiveETLAuditKey)
							+ '; Cut-off Date = ' + CONVERT(varchar(4), Year(@cutOffDate)) + '-' + CONVERT(varchar(2), Month(@cutOffDate)) + '-' + CONVERT(varchar(4), Day(@cutOffDate))
							+ '; Archiving Mode = ' + @archivingMode
							+ '; In Debugging Mode = ' + CASE @inDebuggingMode WHEN 0 THEN 'False' ELSE 'True' END
							+ '; ' + @scopeArchivingOnString
							+ '; AlarmAndEventLog Archive Count = ' + CONVERT(varchar(10), @alarmAndEventLogCount)
							+ '; AuditLog Archive Count = ' + CONVERT(varchar(10), @auditLogCount)
							+ '; Transaction Archive Count = ' + CONVERT(varchar(10), @transactionCount)

		INSERT INTO [dbo].[tblAuditLog] (SessionID, ActionID, TypeID, ID, ParentTypeId, PropertyId, NewValue, OldValue, SiteGuid, CreatedBy, CreatedDate) 
		VALUES (NULL, 'Archive', 'Archiving Scope', '', '', '', @logText, '', @siteAdminKey, 'Administrator', SYSDATETIMEOFFSET())

		INSERT INTO [dbo].[tblAlarmAndEventLog] (Source, Alarm, Acknowledged, ID, AssociatedData, SiteGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
		VALUES ('System', 0, 0, 'Archiving Session Event', @logText, @siteAdminKey, 'Administrator', SYSDATETIMEOFFSET(), 'Administrator', GetDate())

	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [archive].[usp_LogArchivingSession]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


