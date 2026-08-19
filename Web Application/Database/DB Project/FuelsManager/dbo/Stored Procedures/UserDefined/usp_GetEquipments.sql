


/*
	EXEC [dbo].[usp_GetEquipments] '00000000-0000-0000-0000-000000000001', NULL, NULL, NULL, 8, 5, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', NULL, NULL, NULL, 8, 5, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] 'B7BD440B-674F-46F6-977A-CEFC540B1A90', NULL, NULL, NULL, 8, 5, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', NULL, NULL, NULL, 8, 5, '{Unassigned}', '%4%', NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] NULL, NULL, NULL, NULL, NULL, NULL, NULL, '%HBEquipment1%', NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] '92e8d5fc-21fd-4560-be57-03a8bc0cf480', NULL, NULL, NULL, NULL, 5, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] '00000000-0000-0000-0000-000000000001', NULL, NULL, NULL, NULL, 5, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipments] '00000000-0000-0000-0000-000000000001', NULL, NULL, NULL, NULL, NULL, '{Unassigned}', NULL, NULL, NULL, NULL, NULL, NULL, NULL

*/

CREATE PROCEDURE [dbo].[usp_GetEquipments]
(
	@TargetSiteGuid uniqueidentifier, 
	@ManagedEquipmentOnly bit, 
	@SecondaryStorageOnly bit, 
	@EquipmentTypeGuid uniqueidentifier, 
	@EquipmentType int, 
	@EquipmentTypeToIgnore int, 
	@UnassignedStr nvarchar(30), 
	@IdFilter nvarchar(32), 
	@DefenceDateFilter nvarchar(30), 
	@VolumeFilter nvarchar(30), 
	@ProductIdFilter nvarchar(32), 
	@ApplyUnassignedCompanyIdFilter bit, 
	@CompanyIdFilter nvarchar(32), 
	@CompanyEquipmentIdFilter nvarchar(32),
	@HideHiddenEquipmentRecords BIT = 0,
	@Limit int = 1500
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipments] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve a set of Equipment records based on a set of filtering parameters.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @ManagedEquipmentOnly: 1: Limit results to Managed Equipments only
	-- 3. @SecondaryStorageOnly: 1: Limit results to secondary storage only
	-- 4. @EquipmentTypeGuid: Limit results to Equipments that belong to the Equipment Type specific by this Guid only
	-- 5. @EquipmentType: Limit results to Equipments that belong to the Equipment Type specific by this index only
	-- 6. @EquipmentTypeToIgnore: Exclude Equipments of this equipment type from the results
	-- 7. @UnassignedStr: Replace missing CompanyIds & ProductIds in the resultset with this string
	-- 8. @IdFilter: Limit the results to Equipments that have an ID value that contains the @filter string
	-- 9. @DefenceDateFilter: Limit the results according to the date filter
	-- 10. @ApplyVolumeFilter: Limit the results according to the volume filter
	-- 11. @ProductIdFilter: Limit the results according to the Product filter
	-- 12: @ApplyUnassignedCompanyIdFilter: Limit the results to equipments to those with unassigned CompanyIds
	-- 13. @ApplyCompanyIdFilter: Limit the results according to the company id filter		
	-- 14. @CompanyEquipmentIdFilter: Limit the results to the Company Equipment Id filter
	-- 15. @HideHiddenEquipmentRecords: If true (1), only equipment records with a NULL hiddenDate will be returned
	-- 16. This stored procedure replaces the EquipmentClass.EquipmentListEnumerateSql inline SQL and the EquipmentClass.EnumerateByCompanyGetIDTypeOnlySQL inline SQL.
	-- 17. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).

	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @CompartmentEquipmentTypeIndex int
		SELECT @CompartmentEquipmentTypeIndex = EquipmentTypeIndex FROM lookup.tblEquipmentType WHERE EquipmentTypeCode = 'COMPARTMENT_TYPE'


		SELECT TOP(@Limit) a.SiteGuid, a.EquipmentGuid, a.ID, a._MasterRecordGuid,
		dbo.udf_ConvertFromSIUnits(a.Capacity, a.VolumeUnitIndex, a.VolumeDecimalPlaces) AS Capacity,  
		a.CompanyEquipmentID,
		CASE WHEN ISNULL(LTRIM(RTRIM(b.ProductID)), '') = '' THEN @UnassignedStr ELSE b.ProductID END AS ProductID,
		a.QCDate,  e.EstReturnToServiceDate AS ReturnToServiceDate,  
		CASE WHEN ISNULL(a.LockedOut,0) = 0 THEN 'No' ELSE 'Yes' END AS LockedOut,  
		CASE WHEN ISNULL(LTRIM(RTRIM(c.ID)), '') = '' THEN @UnassignedStr ELSE c.ID END AS Company,  
		CASE WHEN ISNULL(e.InServiceFlag, '') = '' OR e.InServiceFlag <> 0 THEN 'Yes' ELSE 'No' END AS InServiceFlag,  
		(SELECT dbo.udf_GetUnitAbbrev(a.VolumeUnitIndex,0)) AS VolumeUnit,  
		(SELECT dbo.udf_ConvertFromSIUnits(a.Volume, a.VolumeUnitIndex, a.VolumeDecimalPlaces)) AS Volume,  
		a.HiddenDate,
		c.Name AS CompanyName,
		c.ID AS CompanyID,
		c.Address1 AS CompanyAddress,
		c.City AS CompanyCity,
		c.State AS CompanyState,
		a.SerialNumber,
		a.Description,
		a.Make,
		a.Model,
		a.Year,
		d.EqTypeName,
		d.LookupEquipmentTypeIndex,
		fc.ID AS FuelCardID,
		(SELECT COUNT(*) FROM map.tblMeterToEquipment m WHERE m.EquipmentGuid = a.EquipmentGuid) AS MeterCount		
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) x
		INNER JOIN tblEquipment a ON a.EquipmentGuid = x.EquipmentGuid
		LEFT JOIN tblProducts b  ON b.ProductGuid = a.ProductGuid  
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) cx ON cx.MasterRecordGuid = a.CompanyGuid
		LEFT JOIN tblCompanies c ON c.CompanyGuid = cx.CompanyGuid		
		LEFT JOIN tblEquipmentTypes d ON d.EquipmentTypeGuid = a.EquipmentTypeGuid  
		LEFT JOIN tblEquipmentMaintenanceLog e ON e.EquipmentGuid = a.EquipmentGuid  
		LEFT JOIN tblTestSetEquipmentResults f ON f.EquipmentGuid = a.EquipmentGuid  
		LEFT JOIN tblFuelCards fc ON a.FuelCardGuid = fc.FuelCardGuid
		LEFT JOIN 
		(
			SELECT gg.*, EquipmentGuid, hh.Memo, hh.CreatedDate as QualityCreatedDate, hh.CreatedBy as QualityCreatedBy, hh.UpdatedDate as QualityUpdatedDate, 
			hh.UpdatedBy as QualityUpdatedBy FROM tblEquipmentQualityTagLog hh 
			LEFT JOIN tblQualityTags gg  ON gg.QualityTagGuid = hh.QualityTagGuid 
			WHERE RemovedDate IS NULL AND  hh.TaggedDate = 
			(
				SELECT MAX(TaggedDate) FROM tblEquipmentQualityTagLog  
				WHERE tblEquipmentQualityTagLog.EquipmentGuid = hh.EquipmentGuid 
			)
		) g 
		ON g.EquipmentGuid = a.EquipmentGuid  
		WHERE 
		(						  
			((@EquipmentTypeToIgnore = @CompartmentEquipmentTypeIndex) AND (a.ParentEquipmentGuid IS NULL))  --The EquipmentTypeGuid is left as NULL for Compartments and therefore cannot be used when having to filter out Compartments.
			OR ((@EquipmentTypeToIgnore <> @CompartmentEquipmentTypeIndex) AND (d.LookupEquipmentTypeIndex <> @EquipmentTypeToIgnore))
			OR ((@EquipmentTypeToIgnore <> @CompartmentEquipmentTypeIndex) AND (d.LookupEquipmentTypeIndex IS NULL))
			OR (@EquipmentTypeToIgnore IS NULL)
		)		
		AND 
		(
			e.ChangeDate IS NULL OR e.ChangeDate = 
			(
				SELECT MAX(ChangeDate) FROM tblEquipmentMaintenanceLog WHERE tblEquipmentMaintenanceLog.EquipmentGuid = E.EquipmentGuid
			)
		) 
		AND 
		(
			f.ResultTimeStamp IS NULL OR f.ResultTimeStamp = 
			(
				SELECT MAX(ResultTimeStamp) FROM tblTestSetEquipmentResults 
				WHERE tblTestSetEquipmentResults.EquipmentGuid = f.EquipmentGuid
			)
		)  
		AND ((a.ManagedEquipmentFlag = 1 AND @ManagedEquipmentOnly = 1) OR (ISNULL(@ManagedEquipmentOnly, 0) <> 1))
		AND ((a.SecondaryStorageFlag = 1 AND @SecondaryStorageOnly = 1) OR (ISNULL(@SecondaryStorageOnly, 0) <> 1))
		AND 
		(
			((a.EquipmentTypeGuid IS NULL) AND (@EquipmentTypeGuid = @emptyGuid))
			OR 
			((a.EquipmentTypeGuid = @EquipmentTypeGuid) AND (@EquipmentTypeGuid IS NOT NULL))
			OR 
			(@EquipmentTypeGuid IS NULL)
		)
		AND ((d.LookupEquipmentTypeIndex = @EquipmentType) OR (@EquipmentType IS NULL))
		AND 
		( 
			(@IdFilter IS NULL)
			OR
			(a.ID LIKE @IdFilter)
			OR (a.SerialNumber LIKE @IdFilter)
			OR (a.Description LIKE @IdFilter)
			OR (a.Make LIKE @IdFilter)
			OR (a.Model LIKE @IdFilter)
			OR (a.Year LIKE @IdFilter)
			OR (d.EqTypeName LIKE @IdFilter)
			OR (fc.ID LIKE @IdFilter)
			OR
			(
				(@DefenceDateFilter IS NOT NULL)
				AND
				(					
					(DATEDIFF(d, a.QCDate, @DefenceDateFilter) = -1)
					OR 
					(DATEDIFF(d, e.EstReturnToServiceDate, @DefenceDateFilter) = -1)
				)
			)
			OR
			( 
				(@VolumeFilter IS NOT NULL) AND (a.Volume = dbo.udf_ConvertToSIUnits(@VolumeFilter , a.VolumeUnitIndex))
			)
			OR b.ProductID LIKE @ProductIdFilter
			OR
			( 
				(@ApplyUnassignedCompanyIdFilter = 1) AND (C.ID = '')
			)
			OR
			(
				(@CompanyIdFilter IS NOT NULL) AND	(C.ID LIKE @CompanyIdFilter)
			)
			OR
			(
				(@CompanyEquipmentIdFilter IS NOT NULL) AND (a.CompanyEquipmentID LIKE @CompanyEquipmentIdFilter)
			)
		)
		AND (@HideHiddenEquipmentRecords = 0 OR a.HiddenDate IS NULL) 
		ORDER BY a.ID

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
						+ 'Procedure Name: [dbo].usp_GetEquipments' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     

