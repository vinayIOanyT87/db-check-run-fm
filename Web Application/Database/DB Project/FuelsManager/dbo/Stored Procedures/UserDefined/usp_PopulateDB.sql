/****** Object:  Stored Procedure dbo.usp_PopulateDB    Script Date: 2/27/2002 4:51:51 PM ******/

-- Insert row into tblConfigurationSetting if there is not already an identical SettingKey
CREATE PROCEDURE [dbo].[usp_InsertConfigSetting]
(
  @KeyType [nvarchar](8)
, @SettingKey [nvarchar](50)
, @SettingValue [nvarchar](1000)
)
AS
BEGIN

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

if not exists (Select 1 from dbo.tblConfigurationSetting where SettingKey = @SettingKey)
	BEGIN
		INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
		VALUES(NEWID(),@KeyType,@SettingKey,@SettingValue,'1900-01-01','Administrator','1900-01-01','Administrator')
	END
else 
	if not exists (Select 1 from dbo.tblConfigurationSetting where SettingValue = @SettingValue)
	BEGIN
		DELETE dbo.tblConfigurationSetting WHERE SettingKey = @SettingKey
		INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
		VALUES(NEWID(),@KeyType,@SettingKey,@SettingValue,'1900-01-01','Administrator','1900-01-01','Administrator')
	END
END
GO

-- Insert row into tblDataDictionaries if there is not already an identical key for the site
CREATE PROCEDURE [dbo].[usp_InsertDataDictionaryRow]
(
  @SiteId [nvarchar](30)
 ,@Key [nvarchar](100)
, @Value [nvarchar](1000)
)
AS
BEGIN

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
DECLARE @SiteGuid uniqueidentifier

select top 1 @SiteGuid=SiteGuid  from tblSites where id=@SiteId

if not exists (Select 1 from dbo.tblDataDictionaries where [Key] = @Key and [SiteGuid]=@SiteGuid)
	BEGIN
		INSERT INTO [dbo].[tblDataDictionaries]
				   ([Key]
				   ,[Value]
				   ,[CreatedDate]
				   ,[CreatedBy]
				   ,[UpdatedDate]
				   ,[UpdatedBy]
				   ,[DataDictionaryGuid]
				   ,[SiteGuid])
			 VALUES(
					@Key
				   ,@Value 
				   ,'1900-01-01'
				   ,'Administrator'
				   ,'1900-01-01'
				   ,'Administrator'
				   ,NEWID()
				   ,@SiteGuid )
	END
END
GO


