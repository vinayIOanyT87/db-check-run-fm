USE [ConsolidatedDB]
GO
/****** Object:  StoredProcedure [dbo].[Migration_ConsolidatedDB6To8Users_2]    ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.Migration_ConsolidatedDB6To8Users_2') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.Migration_ConsolidatedDB6To8Users_2
GO

CREATE PROCEDURE dbo.Migration_ConsolidatedDB6To8Users_2
 /*=============================================
 Author:			URVI PATEL
 Create date:		1/13/2010
 Description:		Migrating ConsolidatedDB 6.0 to ConsolidatedDB 8.0 Users Table
 Modification History:
	Date		by		Description
	
 =============================================*/
/*

EXEC Migration_ConsolidatedDB6To8Users_2 2, null


*/
(

@IsBaseDB smallint, -- 0 = Base to Base, 1 = Base to Enterprise, 2 = Enterprise to Enterprise
@SiteID varchar(max) = NULL

)

AS 

IF @SiteID = 'All Sites' or @IsBaseDB <> 2 SET @SiteID = NULL

--BEGIN TRANSACTION



--select * FROM [ConsolidatedDB].[dbo].tblUsers 


INSERT INTO [ConsolidatedDB].[dbo].tblUsers
(
 SiteIndex, 
 UserID, 
 [Password],
 UpdatedDate, 
 UpdatedBy, 
 CreatedDate, 
 CreatedBy, 
 LastLoginDate, 
 LastLogoffDate, 
 PasswordTimeStamp, 
 PasswordLockoutCount, 
 InactivityLockout, 
 ChangePassword
)

SELECT 	-1, --SiteIndex 
		u.UserID, 
		0x4D4849474353734741515142676A64594136426C4D474D474369734741515142676A6459417747675654425441674D4341414543416D595141674942414151510D0A414141414141414141414141414141414141414141415151753941484C6D4C4D774B5753726B3465414377416C6751676F5743784D49364A46774A69775531480D0A30523339747A38714250304354726F366F6D2F766E6A4C5244646F3D0D0A as [password], 
		MAX(ISNULL(u.UpdatedDate, GETDATE())) as updateddate, 
		'Varec' as updatedby, 
		MIN(ISNULL(u.CreatedDate, u.UpdatedDate)) as createddate,
		'Varec'  as createdby,
		MAX(ISNULL(u.LastLoginDate, u.CreatedDate)) as LastLoginDate, 
		MAX(ISNULL(u.LastLogoffDate, u.CreatedDate)) as LastLogoffDate, 
        GETDATE() as PasswordTimeStamp, 
        0 as PasswordLockoutCount, 
        0 as InactivityLockout, 
        1 as ChangePassword
FROM  [ConsolidatedDB6].[dbo].tblUsers u
INNER JOIN [ConsolidatedDB6].[dbo].tblsites s
ON s.siteindex = u.siteindex
INNER JOIN [consolidatedDB].dbo.tblsites site8 
ON site8.id = s.siteid
WHERE u.deleteflag = 0 and s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1
AND s.siteid =  isnull(@SiteID, s.siteid)
AND UPPER(u.UserID) NOT IN (SELECT UPPER(UserID) FROM [ConsolidatedDB].[dbo].tblUsers)
GROUP BY UserID

--select * FROM [ConsolidatedDB].[dbo].tblUsers 

