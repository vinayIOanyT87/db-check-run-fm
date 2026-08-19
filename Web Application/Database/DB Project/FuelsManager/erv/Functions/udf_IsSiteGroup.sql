

CREATE FUNCTION [erv].[udf_IsSiteGroup]
(@SiteGuid uniqueidentifier)
RETURNS bit
AS
BEGIN
	DECLARE @result bit
	SET @result = 1
	
	IF EXISTS 
	(
		SELECT *  FROM dbo.tblSites
		WHERE SiteGuid = @SiteGuid
		AND SiteGroupFlag = 1
	)
		SET @result = 1
	ELSE
		SET @result = 0

	RETURN @result         
END
