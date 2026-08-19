CREATE FUNCTION [dbo].[IpStringToInt]
(@ip CHAR (15))
RETURNS INT
AS
BEGIN 

	DECLARE @nReturn	INT
	DECLARE @o1			INT
	DECLARE @o2			INT
	DECLARE @o3			INT
	DECLARE @o4			INT
	DECLARE @nBase		INT
 
	SET @o1 = CONVERT(INT, PARSENAME(@ip, 4))
	SET @o2 = CONVERT(INT, PARSENAME(@ip, 3))
	SET @o3 = CONVERT(INT, PARSENAME(@ip, 2))
	SET @o4 = CONVERT(INT, PARSENAME(@ip, 1))
 
	IF       (@o1 BETWEEN 0 AND 0xFF) 
		  AND (@o2 BETWEEN 0 AND 0xFF) 
		  AND (@o3 BETWEEN 0 AND 0xFF) 
		  AND (@o4 BETWEEN 0 AND 0xFF) 
	BEGIN      
	
		SET @nBase =
				CASE 
					WHEN @o1 < 0x80 THEN			    (@o1  * 0x01000000) 
					ELSE						- (0x100 - @o1) * 0x01000000
				END

		SET @nReturn =	@nBase				+
						  (@o2 * 0x00010000) +
						  (@o3 * 0x00000100) +
						  (@o4 * 0x00000001) 
	 END 
	 ELSE 
		SET @nReturn = 0xFFFFFFFF

	 RETURN @nReturn 
END