CREATE PROCEDURE [rpt].[usp_SummaryJournal] 
(
	@Sites nvarchar(max),
	@Managers nvarchar(max),
	@Owners nvarchar(max),
	@Product uniqueidentifier,
	@MonthYear nvarchar(20),
	@GrossNet BIT,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER,
	@EnterpriseStatus BIT
)
AS 
BEGIN

	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_SummaryJournal] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Summary Journal Report
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: Product MasterRecordGuid that the transactions list as the product for itself to be included in the results
	-- 5. @MonthYear: Month and year to filter transactions
	-- 6. @GrossNet: Bit indicating whether to return the Gross or Net quantity values in the transaction list.
	-- 7. @SiteGuid: Identifies the site the report is being run from
	-- 8. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @LocalResults TABLE 
		(
			[OwnerID]			nvarchar(100),
			InventoryDate		datetimeoffset(7),
			[Begin Inventory]	float,
			[Book Inventory]    float,
			[TransAmt]			float,
			[DetailTrans]		float,
			[24 Hr]				float,
			[Adjustment]		float,
			[Bulk Issue]		float,
			[Defuel]			float,
			[Issue]				float,
			[Load Rack]			float,
			[LR Receipt]		float,
			[Receipt]			float,
			[Rotation]			float,
			[Transfer]			float,
			[ManagerID]			nvarchar(100),
			[SiteID]			nvarchar(30),
			[Address1]			nvarchar(30),
			[Address2]			nvarchar(30),
			[CityStateZip]		nvarchar(60),
			[Country]			nvarchar(30),
			[Phone]				nvarchar(20),
			[VolumeUnitIndex]	INT,
			[VolumeDecimalPlaces] TINYINT,
			[ProductID]			nvarchar(30)
			primary key NONCLUSTERED( InventoryDate, SiteID,ManagerID, OwnerID)
		)	

		DECLARE @BeginDate datetimeoffset(7)
		Set @BeginDate = rpt.udf_MonthYearToTimestamp(@MonthYear)

		DECLARE @EndDate datetimeoffset(7)
    	Set @EndDate= DATEADD(month,1,@BeginDate)

		INSERT INTO @LocalResults
		SELECT 
		(Select ID from tblCompanies where CompanyGuid = a.OwnerGuid),
		convert(date,results.[InventoryDate]) as InventoryDate,
		results.[Begin Inventory],
		results.[Book Inventory],
		results.[TransAmt],
		results.[DetailTrans],
		results.[24 Hr],
		results.[Adjustment],
		results.[Bulk Issue],
		results.[Defuel],
		results.[Issue],
		results.[Load Rack],
		results.[LR Receipt],
		results.[Receipt],
		results.[Rotation],
		results.[Transfer],
		(Select ID from tblCompanies where CompanyGuid = a.ManagerGuid),
		s.[ID],
		s.[Address1],
		s.[Address2],
		CASE WHEN CONCAT(s.City, s.State, s.Zip) = '' THEN NULL 
			ELSE CONCAT(s.City, ', ', s.State, ' ', s.Zip) END,
		s.[Country],
		s.[Phone],
		results.VolumeUnitIndex,
		results.VolumeDecimalPlaces,
		results.ProductID
		FROM (Select * from [rpt].[udf_MonthlyJournalBaseTable](@Sites,@Managers,@Owners,@Product)) a
		CROSS APPLY rpt.udf_MonthlyJournalSub(a.SiteGuid,a.ManagerGuid,a.OwnerGuid,@Product,@BeginDate,@EndDate,@GrossNet, @SiteGuid, @UserGuid,1, @EnterpriseStatus) results
		INNER JOIN tblSites s ON a.SiteGuid = s.SiteGuid

		DECLARE @InventoryDateResults TABLE 
		(
			[OwnerID]			nvarchar(100),
			[ManagerID]			nvarchar(100),
			[SiteID]			nvarchar(30),
			MinInventoryDate		datetimeoffset(7),
			MaxInventoryDate		datetimeoffset(7)
		)

		INSERT INTO @InventoryDateResults
		SELECT OwnerID,ManagerID,SiteID,
		MIN(InventoryDate) AS MinInventoryDate, 
		MAX(InventoryDate) AS MaxInventoryDate
		FROM @LocalResults
		GROUP BY OwnerID,ManagerID,SiteID

		DECLARE @BeginInventoryResults TABLE 
		(
			[OwnerID]			nvarchar(100),
			[ManagerID]			nvarchar(100),
			[SiteID]			nvarchar(30),
			[Begin Inventory]	float,
			primary key NONCLUSTERED( [SiteID], [ManagerID], [OwnerID])
		)

		INSERT INTO @BeginInventoryResults
		SELECT a.OwnerID,a.ManagerID,a.SiteID,SUM(a.[Begin Inventory]) AS [Begin Inventory]
		FROM @LocalResults a
		INNER JOIN @InventoryDateResults b
		ON a.OwnerID = b.OwnerID AND a.ManagerID = b.ManagerID AND a.SiteID = b.SiteID AND a.InventoryDate = b.MinInventoryDate
		GROUP BY a.OwnerID, a.ManagerID, a.SiteID

		DECLARE @BookInventoryResults TABLE 
		(
			[OwnerID]			nvarchar(100),
			[ManagerID]			nvarchar(100),
			[SiteID]			nvarchar(30),
			[Book Inventory]    float,
			primary key NONCLUSTERED ( [SiteID], [ManagerID], [OwnerID])	
			
		)

		INSERT INTO @BookInventoryResults
		SELECT a.OwnerID,a.ManagerID,a.SiteID,SUM(a.[Book Inventory]) AS [Book Inventory]
		FROM @LocalResults a
		INNER JOIN @InventoryDateResults b
		ON a.OwnerID = b.OwnerID AND a.ManagerID = b.ManagerID AND a.SiteID = b.SiteID AND a.InventoryDate = b.MaxInventoryDate
		GROUP BY a.OwnerID, a.ManagerID, a.SiteID

		SELECT a.OwnerID, 
		a.ManagerID,
		a.SiteID,
		a.Address1,
		a.Address2,
		a.CityStateZip,
		a.Country,
		a.Phone,
		SUM(Issue) AS Issue_Total, 
		SUM([24 Hr]) AS ID_24_Hr_Total, 
		SUM([Adjustment]) AS Adjustment_Total, 
		SUM([Bulk Issue]) AS Bulk_Issue_Total, 
		SUM([Defuel]) AS Defuel_Total, 
		SUM([Load Rack]) AS Load_Rack_Total, 
		SUM([LR Receipt]) AS LR_Receipt_Total, 
		SUM([Receipt]) AS Receipt_Total, 
		SUM([Rotation]) AS Rotation_Total, 
		SUM([Transfer]) AS Transfer_Total,
		b.[Begin Inventory] AS [Begin Inventory], 
		c.[Book Inventory] AS [Book Inventory],
		lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName,
		a.VolumeDecimalPlaces,
		a.ProductID
		FROM @LocalResults a 
		INNER JOIN @BeginInventoryResults b ON a.OwnerID = b.OwnerID AND a.ManagerID = b.ManagerID AND a.SiteID = b.SiteID
		INNER JOIN @BookInventoryResults c ON a.OwnerID = c.OwnerID AND a.ManagerID = c.ManagerID AND a.SiteID = c.SiteID
		INNER JOIN tblSites ON tblSites.ID = a.SiteID
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = a.VolumeUnitIndex
		GROUP BY a.OwnerID, a.ManagerID, a.SiteID, a.Address1, a.Address2, a.CityStateZip, a.Country, a.Phone, b.[Begin Inventory],c.[Book Inventory], lookup.tblEngineeringUnit.EngineeringUnitName,a.VolumeDecimalPlaces,a.ProductID

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [rpt].usp_SummaryJournal' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END