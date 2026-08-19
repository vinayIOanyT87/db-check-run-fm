/*
	DROP FUNCTION [archive].[udf_GetArchiveCutOffDate]

	SELECT [archive].[udf_GetArchiveCutOffDate] ()	

*/
CREATE FUNCTION [archive].[udf_GetArchiveCutOffDate]
(
)
RETURNS Date
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [archive].[udf_GetArchiveCutOffDate]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the current Archive cut-off date, the date from which all earlier data (including data on the cut-off date) are to be archived.
	-- Notes:
	-- 1. The same cut-off date is applicable to all scopes
	------------------------------------------------------------------------------------------------------

	DECLARE @retentionMonths int
	DECLARE @result date
	
	SET @result = NULL

	SELECT @retentionMonths = CONVERT(int, SettingValue) FROM [dbo].[tblConfigurationSetting]
	WHERE SettingKey = 'ArchiveRetentionPeriodInMonths'

	IF ((@retentionMonths IS NULL) OR (@retentionMonths <= 0))
	BEGIN
		RETURN NULL
	END

	SET @result = DATEADD(Month, (0 - @retentionMonths), GetDate())
	SET @result = DATEADD(Day, -1, @result)
	
	RETURN @result;

END

GO


