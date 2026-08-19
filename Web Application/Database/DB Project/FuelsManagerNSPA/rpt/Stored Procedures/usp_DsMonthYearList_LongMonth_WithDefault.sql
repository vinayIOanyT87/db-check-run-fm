CREATE PROCEDURE [rpt].[usp_DsMonthYearList_LongMonth_WithDefault]

AS
BEGIN
	DECLARE @MonthYear TABLE(
		[MonthYearID] nvarchar (20) NOT NULL
	);

	INSERT INTO @MonthYear (MonthYearID) VALUES ('<Latest Available>')
	INSERT INTO @MonthYear EXEC [rpt].[usp_DsMonthYearList_LongMonth]

	SELECT * FROM @MonthYear
	
END