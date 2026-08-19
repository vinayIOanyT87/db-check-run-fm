CREATE PROCEDURE [dbo].[gsp_PersonnelUpdateByPK]
(
		@PersonnelGuid uniqueidentifier
	,	@PersonID nvarchar(50)=NULL
	,	@CardNumber nvarchar(30)=NULL
	,	@FirstName nvarchar(20)=NULL
	,	@MiddleName nvarchar(20)=NULL
	,	@LastName nvarchar(30)=NULL
	,	@Title nvarchar(50)=NULL
	,	@Department nvarchar(20)=NULL
	,	@Address1 nvarchar(50)=NULL
	,	@Address2 nvarchar(50)=NULL
	,	@City nvarchar(60)=NULL
	,	@State nvarchar(20)=NULL
	,	@Zip nvarchar(10)=NULL
	,	@Country nvarchar(20)=NULL
	,	@Phone1 nvarchar(50)=NULL
	,	@Phone2 nvarchar(50)=NULL
	,	@AssignmentDate datetimeoffset(7)=NULL
	,	@SupervisionDate datetimeoffset(7)=NULL
	,	@SSAN nvarchar(11)=NULL
	,	@BirthDate datetimeoffset(7)=NULL
	,	@PayRate money=NULL
	,	@LaborRate1 float=NULL
	,	@LaborRate2 float=NULL
	,	@LaborRate3 float=NULL
	,	@LaborRate4 float=NULL
	,	@Status smallint=NULL
	,	@Email nvarchar(50)=NULL
	,	@ResponsibleOfficer bit=NULL
	,	@Shift smallint=NULL
	,	@PINNumber varbinary(256)=NULL
	,	@PINRequired bit=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@LastActivityDate datetimeoffset(7)=NULL
	,	@CardedIn bit=NULL
	,	@ShortCardNumber nvarchar(6)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@OnFileSignature image=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UserData9 nvarchar(60)=NULL
	,	@UserData10 nvarchar(60)=NULL
	,	@UserData11 nvarchar(60)=NULL
	,	@UserData12 nvarchar(60)=NULL
	,	@UserData13 nvarchar(60)=NULL
	,	@UserData14 nvarchar(60)=NULL
	,	@UserData15 nvarchar(60)=NULL
	,	@UserData16 nvarchar(60)=NULL
	,	@UserData17 nvarchar(60)=NULL
	,	@UserData18 nvarchar(60)=NULL
	,	@UserData19 nvarchar(60)=NULL
	,	@UserData20 nvarchar(60)=NULL
	,	@UserData21 nvarchar(60)=NULL
	,	@UserData22 nvarchar(60)=NULL
	,	@UserData23 nvarchar(60)=NULL
	,	@UserData24 nvarchar(60)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@CompanyGuid uniqueidentifier=NULL
	,	@SupervisorPersonnelGuid uniqueidentifier=NULL
	,	@UserGuid uniqueidentifier=NULL
	,	@AssignedEquipmentGuid uniqueidentifier=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverridePersonID BIT=0 
	,	@NullOverrideCardNumber BIT=0 
	,	@NullOverrideFirstName BIT=0 
	,	@NullOverrideMiddleName BIT=0 
	,	@NullOverrideLastName BIT=0 
	,	@NullOverrideTitle BIT=0 
	,	@NullOverrideDepartment BIT=0 
	,	@NullOverrideAddress1 BIT=0 
	,	@NullOverrideAddress2 BIT=0 
	,	@NullOverrideCity BIT=0 
	,	@NullOverrideState BIT=0 
	,	@NullOverrideZip BIT=0 
	,	@NullOverrideCountry BIT=0 
	,	@NullOverridePhone1 BIT=0 
	,	@NullOverridePhone2 BIT=0 
	,	@NullOverrideAssignmentDate BIT=0 
	,	@NullOverrideSupervisionDate BIT=0 
	,	@NullOverrideSSAN BIT=0 
	,	@NullOverrideBirthDate BIT=0 
	,	@NullOverridePayRate BIT=0 
	,	@NullOverrideLaborRate1 BIT=0 
	,	@NullOverrideLaborRate2 BIT=0 
	,	@NullOverrideLaborRate3 BIT=0 
	,	@NullOverrideLaborRate4 BIT=0 
	,	@NullOverrideStatus BIT=0 
	,	@NullOverrideEmail BIT=0 
	,	@NullOverrideResponsibleOfficer BIT=0 
	,	@NullOverrideShift BIT=0 
	,	@NullOverridePINNumber BIT=0 
	,	@NullOverridePINRequired BIT=0 
	,	@NullOverrideLockedOut BIT=0 
	,	@NullOverrideLockedOutReason BIT=0 
	,	@NullOverrideLockedOutDate BIT=0 
	,	@NullOverrideLastActivityDate BIT=0 
	,	@NullOverrideCardedIn BIT=0 
	,	@NullOverrideShortCardNumber BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideOnFileSignature BIT=0 
	,	@NullOverrideUserData1 BIT=0 
	,	@NullOverrideUserData2 BIT=0 
	,	@NullOverrideUserData3 BIT=0 
	,	@NullOverrideUserData4 BIT=0 
	,	@NullOverrideUserData5 BIT=0 
	,	@NullOverrideUserData6 BIT=0 
	,	@NullOverrideUserData7 BIT=0 
	,	@NullOverrideUserData8 BIT=0 
	,	@NullOverrideUserData9 BIT=0 
	,	@NullOverrideUserData10 BIT=0 
	,	@NullOverrideUserData11 BIT=0 
	,	@NullOverrideUserData12 BIT=0 
	,	@NullOverrideUserData13 BIT=0 
	,	@NullOverrideUserData14 BIT=0 
	,	@NullOverrideUserData15 BIT=0 
	,	@NullOverrideUserData16 BIT=0 
	,	@NullOverrideUserData17 BIT=0 
	,	@NullOverrideUserData18 BIT=0 
	,	@NullOverrideUserData19 BIT=0 
	,	@NullOverrideUserData20 BIT=0 
	,	@NullOverrideUserData21 BIT=0 
	,	@NullOverrideUserData22 BIT=0 
	,	@NullOverrideUserData23 BIT=0 
	,	@NullOverrideUserData24 BIT=0 
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideCompanyGuid BIT=0 
	,	@NullOverrideSupervisorPersonnelGuid BIT=0 
	,	@NullOverrideUserGuid BIT=0 
	,	@NullOverrideAssignedEquipmentGuid BIT=0 
	,	@NullOverride_MasterRecordGuid BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PersonnelUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.6064078 -05:00
	-- Purpose: Update table [dbo].[tblPersonnel]
	-- Notes:
	-- 1. @PersonnelGuid and @UpdatedBy are required parameter.
	-- 2. If a value other than NULL is passed on @_RowVersion parameter then the stored procedure verifies whether _RowVersion of the record matches with the  
	--    @_RowVersion parameter and it will throw an exception if they don't match, otherwise it saves the parameters regardless.
	-- 3. The @_RowVersion output parameter will always be updated with new timestamp generated by the updating of the record.
	-- 4. To update a column with NULL then set the corresponding "@NullOverride..." parameter to 1 and either pass NULL through the correlated parameter 
	--    or do not include the parameter at all. 
	--    Example - Saving NULL to SiteGuid on tblEquipment:
	--            EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...',@SiteGuid=NULL, @NullOverrideSiteGuid=1 
	--       or   EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...', @NullOverrideSiteGuid=1 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblPersonnel] WHERE PersonnelGuid=@PersonnelGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblPersonnel] SET
			[PersonID]=(CASE ISNULL(@NullOverridePersonID,0) WHEN 1 THEN @PersonID ELSE ISNULL(@PersonID,[PersonID]) END)
		,	[CardNumber]=(CASE ISNULL(@NullOverrideCardNumber,0) WHEN 1 THEN @CardNumber ELSE ISNULL(@CardNumber,[CardNumber]) END)
		,	[FirstName]=(CASE ISNULL(@NullOverrideFirstName,0) WHEN 1 THEN @FirstName ELSE ISNULL(@FirstName,[FirstName]) END)
		,	[MiddleName]=(CASE ISNULL(@NullOverrideMiddleName,0) WHEN 1 THEN @MiddleName ELSE ISNULL(@MiddleName,[MiddleName]) END)
		,	[LastName]=(CASE ISNULL(@NullOverrideLastName,0) WHEN 1 THEN @LastName ELSE ISNULL(@LastName,[LastName]) END)
		,	[Title]=(CASE ISNULL(@NullOverrideTitle,0) WHEN 1 THEN @Title ELSE ISNULL(@Title,[Title]) END)
		,	[Department]=(CASE ISNULL(@NullOverrideDepartment,0) WHEN 1 THEN @Department ELSE ISNULL(@Department,[Department]) END)
		,	[Address1]=(CASE ISNULL(@NullOverrideAddress1,0) WHEN 1 THEN @Address1 ELSE ISNULL(@Address1,[Address1]) END)
		,	[Address2]=(CASE ISNULL(@NullOverrideAddress2,0) WHEN 1 THEN @Address2 ELSE ISNULL(@Address2,[Address2]) END)
		,	[City]=(CASE ISNULL(@NullOverrideCity,0) WHEN 1 THEN @City ELSE ISNULL(@City,[City]) END)
		,	[State]=(CASE ISNULL(@NullOverrideState,0) WHEN 1 THEN @State ELSE ISNULL(@State,[State]) END)
		,	[Zip]=(CASE ISNULL(@NullOverrideZip,0) WHEN 1 THEN @Zip ELSE ISNULL(@Zip,[Zip]) END)
		,	[Country]=(CASE ISNULL(@NullOverrideCountry,0) WHEN 1 THEN @Country ELSE ISNULL(@Country,[Country]) END)
		,	[Phone1]=(CASE ISNULL(@NullOverridePhone1,0) WHEN 1 THEN @Phone1 ELSE ISNULL(@Phone1,[Phone1]) END)
		,	[Phone2]=(CASE ISNULL(@NullOverridePhone2,0) WHEN 1 THEN @Phone2 ELSE ISNULL(@Phone2,[Phone2]) END)
		,	[AssignmentDate]=(CASE ISNULL(@NullOverrideAssignmentDate,0) WHEN 1 THEN @AssignmentDate ELSE ISNULL(@AssignmentDate,[AssignmentDate]) END)
		,	[SupervisionDate]=(CASE ISNULL(@NullOverrideSupervisionDate,0) WHEN 1 THEN @SupervisionDate ELSE ISNULL(@SupervisionDate,[SupervisionDate]) END)
		,	[SSAN]=(CASE ISNULL(@NullOverrideSSAN,0) WHEN 1 THEN @SSAN ELSE ISNULL(@SSAN,[SSAN]) END)
		,	[BirthDate]=(CASE ISNULL(@NullOverrideBirthDate,0) WHEN 1 THEN @BirthDate ELSE ISNULL(@BirthDate,[BirthDate]) END)
		,	[PayRate]=(CASE ISNULL(@NullOverridePayRate,0) WHEN 1 THEN @PayRate ELSE ISNULL(@PayRate,[PayRate]) END)
		,	[LaborRate1]=(CASE ISNULL(@NullOverrideLaborRate1,0) WHEN 1 THEN @LaborRate1 ELSE ISNULL(@LaborRate1,[LaborRate1]) END)
		,	[LaborRate2]=(CASE ISNULL(@NullOverrideLaborRate2,0) WHEN 1 THEN @LaborRate2 ELSE ISNULL(@LaborRate2,[LaborRate2]) END)
		,	[LaborRate3]=(CASE ISNULL(@NullOverrideLaborRate3,0) WHEN 1 THEN @LaborRate3 ELSE ISNULL(@LaborRate3,[LaborRate3]) END)
		,	[LaborRate4]=(CASE ISNULL(@NullOverrideLaborRate4,0) WHEN 1 THEN @LaborRate4 ELSE ISNULL(@LaborRate4,[LaborRate4]) END)
		,	[Status]=(CASE ISNULL(@NullOverrideStatus,0) WHEN 1 THEN @Status ELSE ISNULL(@Status,[Status]) END)
		,	[Email]=(CASE ISNULL(@NullOverrideEmail,0) WHEN 1 THEN @Email ELSE ISNULL(@Email,[Email]) END)
		,	[ResponsibleOfficer]=(CASE ISNULL(@NullOverrideResponsibleOfficer,0) WHEN 1 THEN @ResponsibleOfficer ELSE ISNULL(@ResponsibleOfficer,[ResponsibleOfficer]) END)
		,	[Shift]=(CASE ISNULL(@NullOverrideShift,0) WHEN 1 THEN @Shift ELSE ISNULL(@Shift,[Shift]) END)
		,	[PINNumber]=(CASE ISNULL(@NullOverridePINNumber,0) WHEN 1 THEN @PINNumber ELSE ISNULL(@PINNumber,[PINNumber]) END)
		,	[PINRequired]=(CASE ISNULL(@NullOverridePINRequired,0) WHEN 1 THEN @PINRequired ELSE ISNULL(@PINRequired,[PINRequired]) END)
		,	[LockedOut]=(CASE ISNULL(@NullOverrideLockedOut,0) WHEN 1 THEN @LockedOut ELSE ISNULL(@LockedOut,[LockedOut]) END)
		,	[LockedOutReason]=(CASE ISNULL(@NullOverrideLockedOutReason,0) WHEN 1 THEN @LockedOutReason ELSE ISNULL(@LockedOutReason,[LockedOutReason]) END)
		,	[LockedOutDate]=(CASE ISNULL(@NullOverrideLockedOutDate,0) WHEN 1 THEN @LockedOutDate ELSE ISNULL(@LockedOutDate,[LockedOutDate]) END)
		,	[LastActivityDate]=(CASE ISNULL(@NullOverrideLastActivityDate,0) WHEN 1 THEN @LastActivityDate ELSE ISNULL(@LastActivityDate,[LastActivityDate]) END)
		,	[CardedIn]=(CASE ISNULL(@NullOverrideCardedIn,0) WHEN 1 THEN @CardedIn ELSE ISNULL(@CardedIn,[CardedIn]) END)
		,	[ShortCardNumber]=(CASE ISNULL(@NullOverrideShortCardNumber,0) WHEN 1 THEN @ShortCardNumber ELSE ISNULL(@ShortCardNumber,[ShortCardNumber]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[OnFileSignature]=(CASE ISNULL(@NullOverrideOnFileSignature,0) WHEN 1 THEN @OnFileSignature ELSE ISNULL(@OnFileSignature,[OnFileSignature]) END)
		,	[UserData1]=(CASE ISNULL(@NullOverrideUserData1,0) WHEN 1 THEN @UserData1 ELSE ISNULL(@UserData1,[UserData1]) END)
		,	[UserData2]=(CASE ISNULL(@NullOverrideUserData2,0) WHEN 1 THEN @UserData2 ELSE ISNULL(@UserData2,[UserData2]) END)
		,	[UserData3]=(CASE ISNULL(@NullOverrideUserData3,0) WHEN 1 THEN @UserData3 ELSE ISNULL(@UserData3,[UserData3]) END)
		,	[UserData4]=(CASE ISNULL(@NullOverrideUserData4,0) WHEN 1 THEN @UserData4 ELSE ISNULL(@UserData4,[UserData4]) END)
		,	[UserData5]=(CASE ISNULL(@NullOverrideUserData5,0) WHEN 1 THEN @UserData5 ELSE ISNULL(@UserData5,[UserData5]) END)
		,	[UserData6]=(CASE ISNULL(@NullOverrideUserData6,0) WHEN 1 THEN @UserData6 ELSE ISNULL(@UserData6,[UserData6]) END)
		,	[UserData7]=(CASE ISNULL(@NullOverrideUserData7,0) WHEN 1 THEN @UserData7 ELSE ISNULL(@UserData7,[UserData7]) END)
		,	[UserData8]=(CASE ISNULL(@NullOverrideUserData8,0) WHEN 1 THEN @UserData8 ELSE ISNULL(@UserData8,[UserData8]) END)
		,	[UserData9]=(CASE ISNULL(@NullOverrideUserData9,0) WHEN 1 THEN @UserData9 ELSE ISNULL(@UserData9,[UserData9]) END)
		,	[UserData10]=(CASE ISNULL(@NullOverrideUserData10,0) WHEN 1 THEN @UserData10 ELSE ISNULL(@UserData10,[UserData10]) END)
		,	[UserData11]=(CASE ISNULL(@NullOverrideUserData11,0) WHEN 1 THEN @UserData11 ELSE ISNULL(@UserData11,[UserData11]) END)
		,	[UserData12]=(CASE ISNULL(@NullOverrideUserData12,0) WHEN 1 THEN @UserData12 ELSE ISNULL(@UserData12,[UserData12]) END)
		,	[UserData13]=(CASE ISNULL(@NullOverrideUserData13,0) WHEN 1 THEN @UserData13 ELSE ISNULL(@UserData13,[UserData13]) END)
		,	[UserData14]=(CASE ISNULL(@NullOverrideUserData14,0) WHEN 1 THEN @UserData14 ELSE ISNULL(@UserData14,[UserData14]) END)
		,	[UserData15]=(CASE ISNULL(@NullOverrideUserData15,0) WHEN 1 THEN @UserData15 ELSE ISNULL(@UserData15,[UserData15]) END)
		,	[UserData16]=(CASE ISNULL(@NullOverrideUserData16,0) WHEN 1 THEN @UserData16 ELSE ISNULL(@UserData16,[UserData16]) END)
		,	[UserData17]=(CASE ISNULL(@NullOverrideUserData17,0) WHEN 1 THEN @UserData17 ELSE ISNULL(@UserData17,[UserData17]) END)
		,	[UserData18]=(CASE ISNULL(@NullOverrideUserData18,0) WHEN 1 THEN @UserData18 ELSE ISNULL(@UserData18,[UserData18]) END)
		,	[UserData19]=(CASE ISNULL(@NullOverrideUserData19,0) WHEN 1 THEN @UserData19 ELSE ISNULL(@UserData19,[UserData19]) END)
		,	[UserData20]=(CASE ISNULL(@NullOverrideUserData20,0) WHEN 1 THEN @UserData20 ELSE ISNULL(@UserData20,[UserData20]) END)
		,	[UserData21]=(CASE ISNULL(@NullOverrideUserData21,0) WHEN 1 THEN @UserData21 ELSE ISNULL(@UserData21,[UserData21]) END)
		,	[UserData22]=(CASE ISNULL(@NullOverrideUserData22,0) WHEN 1 THEN @UserData22 ELSE ISNULL(@UserData22,[UserData22]) END)
		,	[UserData23]=(CASE ISNULL(@NullOverrideUserData23,0) WHEN 1 THEN @UserData23 ELSE ISNULL(@UserData23,[UserData23]) END)
		,	[UserData24]=(CASE ISNULL(@NullOverrideUserData24,0) WHEN 1 THEN @UserData24 ELSE ISNULL(@UserData24,[UserData24]) END)
		,	[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[CompanyGuid]=(CASE ISNULL(@NullOverrideCompanyGuid,0) WHEN 1 THEN @CompanyGuid ELSE ISNULL(@CompanyGuid,[CompanyGuid]) END)
		,	[SupervisorPersonnelGuid]=(CASE ISNULL(@NullOverrideSupervisorPersonnelGuid,0) WHEN 1 THEN @SupervisorPersonnelGuid ELSE ISNULL(@SupervisorPersonnelGuid,[SupervisorPersonnelGuid]) END)
		,	[UserGuid]=(CASE ISNULL(@NullOverrideUserGuid,0) WHEN 1 THEN @UserGuid ELSE ISNULL(@UserGuid,[UserGuid]) END)
		,	[AssignedEquipmentGuid]=(CASE ISNULL(@NullOverrideAssignedEquipmentGuid,0) WHEN 1 THEN @AssignedEquipmentGuid ELSE ISNULL(@AssignedEquipmentGuid,[AssignedEquipmentGuid]) END)
		,	[_MasterRecordGuid]=(CASE ISNULL(@NullOverride_MasterRecordGuid,0) WHEN 1 THEN @_MasterRecordGuid ELSE ISNULL(@_MasterRecordGuid,[_MasterRecordGuid]) END)
		WHERE	PersonnelGuid=@PersonnelGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPersonnel]           
		WHERE PersonnelGuid=@PersonnelGuid;
	
 
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
						+ 'Procedure Name: gsp_PersonnelUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
