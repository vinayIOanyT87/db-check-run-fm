CREATE PROCEDURE [dbo].[usp_GetMovementSummaryIfNewer]
(
	@MovementSummaryGuid UNIQUEIDENTIFIER,
	@prevRowVersion ROWVERSION
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetMovementSummaryIfNewer] 
	-- Author: Srini Divyakolu
	-- Version/Date: 1.0
	-- Purpose: Get latest Movement Summary by Guid if it's newer
	-- Notes:
	-- 1. @MovementSummaryGuid: @MovementSummaryGuid is the Guid of the Movement Summary to retrieve
	-- 2. @prevRowVersion: Previous RowVersion to compare
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @NeedsRefresh INT

		SELECT @NeedsRefresh = (SELECT CASE WHEN EXISTS(SELECT 1 FROM tblMovementSummary WHERE   MovementSummaryGuid = @movementSummaryGuid AND CONVERT(BINARY(8), _RowVersion, 1) > @prevRowVersion) THEN 1 ELSE 0 END)

		IF @NeedsRefresh = 1
		BEGIN
			EXEC [dbo].[usp_MovementSummaryGetByPK] @MovementSummaryGuid = @MovementSummaryGuid
		END

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
						+ 'Procedure Name: [dbo].[usp_GetMovementSummaryIfNewer]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END