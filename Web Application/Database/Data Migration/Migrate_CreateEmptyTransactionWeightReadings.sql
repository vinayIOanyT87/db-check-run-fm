USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_CreateEmptyTransactionWeightReadings]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_CreateEmptyTransactionWeightReadings]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_CreateEmptyTransactionWeightReadings]
GO

CREATE PROCEDURE [dbo].[Migrate_CreateEmptyTransactionWeightReadings]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_CreateEmptyTransactionWeightReadings 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 

IF NOT EXISTS(Select * from sys.databases where [name] = 'ConsolidatedDB6')
BEGIN
	Select 'ConsolidatedDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 ConsolidatedDB Database before running this stored procedure';
	return
END

IF NOT EXISTS(Select * from sys.databases where [name] = 'AccountingDB6')
BEGIN
	Select 'AccountingDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Accounting Database before running this stored procedure';
	return
END



if(@IsBaseDB <> 2)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'AviationDB6')
	BEGIN
		Select 'AviationDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 Aviation Database before running this stored procedure';
		return
	END

END


declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @count int;

--Set @siteIndex8 = (Select Min(Isnull(SiteIndex,0)) from ConsolidatedDB.dbo.tblSites where ID = @SiteID);

Set @fill = '?';
Set @blank = '';


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END


SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1 AND
(Select isnull(COUNT(tt.TransID),0) from ConsolidatedDB.dbo.tblTransactions tt
	where tt.AliasName in 
	('Sale',
	'Commercial',
	'Contract',
	'Defuel',
	'Fillstand',
	'Inflight',
	'Reissue',
	'Return to Bulk',
	'Sale',
	'Shipment - Contract',
	'Shipment - Transfer') AND tt.SiteIndex = S8.siteIndex) > 0
ORDER BY S6.SiteIndex;





/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/



	Insert Into ConsolidatedDB.dbo.tblTransactionWeightReadings
	(
	CompartmentID,
	BeginQuantityValue,
	RequestedQuantityValue,
	FinalQuantityValue,
	CreatedBy,
	CreatedDate,
	UpdatedBy,
	UpdatedDate,
	TransVersion,
	TransIndex
	)
	Select
	/* CompartmentID */@blank,
	/* BeginQuantityValue */0,
	/* RequestedQuantityValue */0,
	/* FinalQuantityValue */0,
	/* CreatedBy */ 'Varec',
	/* CreatedDate */ GETDATE(),
	/* UpdatedBy */ 'Varec',
	/* UpdatedDate */ GETDATE(),
	/* TransVersion */tt.TransVersion,
	/* TransIndex */tt.TransIndex
	from ConsolidatedDB.dbo.tblTransactions tt join #tmpsites s on tt.SiteIndex = s.SiteIndex8
	where 
	tt.AliasName in 
	('Sale',
	'Commercial',
	'Contract',
	'Defuel',
	'Fillstand',
	'Inflight',
	'Reissue',
	'Return to Bulk',
	'Sale',
	'Shipment - Contract',
	'Shipment - Transfer') 
	
	
--SELECT * FROM ConsolidatedDB.dbo.tblTransactionWeightReadings	;
	
	


	
/*	
IF @@TRANCOUNT > 0    
BEGIN     
--	ROLLBACK TRANSACTION;  
   
	COMMIT TRANSACTION  
END   
 
END TRY

BEGIN CATCH
IF @@TRANCOUNT > 0    
BEGIN     
	ROLLBACK TRANSACTION; 
	--SELECT  'ERROR: ' + ISNULL(@MSG,'Unknown Error')  as [Status]; 
	DECLARE @MSG nvarchar(MAX)
	SET @MSG = ERROR_MESSAGE()    
	RAISERROR  (@MSG,0,1)  
END  
END CATCH
*/