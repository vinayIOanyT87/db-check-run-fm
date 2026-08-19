
CREATE PROCEDURE [rpt].[usp_DsMonthYearList]
WITH EXECUTE AS CALLER
AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_DsMonthYearList] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Generate a month year list for the date range found in tblTransactions.InventoryDate
	------------------------------------------------------------------------------------------------------

	DECLARE @BeginDate DATETIMEOFFSET(7)
	DECLARE @EndDate DATETIMEOFFSET(7)
	DECLARE @MinDate DATETIMEOFFSET(7)
	DECLARE @MaxDate DATETIMEOFFSET(7)
	SET @MinDate = '1/1/1900'
	SET @MaxDate = '12/31/2099'
	SELECT @EndDate = MAX(InventoryDate), @BeginDate = MIN(InventoryDate) FROM tblTransactions
	IF(@EndDate < @MinDate OR @EndDate > @MaxDate)
		SET @EndDate = @MaxDate
	IF(@BeginDate < @MinDate OR @BeginDate > @MaxDate)
		SET @BeginDate = @MinDate


	
	DECLARE @MonthYear TABLE(
		[MonthYearID] nvarchar (20) NOT NULL
	);

	IF(@EndDate IS NULL OR MONTH(@EndDate) < MONTH(@BeginDate) AND YEAR(@EndDate) = YEAR(@BeginDate)) --to include the year
	BEGIN
		SELECT * FROM @MonthYear
		RETURN
	END
	ELSE
	BEGIN

		WHILE(@EndDate >= @BeginDate)
		BEGIN
			IF(MONTH(@EndDate) = 1)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('January ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 2)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('February ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 3)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('March ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 4)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('April ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 5)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('May ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 6)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('June ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 7)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('July ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 8)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('August ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 9)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('September ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 10)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('October ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 11)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('November ' + CONVERT(nvarchar,YEAR(@EndDate),4))
			ELSE IF(MONTH(@EndDate) = 12)
				INSERT INTO @MonthYear (MonthYearID) VALUES ('December ' + CONVERT(nvarchar,YEAR(@EndDate),4))
	
			SET @EndDate = DATEADD(month,-1,@EndDate) 

		END
	END
	SELECT * FROM @MonthYear
	RETURN
END



