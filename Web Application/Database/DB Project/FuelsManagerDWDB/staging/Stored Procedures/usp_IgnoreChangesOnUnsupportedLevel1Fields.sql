/*
	DROP PROCEDURE [Staging].[usp_IgnoreChangesOnUnsupportedLevel1Fields]

	EXEC [staging].[usp_IgnoreChangesOnUnsupportedLevel1Fields]
	
*/
CREATE PROCEDURE [staging].[usp_IgnoreChangesOnUnsupportedLevel1Fields]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_IgnoreChangesOnUnsupportedLevel1Fields]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Ignore record changes to Level 1 entity tables when the changes do not affect the fields that are supported on the 
  --          corresponding OLAP dimensions.
  -- Notes:
  -- 1. This process is limited to tables for which historical records are captured on the OLTP database.
  -- 2. This procedure prevents the unnecessary generation of new historical records for record changes on fields that are not covered
  --    in the data warehouse.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

	DECLARE @dummyId varchar(15) = '<NOT AVAILABLE>'
    DECLARE @shortDummyId varchar(4) = '<NA>'
	DECLARE @veryShortDummyId varchar(2) = 'NA'
    DECLARE @dummyDate datetime = '1/1/1900'
    DECLARE @dummyKey uniqueidentifier = cast(cast(0 as binary) as uniqueidentifier)
	DECLARE @dummyTransactionTypeSKey int = -100

	DECLARE @sKey int
	DECLARE @aKey nvarchar(50)
	DECLARE @previousAKey nvarchar(50)
	DECLARE @validChangesDetectedAgainstDim bit
	DECLARE @dimChecksum int
	DECLARE @stagingChecksum int
	DECLARE @previousStagingChecksum int
	
	-- Product
	SET @validChangesDetectedAgainstDim = 0
	SET @previousAKey = ''
	TRUNCATE TABLE staging.tblEntityChecksum
	TRUNCATE TABLE staging.tblProductComparisonTemp

	INSERT INTO staging.tblProductComparisonTemp
	(SourceTable, ProductSKey, ProductKey, SiteKey, ProductId, ProductCode, Description, ProductTypeName, TrackingProductSKey, TrackingProductId, VolumeDecimalPlaces, AviationFuelFlag, GroundFuel, LockedOut, LockedOutReason, LockedOutDate, VarianceTolerance, StartDate, EndDate)
	SELECT 'Staging', 
	SKey, 
	ProductKey, 
	SiteKey, 	
	TRIM([ProductID]) [ProductID],        
    ISNULL([ProductCode], @dummyId) [ProductCode],
    [Description],
    [ProductTypeName],
	ISNULL([TrackingProductSKey], 0), 
	[TrackingProductID], 
	[VolumeDecimalPlaces],
    ISNULL([AviationFuelFlag], 0) [AviationFuelFlag],
    ISNULL([GroundFuel], 0) [GroundFuel],
    ISNULL([LockedOut], 0) [LockedOut],
    [LockedOutReason],
    [LockedOutDate],
    ISNULL([VarianceTolerance], 0) [VarianceTolerance],
	StartDate, 
	EndDate
	FROM staging.tblProducts
	WHERE IgnoreRecord <> 1
	AND ISNULL(IsRecordDeleted, 0) = 0

	INSERT INTO staging.tblProductComparisonTemp
	(SourceTable, ProductSKey, ProductKey, SiteKey, ProductId, ProductCode, Description, ProductTypeName, TrackingProductSKey, TrackingProductId, VolumeDecimalPlaces, AviationFuelFlag, GroundFuel, LockedOut, LockedOutReason, LockedOutDate, VarianceTolerance, StartDate, EndDate)
	SELECT 'Dim', b.SKey, b.AKey, c.AKey, b.ProductID, b.ProductCode, b.Description, b.ProductTypeName, b.TrackingProductSKey, b.TrackingProductID, b.VolumeDecimalPlaces, b.AviationFuelFlag, b.GroundFuel, b.LockedOut, b.LockedOutReason, b.LockedOutDate, b.VarianceTolerance, b.StartDate, b.EndDate
	FROM (SELECT DISTINCT ProductKey FROM staging.tblProductComparisonTemp) a
	INNER JOIN dbo.DimProduct b
	ON b.AKey = a.ProductKey
	INNER JOIN dbo.DimSite c
	ON c.SKey = b.SiteSKey
	WHERE b.EndDate IS NULL

	UPDATE staging.tblProductComparisonTemp
	SET RecordChecksum = CHECKSUM
	(
		ProductId, 		 
		ProductCode,
		Description,
		ProductTypeName, 
		VolumeDecimalPlaces,
		AviationFuelFlag,
		GroundFuel,
		LockedOut,
		LockedOutReason, 
		LockedOutDate,
		VarianceTolerance
	)

	INSERT INTO staging.tblEntityChecksum
	(EntitySKey, EntityKey, StartDate, RecordChecksum)
	SELECT ProductSKey, ProductKey, StartDate, RecordChecksum
	FROM staging.tblProductComparisonTemp
	WHERE SourceTable = 'Staging'
	ORDER BY ProductKey, StartDate

	UPDATE a
    SET a.RecordPreviousChecksum = b.RecordChecksum
    FROM staging.tblEntityChecksum a
    INNER JOIN staging.tblEntityChecksum b
      ON b.EntityKey = a.EntityKey
      AND b.RowIndex = (a.RowIndex - 1)

	UPDATE a
	SET a.DimChecksum = c.RecordChecksum
	FROM staging.tblEntityChecksum a
	INNER JOIN
	(SELECT DISTINCT EntityKey FROM staging.tblEntityChecksum) b
	ON b.EntityKey = a.EntityKey
	INNER JOIN staging.tblProductComparisonTemp c
	ON c.ProductKey = b.EntityKey
	WHERE c.SourceTable = 'Dim' 
	AND c.EndDate IS NULL


	-- Because Product has both Level 1 components and a Level 2 component (TrackingProduct), it is handled differently when detecting the record changes.
	-- Allow all record changes to Product Keys for which the record changes include a change to the TrackingProductKey within the staging Product table itself. 
	-- Cannot check on DimProduct.TrackingProductSKey changes on individual Staging records as they are not available yet in staging.Products. 
	-- Allow all changes for this whole Product AKey.
	UPDATE a
	SET a.IgnoreRecord = 1
	FROM
	staging.tblEntityChecksum a
	INNER JOIN
	(
		SELECT ProductKey, TrackingProductKey, COUNT(*) RecCount
		FROM staging.tblProducts
		GROUP BY ProductKey, TrackingProductKey
	) b
	ON b.ProductKey = a.EntityKey
	INNER JOIN
	(
		SELECT ProductKey, COUNT(*) RecCount
		FROM staging.tblProducts
		GROUP BY ProductKey
	) c
	ON c.ProductKey = a.EntityKey
	WHERE b.RecCount <> c.RecCount
	
	-- Allow all record changes to Product Keys for which the record changes include a change to the TrackingProductKey as compared to the DimProduct record. 
	-- Cannot check on DimProduct.TrackingProductSKey changes on individual Staging records as they are not available yet in staging.Products. 
	-- Allow all changes for this whole Product AKey.
	UPDATE a
	SET a.IgnoreRecord = 1
	FROM
	staging.tblEntityChecksum a
	INNER JOIN staging.tblProducts b
	ON b.SKey = a.EntitySKey
	INNER JOIN
	(
		SELECT c.AKey, d.AKey TrackingProductKey FROM dbo.DimProduct c
		INNER JOIN dbo.DimProduct d
		ON d.SKey = c.TrackingProductSKey
		WHERE c.EndDate IS NULL
	) e
	ON e.AKey = b.ProductKey
	WHERE b.TrackingProductKey <> e.TrackingProductKey
	
	UPDATE a
	SET a.IgnoreRecord = 1
	FROM staging.tblEntityChecksum a
	INNER JOIN staging.tblProducts b
	ON b.SKey = a.EntitySKey
	INNER JOIN
	(
		SELECT c.AKey, c.TrackingProductSKey FROM dbo.DimProduct c
		WHERE c.EndDate IS NULL
	) e
	ON e.AKey = b.ProductKey
	WHERE ((b.TrackingProductKey IS NULL) AND (NULLIF(e.TrackingProductSKey, 0) IS NOT NULL))
	OR
	((b.TrackingProductKey IS NOT NULL) AND (NULLIF(e.TrackingProductSKey, 0) IS NULL))

	
	DECLARE TableCursor CURSOR FOR 
	  SELECT EntitySKey, EntityKey, RecordChecksum, RecordPreviousChecksum, DimChecksum FROM staging.tblEntityChecksum
	  WHERE IgnoreRecord <> 1
	  ORDER BY EntityKey, StartDate
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum

	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF (@aKey <> @previousAKey)
		BEGIN
			SET @validChangesDetectedAgainstDim = 0
		END

		--Compare staging record with dim record
		IF (@validChangesDetectedAgainstDim = 0) 
		BEGIN						
			IF (@stagingChecksum = @dimChecksum)
			BEGIN
				UPDATE staging.tblProducts
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
				
				SET @previousAKey = @aKey				
				SET @validChangesDetectedAgainstDim = 1
				CONTINUE
			END
		END
	
		--Compare staging record with previous staging record
		IF (@previousStagingChecksum IS NOT NULL)
		BEGIN		
			IF (@stagingChecksum = @previousStagingChecksum)
			BEGIN
				UPDATE staging.tblProducts
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
			END
		END
		SET @previousAKey = @aKey				

		FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum
	END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 
	

	
    -- Company
	SET @validChangesDetectedAgainstDim = 0
	SET @previousAKey = ''
	TRUNCATE TABLE staging.tblEntityChecksum
	TRUNCATE TABLE staging.tblCompanyComparisonTemp	

	INSERT INTO staging.tblCompanyComparisonTemp
	(SourceTable, CompanySKey, CompanyKey, SiteKey, CompanyId, CompanyName, CompanyCode, Address1, Address2, City, State, Zip, Country, Phone, EmergencyContact, EmergencyPhone, LockedOut, LockedOutReason, LockedOutDate, StartDate, EndDate)
	SELECT 'Staging', 
	SKey, 
	CompanyKey, 
	SiteKey, 
	TRIM([ID]) [CompanyID],  
    [Name],
    [Code],
    [Address1],
    [Address2],
    [City],
    [State],
    [Zip],
    [Country],
    [Phone],
    [EmergencyContact],
    [EmergencyPhone],
    ISNULL([LockedOut], 0) [LockedOut],
    [LockedOutReason],
    [LockedOutDate],
	StartDate, 
	EndDate
	FROM staging.tblCompanies
	WHERE IgnoreRecord <> 1
	AND ISNULL(IsRecordDeleted, 0) = 0

	INSERT INTO staging.tblCompanyComparisonTemp
	(SourceTable, CompanySKey, CompanyKey, SiteKey, CompanyId, CompanyName, CompanyCode, Address1, Address2, City, State, Zip, Country, Phone, EmergencyContact, EmergencyPhone, LockedOut, LockedOutReason, LockedOutDate, StartDate, EndDate)
	SELECT 'Dim', b.SKey, b.AKey, c.AKey, b.CompanyId, b.Name, b.Code, b.Address1, b.Address2, b.City, b.State, b.Zip, b.Country, b.Phone, b.EmergencyContact, b.EmergencyPhone, b.LockedOut, b.LockedOutReason, b.LockedOutDate, b.StartDate, b.EndDate
	FROM (SELECT DISTINCT CompanyKey FROM staging.tblCompanyComparisonTemp) a
	INNER JOIN dbo.DimCompany b
	ON b.AKey = a.CompanyKey
	INNER JOIN dbo.DimSite c
	ON c.SKey = b.SiteSKey
	WHERE b.EndDate IS NULL

	UPDATE staging.tblCompanyComparisonTemp
	SET RecordChecksum = CHECKSUM
	(
		[CompanyId], 
		[CompanyName], 
		[CompanyCode], 
		[Address1],
		[Address2],
		[City], 
		[State], 
		[Zip],
		[Country],
		[Phone],
		[EmergencyContact],
		[EmergencyPhone],
		[LockedOut],
		[LockedOutReason],
		[LockedOutDate]
	)

	INSERT INTO staging.tblEntityChecksum
	(EntitySKey, EntityKey, StartDate, RecordChecksum)
	SELECT CompanySKey, CompanyKey, StartDate, RecordChecksum
	FROM staging.tblCompanyComparisonTemp
	WHERE SourceTable = 'Staging'
	ORDER BY CompanyKey, StartDate

	UPDATE a
    SET a.RecordPreviousChecksum = b.RecordChecksum
    FROM staging.tblEntityChecksum a
    INNER JOIN staging.tblEntityChecksum b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = (a.RowIndex - 1)

	UPDATE a
	SET a.DimChecksum = c.RecordChecksum
	FROM staging.tblEntityChecksum a
	INNER JOIN
	(SELECT DISTINCT EntityKey FROM staging.tblEntityChecksum) b
	ON b.EntityKey = a.EntityKey
	INNER JOIN staging.tblCompanyComparisonTemp c
	ON c.CompanyKey = b.EntityKey
	WHERE c.SourceTable = 'Dim' 
	AND c.EndDate IS NULL
	
	DECLARE TableCursor CURSOR FOR 
	  SELECT EntitySKey, EntityKey, RecordChecksum, RecordPreviousChecksum, DimChecksum FROM staging.tblEntityChecksum
	  WHERE IgnoreRecord <> 1
	  ORDER BY EntityKey, StartDate
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum

	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF (@aKey <> @previousAKey)
		BEGIN
			SET @validChangesDetectedAgainstDim = 0
		END

		--Compare staging record with dim record
		IF (@validChangesDetectedAgainstDim = 0) 
		BEGIN						
			IF (@stagingChecksum = @dimChecksum)
			BEGIN
				UPDATE staging.tblCompanies
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
				
				SET @previousAKey = @aKey				
				SET @validChangesDetectedAgainstDim = 1
				CONTINUE
			END
		END
	
		--Compare staging record with previous staging record
		IF (@previousStagingChecksum IS NOT NULL)
		BEGIN		
			IF (@stagingChecksum = @previousStagingChecksum)
			BEGIN
				UPDATE staging.tblCompanies
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
			END
		END
		SET @previousAKey = @aKey				

		FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum
	END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 
	
	
	-- Equipment
	SET @validChangesDetectedAgainstDim = 0
	SET @previousAKey = ''
	TRUNCATE TABLE staging.tblEntityChecksum
	TRUNCATE TABLE staging.tblEquipmentComparisonTemp

	INSERT INTO staging.tblEquipmentComparisonTemp
	(SourceTable, EquipmentSKey, EquipmentKey, SiteKey, Id, EquipmentDescription, EquipmentTypeSKey, Make, Model, InUse, SerialNumber, StartDate, EndDate)
	SELECT 'Staging', 
	SKey, 
	EquipmentKey, 
	SiteKey, 
	TRIM(Id), 
	Description, 
	EquipmentTypeSKey, 
	CASE WHEN ([Make] IS NULL OR (LEN(TRIM([Make])) = 0)) THEN @dummyId ELSE [Make] END [Make],
    CASE WHEN ([Model] IS NULL OR (LEN(TRIM([Model])) = 0)) THEN @dummyId ELSE [Model] END [Model],
    ISNULL([InUse], 0) [InUse],
    CASE WHEN ([SerialNumber] IS NULL OR (LEN(TRIM([SerialNumber])) = 0)) THEN @shortDummyId ELSE [SerialNumber] END [SerialNumber],  
	StartDate, 
	EndDate
	FROM staging.tblEquipment
	WHERE IgnoreRecord <> 1
	AND ISNULL(IsRecordDeleted, 0) = 0

	INSERT INTO staging.tblEquipmentComparisonTemp
	(SourceTable, EquipmentSKey, EquipmentKey, SiteKey, Id, EquipmentDescription, EquipmentTypeSKey, Make, Model, InUse, SerialNumber, StartDate, EndDate)
	SELECT 'Dim', b.SKey, b.AKey, c.AKey, b.EquipmentId, b.Description, b.EquipmentTypeSKey, b.Make, b.Model, b.InUse, b.SerialNumber, b.StartDate, b.EndDate
	FROM (SELECT DISTINCT EquipmentKey FROM staging.tblEquipmentComparisonTemp) a
	INNER JOIN dbo.DimEquipment b
	ON b.AKey = a.EquipmentKey
	INNER JOIN dbo.DimSite c
	ON c.SKey = b.SiteSKey
	WHERE b.EndDate IS NULL

	UPDATE staging.tblEquipmentComparisonTemp
	SET RecordChecksum = CHECKSUM
	(
		Id, 
		EquipmentDescription, 
		EquipmentTypeSKey, 
		Make, 
		Model, 
		InUse, 
		SerialNumber
	)

	INSERT INTO staging.tblEntityChecksum
	(EntitySKey, EntityKey, StartDate, RecordChecksum)
	SELECT EquipmentSKey, EquipmentKey, StartDate, RecordChecksum
	FROM staging.tblEquipmentComparisonTemp
	WHERE SourceTable = 'Staging'
	ORDER BY EquipmentKey, StartDate
	
	UPDATE a
    SET a.RecordPreviousChecksum = b.RecordChecksum
    FROM staging.tblEntityChecksum a
    INNER JOIN staging.tblEntityChecksum b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = (a.RowIndex - 1)

	UPDATE a
	SET a.DimChecksum = c.RecordChecksum
	FROM staging.tblEntityChecksum a
	INNER JOIN
	(SELECT DISTINCT EntityKey FROM staging.tblEntityChecksum) b
	ON b.EntityKey = a.EntityKey
	INNER JOIN staging.tblEquipmentComparisonTemp c
	ON c.EquipmentKey = b.EntityKey
	WHERE c.SourceTable = 'Dim' 
	AND c.EndDate IS NULL
	
	DECLARE TableCursor CURSOR FOR 
	  SELECT EntitySKey, EntityKey, RecordChecksum, RecordPreviousChecksum, DimChecksum FROM staging.tblEntityChecksum
	  WHERE IgnoreRecord <> 1
	  ORDER BY EntityKey, StartDate
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum

	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF (@aKey <> @previousAKey)
		BEGIN
			SET @validChangesDetectedAgainstDim = 0
		END

		--Compare staging record with dim record
		IF (@validChangesDetectedAgainstDim = 0) 
		BEGIN						
			IF (@stagingChecksum = @dimChecksum)
			BEGIN
				UPDATE staging.tblEquipment
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
				
				SET @previousAKey = @aKey				
				SET @validChangesDetectedAgainstDim = 1
				CONTINUE
			END
		END
	
		--Compare staging record with previous staging record
		IF (@previousStagingChecksum IS NOT NULL)
		BEGIN		
			IF (@stagingChecksum = @previousStagingChecksum)
			BEGIN
				UPDATE staging.tblEquipment
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
			END
		END
		SET @previousAKey = @aKey				

		FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum
	END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 



	-- Personnel
	SET @validChangesDetectedAgainstDim = 0
	SET @previousAKey = ''
	TRUNCATE TABLE staging.tblEntityChecksum
	TRUNCATE TABLE staging.tblPersonnelComparisonTemp

	INSERT INTO staging.tblPersonnelComparisonTemp
	(SourceTable, PersonnelSKey, PersonnelKey, SiteKey, PersonID, FirstName, MiddleName, LastName, LockedOut, LockedOutReason, LockedOutDate, StartDate, EndDate)
	SELECT 'Staging', 
	SKey, 
	PersonnelKey, 
	SiteKey, 
	TRIM([PersonID]) [PersonID],
    [FirstName],
	[MiddleName],
    [LastName],
    ISNULL([LockedOut], 0) [LockedOut],
    [LockedOutReason],
    [LockedOutDate],
	StartDate, 
	EndDate
	FROM staging.tblPersonnel
	WHERE IgnoreRecord <> 1
	AND ISNULL(IsRecordDeleted, 0) = 0

	INSERT INTO staging.tblPersonnelComparisonTemp
	(SourceTable, PersonnelSKey, PersonnelKey, SiteKey, PersonID, FirstName, MiddleName, LastName, LockedOut, LockedOutReason, LockedOutDate, StartDate, EndDate)
	SELECT 'Dim', b.SKey, b.AKey, c.AKey, b.PersonID, b.FirstName, b.MiddleName, b.LastName, b.LockedOut, b.LockedOutReason, b.LockedOutDate, b.StartDate, b.EndDate
	FROM (SELECT DISTINCT PersonnelKey FROM staging.tblPersonnelComparisonTemp) a
	INNER JOIN dbo.DimPersonnel b
	ON b.AKey = a.PersonnelKey
	INNER JOIN dbo.DimSite c
	ON c.SKey = b.SiteSKey
	WHERE b.EndDate IS NULL	

	UPDATE staging.tblPersonnelComparisonTemp
	SET RecordChecksum = CHECKSUM
	(
		PersonID, 
		FirstName, 
		MiddleName,
		LastName, 		
		[LockedOut],
		[LockedOutReason],
		[LockedOutDate]
	)

	INSERT INTO staging.tblEntityChecksum
	(EntitySKey, EntityKey, StartDate, RecordChecksum)
	SELECT PersonnelSKey, PersonnelKey, StartDate, RecordChecksum
	FROM staging.tblPersonnelComparisonTemp
	WHERE SourceTable = 'Staging'
	ORDER BY PersonnelKey, StartDate
	
	UPDATE a
    SET a.RecordPreviousChecksum = b.RecordChecksum
    FROM staging.tblEntityChecksum a
    INNER JOIN staging.tblEntityChecksum b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = (a.RowIndex - 1)

	UPDATE a
	SET a.DimChecksum = c.RecordChecksum
	FROM staging.tblEntityChecksum a
	INNER JOIN
	(SELECT DISTINCT EntityKey FROM staging.tblEntityChecksum) b
	ON b.EntityKey = a.EntityKey
	INNER JOIN staging.tblPersonnelComparisonTemp c
	ON c.PersonnelKey = b.EntityKey
	WHERE c.SourceTable = 'Dim' 
	AND c.EndDate IS NULL
	
	DECLARE TableCursor CURSOR FOR 
	  SELECT EntitySKey, EntityKey, RecordChecksum, RecordPreviousChecksum, DimChecksum FROM staging.tblEntityChecksum
	  WHERE IgnoreRecord <> 1
	  ORDER BY EntityKey, StartDate
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum

	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF (@aKey <> @previousAKey)
		BEGIN
			SET @validChangesDetectedAgainstDim = 0
		END

		--Compare staging record with dim record
		IF (@validChangesDetectedAgainstDim = 0) 
		BEGIN						
			IF (@stagingChecksum = @dimChecksum)
			BEGIN
				UPDATE staging.tblPersonnel
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
				
				SET @previousAKey = @aKey				
				SET @validChangesDetectedAgainstDim = 1
				CONTINUE
			END
		END
	
		--Compare staging record with previous staging record
		IF (@previousStagingChecksum IS NOT NULL)
		BEGIN		
			IF (@stagingChecksum = @previousStagingChecksum)
			BEGIN
				UPDATE staging.tblPersonnel
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
			END
		END
		SET @previousAKey = @aKey				

		FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum
	END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 


	-- TransactionAlias
	SET @validChangesDetectedAgainstDim = 0
	SET @previousAKey = ''
	TRUNCATE TABLE staging.tblEntityChecksum
	TRUNCATE TABLE staging.tblTransactionAliasComparisonTemp

	INSERT INTO staging.tblTransactionAliasComparisonTemp
	(SourceTable, TransactionAliasSKey, TransactionAliasKey, SiteKey, AliasName, TransactionTypeSKey, StartDate, EndDate)
	SELECT 'Staging', 
	SKey, 
	TransactionAliasKey, 
	SiteKey, 
	TRIM([AliasName]) [AliasName],
    ISNULL([TransactionTypeSKey], 0) [TransactionTypeSKey],
	StartDate, 
	EndDate
	FROM staging.tblTransactionAliases
	WHERE IgnoreRecord <> 1
	AND ISNULL(IsRecordDeleted, 0) = 0

	INSERT INTO staging.tblTransactionAliasComparisonTemp
	(SourceTable, TransactionAliasSKey, TransactionAliasKey, SiteKey, AliasName, TransactionTypeSKey, StartDate, EndDate)
	SELECT 'Dim', b.SKey, b.AKey, c.AKey, b.AliasName, b.TransactionTypeSKey , b.StartDate, b.EndDate
	FROM (SELECT DISTINCT TransactionAliasKey FROM staging.tblTransactionAliasComparisonTemp) a
	INNER JOIN dbo.DimTransactionAlias b
	ON b.AKey = a.TransactionAliasKey
	INNER JOIN dbo.DimSite c
	ON c.SKey = b.SiteSKey
	WHERE b.EndDate IS NULL	

	UPDATE staging.tblTransactionAliasComparisonTemp
	SET RecordChecksum = CHECKSUM
	(
		AliasName, 
		TransactionTypeSKey 
	)

	INSERT INTO staging.tblEntityChecksum
	(EntitySKey, EntityKey, StartDate, RecordChecksum)
	SELECT TransactionAliasSKey, TransactionAliasKey, StartDate, RecordChecksum
	FROM staging.tblTransactionAliasComparisonTemp
	WHERE SourceTable = 'Staging'
	ORDER BY TransactionAliasKey, StartDate
	
	UPDATE a
    SET a.RecordPreviousChecksum = b.RecordChecksum
    FROM staging.tblEntityChecksum a
    INNER JOIN staging.tblEntityChecksum b
    ON b.EntityKey = a.EntityKey
    AND b.RowIndex = (a.RowIndex - 1)

	UPDATE a
	SET a.DimChecksum = c.RecordChecksum
	FROM staging.tblEntityChecksum a
	INNER JOIN
	(SELECT DISTINCT EntityKey FROM staging.tblEntityChecksum) b
	ON b.EntityKey = a.EntityKey
	INNER JOIN staging.tblTransactionAliasComparisonTemp c
	ON c.TransactionAliasKey = b.EntityKey
	WHERE c.SourceTable = 'Dim' 
	AND c.EndDate IS NULL
	
	DECLARE TableCursor CURSOR FOR 
	  SELECT EntitySKey, EntityKey, RecordChecksum, RecordPreviousChecksum, DimChecksum FROM staging.tblEntityChecksum
	  WHERE IgnoreRecord <> 1
	  ORDER BY EntityKey, StartDate
	OPEN TableCursor 
	FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum

	WHILE @@FETCH_STATUS = 0  
	BEGIN 
		IF (@aKey <> @previousAKey)
		BEGIN
			SET @validChangesDetectedAgainstDim = 0
		END

		--Compare staging record with dim record
		IF (@validChangesDetectedAgainstDim = 0) 
		BEGIN						
			IF (@stagingChecksum = @dimChecksum)
			BEGIN
				UPDATE staging.tblTransactionAliases
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
				
				SET @previousAKey = @aKey				
				SET @validChangesDetectedAgainstDim = 1
				CONTINUE
			END
		END
	
		--Compare staging record with previous staging record
		IF (@previousStagingChecksum IS NOT NULL)
		BEGIN		
			IF (@stagingChecksum = @previousStagingChecksum)
			BEGIN
				UPDATE staging.tblTransactionAliases
				SET IgnoreRecord = 1
				WHERE SKey = @sKey
			END
		END
		SET @previousAKey = @aKey				

		FETCH NEXT FROM TableCursor INTO @sKey, @aKey, @stagingChecksum, @previousStagingChecksum, @dimChecksum
	END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 


  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [staging].[usp_IgnoreChangesOnUnsupportedLevel1Fields]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END