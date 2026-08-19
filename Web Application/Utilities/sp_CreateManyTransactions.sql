-- ==================================================================
-- Author:		Richard Panachida
-- Create date: 	August 8,2006
-- Description:	This SP will create X number of transactions for X number of years.
--              		It randomizes the reference data.
-- ==================================================================
CREATE PROCEDURE spCreateManyTransactions
(
	@SiteIndex int,
                  @LoginSiteIndex int,
	@NumOfYears int,
	@TotalRecPerDay int,
	@Delay int,
	@StartDate datetime
)

AS
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Initialize local variables
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
DECLARE @CompanyMax0 int;
DECLARE @CompanyMax1 int;
DECLARE @CompanyMax2 int;
DECLARE @CompanyMax3 int;
DECLARE @CompanyMax4 int;
DECLARE @CompanyMax5 int;
DECLARE @CompanyMax6 int;
DECLARE @EquipMax int;
DECLARE @ProductMax int;
DECLARE @AliasMax int;
DECLARE @TotalDays int;
DECLARE @InventoryDate datetime;
DECLARE @DayCounter int;
DECLARE @TotalNumOfDays int;
DECLARE @SiteID nvarchar (50);
DECLARE @TotalAliasCount int

SELECT @CompanyMax0 = 1;
SELECT @CompanyMax1 = 1;
SELECT @CompanyMax2 = 1;
SELECT @CompanyMax3 = 1;
SELECT @CompanyMax4 = 1;
SELECT @CompanyMax5 = 1;
SELECT @CompanyMax6 = 1;
SELECT @EquipMax = 1;
SELECT @ProductMax = 1;
SELECT @AliasMax = 1;
SELECT @TotalDays = 365;
SELECT @DayCounter = 0;
SELECT @TotalNumOfDays = 365 * @NumOfYears;

-- Set so that the total records per day is what the user requested.
-- There will be a loop for the records per day and inside that loop
-- will be a loop for aliases. The two loops should generate the number
-- of records per day. 
IF (@TotalRecPerDay < 2)
   BEGIN
      SELECT @TotalRecPerDay = 2;
   END

IF (@TotalRecPerDay > 9)
   BEGIN
      SELECT @TotalAliasCount = 10;
      SELECT @TotalRecPerDay = @TotalRecPerDay / 10;
   END
ELSE
   BEGIN
      SELECT @TotalAliasCount = 1;
      SELECT @TotalRecPerDay = @TotalRecPerDay - 1;
   END

--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Create temp tables for processing
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE TABLE [dbo].[#TBL_TEMP_TX_ALIASES]
 (
	[TxAliasIndex] [int] IDENTITY (1, 1) NOT NULL,
	[AliasName] [nvarchar] (50) NULL,
	[AliasID] [nvarchar] (50) NULL,
	[TransTypeID] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];


CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_MANAGER]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];

CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_OWNER]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];


CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPPER]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];

CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_BILLTO]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];


CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPTO]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];

CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_CARRIER]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];

CREATE TABLE [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SUPPLIER]
 (
	[CompanyIndex] [int] IDENTITY (1, 1) NOT NULL,
	[CompanyID] [nvarchar] (60) NULL,
	[CompanyCode] [nvarchar] (50) NULL,
	[CompanyRole] [nvarchar] (50) NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];


