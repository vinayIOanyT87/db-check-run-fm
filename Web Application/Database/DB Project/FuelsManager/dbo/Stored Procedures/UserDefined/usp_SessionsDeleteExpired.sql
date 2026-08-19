
CREATE PROCEDURE [dbo].[usp_SessionsDeleteExpired]
AS
BEGIN 
	BEGIN TRY
		SET NOCOUNT ON;
		
		/* Delete records where the session is older than the timeout value, or if a negative timeout value is specified 
		where the session is older than a day. Sessions with a timeout = 0 are not deleted at the moment */
		DECLARE @Now DATETIMEOFFSET = SYSDATETIMEOFFSET()

		DELETE FROM [map].[tblSessionToSQLProcess] 
		WHERE SessionGuid IN (SELECT SessionGuid FROM dbo.tblSessions 
			WHERE ([Timeout] > 0 AND DATEDIFF(mi, UpdatedDate, @Now) > [Timeout])
			OR ([Timeout] < 0 AND DATEDIFF(mi, UpdatedDate, @Now) > 1440)) -- 1440 minutes = 1 day

		SET NOCOUNT OFF;

		DELETE FROM dbo.tblOperateStatistics WHERE SessionGuid IN (SELECT SessionGuid FROM dbo.tblSessions 
		WHERE ([Timeout] > 0 AND DATEDIFF(mi, UpdatedDate, @Now) > [Timeout])
			OR ([Timeout] < 0 AND DATEDIFF(mi, UpdatedDate, @Now) > 1440)) -- 1440 minutes = 1 day


		DELETE FROM dbo.tblSessions 
		WHERE ([Timeout] > 0 AND DATEDIFF(mi, UpdatedDate, @Now) > [Timeout])
			OR ([Timeout] < 0 AND DATEDIFF(mi, UpdatedDate, @Now) > 1440) -- 1440 minutes = 1 day

	END TRY
	BEGIN CATCH
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
		RAISERROR(@ErrMessage,16,1)
	END CATCH
END 


