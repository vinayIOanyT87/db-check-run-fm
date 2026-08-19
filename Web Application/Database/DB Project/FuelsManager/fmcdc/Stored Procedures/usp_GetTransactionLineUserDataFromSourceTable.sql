/*
	DROP PROCEDURE [fmcdc].[usp_GetTransactionLineUserDataFromSourceTable]
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionLineUserDataFromSourceTable]
(
	@startDate datetime,
	@beginIndex int,
	@endIndex int,
	@extractByInventoryDate bit
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [fmcdc].[usp_GetTransactionLineUserDataFromSourceTable]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieves all the records from tblTransactionLineUserData for which the UpdatedDate is on or greater a given date, and for 
--          which the _ClusterIdx falls within a given range.
-- Notes:
-- 1. @startDate: UpdateDate from which to filter the records.
-- 2. @beginIndex: _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
-- 3. @endIndex: _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
-- 4. @extractByInventoryDate: 0: Filter By Update Date; 1: Filter by Inventory Date
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

	SELECT 
	   a.[UserData1]
      ,a.[UserData2]
      ,a.[UserData3]
      ,a.[UserData4]
      ,a.[UserData5]
      ,a.[UserData6]
      ,a.[UserData7]
      ,a.[UserData8]
      ,a.[UserData9]
      ,a.[UserData10]
      ,a.[UserData11]
      ,a.[UserData12]
      ,a.[UserData13]
      ,a.[UserData14]
      ,a.[UserData15]
      ,a.[UserData16]
      ,a.[UserData17]
      ,a.[UserData18]
      ,a.[UserData19]
      ,a.[UserData20]
      ,a.[UserData21]
      ,a.[UserData22]
      ,a.[UserData23]
      ,a.[UserData24]
      ,a.[CreatedBy]
      ,a.[CreatedDate]
      ,a.[UpdatedBy]
      ,a.[UpdatedDate]
      ,a.[TransactionLineItemUserDataGuid]     
      ,a.[TransactionLineItemGuid]
      ,a.[_ClusterIdx] 
	  ,a.[_RowVersion],
	  CONVERT(BigInt, a.[_RowVersion]) RowVersionInt
	FROM dbo.tblTransactionLineItemUserData a
	INNER JOIN dbo.tblTransactionLineItems b
	ON b.TransactionLineItemGuid = a.TransactionLineItemGuid
	INNER JOIN dbo.tblTransactions c
	ON c.TransactionGuid = b.TransactionGuid
	WHERE 
		(
			((@extractByInventoryDate = 0) AND (a.UpdatedDate >= @startdate))
			OR ((@extractByInventoryDate = 1) AND (cast(c.InventoryDate as datetime) >= @startdate))
		)
	AND ((c._ClusterIdx>= @beginIndex) OR (ISNULL(@beginIndex, 0) = 0)) 
	AND ((c._ClusterIdx<= @endIndex) OR (ISNULL(@endIndex, 0) = 0))
					
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
						+ 'Procedure Name: [fmcdc].[usp_GetTransactionLineItemUserDataFromSourceTable]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END