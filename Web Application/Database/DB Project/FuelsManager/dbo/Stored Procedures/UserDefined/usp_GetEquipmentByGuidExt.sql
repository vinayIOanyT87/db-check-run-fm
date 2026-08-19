


/*
	EXEC [dbo].[usp_GetEquipmentByGuidExt] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', NULL
	EXEC [dbo].[usp_GetEquipmentByGuidExt] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [dbo].[usp_GetEquipmentByGuidExt] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'
	EXEC [dbo].[usp_GetEquipmentByGuidExt] 'C1078CE3-EC80-4CB7-81C3-0D4FA0D10215', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'
	EXEC [dbo].[usp_GetEquipmentByGuidExt] 'C1078CE3-EC80-4CB7-81C3-0D4FA0D10215', '6F38FF9E-D815-4E5B-B6B6-E6EAC0B1B76B'
	EXEC [dbo].[usp_GetEquipmentByGuidExt] 'b44649ad-877a-4a41-93b1-9b0e048be377', '00000000-0000-0000-0000-000000000001'
	EXEC [dbo].[usp_GetEquipmentByGuidExt] '1f9e9d4b-7d4d-4a83-bedb-d3d467da420e', '23a3f8fc-0d49-43bc-b20b-04ceda6a4346'	
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentByGuidExt]
(
	@EquipmentGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentByGuidExt] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve an Equipment record by Guid.
	-- Notes:
	-- 1. @EquipmentGuid: If @TargetSiteGuid is null, then @EquipmentGuid is the Guid of the Equipment to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the Equipment record to retrieve.
	-- 2. @TargetSiteGuid: IF TargetSiteGuid is not null, then it is used as the target owner site of the record version that needs to be retrieved.
	-- 3. This query can be used in two modes: 
	--		(a) When the exact GUID of the target Equipment record is known, in which case the @TargetSiteGuid can be left null.
	--		(b) When trying to verify if an equipment record has a record version (child or parent) against a specific site/sitegroup, in which case the @TargetSiteGuid must be provided.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblEquipment
		WHERE EquipmentGuid = @EquipmentGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @EquipmentGuid
		END
		
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
		FROM tblEquipment b
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
		WHERE b.EquipmentGuid = @targetRecordGuid
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
						+ 'Procedure Name: [dbo].usp_GetEquipmentByGuidExt' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END