CREATE PROCEDURE [dbo].[usp_PopulateDB] AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @Now DATETIMEOFFSET(7)
	SET @Now=SYSDATETIMEOFFSET()
	
	/* INSERT DEFAULT SITE */
	if not exists (Select SiteGuid from tblSites where SiteGuid = '00000000-0000-0000-0000-000000000001')
		BEGIN
	INSERT INTO tblSites
		(ID, LevelUnitIndex, TemperatureUnitIndex, DensityUnitIndex, PressureUnitIndex, FlowUnitIndex, VolumeUnitIndex, MassUnitIndex, AdditiveVolumeUnitIndex, 
		LevelDecimalPlaces, TemperatureDecimalPlaces, DensityDecimalPlaces, PressureDecimalPlaces, FlowDecimalPlaces, VolumeDecimalPlaces, MassDecimalPlaces, AdditiveVolumeDecimalPlaces,
		SiteGroupFlag, ReportDirectory, EnableAuditLogging, AdministrativeLockDate, OperationalLockDate, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy,SiteGuid,SecurityMode,SecurityPolicy,MessageEncoding,UserIdentityMethod,MaximumDaysToRetainArchive, EnforceSalesOrderLimit)
	VALUES ('SiteAdmin', 27, 2, 191, 73, 109, 46, 64, 40, 2, 0, 0, 2, 1, 0, 0, 0, 1, '/Standard Reports', 1, @Now, @Now, @Now, 'Varec', @Now, 'Varec', '00000000-0000-0000-0000-000000000001', 'None', 'None', 'Binary', 'Anonymous', 365, 0);
		END

	/* INSERT SITE TO SITE MAP FOR DEFAULT SITE */
	if not exists (Select ParentSiteGuid from map.tblSiteToSite where ChildSiteGuid = '00000000-0000-0000-0000-000000000001')
		BEGIN
	INSERT INTO map.tblSiteToSite(ParentSiteGuid,ChildSiteGuid)
	SELECT NULL,'00000000-0000-0000-0000-000000000001'
		END

	/* INSERT RIGHTS FOR EXSTARS */
	if not exists (Select RightIndex from lookup.tblRight where RightIndex = 178)
		BEGIN
    INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
    VALUES (178, 'CREATE_IRS_EXSTARS_REPORT', 'CREATE_IRS_EXSTARS_REPORT')
		END
	if not exists (Select RightIndex from lookup.tblRight where RightIndex = 179)
		BEGIN
    INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
    VALUES (179, 'VIEW_IRS_EXSTARS_REPORT', 'VIEW_IRS_EXSTARS_REPORT')
		END




	/*INSERT DEFAULT GROUPS */
	if not exists (Select GroupGuid from tblGroups where GroupGuid = '00000000-0000-0000-0000-000000000003')
		BEGIN
	INSERT INTO tblGroups
		 (GroupID, GroupDescription, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy,GroupGuid)
	VALUES ('Administrator', 'System Administrators', '00000000-0000-0000-0000-000000000001', @Now, 'Varec', @Now, 'Varec','00000000-0000-0000-0000-000000000003');
		END


	/* Insert default company map "<All>" for Administrator Group */
	if not exists (Select GroupGuid from map.tblCompanyCompanyToUserGroup where GroupGuid = '00000000-0000-0000-0000-000000000003')
		BEGIN
	INSERT INTO map.tblCompanyCompanyToUserGroup
		(SiteGuid,GroupGuid,CompanyGuid,[ID],CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000003',NULL, '',@Now, 'Varec', @Now, 'Varec')
		END

	------------------------------

	/* INSERT DEFAULT USERS */
	if not exists (Select UserGuid from tblUsers where UserGuid = '00000000-0000-0000-0000-000000000002')
		BEGIN
	INSERT INTO tblUsers
		(SiteGuid, UserGuid,UserID, [Password], LastLoginDate, LastLogoffDate, ChangePassword, PasswordTimeStamp, Name, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES ('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002','Administrator', CAST('f974761263a735e3670ea8cba52631f95daf8f9e' AS varbinary(246)), @Now, @Now, 0, @Now, 'Administrator', @Now, 'Varec', @Now, 'Varec')
	END

	/* INSERT DEFAULT GROUP to RIGHTS MAP */

	--View Users
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 0)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 0,  @Now, 'Varec','Varec',@Now);
		END

	--View User Groups
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 1)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 1,  @Now, 'Varec','Varec',@Now);
		END
	
	--Modfy Users
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 2)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 2,  @Now, 'Varec','Varec',@Now);
		END

	--Modify User Groups
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 3)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 3,  @Now, 'Varec','Varec',@Now);
		END

	--Import Configuration Data
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 4)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex,  CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 4,  @Now, 'Varec','Varec',@Now);
		END

	--Export Configuration Data
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 5)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 5,  @Now, 'Varec','Varec',@Now);
		END

	--Perform Product Update (OBSOLETE)
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 6)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex,  CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 6,  @Now, 'Varec','Varec',@Now);
		END

	--View Installed Module Status
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 7)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 7,  @Now, 'Varec','Varec',@Now);
		END

	--View Sites and Site Groups
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 8)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 8,  @Now, 'Varec','Varec',@Now);
		END

	--Modify Site and Site Groups
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 9)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 9,  @Now, 'Varec','Varec',@Now);
		END

	--View Company Data
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 10)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 10, @Now, 'Varec','Varec',@Now);
		END
	--Modify Company Data
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 11)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 11, @Now, 'Varec','Varec',@Now);
		END

	--
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 12)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 12, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 13)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 13, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 14)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 14, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 15)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 15, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 16)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex,  CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 16, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 17)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex,  CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 17, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 18)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 18, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 19)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 19, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 20)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex,  CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 20, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 21)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 21, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 22)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 22, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 23)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 23, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 24)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 24, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 25)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 25, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 26)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 26, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 27)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 27, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 28)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 28, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 29)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 29, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 30)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 30, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 31)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 31, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 32)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 32, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 33)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 33, @Now, 'Varec','Varec',@Now);
		END
	
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 34)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 34, @Now, 'Varec','Varec',@Now);
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 35)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 35, @Now, 'Varec','Varec',@Now); /* Modify Orders */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 36)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 36, @Now, 'Varec','Varec',@Now); /* View Orders */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 37)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 37, @Now, 'Varec','Varec',@Now); /* Create Orders */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 38)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 38, @Now, 'Varec','Varec',@Now); /* View Queries */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 39)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 39, @Now, 'Varec','Varec',@Now); /* Modify Queries */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 40)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 40, @Now, 'Varec','Varec',@Now); /* Modify System Settings */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 41)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 41, @Now, 'Varec','Varec',@Now); /* Perform Reverse Transaction */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 42)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 42, @Now, 'Varec','Varec',@Now); /* View Standing Offers */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 43)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 43, @Now, 'Varec','Varec',@Now); /* Modify Standing Offers */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 44)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 44, @Now, 'Varec','Varec',@Now); /* View Graphics */
		END
		
	
	
	delete from tblExportTransportModeMapping;
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Pipeline','PIPELINE' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Rail Car','RAIL' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Railcar','RAIL' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Tank','OTHER' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Tank Transfer','OTHER' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Truck','TRUCK' )
	insert into tblExportTransportModeMapping(FMATransportMode, FuelPlusTransPortMode) values( 'Vessel','VESSEL' )
	
	/* REMOVE OBSOLETE RIGHTS from map.tblGroupToRight */
	
	--Perform Product Update (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 6;
	--MODIFY ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 35;
	--VIEW ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 36;
	--CREATE ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 37;
	--MODIFY PAYMENT DATA (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 58;
	--VIEW RECOVERY DATA (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 59;
	--MODIFY RECOVERY DATA (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 60;
	--VIEW SUPPLY ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 61;
	--CREATE SUPPLY ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 62;
	--MOIDFY SUPPLY ORDERS (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 63;
	--CREATE ADJUSTMENT (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 65;
	--MODIFY ADJUSTMENT (OBSOLETE)
	DELETE FROM map.tblGroupToRight WHERE LookupRightIndex = 66;
	
	
	
	/*  REMOVE OBSOLETE RIGHTS FROM lookup.tblRight  */
	--Perform Product Update (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 6;
	--MODIFY ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 35;
	--VIEW ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 36;
	--CREATE ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 37;
	--MODIFY PAYMENT DATA (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 58;
	--VIEW RECOVERY DATA (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 59;
	--MODIFY RECOVERY DATA (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 60;
	--VIEW SUPPLY ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 61;
	--CREATE SUPPLY ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 62;
	--MODIFY SUPPLY ORDERS (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 63;
	--CREATE ADJUSTMENT (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 65;
	--MODIFY ADJUSTMENT (OBSOLETE)
	DELETE FROM lookup.tblRight WHERE RightIndex = 66;
	
	-- vthompson CSI 5773
	
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 45)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 45,  @Now, 'Varec','Varec',@Now);          -- View PIDX Profiles
		END

	
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 46)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 46,  @Now, 'Varec','Varec',@Now);          -- Modify PIDX Profiles
		END

	
	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 47)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 47, @Now, 'Varec','Varec',@Now); /* Enable/Disable Stations */
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 178)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 178, @Now, 'Varec','Varec',@Now); /* CREATE_IRS_EXSTARS_REPORT*/
		END

	if not exists (Select GroupGuid from map.tblGroupToRight where GroupGuid = '00000000-0000-0000-0000-000000000003' and LookupRightIndex = 179)
		BEGIN
	INSERT INTO map.tblGroupToRight
		 (GroupGuid, LookupRightIndex, CreatedDate, CreatedBy,UpdatedBy,UpdatedDate)
	VALUES ('00000000-0000-0000-0000-000000000003', 179, @Now, 'Varec','Varec',@Now); /* VIEW_IRS_EXSTARS_REPORT */
		END

	--------------------------

	/* INSERT DEFAULT USER GROUP MAP */

	if not exists (Select GroupGuid from map.tblUserToGroup where GroupGuid = '00000000-0000-0000-0000-000000000003' and UserGuid = '00000000-0000-0000-0000-000000000002')
		BEGIN
	INSERT INTO map.tblUserToGroup
		 (UserGuid,GroupGuid, CreatedDate, CreatedBy,UpdatedBy)
	VALUES ('00000000-0000-0000-0000-000000000002','00000000-0000-0000-0000-000000000003', @Now, 'Varec','Varec');
		END

	if not exists (Select UserGuid from map.tblEntityUserToSite where SiteGuid = '00000000-0000-0000-0000-000000000001' and UserGuid = '00000000-0000-0000-0000-000000000002')
		BEGIN
	INSERT INTO map.tblEntityUserToSite --tblEntityToSiteMap
		 (SiteGuid, UserGuid, CreatedDate, CreatedBy,UpdatedBy)
	VALUES ('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000002', @Now, 'Varec','Varec');
		END

	if not exists (Select GroupGuid from map.tblEntityUserGroupToSite where SiteGuid = '00000000-0000-0000-0000-000000000001' and GroupGuid = '00000000-0000-0000-0000-000000000003')
		BEGIN
	INSERT INTO map.tblEntityUserGroupToSite --tblEntityToSiteMap
		 (SiteGuid, GroupGuid, CreatedDate, CreatedBy,UpdatedBy)
	VALUES ('00000000-0000-0000-0000-000000000001','00000000-0000-0000-0000-000000000003', @Now, 'Varec','Varec');
		END

	/* INSERT DEFAULT SYSTEM SETTINGS */

	if not exists (Select ReportServerURL from tblSystemSettings where ReportServerURL = 'http://localhost/ReportServer')
		BEGIN
	INSERT INTO tblSystemSettings
		 (ReportServerURL, CreatedDate, CreatedBY, UpdatedDate, UpdatedBy)
	VALUES ('http://localhost/ReportServer', @Now, 'Varec', @Now, 'Varec');
		END

	/* POPULATE tblConfigurationSetting*/
	-- AccountingEnterpriseInterface
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'AccountingEnterpriseInterface')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','AccountingEnterpriseInterface','','1900-01-01','Administrator','1900-01-01','Administrator')
		END
	
	-- BKUtility_AdditionalFilesPaths
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_AdditionalFilesPaths')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','BKUtility_AdditionalFilesPaths','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_BUC
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_BUC')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_BUC','0','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_CurrDB
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_CurrDB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_CurrDB','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_LogFileFullPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_LogFileFullPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_LogFileFullPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_LogFilePath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_LogFilePath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_LogFilePath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_Project
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_Project')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_Project','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_SQLDataRoot
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_SQLDataRoot')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_SQLDataRoot','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_SQLTraceFolder
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_SQLTraceFolder')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_SQLTraceFolder','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_SyncTechSystemHome
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_SyncTechSystemHome')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_SyncTechSystemHome','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_Ticks
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_Ticks')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_Ticks','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_xPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_xPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_xPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_yPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_yPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_yPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_ZipFilePath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_ZipFilePath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_ZipFilePath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_zxPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_zxPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_zxPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_zyPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_zyPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_zyPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_DataPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_DataPath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_DataPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseCommandTimeout
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseCommandTimeout')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_EnterpriseCommandTimeout','120','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseConnectionTimeout
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseConnectionTimeout')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'DWORD','BSME_EnterpriseConnectionTimeout','120','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- BSME_EnterpriseDataSource
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseDataSource')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterpriseDataSource','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterprisePassword
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterprisePassword')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterprisePassword','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterprisePort
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterprisePort')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_EnterprisePort','8089','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseUserID
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseUserID')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterpriseUserID','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_LatestSequenceNumber
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_LatestSequenceNumber')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_LatestSequenceNumber','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MaxEnterpriseConcurrentConnections
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MaxEnterpriseConcurrentConnections')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_MaxEnterpriseConcurrentConnections','20','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MaxExpressBatch
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MaxExpressBatch')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_MaxExpressBatch','200','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MFCSLogPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MFCSLogPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_MFCSLogPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_ProcessingSites
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_ProcessingSites')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','BSME_ProcessingSites','SiteAdmin;Base','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_ScanFrequencySeconds
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_ScanFrequencySeconds')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_ScanFrequencySeconds','60','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_TransactionTimeoutSeconds
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_TransactionTimeoutSeconds')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_TransactionTimeoutSeconds','120','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- Common Access Card (CAC) Enable
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'Common Access Card (CAC) Enable')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','Common Access Card (CAC) Enable','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- CustomClientScriptName
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'CustomClientScriptName')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','CustomClientScriptName','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- CustomTransactionFieldAssemblyPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'CustomTransactionFieldAssemblyPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','CustomTransactionFieldAssemblyPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- DataDictionaryAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'DataDictionaryAssemblies')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'MULTI_SZ','DataDictionaryAssemblies','FuelsManager.dll;','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- DISPATCH_PollTime
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'DISPATCH_PollTime')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','DISPATCH_PollTime','3','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- ExternalExportResultsInterfaceName
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'ExternalExportResultsInterfaceName')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','ExternalExportResultsInterfaceName','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- FMAETranslationsConfigurationSiteGroup
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'FMAETranslationsConfigurationSiteGroup')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','FMAETranslationsConfigurationSiteGroup','Varec','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IDependencyAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IDependencyAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','IDependencyAssemblies','FMBusinessServices.dll','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IDiscoveryAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IDiscoveryAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','IDiscoveryAssemblies','FuelsManager.dll;FMBusinessObjects.dll;','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- InstallDetailsSynchronizationProfileID
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'InstallDetailsSynchronizationProfileID')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','InstallDetailsSynchronizationProfileID','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsApplicationReceiversCode_GS03
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsApplicationReceiversCode_GS03')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsApplicationReceiversCode_GS03','040539587050','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsDunsNumber_ISA08
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsDunsNumber_ISA08')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsDunsNumber_ISA08','040539587','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08','004030','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsInterchangeControlVersion_ISA12
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsInterchangeControlVersion_ISA12')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsInterchangeControlVersion_ISA12','00403','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- ExSTARS IRS Transportation Modes FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09	Rev 11-2005, page 14
	-- All values should be upper case and separated by "="
	-- TFS06 is 2 characters in length. Trailing spaces are added by the applicatio for codes J, B, R and S. 
	-- PRIMARY and SECONDARY storage can be optionally specified

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'HYDRANT TRUCK=J,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','HYDRANT TRUCK=J,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END


	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'GSE TRUCK=RS,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','GSE TRUCK=RS,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END
	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'HYDRANT CART=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','HYDRANT CART=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'STATIONARY CART=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','STATIONARY CART=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'FILL STAND=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','FILL STAND=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TANK=RT,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TANK=RT,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'FILTER=RT')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','FILTER=RT','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TANKER=J,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TANKER=J,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE-I=IP,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE-I=IP,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK-I=IJ,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK-I=IJ,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL-I=IR')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL-I=IR','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP-I=IS')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP-I=IS','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE-I=IB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE-I=IB','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE-E=EP,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE-E=EP,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK-E=EJ')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK-E=EJ','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL-E=ER')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL-E=ER','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP-E=ES')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP-E=ES','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE-E=EB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE-E=EB','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE=PL,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE=PL,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK=J,SECONDARY ')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK=J,SECONDARY ','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL=R')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL=R','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP=S')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP=S','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE=B')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE=B','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BOOK ADJUSTMENT=BA')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BOOK ADJUSTMENT=BA','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SUMMARY=CE')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SUMMARY=CE','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'REMOVE FROM TERMINAL=RT')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','REMOVE FROM TERMINAL=RT','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsISA05Qualifier
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsISA05Qualifier')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsISA05Qualifier','32','1900-01-01','Administrator','1900-01-01','Administrator')
		END

			-- IrsExStarsEnableDebugFeatures
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsEnableDebugFeatures')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsEnableDebugFeatures','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsProductCodesRegEx
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsProductCodesRegEx')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'SZ','IrsProductCodesRegEx','^((E|M|B|D)\d\d)|(090|125|248|122|055|249|093|126|059|223|121|199|100|076|198|224|161|167|150|154|282|283|226|227|231|153|052|196|065|058|145|147|073|074|130|077|225|279|280|265|281|054|075|092|001|049|188|960|285|091)$','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- ISecurityAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'ISecurityAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','ISecurityAssemblies','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LoadRackInstalled
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LoadRackInstalled')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','LoadRackInstalled','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LoadRackPort
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LoadRackPort')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','LoadRackPort','8087','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LR_QualityAssuranceInterface
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LR_QualityAssuranceInterface')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','LR_QualityAssuranceInterface','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- MaxConcurrentSessionsPerUser
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'MaxConcurrentSessionsPerUser')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','MaxConcurrentSessionsPerUser','100000','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- NSPA_FuelCardImportConnectionString
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'NSPA_FuelCardImportConnectionString')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','NSPA_FuelCardImportConnectionString','Provider=Microsoft.ACE.OLEDB.12.0;Data Source=<filename>;Extended Properties="Excel 12.0 Xml;HDR=YES;IMEX=1;"','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineAdminDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineAdminDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineAdminDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineAdminTutorialDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineAdminTutorialDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineAdminTutorialDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineHelpDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineHelpDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineHelpDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineTutorialDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineTutorialDoc')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineTutorialDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END


