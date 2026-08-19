CREATE VIEW [dbo].[vw_ExStarsTransactionCombo]
AS
	

	SELECT
		T.AliasName
		, T.SiteGuid
		, T.TransactionGuid
		, T.TransId
		, T.SubType
		, P.ProductGuid
		, P.TaxCode 
		, P.AviationFuelFlag
		, P.GroundFuel
		, ( select top 1 ISNULL( ProductID, '') as ProductId 
			from tblProducts p2		
			where LI.ProductGuid = p2.ProductGuid) 
		  as ProductId
		, T.InventoryDate
		, YEAR( T.InventoryDate)  as ReportYear
		, MONTH( T.InventoryDate) as ReportMonth
		, DAY( T.InventoryDate)  as ReportDay
		, T.[DocumentNumber] 
		, T.ManagerCompanyGuid
		, T.CarrierCompanyGuid
		, T.ShipperCompanyGuid
		, T.OwnerCompanyGuid
		, T.SupplierCompanyGuid
		, T.ShipToCompanyGuid
		, ISNULL( T.ManagerID, '')  as ManagerID
		, ISNULL( T.CarrierID, '') as CarrierCompanyId
		, ISNULL( T.ShipperID, '') as ShipperCompanyId
		, ISNULL( T.OwnerId, '') as OwnerId
		, ISNULL( MGR.[State], '') as ManagerState
		, ISNULL( SUP.[State], '') as SupplierState
		, ISNULL( T.SupplierID, '') as SupplierId
		, ISNULL( T.ShipToID, '') as ShipToID
		, ISNULL( MGR.FederalID, '') as ManagerFederalId
		, ISNULL( SUP.FederalID, '') as SupplierFederalId
		, ISNULL( SHIPTO.FederalID, '') as ShipToFederalId
		, ISNULL( SHIPTO.State, '') as ShipToState
		--ConvertUNitIndex(46) = US_Gallon
		, [dbo].[udf_ConvertFromSIUnits]( ISNULL( LI.GrossQuantity, 0.0), 46, 0) as GrossQuantity
		, [dbo].[udf_ConvertFromSIUnits]( ISNULL( LI.NetQuantity, 0.0), 46, 0) as NetQuantity

		, T.Source1EquipmentGuid as SrcEquipmentGuid
		, T.Destination1EquipmentGuid as DestEquipmentGuid

		, ISNULL( ISNULL( ISNULL( T.SourceEquipmentType1  ,LI.SourceEquipmentType), SRCEQTYP.EqTypeName) , '')
			as SrcEquipmentType
		, ISNULL( ISNULL( T.SourceRegistrationID1 ,LI.SourceRegistrationID), '') as SrcEquipmentRegistrationId
		, ISNULL( ISNULL( T.SourceSerialNumber1   ,LI.SourceSerialNumber), '')   as SrcEquipmentSerialNumber		

		, ISNULL( ISNULL( ISNULL( T.DestinationEquipmentType1  ,LI.DestinationEquipmentType), DESTEQTYP.EqTypeName), '')
			as DestEquipmentType
		,ISNULL( ISNULL( T.DestinationRegistrationID1 ,LI.DestinationRegistrationID), '') as DestEquipmentRegistrationId
		,ISNULL( ISNULL( T.DestinationSerialNumber1   ,LI.DestinationSerialNumber), '')   as DestEquipmentSerialNumber

		-- For Alias Adjustment:
		--      UserData2  may contain 'blend' or 'Regrade'
		--      UserData4 = '1' if ADJUSTMENT_USED_AS_RECEIPT 
		--		UserData10 = 'CE' if Summary reporting
		, ISNULL( UD.UserData2, '') as UserData2
		, ISNULL( UD.UserData4, '') as UserData4
		, ISNULL( UD.UserData10, '') as UserData10
		, LI.UpdatedDate

	FROM 
		tblTransactions T
		INNER JOIN tblTransactionLineItems LI on LI.TransactionGuid = T.TransactionGuid
		INNER JOIN tblProducts P on P.ProductGuid = LI.ProductGuid
		LEFT JOIN tblCompanies MGR on MGR.CompanyGuid = T.ManagerCompanyGuid
		LEFT JOIN tblCompanies SUP on SUP.CompanyGuid = T.SupplierCompanyGuid
		LEFT JOIN tblCompanies SHIPTO on SHIPTO.CompanyGuid = T.ShipToCompanyGuid

		LEFT JOIN tblEquipment SRCEQ on SRCEQ.EquipmentGuid=T.Source1EquipmentGuid
		LEFT JOIN tblEquipmentTypes SRCEQTYP on SRCEQTYP.EquipmentTypeGuid=SRCEQ.EquipmentTypeGuid

		LEFT JOIN tblEquipment DESTEQ on DESTEQ.EquipmentGuid=T.Destination1EquipmentGuid
		LEFT JOIN tblEquipmentTypes DESTEQTYP on DESTEQTYP.EquipmentTypeGuid=DESTEQ.EquipmentTypeGuid

		LEFT JOIN tblTransactionUserData UD on UD.TransactionGuid = T.TransactionGuid