CREATE TABLE [dbo].[#TBL_TEMP_PRODUCT_REF_DATA]
 (
	[ProductIndex] [int] IDENTITY (1, 1) NOT NULL,
	[ProductID] [nvarchar] (50) NULL,
	[ProductType] [nvarchar] (50) NULL,
	[ProductCode] [nvarchar] (50) NULL,
	[ProductPrice] [money]  NULL,
	[ReferenceIndex] [int] NOT NULL
	
) ON [PRIMARY];


CREATE TABLE [dbo].[#TBL_TEMP_EQUIP_REF_DATA]
 (
	[EquipIndex] [int] IDENTITY (1, 1) NOT NULL,
	[EquipID] [nvarchar] (50) NULL,
	[Model] [nvarchar] (50) NULL,
	[SerialNumber] [nvarchar] (50) NULL,
	[Type] [nvarchar]  (50) NULL
	
) ON [PRIMARY];


--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- Build up alias reference data
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO [dbo].[#TBL_TEMP_TX_ALIASES]
	SELECT ta.AliasName, ta.AliasID, ta.TransTypeID, ta.AliasID AS ReferenceIndex
	FROM tblTransactionAliases ta,
		 (SELECT tblEntityToSiteMap.*,
			     (SELECT SubTable.SiteIndex
			      FROM tblEntityToSiteMap SubTable
			      WHERE SubTable.TypeID = 'Transaction Aliases' AND 
                        SubTable.[Index] = tblEntityToSiteMap.[Index] AND 
                        SubTable.SiteIndex = @LoginSiteIndex) AS LoginSiteIndex
		  FROM tblEntityToSiteMap
		  WHERE TypeID = 'Transaction Aliases' AND SiteIndex = @SiteIndex) tblEntities
	WHERE tblEntities.[Index] = ta.AliasID AND 
          (ta.SiteIndex = @SiteIndex OR tblEntities.LoginSiteIndex = @LoginSiteIndex) AND
          TransTypeID IN (1,5,6,7,4,8,14,17)
	ORDER BY ta.AliasName


INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_MANAGER]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 0) 


INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_OWNER]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 1) 

INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPPER]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 2) 

INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_BILLTO]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 3) 

INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_ShipTo]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 4) 

INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_CARRIER]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 5) 

INSERT INTO [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SUPPLIER]
	SELECT DISTINCT c.[ID],  c.Code, 'Manager' AS CompanyRole, c.CompanyIndex AS ReferenceIndex
    FROM tblCompanies c, (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Companies' AND SiteIndex = @SiteIndex) tblEntities 
    WHERE tblEntities.[Index] = c.CompanyIndex AND 
          c.CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE tblCompanyRoleMap.Role = 6) 

