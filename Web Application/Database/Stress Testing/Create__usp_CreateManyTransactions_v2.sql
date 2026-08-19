/*************************************************************************** 
																									  
	FILE NAME:		Create__usp_CreateManyTransactions_v2.sql						  
																									  
	PURPOSE:			Creates X number of transactions for X number of years.	  
						It randomizes the reference data.								  
																									  
	Copyright (C) 1999-2009		Varec, Inc.												  
										Norcross, GA, USA    All Rights Reserved		  
																									  
	This file shall not be copied or reproduced in any form without the		  
	express written consent of Varec.													  
																									  
	MODIFICATION HISTORY																		  
																									  
		DATE:				BY:				VERSION:		REASON:							  
		===========		============	========		============================ 
		08-08-2008		R. Panachida	1				Initial creation.				  
																									  
		03-04-2009		L. Leonard		2				Fixed "= NULL".  Fixed a	  
																randomization problem.		  
																Per AS this version addresses
																only transactions: STC055.01 
																(h), (i), and (j).			  
																									  
		04-03-2009		I.Orndorff		3				- Removed all instances of LoginSiteIndex. This 
																  is not needed for entity assignment. 
																- Changed the build up alias reference data to 
																  use transaction type instead of alias id. 
																- Dropped all temp tables at the end. 
																- Randomized all transaction aliases regardless 
																  of the section (H,I,J).    
																- Added entity change to the equipment temp table. 
																- Use @@IDLE for random seed.
																									  
		04-10-2009		L. Leonard		4				Two of the transaction tables
																are clustered on a GUID. This
																was felt to be ok, as they   
																were concocting a sequential 
																GUID in C#. Sadly, NEW_ID()  
																is not sequential, so we've  
																been clustered on a GUID.    
																Wrote a quick and dirty fnx  
																to generate a sequential GUID.
																									  
		04-20-2009		L. Leonard		5				Fixed division by zero problem. 
                                                                             
		04-28-2009		L. Leonard		6				Uncommented out code that someone 
																commented out (GrossQuantity, etc.) 
																Improved progress reporting. 
                                                                             
		05-01-2009		I. Orndorff		7				- Moved dropping of the temp tables to 
														      the beginning of the procedure. 
                                                                              
		05-04-2009		I.Orndorff		8				- Added quality	to lineitem insert so the
														      transaction would be viewable\editable in the 
														      application. 					  
                                                                             
****************************************************************************/ 


---------------------------------------------------------------------------- 
-- Task 1692: Fusion/ADF - ATP055GVEN0708: "System Capacity and Performance" 
-- The load test script needs to populate the database to meet requirements, 
-- as follows:																					  
---------------------------------------------------------------------------- 
--																									  
-- STC055.01: Is the System able to operate with the following data,			  
--				  transaction load rates, and number of users whilst performing  
--				  all of its mission functions:											  
--																									  
--		a.	Number of users using the System simultaneously:					 200 
--																									  
--		b.	Number of fuel cards assigned to vehicles:						 50,000 
--																									  
--		c.	Avg number of transactions (and queries) per working day:		2400 
--																									  
--		d.	Avg number of transactions (and queries) per working hour:		 300 
--																									  
--		e.	Number of Aviation and Maritime Bases with one or more Fuel			  
--			Facilities:																		  22 
--																									  
--		f.	Number of Navy Tankers:														  10 
--																									  
--		g.	Number of Ground Facilities with one or more Ground Fuel 			  
--			Control Towers:																  46 
--																									  
------------------------------------------------------------------------------------------------------------------------------------------------------------
--                                           SECTION 'H' IS NOT NEEDED UNTIL PHASE 2, CURRENTLY SCHEDULED FOR OCTOBER 2009.											
------------------------------------------------------------------------------------------------------------------------------------------------------------
--																											PHASE 1 PERCENTAGE		PHASE 2 PERCENTAGE											
--    h. Avg number of ground fuel commercial transactions per year					------------------		------------------											
--       (reported by Fuel Companies in hard copy form):             150,000																											
--																														-							32%														
--          AliasName                   AliasIndex			TransTypeID																																					
--          ------------------          ----------			-----------																															
--          Commercial                  30					12																															
--																																																				
------------------------------------------------------------------------------------------------------------------------------------------------------------
--																																																				
--    i. Avg number of ground fuel on base transactions per year:    200,000 																											
--                                                                      						65%						43% 														
--          AliasName                   AliasIndex        TransTypeID                   																											
--          ------------------         ----------         -----------                   																											
--          Issue (Ground)              12                5                   																											
--          Sale (Ground)               15                5                   																											
--                                                                           																											
--    j. Avg number of aviation and marine transactions per year:    110,000 																											
--                                                                        						35%						25%														
--          AliasName                   AliasIndex        TransTypeID                   																											
--          ------------------          ----------        -----------                   																											
--          Issue (Aviation)             2                5                   																											
--          Issue (Marine)              13                5                   																											
--          Sale (Aviation)              6                5                   																											
--          Sale (Marine)               14                5                   																											
--                                                                           																											
--    Additional Varec specification - Number of years: 7                               																							
--                                                                           																											
------------------------------------------------------------------------------------------------------------------------------------------------------------
																									  																										
---------------------------------------------------------------------------- 
-- Notes from Tim Ayotte:                                                    
--                                                                           
-- The only sites we are interested in for ADF (Phase 1) are:                
--                                                                           
--                                                                           
--      Site     Site                   Service      Transaction       Alias 
--      Index    Name                   Branch       Alias                ID 
--      -----    ------------------     ---------    -----------------    -- 
--        5      Edinburgh              Air Force    Issue (Aviation)      2 
--                                                   Sale (Aviation)       6 
--                                                                           
--        7      Stirling               Marines      Issue (Marine)       13 
--                                                   Sale (Marine)        14 
--                                                                           
--       45      Edinburgh - Ground     Air Force    Issue Ground         12 
--                                                   Sale Ground          15 
--                                                                           
--       43      Stirling - Ground      Marines      Issue Ground         12 
--                                                   Sale Ground          15 
--                                                                           
---------------------------------------------------------------------------- 


USE ConsolidatedDB

SET NOCOUNT ON
SET ANSI_NULLS ON
SET XACT_ABORT ON
SET QUOTED_IDENTIFIER ON


