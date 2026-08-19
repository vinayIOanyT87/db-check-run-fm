


/*
	DECLARE @EqTestList dbo.utt_EquipmentType
	INSERT INTO @EqTestList (EquipmentType) VALUES (1), (2)
	EXEC [dbo].[usp_GetEquipmentsByFuelCardAndProductExt] NULL, NULL, @EqTestList, NULL, NULL, NULl
	EXEC [dbo].[usp_GetEquipmentsByFuelCardAndProductExt] NULL, '70A85585-4EEB-4C5B-AA40-B5240214F9D3', @EqTestList, NULL, NULL, NULl
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentsByFuelCardAndProductExt]
(
	@TargetSiteGuid uniqueidentifier, @CompanyGuid uniqueidentifier, @EquipmentTypeList dbo.utt_EquipmentType READONLY, @FuelCardGuid uniqueidentifier, @ProductGuid uniqueidentifier, @SecondaryStorageFlag bit
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetEquipmentsByFuelCardAndProductExt] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Equipment records for a given target site/sitegroup, by FuelCard, Product, EquipmentType list, Company and SecondaryStorageFlag.
	-- This is an extended version of dbo.usp_GetEquipmentsByFuelCardAndProduct. It simply joins on more tables and returns more fields than dbo.usp_GetEquipmentsByFuelCardAndProduct.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: Limit the results to Equipments that have a CompanyGuid value that correspond to the @CompanyGuid value
	-- 3. @EquipmentTypeList: Limit the results to Equipments that an EquipmentType in the Equipment Type list.
	-- 4. @FuelCardGuid: Limit the results to Equipments that have a FuelCardGuid value that correspond to the @FuelCardGuid value
	-- 5. @ProductGuid: Limit the results to Equipments that have a ProductGuid value that correspond to the @ProductGuid value
	-- 6. @SecondaryStorageFlag: Limit the results to Equipments that have a SecondaryStorageFlag value that correspond to the @SecondaryStorageFlag value
	-- 7. This stored procedure replaces the EquipmentClass.EnumerateByTypesCompanyFuelCardProductAndSourceSQL inline SQL.
	-- 8. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @AllFilterGuid uniqueidentifier
		SET @AllFilterGuid = CAST('10000000-0000-0000-0000-000000000000' AS uniqueidentifier)
		DECLARE @equipmentTypeCount int

		SELECT @equipmentTypeCount = COUNT(*) FROM @EquipmentTypeList

		SELECT b.*, c.ID AS CompanyID, c.Name, c.Address1, c.City, c.State, d.EqTypeName, d.LookupEquipmentTypeIndex, d.Capacity, 
		d.SafeFill, d.MultiCompartment, d.Isspt, d.LookupCompanyRoleIndex, e.EstReturnToServiceDate AS ReturnToServiceDate, e.MaintenanceReason AS StatusDescription, e.InServiceFlag, e.Memo AS MaintenanceNote, 
		e.ChangeDate, e.OperatorID as MaintenanceOperatorID, e.WorkOrder as MaintenanceWorkOrder, e.CreatedDate as MaintenanceCreatedDate, e.CreatedBy as MaintenenaceCreatedBy, 
		e.UpdatedDate as MaintenenaceUpdatedDate, e.UpdatedBy as MaintenanceUpdatedBy, 
		CASE WHEN ISNULL(LTRIM(RTRIM(g.Memo)), '') = '' THEN '' ELSE 'QC Tag Memo: ' + g.Memo + CHAR(0x0d) + CHAR(0x0d) END + 
		CASE WHEN ISNULL(LTRIM(RTRIM(f.Memo)), '') = '' THEN '' ELSE 'Test Result Memo: ' + f.Memo END as QCNote, 
		g.QualityCreatedDate, g.QualityCreatedBy, g.QualityUpdatedDate, 
		g.QualityUpdatedBy, g.QualityTagGuid, g.SiteGuid AS QualityTagSiteGuid, g.Name AS QualityTagName, g.Severity, g.Active, 
		(SELECT ProductID FROM tblProducts WHERE tblProducts.ProductGuid = b.ProductGuid) AS ProductID, 
		(SELECT ID FROM tblFuelCards fc WHERE fc.FuelCardGuid = b.FuelCardGuid) AS FuelCardID  
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN tblEquipmentTypes d ON d.EquipmentTypeGuid = b.EquipmentTypeGuid  
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) cx ON cx.MasterRecordGuid = b.CompanyGuid
		LEFT JOIN tblCompanies c ON c.CompanyGuid = cx.CompanyGuid		
		LEFT JOIN tblEquipmentMaintenanceLog e ON e.EquipmentGuid = b._MasterRecordGuid  --Join on MasterRecordGuid since Record Versioning is not supported on tblEquipmentMaintenanceLog
		LEFT JOIN tblTestSetEquipmentResults f ON f.EquipmentGuid = b._MasterRecordGuid	 --Join on MasterRecordGuid since Record Versioning is not supported on tblTestSetEquipmentResults
		LEFT JOIN 
		(
			SELECT GG.*, EquipmentGuid, HH.Memo, HH.CreatedDate as QualityCreatedDate, HH.CreatedBy as QualityCreatedBy, HH.UpdatedDate as QualityUpdatedDate, 
			HH.UpdatedBy as QualityUpdatedBy 
			FROM tblEquipmentQualityTagLog HH 
			LEFT JOIN tblQualityTags GG  
			ON GG.QualityTagGuid = HH.QualityTagGuid 
			WHERE RemovedDate IS NULL AND  HH.TaggedDate = 
			(
				SELECT MAX(TaggedDate) FROM tblEquipmentQualityTagLog 
				WHERE tblEquipmentQualityTagLog.EquipmentGuid = HH.EquipmentGuid 
			)
		) g 
		ON g.EquipmentGuid = b._MasterRecordGuid  --Join on MasterRecordGuid since Record Versioning is not supported on tblEquipmentQualityTagLog
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
		AND 
		(
			f.ResultTimeStamp IS NULL OR 
			f.ResultTimeStamp = 
			(
				SELECT MAX(ResultTimeStamp) FROM tblTestSetEquipmentResults 
				WHERE tblTestSetEquipmentResults.EquipmentGuid = f.EquipmentGuid
			)
		)
		AND ISNULL(InServiceFlag, 1) = 1 
		ORDER BY d.LookupEquipmentTypeIndex, b.ID

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByFuelCardAndProductExt' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END