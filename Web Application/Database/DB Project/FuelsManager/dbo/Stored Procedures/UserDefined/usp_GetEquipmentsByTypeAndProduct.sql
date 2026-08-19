


/*
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', NULL, '291C57D1-06CF-49B3-A732-47F37A1267A0'
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] '46426312-E408-4AF8-85FD-338B622B32BF', 1, Null
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] '46426312-E408-4AF8-85FD-338B622B32BF', 1, Null, 0
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] 'B7BD440B-674F-46F6-977A-CEFC540B1A90', 1, Null, 1
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] '46426312-E408-4AF8-85FD-338B622B32BF', 1, Null, 0, 0
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] '46426312-E408-4AF8-85FD-338B622B32BF', 1, Null, 0, 1
	EXEC [dbo].[usp_GetEquipmentsByTypeAndProduct] '00000000-0000-0000-0000-000000000001', NULL, Null, 0, 1
	

*/



CREATE PROCEDURE [dbo].[usp_GetEquipmentsByTypeAndProduct]
(
	@TargetSiteGuid uniqueidentifier, @EquipmentType int, @ProductGuid uniqueidentifier, @ExcludeNonEditableCompanyGuid bit = NULL, @ExcludeNonEditableFuelCardGuid bit = NULL, @HideHiddenEquipmentRecords BIT = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsByTypeAndProduct] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Equipment records for a given target site/sitegroup, by EquipmentType and Product.
	-- Notes:
	-- 1. @TargetSiteGuid: Guid of the site/sitegroup for which to limit the search.
	-- 2. @EquipmentType: Index of the EquipmentType for which to limit the search.
	-- 3. @ProductGuid: Guid of the Product for which to limit the search. 
	-- 4. @ExcludeNonEditableCompanyGuid: 
	--		1: Exclude those Equipment record versions for which the CompanyGuid field cannot be edited, according to the applicable FLC configuration.
	--		0 or Null: Ignore the FLC configuration check on the CompanyGuid field, and retrieve the Equipment record versions irrespective of whether their CompanyGuid field can be edited or not.
	--		Note: Typically the query will be executed for both @ExcludeNonEditableCompanyGuid and @ExcludeNonEditableFuelCardGuid being Null/0, or one of them bring set to 1, but not both of them being set to 1.
	-- 5. @ExcludeNonEditableFuelCardGuid: 
	--		1: Exclude those Equipment record versions for which the FuelCardGuid field cannot be edited, according to the applicable FLC configuration.
	--		0 or Null: Ignore the FLC configuration check on the FuelCardGuid field, and retrieve the Equipment record versions irrespective of whether their FuelCardGuid field can be edited or not.
	--		Note: Typically the query will be executed for both @ExcludeNonEditableCompanyGuid and @ExcludeNonEditableFuelCardGuid being Null/0, or one of them bring set to 1, but not both of them being set to 1.
	-- 6. @HideHiddenEquipmentRecords: If true (1), only equipment records with a NULL hiddenDate will be returned
	-- 7. This stored procedure replaces the EquipmentClass.EnumerateByTypeAndProductSQL inline SQL.
	-- 8. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		DECLARE @callingRefGuid uniqueidentifier

		DECLARE @tblRecordVersions TABLE
		(
			EquipmentGuid uniqueidentifier NOT NULL,
			MasterRecordGuid uniqueidentifier NOT NULL,
			AssignedFromSiteGuid uniqueidentifier NULL,
			AssignedToSiteGuid uniqueidentifier NULL,
			IsEditableCompanyGuid bit NULL,
			IsEditableFuelCardGuid bit NULL
		)

		INSERT INTO @tblRecordVersions
		(EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid)
		SELECT EquipmentGuid, MasterRecordGuid, AssignedFromSiteGuid, AssignedToSiteGuid FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		
		IF (@ExcludeNonEditableCompanyGuid = 1)
		BEGIN		
			UPDATE @tblRecordVersions
			SET IsEditableCompanyGuid = 1
			WHERE EquipmentGuid = MasterRecordGuid

			SET @callingRefGuid = NEWID()
			EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Equipment', NULL, 'EquipmentTypeGuid', NULL, 'Company', NULL, @callingRefGuid

			--Set the IsEditableCompanyGuid for each record according to the FLC configuration on the Sitegroup from which the child record version was created/assigned from.
			UPDATE a
			SET a.IsEditableCompanyGuid = 1
			FROM @tblRecordVersions a
 			INNER JOIN tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN erv.tblTempFieldLevelConfigMatrix c
			ON c.SiteGroupGuid = a.AssignedFromSiteGuid
			AND ISNULL(c.FilterValueGuid, @emptyGuid) = ISNULL(b.EquipmentTypeGuid, @emptyGuid)
			WHERE c._CallingReferenceGuid = @callingRefGuid
			AND c.FilterFieldName = 'EquipmentTypeGuid'
			AND c.ForwardControlMode = 'VersionSpecific'
			AND c.FilterValueGuid = b.EquipmentTypeGuid
			AND a.EquipmentGuid <> a.MasterRecordGuid

			UPDATE @tblRecordVersions
			SET IsEditableCompanyGuid = 0
			WHERE IsEditableCompanyGuid IS NULL

			DELETE erv.tblTempFieldLevelConfigMatrix
			WHERE _CallingReferenceGuid = @callingRefGuid
		END
		IF (@ExcludeNonEditableFuelCardGuid = 1)
		BEGIN
			--If the query is to be used to return the Equipment records for the purpose of updating the FuelCardGuid field of those same 
			--Equipment records (this is different from returning Equipment records for the purpose of updating another entity record, 
			--e.g. Transaction), then need to restrict the list to only the actual child record versions of the TargetSite/Sitegroup, 
			--to prevent a lower site/sitegroup from updating the Equipment records of a parent sitegroup. A record version should only 
			--be directly updated (i.e. not through record version change propagation) from the site/sitegroup that owns it.			
			--Note: The query below that looks for a VersionSpecific FCM for the FuelCardGuid field will also achieve the same filtering,
			--but deleting those records explicitly below just helps highlight the need for the filtering.
			DELETE a FROM @tblRecordVersions a
			INNER JOIN tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			WHERE b.SiteGuid <> @TargetSiteGuid
			
			UPDATE @tblRecordVersions
			SET IsEditableFuelCardGuid = 1
			WHERE EquipmentGuid = MasterRecordGuid
   			
			SET @callingRefGuid = NEWID()
			EXEC [erv].[usp_GetFieldLevelConfigMatrix] 'Equipment', NULL, 'EquipmentTypeGuid', NULL, 'Fuel Card', NULL, @callingRefGuid

			--Set the IsEditableFuelCardGuid for each record according to the FLC configuration on the Sitegroup from which the child record version was created/assigned from.
			UPDATE a
			SET a.IsEditableFuelCardGuid = 1
			FROM @tblRecordVersions a
 			INNER JOIN tblEquipment b
			ON b.EquipmentGuid = a.EquipmentGuid
			INNER JOIN erv.tblTempFieldLevelConfigMatrix c
			ON c.SiteGroupGuid = a.AssignedFromSiteGuid
			AND ISNULL(c.FilterValueGuid, @emptyGuid) = ISNULL(b.EquipmentTypeGuid, @emptyGuid)
			WHERE c._CallingReferenceGuid = @callingRefGuid
			AND c.FilterFieldName = 'EquipmentTypeGuid'
			AND c.ForwardControlMode = 'VersionSpecific'
			AND c.FilterValueGuid = b.EquipmentTypeGuid
			AND a.EquipmentGuid <> a.MasterRecordGuid

			UPDATE @tblRecordVersions
			SET IsEditableFuelCardGuid = 0
			WHERE IsEditableFuelCardGuid IS NULL

			DELETE erv.tblTempFieldLevelConfigMatrix
			WHERE _CallingReferenceGuid = @callingRefGuid
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
		FROM @tblRecordVersions a
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
		WHERE 
		(
			(@ExcludeNonEditableCompanyGuid IS NULL) OR (@ExcludeNonEditableCompanyGuid = 0)
			OR ((@ExcludeNonEditableCompanyGuid = 1) AND (a.IsEditableCompanyGuid = 1))
		)
		AND
		(
			(@ExcludeNonEditableFuelCardGuid IS NULL) OR (@ExcludeNonEditableFuelCardGuid = 0)
			OR ((@ExcludeNonEditableFuelCardGuid = 1) AND (a.IsEditableFuelCardGuid = 1))
		)
		AND
		(
			((@EquipmentType IS NULL) AND (b.EquipmentTypeGuid IS NOT NULL))
			OR
			((@EquipmentType IS NOT NULL) AND (d.LookupEquipmentTypeIndex = @EquipmentType))
		)
		AND
		(
			-- Return equipment records without an assigned product even if a product is specified in the input parameters
			(@ProductGuid IS NULL) OR (b.ProductGuid = @ProductGuid OR b.ProductGuid IS NULL)  
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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByTypeAndProduct' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END