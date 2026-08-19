CREATE PROCEDURE [dbo].[gsp_SyncClientConfigurationInsertByPK]
(
		@SyncClientConfigurationGuid uniqueidentifier=NULL OUTPUT
	,	@RootSiteID nvarchar(30)=NULL
	,	@EnterpriseURL nvarchar(1024)=NULL
	,	@SuspendSynchronizationFlag bit=NULL
	,	@ServerAuthUserName nvarchar(256)=NULL
	,	@ServerAuthPassword varbinary=NULL
	,	@ServerAuthDomain nvarchar(256)=NULL
	,	@ServerAuthClientCertificate nvarchar(768)=NULL
	,	@FMAuthUserName udtUserID=NULL
	,	@FMAuthPassword varbinary=NULL
	,	@FMAuthClientCertificate nvarchar(768)=NULL
	,	@MessageSecuritySigningCertificate nvarchar(768)=NULL
	,	@MessageSecurityOfflineEncryptionCertificate nvarchar(768)=NULL
	,	@MessageSecurityOfflineDecryptionCertificate nvarchar(768)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SyncClientConfigurationInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4872767 -05:00
	-- Purpose: Insert into table [dbo].[tblSyncClientConfiguration]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SyncClientConfigurationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSyncClientConfiguration] 
		(
			[SyncClientConfigurationGuid]
		,	[RootSiteID]
		,	[EnterpriseURL]
		,	[SuspendSynchronizationFlag]
		,	[ServerAuthUserName]
		,	[ServerAuthPassword]
		,	[ServerAuthDomain]
		,	[ServerAuthClientCertificate]
		,	[FMAuthUserName]
		,	[FMAuthPassword]
		,	[FMAuthClientCertificate]
		,	[MessageSecuritySigningCertificate]
		,	[MessageSecurityOfflineEncryptionCertificate]
		,	[MessageSecurityOfflineDecryptionCertificate]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@SyncClientConfigurationGuid
		,	@RootSiteID
		,	@EnterpriseURL
		,	@SuspendSynchronizationFlag
		,	@ServerAuthUserName
		,	@ServerAuthPassword
		,	@ServerAuthDomain
		,	@ServerAuthClientCertificate
		,	@FMAuthUserName
		,	@FMAuthPassword
		,	@FMAuthClientCertificate
		,	@MessageSecuritySigningCertificate
		,	@MessageSecurityOfflineEncryptionCertificate
		,	@MessageSecurityOfflineDecryptionCertificate
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSyncClientConfiguration]           
		WHERE SyncClientConfigurationGuid=@SyncClientConfigurationGuid;
	
 
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
						+ 'Procedure Name: gsp_SyncClientConfigurationInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
