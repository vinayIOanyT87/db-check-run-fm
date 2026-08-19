

/*
	DECLARE @EqTestList dbo.utt_EquipmentType
	INSERT INTO @EqTestList (EquipmentType) VALUES (1), (2)
	EXEC [dbo].[usp_GetEquipmentsByFuelCardAndProduct] NULL, NULL, @EqTestList, NULL, NULL, NULl
	EXEC [dbo].[usp_GetEquipmentsByFuelCardAndProduct] NULL, '70A85585-4EEB-4C5B-AA40-B5240214F9D3', @EqTestList, NULL, NULL, NULl
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentsByFuelCardAndProduct]
(
	@TargetSiteGuid uniqueidentifier, 
	@CompanyGuid uniqueidentifier, 
	@EquipmentTypeList dbo.utt_EquipmentType READONLY, 
	@FuelCardGuid uniqueidentifier, 
	@ProductGuid uniqueidentifier, 
	@SecondaryStorageFlag bit, 
	@HideHiddenEquipmentRecords BIT = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsByFuelCardAndProduct] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.004 / 2014-04-16 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Equipment records for a given target site/sitegroup, by FuelCard, Product, EquipmentType list, Company and SecondaryStorageFlag.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: Limit the results to Equipments that have a CompanyGuid value that correspond to the @CompanyGuid value
	-- 3. @EquipmentTypeList: Limit the results to Equipments that an EquipmentType in the Equipment Type list.
	-- 4. @FuelCardGuid: Limit the results to Equipments that have a FuelCardGuid value that correspond to the @FuelCardGuid value
	-- 5. @ProductGuid: Limit the results to Equipments that have a ProductGuid value that correspond to the @ProductGuid value
	-- 6. @SecondaryStorageFlag: Limit the results to Equipments that have a SecondaryStorageFlag value that correspond to the @SecondaryStorageFlag value
	-- 7. @HideHiddenEquipmentRecords: If true (1), only equipment records with a NULL hiddenDate will be returned
	-- 8. This stored procedure replaces the EquipmentClass.EnumerateInfoByTypesCompanyFuelCardProductAndSourceSQL inline SQL.
	-- 9. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @AllFilterGuid uniqueidentifier
		SET @AllFilterGuid = CAST('10000000-0000-0000-0000-000000000000' AS uniqueidentifier)
		DECLARE @equipmentTypeCount int

		SELECT @equipmentTypeCount = COUNT(*) FROM @EquipmentTypeList

		SELECT b.Id, b.Xref, b.SiteGuid, b.EquipmentGuid, b._MasterRecordGuid, b.FuelingType, b.ProductGuid, b.FuelCardGuid, b.EquipmentTypeGuid, b.SecondaryStorageFlag, b.ManagedEquipmentFlag,
		b.TruckCardNumber, b.VolumeUnitIndex, b.VolumeDecimalPlaces, b.DensityUnitIndex, b.DensityDecimalPlaces, b.TemperatureUnitIndex, b.TemperatureDecimalPlaces
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN tblEquipmentTypes d ON d.EquipmentTypeGuid = b.EquipmentTypeGuid  
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) cx ON cx.MasterRecordGuid = b.CompanyGuid
		LEFT JOIN tblCompanies c ON c.CompanyGuid = cx.CompanyGuid		
		LEFT JOIN tblEquipmentMaintenanceLog e ON e.EquipmentGuid = b._MasterRecordGuid  --Join on MasterRecordGuid since Record Versioning is not supported on tblEquipmentMaintenanceLog
		WHERE 
		(
			(@equipmentTypeCount = 0) 
			OR (d.LookupEquipmentTypeIndex IS NULL OR d.LookupEquipmentTypeIndex IN (SELECT EquipmentType FROM @EquipmentTypeList))
		)		
		AND
		(
			((b.CompanyGuid IS NULL) AND (@CompanyGuid = @emptyGuid))
			OR 
			(
				(@CompanyGuid <> @emptyGuid) AND
				(
					(b.CompanyGuid = @CompanyGuid)
					OR (b.CompanyGuid IN (SELECT CompanyGuid FROM map.tblCompanyAuthorizedCarrierToCompany WHERE AssignedToCompanyGuid = @CompanyGuid))
				)
			)
			OR @CompanyGuid IS NULL
		) 
		AND
		(
			(@FuelCardGuid IS NULL)
			OR ((@FuelCardGuid = @AllFilterGuid) AND (b.FuelCardGuid IS NOT NULL))
			OR ((@FuelCardGuid = @emptyGuid) AND (b.FuelCardGuid IS NULL))
			OR ((@FuelCardGuid <> @AllFilterGuid) AND (@FuelCardGuid <> @emptyGuid) AND (b.FuelCardGuid = @FuelCardGuid))
		)
		AND
		(
			(@ProductGuid IS NULL)
			OR ((@ProductGuid = @AllFilterGuid))
			OR ((@ProductGuid = @emptyGuid) AND (b.ProductGuid IS NULL))
			-- Return equipment records without an assigned product even if a product is specified in the input parameters
			OR ((@ProductGuid <> @AllFilterGuid) AND (@ProductGuid <> @emptyGuid) AND ((b.ProductGuid = @ProductGuid) OR (b.ProductGuid IS NULL)))
		)
		AND ((b.SecondaryStorageFlag = @SecondaryStorageFlag) OR (@SecondaryStorageFlag IS NULL))
		AND
		(
			e.ChangeDate IS NULL OR e.ChangeDate = 
			(
				SELECT MAX(ChangeDate) FROM tblEquipmentMaintenanceLog 
				WHERE tblEquipmentMaintenanceLog.EquipmentGuid = e.EquipmentGuid
			)
		)  
		AND ISNULL(InServiceFlag, 1) = 1 
		AND (@HideHiddenEquipmentRecords = 0 OR b.HiddenDate IS NULL)  
		ORDER BY b.ID

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByFuelCardAndProduct' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END