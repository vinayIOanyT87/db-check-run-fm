CREATE PROCEDURE [dbo].[usp_CleanSessionTable]
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @OneDayInMinutes INT
		DECLARE @NoTimeout INT

		SET @OneDayInMinutes = 1440
		SET @NoTimeout = 0

		-- The ABS is to ensure absolute value for the minutes difference.
		-- The CONVERT function converts from date time offset to UTC to ensure the comparison is correct with the 
		-- current time of the server which is also converted to UTC.
		CREATE TABLE #tblTimeDifference (MinutesDiff INT, TimeoutInMinutes INT, SessionGuid UNIQUEIDENTIFIER)
		INSERT INTO #tblTimeDifference
		SELECT ABS(DATEDIFF(MINUTE, CONVERT(DATETIME2, UpdatedDate, 1), CONVERT(DATETIME2, SYSDATETIMEOFFSET(), 1))) AS MinutesDiff
			   , CASE WHEN [Timeout] <= @NoTimeout THEN @OneDayInMinutes ELSE [Timeout] END AS TimeoutInMinutes
			   , SessionGuid
		FROM dbo.tblSessions

		CREATE TABLE #tblDeleteList (SessionGuid UNIQUEIDENTIFIER, DeleteFlag INT)
		INSERT INTO #tblDeleteList
		SELECT SessionGuid
			  , CASE WHEN MinutesDiff >= TimeoutInMinutes THEN 1 ELSE 0 END AS DeleteFlag
		FROM #tblTimeDifference

		DELETE FROM dbo.tblOperateStatistics WHERE SessionGuid IN (SELECT SessionGuid FROM #tblDeleteList WHERE DeleteFlag = 1)

		DELETE FROM dbo.tblSessions WHERE SessionGuid IN (SELECT SessionGuid FROM #tblDeleteList WHERE DeleteFlag = 1)

		DROP TABLE #tblTimeDifference
		DROP TABLE #tblDeleteList
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
						+ 'Procedure Name: usp_CleanSessionTable' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
