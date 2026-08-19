/*
Aviation Post-Deployment Script							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

/*
************************************
	DATA INSERT SECTION:
************************************
*/
--:r .\DataScripts\Script.ConfigureDefaultSettings.sql

/*
	This version number corresponds to changes made to the FM Export Service
	TFS #72786 - FM Export Service Web Service Plugins
	Bryan Ponnwitz - 4/18/2017
*/
--IF NOT EXISTS(SELECT 1 FROM tblVersion WHERE [Version]='9.2.0.118.0')
--BEGIN
--	INSERT INTO tblVersion([Version],packageName,DateApplied,Comments,Check1,Check2,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
--	VALUES ('9.2.0.118.0','StandardDatabase',SYSDATETIMEOFFSET(),'FuelsManager 9.2.0.118',0,0,SYSDATETIMEOFFSET(),'Administrator',SYSDATETIMEOFFSET(),'Administrator')
--END
--GO

/*
	DocumentNumber index for searching in Accounting 9
	TFS #76149 - Search and Edit Transactions Based on Ticket Number
	Bryan Ponnwitz - 8/15/2017
*/
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_tblTransactionLineItems_DocumentNumber')
BEGIN
	CREATE NONCLUSTERED INDEX IX_tblTransactionLineItems_DocumentNumber ON dbo.tblTransactionLineItems
	(
		DocumentNumber
	) WITH(
		STATISTICS_NORECOMPUTE = OFF,
		IGNORE_DUP_KEY = OFF,
		ALLOW_ROW_LOCKS = ON,
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY];
END;
GO
ALTER VIEW dbo.vw_TransactionSummary AS
	SELECT T.SiteGuid, T.TransID, T.InventoryDate, T.TransDateTime, T.OwnerID, T.ManagerID, T.ShipToID, L.Product, dbo.udf_ConvertFromSIUnits(L.GrossQuantity, 
						dbo.tblSites.VolumeUnitIndex, dbo.tblSites.VolumeDecimalPlaces) AS GrossQuantity, dbo.udf_ConvertFromSIUnits(L.NetQuantity, dbo.tblSites.VolumeUnitIndex, 
						dbo.tblSites.VolumeDecimalPlaces) AS NetQuantity, T.LookupTransactionStatusIndex, T.AliasName, L.DocumentNumber
	FROM     dbo.tblTransactionLineItems AS L INNER JOIN
						dbo.tblTransactions AS T ON L.TransactionGuid = T.TransactionGuid AND T.DeleteFlag = 0 INNER JOIN
						dbo.tblSites ON T.SiteGuid = dbo.tblSites.SiteGuid
GO
