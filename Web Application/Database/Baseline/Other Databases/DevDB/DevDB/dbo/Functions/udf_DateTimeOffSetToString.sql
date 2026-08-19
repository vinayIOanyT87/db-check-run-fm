CREATE FUNCTION [dbo].[udf_DateTimeOffSetToString](
    @value datetimeoffset(7)
)
RETURNS 
	nvarchar(max)
AS
BEGIN
    IF (@value IS NULL)
    BEGIN
        RETURN @value
    END

    RETURN (CONCAT('N''', CONVERT(nvarchar(max), @value), ''''));
END
