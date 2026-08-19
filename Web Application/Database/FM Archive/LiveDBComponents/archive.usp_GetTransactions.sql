/*
	DROP PROCEDURE [archive].[usp_GetTransactions]

	EXEC [archive].[usp_GetTransactions] '2015-08-01', 60000, 80000

	EXEC [archive].[usp_GetTransactions] '2015-08-01', NULL, NULL

*/
CREATE PROCEDURE [archive].[usp_GetTransactions]
(
	@cutOffDate date,
	@beginTransactionKey  int,
	@endTransactionKey int
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [archive].[usp_GetTransactions]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves data from the dbo.tblTransactions table up for a given cut-off date and a given _ClusterIdx range.
	-- Notes:
	-- 1. @cutOffDate: Date up to which data must be fetched from the dbo.tblTransactions table.
	-- 2. @beginTransactionKey: _ClusterIdx from which to filter the records. Leave as 0 to ignore this filter.
	-- 3. @endTransactionKey: _ClusterIdx up to which to filter the records. Leave as 0 to ignore this filter.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @isArchivingOn bit

		SELECT @isArchivingOn = a.IsArchivingOn FROM [archive].[tblArchiveScope] a
		INNER JOIN [archive].[tblArchiveScopeToTable] b
		ON b.ArchiveScopeGuid = a.ArchiveScopeGuid
		WHERE b.SourceArchiveTable = '[dbo].[tblTransactions]'

		SELECT a.*, CONVERT(BigInt, a._RowVersion) RowVersionInt FROM dbo.tblTransactions a WITH (NOLOCK)
		WHERE @isArchivingOn = 1
		AND a.InventoryDate <= @cutOffDate
		AND ((a._ClusterIdx >= @beginTransactionKey) OR (ISNULL(@beginTransactionKey, 0) = 0))
		AND ((a._ClusterIdx < @endTransactionKey) OR (ISNULL(@endTransactionKey, 0) = 0))
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
						+ 'Procedure Name: [archive].[usp_GetTransactions]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END

GO


