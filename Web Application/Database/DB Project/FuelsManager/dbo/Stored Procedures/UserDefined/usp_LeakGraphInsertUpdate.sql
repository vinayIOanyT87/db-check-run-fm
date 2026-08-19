CREATE PROCEDURE [dbo].[usp_LeakGraphInsertUpdate]
(
	@LeakSamples dbo.LeakGraphType READONLY
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY

		MERGE tblLeakReportGraph AS target
		USING (
			SELECT 
				LeakReportId,
				SampleTime,
				SampleVolume,
				IsUsed
			FROM @LeakSamples
			) AS source
		ON source.LeakReportId = target.LeakReportId
			AND source.SampleTime = target.SampleTime
		WHEN MATCHED THEN UPDATE SET
			SampleVolume = source.SampleVolume,
			IsUsed = source.IsUsed
		WHEN NOT MATCHED THEN INSERT 
		(
			LeakReportId,
			SampleTime,
			SampleVolume,
			IsUsed
		)
		VALUES
		(
			source.LeakReportId,
			source.SampleTime,
			source.SampleVolume,
			source.IsUsed
		);

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
		         
		IF (@_ErrNumber = 50000 AND  @_ErrMessage = 'Cannot insert/update duplicate DocumentNumber.')
		BEGIN
			RAISERROR(@_ErrMessage, 16, 1);
		END
		ELSE
		BEGIN     
			SET @_ErrProcName= ERROR_PROCEDURE();        
			SET @_ErrLineNumber = ERROR_LINE();            
			SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
							+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
							+ 'Procedure Name: dbo.usp_TransactionHeaderInsertUpdate' + CHAR(13)+CHAR(10)                  
							+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
			RAISERROR(@_ErrMessage, 16, 1);
		END      
	END CATCH    
END 
