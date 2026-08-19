
CREATE PROCEDURE [rpt].[usp_DsSiteAddressInfo] 
(
	@SiteGuid uniqueidentifier
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsSiteAddressInfo] 
	-- Author: Eric Simmons
	-- Version/Date: 5/5/2015 4:34:00 PM 
	-- Purpose: Retrieve Address Informaiton for Site
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

	Select s.ID AS SiteID
	,s.Phone
	,s.Address1
	,s.Address2
	,CASE WHEN CONCAT(s.City, s.State, s.Zip) = '' THEN NULL 
		ELSE CONCAT(s.City, ', ', s.State, ' ', s.Zip) 
		END AS CityStateZip
	,s.Country
	FROM tblSites s
	WHERE SiteGuid=@SiteGuid

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
						+ 'Procedure Name: [rpt].usp_DsSiteAddressInfo' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END