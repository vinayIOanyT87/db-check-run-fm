CREATE PROCEDURE [rpt].[usp_DsMonthYearList_LongMonth]

AS
BEGIN
	DECLARE @BeginDate DATETIMEOFFSET(7)
	DECLARE @EndDate DATETIMEOFFSET(7)
	DECLARE @NewEndDate DATETIMEOFFSET(7)
	 
	DECLARE @DateRange TABLE (
		[EndDate] DATETIMEOFFSET(7) ,
		[BeginDate] DATETIMEOFFSET(7)
	);

	INSERT INTO @DateRange SELECT MAX(InventoryDate) EndDate, MIN(InventoryDate) BeginDate FROM tblTransactions
	
	SET @BeginDate = (SELECT BeginDate FROM @DateRange)
	SET @EndDate = (SELECT EndDate FROM @DateRange)
	SET @NewEndDate = (SELECT dateadd (mm,-12,@EndDate))
	DECLARE @MonthYear TABLE(
		[MonthYearID] nvarchar (20) NOT NULL
	);

	NextMonth:

	--IF(@EndDate IS NULL OR MONTH(@EndDate) < MONTH(@BeginDate))
	IF(@EndDate IS NULL OR MONTH(@EndDate) < MONTH(@BeginDate) AND YEAR(@EndDate) = YEAR(@BeginDate)) --to include the year
	BEGIN
		SELECT * FROM @MonthYear
		RETURN
	END
	
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
	
	--SET @EndDate = (SELECT DATEADD(month,-1,@EndDate) FROM @DateRange WHERE @EndDate >=@NewEndDate)--go back to 1 year
	SET @EndDate = (SELECT DATEADD(month,-1,@EndDate) FROM @DateRange WHERE @EndDate >= @BeginDate) --list for all years in database

	GOTO NextMonth
/*
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
	 
	SET @EndDate = (SELECT DATEADD(month,-1,@EndDate))

	GOTO NextMonth
	*/
END