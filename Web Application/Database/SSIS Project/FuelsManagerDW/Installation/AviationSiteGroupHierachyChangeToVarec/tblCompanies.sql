/**************************************************************************************************
ENTITY: COMPANIES:
 Notes: 
 - There are companies created at aviation
 - There are companies mapped directly to the Varec >> Aviation hierarchy and others only mapped to Aviation.
 Fix: Map all company related tables from Aviation to Varec

Tables: 
tblcompanies  
map.tblCompanyCompanyToUserGroup
map.tblCompanyToRole
map.tblEntityCompanyToSite


**************************************************************************************************/

-- tblCompanies --- Companies assigned to Aviation siteguid
-- companies created at aviation
Select a.Id CompanyId, b.Id SiteId, * From tblCompanies a
Inner Join tblSites b on b.SiteGuid = a.SiteGuid
Where a.CompanyGuid = _MasterRecordGuid
and b.Id = 'Aviation'

--select * from aviation_companies
Select a.* 
INTO aviation_companies
From tblCompanies a
Inner Join tblSites b on b.SiteGuid = a.SiteGuid
Where a.CompanyGuid = _MasterRecordGuid
and b.Id = 'Aviation'





-- duplicate issue --- Companies created both at Varec and Aviation Sitegroups

select siteguid, * from  tblCompanies
where id in (
select distinct a.id from tblCompanies c
, tblcompanies a
where c.id=a.id
group by a.id, c.id--,c.companyguid, a.companyguid
having count(*)>1
)
order by id

--- Aviation_VarecDuplicateCompanies
select * into Aviation_VarecDuplicateCompanies
from  tblCompanies
WHERE SITEGUID='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
and id in (
select distinct a.id from tblCompanies c
, tblcompanies a
where c.id=a.id
group by a.id, c.id--,c.companyguid, a.companyguid
having count(*)>1
)
order by id

--- Varec_AviationDuplicateCompanies
select * into Varec_AviationDuplicateCompanies
from  tblCompanies
WHERE SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
and id in (
select distinct a.id from tblCompanies c
, tblcompanies a
where c.id=a.id
group by a.id, c.id--,c.companyguid, a.companyguid
having count(*)>1
)
order by id

-- ChildSite Assignment Created on 
select * from Varec_AviationDuplicateCompanies
where
id not in (
select v.id from Varec_AviationDuplicateCompanies v
inner join Aviation_VarecDuplicateCompanies a
on v.id=a.id)

--select siteguid, companyguid, * from tblCompanies where id='RU - Air Bridge Cargo'
--select id, * from tblsites where siteguid='A234A347-4AAA-4B66-AF7A-59749E09A83D'


-- REMOVE CHILDSITE
DELETE FROM Varec_AviationDuplicateCompanies
WHERE ID NOT IN 
(select ID from Varec_AviationDuplicateCompanies
where
id in (
select v.id from Varec_AviationDuplicateCompanies v
inner join Aviation_VarecDuplicateCompanies a
on v.id=a.id)
)


--select siteguid, * from Varec_AviationDuplicateCompanies v
--select * from  Aviation_VarecDuplicateCompanies


--- Note: There are no transactions on the Varec Sitegroup assigment of these duplicates
-- Varec Entity Assignments for the Duplicates
 -- SELECT * FROM VarecChildEntityCompanies

select  a.CompanyToSiteGuid,a.companyguid, p._masterrecordguid,a.siteguid, a.AssignedFromSiteGuid, p.id, a.createddate, a.updateddate
into VarecChildEntityCompanies
from map.tblEntityCompanyToSite a
inner join tblCompanies p on a.Companyguid=p.Companyguid
and a.companyguid=p._masterrecordguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
 a.CompanyGuid in
 (select _masterrecordguid from Varec_AviationDuplicateCompanies)



 -- Delete VarecChildEntityCompanies for Companies assigned both to Varec and Aviation Sitegroups
DELETE FROM map.tblEntityCompanyToSite
WHERE CompanyToSiteGuid IN
(SELECT CompanyToSiteGuid FROM VarecChildEntityCompanies)


--- Validation
SELECT * FROM map.tblEntityCompanyToSite
WHERE CompanyGuid IN
(SELECT CompanyGuid FROM VarecChildEntityCompanies)


--- map.tblCompanyToRole Companies created at Varec with no transacations that are also created at Aviation Sitegroups

-- VALIDATION 
select * from map.tblCompanyToRole
WHERE Companyguid in 
(select c.companyguid from tblCompanies c
inner join Varec_AviationDuplicateCompanies v
on c.Companyguid=v.Companyguid
and c._masterrecordguid=v._masterrecordguid
and c.siteguid=v.siteguid)


DELETE FROM map.tblCompanyToRole
WHERE Companyguid in 
(select c.companyguid from tblCompanies c
inner join Varec_AviationDuplicateCompanies v
on c.Companyguid=v.Companyguid
and c._masterrecordguid=v._masterrecordguid
and c.siteguid=v.siteguid)




-- DELETE  Companies created at Varec with no transacations that are also created at Aviation Sitegroups

DELETE FROM dbo.tblCompanies 
WHERE Companyguid in 
(select c.companyguid from tblCompanies c
inner join Varec_AviationDuplicateCompanies v
on c.Companyguid=v.Companyguid
and c._masterrecordguid=v._masterrecordguid
and c.siteguid=v.siteguid)


-- VALIDATION 

select c.* from tblCompanies c
WHERE Companyguid in 
(select c.companyguid from tblCompanies c
inner join Varec_AviationDuplicateCompanies v
on c.Companyguid=v.Companyguid
and c._masterrecordguid=v._masterrecordguid
and c.siteguid=v.siteguid)


/************ REMAP AVIATION ASSIGNED COMPANIES TO VAREC  ****/

-- tblCompanies 
--- Map Companies from Aviation to Varec
Select siteguid, * from aviation_companies


UPDATE
dbo.tblCompanies 
SET siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
FROM tblCompanies  C
where  c.siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


--- VALIDATIONS
SELECT * FROM tblCompanies
where  siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
-------------------------------------------------------------------------------------