INSERT INTO [consolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT 'Users',
		site8.SiteIndex AS '8.0SiteIndex',
		users8.Userindex AS '8.0UserIndex', 
		users8.CreatedDate AS 'CreatedDate',
		users8.CreatedBy AS 'CreatedBy'
				
FROM  [ConsolidatedDB6].[dbo].tblUsers u
INNER JOIN [ConsolidatedDB6].[dbo].tblsites s
ON s.siteindex = u.siteindex
INNER JOIN [consolidatedDB].dbo.tblsites site8 
ON site8.id = s.siteid
INNER JOIN [consolidatedDB].dbo.tblusers users8 
ON users8.userid = u.userid AND site8.siteindex = users8.siteindex
WHERE u.deleteflag = 0 and s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1
AND s.siteid =  isnull(@SiteID, s.siteid)
AND users8.UserIndex NOT IN (SELECT [Index] FROM  [consolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Users' AND SiteIndex=site8.SiteIndex)


/*Add administrator access to migrated sites*/
INSERT INTO [consolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT 'Users',
		site8.SiteIndex AS '8.0SiteIndex',
		u8.userindex AS '8.0userIndex', 
		GETDATE() AS 'CreatedDate',
		'Varec'
FROM  [ConsolidatedDB].[dbo].tblUsers u8,
[consolidatedDB].dbo.tblsites site8 
INNER JOIN [ConsolidatedDB6].[dbo].tblsites s
ON site8.id = s.siteid
WHERE s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1 AND UPPER(u8.UserID) IN ('ADMINISTRATOR','BSMEADMIN')
AND s.siteid =  isnull(@SiteID, s.siteid)
AND u8.UserIndex NOT IN (SELECT [Index] FROM  [consolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Users' AND SiteIndex=site8.SiteIndex)

/*Add assign users to proper migrated sites*/
INSERT INTO [consolidatedDB].dbo.tblEntityToSiteMap (TypeID, SiteIndex,[Index], CreatedDate, CreatedBy)
SELECT 'Users' as typeid,
		site8.SiteIndex AS SiteIndex8,
		u8.userindex AS UserIndex8, 
		GETDATE() AS 'CreatedDate',
		'Varec' as createdby
FROM  [ConsolidatedDB].[dbo].tblUsers u8 join
[ConsolidatedDB6].[dbo].tblUsers u6 
on u8.UserID=u6.userid
JOIN [ConsolidatedDB6].[dbo].tblsites s
on s.SiteIndex=u6.siteindex
join [consolidatedDB].dbo.tblsites site8  
ON site8.id = s.siteid
WHERE u6.deleteflag=0 and s.deleteflag = 0 and s.RegionalSiteFlag = 0
AND s.siteindex <> -1 AND UPPER(u8.UserID) NOT IN ('ADMINISTRATOR','BSMEADMIN')
AND s.siteid =  isnull(@SiteID, s.siteid)
AND u8.UserIndex NOT IN (SELECT [Index] FROM  [consolidatedDB].dbo.tblEntityToSiteMap WHERE TypeID='Users' AND SiteIndex=site8.SiteIndex)

IF EXISTS(SELECT * FROM ConsolidatedDB6.sys.tables WHERE name='tblArchivedUsers')
BEGIN
INSERT INTO [ConsolidatedDB].[dbo].[tblArchivedUsers] (
	[UserIndex]
	,[SiteIndex]
	,[UserID]
	,[Name]
	,[Password]
	,[LastLoginDate]
	,[LastLogoffDate]
	,[ChangePassword]
	,[PasswordTimeStamp]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	,[InactivityLockout]
	,[ArchivedDate]
)
/* Migrate tblArchivedUsers */
SELECT 
	A.[UserIndex]
	,S8.[SiteIndex]
	,A.[UserID]
	,A.[UserID] --Name
	,A.[Password]
	,ISNULL([LastLoginDate], ISNULL([LastLogoffDate], '1/1/1900'))
	,ISNULL([LastLogoffDate], ISNULL([LastLoginDate], '1/1/1900'))
	,A.[ForcePWDChangeFlag]
	,A.[LastPasswordChange]
	,GETDATE()
	,'Varec'
	,ISNULL(A.[UpdatedDate], ISNULL(A.CreatedDate,GETDATE()))
	,ISNULL(A.[UpdatedBy],ISNULL(A.CreatedBy,'Varec'))
	,A.[Disabled]
	,A.[ArchivedDate]
  FROM [ConsolidatedDB6].[dbo].[tblArchivedUsers] A 
  JOIN [ConsolidatedDB6].[dbo].[tblSites] s6 ON s6.SiteIndex = A.SiteIndex
  JOIN [ConsolidatedDB].[dbo].[tblSites] s8 ON s6.SiteID = s8.ID
  WHERE A.[UserID] NOT IN (SELECT UserID FROM [ConsolidatedDB].[dbo].[tblArchivedUsers])
  UNION
  SELECT u.[UserIndex]
	,s8.[SiteIndex]
	,u.[UserID]
	,u.[UserID]  -- Name
	,u.[Password]
	,ISNULL([LastLoginDate], ISNULL([LastLogoffDate], '1/1/1900'))
	,ISNULL([LastLogoffDate], ISNULL([LastLoginDate], '1/1/1900'))
	,u.[ForcePWDChangeFlag]
	,u.[LastPasswordChange]
	,GETDATE()
	,'Varec'
	,ISNULL(u.[UpdatedDate], ISNULL(u.CreatedDate,GETDATE()))
	,ISNULL(u.[UpdatedBy],ISNULL(u.CreatedBy,'Varec'))
	,u.[Disabled]
	,GETDATE()
  FROM [ConsolidatedDB6].[dbo].[tblUsers] u 
  JOIN [ConsolidatedDB6].[dbo].[tblSites] s6 ON s6.SiteIndex = u.SiteIndex
  JOIN [ConsolidatedDB].[dbo].[tblSites] s8 ON s6.SiteID = s8.ID  
  WHERE u.DeleteFlag = 1
  AND u.[UserID] NOT IN (SELECT UserID FROM [ConsolidatedDB6].[dbo].[tblArchivedUsers])
  AND u.[UserID] NOT IN (SELECT UserID FROM [ConsolidatedDB].[dbo].[tblArchivedUsers])
 END
 ELSE
 BEGIN
 INSERT INTO [ConsolidatedDB].[dbo].[tblArchivedUsers] (
	[UserIndex]
	,[SiteIndex]
	,[UserID]
	,[Name]
	,[Password]
	,[LastLoginDate]
	,[LastLogoffDate]
	,[ChangePassword]
	,[PasswordTimeStamp]
	,[CreatedDate]
	,[CreatedBy]
	,[UpdatedDate]
	,[UpdatedBy]
	,[InactivityLockout]
	,[ArchivedDate]
)
  SELECT u.[UserIndex]
	,s8.[SiteIndex] 
	,u.[UserID]
	,u.[UserID]  -- Name
	,u.[Password]
	,ISNULL([LastLoginDate], ISNULL([LastLogoffDate], '1/1/1900'))
	,ISNULL([LastLogoffDate], ISNULL([LastLoginDate], '1/1/1900'))
	,u.[ForcePWDChangeFlag]
	,u.[LastPasswordChange]
	,GETDATE()
	,'Varec'
	,ISNULL(u.[UpdatedDate], ISNULL(u.CreatedDate,GETDATE()))
	,ISNULL(u.[UpdatedBy],ISNULL(u.CreatedBy,'Varec'))
	,u.[Disabled]
	,GETDATE()
  FROM [ConsolidatedDB6].[dbo].[tblUsers] u 
  JOIN [ConsolidatedDB6].[dbo].[tblSites] s6 ON s6.SiteIndex = u.SiteIndex
  JOIN [ConsolidatedDB].[dbo].[tblSites] s8 ON s6.SiteID = s8.ID  
  WHERE u.DeleteFlag = 1
  AND u.[UserID] NOT IN (SELECT UserID FROM [ConsolidatedDB].[dbo].[tblArchivedUsers])
 END
 
 --select UserID, m.siteindex FROM [ConsolidatedDB].[dbo].tblEntityToSiteMap m join [ConsolidatedDB].[dbo].tblUsers u
 --on m.[Index]  = u.[userindex] and typeid='Users' order by UserID, m.siteindex

  --ROLLBACK