INSERT INTO [dbo].[#TBL_TEMP_PRODUCT_REF_DATA]
SELECT p.ProductID, 
	  (CASE p.ProductType
		    WHEN 0 THEN 'COMPONENT_PRODUCT'
		    WHEN 1 THEN 'BLEND_PRODUCT'
		    WHEN 2 THEN 'ADDITIVE_PRODUCT'
	   END) AS Type, 
       p.ProductCode, 
       p.Price, 
       p.ProductIndex AS ReferenceIndex
FROM tblProducts p,
     (SELECT tblEntityToSiteMap.*,
			(SELECT SubTable.SiteIndex
			  FROM tblEntityToSiteMap SubTable
			   WHERE SubTable.TypeID = 'Products' AND 
                     SubTable.[Index] = tblEntityToSiteMap.[Index] AND 
                     SubTable.SiteIndex = @LoginSiteIndex) AS LoginSiteIndex
		  FROM tblEntityToSiteMap
		  WHERE TypeID = 'Products' AND SiteIndex = @SiteIndex) tblEntities
WHERE p.ProductType IN (0, 1, 2) AND 
      p.ProductIndex IN (tblEntities.[Index]) AND
      (p.SiteIndex = @SiteIndex OR p.SiteIndex = @LoginSiteIndex)

INSERT INTO [dbo].[#TBL_TEMP_EQUIP_REF_DATA]
SELECT [ID], Model, SerialNumber, 
	(CASE Type
		WHEN 0 THEN 'TRAILER'
		WHEN 1 THEN 'TRACTOR'
		WHEN 2 THEN 'AIRCRAFT'
		WHEN 3 THEN 'RAILCAR'
		WHEN 4 THEN 'BARGE'
		WHEN 5 THEN 'COMPARTMENT'
		WHEN 6 THEN 'SHIP'
		WHEN 7 THEN 'PIPELINE'
		WHEN 8 THEN 'HYDRANT_CART'
		WHEN 9 THEN 'TANKER'
		WHEN 10 THEN 'STATIONARY_CART'
	END) AS Type
FROM tblEquipment
WHERE SiteIndex = @SiteIndex

SELECT @CompanyMax0 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_MANAGER]
SELECT @CompanyMax1 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_OWNER]
SELECT @CompanyMax2 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPPER]
SELECT @CompanyMax3 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_BILLTO]
SELECT @CompanyMax4 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPTO]
SELECT @CompanyMax5 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_CARRIER]
SELECT @CompanyMax6 = Max(CompanyIndex) FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SUPPLIER]
SELECT @ProductMax = Max(ProductIndex) FROM [dbo].[#TBL_TEMP_PRODUCT_REF_DATA]
SELECT @EquipMax = Max(EquipIndex) FROM [dbo].[#TBL_TEMP_EQUIP_REF_DATA]
SELECT @AliasMax = Max(TxAliasIndex) FROM [dbo].[#TBL_TEMP_TX_ALIASES]

SELECT @SiteID = [ID] FROM tblSites WHERE SiteIndex = @SiteIndex;

DECLARE @Counter int
DECLARE @Sdate datetime
DECLARE @seed int
DECLARE @SeedIndex int
DECLARE @CompanyIndex0 int
DECLARE @CompanyIndex1 int
DECLARE @CompanyIndex2 int
DECLARE @CompanyIndex3 int
DECLARE @CompanyIndex4 int
DECLARE @CompanyIndex5 int
DECLARE @CompanyIndex6 int
DECLARE @ProductIndex int
DECLARE @EquipIndex int
DECLARE @AliasIndex int
DECLARE @CreatedUpdatedDate datetime
DECLARE @InventoryDateStr nvarchar (22)
DECLARE @SignToggle float
DECLARE @Year nvarchar(4)
DECLARE @Month nvarchar(2)
DECLARE @Day nvarchar(2)
DECLARE @TxLineItemIDMax bigint

-- Retrieve randomly the data from the reference table.  This data will be used to create transactions.
DECLARE @AliasNameR nvarchar (50)
DECLARE @AliasIndexR int
DECLARE @TransTypeIDR nvarchar (50)
DECLARE @EquipIDR nvarchar (50)
DECLARE @EquipModelR nvarchar (50)
DECLARE @EquipSerialNumberR nvarchar (50)
DECLARE @EquipTypeR nvarchar (50)
DECLARE @ProductIDR nvarchar (50)
DECLARE @ProductCodeR nvarchar (50)
DECLARE @ProductTypeR nvarchar (50)
DECLARE @ProductPriceR money
DECLARE @ProductIndexR int
DECLARE @ManagerIDR nvarchar (50)
DECLARE @OwnerIDR nvarchar (50)
DECLARE @ShipperIDR nvarchar (50)
DECLARE @BillToIDR nvarchar (50)
DECLARE @ShipToIDR nvarchar (50)
DECLARE @CarrierIDR nvarchar (50)
DECLARE @SupplierIDR nvarchar (50)
DECLARE @ManagerCodeR nvarchar (50)
DECLARE @OwnerCodeR nvarchar (50)
DECLARE @ShipperCodeR nvarchar (50)
DECLARE @BillToCodeR nvarchar (50)
DECLARE @ShipToCodeR nvarchar (50)
DECLARE @CarrierCodeR nvarchar (50)
DECLARE @SupplierCodeR nvarchar (50)
DECLARE @ManagerIndexR int
DECLARE @OwnerIndexR int
DECLARE @ShipperIndexR int
DECLARE @BillToIndexR int
DECLARE @ShipToIndexR int
DECLARE @CarrierIndexR int
DECLARE @SupplierIndexR int
DECLARE @Guid nvarchar (50)
DECLARE @GrossQuantity float
DECLARE @NetQuantity float
DECLARE @DeleteFLag bit

SET LOCK_TIMEOUT 25000

-- If Delay is set to one, then the insert will delay X seconds
-- before the next insert.
IF ((@Delay < 0) OR (@Delay > 1))
  BEGIN
     SET @Delay = 0
  END

-- Loop unitl all days have expired (5 years)
WHILE @DayCounter < @TotalNumOfDays
	BEGIN

		IF (@StartDate = null)
		  BEGIN
			SET @CreatedUpdatedDate = DATEADD(day, @DayCounter, GetDate())
			SET @InventoryDate = DATEADD(day, @DayCounter, GetDate())
		  END
		ELSE
		  BEGIN
			SET @CreatedUpdatedDate = DATEADD(day, @DayCounter, @StartDate)
			SET @InventoryDate = DATEADD(day, @DayCounter, @StartDate)
		  END

		SET @Year = DATEPART(Year, @InventoryDate)
		SET @Month = DATEPART(Month, @InventoryDate)
		SET @Day = DATEPART(Day, @InventoryDate)
		SET @InventoryDateStr = @Year + '-' + @Month + '-' + @Day + ' 00:00:00'
		SET @Counter = 0
		SET @SignToggle = 1.0

		-- Loop for inserting X number of transaction records per day
		WHILE @Counter < @TotalRecPerDay
			BEGIN
				SET @Sdate = DATEADD(day, 0, GetDate());
		      	SET @seed = (DATEPART(ms, @Sdate) * 100000)
				SET @CompanyIndex0 =  RAND(@seed) * 100
				SET @CompanyIndex1 =  RAND(@seed) * 100
				SET @CompanyIndex2 =  RAND(@seed) * 100
				SET @CompanyIndex3 =  RAND(@seed) * 100
				SET @CompanyIndex4 =  RAND(@seed) * 100
				SET @CompanyIndex5 =  RAND(@seed) * 100
				SET @CompanyIndex6 =  RAND(@seed) * 100
				SET @ProductIndex  =  RAND(@seed) * 100
				SET @EquipIndex    =  RAND(@seed) * 100
		
				-- Set the random indexes to be within the boundries of the tables
				WHILE @CompanyIndex0 > @CompanyMax0
					BEGIN
						SET @CompanyIndex0 = @CompanyIndex0 / 2
					END
				WHILE @CompanyIndex1 > @CompanyMax1
					BEGIN
						SET @CompanyIndex1 = @CompanyIndex1 / 2
					END
				WHILE @CompanyIndex2 > @CompanyMax2
					BEGIN
						SET @CompanyIndex2 = @CompanyIndex2 / 2
					END
				WHILE @CompanyIndex3 > @CompanyMax3
					BEGIN
						SET @CompanyIndex3 = @CompanyIndex3 / 2
					END
				WHILE @CompanyIndex4 > @CompanyMax4
					BEGIN
						SET @CompanyIndex4 = @CompanyIndex4 / 2
					END
				WHILE @CompanyIndex5 > @CompanyMax5
					BEGIN
						SET @CompanyIndex5 = @CompanyIndex5 / 2
					END
				WHILE @CompanyIndex6 > @CompanyMax6
					BEGIN
						SET @CompanyIndex6 = @CompanyIndex6 / 2
					END
				WHILE @ProductIndex > @ProductMax
					BEGIN
						SET @ProductIndex = @ProductIndex / 2
					END
				WHILE @EquipIndex > @EquipMax
					BEGIN
						SET @EquipIndex = @EquipIndex / 2
					END

				SELECT @EquipIDR = EquipID, @EquipModelR = Model, @EquipSerialNumberR = SerialNumber, @EquipTypeR = Type
				FROM [dbo].[#TBL_TEMP_EQUIP_REF_DATA] 
				WHERE EquipIndex = @EquipIndex
		
				SELECT @ProductIDR = ProductID, @ProductCodeR = ProductCode, @ProductTypeR = ProductType, 
						@ProductPriceR = ProductPrice, @ProductIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_PRODUCT_REF_DATA] 
				WHERE ProductIndex = @ProductIndex
		
				SELECT @ManagerIDR = CompanyID, @ManagerCodeR = CompanyCode, @ManagerIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_MANAGER] 
				WHERE CompanyIndex = @CompanyIndex0
		
				SELECT @OwnerIDR = CompanyID, @OwnerCodeR = CompanyCode, @OwnerIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_OWNER] 
				WHERE CompanyIndex = @CompanyIndex1
		
				SELECT @ShipperIDR = CompanyID, @ShipperCodeR = CompanyCode, @ShipperIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPPER] 
				WHERE CompanyIndex = @CompanyIndex2
		
				SELECT @BillToIDR = CompanyID, @BillToCodeR = CompanyCode, @BillToIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_BILLTO] 
				WHERE CompanyIndex = @CompanyIndex3
		
				SELECT @ShipToIDR = CompanyID, @ShipToCodeR = CompanyCode, @ShipToIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPTO] 
				WHERE CompanyIndex = @CompanyIndex4
		
				SELECT @CarrierIDR = CompanyID, @CarrierCodeR = CompanyCode, @CarrierIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_CARRIER] 
				WHERE CompanyIndex = @CompanyIndex5
		
				SELECT @SupplierIDR = CompanyID, @SupplierCodeR = CompanyCode, @SupplierIndexR = ReferenceIndex
				FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SUPPLIER] 
				WHERE CompanyIndex = @CompanyIndex6

				DECLARE @AliasCount INT
				SET @AliasCount = 0

				-- For each record generate a new alias for x number of
				-- aliases.
				WHILE @AliasCount < @TotalAliasCount
                   BEGIN
				      SET @Sdate = DATEADD(day, 0, GetDate());
		      	      SET @seed = (DATEPART(ms, @Sdate) * 100000)
				      SET @AliasIndex = RAND(@seed) * 100

				      WHILE @AliasIndex > @AliasMax
					     BEGIN
						    SET @AliasIndex = @AliasIndex / 2
					     END
		
				      SELECT @AliasNameR = AliasName, @TransTypeIDR = TransTypeID, @AliasIndexR = ReferenceIndex
				      FROM [dbo].[#TBL_TEMP_TX_ALIASES] 
				      WHERE TxAliasIndex = @AliasIndex
		
				      SET @Guid = NEWID()
				      SET @DeleteFlag = 0
		
				      BEGIN TRANSACTION T1
				      -- Insert the header part of the transaction record
				      INSERT INTO tblTransactions WITH (ROWLOCK) (TransID, InventoryDate, CreatedDate, UpdatedDate, CreatedBy, 
								      UpdatedBy, AliasName, TransTypeID, Site, ShipToID, ShipToCode, SupplierID, SupplierCode, 
								      ManagerID, ManagerCode, OwnerID, OwnerCode, BillToID, BillToCode, CarrierID, CarrierCode, 
								      ShipperID, ShipperCode, DeleteFlag, ReversalType, TransactionStatus, InternationalRouteIndicator, 
								      TicketMode, TransDateTime, ManagerIndex, OwnerIndex, BillToIndex, ShipToIndex, CarrierIndex, 
								      SupplierIndex, ShipperIndex, AliasIndex, SiteIndex)
					      VALUES (@Guid, @InventoryDateStr, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Rabbit', 'Rabbit', @AliasNameR, 
							      @TransTypeIDR, @SiteID, @ShipToIDR, @ShipToCodeR, @SupplierIDR, @SupplierCodeR, @ManagerIDR, 
							      @ManagerCodeR, @OwnerIDR, @OwnerCodeR, @BillToIDR, @BillToCodeR, @CarrierIDR, @CarrierCodeR, 
							      @ShipperIDR, @ShipperCodeR, @DeleteFlag, ' ', 0, 0, 0, @InventoryDateStr, @ManagerIndexR, 
							      @OwnerIndexR, @BillToIndexR, @ShipToIndexR, @CarrierIndexR, @SupplierIndexR, @ShipperIndexR,
								  @AliasIndexR, @SiteIndex)

				      SET @GrossQuantity =  RAND(@seed) * 100
				      SET @NetQuantity = @GrossQuantity

				      -- On issues types change the value to be a negative.  In addition, change the sign toggle variable.
				      IF ((@TransTypeIDR = 5) OR (@TransTypeIDR = 6))
				        BEGIN
				             SET @GrossQuantity = @GrossQuantity * -1.0
				             SET @NetQuantity = @NetQuantity * -1.0
				             SET @SignToggle = @SignToggle * -1.0
				        END

				      -- On Adjustments and Physical inventories multiple by the sign toggle value.
				      IF ((@TransTypeIDR = 1) OR (@TransTypeIDR = 14))
				        BEGIN
				             SET @GrossQuantity = @GrossQuantity * @SignToggle
				             SET @NetQuantity = @NetQuantity * @SignToggle
				        END

				      -- Get the last ID in the line item table
				      SELECT @TxLineItemIDMax = Max(TransLineItemID) FROM tblTransactionLineItems
					  IF (@TxLineItemIDMax IS NULL)
						BEGIN
							SET @TxLineItemIDMax = 0
						END
				      SET @TxLineItemIDMax = @TxLineItemIDMax + 1

					  -- Ensure that the accounting line item sequence number is kept up to date.
					  UPDATE tblAccountingSequences SET SequenceValue = @TxLineItemIDMax 
					     WHERE SequenceName = 'LineItemID'

				      -- Insert the line item part of the transaction record
				      INSERT INTO tblTransactionLineItems WITH (ROWLOCK) 
								(TransLineItemID, TransID, SequenceID, LineItemSequenceNumber, Product, ProductCode, ProductType, ProductPrice,
								 DestinationRegistrationID, DestinationSerialNumber, DestinationEquipmentType, DestinationEquipmentModel,
								 GrossQuantity, NetQuantity, DeleteFlag, ProductIndex, TransactionStatus)
					      VALUES (@TxLineItemIDMax, @Guid, 0, 0, @ProductIDR, @ProductCodeR, @ProductTypeR, @ProductPriceR,
							      @EquipIDR, @EquipSerialNumberR, @EquipTypeR, @EquipModelR,
							      @GrossQuantity, @NetQuantity, @DeleteFlag, @ProductIndexR, 0)

				      -- Need empty records in the following tables...
				      INSERT INTO tblTransactionUserData WITH (ROWLOCK) (TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy) 
					      VALUES (@Guid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Rabbit', 'Rabbit')
				      INSERT INTO tblTransactionNotes WITH (ROWLOCK) (TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy) 
					      VALUES (@Guid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Rabbit', 'Rabbit')
				      INSERT INTO tblTransactionWeightReadings WITH (ROWLOCK) (TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy, 
										BeginQuantityValue, RequestedQuantityValue, FinalQuantityValue, CompartmentID) 
					      VALUES (@Guid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Rabbit', 'Rabbit', 0.0, 0.0, 0.0, '')

				      COMMIT TRANSACTION T1
		
				      IF (@Delay = 1)
				        BEGIN
					      WAITFOR DELAY '00:00:03'
				        END

				      SET NOCOUNT ON
				      SET @AliasCount = @AliasCount + 1
				      SET NOCOUNT OFF
				   END -- End alias loop

				SET NOCOUNT ON
				SET @Counter = @Counter + 1
				SET NOCOUNT OFF
			END -- End record insert loop

		SET NOCOUNT ON
		SET @DayCounter = @DayCounter + 1
		SET NOCOUNT OFF
	END -- End number of days loop

-- Remove a data from temp tables.
DELETE FROM [dbo].[#TBL_TEMP_TX_ALIASES] 
DELETE FROM [dbo].[#TBL_TEMP_EQUIP_REF_DATA] 
DELETE FROM [dbo].[#TBL_TEMP_PRODUCT_REF_DATA] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_MANAGER] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_OWNER] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPPER] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_BILLTO] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SHIPTO] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_CARRIER] 
DELETE FROM [dbo].[#TBL_TEMP_COMPANY_REF_DATA_SUPPLIER]

GO
