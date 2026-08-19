CREATE PROCEDURE [dbo].[usp_AccountingTransactionSummary]
	@SiteGuid UNIQUEIDENTIFIER,
	@BeginDate DATE,
	@EndDate DATE,
	@AliasName NVARCHAR(32) = NULL,
	@FindText NVARCHAR(22) = NULL,
	@DocumentNumber NVARCHAR(30) = NULL,
	@PageStart INT,
	@PageLength INT,
	@OrderBy NVARCHAR(1000)
AS
BEGIN
	SET NOCOUNT ON;

	-- Start building the SQL to get the information we want to display on the Transaction Summary screen.
	-- The SELECT COUNT(*) OVER will return the total record count as a part of each row. This is an alternative to issuing a separate, similar query to get the count.
	-- We need the total count so we can tell the user how many records matched their search - keep in mind that we only return a subset (page) of all of the records
	DECLARE @SQL NVARCHAR(MAX) = N'SELECT COUNT(*) OVER() AS RecordCount, dbo.vw_TransactionSummary.*, lookup.tblTransactionStatus.TransactionStatusName AS TransactionStatus
                      FROM dbo.vw_TransactionSummary
                      LEFT OUTER JOIN lookup.tblTransactionStatus ON lookup.tblTransactionStatus.TransactionStatusIndex = LookupTransactionStatusIndex
					  WHERE SiteGuid = @SiteGuid';

	-- Search by inventory starting date, if it was provided.
	IF (@BeginDate IS NOT NULL)
	BEGIN
		SET @SQL += N' AND InventoryDate >= @BeginDate';
	END;

	-- Search by inventory ending date, if it was provided.
	IF (@EndDate IS NOT NULL)
	BEGIN
		SET @SQL += N' AND InventoryDate <= @EndDate';
	END;

	-- Search by the Transaction Alias Name if it was provided
	IF (@AliasName IS NOT NULL)
	BEGIN
		SET @SQL += N' AND AliasName = @AliasName'
	END

	-- Search by the Document Number, if it was provided.
	IF (@DocumentNumber IS NOT NULL)
	BEGIN
		SET @SQL += N' AND DocumentNumber = @DocumentNumber';
	END;

	-- Search most columns by the find text if it was provided. We don't search the date columns.
	-- Note that the application adds the percent signs before and after the findText.
	IF (@FindText IS NOT NULL)
	BEGIN
		SET @SQL += N' AND (
					AliasName LIKE @FindText
					OR lookup.tblTransactionStatus.TransactionStatusName LIKE @FindText
					OR OwnerID LIKE @FindText
					OR ManagerID LIKE @FindText
					OR ShipToID LIKE @FindText
					OR Product LIKE @FindText
					OR GrossQuantity LIKE @FindText
					OR NetQuantity LIKE @FindText)'
	END

	-- Add the dynamic order by clause, which is built in the application. The dynamic nature of the order by is the main reason
	-- for using dynamic sql in this stored procedure
	SET @SQL += N' ORDER BY ' + @OrderBy

	-- Page the results. Starting at record @PageStart in the result set, get the next @PageLength records
	SET @SQL += N' OFFSET @PageStart ROWS FETCH NEXT @PageLength ROWS ONLY'

	DECLARE @Params NVARCHAR(MAX)
	
	SET @Params = N'@SiteGuid UNIQUEIDENTIFIER,
		@BeginDate DATE,
		@EndDate DATE,
		@AliasName NVARCHAR(32),
		@FindText NVARCHAR(22),
		@DocumentNumber NVARCHAR(30),
		@PageStart INT,
		@PageLength INT'

	EXEC sp_executesql @SQL, @Params,
		@SiteGuid = @SiteGuid,
		@BeginDate = @BeginDate,
		@EndDate = @EndDate,
		@AliasName = @AliasName,
		@FindText = @FindText,
		@DocumentNumber = @DocumentNumber,
		@PageStart = @PageStart,
		@PageLength = @PageLength
END