
CREATE PROCEDURE [dbo].[usp_MonthlyJournal]
@MonthYear NVARCHAR (20), @Manager NVARCHAR (100), @Owner NVARCHAR (100), @Product NVARCHAR (30), @LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @UserGuid UNIQUEIDENTIFIER, @Gross BIT
AS
BEGIN
	SET NOCOUNT ON

	SET @MonthYear = (SELECT REPLACE(@MonthYear,'January ','1/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'February ','2/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'March ','3/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'April ','4/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'May ','5/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'June ','6/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'July ','7/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'August ','8/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'September ','9/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'October ','10/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'November ','11/1/'))
	SET @MonthYear = (SELECT REPLACE(@MonthYear,'January ','12/1/'))

	DECLARE @BeginDate DATE
	
	SELECT @BeginDate = @MonthYear

	DECLARE @EndDate DATE
	SELECT @EndDate=DATEADD(month,1,@BeginDate)

	EXEC usp_JournalList @BeginDate,@EndDate,@Manager,@Owner,@Product,@LoginSiteGuid,@SiteGuid,@UserGuid,@Gross
END
