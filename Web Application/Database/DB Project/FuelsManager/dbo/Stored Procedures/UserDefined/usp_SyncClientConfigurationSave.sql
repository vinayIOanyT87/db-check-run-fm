
CREATE PROCEDURE [dbo].[usp_SyncClientConfigurationSave](
	@IdentityGuid uniqueidentifier = NULL
	,@RootSiteID nvarchar(30)
	,@EnterpriseURL nvarchar(1024)
	,@SuspendSynchronizationFlag bit
	,@ServerAuthUserName nvarchar(256)
	,@ServerAuthPassword varbinary(256)
	,@ServerAuthDomain nvarchar(256)
	,@ServerAuthClientCertificate nvarchar(768)
	,@FMAuthUserName udtUserID
	,@FMAuthPassword varbinary(256)
	,@FMAuthClientCertificate nvarchar(768)
	,@MessageSecuritySigningCertificate nvarchar(768)
	,@MessageSecurityOfflineEncryptionCertificate nvarchar(768)
	,@MessageSecurityOfflineDecryptionCertificate nvarchar(768)
	,@ServiceMaximumRetryAttempts int
	,@ServiceRetryWaitTime int
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;

	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ;   
    MERGE [dbo].[tblSyncClientConfiguration] AS existing
    USING (SELECT @NewRowGuid
					,@RootSiteID
					,@EnterpriseURL
					,@SuspendSynchronizationFlag
					,@ServerAuthUserName
					,@ServerAuthPassword
					,@ServerAuthDomain
					,@ServerAuthClientCertificate
					,@FMAuthUserName
					,@FMAuthPassword
					,@FMAuthClientCertificate
					,@MessageSecuritySigningCertificate
					,@MessageSecurityOfflineEncryptionCertificate
					,@MessageSecurityOfflineDecryptionCertificate
					,@ServiceMaximumRetryAttempts
					,@ServiceRetryWaitTime
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncClientConfigurationGuid
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
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncClientConfigurationGuid = updates.SyncClientConfigurationGuid)
    WHEN Matched
    THEN
		UPDATE SET RootSiteID = updates.RootSiteID
					,EnterpriseURL = updates.EnterpriseURL
					,SuspendSynchronizationFlag = updates.SuspendSynchronizationFlag
					,ServerAuthUserName = updates.ServerAuthUserName
					,ServerAuthPassword = updates.ServerAuthPassword
					,ServerAuthDomain = updates.ServerAuthDomain
					,ServerAuthClientCertificate = updates.ServerAuthClientCertificate
					,FMAuthUserName = updates.FMAuthUserName
					,FMAuthPassword = updates.FMAuthPassword
					,FMAuthClientCertificate = updates.FMAuthClientCertificate
					,MessageSecuritySigningCertificate = updates.MessageSecuritySigningCertificate
					,MessageSecurityOfflineEncryptionCertificate = updates.MessageSecurityOfflineEncryptionCertificate
					,MessageSecurityOfflineDecryptionCertificate = updates.MessageSecurityOfflineDecryptionCertificate
					,ServiceMaximumRetryAttempts = updates.ServiceMaximumRetryAttempts
					,ServiceRetryWaitTime = updates.ServiceRetryWaitTime
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncClientConfigurationGuid
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
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@RootSiteID
					,@EnterpriseURL
					,@SuspendSynchronizationFlag
					,@ServerAuthUserName
					,@ServerAuthPassword
					,@ServerAuthDomain
					,@ServerAuthClientCertificate
					,@FMAuthUserName
					,@FMAuthPassword
					,@FMAuthClientCertificate
					,@MessageSecuritySigningCertificate
					,@MessageSecurityOfflineEncryptionCertificate
					,@MessageSecurityOfflineDecryptionCertificate
					,@ServiceMaximumRetryAttempts
					,@ServiceRetryWaitTime
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END