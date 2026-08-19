
/*
	DROP PROCEDURE [dbo].[usp_GetUserGroupMappingChangePlan]
*/
CREATE PROCEDURE [dbo].[usp_GetUserGroupMappingChangePlan]
(
	@UserGroupADMappingTable dbo.utt_UserGroupADMapping READONLY, @DeleteMappingsOfNonListedADUsers bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[usp_GetUserGroupMappingChangePlan] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Determine the mapping changes that need to be applied to the FuelsManager User-to-UserGroup mappings to satisfy a given Active Directory User-to-UserGroup mapping set.
	-- Notes:
	-- 1. @UserGroupADMappingTable: Table containing a list of Active Directory User-to-UserGroup mappings
	-- 2. @DeleteMappingsOfNonListedADUsers: if True (1), mark for deletion the existing user-to-site mappings of AD users that are not listed at all in the @UserADMappingTable
	-- 3. The Stored Procedure assumes that for any user listed in the @UserGroupADMappingTable, the Active Directory mappings for that user in @UserGroupADMappingTable is comprehensive, i.e. it contains all the mappings for that user, even those that have already been processed.
	-- 4. The Stored Procedure assumes that the User for each mapping already exists in FuelsManager, if not an ErrorMessage is returned for the specific mapping.
	-- 5. For each user, the supplied Active Directory mappings are compared to the current FuelsManager User-to-UserGroup mappings of the user to determine the FuelsManager mapping change actions required to satisfy the Active Directory mappings.
	-- 6. The Mapping Change Action for each user mapping, is captured in the MappingChangeAction column of the resultset.
	-- 7. The possible values of the MappingChangeAction column are: (0: No Action; 1: Add; 2: Delete; 3: Delete Mapping of Missing User).	
	-- 8. MappingChangeAction 2 is for individual mapping deletion for a user whose Active Directory mappings was supplied in @UserGroupADMappingTable.
	-- 9. MappingChangeAction 3 is for complete mapping deletions for Active Directory users that are not listed at all in @UserGroupADMappingTable.
	-- 10. User mappings that cannot be supported are captured in the result with an error message in the ErrorMessage column and a MappingChangeAction of 0.
	-- 11. Unlike for entity-to-site assignments, this operation assumes that the User-to-UserGroup assignment is not a cascading assignment, i.e. 
	--    for a User to be linked to a UserGroup at a site/sitegroup, it is not necessary for the same User-to-UserGroup mapping to exist
	--    at the parent sitegroup. All that is necessary is for both the User and the UserGroup to be assigned to the target site/group
	--    of the User-to-UserGroup assigment.
	-- 12. Since the User-to-UserGroup assigment is not a cascading assignment, partial mappings are allowed. If one mapping of a user is not supported, then only that mapping is flagged as an invalid mapping, while all the other mappings of the user are processed as valid mappings.


	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY		

		DECLARE @tblUserGroupMappingChangePlan TABLE
		(
			runningIndex int identity			
			, UserId nvarchar(100)
			, UserGuid uniqueidentifier			
			, MappingChangeAction int
			, SiteGuid uniqueidentifier
			, UserGroupGuid uniqueidentifier
			, ErrorMessage nvarchar(250)		
		);
		

		DECLARE @tblTargetUserGroupMapping TABLE
		(			
			UserId nvarchar(100) ,
			UserGuid uniqueidentifier,			
			UserGroupGuid uniqueidentifier
		);


		DECLARE @tblTargetUserSiteMapping TABLE
		(			
			UserGuid uniqueidentifier,
			SiteGuid uniqueidentifier		
		);

		DECLARE @tblTargetUserGroupSiteMapping TABLE
		(			
			UserGroupGuid uniqueidentifier,
			SiteGuid uniqueidentifier		
		);

		INSERT INTO @tblTargetUserGroupMapping
		(UserId, UserGuid, UserGroupGuid)
		SELECT DISTINCT a.UserId, b.UserGuid, a.UserGroupGuid
		FROM @UserGroupADMappingTable a
		INNER JOIN dbo.tblUsers b
		ON b.UserId = a.UserId

		DECLARE @errorMsg nvarchar(250)

		-- Ignore unmatched users
		SET @errorMsg = 'User record cannot be located'
		INSERT INTO @tblUserGroupMappingChangePlan
		(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
		SELECT DISTINCT UserId, NULL, 0, NULL, UserGroupGuid, @errorMsg 
		FROM @UserGroupADMappingTable a	
		WHERE NOT EXISTS
		(
			SELECT * FROM @tblTargetUserGroupMapping b
			WHERE b.UserId = a.UserId
		)
		
		INSERT INTO @tblTargetUserSiteMapping
		(UserGuid, SiteGuid)
		SELECT a.UserGuid, b.SiteGuid
		FROM (SELECT DISTINCT UserGuid FROM @tblTargetUserGroupMapping) a
		INNER JOIN map.tblEntityUserToSite b
		ON b.UserGuid = a.UserGuid

		INSERT INTO @tblTargetUserGroupSiteMapping
		(UserGroupGuid, SiteGuid)
		SELECT a.UserGroupGuid, b.SiteGuid
		FROM (SELECT DISTINCT UserGroupGuid FROM @tblTargetUserGroupMapping) a
		INNER JOIN map.tblEntityUserGroupToSite b
		ON b.GroupGuid = a.UserGroupGuid


		-- Ignore already mapped entries
		INSERT INTO @tblUserGroupMappingChangePlan
		(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
		SELECT a.UserId, a.UserGuid, 0, b.SiteGuid, a.UserGroupGuid, NULL
		FROM @tblTargetUserGroupMapping a	
		INNER JOIN @tblTargetUserGroupSiteMapping b
		ON b.UserGroupGuid = a.UserGroupGuid
		INNER JOIN @tblTargetUserSiteMapping c
		ON c.UserGuid = a.UserGuid
		AND c.SiteGuid = b.SiteGuid
		INNER JOIN map.tblUserToGroup d
		ON d.GroupGuid = a.UserGroupGuid
		AND d.UserGuid = a.UserGuid
		AND d.SiteGuid =  b.SiteGuid


		-- Add new mappings
		INSERT INTO @tblUserGroupMappingChangePlan
		(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
		SELECT a.UserId, a.UserGuid, 1, b.SiteGuid, a.UserGroupGuid, NULL
		FROM @tblTargetUserGroupMapping a	
		INNER JOIN @tblTargetUserGroupSiteMapping b
		ON b.UserGroupGuid = a.UserGroupGuid
		INNER JOIN @tblTargetUserSiteMapping c
		ON c.UserGuid = a.UserGuid
		AND c.SiteGuid = b.SiteGuid
		WHERE NOT EXISTS
		(
			SELECT * FROM map.tblUserToGroup d
			WHERE d.GroupGuid = a.UserGroupGuid
			AND d.UserGuid = a.UserGuid
			AND d.SiteGuid =  b.SiteGuid
		)


		-- Mark as invalid AD user-to-UserGroup mappings for which the user is not mapped to any of the sites to which the usergroup is mapped
		SET @errorMsg = 'User is not mapped to any of the sites to which the UserGroup is mapped'
		INSERT INTO @tblUserGroupMappingChangePlan
		(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
		SELECT a.UserId, a.UserGuid, 0, NULL, a.UserGroupGuid, @errorMsg
		FROM @tblTargetUserGroupMapping a
		INNER JOIN
		(
			SELECT x.UserGuid, x.UserGroupGuid, SUM(x.ValidMapping) ValidMappingCount	
			FROM	
			(
				SELECT b.UserGuid, c.UserGroupGuid, 
				(CASE WHEN (c.SiteGuid IS NULL OR f.SiteGuid IS NULL) THEN 0 ELSE 1 END) ValidMapping 
				FROM @tblTargetUserGroupMapping b	
				INNER JOIN @tblTargetUserGroupSiteMapping c
				ON c.UserGroupGuid = b.UserGroupGuid
				FULL OUTER JOIN
				(
					SELECT d.UserGuid, e.SiteGuid
					FROM @tblTargetUserGroupMapping d
					INNER JOIN @tblTargetUserSiteMapping e
					ON e.UserGuid = d.UserGuid
				) f
				ON f.UserGuid = b.UserGuid
				AND f.SiteGuid = c.SiteGuid
			) x
			GROUP BY x.UserGuid, x.UserGroupGuid
		) y
		ON y.UserGuid = a.UserGuid
		AND y.UserGroupGuid = a.UserGroupGuid
		WHERE y.ValidMappingCount = 0


		-- Mark for deletion FuelsManager User-to-UserGroup mappings not supported anymore in AD for users who still do have AD User-to-UserGroup mappings to one or more UserGroups
		INSERT INTO @tblUserGroupMappingChangePlan
		(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
		SELECT b.UserId, a.UserGuid, 2, a.SiteGuid, a.GroupGuid, NULL
		FROM map.tblUserToGroup a
		INNER JOIN dbo.tblUsers b
		ON b.UserGuid = a.UserGuid
		INNER JOIN 
		(
			SELECT DISTINCT UserGuid FROM  @tblTargetUserGroupMapping 
		) c
		ON c.UserGuid = a.UserGuid
		INNER JOIN
		(
			SELECT w.UserGuid, w.GroupGuid
			FROM 
			(
				SELECT UserGuid, GroupGuid
				FROM map.tblUserToGroup 
				GROUP BY UserGuid, GroupGuid
			) w
			WHERE NOT EXISTS
			(
				SELECT * FROM @tblTargetUserGroupMapping x
				WHERE x.UserGuid = w.UserGuid
				AND x.UserGroupGuid = w.GroupGuid 
			)
		) y
		ON y.UserGuid = a.UserGuid
		AND y.GroupGuid =  a.GroupGuid


		-- Mark for deletion all FuelsManager User-to-UserGroup mappings for users who do not have any AD User-to-UserGroup mappings.
		IF (@DeleteMappingsOfNonListedADUsers = 1)
		BEGIN
			INSERT INTO @tblUserGroupMappingChangePlan
			(UserId, UserGuid, MappingChangeAction, SiteGuid, UserGroupGuid, ErrorMessage)
			SELECT b.UserId, a.UserGuid, 3, NULL, a.GroupGuid, NULL
			FROM map.tblUserToGroup a
			INNER JOIN dbo.tblUsers b
			ON b.UserGuid = a.UserGuid
			WHERE b.ActiveDirectoryUser = 1 
			AND NOT EXISTS
			(
				SELECT * FROM
				(
					SELECT DISTINCT c.UserGuid FROM  @tblTargetUserGroupMapping c
				) d
				WHERE d.UserGuid = a.UserGuid
			)
			GROUP BY b.UserId, a.UserGuid, a.GroupGuid
		END

		SELECT * FROM @tblUserGroupMappingChangePlan
		ORDER BY UserId, UserGroupGuid
		
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
						+ 'Procedure Name: [dbo].usp_GetUserGroupMappingChangePlan' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END

GO


