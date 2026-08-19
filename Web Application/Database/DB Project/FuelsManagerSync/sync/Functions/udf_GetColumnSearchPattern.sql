
CREATE FUNCTION [sync].[udf_GetColumnSearchPattern] (
	@pString varchar(max)
) 
RETURNS varchar(max)
AS
BEGIN
	RETURN ('%|' + @pString + '|%');
END
