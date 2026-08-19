CREATE PROCEDURE [dbo].[gsp_CompaniesUpdateByPK]
(
		@CompanyGuid uniqueidentifier
	,	@ID nvarchar(100)=NULL
	,	@Code nvarchar(10)=NULL
	,	@Name nvarchar(100)=NULL
	,	@Address1 nvarchar(60)=NULL
	,	@Address2 nvarchar(60)=NULL
	,	@City nvarchar(60)=NULL
	,	@State nvarchar(20)=NULL
	,	@Zip nvarchar(11)=NULL
	,	@Country nvarchar(30)=NULL
	,	@Phone nvarchar(20)=NULL
	,	@FAX nvarchar(20)=NULL
	,	@EmergencyContact nvarchar(30)=NULL
	,	@EmergencyPhone nvarchar(20)=NULL
	,	@FlightPrefix nvarchar(5)=NULL
	,	@EffectiveDate datetimeoffset(7)=NULL
	,	@ExpirationDate datetimeoffset(7)=NULL
	,	@OnHold bit=NULL
	,	@PickupFLights bit=NULL
	,	@StockTrack bit=NULL
	,	@SufferLossGain bit=NULL
	,	@LowStockWarning float=NULL
	,	@LockedOut bit=NULL
	,	@LockedOutReason nvarchar(80)=NULL
	,	@LockedOutDate datetimeoffset(7)=NULL
	,	@ReceivableAccount nvarchar(20)=NULL
	,	@RefinerCode nvarchar(20)=NULL
	,	@LastActivityDate datetimeoffset(7)=NULL
	,	@CreditOK bit=NULL
	,	@AdditiveAccounting bit=NULL
	,	@PurchaseOrderRequired bit=NULL
	,	@EPANumber nvarchar(20)=NULL
	,	@FederalID nvarchar(20)=NULL
	,	@TaxNumber nvarchar(20)=NULL
	,	@FlushPermitted bit=NULL
	,	@PumpOffPermitted bit=NULL
	,	@DeliveryToTerminalPermitted bit=NULL
	,	@LicenseNumber nvarchar(20)=NULL
	,	@LicenseExpiration datetimeoffset(7)=NULL
	,	@InsuranceCompany nvarchar(20)=NULL
	,	@InsurancePolicy nvarchar(20)=NULL
	,	@LiabilityAmount money=NULL
	,	@HazardousMaterialExclusion bit=NULL
	,	@InsuranceExpiration datetimeoffset(7)=NULL
	,	@AllowDriverEntry bit=NULL
	,	@PINRequired bit=NULL
	,	@MaximumVehicleWeight float=NULL
	,	@WeightUnits smallint=NULL
	,	@AccountNumber nvarchar(30)=NULL
	,	@SCACCode nvarchar(4)=NULL
	,	@DisableOwnerAllocationsCheck bit=NULL
	,	@DisableShipperAllocationsCheck bit=NULL
	,	@DisableBillToAllocationsCheck bit=NULL
	,	@DisableShipToAllocationsCheck bit=NULL
	,	@LoadRackDisplayText nvarchar(30)=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@IATAGuid uniqueidentifier=NULL
	,	@ShipperTypeApplicationStringGuid uniqueidentifier=NULL
	,	@CustomerBillToTypeApplicationStringGuid uniqueidentifier=NULL
	,	@CustomerShipToTypeApplicationStringGuid uniqueidentifier=NULL
	,	@Contact1Name nvarchar(30)=NULL
	,	@Contact1Address1 nvarchar(30)=NULL
	,	@Contact1Address2 nvarchar(30)=NULL
	,	@Contact1City nvarchar(60)=NULL
	,	@Contact1State nvarchar(20)=NULL
	,	@Contact1Zip nvarchar(11)=NULL
	,	@Contact1Country nvarchar(30)=NULL
	,	@Contact1PhoneOffice nvarchar(20)=NULL
	,	@Contact1Fax nvarchar(20)=NULL
	,	@Contact1EmailAddress nvarchar(30)=NULL
	,	@Contact2Name nvarchar(30)=NULL
	,	@Contact2Address1 nvarchar(30)=NULL
	,	@Contact2Address2 nvarchar(30)=NULL
	,	@Contact2City nvarchar(60)=NULL
	,	@Contact2State nvarchar(20)=NULL
	,	@Contact2Zip nvarchar(11)=NULL
	,	@Contact2Country nvarchar(30)=NULL
	,	@Contact2PhoneOffice nvarchar(20)=NULL
	,	@Contact2Fax nvarchar(20)=NULL
	,	@Contact2EmailAddress nvarchar(30)=NULL
	,	@Contact1PhoneMobile nvarchar(20)=NULL
	,	@Contact2PhoneMobile nvarchar(20)=NULL
	,	@_MasterRecordGuid uniqueidentifier=NULL
	,	@Note nvarchar(2000)=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
	,	@NullOverrideID BIT=0 
	,	@NullOverrideCode BIT=0 
	,	@NullOverrideName BIT=0 
	,	@NullOverrideAddress1 BIT=0 
	,	@NullOverrideAddress2 BIT=0 
	,	@NullOverrideCity BIT=0 
	,	@NullOverrideState BIT=0 
	,	@NullOverrideZip BIT=0 
	,	@NullOverrideCountry BIT=0 
	,	@NullOverridePhone BIT=0 
	,	@NullOverrideFAX BIT=0 
	,	@NullOverrideEmergencyContact BIT=0 
	,	@NullOverrideEmergencyPhone BIT=0 
	,	@NullOverrideFlightPrefix BIT=0 
	,	@NullOverrideEffectiveDate BIT=0 
	,	@NullOverrideExpirationDate BIT=0 
	,	@NullOverrideOnHold BIT=0 
	,	@NullOverridePickupFLights BIT=0 
	,	@NullOverrideStockTrack BIT=0 
	,	@NullOverrideSufferLossGain BIT=0 
	,	@NullOverrideLowStockWarning BIT=0 
	,	@NullOverrideLockedOut BIT=0 
	,	@NullOverrideLockedOutReason BIT=0 
	,	@NullOverrideLockedOutDate BIT=0 
	,	@NullOverrideReceivableAccount BIT=0 
	,	@NullOverrideRefinerCode BIT=0 
	,	@NullOverrideLastActivityDate BIT=0 
	,	@NullOverrideCreditOK BIT=0 
	,	@NullOverrideAdditiveAccounting BIT=0 
	,	@NullOverridePurchaseOrderRequired BIT=0 
	,	@NullOverrideEPANumber BIT=0 
	,	@NullOverrideFederalID BIT=0 
	,	@NullOverrideTaxNumber BIT=0 
	,	@NullOverrideFlushPermitted BIT=0 
	,	@NullOverridePumpOffPermitted BIT=0 
	,	@NullOverrideDeliveryToTerminalPermitted BIT=0 
	,	@NullOverrideLicenseNumber BIT=0 
	,	@NullOverrideLicenseExpiration BIT=0 
	,	@NullOverrideInsuranceCompany BIT=0 
	,	@NullOverrideInsurancePolicy BIT=0 
	,	@NullOverrideLiabilityAmount BIT=0 
	,	@NullOverrideHazardousMaterialExclusion BIT=0 
	,	@NullOverrideInsuranceExpiration BIT=0 
	,	@NullOverrideAllowDriverEntry BIT=0 
	,	@NullOverridePINRequired BIT=0 
	,	@NullOverrideMaximumVehicleWeight BIT=0 
	,	@NullOverrideWeightUnits BIT=0 
	,	@NullOverrideAccountNumber BIT=0 
	,	@NullOverrideSCACCode BIT=0 
	,	@NullOverrideDisableOwnerAllocationsCheck BIT=0 
	,	@NullOverrideDisableShipperAllocationsCheck BIT=0 
	,	@NullOverrideDisableBillToAllocationsCheck BIT=0 
	,	@NullOverrideDisableShipToAllocationsCheck BIT=0 
	,	@NullOverrideLoadRackDisplayText BIT=0 
	,	@NullOverrideUserData1 BIT=0 
	,	@NullOverrideUserData2 BIT=0 
	,	@NullOverrideUserData3 BIT=0 
	,	@NullOverrideUserData4 BIT=0 
	,	@NullOverrideUserData5 BIT=0 
	,	@NullOverrideUserData6 BIT=0 
	,	@NullOverrideUserData7 BIT=0 
	,	@NullOverrideUserData8 BIT=0 
	,	@NullOverrideUpdatedDate BIT=0 
	,	@NullOverrideSiteGuid BIT=0 
	,	@NullOverrideIATAGuid BIT=0 
	,	@NullOverrideShipperTypeApplicationStringGuid BIT=0 
	,	@NullOverrideCustomerBillToTypeApplicationStringGuid BIT=0 
	,	@NullOverrideCustomerShipToTypeApplicationStringGuid BIT=0 
	,	@NullOverrideContact1Name BIT=0 
	,	@NullOverrideContact1Address1 BIT=0 
	,	@NullOverrideContact1Address2 BIT=0 
	,	@NullOverrideContact1City BIT=0 
	,	@NullOverrideContact1State BIT=0 
	,	@NullOverrideContact1Zip BIT=0 
	,	@NullOverrideContact1Country BIT=0 
	,	@NullOverrideContact1PhoneOffice BIT=0 
	,	@NullOverrideContact1Fax BIT=0 
	,	@NullOverrideContact1EmailAddress BIT=0 
	,	@NullOverrideContact2Name BIT=0 
	,	@NullOverrideContact2Address1 BIT=0 
	,	@NullOverrideContact2Address2 BIT=0 
	,	@NullOverrideContact2City BIT=0 
	,	@NullOverrideContact2State BIT=0 
	,	@NullOverrideContact2Zip BIT=0 
	,	@NullOverrideContact2Country BIT=0 
	,	@NullOverrideContact2PhoneOffice BIT=0 
	,	@NullOverrideContact2Fax BIT=0 
	,	@NullOverrideContact2EmailAddress BIT=0 
	,	@NullOverrideContact1PhoneMobile BIT=0 
	,	@NullOverrideContact2PhoneMobile BIT=0 
	,	@NullOverride_MasterRecordGuid BIT=0 
	,	@NullOverrideNote BIT=0 
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_CompaniesUpdateByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.003 / 2014-02-05 16:52:29.4353907 -05:00
	-- Purpose: Update table [dbo].[tblCompanies]
	-- Notes:
	-- 1. @CompanyGuid and @UpdatedBy are required parameter.
	-- 2. If a value other than NULL is passed on @_RowVersion parameter then the stored procedure verifies whether _RowVersion of the record matches with the  
	--    @_RowVersion parameter and it will throw an exception if they don't match, otherwise it saves the parameters regardless.
	-- 3. The @_RowVersion output parameter will always be updated with new timestamp generated by the updating of the record.
	-- 4. To update a column with NULL then set the corresponding "@NullOverride..." parameter to 1 and either pass NULL through the correlated parameter 
	--    or do not include the parameter at all. 
	--    Example - Saving NULL to SiteGuid on tblEquipment:
	--            EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...',@SiteGuid=NULL, @NullOverrideSiteGuid=1 
	--       or   EXEC gsp_EquipmentUpdateByPK @EquipmentGuid='0000-...', @NullOverrideSiteGuid=1 
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		IF @_RowVersion IS NOT NULL AND NOT EXISTS(SELECT 1 FROM [dbo].[tblCompanies] WHERE CompanyGuid=@CompanyGuid AND _RowVersion=@_RowVersion)
		BEGIN
			RAISERROR('Attempted to modify a stale copy of the record',16,1);
			RETURN;
		END
 
		UPDATE [dbo].[tblCompanies] SET
			[ID]=(CASE ISNULL(@NullOverrideID,0) WHEN 1 THEN @ID ELSE ISNULL(@ID,[ID]) END)
		,	[Code]=(CASE ISNULL(@NullOverrideCode,0) WHEN 1 THEN @Code ELSE ISNULL(@Code,[Code]) END)
		,	[Name]=(CASE ISNULL(@NullOverrideName,0) WHEN 1 THEN @Name ELSE ISNULL(@Name,[Name]) END)
		,	[Address1]=(CASE ISNULL(@NullOverrideAddress1,0) WHEN 1 THEN @Address1 ELSE ISNULL(@Address1,[Address1]) END)
		,	[Address2]=(CASE ISNULL(@NullOverrideAddress2,0) WHEN 1 THEN @Address2 ELSE ISNULL(@Address2,[Address2]) END)
		,	[City]=(CASE ISNULL(@NullOverrideCity,0) WHEN 1 THEN @City ELSE ISNULL(@City,[City]) END)
		,	[State]=(CASE ISNULL(@NullOverrideState,0) WHEN 1 THEN @State ELSE ISNULL(@State,[State]) END)
		,	[Zip]=(CASE ISNULL(@NullOverrideZip,0) WHEN 1 THEN @Zip ELSE ISNULL(@Zip,[Zip]) END)
		,	[Country]=(CASE ISNULL(@NullOverrideCountry,0) WHEN 1 THEN @Country ELSE ISNULL(@Country,[Country]) END)
		,	[Phone]=(CASE ISNULL(@NullOverridePhone,0) WHEN 1 THEN @Phone ELSE ISNULL(@Phone,[Phone]) END)
		,	[FAX]=(CASE ISNULL(@NullOverrideFAX,0) WHEN 1 THEN @FAX ELSE ISNULL(@FAX,[FAX]) END)
		,	[EmergencyContact]=(CASE ISNULL(@NullOverrideEmergencyContact,0) WHEN 1 THEN @EmergencyContact ELSE ISNULL(@EmergencyContact,[EmergencyContact]) END)
		,	[EmergencyPhone]=(CASE ISNULL(@NullOverrideEmergencyPhone,0) WHEN 1 THEN @EmergencyPhone ELSE ISNULL(@EmergencyPhone,[EmergencyPhone]) END)
		,	[FlightPrefix]=(CASE ISNULL(@NullOverrideFlightPrefix,0) WHEN 1 THEN @FlightPrefix ELSE ISNULL(@FlightPrefix,[FlightPrefix]) END)
		,	[EffectiveDate]=(CASE ISNULL(@NullOverrideEffectiveDate,0) WHEN 1 THEN @EffectiveDate ELSE ISNULL(@EffectiveDate,[EffectiveDate]) END)
		,	[ExpirationDate]=(CASE ISNULL(@NullOverrideExpirationDate,0) WHEN 1 THEN @ExpirationDate ELSE ISNULL(@ExpirationDate,[ExpirationDate]) END)
		,	[OnHold]=(CASE ISNULL(@NullOverrideOnHold,0) WHEN 1 THEN @OnHold ELSE ISNULL(@OnHold,[OnHold]) END)
		,	[PickupFLights]=(CASE ISNULL(@NullOverridePickupFLights,0) WHEN 1 THEN @PickupFLights ELSE ISNULL(@PickupFLights,[PickupFLights]) END)
		,	[StockTrack]=(CASE ISNULL(@NullOverrideStockTrack,0) WHEN 1 THEN @StockTrack ELSE ISNULL(@StockTrack,[StockTrack]) END)
		,	[SufferLossGain]=(CASE ISNULL(@NullOverrideSufferLossGain,0) WHEN 1 THEN @SufferLossGain ELSE ISNULL(@SufferLossGain,[SufferLossGain]) END)
		,	[LowStockWarning]=(CASE ISNULL(@NullOverrideLowStockWarning,0) WHEN 1 THEN @LowStockWarning ELSE ISNULL(@LowStockWarning,[LowStockWarning]) END)
		,	[LockedOut]=(CASE ISNULL(@NullOverrideLockedOut,0) WHEN 1 THEN @LockedOut ELSE ISNULL(@LockedOut,[LockedOut]) END)
		,	[LockedOutReason]=(CASE ISNULL(@NullOverrideLockedOutReason,0) WHEN 1 THEN @LockedOutReason ELSE ISNULL(@LockedOutReason,[LockedOutReason]) END)
		,	[LockedOutDate]=(CASE ISNULL(@NullOverrideLockedOutDate,0) WHEN 1 THEN @LockedOutDate ELSE ISNULL(@LockedOutDate,[LockedOutDate]) END)
		,	[ReceivableAccount]=(CASE ISNULL(@NullOverrideReceivableAccount,0) WHEN 1 THEN @ReceivableAccount ELSE ISNULL(@ReceivableAccount,[ReceivableAccount]) END)
		,	[RefinerCode]=(CASE ISNULL(@NullOverrideRefinerCode,0) WHEN 1 THEN @RefinerCode ELSE ISNULL(@RefinerCode,[RefinerCode]) END)
		,	[LastActivityDate]=(CASE ISNULL(@NullOverrideLastActivityDate,0) WHEN 1 THEN @LastActivityDate ELSE ISNULL(@LastActivityDate,[LastActivityDate]) END)
		,	[CreditOK]=(CASE ISNULL(@NullOverrideCreditOK,0) WHEN 1 THEN @CreditOK ELSE ISNULL(@CreditOK,[CreditOK]) END)
		,	[AdditiveAccounting]=(CASE ISNULL(@NullOverrideAdditiveAccounting,0) WHEN 1 THEN @AdditiveAccounting ELSE ISNULL(@AdditiveAccounting,[AdditiveAccounting]) END)
		,	[PurchaseOrderRequired]=(CASE ISNULL(@NullOverridePurchaseOrderRequired,0) WHEN 1 THEN @PurchaseOrderRequired ELSE ISNULL(@PurchaseOrderRequired,[PurchaseOrderRequired]) END)
		,	[EPANumber]=(CASE ISNULL(@NullOverrideEPANumber,0) WHEN 1 THEN @EPANumber ELSE ISNULL(@EPANumber,[EPANumber]) END)
		,	[FederalID]=(CASE ISNULL(@NullOverrideFederalID,0) WHEN 1 THEN @FederalID ELSE ISNULL(@FederalID,[FederalID]) END)
		,	[TaxNumber]=(CASE ISNULL(@NullOverrideTaxNumber,0) WHEN 1 THEN @TaxNumber ELSE ISNULL(@TaxNumber,[TaxNumber]) END)
		,	[FlushPermitted]=(CASE ISNULL(@NullOverrideFlushPermitted,0) WHEN 1 THEN @FlushPermitted ELSE ISNULL(@FlushPermitted,[FlushPermitted]) END)
		,	[PumpOffPermitted]=(CASE ISNULL(@NullOverridePumpOffPermitted,0) WHEN 1 THEN @PumpOffPermitted ELSE ISNULL(@PumpOffPermitted,[PumpOffPermitted]) END)
		,	[DeliveryToTerminalPermitted]=(CASE ISNULL(@NullOverrideDeliveryToTerminalPermitted,0) WHEN 1 THEN @DeliveryToTerminalPermitted ELSE ISNULL(@DeliveryToTerminalPermitted,[DeliveryToTerminalPermitted]) END)
		,	[LicenseNumber]=(CASE ISNULL(@NullOverrideLicenseNumber,0) WHEN 1 THEN @LicenseNumber ELSE ISNULL(@LicenseNumber,[LicenseNumber]) END)
		,	[LicenseExpiration]=(CASE ISNULL(@NullOverrideLicenseExpiration,0) WHEN 1 THEN @LicenseExpiration ELSE ISNULL(@LicenseExpiration,[LicenseExpiration]) END)
		,	[InsuranceCompany]=(CASE ISNULL(@NullOverrideInsuranceCompany,0) WHEN 1 THEN @InsuranceCompany ELSE ISNULL(@InsuranceCompany,[InsuranceCompany]) END)
		,	[InsurancePolicy]=(CASE ISNULL(@NullOverrideInsurancePolicy,0) WHEN 1 THEN @InsurancePolicy ELSE ISNULL(@InsurancePolicy,[InsurancePolicy]) END)
		,	[LiabilityAmount]=(CASE ISNULL(@NullOverrideLiabilityAmount,0) WHEN 1 THEN @LiabilityAmount ELSE ISNULL(@LiabilityAmount,[LiabilityAmount]) END)
		,	[HazardousMaterialExclusion]=(CASE ISNULL(@NullOverrideHazardousMaterialExclusion,0) WHEN 1 THEN @HazardousMaterialExclusion ELSE ISNULL(@HazardousMaterialExclusion,[HazardousMaterialExclusion]) END)
		,	[InsuranceExpiration]=(CASE ISNULL(@NullOverrideInsuranceExpiration,0) WHEN 1 THEN @InsuranceExpiration ELSE ISNULL(@InsuranceExpiration,[InsuranceExpiration]) END)
		,	[AllowDriverEntry]=(CASE ISNULL(@NullOverrideAllowDriverEntry,0) WHEN 1 THEN @AllowDriverEntry ELSE ISNULL(@AllowDriverEntry,[AllowDriverEntry]) END)
		,	[PINRequired]=(CASE ISNULL(@NullOverridePINRequired,0) WHEN 1 THEN @PINRequired ELSE ISNULL(@PINRequired,[PINRequired]) END)
		,	[MaximumVehicleWeight]=(CASE ISNULL(@NullOverrideMaximumVehicleWeight,0) WHEN 1 THEN @MaximumVehicleWeight ELSE ISNULL(@MaximumVehicleWeight,[MaximumVehicleWeight]) END)
		,	[WeightUnits]=(CASE ISNULL(@NullOverrideWeightUnits,0) WHEN 1 THEN @WeightUnits ELSE ISNULL(@WeightUnits,[WeightUnits]) END)
		,	[AccountNumber]=(CASE ISNULL(@NullOverrideAccountNumber,0) WHEN 1 THEN @AccountNumber ELSE ISNULL(@AccountNumber,[AccountNumber]) END)
		,	[SCACCode]=(CASE ISNULL(@NullOverrideSCACCode,0) WHEN 1 THEN @SCACCode ELSE ISNULL(@SCACCode,[SCACCode]) END)
		,	[DisableOwnerAllocationsCheck]=(CASE ISNULL(@NullOverrideDisableOwnerAllocationsCheck,0) WHEN 1 THEN @DisableOwnerAllocationsCheck ELSE ISNULL(@DisableOwnerAllocationsCheck,[DisableOwnerAllocationsCheck]) END)
		,	[DisableShipperAllocationsCheck]=(CASE ISNULL(@NullOverrideDisableShipperAllocationsCheck,0) WHEN 1 THEN @DisableShipperAllocationsCheck ELSE ISNULL(@DisableShipperAllocationsCheck,[DisableShipperAllocationsCheck]) END)
		,	[DisableBillToAllocationsCheck]=(CASE ISNULL(@NullOverrideDisableBillToAllocationsCheck,0) WHEN 1 THEN @DisableBillToAllocationsCheck ELSE ISNULL(@DisableBillToAllocationsCheck,[DisableBillToAllocationsCheck]) END)
		,	[DisableShipToAllocationsCheck]=(CASE ISNULL(@NullOverrideDisableShipToAllocationsCheck,0) WHEN 1 THEN @DisableShipToAllocationsCheck ELSE ISNULL(@DisableShipToAllocationsCheck,[DisableShipToAllocationsCheck]) END)
		,	[LoadRackDisplayText]=(CASE ISNULL(@NullOverrideLoadRackDisplayText,0) WHEN 1 THEN @LoadRackDisplayText ELSE ISNULL(@LoadRackDisplayText,[LoadRackDisplayText]) END)
		,	[UserData1]=(CASE ISNULL(@NullOverrideUserData1,0) WHEN 1 THEN @UserData1 ELSE ISNULL(@UserData1,[UserData1]) END)
		,	[UserData2]=(CASE ISNULL(@NullOverrideUserData2,0) WHEN 1 THEN @UserData2 ELSE ISNULL(@UserData2,[UserData2]) END)
		,	[UserData3]=(CASE ISNULL(@NullOverrideUserData3,0) WHEN 1 THEN @UserData3 ELSE ISNULL(@UserData3,[UserData3]) END)
		,	[UserData4]=(CASE ISNULL(@NullOverrideUserData4,0) WHEN 1 THEN @UserData4 ELSE ISNULL(@UserData4,[UserData4]) END)
		,	[UserData5]=(CASE ISNULL(@NullOverrideUserData5,0) WHEN 1 THEN @UserData5 ELSE ISNULL(@UserData5,[UserData5]) END)
		,	[UserData6]=(CASE ISNULL(@NullOverrideUserData6,0) WHEN 1 THEN @UserData6 ELSE ISNULL(@UserData6,[UserData6]) END)
		,	[UserData7]=(CASE ISNULL(@NullOverrideUserData7,0) WHEN 1 THEN @UserData7 ELSE ISNULL(@UserData7,[UserData7]) END)
		,	[UserData8]=(CASE ISNULL(@NullOverrideUserData8,0) WHEN 1 THEN @UserData8 ELSE ISNULL(@UserData8,[UserData8]) END)
		,	[UpdatedDate]=ISNULL(@UpdatedDate,SYSDATETIMEOFFSET())
		,	[UpdatedBy]= ISNULL(@UpdatedBy,SUSER_SNAME())
		,	[SiteGuid]=(CASE ISNULL(@NullOverrideSiteGuid,0) WHEN 1 THEN @SiteGuid ELSE ISNULL(@SiteGuid,[SiteGuid]) END)
		,	[IATAGuid]=(CASE ISNULL(@NullOverrideIATAGuid,0) WHEN 1 THEN @IATAGuid ELSE ISNULL(@IATAGuid,[IATAGuid]) END)
		,	[ShipperTypeApplicationStringGuid]=(CASE ISNULL(@NullOverrideShipperTypeApplicationStringGuid,0) WHEN 1 THEN @ShipperTypeApplicationStringGuid ELSE ISNULL(@ShipperTypeApplicationStringGuid,[ShipperTypeApplicationStringGuid]) END)
		,	[CustomerBillToTypeApplicationStringGuid]=(CASE ISNULL(@NullOverrideCustomerBillToTypeApplicationStringGuid,0) WHEN 1 THEN @CustomerBillToTypeApplicationStringGuid ELSE ISNULL(@CustomerBillToTypeApplicationStringGuid,[CustomerBillToTypeApplicationStringGuid]) END)
		,	[CustomerShipToTypeApplicationStringGuid]=(CASE ISNULL(@NullOverrideCustomerShipToTypeApplicationStringGuid,0) WHEN 1 THEN @CustomerShipToTypeApplicationStringGuid ELSE ISNULL(@CustomerShipToTypeApplicationStringGuid,[CustomerShipToTypeApplicationStringGuid]) END)
		,	[Contact1Name]=(CASE ISNULL(@NullOverrideContact1Name,0) WHEN 1 THEN @Contact1Name ELSE ISNULL(@Contact1Name,[Contact1Name]) END)
		,	[Contact1Address1]=(CASE ISNULL(@NullOverrideContact1Address1,0) WHEN 1 THEN @Contact1Address1 ELSE ISNULL(@Contact1Address1,[Contact1Address1]) END)
		,	[Contact1Address2]=(CASE ISNULL(@NullOverrideContact1Address2,0) WHEN 1 THEN @Contact1Address2 ELSE ISNULL(@Contact1Address2,[Contact1Address2]) END)
		,	[Contact1City]=(CASE ISNULL(@NullOverrideContact1City,0) WHEN 1 THEN @Contact1City ELSE ISNULL(@Contact1City,[Contact1City]) END)
		,	[Contact1State]=(CASE ISNULL(@NullOverrideContact1State,0) WHEN 1 THEN @Contact1State ELSE ISNULL(@Contact1State,[Contact1State]) END)
		,	[Contact1Zip]=(CASE ISNULL(@NullOverrideContact1Zip,0) WHEN 1 THEN @Contact1Zip ELSE ISNULL(@Contact1Zip,[Contact1Zip]) END)
		,	[Contact1Country]=(CASE ISNULL(@NullOverrideContact1Country,0) WHEN 1 THEN @Contact1Country ELSE ISNULL(@Contact1Country,[Contact1Country]) END)
		,	[Contact1PhoneOffice]=(CASE ISNULL(@NullOverrideContact1PhoneOffice,0) WHEN 1 THEN @Contact1PhoneOffice ELSE ISNULL(@Contact1PhoneOffice,[Contact1PhoneOffice]) END)
		,	[Contact1Fax]=(CASE ISNULL(@NullOverrideContact1Fax,0) WHEN 1 THEN @Contact1Fax ELSE ISNULL(@Contact1Fax,[Contact1Fax]) END)
		,	[Contact1EmailAddress]=(CASE ISNULL(@NullOverrideContact1EmailAddress,0) WHEN 1 THEN @Contact1EmailAddress ELSE ISNULL(@Contact1EmailAddress,[Contact1EmailAddress]) END)
		,	[Contact2Name]=(CASE ISNULL(@NullOverrideContact2Name,0) WHEN 1 THEN @Contact2Name ELSE ISNULL(@Contact2Name,[Contact2Name]) END)
		,	[Contact2Address1]=(CASE ISNULL(@NullOverrideContact2Address1,0) WHEN 1 THEN @Contact2Address1 ELSE ISNULL(@Contact2Address1,[Contact2Address1]) END)
		,	[Contact2Address2]=(CASE ISNULL(@NullOverrideContact2Address2,0) WHEN 1 THEN @Contact2Address2 ELSE ISNULL(@Contact2Address2,[Contact2Address2]) END)
		,	[Contact2City]=(CASE ISNULL(@NullOverrideContact2City,0) WHEN 1 THEN @Contact2City ELSE ISNULL(@Contact2City,[Contact2City]) END)
		,	[Contact2State]=(CASE ISNULL(@NullOverrideContact2State,0) WHEN 1 THEN @Contact2State ELSE ISNULL(@Contact2State,[Contact2State]) END)
		,	[Contact2Zip]=(CASE ISNULL(@NullOverrideContact2Zip,0) WHEN 1 THEN @Contact2Zip ELSE ISNULL(@Contact2Zip,[Contact2Zip]) END)
		,	[Contact2Country]=(CASE ISNULL(@NullOverrideContact2Country,0) WHEN 1 THEN @Contact2Country ELSE ISNULL(@Contact2Country,[Contact2Country]) END)
		,	[Contact2PhoneOffice]=(CASE ISNULL(@NullOverrideContact2PhoneOffice,0) WHEN 1 THEN @Contact2PhoneOffice ELSE ISNULL(@Contact2PhoneOffice,[Contact2PhoneOffice]) END)
		,	[Contact2Fax]=(CASE ISNULL(@NullOverrideContact2Fax,0) WHEN 1 THEN @Contact2Fax ELSE ISNULL(@Contact2Fax,[Contact2Fax]) END)
		,	[Contact2EmailAddress]=(CASE ISNULL(@NullOverrideContact2EmailAddress,0) WHEN 1 THEN @Contact2EmailAddress ELSE ISNULL(@Contact2EmailAddress,[Contact2EmailAddress]) END)
		,	[Contact1PhoneMobile]=(CASE ISNULL(@NullOverrideContact1PhoneMobile,0) WHEN 1 THEN @Contact1PhoneMobile ELSE ISNULL(@Contact1PhoneMobile,[Contact1PhoneMobile]) END)
		,	[Contact2PhoneMobile]=(CASE ISNULL(@NullOverrideContact2PhoneMobile,0) WHEN 1 THEN @Contact2PhoneMobile ELSE ISNULL(@Contact2PhoneMobile,[Contact2PhoneMobile]) END)
		,	[_MasterRecordGuid]=(CASE ISNULL(@NullOverride_MasterRecordGuid,0) WHEN 1 THEN @_MasterRecordGuid ELSE ISNULL(@_MasterRecordGuid,[_MasterRecordGuid]) END)
		,	[Note]=(CASE ISNULL(@NullOverrideNote,0) WHEN 1 THEN @Note ELSE ISNULL(@Note,[Note]) END)
		WHERE	CompanyGuid=@CompanyGuid;
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblCompanies]           
		WHERE CompanyGuid=@CompanyGuid;
	
 
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
						+ 'Procedure Name: gsp_CompaniesUpdateByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
