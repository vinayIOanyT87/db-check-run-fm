USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_MonthlyLedger') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_MonthlyLedger
GO

CREATE PROCEDURE dbo.[rpt_sp_ta_MonthlyLedger]

/** =============================================
 Author:		Kimberly Foote	  
 Create date: 12/11/2008
 Description:	Exec rpt_sp_JournalList_All_Product
				Replaces the fm_MonthlyJournal. This 
				is used to give capability of selecting "All" for
				parameters @Product. Did not want to modify 
				Original sp.
 Version:		7.5.1.2
 Execution:
		Execute rpt_sp_ta_MonthlyLedger 'January 2010','3427 - CITGO Petroleum Corp','<All>','4001',1,1,2,0,0,0
											
 Modification History:
Date		By		Description
5/14/09		KF		Add @ActiveProducts and @LockedOut in order to show products
					that are active.
5/14/09		KF		Add ProductType in(0,2) to exclude Additive Products, but additive
					products will need to be included when selecting a Tracked Product.
5/14/09		KF		Add @TrackedProducts to pull only tracked products including ProductType 1 only.
5/19/09		KF		Add #temp table to do the grouping by stockholder, product, Inventorydate. This
					will pull only ledgers that don't have all zero's for that month.
5/21/09		KF		Modified DATEADD(month,1,@BeginDate) to DATEADD(month,DATEDIFF(month,-1,@BeginDate),-1)
					to pull last day of month.
6/2/09		KF		changing criteria for tracked product (TrackingProductIndex <> '' or TrackingProductIndex <> null)
7/1/09		KF		Modified @Tracked Products
7/23/09		KF		Rename to standard sp from rpt_citgo_sp_ta_MonthlyLedger to rpt_sp_ta_MonthlyLedger
12/10/2009	KF		Version 7.5.1.0
3/8/2010	KF		AuthorizedCompanies replaced by rpt_fn_ta_AuthorizedCompanies
3/9/2010	KF		Has incorrect sp for temp table change rpt_citgo_sp_ta_LedgerList to rpt_sp_ta_LedgerList.sql
04/15/2011  AL		Included missing columns on DECLARATION session on variable table @TempTable to resolve Bug 22303 

 =============================================***/

	@MonthYear nvarchar(20),
	@Manager nvarchar(30),
	@Owner nvarchar(30),
	@Product nvarchar(30),
	@LoginSiteIndex int,
	@SiteIndex int,
	@UserIndex int,
	@Gross bit,
	@ActiveProducts bit,
	@TrackedProducts bit
AS
BEGIN
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

	DECLARE @BeginDate datetime

	SELECT @BeginDate = @MonthYear

	DECLARE @EndDate datetime
	SELECT @EndDate= DATEADD(month,DATEDIFF(month,-1,@BeginDate),-1) --***** FIXED pull last day of month******

	DECLARE @AuthorizedCompanies TABLE (CompanyID nvarchar(30))
	INSERT INTO @AuthorizedCompanies SELECT * FROM rpt_fn_ta_AuthorizedCompanies(@LoginSiteIndex, @SiteIndex,@UserIndex)

	declare @TempTable table
	( 
		[Inventory Date] datetime, 
		[Begin Inventory] float, 
		[Book Inventory] float, 
		[Adjustment] float, 
		[BOL] float, 
		[Issue] float,
		[Meter Closeout] float, 
		[Receipt] float, 
		[Regrade] float, 
		[Shipment] float, 
		[Test Loss] float,
		[Test_Acct_payable_Invoice] float,
		[Test_Acct_Receivable_Invoice] float,
		[Test_Adjust_Primary_Storage] float,
		[Test_Adjust_Secondary_Storage] float,
		[Test_Diff_Storage_Equipment] float,
		[Test_No_Effect_Storage] float,
		[Test_Prod_Disb_Primary_Storage] float,
		[Test_Prod_Disb_Second_Storage] float,
		[Test_receipt] float,
		[Test_Regrade_Primary] float,
		[Test_Regrade_Secondary] float,
		[Test_Return_Primary] float,
		[Test_Return_Secondary] float,
		[Test_Transfer_Owner_Owner] float,
		[TestBarter] float,
		[Transfer] float
	)

	declare @JournalReportFinal table( ProductID nvarchar(30), ProductType int, LockedOut bit,TrackingProductIndex int, OwnerID nvarchar(30), [Inventory Date] datetime, [Begin Inventory] float, [Book Inventory] float, Adjustment float,
									   BOL float, [Meter Closeout] float, Receipt float, Regrade float, Shipment float, Transfer float)

