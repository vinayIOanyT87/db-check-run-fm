
CREATE PROCEDURE [dbo].[usp_OrderSummaryList]
	@AliasName NVARCHAR (200), 
	@ManagerID NVARCHAR (100), 
	@OwnerID NVARCHAR (100), 
	@Product NVARCHAR (30), 
	@Carrier NVARCHAR (100), 
	@ShipTo NVARCHAR (100), 
	@BillTo NVARCHAR (100), 
	@Shipper NVARCHAR (100), 
	@Status INT, 
	@LookupTransTypeIndex SMALLINT, 
	@LoginSiteGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER, 
	@UserGuid UNIQUEIDENTIFIER, 
	@MoreWhereClause NVARCHAR (MAX)
AS
BEGIN

	SET NOCOUNT ON

	DECLARE @TempStr nvarchar(max)

	SET @TempStr = 'DECLARE @AuthorizedCompanies TABLE (
		[ID] [nvarchar] (100) NOT NULL
	);

	INSERT INTO @AuthorizedCompanies(ID) SELECT * FROM dbo.udf_AuthorizedCompanies('''+CAST(@LoginSiteGuid AS VARCHAR(50))+''','''+CAST(@SiteGuid AS VARCHAR(50))+''','''+CAST(@UserGuid AS VARCHAR(50))+''')'

	if ( @Product = '' )
	BEGIN
		SET @TempStr = @TempStr + '
		SELECT TOP 500
				A.TransactionID,
				A.TransactionAlias,
				A.LookupTransactionStatusIndex,
				A.TransactionDate,
				A.InventoryDate,
				A.SupplierID, 
				A.ManagerID, 
				A.OwnerID, 
				A.BillToID,  
				A.ShipperID, 
				A.ShipToID, 
				A.CarrierID,
				A.DocumentNumber,
				A.PONumber,
				A.ScheduledDate,
				A.DeleteFlag,
				A.EffectiveDate,
				A.ExpirationDate,
				A.Site,
				A.TransStatus,
				A.ETA,
				A.RequestedDeliveryDate,
				A.ShipmentNumber,
				A.OperatorID,
				A.DestinationRegistrationID1,
				A.DestinationRegistrationID2,
				A.DestinationRegistrationID3,
				A.UserData1,
				A.UserData2,
				A.UserData3,
				A.UserData4,
				A.UserData5,
				A.UserData6,
				A.UserData7,
				A.UserData8,
				A.UserData9,
				A.UserData10,
				A.UserData11,
				A.UserData12,
				A.UserData13,
				A.UserData14,
				A.UserData15,
				A.UserData16,
				A.UserData17,
				A.UserData18,
				A.UserData19,
				A.UserData20,
				A.UserData21,
				A.UserData22,
				A.UserData23,
				A.UserData24,
				BillTo.Name AS BillToName, BillTo.Address1 AS BillToAddress, BillTo.City AS BillToCity, BillTo.State AS BillToState,
				ShipTo.Name AS ShipToName, ShipTo.Address1 AS ShipToAddress, ShipTo.City AS ShipToCity, ShipTo.State AS ShipToState,
				Carrier.Name AS CarrierName, Carrier.Address1 AS CarrierAddress, Carrier.City as CarrierCity, Carrier.State as CarrierState
			FROM (((vw_OrderSummary A 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) BillTo ON A.BillToCompanyGuid = BillTo.CompanyGuid OR A.BillToCompanyGuid = BillTo._MasterRecordGuid 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON A.ShipToCompanyGuid = ShipTo.CompanyGuid OR A.ShipToCompanyGuid = ShipTo._MasterRecordGuid 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Carrier ON A.CarrierCompanyGuid = Carrier.CompanyGuid OR A.CarrierCompanyGuid = Carrier._MasterRecordGuid 
				 )))'

	END 
	ELSE BEGIN
		SET @TempStr = @TempStr + '
			SELECT TOP 500
				A.TransactionID,
				A.TransactionAlias,
				A.LookupTransactionStatusIndex,
				A.TransactionDate,
				A.InventoryDate,
				A.SupplierID, 
				A.ManagerID, 
				A.OwnerID, 
				A.BillToID,  
				A.ShipperID, 
				A.ShipToID, 
				A.CarrierID,
				A.DocumentNumber,
				A.PONumber,
				A.ScheduledDate,
				A.DeleteFlag,
				A.EffectiveDate,
				A.ExpirationDate,
				A.Site,
				A.TransStatus,
				A.ETA,
				A.RequestedDeliveryDate,
				A.ShipmentNumber,
				A.OperatorID,
				A.DestinationRegistrationID1,
				A.DestinationRegistrationID2,
				A.DestinationRegistrationID3,
				A.UserData1,
				A.UserData2,
				A.UserData3,
				A.UserData4,
				A.UserData5,
				A.UserData6,
				A.UserData7,
				A.UserData8,
				A.UserData9,
				A.UserData10,
				A.UserData11,
				A.UserData12,
				A.UserData13,
				A.UserData14,
				A.UserData15,
				A.UserData16,
				A.UserData17,
				A.UserData18,
				A.UserData19,
				A.UserData20,
				A.UserData21,
				A.UserData22,
				A.UserData23,
				A.UserData24,
				BillTo.Name AS BillToName, BillTo.Address1 AS BillToAddress, BillTo.City AS BillToCity, BillTo.State AS BillToState,
				ShipTo.Name AS ShipToName, ShipTo.Address1 AS ShipToAddress, ShipTo.City AS ShipToCity, ShipTo.State AS ShipToState,
				Carrier.Name AS CarrierName, Carrier.Address1 AS CarrierAddress, Carrier.City as CarrierCity, Carrier.State as CarrierState
			FROM (((vw_OrderSummaryProduct A 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) BillTo ON A.BillToCompanyGuid = BillTo.CompanyGuid OR A.BillToCompanyGuid = BillTo._MasterRecordGuid 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) ShipTo ON A.ShipToCompanyGuid = ShipTo.CompanyGuid OR A.ShipToCompanyGuid = ShipTo._MasterRecordGuid 
				 LEFT JOIN (select SRM.* from erv.udf_GetCompanyRecordVersions (''' + CAST(@SiteGuid AS VARCHAR(50))+ ''') LKM inner join tblCompanies SRM on LKM.CompanyGuid = SRM.CompanyGuid) Carrier ON A.CarrierCompanyGuid = Carrier.CompanyGuid OR A.CarrierCompanyGuid = Carrier._MasterRecordGuid 
				 )))'
	END 

	SET @TempStr = @TempStr + ' WHERE 1=1 '

	if ( @Product <> '' )
	  BEGIN
		  SET @TempStr = @TempStr + ' AND (A.Product = '''+ @Product + ''')'
	  END

	/* Add Alias */
	IF @AliasName <> '' SET @TempStr = @TempStr + ' AND (A.TransactionAlias = '''+ @AliasName + ''')'

	/* Add Manager */
	IF @ManagerID <> '' SET @TempStr = @TempStr + ' AND (A.ManagerID = ''' + @ManagerID + ''')'

	/* Add Owner */
	IF @OwnerID <> '' SET @TempStr = @TempStr + ' AND (A.OwnerID = ''' + @OwnerID + ''')'

	/* Add Carrier */
	IF @Carrier <> '' SET @TempStr = @TempStr + ' AND (A.CarrierID = ''' + @Carrier + ''')'

	/* Add ShipTo */
	IF @ShipTo <> '' SET @TempStr = @TempStr + ' AND (A.ShipToID = ''' + @ShipTo + ''')'

	/* Add BillTo */
	IF @BillTo <> '' SET @TempStr = @TempStr + ' AND (A.BillToID = ''' + @BillTo + ''')'

	/* Add Shipper */
	IF @Shipper <> '' SET @TempStr = @TempStr + ' AND (A.ShipperID = ''' + @Shipper + ''')'

	/* Add Transaction Status */
	IF @Status <> -1 SET @TempStr = @TempStr + ' AND (A.LookupTransactionStatusIndex = ' + STR(@Status) + ')'

	/* Add Authorized Lists */
	IF @UserGuid IS NOT NULL SET @TempStr = @TempStr + ' AND (
			((A.CarrierID IN (Select * FROM @AuthorizedCompanies))
			OR (A.ShipperID IN (Select * FROM @AuthorizedCompanies))
			OR (A.ShipToID IN (Select * FROM @AuthorizedCompanies))
			OR (A.SupplierID IN (Select * FROM @AuthorizedCompanies))
			OR (A.ManagerID IN (Select * FROM @AuthorizedCompanies))
			OR (A.OwnerID IN (Select * FROM @AuthorizedCompanies))
			OR (A.BillToID IN (Select * FROM @AuthorizedCompanies))))
		'

	SET @TempStr = @TempStr + ' ' + @MoreWhereClause
	EXEC sp_executesql @TempStr

END

