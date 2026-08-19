CREATE PROCEDURE [dbo].[usp_EnumerateActiveAlarmsBySite]
(
	@SiteGuid UniqueIdentifier 
	, @Unacknowledged BIT
	, @Unsilenced BIT
	, @Notify BIT
)
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @IsEnterpriseSystem BIT = [dbo].[udf_IsEnterprise]();
		DECLARE @IsEnterpriseSite BIT = (SELECT Enterprise FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid);

      SELECT a.AlarmGuid AS AlarmGuid
      , at.[AlarmTestGuid] as AlarmTestGuid
      , at.[Order] AS AlarmTestPriority
      , ap.[Priority] AS AlarmPriority
      , ptas.Acknowledged AS Acknowledged
      , CASE WHEN ISNULL(@IsEnterpriseSystem, 0) = 1 AND ISNULL(@IsEnterpriseSite, 0) = 0 THEN CAST(1 AS BIT) ELSE ptas.Silenced END AS Silenced
      , ~ptas.Acknowledged & ~ptas.AlarmTestFailed  AS IsNormal
      , a.ID AS AlarmID
      , p.Description AS Description
      , aps.ID AS PointType
      , s.ID AS SiteID
      , s.SiteGuid as SiteGuid
      , p.ID AS PointID
      , p.PointGuid AS PointGuid
      , CASE ptas.AlarmTestFailed WHEN 0 THEN at.AlarmState + ' ' + a.NotAlarmState WHEN 1 THEN at.AlarmState End AS Status
      , t.ID AS TagID
      , a.InputTagGuid AS TagGuid
      , ptas.AlarmTestFailedTimestamp AS Timestamp
      , ptas.PointTagAlarmStatusGuid as PointTagAlarmStatusGuid
      , ap.ID as AlarmPriorityID
      , ap.BackgroundSteady AS AlarmBackgroundSteadyColor
      , ap.TextSteady AS AlarmTextSteadyColor
      , ap.BackgroundAlternate AS AlarmBackgroundAlternateColor
      , ap.TextAlternate AS AlarmTextAlternateColor
      , CASE ptas.AlarmTestFailed WHEN 0 THEN nap.SoundFile WHEN 1 THEN ap.SoundFile End AS SoundFile
      , nap.BackgroundSteady AS NormalUnacknowledgedAlarmBackgroundSteadyColor
      , nap.TextSteady AS NormalUnacknowledgedAlarmTextSteadyColor
      , nap.BackgroundAlternate AS NormalUnacknowledgedAlarmBackgroundAlternateColor
      , nap.TextAlternate AS NormalUnacknowledgedAlarmTextAlternateColor
      , s.ShortDatePattern AS ShortDatePattern
      , s.TimePattern AS TimePattern
      , s.TimeZone AS TimeZone
      from tblPointTagAlarmStatus ptas
      INNER JOIN tblAlarmTest at ON at.AlarmTestGuid = ptas.AlarmTestGuid
      INNER JOIN tblAlarm a ON a.AlarmGuid = at.AlarmGuid
      INNER JOIN tblPointTag t ON t.PointTagGuid = a.InputTagGuid
      INNER JOIN tblPoint p ON p.PointGuid = t.PointGuid
      INNER JOIN tblAlarmPriorities ap ON ap.AlarmPriorityGuid = at.AlarmPriorityGuid
      INNER JOIN tblAlarmPriorities nap ON nap.AlarmPriorityGuid = at.NormalUnacknowledgedAlarmPriorityGuid
      INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid
      INNER JOIN tblPointTemplate pt ON pt.PointTemplateGuid = p.PointTemplateGuid
      INNER JOIN tblApplicationString aps ON aps.ApplicationStringGuid = pt.PointTemplateTypeApplicationStringGuid
      Where p.Enabled = 1
      AND p.SiteGuid = @SiteGuid
      AND a.Enabled = 1
      AND a.Suppressed = 0
      AND a.ShelvedOneShot = 0
      AND (a.ShelvedEndTimeStamp IS NULL OR a.ShelvedEndTimeStamp < SYSDATETIMEOFFSET())
      AND at.Enabled = 1
      AND (ptas.AlarmTestFailed = 1 OR (ptas.AlarmTestFailed = 0 AND ptas.Acknowledged = 0))
		AND ((@Unacknowledged = CAST(1 AS BIT) AND @Unsilenced = CAST(1 AS BIT) AND (ptas.Acknowledged = CAST(0 as BIT) OR ptas.Silenced = CAST(0 as BIT)))
		OR (@Unacknowledged = CAST(1 AS BIT) AND @Unsilenced = CAST(0 AS BIT) AND ptas.Acknowledged = CAST(0 as BIT))
		OR (@Unacknowledged = CAST(0 AS BIT) AND @Unsilenced = CAST(1 AS BIT) AND ptas.Silenced = CAST(0 as BIT))
		OR (@Unacknowledged = CAST(0 AS BIT) AND @Unsilenced = CAST(0 AS BIT)))
		AND (--Begin 1
            (
               @Notify = CAST(1 AS BIT) AND (a.Notify = CAST(1 as BIT)) 
               OR 
               (@Notify = CAST(0 AS BIT))
            )

            -- The following excludes tests that have common alarm with lower order active alarm test
            AND 0 = (--Begin 2
                        SELECT COUNT(*) FROM dbo.tblAlarm a1
                        LEFT JOIN dbo.tblAlarmTest at1 ON at1.AlarmGuid = a1.AlarmGuid
                        LEFT JOIN dbo.tblPointTagAlarmStatus ptas1 ON ptas1.AlarmTestGuid = at1.AlarmTestGuid
                        WHERE a1.AlarmGuid = a.AlarmGuid
                        AND at1.Enabled = 1
                        AND 
                        (--Begin 3
					            -- Exclude Unacknowledged Test with another other ActiveAlarm Test
                           (at1.[Order] <> at.[Order] AND (ptas.AlarmTestFailed = 0 AND ptas1.AlarmTestFailed = 1))

					            -- Exclude ActiveAlarm Test with lower order ActiveAlarm Test
					            OR (at1.[Order] < at.[Order] AND (ptas.AlarmTestFailed = 1 and ptas1.AlarmTestFailed = 1))

					            -- Exclude Unacknowledged Test with lower order Unacknowledged Test
					            OR (at1.[Order] < at.[Order] AND (ptas.AlarmTestFailed = 0 and ptas1.AlarmTestFailed = 0 AND ptas1.Acknowledged = 0))
                        )--End 3
                     )--End 2
         )--End 1
		ORDER BY Timestamp, ap.Priority
 
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
						+ 'Procedure Name: usp_EnumerateActiveAlarmsBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END

