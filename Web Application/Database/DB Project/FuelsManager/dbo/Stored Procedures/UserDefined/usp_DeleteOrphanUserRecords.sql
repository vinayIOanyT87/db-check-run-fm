/*
	DROP PROCEDURE [dbo].[usp_DeleteOrphanUserRecords]
*/
CREATE PROCEDURE [dbo].[usp_DeleteOrphanUserRecords]
(
	@IsActiveDirectoryUsers bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_DeleteOrphanUserRecords] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Delete all users for which the user records are not mapped properly. 
	-- Notes:
	-- 1. @IsActiveDirectoryUsers: 0:Limit the orphan user record deletion to non-AD Users only; 1:Limit the orphan user record deletion to Active Directory users only; NULL: Delete all orphan user records
	-- 2. For Active Directory users, an orphan User record is one that either no Site mappings or no UserGroup mappings
	-- 3. For regular FuelsManager users, an orphan User record is one that has no Site mappings
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY	
	
		DECLARE @tblOrphanUsers TABLE
		(
			UserGuid uniqueidentifier
		)

		INSERT INTO @tblOrphanUsers
		(UserGuid)
		SELECT a.UserGuid FROM dbo.tblUsers a
		WHERE a.ActiveDirectoryUser = 1
		AND
		(
			NOT EXISTS
			(
				SELECT * FROM map.tblEntityUserToSite b
				WHERE b.UserGuid = a.UserGuid
			)
			OR NOT EXISTS
			(
				SELECT * FROM map.tblUserToGroup c
				WHERE c.UserGuid = a.UserGuid
			)
		)
		AND ((@IsActiveDirectoryUsers IS NULL) OR (@IsActiveDirectoryUsers = 1))


		INSERT INTO @tblOrphanUsers
		(UserGuid)
		SELECT a.UserGuid FROM dbo.tblUsers a
		wHERE ISNULL(a.ActiveDirectoryUser, 0) = 0
		AND NOT EXISTS
		(
			SELECT * FROM map.tblEntityUserToSite b
			WHERE b.UserGuid = a.UserGuid
		)
		AND ((@IsActiveDirectoryUsers IS NULL) OR (@IsActiveDirectoryUsers = 0))


		DELETE a
		FROM dbo.tblAccessibilityConfigurationSettings a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM dbo.tblDispatchGridColumn a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		--DELETE a
		--FROM dbo.tblErrorTransactionSubmissions a
		--INNER JOIN @tblOrphanUsers b
		--ON b.UserGuid = a.SubmittedUserGuid

		DELETE a
		FROM dbo.tblMenuFavorites a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM dbo.tblPersonnel a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM dbo.tblQueryStorage a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.OwnerUserGuid

		DELETE a
		FROM dbo.tblSavedQueries a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM dbo.tblSessions a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM dbo.tblUserViewStateSettings a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM map.tblUserToGroup a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

		DELETE a
		FROM map.tblEntityUserToSite a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid
		INNER JOIN dbo.tblUsers c
		ON c.UserGuid = a.UserGuid
		WHERE c.ActiveDirectoryUser = 1

		DELETE a
		FROM dbo.tblUsers a
		INNER JOIN @tblOrphanUsers b
		ON b.UserGuid = a.UserGuid

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
						+ 'Procedure Name: [dbo].usp_DeleteOrphanUserRecords' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END

GO