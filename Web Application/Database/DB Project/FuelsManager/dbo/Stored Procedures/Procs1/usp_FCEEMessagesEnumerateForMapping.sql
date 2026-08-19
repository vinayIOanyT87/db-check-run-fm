CREATE PROCEDURE [dbo].[usp_FCEEMessagesEnumerateForMapping]
	-- Add the parameters for the stored procedure here
(
	@fceeMappingGuid uniqueidentifier
)
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
BEGIN TRY
	SELECT TOP (1) fm.FCEEMessageGuid, fm.Timestamp, fm.ImeiNumber, fm.MsgType, fm.[Index], fm.BinaryData, fm.EdgeData, fm.SoftwareVersion, fm.Validity FROM [dbo].[tblFCEEMessage] fm
	INNER JOIN [dbo].[tblFCEDevice] fd on fd.ImeiNumber = fm.ImeiNumber
	INNER JOIN [dbo].[tblFCEEMapping] fceeMapping on fceeMapping.[Index] = fm.[Index] AND fceeMapping.MsgType = fm.MsgType AND COALESCE(fm.Device, 255) = COALESCE(fceeMapping.Device, 255)
	WHERE @fceeMappingGuid = fceeMapping.FCEEMappingGuid
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
