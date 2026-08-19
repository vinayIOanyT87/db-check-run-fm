USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData]
GO

CREATE PROCEDURE [dbo].[Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/21/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_FMD6SHIPMENTWithShippingDocumentFieldsToFMD8LineItemUserData 2, null

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
	/*if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END*/
	/*if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
	IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
		BEGIN
		Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
		return;
		END
		*/
END
/*ELSE
BEGIN
	
	if(isnull(@SiteID,'') = '')
		BEGIN
		Select 'A site must be specified when running this script as a base level script.';
		return;
		End
		
	if(isnull(@SiteID,'') <> '')
	BEGIN
		IF NOT EXISTS(Select * from tblSites where [ID] = @SiteID)
			BEGIN
			Select 'Site: ' + @SiteID + ' does not exist in the database.  An existing site must be specified when running this script as a base level script.';
			return;
			END
	END
END*/

declare @aliasName6 nvarchar(50);
declare @aliasName8 nvarchar(50);

Set @aliasName6 = 'SHIPMENT'
Set @aliasName8 = 'Shipment - Contract'

declare @fill nvarchar(30);
declare @blank nvarchar(1);
declare @count int;
declare @massindex int;
declare @massdecimalplaces tinyint;

Set @fill = '?';
Set @blank = '';

IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END



SELECT S8.[ID] AS SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8, 
isnull(s8.massunitindex,64) AS massindex, isnull(MassDecimalPlaces,0) AS massdecimalplaces INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1 AND
( Select isnull(COUNT(tt.TransIndex),0) 
	from
	ConsolidatedDB.dbo.tblTransactions tt,
	ConsolidatedDB.dbo.tblTransactionLineItems ttl,
	AccountingDB6.dbo.t_Acct_FuelLoad taf
	where 
	tt.SiteIndex = S8.siteIndex and 
	tt.TransIndex = ttl.TransIndex and 
	ttl.SequenceID = taf.FuelLoadIndex) > 0
	ORDER BY S6.SiteIndex

	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	

*/
	--set @massindex = (select isnull(massunitindex,64) from ConsolidatedDB.dbo.tblSites where siteindex = @siteIndex8);
	--set @massdecimalplaces = (select isnull(MassDecimalPlaces,0) from ConsolidatedDB.dbo.tblSites where siteindex = @siteIndex8);

	--Insert Notes from t_Acct_Tx1
	Insert Into ConsolidatedDB.dbo.tblTransactionLineItemUserData
	(TransLineItemID,
	UserData1,
	UserData2,
	UserData3,
	UserData4,
	UserData5,
	UserData6,
	UserData7,
	UserData8,
	UserData9,
	UserData10,
	UserData11,
	UserData12,
	UserData13,
	UserData14,
	UserData15,
	UserData16,
	UserData17,
	UserData18,
	UserData19,
	UserData20,
	UserData21,
	UserData22,
	UserData23,
	UserData24,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy)
	
	Select
	/* TransLineItemID */ ttl.TransLineItemID,  
	/* UserData1 */ @blank,
	/* UserData2 */@blank,
		/*UserData3*/CASE UPPER(ISNULL(taf.SealLocation,'')) 
	WHEN 'TDF' THEN 'Tank Dome Front'
	WHEN 'TDB' THEN 'Tank Dome Back'
	WHEN 'M' THEN 'Manifold'
	WHEN 'TS' THEN 'Tank Sump'
	ELSE ''
	END,
	/* UserData4 */@blank,
	/* UserData5 */@blank,
	/* UserData6 */@blank,
	/* UserData7 */@blank,
	/* UserData8 */@blank,
	/* UserData9 */@blank,
	/* UserData10 */@blank,
	/* UserData11 */@blank,
	/* UserData12 */@blank,
		/*UserData13*/taf.TruckNumber,
	/* UserData14 */@blank,
	/* UserData15 */@blank,
	/* UserData16 */@blank,
	/* UserData17 */@blank,  
	/* UserData18 */@blank,
	/* UserData19 */@blank,
	/* UserData20 */@blank,
	/* UserData21 */@blank,
	/* UserData22 */@blank,
	/* UserData23 */@blank,
	/* UserData24 */convert(nvarchar(20),dbo.ConvertFromSIUnits(isnull(taf.GrossWeight,0.0),s.massindex,s.massDecimalPlaces)),
	ISNULL(ttl.CreatedDate,GETDATE()),
	ISNULL(ttl.CreatedBy,'Varec'),
	ISNULL(ttl.UpdatedDate,getdate()),
	ISNULL(ttl.UpdatedBy,'Varec')
	from 
	ConsolidatedDB.dbo.tblTransactions tt
	JOIN #TMPSITES s ON s.SiteIndex8 = tt.SiteIndex,
	ConsolidatedDB.dbo.tblTransactionLineItems ttl,
	AccountingDB6.dbo.t_Acct_FuelLoad taf
	where 
--	tt.SiteIndex = s.siteIndex8 and 
	tt.TransIndex = ttl.TransIndex and 
	ttl.SequenceID = taf.FuelLoadIndex and
	tt.AliasName like 'Shipment%'

DROP TABLE #TMPSITES

--select * from ConsolidatedDB.dbo.tblTransactionLineItemUserData 


/*	
IF @@TRANCOUNT > 0    
BEGIN     
--	 ROLLBACK TRANSACTION;
    
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
