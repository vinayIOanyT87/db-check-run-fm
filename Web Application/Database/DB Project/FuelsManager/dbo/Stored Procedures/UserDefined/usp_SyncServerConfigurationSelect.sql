
CREATE PROCEDURE [dbo].[usp_SyncServerConfigurationSelect](
	@IdentityGuid uniqueidentifier = NULL
)
AS
BEGIN
	BEGIN TRY
		IF @IdentityGuid IS NULL
		BEGIN
			SELECT TOP(1) @IdentityGuid = SyncServerConfigurationGuid FROM [dbo].[tblSyncServerConfiguration] WITH (NOLOCK);
		END

		SELECT SyncServerConfigurationGuid
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
				,UpdatedBy
				,_RowVersion
				FROM [dbo].[tblSyncServerConfiguration] WITH (NOLOCK)
				WHERE (@IdentityGuid IS NOT NULL AND SyncServerConfigurationGuid = @IdentityGuid)
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
						+ 'Procedure Name: usp_SyncServerConfigurationSelect' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END