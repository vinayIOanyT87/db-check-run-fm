


CREATE PROCEDURE [dbo].[usp_CreateDefaultSingleSite]

AS
BEGIN
	SET NOCOUNT ON

	------------------------------------------------------------------------------ 
	-- Check for bad initial conditions. 
	------------------------------------------------------------------------------ 

	-- Return if the default site already exists. 
	IF EXISTS (SELECT SiteGuid FROM dbo.tblSites WHERE [ID] = 'Default')
	BEGIN
		RAISERROR('ERROR - Default site already exists', 16, 1)
		RETURN
	END

	-- Return if if there is more than one site. 
	IF 1 < (SELECT COUNT(*) FROM dbo.tblSites)
	BEGIN
		RAISERROR('ERROR - Multiple sites already exist.', 16, 1)
		RETURN
	END

	-- Return if there is more than one user. 
	IF 1 < (SELECT COUNT(*) FROM dbo.tblUsers)
	BEGIN
		RAISERROR('ERROR - Multiple users already exist.', 16, 1)
		RETURN
	END

	-- Return if there is more than one user group. 
	IF 1 < (SELECT COUNT(*) FROM dbo.tblGroups)
	BEGIN
		RAISERROR('ERROR - Multiple user groups already exist.', 16, 1)
		RETURN
	END


	------------------------------------------------------------------------------ 
	-- Insert data - order is critical. 
	------------------------------------------------------------------------------ 
	DECLARE @nSiteGuid UNIQUEIDENTIFIER
	SET @nSiteGuid = NEWID()
	
	-- Insert default site. 
	INSERT INTO dbo.tblSites
	(
		SiteGuid,
		ID,							LevelUnitIndex,					TemperatureUnitIndex,
			DensityUnitIndex,			PressureUnitIndex,				FlowUnitIndex,
				VolumeUnitIndex,			MassUnitIndex,						AdditiveVolumeUnitIndex,
					LevelDecimalPlaces,		TemperatureDecimalPlaces,		DensityDecimalPlaces,
						PressureDecimalPlaces,	FlowDecimalPlaces,				VolumeDecimalPlaces,
							MassDecimalPlaces,		AdditiveVolumeDecimalPlaces,	SiteGroupFlag,
								ReportDirectory,			EnableAuditLogging,				AdministrativeLockDate,
									CreatedDate,				CreatedBy,							UpdatedDate,
										UpdatedBy
	)
	VALUES
	(	@nSiteGuid,
		'Default',					27,									2,
			191,							73,									109,
				46,							64,									40,
					2,								0,										0,
						2,								1,										0,
							0,								0,										0,
								'/Standard Reports',		1,										SYSDATETIMEOFFSET(),
									SYSDATETIMEOFFSET(),					'Varec',								SYSDATETIMEOFFSET(),
										'Varec'
	)

	-- Insert default group. 
	DECLARE @nGroupGuid UNIQUEIDENTIFIER
	SET @nGroupGuid = NEWID()
	
	INSERT INTO dbo.tblGroups
		(GroupGuid,GroupID, GroupDescription, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(@nGroupGuid,'Administrator', 'System Administrators', @nSiteGuid, SYSDATETIMEOFFSET(), 'Varec', SYSDATETIMEOFFSET(), 'Varec')



	-- Insert default group to rights map. 
	DECLARE @nRightIndex INT
	SET @nRightIndex = 0

	-- (gpeters) Changed to only insert the values that exist in the lookup.tblRight table since there is
	-- a foreign key constraint to it on the mapping table.  We can't create a mapping entry that doesn't
	-- have a corresponding right index.
	INSERT INTO [map].[tblGroupToRight]
		(GroupGuid, LookupRightIndex, CreatedDate, CreatedBy)
	SELECT @nGroupGuid, RightIndex 'LookupRightIndex', SYSDATETIMEOFFSET(), 'Varec'
		FROM [lookup].[tblRight]

	-- (Kendall) Insert many rights mappings to the administrator ID.  The RightsClass in 
	-- Shared Components will ignore values above the current mapping in the RIGHT enumeration.
	-- This gets cleaned up when Shared Components starts
	--WHILE @nRightIndex < 500
	--BEGIN
		--INSERT INTO [map].[tblGroupToRight]
			--(GroupGuid, LookupRightIndex, CreatedDate, CreatedBy)
		--VALUES
			--(@nGroupGuid, @nRightIndex, SYSDATETIMEOFFSET(), 'Varec')
			
		--SET @nRightIndex = @nRightIndex + 1
	--END

	-- Insert default company map "<All>" for Administrator Group. 
	DECLARE @USER_GROUP_COMPANY_MAP INT
	SET @USER_GROUP_COMPANY_MAP = 6

	INSERT INTO map.tblCompanyCompanyToUserGroup
		(SiteGuid, GroupGuid, CompanyGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(@nSiteGuid, @nGroupGuid, NULL, '', SYSDATETIMEOFFSET(), 'Varec', SYSDATETIMEOFFSET(), 'Varec')

	DECLARE @nCompanyMapIndex NUMERIC
	SET @nCompanyMapIndex = SCOPE_IDENTITY()

	-- Insert default user ("Administrator"). 
	DECLARE @nUserGuid UNIQUEIDENTIFIER
	SET @nUserGuid = NEWID()
	
	INSERT INTO dbo.tblUsers
	(	UserGuid,
		SiteGuid,
			UserID,
				Password,
					LastLoginDate,
						LastLogoffDate,
							ChangePassword,
								PasswordTimeStamp,
									Name,
										CreatedDate,
											CreatedBy,
												UpdatedDate,
													UpdatedBy
	)
	VALUES
	(	@nUserGuid
		,@nSiteGuid
			, 'Administrator'
				, 0x4D4849474353734741515142676A64594136426C4D474D474369734741515142676A6459417747675654425441674D4341414543416D595141674942414151510D0A414141414141414141414141414141414141414141415151753941484C6D4C4D774B5753726B3465414377416C6751676F5743784D49364A46774A69775531480D0A30523339747A38714250304354726F366F6D2F766E6A4C5244646F3D0D0A
					, SYSDATETIMEOFFSET()
						, SYSDATETIMEOFFSET()
							, 0
								, SYSDATETIMEOFFSET()
									, 'Administrator'
										, SYSDATETIMEOFFSET()
											, 'Varec'
												, SYSDATETIMEOFFSET()
													, 'Varec')
	


	-- Insert default user group map. 
	INSERT INTO [map].[tblUserToGroup]
		(UserGuid, GroupGuid, CreatedDate, CreatedBy, SiteGuid)
	VALUES
		(@nUserGuid, @nGroupGuid, SYSDATETIMEOFFSET(), 'Varec', @nSiteGuid)
		
	INSERT INTO map.tblEntityUserToSite
		(SiteGuid, UserGuid, CreatedDate, CreatedBy, AssignedFromSiteGuid)
	VALUES
		(@nSiteGuid, @nUserGuid, SYSDATETIMEOFFSET(), 'Varec', @nSiteGuid)
		
	INSERT INTO map.tblEntityUserGroupToSite
		(SiteGuid, GroupGuid, CreatedDate, CreatedBy, AssignedFromSiteGuid)
	VALUES
		(@nSiteGuid, @nGroupGuid, SYSDATETIMEOFFSET(), 'Varec', @nSiteGuid)

	RAISERROR('Completed sucessfully', 10, 1)
END



