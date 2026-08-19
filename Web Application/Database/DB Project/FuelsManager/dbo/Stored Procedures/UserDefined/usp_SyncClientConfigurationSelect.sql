
CREATE PROCEDURE [dbo].[usp_SyncClientConfigurationSelect](
	@IdentityGuid uniqueidentifier = NULL
)
AS
BEGIN
	BEGIN TRY
		IF @IdentityGuid IS NULL
		BEGIN
			SELECT TOP(1) @IdentityGuid = SyncClientConfigurationGuid FROM [dbo].[tblSyncClientConfiguration] WITH (NOLOCK)
		END

		SELECT SyncClientConfigurationGuid
				,RootSiteID
				,EnterpriseURL
				,SuspendSynchronizationFlag
				,ServerAuthUserName
				,ServerAuthPassword
				,ServerAuthDomain
				,ServerAuthClientCertificate
				,FMAuthUserName
				,FMAuthPassword
				,FMAuthClientCertificate
				,MessageSecuritySigningCertificate
				,MessageSecurityOfflineEncryptionCertificate
				,MessageSecurityOfflineDecryptionCertificate
				,ServiceMaximumRetryAttempts
				,ServiceRetryWaitTime
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy
				,_RowVersion
				FROM [dbo].[tblSyncClientConfiguration] WITH (NOLOCK)
				WHERE (@IdentityGuid IS NOT NULL AND SyncClientConfigurationGuid = @IdentityGuid)
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
						+ 'Procedure Name: usp_SyncClientConfigurationSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END