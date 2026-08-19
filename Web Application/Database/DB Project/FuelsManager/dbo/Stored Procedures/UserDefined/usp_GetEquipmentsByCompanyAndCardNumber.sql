

/*
	EXEC [dbo].[usp_GetEquipmentsByCompanyAndCardNumber] NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipmentsByCompanyAndCardNumber] '00000000-0000-0000-0000-000000000001', 'E599EFC5-9B85-4DDA-BC17-DC6084E6D176', ''
	EXEC [dbo].[usp_GetEquipmentsByCompanyAndCardNumber] 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421', '012D8DD3-E6FA-4B78-A81A-C84F1C360558', ''
	
*/

CREATE PROCEDURE [dbo].[usp_GetEquipmentsByCompanyAndCardNumber]
(
	@TargetSiteGuid uniqueidentifier, @CompanyGuid uniqueidentifier, @TruckCardNumber nvarchar(32)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsByCompanyAndCardNumber] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Equipment records by Company and Truck Card Number.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit search to Equipments that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: Guid of the Company for which the Equipment record is to be located.
	-- 3. @TruckCardNumber: Truck Card Number for which the Equipment record is to be located.
	-- 4. This stored procedure replaces the EquipmentClass.SelectByCardNumberAndEquipmentIDSQL inline SQL.
	-- 5. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.ID AS CompanyID, c.Name, c.Address1, c.City, c.State, d.EqTypeName, d.LookupEquipmentTypeIndex, d.Capacity, 
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
		LEFT JOIN [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid) cx ON cx.MasterRecordGuid = b.CompanyGuid
		LEFT JOIN tblCompanies c ON c.CompanyGuid = cx.CompanyGuid		
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
		WHERE b.CompanyGuid = @CompanyGuid
		AND b.TruckCardNumber = @TruckCardNumber
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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByCompanyAndCardNumber' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END