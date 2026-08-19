/*
	DROP PROCEDURE [erv].[usp_CreateProductChildRecordVersion]

	DECLARE @dt DateTimeOffset(7)
	SET @dt = GETDATE()
	--EXEC [erv].[usp_CreateProductChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '0F7228B9-D8E4-41C8-A862-B71FB3F38763', @dt, 'HB'
	EXEC [erv].[usp_CreateProductChildRecordVersion] 'F5EA57B8-2CFB-4605-9B55-8850199671C7', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364', @dt, 'HB'

	SELECT ProductGuid, ProductId, _MasterRecordGuid, SiteGuid, * FROM tblProducts WHERE _MasterRecordGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'	
*/

CREATE PROCEDURE [erv].[usp_CreateProductChildRecordVersion]
(
	@ParentEntityGuid uniqueidentifier, @TargetSiteIndex uniqueidentifier, @CreatedDate datetimeoffset(7), @CreatedBy nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_CreateProductChildRecordVersion] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Creates a new Product record version for a target site/sitegroup, off a parent record version 
	-- Notes:
	-- 1. @ParentEntityGuid: Entity Guid of the record to be cloned.
	-- 2. @TargetSiteIndex: Site/SiteGroup for which the new clone needs to be created.
	-- 3. For performance reasons and to eliminate redundant validation steps, this operation does not verify if the necessary FLC configurations
	--    are in place to support the creation of the new record version. It is the responsibility of the caller to do so.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @childRecordVersionGuid uniqueidentifier
		SET @childRecordVersionGuid = NEWID()

		DECLARE @masterRecGuid uniqueidentifier
		DECLARE @sourceSite uniqueidentifier
		SELECT @masterRecGuid = _MasterRecordGuid, @sourceSite = SiteGuid FROM tblProducts
		WHERE ProductGuid = @ParentEntityGuid

		IF NOT EXISTS
		(
			SELECT * FROM map.tblEntityProductToSite
			WHERE ProductGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		IF EXISTS
		(
			SELECT * FROM tblProducts
			WHERE _MasterRecordGuid = @masterRecGuid
			AND SiteGuid = @TargetSiteIndex
		)
		BEGIN
			RETURN
		END

		--Create the child record version by cloning the internal fields of the parent record version
		INSERT INTO tblProducts
		(ProductGuid,ProductID,SiteGuid,_MasterRecordGuid,ApplyDensityLimits,ApplyStandardDensity,ApplyTemperatureLimits,ApplyVolumeCorrection,AutomaticCloseout,AviationFuelFlag,Bonded,Capitalize,ComponentTolerance,ContaminationPromptLoadRackText,CreatedBy,CreatedDate,DensityDeadband,DensityDecimalPlaces,DensityHighLimit,DensityLowLimit,DensityUnitIndex,Description,DielectricTolerance,FlowDecimalPlaces,FlowUnitIndex,GenericType,GroundFuel,HazardousMaterial,HiddenDate,InhibitAccounting,LevelDecimalPlaces,LevelUnitIndex,LoadByWeight,LoadRackDisplayText,LockedOut,LockedOutDate,LockedOutReason,LookupProductTypeIndex,LowStockWarning,MassDecimalPlaces,MassPackageSize,MassUnitIndex,OctaneNumber,PatternColor,PatternNumber,PIDXCode,PIDXFamilyCode,IsEthanol,PressureDecimalPlaces,PressureUnitIndex,Price,ProductCode,ProductColor,RegulatoryClass,ReidVaporPressure,StandardDensity,StockResetDate,StockTrack,TaxCode,TemperatureDeadband,TemperatureDecimalPlaces,TemperatureHighLimit,TemperatureHiHiLimit,TemperatureLoLoLimit,TemperatureLowLimit,TemperatureUnitIndex,TrackingProductGuid,UpdatedBy,UpdatedDate,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,VaporRecovery,VarianceTolerance,VcfModuleSettings,VolumeDecimalPlaces,VolumePackageSize,VolumeUnitIndex)	
		SELECT @childRecordVersionGuid,ProductID,@TargetSiteIndex,_MasterRecordGuid,ApplyDensityLimits,ApplyStandardDensity,ApplyTemperatureLimits,ApplyVolumeCorrection,AutomaticCloseout,AviationFuelFlag,Bonded,Capitalize,ComponentTolerance,ContaminationPromptLoadRackText,@CreatedBy,@CreatedDate,DensityDeadband,DensityDecimalPlaces,DensityHighLimit,DensityLowLimit,DensityUnitIndex,Description,DielectricTolerance,FlowDecimalPlaces,FlowUnitIndex,GenericType,GroundFuel,HazardousMaterial,HiddenDate,InhibitAccounting,LevelDecimalPlaces,LevelUnitIndex,LoadByWeight,LoadRackDisplayText,LockedOut,LockedOutDate,LockedOutReason,LookupProductTypeIndex,LowStockWarning,MassDecimalPlaces,MassPackageSize,MassUnitIndex,OctaneNumber,PatternColor,PatternNumber,PIDXCode,PIDXFamilyCode,IsEthanol,PressureDecimalPlaces,PressureUnitIndex,Price,ProductCode,ProductColor,RegulatoryClass,ReidVaporPressure,StandardDensity,StockResetDate,StockTrack,TaxCode,TemperatureDeadband,TemperatureDecimalPlaces,TemperatureHighLimit,TemperatureHiHiLimit,TemperatureLoLoLimit,TemperatureLowLimit,TemperatureUnitIndex,TrackingProductGuid,@CreatedBy,@CreatedDate,UserData1,UserData2,UserData3,UserData4,UserData5,UserData6,UserData7,UserData8,VaporRecovery,VarianceTolerance,VcfModuleSettings,VolumeDecimalPlaces,VolumePackageSize,VolumeUnitIndex
		FROM tblProducts
		WHERE ProductGuid = @ParentEntityGuid


		--Clone the external attributes of the parent record version
		--Authorised Customers - ProductToCompany
		-- For all the ProductToCompany mappings that reference the Parent Product record version instead of the actual Product child record version, because Record Versioning 
		-- was previously OFF for Product for that site, update the Product field of the mapping to point to the newly created Product child record version.				
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblProductToCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.ProductGuid = @ParentEntityGuid
		AND b.SiteGuid = @TargetSiteIndex
		
		--Clone the ProductToCompany mappings, making sure to use the appropriate Company record version guid, and ignoring those mappings that might have already been introduced through the mapping Update statement above.
		INSERT INTO [map].[tblProductToCompany]
		(AssignedToCompanyGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterID, 
		ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, SpecialInstructionNote, UnavailableInventoryGross, UnavailableInventoryNet, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', b.MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		@childRecordVersionGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterID, 
		a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.SpecialInstructionNote, a.UnavailableInventoryGross, a.UnavailableInventoryNet,
		@CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompany] a
		INNER JOIN [erv].[udf_GetCompanyRecordVersions](@sourceSite) b  --Only clone those Product mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Product RecordVersioning was Off (and Company RecordVersioning was On).
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
			SELECT * FROM [map].[tblProductToCompany] c 
			WHERE c.ProductGuid = @childRecordVersionGuid
			AND c.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid] ('Company', b.MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid)
		)


		--Authorised Customers - ProductToCompanyGroup
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblProductToCompanyGroup] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToCompanyGroup]
		(AssignedToApplicationStringGuid, ProductGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterID, 
		ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, SpecialInstructionNote, UnavailableInventoryGross, UnavailableInventoryNet, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.AssignedToApplicationStringGuid, @childRecordVersionGuid, a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, 
		a.AdditiveProfileGuid, a.TankGuid, a.MeterID, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.SpecialInstructionNote, a.UnavailableInventoryGross, 
		a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToCompanyGroup] a
		WHERE ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToCompanyGroup]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.AssignedToApplicationStringGuid = a.AssignedToApplicationStringGuid
		)

		--Assigned Messages - Regular Product Messages
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblApplicationStringToProductMessage] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblApplicationStringToProductMessage]
		(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.ApplicationStringGuid, @childRecordVersionGuid, a.Sequence, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblApplicationStringToProductMessage] a
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblApplicationStringToProductMessage]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.ApplicationStringGuid = a.ApplicationStringGuid
		)

		--Assigned Messages - DOT Hazardous Messages
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblApplicationStringToDotHazardousMessage] a
		INNER JOIN tblProducts b
		ON b.ProductGuid = a.ProductGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblApplicationStringToDotHazardousMessage]
		(ApplicationStringGuid, ProductGuid, Sequence, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT a.ApplicationStringGuid, @childRecordVersionGuid, a.Sequence, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblApplicationStringToProductMessage] a
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblApplicationStringToDotHazardousMessage]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.ApplicationStringGuid = a.ApplicationStringGuid
		)

		--UnavailableInventories
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToUnavailableInventoryCompany]
		(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b.MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToUnavailableInventoryCompany] a
		INNER JOIN [erv].[udf_GetCompanyRecordVersions](@sourceSite) b  --Only clone those Product mappings that are pertinent to the parent/source site. This filter covers the case where the child record version mappings were originally built when Product RecordVersioning was Off (and Company RecordVersioning was On).
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToUnavailableInventoryCompany]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b.MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid)
		)

		--SupplierAuthorizedProducts
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToSupplierProductCompany]
		(ProductGuid, AssignedToCompanyGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid), --Clone the mapping even if the AssignedToCompany is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToCompany is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToSupplierProductCompany] a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.AssignedToCompanyGuid
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToSupplierProductCompany]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.AssignedToCompanyGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Company', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToCompanyGuid)
		)

		--TransactionAliasExclusion
		UPDATE a 
		SET a.ProductGuid = @childRecordVersionGuid
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		WHERE b.SiteGuid = @TargetSiteIndex
		AND a.ProductGuid = @ParentEntityGuid

		INSERT INTO [map].[tblProductToTransactionAliasExclusion]
		(ProductGuid, AssignedToTransactionAliasGuid, Sequence, BlendPercentage, AdditiveRate, Ratio, AdditiveCycleVolume, Tolerance, PresetNumber, AdditiveProfileGuid, TankGuid, MeterId, ShipToProductID, ShipToProductCode, ShipToLoadRackDisplayText, UnavailableInventoryGross, UnavailableInventoryNet, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		SELECT @childRecordVersionGuid, 
		ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToTransactionAliasGuid), --Clone the mapping even if the AssignedToTransactionAliasGuid is not assigned to the target site, so that the invalid mapping is available when/if the AssignedToTransactionAliasGuid is eventually mapped to the site.
		a.Sequence, a.BlendPercentage, a.AdditiveRate, a.Ratio, a.AdditiveCycleVolume, a.Tolerance, a.PresetNumber, a.AdditiveProfileGuid, a.TankGuid, a.MeterId, a.ShipToProductID, a.ShipToProductCode, a.ShipToLoadRackDisplayText, a.UnavailableInventoryGross, a.UnavailableInventoryNet, @CreatedDate, @CreatedBy, @CreatedDate, @CreatedBy
		FROM [map].[tblProductToTransactionAliasExclusion] a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.AssignedToTransactionAliasGuid
		WHERE a.ProductGuid = @ParentEntityGuid
		AND NOT EXISTS
		(
		  SELECT * FROM [map].[tblProductToTransactionAliasExclusion]  c
		  WHERE c.ProductGuid = @childRecordVersionGuid
		  AND c.AssignedToTransactionAliasGuid = ISNULL([erv].[udf_GetFirstParentRecordVersionGuid]('Transaction_Alias', b._MasterRecordGuid, @TargetSiteIndex), a.AssignedToTransactionAliasGuid)
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
						+ 'Procedure Name: [erv].usp_CreateProductChildRecordVersion' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
