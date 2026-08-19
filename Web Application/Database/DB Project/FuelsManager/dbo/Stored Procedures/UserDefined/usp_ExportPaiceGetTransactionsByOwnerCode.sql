CREATE PROCEDURE [dbo].[usp_ExportPaiceGetTransactionsByOwnerCode]
(
	@OwnerCode NVARCHAR (10)
	,@AirCanadaOwnerCodes NVARCHAR(MAX)
	,@ExcludeManagers NVARCHAR(MAX)
	,@SiteList NVARCHAR(MAX)
	,@LatestRowVersion BIGINT
	,@BaselineDate DATE
)
/*
	Get transactions that need to be exported to PAICE system.  Only includes transactions that have not previously been sent
	Parameters provide filters for the query:
		@OwnerCode  			: Filter for owner company. If this is "AC" use the next parameter
		@AirCanadaOwnerCodes : A comma separated list of company IDs with the role of owner that are grouped with Air Canada
		@ExcludeManagers 		: A comma separated list of company IDs with the role of manager that will not be reported
		@SiteList				: A comma separated list of site IDs that will be reported.
		
*/
AS
BEGIN

	SET NOCOUNT OFF

	DECLARE @BeginningOfTime DATE = '1/1/2000'

	DECLARE @OwnerCompanyGuids TABLE (OwnerGuid UNIQUEIDENTIFIER)

	IF @OwnerCode = 'AC'
		BEGIN
			INSERT INTO @OwnerCompanyGuids (OwnerGuid)
				SELECT DISTINCT _MasterRecordGuid
				FROM tblCompanies c
				WHERE CODE IN (SELECT * from dbo.[udf_GetTableOfStringsFromStringList](@AirCanadaOwnerCodes))
		END
	ELSE
		BEGIN
			INSERT INTO @OwnerCompanyGuids (OwnerGuid)
				SELECT _MasterRecordGuid
				FROM tblCompanies c
				WHERE CODE = @OwnerCode
		END
	
	DECLARE @ExcludedManagers TABLE( ManagerGuid UNIQUEIDENTIFIER)
	INSERT INTO @ExcludedManagers (ManagerGuid)
		SELECT _MasterRecordGuid
		FROM tblCompanies c
		WHERE ID IN (SELECT * from dbo.[udf_GetTableOfStringsFromStringList](@ExcludeManagers))

	DECLARE @IncludedSites TABLE( SiteGuid UNIQUEIDENTIFIER)
	INSERT INTO @IncludedSites (SiteGuid)
		SELECT SiteGuid
		FROM tblSites s
		WHERE ID IN (	SELECT * FROM dbo.[udf_GetTableOfStringsFromStringList](@SiteList))

	DECLARE @CloseOutInventory TABLE( ManagerGuid UNIQUEIDENTIFIER, SiteGuid UNIQUEIDENTIFIER,  ProductGuid UNIQUEIDENTIFIER, CloseOutDate date)
	INSERT into @CloseOutInventory ( ManagerGuid , SiteGuid , ProductGuid, CloseOutDate )
		SELECT  ManagerCompanyGuid, SiteGuid ,ProductGuid, ISNULL(MAX(CloseoutDate), @BeginningOfTime) as CloseOutDate
	    FROM tblCloseoutInventory
		WHERE SiteGuid IN (SELECT * FROM @IncludedSites)
		AND  ManagerCompanyGuid NOT IN (SELECT * FROM @ExcludedManagers)
	    GROUP BY SiteGuid,ManagerCompanyGuid ,ProductGuid;



WITH Transactions_CTE (TransID, AliasName, SubType, Site, InventoryDate, SupplierID, SupplierCode, TransDateTime, OwnerID, OwnerCode, 
		ManagerCode, ConjoinTransID, RoutingID, NextStationIATAID, DocumentNumber, DeleteFlag, Product, ProductCode, GrossQuantity, 
		DestinationRegistrationID, UserData1, UserData16, TransactionGuid, HasBeenUpdated, TransVersion, UpdatedDate)
