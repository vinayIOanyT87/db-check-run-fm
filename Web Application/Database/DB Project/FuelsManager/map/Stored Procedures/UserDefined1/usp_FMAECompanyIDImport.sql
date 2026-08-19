
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
CREATE PROCEDURE [map].[usp_FMAECompanyIDImport]
(
	@FMAETranslations map.FMAECompanyIDType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		MERGE map.tblFMAECompanyID AS target
		USING (SELECT 
				FMAECompanyID, 
				CompanyGuid,  
				UserID
				FROM @FMAETranslations
			) AS source
		ON source.FMAECompanyID = target.FMAECompanyID
		WHEN MATCHED THEN 
			UPDATE 
			SET	
				target.CompanyGuid = source.CompanyGuid,
				target.UpdatedDate = SYSDATETIMEOFFSET(),
				target.UpdatedBy = source.UserID
		WHEN NOT MATCHED THEN 
			INSERT
			(
				FMAECompanyIDMapGuid,
				FMAECompanyID,
				CompanyGuid,
				CreatedDate,
				CreatedBy,
				UpdatedDate,
				UpdatedBy
			)
			VALUES
			(
				NEWID(),
				source.FMAECompanyID,
				source.CompanyGuid,
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
						+ 'Procedure Name: map.usp_FMAECompanyIDImport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END