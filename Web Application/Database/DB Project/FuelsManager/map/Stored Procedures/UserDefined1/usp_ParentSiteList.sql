CREATE PROCEDURE [map].[usp_ParentSiteList]
	@ChildSiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_ParentSiteList] 
	-- Author: Richard Panachida
	-- Version/Date: 1.0.000 / 2014-01-28
	-- Purpose: Retrieve a list of parent site GUID for a given child site GUID.
	-- Notes:
	-- 1. @ChildSiteGuid: The child site GUID that you want the list of parents.
	------------------------------------------------------------------------------------------------------

	BEGIN TRY
		DECLARE @SearchSiteGuid UNIQUEIDENTIFIER
		DECLARE @SiteGuid UNIQUEIDENTIFIER
		DECLARE @ID NVARCHAR (100)

		CREATE TABLE #tblParentList
		(
			SiteGuid uniqueidentifier,
			ID NVARCHAR (100)
		)

		SET @SearchSiteGuid = @ChildSiteGuid

		WHILE @SearchSiteGuid IS NOT NULL
		BEGIN
			SELECT @SiteGuid = SiteGuid, @ID = ID
						FROM dbo.tblSites 
						WHERE SiteGuid IN (SELECT ParentSiteGuid FROM [map].[tblSiteToSite] WHERE ChildSiteGuid = @SearchSiteGuid) AND
							SiteGuid <> @SearchSiteGuid

			SET @SearchSiteGuid = @SiteGuid

			IF (@SearchSiteGuid IS NOT NULL)
			BEGIN
				INSERT INTO #tblParentList (SiteGuid, ID)
				VALUES (@SiteGuid, @ID)
			END

			SET @SiteGuid = NULL
			SET @ID = NULL
		END

		SELECT SiteGuid, ID FROM #tblParentList
		DROP TABLE #tblParentList
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
						+ 'Procedure Name: [map].usp_ParentSiteList' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH   
END
GO
