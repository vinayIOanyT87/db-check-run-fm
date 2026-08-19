CREATE FUNCTION [dbo].[udf_SplitString](
	@Source as nvarchar(max),
	@Delimeter as char, 
	@ReturnPart as Int) 
RETURNS @tValues TABLE
(
	Value nvarchar(512)
)
AS
BEGIN
	DECLARE @CurrentIndex As Int
	DECLARE @CurrentPart As Int
	DECLARE @StartOfValue As Int
	DECLARE @FoundDelimiter As tinyint

	SET @CurrentPart = 1
	SET @FoundDelimiter = 0
	SET @CurrentIndex = 1
	SET @StartOfValue = 1

	WHILE @CurrentIndex <= LEN(@Source)
	BEGIN
		IF SUBSTRING(@Source, @CurrentIndex, 1) = @Delimeter
		BEGIN
			SET @FoundDelimiter = 1

			IF ((@ReturnPart = 0) OR (@CurrentPart = @ReturnPart))
			BEGIN
				INSERT INTO @tValues (Value) VALUES (CAST(SUBSTRING(@Source,@StartOfValue,(@CurrentIndex-@StartOfValue)) AS nvarchar(512)))
			END

			SET @StartOfValue = @CurrentIndex + 1
			SET @CurrentPart = @CurrentPart + 1
		END
		
		SET @CurrentIndex = @CurrentIndex + 1
	END
	
	--If we didn''t find any delimiters, we only have one value, put it into the table as a
	--single result
	IF (@FoundDelimiter = 0)
	BEGIN
		INSERT INTO @tValues (Value) VALUES (CAST(@Source AS nvarchar(512)))
	END
	ELSE
	BEGIN
		-- Make sure we insert the last item in the list if the string didn''t end with a delimiter
		IF (SUBSTRING(@Source, LEN(@Source), 1)) <> @Delimeter
		BEGIN
			IF ((@ReturnPart = 0) OR (@CurrentPart = @ReturnPart))
				INSERT INTO @tValues (Value) VALUES (CAST(SUBSTRING(@Source,@StartOfValue,(@CurrentIndex-@StartOfValue)) AS nvarchar(512)))
		END
	END

	IF (@ReturnPart > @CurrentPart)
	BEGIN
		INSERT INTO @tValues (Value) VALUES (NULL)
	END
RETURN
END