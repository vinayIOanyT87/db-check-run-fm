CREATE PROCEDURE [dbo].[usp_PointGroupRowGetBypointGroupGuid]
(
	@PointGroupGuid uniqueidentifier 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_PointGroupRowGetBypointGroupGuid] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0
	-- Purpose: Retrieve a Point Group Row by Guid
	-- Notes:
	-- 1. @PointGroupGuid: @PointGroupGuid is the Guid of the Point Group to retrieve
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT pgr.PointGroupGuid,
		pgr.PointGroupRowsGuid,
		pgr.RowsDefinition,
		pgr.OwnerUserGuid,
		pgr.SiteGuid,
		pgr.CreatedDate,
		pgr.CreatedBy,
		pgr.UpdatedDate,
		pgr.UpdatedBy
 		FROM tblPointGroupRows pgr
		WHERE pgr.PointGroupGuid = @PointGroupGuid

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
						+ 'Procedure Name: [dbo].[usp_PointGroupRowGetBypointGroupGuid]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END