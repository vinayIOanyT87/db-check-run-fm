CREATE PROCEDURE [dbo].[usp_EnumerateRestrictedAccessByAlarmTestGuids]
(
	@SiteGuid UniqueIdentifier,
	@UserGuid UniqueIdentifier,
	@AlarmTestGuids GuidListType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @IsEnterpriseSystem BIT = [dbo].[udf_IsEnterprise]();
		DECLARE @IsEnterpriseSite BIT = (SELECT Enterprise FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid);

		-- Get Point Access Groups assigned to User
		DECLARE @PointAccessGroupGuidTable TABLE(PointAccessGroupGuid UniqueIdentifier)

		INSERT INTO @PointAccessGroupGuidTable SELECT DISTINCT pagtug.PointAccessGroupGuid FROM map.tblUserToGroup utg
		INNER JOIN map.tblPointAccessGroupToUserGroup pagtug ON pagtug.UserGroupGuid = utg.GroupGuid
		INNER JOIN dbo.tblPointAccessGroup pag ON pag.PointAccessGroupGuid = pagtug.PointAccessGroupGuid AND pag.SiteGuid = utg.SiteGuid
		WHERE utg.SiteGuid = @SiteGuid AND utg.UserGuid = @UserGuid

		-- Get Points assigned to Point Access Groups
		CREATE TABLE tempdb.#PointTable (
				PointGuid UniqueIdentifier,
				PointAccessGroupGuid UniqueIdentifier
		 )
 
		 INSERT INTO #PointTable SELECT DISTINCT PointGuid, PointAccessGroupGuid FROM
		  (SELECT p.PointGuid, pagtpt.PointAccessGroupGuid FROM dbo.tblPoint p
			INNER JOIN map.tblPointAccessGroupToPointTemplate pagtpt ON pagtpt.PointTemplateGuid = p.PointTemplateGuid
			INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtpt.PointAccessGroupGuid
			WHERE p.SiteGuid = @SiteGuid
			UNION
			SELECT p.PointGuid, pagtp.PointAccessGroupGuid FROM dbo.tblPoint p
			INNER JOIN map.tblPointAccessGroupToPoint pagtp ON pagtp.PointGuid = p.PointGuid
			INNER JOIN @PointAccessGroupGuidTable paggt ON paggt.PointAccessGroupGuid = pagtp.PointAccessGroupGuid
			WHERE p.SiteGuid = @SiteGuid) s
   
		-- Get Value Information		
		CREATE TABLE tempdb.#AlarmTestTable (
				AlarmTestGuid UniqueIdentifier,
				AlarmTestTemplateGuid UniqueIdentifier,
				PointGuid UniqueIdentifier,
				PointTemplateGuid UniqueIdentifier,
				PointTemplateTagGuid UniqueIdentifier,
				[View] bit,
				Acknowledge bit
		)
		
		CREATE NONCLUSTERED INDEX [IX_AlarmTestTable_AlarmTestGuid] ON [tempdb].[#AlarmTestTable](AlarmTestGuid ASC)

		INSERT INTO [#AlarmTestTable]
		SELECT atg.Guid as AlarmTestGuid, at.AlarmTestTemplateGuid as AlarmTestTemplateGuid, pt.PointGuid, p.PointTemplateGuid,
		CASE WHEN pt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' THEN NULL
		ELSE pt.PointTemplateTagGuid
		END,
		CAST(0 AS BIT), CAST(0 AS BIT) FROM @AlarmTestGuids atg
		LEFT JOIN dbo.tblAlarmTest at ON at.AlarmTestGuid = atg.Guid
		LEFT JOIN dbo.tblAlarm a ON a.AlarmGuid = at.AlarmGuid
		LEFT JOIN dbo.tblPointTag pt ON pt.PointTagGuid = a.InputTagGuid
		LEFT JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid;

--		SELECT * FROM #AlarmTestTable;

		-- Update AlarmTest Information with actuals from Point Access Groups Alarm Tests
		WITH AlarmTestUpdates AS
		(SELECT att.AlarmTestGuid,
		SUM(CASE WHEN pagat.[View] IS NULL OR pagat.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagat.Acknowledge IS NULL OR pagat.Acknowledge = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Acknowledge
		FROM #AlarmTestTable att
		INNER JOIN #PointTable pt ON pt.PointGuid = att.PointGuid
		LEFT JOIN map.tblPointAccessGroupToAlarmTest pagat ON pagat.AlarmTestGuid = att.AlarmTestTemplateGuid AND pagat.PointAccessGroupGuid = pt.PointAccessGroupGuid
		GROUP BY att.AlarmTestGuid)
		UPDATE [#AlarmTestTable]
		SET [#AlarmTestTable].[View] = CASE WHEN atu.[View] > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END,
		[#AlarmTestTable].Acknowledge = CASE WHEN ISNULL(@IsEnterpriseSystem, 0) = 1 AND ISNULL(@IsEnterpriseSite, 0) = 0 THEN CAST(0 AS BIT) 
														WHEN atu.Acknowledge > 0  THEN CAST(1 AS BIT)
														ELSE CAST(0 AS BIT)
														END
		FROM AlarmTestUpdates atu WHERE [#AlarmTestTable].AlarmTestGuid = atu.AlarmTestGuid AND [#AlarmTestTable].[PointTemplateTagGuid] IS NOT NULL;

		-- Update AlarmTest Information with actuals from Point Access Groups Point Alarm Tests
		WITH AlarmTestUpdates AS
		(SELECT att.AlarmTestGuid,
		SUM(CASE WHEN pagpat.[View] IS NULL OR pagpat.[View] = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS [View],
		SUM(CASE WHEN pagpat.Acknowledge IS NULL OR pagpat.Acknowledge = CAST(1 AS BIT) THEN 1 ELSE 0 END) AS Acknowledge
		FROM #AlarmTestTable att
		INNER JOIN #PointTable pt ON pt.PointGuid = att.PointGuid
		LEFT JOIN map.tblPointAccessGroupToPointAlarmTest pagpat ON pagpat.AlarmTestGuid = att.AlarmTestGuid AND pagpat.PointAccessGroupGuid = pt.PointAccessGroupGuid
		GROUP BY att.AlarmTestGuid)
		UPDATE [#AlarmTestTable]
		SET [#AlarmTestTable].[View] = CASE WHEN atu.[View] > 0 THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END,
		[#AlarmTestTable].Acknowledge = CASE WHEN ISNULL(@IsEnterpriseSystem, 0) = 1 AND ISNULL(@IsEnterpriseSite, 0) = 0 THEN CAST(0 AS BIT) 
														 WHEN atu.Acknowledge > 0 THEN CAST(1 AS BIT)
														 ELSE CAST(0 AS BIT)
														 END
		FROM AlarmTestUpdates atu WHERE [#AlarmTestTable].AlarmTestGuid = atu.AlarmTestGuid  AND [#AlarmTestTable].[PointTemplateTagGuid] IS NULL;


		SELECT att.AlarmTestGuid, att.[View], att.Acknowledge FROM [#AlarmTestTable] att
		WHERE att.[View] = CAST(0 AS BIT) OR att.Acknowledge = CAST(0 AS BIT)  
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
						+ 'Procedure Name: usp_EnumerateRestrictedAccessByAlarmTestGuids' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END
GO


