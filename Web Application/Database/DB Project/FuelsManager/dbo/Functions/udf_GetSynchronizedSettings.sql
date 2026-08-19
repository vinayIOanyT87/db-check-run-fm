CREATE FUNCTION [dbo].[udf_GetSynchronizedSettings]
(
)
RETURNS @returntable TABLE
(
	Setting NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @SynchronizedSettings NVARCHAR(MAX)
	SET @SynchronizedSettings = (SELECT SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'SynchronizedSettings')
	IF @SynchronizedSettings = NULL SET @SynchronizedSettings = 'SynchronizedSettings'

	DECLARE @xml xml

	SET @xml = N'<root><r>' + replace((SELECT @SynchronizedSettings FOR XML PATH('') ), ';', '</r><r>') + '</r></root>'

	INSERT INTO @ReturnTable SELECT t.value('.','NVARCHAR(MAX)') as [delimited items] FROM @xml.nodes('//root/r') as a(t)

	RETURN
END
