CREATE PROCEDURE [dbo].[usp_PointTagDataUpdate]
(
	@User NVARCHAR(30), 
	@PointTagData dbo.PointTagDataType READONLY,
	@EnterpriseVisibility BIT
)
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRANSACTION;  

	DECLARE @IsEnterprise BIT;
	SET @IsEnterprise = (CAST((SELECT SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'IsEnterprise') AS BIT)) 

	DECLARE @ChangeTrackingSessionGuid UNIQUEIDENTIFIER
	DECLARE @InsertedTrackingSession TABLE( ChangeTrackingSessionGuid UNIQUEIDENTIFIER )


	-- Disable the tracking triggers when executing this trigger.  Disable only for the current SPID
	-- BypassTrackingFlags: Bypass Insert Change Tracking = 0x01
	--						Bypass Update Change Tracking = 0x02
	--						Bypass Delete Change Tracking = 0x04
	--
	-- Bypass all triggers: 0x01 & 0x02 & 0x04
	--

	IF @EnterpriseVisibility = CAST(1 AS BIT)
	BEGIN
		INSERT [track].[tblChangeTrackingSession]( [ChangeTrackingSessionGuid], [SqlServerSessionID], [ContextName], [BypassTrackingFlags], [BypassReason], [CreatedDate])
		OUTPUT INSERTED.[ChangeTrackingSessionGuid] INTO @InsertedTrackingSession
		SELECT newid(), @@spid, 'usp_PointTagDataUpdate', 0x07, 'Ignore change to values', SYSDATETIMEOFFSET()
	END

	BEGIN TRY
		-- perform the table update
			IF @EnterpriseVisibility = CAST(0 AS BIT)
			BEGIN
				UPDATE [dbo].[tblPoint]
				SET UpdatedBy = @User, UpdatedDate = sysdatetimeoffset()
				FROM [dbo].[tblPointTag] PT
					INNER JOIN @PointTagData PTD ON PTD.PointTagGuid = PT.PointTagGuid
					INNER JOIN [dbo].[tblPoint] P ON P.PointGuid = PT.PointGuid
					INNER JOIN [dbo].[tblSites] S ON S.SiteGuid = P.SiteGuid
					WHERE @IsEnterprise = CAST(1 AS BIT)
					AND S.Enterprise = CAST(0 AS BIT);
			END

			UPDATE [dbo].[tblPointTag] 
			SET
				[EngineeringUnitsType] = PTD.[EngineeringUnitsType],
				[EngineeringUnitsIndex] = PTD.[EngineeringUnitsIndex], 
				[DecimalPlaces] = PTD.[DecimalPlaces],
				[Maximum] = PTD.[Maximum],
				[Minimum] = PTD.[Minimum],
				[Value] = 
                CASE WHEN @EnterpriseVisibility = CAST(0 AS BIT) THEN PTD.[Value]
							WHEN [ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND pt.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN pt.[Value]
					      WHEN [ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND pt.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN pt.[Value]
				         ELSE PTD.[Value]
					 END,
				[Status] = 
                CASE WHEN @EnterpriseVisibility = CAST(0 AS BIT) THEN PTD.[Status]
							WHEN [ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND pt.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN pt.[Status]
					      WHEN [ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND pt.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN pt.[Status]
				         ELSE PTD.[Status]
					 END,
				[ServerTimeStamp] =
                CASE WHEN @EnterpriseVisibility = CAST(0 AS BIT) THEN PTD.[ServerTimeStamp]
							WHEN [ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND pt.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN pt.[ServerTimeStamp]
					      WHEN [ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND pt.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN pt.[ServerTimeStamp]
				         ELSE PTD.[ServerTimeStamp]
					 END,
				[SourceTimeStamp] =
                CASE WHEN @EnterpriseVisibility = CAST(0 AS BIT) THEN PTD.[SourceTimeStamp]
							WHEN [ValueType] = 'FMBusinessObjects.DataObjects.PointCommandStatusListReference' AND pt.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(PointCommandStatusListReference/PointCommandStatusListGuid)[1]','nvarchar(max)') THEN pt.[SourceTimeStamp]
					      WHEN [ValueType] = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND pt.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') <> PTD.[Value].value('(DeviceAlarmMapReference/DeviceAlarmMapGuid)[1]','nvarchar(max)') THEN pt.[SourceTimeStamp]
				         ELSE PTD.[SourceTimeStamp]
					 END,
				[UpdatedBy] = @User,
				[UpdatedDate] = sysdatetimeoffset()
			FROM
				[dbo].[tblPointTag] pt
				INNER JOIN @PointTagData PTD ON PTD.PointTagGuid = pt.PointTagGuid;

			IF @EnterpriseVisibility = CAST(1 AS BIT)
			BEGIN
				-- Re-enable the tracking triggers
				SELECT @ChangeTrackingSessionGuid = [ChangeTrackingSessionGuid]
				FROM @InsertedTrackingSession

				DELETE 
				FROM [track].[tblChangeTrackingSession]
				WHERE [ChangeTrackingSessionGuid] = @ChangeTrackingSessionGuid
			END
	END TRY
	BEGIN CATCH        
		IF @EnterpriseVisibility = CAST(1 AS BIT)
		BEGIN
			-- Re-enable the tracking triggers
			SELECT @ChangeTrackingSessionGuid = [ChangeTrackingSessionGuid]
			FROM @InsertedTrackingSession

			DELETE 
			FROM [track].[tblChangeTrackingSession]
			WHERE [ChangeTrackingSessionGuid] = @ChangeTrackingSessionGuid
		END


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
						+ 'Procedure Name: usp_PointTagDataUpdate' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      

		IF @@TRANCOUNT > 0  
			ROLLBACK TRANSACTION;  

	END CATCH    

	IF @@TRANCOUNT > 0  
		 COMMIT TRANSACTION;  
END
GO


