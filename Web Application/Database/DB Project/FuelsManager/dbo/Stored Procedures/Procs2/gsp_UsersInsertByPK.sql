CREATE PROCEDURE [dbo].[gsp_UsersInsertByPK]
(
		@UserGuid uniqueidentifier=NULL OUTPUT
	,	@UserID udtUserID=NULL
	,	@Password varbinary=NULL
	,	@LastLoginDate datetimeoffset(7)=NULL
	,	@LastLogoffDate datetimeoffset(7)=NULL
	,	@ChangePassword bit=NULL
	,	@PasswordTimeStamp datetimeoffset(7)=NULL
	,	@Name nvarchar(50)=NULL
	,	@EmailAddress nvarchar(50)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@PasswordHistory1 varbinary=NULL
	,	@PasswordHistory2 varbinary=NULL
	,	@PasswordHistory3 varbinary=NULL
	,	@PasswordHistory4 varbinary=NULL
	,	@PasswordHistory5 varbinary=NULL
	,	@PasswordHistory6 varbinary=NULL
	,	@PasswordHistory7 varbinary=NULL
	,	@PasswordHistory8 varbinary=NULL
	,	@PasswordHistory9 varbinary=NULL
	,	@PasswordHistory10 varbinary=NULL
	,	@PasswordHistory11 varbinary=NULL
	,	@PasswordHistory12 varbinary=NULL
	,	@PasswordHistory13 varbinary=NULL
	,	@PasswordHistory14 varbinary=NULL
	,	@PasswordHistory15 varbinary=NULL
	,	@PasswordHistory16 varbinary=NULL
	,	@PasswordHistory17 varbinary=NULL
	,	@PasswordHistory18 varbinary=NULL
	,	@PasswordHistory19 varbinary=NULL
	,	@PasswordHistory20 varbinary=NULL
	,	@PasswordHistory21 varbinary=NULL
	,	@PasswordHistory22 varbinary=NULL
	,	@PasswordHistory23 varbinary=NULL
	,	@PasswordHistory24 varbinary=NULL
	,	@PasswordLockoutCount int=NULL
	,	@InactivityLockout bit=NULL
	,	@InactivityLockoutDate datetimeoffset(7)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@PasswordHint varchar(40)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_UsersInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.6222767 -05:00
	-- Purpose: Insert into table [dbo].[tblUsers]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @UserGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblUsers] 
		(
			[UserGuid]
		,	[UserID]
		,	[Password]
		,	[LastLoginDate]
		,	[LastLogoffDate]
		,	[ChangePassword]
		,	[PasswordTimeStamp]
		,	[Name]
		,	[EmailAddress]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PasswordHistory1]
		,	[PasswordHistory2]
		,	[PasswordHistory3]
		,	[PasswordHistory4]
		,	[PasswordHistory5]
		,	[PasswordHistory6]
		,	[PasswordHistory7]
		,	[PasswordHistory8]
		,	[PasswordHistory9]
		,	[PasswordHistory10]
		,	[PasswordHistory11]
		,	[PasswordHistory12]
		,	[PasswordHistory13]
		,	[PasswordHistory14]
		,	[PasswordHistory15]
		,	[PasswordHistory16]
		,	[PasswordHistory17]
		,	[PasswordHistory18]
		,	[PasswordHistory19]
		,	[PasswordHistory20]
		,	[PasswordHistory21]
		,	[PasswordHistory22]
		,	[PasswordHistory23]
		,	[PasswordHistory24]
		,	[PasswordLockoutCount]
		,	[InactivityLockout]
		,	[InactivityLockoutDate]
		,	[SiteGuid]
		,	[PasswordHint]
		)
		VALUES
		(
			@UserGuid
		,	@UserID
		,	@Password
		,	@LastLoginDate
		,	@LastLogoffDate
		,	@ChangePassword
		,	@PasswordTimeStamp
		,	@Name
		,	@EmailAddress
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@PasswordHistory1
		,	@PasswordHistory2
		,	@PasswordHistory3
		,	@PasswordHistory4
		,	@PasswordHistory5
		,	@PasswordHistory6
		,	@PasswordHistory7
		,	@PasswordHistory8
		,	@PasswordHistory9
		,	@PasswordHistory10
		,	@PasswordHistory11
		,	@PasswordHistory12
		,	@PasswordHistory13
		,	@PasswordHistory14
		,	@PasswordHistory15
		,	@PasswordHistory16
		,	@PasswordHistory17
		,	@PasswordHistory18
		,	@PasswordHistory19
		,	@PasswordHistory20
		,	@PasswordHistory21
		,	@PasswordHistory22
		,	@PasswordHistory23
		,	@PasswordHistory24
		,	@PasswordLockoutCount
		,	@InactivityLockout
		,	@InactivityLockoutDate
		,	@SiteGuid
		,	@PasswordHint
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblUsers]           
		WHERE UserGuid=@UserGuid;
	
 
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
						+ 'Procedure Name: gsp_UsersInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
