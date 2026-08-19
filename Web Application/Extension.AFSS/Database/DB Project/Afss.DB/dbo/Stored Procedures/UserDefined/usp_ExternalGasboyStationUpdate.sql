CREATE PROCEDURE [dbo].[usp_ExternalGasboyStationUpdate]
	@IdentityGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER, 
	@ID NVARCHAR(50), 
	@SiteCode NVARCHAR(6), 
	@IPAddress NVARCHAR(50) = NULL, 
	@UserName NVARCHAR(50) = NULL, 
	@Password VARBINARY(256) = NULL, 
	@BillingID NVARCHAR(50) = NULL, 
	@DownloadTransactionsAutomatically BIT, 
	@LookupExternalStationStatusIndex INT, 
	@LastSuccessfulConnection DATETIMEOFFSET = NULL, 
	@LastConnectionAttempt DATETIMEOFFSET = NULL, 
	@LastTransactionID BIGINT = NULL, 
	@LastDeviceCount INT = NULL,
	@UpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @UpdatedDate DATETIMEOFFSET(7);
		SET @UpdatedDate = SYSDATETIMEOFFSET();

		UPDATE [dbo].[tblExternalStation]
		SET ID = @ID,
			SiteGuid = @SiteGuid,
			BillingID = @BillingID, 
			DownloadTransactionsAutomatically = @DownloadTransactionsAutomatically, 
			LookupExternalStationStatusIndex = @LookupExternalStationStatusIndex, 
			LastSuccessfulConnection = @LastSuccessfulConnection, 
			LastConnectionAttempt = @LastConnectionAttempt, 
			LastTransactionID = @LastTransactionID, 
			LastDeviceCount = @LastDeviceCount,
			UpdatedBy = @UpdatedBy,
			UpdatedDate = @UpdatedDate
		WHERE ExternalStationGuid = @IdentityGuid

		UPDATE tblExternalGasboyStation
		SET SiteCode = @SiteCode, 
			IPAddress = @IPAddress, 
			UserName = @UserName, 
			[Password] = @Password, 
			UpdatedBy = @UpdatedBy,
			UpdatedDate = @UpdatedDate
		WHERE ExternalStationGuid = @IdentityGuid

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
						+ 'Procedure Name: usp_ExternalGasboyStationUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
