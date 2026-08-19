CREATE PROCEDURE [dbo].[usp_MovementHistoryUpdateComment]
(
	@MovementHistoryGuid UNIQUEIDENTIFIER
	, @Comment NVARCHAR(1000)
	, @CommentUserID NVARCHAR(50)
	, @CommentDateTime DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	
	BEGIN TRY
		UPDATE tblMovementHistory SET Comment = @Comment, CommentUserID = @CommentUserID, CommentDateTime = @CommentDateTime
		WHERE MovementHistoryGuid = @MovementHistoryGuid
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
						+ 'Procedure Name: dbo.usp_MovementHistoryUpdateComment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 

