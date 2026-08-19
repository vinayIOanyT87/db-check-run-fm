/************************************************************************
TBLSITES AND RELATED TABLES 

NOTE: Sites: There are no sites directly assigned to Aviation, only at site groups level.

i. Create a new hierarchy Site-to-Site mappings from Varec to all the Site and Site groups under Aviation
Tables:
- map.tblSiteToSite
- tblsites 
- dbo.tblSitesAncillaryData
- dbo.tblSitesShadow
*********************************************************************************/


-- STEP 1:  dbo.tblSitesAncillaryData -- delete Aviation before deleting Aviation Site from tblsites

select * from dbo.tblSitesAncillaryData
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


DELETE FROM
dbo.tblSitesAncillaryData
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

--VALIDATION
SELECT * FROM
dbo.tblSitesAncillaryData
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

----------------------------------------------------------------------------------------

-- STEP 2: dbo.tblUsers
SELECT * FROM dbo.tblUsers
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

UPDATE dbo.tblUsers
SET siteguid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE Userguid in 
(SELECT userguid FROM dbo.tblUsers
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')


----------------------------------------------------------------------------------------------
---- STEP 3: map.tblSiteToSite

-- drop table AviationParentChildSiteGroup

Select b.Id ParentSiteId, b.SiteGroupFlag, b.siteguid ParentSiteguid, c.Id ChildSiteId, a.ChildSiteGuid 
Into AviationParentChildSiteGroup
From map.tblSiteToSite a
Inner Join tblSites b
On b.SiteGuid = a.ParentSiteGuid
Inner Join tblSites c
On c.SiteGuid = a.ChildSiteGuid
Where b.Id = 'Aviation' and b.SiteGroupFlag=1  
AND a.ChildSiteGuid<> '4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
order by 3 Desc


--SELECT * FROM AviationParentChildSiteGroup

-- ChildSite = Aviation
-- drop table AviationChildSiteGroup
Select b.Id ParentSiteId, b.SiteGroupFlag, b.siteguid ParentSiteguid, c.Id ChildSiteId, a.ChildSiteGuid
Into AviationChildSiteGroup
From map.tblSiteToSite a
Inner Join tblSites b
On b.SiteGuid = a.ParentSiteGuid
Inner Join tblSites c
On c.SiteGuid = a.ChildSiteGuid
Where c.Id = 'Aviation' and b.SiteGroupFlag=1
order by 3 Desc

SELECT * FROM AviationChildSiteGroup


--select * from AviationParentChildSiteGroup
-- select * from  AviationChildSiteGroup 

select * from dbo.tblsites where siteguid='6733898E-CFE0-4067-9404-7CEC984BC955'

-- STEP: change the parentsiteguid to Varec on map.tblSiteToSite 
update map.tblSiteToSite 
set ParentSiteguid='6733898E-CFE0-4067-9404-7CEC984BC955'
from map.tblSiteToSite m
inner join  AviationParentChildSiteGroup p
on m.childsiteguid=p.childsiteguid
and m.parentsiteguid=p.parentsiteguid


-- STEP: Delete Aviation Mappings FROM  map.tblSiteToSite 
DELETE
From map.tblSiteToSite 
where siteToSiteGuid in
(Select siteToSiteGuid
From map.tblSiteToSite a
Inner Join tblSites b
On b.SiteGuid = a.ParentSiteGuid
Inner Join tblSites c
On c.SiteGuid = a.ChildSiteGuid
Where c.Id = 'Aviation' and b.SiteGroupFlag=1)


-- VALIDATION  map.tblSiteToSite Aviation to Varec Hierachy Changes
-- Childsites are now mapped to Varec

select m.* from 
 map.tblSiteToSite m
inner join  AviationParentChildSiteGroup p
on m.childsiteguid=p.childsiteguid
and m.parentsiteguid=
'6733898E-CFE0-4067-9404-7CEC984BC955'



select m.* from  map.tblSiteToSite m 
where  m.parentsiteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Select b.Id ParentSiteId, b.SiteGroupFlag, b.siteguid ParentSiteguid, c.Id ChildSiteId, c.Siteguid
From map.tblSiteToSite a
Inner Join tblSites b
On b.SiteGuid = a.ParentSiteGuid
Inner Join tblSites c
On c.SiteGuid = a.ChildSiteGuid
Where
b.Id = 'Varec' and b.SiteGroupFlag=1  
and a.ChildSiteGuid in 
(select ChildSiteGuid from AviationParentChildSiteGroup )

--- Aviation site does not exist in map.tblSiteToSite
select * 
From map.tblSiteToSite 
where siteToSiteGuid in
(Select siteToSiteGuid
From map.tblSiteToSite a
Inner Join tblSites b
On b.SiteGuid = a.ParentSiteGuid
Inner Join tblSites c
On c.SiteGuid = a.ChildSiteGuid
Where c.Id = 'Aviation' and b.SiteGroupFlag=1)


-------------------------------------------------------------------------------------------

-- STEP: dbo.tblSitesShadow -- delete after tblsites after running fmaudit log

select * from dbo.tblSitesShadow
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


DELETE 
FROM dbo.tblSitesShadow
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

-- VALIDATION
SELECT * FROM dbo.tblSitesShadow
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

/**************************************************************************************
LAST STEPS TO DELETE AVIATION SITE

***************************************************************************************/



--- STEP 1 : Delete aviation record versioning data  erv.tblEntityRecordVersioningFieldConfig
select * 
into EntityRecordVersioningFieldConfigAviationSitegroup
from erv.tblEntityRecordVersioningFieldConfig
where sitegroupguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

DELETE FROM erv.tblEntityRecordVersioningFieldConfig
WHERE sitegroupguid in 
(select sitegroupguid from EntityRecordVersioningFieldConfigAviationSitegroup)

--- VALIDATION
  select * from erv.tblEntityRecordVersioningFieldConfig
where sitegroupguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
  


--STEP 2 : dbo.tblSites Delete Aviation Site Completely
select * from dbo.tblsites where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 

DELETE 
FROM dbo.tblSites
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
