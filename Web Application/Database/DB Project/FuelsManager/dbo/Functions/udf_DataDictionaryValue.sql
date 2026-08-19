
CREATE FUNCTION [dbo].[udf_DataDictionaryValue]
(@DBName NVARCHAR (50), @LoginSiteGuid UNIQUEIDENTIFIER)
RETURNS NVARCHAR (50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @DataDictionaryValue nvarchar(50)
	SET @DataDictionaryValue = (SELECT [Value] 
								FROM dbo.tblDataDictionaries 
								WHERE (SiteGuid = @LoginSiteGuid 
								      OR SiteGuid = (SELECT OwnerSiteGuid
													  FROM map.tblEntityDataDictionaryToSite
													  WHERE MapToSiteGuid = @LoginSiteGuid )
									   )
									  AND [Key] = @DBName
								)
	
	IF @DataDictionaryValue IS NULL 
		SET @DataDictionaryValue = @DBName
	
	RETURN @DataDictionaryValue

END