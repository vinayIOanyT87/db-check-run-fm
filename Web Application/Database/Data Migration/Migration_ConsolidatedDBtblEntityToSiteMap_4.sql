USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDBtblEntityToSiteMap_4]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDBtblEntityToSiteMap_4') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDBtblEntityToSiteMap_4
GO


CREATE PROCEDURE [dbo].Migration_ConsolidatedDBtblEntityToSiteMap_4
 /*=============================================
 Author:			URVI PATEL
 Create date:		1/22/2010 Creating groups for tblEntityToSiteMap table
 
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDBtblEntityToSiteMap_4 2, null
*/

(
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID varchar(max) = NULL
)
AS 
BEGIN
IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END

SELECT S8.[ID] as SiteID8, S6.SiteIndex AS SiteIndex6, S8.SiteIndex AS SiteIndex8 INTO #TMPSITES
FROM [ConsolidatedDB6].[dbo].tblSites S6 JOIN [ConsolidatedDB].[dbo].tblSites S8 ON S6.SiteID=S8.[ID] 
WHERE S6.DeleteFlag = 0 AND (@SiteID IS NULL OR S6.SiteID = @SiteID) AND S6.SiteIndex <> -1
ORDER BY S6.SiteIndex




	
/*	-- Left it commented out rather than delete so that it can be used when debugging.
BEGIN TRY
BEGIN TRANSACTION
	
*/
declare @currentDate datetime;
Set @currentDate = GETDATE();
/* Inserting User Groups */
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT 
	DISTINCT 'User Groups', 
	site8.siteIndex,
	g.GroupIndex, 
		@currentDate,
		'Varec'
FROM [ConsolidatedDB].dbo.tblsites site8, [ConsolidatedDB].dbo.tblGroups g, #TMPSITES s
WHERE site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='User Groups' 
	AND [Index] = g.GroupIndex AND SiteIndex = site8.SiteIndex)

/* Insert Ledger Aggregate Column*/
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT DISTINCT 'Ledger Aggregate Column',
		site8.SiteIndex,
		l.[index],
		@currentDate,
		'Varec'

FROM [ConsolidatedDB].dbo.tblsites site8 , [ConsolidatedDB].dbo.tblLedgerAggregateColumns l, #TMPSITES s
WHERE Site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Ledger Aggregate Column' 
	AND [Index] = l.[Index] AND SiteIndex = site8.SiteIndex)

/* Insert List Views*/
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT DISTINCT 'List Views',
		site8.SiteIndex,
		l.[index],
		@currentDate,
		'Varec'

FROM [ConsolidatedDB].dbo.tblsites site8 , [ConsolidatedDB].dbo.tblListViews l, #TMPSITES s
WHERE Site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='List Views' 
	AND [Index] = l.[Index] AND SiteIndex = site8.SiteIndex)


/* Insert Data Dictionary */
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT DISTINCT 'Data Dictionary',
		site8.SiteIndex,
		-1,
		@currentDate,
		'Varec'

FROM [ConsolidatedDB].dbo.tblsites site8, #TMPSITES s 
WHERE Site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Data Dictionary' 
	AND [Index] = -1 AND SiteIndex = site8.SiteIndex)

/*  Insert Transaction Aliases */
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT DISTINCT 'Transaction Aliases',
		site8.SiteIndex,
		ta.AliasID,
		@currentDate,
		'Varec'

FROM [ConsolidatedDB].dbo.tblsites site8 , [ConsolidatedDB].dbo.tblTransactionAliases ta, #TMPSITES s
WHERE Site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Transaction Aliases' 
	AND [Index] = ta.AliasID AND SiteIndex = site8.SiteIndex)
	

/*  Insert Query Settings */
INSERT INTO [ConsolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT DISTINCT 'Query Settings',
		site8.SiteIndex,
		q.[Index],
		@currentDate,
		'Varec'

FROM [ConsolidatedDB].dbo.tblsites site8 , [ConsolidatedDB].dbo.tblQueryDefaults q, #TMPSITES s
WHERE Site8.SiteIndex = s.SiteIndex8
AND NOT EXISTS(SELECT * FROM [ConsolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Query Settings' 
	AND [Index] = q.[Index] AND SiteIndex = site8.SiteIndex)


declare @descCompanyIndex int;
Set @descCompanyIndex = (Select MIN(CompanyIndex) from [ConsolidatedDB].[dbo].[tblCompanies] where ID = 'DESC')
if not exists(Select * from tblEntityToSiteMap m join #TMPSITES s ON m.SiteIndex=s.SiteIndex8 
		where TypeID = 'Companies' and [Index] = @descCompanyIndex)
BEGIN
INSERT INTO [ConsolidatedDB].[dbo].[tblEntityToSiteMap] (TypeID,SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT 'Companies',SiteIndex8,@descCompanyIndex,@currentDate,'Varec' from #TMPSITES;
END

--Eric Simmons (5/2/2010) [CR #13944]
--Added to make sure the general settings are properly set
if not exists(Select SiteIndex from ConsolidatedDB.dbo.tblGeneralConfiguration g join #TMPSITES s ON g.SiteIndex=s.SiteIndex8 )
BEGIN
Insert Into ConsolidatedDB.dbo.tblGeneralConfiguration
(SiteIndex,Method,ConsortiumFlag,ShowDeletedTrxFlag,AllowUndeleteFlag,
 ReverseTrxDateMode,ForcedCloseout,SecurityCode,AuthorizationCode,
 MeterTolerance,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,SetBeginInventoryToZeroFlag)
 Select s.SiteIndex8,Method,ConsortiumFlag,ShowDeletedTrxFlag,AllowUndeleteFlag,
 ReverseTrxDateMode,ForcedCloseout,SecurityCode,AuthorizationCode,
 MeterTolerance,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,SetBeginInventoryToZeroFlag
 from ConsolidatedDB.dbo.tblGeneralConfiguration g, #TMPSITES s where SiteIndex = 1
 END

DROP TABLE #tmpsites;




/*		
IF @@TRANCOUNT > 0    
BEGIN     
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
END;

