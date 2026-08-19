CREATE FUNCTION [dbo].[udf_GetTableOfStringsFromStringList]
(
	@List NVARCHAR(MAX)
)
RETURNS @tblGuids TABLE
(
	Guid NVARCHAR(MAX)
)
AS
BEGIN
	DECLARE @start INT
	DECLARE @nextComma INT
	DECLARE @len INT
	SET @start = 1

	SET @List = ISNULL(@List, '') + ','
	SET @len = LEN(@List)
	WHILE( @start < @len)
	BEGIN
		SET @nextComma = CHARINDEX( ',', @List, @start)
		INSERT INTO @tblGuids (Guid) 
			SELECT  SUBSTRING( @List, @start, @nextComma - @start) 
		SET @start = @nextComma + 1
	END

	RETURN;
END     


GO