/********
Product and Owner List
**********/
DECLARE @ProductList TABLE ( ProductID nvarchar(30),ProductType int,LockedOut bit, TrackingProductIndex int)

IF @TrackedProducts = cast(0 as bit)

BEGIN

IF (@Product = '<All>')and @ActiveProducts = cast(0 as bit) --0 is Active Products, 1 is not Active Products
																   
	BEGIN
		INSERT INTO @ProductList	SELECT	ProductID, ProductType,LockedOut,TrackingProductIndex
									FROM	dbo.tblProducts 
									WHERE	SiteIndex = @SiteIndex and 
											ProductType in(0,2)
	END	
ELSE
	BEGIN
		INSERT INTO @ProductList	SELECT	ProductID, ProductType,LockedOut,TrackingProductIndex
									FROM	dbo.tblProducts 
									WHERE	SiteIndex = @SiteIndex and
											ProductID = @Product and 
											ProductType in(0,2)
	END


IF (@Product = '<All>') and @ActiveProducts = cast(1 as bit) --0 is Active Products, 1 is not Active Products
	
BEGIN
		INSERT INTO @ProductList	SELECT	ProductID, ProductType,LockedOut,TrackingProductIndex
									FROM	dbo.tblProducts 
									WHERE	SiteIndex = @SiteIndex and
											LockedOut = cast(0 as bit) and 
											ProductType in(0,2)
										
	END	
ELSE
	BEGIN
		INSERT INTO @ProductList	SELECT	ProductID, ProductType,LockedOut,TrackingProductIndex
									FROM	dbo.tblProducts 
									WHERE	SiteIndex = @SiteIndex and
											ProductID = @Product  and
											LockedOut = cast(0 as bit) and 
											ProductType in(0,2)
	END
END
ELSE
	BEGIN

IF @TrackedProducts = cast(1 as bit)

 BEGIN

IF (@Product = '<All>')and @ActiveProducts = cast(0 as bit) --0 is Active Products, 1 is not Active Products
																   
	BEGIN
		INSERT INTO @ProductList	SELECT	distinct p.ProductID, tp.ProductType,tp.LockedOut,tp.TrackingProductIndex ---*****Fixed 7/1/2009*****
									FROM	dbo.tblProducts p 
												Join dbo.tblProducts tp on
														tp.TrackingProductIndex = p.ProductIndex
									WHERE	p.SiteIndex = @SiteIndex 
											
	END	
ELSE
	BEGIN
		INSERT INTO @ProductList	SELECT	p.ProductID, tp.ProductType,tp.LockedOut,tp.TrackingProductIndex ---*****Fixed 7/1/2009*****
									FROM	dbo.tblProducts p 
												Join dbo.tblProducts tp on
														tp.TrackingProductIndex = p.ProductIndex
									WHERE	p.SiteIndex = @SiteIndex and
											p.ProductID = @Product 
											
	END

IF (@Product = '<All>') and @ActiveProducts = cast(1 as bit) --0 is Active Products, 1 is not Active Products
	
BEGIN
		INSERT INTO @ProductList	SELECT	distinct p.ProductID, tp.ProductType,tp.LockedOut,tp.TrackingProductIndex ---*****Fixed 7/1/2009*****
									FROM	dbo.tblProducts p
												Join dbo.tblProducts tp on
														tp.TrackingProductIndex = p.ProductIndex
									WHERE	p.SiteIndex = @SiteIndex and
											p.LockedOut = cast(1 as bit)

								
	END	
