CREATE PROCEDURE [dbo].[usp_TransactionFieldDataTypes]

AS
SET NOCOUNT ON

	SELECT DISTINCT
		syscolumns.name as [Column Name],
		systypes.name as [Data Type]

	FROM syscolumns, systypes
	WHERE 
		(object_name(syscolumns.id) = 'tblTransactions' or
		 object_name(syscolumns.id) = 'tblTransactionLineItems' or
		 object_name(syscolumns.id) = 'tblTransactionWeightReadings') and
		syscolumns.xtype = systypes.xtype and 
		systypes.name <> 'sysname' 
	ORDER BY [Column Name];