/*
	DROP PROCEDURE [rpt].[usp_GetFactTransactionDetailsParamList]

	EXEC [rpt].[usp_GetFactTransactionDetailsParamList]  @UserGuid=N'00000000-0000-0000-0000-000000000002',@SiteId=N'[Site].[Site Id].&[Baltimore]',@ProductId=NULL,@TransactionAlias=NULL,@InventoryDateKey=NULL,@InventoryBeginDate=NULL,@InventoryEndDate=NULL,@TransactionDateKey=NULL,@TransactionBeginDate='2018-10-01 00:00:00',@TransactionEndDate=N'10/29/2023 12:00:00 AM',@IsInvalidTerminalTime=N'True', @IsDeleted=N'False'

	EXEC [rpt].[usp_GetFactTransactionDetailsParamList] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', NULL, '08/01/2018', '08/30/2018', NULL, NULL, NULL, NULL, NULL

	EXEC [rpt].[usp_GetFactTransactionDetailsParamList] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].&[2018]&[4]&[12]', NULL, NULL, NULL, NULL, NULL, 'True', 'False'
	
	EXEC [rpt].[usp_GetFactTransactionDetailsParamList] '00000000-0000-0000-0000-000000000002', '[Site].[Site Id].&[Baltimore],[Site].[Site Id].&[SiteAdmin]', '[Product].[Product Id].&[3201], [Product].[Product Id].&[13203]', '[Transaction Alias].[Alias Name].&[BOL],[Transaction Alias].[Alias Name].&[Receipt]', '[Inventory Date].[YearMonth Hierarchy].[Calendar Year Month].[ALL]', NULL, NULL, NULL, NULL, NULL, NULL, NULL

	EXEC [rpt].[usp_GetFactTransactionDetailsParamList] @UserKey=NULL,@SiteId=N'[Site].[Site Id].&[Baltimore]', @ProductId=NULL,@TransactionAlias=NULL,@InventoryDateKey=NULL,@InventoryBeginDate=NULL,@InventoryEndDate=NULL,@TransactionDateKey=NULL,--@TransactionDateKey=N'20210223',@TransactionBeginDate=NULL,@TransactionEndDate=NULL,@IsInvalidTerminalTime=NULL

*/
CREATE PROCEDURE [rpt].[usp_GetFactTransactionDetailsParamList]
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
	-- Stored procedure: [rpt].[usp_GetFactTransactionDetailsParamList]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Translate the parameters for the Transaction Details report into a user-friendly parameter list.
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
		DECLARE @userIdStr nvarchar(100)
		DECLARE @siteIdStr nvarchar(500)
		DECLARE @productIdStr nvarchar(500)
		DECLARE @transactionAliasStr nvarchar(100)
		DECLARE @TerminalTimeValidityStr nvarchar(100)
		DECLARE @IsDeletedStr nvarchar(500)
		DECLARE @dateStr nvarchar(500)
		DECLARE @csvList nvarchar(500)

		SET @userKey = CONVERT(nvarchar(50), @UserGuid)

		SELECT @userIdStr = FMUserID FROM dbo.DimFMUser WHERE AKey = @userKey
		IF (@UserKey IS NULL)
		BEGIN
			SET @userIdStr = '[All Users]'
		END

		DECLARE @tblDimValue TABLE
		(
			[ParameterIndex] [int] NULL,
			[ParameterStrValue] [nvarchar](100) NULL,
			[BeginDateKey] [int] NULL,
			[EndDateKey] [int] NULL
		)

		INSERT INTO @tblDimValue (ParameterStrValue) 
		SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@SiteId, 0)
		IF ((@SiteId IS NULL) OR (CHARINDEX ('[ALL]', @SiteId) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0))
		BEGIN
			SET @siteIdStr = '[All Sites]'
		END
		ELSE
		BEGIN
			SET @csvList = ''
			SELECT @csvList = 
				CASE WHEN @csvList = ''
					THEN ParameterStrValue
					ELSE @csvList + coalesce(', ' + ParameterStrValue, '')
				END
			FROM @tblDimValue
			SET @siteIdStr = @csvList
		END


		DELETE @tblDimValue
		INSERT INTO @tblDimValue (ParameterStrValue) 
		SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@ProductId, 0)
		IF ((@ProductId IS NULL) OR (CHARINDEX ('[ALL]', @ProductId) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0))
		BEGIN
			SET @productIdStr = '[All Products]'
		END
		ELSE
		BEGIN
			SET @csvList = ''
			SELECT @csvList = 
				CASE WHEN @csvList = ''
					THEN ParameterStrValue
					ELSE @csvList + coalesce(', ' + ParameterStrValue, '')
				END
			FROM @tblDimValue
			SET @productIdStr = @csvList
		END


		DELETE @tblDimValue
		INSERT INTO @tblDimValue (ParameterStrValue) 
		SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@TransactionAlias, 0)
		IF ((@TransactionAlias IS NULL) OR (CHARINDEX ('[ALL]', @TransactionAlias) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0))
		BEGIN
			SET @transactionAliasStr = '[All Aliases]'
		END
		ELSE
		BEGIN
			SET @csvList = ''
			SELECT @csvList = 
				CASE WHEN @csvList = ''
					THEN ParameterStrValue
					ELSE @csvList + coalesce(', ' + ParameterStrValue, '')
				END
			FROM @tblDimValue
			SET @transactionAliasStr = @csvList
		END

		DELETE @tblDimValue
		INSERT INTO @tblDimValue (ParameterStrValue) 
		SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@IsInvalidTerminalTime, 0)
		IF ((@IsInvalidTerminalTime IS NULL) OR (CHARINDEX ('[ALL]', @IsInvalidTerminalTime) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0) OR ((SELECT COUNT(*) FROM @tblDimValue) > 1))
		BEGIN
			SET @TerminalTimeValidityStr = '[All]'
		END
		ELSE
		BEGIN
			DECLARE @invalidTime bit = NULL
			SET @invalidTime = (SELECT TOP(1) CONVERT(bit, ParameterStrValue) FROM @tblDimValue)
			SELECT @TerminalTimeValidityStr = IIF(@invalidTime = 1, '[Invalid times only]', '[Valid times only]')
		END

		DELETE @tblDimValue
		INSERT INTO @tblDimValue (ParameterStrValue) 
		SELECT ParameterStrValue FROM [rpt].[udf_GetDimensionParameterValues] (@IsDeleted, 0)
		IF ((@IsDeleted IS NULL) OR (CHARINDEX ('[ALL]', @IsDeleted) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0) OR ((SELECT COUNT(*) FROM @tblDimValue) > 1))
		BEGIN
			SET @IsDeletedStr = '[All]'
		END
		ELSE
		BEGIN
			DECLARE @deletedRecords bit = NULL
			SET @deletedRecords = (SELECT TOP(1) CONVERT(bit, ParameterStrValue) FROM @tblDimValue)
			SELECT @IsDeletedStr = IIF(@deletedRecords = 1, '[Deleted records only]', '[Non-deleted records only]')
		END

		DELETE @tblDimValue	
		DECLARE @beginDateStr varchar(100) = '<NULL>'
		DECLARE @endDateStr varchar(100) = '<NULL>'
		IF (@InventoryDateKey IS NULL AND @InventoryBeginDate IS NULL AND @InventoryEndDate IS NULL AND @TransactionDateKey IS NULL AND @TransactionBeginDate IS NULL AND @TransactionEndDate IS NULL)
		BEGIN
			SET @DateStr = '[All Dates]'
		END
		ELSE IF (LEN(@InventoryDateKey) > 0)
		BEGIN
			INSERT INTO @tblDimValue (ParameterIndex, BeginDateKey, EndDateKey)  
			SELECT ParameterIndex, BeginDateKey, EndDateKey FROM [rpt].[udf_GetDimensionParameterValues] (@InventoryDateKey, 1)	
			IF (CHARINDEX ('[ALL]', @InventoryDateKey) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0)
			BEGIN
				SET @DateStr = '[All Dates]'
			END
			ELSE
			BEGIN
				SELECT @DateStr = CONVERT(varchar, BeginDateKey) + ' to ' + CONVERT(varchar, EndDateKey) FROM @tblDimValue WHERE ParameterIndex = 0  -- only support a single date range
			END
		END
		ELSE IF (@InventoryBeginDate IS NOT NULL OR @InventoryEndDate IS NOT NULL)
		BEGIN			
			IF (@InventoryBeginDate IS NOT NULL)
			BEGIN
				SET @beginDateStr = CONVERT(varchar(100), dbo.udf_ConvertToDateKey(@InventoryBeginDate))		
			END
			IF (@InventoryEndDate IS NOT NULL)
			BEGIN
				SET @endDateStr = CONVERT(varchar(100), dbo.udf_ConvertToDateKey(@InventoryEndDate))
			END
			SELECT @DateStr = @beginDateStr + ' to ' + @endDateStr
		END
		ELSE IF (LEN(@TransactionDateKey) > 0)
		BEGIN
			INSERT INTO @tblDimValue (ParameterIndex, BeginDateKey, EndDateKey)  
			SELECT ParameterIndex, BeginDateKey, EndDateKey FROM [rpt].[udf_GetDimensionParameterValues] (@TransactionDateKey, 1)	
			IF (CHARINDEX ('[ALL]', @TransactionDateKey) > 0) OR ((SELECT COUNT(*) FROM @tblDimValue) = 0)
			BEGIN
				SET @DateStr = '[All Dates]'
			END
			ELSE
			BEGIN
				SELECT @DateStr = CONVERT(varchar, BeginDateKey) + ' to ' + CONVERT(varchar, EndDateKey) FROM @tblDimValue WHERE ParameterIndex = 0  -- only support a single date range
			END
		END
		ELSE IF (@TransactionBeginDate IS NOT NULL OR @TransactionEndDate IS NOT NULL)
		BEGIN
			IF (@TransactionBeginDate IS NOT NULL)
			BEGIN
				SET @beginDateStr = CONVERT(varchar(100), dbo.udf_ConvertToDateKey(@TransactionBeginDate))		
			END
			IF (@TransactionEndDate IS NOT NULL)
			BEGIN
				SET @endDateStr = CONVERT(varchar(100), dbo.udf_ConvertToDateKey(@TransactionEndDate))
			END
			SELECT @DateStr = @beginDateStr + ' to ' + @endDateStr
		END

		
		SELECT @userIdStr UserIdList, @siteIdStr SiteIdList, @productIdStr ProductIdList, @transactionAliasStr TransactionAliasList, @DateStr DateRange, @TerminalTimeValidityStr TerminalTimeValidity, @IsDeletedStr DeletionStatus
					
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
						+ 'Procedure Name: [rpt].[usp_GetFactTransactionDetailsParamList]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END