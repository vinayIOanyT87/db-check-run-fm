-- ==========================================================================================
-- Description:	Stored procedure to add a default report configuration for Standard and CITGO reports into the system
-- Author:		FJM
-- Create date: 2022-10-12/1.0.0
-- ==========================================================================================
CREATE PROCEDURE usp_AddDefaultReportConfiguration
	@Siteguid uniqueIdentifier
AS
BEGIN
	IF NOT EXISTS ( SELECT 1 FROM sys.databases WHERE name = 'ReportServer' ) 
	BEGIN
		RAISERROR('Reporting Server Database is not in the same DB Server as FuelsManager', 16, 1)
	END

	SET NOCOUNT ON

	CREATE TABLE #tblReportGroups(
		[GroupName] [nvarchar](30) NOT NULL,
		[OrderNumber] [int] NULL,
		[ReportGroupGuid] [uniqueidentifier] NOT NULL,
	) 

	CREATE TABLE [#tblReportDetails] (
	[ReportName] [nvarchar] (60),
	[ReportDescription] [nvarchar] (255),
	[ReportPath] [nvarchar] (200),
	[OrderNumber] [int] NULL,
	[PrintOnlyFlag] [bit] NULL,
	[PrimaryPrinterName] [nvarchar] (100) NULL,
	[SecondaryPrinterName] [nvarchar] (100) NULL,
	[PrintAtEndOfDay] [bit] NULL,
	[PrintAtEndOfMonth] [bit] NULL,
	[ReportGroup] [nvarchar] (100) NULL,
	[ReportDetailGuid] [uniqueidentifier] NOT NULL
	)

	-- List of Report Groups
	INSERT #tblReportGroups (GroupName, OrderNumber, ReportGroupGuid)
	SELECT 'Operations', 1, newId()
	UNION 
	SELECT 'Tank Farm', 2, newId()
	UNION 
	SELECT 'Inventory Management', 3, newId()
	UNION 
	SELECT 'Accounting', 4, newId()

	-- Update the list of Report Groups
	MERGE tblReportGroups AS Target
	USING #tblReportGroups	AS Source
	ON Source.GroupName = Target.GroupName
	AND Target.SiteGuid = @Siteguid
	WHEN NOT MATCHED BY Target THEN
		INSERT (GroupName, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, OrderNumber,	ReportGroupGuid, SiteGuid)
		VALUES (Source.GroupName, 'administrator', SYSDATETIMEOFFSET(), 'administrator', SYSDATETIMEOFFSET(), Source.OrderNumber, Source.ReportGroupGuid, @SiteGuid);

	-- List of Report Details
	INSERT [#tblReportDetails] ([ReportName],[ReportDescription],[ReportPath],[OrderNumber],[PrintOnlyFlag],[PrimaryPrinterName],[SecondaryPrinterName],[PrintAtEndOfDay],[PrintAtEndOfMonth], [ReportGroup], [ReportDetailGuid])
       SELECT N'12 Hour Additive Report',N'12 Hour Additive Report',N'FM_12HrAdditiveReport',1,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId() 
       UNION ALL
       SELECT N'Additive Profiles Report',N'Lists configured additive profiles',N'FM_AdditiveProfilesReport',2,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Alarm and Event Log Report',N'List of Alarm and Events',N'FM_AlarmAndEventLogReport',3,0,N'{None}',N'{None}',0,0, 'Operations', newId() 
       UNION ALL
       SELECT N'Allocation Summary Report',N'Allocation Summary report',N'FM_AllocationSummaryReport',4,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Audit Log Report',N'Audit Log Report',N'FM_AuditLogReport',5,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'BOL report',N'View a Bill of Lading (BOL) for a particular day',N'FM_BOLReport',6,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Bulk Additive Report',N'Track the volume of bulk additives that go through a meter',N'FM_Bulk AdditiveReport',7,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Company List',N'List of companies',N'FM_CompanyReport',8,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Current Tank Inventory',N'Current Tank Inventory',N'IM_CurrentTankInventory',9,0,N'{None}',N'{None}',0,0, 'Tank Farm', newId() 
       UNION ALL
       SELECT N'Customer Account by Shipper',N'List of customers and the associated Load ID grouped by Shipper',N'FM_CustomerAccountInformation',10,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Customer Account with PIDX',N'Customer number list by Shipper with PIDX profile',N'FM_CustomerAccountInformationwithPIDX',11,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Delivered Product Report',N'Track product deliveries per BOL',N'FM_DeliveredProductReport',12,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Driver Activity', N'Truck loading activities per Driver and/or Carrier: when the Driver loaded fuel, how much gallons of fuel were loaded, time spent loading, time the Driver spent at the Terminal',N'FM_DriverActivityReport',13,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Equipment Trailer Report',N'Equipment Trailer Report',N'FM_EquipmentTrailerReport',14,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Inventory Reconciliation',N'Displays the month to date gains/losses for a selected Month, Manager, and Product',N'FM_InventoryReconciliation',15,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Inventory Recon Summary',N'Displays the month to date gains/losses for all Stockholders and Products by month',N'FM_InventoryReconciliationSummary',16,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Ledger',N'Shows transaction totals per month for the selected Manager, for all or a specific Site Group, Site, Stockholder, and Product',N'FM_Ledger',17,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Meter Reconciliation',N'View data collected for a meter to reconcile meters and check for meter skips or out of tolerance. Displays meter closeout values versus daily transactions for each meter.',N'FM_MeterReconciliation',18,0,N'{None}',N'{None}',0,0 , 'Inventory Management', newId()
       UNION ALL
       SELECT N'Meter Reconciliation Summary',N'Summary of meter transactions per Site by date',N'FM_MeterReconciliationSummaryReport',19,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Personnel Driver Report',N'List of Drivers by Company',N'FM_PersonnelDriverReport',20,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Personnel Driver Locked out',N'Allows you to view the status of Drivers, whether they are locked out or not from loading fuel at the Load Rack',N'FM_PersonnelDriverLockOutReport',21,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Product Report',N'Product list',N'FM_ProductsConfigurationReport',22,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'Receipt Report',N'View Receipt transactions per Product for a particular Manager within a specified period',N'FM_ReceiptReport',23,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Ship To - Product and Carrier',N'Ship To - Product and Carrier',N'FM_ShiptoCoAuthProductsAndCarriers',24,0,N'{None}',N'{None}',0,0 , 'Operations', newId()
       UNION ALL
       SELECT N'Stock Data Report',N'Track how much product (stock) is available per Stockholder',N'FM_StockData',25,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'Tank Inventory by Date',N'Tank Inventory by Date',N'IM_TankInventoryByDate',26,0,N'{None}',N'{None}',0,0, 'Tank Farm' , newId()
       UNION ALL
       SELECT N'Tank Change Report',N'Tank Change Report',N'IM_TankChangeReport',27,0,N'{None}',N'{None}',0,0, 'Tank Farm' , newId()
       UNION ALL
       SELECT N'Tank Change Report by Date/Time',N'Tank Change Report by Date/Time',N'IM_TankChangeReportDateTime',28,0,N'{None}',N'{None}',0,0, 'Tank Farm' , newId()
       UNION ALL
       SELECT N'Trailer Inspections Report', N'Track expiration dates for Tests and Inspections, as well as Tags and Licenses for Trailers by Carrier',N'FM_TrailerInspectionLicenseReport',29,0,N'{None}',N'{None}',0,0, 'Operations' , newId()
       UNION ALL
       SELECT N'Transaction Detail',N'View transactions that affect product inventory per Site, Stockholder, and product',N'FM_TransactionDetails',30,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'CITGO 12Hr Additive Ship To',N'View product with additives that were shipped to customers per month',N'CITGO_12HrAdditivewithShipToReport',31,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId() 
       UNION ALL
       SELECT N'CITGO Activity by Additive',N'View totals for products with additives delivered for a selected period',N'CITGO_ActivityByAdditiveCode',32,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'CITGO Activity by Product',N'View Product totals delivered for a selected period',N'CITGO_ActivityByProductCode',33,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'CITGO Additive Profile Ovr Sht',N'View Additives added to products being over or short of the desired additive amount. Totals by Additive Code',N'CITGO_AdditiveProfileOverShort',34,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'CITGO Additive Totals By Prod',N'Track the totals of additives per product within a specified period',N'CITGO_AdditiveTotalsByProduct',35,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()
       UNION ALL
       SELECT N'CITGO BOL Report',N'Allows you to print a BOL',N'CITGO_BOLReport',36,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId() 
       UNION ALL
       SELECT N'CITGO BOL Summary',N'Allows you to view multiple BOLs',N'CITGO_BOLSummaryReport',37,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId() 
       UNION ALL
       SELECT N'CITGO BOL Exception Report',N'List of broken blends, no line items, skipped BOL numbers, more than 8 batches, containing 0 density, and not posted to PIDX for a selected period',N'CITGO_BOLExceptionReport',38,0,N'{None}',N'{None}',0,0, 'Operations', newId()
       UNION ALL
       SELECT N'CITGO Product Movement Report',N'View totals of product moved (transferred) and received per Stockholder',N'CITGO_ProductMovementReport',39,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()  
       UNION ALL
       SELECT N'CITGO VAR Report',N'Shows the throughput of transactions containing additives used in a given month',N'CITGO_VARReport',40,0,N'{None}',N'{None}',0,0, 'Inventory Management', newId()


	MERGE tblReportDetails AS Target
	USING (SELECT rd.*, rg.ReportGroupGuid 
		FROM [#tblReportDetails] rd
		JOIN tblReportGroups rg
		ON rg.GroupName = rd.ReportGroup 
		AND rg.SiteGuid = @Siteguid
		JOIN [ReportServer].[dbo].[Catalog] c
		ON c.Type = 2
		AND c.path like '/Standard Reports/%'
		AND c.Name COLLATE SQL_Latin1_General_CP1_CI_AS = rd.ReportPath COLLATE SQL_Latin1_General_CP1_CI_AS
	)	AS Source
	ON Source.ReportName = Target.ReportName
	AND Target.SiteGuid = @Siteguid
	WHEN NOT MATCHED BY Target THEN
		INSERT (ReportName, ReportDescription, ReportPath, OrderNumber, PrintOnlyFlag, PrimaryPrinterName, SecondaryPrinterName, PrintAtEndOfDay, PrintAtEndOfMonth, 
					ReportGroupGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, SiteGuid, ReportDetailGuid)
		VALUES (Source.ReportName, Source.ReportDescription, Source.ReportPath, Source.OrderNumber, Source.PrintOnlyFlag, Source.PrimaryPrinterName, Source.SecondaryPrinterName, Source.PrintAtEndOfDay, Source.PrintAtEndOfMonth,
					Source.ReportGroupGuid, 'administrator', SYSDATETIMEOFFSET(), 'administrator', SYSDATETIMEOFFSET(), @SiteGuid, Source.ReportDetailGuid);

	MERGE [map].[tblGroupToReportDetail] AS Target
	USING (SELECT rd.*, rg.ReportGroupGuid 
		FROM [#tblReportDetails] rd
		JOIN tblReportDetails rg
		ON rg.ReportDetailGuid = rd.ReportDetailGuid 
	)	AS Source
	ON Source.ReportDetailGuid = Target.ReportDetailGuid
	AND Target.GroupGuid = '00000000-0000-0000-0000-000000000003'
	WHEN NOT MATCHED BY Target THEN
		INSERT ( GroupToReportDetailGuid, GroupGuid, ReportDetailGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
		VALUES (newId(), '00000000-0000-0000-0000-000000000003', Source.ReportDetailGuid, 'administrator', SYSDATETIMEOFFSET(), 'administrator', SYSDATETIMEOFFSET());


	DROP TABLE #tblReportGroups
	DROP TABLE #tblReportDetails

END