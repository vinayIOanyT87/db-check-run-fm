
CREATE FUNCTION fmaudit.udf_DisableTriggerByAuditRule(@SchemaName NVARCHAR(200),@TableName NVARCHAR(500),@Action CHAR(1))
RETURNS BIT
AS
BEGIN
	DECLARE @Result BIT

	IF @SchemaName = 'dbo' AND @TableName = 'tblPointTag'
	BEGIN
		DECLARE @changeContextName nvarchar(100) 
		DECLARE @bypassTrackingFlags int 
		DECLARE @bypassReason nvarchar(512)
 
		SELECT @changeContextName = ContextName 
				,@bypassTrackingFlags = BypassTrackingFlags 
				,@bypassReason = BypassReason 
			FROM [track].[udf_GetChangeTrackingSessionDetails]()

		IF (@changeContextName = 'usp_PointTagDataUpdate' AND [track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 0)
		BEGIN 
			SET @Result=1
		END
		ELSE
		BEGIN
			IF EXISTS (SELECT 1 FROM dbo.tblConfigurationSetting WHERE KeyType IN ('SZ', 'DWORD') AND SettingKey='AuditEnabled' AND SettingValue='1')
				SET @Result=0
			ELSE
				SET @Result=1
		END
	END

	ELSE
	BEGIN
		IF EXISTS (SELECT 1 FROM dbo.tblConfigurationSetting WHERE KeyType IN ('SZ', 'DWORD') AND SettingKey='AuditEnabled' AND SettingValue='1')
			SET @Result=0
		ELSE
			SET @Result=1
	END
	
	RETURN(@Result)
END