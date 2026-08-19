CREATE PROCEDURE [dbo].[usp_ExternalStationEnumerate]
	@SiteGuid UNIQUEIDENTIFIER,
	@ID NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY

	--DECLARE @isEnterprise bit;
	--SELECT @isEnterprise= SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'IsEnterprise' 

	If (@SiteGuid = '00000000-0000-0000-0000-000000000001') --if SiteAdmin, iterate through all the stations present
			IF (@ID IS NOT NULL)
					BEGIN
			-- If the ID is provided, retrieve all external stations
			-- assigned to or owned by the site which partially match the id 
			SELECT 
				tblExternalStation.ExternalStationGuid,
				tblExternalStation.ID,
				tblExternalStation.SiteGuid,
				tblExternalStation.BillingID,
                tblExternalStation.LookupExternalStationTypeIndex,
				tblExternalStation.LookupExternalStationStatusIndex,
				tblExternalStation.LastConnectionAttempt,
				tblExternalStation.LastSuccessfulConnection,
				tblExternalStation.LastTransactionID,
				tblExternalStation.LastDeviceCount,
				tblExternalStation.DownloadTransactionsAutomatically,
				tblExternalGasboyStation.SiteCode, 
				tblExternalGasboyStation.IPAddress, 
				tblExternalGasboyStation.UserName, 
				tblExternalGasboyStation.Password
			FROM tblExternalStation 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].ExternalStationGuid = [dbo].[tblExternalGasboyStation].ExternalStationGuid
			WHERE EXISTS (SELECT * FROM map.tblEntityExternalStationToSite 
				-- WHERE map.tblEntityExternalStationToSite.SiteGuid = @SiteGuid
					WHERE map.tblEntityExternalStationToSite.ExternalStationGuid = tblExternalStation.ExternalStationGuid)
			AND (tblExternalStation.ID LIKE ('%' + @ID + '%') OR tblExternalStation.BillingID LIKE ('%' + @ID + '%'))
			ORDER BY tblExternalStation.ID
			END
		ELSE
			BEGIN
			-- Retrieve all external stations
			-- assigned to or owned by the site which partially match the id
			SELECT 
				tblExternalStation.ExternalStationGuid,
				tblExternalStation.ID,
				tblExternalStation.SiteGuid,
				tblExternalStation.BillingID,
                tblExternalStation.LookupExternalStationTypeIndex,
				tblExternalStation.LookupExternalStationStatusIndex,
				tblExternalStation.LastConnectionAttempt,
				tblExternalStation.LastSuccessfulConnection,
				tblExternalStation.LastTransactionID,
				tblExternalStation.LastDeviceCount,
				tblExternalStation.DownloadTransactionsAutomatically,
				tblExternalGasboyStation.SiteCode, 
				tblExternalGasboyStation.IPAddress, 
				tblExternalGasboyStation.UserName, 
				tblExternalGasboyStation.Password
			FROM tblExternalStation 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].ExternalStationGuid = [dbo].[tblExternalGasboyStation].ExternalStationGuid
			WHERE EXISTS (SELECT * FROM map.tblEntityExternalStationToSite 
				-- WHERE map.tblEntityExternalStationToSite.SiteGuid = @SiteGuid
				WHERE map.tblEntityExternalStationToSite.ExternalStationGuid = tblExternalStation.ExternalStationGuid)
			ORDER BY tblExternalStation.ID
			END
	ELSE --if not Enterprise, only return from the site given
		IF (@ID IS NOT NULL)
		BEGIN
			-- If the ID is provided, retrieve all external stations
			-- assigned to or owned by the site which partially match the id 
			SELECT 
				tblExternalStation.ExternalStationGuid,
				tblExternalStation.ID,
				tblExternalStation.SiteGuid,
				tblExternalStation.BillingID,
                tblExternalStation.LookupExternalStationTypeIndex,
				tblExternalStation.LookupExternalStationStatusIndex,
				tblExternalStation.LastConnectionAttempt,
				tblExternalStation.LastSuccessfulConnection,
				tblExternalStation.LastTransactionID,
				tblExternalStation.LastDeviceCount,
				tblExternalStation.DownloadTransactionsAutomatically,
				tblExternalGasboyStation.SiteCode, 
				tblExternalGasboyStation.IPAddress, 
				tblExternalGasboyStation.UserName, 
				tblExternalGasboyStation.Password
			FROM tblExternalStation 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].ExternalStationGuid = [dbo].[tblExternalGasboyStation].ExternalStationGuid
			WHERE EXISTS (SELECT * FROM map.tblEntityExternalStationToSite 
				 WHERE map.tblEntityExternalStationToSite.SiteGuid = @SiteGuid
					AND map.tblEntityExternalStationToSite.ExternalStationGuid = tblExternalStation.ExternalStationGuid)
			AND (tblExternalStation.ID LIKE ('%' + @ID + '%') OR tblExternalStation.BillingID LIKE ('%' + @ID + '%'))
			ORDER BY tblExternalStation.ID
		END
		ELSE
		BEGIN
			-- Retrieve all external stations
			-- assigned to or owned by the site which partially match the id
			SELECT 
				tblExternalStation.ExternalStationGuid,
				tblExternalStation.ID,
				tblExternalStation.SiteGuid,
				tblExternalStation.BillingID,
                tblExternalStation.LookupExternalStationTypeIndex,
				tblExternalStation.LookupExternalStationStatusIndex,
				tblExternalStation.LastConnectionAttempt,
				tblExternalStation.LastSuccessfulConnection,
				tblExternalStation.LastTransactionID,
				tblExternalStation.LastDeviceCount,
				tblExternalStation.DownloadTransactionsAutomatically,
				tblExternalGasboyStation.SiteCode, 
				tblExternalGasboyStation.IPAddress, 
				tblExternalGasboyStation.UserName, 
				tblExternalGasboyStation.Password
			FROM tblExternalStation 
					INNER JOIN [dbo].[tblExternalGasboyStation]
						ON [dbo].[tblExternalStation].ExternalStationGuid = [dbo].[tblExternalGasboyStation].ExternalStationGuid
			WHERE EXISTS (SELECT * FROM map.tblEntityExternalStationToSite 
				WHERE map.tblEntityExternalStationToSite.SiteGuid = @SiteGuid
					AND map.tblEntityExternalStationToSite.ExternalStationGuid = tblExternalStation.ExternalStationGuid)
			ORDER BY tblExternalStation.ID
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
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13) + CHAR(10)                 
						+ 'Procedure Name: usp_ExternalStationEnumerate' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END