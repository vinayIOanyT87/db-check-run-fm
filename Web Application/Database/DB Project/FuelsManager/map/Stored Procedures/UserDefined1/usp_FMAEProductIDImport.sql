
/*
=============================================
Author: Ryan Hill
Create date: 01/13/2014
Description:
	Import translations into the system.
	Translations that are found with a matching legacy ID will be updated.
	Translations not found will be created.
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAEProductIDImport]
(
	@FMAETranslations map.FMAEProductIDType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		MERGE map.tblFMAEProductID AS target
		USING (SELECT 
				FMAEProductID, 
				ProductGuid,  
				UserID
				FROM @FMAETranslations
			) AS source
		ON source.FMAEProductID = target.FMAEProductID
		WHEN MATCHED THEN 
			UPDATE 
			SET	
				target.ProductGuid = source.ProductGuid,
				target.UpdatedDate = SYSDATETIMEOFFSET(),
				target.UpdatedBy = source.UserID
		WHEN NOT MATCHED THEN 
			INSERT
			(
				FMAEProductIDMapGuid,
				FMAEProductID,
				ProductGuid,
				CreatedDate,
				CreatedBy,
				UpdatedDate,
				UpdatedBy
			)
			VALUES
			(
				NEWID(),
				source.FMAEProductID,
				source.ProductGuid,
				SYSDATETIMEOFFSET(),
				source.UserID,
				SYSDATETIMEOFFSET(),
				source.UserID	
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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: map.usp_FMAEProductIDImport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END