IF( 126 <> ( SELECT count(*) as count FROM [dbo].[tblExStarsIrsErrorCodes]))	
BEGIN
	DELETE [dbo].[tblExStarsIrsErrorCodes]
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'1001',	'Tax Information Code','TIA01')                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'1003',	'Fixed Format Code','TIA03')                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'1004',	'Quantity','TIA04')                                                        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'1005',	'Unit of Measure','TIA05')                                                 
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2017',	'Permit Qualifier Code',	'N/A')                                          
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2018',	'Transaction Purpose Code','BTI13')                                        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2019',	'Transaction Type Code','BTI14')                                           
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2020',	'Reference ID Qualifier','TFS01')                                          
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2021',	'Reference ID','TFS02')                                                    
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2022',	'Reference ID Qualifier','TFS03')                                          
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2023',	'Reference ID','TFS04')                                                    
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2024',	'ID Code Qualifier','TFS05')                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2025',	'ID Code','TFS06')                                                         
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2026',	'Reference ID Qualifier','REF01')                                          
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2027',	'Reference ID','REF02')                                                    
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2028',	'Reference ID','REF03')                                                    
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2029',	'Reference ID',	'REF04-01')                                              
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2030',	'Reference ID',	'REF04-02')                                              
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2031',	'Reference ID',	'REF04-03')                                              
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2032',	'Reference ID',	'REF04-04')        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2033',	'Assigned ID',	'FGS01')        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2034',	'Assigned ID',	'FGS02')        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2035',	'Reference ID',	'FGS03')        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2036',	'ID Code Qualifier',	'BTI03')        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'2037',	'ID Code',	'BTI12')        	                                      
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'4007',	'Date','DTM02')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6001',	'Entity ID Code','N101')                                                   
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6002',	'Information Provider Name','N102')                                        
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6003',	'Identification Qualification Code','N103')                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6004',	'Identification Code','N104')                                              
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6005',	'Contact Function Code','PER01')                                           
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6006',	'Contact Name','PER02')                                                    
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6007',	'Telephone Number Qualifier','PER03')                                      
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6008',	'Telephone Number','PER04')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6009',	'Fax Number Qualifier','PER05')                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6010',	'Fax Number','PER06')                                                      
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6011',	'E-mail Qualifier','PER07')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6012',	'E-mail Address','PER08')                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6013',	'Address Information','N301')                                              
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6014',	'City','N401')                                                             
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6015',	'State or Province','N402')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6016',	'Zip Code','N403')                                                         
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6017',	'Country','N404')                                                          
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6888',	'Invalid use of Foreign Flag code','N104')                                 
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'6999',	'Invalid Non-bulk Carrier','N104')                                         
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Primary',	'9999',	'Out of Balanace',	'N/A')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'01',	'Invalid',                        	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'02',	'Invalid Based on Related Data',  	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'03',	'Non Numeric',                    	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'04',	'Calculation Error',              	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'05',	'Missing',                        	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'06',	'Required due to Related Data',   	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'07',	'Not Found',                      	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'08',	'Format Error',                   	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'09',	'Negative',                       	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'10',	'Duplicate',                      	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'11',	'Tolerance',                      	'')                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI01_Secondary',	'12',	'Out of Range',                   	'')                                       

	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'100',	'The field is mandatory, but does not contain a value.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'101',	'The field contains an invalid value.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'102',	'The field contains an invalid date or a date in the future.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'103',	'The field is mandatory for amended submission, but does not contain a value.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'104',	'The field is mandatory for initial submission, but does not contain a value.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'105',	'The telephone number is incomplete.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'106',	'The field contains an invalid data type.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'107',	'The field contains a value different from ST02.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'108',	'The field contains a value different from GS06.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'109',	'The field contains a value different from ISA13.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'112',	'The N1 segment for Position Holder must be present.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'113',	'The N1 segment for Point of Origin must be present.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'114',	'The N1 segment for Carrier must be present.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'115',	'The N1 segment for Point of Destination must be present.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'116',	'The N1 segment for Consignor must be present for transactions.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'117',	'All Dates must be equal to or less than todays date.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'118',	'Ticket dates cannot be any older than 1 year.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'121',	'If a terminal shows either receipts or disbursements a TOR ending inventory report is required.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'122',	'If the filing company is a terminal operator and a carrier and the terminal operator report show carrier activity for a company on the schedules, then a CCR report is required.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'123',	'If and EDI file is transmitted to the IRS that does not have a TOR or CCR section but has schedule activity, the file is incomplete.  The file needs to be corrected and resubmitted.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'124',	'If and EDI file is transmitted to the IRS that has a TOR or CCR section but has no schedule activity and has not indicated in teh TOR or CCR section that the company has no business activity, the file is incomplete.  The file needs to be corrected and resubmitted.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'125',	'If the transaction is a terminal receipt, then the net gallons value is required.  The gross gallons value is optional.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'126',	'If the transaction is a bulk terminal disbursement, then the net galons value is required.  The gross gallons value is optional.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'127',	'If the transaction is a non-bulk disbursement reported by the operator then the net gallons value is required.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'128',	'If the transaction is a non-bulk disbursement reported by the operator then the gross gallons value is required.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'129',	'If the transaction is a carrier delivery, then the net gallons is required.  Gross gallons are optional.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'130',	'If the transaction is a terminal receipt for a carrier then the net gallons value is required.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'131',	'Information is invalid because of related information in the TFS loop.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'132',	'Information is invalid because of related information in the FGS loop.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'133',	'Duplicate Originals',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'134',	'Duplicate Sequence Numbers',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'135',	'Missing Sequence Number',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Primary',	'999',	'Transaction is Out of Balance.',	'')

	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Secondary',	'1',	'Fatal Error.  Out of Balance.  File not accepted by the IRS as a filed return.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Secondary',	'2',	'Correction Error - Error must be corrected and resubmitted prior to next months filing.  File is accepted as a filed return.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Secondary',	'3',	'Minor Error (Warning message) - Information Provider will not have to resubmit the correction, just correct the system for next months filing.  File is accepted as a filed return.',	'')
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI03_Secondary',	'4',	'',	'')

	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'00000',	'Overview',	'')      -- Used at the top of the report
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0001',	'Transaction Set Control Number',	'')      -- Informational Message: An information only message is provided                                       
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0002',	'Total net Gallons Reported in Information Return',	'')                           
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0003',	'Ending Inventory Net Gallons',	'')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0004',	'Total Net Gallons Transported',	'')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0005',	'Net Gallons',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0006',	'Gross Gallons',	'')                                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0007',	'Information Provider Name',	'')                                                   
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0008',	'Origin Terminal',	'')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0009',	'Ship From State',	'')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0010',	'Consignor',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0011',	'Carrier Name',	'')                                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0012',	'Destination Terminal',	'')                                                         
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0013',	'Ship To State',	'')                                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0014',	'Period End Date',	'')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0015',	'Inventory Date',	'')                                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0016',	'Document Date',	'')                                                               
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0017',	'Position Holder',	'')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0018',	'637 Number',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0019',	'Relationship to Information',	'')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0020',	'Sequence Number',	'')                                                            
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0021',	'No Activity',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0022',	'Information Provider Location',	'')                                                
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0023',	'Terminal Operator Report (TOR)',	'')                                             
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0024',	'Carrier Report (CCR)',	'')                                                         
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0025',	'Schedules',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0026',	'Ending Inventory Loop',	'')                                                      
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0027',	'Shipping document Loop',	'')                                                      
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0028',	'Carrier EIN',	'')                                                                  
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0029',	'Change of Terminal Operator Date',	'')                                             
	INSERT INTO [dbo].[tblExStarsIrsErrorCodes] ([CodeGroup],[Code],[Description],[ElementId]) VALUES('PBI04',	'E0030',	'Carrier EIN for non-bulk terminal disbursements - may be required by states',	'')