----------------------------------------------------------------------------- 
-- Create the stored procdure.															   
----------------------------------------------------------------------------- 

-- Drop any existing one. 
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'usp_CreateManyTransactions_v2')
BEGIN
	RAISERROR ('Dropping stored procedure usp_CreateManyTransactions_v2', 10, 1) WITH NOWAIT, LOG
	DROP PROCEDURE usp_CreateManyTransactions_v2
	RAISERROR ('Dropped  stored procedure usp_CreateManyTransactions_v2', 10, 1) WITH NOWAIT, LOG
END

-- Create stored procedure. 
RAISERROR ('Creating stored procedure usp_CreateManyTransactions_v2', 10, 1) WITH NOWAIT, LOG
GO

CREATE PROCEDURE usp_CreateManyTransactions_v2(@SiteIndex			INT,
															  @NumOfDays			INT,
															  @RowsPerDay			INT,
															  @bDelay				INT,
															  @StartDate			DATETIME,
															  @StartTime			DATETIME,
															  @RowsAlready			INT,
															  @TotalRows			INT,
															  @RowsAddedThisRun	INT)
AS
BEGIN

	----------------------------------------------------------------------------- 
	-- Initialize all variables first to avoid performance hit.						   
	----------------------------------------------------------------------------- 

	RAISERROR ('Initializing variables.', 10, 1) WITH NOWAIT, LOG

	DECLARE @AliasCount				INT					SET @AliasCount 			  = 0
	DECLARE @AliasRowNumForLetter	INT					SET @AliasRowNumForLetter = 0
	DECLARE @AliasIndex				INT					SET @AliasIndex 			  = 0
	DECLARE @AliasIndexR				INT					SET @AliasIndexR 			  = 0
	DECLARE @AliasRandom				FLOAT					SET @AliasRandom			  = 0.0
	DECLARE @AliasPercentageH		FLOAT					SET @AliasPercentageH	  = 0.32
	DECLARE @AliasPercentageI		FLOAT					SET @AliasPercentageI	  = 0.43
	DECLARE @AliasPercentageJ		FLOAT					SET @AliasPercentageJ	  = 0.25
	DECLARE @AliasCountH				FLOAT					SET @AliasCountH			  = 0
	DECLARE @AliasCountI				FLOAT					SET @AliasCountI			  = 0
	DECLARE @AliasCountJ				FLOAT					SET @AliasCountJ			  = 0
	DECLARE @AliasMax					INT					SET @AliasMax 				  = 1
	DECLARE @AliasNameR				NVARCHAR(50)		SET @AliasNameR 			  = ''
	DECLARE @BillToCodeR				NVARCHAR(50)		SET @BillToCodeR 			  = ''
	DECLARE @BillToIDR				NVARCHAR(50)		SET @BillToIDR 			  = ''
	DECLARE @BillToIndexR			INT					SET @BillToIndexR 		  = 0
	DECLARE @CarrierCodeR			NVARCHAR(50)		SET @CarrierCodeR 		  = ''
	DECLARE @CarrierIDR				NVARCHAR(50)		SET @CarrierIDR 			  = ''
	DECLARE @CarrierIndexR			INT					SET @CarrierIndexR 		  = 0
	DECLARE @CompanyBilltoIndex	INT					SET @CompanyBilltoIndex   = 0
	DECLARE @CompanyBilltoMax		INT					SET @CompanyBilltoMax 	  = 1
	DECLARE @CompanyCarrierIndex	INT					SET @CompanyCarrierIndex  = 0
	DECLARE @CompanyCarrierMax		INT					SET @CompanyCarrierMax 	  = 1
	DECLARE @CompanyManagerIndex	INT					SET @CompanyManagerIndex  = 0
	DECLARE @CompanyManagerMax		INT					SET @CompanyManagerMax 	  = 1
	DECLARE @CompanyOwnerIndex		INT					SET @CompanyOwnerIndex 	  = 0
	DECLARE @CompanyOwnerMax		INT					SET @CompanyOwnerMax 	  = 1
	DECLARE @CompanyShipperIndex	INT					SET @CompanyShipperIndex  = 0
	DECLARE @CompanyShipperMax		INT					SET @CompanyShipperMax 	  = 1
	DECLARE @CompanyShiptoIndex	INT					SET @CompanyShiptoIndex   = 0
	DECLARE @CompanyShiptoMax		INT					SET @CompanyShiptoMax 	  = 1
	DECLARE @CompanySupplierIndex	INT					SET @CompanySupplierIndex = 0
	DECLARE @CompanySupplierMax	INT					SET @CompanySupplierMax   = 1
	DECLARE @RowCounter				INT					SET @RowCounter 			  = 0
	DECLARE @RowsPerDayOrig			INT					SET @RowsPerDayOrig		  = @RowsPerDay
	DECLARE @CreatedUpdatedDate	DATETIME				SET @CreatedUpdatedDate   = GETDATE()
	DECLARE @DayCounter				INT					SET @DayCounter 			  = 0
	DECLARE @DeleteFlag				BIT					SET @DeleteFlag 			  = 0
	DECLARE @EquipIDR					NVARCHAR(50)		SET @EquipIDR 				  = ''
	DECLARE @EquipIndex				INT					SET @EquipIndex 			  = 0
	DECLARE @EquipMax					INT					SET @EquipMax 				  = 1
	DECLARE @EquipModelR				NVARCHAR(50)		SET @EquipModelR 			  = ''
	DECLARE @EquipSerialNumberR	NVARCHAR(50)		SET @EquipSerialNumberR   = ''
	DECLARE @EquipTypeR				NVARCHAR(50)		SET @EquipTypeR 			  = ''
	DECLARE @GrossQuantity			FLOAT					SET @GrossQuantity 		  = 0.0
	DECLARE @sGuid						NVARCHAR(64)		SET @sGuid 					  = ''
	DEClARE @nGuid						BIGINT				SET @nGuid					  = 1
	DECLARE @InventoryDateStr		NVARCHAR(22)		SET @InventoryDateStr 	  = ''
	DECLARE @ManagerCodeR			NVARCHAR(50)		SET @ManagerCodeR 		  = ''
	DECLARE @ManagerIDR				NVARCHAR(50)		SET @ManagerIDR 			  = ''
	DECLARE @ManagerIndexR			INT					SET @ManagerIndexR 		  = 0
	DECLARE @NetQuantity				FLOAT					SET @NetQuantity 			  = 0.0
	DECLARE @OwnerCodeR				NVARCHAR(50)		SET @OwnerCodeR 			  = ''
	DECLARE @OwnerIDR					NVARCHAR(50)		SET @OwnerIDR 				  = ''
	DECLARE @OwnerIndexR				INT					SET @OwnerIndexR 			  = 0
	DECLARE @ProductCodeR			NVARCHAR(50)		SET @ProductCodeR 		  = ''
	DECLARE @ProductIDR				NVARCHAR(50)		SET @ProductIDR 			  = ''
	DECLARE @ProductIndex			INT					SET @ProductIndex 		  = 0
	DECLARE @ProductIndexR			INT					SET @ProductIndexR 		  = 0
	DECLARE @ProductMax				INT					SET @ProductMax 			  = 1
	DECLARE @ProductPriceR			MONEY					SET @ProductPriceR 		  = 0.0
	DECLARE @ProductTypeR			NVARCHAR(50)		SET @ProductTypeR 		  = ''
	DECLARE @RandSeed					INT					SET @RandSeed 				  = 0
	DECLARE @RecoveryModel			NVARCHAR(128)		SET @RecoveryModel        = ''
	DECLARE @RowCount					INT					SET @RowCount 				  = 0
	DECLARE @ShipToCodeR				NVARCHAR(50)		SET @ShipToCodeR 			  = ''
	DECLARE @ShipToIDR				NVARCHAR(50)		SET @ShipToIDR 			  = ''
	DECLARE @ShipToIndexR			INT					SET @ShipToIndexR 		  = 0
	DECLARE @ShipperCodeR			NVARCHAR(50)		SET @ShipperCodeR 		  = ''
	DECLARE @ShipperIDR				NVARCHAR(50)		SET @ShipperIDR 			  = ''
	DECLARE @ShipperIndexR			INT					SET @ShipperIndexR 		  = 0
	DECLARE @SignToggle				FLOAT					SET @SignToggle 			  = 0.0
	DECLARE @SiteID					NVARCHAR(50)		SET @SiteID 				  = ''
	DECLARE @StartDateStr			NVARCHAR(50)		SET @StartDateStr 		  = ''
	DECLARE @SupplierCodeR			NVARCHAR(50)		SET @SupplierCodeR 		  = ''
	DECLARE @SupplierIDR				NVARCHAR(50)		SET @SupplierIDR 			  = ''
	DECLARE @SupplierIndexR			INT					SET @SupplierIndexR 		  = 0
	DECLARE @TotalAliasCount		INT					SET @TotalAliasCount 	  = 0
	DECLARE @TransTypeIDR			NVARCHAR(50)		SET @TransTypeIDR 		  = ''
	DECLARE @TxLineItemIDMax		BIGINT				SET @TxLineItemIDMax 	  = 0


	-----------------------------------------------------------------------------
	-- Before we start, check we have data we need to succeed.						  
	-----------------------------------------------------------------------------
	
	IF NOT EXISTS (SELECT TOP 1 * FROM tblTransactionAliases)				RAISERROR ('   *** PROBLEM: tblTransactionAliases is empty!', 20, 1) WITH NOWAIT, LOG
	IF NOT EXISTS (SELECT * FROM tblSites WHERE SiteIndex = @SiteIndex)	RAISERROR ('   *** PROBLEM: Invalid @SiteIndex passed in!',   20, 1) WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- You REALLY don't want to run this in "Full" Recovery Model.					  
	-----------------------------------------------------------------------------

	SET @RecoveryModel = CONVERT(NVARCHAR(128), DATABASEPROPERTYEX(DB_NAME(), 'Recovery'))
	
	IF @RecoveryModel <> 'SIMPLE'
	BEGIN
		RAISERROR ('   *** PROBLEM: Database is using %s Recovery Model.  Use %s for better performance.', 20, 1, @RecoveryModel, 'SIMPLE') WITH NOWAIT, LOG
	END
	
	
	-----------------------------------------------------------------------------
	-- Validate passed-in arguments.															  
	-----------------------------------------------------------------------------

	-- If bDelay is one, the insert will delay 3 seconds before the next insert.
	IF @bDelay < 0 OR @bDelay > 1		SET @bDelay = 0
	
	-- Default to today, and truncate down to date-only (no time portion).
	IF @StartDate IS NULL	SET @StartDate = GETDATE()
	SET @StartDate = CAST(CONVERT(NVARCHAR(50), @StartDate, 101) AS DATETIME)
	SET @StartDateStr = CONVERT(NVARCHAR(50), @StartDate, 120)

	-- Echo the input for posterity.
	RAISERROR ('@SiteIndex       is %4d', 10, 1, @SiteIndex)			WITH NOWAIT, LOG
	RAISERROR ('@NumOfDays       is %4d', 10, 1, @NumOfDays)			WITH NOWAIT, LOG
	RAISERROR ('@RowsPerDay      is %4d', 10, 1, @RowsPerDay)		WITH NOWAIT, LOG
	RAISERROR ('@bDelay          is %4d', 10, 1, @bDelay)				WITH NOWAIT, LOG
	RAISERROR ('@StartDate       is %s',  10, 1, @StartDateStr)		WITH NOWAIT, LOG

	-- Setting the seed to a known number allows us to duplicate the data we generate.
	-- Note that RAND() is not a very good RNG: the seeds need be be far apart to
	-- have any kind of true randomness.
	-- 
	-- All subsequent calls to RAND() must NOT have an argument.
	SET @RandSeed = @@IDLE + (10000 * @SiteIndex)
	RAISERROR ('@RandSeed is %d', 10, 1, @RandSeed)
	PRINT 'First random value is ' + CAST(RAND(@RandSeed) AS NVARCHAR)
	
	
	-----------------------------------------------------------------------------
	-- Drop temp tables before processing new site so identities are set to one. 
	-----------------------------------------------------------------------------

	DROP TABLE dbo.TBL_TEMP_TX_ALIASES
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
	DROP TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
	DROP TABLE dbo.TBL_TEMP_PRODUCT_REF_DATA
	DROP TABLE dbo.TBL_TEMP_EQUIP_REF_DATA
	
	
	-----------------------------------------------------------------------------
	-- Create temp tables for processing.													  
	-----------------------------------------------------------------------------
	
	RAISERROR ('Creating temp tables.', 10, 1) WITH NOWAIT, LOG

	IF OBJECT_ID('dbo.TBL_TEMP_TX_ALIASES') IS NULL
		CREATE TABLE dbo.TBL_TEMP_TX_ALIASES
		(
			TxAliasIndex   INT IDENTITY(1, 1) NOT NULL,
			AliasName	   NVARCHAR(50)			  NULL,
			AliasID		   NVARCHAR(50)			  NULL,
			TransTypeID	   NVARCHAR(50)			  NULL,
			ReferenceIndex INT					 NOT NULL,
			STCLetter		CHAR(1)					  NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
		(
			CompanyIndex	INT IDENTITY(1, 1) NOT NULL,
			CompanyID		NVARCHAR(60)			  NULL,
			CompanyCode		NVARCHAR(50)			  NULL,
			CompanyRole		NVARCHAR(50)			  NULL,
			ReferenceIndex	INT					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER') IS NULL
		CREATE TABLE dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
		(
			CompanyIndex   INT IDENTITY(1, 1) NOT NULL,
			CompanyID	   NVARCHAR(60) 			  NULL,
			CompanyCode	   NVARCHAR(50) 			  NULL,
			CompanyRole	   NVARCHAR(50) 			  NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_PRODUCT_REF_DATA') IS NULL
		CREATE TABLE dbo.TBL_TEMP_PRODUCT_REF_DATA
		(
			ProductIndex   INT IDENTITY(1, 1) NOT NULL,
			ProductID	   NVARCHAR(50) 		 NOT NULL,
			ProductType	   NVARCHAR(50) 		 NOT NULL,
			ProductCode	   NVARCHAR(50) 		 NOT NULL,
			ProductPrice   MONEY 				 NOT NULL,
			ReferenceIndex INT 					 NOT NULL
		)

	IF OBJECT_ID('dbo.TBL_TEMP_EQUIP_REF_DATA') IS NULL
		CREATE TABLE dbo.TBL_TEMP_EQUIP_REF_DATA
		(
			EquipIndex		INT IDENTITY(1, 1) NOT NULL,
			EquipID			NVARCHAR(30) 			  NULL,
			Model				NVARCHAR(50) 			  NULL,
			SerialNumber	NVARCHAR(30) 			  NULL,
			Type				NVARCHAR(50) 			  NULL
		)
	
	RAISERROR ('Created  temp tables.', 10, 1) WITH NOWAIT, LOG
	
	
	-----------------------------------------------------------------------------
	-- Build up alias reference data.														  
	-----------------------------------------------------------------------------

	-- TBL_TEMP_TX_ALIASES
	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_TX_ALIASES)
	BEGIN
		INSERT INTO dbo.TBL_TEMP_TX_ALIASES
			SELECT RTRIM(tblTransactionAliases.AliasName),
					 tblTransactionAliases.AliasID,
					 tblTransactionAliases.TransTypeID,
					 tblTransactionAliases.AliasID AS ReferenceIndex,
					 ' ' AS STCLetter
			 FROM tblTransactionAliases,

					 (SELECT tblEntityToSiteMap.*
						 FROM tblEntityToSiteMap
						WHERE TypeID = 'Transaction Aliases'
						  AND SiteIndex = @SiteIndex) AS tEntities

			 WHERE tEntities.[Index] = tblTransactionAliases.AliasID 
				AND TransTypeID IN (1, 3, 4, 5, 6, 7, 8, 12, 14, 15, 17, 18, 21, 22)			-- Same for all customers.
			 ORDER BY tblTransactionAliases.AliasName

		SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_TX_ALIASES
		IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_TX_ALIASES') WITH NOWAIT, LOG
		ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_TX_ALIASES') WITH NOWAIT, LOG

		-- Mark each Alias as to which STC specification it fulfills.
		UPDATE dbo.TBL_TEMP_TX_ALIASES	SET STCLetter = 'H'	WHERE TransTypeID IN (12)
		UPDATE dbo.TBL_TEMP_TX_ALIASES	SET STCLetter = 'I'	WHERE TransTypeID IN (1, 3, 4, 5, 6, 7, 8)
		UPDATE dbo.TBL_TEMP_TX_ALIASES	SET STCLetter = 'J'	WHERE TransTypeID IN (14, 15, 17, 18, 21, 22)
		
		SELECT @AliasCountH = COUNT(*) FROM dbo.TBL_TEMP_TX_ALIASES WHERE STCLetter = 'H'
		SELECT @AliasCountI = COUNT(*) FROM dbo.TBL_TEMP_TX_ALIASES WHERE STCLetter = 'I'
		SELECT @AliasCountJ = COUNT(*) FROM dbo.TBL_TEMP_TX_ALIASES WHERE STCLetter = 'J'

		SELECT * FROM dbo.TBL_TEMP_TX_ALIASES
	END
	
	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_MANAGER
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
				   FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
		   AND c.CompanyIndex IN
				 (SELECT CompanyIndex
				    FROM tblCompanyRoleMap
				   WHERE tblCompanyRoleMap.Role = 0)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_MANAGER') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_MANAGER') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_OWNER
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
			AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 1)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_OWNER') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_OWNER') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_SHIPPER
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
			AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 2)
	
	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_SHIPPER') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_SHIPPER') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_BILLTO
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
			AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 3)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_BILLTO') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_BILLTO') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_SHIPTO
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
			AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 4)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_SHIPTO') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_SHIPTO') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_CARRIER
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
			AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 5)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_CARRIER') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_CARRIER') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER)
	INSERT INTO dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
		SELECT DISTINCT
				 c.ID,
				 c.Code,
				 'Manager'      AS CompanyRole,
				 c.CompanyIndex AS ReferenceIndex
		  FROM tblCompanies c,

				(SELECT tblEntityToSiteMap.*
					FROM tblEntityToSiteMap
				  WHERE TypeID    = 'Companies'
					 AND SiteIndex = @SiteIndex) AS tEntities

		 WHERE tEntities.[Index] = c.CompanyIndex
		   AND c.CompanyIndex IN
				 (SELECT CompanyIndex
					 FROM tblCompanyRoleMap
					WHERE tblCompanyRoleMap.Role = 6)

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_COMPANY_REF_DATA_SUPPLIER') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_COMPANY_REF_DATA_SUPPLIER') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_EQUIP_REF_DATA
	-----------------------------------------------------------------------------

	IF NOT EXISTS (SELECT TOP 1 * FROM dbo.TBL_TEMP_EQUIP_REF_DATA)
	INSERT INTO dbo.TBL_TEMP_EQUIP_REF_DATA
		SELECT ID,
				 Model,
				 SerialNumber,
				 (CASE Type
						WHEN  0  THEN 'TRAILER'
						WHEN  1  THEN 'TRACTOR'
						WHEN  2  THEN 'AIRCRAFT'
						WHEN  3  THEN 'RAILCAR'
						WHEN  4  THEN 'BARGE'
						WHEN  5  THEN 'COMPARTMENT'
						WHEN  6  THEN 'SHIP'
						WHEN  7  THEN 'PIPELINE'
						WHEN  8  THEN 'HYDRANT_CART'
						WHEN  9  THEN 'TANKER'
						WHEN 10  THEN 'STATIONARY_CART'
					END) AS Type
		  FROM tblEquipment,
				 (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Equipment' AND SiteIndex = @SiteIndex) AS tEntities
		  WHERE tblEquipment.[Type] IN (0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10) AND tblEquipment.[Index] = tEntities.[Index]

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_EQUIP_REF_DATA 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_EQUIP_REF_DATA') WITH NOWAIT, LOG
	ELSE RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_EQUIP_REF_DATA') WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- TBL_TEMP_PRODUCT_REF_DATA - Must be rebuilt every time.  Don't call DBCC  
	-- CHECKIDENT - it can take over a minute to return!								  
	-----------------------------------------------------------------------------

	RAISERROR ('Deleting and rebuilding %s.', 10, 1, 'TBL_TEMP_PRODUCT_REF_DATA') WITH NOWAIT, LOG
	DELETE FROM dbo.TBL_TEMP_PRODUCT_REF_DATA

	INSERT INTO dbo.TBL_TEMP_PRODUCT_REF_DATA
		SELECT tblProducts.ProductID,
				(CASE tblProducts.ProductType
						WHEN 0 THEN 'COMPONENT_PRODUCT'
						WHEN 1 THEN 'BLEND_PRODUCT'
						WHEN 2 THEN 'ADDITIVE_PRODUCT'
						WHEN 3 THEN 'ADDITIZED_PRODUCT'
				 END) AS colProductType,
				 tblProducts.ProductCode,
				 tblProducts.Price,
				 tblProducts.ProductIndex AS ReferenceIndex
		  FROM tblProducts,
				 (SELECT tblEntityToSiteMap.* FROM tblEntityToSiteMap WHERE TypeID = 'Products' AND SiteIndex = @SiteIndex) AS tEntities
		  WHERE tblProducts.ProductType IN (0, 1, 2, 3) AND tblProducts.ProductIndex = tEntities.[Index]

	SELECT @RowCount = COUNT(*) FROM dbo.TBL_TEMP_PRODUCT_REF_DATA 
	IF @RowCount > 0  RAISERROR ('There are %8d rows in %s.', 10, 1, @RowCount, 'TBL_TEMP_PRODUCT_REF_DATA') WITH NOWAIT, LOG
	ELSE  RAISERROR ('   *** PROBLEM: Zero rows found in %s.', 20, 1, 'TBL_TEMP_PRODUCT_REF_DATA') WITH NOWAIT, LOG

	SELECT * FROM dbo.TBL_TEMP_PRODUCT_REF_DATA
	

	-----------------------------------------------------------------------------
	-- Get the highest numbered index from the temp tables.							  
	-----------------------------------------------------------------------------

	SELECT @CompanyManagerMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
	SELECT @CompanyOwnerMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
	SELECT @CompanyShipperMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
	SELECT @CompanyBilltoMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
	SELECT @CompanyShiptoMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
	SELECT @CompanyCarrierMax 	= MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
	SELECT @CompanySupplierMax = MAX(CompanyIndex) FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
	SELECT @ProductMax 			= MAX(ProductIndex) FROM dbo.TBL_TEMP_PRODUCT_REF_DATA
	SELECT @EquipMax 				= MAX(EquipIndex)   FROM dbo.TBL_TEMP_EQUIP_REF_DATA
	SELECT @AliasMax 				= MAX(TxAliasIndex) FROM dbo.TBL_TEMP_TX_ALIASES
	SELECT @SiteID = ID FROM tblSites WHERE SiteIndex = @SiteIndex

	RAISERROR ('@CompanyManagerMax  is %8d', 10, 1, @CompanyManagerMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanyOwnerMax	  is %8d', 10, 1, @CompanyOwnerMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanyShipperMax  is %8d', 10, 1, @CompanyShipperMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanyBilltoMax	  is %8d', 10, 1, @CompanyBilltoMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanyShiptoMax	  is %8d', 10, 1, @CompanyShiptoMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanyCarrierMax  is %8d', 10, 1, @CompanyCarrierMax) 	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@CompanySupplierMax is %8d', 10, 1, @CompanySupplierMax)	-- slow    WITH NOWAIT, LOG
	RAISERROR ('@ProductMax			  is %8d', 10, 1, @ProductMax)			-- slow    WITH NOWAIT, LOG
	RAISERROR ('@SiteID             is %8s', 10, 1, @SiteID) 				-- slow    WITH NOWAIT, LOG


	-----------------------------------------------------------------------------
	-- Set so that the total rows per day is what the user requested. There will 
	-- be a loop for the rows per day and inside that loop will be a loop for    
	-- aliases. The two loops should generate the number of rows per day.        
	-----------------------------------------------------------------------------
	
	IF @RowsPerDayOrig < 2								-- 1 day.
	BEGIN
		SET @RowsPerDay = 1
		SET @TotalAliasCount = 1
	END
	ELSE IF @RowsPerDayOrig > @AliasMax				-- 30 rows per day, 13 aliases.
	BEGIN
		SET @RowsPerDay = @RowsPerDayOrig / @AliasMax
		IF @RowsPerDay = 0	SET @RowsPerDay = 1
		SET @TotalAliasCount = @RowsPerDayOrig / @RowsPerDay
	END
	ELSE
	BEGIN														-- 4 rows per day, 13 aliases.
		SET @RowsPerDay = SQRT(@RowsPerDayOrig)
		IF @RowsPerDay = 0	SET @RowsPerDay = 1
		SET @TotalAliasCount = @RowsPerDay
	END
	
	WHILE @RowsPerDayOrig > @RowsPerDay * @TotalAliasCount
	BEGIN
		SET @RowsPerDay = @RowsPerDay + 1
		PRINT '@RowsPerDay incremented to ' + CAST(@RowsPerDay AS NVARCHAR)
	END

	RAISERROR ('To get %d rows per day spread among %d Aliases, resetting @RowsPerDay to %d and @TotalAliasCount to %d.',
		10, 1, @RowsPerDayOrig, @AliasMax, @RowsPerDay, @TotalAliasCount) WITH NOWAIT, LOG

	-- Specifies the number of milliseconds a statement waits for a lock to be released.
	SET LOCK_TIMEOUT 25000


	-----------------------------------------------------------------------------
	-- Loop until all days have expired for years sent.								  
	-----------------------------------------------------------------------------

	WHILE @DayCounter < @NumOfDays
	BEGIN

		-- Set up date variables for this day.  @StartDate is truncated to date-only.
		SET @CreatedUpdatedDate = DATEADD(DAY, @DayCounter, @StartDate)
		SET @InventoryDateStr   = CONVERT(NVARCHAR(22), @CreatedUpdatedDate, 101)
		RAISERROR ('   Looping through day %d of %d days (%s)...', 10, 1, @DayCounter, @NumOfDays, @InventoryDateStr) WITH NOWAIT, LOG

		-- Loop for inserting X number of transaction rows per day.
		SET @RowCounter = 0
		SET @SignToggle = 1.0

		WHILE @RowCounter < @RowsPerDay
		BEGIN
			--RAISERROR ('      Looping through row %d of %d rows...', 10, 1, @RowCounter, @RowsPerDay) -- slow    WITH NOWAIT, LOG

			-- Set the random indexes to be within the boundries of the tables.
			SET @CompanyManagerIndex  = 1 + CAST((RAND() * @CompanyManagerMax)  	AS INT)
			SET @CompanyOwnerIndex    = 1 + CAST((RAND() * @CompanyOwnerMax) 		AS INT)
			SET @CompanyShipperIndex  = 1 + CAST((RAND() * @CompanyShipperMax)  	AS INT)
			SET @CompanyBilltoIndex   = 1 + CAST((RAND() * @CompanyBilltoMax)		AS INT)
			SET @CompanyShiptoIndex   = 1 + CAST((RAND() * @CompanyShiptoMax) 	AS INT)
			SET @CompanyCarrierIndex  = 1 + CAST((RAND() * @CompanyCarrierMax)  	AS INT)
			SET @CompanySupplierIndex = 1 + CAST((RAND() * @CompanySupplierMax)	AS INT)
			SET @ProductIndex         = 1 + CAST((RAND() * @ProductMax) 			AS INT)
			SET @EquipIndex           = 1 + CAST((RAND() * @EquipMax) 				AS INT)

			--RAISERROR ('         @CompanyManagerIndex  is %8d', 10, 1, @CompanyManagerIndex) 	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanyOwnerIndex    is %8d', 10, 1, @CompanyOwnerIndex) 	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanyShipperIndex  is %8d', 10, 1, @CompanyShipperIndex) 	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanyBilltoIndex   is %8d', 10, 1, @CompanyBilltoIndex)	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanyShiptoIndex   is %8d', 10, 1, @CompanyShiptoIndex) 	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanyCarrierIndex  is %8d', 10, 1, @CompanyCarrierIndex) 	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @CompanySupplierIndex is %8d', 10, 1, @CompanySupplierIndex)	-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @ProductIndex         is %8d', 10, 1, @ProductIndex) 			-- slow    WITH NOWAIT, LOG
			--RAISERROR ('         @EquipIndex           is %8d', 10, 1, @EquipIndex) 				-- slow    WITH NOWAIT, LOG


			-----------------------------------------------------------------------------
			-- Load variables from the temp tables for later insertion into real tables. 
			-----------------------------------------------------------------------------

			SELECT @EquipIDR           = EquipID,
					 @EquipModelR        = Model,
					 @EquipSerialNumberR = SerialNumber,
					 @EquipTypeR         = Type
			  FROM dbo.TBL_TEMP_EQUIP_REF_DATA
			 WHERE EquipIndex = @EquipIndex

			SELECT @ProductIDR    = ProductID,
					 @ProductCodeR  = ProductCode,
					 @ProductTypeR  = ProductType,
					 @ProductPriceR = ProductPrice,
					 @ProductIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_PRODUCT_REF_DATA
			 WHERE ProductIndex = @ProductIndex

			SELECT @ManagerIDR    = CompanyID,
					 @ManagerCodeR  = CompanyCode,
					 @ManagerIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_MANAGER
			 WHERE CompanyIndex = @CompanyManagerIndex

			SELECT @OwnerIDR    = CompanyID,
					 @OwnerCodeR  = CompanyCode,
					 @OwnerIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_OWNER
			 WHERE CompanyIndex   = @CompanyOwnerIndex

			SELECT @ShipperIDR    = CompanyID,
					 @ShipperCodeR  = CompanyCode,
					 @ShipperIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPPER
			 WHERE CompanyIndex  = @CompanyShipperIndex

			SELECT @BillToIDR    = CompanyID,
					 @BillToCodeR  = CompanyCode,
					 @BillToIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_BILLTO
			 WHERE CompanyIndex  = @CompanyBilltoIndex

			SELECT @ShipToIDR    = CompanyID,
					 @ShipToCodeR  = CompanyCode,
					 @ShipToIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SHIPTO
			 WHERE CompanyIndex   = @CompanyShiptoIndex

			SELECT @CarrierIDR    = CompanyID,
					 @CarrierCodeR  = CompanyCode,
					 @CarrierIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_CARRIER
			 WHERE CompanyIndex    = @CompanyCarrierIndex

			SELECT @SupplierIDR    = CompanyID,
					 @SupplierCodeR  = CompanyCode,
					 @SupplierIndexR = ReferenceIndex
			  FROM dbo.TBL_TEMP_COMPANY_REF_DATA_SUPPLIER
			 WHERE CompanyIndex = @CompanySupplierIndex

			-- For each row generate a new alias for x number of aliases.
			SET @AliasCount = 0

			WHILE @AliasCount < @TotalAliasCount
			BEGIN
	
				-- Randomize all aliases regardless of the letter
				SET @AliasRowNumForLetter = 1 + CAST((RAND() * (@AliasCountH+@AliasCountI+@AliasCountJ)) AS INT)
				SELECT @AliasIndex = AliasID FROM dbo.TBL_TEMP_TX_ALIASES WHERE TxAliasIndex = @AliasRowNumForLetter

				-- Now that we know which Alias we want, get its values.
				SELECT @AliasNameR   = AliasName,
						 @TransTypeIDR = TransTypeID,
						 @AliasIndexR  = ReferenceIndex
				  FROM dbo.TBL_TEMP_TX_ALIASES
				 WHERE AliasID = @AliasIndex

				-- Now we create a poor man's sequential GUID. In the real code they do this in C#. 
				-- Since some of these tables are clustered on a GUID, it's critical this be sequential. 
				-- Since there is already a million in the table, we pad with leading F's to be sure they 
				-- are appended to the end.  We have to make this unique per site, so we insert @SiteIndex.
				-- Result will look like 'FFFFFFFF-FFFF-FFFF-003A-00000000000120CB'.
				-- Never, ever, cluster on a GUID.  Not even a sequential one.
				DECLARE @sIHateGuids NVARCHAR(18)
				SET @sIHateGuids = dbo.IntToHexString(CAST(@SiteIndex AS SMALLINT))
				SET @sIHateGuids = REPLACE(@sIHateGuids, '0x', '')

				SET @sGuid = dbo.IntToFakeGuidString(@nGuid, 'F')
				SET @sGuid = STUFF(@sGuid, 20, 4, @sIHateGuids)
				
				SET @nGuid = @nGuid + 1

				-- Insert the header part of the transaction row.
				SET @DeleteFlag = 0
				RAISERROR ('         Looping through alias %2d of %2d aliases.  @AliasIndex is %2d (%s)', 10, 1, @AliasCount, @TotalAliasCount, @AliasIndex, @AliasNameR) -- slow    WITH NOWAIT, LOG
				BEGIN TRANSACTION

				INSERT INTO tblTransactions
					(TransID, InventoryDate, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy, AliasName, TransTypeID, Site,
					 ShipToID, ShipToCode, SupplierID, SupplierCode, ManagerID, ManagerCode, OwnerID, OwnerCode, BillToID,
					 BillToCode, CarrierID, CarrierCode, ShipperID, ShipperCode, DeleteFlag, ReversalType, TransactionStatus,
					 InternationalRouteIndicator, TicketMode, TransDateTime, ManagerIndex, OwnerIndex, BillToIndex,
					 ShipToIndex, CarrierIndex, SupplierIndex, ShipperIndex, AliasIndex, SiteIndex)
				VALUES
					(@sGuid, @InventoryDateStr, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Owl', 'Owl', @AliasNameR,	@TransTypeIDR, @SiteID,
					 @ShipToIDR, @ShipToCodeR,	@SupplierIDR, @SupplierCodeR, @ManagerIDR, @ManagerCodeR, @OwnerIDR, @OwnerCodeR, @BillToIDR,
					 @BillToCodeR, @CarrierIDR, @CarrierCodeR, @ShipperIDR, @ShipperCodeR, @DeleteFlag,	' ', 0,
					 0, 0, @InventoryDateStr, @ManagerIndexR, @OwnerIndexR, @BillToIndexR,
					 @ShipToIndexR, @CarrierIndexR, @SupplierIndexR, @ShipperIndexR, @AliasIndexR, @SiteIndex)

				SET @GrossQuantity = RAND() * 100
				SET @NetQuantity = @GrossQuantity

				-- On issues types change the value to be a negative.  In addition, change the sign toggle variable.
				IF @TransTypeIDR = 5  OR  @TransTypeIDR = 6
				BEGIN
					SET @GrossQuantity = @GrossQuantity * -1.0
					SET @NetQuantity   = @NetQuantity   * -1.0
					SET @SignToggle    = @SignToggle    * -1.0
				END

				-- On Adjustments and Physical inventories multiple by the sign toggle value.
				IF @TransTypeIDR = 1  OR  @TransTypeIDR = 14
				BEGIN
					SET @GrossQuantity = @GrossQuantity * @SignToggle
					SET @NetQuantity   = @NetQuantity   * @SignToggle
				END

				-- Get the last ID in the line item table.
				SELECT @TxLineItemIDMax = MAX(TransLineItemID)
				  FROM tblTransactionLineItems
				IF @TxLineItemIDMax IS NULL
				BEGIN
					SET @TxLineItemIDMax = 0
				END

				SET @TxLineItemIDMax = @TxLineItemIDMax + 1

				-- Ensure that the accounting line item sequence number is kept up to date.
				UPDATE tblAccountingSequences
					SET SequenceValue = @TxLineItemIDMax
				 WHERE SequenceName  = 'LineItemID'

				-- Debug line for testing (IGO 01-May-2009)
				-- print cast(@TxLineItemIDMax as nvarchar) + ' ' + @sGuid + ' ' + @ProductIDR + ' ' + @ProductCodeR + ' ' + @ProductTypeR + ' ' + cast(@ProductPriceR as nvarchar) + ' ' + @EquipIDR + ' ' + @EquipSerialNumberR + ' ' + @EquipTypeR + ' ' + @EquipModelR + ' ' + cast(@GrossQuantity as nvarchar) + ' ' + cast(@NetQuantity as nvarchar) + ' ' + cast(@DeleteFlag as nvarchar) + ' ' + cast(@ProductIndexR as nvarchar) + ' ' + cast(@InventoryDateStr as nvarchar)

				-- Insert the line item part of the transaction row.
				INSERT INTO tblTransactionLineItems
					(TransLineItemID, TransID, SequenceID, LineItemSequenceNumber, Product, ProductCode, ProductType,
					 ProductPrice, DestinationRegistrationID, DestinationSerialNumber, DestinationEquipmentType, CreatedBy,
					 DestinationEquipmentModel, GrossQuantity, NetQuantity, DeleteFlag, 
					 ProductIndex, TransactionStatus, TransactionInventoryDate, Quality)
				VALUES
					(@TxLineItemIDMax, @sGuid, 0, 0, @ProductIDR, @ProductCodeR, @ProductTypeR,
					 @ProductPriceR, @EquipIDR, @EquipSerialNumberR, @EquipTypeR, 'Owl',
					 @EquipModelR, @GrossQuantity, @NetQuantity, @DeleteFlag,
					 @ProductIndexR, 0, @InventoryDateStr, 1)
				
				-- Need empty rows in the following tables.
				INSERT INTO tblTransactionLineItemUserData
					(TransLineItemID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy)
				VALUES
					(@TxLineItemIDMax, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Owl', 'Owl')

				INSERT INTO tblTransactionUserData
					(TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy)
				VALUES
					(@sGuid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Owl', 'Owl')

				INSERT INTO tblTransactionNotes
					(TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy)
				VALUES
					(@sGuid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Owl', 'Owl')

				INSERT INTO tblTransactionWeightReadings
					(TransID, CreatedDate, UpdatedDate, CreatedBy, UpdatedBy, BeginQuantityValue,
					 RequestedQuantityValue, FinalQuantityValue, CompartmentID)
				VALUES
					(@sGuid, @CreatedUpdatedDate, @CreatedUpdatedDate, 'Owl', 'Owl', 0.0, 0.0, 0.0, '')

				COMMIT TRANSACTION

				-- Optional three-second delay.
				IF @bDelay = 1		WAITFOR DELAY '00:00:03'
				
				SET @AliasCount = @AliasCount + 1
			END

			-- End alias loop.
			SET @RowCounter = @RowCounter + 1
			SET @RowsAddedThisRun = @RowsAddedThisRun + 1
		END

		-- End row loop.
		SET @DayCounter = @DayCounter + 1

		EXEC tmp_usp_TimingReport @StartTime, @TotalRows, @RowsAddedThisRun
	END
	-- End days loop.

	RAISERROR ('Finished site %d successfully.', 10, 1, @SiteIndex) WITH NOWAIT, LOG
END

GO

RAISERROR ('Created  stored procedure usp_CreateManyTransactions_v2', 10, 1) WITH NOWAIT, LOG


-------------------------------------------------------------------------------
-- Helper proc to report "rows per minute".												 
-------------------------------------------------------------------------------

-- Drop any existing one. 
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'tmp_usp_TimingReport')
BEGIN
	RAISERROR ('Dropping stored procedure tmp_usp_TimingReport', 10, 1) WITH NOWAIT, LOG
	DROP PROCEDURE tmp_usp_TimingReport
	RAISERROR ('Dropped  stored procedure tmp_usp_TimingReport', 10, 1) WITH NOWAIT, LOG
END

-- Create stored procedure. 
RAISERROR ('Creating stored procedure tmp_usp_TimingReport', 10, 1) WITH NOWAIT, LOG
GO

CREATE PROCEDURE dbo.tmp_usp_TimingReport(@StopwatchStart	DATETIME,
														@TotalRowsToAdd	INT,			-- Desired number of rows to end up with.
														@RowsAddedThisRun	INT)
AS
BEGIN
	DECLARE @StopwatchStop			DATETIME
	DECLARE @ElapsedSeconds			INT
	DECLARE @ElapsedMins				INT
	DECLARE @RowsPerMinProcessed	INT
	DECLARE @RowsRemaining			INT
 	DECLARE @RequiredMins			INT
 	DECLARE @dateETA					DATETIME
 	DECLARE @sETA						NVARCHAR(400)
	DECLARE @sStopwatchStop			NVARCHAR

	SET @sStopwatchStop = CAST(GETDATE() AS NVARCHAR)

	IF @sStopwatchStop IS NULL
	BEGIN
		RAISERROR ('*** Error: NULL??: %s', 10, 1, @sStopwatchStop)
		RETURN
	END

	SET @sStopwatchStop = CAST(@StopwatchStop AS NVARCHAR)
	RAISERROR ('             Current Time: %s', 10, 1, @sStopwatchStop)
	--print  @sStopwatchStop -- ??

	SET @ElapsedSeconds = DATEDIFF(second, @StopwatchStart, @StopwatchStop)
	SET @ElapsedMins = @ElapsedSeconds / 60
	RAISERROR ('             Elapsed time: %d minutes', 10, 1, @ElapsedMins) WITH NOWAIT, LOG
 
	RAISERROR ('             Rows Added This Run: %d', 10, 1, @RowsAddedThisRun) WITH NOWAIT, LOG

	IF @ElapsedSeconds = 0  SET @ElapsedSeconds = 1
	SET @RowsPerMinProcessed = (@RowsAddedThisRun  * 60) / @ElapsedSeconds
	RAISERROR ('             Rows Processed Per Min: %d', 10, 1, @RowsPerMinProcessed) WITH NOWAIT, LOG

	IF @RowsPerMinProcessed = 0  SET @RowsPerMinProcessed = 1
	SET @RowsRemaining = @TotalRowsToAdd - @RowsAddedThisRun
	RAISERROR ('             Rows Remaining: %d', 10, 1, @RowsPerMinProcessed) WITH NOWAIT, LOG

	SET @RequiredMins = @RowsRemaining / @RowsPerMinProcessed
	SET @dateETA = DATEADD(minute, @RequiredMins, GETDATE())
	SET @sETA = 'Estimated completion: ' + CAST(@dateETA AS NVARCHAR)
	RAISERROR (@sETA, 10, 1) WITH NOWAIT, LOG
	RAISERROR (' ', 10, 1) WITH NOWAIT
END

GO

-------------------------------------------------------------------------------
----										END OF FILE											    
-------------------------------------------------------------------------------
