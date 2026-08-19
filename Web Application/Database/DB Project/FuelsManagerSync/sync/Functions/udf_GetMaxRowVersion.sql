CREATE FUNCTION [sync].[udf_GetMaxRowVersion]
(
	@RowVersion1 varbinary(8),
	@RowVersion2 varbinary(8),
	@RowVersion3 varbinary(8)
)
RETURNS varbinary(8)
AS
BEGIN
	if(@RowVersion1 IS NOT NULL AND @RowVersion1 >= ISNULL(@RowVersion2,0) AND @RowVersion1 >= ISNULL(@RowVersion3,0))
		RETURN @RowVersion1

	if(@RowVersion2 IS NOT NULL AND @RowVersion2 >= ISNULL(@RowVersion1,0) AND @RowVersion2 >= ISNULL(@RowVersion3,0))
		RETURN @RowVersion2

	RETURN @RowVersion3
END
