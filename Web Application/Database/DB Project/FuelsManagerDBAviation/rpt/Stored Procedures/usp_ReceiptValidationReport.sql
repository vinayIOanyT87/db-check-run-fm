CREATE PROCEDURE [rpt].[usp_ReceiptValidationReport] 
(
	@SiteGuid UNIQUEIDENTIFIER
	,@UserGuid UNIQUEIDENTIFIER
	,@FromDate DATETIMEOFFSET(7)
	,@ToDate DATETIMEOFFSET(7)
	,@Managers NVARCHAR(MAX)
	,@Owners NVARCHAR(MAX)
	,@Product NVARCHAR(MAX)
	,@Suppliers NVARCHAR(MAX)
	,@Sites NVARCHAR(MAX)

)
 AS
BEGIN
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

	BEGIN TRY	

	-- Trim off passed in hours and minute, add seconds until end of day
	SET @ToDate = DATEADD(second,86399,DATEADD(dd, DATEDIFF(dd, 0,@ToDate), 0))

	DECLARE @SiteGroupLevelVolumeUnitIndex INT
	DECLARE @SiteGroupLevelVolumeDecimalPlaces INT

	SELECT @SiteGroupLevelVolumeUnitIndex = ISNULL(VolumeUnitIndex, 46),
		@SiteGroupLevelVolumeDecimalPlaces = ISNULL(VolumeDecimalPlaces, 0)
	FROM tblSites 
	WHERE SiteGuid = @SiteGuid

	SELECT 
		 t.TransID
		,t.AliasName
		,t.Site
	    ,t.ManagerID
		,t.InventoryDate
		,t.SupplierID
		,t.TransDateTime
		,t.ShipperID
		,t.OwnerID
		,t.CarrierID
		,l.DocumentNumber
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.GrossQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS GrossQuantity
		,dbo.udf_ConvertFromSIUnits(abs(ISNULL(l.NetQuantity,0.0)), @SiteGroupLevelVolumeUnitIndex, @SiteGroupLevelVolumeDecimalPlaces) AS NetQuantity
		,l. Product
		,l.ProductCode
		,u.UserData4 
		,t.SourceCompanyEquipmentID1
		,l.SourceCompanyEquipmentID
		,l.SourceEquipmentType
		,l.SourceEquipmentGuid
		,@SiteGroupLevelVolumeDecimalPlaces AS VolumeDecimalPlaces
		,te.EngineeringUnitName AS VolumeEngUnitName

	FROM  tblTransactions t WITH(nolock)
	INNER JOIN tblTransactionLineItems l with(nolock) On t.TransactionGuid = l.TransactionGuid
	INNER JOIN lookup.tblEngineeringUnit te on @SiteGroupLevelVolumeUnitIndex = te.EngineeringUnitIndex
	LEFT JOIN tblTransactionUserData u with(nolock) On	t.TransactionGuid = u.TransactionGuid					
	WHERE 1=1
	AND	l.DeleteFlag = cast(0 AS bit)
	AND t.DeleteFlag = cast(0 AS bit)
	AND t.InventoryDate >= @FromDate AND t.InventoryDate < @ToDate 
	AND t.LookupTransTypeIndex = 8 -- T8_Receipt
	AND l.ProductGuid = @Product
	AND t.SiteGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Sites) c)
	AND t.ManagerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Managers) c)
	AND t.OwnerCompanyGuid in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Owners) c)
	AND t.SupplierCompanyGuid  in (SELECT c.Guid FROM rpt.udf_GetTableFromStringList(@Suppliers) c)
			AND EXISTS (SELECT *
				FROM (SELECT * FROM [dbo].[udf_AuthorizedCompaniesGuid](@SiteGuid, @UserGuid)) authorizedCompaniesGuids 
				WHERE authorizedCompaniesGuids.CompanyGuid IS NULL
				OR authorizedCompaniesGuids.CompanyGuid IN 
				(t.ShipToCompanyGuid, t.SupplierCompanyGuid, t.ShipperCompanyGuid, t.OwnerCompanyGuid, t.ManagerCompanyGuid, t.CarrierCompanyGuid, t.BillToCompanyGuid))

	Order by l.Product, te.EngineeringUnitName, t.Site, t.ManagerID ,t.OwnerID,t.SupplierID,t.InventoryDate

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
						+ 'Procedure Name: [rpt].usp_ReceiptValidationReport' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END