END


/*


	DECLARE @SiteGuid UNIQUEIDENTIFIER='83E7A158-4A71-4C2F-9157-9BDC383C9D51'
	DECLARE @ManagerCompanyGuid UNIQUEIDENTIFIER='7158995A-23C6-4348-BA80-96B3D93BE9A4'
	DECLARE @InterchangeSenderId NVARCHAR(15)='581330111'
	DECLARE @ApplicationSendersCode NVARCHAR(15)='546437422644467'
	DECLARE @AuthorizationCode NVARCHAR(10)='T022110307'
	DECLARE @FeinCode NVARCHAR(18)='581330111'
	DECLARE @SecurityCode NVARCHAR(10)='3157858426'
	DECLARE @InfoProviderName NVARCHAR(18)='VAREC'
	DECLARE @AbbreviatedProviderName NVARCHAR(18)='VARE'
	DECLARE @GroupControlNumber NVARCHAR(9)='notused'
	DECLARE @IRS_637Registration NVARCHAR(18)='' -- BTI12: Terminal operators only, blank for airports
	DECLARE @TerminalControlNumber NVARCHAR(9) = 'T36IL3325'
	DECLARE @UpdatedBy udtUserID='pcarpenter'


	INSERT INTO [dbo].[tblExStarsSiteConfig]
				([SiteGuid]
				,[ManagerCompanyGuid]
				,[InterchangeSenderId]
				,[ApplicationSendersCode]
				,[AuthorizationCode]
				,[FeinCode]
				,[SecurityCode]
				,[InfoProviderName]
				,[AbbreviatedProviderName]
				,[GroupControlNumber]
				,[IRS_637Registration]
				,[TerminalControlNumber]
				,[CreatedDate]
				,[CreatedBy]
				,[UpdatedDate]
				,[UpdatedBy])
			VALUES(
					@SiteGuid
				,@ManagerCompanyGuid
				,@InterchangeSenderId
				,@ApplicationSendersCode
				,@AuthorizationCode
				,@FeinCode
				,@SecurityCode
				,@InfoProviderName
				,@AbbreviatedProviderName
				,@GroupControlNumber
				,@IRS_637Registration
				,@TerminalControlNumber
				,GETDATE()
				,@UpdatedBy
				,GETDATE()
				,@UpdatedBy
					)

*/


	--delete tblDataDictionaries where SettingKey like 'ExSTARS%'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Std Monthly Report Description', 'Note: This report type should be used to generate a standard monthly IRS report where the transactional data in the report will correspond to the Month,Year, and Manager selected below. DO NOT use this report type  if there was an inventory manager hand-over within the month and year selected.'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Outgoing Mgr Report Description','Note: Only the OUTGOING inventory manager should use this report type  to generate an IRS report that will contain activity data from the first day of the month associated with the Inventory Hand-over Date up to and including the Inventory Hand-over Date.'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Incoming Mgr Report Description','Note: Only the INCOMING inventory manager should use this report type  to generate an IRS report that will contain the physical inventory reading on the Inventory Hand-over Date along with all remaining activity data up to and including the last day of the month associated with the day.'

	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Incoming/Outgoing Mgr Warning', 'Outgoing and Incoming manager reports should only be created when the company managing the fuel is changing. They must always be created in pairs, with the outgoing report created first.   <br><br>Do you wish to create this report?'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS StdMonthly',        'Standard Monthly'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS OutgoingManger',	'Outgoing Manager'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS IncomingManager',	'Incoming Manager'
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Recreation Warning',	'Warning: Re-Creating Report '
	exec [usp_InsertDataDictionaryRow] 'MDW - Skytanking', 'ExSTARS Recreation Description',	'Reports can be recreated under two circumstances: (1) When the report has not been sent to the IRS. and (2) When the report had errors that prevented a 151 file from being created.'

END