ELSE
	BEGIN
		INSERT INTO @ProductList	SELECT	distinct p.ProductID, tp.ProductType,tp.LockedOut,tp.TrackingProductIndex ---*****Fixed 7/1/2009*****
									FROM	dbo.tblProducts p
												Join dbo.tblProducts tp on
														tp.TrackingProductIndex = p.ProductIndex
									WHERE	p.SiteIndex = @SiteIndex and
											p.ProductID = @Product  and
											p.LockedOut = cast(1 as bit)
										
	END
END

	END


DECLARE @OwnerList TABLE
    (ProductID nvarchar (30) NOT NULL,
	 ProductType int,
	 LockedOut bit,
	 TrackingProductIndex int,
	 OwnerID nvarchar (30) NOT NULL)

IF (@Owner = '<All>')
	BEGIN
		INSERT INTO @OwnerList		SELECT Distinct ProductID, ProductType, LockedOut,TrackingProductIndex,CompanyName AS OwnerID
									FROM dbo.CompanyList(@LoginSiteIndex,@SiteIndex,1,0), @ProductList
									WHERE CompanyName IN(SELECT * FROM @AuthorizedCompanies) 
									ORDER BY CompanyName, ProductID
	END
ELSE
	BEGIN
		INSERT INTO @OwnerList		SELECT DISTINCT ProductID, ProductType, LockedOut,TrackingProductIndex,CompanyName AS OwnerID
									FROM dbo.CompanyList(@LoginSiteIndex,@SiteIndex,1,0), @ProductList
									WHERE CompanyName = @Owner
									ORDER BY CompanyName, ProductID
	END


DECLARE ProdOwnerCursor CURSOR FOR SELECT * FROM @OwnerList 
DECLARE @ProductType int
DECLARE @LockedOut bit
DECLARE @TrackingProductIndex int



	/*****
	Main Query
	*******/
	OPEN ProdOwnerCursor
	FETCH NEXT FROM ProdOwnerCursor INTO @Product, @ProductType, @LockedOut,@TrackingProductIndex,@Owner

	WHILE @@FETCH_STATUS = 0
	BEGIN

		insert @TempTable(
			[Inventory Date],
			[Begin Inventory],
			[Book Inventory],
			[Adjustment],
			[BOL],
			[Meter Closeout],
			[Receipt],
			[Regrade],
			[Shipment],
			[Transfer]
			)
		EXEC  rpt_sp_ta_LedgerList @BeginDate,@EndDate,@Manager,@Owner,@Product,@LoginSiteIndex,@SiteIndex,@UserIndex,@Gross
	
	
		insert into @JournalReportFinal select	@Product, @ProductType, @LockedOut,@TrackingProductIndex,@Owner, [Inventory Date], [Begin Inventory], [Book Inventory], Adjustment, BOL, [Meter Closeout], Receipt, Regrade, Shipment, Transfer			
			from @TempTable

		delete from @TempTable

		FETCH NEXT FROM ProdOwnerCursor INTO @Product, @ProductType, @LockedOut,@TrackingProductIndex,@Owner
	END
	CLOSE ProdOwnerCursor



--/**********************
--	BEGIN GROUP QUERY
--***********************/
Select distinct
	 OwnerID
	,ProductID

INTO #temp

From @JournalReportFinal

Where ([Begin Inventory] <> 0 and [Book Inventory] <> 0)

Group by 
OwnerID,
ProductID,
[Inventory Date],
[Begin Inventory],
[Book Inventory]

Order by
OwnerID,
ProductID

/**********************
	END GROUP QUERY
***********************/

/*****************
	MAIN QUERY
******************/

Select * 
From @JournalReportFinal a, #Temp b
where a.OwnerID = b.OwnerID and a.ProductID = b.ProductID 



drop table #temp

END

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_MonthlyLedger TO [public]
GO