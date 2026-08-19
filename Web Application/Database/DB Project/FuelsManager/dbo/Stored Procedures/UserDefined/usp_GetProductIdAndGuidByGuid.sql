

/*
	EXEC [dbo].[usp_GetProductIdAndGuidByGuid] NULL, '792BCE1F-CEA2-4234-B910-88DE10F3C5C7'
	EXEC [dbo].[usp_GetProductIdAndGuidByGuid] '6F38FF9E-D815-4E5B-B6B6-E6EAC0B1B76B', '80B08634-D356-4569-B9A2-CD36DF955BD0'
*/



CREATE PROCEDURE [dbo].[usp_GetProductIdAndGuidByGuid]
(
	@TargetSiteGuid uniqueidentifier, @ProductGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProductIdAndGuidByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product record that has a given Product Guid and that has been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @ProductGuid: If @TargetSiteGuid is null, then @ProductGuid is the Guid of the Product to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the Product record to retrieve.
	-- 2. @TargetSiteGuid: If TargetSiteGuid is not null, then it is used as the target owner site of the record version that needs to be retrieved.
	-- 3. This query can be used in two modes: 
	--		(a) When the exact GUID of the target Product record is known, in which case the @TargetSiteGuid can be left null.
	--		(b) When trying to verify if a product record has a record version (child or parent) against a specific site/sitegroup, in which case the @TargetSiteGuid must be provided.
	-- 4. This stored procedure replaces the ProductClass.SelectIdAndGuidSql() inline SQL for the case where the bInTransaction is false.
	--    This functionality was copied over from the inline SQL as part of the Record Versioning changes, and the need for this version of the Stored Procedure will need to be reviewed in the future.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblProducts
		WHERE ProductGuid = @ProductGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @ProductGuid
		END

		SELECT ProductID, ProductGuid, _MasterRecordGuid, SiteGuid
		FROM tblProducts
		WHERE ProductGuid = @targetRecordGuid

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
						+ 'Procedure Name: [dbo].usp_GetProductIdAndGuidByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
