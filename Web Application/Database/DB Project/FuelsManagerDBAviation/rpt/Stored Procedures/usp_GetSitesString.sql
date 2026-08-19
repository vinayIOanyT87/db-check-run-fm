
CREATE PROCEDURE [rpt].[usp_GetSitesString] 
(
	@Sites nvarchar(max)
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_GetSitesString] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the Company ID's and concatenate to make a comma delimited list
	-- Notes:
	-- 1. @Sites: List of site Guids 
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @tblSiteName TABLE (
			[name] nvarchar(1000)
		);
		INSERT INTO @tblSiteName SELECT a.ID name FROM tblSites a INNER JOIN rpt.udf_GetTableFromStringList(@Sites) b ON a.SiteGuid = b.Guid
		SELECT STUFF((SELECT ','+name  FROM @tblSiteName FOR XML PATH('')),1,1,'') AS name;
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
						+ 'Procedure Name: [rpt].usp_GetSitesString' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END