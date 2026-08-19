CREATE PROCEDURE [dbo].[gsp_ExStarsTransSelect]
	/*
	 * For standard monthly report, the Start and End dates are the first and last day of a single mnth
	 * For Outgoing manager, the End Date is the last date of that manager, possibly not the end of the month
	 * For Incoming manager, the Start Date is his first day on the job, possibly not the 1st
	 */
	 @SiteGuid UNIQUEIDENTIFIER=NULL
	,@ManagerCompanyGuid UNIQUEIDENTIFIER=NULL
	,@StartDate Date -- whole day 
	,@EndDate Date   --  whole day 
	,@UpdatedSince DATETIME -- for use with supplemental data only
	,@Alias varchar(32) -- from tblTransactionAliases
AS
BEGIN

	-- Modify EndDate so that it includes every last second
	DECLARE @EndDateTime DATETIME =  DATEADD( SECOND, -1, DATEADD( DAY, 1,  cast( @EndDate as DATETIME)));
	SELECT
		AliasName
		, TransactionGuid
		, TransId
		, SubType
		, ProductGuid
		, AviationFuelFlag
		, GroundFuel
		, TaxCode
		, ProductId
		, ReportYear
		, ReportMonth
		, ReportDay
		, [DocumentNumber]
		, ManagerCompanyGuid
		, CarrierCompanyGuid
		, ShipperCompanyGuid
		, OwnerCompanyGuid
		, SupplierCompanyGuid
		, ShipToCompanyGuid
		, ManagerID
		, CarrierCompanyId
		, ShipperCompanyId
		, ShipToID
		, OwnerId
		, SupplierId
		, ManagerFederalId
		, SupplierFederalId
		, ShipToFederalId
		, ShipToState
		, NetQuantity
		, GrossQuantity
		, SrcEquipmentType
		, SrcEquipmentRegistrationId
		, SrcEquipmentSerialNumber
		, DestEquipmentType
		, DestEquipmentRegistrationId
		, DestEquipmentSerialNumber
		, UserData2
		, UserData4
		, UserData10
	FROM  [dbo].[vw_ExStarsTransactionCombo]
	WHERE  
		UpdatedDate > @UpdatedSince
		AND AliasName = @Alias
		AND SiteGuid=@SiteGuid
		AND ManagerCompanyGuid=@ManagerCompanyGuid
		AND InventoryDate between @StartDate and @EndDateTime
		AND NetQuantity <> 0.0
		AND TaxCode <> ''
		-- ref: Release v7.1 SP5/Core/ExSTARS Reporting Utility/Book_Adjustments.cpp    
		-- C_Rec_Book_Adjustments_TX1::GetDefaultSQL() ~ 111
		-- C_Rec_Book_Adjustments_TX2::GetDefaultSQL() ~ 249
		AND ( AliasName <> 'Adjustment' OR UserData2 in ('blend', 'Regrade') OR UserData10 = 'CE' )
	ORDER BY 
		SupplierId
		, OwnerId
		, TaxCode 
		, SrcEquipmentType
		, DestEquipmentType
		, ReportYear
		, ReportMonth
		, ReportDay

END