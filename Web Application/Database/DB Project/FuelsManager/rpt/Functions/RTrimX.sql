CREATE FUNCTION rpt.RTrimX(@str VARCHAR(MAX)) RETURNS VARCHAR(MAX)
AS
BEGIN
DECLARE @trimchars VARCHAR(10)
SET @trimchars = CHAR(9)+CHAR(10)+CHAR(13)+CHAR(32)
IF @str LIKE '%[' + @trimchars + ']'
SET @str = REVERSE(rpt.LTrimX(REVERSE(@str)))
RETURN @str
END