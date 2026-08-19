CREATE FUNCTION [dbo].[udf_ToUnicodeString](
    @value nvarchar(max)
)
RETURNS 
	nvarchar(max)
AS
BEGIN
    IF (@value IS NULL)
    BEGIN
        RETURN @value
    END

    RETURN (CONCAT('N''', REPLACE(@value,'''', ''''''), ''''));
END
