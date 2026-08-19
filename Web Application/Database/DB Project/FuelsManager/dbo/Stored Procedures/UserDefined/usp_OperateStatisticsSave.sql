CREATE PROCEDURE [dbo].[usp_OperateStatisticsSave]
    @WindowName NVARCHAR (50),
    @AvgMinuteTimeAlarmNotifications INT,
    @MaxMinuteTimeAlarmNotifications INT,
    @AvgSessionTimeAlarmNotifications INT,
    @MaxSessionTimeAlarmNotifications INT,
    @AvgMinuteTimeAlarmRefresh INT,
    @MaxMinuteTimeAlarmRefresh INT,
    @AvgSessionTimeAlarmRefresh INT,
    @MaxSessionTimeAlarmRefresh INT,
    @AvgMinuteTimeUpdateValues INT,
    @MaxMinuteTimeUpdateValues INT,
    @AvgSessionTimeUpdateValues INT,
    @MaxSessionTimeUpdateValues INT,
    @AvgMinuteTimeDynamicPointGroup INT,
    @MaxMinuteTimeDynamicPointGroup INT,
    @AvgSessionTimeDynamicPointGroup INT,
    @MaxSessionTimeDynamicPointGroup INT,
    @CreatedDate DATETIMEOFFSET(7),
    @CreatedBy [dbo].[udtUserID],
    @UpdatedDate DATETIMEOFFSET(7),
    @UpdatedBy [dbo].[udtUserID],
    @SessionGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
			MERGE dbo.tblOperateStatistics AS Target
			USING ( SELECT
			@WindowName AS WindowName,
			@AvgMinuteTimeAlarmNotifications AS AvgMinuteTimeAlarmNotifications,
			@MaxMinuteTimeAlarmNotifications AS MaxMinuteTimeAlarmNotifications,
			@AvgSessionTimeAlarmNotifications AS AvgSessionTimeAlarmNotifications,
			@MaxSessionTimeAlarmNotifications AS MaxSessionTimeAlarmNotifications,
			@AvgMinuteTimeAlarmRefresh AS AvgMinuteTimeAlarmRefresh,
			@MaxMinuteTimeAlarmRefresh AS MaxMinuteTimeAlarmRefresh,
			@AvgSessionTimeAlarmRefresh AS AvgSessionTimeAlarmRefresh,
			@MaxSessionTimeAlarmRefresh AS MaxSessionTimeAlarmRefresh,
			@AvgMinuteTimeUpdateValues AS AvgMinuteTimeUpdateValues,
			@MaxMinuteTimeUpdateValues AS MaxMinuteTimeUpdateValues,
			@AvgSessionTimeUpdateValues AS AvgSessionTimeUpdateValues,
			@MaxSessionTimeUpdateValues AS MaxSessionTimeUpdateValues,
			@AvgMinuteTimeDynamicPointGroup AS AvgMinuteTimeDynamicPointGroup,
			@MaxMinuteTimeDynamicPointGroup AS MaxMinuteTimeDynamicPointGroup,
			@AvgSessionTimeDynamicPointGroup AS AvgSessionTimeDynamicPointGroup,
			@MaxSessionTimeDynamicPointGroup AS MaxSessionTimeDynamicPointGroup,
			@CreatedDate AS CreatedDate,
			@CreatedBy AS CreatedBy,
			@UpdatedDate AS UpdatedDate,
			@UpdatedBy AS UpdatedBy,
			@SessionGuid AS SessionGuid
		) AS Source
		ON (Target.[SessionGuid] = Source.[SessionGuid] AND Target.[WindowName] = Source.[WindowName])
		WHEN MATCHED THEN

			UPDATE SET
			target.[OperateActiveStartTime]= SYSDATETIMEOFFSET(),
			target.[OperateActiveStopTime] = NULL,
			target.[AvgMinuteTimeAlarmNotifications] = @AvgMinuteTimeAlarmNotifications,
			target.[MaxMinuteTimeAlarmNotifications] = @MaxMinuteTimeAlarmNotifications,
			target.[AvgSessionTimeAlarmNotifications] = @AvgSessionTimeAlarmNotifications,
			target.[MaxSessionTimeAlarmNotifications] = @MaxSessionTimeAlarmNotifications,
			target.[AvgMinuteTimeAlarmRefresh] = @AvgMinuteTimeAlarmRefresh,
			target.[MaxMinuteTimeAlarmRefresh] = @MaxMinuteTimeAlarmRefresh,
			target.[AvgSessionTimeAlarmRefresh] = @AvgSessionTimeAlarmRefresh,
			target.[MaxSessionTimeAlarmRefresh] = @MaxSessionTimeAlarmRefresh,
			target.[AvgMinuteTimeUpdateValues] = @AvgMinuteTimeUpdateValues,
			target.[MaxMinuteTimeUpdateValues] = @MaxMinuteTimeUpdateValues,
			target.[AvgSessionTimeUpdateValues] = @AvgSessionTimeUpdateValues,
			target.[MaxSessionTimeUpdateValues] = @MaxSessionTimeUpdateValues,
			target.[AvgMinuteTimeDynamicPointGroup] = @AvgMinuteTimeDynamicPointGroup,
			target.[MaxMinuteTimeDynamicPointGroup] = @MaxMinuteTimeDynamicPointGroup,
			target.[AvgSessionTimeDynamicPointGroup] = @AvgSessionTimeDynamicPointGroup,
			target.[MaxSessionTimeDynamicPointGroup] = @MaxSessionTimeDynamicPointGroup,
			target.[UpdatedDate] = @UpdatedDate,
			target.[UpdatedBy] = @UpdatedBy

		WHEN NOT MATCHED BY TARGET THEN 
			INSERT (
			[WindowName],
			[OperateActiveStartTime],
			[OperateActiveStopTime],
			[AvgMinuteTimeAlarmNotifications],
			[MaxMinuteTimeAlarmNotifications],
			[AvgSessionTimeAlarmNotifications],
			[MaxSessionTimeAlarmNotifications],
			[AvgMinuteTimeAlarmRefresh],
			[MaxMinuteTimeAlarmRefresh],
			[AvgSessionTimeAlarmRefresh],
			[MaxSessionTimeAlarmRefresh],
			[AvgMinuteTimeUpdateValues],
			[MaxMinuteTimeUpdateValues],
			[AvgSessionTimeUpdateValues],
			[MaxSessionTimeUpdateValues],
			[AvgMinuteTimeDynamicPointGroup],
			[MaxMinuteTimeDynamicPointGroup],
			[AvgSessionTimeDynamicPointGroup],
			[MaxSessionTimeDynamicPointGroup],
			[CreatedDate],
			[CreatedBy],
			[UpdatedDate],
			[UpdatedBy],
			[SessionGuid]
			)
			VALUES (
			Source.[WindowName],
			SYSDATETIMEOFFSET(),
			NULL,
			Source.[AvgMinuteTimeAlarmNotifications],
			Source.[MaxMinuteTimeAlarmNotifications],
			Source.[AvgSessionTimeAlarmNotifications],
			Source.[MaxSessionTimeAlarmNotifications],
			Source.[AvgMinuteTimeAlarmRefresh],
			Source.[MaxMinuteTimeAlarmRefresh],
			Source.[AvgSessionTimeAlarmRefresh],
			Source.[MaxSessionTimeAlarmRefresh],
			Source.[AvgMinuteTimeUpdateValues],
			Source.[MaxMinuteTimeUpdateValues],
			Source.[AvgSessionTimeUpdateValues],
			Source.[MaxSessionTimeUpdateValues],
			Source.[AvgMinuteTimeDynamicPointGroup],
			Source.[MaxMinuteTimeDynamicPointGroup],
			Source.[AvgSessionTimeDynamicPointGroup],
			Source.[MaxSessionTimeDynamicPointGroup],
			Source.[CreatedDate],
			Source.[CreatedBy],
			Source.[UpdatedDate],
			Source.[UpdatedBy],
			Source.[SessionGuid]
			);

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
						+ 'Procedure Name: usp_OperateStatisticsSave' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
END
GO