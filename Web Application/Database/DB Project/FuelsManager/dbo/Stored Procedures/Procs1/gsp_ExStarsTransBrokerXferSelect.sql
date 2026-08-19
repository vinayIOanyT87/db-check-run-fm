CREATE PROCEDURE [dbo].[gsp_ExStarsTransBrokerXferSelect]
	 @SiteGuid UNIQUEIDENTIFIER=NULL
	,@ManagerCompanyGuid UNIQUEIDENTIFIER=NULL
	-- if @ToOwnerCompanyGuid is null it matches all owners
	,@ToOwnerCompanyGuid UNIQUEIDENTIFIER=NULL
	,@StartDate Date -- whole day 
	,@EndDate Date   --  whole day 
	,@UpdatedSince Date -- for use with supplemental data only
	,@UseFromToOwners bit
AS
BEGIN
	DECLARE @TransTypeTransfer varchar(30) = 'Transfer'
	DECLARE @IsBrokerTransfer varchar(30) = 'BRKTFR'
	-- Modify EndDate so that it includes every last second
	DECLARE @EndDateTime DATETIME =  DATEADD( SECOND, -1, DATEADD( DAY, 1,  cast( @EndDate as DATETIME)));

	IF( @UseFromToOwners = 1)
		SELECT
			AliasName
			, SiteGuid
			, TransactionAliasGuid
			, SubType
			, DebitTransGuid
			, CreditTransGuid
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, DocumentNumber
			, ProductGuid
			, ManagerCompanyGuid
			, CarrierCompanyGuid
			, ShipperCompanyGuid
			, FromOwnerCompanyGuid
			, null as ToOwnerCompanyGuid
			, SupplierCompanyGuid
			, ManagerID
			, CarrierCompanyId
			, ShipperCompanyId
			, FromOwnerId
			, null as ToOwnerId
			, FromOwnerCode
			, '' as ToOwnerCode
			, ManagerState
			, SupplierId
			, ManagerFederalId
			, SUM( NetQuantity) as NetQuantity
			, SUM( GrossQuantity) as GrossQuantity
			, EquipmentType
			, EquipmentRegistrationId
			, EquipmentSerialNumber

		FROM  [dbo].[vw_ExStarsTransferTransactions]

		WHERE  AliasName=@TransTypeTransfer
			AND BrokerTransferIndicator=@IsBrokerTransfer
			AND UpdatedDate > @UpdatedSince
			AND SiteGuid=@SiteGuid
			AND ManagerCompanyGuid=@ManagerCompanyGuid
			AND ISNULL(@ToOwnerCompanyGuid,  ToOwnerCompanyGuid) = ToOwnerCompanyGuid
			AND InventoryDate between @StartDate and @EndDateTime
			AND NetQuantity <> 0.0
			AND TaxCode <> ''

		GROUP BY
			AliasName
			, SubType
			, SiteGuid
			, TransactionAliasGuid
			, DebitTransGuid
			, CreditTransGuid
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, DocumentNumber
			, ProductGuid
			, ManagerCompanyGuid
			, CarrierCompanyGuid
			, ShipperCompanyGuid
			, FromOwnerCompanyGuid
			--, ToOwnerCompanyGuid
			, SupplierCompanyGuid
			, ManagerID
			, ShipperCompanyId
			, CarrierCompanyId
			, FromOwnerId
			--, ToOwnerId
			, FromOwnerCode
			--, ToOwnerCode
			, ManagerState
			, SupplierId
			, ManagerFederalId
			, EquipmentType
			, EquipmentRegistrationId
			, EquipmentSerialNumber
			, UpdatedDate

		ORDER BY 
			 ManagerID
			, ToOwnerId
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, [DocumentNumber]
			, ManagerFederalId
			, EquipmentType

	ELSE
		SELECT
			AliasName
			, SiteGuid
			, TransactionAliasGuid
			, SubType
			, DebitTransGuid
			, CreditTransGuid
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, DocumentNumber
			, ProductGuid
			, ManagerCompanyGuid
			, CarrierCompanyGuid
			, ShipperCompanyGuid
			, FromOwnerCompanyGuid
			, ToOwnerCompanyGuid
			, SupplierCompanyGuid
			, ManagerID
			, CarrierCompanyId
			, ShipperCompanyId
			, FromOwnerId
			, ToOwnerId
			, FromOwnerCode
			, ToOwnerCode
			, ManagerState
			, SupplierId
			, ManagerFederalId
			, SUM( NetQuantity) as NetQuantity
			, SUM( GrossQuantity) as GrossQuantity
			, EquipmentType
			, EquipmentRegistrationId
			, EquipmentSerialNumber

		FROM  [dbo].[vw_ExStarsTransferTransactions]

		WHERE  AliasName=@TransTypeTransfer
			AND BrokerTransferIndicator=@IsBrokerTransfer
			AND UpdatedDate > @UpdatedSince
			AND SiteGuid=@SiteGuid
			AND ManagerCompanyGuid=@ManagerCompanyGuid
			AND ISNULL(@ToOwnerCompanyGuid,  ToOwnerCompanyGuid) = ToOwnerCompanyGuid
			AND InventoryDate between @StartDate and @EndDateTime
			AND NetQuantity <> 0.0
			AND TaxCode <> ''
		GROUP BY
			AliasName
			, SubType
			, SiteGuid
			, TransactionAliasGuid
			, DebitTransGuid
			, CreditTransGuid
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, DocumentNumber
			, ProductGuid
			, ManagerCompanyGuid
			, CarrierCompanyGuid
			, ShipperCompanyGuid
			, FromOwnerCompanyGuid
			, ToOwnerCompanyGuid
			, SupplierCompanyGuid
			, ManagerID
			, ShipperCompanyId
			, CarrierCompanyId
			, FromOwnerId
			, ToOwnerId
			, FromOwnerCode
			, ToOwnerCode
			, ManagerState
			, SupplierId
			, ManagerFederalId
			, EquipmentType
			, EquipmentRegistrationId
			, EquipmentSerialNumber
			, UpdatedDate
		ORDER BY 
			 ManagerID
			, ToOwnerId
			, ProductId
			, ReportYear
			, ReportMonth
			, ReportDay
			, [DocumentNumber]
			, ManagerFederalId
			, EquipmentType
END