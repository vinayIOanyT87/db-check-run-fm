/*
	DROP PROCEDURE [erv].[usp_CreateCompanyChildRecordVersion]
	
	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreateCompanyChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreateCompanyChildRecordVersion] '012D8DD3-E6FA-4B78-A81A-C84F1C360558', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', @dt, 'HB'

	SELECT CompanyGuid, Id, _MasterRecordGuid, SiteGuid, * FROM tblCompany WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreateCompanyChildRecordVersion]
(
	@ParentEntityGuid uniqueidentifier, @TargetSiteIndex uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateCompanyChildRecordVersion] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Company record version for a target site/sitegroup, off a parent record version 
	-- Notes:
	-- 1. @ParentEntityGuid: Entity Guid of the record to be cloned.
	-- 2. @TargetSiteIndex: Site/SiteGroup for which the new clone needs to be created.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	-- 4. For external relationships, usually the entity under record versioning that owns the relationship is referenced using its specifc record version guid, whereas the external entity 
	--	  is referenced using its master record guid to account for cases where Record Versioning might later be turned off on the external entity type.
	--	  E.g. Equipment maintains a foreign relationship with Companies. The relationship is owned and maintained directly in tblEquipment itself. Therefore in this relationship, the 
	--	  equipment is referenced using the specifc Equipment record version guid, and the Company record is referenced using its master record guid.
	--	  However, when both entity types in a relationship supports Record Versioning, and this relationship is not owned by either one, but is owned by both, and is maintained in a 
	--	  separate mapping table that is configurable from either entity type (symmetry configurations), then both entities are referenced by their specific record version guid. This
	--	  also holds true for mappings between the same entity types (e.g. Company-To-Company mappings).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @childRecordVersionGuid uniqueidentifier
		SET @childRecordVersionGuid = NEWID()

		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @sourceSite uniqueidentifier
		SELECT @masterRecGuid = _MasterRecordGuid, @sourceSite = SiteGuid FROM tblCompanies
		WHERE CompanyGuid = @ParentEntityGuid

		IF NOT EXISTS
		(
			SELECT * FROM map.tblEntityCompanyToSite
			WHERE CompanyGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		IF EXISTS
		(
			SELECT * FROM tblCompanies
			WHERE _MasterRecordGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		--Create the child record version by cloning the internal fields of the parent record version
		INSERT INTO tblCompanies
		(CompanyGuid,ID,SiteGuid, _MasterRecordGuid,Code,Name,Address1,Address2,City,State,Zip,Country,Phone,FAX,EmergencyContact,EmergencyPhone,FlightPrefix,EffectiveDate,ExpirationDate,OnHold,PickupFLights,StockTrack,SufferLossGain,LowStockWarning,LockedOut,LockedOutReason,LockedOutDate,ReceivableAccount,RefinerCode,LastActivityDate,CreditOK,AdditiveAccounting,PurchaseOrderRequired,EPANumber,FederalID,TaxNumber,FlushPermitted,PumpOffPermitted,DeliveryToTerminalPermitted,LicenseNumber,LicenseExpiration,InsuranceCompany,InsurancePolicy,LiabilityAmount,HazardousMaterialExclusion,InsuranceExpiration,AllowDriverEntry,PINRequired,MaximumVehicleWeight,WeightUnits,AccountNumber,SCACCode,DisableOwnerAllocationsCheck,DisableShipperAllocationsCheck,DisableBillToAllocationsCheck,DisableShipToAllocationsCheck,LoadRackDisplayText,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,IATAGuid,ShipperTypeApplicationStringGuid,CustomerBillToTypeApplicationStringGuid,CustomerShipToTypeApplicationStringGuid,Contact1Name,Contact1Address1,Contact1Address2,Contact1City,Contact1State,Contact1Zip,Contact1Country,Contact1PhoneOffice,Contact1Fax,Contact1EmailAddress,Contact2Name,Contact2Address1,Contact2Address2,Contact2City,Contact2State,Contact2Zip,Contact2Country,Contact2PhoneOffice,Contact2Fax,Contact2EmailAddress,Contact1PhoneMobile,Contact2PhoneMobile,Note,HiddenDate,ConsortiumTypeIndex,FederalID2,FederalID3,FederalID4,FederalID5,ScullyRequired,ShortName,StateID)
		SELECT @childRecordVersionGuid,ID,@TargetSiteIndex,_MasterRecordGuid,Code,Name,Address1,Address2,City,State,Zip,Country,Phone,FAX,EmergencyContact,EmergencyPhone,FlightPrefix,EffectiveDate,ExpirationDate,OnHold,PickupFLights,StockTrack,SufferLossGain,LowStockWarning,LockedOut,LockedOutReason,LockedOutDate,ReceivableAccount,RefinerCode,LastActivityDate,CreditOK,AdditiveAccounting,PurchaseOrderRequired,EPANumber,FederalID,TaxNumber,FlushPermitted,PumpOffPermitted,DeliveryToTerminalPermitted,LicenseNumber,LicenseExpiration,InsuranceCompany,InsurancePolicy,LiabilityAmount,HazardousMaterialExclusion,InsuranceExpiration,AllowDriverEntry,PINRequired,MaximumVehicleWeight,WeightUnits,AccountNumber,SCACCode,DisableOwnerAllocationsCheck,DisableShipperAllocationsCheck,DisableBillToAllocationsCheck,DisableShipToAllocationsCheck,LoadRackDisplayText,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,@CreatedBy,@CreatedDate,@CreatedBy,@CreatedDate,IATAGuid,ShipperTypeApplicationStringGuid,CustomerBillToTypeApplicationStringGuid,CustomerShipToTypeApplicationStringGuid,Contact1Name,Contact1Address1,Contact1Address2,Contact1City,Contact1State,Contact1Zip,Contact1Country,Contact1PhoneOffice,Contact1Fax,Contact1EmailAddress,Contact2Name,Contact2Address1,Contact2Address2,Contact2City,Contact2State,Contact2Zip,Contact2Country,Contact2PhoneOffice,Contact2Fax,Contact2EmailAddress,Contact1PhoneMobile,Contact2PhoneMobile,Note,HiddenDate,ConsortiumTypeIndex,FederalID2, FederalID3,FederalID4,FederalID5,ScullyRequired,ShortName,StateID
		FROM tblCompanies
		WHERE CompanyGuid = @ParentEntityGuid

		--Clone the external attributes of the parent record version
		--Equipments. 
		--The relationship between tblCompanies and tblEquipment is maintained fully on the tblEquipment side, which references the Company using the Company MasterRecordGuid. 
		--Therefore newly created Company child record versions are automatically going to inherit the applicable tblCompanies-tblEquipment relationships
		--and there are no actions to be taken for the new Company child record versions as far as the tblCompanies-tblEquipment relationships are concerned.


		--AuthorizedShipTo
		UPDATE a 
		SET a.CompanyGuid = @childRecordVersionGuid
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.CompanyGuid = @ParentEntityGuid		

		INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
		(CompanyGuid, AssignedToCompanyGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		@TargetSiteIndex, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.CompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] c
			WHERE c.CompanyGuid = @childRecordVersionGuid
			AND c.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid)
			AND c.SiteGuid = @TargetSiteIndex
		)

		--Drivers
		UPDATE a 
		SET a.CompanyGuid = @childRecordVersionGuid
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN tblPersonnel b
		ON b.PersonnelGuid = a.PersonnelGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.CompanyGuid = @ParentEntityGuid

		INSERT INTO [map].[tblCompanyPersonnelAssignedToCompany]
		(CompanyGuid, PersonnelGuid, SiteGuid, ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Personnel', b.MasterRecordGuid, @TargetSiteIndex), a.PersonnelGuid), --Clone the mapping even if the Personnel is not assigned to the target site, so that the invalid mapping is available when/if the Personnel is eventually mapped to the site.
		@TargetSiteIndex, a.ID, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyPersonnelAssignedToCompany] a
		INNER JOIN [erv].[udf_GetPersonnelRecordVersions](@sourceSite) b  --Only clone those Company mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Company RecordVersioning was Off (and Personnel RecordVersioning was On).
		ON b.PersonnelGuid = a.PersonnelGuid
		WHERE a.CompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblCompanyPersonnelAssignedToCompany]  c
		  WHERE c.CompanyGuid = @childRecordVersionGuid
		  AND c.PersonnelGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Personnel', b.MasterRecordGuid, @TargetSiteIndex), a.PersonnelGuid)
		)

		--UnavailableInventories
		UPDATE a 
		SET a.AssignedToCompanyGuid = @childRecordVersionGuid
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.AssignedToCompanyGuid = @ParentEntityGuid		

		INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToUnavailableInventoryCompany] a		
		INNER JOIN [erv].[udf_GetProductRecordVersions](@sourceSite) b  --Only clone those Company mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Company RecordVersioning was Off (and Product RecordVersioning was On).
		ON b.ProductGuid = a.ProductGuid
		WHERE a.AssignedToCompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToUnavailableInventoryCompany]  c
		  WHERE c.AssignedToCompanyGuid = @childRecordVersionGuid
		  AND c.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid)
		)

		--ShipToAuthorizedProducts		
		UPDATE a 
		SET a.AssignedToCompanyGuid = @childRecordVersionGuid
		FROM [map].[tblProductToCompany] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.AssignedToCompanyGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, SpecialInstructionNote, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, a.SpecialInstructionNote, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompany] a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@sourceSite) b  --Only clone those Company mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Company RecordVersioning was Off (and Product RecordVersioning was On).
		ON b.ProductGuid = a.ProductGuid
		WHERE a.AssignedToCompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToCompany]  c
		  WHERE c.AssignedToCompanyGuid = @childRecordVersionGuid
		  AND c.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid)
		)

		--AuthorizedCarriers
		UPDATE a 
		SET a.AssignedToCompanyGuid = @childRecordVersionGuid
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.AssignedToCompanyGuid = @ParentEntityGuid		

		INSERT INTO [map].[tblCompanyAuthorizedCarrierToCompany]
		(AssignedToCompanyGuid, CompanyGuid, SiteGuid, Id, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.CompanyGuid), --Clone the mapping even if the Company is not assigned to the target site, so that the invalid mapping is available when/if the Company is eventually mapped to the site.
		@TargetSiteIndex, a.Id, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblCompanyAuthorizedCarrierToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		WHERE a.AssignedToCompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblCompanyAuthorizedCarrierToCompany] c
			WHERE c.AssignedToCompanyGuid = @childRecordVersionGuid
			AND c.CompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.CompanyGuid)
			AND c.SiteGuid = @TargetSiteIndex
		)

		--SupplierAuthorizedProducts
		UPDATE a 
		SET a.AssignedToCompanyGuid = @childRecordVersionGuid
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.AssignedToCompanyGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToSupplierProductCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid), --Clone the mapping even if the Product is not assigned to the target site, so that the invalid mapping is available when/if the Product is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN [erv].[udf_GetProductRecordVersions](@sourceSite) b  --Only clone those Company mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Company RecordVersioning was Off (and Product RecordVersioning was On).
		ON b.ProductGuid = a.ProductGuid
		WHERE a.AssignedToCompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToSupplierProductCompany]  c
		  WHERE c.AssignedToCompanyGuid = @childRecordVersionGuid
		  AND c.ProductGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Product', b.MasterRecordGuid, @TargetSiteIndex), a.ProductGuid)
		)

		--AccessSchedule
		INSERT INTO [dbo].[tblScheduleCompanyAccess]
		(CompanyGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, LookupDayOfWeekIndex, Enabled, OpeningTime, ClosingTime, EndOfDayEnabled, EndOfDayTime, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [dbo].[tblScheduleCompanyAccess]
		WHERE CompanyGuid = @ParentEntityGuid

		--CertificatesAndPermits
		UPDATE a 
		SET a.CompanyGuid = @childRecordVersionGuid
		FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		INNER JOIN [dbo].[tblQualifications] b
		ON b.QualificationGuid = a.QualificationGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.CompanyGuid = @ParentEntityGuid

		INSERT INTO [map].[tblQualificationCompanyCertificateAndPermitToCompany]
		(CompanyGuid, QualificationGuid, Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, QualificationGuid,
		Sequence, Instructor, DateCompleted, DateDue, ExpirationDate, ID, Rating, HistoricalRecord, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany] a
		WHERE a.CompanyGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany]  c
		  WHERE c.CompanyGuid = @childRecordVersionGuid
		  AND c.QualificationGuid = a.QualificationGuid
		)

		--UserGroups
		--User Groups are are created during company-to-site and group-to-site assignment. After with FLC on for UserGroups the assignments may be configured by site.

		--CompanyRoles
		--Company Roles are created/cloned and deleted independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole for each company-to-site assignment, using a combination of MasterRecordGuid and Siteguid.		

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
						+ 'Procedure Name: [erv].usp_CreateCompanyChildRecordVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO


