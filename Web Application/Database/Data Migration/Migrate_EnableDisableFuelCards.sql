USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_EnableDisableFuelCards]    Script Date: 04/07/2010 03:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_EnableDisableFuelCards]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_EnableDisableFuelCards]
GO

CREATE PROCEDURE [dbo].[Migrate_EnableDisableFuelCards]
 /*=============================================
 Author:			Eric Simmons
 Create date:		4/7/2010
 Description:		Enables or Disables products from being selected in system based on their usage in transactions.
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_EnableDisableFuelCards 2, null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 


declare @siteIndex8 int;


IF @SiteID = 'All Sites' or @IsBaseDB <> 2
BEGIN
	SET @SiteID = NULL
END


DECLARE @SiteID8 NVARCHAR(MAX)

DECLARE SiteIndexes_Cursor CURSOR FOR SELECT S8.[ID], S8.SiteIndex AS SiteIndex8 
FROM [ConsolidatedDB].[dbo].tblSites S8
WHERE (@SiteID IS NULL OR S8.[ID] = @SiteID) AND S8.SiteIndex <> -1
ORDER BY S8.SiteIndex


OPEN SiteIndexes_Cursor 
FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteID8, @siteIndex8 
WHILE @@FETCH_STATUS = 0 
BEGIN 

	if(isnull(@SiteID,'') <> '' and @SiteID <> @SiteID8)
	BEGIN
		FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteID8, @siteIndex8 
		continue;
	END

Update tblFuelCards Set ActivationStatus = 0
where FuelCardIndex in
(Select Distinct FuelCardIndex from tblTransactions where FuelCardIndex is not null and InventoryDate > DATEADD(month,-6,getDate()) and SiteIndex = @siteIndex8)

Update tblFuelCards Set ActivationStatus = 1
where FuelCardIndex not in
(Select Distinct FuelCardIndex from tblTransactions where FuelCardIndex is not null and InventoryDate > DATEADD(month,-6,getDate()) and SiteIndex = @siteIndex8)

FETCH NEXT FROM SiteIndexes_Cursor INTO @SiteID8 , @SiteIndex8
END 
CLOSE SiteIndexes_Cursor 
DEALLOCATE SiteIndexes_Cursor;