/*
	DROP FUNCTION [rpt].[udf_GetDimensionParameterValues]

	--DECLARE @params nvarchar(1000) = '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]'
	--DECLARE @params nvarchar(1000) = '[Site].[Site Id].&[Baltimore]'
	--DECLARE @params nvarchar(1000) = '[Product].[Product Id].&[3201], [Product].[Product Id].&[9025]'
	DECLARE @params nvarchar(1000) = '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]'
	SELECT * FROM [rpt].[udf_GetDimensionParameterValues] (@params, 0)
	
	
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[1]&[1],[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2023]&[3]&[7]'
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[1]'
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]'
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[Full Date].&[1903]&[9]&[25]&[1903-09-25T00:00:00]'
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[1984]&[1]&[2]'
	--DECLARE @params nvarchar(1000) =  '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].[ALL]'
	--DECLARE @params nvarchar(1000) = '[Inventory Date].[Date Key].&[20180401]'
	--DECLARE @params nvarchar(1000) = '2018-08-09 00:00:00'
	--DECLARE @params nvarchar(1000) = NULL
	DECLARE @params nvarchar(1000) = '20210223'
	SELECT * FROM [rpt].[udf_GetDimensionParameterValues] (@params, 1)
*/
CREATE FUNCTION [rpt].[udf_GetDimensionParameterValues]
(
	@CSVList nvarchar(1000),
	@IsDateDim bit
)
RETURNS @tblDimValue TABLE
(
	[ParameterIndex] [int] NULL,
	[ParameterStrValue] [nvarchar](100) NULL,
	[BeginDateKey] [int] NULL,
	[EndDateKey] [int] NULL
)
AS
BEGIN
	DECLARE @splitOn CHAR(1) = ','
	DECLARE @param nvarchar(200)
	DECLARE @paramStrValue nvarchar(100)
	DECLARE @beginDateKey int
	DECLARE @endDateKey int
	DECLARE @pos int
	DECLARE @year int
	DECLARE @quarter int
	DECLARE @month int
	DECLARE @day int
	DECLARE @isFullDateMember bit = 0
	DECLARE @fullDateKeyStr varchar(8)
	DECLARE @parameterIndex int = 0

	WHILE ((@CSVList IS NOT NULL) AND (LEN(@CSVList) > 0))
	BEGIN
		IF (CHARINDEX ('[ALL]', @CSVList) > 0)
		BEGIN
			BREAK
		END

		SELECT @param = @CSVList
		IF (CHARINDEX(',', @CSVList) > 0)
		BEGIN
			SELECT @param = LTRIM(RTRIM(SUBSTRING(@CSVList, 1, CHARINDEX(@splitOn, @CSVList)-1))) 
		END
		SELECT @paramStrValue = @param
				
		IF (ISNULL(@IsDateDim, 0) = 0)
		BEGIN
			IF ((CHARINDEX ('[', REVERSE(@param))-1) > 0)
			BEGIN
				SELECT @paramStrValue = RIGHT(@param , CHARINDEX('[', REVERSE(@param))-1)
				SELECT @paramStrValue = LEFT(@paramStrValue, CHARINDEX(']', @paramStrValue)-1)
			END
		END		

		INSERT INTO @tblDimValue
		(ParameterIndex, ParameterStrValue)
		VALUES (@parameterIndex, @paramStrValue)

		IF (@IsDateDim = 1)
		BEGIN
			SET @year = NULL
			SET @quarter = NULL
			SET @month = NULL
			SET @day = NULL
			SET @beginDateKey = NULL
			SET @endDateKey = NULL
			SELECT @pos = CHARINDEX(']', @paramStrValue)
			IF (@pos = 0)  -- format:YYYY or YYYY-MM or YYYY-MM-dd or YYYYMMdd
			BEGIN				
				SELECT @pos = CHARINDEX('-', @paramStrValue)				
				IF (@pos = 0)
				BEGIN -- format:YYYY or YYYYMMdd
					IF (TRY_CONVERT(int, @paramStrValue) IS NOT NULL)
					BEGIN
						IF (LEN(@paramStrValue) = 4)
						BEGIN
							SELECT @year = CONVERT(int, @paramStrValue)
						END
						ELSE IF (LEN(@paramStrValue) = 8)
						BEGIN
							SELECT @year = CONVERT(int, SUBSTRING(@paramStrValue, 1, 4))
							SELECT @month = CONVERT(int, SUBSTRING(@paramStrValue, 5, 2))
							SELECT @day = CONVERT(int, SUBSTRING(@paramStrValue, 7, 2))
						END
					END
				END
				ELSE
				BEGIN -- format:YYYY-MM or YYYY-MM-dd
					SELECT @year = CONVERT(int, LEFT(@paramStrValue, @pos-1))
					SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+1, LEN(@paramStrValue))
					SELECT @pos = CHARINDEX('-', @paramStrValue)
					IF (@pos = 0) -- format: YYYY-MM
					BEGIN
						SELECT @month = CONVERT(int, @paramStrValue)
					END
					ELSE
					BEGIN  -- format: YYYY-MM-dd or YYYY-MM-dd hh:mm:ss
						SELECT @month = CONVERT(int, LEFT(@paramStrValue, @pos-1))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+1, LEN(@paramStrValue))
						SELECT @pos = CHARINDEX(' ', @paramStrValue)
						IF (@pos = 0) -- format: YYYY-MM-dd
						BEGIN
							SELECT @day = CONVERT(int, @paramStrValue)
						END
						ELSE
						BEGIN	-- format: YYYY-MM-dd hh:mm:ss
							SELECT @day = CONVERT(int, LEFT(@paramStrValue, @pos-1))
						END
					END
				END
			END
			ELSE
			BEGIN  -- format: [Dim components][Date Components][Optional Full Date Component]  (e.g. [Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[1]&[1] or [Inventory Date].[Full Date].&[1903]&[9]&[25]&[1903-09-25T00:00:00] or [Inventory Date].[Date Key].&[20180401])				
				DECLARE @fullDateTest nvarchar(1000) = @paramStrValue
				SELECT @fullDateKeyStr = NULL
				SELECT @fullDateTest = SUBSTRING(@fullDateTest, 1, LEN(@fullDateTest)-1)
				IF (TRY_CONVERT(int, RIGHT(@fullDateTest , CHARINDEX('[', REVERSE(@fullDateTest))-1)) IS NULL) 			
				BEGIN
					SELECT @isFullDateMember = 1
				END
				ELSE IF (CONVERT(int, RIGHT(@fullDateTest , CHARINDEX('[', REVERSE(@fullDateTest))-1)) > 10000)
				BEGIN
					SELECT @isFullDateMember = 1
					SELECT @fullDateKeyStr = RIGHT(@fullDateTest , CHARINDEX('[', REVERSE(@fullDateTest))-1)
				END
				WHILE (1 = 1)
				BEGIN				
					SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
					SELECT @pos = CHARINDEX(']', @paramStrValue)
					IF (TRY_CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2)) IS NULL)
					BEGIN
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+2, LEN(@paramStrValue))						
					END
					ELSE
					BEGIN
						BREAK
					END
				END			

				SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
				SELECT @pos = CHARINDEX(']', @paramStrValue)				
				IF (@isFullDateMember = 0)  -- Date Components format:[YYYY]&[Quarter] or [YYYY]&[Quarter]&[MM] 
				BEGIN
					SELECT @year = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
					SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+1, LEN(@paramStrValue))
					IF (CHARINDEX('[', @paramStrValue) > 0)
					BEGIN	--format: [YYYY]&[Qtr]
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
						SELECT @pos = CHARINDEX(']', @paramStrValue)
						SELECT @quarter = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos, LEN(@paramStrValue))
						IF (CHARINDEX('[', @paramStrValue) > 0)
						BEGIN --format: [YYYY]&[Qtr]&[Month]
							SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
							SELECT @pos = CHARINDEX(']', @paramStrValue)							
							SELECT @month = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
						END
					END
				END
				ELSE  --format: [YYYY]&[MM]&[dd]&[Full Date Component] or [YYYYMMdd]
				BEGIN										
					
					IF (@fullDateKeyStr IS NULL)
					BEGIN
						SELECT @year = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+1, LEN(@paramStrValue))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
						SELECT @pos = CHARINDEX(']', @paramStrValue)

						SELECT @month = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, @pos+1, LEN(@paramStrValue))
						SELECT @paramStrValue = SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue), LEN(@paramStrValue))
						SELECT @pos = CHARINDEX(']', @paramStrValue)
						SELECT @day = CONVERT(int, SUBSTRING(@paramStrValue, CHARINDEX('[', @paramStrValue) + 1, @pos-2))
					END
					ELSE
					BEGIN 
						SELECT @year = CONVERT(int, SUBSTRING(@fullDateKeyStr, 1, 4))
						SELECT @month = CONVERT(int, SUBSTRING(@fullDateKeyStr, 5, 2))
						SELECT @day = CONVERT(int, SUBSTRING(@fullDateKeyStr, 7, 2))
					END
				END

			END

			IF (@day IS NOT NULL)
			BEGIN
				SELECT @beginDateKey = @year*10000 + @month*100 + @day
				SELECT @endDateKey = @beginDateKey
			END
			ELSE IF (@month IS NOT NULL)
			BEGIN
				SELECT @beginDateKey = @year*10000 + @month*100 + 1
				SET @endDateKey = (SELECT TOP(1) SKey FROM dbo.DimDate WHERE CalendarYearMonthNumber = (@year*100 + @month) ORDER BY SKey DESC)
			END
			ELSE IF (@quarter IS NOT NULL)
			BEGIN
				SELECT @beginDateKey = (SELECT TOP(1) SKey FROM dbo.DimDate WHERE CalendarYear = @year AND CalendarQuarter = @quarter ORDER BY SKey)
				SELECT @endDateKey = (SELECT TOP(1) SKey FROM dbo.DimDate WHERE CalendarYear = @year AND CalendarQuarter = @quarter ORDER BY SKey DESC)
			END
			ELSE IF (@year IS NOT NULL)
			BEGIN
				SELECT @beginDateKey = @year*10000 + 100 + 1
				SELECT @endDateKey = @year*10000 + 1200 + 31
			END
			IF (@beginDateKey IS NOT NULL)
			BEGIN
				UPDATE @tblDimValue 
				SET BeginDateKey = @beginDateKey, EndDateKey = @endDateKey
				WHERE ParameterIndex = @parameterIndex
			END

		END	
		
		IF (CHARINDEX(',', @CSVList) > 0)
		BEGIN
			SET @CSVList = SUBSTRING(@CSVList, CHARINDEX(@splitOn, @CSVList) + LEN(@splitOn), lEN(@CSVList))	
			SET @parameterIndex = @parameterIndex + 1
		END
		ELSE
		BEGIN
			BREAK;
		END
	END
	RETURN
END