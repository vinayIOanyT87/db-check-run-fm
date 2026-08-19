/*
	DROP PROCEDURE [archive].[usp_GetTransactionUserData]

	EXEC [archive].[usp_GetTransactionUserData] '2015-08-01', 60000, 80000
	EXEC [archive].[usp_GetTransactionUserData] '2015-08-01', NULL, NULL

*/
CREATE PROCEDURE [archive].[usp_GetTransactionUserData]
(
	@cutOffDate date,
	@beginTransactionKey  int,
	@endTransactionKey int
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_GetTransactionUserData]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves data from the dbo.tblTransactionUserData table up to a given cut-off date.
	-- Notes:
	-- 1. @cutOffDate: Date up to which data must be fetched from the dbo.tblTransactionUserData table.
	-- 2. @beginTransactionKey: Transaction Header _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
	-- 3. @endTransactionKey: Transaction Header _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @isArchivingOn bit

		SELECT @isArchivingOn = a.IsArchivingOn FROM [archive].[tblArchiveScope] a
		INNER JOIN [archive].[tblArchiveScopeToTable] b
		ON b.ArchiveScopeGuid = a.ArchiveScopeGuid
		WHERE b.SourceArchiveTable = '[dbo].[tblTransactionUserData]'

		SELECT a.*, CONVERT(BigInt, a._RowVersion) RowVersionInt FROM dbo.tblTransactionUserData a WITH (NOLOCK)
		INNER JOIN dbo.tblTransactions b WITH (NOLOCK)
		ON b.TransactionGuid = a.TransactionGuid
		WHERE @isArchivingOn = 1 
		AND b.InventoryDate <= @cutOffDate
		AND ((b._ClusterIdx >= @beginTransactionKey) OR (ISNULL(@beginTransactionKey, 0) = 0))
		AND ((b._ClusterIdx < @endTransactionKey) OR (ISNULL(@endTransactionKey, 0) = 0))
		ORDER BY a._ClusterIdx
					
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
						+ 'Procedure Name: [archive].[usp_GetTransactionUserData]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


