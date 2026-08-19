
CREATE PROCEDURE [dbo].[usp_AlarmAndEventLogInsert]
(
	@AlarmAndEventLogs dbo.AlarmAndEventLogType READONLY
)
AS
BEGIN
	SET NOCOUNT ON

	BEGIN TRY
		DECLARE @SourceNode NVARCHAR(256)
		SELECT @SourceNode = SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'InstallDetailsSynchronizationNodeName'

		------------------------------------------------------------------------------------------------------
		-- Stored procedure: usp_AlarmAndEventLogInsert
		-- Author: Ryan Hill
		-- Purpose: Insert alarm and event log records in bulk
		------------------------------------------------------------------------------------------------------

		INSERT INTO tblAlarmAndEventLog
		(
			SiteGuid,
			[Source],
			Alarm,
			ID,
			AssociatedData,
			Acknowledged,
			CategoryID,
			PriorityID,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy,
			SourceNode
		)
		SELECT 
			alarmAndEventLogs.SiteGuid,
			alarmAndEventLogs.[Source],
			alarmAndEventLogs.Alarm,
			alarmAndEventLogs.ID,
			alarmAndEventLogs.AssociatedData,
			alarmAndEventLogs.Acknowledged,
			ISNULL(tblApplicationString.ID, '{None}'),
			ISNULL(tblAlarmPriorities.ID, '{None}'),
			alarmAndEventLogs.CreatedDate,
			alarmAndEventLogs.CreatedBy,
			alarmAndEventLogs.UpdatedDate,
			alarmAndEventLogs.UpdatedBy,
			@SourceNode
		FROM @AlarmAndEventLogs alarmAndEventLogs
		INNER JOIN tblAlarmAndEvents ON alarmAndEventLogs.[Source] = tblAlarmAndEvents.[Source] 
			AND alarmAndEventLogs.ID = tblAlarmAndEvents.ID 
			-- Only write alarm and event log records if the source is enabled
			AND tblAlarmAndEvents.[Enabled] = 1
			AND (alarmAndEventLogs.SiteGuid = tblAlarmAndEvents.SiteGuid 
			OR tblAlarmAndEvents.SiteGuid IN (SELECT OwnerSiteGuid FROM map.tblEntityAlarmAndEventToSite WHERE MapToSiteGuid = alarmAndEventLogs.SiteGuid))
		LEFT JOIN tblApplicationString ON tblApplicationString.ApplicationStringGuid = tblAlarmAndEvents.CategoryGuid
		LEFT JOIN tblAlarmPriorities ON tblAlarmPriorities.AlarmPriorityGuid = tblAlarmAndEvents.PriorityGuid

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
						+ 'Procedure Name: usp_AlarmAndEventLogInsert' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END 
