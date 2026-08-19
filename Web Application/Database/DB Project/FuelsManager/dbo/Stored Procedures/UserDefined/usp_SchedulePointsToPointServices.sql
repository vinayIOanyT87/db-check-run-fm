CREATE PROCEDURE [dbo].[usp_SchedulePointsToPointServices] 
(
       @Hostname nvarchar(256)
)
AS
BEGIN
       ------------------------------------------------------------------------------------------------------
       -- Stored Procedure: [dbo].[usp_SchedulePointsToPointServices] 
       -- Author: Shawn Marlin
       -- Version/Date: 1.0.0 / 2015-08-25 14:21:10.4470770 -04:00
       -- Purpose: Detect Point Service Failure and put points back on free list to be schedule and to schedule
       -- all point to Point Services in a load balanced fashion
       ------------------------------------------------------------------------------------------------------
       BEGIN TRY     
              --DETERMINE IF IT IS TIME TO DO SCHEDULING
              DECLARE @CURRENT_TIME DATETIMEOFFSET(7)
              SET @CURRENT_TIME = SYSDATETIMEOFFSET()
              DECLARE @MAX_MISSED_PINGS INT
              SET @MAX_MISSED_PINGS = (SELECT 3) -- Num Missed Pings To Declare a PointService Unresponsive
              DECLARE @GOOD_HEALTH_STATUS INT
              SET @GOOD_HEALTH_STATUS = (SELECT 0) -- Could do a query against lookup.tblPointServiceHealthStatus, but table is pre-initalized for Good = 0
              DECLARE @IsEnterprise BIT
              SET @IsEnterprise = (SELECT CAST(SettingValue AS BIT) FROM dbo.tblConfigurationSetting WHERE SettingKey = 'IsEnterprise')


              DECLARE @SCHED_ENTRIES int
              DECLARE @DO_SCHED bit
              SET @DO_SCHED = 0
              SET @SCHED_ENTRIES = (SELECT COUNT(LastSchedulingTime) FROM tblPointServiceSchedulingTime WITH(TABLOCKX,HOLDLOCK))
              IF (@SCHED_ENTRIES = 0)
                     BEGIN
                           SET @DO_SCHED = 1
                     END
              ELSE
                     BEGIN
                           DECLARE @LAST_SCHED_TIME DATETIMEOFFSET(7)
                           SET @LAST_SCHED_TIME = (SELECT TOP 1 LastSchedulingTime FROM tblPointServiceSchedulingTime ORDER BY LastSchedulingTime DESC)
                           DECLARE @SCHED_PERIOD_SECONDS int
                           SET @SCHED_PERIOD_SECONDS = 5
                           DECLARE @SECONDS_SINCE_LAST_SCHED int
                           SET @SECONDS_SINCE_LAST_SCHED = (SELECT ABS(DATEDIFF(second,@LAST_SCHED_TIME,@CURRENT_TIME)))
                           IF (@SECONDS_SINCE_LAST_SCHED > @SCHED_PERIOD_SECONDS)
                           BEGIN
                                  SET @DO_SCHED = 1
                           END
                     END

              --PERFORM SCHEDULING
              IF (@DO_SCHED = 1)
                     BEGIN
                           INSERT INTO tblPointServiceSchedulingTime (Hostname,LastSchedulingTime) VALUES (@Hostname, @CURRENT_TIME)                             
                           DELETE FROM tblPointServiceSchedulingTime WHERE  PointServiceSchedulingTimeGuid NOT IN ( SELECT TOP  (100) PointServiceSchedulingTimeGuid FROM tblPointServiceSchedulingTime ORDER BY LastSchedulingTime DESC )


						   --Remove disabled Points or Point Enterprise Setting doesn't match System Enterprise Setting
                           DELETE map.tblPointToPointService FROM map.tblPointToPointService m
                           INNER JOIN tblPoint p ON m.PointGuid = p.PointGuid
									INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid
                           WHERE p.Enabled = CAST(0 AS BIT) OR s.Enterprise <> @IsEnterprise Or s.Enabled = CAST(0 AS BIT)

                           --If any Point Service reported it's health as bad in dbo.tblPointService then remove all of its point 
                           --assignments in map.tblPointToPointService.
						   --If any Point Service has missed "n" (I suggest 3) ping periods as determined by 
                           --dbo.tblPointService.PingIntervalInSeconds, dbo.tblPointService.LastPingTime, and current system time 
                           --then remove all of its point assignments in map.tblPointToPointService.
                           DELETE map.tblPointToPointService FROM map.tblPointToPointService m
                           INNER JOIN dbo.tblPointService ps ON m.PointServiceGuid = ps.PointServiceGuid
                           WHERE ps.HealthStatusIndex <> @GOOD_HEALTH_STATUS
						         OR ps.PingIntervalInSeconds * @MAX_MISSED_PINGS < ABS(DATEDIFF(second,ps.LastPingTime,@CURRENT_TIME))

                           DELETE tblPointService WHERE HealthStatusIndex <> @GOOD_HEALTH_STATUS
						         OR PingIntervalInSeconds * @MAX_MISSED_PINGS < ABS(DATEDIFF(second,LastPingTime,@CURRENT_TIME))


                           DECLARE @PointServiceTable AS Table
                           (
                                  PointServiceGuid UNIQUEIDENTIFIER NOT NULL,
                                  MaxNumberOfPoints INT NOT NULL, 
                                  NumberOfPointsAssigned INT NOT NULL,
                                  NumberOfPointsToAssign INT NOT NULL
                           );

                           INSERT INTO @PointServiceTable (PointServiceGuid, MaxNumberOfPoints, NumberOfPointsAssigned, NumberOfPointsToAssign)
                           SELECT ps.PointServiceGuid, ps.MaxNumberOfPoints, (SELECT COUNT(PointGuid) from map.tblPointToPointService p where p.PointServiceGuid = ps.PointServiceGuid), 0 
                           FROM dbo.tblPointService ps 
                           WHERE ps.HealthStatusIndex = @GOOD_HEALTH_STATUS

                           DECLARE @FreePoints AS Table
                           (
                                  PointGuid UNIQUEIDENTIFIER NOT NULL
                           );

                           INSERT INTO @FreePoints (PointGuid)
                           SELECT p.PointGuid FROM tblPoint p
                           INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid
                           WHERE p.PointGuid NOT IN 
                           (
                                  SELECT PointGuid FROM map.tblPointToPointService p2ps 
                                  INNER JOIN @PointServiceTable pst ON pst.PointServiceGuid = p2ps.PointServiceGuid
                           )
                           AND p.Enabled = CAST(1 AS BIT)  AND s.Enterprise =  @IsEnterprise AND s.Enabled = CAST(1 AS BIT)

                           --Find all points that are enabled and not in map.tblPointToPointService and load balance them across
                           --available point services taking utilization and throttle levels into consideration (see statement above)
                           --and record assignment in map.tblPointToPointService.
                           DECLARE @NUM_VALID_POINT_SERVICES int
                           SET @NUM_VALID_POINT_SERVICES = (SELECT COUNT(PointServiceGuid) FROM @PointServiceTable)
                           IF (@NUM_VALID_POINT_SERVICES > 0)
                                  BEGIN
                                         DECLARE @NUM_ENABLED_POINTS int
                                         SET @NUM_ENABLED_POINTS = (SELECT COUNT(PointGuid)
                                         FROM tblPoint p
                                         INNER JOIN tblSites s ON s.SiteGuid = p.SiteGuid
                                         WHERE p.Enabled = CAST(1 AS BIT) AND s.Enterprise = @IsEnterprise AND s.Enabled = CAST(1 AS BIT))
                                         DECLARE @TOTAL_MAX_POINTS int
                                         SET @TOTAL_MAX_POINTS =  (SELECT SUM(MaxNumberOfPoints) FROM @POINTSERVICETABLE)

                                         IF( @NUM_ENABLED_POINTS  > @TOTAL_MAX_POINTS)
                                                BEGIN
                                                       SET @NUM_ENABLED_POINTS = @TOTAL_MAX_POINTS
                                                END
                                         ELSE
                                                BEGIN
                                                       SET @NUM_ENABLED_POINTS = @NUM_ENABLED_POINTS  
                                                END

                                         DECLARE @EXTRA_CAPACITY_AFTER_SCHEDULING int
                                         SET @EXTRA_CAPACITY_AFTER_SCHEDULING = @TOTAL_MAX_POINTS - @NUM_ENABLED_POINTS
                                         DECLARE @EXTRA_CAPACITY_AFTER_SCHEDULING_PER_POINTSERVICE int
                                         SET @EXTRA_CAPACITY_AFTER_SCHEDULING_PER_POINTSERVICE = @EXTRA_CAPACITY_AFTER_SCHEDULING / @NUM_VALID_POINT_SERVICES 

                                         UPDATE @POINTSERVICETABLE SET NumberOfPointsToAssign = MaxNumberOfPoints - @EXTRA_CAPACITY_AFTER_SCHEDULING_PER_POINTSERVICE - NumberOfPointsAssigned

                                         DECLARE @PointServiceGuid1 UNIQUEIDENTIFIER
                                         DECLARE PointServiceCursor CURSOR FAST_FORWARD FOR
                                         SELECT PointServiceGuid FROM @PointServiceTable ORDER BY NumberOfPointsToAssign ASC
                                         OPEN PointServiceCursor
                                         FETCH NEXT FROM PointServiceCursor INTO @PointServiceGuid1
                                         WHILE @@FETCH_STATUS=0
                                                BEGIN
                                                       DECLARE @NumPointsToAssign int
                                                       SET @NumPointsToAssign = (SELECT NumberOfPointsToAssign FROM @PointServiceTable WHERE PointServiceGuid = @PointServiceGuid1)

                                                       IF(@NumPointsToAssign < 0)
                                                              BEGIN
                                                                     --Remove Overallocated
                                                                     DECLARE @NumPointsToRemove int
                                                                     SET @NumPointsToRemove = @NumPointsToAssign * -1
                                                                     INSERT INTO @FreePoints (PointGuid)
                                                                     SELECT TOP(@NumPointsToRemove) PointGuid FROM map.tblPointToPointService 
                                                                     WHERE  PointServiceGuid = @PointServiceGuid1 

                                                                     DELETE m FROM map.tblPointToPointService AS m 
                                                                     INNER JOIN (
                                                                           SELECT TOP  (@NumPointsToRemove) PointGuid FROM  map.tblPointToPointService
                                                                           WHERE PointServiceGuid = @PointServiceGuid1
                                                                     ) tm
                                                                     ON m.PointGuid = tm.PointGuid

                                                              END
                                                       ELSE
                                                       BEGIN
                                                              --Assign Unassigned
                                                              IF(@NumPointsToAssign > 0)
                                                                     BEGIN
                                                                           INSERT INTO map.tblPointToPointService (PointGuid,PointServiceGuid,TimeAssigned)
                                                                           SELECT TOP(@NumPointsToAssign) PointGuid,@PointServiceGuid1,SYSDATETIMEOFFSET() FROM @FreePoints
                                                                           --DELETE @FreePoints WHERE PointGuid IN (SELECT TOP(@NumPointsToAssign) PointGuid FROM @FreePoints)
                                                                           DELETE f FROM @FreePoints AS f 
                                                                           INNER JOIN map.tblPointToPointService m
                                                                           ON f.PointGuid = m.PointGuid
                                                                     END
                                                       END
                                                       FETCH NEXT FROM PointServiceCursor INTO @PointServiceGuid1
                                                END
                                                CLOSE PointServiceCursor
                                                DEALLOCATE PointServiceCursor
                                         END
                           END
              END
       TRY
       BEGIN CATCH  
              DECLARE       @_ErrMessage NVARCHAR(2048)      
                           , @_ErrNumber INT           
                           , @_ErrProcName NVARCHAR(126)           
                           , @_ErrLineNumber INT;            
              SET @_ErrMessage = ERROR_MESSAGE();        
              SET @_ErrNumber = ERROR_NUMBER();        
              SET @_ErrProcName= ERROR_PROCEDURE();        
              SET @_ErrLineNumber = ERROR_LINE();            
              SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
                                         + 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
                                         + 'Procedure Name: [dbo].usp_SchedulePointsToPointServices' + CHAR(13)+CHAR(10)                  
                                         + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
              RAISERROR(@_ErrMessage,18,1);      
       END CATCH    
       
END
GO


