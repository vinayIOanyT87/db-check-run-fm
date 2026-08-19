
/*
	DROP PROCEDURE [dbo].[usp_GetUserMappingChangePlan]
*/
CREATE PROCEDURE [dbo].[usp_GetUserMappingChangePlan]
(
	@UserADMappingTable dbo.utt_UserADMapping READONLY, @DeleteMappingsOfNonListedADUsers bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_GetUserMappingChangePlan] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Determine the mapping changes that need to be applied to the FuelsManager User-to-Site mappings to satisfy a given Active Directory User-to-Site mapping set.
	-- Notes:
	-- 1. @UserADMappingTable: Table containing a list of Active Directory User-to-Site mappings
	-- 2. @DeleteMappingsOfNonListedADUsers: if True (1), mark for deletion the existing user-to-site mappings of AD users that are not listed at all in the @UserADMappingTable
	-- 3. The Stored Procedure assumes that for any user listed in the @UserADMappingTable, the Active Directory mappings for that user in @UserADMappingTable is comprehensive, i.e. it contains all the mappings for that user, even those that have already been processed.
	-- 4. For each user, the supplied Active Directory mappings are compared to the current FuelsManager User-to-Site mappings of the user to determine the FuelsManager mapping change actions required to satisfy the Active Directory mappings.
	-- 5. The Mapping Change Action for each user mapping, is captured in the MappingChangeAction column of the resultset.
	-- 6. The possible values of the MappingChangeAction column are: (0: No Action; 1: Add; 2: Delete; 3: Delete Mapping of Missing User).	
	-- 7. MappingChangeAction 2 is for individual mapping deletion for a user whose Active Directory mappings was supplied in @UserADMappingTable.
	-- 8. MappingChangeAction 3 is for complete mapping deletions for Active Directory users that are not listed at all in @UserADMappingTable.
	-- 9. User mappings that cannot be supported are captured in the result with an error message in the ErrorMessage column and a MappingChangeAction of 0.
	-- 10. Partial mappings are not allowed. If one mapping of a user is not supported, then all of the mapping changes for that user are ignored.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @tblUserADMappings dbo.utt_UserADMapping

		DECLARE @tblUserMappingChangePlan TABLE
		(
			runningIndex int identity			
			, UserId nvarchar(100)
			, UserGuid uniqueidentifier			
			, MappingChangeAction int
			, AssignedFromSiteGuid uniqueidentifier
			, AssignedToSiteGuid uniqueidentifier
			, AssignedToHierarchyLevel int
			, ErrorMessage nvarchar(250)		
		);
		

		DECLARE @tblTargetUserSiteMapping TABLE
		(			
			AssignedToSiteGuid uniqueidentifier, 
			HierarchyLevel int
		);

		DECLARE @tblFullSiteHierarchy table
		(
			SiteGuid uniqueidentifier,
			SiteId nvarchar(30),
			HierarchyLevel integer
		)

		DECLARE @tblTargetSiteHierarchy table
		(
			SiteGuid uniqueidentifier,
			SiteId nvarchar(30),
			HierarchyLevel integer
		)

		INSERT INTO @tblUserADMappings
		(UserId, SiteGuid)
		SELECT DISTINCT UserId, SiteGuid FROM @UserADMappingTable
				
		INSERT INTO @tblFullSiteHierarchy
		EXEC [erv].[usp_GetFLCSiteHierarchy] NULL, 0

		--Include any stand-alone site that is not linked under the root SiteGroup
		INSERT INTO @tblFullSiteHierarchy
		(
			SiteGuid,
			SiteId,
			HierarchyLevel
		)
		SELECT a.SiteGuid, a.Id, 0 FROM tblSites a
		INNER JOIN map.tblSiteToSite b
		ON b.ParentSiteGuid = a.SiteGuid
		WHERE b.ParentSiteGuid = b.ChildSiteGuid
		AND a.SiteGroupFlag <> 1
		AND NOT EXISTS
		(
			SELECT * FROM @tblFullSiteHierarchy c
			WHERE c.SiteGuid = a.SiteGuid
		)
		
		DECLARE @targetUserId nvarchar(100)
		DECLARE @targetUserGuid uniqueidentifier
		DECLARE @invalidMapping bit
		DECLARE @errorMsg nvarchar(250)
		DECLARE @topHierarchyLevel int
		DECLARE @currentHierarchyLevel int
		DECLARE @lastHierarchyLevel int

		DECLARE TargetUsersCursor CURSOR FOR 
			SELECT UserId
			FROM @tblUserADMappings
			GROUP BY UserId
			ORDER BY UserId
		OPEN TargetUsersCursor
			--Parse the AD mappings
			FETCH NEXT FROM TargetUsersCursor INTO @targetUserId
			WHILE @@FETCH_STATUS = 0
			BEGIN				
				SET @invalidMapping = 0
				SET @errorMsg = NULL
				SET @targetUserGuid = NULL
				SET @topHierarchyLevel = NULL
				SET @lastHierarchyLevel = NULL
				DELETE @tblTargetSiteHierarchy
				DELETE @tblTargetUserSiteMapping		

				SELECT @targetUserGuid = UserGuid FROM dbo.tblUsers WHERE UserId = @targetUserId

				INSERT INTO @tblTargetUserSiteMapping
				(AssignedToSiteGuid)
				SELECT SiteGuid FROM @tblUserADMappings
				WHERE UserId = @targetUserId

				UPDATE a
				SET a.HierarchyLevel = b.HierarchyLevel
				FROM @tblTargetUserSiteMapping a
				INNER JOIN @tblFullSiteHierarchy b
				ON b.SiteGuid = a.AssignedToSiteGuid

				SELECT @topHierarchyLevel = MIN(HierarchyLevel) FROM @tblTargetUserSiteMapping
				SELECT @lastHierarchyLevel = MAX(HierarchyLevel) FROM @tblTargetUserSiteMapping

				IF((SELECT COUNT(*) FROM @tblTargetUserSiteMapping WHERE HierarchyLevel = @topHierarchyLevel) > 1)
				BEGIN
					SET @invalidMapping = 1
					SET @errorMsg = 'Missing common parent Site mapping'
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, 0, NULL, AssignedToSiteGuid, HierarchyLevel, @errorMsg 
					FROM @tblTargetUserSiteMapping
					WHERE HierarchyLevel = @topHierarchyLevel
				END 

				IF (@invalidMapping = 0)
				BEGIN
					DECLARE @topSiteGuid uniqueidentifier
					SELECT @topSiteGuid = AssignedToSiteGuid FROM @tblTargetUserSiteMapping WHERE HierarchyLevel = @topHierarchyLevel

					INSERT INTO @tblTargetSiteHierarchy
					EXEC [erv].[usp_GetFLCSiteHierarchy] @topSiteGuid, 0

					IF(
						(
							SELECT COUNT(*) FROM @tblTargetUserSiteMapping a
							WHERE NOT EXISTS 
							(
								SELECT * FROM @tblTargetSiteHierarchy b
								WHERE a.AssignedToSiteGuid = b.SiteGuid
							)
						) 
						> 1
					)
					BEGIN
						SET @invalidMapping = 1
						SET @errorMsg = 'Mapping is outside of the targetted site hierarchy'
						INSERT INTO @tblUserMappingChangePlan
						(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
						SELECT @targetUserId, @targetUserGuid, 0, NULL, AssignedToSiteGuid, HierarchyLevel, @errorMsg 
						FROM @tblTargetUserSiteMapping a
						WHERE NOT EXISTS 
						(
							SELECT * FROM @tblTargetSiteHierarchy b
							WHERE a.AssignedToSiteGuid = b.SiteGuid
						)
					END 

				END


				IF (@invalidMapping = 1)
				BEGIN
					--Ingore all mappings for the user, even valid ones, because of the invalid mapping/s
					SET @errorMsg = 'Mapping skipped because of other invalid mappings for the user'
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, 0, NULL, AssignedToSiteGuid, HierarchyLevel, @errorMsg 
					FROM @tblTargetUserSiteMapping a	
					WHERE NOT EXISTS
					(
						SELECT * FROM @tblUserMappingChangePlan b
						WHERE b.AssignedToSiteGuid = a.AssignedToSiteGuid
					)
					FETCH NEXT FROM TargetUsersCursor INTO @targetUserId
					CONTINUE
				END

				--Process the top level user-to-site mapping for the user
				INSERT INTO @tblUserMappingChangePlan
				(UserId, UserGuid, MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
				SELECT @targetUserId, @targetUserGuid, (CASE WHEN b.UserToSiteGuid IS NULL THEN 1 ELSE 0 END), @topSiteGuid, @topSiteGuid, a.HierarchyLevel, NULL FROM @tblTargetUserSiteMapping a
				LEFT OUTER JOIN map.tblEntityUserToSite b
				ON b.UserGuid = @targetUserGuid
				AND b.AssignedFromSiteGuid = @topSiteGuid
				AND b.SiteGuid = @topSiteGuid
				WHERE AssignedToSiteGuid = @topSiteGuid

				--Process the lower levels user-to-site mapping for the user
				SET @currentHierarchyLevel = @topHierarchyLevel
				WHILE (@currentHierarchyLevel < @lastHierarchyLevel)
				BEGIN
					SET @currentHierarchyLevel = @currentHierarchyLevel + 1

					--Capture all user-to-site mappings that needs to be added or left untouched for each hierarchy level
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid, MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, (CASE WHEN d.UserToSiteGuid IS NULL THEN 1 ELSE 0 END), b.ParentSiteGuid, a.AssignedToSiteGuid, a.HierarchyLevel, NULL 
					FROM @tblTargetUserSiteMapping a
					INNER JOIN 
					(
						SELECT MAX(x.ParentSiteGuid) ParentSiteGuid, x.ChildSiteGuid FROM map.tblSiteToSite x						
						INNER JOIN @tblUserMappingChangePlan y
						ON y.AssignedToSiteGuid = x.ParentSiteGuid
						WHERE y.UserId = @targetUserId
						GROUP BY x.ChildSiteGuid
					) b
					ON b.ChildSiteGuid = a.AssignedToSiteGuid
					LEFT OUTER JOIN map.tblEntityUserToSite d
					ON d.UserGuid = @targetUserGuid
					AND d.AssignedFromSiteGuid = b.ParentSiteGuid
					AND d.SiteGuid = a.AssignedToSiteGuid
					WHERE a.HierarchyLevel = @currentHierarchyLevel
				END

				
				IF (
						(
							SELECT COUNT(*) FROM @tblTargetUserSiteMapping a
							WHERE NOT EXISTS 
							(
								SELECT * FROM @tblUserMappingChangePlan b
								WHERE b.UserId = @targetUserId
								AND b.AssignedToSiteGuid = a.AssignedToSiteGuid
							)							
						) > 0
					)  
				BEGIN
					-- User has AD user-to-site mappings that could not be resolved/processed.
					SET @errorMsg = 'Missing intermediate Site mapping'
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, 0, NULL, AssignedToSiteGuid, HierarchyLevel, @errorMsg 
					FROM @tblTargetUserSiteMapping a
					WHERE NOT EXISTS 
					(
						SELECT * FROM @tblUserMappingChangePlan b
						WHERE b.UserId = @targetUserId
						AND b.AssignedToSiteGuid = a.AssignedToSiteGuid
					)	

					SET @errorMsg = 'Mapping skipped because of other invalid mappings for the user'
					UPDATE @tblUserMappingChangePlan					
					SET MappingChangeAction = 0, ErrorMessage = @errorMsg
					WHERE UserId = @targetUserId
					AND MappingChangeAction <> 0	
				END
				ELSE
				BEGIN
					--Mark for deletion the user existing user-to-site mappings that do not appear in the AD user-to-site mappings
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, 2, a.AssignedFromSiteGuid, a.SiteGuid, NULL, NULL FROM map.tblEntityUserToSite a
					WHERE a.UserGuid = @targetUserGuid
					AND NOT EXISTS
					(
						SELECT * FROM @tblTargetUserSiteMapping b
						WHERE b.AssignedToSiteGuid = a.SiteGuid
					)

					--Mark for deletion the user existing self-site user-to-site mapping if the new AD mappings require the owner site of the user to change
					INSERT INTO @tblUserMappingChangePlan
					(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
					SELECT @targetUserId, @targetUserGuid, 2, a.AssignedFromSiteGuid, a.SiteGuid, NULL, NULL FROM map.tblEntityUserToSite a
					WHERE a.UserGuid = @targetUserGuid
					AND a.AssignedFromSiteGuid = a.SiteGuid
					AND a.AssignedFromSiteGuid <> @topSiteGuid
					AND NOT EXISTS
					(
						SELECT * FROM @tblUserMappingChangePlan b
						WHERE b.UserGuid = a.UserGuid
						AND b.AssignedFromSiteGuid = a.AssignedFromSiteGuid
						AND b.AssignedToSiteGuid = a.SiteGuid
						AND b.AssignedFromSiteGuid = b.AssignedToSiteGuid
						AND b.MappingChangeAction = 2
					)
				END

				FETCH NEXT FROM TargetUsersCursor INTO @targetUserId
			END 
		CLOSE TargetUsersCursor
		DEALLOCATE TargetUsersCursor
		
		--Mark for deletion all the user-to-site mappings of all Active Directory users who are absent completely from the AD user-to-site mappings
		IF (@DeleteMappingsOfNonListedADUsers = 1)
		BEGIN
			INSERT INTO @tblUserMappingChangePlan
			(UserId, UserGuid,  MappingChangeAction, AssignedFromSiteGuid, AssignedToSiteGuid, AssignedToHierarchyLevel, ErrorMessage)
			SELECT b.UserID, a.UserGuid, 3, NULL, NULL, NULL, NULL
			FROM map.tblEntityUserToSite a
			INNER JOIN dbo.tblUsers b
			ON b.UserGuid = a.UserGuid
			WHERE b.ActiveDirectoryUser = 1 
			AND NOT EXISTS
			(
				SELECT * FROM
				(
					SELECT d.UserGuid FROM @tblUserADMappings c
					INNER JOIN dbo.tblUsers d
					ON d.UserID = c.UserId
					GROUP BY d.UserGuid
				) e
				WHERE e.UserGuid = a.UserGuid
			)
			GROUP BY b.UserId, a.UserGuid
		END

		SELECT * FROM @tblUserMappingChangePlan
		ORDER BY UserId, AssignedToHierarchyLevel

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
						+ 'Procedure Name: [dbo].usp_GetUserMappingChangePlan' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END

GO


