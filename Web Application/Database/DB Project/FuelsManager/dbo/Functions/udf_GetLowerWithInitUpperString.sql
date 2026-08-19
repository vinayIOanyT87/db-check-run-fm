
CREATE FUNCTION [dbo].[udf_GetLowerWithInitUpperString](@String NVARCHAR(500))
	RETURNS NVARCHAR(500)
AS
BEGIN

	DECLARE @Position INT
		,	@TotalTokens INT;
	DECLARE @Tokens TABLE(RowNumber INT IDENTITY,Token NVARCHAR(500))
	DECLARE @SubString NVARCHAR(500);
	SET @Position=0;
	SET @TotalTokens = 0;
	SELECT @Position=CHARINDEX(' ',@String,@Position);

	BEGIN
		-- Capture each token
		WHILE @Position > 0 AND @Position < LEN(@String)-1
		BEGIN
			SET @SubString = LEFT(@String,@Position - 1);
			IF(LEN(@SubString) > 0)
			BEGIN
				INSERT INTO @Tokens(Token) VALUES(@SubString);
				SET @TotalTokens += 1;
			END
			SET @String = SUBSTRING(@String,@Position+1,LEN(@String)-@Position);
			SELECT @Position=CHARINDEX(' ',@String,@Position);
		END
	END
	
	-- Insert last token
	IF(LEN(@String) > 0)
	BEGIN
		INSERT INTO @Tokens(Token) VALUES(@String);
		SET @TotalTokens += 1;	
	END
	
	-- REBUILD String
	SET @String='';
	SET @Substring='';
	SET @Position = 1;
	WHILE @Position <= @TotalTokens
	BEGIN
		SELECT @Substring=Token FROM @Tokens WHERE RowNumber = @Position;
		
		SET @String += LEFT(@Substring,1) + LOWER(RIGHT(@Substring,LEN(@Substring)-1)) + ' ';
		SET @Position+=1;
	END
	SET @String = RTRIM(@String);
	
	RETURN @String;
END