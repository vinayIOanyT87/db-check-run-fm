/*
	DROP PROCEDURE [erv].[usp_PropagateCompanyRecordVersionBySegment]

	EXEC [erv].[usp_PropagateCompanyRecordVersionBySegment] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_PropagateCompanyRecordVersionBySegment] '1eacc1d7-292d-4932-bc59-9c02740c6c19'

*/

CREATE PROCEDURE [erv].[usp_PropagateCompanyRecordVersionBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PropagateCompanyRecordVersionBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Propagate all the Parent Specific fields of all the record versions in a Company segment from a given sitegroup down to all the sites/sitegroups that have a direct assignment from the given sitegroup.
	-- This Stored Procedure is to be used to enforce the effect of fields being changed from VersionSpecific to ParentSpecific as a result of Field Level Control configuration changes.
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Entity Segment Template that needs to be processed.
	-- 3. @SourceSiteGroupGuid: Guid of the segment SiteGroup from which the ParentSpecific fields are to be propagated.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		--Retrieve the VersionSpecific fields for the entity record
		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid

		IF (NOT EXISTS (SELECT * FROM @tblSourceVersionSpecificFields))
		BEGIN				
			/*
				All fields are ParentSpecific. This means that there will be no child record versions of the entity record for any site/sitegroup in the hierarchy below owner 
				sitegroup of the entity record, i.e. Record Versioning field data propagation does not apply.
			*/
			RETURN;
		END

		DECLARE @entityTypeId nvarchar(100)
		SELECT @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		DECLARE @callingRefGuid uniqueidentifier
		SET @callingRefGuid = NEWID()

		--Capture the Site/SiteGroup, MasterRecordGuid, and CompanyGuid of the child record versions that need to be updated.
		--This includes all the child record versions down the site hierarchy that have the same masterrecordguid as those owned by the SourceSiteGroup and which share the same filter value as the segment being processed, irrespective of where they were assigned from.
		IF (@entityTypeId = 'Company')
		BEGIN
			INSERT INTO erv.tblTempTargetEntitySite
			(SiteGuid, MasterRecordGuid, EntityGuid, ParentEntityGuid, _CallingReferenceGuid)
			SELECT a.SiteGuid, a._MasterRecordGuid, a.CompanyGuid, d.CompanyGuid, @callingRefGuid
			FROM [dbo].[tblCompanies] a
			INNER JOIN map.tblEntityCompanyToSite b
			ON b.CompanyGuid = a._MasterRecordGuid
			AND b.SiteGuid = a.SiteGuid
			INNER JOIN tblCompanies d
			ON d._MasterRecordGuid = b.CompanyGuid
			AND d.SiteGuid = b.AssignedFromSiteGuid  --Note: we do not use erv.udf_GetFirstParentRecordVersionGuid in that instance, but instead we get the parent record version directly from the AssignedFrom sitegroup. This simplification is made possible by the fact that this process is only concerned about updating child record versions. If the parent record version is not available from the direct AssignedFrom sitegroup, then it means that Record Versioning has been turned off at the parent sitegroup, and if that is the case it would be turned off at the current sitegroup/site as well, and therefore their would be no child record version to update in any case.
			WHERE b.AssignedFromSiteGuid = @SourceSiteGroupGuid
			AND a.CompanyGuid <> a._MasterRecordGuid
		END											
		
		IF (NOT EXISTS (SELECT * FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid))
		BEGIN							
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblCompanies table, and set the flag according to whether the field is VersionSpecific or not.
		INSERT INTO erv.tblTempCompanyRecordVersioningFlag
		(CompanyGuid, _CallingReferenceGuid)
		SELECT DISTINCT MasterRecordGuid, @callingRefGuid FROM erv.tblTempTargetEntitySite WHERE _CallingReferenceGuid = @callingRefGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForSegment] @EntitySegmentTemplateGuid, NULL, @SourceSiteGroupGuid, NULL, @callingRefGuid

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --PropagateToChildRecordVersions
            SET @BeginTran = 1   
		END  		

		-- Update all the internal ParentSpecific fields for all applicable sites and sitegroups
		UPDATE a
			SET	a.[AccountNumber] = (CASE e.[AccountNumber_RVFlag] WHEN 1 THEN a.[AccountNumber] ELSE b.[AccountNumber] END),
			a.[AdditiveAccounting] = (CASE e.[AdditiveAccounting_RVFlag] WHEN 1 THEN a.[AdditiveAccounting] ELSE b.[AdditiveAccounting] END),
			a.[Address1] = (CASE e.[Address1_RVFlag] WHEN 1 THEN a.[Address1] ELSE b.[Address1] END),
			a.[Address2] = (CASE e.[Address2_RVFlag] WHEN 1 THEN a.[Address2] ELSE b.[Address2] END),
			a.[AllowDriverEntry] = (CASE e.[AllowDriverEntry_RVFlag] WHEN 1 THEN a.[AllowDriverEntry] ELSE b.[AllowDriverEntry] END),
			a.[City] = (CASE e.[City_RVFlag] WHEN 1 THEN a.[City] ELSE b.[City] END),
			a.[Code] = (CASE e.[Code_RVFlag] WHEN 1 THEN a.[Code] ELSE b.[Code] END),
			a.[ConsortiumTypeIndex] = (CASE e.[ConsortiumTypeIndex_RVFlag] WHEN 1 THEN a.[ConsortiumTypeIndex] ELSE b.[ConsortiumTypeIndex] END),
			a.[Contact1Address1] = (CASE e.[Contact1Address1_RVFlag] WHEN 1 THEN a.[Contact1Address1] ELSE b.[Contact1Address1] END),
			a.[Contact1Address2] = (CASE e.[Contact1Address2_RVFlag] WHEN 1 THEN a.[Contact1Address2] ELSE b.[Contact1Address2] END),
			a.[Contact1City] = (CASE e.[Contact1City_RVFlag] WHEN 1 THEN a.[Contact1City] ELSE b.[Contact1City] END),
			a.[Contact1Country] = (CASE e.[Contact1Country_RVFlag] WHEN 1 THEN a.[Contact1Country] ELSE b.[Contact1Country] END),
			a.[Contact1EmailAddress] = (CASE e.[Contact1EmailAddress_RVFlag] WHEN 1 THEN a.[Contact1EmailAddress] ELSE b.[Contact1EmailAddress] END),
			a.[Contact1Fax] = (CASE e.[Contact1Fax_RVFlag] WHEN 1 THEN a.[Contact1Fax] ELSE b.[Contact1Fax] END),
			a.[Contact1Name] = (CASE e.[Contact1Name_RVFlag] WHEN 1 THEN a.[Contact1Name] ELSE b.[Contact1Name] END),
			a.[Contact1PhoneMobile] = (CASE e.[Contact1PhoneMobile_RVFlag] WHEN 1 THEN a.[Contact1PhoneMobile] ELSE b.[Contact1PhoneMobile] END),
			a.[Contact1PhoneOffice] = (CASE e.[Contact1PhoneOffice_RVFlag] WHEN 1 THEN a.[Contact1PhoneOffice] ELSE b.[Contact1PhoneOffice] END),
			a.[Contact1State] = (CASE e.[Contact1State_RVFlag] WHEN 1 THEN a.[Contact1State] ELSE b.[Contact1State] END),
			a.[Contact1Zip] = (CASE e.[Contact1Zip_RVFlag] WHEN 1 THEN a.[Contact1Zip] ELSE b.[Contact1Zip] END),
			a.[Contact2Address1] = (CASE e.[Contact2Address1_RVFlag] WHEN 1 THEN a.[Contact2Address1] ELSE b.[Contact2Address1] END),
			a.[Contact2Address2] = (CASE e.[Contact2Address2_RVFlag] WHEN 1 THEN a.[Contact2Address2] ELSE b.[Contact2Address2] END),
			a.[Contact2City] = (CASE e.[Contact2City_RVFlag] WHEN 1 THEN a.[Contact2City] ELSE b.[Contact2City] END),
			a.[Contact2Country] = (CASE e.[Contact2Country_RVFlag] WHEN 1 THEN a.[Contact2Country] ELSE b.[Contact2Country] END),
			a.[Contact2EmailAddress] = (CASE e.[Contact2EmailAddress_RVFlag] WHEN 1 THEN a.[Contact2EmailAddress] ELSE b.[Contact2EmailAddress] END),
			a.[Contact2Fax] = (CASE e.[Contact2Fax_RVFlag] WHEN 1 THEN a.[Contact2Fax] ELSE b.[Contact2Fax] END),
			a.[Contact2Name] = (CASE e.[Contact2Name_RVFlag] WHEN 1 THEN a.[Contact2Name] ELSE b.[Contact2Name] END),
			a.[Contact2PhoneMobile] = (CASE e.[Contact2PhoneMobile_RVFlag] WHEN 1 THEN a.[Contact2PhoneMobile] ELSE b.[Contact2PhoneMobile] END),
			a.[Contact2PhoneOffice] = (CASE e.[Contact2PhoneOffice_RVFlag] WHEN 1 THEN a.[Contact2PhoneOffice] ELSE b.[Contact2PhoneOffice] END),
			a.[Contact2State] = (CASE e.[Contact2State_RVFlag] WHEN 1 THEN a.[Contact2State] ELSE b.[Contact2State] END),
			a.[Contact2Zip] = (CASE e.[Contact2Zip_RVFlag] WHEN 1 THEN a.[Contact2Zip] ELSE b.[Contact2Zip] END),
			a.[Country] = (CASE e.[Country_RVFlag] WHEN 1 THEN a.[Country] ELSE b.[Country] END),
			a.[CreditOK] = (CASE e.[CreditOK_RVFlag] WHEN 1 THEN a.[CreditOK] ELSE b.[CreditOK] END),
			a.[CustomerBillToTypeApplicationStringGuid] = (CASE e.[CustomerBillToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[CustomerBillToTypeApplicationStringGuid] ELSE b.[CustomerBillToTypeApplicationStringGuid] END),
			a.[CustomerShipToTypeApplicationStringGuid] = (CASE e.[CustomerShipToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[CustomerShipToTypeApplicationStringGuid] ELSE b.[CustomerShipToTypeApplicationStringGuid] END),
			a.[DeliveryToTerminalPermitted] = (CASE e.[DeliveryToTerminalPermitted_RVFlag] WHEN 1 THEN a.[DeliveryToTerminalPermitted] ELSE b.[DeliveryToTerminalPermitted] END),
			a.[DisableBillToAllocationsCheck] = (CASE e.[DisableBillToAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableBillToAllocationsCheck] ELSE b.[DisableBillToAllocationsCheck] END),
			a.[DisableOwnerAllocationsCheck] = (CASE e.[DisableOwnerAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableOwnerAllocationsCheck] ELSE b.[DisableOwnerAllocationsCheck] END),
			a.[DisableShipperAllocationsCheck] = (CASE e.[DisableShipperAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableShipperAllocationsCheck] ELSE b.[DisableShipperAllocationsCheck] END),
			a.[DisableShipToAllocationsCheck] = (CASE e.[DisableShipToAllocationsCheck_RVFlag] WHEN 1 THEN a.[DisableShipToAllocationsCheck] ELSE b.[DisableShipToAllocationsCheck] END),
			a.[EffectiveDate] = (CASE e.[EffectiveDate_RVFlag] WHEN 1 THEN a.[EffectiveDate] ELSE b.[EffectiveDate] END),
			a.[EmergencyContact] = (CASE e.[EmergencyContact_RVFlag] WHEN 1 THEN a.[EmergencyContact] ELSE b.[EmergencyContact] END),
			a.[EmergencyPhone] = (CASE e.[EmergencyPhone_RVFlag] WHEN 1 THEN a.[EmergencyPhone] ELSE b.[EmergencyPhone] END),
			a.[EPANumber] = (CASE e.[EPANumber_RVFlag] WHEN 1 THEN a.[EPANumber] ELSE b.[EPANumber] END),
			a.[ExpirationDate] = (CASE e.[ExpirationDate_RVFlag] WHEN 1 THEN a.[ExpirationDate] ELSE b.[ExpirationDate] END),
			a.[FAX] = (CASE e.[FAX_RVFlag] WHEN 1 THEN a.[FAX] ELSE b.[FAX] END),
			a.[FederalID] = (CASE e.[FederalID_RVFlag] WHEN 1 THEN a.[FederalID] ELSE b.[FederalID] END),
			a.[FederalID2] = (CASE e.[FederalID2_RVFlag] WHEN 1 THEN a.[FederalID2] ELSE b.[FederalID2] END),
			a.[FederalID3] = (CASE e.[FederalID3_RVFlag] WHEN 1 THEN a.[FederalID3] ELSE b.[FederalID3] END),
			a.[FederalID4] = (CASE e.[FederalID4_RVFlag] WHEN 1 THEN a.[FederalID4] ELSE b.[FederalID4] END),
			a.[FederalID5] = (CASE e.[FederalID5_RVFlag] WHEN 1 THEN a.[FederalID5] ELSE b.[FederalID5] END),
			a.[FlightPrefix] = (CASE e.[FlightPrefix_RVFlag] WHEN 1 THEN a.[FlightPrefix] ELSE b.[FlightPrefix] END),
			a.[FlushPermitted] = (CASE e.[FlushPermitted_RVFlag] WHEN 1 THEN a.[FlushPermitted] ELSE b.[FlushPermitted] END),
			a.[HazardousMaterialExclusion] = (CASE e.[HazardousMaterialExclusion_RVFlag] WHEN 1 THEN a.[HazardousMaterialExclusion] ELSE b.[HazardousMaterialExclusion] END),
			a.[HiddenDate] = (CASE e.[HiddenDate_RVFlag] WHEN 1 THEN a.[HiddenDate] ELSE b.[HiddenDate] END),
			a.[IATAGuid] = (CASE e.[IATAGuid_RVFlag] WHEN 1 THEN a.[IATAGuid] ELSE b.[IATAGuid] END),
			a.[ID] = (CASE e.[ID_RVFlag] WHEN 1 THEN a.[ID] ELSE b.[ID] END),
			a.[InsuranceCompany] = (CASE e.[InsuranceCompany_RVFlag] WHEN 1 THEN a.[InsuranceCompany] ELSE b.[InsuranceCompany] END),
			a.[InsuranceExpiration] = (CASE e.[InsuranceExpiration_RVFlag] WHEN 1 THEN a.[InsuranceExpiration] ELSE b.[InsuranceExpiration] END),
			a.[InsurancePolicy] = (CASE e.[InsurancePolicy_RVFlag] WHEN 1 THEN a.[InsurancePolicy] ELSE b.[InsurancePolicy] END),
			a.[LastActivityDate] = (CASE e.[LastActivityDate_RVFlag] WHEN 1 THEN a.[LastActivityDate] ELSE b.[LastActivityDate] END),
			a.[LiabilityAmount] = (CASE e.[LiabilityAmount_RVFlag] WHEN 1 THEN a.[LiabilityAmount] ELSE b.[LiabilityAmount] END),
			a.[LicenseExpiration] = (CASE e.[LicenseExpiration_RVFlag] WHEN 1 THEN a.[LicenseExpiration] ELSE b.[LicenseExpiration] END),
			a.[LicenseNumber] = (CASE e.[LicenseNumber_RVFlag] WHEN 1 THEN a.[LicenseNumber] ELSE b.[LicenseNumber] END),
			a.[LoadRackDisplayText] = (CASE e.[LoadRackDisplayText_RVFlag] WHEN 1 THEN a.[LoadRackDisplayText] ELSE b.[LoadRackDisplayText] END),
			a.[LockedOut] = (CASE e.[LockedOut_RVFlag] WHEN 1 THEN a.[LockedOut] ELSE b.[LockedOut] END),
			a.[LockedOutDate] = (CASE e.[LockedOutDate_RVFlag] WHEN 1 THEN a.[LockedOutDate] ELSE b.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE e.[LockedOutReason_RVFlag] WHEN 1 THEN a.[LockedOutReason] ELSE b.[LockedOutReason] END),
			a.[LowStockWarning] = (CASE e.[LowStockWarning_RVFlag] WHEN 1 THEN a.[LowStockWarning] ELSE b.[LowStockWarning] END),
			a.[MaximumVehicleWeight] = (CASE e.[MaximumVehicleWeight_RVFlag] WHEN 1 THEN a.[MaximumVehicleWeight] ELSE b.[MaximumVehicleWeight] END),
			a.[Name] = (CASE e.[Name_RVFlag] WHEN 1 THEN a.[Name] ELSE b.[Name] END),
			a.[Note] = (CASE e.[Note_RVFlag] WHEN 1 THEN a.[Note] ELSE b.[Note] END),
			a.[OnHold] = (CASE e.[OnHold_RVFlag] WHEN 1 THEN a.[OnHold] ELSE b.[OnHold] END),
			a.[Phone] = (CASE e.[Phone_RVFlag] WHEN 1 THEN a.[Phone] ELSE b.[Phone] END),
			a.[PickupFLights] = (CASE e.[PickupFLights_RVFlag] WHEN 1 THEN a.[PickupFLights] ELSE b.[PickupFLights] END),
			a.[PINRequired] = (CASE e.[PINRequired_RVFlag] WHEN 1 THEN a.[PINRequired] ELSE b.[PINRequired] END),
			a.[PumpOffPermitted] = (CASE e.[PumpOffPermitted_RVFlag] WHEN 1 THEN a.[PumpOffPermitted] ELSE b.[PumpOffPermitted] END),
			a.[PurchaseOrderRequired] = (CASE e.[PurchaseOrderRequired_RVFlag] WHEN 1 THEN a.[PurchaseOrderRequired] ELSE b.[PurchaseOrderRequired] END),
			a.[ReceivableAccount] = (CASE e.[ReceivableAccount_RVFlag] WHEN 1 THEN a.[ReceivableAccount] ELSE b.[ReceivableAccount] END),
			a.[RefinerCode] = (CASE e.[RefinerCode_RVFlag] WHEN 1 THEN a.[RefinerCode] ELSE b.[RefinerCode] END),
			a.[SCACCode] = (CASE e.[SCACCode_RVFlag] WHEN 1 THEN a.[SCACCode] ELSE b.[SCACCode] END),
			a.[ScullyRequired] = (CASE e.[ScullyRequired_RVFlag] WHEN 1 THEN a.[ScullyRequired] ELSE b.[ScullyRequired] END),
			a.[ShipperTypeApplicationStringGuid] = (CASE e.[ShipperTypeApplicationStringGuid_RVFlag] WHEN 1 THEN a.[ShipperTypeApplicationStringGuid] ELSE b.[ShipperTypeApplicationStringGuid] END),
			a.[ShortName] = (CASE e.[ShortName_RVFlag] WHEN 1 THEN a.[ShortName] ELSE b.[ShortName] END),
			a.[State] = (CASE e.[State_RVFlag] WHEN 1 THEN a.[State] ELSE b.[State] END),
			a.[StateID] = (CASE e.[StateId_RVFlag] WHEN 1 THEN a.[StateID] ELSE b.[StateID] END),
			a.[StockTrack] = (CASE e.[StockTrack_RVFlag] WHEN 1 THEN a.[StockTrack] ELSE b.[StockTrack] END),
			a.[SufferLossGain] = (CASE e.[SufferLossGain_RVFlag] WHEN 1 THEN a.[SufferLossGain] ELSE b.[SufferLossGain] END),
			a.[TaxNumber] = (CASE e.[TaxNumber_RVFlag] WHEN 1 THEN a.[TaxNumber] ELSE b.[TaxNumber] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE e.[UserData1_RVFlag] WHEN 1 THEN a.[UserData1] ELSE b.[UserData1] END),
			a.[UserData2] = (CASE e.[UserData2_RVFlag] WHEN 1 THEN a.[UserData2] ELSE b.[UserData2] END),
			a.[UserData3] = (CASE e.[UserData3_RVFlag] WHEN 1 THEN a.[UserData3] ELSE b.[UserData3] END),
			a.[UserData4] = (CASE e.[UserData4_RVFlag] WHEN 1 THEN a.[UserData4] ELSE b.[UserData4] END),
			a.[UserData5] = (CASE e.[UserData5_RVFlag] WHEN 1 THEN a.[UserData5] ELSE b.[UserData5] END),
			a.[UserData6] = (CASE e.[UserData6_RVFlag] WHEN 1 THEN a.[UserData6] ELSE b.[UserData6] END),
			a.[UserData7] = (CASE e.[UserData7_RVFlag] WHEN 1 THEN a.[UserData7] ELSE b.[UserData7] END),
			a.[UserData8] = (CASE e.[UserData8_RVFlag] WHEN 1 THEN a.[UserData8] ELSE b.[UserData8] END),
			a.[WeightUnits] = (CASE e.[WeightUnits_RVFlag] WHEN 1 THEN a.[WeightUnits] ELSE b.[WeightUnits] END),
			a.[Zip] = (CASE e.[Zip_RVFlag] WHEN 1 THEN a.[Zip] ELSE b.[Zip] END)			
		FROM tblCompanies a
		INNER JOIN tblCompanies b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempTargetEntitySite c
		ON c.EntityGuid = a.CompanyGuid
		INNER JOIN erv.tblTempTargetEntitySite d
		ON d.ParentEntityGuid = b.CompanyGuid
		INNER JOIN erv.tblTempCompanyRecordVersioningFlag e
		ON e.CompanyGuid = a._MasterRecordGuid
		WHERE e._CallingReferenceGuid = @callingRefGuid
		AND c._CallingReferenceGuid = @callingRefGuid
		AND d._CallingReferenceGuid = @callingRefGuid

		DELETE erv.tblTempCompanyRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRefGuid 

		-- Process those ParentSpecific External fields whose propagation require custom handling.
		DECLARE @tblParentSpecificExternalFields TABLE
		(
			TargetField nvarchar(100)
		)

		/*Process those ParentSpecific External fields whose propagation require custom handling. */
		--Equipments. 
		--The relationship between tblCompanies and tblEquipment is maintained fully on the tblEquipment side, which references the Company using the Company MasterRecordGuid. 
		--Therefore the changes made to the Equipments assignments of a Company record will be propagated only according to the FLC configuration on the associated equipment records.
		--There are no record versioning propagation actions to be taken for the Company child record versions as far as the tblCompanies-tblEquipment relationships are concerned.

		-- Process [AuthorizedShipTo] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedShipTo') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d				
				WHERE d.CompanyGuid = b.ParentEntityGuid
				AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Id = d.Id,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
			ON d.CompanyGuid = b.ParentEntityGuid
			AND d.AssignedToCompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
			(AssignedToCompanyGuid, CompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid), 
			 b.EntityGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.CompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
				WHERE d.CompanyGuid = b.EntityGuid
				AND d.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.AssignedToCompanyGuid)
			)	
			AND b._CallingReferenceGuid = @callingRefGuid						
		END

		--Drivers.
		--The relationship between tblCompanies and tblPersonnel is maintained fully on the tblPersonnel side, which references the Company using the Company MasterRecordGuid. 
		--Therefore the changes made to the Personnels assignments of a Company record will be propagated only according to the FLC configuration on the associated personnel records.
		--There are no record versioning propagation actions to be taken for the Company child record versions as far as the tblCompanies-tblPersonnel relationships are concerned.

		-- Process [UnavailableInventories] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') = 0)
		BEGIN
			--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.ProductGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Product or by a Product child record version whose UnavailableInventories field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d				
				WHERE d.AssignedToCompanyGuid = b.ParentEntityGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
			ON d.AssignedToCompanyGuid = b.ParentEntityGuid
			AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
				WHERE d.AssignedToCompanyGuid = b.EntityGuid
				AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [ShipToAuthorizedProducts] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'ShipToAuthorizedProducts') = 0)
		BEGIN
			--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.ProductGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Product or by a Product child record version whose AuthorizedCustomers field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompany] d				
				WHERE d.AssignedToCompanyGuid = b.ParentEntityGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.SpecialInstructionNote = d.SpecialInstructionNote,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.AssignedToCompanyGuid = b.ParentEntityGuid
			AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
													
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompany] d
				WHERE d.AssignedToCompanyGuid = b.EntityGuid
				AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [AuthorizedCarriers] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCarriers') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d				
				WHERE d.AssignedToCompanyGuid = b.ParentEntityGuid
				AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
			
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Id = d.Id,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
			ON d.AssignedToCompanyGuid = b.ParentEntityGuid
			AND d.CompanyGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
			
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
			(CompanyGuid, AssignedToCompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid), 
			 b.EntityGuid, b.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
				WHERE d.AssignedToCompanyGuid = b.EntityGuid
				AND d.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, b.SiteGuid), a.CompanyGuid)
			)	
			AND b._CallingReferenceGuid = @callingRefGuid						
		END

		-- Process [SupplierAuthorizedProducts] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') = 0)
		BEGIN
			--Only delete the child record version Product mappings that are not supported anymore in the parent Company and that are not tied to a local Product or a Product child record version whose mappings to Company is VersionSpecific (so that the local Product or the Product child record version does not loose its Company mappings when Company RecordVersioning is turned off).
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.ProductGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Product or by a Product child record version whose SupplierAuthorizedProducts field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d				
				WHERE d.AssignedToCompanyGuid = b.ParentEntityGuid
				AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid	
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.BlendPercentage = d.BlendPercentage,
			a.AdditiveRate = d.AdditiveRate,
			a.Ratio = d.Ratio,
			a.AdditiveCycleVolume = d.AdditiveCycleVolume,
			a.Tolerance = d.Tolerance,		
			a.PresetNumber = d.PresetNumber,
			a.AdditiveProfileGuid = d.AdditiveProfileGuid,
			a.TankGuid = d.TankGuid,
			a.MeterID = d.MeterID,
			a.ShipToProductID = d.ShipToProductID,
			a.ShipToProductCode = d.ShipToProductCode,
			a.ShipToLoadRackDisplayText = d.ShipToLoadRackDisplayText,
			a.UnavailableInventoryGross = d.UnavailableInventoryGross,
			a.UnavailableInventoryNet = d.UnavailableInventoryNet,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToSupplierProductCompany] d
			ON d.AssignedToCompanyGuid = b.ParentEntityGuid
			AND d.ProductGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblProductToSupplierProductCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid), 
			 b.EntityGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d
				WHERE d.AssignedToCompanyGuid = b.EntityGuid
				AND d.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, b.SiteGuid), a.ProductGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [AccessSchedule] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AccessSchedule') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblScheduleCompanyAccess] d				
				WHERE d.CompanyGuid = b.ParentEntityGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Enabled = d.Enabled, 
			a.OpeningTime = d.OpeningTime,
			a.ClosingTime = d.ClosingTime,
			a.EndOfDayEnabled = d.EndOfDayEnabled,
			a.EndOfDayTime = d.EndOfDayTime,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			INNER JOIN [dbo].[tblScheduleCompanyAccess] d
			ON d.CompanyGuid = b.ParentEntityGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [dbo].[tblScheduleCompanyAccess]
			(CompanyGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.LookupDayOfWeekIndex, 
			 a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.CompanyGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [dbo].[tblScheduleCompanyAccess] d
				WHERE d.CompanyGuid = b.EntityGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [CertificatesAndPermits] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'CertificatesAndPermits') = 0)
		BEGIN
			-- Delete the child mappings not supported anymore by the parent
			DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			WHERE NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d				
				WHERE d.CompanyGuid = b.ParentEntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
													
			-- Update the active (non-historical) attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Sequence = d.Sequence, 
			a.DateCompleted = d.DateCompleted,
			a.DateDue = d.DateDue,
			a.ExpirationDate = d.ExpirationDate,
			a.id = d.id,		
			a.Instructor = d.Instructor,
			a.Rating = d.Rating,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			INNER JOIN [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
			ON d.CompanyGuid = b.ParentEntityGuid
			AND d.QualificationGuid = a.QualificationGuid
			AND d.HistoricalRecord = a.HistoricalRecord			
			AND d.HistoricalRecord = 0
			WHERE b._CallingReferenceGuid = @callingRefGuid

			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblQualificationCompanyCertificateAndPermitToCompany]
			(CompanyGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT b.EntityGuid, a.QualificationGuid
			 ,a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.CompanyGuid
			INNER JOIN tblQualifications c
			ON c.QualificationGuid = a.QualificationGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
				WHERE d.CompanyGuid = b.EntityGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END

		-- Process [UserGroups] External Field
		-- UserGroup is both an External Attribute of Company (i.e. Company-To-UserGroup mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-UserGroup mappings are also maintained as part of the UserGroup entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UserGroups') = 0)
		BEGIN
			-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NOT NULL
			INSERT INTO [map].[tblCompanyCompanyToUserGroup]
				(CompanyGuid, GroupGuid, SiteGuid, ID)
			SELECT DISTINCT a.CompanyGuid, a.GroupGuid, b.SiteGuid, '' FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.MasterRecordGuid = a.CompanyGuid
			INNER JOIN [map].[tblEntityUserGroupToSite] c
			ON c.GroupGuid = a.GroupGuid
			WHERE NOT EXISTS(SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] d WHERE d.CompanyGuid IS NOT NULL AND a.CompanyGuid = d.CompanyGuid AND a.GroupGuid = d.GroupGuid AND d.SiteGuid = b.SiteGuid)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Insert a new Company-UserGroup child record mapping for each child site in the Entity To Site hierarchy where CompanyGuid is NULL
			INSERT INTO [map].[tblCompanyCompanyToUserGroup]
				(CompanyGuid, GroupGuid, SiteGuid, ID)
			SELECT DISTINCT a.CompanyGuid, a.GroupGuid, b.SiteGuid, '' FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN [map].[tblEntityUserGroupToSite] b
			ON b.GroupGuid = a.GroupGuid
			INNER JOIN erv.tblTempTargetEntitySite c
			ON c.SiteGuid = b.SiteGuid
			WHERE a.CompanyGuid IS NULL
			AND NOT EXISTS(SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] d WHERE d.CompanyGuid IS NULL AND d.GroupGuid = a.GroupGuid AND d.SiteGuid = b.SiteGuid)
			AND c._CallingReferenceGuid = @callingRefGuid

			--Delete all the Company-to-UserGroup mappings of the Company child record version that are not present in the Assigned From Site 	
			DELETE a FROM [map].[tblCompanyCompanyToUserGroup] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.MasterRecordGuid = a.CompanyGuid AND b.SiteGuid = a.SiteGuid
			INNER JOIN [map].[tblEntityCompanyToSite] c
			ON c.CompanyGuid = a.CompanyGuid AND c.SiteGuid = b.SiteGuid
			WHERE NOT EXISTS(SELECT 1 FROM [map].[tblCompanyCompanyToUserGroup] d WHERE d.CompanyGuid = a.CompanyGuid AND d.GroupGuid = a.GroupGuid AND d.SiteGuid = c.AssignedFromSiteGuid)
			AND b._CallingReferenceGuid = @callingRefGuid
		END


		-- Process [CompanyRoles] External Field
		-- Company Roles are created and deleted independently of Record Versioning. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.

		-- Process [Drivers] External Field
		-- Personnel is both an External Attribute of Company (i.e. Company-To-Personnel mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Personnel mappings are also maintained as part of the Personnel entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceVersionSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'Drivers') = 0)
		BEGIN
			--Only delete the child record version Personnel mappings that are not supported anymore in the parent Company and that are not tied to a local Personnel or a Personnel child record version whose mappings to Company is VersionSpecific (so that the local Personnel or the Personnel child record version does not loose its Company mappings when Company RecordVersioning is turned off).
			DELETE a FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
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
			WHERE c.SiteGuid = b.SiteGuid
			AND NOT ((c.PersonnelGuid = c._MasterRecordGuid) OR (ISNULL(e.ForwardControlMode, '') = 'Versionspecific')) --Exclude the mappings that are either owned by a local Personnel or by a Personnel child record version whose Carrier field is set as VersionSpecific.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d				
				WHERE d.CompanyGuid = b.ParentEntityGuid
				AND d.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, @SourceSiteGroupGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid

			-- Update the attributes of child mappings that also exist at the parent		
			UPDATE a
			SET a.Id = d.Id,
			a.UpdatedDate = GETDATE(),
			a.UpdatedBy = d.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.EntityGuid = a.CompanyGuid
			INNER JOIN tblPersonnel c
			ON c.PersonnelGuid = a.PersonnelGuid
			INNER JOIN [map].[tblCompanyPersonnelAssignedToCompany] d
			ON d.CompanyGuid = b.ParentEntityGuid
			AND d.PersonnelGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, @SourceSiteGroupGuid)
			WHERE b._CallingReferenceGuid = @callingRefGuid
													
			-- Create new child mappings for those new parent mappings not found at the child
			INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
			(PersonnelGuid, CompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid), 
			 b.EntityGuid, b.SiteGuid, a.ID, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyPersonnelAssignedToCompany] a
			INNER JOIN erv.tblTempTargetEntitySite b
			ON b.ParentEntityGuid = a.CompanyGuid
			INNER JOIN tblPersonnel c
			ON c.PersonnelGuid = a.PersonnelGuid
			WHERE NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany] d
				WHERE d.CompanyGuid = b.EntityGuid
				AND d.PersonnelGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Personnel', c._MasterRecordGuid, b.SiteGuid), a.PersonnelGuid)
			)
			AND b._CallingReferenceGuid = @callingRefGuid
		END


		DELETE erv.tblTempTargetEntitySite
		WHERE _CallingReferenceGuid = @callingRefGuid	

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
						+ 'Procedure Name: [erv].usp_PropagateCompanyRecordVersionBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
