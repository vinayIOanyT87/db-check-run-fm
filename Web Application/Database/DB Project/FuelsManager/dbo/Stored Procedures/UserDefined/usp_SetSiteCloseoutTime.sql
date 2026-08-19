CREATE  PROCEDURE [dbo].[usp_SetSiteCloseoutTime](
@SiteGuid UNIQUEIDENTIFIER, 
@CloseoutTime TIME,					-- previous closeout time that needs archiving. if blank, use the current site closeout time from the site (points changed)
@ExpirationDateTime DateTimeOffset,	--ignored
@UpdatedBy NVARCHAR(100),
@PointsChanged BIT
)
AS
BEGIN

	IF NOT EXISTS (SELECT 1 FROM tblSites WHERE SiteGuid = @SiteGuid)
		RETURN

	DECLARE @TIME235959 TIME = CONVERT(TIME, '23:59:59')
	BEGIN TRY
	    
      DECLARE @LastExpirationDate DateTimeOffset = ISNULL((SELECT Max(ExpirationDate) FROM tblSiteCloseoutTime WHERE SiteGuid=@SiteGuid), '1900-01-01')
		DECLARE @LastCloseout Time = ISNULL((SELECT CloseoutTime FROM tblSiteCloseoutTime WHERE SiteGuid=@SiteGuid AND ExpirationDate=@LastExpirationDate), '23:59:59')

		DECLARE @TargetCloseoutTime TIME

		IF @PointsChanged = 0
		SET @TargetCloseoutTime = ISNULL(@CloseoutTime, @TIME235959)
		ELSE
		SET @TargetCloseoutTime = ISNULL((select CloseoutTime FROM tblSites WHERE SiteGuid = @SiteGuid),@TIME235959) --if points changed, use the current closeout time

		DECLARE @SiteTimeZone as nvarchar(50)
		SET @SiteTimeZone = (select Timezone FROM tblSites WHERE SiteGuid = @SiteGuid)

		DECLARE @currentDateTime DateTimeOffset = SysDateTimeOffset() AT TIME ZONE @SiteTimeZone
		DECLARE @CurrentDate235959 DateTimeOffset = DATEADD(second,-1, CONVERT(DATETIME, DATEADD(DAY, 1, CONVERT(DATE, @currentDateTime)))  AT TIME ZONE @SiteTimeZone) 

		DECLARE @currentTime TIME =  CONVERT(TIME, @currentDateTime)  
		DECLARE @PreviousDate235959 DateTimeOffset = DATEADD(DAY,-1, @CurrentDate235959) 

		DECLARE @ExpirationDate235959 DateTimeOffset

		IF @currentTime > @TargetCloseoutTime
		BEGIN
			--closeout already happened for the current day
			SET @ExpirationDate235959 = @CurrentDate235959
		END
		ELSE
		BEGIN
			--closeout yet to happen for the current day
			SET @ExpirationDate235959 = @PreviousDate235959
		END

		DECLARE @PointTagRefDataAsXML XML

		SET @PointTagRefDataAsXML = rpt.udf_GetPointTagRefDataAsXML(@SiteGuid)

		IF EXISTS (SELECT TOP 1 1 FROM tblSiteCloseoutTime WHERE SiteGuid=@SiteGuid)
		BEGIN		
			IF @LastCloseout=@TargetCloseoutTime AND @PointsChanged = 0 -- need to do a full row insert if points changed
			BEGIN
				UPDATE tblSiteCloseoutTime SET ExpirationDate=@ExpirationDate235959 WHERE SiteGuid=@SiteGuid AND ExpirationDate=@LastExpirationDate
				RETURN
			END

			IF @LastExpirationDate >= @ExpirationDate235959
			BEGIN
				-- a closeout time, which already happend in current day, was already archived. Any more changes don't need archiving since they couldn't have been used.
				--closeout that is in tblSites will be started to be used starting next day.
				RETURN
			END
		END

		INSERT INTO tblSiteCloseoutTime (SiteCloseoutTimeGuid, EffectiveDate, ExpirationDate, CloseoutTime, PointTagRefDataAsXML, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES( NEWID(),  DATEADD(second, 1, @LastExpirationDate), @ExpirationDate235959, @TargetCloseoutTime, @PointTagRefDataAsXML, @SiteGuid, SYSDATETIMEOFFSET(), @UpdatedBy, SYSDATETIMEOFFSET(), @UpdatedBy )

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
						+ 'Procedure Name: [dbo].usp_SetSiteCloseoutTime' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END