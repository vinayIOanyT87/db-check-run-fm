
/* 
=============================================
Author: Ryan Hill
Create date: 4/24/12

Description:	
This function calculates and returns the meter total for a specific meter, handling meter rollover and backwards rotating meters

You must provide the meter start and stop values, as well as whether the meter rotates backwards and how many digits it has (to detect rollover)
=============================================
*/
CREATE FUNCTION [dbo].[udf_MeterReconciliationCalculateMeterTotal]
(
	@RotatesBackwardsFlag BIT,
	@NumberOfDigits INT,
	@MeterStart FLOAT,
	@MeterStop FLOAT
)
RETURNS FLOAT
AS
BEGIN
	--the meter total is the value that we will calculate and return
	DECLARE @MeterTotal FLOAT

	DECLARE @BeginValue FLOAT
	DECLARE @EndValue FLOAT

	--if the meter rotates backwards
	IF(@RotatesBackwardsFlag = 1)
	BEGIN		
		SET @BeginValue = @MeterStop
		SET @EndValue = @MeterStart
	END
	ELSE
	BEGIN
		SET @BeginValue = @MeterStart
		SET @EndValue = @MeterStop
	END

	--when the meter rotates forwards and the meter stop is less than the start, then a rollover occurred.
	IF(@BeginValue > @EndValue)
	BEGIN
		--to calculate the total, we take the number of digits of the meter and determine the maximum meter value. 
		--For example, three digit meter's maximum value would be 999.
		--the meter total is the meter stop + the maximum value - meter start + 1
		--the + 1 is to account for the all zero meter reading
		--example: Meter Start = 100. Meter Stop = 900. # of digits = 3.
		-- 100 + 999 - 900 + 1 = 200
		SET @MeterTotal = @EndValue + (CAST(REPLICATE('9', @NumberOfDigits) AS FLOAT) - @BeginValue + 1)	
	END
	ELSE -- no rollover
	BEGIN
		SET @MeterTotal = @EndValue - @BeginValue
	END

	--return the value we calculated
	RETURN @MeterTotal
END