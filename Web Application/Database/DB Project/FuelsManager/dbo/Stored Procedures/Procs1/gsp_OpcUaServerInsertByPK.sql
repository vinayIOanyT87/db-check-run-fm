CREATE PROCEDURE [dbo].[gsp_OpcUaServerInsertByPK]
(
		@OpcUaServerGuid uniqueidentifier=NULL OUTPUT
	,	@ServerEndPoint nvarchar(250)=NULL
	,	@SecurityMode nvarchar(50)=NULL
	,	@SecurityPolicy nvarchar(50)=NULL
	,	@MessageEncoding nvarchar(50)=NULL
	,	@UserIdentityMethod nvarchar(50)=NULL
	,	@UserId nvarchar(250)=NULL
	,	@UserPassword nvarchar(250)=NULL
	,	@UserCertificatePath nvarchar(250)=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_ClusterIdx bigint=NULL OUTPUT
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_OpcUaServerInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2016-07-01 19:14:23.4873475 -04:00
	-- Purpose: Insert into table [dbo].[tblOpcUaServer]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @OpcUaServerGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblOpcUaServer] 
		(
			[OpcUaServerGuid]
		,	[ServerEndPoint]
		,	[SecurityMode]
		,	[SecurityPolicy]
		,	[MessageEncoding]
		,	[UserIdentityMethod]
		,	[UserId]
		,	[UserPassword]
		,	[UserCertificatePath]
		,	[SiteGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@OpcUaServerGuid
		,	@ServerEndPoint
		,	@SecurityMode
		,	@SecurityPolicy
		,	@MessageEncoding
		,	@UserIdentityMethod
		,	@UserId
		,	@UserPassword
		,	@UserCertificatePath
		,	@SiteGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion = _RowVersion,@_ClusterIdx = _ClusterIdx        
		FROM [dbo].[tblOpcUaServer]           
		WHERE OpcUaServerGuid=@OpcUaServerGuid;
	
 
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
						+ 'Procedure Name: gsp_OpcUaServerInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
GO
 