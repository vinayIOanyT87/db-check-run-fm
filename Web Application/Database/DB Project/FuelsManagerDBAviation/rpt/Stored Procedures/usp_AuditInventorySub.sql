CREATE PROCEDURE [rpt].[usp_AuditInventorySub] 
(
	@Sites NVARCHAR(MAX),
	@Managers NVARCHAR(MAX),
	@Owners NVARCHAR(MAX),
	@Product UNIQUEIDENTIFIER,
	@MonthYear NVARCHAR(20),
	@GrossNet BIT,
	@SiteGuid UNIQUEIDENTIFIER,
	@UserGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[usp_AuditInventorySub] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Audit Inventory Report
	--          for single product and multiple  managers, owners, sites.
	-- Notes:
	-- 1. @Sites: List of SiteGuids (not SiteGroups) to retrieve the transactions from.
	-- 2. @Managers: List of company MasterRecordGuids assigned the role of manager that the transactions list as the manager for itself to be included in the results
	-- 3. @Owners: List of company MasterRecordGuids assigned the role of owner that the transactions list as the owner for itself to be included in the results
	-- 4. @Product: Product MasterRecordGuid that is associated witht he transactions that are included in the transaction list
	-- 5. @MnthYear: Month and year to collect transactions meeting criteria
	-- 6. @GrossNet: Bit specifying whether the quantity values are returned as Gross or Net
	-- 7. @SiteGuid: Identifies the site the report is being run from
	-- 8. @UserGuid: Identifies the user running the report
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SET NOCOUNT ON

		DECLARE @BeginDate DATETIMEOFFSET(7)

		--Translating MonthYear into a begin and end date
		SET @BeginDate = rpt.udf_MonthYearToTimestamp(@MonthYear)
		DECLARE @EndDate DATETIMEOFFSET(7)
		SET @EndDate = DATEADD(month,1,@BeginDate)
		SET @EndDate = DATEADD(s,-1,@EndDate)

		DECLARE @BookInventoryTable TABLE
		(
			OwnerID				NVARCHAR(100),
			[SiteID]			NVARCHAR(100),
			InventoryDate		DATETIMEOFFSET(7),
			[Book Inventory]	FLOAT,
			ManagerID			NVARCHAR(100)
		)

		INSERT INTO @BookInventoryTable
		SELECT OwnerID
			,[SiteID]
			,InventoryDate
			,CASE WHEN [Book Inventory] = '' THEN 0 ELSE [Book Inventory] END AS [Book Inventory]
			,ManagerID
		FROM (SELECT * FROM rpt.udf_GetTableFromStringList(@Owners)) mgr
		CROSS APPLY rpt.udf_AuditInventory (@Sites, @Managers, mgr.Guid, @Product, @BeginDate, @EndDate, @GrossNet, @SiteGuid, @UserGuid) 
		GROUP BY OwnerID,ManagerID, SiteID,InventoryDate,[Book Inventory]
		ORDER BY OwnerID,ManagerID,SiteID

		/**********************
			BEGIN Total Percentage
		***********************/

		DECLARE @total TABLE
		(	
			InventoryDate	DATETIME
			,[Total]		DECIMAL
			,ManagerID		VARCHAR(100)
			,SiteID			VARCHAR(100)
		)

		INSERT INTO @total
		SELECT InventoryDate, 
			SUM([Book Inventory]) AS [Total],
			ManagerID,
			SiteID
		FROM @BookInventoryTable
		GROUP BY InventoryDate, ManagerID, SiteID
		ORDER BY InventoryDate, ManagerID, SiteID

		/**********************
			END Total Percentage
		***********************/

		/*****************
			MAIN QUERY
		******************/
		SELECT te.OwnerID, te.ManagerID, te.[SiteID]
			,CAST(te.InventoryDate AS DATE) AS InventoryDate
			,te.[Book Inventory]
			,[Total Inventory] = tt.[Total]
			,[Total Percentage] = COALESCE((100 * te.[Book Inventory]) / (NULLIF(tt.[Total], 0)), 0)
			,s.City AS SiteCity
			,s.State AS SiteState
			,lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName
		FROM @BookInventoryTable te
		INNER JOIN @total tt ON te.InventoryDate = tt.InventoryDate AND tt.ManagerID=te.ManagerID AND te.SiteID = tt.SiteID
		INNER JOIN tblSites s ON s.ID = te.SiteID
		INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = s.VolumeUnitIndex

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
						+ 'Procedure Name: [rpt].usp_AuditInventorySub' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END