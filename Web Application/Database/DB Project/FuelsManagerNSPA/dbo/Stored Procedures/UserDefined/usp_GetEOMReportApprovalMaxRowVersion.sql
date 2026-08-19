CREATE PROCEDURE [dbo].[usp_GetEOMReportApprovalMaxRowVersion] 
(
	@MonthYear NVARCHAR (30), 
	@CompanyManagerMasterRecordGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetEOMReportApprovalMaxRowVersion] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve the transaction records for the Monthly Journal Report
	-- Notes:
	-- 1. @MonthYear: Month and year to Approve
	-- 2. @CompanyManagerMasterRecordGuid:  Company MasterRecordGuid assigned the role of manager for the products to approve and closeout.
	-- 3. @SiteGuid: Identifies the site the report is being run from
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

	Declare @InventoryDateTimeBegin datetimeoffset(7)
	Declare @InventroyDateTimeEnd datetimeoffset(7)

	Set @InventoryDateTimeBegin = @MonthYear
	Set @InventroyDateTimeEnd = DATEADD(m,1,@InventoryDateTimeBegin)

	Declare @MaxRVtblTransactions int
	Set @MaxRVtblTransactions = ISNULL((select MAX(t._RowVersion) FROM tblTransactions t
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionLineItems int
	Set @MaxRVtblTransactionLineItems = ISNULL((select MAX(l._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
	ON t.TransactionGuid = l.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionLineItemUserData int
	Set @MaxRVtblTransactionLineItemUserData = ISNULL((select MAX(u._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionLineItems l
	ON t.TransactionGuid = l.TransactionGuid
	INNER JOIN tblTransactionLineItemUserData u
	ON u.TransactionLineItemGuid = l.TransactionLineItemGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionNotes int
	Set @MaxRVtblTransactionNotes = ISNULL((select MAX(n._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionNotes n
	ON t.TransactionGuid = n.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionPIDX int
	Set @MaxRVtblTransactionPIDX = ISNULL((select MAX(p._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionPIDX p
	ON t.TransactionGuid = p.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionSignature int
	Set @MaxRVtblTransactionSignature = ISNULL((select MAX(x._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionSignature x
	ON t.TransactionGuid = x.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionTransportLineItems int
	Set @MaxRVtblTransactionTransportLineItems = ISNULL((select MAX(x._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionTransportLineItems x
	ON t.TransactionGuid = x.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionUserData int
	Set @MaxRVtblTransactionUserData = ISNULL((select MAX(x._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionUserData x
	ON t.TransactionGuid = x.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionWeightReadings int
	Set @MaxRVtblTransactionWeightReadings = ISNULL((select MAX(x._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionWeightReadings x
	ON t.TransactionGuid = x.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @MaxRVtblTransactionSubLineItems int
	Set @MaxRVtblTransactionSubLineItems = ISNULL((select MAX(s._RowVersion) FROM tblTransactions t
	INNER JOIN tblTransactionSubLineItems s
	ON t.TransactionGuid = s.TransactionGuid
	WHERE t.SiteGuid = @SiteGuid AND t.ManagerCompanyGuid = @CompanyManagerMasterRecordGuid AND t.InventoryDate >= @InventoryDateTimeBegin AND t.InventoryDate < @InventroyDateTimeEnd
	),-1)

	Declare @Result int
	SET @Result = IIF ( @MaxRVtblTransactions > @MaxRVtblTransactionLineItems,@MaxRVtblTransactions,@MaxRVtblTransactionLineItems )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionLineItemUserData,		@Result,@MaxRVtblTransactionLineItemUserData )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionNotes,				@Result,@MaxRVtblTransactionNotes )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionPIDX,					@Result,@MaxRVtblTransactionPIDX )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionSignature,			@Result,@MaxRVtblTransactionSignature )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionTransportLineItems,	@Result,@MaxRVtblTransactionTransportLineItems )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionUserData,				@Result,@MaxRVtblTransactionUserData )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionWeightReadings,		@Result,@MaxRVtblTransactionWeightReadings )
	SET @Result = IIF ( @Result > @MaxRVtblTransactionSubLineItems,			@Result,@MaxRVtblTransactionSubLineItems )

	SELECT @Result AS MaxRowVersion

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
						+ 'Procedure Name: [dbo].[usp_GetEOMReportApprovalMaxRowVersion] ' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
END     