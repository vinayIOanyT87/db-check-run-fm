CREATE PROCEDURE [dbo].[usp_MovementHistoryGetAllMovementsBySite]
(
	@SiteGuid UNIQUEIDENTIFIER
	, @StartTime DATETIME
	, @EndTime DATETIME
	, @OrderColumnName NVARCHAR (100)
	, @OrderDirection NVARCHAR (10)
	, @AutoGauge BIT
	, @HandGauge BIT
	, @MidnightRecord BIT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @Query NVARCHAR(MAX)

		-- We never want to show midnight records in the history
		-- Midnight records have a status that is anything but
		-- Inactive or Complete.
		SET @Query = 'SELECT * FROM tblMovementHistory'
		SET @Query = @Query + ' WHERE SiteGuid = ''' + CONVERT(VARCHAR(38), @SiteGuid) + ''''
		SET @Query = @Query + ' AND TimeStamp >= ''' + CONVERT(VARCHAR(25), @StartTime, 121) + ''''
		SET @Query = @Query + ' AND TimeStamp < ''' + CONVERT(VARCHAR(25), DATEADD(second, 1, @EndTime), 121) + '''' --adjust so that a request for end time 12:04:52 will capture end times with fractional seconds

		IF(@AutoGauge = 0)
		BEGIN			
			SET @Query = @Query + ' AND RecordType <> 1 '
		END

		IF(@HandGauge = 0)
		BEGIN			
			SET @Query = @Query + ' AND RecordType <> 2 '
		END

		IF(@MidnightRecord = 0)
		BEGIN			
			SET @Query = @Query + ' AND (MidnightRecord = 0 OR MidnightRecord IS NULL) '
		END

		-- If Midnight Records are requested (@MidnightRecord = 1), we should not exclude the other records at the same time
		--IF(@MidnightRecord = 1)
		--BEGIN
		--	SET @Query = @Query + ' AND MidnightRecord = 1 '
		--END
		
		SET @Query = @Query + ' ORDER BY ' + @OrderColumnName + ' ' + @OrderDirection + ', InitiationCount, RecordSeq, TransferDirection DESC'

		EXEC sp_executesql @Query
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
						+ 'Procedure Name: dbo.usp_MovementHistoryGetAllMovementsBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END