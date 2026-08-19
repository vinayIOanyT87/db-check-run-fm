

CREATE FUNCTION [dbo].[udf_FormatProcessVariable]
(@Value FLOAT, @EngineeringUnitsIndex INT)
RETURNS NVARCHAR(20)
WITH SCHEMABINDING
AS
BEGIN 
	DECLARE @Result NVARCHAR(20)
	
	-- Ft In 8th or Ft In 16th
	IF @EngineeringUnitsIndex = 19
	OR @EngineeringUnitsIndex = 27
	BEGIN
		DECLARE @Feet INT
				, @Inch INT
				, @Fract INT
				, @Fraction FLOAT
				, @Negative BIT
				, @Factor INT
		
		-- Get Whole Feet to Integer
		IF @Value < 0
			SET @Negative = 1
		ELSE
			SET @Negative = 0

		if @Negative = 1
			SET @Value = -1 * @Value

		SET @Feet = CONVERT(INT,@Value)

		SET @Fraction = @Value - @Feet

		-- Convert to Inches
		SET @Fraction = @Fraction * 12.0000
		SET @Inch = CONVERT(INT,@Fraction)
		SET @Fraction = @Fraction - @Inch

		IF @EngineeringUnitsIndex = 27
			SET @Factor = 16
		ELSE
			SET @Factor = 8


		-- Convert to Fraction
		SET @Fraction = @Fraction * @Factor
		SET @Fract = CONVERT(INT,@Fraction + 0.500) 

		IF @Fract >= @Factor
		BEGIN
			SET @Inch = @Inch + 1
			SET @Fract = 0

			if @Inch >= 12
			BEGIN
				SET @Feet = @Feet + 1
				SET @Inch = 0
			END
		END

		IF @EngineeringUnitsIndex = 27
			SET @Result = CONVERT(NVARCHAR,@Feet) + '-' + RIGHT('0' + CONVERT(NVARCHAR,@Inch),2) + '-' + RIGHT('0' + CONVERT(NVARCHAR,@Fract),2)
		ELSE	
			SET @Result = CONVERT(NVARCHAR,@Feet) + '-' + RIGHT(' 0' + CONVERT(NVARCHAR,@Inch),2) + '-' + CONVERT(NVARCHAR,@Fract)

		IF @Negative = 1
			SET @Result = '-' + @Result
	END
	ELSE
		SET @Result = CONVERT(NVARCHAR(20), @Value)

	RETURN @Result 
END

