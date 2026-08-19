CREATE PROCEDURE [dbo].[usp_ExternalGasboyStationGet]
	@IdentityGuid UNIQUEIDENTIFIER = NULL,
	@ID NVARCHAR(50) = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@SiteCode NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

		IF (@IdentityGuid IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record by its primary key
			SELECT 
				[dbo].[tblExternalStation].ExternalStationGuid, 
				[dbo].[tblExternalStation].SiteGuid, 
				[dbo].[tblExternalStation].ID, 
				[dbo].[tblExternalStation].LookupExternalStationTypeIndex, 
				[dbo].[tblExternalGasboyStation].SiteCode, 
				[dbo].[tblExternalGasboyStation].IPAddress, 
				[dbo].[tblExternalGasboyStation].UserName, 
				[dbo].[tblExternalGasboyStation].[Password], 
				[dbo].[tblExternalStation].BillingID, 
				[dbo].[tblExternalStation].DownloadTransactionsAutomatically, 
				[dbo].[tblExternalStation].LookupExternalStationStatusIndex, 
				[dbo].[tblExternalStation].LastSuccessfulConnection, 
				[dbo].[tblExternalStation].LastConnectionAttempt, 
				[dbo].[tblExternalStation].LastTransactionID, 
				[dbo].[tblExternalStation].LastDeviceCount, 
				[dbo].[tblExternalGasboyStation].CreatedBy,
				[dbo].[tblExternalGasboyStation].CreatedDate,
				[dbo].[tblExternalGasboyStation].UpdatedBy,
				[dbo].[tblExternalGasboyStation].UpdatedDate
			FROM [dbo].[tblExternalStation] 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].ExternalStationGuid = [dbo].[tblExternalGasboyStation].ExternalStationGuid
			WHERE [dbo].[tblExternalStation].ExternalStationGuid = @IdentityGuid
					AND [dbo].[tblExternalStation].[LookupExternalStationTypeIndex] = 0 -- Gasboy
		END
		ELSE 
		IF (@ID IS NOT NULL)
		BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblExternalStation].ExternalStationGuid, 
				[dbo].[tblExternalStation].SiteGuid, 
				[dbo].[tblExternalStation].ID, 
				[dbo].[tblExternalStation].LookupExternalStationTypeIndex, 
				[dbo].[tblExternalGasboyStation].SiteCode, 
				[dbo].[tblExternalGasboyStation].IPAddress, 
				[dbo].[tblExternalGasboyStation].UserName, 
				[dbo].[tblExternalGasboyStation].[Password], 
				[dbo].[tblExternalStation].BillingID, 
				[dbo].[tblExternalStation].DownloadTransactionsAutomatically, 
				[dbo].[tblExternalStation].LookupExternalStationStatusIndex, 
				[dbo].[tblExternalStation].LastSuccessfulConnection, 
				[dbo].[tblExternalStation].LastConnectionAttempt, 
				[dbo].[tblExternalStation].LastTransactionID, 
				[dbo].[tblExternalStation].LastDeviceCount, 
				[dbo].[tblExternalGasboyStation].CreatedBy,
				[dbo].[tblExternalGasboyStation].CreatedDate,
				[dbo].[tblExternalGasboyStation].UpdatedBy,
				[dbo].[tblExternalGasboyStation].UpdatedDate
			FROM [dbo].[tblExternalStation] 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalGasboyStation].[ExternalStationGuid]
					INNER JOIN [map].[tblEntityExternalStationToSite]
						ON [map].[tblEntityExternalStationToSite].[ExternalStationGuid] = [dbo].[tblExternalGasboyStation].[ExternalStationGuid]
			WHERE [map].[tblEntityExternalStationToSite].[SiteGuid] = @SiteGuid 
					AND [dbo].[tblExternalStation].[ID] = @ID
					AND [dbo].[tblExternalStation].[LookupExternalStationTypeIndex] = 0 -- Gasboy
		END
		ELSE 
		IF (@SiteCode IS NOT NULL)
				BEGIN
			-- Retrieve the External Station record matching the provided ID 
			-- that is owned by or assigned to the site provided.
			SELECT 
				[dbo].[tblExternalStation].ExternalStationGuid, 
				[dbo].[tblExternalStation].SiteGuid, 
				[dbo].[tblExternalStation].ID, 
				[dbo].[tblExternalStation].LookupExternalStationTypeIndex, 
				[dbo].[tblExternalGasboyStation].SiteCode, 
				[dbo].[tblExternalGasboyStation].IPAddress, 
				[dbo].[tblExternalGasboyStation].UserName, 
				[dbo].[tblExternalGasboyStation].[Password], 
				[dbo].[tblExternalStation].BillingID, 
				[dbo].[tblExternalStation].DownloadTransactionsAutomatically, 
				[dbo].[tblExternalStation].LookupExternalStationStatusIndex, 
				[dbo].[tblExternalStation].LastSuccessfulConnection, 
				[dbo].[tblExternalStation].LastConnectionAttempt, 
				[dbo].[tblExternalStation].LastTransactionID, 
				[dbo].[tblExternalStation].LastDeviceCount, 
				[dbo].[tblExternalGasboyStation].CreatedBy,
				[dbo].[tblExternalGasboyStation].CreatedDate,
				[dbo].[tblExternalGasboyStation].UpdatedBy,
				[dbo].[tblExternalGasboyStation].UpdatedDate
			FROM [dbo].[tblExternalStation] 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].[ExternalStationGuid] = [dbo].[tblExternalGasboyStation].[ExternalStationGuid]
					INNER JOIN [map].[tblEntityExternalStationToSite]
						ON [map].[tblEntityExternalStationToSite].[ExternalStationGuid] = [dbo].[tblExternalGasboyStation].[ExternalStationGuid]
			WHERE --[map].[tblEntityExternalStationToSite].[SiteGuid] = @SiteGuid 
					[dbo].[tblExternalGasboyStation].[SiteCode] = @SiteCode
					AND [dbo].[tblExternalStation].[LookupExternalStationTypeIndex] = 0 -- Gasboy
			END

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
						+ 'Procedure Name: usp_ExternalGasboyStationGet' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END