AS
(
	SELECT t.TransID
		, t.AliasName
		, t.SubType
		, t.Site
		, t.InventoryDate
		, t.SupplierID
		, t.SupplierCode
		, t.TransDateTime
		, CASE WHEN @OwnerCode = 'AC' THEN 'AC - Air Canada' ELSE t.OwnerID END AS 'OwnerID'
		, CASE WHEN @OwnerCode = 'AC' THEN 'AC' ELSE t.OwnerCode END AS 'OwnerCode'
		, t.ManagerCode
		, t.ConjoinTransID
		, t.RoutingID
		, t.NextStationIATAID
		, t.DocumentNumber
		, t.DeleteFlag
		, l.Product
		, l.ProductCode
		, l.GrossQuantity
		, l.DestinationRegistrationID
		, u.UserData1
		, u.UserData16
		, t.TransactionGuid
		, CASE WHEN t.CreatedDate = t.UpdatedDate THEN cast(0 as bit) ELSE cast(1 as bit) END AS HasBeenUpdated 
		, t.TransVersion 
		, t.UpdatedDate 
	FROM tblTransactions t 
	INNER JOIN tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid
	LEFT JOIN tblTransactionUserData u ON t.TransactionGuid = u.TransactionGuid
	--LEFT JOIN tblExportPaiceTransTracking trk ON t.TransID = trk.TransID
	LEFT JOIN @CloseOutInventory coi ON 
		    coi.SiteGuid=t.SiteGuid 
		AND coi.ManagerGuid=t.ManagerCompanyGuid 
		AND coi.ProductGuid=l.ProductGuid 
	WHERE CAST(t._RowVersion AS BIGINT) > @LatestRowVersion AND t.InventoryDate >= @BaselineDate
	AND t.OwnerCompanyGuid IN (SELECT * FROM @OwnerCompanyGuids)
	AND t.ManagerCompanyGuid NOT IN (SELECT * FROM @ExcludedManagers)
	AND t.SiteGuid IN (SELECT * FROM @IncludedSites)
	--Ignore closed out records. We consider a transaction closed out if there's a record in tblCloseoutInventory
	--matching the site, product, and manager and where the closeout date is on or after the transaction's inventory date.	
	AND ISNULL(coi.CloseOutDate, @BeginningOfTime) < t.InventoryDate
),	
	

ExportResultDetailsDate01_CTE(RecordID, Date01)
AS
(
	SELECT A.RecordID, 
	CASE WHEN InterfaceData01 IS NULL THEN NULL ELSE CAST(InterfaceData01 AS DATETIMEOFFSET(7)) END AS Date01  
	FROM  tblExportResultDetails A 
	INNER JOIN 
	(select 
	RecordID, 
	MAX(UpdatedDate) AS UpdatedDate 
	FROM  tblExportResultDetails  
	GROUP BY RecordID  ) A1 ON A.RecordID = A1.RecordID AND A.UpdatedDate = A1.UpdatedDate 
),

ExportResultDetailsNumber01_CTE(RecordID, Number01)
AS
(
	SELECT A.RecordID,  
	CASE WHEN InterfaceData02 IS NULL THEN NULL ELSE CAST(InterfaceData02 AS FLOAT) END AS Number01 
	FROM  tblExportResultDetails A 
	INNER JOIN 
	(select 
	RecordID, 
	MIN(UpdatedDate) AS UpdatedDate 
	FROM  tblExportResultDetails  
	GROUP BY RecordID  ) A1 ON A.RecordID = A1.RecordID AND A.UpdatedDate = A1.UpdatedDate 
	) 

SELECT TransID, AliasName, SubType, Site, InventoryDate, SupplierID, SupplierCode, TransDateTime, OwnerID, OwnerCode, 
		ManagerCode, ConjoinTransID, RoutingID, NextStationIATAID, DocumentNumber, DeleteFlag, Product, ProductCode, GrossQuantity, 
		DestinationRegistrationID, UserData1, UserData16, TransactionGuid, HasBeenUpdated, TransVersion, C2.Date01, C3.Number01 
from Transactions_CTE C1 
LEFT JOIN ExportResultDetailsDate01_CTE C2 on C1.TransID = C2.RecordID 
LEFT JOIN ExportResultDetailsNumber01_CTE C3 on C1.TransID = C3.RecordID 
WHERE (   (     (NULLIF(DeleteFlag, 0) IS NULL)   -- not a deleted record
			  AND (   (DATE01 IS NULL)              -- AND Never sent 
			       OR (UpdatedDate >DATE01 )))    --     OR  Sent prior to update/delete
		 OR ( DeleteFlag = 1 AND DATE01 IS NOT NULL AND UpdatedDate > DATE01))
	ORDER BY DocumentNumber

END