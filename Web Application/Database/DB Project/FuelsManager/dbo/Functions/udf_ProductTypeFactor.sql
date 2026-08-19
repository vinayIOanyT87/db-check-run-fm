CREATE FUNCTION [dbo].[udf_ProductTypeFactor]
(@Type NVARCHAR (20), @AdditiveFactor FLOAT, @ComponentFactor FLOAT)
RETURNS FLOAT
WITH SCHEMABINDING
AS
BEGIN
	DECLARE @Result float 
	if(@Type = 'Additive')
		set @Result=@AdditiveFactor
	else
		set @Result=@ComponentFactor
	return @Result 
END