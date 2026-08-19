CREATE FUNCTION [dbo].[udf_IpIntToString]
(@ip INT)
RETURNS CHAR (15)
AS
BEGIN
	DECLARE @o1 INT
	DECLARE @o2 INT
	DECLARE @o3 INT
	DECLARE @o4 INT
	
	IF ABS(@ip) > 0x7FFFFFFF	RETURN '255.255.255.255'
	SET @o1 = @ip / 0x01000000
	
	IF @o1  = 0
	BEGIN
		SELECT @o1 = 0xFF
		SELECT @ip = @ip + 0x01000000
	END
	ELSE
	IF @o1 < 0
	BEGIN
		IF (@ip % 0x0100000) = 0	SET @o1 = @o1 + 0x0100
		ELSE
		BEGIN
			SET @o1 = @o1 + 0xFF
			IF @o1 = 0x0080	SET @ip = @ip + 0x80000000
			ELSE					SET @ip = @ip + (0x01000000 * (0x0100 - @o1))
		END
	END
	ELSE SET @ip = @ip - (0x01000000 * @o1)

	SET @ip = @ip % 0x01000000
	SET @o2 = @ip / 0x00010000
	SET @ip = @ip % 0x00010000
	SET @o3 = @ip / 0x00000100
	SET @ip = @ip % 0x00000100
	SET @o4 = @ip
	
	RETURN CONVERT(VARCHAR(4), @o1) + '.' + CONVERT(VARCHAR(4), @o2) + '.' +
			 CONVERT(VARCHAR(4), @o3) + '.' + CONVERT(VARCHAR(4), @o4)
END