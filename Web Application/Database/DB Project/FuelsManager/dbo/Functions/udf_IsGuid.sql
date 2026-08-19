CREATE FUNCTION [dbo].[udf_IsGuid]
(@sTest VARCHAR (38))
RETURNS INT
AS
BEGIN
	SELECT @sTest = REPLACE(REPLACE(@sTest, '{', ''), '}', '')

	DECLARE @bIsGuid INT

	SELECT @bIsGuid =
		CASE
			WHEN @sTest LIKE
				(REPLICATE('[0-9A-Fa-f]', 8) + '-' + 
				 REPLICATE('[0-9A-Fa-f]', 4) + '-' +
				 REPLICATE('[0-9A-Fa-f]', 4) + '-' +
				 REPLICATE('[0-9A-Fa-f]', 4) + '-' +
				 REPLICATE('[0-9A-Fa-f]', 12))
			THEN 1
			ELSE 0
		END
		
	RETURN @bIsGuid
END