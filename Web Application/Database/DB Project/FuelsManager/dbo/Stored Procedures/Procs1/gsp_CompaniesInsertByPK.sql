CREATE PROCEDURE [dbo].[gsp_CompaniesInsertByPK]
(
		@CompanyGuid uniqueidentifier=NULL OUTPUT
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
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
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
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_CompaniesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1022767 -05:00
	-- Purpose: Insert into table [dbo].[tblCompanies]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @CompanyGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblCompanies] 
		(
			[CompanyGuid]
		,	[ID]
		,	[Code]
		,	[Name]
		,	[Address1]
		,	[Address2]
		,	[City]
		,	[State]
		,	[Zip]
		,	[Country]
		,	[Phone]
		,	[FAX]
		,	[EmergencyContact]
		,	[EmergencyPhone]
		,	[FlightPrefix]
		,	[EffectiveDate]
		,	[ExpirationDate]
		,	[OnHold]
		,	[PickupFLights]
		,	[StockTrack]
		,	[SufferLossGain]
		,	[LowStockWarning]
		,	[LockedOut]
		,	[LockedOutReason]
		,	[LockedOutDate]
		,	[ReceivableAccount]
		,	[RefinerCode]
		,	[LastActivityDate]
		,	[CreditOK]
		,	[AdditiveAccounting]
		,	[PurchaseOrderRequired]
		,	[EPANumber]
		,	[FederalID]
		,	[TaxNumber]
		,	[FlushPermitted]
		,	[PumpOffPermitted]
		,	[DeliveryToTerminalPermitted]
		,	[LicenseNumber]
		,	[LicenseExpiration]
		,	[InsuranceCompany]
		,	[InsurancePolicy]
		,	[LiabilityAmount]
		,	[HazardousMaterialExclusion]
		,	[InsuranceExpiration]
		,	[AllowDriverEntry]
		,	[PINRequired]
		,	[MaximumVehicleWeight]
		,	[WeightUnits]
		,	[AccountNumber]
		,	[SCACCode]
		,	[DisableOwnerAllocationsCheck]
		,	[DisableShipperAllocationsCheck]
		,	[DisableBillToAllocationsCheck]
		,	[DisableShipToAllocationsCheck]
		,	[LoadRackDisplayText]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[IATAGuid]
		,	[ShipperTypeApplicationStringGuid]
		,	[CustomerBillToTypeApplicationStringGuid]
		,	[CustomerShipToTypeApplicationStringGuid]
		,	[Contact1Name]
		,	[Contact1Address1]
		,	[Contact1Address2]
		,	[Contact1City]
		,	[Contact1State]
		,	[Contact1Zip]
		,	[Contact1Country]
		,	[Contact1PhoneOffice]
		,	[Contact1Fax]
		,	[Contact1EmailAddress]
		,	[Contact2Name]
		,	[Contact2Address1]
		,	[Contact2Address2]
		,	[Contact2City]
		,	[Contact2State]
		,	[Contact2Zip]
		,	[Contact2Country]
		,	[Contact2PhoneOffice]
		,	[Contact2Fax]
		,	[Contact2EmailAddress]
		,	[Contact1PhoneMobile]
		,	[Contact2PhoneMobile]
		,	[_MasterRecordGuid]
		,	[Note]
		)
		VALUES
		(
			@CompanyGuid
		,	@ID
		,	@Code
		,	@Name
		,	@Address1
		,	@Address2
		,	@City
		,	@State
		,	@Zip
		,	@Country
		,	@Phone
		,	@FAX
		,	@EmergencyContact
		,	@EmergencyPhone
		,	@FlightPrefix
		,	@EffectiveDate
		,	@ExpirationDate
		,	@OnHold
		,	@PickupFLights
		,	@StockTrack
		,	@SufferLossGain
		,	@LowStockWarning
		,	@LockedOut
		,	@LockedOutReason
		,	@LockedOutDate
		,	@ReceivableAccount
		,	@RefinerCode
		,	@LastActivityDate
		,	@CreditOK
		,	@AdditiveAccounting
		,	@PurchaseOrderRequired
		,	@EPANumber
		,	@FederalID
		,	@TaxNumber
		,	@FlushPermitted
		,	@PumpOffPermitted
		,	@DeliveryToTerminalPermitted
		,	@LicenseNumber
		,	@LicenseExpiration
		,	@InsuranceCompany
		,	@InsurancePolicy
		,	@LiabilityAmount
		,	@HazardousMaterialExclusion
		,	@InsuranceExpiration
		,	@AllowDriverEntry
		,	@PINRequired
		,	@MaximumVehicleWeight
		,	@WeightUnits
		,	@AccountNumber
		,	@SCACCode
		,	@DisableOwnerAllocationsCheck
		,	@DisableShipperAllocationsCheck
		,	@DisableBillToAllocationsCheck
		,	@DisableShipToAllocationsCheck
		,	@LoadRackDisplayText
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@IATAGuid
		,	@ShipperTypeApplicationStringGuid
		,	@CustomerBillToTypeApplicationStringGuid
		,	@CustomerShipToTypeApplicationStringGuid
		,	@Contact1Name
		,	@Contact1Address1
		,	@Contact1Address2
		,	@Contact1City
		,	@Contact1State
		,	@Contact1Zip
		,	@Contact1Country
		,	@Contact1PhoneOffice
		,	@Contact1Fax
		,	@Contact1EmailAddress
		,	@Contact2Name
		,	@Contact2Address1
		,	@Contact2Address2
		,	@Contact2City
		,	@Contact2State
		,	@Contact2Zip
		,	@Contact2Country
		,	@Contact2PhoneOffice
		,	@Contact2Fax
		,	@Contact2EmailAddress
		,	@Contact1PhoneMobile
		,	@Contact2PhoneMobile
		,	@_MasterRecordGuid
		,	@Note
		)
 
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
						+ 'Procedure Name: gsp_CompaniesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
