
CREATE PROCEDURE [dbo].[usp_GetUserGroupsAcrossSites] @loggedInUser uniqueidentifier, @SiteGuid uniqueidentifier, @loadChildrenSites bit, @userToModify uniqueidentifier, @filter nvarchar(100) = null
AS 

SET NOCOUNT ON

DECLARE @emptyGuid uniqueidentifier
SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)


IF (@filter IS NOT NULL AND LEN(@filter) > 0 )
BEGIN
	SET @filter = '%' + @filter +'%'
END


declare @sitesTable table
(
	SiteGuid uniqueidentifier
)


INSERT INTO @sitesTable SELECT @siteGuid

if (@loadChildrenSites = 1 )
BEGIN
	-- all sites current user is entity assigned to
	insert into @sitesTable SELECT DISTINCT [ChildSiteGuid] FROM map.tblSiteToSite SM
	INNER join tblSites S on S.SiteGuid = SM.ChildSiteGuid
	WHERE SM.ParentSiteGuid = @SiteGuid 
END


--remove sites where the user does not have permission to modify users
DELETE S FROM @sitesTable S
LEFT JOIN map.tblUserToGroup UGM 
	INNER JOIN map.tblGroupToRight GRM ON UGM.UserGuid = @loggedInUser AND UGM.GroupGuid =GRM.GroupGuid AND GRM.LookupRightIndex = 2 --modify users permission
	ON  UGM.SiteGuid = S.SiteGuid
WHERE UGM.SiteGuid IS NULL


-- get potential users to modify
declare @usersTable table
(
	UserGuid uniqueidentifier,
	SiteGuid uniqueidentifier
)

if (@userToModify = @emptyGuid )
BEGIN
	-- all users for the sites the current user is entity assigned to
	insert into @usersTable SELECT ESM.[UserGuid],  ESM.[SiteGuid] 
		FROM map.tblEntityUserToSite ESM 
			INNER JOIN @sitesTable S ON S.[SiteGuid] = ESM.[SiteGuid]
END
ELSE
BEGIN
	-- just the user specified
	insert into @usersTable SELECT ESM.[UserGuid],  ESM.[SiteGuid] 
		FROM map.tblEntityUserToSite ESM 
			INNER JOIN @sitesTable S ON S.[Siteguid] = ESM.[Siteguid] AND ESM.UserGuid = @userToModify
END

create table #SiteGroups
(
	SiteGuid uniqueidentifier,
	GroupGuid uniqueidentifier,
	AssignedValue int, -- 1 means they have, 0 means they don't have and -1 means they can't get it
)

create table #UserSiteGroups
(
	SiteGuid uniqueidentifier,
	UserGuid uniqueidentifier,
	GroupGuid uniqueidentifier,
	AssignedValue int, -- 1 means they have, 0 means they don't have and -1 means they can't get it
)



;WITH AllSiteGroups (GroupGuid, SiteGuid)
AS
(
	select G.GroupGuid, ST.SiteGuid 
	from tblGroups G 
	CROSS APPLY  @sitesTable ST
)
--get all groups for the site(s) specified and if they are assigned to the site
INSERT INTO #SiteGroups
SELECT ASG.SiteGuid,ASG.GroupGuid, CASE WHEN ESM.SiteGuid IS NULL THEN 0 ELSE 1 END
from  AllSiteGroups ASG 
LEFT OUTER JOIN map.tblEntityUserGroupToSite ESM 
	ON ESM.[GroupGuid] = ASG.GroupGuid AND ESM.SiteGuid = ASG.SiteGuid
	
CREATE INDEX IDX_SiteGroups on #SiteGroups(Siteguid,Groupguid)


--get the groups users have
INSERT INTO #UserSiteGroups
select  LUSG.[SiteGuid], UT.[UserGuid], LUSG.[GroupGuid], 
	CASE
		WHEN CAST(LUGM.DenyADPermission AS INT) = 1 THEN 2  
		WHEN LUGM.GroupGuid IS NULL THEN 0 
		ELSE 1 END
from #SiteGroups LUSG
	INNER JOIN @usersTable UT ON LUSG.SiteGuid = UT.SiteGuid 
	LEFT JOIN map.tblUserToGroup LUGM ON 
		LUSG.SiteGuid = LUGM.SiteGuid AND 
		LUSG.GroupGuid = LUGM.GroupGuid AND 
		LUGM.UserGuid = UT.UserGuid

CREATE INDEX IDX_UserSiteGroups_2 on #UserSiteGroups(SiteGuid,GroupGuid)

--build the group list for the column text
DECLARE @cols1 NVARCHAR(MAX)
SELECT  @cols1 = STUFF(( SELECT ',' + QUOTENAME(G.GroupGuid)
                        FROM    #SiteGroups AS U INNER JOIN tblGroups G on U.GroupGuid = G.GroupGuid
                        GROUP BY G.GroupGuid, G.GroupID
                        ORDER BY G.GroupID
                        FOR XML PATH('')
                      ), 1, 1, '')

print @cols1

DECLARE @query1 NVARCHAR(MAX)
SET @query1 = N'SELECT pvt.[SiteGuid], s.id AS SiteID,  U.[UserGuid], U.UserID, CASE WHEN U.SiteGuid = pvt.SiteGuid THEN 1 ELSE 0 END AS OwnedBy, '
+ @cols1 +'
FROM
(SELECT  DISTINCT 
      UserGroups.[SiteGuid], UserGroups.UserGuid, SiteGroups.GroupGuid, CASE WHEN SiteGroups.AssignedValue = 0 THEN -1 ELSE UserGroups.AssignedValue END AS MODGroupIndex
FROM    #SiteGroups SiteGroups
    INNER JOIN #UserSiteGroups UserGroups ON UserGroups.SiteGuid = SiteGroups.SiteGuid AND 
		UserGroups.GroupGuid = SiteGroups.GroupGuid) p
   PIVOT
(
MAX(P.[MODGroupIndex])
FOR P.[GroupGuid] IN
( '+ @cols1 +' )
) AS pvt
	INNER JOIN tblSites S on pvt.SiteGuid = S.SiteGuid
	INNER JOIN tblUsers U on pvt.UserGuid = U.UserGuid'
	
	
IF (@filter IS NOT NULL AND LEN(@filter) > 0 )
BEGIN
	SET @query1 = @query1 +
		' WHERE S.ID like @queryFilter  OR U.UserID LIKE @queryFilter'
END

--print @query1
EXEC sp_executesql @query1, N' @queryFilter nvarchar(100)', @filter

drop table #UserSiteGroups
drop table #SiteGroups



GO


