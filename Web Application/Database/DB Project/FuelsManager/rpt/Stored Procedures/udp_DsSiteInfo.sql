
CREATE PROCEDURE [rpt].[usp_DsSiteInfo] 
(
	@SiteGuid uniqueidentifier
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsSiteInfo] 
	-- Author: Paul Carpenter
	-- Version/Date: 1/10/2013 4:38:00 PM 
	-- Purpose: Retrieve time stamp for the site group
	-- Notes:
	-- 1. @SiteGuid: Site/SiteGroup that report is being executed at for the purpose of retrieving proper units and decimal places.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

	DECLARE @SiteZoneDesc varchar(200)
	DECLARE @Format varchar(50)
	DECLARE @VolumeUnitName varchar(100)
	-- SELECT @Format = ShortDatePattern+ ' ' + TimePattern, @SiteZoneDesc = RTRIM(TimeZone)
	SELECT @Format = 'dd-MMM-yy hh:mm:ss tt', @SiteZoneDesc = RTRIM(TimeZone)
	,@VolumeUnitName = eu.EngineeringUnitName
	FROM tblSites s
	INNER JOIN lookup.tblEngineeringUnit eu on eu.EngineeringUnitIndex=s.VolumeUnitIndex
	WHERE SiteGuid=@SiteGuid

	DECLARE @RetTable TABLE
	(	
		SiteTime		 datetime
		,FormattedSiteTime varchar(200)
		,VolumeUnitName varchar(100)
	)

	INSERT INTO @RetTable
		SELECT TOP 1 
		    DATEADD( MINUTE, tz.OffsetMinutes, GetUTCDate())
		   , FORMAT( DATEADD( MINUTE, tz.OffsetMinutes, GetUTCDate()) , @Format) + ' ' + @SiteZoneDesc	
		   , @VolumeUnitName
		FROM lookup.tbltimezone tz		
		WHERE tz.TimeZoneName=@SiteZoneDesc

	IF( @@ROWCOUNT=0)
	BEGIN
		INSERT INTO @RetTable
			SELECT TOP 1  
			   DATEADD( MINUTE, tz.OffsetMinutes, GetUTCDate())
			   , FORMAT( DATEADD( MINUTE, tz.OffsetMinutes, GetUTCDate()) , @Format) + ' ' + 'GMT'
			   , @VolumeUnitName
			FROM lookup.tbltimezone tz
			WHERE tz.TimeZoneName = 'GMT Standard Time'
	END

	SELECT * FROM @RetTable
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
						+ 'Procedure Name: [rpt].usp_DsSiteInfo' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END