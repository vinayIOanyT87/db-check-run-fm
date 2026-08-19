CREATE PROCEDURE [dbo].[gsp_PIDXProfilesInsertByPK]
(
		@PIDXProfileGuid uniqueidentifier=NULL OUTPUT
	,	@Type tinyint=NULL
	,	@ID nvarchar(30)=NULL
	,	@IPAddress nvarchar(60)=NULL
	,	@Port int=NULL
	,	@TerminalID nvarchar(30)=NULL
	,	@UserID nvarchar(30)=NULL
	,	@Password nvarchar(30)=NULL
	,	@Enabled bit=NULL
	,	@LoggingEnabled bit=NULL
	,	@LogFilePath nvarchar(255)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PIDXProfilesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3202767 -05:00
	-- Purpose: Insert into table [dbo].[tblPIDXProfiles]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PIDXProfileGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPIDXProfiles] 
		(
			[PIDXProfileGuid]
		,	[Type]
		,	[ID]
		,	[IPAddress]
		,	[Port]
		,	[TerminalID]
		,	[UserID]
		,	[Password]
		,	[Enabled]
		,	[LoggingEnabled]
		,	[LogFilePath]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		)
		VALUES
		(
			@PIDXProfileGuid
		,	@Type
		,	@ID
		,	@IPAddress
		,	@Port
		,	@TerminalID
		,	@UserID
		,	@Password
		,	@Enabled
		,	@LoggingEnabled
		,	@LogFilePath
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPIDXProfiles]           
		WHERE PIDXProfileGuid=@PIDXProfileGuid;
	
 
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
						+ 'Procedure Name: gsp_PIDXProfilesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
