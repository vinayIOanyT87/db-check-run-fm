/*
	DROP PROCEDURE [rpt].[usp_GetFactTransactionDetails]

	EXEC rpt.usp_GetFactTransactionDetails @UserGuid=N'00000000-0000-0000-0000-000000000002',@SiteId=N'[Site].[Site Id].&[Baltimore]',@ProductId=NULL,@TransactionAlias=NULL,@InventoryDateKey=NULL,@InventoryBeginDate=NULL,@InventoryEndDate=NULL,@TransactionDateKey=NULL,@TransactionBeginDate='2018-10-01 00:00:00',@TransactionEndDate=N'10/29/2023 12:00:00 AM',@IsInvalidTerminalTime=N'True', @IsDeleted=N'False'

	EXEC [rpt].[usp_GetFactTransactionDetails] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', NULL, '08/01/2018', '08/30/2018', NULL, NULL, NULL, 'True', 'False'

	EXEC [rpt].[usp_GetFactTransactionDetails] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[4]&[12]', NULL, NULL, NULL, NULL, NULL, 'True', 'False'

	EXEC [rpt].[usp_GetFactTransactionDetails] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].[ALL]', NULL, NULL, NULL, NULL, NULL, NULL, NULL
*/
CREATE PROCEDURE [rpt].[usp_GetFactTransactionDetails]
(
	@UserGuid uniqueidentifier,
	@SiteId nvarchar(1000),
	@ProductId nvarchar(1000),
	@TransactionAlias nvarchar(1000),
	@InventoryDateKey nvarchar(1000),
	@InventoryBeginDate datetime,
	@InventoryEndDate datetime,
	@TransactionDateKey nvarchar(1000),
	@TransactionBeginDate datetime,
	@TransactionEndDate datetime,
	@IsInvalidTerminalTime nvarchar(100),
	@IsDeleted nvarchar(100)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [rpt].[usp_GetFactTransactionDetails]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve a detailed list of FactTransaction records.
	-- Notes:
	-- 1. @UserGuid: Guid of the User running the report
	-- 2. @SiteId: Comma-separated list of SiteId
	-- 3. @ProductId: Comma-separated list of ProductId
	-- 4. @TransactionAlias: Comma-separated list of TransactionAlias
	-- 5. @InventoryDateSKey: Date Dimension formatted date key, e.g. [Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[4]&[12]
	-- 6. @InventoryBeginDate: Begin Inventory Date
	-- 7. @InventoryEndDate: End Inventory Date
	-- 8. @TransactionDateSKey: Date Dimension formatted date key, e.g. [Transaction Date].[Date Key].&[20230204]
	-- 9. @TransactionBeginDate: Begin Inventory Date
	-- 10. @TransactionEndDate: End Inventory Date
	-- 11. @IsInvalidTerminalTime: 0: Limit the query to valid terminal times only; 1: Limit the query to invalid terminal times only; 
	-- 12. @IsDeleted: 0: Limit the query to non-deleted transactions records only; 1: Limit the query to delete transaction records only;
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		DECLARE @userKey nvarchar(50)
		SET @userKey = CONVERT(nvarchar(50), @UserGuid)

		SELECT @InventoryDateKey = IIF(LEN(@InventoryDateKey) = 0, NULL, @InventoryDateKey)
		SELECT @TransactionDateKey = IIF(LEN(@TransactionDateKey) = 0, NULL, @TransactionDateKey)

		DECLARE @tblAuthorisedCompanies TABLE ([Id] [nvarchar] (100) NOT NULL);
		INSERT INTO @tblAuthorisedCompanies SELECT * FROM [rpt].[udf_GetAuthorisedCompanies](@userKey)

		DECLARE @tblAuthorisedSites TABLE ([Id] [nvarchar] (100) NOT NULL);
		INSERT INTO @tblAuthorisedSites SELECT * FROM [rpt].[udf_GetAuthorisedSites](@userKey)

		DECLARE @tblDimSiteValue TABLE
		(
			[ParameterStrValue] [nvarchar](100) NULL
		)
		INSERT INTO @tblDimSiteValue SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@SiteId, 0)
		IF ((CHARINDEX ('[ALL]', @SiteId) > 0) OR ((SELECT COUNT(*) FROM @tblDimSiteValue) = 0))
		BEGIN
			SET @SiteId = NULL
		END

		DECLARE @tblDimProductValue TABLE
		(
			[ParameterStrValue] [nvarchar](100) NULL
		)
		INSERT INTO @tblDimProductValue SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@ProductId, 0)
		IF ((CHARINDEX ('[ALL]', @ProductId) > 0) OR ((SELECT COUNT(*) FROM @tblDimProductValue) = 0))
		BEGIN
			SET @ProductId = NULL
		END

		DECLARE @tblDimTransactionAliasValue TABLE
		(
			[ParameterStrValue] [nvarchar](100) NULL
		)
		INSERT INTO @tblDimTransactionAliasValue SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@TransactionAlias, 0)
		IF ((CHARINDEX ('[ALL]', @TransactionAlias) > 0) OR ((SELECT COUNT(*) FROM @tblDimTransactionAliasValue) = 0))
		BEGIN
			SET @TransactionAlias = NULL
		END

		DECLARE @beginInvDateSKey int = NULL
		DECLARE @endInvDateSKey int = NULL		
		IF (@InventoryDateKey IS NOT NULL)
		BEGIN
			DECLARE @tblDimInvDateValue TABLE
			(
				[ParameterIndex] [int] NULL,
				[BeginDateKey] [int] NULL,
				[EndDateKey] [int] NULL
			)
			INSERT INTO @tblDimInvDateValue SELECT ParameterIndex, BeginDateKey, EndDateKey FROM [rpt].[udf_GetDimensionParameterValues] (@InventoryDateKey, 1)			
			SELECT @beginInvDateSKey = BeginDateKey, @endInvDateSKey = EndDateKey FROM @tblDimInvDateValue WHERE ParameterIndex = 0  -- only support a single date range
			IF ((CHARINDEX ('[ALL]', @InventoryDateKey) > 0) OR ((SELECT COUNT(*) FROM @tblDimInvDateValue) = 0))
			BEGIN
				SET @InventoryDateKey = NULL
			END
		END
		IF (@InventoryBeginDate IS NOT NULL)
		BEGIN
			SET @beginInvDateSKey = dbo.udf_ConvertToDateKey(@InventoryBeginDate)			
		END
		IF (@InventoryEndDate IS NOT NULL)
		BEGIN
			SET @endInvDateSKey = dbo.udf_ConvertToDateKey(@InventoryEndDate)			
		END


		DECLARE @beginTransDateSKey int = NULL
		DECLARE @endTransDateSKey int = NULL		
		IF (@beginInvDateSKey IS NULL and @endInvDateSKey IS NULL AND @TransactionDateKey IS NOT NULL)
		BEGIN
			DECLARE @tblDimTransDateValue TABLE
			(
				[ParameterIndex] [int] NULL,
				[BeginDateKey] [int] NULL,
				[EndDateKey] [int] NULL
			)
			INSERT INTO @tblDimTransDateValue SELECT ParameterIndex, BeginDateKey, EndDateKey FROM [rpt].[udf_GetDimensionParameterValues] (@TransactionDateKey, 1)			
			SELECT @beginTransDateSKey = BeginDateKey, @endTransDateSKey = EndDateKey FROM @tblDimTransDateValue WHERE ParameterIndex = 0  -- only support a single date range
			IF ((CHARINDEX ('[ALL]', @TransactionDateKey) > 0) OR ((SELECT COUNT(*) FROM @tblDimTransDateValue) = 0))
			BEGIN
				SET @TransactionDateKey = NULL
			END
		END
		IF (@TransactionBeginDate IS NOT NULL)
		BEGIN
			SET @beginTransDateSKey = dbo.udf_ConvertToDateKey(@TransactionBeginDate)			
		END
		IF (@TransactionEndDate IS NOT NULL)
		BEGIN
			SET @endTransDateSKey = dbo.udf_ConvertToDateKey(@TransactionEndDate)			
		END

		
		SELECT TOP(1000) a.InventoryDateSKey, a.SKey, 
		a.TransactionKey, a.TransID, a.TransactionLineItemKey, a.TransactionSubLineItemKey,
		b.SiteId, c.AliasName, a.ReversalType, g.ProductId,
		a.Line_GrossQuantitySI, a.Line_GrossQuantityUSGallon, a.Line_NetQuantitySI, a.Line_NetQuantityUSGallon,
		h.PersonID, i.CompanyId ManagerCompanyId, j.CompanyId OwnerCompanyId, k.CompanyId BillToCompanyId, l.CompanyId CarrierCompanyId, m.CompanyId ShipToCompanyId, n.CompanyId ShipperCompanyId, o.CompanyId SupplierCompanyId,
		p.StationId, p.StationInterfaceTypeCode, q.ArmNumber,
		a.TimeIn, a.TimeOut, a.Line_MeterStartDateTime, a.Line_MeterStopDateTime,
		a.DocumentNumber, a.ReversedTransID, a.ConjoinTransID, a.DeleteFlag, f.InvalidTerminalTime,
		a.TransDateTime, a.CreatedDate, a._RecordUpdatedDateSKey RecordUpdatedDateSKey
		FROM dbo.FactTransaction a
		INNER JOIN dbo.DimSite b
		ON b.SKey = a.SiteSKey
		INNER JOIN dbo.DimTransactionAlias c
		ON c.SKey = a.TransactionAliasSKey
		INNER JOIN dbo.DimTransactionAttributes d
		ON d.SKey = a.TransactionAttributesSKey
		INNER JOIN dbo.FactTransactionSummary e
		ON e.TransactionKey = a.TransactionKey
		INNER JOIN dbo.DimTransactionAttributes f
		ON f.SKey = e.TransactionAttributesSKey
		LEFT JOIN dbo.DimProduct g
		ON g.SKey = a.Line_ProductSKey
		LEFT OUTER JOIN dbo.DimPersonnel h
		ON h.SKey = a.OperatorPersonnelSKey
		LEFT OUTER JOIN dbo.DimCompany i
		ON i.SKey = a.ManagerCompanySKey
		LEFT OUTER JOIN dbo.DimCompany j
		ON j.SKey = a.OwnerCompanySKey
		LEFT OUTER JOIN dbo.DimCompany k
		ON k.SKey = a.BillToCompanySKey
		LEFT OUTER JOIN dbo.DimCompany l
		ON l.SKey = a.CarrierCompanySKey
		LEFT OUTER JOIN dbo.DimCompany m
		ON m.SKey = a.ShipToCompanySKey
		LEFT OUTER JOIN dbo.DimCompany n
		ON n.SKey = a.ShipperCompanySKey
		LEFT OUTER JOIN dbo.DimCompany o
		ON o.SKey = a.SupplierCompanySKey
		LEFT OUTER JOIN dbo.DimStation p
		ON p.SKey = a.Line_StationSKey
		LEFT OUTER JOIN dbo.DimLoadArm q
		ON q.SKey = a.Line_LoadArmSKey
		WHERE ((@SiteId IS NULL) OR (b.SiteId IN (SELECT ParameterStrValue FROM @tblDimSiteValue)))
		AND ((@ProductId IS NULL) OR (g.ProductId IN (SELECT ParameterStrValue FROM @tblDimProductValue)))
		AND ((@TransactionAlias IS NULL) OR (c.AliasName IN (SELECT ParameterStrValue FROM @tblDimTransactionAliasValue)))
		AND ((@beginInvDateSKey IS NULL) OR (a.InventoryDateSKey >= @beginInvDateSKey))
		AND ((@endInvDateSKey IS NULL) OR (a.InventoryDateSKey <= @endInvDateSKey))
		AND ((@beginTransDateSKey IS NULL) OR (a.TransDateSKey >= @beginTransDateSKey))
		AND ((@endTransDateSKey IS NULL) OR (a.TransDateSKey <= @endTransDateSKey))
		AND ((@IsInvalidTerminalTime IS NULL) OR (f.InvalidTerminalTime = @IsInvalidTerminalTime))
		AND 
		(
			(@IsDeleted IS NULL) 
			OR 
			(
				(@IsDeleted = 'true') 
				AND 
				(
					(d.DeleteFlag = 'true') OR (f.DeleteFlag = 'true') OR (d.IsRecordDeleted = 'true') OR (f.IsRecordDeleted = 'true')
				)
			)
			OR 
			(
				(@IsDeleted = 'false') 
				AND 
				(
					(d.DeleteFlag = 'false') AND (f.DeleteFlag = 'false') AND (d.IsRecordDeleted = 'false') AND (f.IsRecordDeleted = 'false')
				)
			)
		)
		AND EXISTS
		(
			SELECT * FROM @tblAuthorisedCompanies p
			WHERE p.Id IN (i.CompanyId, j.CompanyId, k.CompanyId, l.CompanyId, m.CompanyId, n.CompanyId, o.CompanyId)
		)
		AND EXISTS
		(
			SELECT * FROM @tblAuthorisedSites q
			WHERE q.Id IN (b.SiteId)
		)
		ORDER BY a.InventoryDateSKey DESC, b.SiteId, a._RecordUpdatedDateSKey DESC
					
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
						+ 'Procedure Name: [rpt].[usp_GetFactTransactionDetails]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END