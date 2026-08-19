CREATE PROCEDURE [dbo].[usp_MovementSummaryGetByPK]
(
	@MovementSummaryGuid uniqueidentifier 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_MovementSummaryGetByPK] 
	-- Author: Francisco Martin
	-- Version/Date: 1.1
	-- Purpose: Retrieve a Movement Summary by Guid
	-- Notes:
	-- 1. @MovementSummaryGuid: @MovementSummaryGuid is the Guid of the Movement Summary to retrieve
	-- Last Updated: 10-01-2025
	-- By: Srini
	-- Updated to include _RowVersion in the result set
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT ms.MovementSummaryGuid,
		ms.ID,
		ms.[Description],
		ms.MovementSummaryType,
		ms.ColumnsDefinition,
		ms.FontSize,
		ms.RowsDefinition,
		ms.OwnerUserGuid,
		ms.SiteGuid,
		ms.CreatedDate,
		ms.CreatedBy,
		ms.UpdatedDate,
		ms.UpdatedBy,
		ms._RowVersion
 		FROM tblMovementSummary ms
		WHERE ms.MovementSummaryGuid = @MovementSummaryGuid

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
						+ 'Procedure Name: [dbo].[usp_MovementSummaryGetByPK]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END