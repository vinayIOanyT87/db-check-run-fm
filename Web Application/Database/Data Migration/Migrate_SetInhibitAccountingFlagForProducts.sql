USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  StoredProcedure [dbo].[Migrate_SetInhibitAccountingFlagForProducts]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_SetInhibitAccountingFlagForProducts]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_SetInhibitAccountingFlagForProducts]
GO

CREATE PROCEDURE [dbo].[Migrate_SetInhibitAccountingFlagForProducts]
 /*=============================================
 Author:			Eric Simmons
 Create date:		4/7/2010
 Description:		Enables or Disables products from being selected in system based on their usage in transactions.
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC [Migrate_SetInhibitAccountingFlagForProducts] 2,null

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 

Update tblProducts Set InhibitAccounting = 0 
where ProductID in
(Select Distinct Product from tblTransactionLineItems where Product is not null and TransactionInventoryDate > DATEADD(month,-6,getDate()))

Update tblProducts Set InhibitAccounting = 1 
where ProductID not in
(Select Distinct Product from tblTransactionLineItems where Product is not null and TransactionInventoryDate > DATEADD(month,-6,getDate()))
