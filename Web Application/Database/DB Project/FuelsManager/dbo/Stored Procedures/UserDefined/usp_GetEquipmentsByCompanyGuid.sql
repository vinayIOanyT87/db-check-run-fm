

/*
	EXEC [dbo].[usp_GetEquipmentsByCompanyGuid] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', NULL
	EXEC [dbo].[usp_GetEquipmentsByCompanyGuid] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', '012D8DD3-E6FA-4B78-A81A-C84F1C360558'
	EXEC [dbo].[usp_GetEquipmentsByCompanyGuid] '46426312-E408-4AF8-85FD-338B622B32BF', 'C1AE954A-B227-4AA0-A0B4-EC312503595A'
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentsByCompanyGuid]
(
	@TargetSiteGuid uniqueidentifier, @CompanyGuid uniqueidentifier, @HideHiddenEquipmentRecords BIT = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsByCompanyGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Equipment records for a given target site/sitegroup, by CompanyGuid.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: Limit the results to Equipments that have a CompanyGuid value that correspond to the @CompanyGuid value
	-- 3. @HideHiddenEquipmentRecords: If true (1), only equipment records with a NULL hiddenDate will be returned
	-- 4. This stored procedure replaces the EquipmentClass.EnumerateByCompanySQL inline SQL.
	-- 5. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 6. This query is both Equipment RecordVersioning-aware and Company RecordVersioning-aware.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		IF (@TargetSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid arguments. TargetSite cannot be null.',16,1); 
			RETURN;
		END

		--Get the Company MasterRecordGuid. Equipment references Company using the Company MasterRecordGuid.	
		DECLARE @companyMasterRecordGuid uniqueidentifier
		SELECT @companyMasterRecordGuid = _MasterRecordGuid FROM tblCompanies WHERE CompanyGuid = @CompanyGuid

		SELECT b.*, h.ID AS CompanyID, h.Name, h.Address1, h.City, h.State, d.EqTypeName, d.LookupEquipmentTypeIndex, d.Capacity, 
		d.SafeFill, d.MultiCompartment, d.Isspt, d.LookupCompanyRoleIndex, e.EstReturnToServiceDate AS ReturnToServiceDate, e.MaintenanceReason AS StatusDescription, e.InServiceFlag, e.Memo AS MaintenanceNote, 
		e.ChangeDate, e.OperatorID as MaintenanceOperatorID, e.WorkOrder as MaintenanceWorkOrder, e.CreatedDate as MaintenanceCreatedDate, e.CreatedBy as MaintenenaceCreatedBy, 
		e.UpdatedDate as MaintenenaceUpdatedDate, e.UpdatedBy as MaintenanceUpdatedBy, 
		CASE WHEN ISNULL(LTRIM(RTRIM(g.Memo)), '') = '' THEN '' ELSE 'QC Tag Memo: ' + g.Memo + CHAR(0x0d) + CHAR(0x0d) END + 
		CASE WHEN ISNULL(LTRIM(RTRIM(f.Memo)), '') = '' THEN '' ELSE 'Test Result Memo: ' + f.Memo END as QCNote, 
		g.QualityCreatedDate, g.QualityCreatedBy, g.QualityUpdatedDate, 
		g.QualityUpdatedBy, g.QualityTagGuid, g.SiteGuid AS QualityTagSiteGuid, g.Name AS QualityTagName, g.Severity, g.Active, 
		(SELECT ProductID FROM tblProducts WHERE tblProducts.ProductGuid = b.ProductGuid) AS ProductID, 
		(SELECT ID FROM tblFuelCards fc WHERE fc.FuelCardGuid = b.FuelCardGuid) AS FuelCardID,
		(SELECT DeviceID FROM tblAssetTrackingDevice atd WHERE atd.AssetTrackingDeviceGuid = b.AssetTrackingDeviceGuid) AS AssetTrackingDeviceID  
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) c ON c.MasterRecordGuid = b.CompanyGuid	--Use the Company record versions applicable to the target site
		LEFT JOIN tblEquipmentTypes d ON d.EquipmentTypeGuid = b.EquipmentTypeGuid  
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
		LEFT JOIN tblCompanies h ON h.CompanyGuid = c.CompanyGuid
		WHERE
		(
			((@CompanyGuid = @emptyGuid) AND (b.CompanyGuid IS NULL))
			OR (@CompanyGuid IS NULL)
			OR ((@CompanyGuid <> @emptyGuid) AND (b.CompanyGuid = @companyMasterRecordGuid))
		)
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
		AND (@HideHiddenEquipmentRecords = 0 OR b.HiddenDate IS NULL)  
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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByCompanyGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END