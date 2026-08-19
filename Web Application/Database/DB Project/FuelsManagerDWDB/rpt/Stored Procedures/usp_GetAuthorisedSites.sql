/*
	DROP PROCEDURE [rpt].[usp_GetAuthorisedSites]

	EXEC [rpt].[usp_GetAuthorisedSites] '00000000-0000-0000-0000-000000000002'
	
*/
CREATE PROCEDURE [rpt].[usp_GetAuthorisedSites]
(
	@UserGuid uniqueidentifier
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [rpt].[usp_GetAuthorisedSites]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieve a detailed list of Sites authorised by a given user.
-- Notes:
-- 1. @UserKey: AKey of the User running the report
------------------------------------------------------------------------------------------------------
BEGIN TRY
	DECLARE @userKey nvarchar(50)
	SET @userKey = CONVERT(nvarchar(50), @UserGuid)
	
	-- If @UserKey = NULL then return all sites
	IF @UserKey IS NULL
	BEGIN
		SELECT SKey, AKey, SiteId FROM dbo.DimSite
	END
	ELSE
	BEGIN
		SELECT a.SKey, a.AKey, a.SiteId FROM dbo.DimSite a
		INNER JOIN dbo.FactFMUserToSite b
		ON b.SiteSKey = a.SKey
		INNER JOIN dbo.DimFMUser d
		ON d.SKey = b.FMUserSKey
		WHERE d.AKey = @UserKey
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
						+ 'Procedure Name: [rpt].[usp_GetAuthorisedSites]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END