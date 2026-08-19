CREATE PROCEDURE [dbo].[usp_MovementHistoryGetFinalRecordInfo]
(
	@RootParentGuid UNIQUEIDENTIFIER
	, @ParentGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		DECLARE @FinalRecordRecordType INT
		SET @FinalRecordRecordType = 3

		SELECT MovementHistoryGuid, RecordSeq, [Node] FROM tblMovementHistory
		WHERE RootParentGuid = @RootParentGuid AND ParentGuid = @ParentGuid AND RecordType = @FinalRecordRecordType
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
						+ 'Procedure Name: dbo.usp_MovementHistoryGetFinalRecordInfo' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
