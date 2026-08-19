CREATE PROCEDURE [dbo].[usp_GetMovementID]
	@SiteGuid UniqueIdentifier,
	@UserID nvarchar(100),
	@MovementID nvarchar(30) OUT
AS
BEGIN 
	SET NOCOUNT ON
	BEGIN TRY
		DECLARE @SiteID NVARCHAR(30) = (SELECT ID FROM tblSites WHERE SiteGuid = @SiteGuid)
		DECLARE @MovementNumbers TABLE (MovementNumber INT)

		UPDATE tblSites SET MovementNumber = ISNULL(MovementNumber,0) + 1, UpdatedBy = @UserID, UpdatedDate = SYSDATETIMEOFFSET() OUTPUT INSERTED.MovementNumber INTO @MovementNumbers WHERE SiteGuid = @SiteGuid

		SET @MovementID = 'Movement ' + @SiteID + + ' ' + CONVERT(NVARCHAR(20),(SELECT TOP(1) MovementNumber FROM @MovementNumbers))	
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
						+ 'Procedure Name: usp_GetMovementID' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END

