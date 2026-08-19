CREATE PROCEDURE [dbo].[usp_FCEEDeviceTimeoutMessages]
	@FCEDeviceGuid UNIQUEIDENTIFIER
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
BEGIN TRY

WITH T AS(Select Validity, x.ImeiNumber, MsgType, Timestamp, [Index], [Device], BinaryData, EdgeData, x.CreatedDate, x.UpdatedDate
From
(
	Select Validity,ImeiNumber,MsgType, Timestamp, [Index], [Device], BinaryData, EdgeData, CreatedDate, UpdatedDate, Row_Number() Over (Partition By ImeiNumber, MsgType, [Index] Order By Timestamp DESC) as [RowNum] From  [tblFCEEMessage]
) x 
INNER JOIN tblFCEDevice d on d.ImeiNumber = x.ImeiNumber 
WHERE x.RowNum = 1 AND d.FCEDeviceGuid=@FCEDeviceGuid AND MsgType BETWEEN 1 AND 20 AND x.CreatedDate >= DATEADD(day, -1, GETDATE()))
UPDATE T
SET Validity = 0, UpdatedDate = SYSDATETIMEOFFSET()

Select x.ImeiNumber, MsgType, Timestamp, [Index], [Device], BinaryData, EdgeData, x.CreatedDate, x.UpdatedDate
From
(
	Select ImeiNumber,MsgType, Timestamp, [Index], [Device], BinaryData,EdgeData, CreatedDate, UpdatedDate, Row_Number() Over (Partition By ImeiNumber, MsgType, [Index] Order By Timestamp DESC) as [RowNum] From  [tblFCEEMessage]
) x 
INNER JOIN tblFCEDevice d on d.ImeiNumber = x.ImeiNumber 
WHERE x.RowNum = 1 AND d.FCEDeviceGuid=@FCEDeviceGuid AND MsgType BETWEEN 1 AND 20 AND x.CreatedDate >= DATEADD(day, -1, GETDATE())
Order By x.ImeiNumber, MsgType


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