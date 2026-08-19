CREATE PROCEDURE [dbo].[gsp_PersonnelInsertByPK]
(
		@PersonnelGuid uniqueidentifier=NULL OUTPUT
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
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
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
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PersonnelInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3122767 -05:00
	-- Purpose: Insert into table [dbo].[tblPersonnel]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PersonnelGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPersonnel] 
		(
			[PersonnelGuid]
		,	[PersonID]
		,	[CardNumber]
		,	[FirstName]
		,	[MiddleName]
		,	[LastName]
		,	[Title]
		,	[Department]
		,	[Address1]
		,	[Address2]
		,	[City]
		,	[State]
		,	[Zip]
		,	[Country]
		,	[Phone1]
		,	[Phone2]
		,	[AssignmentDate]
		,	[SupervisionDate]
		,	[SSAN]
		,	[BirthDate]
		,	[PayRate]
		,	[LaborRate1]
		,	[LaborRate2]
		,	[LaborRate3]
		,	[LaborRate4]
		,	[Status]
		,	[Email]
		,	[ResponsibleOfficer]
		,	[Shift]
		,	[PINNumber]
		,	[PINRequired]
		,	[LockedOut]
		,	[LockedOutReason]
		,	[LockedOutDate]
		,	[LastActivityDate]
		,	[CardedIn]
		,	[ShortCardNumber]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[OnFileSignature]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[UserData9]
		,	[UserData10]
		,	[UserData11]
		,	[UserData12]
		,	[UserData13]
		,	[UserData14]
		,	[UserData15]
		,	[UserData16]
		,	[UserData17]
		,	[UserData18]
		,	[UserData19]
		,	[UserData20]
		,	[UserData21]
		,	[UserData22]
		,	[UserData23]
		,	[UserData24]
		,	[SiteGuid]
		,	[CompanyGuid]
		,	[SupervisorPersonnelGuid]
		,	[UserGuid]
		,	[AssignedEquipmentGuid]
		,	[_MasterRecordGuid]
		)
		VALUES
		(
			@PersonnelGuid
		,	@PersonID
		,	@CardNumber
		,	@FirstName
		,	@MiddleName
		,	@LastName
		,	@Title
		,	@Department
		,	@Address1
		,	@Address2
		,	@City
		,	@State
		,	@Zip
		,	@Country
		,	@Phone1
		,	@Phone2
		,	@AssignmentDate
		,	@SupervisionDate
		,	@SSAN
		,	@BirthDate
		,	@PayRate
		,	@LaborRate1
		,	@LaborRate2
		,	@LaborRate3
		,	@LaborRate4
		,	@Status
		,	@Email
		,	@ResponsibleOfficer
		,	@Shift
		,	@PINNumber
		,	@PINRequired
		,	@LockedOut
		,	@LockedOutReason
		,	@LockedOutDate
		,	@LastActivityDate
		,	@CardedIn
		,	@ShortCardNumber
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@OnFileSignature
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@UserData9
		,	@UserData10
		,	@UserData11
		,	@UserData12
		,	@UserData13
		,	@UserData14
		,	@UserData15
		,	@UserData16
		,	@UserData17
		,	@UserData18
		,	@UserData19
		,	@UserData20
		,	@UserData21
		,	@UserData22
		,	@UserData23
		,	@UserData24
		,	@SiteGuid
		,	@CompanyGuid
		,	@SupervisorPersonnelGuid
		,	@UserGuid
		,	@AssignedEquipmentGuid
		,	@_MasterRecordGuid
		)
 
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
						+ 'Procedure Name: gsp_PersonnelInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
