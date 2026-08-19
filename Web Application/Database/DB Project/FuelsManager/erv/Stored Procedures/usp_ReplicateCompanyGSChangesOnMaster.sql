/*
	DROP PROCEDURE [erv].[usp_ReplicateCompanyGSChangesOnMaster]

	EXEC [erv].[usp_ReplicateCompanyGSChangesOnMaster] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7'
	EXEC [erv].[usp_ReplicateCompanyGSChangesOnMaster] 'F94D0DAB-8C85-4A73-830E-A8168078B6AD'
	EXEC [erv].[usp_ReplicateCompanyGSChangesOnMaster] '1bb8c558-5277-47a5-90ae-2461bbd1eff7'
	EXEC [erv].[usp_ReplicateCompanyGSChangesOnMaster] '64A800A7-67FC-4950-A22D-15863AD475FA'

*/

CREATE PROCEDURE [erv].[usp_ReplicateCompanyGSChangesOnMaster]
(
	@SourceCompanyGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_ReplicateCompanyGSChangesOnMaster] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Replicate the Global Specific field values of a Company child record version onto the Master Record copy.
	--          By replicating those field values onto the master record, we ensure that when the non-VersionSpecific
	--          fields of the master record are propagated down the site hierarchy, that all the GlobalSpecific changes made onto the
	--          the child record version will get propagated onto all the sitegroups and sites where the master record is assigned.
	-- Notes:
	-- 1. @SourceCompanyGuid: Guid of the Company child record version record whose GlobalSpecific fields needs to be replicated to its local Master Record copy 
	--    (and not the parent record of the entity record).
	-- 2. Whereas RecordVersioning propagation is limited to child record versions, the GlobalSpecific field replication targets the master records and allows
	--    modifications to the master records. This also applies to external attributres that represent a reference to another RecordVersioning entity (e.g. Product).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)
		DECLARE @EntityTypeId nvarchar(100)
		SET @EntityTypeId = 'Company'

		DECLARE @masterSiteGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @masterRecordGuid uniqueidentifier
		DECLARE @assignedFromSiteGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid FROM dbo.tblCompanies
		WHERE CompanyGuid = @SourceCompanyGuid
		AND CompanyGuid <> _MasterRecordGuid

		IF (@masterRecordGuid IS NULL)
		BEGIN
			RAISERROR('Cannot locate the source child record for data replication.',16,1); 
			RETURN;
		END

		IF ((SELECT COUNT(*) FROM dbo.tblCompanies WHERE CompanyGuid = @masterRecordGuid AND _MasterRecordGuid = @masterRecordGuid) = 0)
		BEGIN
			RAISERROR('Cannot locate the target master record for data replication.',16,1); 
			RETURN;
		END

		SELECT @masterSiteGuid = SiteGuid FROM dbo.tblCompanies
		WHERE CompanyGuid = @masterRecordGuid
		AND CompanyGuid = _MasterRecordGuid

		SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityCompanyToSite 
		WHERE CompanyGuid = @masterRecordGuid 
		AND SiteGuid = @ownerSiteGuid


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
		

		--Retrieve the GlobalSpecific fields for the AssignedFrom sitegroup of the child record version whose changes need to be replicated
		DECLARE @tblSourceGlobalSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)
		DECLARE @callingRef2Guid uniqueidentifier
		SET @callingRef2Guid = NEWID()

		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, 'GlobalSpecific', @callingRef2Guid 

		INSERT @tblSourceGlobalSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid

		IF (NOT EXISTS (SELECT * FROM @tblSourceGlobalSpecificFields))
		BEGIN				
			/*	No GlobalSpecific fields to update.	*/
			RETURN;
		END

		--Build a table that has one flag column for each column of the tblCompanies table, and set the flag according to whether the field is GlobalSpecific or not.
		DECLARE @callingRef1Guid uniqueidentifier
		SET @callingRef1Guid = NEWID()

		INSERT INTO erv.tblTempCompanyRecordVersioningFlag
		(CompanyGuid, SiteGuid, _CallingReferenceGuid)
		SELECT a.CompanyGuid, a.SiteGuid, @callingRef1Guid FROM tblCompanies a
		WHERE a._MasterRecordGuid = @masterRecordGuid
		AND a.CompanyGuid = a._MasterRecordGuid

		EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] @EntityTypeId, @masterRecordGuid, @assignedFromSiteGuid, @callingRef2Guid, @callingRef1Guid

		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef2Guid
		

		DECLARE @BeginTran BIT = 0 
		IF (@@TRANCOUNT = 0)   
        BEGIN  
            BEGIN TRANSACTION --ReplicateToMasterRecord
            SET @BeginTran = 1   
		END  	
				

		-- Update all the GlobalSpecific fields of the Master record version
		UPDATE a
		SET	a.[AccountNumber] = (CASE d.[AccountNumber_RVFlag] WHEN 1 THEN b.[AccountNumber] ELSE a.[AccountNumber] END),
			a.[AdditiveAccounting] = (CASE d.[AdditiveAccounting_RVFlag] WHEN 1 THEN b.[AdditiveAccounting] ELSE a.[AdditiveAccounting] END),
			a.[Address1] = (CASE d.[Address1_RVFlag] WHEN 1 THEN b.[Address1] ELSE a.[Address1] END),
			a.[Address2] = (CASE d.[Address2_RVFlag] WHEN 1 THEN b.[Address2] ELSE a.[Address2] END),
			a.[AllowDriverEntry] = (CASE d.[AllowDriverEntry_RVFlag] WHEN 1 THEN b.[AllowDriverEntry] ELSE a.[AllowDriverEntry] END),
			a.[City] = (CASE d.[City_RVFlag] WHEN 1 THEN b.[City] ELSE a.[City] END),
			a.[Code] = (CASE d.[Code_RVFlag] WHEN 1 THEN b.[Code] ELSE a.[Code] END),
			a.[ConsortiumTypeIndex] = (CASE d.[ConsortiumTypeIndex_RVFlag] WHEN 1 THEN b.[ConsortiumTypeIndex] ELSE a.[ConsortiumTypeIndex] END),
			a.[Contact1Address1] = (CASE d.[Contact1Address1_RVFlag] WHEN 1 THEN b.[Contact1Address1] ELSE a.[Contact1Address1] END),
			a.[Contact1Address2] = (CASE d.[Contact1Address2_RVFlag] WHEN 1 THEN b.[Contact1Address2] ELSE a.[Contact1Address2] END),
			a.[Contact1City] = (CASE d.[Contact1City_RVFlag] WHEN 1 THEN b.[Contact1City] ELSE a.[Contact1City] END),
			a.[Contact1Country] = (CASE d.[Contact1Country_RVFlag] WHEN 1 THEN b.[Contact1Country] ELSE a.[Contact1Country] END),
			a.[Contact1EmailAddress] = (CASE d.[Contact1EmailAddress_RVFlag] WHEN 1 THEN b.[Contact1EmailAddress] ELSE a.[Contact1EmailAddress] END),
			a.[Contact1Fax] = (CASE d.[Contact1Fax_RVFlag] WHEN 1 THEN b.[Contact1Fax] ELSE a.[Contact1Fax] END),
			a.[Contact1Name] = (CASE d.[Contact1Name_RVFlag] WHEN 1 THEN b.[Contact1Name] ELSE a.[Contact1Name] END),
			a.[Contact1PhoneMobile] = (CASE d.[Contact1PhoneMobile_RVFlag] WHEN 1 THEN b.[Contact1PhoneMobile] ELSE a.[Contact1PhoneMobile] END),
			a.[Contact1PhoneOffice] = (CASE d.[Contact1PhoneOffice_RVFlag] WHEN 1 THEN b.[Contact1PhoneOffice] ELSE a.[Contact1PhoneOffice] END),
			a.[Contact1State] = (CASE d.[Contact1State_RVFlag] WHEN 1 THEN b.[Contact1State] ELSE a.[Contact1State] END),
			a.[Contact1Zip] = (CASE d.[Contact1Zip_RVFlag] WHEN 1 THEN b.[Contact1Zip] ELSE a.[Contact1Zip] END),
			a.[Contact2Address1] = (CASE d.[Contact2Address1_RVFlag] WHEN 1 THEN b.[Contact2Address1] ELSE a.[Contact2Address1] END),
			a.[Contact2Address2] = (CASE d.[Contact2Address2_RVFlag] WHEN 1 THEN b.[Contact2Address2] ELSE a.[Contact2Address2] END),
			a.[Contact2City] = (CASE d.[Contact2City_RVFlag] WHEN 1 THEN b.[Contact2City] ELSE a.[Contact2City] END),
			a.[Contact2Country] = (CASE d.[Contact2Country_RVFlag] WHEN 1 THEN b.[Contact2Country] ELSE a.[Contact2Country] END),
			a.[Contact2EmailAddress] = (CASE d.[Contact2EmailAddress_RVFlag] WHEN 1 THEN b.[Contact2EmailAddress] ELSE a.[Contact2EmailAddress] END),
			a.[Contact2Fax] = (CASE d.[Contact2Fax_RVFlag] WHEN 1 THEN b.[Contact2Fax] ELSE a.[Contact2Fax] END),
			a.[Contact2Name] = (CASE d.[Contact2Name_RVFlag] WHEN 1 THEN b.[Contact2Name] ELSE a.[Contact2Name] END),
			a.[Contact2PhoneMobile] = (CASE d.[Contact2PhoneMobile_RVFlag] WHEN 1 THEN b.[Contact2PhoneMobile] ELSE a.[Contact2PhoneMobile] END),
			a.[Contact2PhoneOffice] = (CASE d.[Contact2PhoneOffice_RVFlag] WHEN 1 THEN b.[Contact2PhoneOffice] ELSE a.[Contact2PhoneOffice] END),
			a.[Contact2State] = (CASE d.[Contact2State_RVFlag] WHEN 1 THEN b.[Contact2State] ELSE a.[Contact2State] END),
			a.[Contact2Zip] = (CASE d.[Contact2Zip_RVFlag] WHEN 1 THEN b.[Contact2Zip] ELSE a.[Contact2Zip] END),
			a.[Country] = (CASE d.[Country_RVFlag] WHEN 1 THEN b.[Country] ELSE a.[Country] END),
			a.[CreditOK] = (CASE d.[CreditOK_RVFlag] WHEN 1 THEN b.[CreditOK] ELSE a.[CreditOK] END),
			a.[CustomerBillToTypeApplicationStringGuid] = (CASE d.[CustomerBillToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN b.[CustomerBillToTypeApplicationStringGuid] ELSE a.[CustomerBillToTypeApplicationStringGuid] END),
			a.[CustomerShipToTypeApplicationStringGuid] = (CASE d.[CustomerShipToTypeApplicationStringGuid_RVFlag] WHEN 1 THEN b.[CustomerShipToTypeApplicationStringGuid] ELSE a.[CustomerShipToTypeApplicationStringGuid] END),
			a.[DeliveryToTerminalPermitted] = (CASE d.[DeliveryToTerminalPermitted_RVFlag] WHEN 1 THEN b.[DeliveryToTerminalPermitted] ELSE a.[DeliveryToTerminalPermitted] END),
			a.[DisableBillToAllocationsCheck] = (CASE d.[DisableBillToAllocationsCheck_RVFlag] WHEN 1 THEN b.[DisableBillToAllocationsCheck] ELSE a.[DisableBillToAllocationsCheck] END),
			a.[DisableOwnerAllocationsCheck] = (CASE d.[DisableOwnerAllocationsCheck_RVFlag] WHEN 1 THEN b.[DisableOwnerAllocationsCheck] ELSE a.[DisableOwnerAllocationsCheck] END),
			a.[DisableShipperAllocationsCheck] = (CASE d.[DisableShipperAllocationsCheck_RVFlag] WHEN 1 THEN b.[DisableShipperAllocationsCheck] ELSE a.[DisableShipperAllocationsCheck] END),
			a.[DisableShipToAllocationsCheck] = (CASE d.[DisableShipToAllocationsCheck_RVFlag] WHEN 1 THEN b.[DisableShipToAllocationsCheck] ELSE a.[DisableShipToAllocationsCheck] END),
			a.[EffectiveDate] = (CASE d.[EffectiveDate_RVFlag] WHEN 1 THEN b.[EffectiveDate] ELSE a.[EffectiveDate] END),
			a.[EmergencyContact] = (CASE d.[EmergencyContact_RVFlag] WHEN 1 THEN b.[EmergencyContact] ELSE a.[EmergencyContact] END),
			a.[EmergencyPhone] = (CASE d.[EmergencyPhone_RVFlag] WHEN 1 THEN b.[EmergencyPhone] ELSE a.[EmergencyPhone] END),
			a.[EPANumber] = (CASE d.[EPANumber_RVFlag] WHEN 1 THEN b.[EPANumber] ELSE a.[EPANumber] END),
			a.[ExpirationDate] = (CASE d.[ExpirationDate_RVFlag] WHEN 1 THEN b.[ExpirationDate] ELSE a.[ExpirationDate] END),
			a.[FAX] = (CASE d.[FAX_RVFlag] WHEN 1 THEN b.[FAX] ELSE a.[FAX] END),
			a.[FederalID] = (CASE d.[FederalID_RVFlag] WHEN 1 THEN b.[FederalID] ELSE a.[FederalID] END),
			a.[FederalID2] = (CASE d.[FederalID2_RVFlag] WHEN 1 THEN b.[FederalID2] ELSE a.[FederalID2] END),
			a.[FederalID3] = (CASE d.[FederalID3_RVFlag] WHEN 1 THEN b.[FederalID3] ELSE a.[FederalID3] END),
			a.[FederalID4] = (CASE d.[FederalID4_RVFlag] WHEN 1 THEN b.[FederalID4] ELSE a.[FederalID4] END),
			a.[FederalID5] = (CASE d.[FederalID5_RVFlag] WHEN 1 THEN b.[FederalID5] ELSE a.[FederalID5] END),
			a.[FlightPrefix] = (CASE d.[FlightPrefix_RVFlag] WHEN 1 THEN b.[FlightPrefix] ELSE a.[FlightPrefix] END),
			a.[FlushPermitted] = (CASE d.[FlushPermitted_RVFlag] WHEN 1 THEN b.[FlushPermitted] ELSE a.[FlushPermitted] END),
			a.[HazardousMaterialExclusion] = (CASE d.[HazardousMaterialExclusion_RVFlag] WHEN 1 THEN b.[HazardousMaterialExclusion] ELSE a.[HazardousMaterialExclusion] END),
			a.[IATAGuid] = (CASE d.[IATAGuid_RVFlag] WHEN 1 THEN b.[IATAGuid] ELSE a.[IATAGuid] END),
			a.[ID] = (CASE d.[ID_RVFlag] WHEN 1 THEN b.[ID] ELSE a.[ID] END),
			a.[InsuranceCompany] = (CASE d.[InsuranceCompany_RVFlag] WHEN 1 THEN b.[InsuranceCompany] ELSE a.[InsuranceCompany] END),
			a.[InsuranceExpiration] = (CASE d.[InsuranceExpiration_RVFlag] WHEN 1 THEN b.[InsuranceExpiration] ELSE a.[InsuranceExpiration] END),
			a.[InsurancePolicy] = (CASE d.[InsurancePolicy_RVFlag] WHEN 1 THEN b.[InsurancePolicy] ELSE a.[InsurancePolicy] END),
			a.[LastActivityDate] = (CASE d.[LastActivityDate_RVFlag] WHEN 1 THEN b.[LastActivityDate] ELSE a.[LastActivityDate] END),
			a.[LiabilityAmount] = (CASE d.[LiabilityAmount_RVFlag] WHEN 1 THEN b.[LiabilityAmount] ELSE a.[LiabilityAmount] END),
			a.[LicenseExpiration] = (CASE d.[LicenseExpiration_RVFlag] WHEN 1 THEN b.[LicenseExpiration] ELSE a.[LicenseExpiration] END),
			a.[LicenseNumber] = (CASE d.[LicenseNumber_RVFlag] WHEN 1 THEN b.[LicenseNumber] ELSE a.[LicenseNumber] END),
			a.[LoadRackDisplayText] = (CASE d.[LoadRackDisplayText_RVFlag] WHEN 1 THEN b.[LoadRackDisplayText] ELSE a.[LoadRackDisplayText] END),
			a.[LockedOut] = (CASE d.[LockedOut_RVFlag] WHEN 1 THEN b.[LockedOut] ELSE a.[LockedOut] END),
			a.[LockedOutDate] = (CASE d.[LockedOutDate_RVFlag] WHEN 1 THEN b.[LockedOutDate] ELSE a.[LockedOutDate] END),
			a.[LockedOutReason] = (CASE d.[LockedOutReason_RVFlag] WHEN 1 THEN b.[LockedOutReason] ELSE a.[LockedOutReason] END),
			a.[LowStockWarning] = (CASE d.[LowStockWarning_RVFlag] WHEN 1 THEN b.[LowStockWarning] ELSE a.[LowStockWarning] END),
			a.[MaximumVehicleWeight] = (CASE d.[MaximumVehicleWeight_RVFlag] WHEN 1 THEN b.[MaximumVehicleWeight] ELSE a.[MaximumVehicleWeight] END),
			a.[Name] = (CASE d.[Name_RVFlag] WHEN 1 THEN b.[Name] ELSE a.[Name] END),
			a.[Note] = (CASE d.[Note_RVFlag] WHEN 1 THEN b.[Note] ELSE a.[Note] END),
			a.[OnHold] = (CASE d.[OnHold_RVFlag] WHEN 1 THEN b.[OnHold] ELSE a.[OnHold] END),
			a.[Phone] = (CASE d.[Phone_RVFlag] WHEN 1 THEN b.[Phone] ELSE a.[Phone] END),
			a.[PickupFLights] = (CASE d.[PickupFLights_RVFlag] WHEN 1 THEN b.[PickupFLights] ELSE a.[PickupFLights] END),
			a.[PINRequired] = (CASE d.[PINRequired_RVFlag] WHEN 1 THEN b.[PINRequired] ELSE a.[PINRequired] END),
			a.[PumpOffPermitted] = (CASE d.[PumpOffPermitted_RVFlag] WHEN 1 THEN b.[PumpOffPermitted] ELSE a.[PumpOffPermitted] END),
			a.[PurchaseOrderRequired] = (CASE d.[PurchaseOrderRequired_RVFlag] WHEN 1 THEN b.[PurchaseOrderRequired] ELSE a.[PurchaseOrderRequired] END),
			a.[ReceivableAccount] = (CASE d.[ReceivableAccount_RVFlag] WHEN 1 THEN b.[ReceivableAccount] ELSE a.[ReceivableAccount] END),
			a.[RefinerCode] = (CASE d.[RefinerCode_RVFlag] WHEN 1 THEN b.[RefinerCode] ELSE a.[RefinerCode] END),
			a.[SCACCode] = (CASE d.[SCACCode_RVFlag] WHEN 1 THEN b.[SCACCode] ELSE a.[SCACCode] END),
			a.[ShipperTypeApplicationStringGuid] = (CASE d.[ShipperTypeApplicationStringGuid_RVFlag] WHEN 1 THEN b.[ShipperTypeApplicationStringGuid] ELSE a.[ShipperTypeApplicationStringGuid] END),
			a.[ShortName] = (CASE d.[ShortName_RVFlag] WHEN 1 THEN b.[ShortName] ELSE a.[ShortName] END),
			a.[State] = (CASE d.[State_RVFlag] WHEN 1 THEN b.[State] ELSE a.[State] END),
			a.[StateID] = (CASE d.[StateId_RVFlag] WHEN 1 THEN b.[StateID] ELSE a.[StateID] END),
			a.[StockTrack] = (CASE d.[StockTrack_RVFlag] WHEN 1 THEN b.[StockTrack] ELSE a.[StockTrack] END),
			a.[SufferLossGain] = (CASE d.[SufferLossGain_RVFlag] WHEN 1 THEN b.[SufferLossGain] ELSE a.[SufferLossGain] END),
			a.[TaxNumber] = (CASE d.[TaxNumber_RVFlag] WHEN 1 THEN b.[TaxNumber] ELSE a.[TaxNumber] END),
			a.[UpdatedBy] = b.[UpdatedBy],
			a.[UpdatedDate] = SYSDATETIMEOFFSET(),
			a.[UserData1] = (CASE d.[UserData1_RVFlag] WHEN 1 THEN b.[UserData1] ELSE a.[UserData1] END),
			a.[UserData2] = (CASE d.[UserData2_RVFlag] WHEN 1 THEN b.[UserData2] ELSE a.[UserData2] END),
			a.[UserData3] = (CASE d.[UserData3_RVFlag] WHEN 1 THEN b.[UserData3] ELSE a.[UserData3] END),
			a.[UserData4] = (CASE d.[UserData4_RVFlag] WHEN 1 THEN b.[UserData4] ELSE a.[UserData4] END),
			a.[UserData5] = (CASE d.[UserData5_RVFlag] WHEN 1 THEN b.[UserData5] ELSE a.[UserData5] END),
			a.[UserData6] = (CASE d.[UserData6_RVFlag] WHEN 1 THEN b.[UserData6] ELSE a.[UserData6] END),
			a.[UserData7] = (CASE d.[UserData7_RVFlag] WHEN 1 THEN b.[UserData7] ELSE a.[UserData7] END),
			a.[UserData8] = (CASE d.[UserData8_RVFlag] WHEN 1 THEN b.[UserData8] ELSE a.[UserData8] END),
			a.[WeightUnits] = (CASE d.[WeightUnits_RVFlag] WHEN 1 THEN b.[WeightUnits] ELSE a.[WeightUnits] END),
			a.[Zip] = (CASE d.[Zip_RVFlag] WHEN 1 THEN b.[Zip] ELSE a.[Zip] END),
			a.[HiddenDate] = (CASE d.[HiddenDate_RVFlag] WHEN 1 THEN b.[HiddenDate] ELSE a.[HiddenDate] END)
		FROM tblCompanies a
		INNER JOIN tblCompanies b
		ON b._MasterRecordGuid = a._MasterRecordGuid
		INNER JOIN erv.tblTempCompanyRecordVersioningFlag d
		ON d.CompanyGuid = a.CompanyGuid
		WHERE b.CompanyGuid = @SourceCompanyGuid
		AND d._CallingReferenceGuid = @callingRef1Guid
		AND a.CompanyGuid = a._MasterRecordGuid


		DELETE erv.tblTempCompanyRecordVersioningFlag 
		WHERE _CallingReferenceGuid = @callingRef1Guid 


		
		/*Process those GlobalSpecific External fields whose replication require custom handling. */			

		--Equipments. 
		--The relationship between tblCompanies and tblEquipment is maintained fully on the tblEquipment side, which references the Company using the Company MasterRecordGuid. 
		--Therefore the changes made to the Equipments assignments of a Company record will be propagated only according to the FLC configuration on the associated equipment records.
		--There are no record versioning propagation actions to be taken for the Company child record versions as far as the tblCompanies-tblEquipment relationships are concerned.

		-- Process [AuthorizedShipTo] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedShipTo') > 0)
		BEGIN
			--Delete the master record version Product mappings that are not supported anymore in the child Company record 	
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN [dbo].tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
				INNER JOIN [dbo].tblCompanies e
				ON e.CompanyGuid = d.CompanyGuid
				INNER JOIN [dbo].tblCompanies f
				ON f.CompanyGuid = d.AssignedToCompanyGuid				
				WHERE d.CompanyGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND c._MasterRecordGuid = f._MasterRecordGuid				
			)			
													
			--Update the master record version mappings that have been modified in the child Company record
			UPDATE d
			SET d.Id = a.Id, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN [dbo].[tblcompanies] b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN dbo.tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
			ON d.CompanyGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblCompanies e
			ON e.CompanyGuid = d.AssignedToCompanyGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND d.SiteGuid = @masterSiteGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
			 (AssignedToCompanyGuid, CompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @masterSiteGuid), c._MasterRecordGuid), 
			 b._MasterRecordGuid, d.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies d
			ON d.CompanyGuid = b._MasterRecordGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] e
				INNER JOIN dbo.tblCompanies f
				ON f.CompanyGuid = e.AssignedToCompanyGuid
				WHERE e.CompanyGuid = b._MasterRecordGuid
				AND f._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		--Drivers.
		--The relationship between tblCompanies and tblPersonnel is maintained fully on the tblPersonnel side, which references the Company using the Company MasterRecordGuid. 
		--Therefore the changes made to the Personnels assignments of a Company record will be propagated only according to the FLC configuration on the associated personnel records.
		--There are no record versioning propagation actions to be taken for the Company child record versions as far as the tblCompanies-tblPersonnel relationships are concerned.

		-- Process [UnavailableInventories] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'UnavailableInventories') > 0)
		BEGIN
			--Delete the master record version Product mappings that are not supported anymore in the child Company record 
			DELETE a FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN [dbo].tblCompanies b
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
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND ((c.ProductGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Product child record version whose UnavailableInventories field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Product Guid = Product MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.					
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] f
				INNER JOIN [dbo].tblCompanies g
				ON g.CompanyGuid = f.AssignedToCompanyGuid
				INNER JOIN [dbo].tblProducts h
				ON h.ProductGuid = f.ProductGuid
				WHERE f.ProductGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)	
																				
			--Update the master record version mappings that have been modified in the child Company record
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
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN dbo.tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToUnavailableInventoryCompany] d
			ON d.AssignedToCompanyGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblProducts e
			ON e.ProductGuid = d.ProductGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid,
			 MeterID, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			 CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @masterSiteGuid), a.ProductGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid,
			 a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			 GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToUnavailableInventoryCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToUnavailableInventoryCompany] d
				INNER JOIN dbo.tblProducts e
				ON e.ProductGuid = d.ProductGuid
				WHERE d.AssignedToCompanyGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [ShipToAuthorizedProducts] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'ShipToAuthorizedProducts') > 0)
		BEGIN
			--Delete the master record version Product mappings that are not supported anymore in the child Company record 
			DELETE a FROM [map].[tblProductToCompany] a
			INNER JOIN [dbo].tblCompanies b
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
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND ((c.ProductGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Product child record version whose AuthorizedCustomers field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Product Guid = Product MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.					
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToCompany] f
				INNER JOIN [dbo].tblCompanies g
				ON g.CompanyGuid = f.AssignedToCompanyGuid
				INNER JOIN [dbo].tblProducts h
				ON h.ProductGuid = f.ProductGuid
				WHERE f.ProductGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)	
													
			--Update the master record version mappings that have been modified in the child Company record
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
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN dbo.tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.AssignedToCompanyGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblProducts e
			ON e.ProductGuid = d.ProductGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
			ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, 
			CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @masterSiteGuid), a.ProductGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToCompany] d
				INNER JOIN dbo.tblProducts e
				ON e.ProductGuid = d.ProductGuid
				WHERE d.AssignedToCompanyGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [AuthorizedCarriers] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AuthorizedCarriers') > 0)
		BEGIN	
			--Delete the master record version Company mappings that are not supported anymore in the child Company record 	
			DELETE a FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN [dbo].tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] d
				INNER JOIN [dbo].tblCompanies e
				ON e.CompanyGuid = d.AssignedToCompanyGuid
				INNER JOIN [dbo].tblCompanies f
				ON f.CompanyGuid = d.CompanyGuid				
				WHERE d.AssignedToCompanyGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND c._MasterRecordGuid = f._MasterRecordGuid				
			)	
													
			--Update the master record version mappings that have been modified in the child Company record
			UPDATE d
			SET d.Id = a.Id, 
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN dbo.tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN [map].[tblCompanyAuthorizedCarrierToCompany] d
			ON d.AssignedToCompanyGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblCompanies e
			ON e.CompanyGuid = d.CompanyGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
			AND d.SiteGuid = @masterSiteGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
			 (CompanyGuid, AssignedToCompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', c._MasterRecordGuid, @masterSiteGuid), c._MasterRecordGuid), 
			 b._MasterRecordGuid, d.SiteGuid, a.Id, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblCompanies c
			ON c.CompanyGuid = a.CompanyGuid
			INNER JOIN tblCompanies d
			ON d.CompanyGuid = b._MasterRecordGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] e
				INNER JOIN dbo.tblCompanies f
				ON f.CompanyGuid = e.CompanyGuid
				WHERE e.CompanyGuid = b._MasterRecordGuid
				AND f._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [SupplierAuthorizedProducts] External Field
		-- Product is both an External Attribute of Company (i.e. Company-To-Product mappings are maintained as part of the Company entity), and an External Client of Company (i.e. Company-To-Product mappings are also maintained as part of the Product entity, i.e. outside of the Company entity)
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'SupplierAuthorizedProducts') > 0)
		BEGIN
			--Delete the master record version Product mappings that are not supported anymore in the child Company record 
			DELETE a FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN [dbo].tblCompanies b
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
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND ((c.ProductGuid = c._MasterRecordGuid) OR (NOT (ISNULL(e.ForwardControlMode, '') = 'Versionspecific'))) --Exclude the mappings that are owned by a Product child record version whose SupplierAuthorizedProducts field is set as VersionSpecific. Note: Deletion of mappings owned by a local Product record (Product Guid = Product MasterRecordGuid) is permitted because by definition GlobalSpecific fields support the update of master records.
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] f
				INNER JOIN [dbo].tblCompanies g
				ON g.CompanyGuid = f.AssignedToCompanyGuid
				INNER JOIN [dbo].tblProducts h
				ON h.ProductGuid = f.ProductGuid
				WHERE f.ProductGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = g._MasterRecordGuid
				AND c._MasterRecordGuid = h._MasterRecordGuid				
			)
													
			--Update the master record version mappings that have been modified in the child Company record
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
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN dbo.tblProducts c
			ON c.ProductGuid = a.ProductGuid
			INNER JOIN [map].[tblProductToCompany] d
			ON d.AssignedToCompanyGuid = b._MasterRecordGuid
			INNER JOIN dbo.tblProducts e
			ON e.ProductGuid = d.ProductGuid
			AND e._MasterRecordGuid = c._MasterRecordGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblProductToSupplierProductCompany]
			(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, 
			ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet,
			CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Product', c._MasterRecordGuid, @masterSiteGuid), a.ProductGuid), 
			 b._MasterRecordGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, 
			a.MeterID, a.ShipToProductId, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblProductToSupplierProductCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.AssignedToCompanyGuid
			INNER JOIN tblProducts c
			ON c.ProductGuid = a.ProductGuid
			WHERE a.AssignedToCompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblProductToSupplierProductCompany] d
				INNER JOIN dbo.tblProducts e
				ON e.ProductGuid = d.ProductGuid
				WHERE d.AssignedToCompanyGuid = b._MasterRecordGuid
				AND e._MasterRecordGuid = c._MasterRecordGuid
			)
		END

		-- Process [AccessSchedule] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'AccessSchedule') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Company record
			DELETE a FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN [dbo].tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [dbo].[tblScheduleCompanyAccess] d
				INNER JOIN [dbo].tblCompanies e
				ON e.CompanyGuid = d.CompanyGuid			
				WHERE d.CompanyGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)			
													
			--Update the master record version mappings that have been modified in the child Company record
			UPDATE d
			SET d.Enabled = a.Enabled, 
			d.OpeningTime = a.OpeningTime,
			d.ClosingTime = a.ClosingTime,
			d.EndOfDayEnabled = a.EndOfDayEnabled,
			d.EndOfDayTime = a.EndOfDayTime,
			d.UpdatedDate = GETDATE(),
			d.UpdatedBy = a.UpdatedBy
			FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN [dbo].[tblScheduleCompanyAccess] d
			ON d.CompanyGuid = b._MasterRecordGuid
			AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			WHERE a.CompanyGuid = @SourceCompanyGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [dbo].[tblScheduleCompanyAccess]
			(CompanyGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, a.LookupDayOfWeekIndex, a.Enabled, a.OpeningTime, a.ClosingTime, a.EndOfDayEnabled, a.EndOfDayTime,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [dbo].[tblScheduleCompanyAccess] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [dbo].[tblScheduleCompanyAccess] d
				WHERE d.CompanyGuid = b._MasterRecordGuid
				AND d.LookupDayOfWeekIndex = a.LookupDayOfWeekIndex
			)
		END
		
		-- Process [CertificatesAndPermits] External Field
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'CertificatesAndPermits') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Company record
			DELETE a FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN [dbo].tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
				INNER JOIN [dbo].tblCompanies e
				ON e.CompanyGuid = d.CompanyGuid			
				WHERE d.CompanyGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)		
													
			--Update the master record version mappings that have been modified in the child Company record
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
			INNER JOIN [dbo].[tblCompanies] b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
			ON d.CompanyGuid = b._MasterRecordGuid
			AND d.QualificationGuid = a.QualificationGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid

			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblQualificationCompanyCertificateAndPermitToCompany]
			(CompanyGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)			
			 SELECT b._MasterRecordGuid, a.QualificationGuid, a.Sequence, a.Instructor, a.DateCompleted, a.DateDue, a.ExpirationDate, a.Id, a.Rating, a.HistoricalRecord,
			GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] d
				WHERE d.CompanyGuid = b._MasterRecordGuid
				AND d.QualificationGuid = a.QualificationGuid
			)
		END


		-- Process [CompanyRoles] External Field
		-- Company Roles are created and deleted independently of Record Versioning. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.
		IF ((SELECT COUNT(*) FROM @tblSourceGlobalSpecificFields WHERE IsExternalAttribute = 1 AND TargetField = 'CompanyRoles') > 0)
		BEGIN
			--Delete the master record version mappings that are not supported anymore in the child Company record
			DELETE a FROM [map].[tblCompanyToRole] a
			INNER JOIN [dbo].tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			WHERE b.CompanyGuid = @masterRecordGuid
			AND b.CompanyGuid = b._MasterRecordGuid
			AND NOT EXISTS 
			(
				SELECT * FROM [map].[tblCompanyToRole] d
				INNER JOIN [dbo].tblCompanies e
				ON e.CompanyGuid = d.CompanyGuid			
				WHERE d.CompanyGuid = @SourceCompanyGuid
				AND b._MasterRecordGuid = e._MasterRecordGuid
				AND d.LookupCompanyRoleIndex = a.LookupCompanyRoleIndex
			)	
													
			--No characteristics of the CompanyToRole mappings to update. The mappings are either inserted or deleted.
			
			--Insert a new mapping for each child record mapping not found in the master record
			INSERT INTO [map].[tblCompanyToRole]
			(CompanyGuid, LookupCompanyRoleIndex, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
			 SELECT b._MasterRecordGuid, a.LookupCompanyRoleIndex, d.SiteGuid, GETDATE(), a.CreatedBy, GETDATE(), a.UpdatedBy
			FROM [map].[tblCompanyToRole] a
			INNER JOIN dbo.tblCompanies b
			ON b.CompanyGuid = a.CompanyGuid
			INNER JOIN dbo.tblCompanies d
			ON d.CompanyGuid = b._MasterRecordGuid
			WHERE a.CompanyGuid = @SourceCompanyGuid
			AND NOT EXISTS
			(
				SELECT * FROM [map].[tblCompanyToRole] e
				WHERE e.CompanyGuid = b._MasterRecordGuid
				AND e.LookupCompanyRoleIndex = a.LookupCompanyRoleIndex
			)
		END

		IF ((@@TRANCOUNT > 0) AND (@BeginTran = 1))
		BEGIN
			COMMIT TRANSACTION --ReplicateToMasterRecord
		END
	END TRY
	BEGIN CATCH        
		IF ((@@TRANCOUNT > 0) AND (XACT_STATE() <> 0) AND (@BeginTran = 1))
			ROLLBACK TRANSACTION --ReplicateToMasterRecord
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
						+ 'Procedure Name: [erv].usp_ReplicateCompanyGSChangesOnMaster' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END