CREATE PROCEDURE [rpt].[usp_MonthlyJournalSub] 
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
	-- Stored Procedure: [rpt].[usp_MonthlyJournalSub] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
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

	Declare @SitesParam nvarchar(max)
	Declare @ManagersParam nvarchar(max)
	Declare @OwnersParam nvarchar(max)
	Declare @ProductParam uniqueidentifier
	Declare @MonthYearParam nvarchar(20)
	Declare @GrossNetParam BIT
	Declare @SiteGuidParam UNIQUEIDENTIFIER
	Declare @UserGuidParam UNIQUEIDENTIFIER

	Set @SitesParam = @Sites
	Set @ManagersParam = @Managers
	Set @OwnersParam = @Owners
	Set @ProductParam = @Product
	Set @MonthYearParam = @MonthYear
	Set @GrossNetParam = @GrossNet
	Set @SiteGuidParam = @SiteGuid
	Set @UserGuidParam = @UserGuid

	DECLARE @BeginDate DATETIMEOFFSET(7)
	SET @BeginDate = rpt.udf_MonthYearToTimestamp(@MonthYearParam)

	DECLARE @EndDate DATETIMEOFFSET(7)
    SET @EndDate = DATEADD(MONTH, 1, @BeginDate)

	SELECT 
		monthlyJournal.ProductID,
		(Select ID from tblCompanies where CompanyGuid = a.ManagerGuid) as ManagerID, 
		(Select ID from tblCompanies where CompanyGuid = a.OwnerGuid) as OwnerID, 
		convert(date,[InventoryDate]) as InventoryDate,
		[Begin Inventory], 
		[Book Inventory],
		TransAmt,
		DetailTrans, 
		[24 Hr],
		Adjustment,
		[Bulk Issue],
		Defuel,Issue, 
		[Load Rack],
		[LR Receipt],
		Receipt,
		Rotation, 
		Transfer,
		s.ID as SiteID,
		s.Address1,
		s.Address2,
		CASE WHEN CONCAT(s.City, s.State, s.Zip) = '' THEN NULL 
			ELSE CONCAT(s.City, ', ', s.State, ' ', s.Zip) END as CityStateZip,
		s.Country,
		s.Phone, 
		lookup.tblEngineeringUnit.EngineeringUnitName AS VolumeUnitName,
		monthlyJournal.VolumeDecimalPlaces
	FROM (Select * from [rpt].[udf_MonthlyJournalBaseTable](@SitesParam,@ManagersParam,@OwnersParam,@ProductParam)) a
	CROSS APPLY rpt.udf_MonthlyJournalSub(a.SiteGuid,a.ManagerGuid,a.OwnerGuid,@ProductParam,@BeginDate,@EndDate,@GrossNetParam, @SiteGuidParam, @UserGuidParam, 1, @EnterpriseStatus) monthlyjournal
	INNER JOIN tblSites s ON a.SiteGuid = s.SiteGuid
	INNER JOIN lookup.tblEngineeringUnit ON lookup.tblEngineeringUnit.EngineeringUnitIndex = monthlyJournal.VolumeUnitIndex
	ORDER BY InventoryDate

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
						+ 'Procedure Name: [rpt].usp_MonthlyJournalSub' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END