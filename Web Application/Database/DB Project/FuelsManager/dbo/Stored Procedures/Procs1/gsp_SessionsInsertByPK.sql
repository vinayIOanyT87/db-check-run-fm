CREATE PROCEDURE [dbo].[gsp_SessionsInsertByPK]
(
		@SessionGuid uniqueidentifier=NULL OUTPUT
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@Timeout int=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@LoginSiteGuid uniqueidentifier=NULL
	,	@UserGuid uniqueidentifier=NULL
	,	@SqlServerSessionID int=NULL
	,	@SynchronizationNodeGuid uniqueidentifier=NULL
	,	@ClientIpAddress nvarchar(50)=NULL
	,	@WebServerName nvarchar(500)=NULL
	,	@WebServerIpAddress nvarchar(50)=NULL
	,	@SessionTokenID uniqueidentifier=NULL
	,	@SessionFailedFlag bit=NULL
	,	@CSRFToken nvarchar(256)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SessionsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4332767 -05:00
	-- Purpose: Insert into table [dbo].[tblSessions]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SessionGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSessions] 
		(
			[SessionGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[Timeout]
		,	[SiteGuid]
		,	[LoginSiteGuid]
		,	[UserGuid]
		,	[SqlServerSessionID]
		,	[SynchronizationNodeGuid]
		,	[ClientIpAddress]
		,	[WebServerName]
		,	[WebServerIpAddress]
		,	[SessionTokenID]
		,	[SessionFailedFlag]
		,	[CSRFToken]
		)
		VALUES
		(
			@SessionGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@Timeout
		,	@SiteGuid
		,	@LoginSiteGuid
		,	@UserGuid
		,	@SqlServerSessionID
		,	@SynchronizationNodeGuid
		,	@ClientIpAddress
		,	@WebServerName
		,	@WebServerIpAddress
		,	@SessionTokenID
		,	@SessionFailedFlag
		,	@CSRFToken
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSessions]           
		WHERE SessionGuid=@SessionGuid;
	
 
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
						+ 'Procedure Name: gsp_SessionsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
