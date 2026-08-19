CREATE FUNCTION [dbo].[udf_ConvertLittleEndianBinaryFloatToFloat]
(
	@binaryfloat binary(8)
)
RETURNS FLOAT
AS
BEGIN
	declare @reversedBinaryFloat binary(8)

	set @reversedBinaryFloat =  cast(reverse(cast(@binaryfloat as char(8))) as binary(8))
	RETURN	SIGN(CAST(@reversedBinaryFloat AS BIGINT))
		* (1.0 + (CAST(@reversedBinaryFloat AS BIGINT) & 0x000FFFFFFFFFFFFF) * POWER(CAST(2 AS FLOAT), -52))
		* POWER(CAST(2 AS FLOAT), (CAST(@reversedBinaryFloat AS BIGINT) & 0x7ff0000000000000) / 0x0010000000000000 - 1023)
END
