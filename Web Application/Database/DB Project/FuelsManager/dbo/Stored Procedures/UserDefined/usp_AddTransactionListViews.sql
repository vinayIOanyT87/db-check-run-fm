-- ==========================================================================================
-- Description:	Stored procedure to add transaction list views into the system
-- Author:		Srinivasa Divyakolu
-- Create date/Revision: 07-27-2022/1.0.0
-- Last Modification date/Revision: 08-09-2022/1.0.001
-- 1. Get all existing listViewGuid from tblListViews for a given list view by ID
-- 2. Check if the given listView is entity assigned to this site from any of the parents by checking  [map].[tblEntityListViewToSite]
-- 3. If it is entity assigned then do not create a new view
-- 4. If it is not entity assigned then create it and entity assign itself
-- ==========================================================================================
CREATE PROCEDURE usp_AddTransactionListViews 
	@SiteGuid UNIQUEIDENTIFIER,
	@ReturnMsg NVARCHAR(2000) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	DECLARE @LVID AS NVARCHAR(100)
	DECLARE @TransactionAliasGuid AS UNIQUEIDENTIFIER
	DECLARE @MapExists AS BIT 
	DECLARE @ListViewGuid AS UNIQUEIDENTIFIER
	DECLARE @User AS NVARCHAR(20) = N'Administrator';
	DECLARE @CUDateTime AS DATETIME = SYSDATETIMEOFFSET();
	DECLARE @ListViewExists AS BIT = 0

	DECLARE @AliasFieldGuid AS UNIQUEIDENTIFIER, @InventoryDateFieldGuid  AS UNIQUEIDENTIFIER, @OwnerFieldGuid AS UNIQUEIDENTIFIER, 
			@ProductFieldGuid AS UNIQUEIDENTIFIER, @NetFieldGuid AS UNIQUEIDENTIFIER, @ReversalFieldGuid AS UNIQUEIDENTIFIER,
			@DocumentNumberFieldGuid AS UNIQUEIDENTIFIER, @GrossQuantityFieldGuid AS UNIQUEIDENTIFIER, @MeterFieldGuid AS UNIQUEIDENTIFIER,
			@MeterStopFieldGuid AS UNIQUEIDENTIFIER, @TankFieldGuid AS UNIQUEIDENTIFIER, @NotesFieldGuid AS UNIQUEIDENTIFIER,
			@TicketNumberFieldGuid AS UNIQUEIDENTIFIER, @PONumberFieldGuid AS UNIQUEIDENTIFIER, @FromProductFieldGuid AS UNIQUEIDENTIFIER,
			@ShipToFieldGuid AS UNIQUEIDENTIFIER, @CarrierIDFieldGuid AS UNIQUEIDENTIFIER, @TransactionStatusFieldGuid AS UNIQUEIDENTIFIER,
			@SupplyOrderNumberFieldGuid AS UNIQUEIDENTIFIER, @SupplierFieldGuid AS UNIQUEIDENTIFIER, @FromOwnerFieldGuid AS UNIQUEIDENTIFIER,
			@BOLNumberFieldGuid AS UNIQUEIDENTIFIER, @ShipperFieldGuid AS UNIQUEIDENTIFIER, @BillToFieldGuid AS UNIQUEIDENTIFIER,
			@StatusFieldGuid AS UNIQUEIDENTIFIER, @DeliveredGrossQuantityFieldGuid AS UNIQUEIDENTIFIER,@DeliveredNetQuantityFieldGuid AS UNIQUEIDENTIFIER,
			@PressureFieldGuid AS UNIQUEIDENTIFIER
	DECLARE @ReturnMessage AS NVARCHAR(2000)

	SET @LVID = N'Adjustment'

	SET @ReturnMessage = @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @OwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Owner')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @OwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @OwnerFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')


	--------------------------------------------------------------
	SET @LVID = N'BOL'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @BOLNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'BOL #')
				SET  @OwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Stockholder')
				SET  @ShipperFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Shipper')
				SET  @BillToFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Bill To')
				SET  @ShipToFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Ship To')
				SET  @CarrierIDFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Carrier')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @GrossQuantityFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Gross')
				SET  @DeliveredGrossQuantityFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DbName = 'DeliveredGrossQuantity')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @DeliveredNetQuantityFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DbName = 'DeliveredNetQuantity')
				SET  @PressureFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DbName = 'Pressure')
				SET  @TankFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Tank')
				SET  @StatusFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Status' AND LookupTransactionFieldTypeIndex = 1)
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @BOLNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @BOLNumberFieldGuid)

				IF @OwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @OwnerFieldGuid)

				IF @ShipperFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ShipperFieldGuid)

				IF @BillToFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @BillToFieldGuid)

				IF @ShipToFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ShipToFieldGuid)

				IF @CarrierIDFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @CarrierIDFieldGuid)
		
				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @GrossQuantityFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (9, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @GrossQuantityFieldGuid)

				IF @DeliveredGrossQuantityFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (10, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @DeliveredGrossQuantityFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (11, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @DeliveredNetQuantityFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (12, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @DeliveredNetQuantityFieldGuid)

				IF @PressureFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (13, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @PressureFieldGuid)

				IF @TankFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (14, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @TankFieldGuid)

				IF @StatusFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (15, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @StatusFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (16, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (17, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Meter Closeout'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @DocumentNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Document Number')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @GrossQuantityFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Gross Quantity')
				SET  @MeterFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Meter')
				SET  @MeterStopFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Stop')
				SET  @TankFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Tank')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @DocumentNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @DocumentNumberFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @GrossQuantityFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @GrossQuantityFieldGuid)

				IF @MeterFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @MeterFieldGuid)

				IF @MeterStopFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @MeterStopFieldGuid)

				IF @TankFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @TankFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Physical Inventory'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @DocumentNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Document Number')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @TankFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Tank')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal Type')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @DocumentNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @DocumentNumberFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @TankFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @TankFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Receipt'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @TicketNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Ticket #')
				SET  @PONumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'PO Number')
				SET  @OwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Owner')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @TicketNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @TicketNumberFieldGuid)

				IF @PONumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @PONumberFieldGuid)

				IF @OwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @OwnerFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Regrade'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @OwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Owner')
				SET  @FromProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'From Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @OwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @OwnerFieldGuid)

				IF @FromProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @FromProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Shipment'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @DocumentNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Document Number')
				SET  @OwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Owner')
				SET  @ShipToFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Ship To')
				SET  @CarrierIDFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Carrier ID')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @DocumentNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @DocumentNumberFieldGuid)

				IF @OwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @OwnerFieldGuid)

				IF @ShipToFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ShipToFieldGuid)

				IF @CarrierIDFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @CarrierIDFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (7, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (8, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (9, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Supply Order'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @TransactionStatusFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Transaction Status')
				SET  @SupplyOrderNumberFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Supply Order Number')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @SupplierFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Supplier')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')

				IF @TransactionStatusFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @TransactionStatusFieldGuid)

				IF @SupplyOrderNumberFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @SupplyOrderNumberFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @SupplierFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @SupplierFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				SET @ReturnMessage += 'View created,'
			END
			ELSE
				SET @ReturnMessage += 'View already exists,'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists,'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned,', 'View already exists,')

	--------------------------------------------------------------
	SET @LVID = N'Transfer'

	SET @ReturnMessage += @LVID + ' - '

	-- Check if map exists in [map].[tblEntityListViewToSite] for this list view for this site
	SET @MapExists = (SELECT COUNT(*) FROM [map].[tblEntityListViewToSite] WHERE [ListViewGuid] IN (SELECT [ListViewGuid] FROM tblListViews where ID = @LVID) AND [SiteGuid] = @SiteGuid)
	SET @TransactionAliasGuid = (SELECT TransactionAliasGuid from tblTransactionAliases where [SiteGuid] = @SiteGuid AND AliasName = @LVID)
	SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)		

	IF @MapExists = 0
	BEGIN
		IF @TransactionAliasGuid IS NOT NULL
		BEGIN
			-- Only add if not already exists
			IF @ListViewGuid IS NULL
			BEGIN

				INSERT [dbo].[tblListViews] ([CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [ListViewGuid], [SiteGuid], [LookupListViewTypeIndex], [TransactionAliasGuid]) VALUES 
				(@CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), @SiteGuid, 1, @TransactionAliasGuid)

				-- Get listview guid for the newly added row
				SET @ListViewGuid = (SELECT ListViewGuid FROM [dbo].[tblListViews] Where ID = @LVID and SiteGuid =  @SiteGuid)

				INSERT [map].[tblEntityListViewToSite] ([ListViewToSiteGuid], [ListViewGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES 
				(NEWID(), @ListViewGuid, @SiteGuid, @CUDateTime, @User, @CUDateTime, @User, @SiteGuid)

				SET  @AliasFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Alias')
				SET  @InventoryDateFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Inventory Date')
				SET  @FromOwnerFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'From Owner')
				SET  @ProductFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Product')
				SET  @NetFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Net')
				SET  @ReversalFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Reversal')
				SET  @NotesFieldGuid = (Select TAF.TransactionAliasFieldGuid FROM [dbo].[tblTransactionAliasFields] TAF JOIN dbo.tblTransactionAliases TA ON TAF.TransactionAliasGuid = TA.TransactionAliasGuid WHERE TA.AliasName = @LVID AND TA.SiteGuid = @SiteGuid AND  TAF.DisplayName = 'Notes')

				IF @AliasFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (0, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @AliasFieldGuid)

				IF @InventoryDateFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (1, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @InventoryDateFieldGuid)

				IF @FromOwnerFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (2, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @FromOwnerFieldGuid)

				IF @ProductFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (3, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ProductFieldGuid)

				IF @NetFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (4, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NetFieldGuid)

				IF @ReversalFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (5, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @ReversalFieldGuid)

				IF @NotesFieldGuid IS NOT NULL
				INSERT [dbo].[tblListViewFields] ([ColumnOrder], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ListViewID], [ListViewFieldGuid],[LookupListViewFieldTypeIndex], [ListViewGuid], [TransactionAliasFieldGuid]) VALUES (6, @CUDateTime, @User, @CUDateTime, @User, @LVID, NEWID(), 2, @ListViewGuid, @NotesFieldGuid)

				SET @ReturnMessage += 'View created'
			END
			ELSE
				SET @ReturnMessage += 'View already exists.'
		END
		ELSE
			SET @ReturnMessage += 'Alias does not exists.'
	END
	ELSE
		SET @ReturnMessage += IIF (@ListViewGuid IS NULL, 'View already entity assigned.', 'View already exists.')

	SET @ReturnMsg = @ReturnMessage
END
GO
