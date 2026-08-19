/*
	DROP PROCEDURE [erv].[usp_PropagateCompanyRevisionByEntityRecordChange]

	EXEC [erv].[usp_PropagateCompanyRevisionByEntityRecordChange] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateCompanyRevisionByEntityRecordChange] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_PropagateCompanyRevisionByEntityRecordChange] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'
	EXEC [erv].[usp_PropagateCompanyRevisionByEntityRecordChange] '64A800A7-67FC-4950-A22D-15863AD475FA'

*/

CREATE PROCEDURE [erv].[usp_PropagateCompanyRevisionByEntityRecordChange]
(
	@SourceCompanyGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateCompanyRevisionByEntityRecordChange] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate the current revision of a given Company entity record down the site hierarchy, according to the rules established by the Field Level Control configurations.
	-- This Stored Procedure is to be used to propagate the effect of an entity record change down to all its children record versions.
	-- Notes:
	-- 1. @SourceCompanyGuid: Guid of the Company record that needs to be propagated down the site hierarchy. This should correspond to the exact record version that has been 
	--    changed (and not the parent record of the entity record).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @EmptyGuid uniqueidentifier
		SET @EmptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Company'

		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		SELECT @ownerSiteGuid = SiteGuid, @masterRecordGuid = _MasterRecordGuid FROM tblCompanies
		WHERE CompanyGuid = @SourceCompanyGuid

		IF ((@masterRecordGuid IS NULL) OR (@ownerSiteGuid IS NULL))
		BEGIN
			RAISERROR('Cannot locate the source record for data propagation.',16,1); 
			RETURN;
		END

		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		
		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @SourceCompanyGuid)
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			RAISERROR('Cannot locate the segment information for the selected entity record.',16,1); 
			RETURN;
		END

		DECLARE @assignedFromSiteGroupGuid uniqueidentifier
		IF (@SourceCompanyGuid = @masterRecordGuid)
		BEGIN
			SET @assignedFromSiteGroupGuid = @ownerSiteGuid
		END
		ELSE
		BEGIN
			SET @assignedFromSiteGroupGuid = (SELECT [erv].[udf_GetEntityAssignedFromSite] (@EntityTypeId, @SourceCompanyGuid, Null))
			IF (@assignedFromSiteGroupGuid IS NULL)
			BEGIN
				RAISERROR('Cannot locate the assignment information for the selected child record version.',16,1); 
				RETURN;
			END
		END				

		-- Retrieve the Entity To Site hierarchy below the owner sitegroup of the entity record whose changes are to be propagated
		-- This corresponds to all the child record versions who derives, directly or indirectly, from the given record version.
		DECLARE @tblEntityToSiteHierarchy TABLE
		(
			SiteGuid uniqueidentifier
			, SiteId nvarchar(30)
			, HierarchyLevel int
			, Processed bit
		);

		INSERT INTO @tblEntityToSiteHierarchy
		(SiteGuid, SiteId, HierarchyLevel, Processed)
		SELECT SiteGuid, SiteId, HierarchyLevel, 0
		FROM [erv].[udf_GetCompanyToSiteHierarchyByRecordVersionGuid](@SourceCompanyGuid)
		WHERE HierarchyLevel > 0
		ORDER BY HierarchyLevel, SiteGuid


		--Retrieve the VersionSpecific fields for the owner sitegroup of the entity record whose changes need to be propagated
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, 'VersionSpecific', @callingRef2Guid 

		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName)
		SELECT TargetField, IsExternalAttribute, InternalFieldName FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid


		--Build a table that has one flag column for each column of the tblCompanies table, and set the flag according to whether the field is VersionSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempCompanyRecordVersioningFlag
		(CompanyGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.CompanyGuid, a.SiteGuid, @callingRef1Guid FROM tblCompanies a
		INNER JOIN @tblEntityToSiteHierarchy b
		ON b.SiteGuid = a.SiteGuid
		WHERE a._MasterRecordGuid = @masterRecordGuid

		DECLARE @tblTargetChildRecordVersions TABLE
		(
			CompanyGuid uniqueidentifier,
			SiteGuid uniqueidentifier,
			HierarchyLevel int,
			Processed bit
		)

		INSERT INTO @tblTargetChildRecordVersions
		(CompanyGuid, SiteGuid, HierarchyLevel, Processed)
		SELECT a.CompanyGuid, b.SiteGuid, c.HierarchyLevel, 0 FROM erv.tblTempCompanyRecordVersioningFlag a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		INNER JOIN @tblEntityToSiteHierarchy c
		ON c.SiteGuid = b.SiteGuid
		WHERE b._MasterRecordGuid = @masterRecordGuid
		AND a._CallingReferenceGuid = @callingRef1Guid


		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  	

		
		-- Process [UserGroups] External Field
		-- UserGroup is both an External Attribute of Company (i.e. Company-To-UserGroup mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-UserGroup mappings are also maintained as part of the UserGroup entity, i.e. outside of the Company entity)
		-- The Company.UserGroup mapping replication is performed using the MasterRecordGuid (since the mapping table has a SiteGuid field), from the current sitegroup down the entity-site mapping hierarchy if either FLC is not set to Version Specific on the Company.UserGroups field or Company Record Versioning is turned OFF.
		-- This is different from regular Record Versioning propagation that is only performed on child record versions, i.e. with Record Versioning turned ON.
		-- For this reason, the Company.UserGroup mappings replication is located outside of the regular Company fields propagation located further down, i.e. before the check on child record versions existence.
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserGroups') = 0)
		BEGIN
			-- Delete the Company-UserGroups mappings that are not supported anymore by the Company at its owner Site.
			DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN @tblEntityToSiteHierarchy b
			ON b.SiteGuid = a.SiteGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND a.SiteGuid <> @ownerSiteGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] d				
				WHERE d.CompanyGuid = @SourceCompanyGuid
				AND d.GroupGuid = a.GroupGuid
				AND d.SiteGuid = @ownerSiteGuid
			)		
													
			--Make sure the Company-UserGroup-Site mappings at the owner sitegroup are replicated down the entity-to-site hierarchy
			INSERT INTO [map].[tblCompanyCompanyToUserGroup]
			(CompanyGuid, GroupGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT a.CompanyGuid, a.GroupGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyCompanyToUserGroup] a
			CROSS JOIN @tblEntityToSiteHierarchy b
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyCompanyToUserGroup] d
				WHERE d.CompanyGuid = a.CompanyGuid
				AND d.GroupGuid = a.GroupGuid
				AND d.SiteGuid = b.SiteGuid
			)
		END
		
		/*	If there are child record versions to update.	*/
		IF (EXISTS (SELECT * FROM erv.tblTempCompanyRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRef1Guid))
		BEGIN						

			EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @ownerSiteGuid, @callingRef2Guid, @callingRef1Guid

			DELETE erv.tblTempRecordVersioningField
			WHERE _CallingReferenceGuid = @callingRef2Guid
		

			-- Update all the internal non-VersionSpecific fields for all applicable child record versions
			UPDATE a
			SET	a.[AccountNumber] = (CASE d.[AccountNumber_RVFlag] WHEN 1 THEN a.[AccountNumber] ELSE b.[AccountNumber] END),
				a.[AdditiveAccounting] = (CASE d.[AdditiveAccounting_RVFlag] WHEN 1 THEN a.[AdditiveAccounting] ELSE b.[AdditiveAccounting] END),
				a.[Address1] = (CASE d.[Address1_RVFlag] WHEN 1 THEN a.[Address1] ELSE b.[Address1] END),
				a.[Address2] = (CASE d.[Address2_RVFlag] WHEN 1 THEN a.[Address2] ELSE b.[Address2] END),
				a.[AllowDriverEntry] = (CASE d.[AllowDriverEntry_RVFlag] WHEN 1 THEN a.[AllowDriverEntry] ELSE b.[AllowDriverEntry] END),
				a.[City] = (CASE d.[City_RVFlag] WHEN 1 THEN a.[City] ELSE b.[City] END),
				a.[Code] = (CASE d.[Code_RVFlag] WHEN 1 THEN a.[Code] ELSE b.[Code] END),
				a.[ConsortiumTypeIndex] = (CASE d.[ConsortiumTypeIndex_RVFlag] WHEN 1 THEN a.[ConsortiumTypeIndex] ELSE b.[ConsortiumTypeIndex] END),
				a.[Contact1Address1] = (CASE d.[Contact1Address1_RVFlag] WHEN 1 THEN a.[Contact1Address1] ELSE b.[Contact1Address1] END),
				a.[Contact1Address2] = (CASE d.[Contact1Address2_RVFlag] WHEN 1 THEN a.[Contact1Address2] ELSE b.[Contact1Address2] END),
				a.[Contact1City] = (CASE d.[Contact1City_RVFlag] WHEN 1 THEN a.[Contact1City] ELSE b.[Contact1City] END),
				a.[Contact1Country] = (CASE d.[Contact1Country_RVFlag] WHEN 1 THEN a.[Contact1Country] ELSE b.[Contact1Country] END),
				a.[Contact1EmailAddress] = (CASE d.[Contact1EmailAddress_RVFlag] WHEN 1 THEN a.[Contact1EmailAddress] ELSE b.[Contact1EmailAddress] END),
				a.[Contact1Fax] = (CASE d.[Contact1Fax_RVFlag] WHEN 1 THEN a.[Contact1Fax] ELSE b.[Contact1Fax] END),
				a.[Contact1Name] = (CASE d.[Contact1Name_RVFlag] WHEN 1 THEN a.[Contact1Name] ELSE b.[Contact1Name] END),
				a.[Contact1PhoneMobile] = (CASE d.[Contact1PhoneMobile_RVFlag] WHEN 1 THEN a.[Contact1PhoneMobile] ELSE b.[Contact1PhoneMobile] END),
				a.[Contact1PhoneOffice] = (CASE d.[Contact1PhoneOffice_RVFlag] WHEN 1 THEN a.[Contact1PhoneOffice] ELSE b.[Contact1PhoneOffice] END),
				a.[Contact1State] = (CASE d.[Contact1State_RVFlag] WHEN 1 THEN a.[Contact1State] ELSE b.[Contact1State] END),
				a.[Contact1Zip] = (CASE d.[Contact1Zip_RVFlag] WHEN 1 THEN a.[Contact1Zip] ELSE b.[Contact1Zip] END),
				a.[Contact2Address1] = (CASE d.[Contact2Address1_RVFlag] WHEN 1 THEN a.[Contact2Address1] ELSE b.[Contact2Address1] END),
				a.[Contact2Address2] = (CASE d.[Contact2Address2_RVFlag] WHEN 1 THEN a.[Contact2Address2] ELSE b.[Contact2Address2] END),
				a.[Contact2City] = (CASE d.[Contact2City_RVFlag] WHEN 1 THEN a.[Contact2City] ELSE b.[Contact2City] END),
				a.[Contact2Country] = (CASE d.[Contact2Country_RVFlag] WHEN 1 THEN a.[Contact2Country] ELSE b.[Contact2Country] END),
				a.[Contact2EmailAddress] = (CASE d.[Contact2EmailAddress_RVFlag] WHEN 1 THEN a.[Contact2EmailAddress] ELSE b.[Contact2EmailAddress] END),
				a.[Contact2Fax] = (CASE d.[Contact2Fax_RVFlag] WHEN 1 THEN a.[Contact2Fax] ELSE b.[Contact2Fax] END),
				a.[Contact2Name] = (CASE d.[Contact2Name_RVFlag] WHEN 1 THEN a.[Contact2Name] ELSE b.[Contact2Name] END),
				a.[Contact2PhoneMobile] = (CASE d.[Contact2PhoneMobile_RVFlag] WHEN 1 THEN a.[Contact2PhoneMobile] ELSE b.[Contact2PhoneMobile] END),
				a.[Contact2PhoneOffice] = (CASE d.[Contact2PhoneOffice_RVFlag] WHEN 1 THEN a.[Contact2PhoneOffice] ELSE b.[Contact2PhoneOffice] END),
				a.[Contact2State] = (CASE d.[Contact2State_RVFlag] WHEN 1 THEN a.[Contact2State] ELSE b.[Contact2State] END),
				a.[Contact2Zip] = (CASE d.[Contact2Zip_RVFlag] WHEN 1 THEN a.[Contact2Zip] ELSE b.[Contact2Zip] END),
				a.[Country] = (CASE d.[Country_RVFlag] WHEN 1 THEN a.[Country] ELSE b.[Country] END),
				a.[CreditOK] = (CASE d.[CreditOK_RVFlag] WHEN 1 THEN a.[CreditOK] ELSE b.[CreditOK] END),
				a.[CustomerBillToTypeApplicationStringGuid] = (CASE d.[CustomerBillToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[CustomerBillToTypeApplicationStringGuid] ELSE b.[CustomerBillToTypeApplicationStringGuid] END),
				a.[CustomerShipToTypeApplicationStringGuid] = (CASE d.[CustomerShipToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[CustomerShipToTypeApplicationStringGuid] ELSE b.[CustomerShipToTypeApplicationStringGuid] END),
				a.[DeliveryToTerminalPermitted] = (CASE d.[DeliveryToTerminalPermitted_RVFlag] WHEN 1 THEN a.[DeliveryToTerminalPermitted] ELSE b.[DeliveryToTerminalPermitted] END),
				a.[DisableBillToAllocationsCheck] = (CASE d.[DisableBillToAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableBillToAllocationsCheck] ELSE b.[DisableBillToAllocationsCheck] END),
				a.[DisableOwnerAllocationsCheck] = (CASE d.[DisableOwnerAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableOwnerAllocationsCheck] ELSE b.[DisableOwnerAllocationsCheck] END),
				a.[DisableShipperAllocationsCheck] = (CASE d.[DisableShipperAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableShipperAllocationsCheck] ELSE b.[DisableShipperAllocationsCheck] END),
				a.[DisableShipToAllocationsCheck] = (CASE d.[DisableShipToAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableShipToAllocationsCheck] ELSE b.[DisableShipToAllocationsCheck] END),
				a.[EffectiveDate] = (CASE d.[EffectiveDate_RVFlag] WHEN 1 THEN a.[EffectiveDate] ELSE b.[EffectiveDate] END),
				a.[EmergencyContact] = (CASE d.[EmergencyContact_RVFlag] WHEN 1 THEN a.[EmergencyContact] ELSE b.[EmergencyContact] END),
				a.[EmergencyPhone] = (CASE d.[EmergencyPhone_RVFlag] WHEN 1 THEN a.[EmergencyPhone] ELSE b.[EmergencyPhone] END),
				a.[EPANumber] = (CASE d.[EPANumber_RVFlag] WHEN 1 THEN a.[EPANumber] ELSE b.[EPANumber] END),
				a.[ExpirationDate] = (CASE d.[ExpirationDate_RVFlag] WHEN 1 THEN a.[ExpirationDate] ELSE b.[ExpirationDate] END),
				a.[FAX] = (CASE d.[FAX_RVFlag] WHEN 1 THEN a.[FAX] ELSE b.[FAX] END),
				a.[FederalID] = (CASE d.[FederalID_RVFlag] WHEN 1 THEN a.[FederalID] ELSE b.[FederalID] END),
				a.[FederalID2] = (CASE d.[FederalID2_RVFlag] WHEN 1 THEN a.[FederalID2] ELSE b.[FederalID2] END),
				a.[FederalID3] = (CASE d.[FederalID3_RVFlag] WHEN 1 THEN a.[FederalID3] ELSE b.[FederalID3] END),
				a.[FederalID4] = (CASE d.[FederalID4_RVFlag] WHEN 1 THEN a.[FederalID4] ELSE b.[FederalID4] END),
				a.[FederalID5] = (CASE d.[FederalID5_RVFlag] WHEN 1 THEN a.[FederalID5] ELSE b.[FederalID5] END),
				a.[FlightPrefix] = (CASE d.[FlightPrefix_RVFlag] WHEN 1 THEN a.[FlightPrefix] ELSE b.[FlightPrefix] END),
				a.[FlushPermitted] = (CASE d.[FlushPermitted_RVFlag] WHEN 1 THEN a.[FlushPermitted] ELSE b.[FlushPermitted] END),
				a.[HazardousMaterialExclusion] = (CASE d.[HazardousMaterialExclusion_RVFlag] WHEN 1 THEN a.[HazardousMaterialExclusion] ELSE b.[HazardousMaterialExclusion] END),
				a.[IATAGuid] = (CASE d.[IATAGuid_RVFlag] WHEN 1 THEN a.[IATAGuid] ELSE b.[IATAGuid] END),
				a.[ID] = (CASE d.[ID_RVFlag] WHEN 1 THEN a.[ID] ELSE b.[ID] END),
				a.[InsuranceCompany] = (CASE d.[InsuranceCompany_RVFlag] WHEN 1 THEN a.[InsuranceCompany] ELSE b.[InsuranceCompany] END),
				a.[InsuranceExpiration] = (CASE d.[InsuranceExpiration_RVFlag] WHEN 1 THEN a.[InsuranceExpiration] ELSE b.[InsuranceExpiration] END),
				a.[InsurancePolicy] = (CASE d.[InsurancePolicy_RVFlag] WHEN 1 THEN a.[InsurancePolicy] ELSE b.[InsurancePolicy] END),
				a.[LastActivityDate] = (CASE d.[LastActivityDate_RVFlag] WHEN 1 THEN a.[LastActivityDate] ELSE b.[LastActivityDate] END),
				a.[LiabilityAmount] = (CASE d.[LiabilityAmount_RVFlag] WHEN 1 THEN a.[LiabilityAmount] ELSE b.[LiabilityAmount] END),
				a.[LicenseExpiration] = (CASE d.[LicenseExpiration_RVFlag] WHEN 1 THEN a.[LicenseExpiration] ELSE b.[LicenseExpiration] END),
				a.[LicenseNumber] = (CASE d.[LicenseNumber_RVFlag] WHEN 1 THEN a.[LicenseNumber] ELSE b.[LicenseNumber] END),
				a.[LoadRackDisplayText] = (CASE d.[LoadRackDisplayText_RVFlag] WHEN 1 THEN a.[LoadRackDisplayText] ELSE b.[LoadRackDisplayText] END),
				a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
				a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
				a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
				a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
				a.[MaximumVehicleWeight] = (CASE d.[MaximumVehicleWeight_RVFlag] WHEN 1 THEN a.[MaximumVehicleWeight] ELSE b.[MaximumVehicleWeight] END),
				a.[Name] = (CASE d.[Name_RVFlag] WHEN 1 THEN a.[Name] ELSE b.[Name] END),
				a.[Note] = (CASE d.[Note_RVFlag] WHEN 1 THEN a.[Note] ELSE b.[Note] END),
				a.[OnHold] = (CASE d.[OnHold_RVFlag] WHEN 1 THEN a.[OnHold] ELSE b.[OnHold] END),
				a.[Phone] = (CASE d.[Phone_RVFlag] WHEN 1 THEN a.[Phone] ELSE b.[Phone] END),
				a.[PickupFLights] = (CASE d.[PickupFLights_RVFlag] WHEN 1 THEN a.[PickupFLights] ELSE b.[PickupFLights] END),
				a.[PINRequired] = (CASE d.[PINRequired_RVFlag] WHEN 1 THEN a.[PINRequired] ELSE b.[PINRequired] END),
				a.[PumpOffPermitted] = (CASE d.[PumpOffPermitted_RVFlag] WHEN 1 THEN a.[PumpOffPermitted] ELSE b.[PumpOffPermitted] END),
				a.[PurchaseOrderRequired] = (CASE d.[PurchaseOrderRequired_RVFlag] WHEN 1 THEN a.[PurchaseOrderRequired] ELSE b.[PurchaseOrderRequired] END),
				a.[ReceivableAccount] = (CASE d.[ReceivableAccount_RVFlag] WHEN 1 THEN a.[ReceivableAccount] ELSE b.[ReceivableAccount] END),
				a.[RefinerCode] = (CASE d.[RefinerCode_RVFlag] WHEN 1 THEN a.[RefinerCode] ELSE b.[RefinerCode] END),
				a.[SCACCode] = (CASE d.[SCACCode_RVFlag] WHEN 1 THEN a.[SCACCode] ELSE b.[SCACCode] END),
				a.[ScullyRequired] = (CASE d.[ScullyRequired_RVFlag] WHEN 1 THEN a.[ScullyRequired] ELSE b.[ScullyRequired] END),
				a.[ShipperTypeApplicationStringGuid] = (CASE d.[ShipperTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[ShipperTypeApplicationStringGuid] ELSE b.[ShipperTypeApplicationStringGuid] END),
				a.[ShortName] = (CASE d.[ShortName_RVFlag] WHEN 1 THEN a.[ShortName] ELSE b.[ShortName] END),
				a.[State] = (CASE d.[State_RVFlag] WHEN 1 THEN a.[State] ELSE b.[State] END),
				a.[StateID] = (CASE d.[StateId_RVFlag] WHEN 1 THEN a.[StateID] ELSE b.[StateID] END),
				a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
				a.[SufferLossGain] = (CASE d.[SufferLossGain_RVFlag] WHEN 1 THEN a.[SufferLossGain] ELSE b.[SufferLossGain] END),
				a.[TaxNumber] = (CASE d.[TaxNumber_RVFlag] WHEN 1 THEN a.[TaxNumber] ELSE b.[TaxNumber] END),
				a.[UpdatedBy] = b.[UpdatedBy],
				a.[UpdatedDate] = SYSDATETIMEOFFSET(),
				a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
				a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
				a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
				a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
				a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
				a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
				a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
				a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
				a.[WeightUnits] = (CASE d.[WeightUnits_RVFlag] WHEN 1 THEN a.[WeightUnits] ELSE b.[WeightUnits] END),
				a.[Zip] = (CASE d.[Zip_RVFlag] WHEN 1 THEN a.[Zip] ELSE b.[Zip] END),
				a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END)
			FROM tblCompanies a
			INNER JOIN tblCompanies b
			ON b._MasterRecordGuid = a._MasterRecordGuid
			INNER JOIN @tblEntityToSiteHierarchy c
			ON a.SiteGuid = c.SiteGuid
			INNER JOIN erv.tblTempCompanyRecordVersioningFlag d
			ON d.CompanyGuid = a.CompanyGuid
			WHERE b.CompanyGuid = @SourceCompanyGuid
			AND d._CallingReferenceGuid = @callingRef1Guid

			DELETE erv.tblTempCompanyRecordVersioningFlag 
			WHERE _CallingReferenceGuid = @callingRef1Guid 


		
			/*Process those non-VersionSpecific External fields whose propagation require custom handling. */		

			--Equipments. 
			--The relationship between tblCompanies and tblEquipment is maintained fully on the tblEquipment side, which references the Company using the Company MasterRecordGuid. 
			--Therefore the changes made to the Equipments assignments of a Company record will be propagated only according to the FLC configuration on the associated equipment records.
			--There are no record versioning propagation actions to be taken for the Company child record versions as far as the tblCompanies-tblEquipment relationships are concerned.

			-- Process [AuthorizedShipTo] External Field
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedShipTo') = 0)
			BEGIN
				--Delete the child record version mappings that are not supported anymore in the parent Company
				DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.CompanyGuid
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.AssignedToCompanyGuid
				WHERE NOT EXISTS 
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d				
					WHERE d.CompanyGuid = @SourceCompanyGuid
					AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid)
				)		
													
				--Update the child record version mappings that have been modified in the parent Product
				UPDATE d
				SET d.Id = a.Id, 
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.AssignedToCompanyGuid
				INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
				ON d.CompanyGuid = b.CompanyGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.CompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
				(AssignedToCompanyGuid, CompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
				 b.CompanyGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.AssignedToCompanyGuid
				WHERE a.CompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
					WHERE d.CompanyGuid = b.CompanyGuid
					AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
				)
			END

			--Process [Drivers] External Field
			-- Personnel is both an External Attribute of Company (i.e. Company-To-Personnel mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Personnel mappings are also maintained as part of the Personnel entity, i.e. outside of the Company entity)
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Drivers') = 0)
			BEGIN
				--Only delete the child record version Personnel mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Personnel or the Personnel child record version does not loose its Company mappings when Company RecordVersioning is turned off).
				DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.CompanyGuid
				INNER JOIN tblPersonnel c
				ON c.PersonnelGuid = a.PersonnelGuid
				INNER JOIN map.tblEntityPersonnelToSite d
				ON d.PersonnelGuid = c._MasterRecordGuid
				AND d.SiteGuid = b.SiteGuid
				LEFT OUTER JOIN 
				(
					SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
					FROM erv.tblEntityRecordVersioningFieldConfig e1
					INNER JOIN erv.tblEntitySegmentTemplate e2
					ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
					WHERE e2.EntityTypeId = 'Personnel'
					AND TargetField = 'Carrier'
				) e
				ON e.SiteGroupGuid = d.AssignedFromSiteGuid	
				WHERE
				(
					(  -- mappings at a lower sitegroup/site to a child record version of the same Personnel record
						c.SiteGuid = b.SiteGuid
						AND c.PersonnelGuid <> c._MasterRecordGuid
						AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Personnel child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

					)		
					OR
					( -- mappings to the same Personnel master record, but at a lower sitegroup/site
						c.SiteGuid <> b.SiteGuid
						AND c.PersonnelGuid = c._MasterRecordGuid
					)	
				)
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d				
					WHERE d.CompanyGuid = @SourceCompanyGuid
					AND d.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, @ownerSiteGuid)
				)
													
				--Update the child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.ID = a.ID, 
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblCompanyPersonnelAssignedToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblPersonnel c
				ON c.PersonnelGuid = a.PersonnelGuid
				INNER JOIN [map].[tblCompanyPersonnelAssignedToCompany] d
				ON d.CompanyGuid = b.CompanyGuid
				AND d.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.CompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
				(PersonnelGuid, CompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid), 
				b.CompanyGuid, b.SiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblCompanyPersonnelAssignedToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblPersonnel c
				ON c.PersonnelGuid = a.PersonnelGuid
				WHERE a.CompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d
					WHERE d.CompanyGuid = b.CompanyGuid
					AND d.PersonnelGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid)
				)
			END

			-- Process [UnavailableInventories] External Field
			-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') = 0)
			BEGIN
				--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
				DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.AssignedToCompanyGuid
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN map.tblEntityProductToSite d
				ON d.ProductGuid = c._MasterRecordGuid
				AND d.SiteGuid = b.SiteGuid
				LEFT OUTER JOIN 
				(
					SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
					FROM erv.tblEntityRecordVersioningFieldConfig e1
					INNER JOIN erv.tblEntitySegmentTemplate e2
					ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
					WHERE e2.EntityTypeId = 'Product'
					AND TargetField = 'UnavailableInventories'
				) e
				ON e.SiteGroupGuid = d.AssignedFromSiteGuid	
				WHERE
				(
					(  -- mappings at a lower sitegroup/site to a child record version of the same Product record
						c.SiteGuid = b.SiteGuid
						AND c.ProductGuid <> c._MasterRecordGuid
						AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Product child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

					)		
					OR
					( -- mappings to the same Product master record, but at a lower sitegroup/site
						c.SiteGuid <> b.SiteGuid
						AND c.ProductGuid = c._MasterRecordGuid
					)	
				)
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d				
					WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
					AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @ownerSiteGuid)
				)	
													
				--Update the child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.Sequence = a.Sequence, 
				d.BlendPercentage = a.BlendPercentage,
				d.AdditiveRate = a.AdditiveRate,
				d.Ratio = a.Ratio,
				d.AdditiveCycleVolume = a.AdditiveCycleVolume,
				d.Tolerance = a.Tolerance,		
				d.PresetNumber = a.PresetNumber,
				d.AdditiveProfileGuid = a.AdditiveProfileGuid,
				d.TankGuid = a.TankGuid,
				d.MeterID = a.MeterID,
				d.ShipToProductID = a.ShipToProductID,
				d.ShipToProductCode = a.ShipToProductCode,
				d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
				d.UnavailableInventoryGross = a.UnavailableInventoryGross,
				d.UnavailableInventoryNet = a.UnavailableInventoryNet,
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblProductToUnavailableInventoryCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
				ON d.AssignedToCompanyGuid = b.CompanyGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
				(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
				b.CompanyGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
				a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, 
				GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblProductToUnavailableInventoryCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
					WHERE d.AssignedToCompanyGuid = b.CompanyGuid
					AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
				)
			END

			-- Process [ShipToAuthorizedProducts] External Field
			-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'ShipToAuthorizedProducts') = 0)
			BEGIN
				--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
				DELETE a FROM [map].[tblProductToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.AssignedToCompanyGuid
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN map.tblEntityProductToSite d
				ON d.ProductGuid = c._MasterRecordGuid
				AND d.SiteGuid = b.SiteGuid
				LEFT OUTER JOIN 
				(
					SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
					FROM erv.tblEntityRecordVersioningFieldConfig e1
					INNER JOIN erv.tblEntitySegmentTemplate e2
					ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
					WHERE e2.EntityTypeId = 'Product'
					AND TargetField = 'AuthorizedCustomers'
				) e
				ON e.SiteGroupGuid = d.AssignedFromSiteGuid	
				WHERE
				(
					(  -- mappings at a lower sitegroup/site to a child record version of the same Product record
						c.SiteGuid = b.SiteGuid
						AND c.ProductGuid <> c._MasterRecordGuid
						AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Product child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

					)		
					OR
					( -- mappings to the same Product master record, but at a lower sitegroup/site
						c.SiteGuid <> b.SiteGuid
						AND c.ProductGuid = c._MasterRecordGuid
					)	
				)
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblProductToCompany] d				
					WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
					AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @ownerSiteGuid)
				)	
													
				--Update the child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.Sequence = a.Sequence, 
				d.BlendPercentage = a.BlendPercentage,
				d.AdditiveRate = a.AdditiveRate,
				d.Ratio = a.Ratio,
				d.AdditiveCycleVolume = a.AdditiveCycleVolume,
				d.Tolerance = a.Tolerance,		
				d.PresetNumber = a.PresetNumber,
				d.AdditiveProfileGuid = a.AdditiveProfileGuid,
				d.TankGuid = a.TankGuid,
				d.MeterID = a.MeterID,
				d.ShipToProductID = a.ShipToProductID,
				d.ShipToProductCode = a.ShipToProductCode,
				d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
				d.UnavailableInventoryGross = a.UnavailableInventoryGross,
				d.UnavailableInventoryNet = a.UnavailableInventoryNet,
				d.SpecialInstructionNote = a.SpecialInstructionNote,
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblProductToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN [map].[tblProductToCompany] d
				ON d.AssignedToCompanyGuid = b.CompanyGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblProductToCompany]
				(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
				ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, 
				CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
				b.CompanyGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
				a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
				GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblProductToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblProductToCompany] d
					WHERE d.AssignedToCompanyGuid = b.CompanyGuid
					AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
				)
			END

			-- Process [AuthorizedCarriers] External Field
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCarriers') = 0)
			BEGIN
				--Delete the child record version mappings that are not supported anymore in the parent Company
				DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.AssignedToCompanyGuid
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.CompanyGuid
				WHERE a.CompanyGuid IS NOT NULL
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d				
					WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
					AND (d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @ownerSiteGuid))					
				)		

				DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.AssignedToCompanyGuid
				WHERE a.CompanyGuid IS NULL
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d				
					WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
					AND ((d.CompanyGuid IS NULL) AND (d.SiteGuid = @ownerSiteGuid))					
				)	
													
				--Update the child record version mappings that have been modified in the parent Product
				UPDATE d
				SET d.Id = a.Id, 
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.CompanyGuid
				INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
				ON d.AssignedToCompanyGuid = b.CompanyGuid
				AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
				(CompanyGuid, AssignedToCompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), 
				 b.CompanyGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblCompanies c
				ON c.CompanyGuid = a.CompanyGuid
				WHERE a.CompanyGuid IS NOT NULL
				AND a.AssignedToCompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
					WHERE d.AssignedToCompanyGuid = b.CompanyGuid
					AND 
					(	
						a.CompanyGuid IS NOT NULL
						AND d.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)				
					)
				)

				INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
				(CompanyGuid, AssignedToCompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				 SELECT NULL, 
				 b.CompanyGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				WHERE a.CompanyGuid IS NULL
				AND a.AssignedToCompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
					WHERE d.AssignedToCompanyGuid = b.CompanyGuid
					AND d.SiteGuid = b.SiteGuid
					AND d.CompanyGuid IS NULL
				)
			END

			-- Process [SupplierAuthorizedProducts] External Field
			-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') = 0)
			BEGIN
				--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
				DELETE a FROM [map].[tblProductToSupplierProductCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.AssignedToCompanyGuid
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN map.tblEntityProductToSite d
				ON d.ProductGuid = c._MasterRecordGuid
				AND d.SiteGuid = b.SiteGuid
				LEFT OUTER JOIN 
				(
					SELECT e1.SiteGroupGuid, e1.ForwardControlMode 
					FROM erv.tblEntityRecordVersioningFieldConfig e1
					INNER JOIN erv.tblEntitySegmentTemplate e2
					ON e2.EntitySegmentTemplateGuid = e1.EntitySegmentTemplateGuid
					WHERE e2.EntityTypeId = 'Product'
					AND TargetField = 'SupplierAuthorizedProducts'
				) e
				ON e.SiteGroupGuid = d.AssignedFromSiteGuid			
				WHERE
				(
					(  -- mappings at a lower sitegroup/site to a child record version of the same Product record
						c.SiteGuid = b.SiteGuid
						AND c.ProductGuid <> c._MasterRecordGuid
						AND NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific') --Exclude the mappings that are owned by a Product child record version whose ShipToAuthorizedProducts field is set as VersionSpecific.

					)		
					OR
					( -- mappings to the same Product master record, but at a lower sitegroup/site
						c.SiteGuid <> b.SiteGuid
						AND c.ProductGuid = c._MasterRecordGuid
					)	
				)
				AND NOT EXISTS 
				(
					SELECT * FROM [map].[tblProductToSupplierProductCompany] d				
					WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
					AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @ownerSiteGuid)
				)	
													
				--Update the child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.Sequence = a.Sequence, 
				d.BlendPercentage = a.BlendPercentage,
				d.AdditiveRate = a.AdditiveRate,
				d.Ratio = a.Ratio,
				d.AdditiveCycleVolume = a.AdditiveCycleVolume,
				d.Tolerance = a.Tolerance,		
				d.PresetNumber = a.PresetNumber,
				d.AdditiveProfileGuid = a.AdditiveProfileGuid,
				d.TankGuid = a.TankGuid,
				d.MeterID = a.MeterID,
				d.ShipToProductID = a.ShipToProductID,
				d.ShipToProductCode = a.ShipToProductCode,
				d.ShipToLoadRackDisplayText = a.ShipToLoadRackDisplayText,
				d.UnavailableInventoryGross = a.UnavailableInventoryGross,
				d.UnavailableInventoryNet = a.UnavailableInventoryNet,
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblProductToSupplierProductCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				INNER JOIN [map].[tblProductToSupplierProductCompany] d
				ON d.AssignedToCompanyGuid = b.CompanyGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid)
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblProductToSupplierProductCompany]
				(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
				ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
				CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
				b.CompanyGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
				a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
				GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblProductToSupplierProductCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN tblProducts c
				ON c.ProductGuid = a.ProductGuid
				WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblProductToSupplierProductCompany] d
					WHERE d.AssignedToCompanyGuid = b.CompanyGuid
					AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
				)
			END

			-- Process [AccessSchedule] External Field
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AccessSchedule') = 0)
			BEGIN
				--Delete the child record version mappings that are not supported anymore in the parent Company
				DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.CompanyGuid
				WHERE NOT EXISTS 
				(
					SELECT * FROM [dbo].[tblScheduleCompanyAccess] d				
					WHERE d.CompanyGuid = @SourceCompanyGuid
					AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
				)		
													
				--Update the child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.Enabled = a.Enabled, 
				d.OpeningTime = a.OpeningTime,
				d.ClosingTime = a.ClosingTime,
				d.EndOfDayEnabled = a.EndOfDayEnabled,
				d.EndOfDayTime = a.EndOfDayTime,
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [dbo].[tblScheduleCompanyAccess] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN [dbo].[tblScheduleCompanyAccess] d
				ON d.CompanyGuid = b.CompanyGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
				WHERE a.CompanyGuid = @SourceCompanyGuid

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [dbo].[tblScheduleCompanyAccess]
				(CompanyGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT b.CompanyGuid, a.LookupDayOfWeekIndex, a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
				GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [dbo].[tblScheduleCompanyAccess] a
				CROSS JOIN @tblTargetChildRecordVersions b
				WHERE a.CompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [dbo].[tblScheduleCompanyAccess] d
					WHERE d.CompanyGuid = b.CompanyGuid
					AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
				)
			END
		
			-- Process [CertificatesAndPermits] External Field
			IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'CertificatesAndPermits') = 0)
			BEGIN
				--Delete the child record version mappings that are not supported anymore in the parent Company
				DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
				INNER JOIN @tblTargetChildRecordVersions b
				ON b.CompanyGuid = a.CompanyGuid
				WHERE NOT EXISTS 
				(
					SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d				
					WHERE d.CompanyGuid = @SourceCompanyGuid
					AND d.QualificationGuid = a.QualificationGuid
				)		
													
				--Update the active (non-historical) child record version mappings that have been modified in the parent Company
				UPDATE d
				SET d.Sequence = a.Sequence, 
				d.DateCompleted = a.DateCompleted,
				d.DateDue = a.DateDue,
				d.ExpirationDate = a.ExpirationDate,
				d.Id = a.Id,		
				d.Instructor = a.Instructor,
				d.Rating = a.Rating,
				d.UpdatedDate = GETDATE(),
				d.UpdatedBy = a.UpdatedBy
				FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				INNER JOIN [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
				ON d.CompanyGuid = b.CompanyGuid
				AND d.QualificationGuid = a.QualificationGuid
				AND d.HistoricalRecord = a.HistoricalRecord			
				WHERE a.CompanyGuid = @SourceCompanyGuid
				AND d.HistoricalRecord = 0

				--Insert a new mapping for each parent mapping not found in the child record versions
				INSERT INTO [map].[tblQualificationCompanyCertificateAndPermitToCompany]
				(CompanyGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
				SELECT b.CompanyGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
				GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
				FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
				CROSS JOIN @tblTargetChildRecordVersions b
				WHERE a.CompanyGuid = @SourceCompanyGuid
				AND NOT EXISTS
				(
					SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
					WHERE d.CompanyGuid = b.CompanyGuid
					AND d.QualificationGuid = a.QualificationGuid
				)
			END

		-- Process [CompanyRoles] External Field
		-- Company Roles are created and deleted independently of Record Versioning. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.		

		END

		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --PropagateToChildRecordVersions
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --PropagateToChildRecordVersions
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
						+ 'Procedure Name: [erv].usp_PropagateCompanyRevisionByEntityRecordChange' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
