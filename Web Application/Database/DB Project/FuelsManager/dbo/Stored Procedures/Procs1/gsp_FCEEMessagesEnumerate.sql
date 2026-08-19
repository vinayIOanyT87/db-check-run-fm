CREATE PROCEDURE [dbo].[gsp_FCEEMessagesEnumerate]
(
	@StartDate date='1970-01-01',
	@EndDate date='2200-12-31'
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[[gsp_FCEEMessagesEnumerate]] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	------------------------------------------------------------------------------------------------------

SET NOCOUNT ON;
BEGIN TRY
	SELECT TOP (5001) fm.FCEEMessageGuid, fm.Timestamp, fm.ImeiNumber, fm.MsgType, fm.[Index], fm.BinaryData, fm.EdgeData, fm.SoftwareVersion, fm.Validity FROM [dbo].[tblFCEEMessage] fm
	WHERE fm.Timestamp BETWEEN @StartDate AND @EndDate
	ORDER BY fm.Timestamp DESC
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
						+ 'Procedure Name: gsp_FCEEMappingInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     