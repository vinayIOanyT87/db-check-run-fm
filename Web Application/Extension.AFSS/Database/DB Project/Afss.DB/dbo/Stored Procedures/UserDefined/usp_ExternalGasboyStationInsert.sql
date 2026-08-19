CREATE PROCEDURE [dbo].[usp_ExternalGasboyStationInsert]
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
	@LastSuccessfulConnection DATETIMEOFFSET(7) = NULL, 
	@LastConnectionAttempt DATETIMEOFFSET(7) = NULL, 
	@LastTransactionID BIGINT = NULL, 
	@LastDeviceCount INT = NULL,
	@CreatedUpdatedBy dbo.udtUserID
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @InsertDateTimeOffset DATETIMEOFFSET(7)

	SET @InsertDateTimeOffset = SYSDATETIMEOFFSET();

	BEGIN TRY

		INSERT INTO tblExternalStation
		(
			ExternalStationGuid, 
			SiteGuid, 
			ID, 
			BillingID, 
			LookupExternalStationTypeIndex,
			DownloadTransactionsAutomatically, 		 
			LookupExternalStationStatusIndex, 
			LastSuccessfulConnection, 
			LastConnectionAttempt, 
			LastTransactionID, 
			LastDeviceCount, 
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		VALUES
		(
			@IdentityGuid, 
			@SiteGuid, 
			@ID, 
			@BillingID, 
			0, -- Gasboy Station
			@DownloadTransactionsAutomatically, 
			@LookupExternalStationStatusIndex, 
			@LastSuccessfulConnection, 
			@LastConnectionAttempt, 
			@LastTransactionID, 
			@LastDeviceCount,
			@CreatedUpdatedBy,
			@InsertDateTimeOffset,
			@CreatedUpdatedBy,
			@InsertDateTimeOffset
		)

		INSERT INTO tblExternalGasboyStation
		(
			ExternalStationGuid, 
			SiteCode, 
			IPAddress, 
			UserName, 
			[Password], 
			CreatedBy,
			CreatedDate,
			UpdatedBy,
			UpdatedDate
		)
		VALUES
		(
			@IdentityGuid, 
			@SiteCode, 
			@IPAddress, 
			@UserName, 
			@Password, 
			@CreatedUpdatedBy,
			@InsertDateTimeOffset,
			@CreatedUpdatedBy,
			@InsertDateTimeOffset
		)

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
						+ 'Procedure Name: usp_ExternalGasboyStationInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH
END
