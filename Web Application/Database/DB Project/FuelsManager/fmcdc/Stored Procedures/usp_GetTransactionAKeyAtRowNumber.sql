/*
       DROP PROCEDURE [fmcdc].[usp_GetTransactionAKeyAtRowNumber]

       EXEC [fmcdc].[usp_GetTransactionAKeyAtRowNumber] '3/1/2018', 100000, 0
       
*/
CREATE PROCEDURE [fmcdc].[usp_GetTransactionAKeyAtRowNumber]
(
       @startDate datetime, @rowNumber int, @extractByInventoryDate bit
)
AS
BEGIN
------------------------------------------------------------------------------------------------------
-- Stored procedure: [fmcdc].[usp_GetTransactionAKeyAtRowNumber]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Retrieves all the TransactionAKey (_ClusterIdx) of the Transaction record that is 
--          located at a given row number of the transactions ordered by the TransactionAKey.
-- Notes:
-- 1. @startDate: Transaction start date for the query
-- 2. @rowNumber: Row number at which to retrieve the TransactionAKey
-- 3. @extractByInventoryDate: 0: Filter By Update Date; 1: Filter by Inventory Date
------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
    BEGIN TRY

		SELECT x._Clusteridx FROM
        (
			SELECT ROW_NUMBER() OVER (ORDER BY _Clusteridx ASC) As RowNum, _Clusteridx FROM dbo.tblTransactions 
			WHERE ((@extractByInventoryDate = 0) AND (UpdatedDate >= @startdate))
			OR ((@extractByInventoryDate = 1) AND (cast(InventoryDate as datetime) >= @startdate))
        ) x
		WHERE x.RowNum =@rowNumber
                                  
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
                           + 'Procedure Name: [fmcdc].[usp_GetTransactionAKeyAtRowNumber]' + CHAR(13)+CHAR(10)                  
                           + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
        RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
       
END