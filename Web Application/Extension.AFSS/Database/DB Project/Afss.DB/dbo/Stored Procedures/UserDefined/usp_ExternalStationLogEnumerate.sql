CREATE PROCEDURE [dbo].[usp_ExternalStationLogEnumerate]
	@SiteGuid UNIQUEIDENTIFIER,
	@BeginDate DATETIMEOFFSET, 
	@EndDate DATETIMEOFFSET, 
	@LogType INT = NULL,
	@ExternalStationGuid UNIQUEIDENTIFIER = NULL
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		IF (@LogType IS NULL AND @ExternalStationGuid IS NULL)
		BEGIN
			SELECT 
				tblExternalStationLog.ExternalStationLogGuid,
				tblExternalStationLog.SiteGuid,  
				tblExternalStationLog.ExternalStationGuid, 
				CASE WHEN LEN(tblExternalStationLog.LogText) > 100 THEN LEFT(tblExternalStationLog.LogText, 100) + '...' 
					ELSE tblExternalStationLog.LogText END AS LogText, 
				tblExternalStationLog.LookupExternalStationLogTypeIndex, 			
				tblExternalStationLog.LogDate,
				tblExternalStation.ID AS ExternalStationID
			FROM tblExternalStationLog 
			INNER JOIN tblExternalStation ON tblExternalStationLog.ExternalStationGuid = tblExternalStation.ExternalStationGuid
			WHERE tblExternalStationLog.SiteGuid = @SiteGuid AND tblExternalStationLog.LogDate >= @BeginDate AND tblExternalStationLog.LogDate <= @EndDate
			ORDER BY tblExternalStationLog.LogDate DESC
		END
		ELSE IF (@LogType IS NOT NULL AND @ExternalStationGuid IS NOT NULL)
		BEGIN
			SELECT 
				tblExternalStationLog.ExternalStationLogGuid,
				tblExternalStationLog.SiteGuid,  
				tblExternalStationLog.ExternalStationGuid, 
				CASE WHEN LEN(tblExternalStationLog.LogText) > 100 THEN LEFT(tblExternalStationLog.LogText, 100) + '...' 
					ELSE tblExternalStationLog.LogText END AS LogText, 
				tblExternalStationLog.LookupExternalStationLogTypeIndex, 			
				tblExternalStationLog.LogDate,
				tblExternalStation.ID AS ExternalStationID		
			FROM tblExternalStationLog
			INNER JOIN tblExternalStation ON tblExternalStationLog.ExternalStationGuid = tblExternalStation.ExternalStationGuid
			WHERE tblExternalStationLog.SiteGuid = @SiteGuid AND tblExternalStationLog.LogDate >= @BeginDate AND tblExternalStationLog.LogDate <= @EndDate
				AND tblExternalStationLog.ExternalStationGuid = @ExternalStationGuid AND LookupExternalStationLogTypeIndex = @LogType
			ORDER BY tblExternalStationLog.LogDate DESC
		END
		ELSE IF (@LogType IS NOT NULL)
		BEGIN
			SELECT 
				tblExternalStationLog.ExternalStationLogGuid,
				tblExternalStationLog.SiteGuid,  
				tblExternalStationLog.ExternalStationGuid, 
				CASE WHEN LEN(tblExternalStationLog.LogText) > 100 THEN LEFT(tblExternalStationLog.LogText, 100) + '...' 
					ELSE tblExternalStationLog.LogText END AS LogText, 
				tblExternalStationLog.LookupExternalStationLogTypeIndex, 			
				tblExternalStationLog.LogDate,
				tblExternalStation.ID AS ExternalStationID		
			FROM tblExternalStationLog
			INNER JOIN tblExternalStation ON tblExternalStationLog.ExternalStationGuid = tblExternalStation.ExternalStationGuid
			WHERE tblExternalStationLog.SiteGuid = @SiteGuid AND tblExternalStationLog.LogDate >= @BeginDate AND tblExternalStationLog.LogDate <= @EndDate
				AND LookupExternalStationLogTypeIndex = @LogType
			ORDER BY tblExternalStationLog.LogDate DESC
		END
		ELSE 
		BEGIN
			SELECT 
				tblExternalStationLog.ExternalStationLogGuid,
				tblExternalStationLog.SiteGuid,  
				tblExternalStationLog.ExternalStationGuid, 
				CASE WHEN LEN(tblExternalStationLog.LogText) > 100 THEN LEFT(tblExternalStationLog.LogText, 100) + '...' 
					ELSE tblExternalStationLog.LogText END AS LogText, 
				tblExternalStationLog.LookupExternalStationLogTypeIndex, 			
				tblExternalStationLog.LogDate,
				tblExternalStation.ID AS ExternalStationID		
			FROM tblExternalStationLog
			INNER JOIN tblExternalStation ON tblExternalStationLog.ExternalStationGuid = tblExternalStation.ExternalStationGuid
			WHERE tblExternalStationLog.SiteGuid = @SiteGuid AND tblExternalStationLog.LogDate >= @BeginDate AND tblExternalStationLog.LogDate <= @EndDate
				AND tblExternalStationLog.ExternalStationGuid = @ExternalStationGuid 
			ORDER BY tblExternalStationLog.LogDate DESC
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
						+ 'Procedure Name: usp_ExternalStationLogEnumerate' + CHAR(13) + CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13) + CHAR(10);         
		RAISERROR(@_ErrMessage,18,1); 
	END CATCH

END
	