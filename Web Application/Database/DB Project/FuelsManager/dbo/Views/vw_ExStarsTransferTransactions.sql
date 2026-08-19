CREATE VIEW [dbo].[vw_ExStarsTransferTransactions]
AS
	SELECT
		DT.AliasName
		, DT.TransId
		, DT.SubType
		, DT.SiteGuid
		, DT.TransactionAliasGuid
		, TA.LookupTransTypeIndex
		, DT.TransactionGuid as DebitTransGuid
		, CT.TransactionGuid as CreditTransGuid
		, P.ProductGuid
		, P.TaxCode 
		, ( select top 1 ISNULL( ProductID, '') as ProductId 
			from tblProducts p2 
			where CLI.ProductGuid = p2.ProductGuid) 
		  as ProductId
		, YEAR( DT.InventoryDate)  as ReportYear
		, MONTH( DT.InventoryDate) as ReportMonth
		, DAY( DT.InventoryDate)  as ReportDay
		, DT.InventoryDate
		, DT.[DocumentNumber] 
		, DT.ManagerCompanyGuid
		, DT.CarrierCompanyGuid
		, DT.ShipperCompanyGuid
		, DT.OwnerCompanyGuid as FromOwnerCompanyGuid
		, CT.OwnerCompanyGuid as ToOwnerCompanyGuid
		, DT.SupplierCompanyGuid
		, ISNULL( DT.ManagerID, '')  as ManagerID
		, ISNULL( DT.CarrierID, '') as CarrierCompanyId
		, ISNULL( DT.ShipperID, '') as ShipperCompanyId
		, ISNULL( DT.OwnerId, '') as FromOwnerId
		, ISNULL( CT.OwnerId, '') as ToOwnerId
		, ISNULL( DT.OwnerCode, '') as FromOwnerCode
		, ISNULL( CT.OwnerCode, '') as ToOwnerCode
		, ISNULL( MGR.[State], '') as ManagerState
		, ISNULL( DT.SupplierID, '') as SupplierId
		, ISNULL( MGR.FederalID, '') as ManagerFederalId
		--ConvertUNitIndex(46) = US_Gallon
		, [dbo].[udf_ConvertFromSIUnits]( ISNULL( CLI.NetQuantity, 0.0), 46, 0) as NetQuantity
		, [dbo].[udf_ConvertFromSIUnits]( ISNULL( CLI.GrossQuantity, 0.0), 46, 0) as GrossQuantity
		, ISNULL( CLI.SourceEquipmentType, '') as  EquipmentType
		, ISNULL( CLI.[SourceRegistrationID], '') as  EquipmentRegistrationId
		, ISNULL( CLI.[SourceSerialNumber], '') as EquipmentSerialNumber
		, ISNULL( UD.[UserData3], '') as BrokerTransferIndicator
		 ,CLI.UpdatedDate

	FROM 
		tblTransactions DT -- Debit Trx joined to Credit Trx
		INNER JOIN tblTransactions CT on CT.ConjoinTransId=DT.TransId 
		INNER JOIN tblTransactionAliases TA on TA.TransactionAliasGuid=DT.TransactionAliasGuid
		INNER JOIN tblTransactionLineItems CLI on CLI.TransactionGuid = CT.TransactionGuid
		INNER JOIN tblCompanies MGR on MGR.CompanyGuid = DT.ManagerCompanyGuid
		INNER JOIN tblProducts P on P.ProductGuid = CLI.ProductGuid
		LEFT  JOIN tblTransactionUserData UD on UD.TransactionGuid = DT.TransactionGuid

	WHERE DT.SubType='D' -- Debit
		AND CT.SubType='C'   -- Credit
		AND DT.OwnerCompanyGuid !=  CT.OwnerCompanyGuid