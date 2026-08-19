CREATE PROCEDURE usp_CleanupPointCalculatorRunTables 
	@IntervalMinutesToKeep int = 1440 
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		BEGIN TRANSACTION
			SELECT PointCalculatorRunId INTO #TEMP FROM [dbo].[tblPointCalculatorRuns] WHERE UpdatedDate < DATEADD(MINUTE, @IntervalMinutesToKeep * -1, SYSDATETIMEOFFSET())
			DELETE FROM [dbo].[tblPointCalculatorRunDetails] WHERE PointCalculatorRunId IN (SELECT PointCalculatorRunId FROM #TEMP)
			DELETE FROM [dbo].[tblPointCalculatorRuns]  WHERE PointCalculatorRunId IN (SELECT PointCalculatorRunId FROM #TEMP)
		COMMIT
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
         ROLLBACK

		DECLARE @ErrMessage NVARCHAR(2048)
			,	@ErrNumber INT
			,	@ErrProcName NVARCHAR(126)
			,	@LineNumber INT
		
		SET @ErrMessage = ERROR_MESSAGE()
		SET	@ErrNumber = ERROR_NUMBER()
		SET @ErrProcName= ERROR_PROCEDURE()
		SET @LineNumber = ERROR_LINE()
		
		SET @ErrMessage =		'Error: ' + @ErrMessage + CHAR(13)+CHAR(10)
							+	'Number: ' + CAST(@ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10) 
							+	'Procedure Name: ' + ISNULL(@ErrProcName,OBJECT_NAME(@@PROCID)) + CHAR(13)+CHAR(10) 
							+	'Line Number: ' + ISNULL(CAST(@LineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10) 
		RAISERROR(@ErrMessage,18,1)
	END CATCH

END
