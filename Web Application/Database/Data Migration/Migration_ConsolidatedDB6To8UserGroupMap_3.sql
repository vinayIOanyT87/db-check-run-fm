
USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8UserGroupMap_3]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8UserGroupMap_3') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8UserGroupMap_3
GO

CREATE PROCEDURE [dbo].Migration_ConsolidatedDB6To8UserGroupMap_3
 /*=============================================
 Author:			URVI PATEL
 Create date:		1/20/2010
 Description:		Migrating ConsolidatedDB 6.0 to ConsolidatedDB 8.0 User Group Map Table
 Modification History:
	Date		by		Description
	
 =============================================*/
/*
EXEC Migration_ConsolidatedDB6To8UserGroupMap_3 2, null
EXEC Migration_ConsolidatedDB6To8UserGroupMap_3 'FP4814'

*/
(
@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise 
@SiteID NVarChar(MAX) = NULL
)
AS 
IF @SiteID = 'All Sites' or @IsBaseDB <> 2 SET @SiteID = NULL

INSERT INTO [consolidatedDB].dbo.tblUserGroupMap (UserIndex, GroupIndex, CreatedDate, CreatedBy)

SELECT UserIndex8, 
		GroupIndex8,
		GETDATE() AS 'CreatedDate',
		'Varec' AS 'CreatedBy'
FROM (SELECT DISTINCT users8.Userindex AS UserIndex8, group8.groupindex AS GroupIndex8
		FROM  [ConsolidatedDB6].[dbo].tblUsers u
		INNER JOIN [ConsolidatedDB6].[dbo].tblsites s
		ON s.siteindex = u.siteindex
		INNER JOIN [consolidatedDB6].dbo.tblUserGroupMap gm 
		ON gm.userindex = u.userindex
		AND gm.siteindex = u.siteindex
		INNER JOIN [consolidatedDB6].dbo.tblGroups g 
		ON g.groupindex = gm.groupindex
		INNER JOIN [consolidatedDB].dbo.tblsites site8 
		ON site8.id = s.siteid
		INNER JOIN [consolidatedDB].dbo.tblusers users8 
		ON users8.userid = u.userid
		AND site8.siteindex = users8.siteindex
		INNER JOIN [consolidatedDB].dbo.tblGroups Group8 
		ON Group8.groupid = g.groupid AND -1 = group8.siteindex
		WHERE s.deleteflag = 0 and s.RegionalSiteFlag = 0 and u.deleteflag = 0
		AND s.siteindex <> -1
		--AND u.userid = 'Yi'
		AND s.siteid =  isnull(@SiteID, s.siteid)) A
Where CONVERT(nvarchar(10),GroupIndex8) + '_' + Convert(nvarchar(10),UserIndex8) not in
(Select CONVERT(nvarchar(10),GroupIndex) + '_' + Convert(nvarchar(10),UserIndex) from 
[consolidatedDB].dbo.tblUserGroupMap)
order by UserIndex8, GroupIndex8

