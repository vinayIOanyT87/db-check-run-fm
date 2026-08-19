
CREATE PROCEDURE [dbo].[usp_SyncServerConfigurationSave](
	@IdentityGuid uniqueidentifier = NULL
	,@AllowSynchronizationFlag bit
	,@AcceptFMUserAuthenticationFlag bit
	,@AcceptClientCertificateAuthenticationFlag bit
	,@ClientSignatureRequiredForMessagesFlag bit
	,@ClientEncryptionRequiredForMessagesFlag bit
	,@NodeHealthCriticalThresholdHours int
	,@NodeHealthCautionThresholdHours int
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
    MERGE [dbo].[tblSyncServerConfiguration] AS existing
    USING (SELECT @NewRowGuid
					,@AllowSynchronizationFlag
					,@AcceptFMUserAuthenticationFlag
					,@AcceptClientCertificateAuthenticationFlag
					,@ClientSignatureRequiredForMessagesFlag
					,@ClientEncryptionRequiredForMessagesFlag
					,@NodeHealthCriticalThresholdHours
					,@NodeHealthCautionThresholdHours
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncServerConfigurationGuid
							,AllowSynchronizationFlag
							,AcceptFMUserAuthenticationFlag
							,AcceptClientCertificateAuthenticationFlag
							,ClientSignatureRequiredForMessagesFlag
							,ClientEncryptionRequiredForMessagesFlag
							,NodeHealthCriticalThresholdHours
							,NodeHealthCautionThresholdHours
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncServerConfigurationGuid = updates.SyncServerConfigurationGuid)
    WHEN Matched
    THEN
		UPDATE SET AllowSynchronizationFlag = updates.AllowSynchronizationFlag
					,AcceptFMUserAuthenticationFlag = updates.AcceptFMUserAuthenticationFlag
					,AcceptClientCertificateAuthenticationFlag = updates.AcceptClientCertificateAuthenticationFlag
					,ClientSignatureRequiredForMessagesFlag = updates.ClientSignatureRequiredForMessagesFlag
					,ClientEncryptionRequiredForMessagesFlag = updates.ClientEncryptionRequiredForMessagesFlag
					,NodeHealthCriticalThresholdHours = updates.NodeHealthCriticalThresholdHours
					,NodeHealthCautionThresholdHours = updates.NodeHealthCautionThresholdHours
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncServerConfigurationGuid
				,AllowSynchronizationFlag
				,AcceptFMUserAuthenticationFlag
				,AcceptClientCertificateAuthenticationFlag
				,ClientSignatureRequiredForMessagesFlag
				,ClientEncryptionRequiredForMessagesFlag
				,NodeHealthCriticalThresholdHours
				,NodeHealthCautionThresholdHours
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@AllowSynchronizationFlag
					,@AcceptFMUserAuthenticationFlag
					,@AcceptClientCertificateAuthenticationFlag
					,@ClientSignatureRequiredForMessagesFlag
					,@ClientEncryptionRequiredForMessagesFlag
					,@NodeHealthCriticalThresholdHours
					,@NodeHealthCautionThresholdHours
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END