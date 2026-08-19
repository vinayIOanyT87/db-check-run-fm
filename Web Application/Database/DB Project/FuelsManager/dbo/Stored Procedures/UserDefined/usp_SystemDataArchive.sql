CREATE Procedure [dbo].[usp_SystemDataArchive]
(
		@start_date datetime,
		@end_date datetime,
		@data_selected nvarchar(64)
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_SystemDataArchive] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @start_date: Beginning of date range to archive records.
	-- 2. @end_date: End of date range to archive records.
	-- 3. @data_selected: A string identifying the type of data to be archived
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	/* SET NOCOUNT ON added to prevent extra result sets from 
	  interfering with SELECT statements.
	  */
	SET NOCOUNT ON; 

	CREATE TABLE #MSG (line int IDENTITY(1,1) NOT NULL, LogTime DateTime, Status nvarchar(32), Info nvarchar(4000)); 
	DECLARE @MSG nvarchar(MAX)

	INSERT INTO #MSG (LogTime , Status , Info )
	(Select GetDate(), 'Info' AS Status, 'Current UTC Date = ' + CONVERT(nvarchar, GETUTCDATE(), 101) + 
	' Archiving records between ' + CAST(@start_date AS nvarchar) + ' and ' + CAST(@end_date AS nvarchar) 
	AS Info); 

	IF UPPER(@data_selected) = 'ACCOUNTING'
	BEGIN
		EXEC dbo.usp_ArchiveTransactions @start_date, @end_date 
	END
	ELSE 
	BEGIN
		BEGIN TRY
			IF @data_selected = 'ALARM LOG'
			BEGIN	
				EXEC dbo.usp_ArchiveAlarmAndEventLog  @start_date, @end_date
			END
			
			IF @data_selected = 'AUDIT LOG'
			BEGIN	
				EXEC dbo.usp_ArchiveAuditLog  @start_date, @end_date
			END
			
			ELSE IF @data_selected = 'MAINTENANCE'
			BEGIN
				EXEC dbo.usp_ArchiveMaintenanceData @start_date ,@end_date
			END

			ELSE IF @data_selected = 'QUALITY CONTROL'
			BEGIN
				EXEC dbo.usp_ArchiveQualityData @start_date ,@end_date
				EXEC dbo.usp_ArchiveTestAndTestSetResultsData @start_date ,@end_date
			END

			IF @@ERROR != 0
			BEGIN
				SET @MSG = ERROR_MESSAGE()
				INSERT INTO #MSG (LogTime , Status , Info ) 
				(SELECT  GetDate(), 'Error' AS Status, @MSG AS Info); 
			END

		END TRY
		BEGIN CATCH

			SET @MSG = ERROR_MESSAGE()
			INSERT INTO #MSG (LogTime , Status , Info ) 
			(SELECT  GetDate(), 'Error' AS Status, @MSG AS Info); 
		
		END CATCH
	END

	SELECT * FROM #MSG

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
						+ 'Procedure Name: [dbo].usp_SystemDataArchive' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     