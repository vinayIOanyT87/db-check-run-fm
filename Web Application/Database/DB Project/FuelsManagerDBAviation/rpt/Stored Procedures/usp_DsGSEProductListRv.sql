Create PROCEDURE [rpt].[usp_DsGSEProductListRv]
@SiteGuid UNIQUEIDENTIFIER,
@ActiveOnly bit
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsProductListRv] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-05-08 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Products based on Site ID for reports.
	-- Notes:
	-- 1. @SiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		IF @ActiveOnly = 1
			BEGIN
				SELECT a._MasterRecordGuid, a.ProductID
				FROM tblProducts a 
				INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) b
				ON a.ProductGuid = b.ProductGuid
				WHERE a.LockedOut = 0 and a.AviationFuelFlag = 0 and a.GroundFuel = 1
				ORDER BY ProductID
			END
		ELSE
			BEGIN
				SELECT a._MasterRecordGuid, a.ProductID
				FROM tblProducts a 
				INNER JOIN erv.udf_GetProductRecordVersions(@SiteGuid) b
				ON a.ProductGuid = b.ProductGuid and a.AviationFuelFlag = 0 and a.GroundFuel = 1
				ORDER BY ProductID
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
						+ 'Procedure Name: [rpt].[usp_DsProductListRv]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END