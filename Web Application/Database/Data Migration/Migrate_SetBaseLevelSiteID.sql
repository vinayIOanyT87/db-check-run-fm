USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/****** Object:  StoredProcedure [dbo].[Migrate_SetBaseLevelSiteID]    Script Date: 03/15/2010 11:39:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Migrate_SetBaseLevelSiteID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[Migrate_SetBaseLevelSiteID]
GO


CREATE PROCEDURE [dbo].[Migrate_SetBaseLevelSiteID]
 /*=============================================
 Author:			Eric Simmons
 Create date:		3/25/2010
 Description:		Migrating FuelsManager Defense 6.0 transaction to FuelsManager 8.0 tblTransactions
 Modification History:
	Date		by			Description
	
 =============================================*/
/*

EXEC Migrate_SetBaseLevelSiteID 1,''

*/
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID NVarChar(MAX) = NULL

AS 

if(@IsBaseDB <> 2)
BEGIN
	IF NOT EXISTS(Select * from sys.databases where [name] = 'ConsolidatedDB6')
	BEGIN
		Select 'ConsolidatedDB6 was not detected.  Please attached a FuelsManager Defense 6.0 SP4 ConsolidatedDB Database before running this stored procedure';
		return
	END


	/*if((Select COUNT(SiteIndex) from tblSites) <> 2)
	BEGIN
		Select 'A base level site must have only two sites in the database.  The "SiteAdmin" site and the actual site.';
		return;
	END*/
	select * from tblSites
	declare @siteIndex6 int;
	declare @siteIndex8 int;
	declare @siteID6 nvarchar(50);
	set @siteIndex6 = (Select MAX(SiteIndex) from ConsolidatedDB6.dbo.tblSites);
	set @siteIndex8 = (Select MAX(SiteIndex) from ConsolidatedDB.dbo.tblSites where ID='New Site');
	set @siteID6 = (Select top 1 SiteID from ConsolidatedDB6.dbo.tblSites where SiteIndex = @siteIndex6);
	Update tblSites Set ID = @siteID6 where SiteIndex = @siteIndex8;
	select * from tblSites
END
