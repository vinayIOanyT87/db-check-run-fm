/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionSubLineItemsFromSourceTable]
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionSubLineItemsFromSourceTable]
(
	@startDate datetime,
	@beginIndex int,
	@endIndex int,
	@extractByInventoryDate bit
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [fmcdc].[usp_GetTransactionSubLineItemsFromSourceTable]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieves all the records from tblTransactionSubLineItems for which the UpdatedDate is on or greater a given date, and for 
--          which the _ClusterIdx falls within a given range.
-- Notes:
-- 1. @startDate: UpdateDate from which to filter the records.
-- 2. @beginIndex: _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
-- 3. @endIndex: _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
-- 4. @extractByInventoryDate: 0: Filter By Update Date; 1: Filter by Inventory Date
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

	SELECT a.*, CONVERT(BigInt, a.[_RowVersion]) RowVersionInt
	FROM dbo.tblTransactionSubLineItems a
	INNER JOIN dbo.tblTransactions b
	on a.transactionguid=b.transactionguid
	WHERE 
	(
		((@extractByInventoryDate = 0) AND (a.UpdatedDate >= @startdate))
		OR ((@extractByInventoryDate = 1) AND (cast(b.InventoryDate as datetime) >= @startdate))
	)
	AND ((b._ClusterIdx>= @beginIndex) OR (ISNULL(@beginIndex, 0) = 0)) 
	AND ((b._ClusterIdx<= @endIndex) OR (ISNULL(@endIndex, 0) = 0))
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionSubLineItemsFromSourceTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END