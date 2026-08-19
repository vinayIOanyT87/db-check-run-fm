------------------------------------------------------------------------------------------------------
-- Stored Procedure: [dbo].[usp_DeleteGroupToRightByGroupGuid] 
-- Author: Richard R. Panachida
-- Version/Date: 1.0.000 / 2022-12-22
-- Purpose: Delete the entire mapping between a group and right based on the group Guid.
-- Notes:
-- 1. @GroupGuid: The group Guid to delete the mapping.
--
-- DECLARE @GroupGuid uniqueidentifier
-- SET @GroupGuid = '2A8766A8-93A3-4F3F-ADC3-EC5357E3054B'
-- EXEC [dbo].[usp_DeleteGroupToRightByGroupGuid] @GroupGuid
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_DeleteGroupToRightByGroupGuid]
(
	@GroupGuid uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRY	

		DELETE FROM map.tblGroupToRight
		WHERE GroupGuid = @GroupGuid

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
						+ 'Procedure Name: [dbo].usp_DeleteGroupToRightByGroupGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
