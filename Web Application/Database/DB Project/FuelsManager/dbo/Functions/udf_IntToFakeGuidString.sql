CREATE FUNCTION [dbo].[udf_IntToFakeGuidString]
(@vbInput VARBINARY (255), @chPadLeft CHAR (1))
RETURNS VARCHAR (40)
AS
BEGIN
	DECLARE @sResult			VARCHAR(40)		SET @sResult		= ''
	DECLARE @LenOfAGuid		TINYINT			SET @LenOfAGuid	= 40
	
	DECLARE @i					INT	SET @i          = 1
	DECLARE @nInputLen		INT	SET @nInputLen  = DATALENGTH(@vbInput)
	DECLARE @nChar				INT	SET @nChar      = 0
	DECLARE @nHiInt			INT	SET @nHiInt		 = 0
	DECLARE @nLoInt			INT	SET @nLoInt		 = 0

	WHILE (@i <= @nInputLen)
	BEGIN
		SET @nChar  = CONVERT(INT, SUBSTRING(@vbInput, @i, 1))
		SET @nHiInt = FLOOR(@nChar / 16)
		SET @nLoInt = @nChar - (@nHiInt * 16)

		SET @sResult = @sResult +
							SUBSTRING('0123456789ABCDEF', @nHiInt + 1, 1) +
							SUBSTRING('0123456789ABCDEF', @nLoInt + 1, 1)
		SET @i = @i + 1
	END

	SET @sResult = REPLICATE(@chPadLeft, @LenOfAGuid - LEN(@sResult)) + @sResult
	SET @sResult = STUFF(@sResult,  9, 1, '-')
	SET @sResult = STUFF(@sResult, 14, 1, '-')
	SET @sResult = STUFF(@sResult, 19, 1, '-')
	SET @sResult = STUFF(@sResult, 24, 1, '-')
	
	RETURN @sResult
END