CREATE FUNCTION [dbo].[udf_IntToHexString]
(@vbInput VARBINARY (255))
RETURNS VARCHAR (18)
AS
BEGIN
	DECLARE @sResult		VARCHAR(18)		SET @sResult   = '0x'
	DECLARE @i				INT				SET @i         = 1
	DECLARE @nInputLen	INT				SET @nInputLen = DATALENGTH(@vbInput)
	DECLARE @nChar			INT				SET @nChar     = 0
	DECLARE @nHiInt		INT				SET @nHiInt		= 0
	DECLARE @nLoInt		INT				SET @nLoInt		= 0

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

	RETURN @sResult
END