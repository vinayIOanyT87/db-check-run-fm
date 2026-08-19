CREATE FUNCTION [dbo].[udf_IsEnterprise]
()
RETURNS BIT
AS
BEGIN
	DECLARE @IsEnterpriseInstall bit
	
    -- The key should technically be "IsEnterpriseFlag" or something like that since being an Enterprise
    -- system is not unique to Nspa.
    SELECT @IsEnterpriseInstall = CONVERT(bit, CASE WHEN COALESCE(SettingValue, '') = 'TRUE' 
                                                        OR COALESCE(SettingValue, '') = '1'
                                                    THEN 1 ELSE 0 END) 
        FROM [dbo].[tblConfigurationSetting] 
            WHERE SettingKey = 'IsEnterprise'

	RETURN @IsEnterpriseInstall;
END