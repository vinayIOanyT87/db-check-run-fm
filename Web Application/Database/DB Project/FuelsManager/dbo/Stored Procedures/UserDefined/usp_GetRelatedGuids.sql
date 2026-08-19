
------------------------------------------------------------------------------------------------------
-- Stored procedure: [dbo].[usp_GetRelatedGuids] 
-- Author: Richard R. Panachida
-- Version/Date: 1.0.000 / 2013-07-25
-- Purpose: Is to retreive all related GUIDs for a collection of transactions.
-- Notes:
-- 1. @RelatedGuidParmTable: Table containing the Entity ID and Entity Type that will be used to retrieve the related GUID.
-- 2. @Section: Identifies the whether the IDs are for the transaction header, line item, or sub-line item.
------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[usp_GetRelatedGuids]
(
	@RelatedGuidParmTable dbo.utt_RelatedGuidParameters READONLY, 
	@Section INT
)
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SectionTypeHeader INT
	SET @SectionTypeHeader = 0

	DECLARE @SectionTypeLineItem INT
	SET @SectionTypeLineItem = 1

	DECLARE @SectionTypeSubLineItem INT
	SET @SectionTypeSubLineItem = 2

	DECLARE @SectionTypeTransportLineItem INT
	SET @SectionTypeTransportLineItem = 3

	-- The Result GUIDs table will contain the results of retrieving GUIDs for each of the
	-- entities in the Related GUID Parameters table.
	CREATE TABLE #ResultGuids
	(
		Section int NOT NULL,
		SiteGuid uniqueidentifier NOT NULL,
		[TransId] nvarchar(100) NOT NULL,
		EntityId nvarchar(100) NULL,
		EntityType nvarchar(100) NULL,
		EntityGuid uniqueidentifier NULL,
		[Identifier] nvarchar(100) NOT NULL
	)

	-- Process transaction header section.
	IF (@Section = @SectionTypeHeader)
	BEGIN
		-- Retrieve the Transaction GUIDs based on the TransID
		INSERT INTO #ResultGuids
			SELECT TOP(1) @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, t.TransactionGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblTransactions t ON p.[TransId] = t.TransID
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'TransID'

		-- Retrieve the Transaction Alias GUIDs based on the Alias Name (a.k.a. ID)
		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityTransactionAliasToSite a INNER JOIN 
					tblTransactionAliases b ON a.TransactionAliasGuid = b.TransactionAliasGuid) ON b.AliasName = p.EntityId
			WHERE p.Section = @SectionTypeHeader
				  AND p.EntityType = 'AliasName'
				  AND a.SiteGuid = p.SiteGuid 

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ShipToID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'SupplierID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ShipperID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'OwnerID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ManagerID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'CarrierID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'BillToID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ToShipToID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ToOwnerID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ToManagerID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityCompanyToSite a INNER JOIN 
					tblCompanies b on a.CompanyGuid = b.CompanyGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'ToBillToID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'DestinationRegistrationID1'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'DestinationRegistrationID2'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'DestinationRegistrationID3'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'SourceRegistrationID1'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'SourceRegistrationID2'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'SourceRegistrationID3'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityPersonnelToSite a INNER JOIN 
					tblPersonnel b on a.PersonnelGuid = b.PersonnelGuid) ON b.PersonID = p.EntityId
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'OperatorID'
					AND a.SiteGuid = p.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, e.FuelCardGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(tblFuelCards e INNER JOIN map.tblEntityFuelCardToSite m
					ON m.FuelCardGuid = e.FuelCardGuid) ON e.[ID] = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'FuelCardID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, e.IATAGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(tblIATA e INNER JOIN map.tblEntityIATACodeToSite m
					ON m.IATAGuid = e.IATAGuid) ON e.IATAID = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'FinalStationID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, e.IATAGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(tblIATA e INNER JOIN map.tblEntityIATACodeToSite m
					ON m.IATAGuid = e.IATAGuid) ON e.IATAID = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'PreviousStationID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, e.IATAGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(tblIATA e INNER JOIN map.tblEntityIATACodeToSite m
					ON m.IATAGuid = e.IATAGuid) ON e.IATAID = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'NextStationID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, e.IATAGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(tblIATA e INNER JOIN map.tblEntityIATACodeToSite m
					ON m.IATAGuid = e.IATAGuid) ON e.IATAID = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeHeader
					AND p.EntityType = 'OriginStationID'

		-- Return results and drop the temp table.
		SELECT * FROM #ResultGuids
		DROP TABLE #ResultGuids

	END -- End header section

	-- Process transaction line item section.
	IF (@Section = @SectionTypeLineItem)
	BEGIN
		INSERT INTO #ResultGuids
			SELECT TOP(1) @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, l.TransactionLineItemGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid) ON p.TransId = t.TransID
			WHERE p.Section = @SectionTypeLineItem
					AND t.SiteGuid = p.SiteGuid
					AND p.EntityType = 'TransactionLineItemID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, a.AdditiveProfileGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(dbo.tblAdditiveProfiles a INNER JOIN map.tblEntityAdditiveProfileToSite m
					ON m.AdditiveProfileGuid = a.AdditiveProfileGuid) ON a.[ID] = p.EntityId AND m.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'AdditiveProfileID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityProductToSite a INNER JOIN 
					tblProducts b on a.ProductGuid = b.ProductGuid) ON b.ProductID = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'Product'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityProductToSite a INNER JOIN 
					tblProducts b on a.ProductGuid = b.ProductGuid) ON b.ProductID = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'ToProduct'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'DestinationRegistrationID1'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid) ON b.[ID] = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'SourceRegistrationID1'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid INNER JOIN 
					dbo.tblEquipmentTypes c ON c.EquipmentTypeGuid = b.EquipmentTypeGuid) ON b.[ID] = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'DestinationCompartmentID'
					AND c.LookupEquipmentTypeIndex = 5 --CompartmentType

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityEquipmentToSite a INNER JOIN 
					tblEquipment b ON a.EquipmentGuid = b.EquipmentGuid INNER JOIN 
					dbo.tblEquipmentTypes c ON c.EquipmentTypeGuid = b.EquipmentTypeGuid) ON b.[ID] = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'SourceCompartmentID'
					AND c.LookupEquipmentTypeIndex = 5 --CompartmentType

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityPersonnelToSite a INNER JOIN 
					tblPersonnel b on a.PersonnelGuid = b.PersonnelGuid) ON b.PersonID = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'OperatorID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, t.TankGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblTanks t ON t.TankID = p.EntityId
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'StorageLocationID'
					AND p.SiteGuid = t.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, t.TankGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblTanks t ON t.TankID = p.EntityId
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'ToStorageLocationID'
					AND p.SiteGuid = t.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, s.StationGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblStations s ON s.[ID] = p.EntityId
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'LoadingLocationID'
					AND p.SiteGuid = s.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, m.MeterGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblMeter m ON m.MeterID = p.EntityId
			WHERE p.Section = @SectionTypeLineItem
					AND p.EntityType = 'MeterID'
					AND p.SiteGuid = m.SiteGuid

		-- Return results and drop the temp table.
		SELECT * FROM #ResultGuids
		DROP TABLE #ResultGuids

	END -- End line item section

	-- Process transaction sub-line item section.
	IF (@Section = @SectionTypeSubLineItem)
	BEGIN
		INSERT INTO #ResultGuids
			SELECT TOP(1) @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, s.TransactionSubLineItemGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					((dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid) LEFT OUTER JOIN
					  dbo.tblTransactionSubLineItems s ON l.TransactionLineItemGuid = s.TransactionLineItemGuid) 
					ON p.TransId = t.TransID
			WHERE p.Section = @SectionTypeSubLineItem
					AND t.SiteGuid = p.SiteGuid
					AND p.EntityType = 'TransactionSubLineItemID'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, b._MasterRecordGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(map.tblEntityProductToSite a INNER JOIN 
					tblProducts b on a.ProductGuid = b.ProductGuid) ON b.ProductID = p.EntityId AND a.SiteGuid = p.SiteGuid
			WHERE p.Section = @SectionTypeSubLineItem
					AND p.EntityType = 'Product'

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, t.TankGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblTanks t ON t.TankID = p.EntityId
			WHERE p.Section = @SectionTypeSubLineItem
					AND p.EntityType = 'StorageLocationID'
					AND p.SiteGuid = t.SiteGuid

		INSERT INTO #ResultGuids
			SELECT @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, m.MeterGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN dbo.tblMeter m ON m.MeterID = p.EntityId
			WHERE p.Section = @SectionTypeSubLineItem
					AND p.EntityType = 'MeterID'
					AND p.SiteGuid = m.SiteGuid

		-- Return results and drop the temp table.
		SELECT * FROM #ResultGuids
		DROP TABLE #ResultGuids

	END -- End sub-line item section

	-- Process transport line item section.
	IF (@Section = @SectionTypeTransportLineItem)
	BEGIN
		INSERT INTO #ResultGuids
			SELECT TOP(1) @Section, p.SiteGuid, p.[TransId], p.EntityId, p.EntityType, tp.TransactionTransportLineItemGuid AS EntityGuid, p.Identifier
			FROM @RelatedGuidParmTable p LEFT OUTER JOIN 
					(dbo.tblTransactions t LEFT OUTER JOIN dbo.tblTransactionTransportLineItems tp ON t.TransactionGuid = tp.TransactionGuid)
					ON p.TransId = t.TransID
			WHERE p.Section = @SectionTypeSubLineItem
					AND t.SiteGuid = p.SiteGuid
					AND p.EntityType = 'TransportLineItemID'

		-- Return results and drop the temp table.
		SELECT * FROM #ResultGuids
		DROP TABLE #ResultGuids

	END -- End transport line item section
END -- End create procedure
