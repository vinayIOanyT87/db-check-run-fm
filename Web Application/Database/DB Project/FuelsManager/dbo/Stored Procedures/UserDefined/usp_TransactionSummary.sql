CREATE PROCEDURE [dbo].[usp_TransactionSummary]
	@LoginSiteGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@BeginDate DATE,
	@EndDate DATE,
	@AliasName NVARCHAR(32) = NULL,
	@FindText NVARCHAR(22) = NULL,
	@PageStart INT,
	@PageLength INT,
	@OrderBy NVARCHAR(1000)
AS
BEGIN
	SET NOCOUNT ON


	-- Start building the SQL to get the information we want to display on the Transaction Summary screen.
	-- The SELECT COUNT(*) OVER will return the total record count as a part of each row. This is an alternative to issuing a separate, similar query to get the count.
	-- We need the total count so we can tell the user how many records matched their search - keep in mind that we only return a subset (page) of all of the records
	DECLARE @SQL NVARCHAR(MAX) = N'DECLARE @AuthorizedCompanies TABLE ([ID] [nvarchar] (100) NOT NULL)

								INSERT INTO @AuthorizedCompanies SELECT ID FROM dbo.udf_AuthorizedCompanies(@LoginSiteGuid, @SiteGuid, @UserGuid)

								SELECT COUNT(*) OVER() AS RecordCount, dbo.vw_TransactionSummary.*
											FROM dbo.vw_TransactionSummary
											WHERE SiteGuid = @SiteGuid
											AND InventoryDate BETWEEN @BeginDate AND @EndDate
											AND (@UserGuid IS NULL
													OR EXISTS (SELECT 1 FROM @AuthorizedCompanies ac
													WHERE ac.ID IN (ManagerID, OwnerID, ShipToID, CarrierID, ShipperID, SupplierID, BillToID) ) )'

	-- Search by the Transaction Alias Name if it was provided
	IF (@AliasName IS NOT NULL)
	BEGIN
		SET @SQL += N' AND AliasName = @AliasName'
	END

	-- Search most columns by the find text if it was provided. We don't search the date columns.
	-- Note that the application adds the percent signs before and after the findText.
	IF (@FindText IS NOT NULL)
	BEGIN
		SET @SQL += N' AND (
					AliasName LIKE @FindText
					OR dbo.vw_TransactionSummary.TransactionStatus LIKE @FindText
					OR OwnerID LIKE @FindText
					OR ManagerID LIKE @FindText
					OR ShipToID LIKE @FindText
					OR Product LIKE @FindText
					OR GrossQuantity LIKE @FindText
					OR NetQuantity LIKE @FindText
					OR DocumentNumber LIKE @FindText)'
	END

	-- Add the dynamic order by clause, which is built in the application. The dynamic nature of the order by is the main reason
	-- for using dynamic sql in this stored procedure
	SET @SQL += N' ORDER BY ' + @OrderBy

	-- Page the results. Starting at record @PageStart in the result set, get the next @PageLength records
	SET @SQL += N' OFFSET @PageStart ROWS FETCH NEXT @PageLength ROWS ONLY'

	DECLARE @Params NVARCHAR(MAX)
	
	SET @Params = N'@LoginSiteGuid UNIQUEIDENTIFIER,
		@SiteGuid UNIQUEIDENTIFIER,
		@UserGuid UNIQUEIDENTIFIER,
		@BeginDate DATE,
		@EndDate DATE,
		@AliasName NVARCHAR(32),
		@FindText NVARCHAR(22),
		@PageStart INT,
		@PageLength INT'

	EXEC sp_executesql @SQL, @Params,
		@LoginSiteGuid = @LoginSiteGuid,
		@SiteGuid = @SiteGuid,
		@UserGuid = @UserGuid,
		@BeginDate = @BeginDate,
		@EndDate = @EndDate,
		@AliasName = @AliasName,
		@FindText = @FindText,
		@PageStart = @PageStart,
		@PageLength = @PageLength
END
