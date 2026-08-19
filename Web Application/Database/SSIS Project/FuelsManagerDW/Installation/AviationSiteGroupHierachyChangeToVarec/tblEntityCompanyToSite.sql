/******** map.tblEntityCompanyToSite ***********/
--- SELECT * FROM map.tblEntityCompanyToSite 

---STEP:  Aviation EntityCompanyToSite Self Assignments
select  a.CompanyToSiteGuid,a.companyguid, p._masterrecordguid,a.siteguid, a.AssignedFromSiteGuid, p.id, a.createddate, a.updateddate
into AviationSelfMappedEntityCompanies
from map.tblEntityCompanyToSite a
inner join tblCompanies p on a.Companyguid=p.Companyguid
and a.companyguid=p._masterrecordguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid-- parent
Inner Join tblSites d on d.SiteGuid = a.SiteGuid --child
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' and
a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

--SELECT * FROM AviationSelfMappedEntityCompanies


UPDATE map.tblEntityCompanyToSite
SET siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955',
AssignedFromSiteGuid ='6733898E-CFE0-4067-9404-7CEC984BC955'
where CompanyToSiteGuid in (select CompanyToSiteGuid from AviationSelfMappedEntityCompanies)

--- VALIDATION
SELECT siteguid,AssignedFromSiteGuid,* FROM  map.tblEntityCompanyToSite
where CompanyToSiteGuid in (select CompanyToSiteGuid from AviationSelfMappedEntityCompanies)



-- STEP:  EntityCompanyToSite Assignments from Aviation
select  a.CompanyToSiteGuid,a.companyguid, p._masterrecordguid,a.siteguid, a.AssignedFromSiteGuid, p.id, a.createddate, a.updateddate
into AviationChildMappedEntityCompanies
from map.tblEntityCompanyToSite a
inner join tblCompanies p on a.Companyguid=p.Companyguid
and a.companyguid=p._masterrecordguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 
and a.siteguid <>'4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



--SELECT * FROM AviationChildMappedEntityCompanies

UPDATE map.tblEntityCompanyToSite
SET AssignedFromSiteGuid ='6733898E-CFE0-4067-9404-7CEC984BC955'
where CompanyToSiteGuid in (select CompanyToSiteGuid from AviationChildMappedEntityCompanies)

-- VALIDATION

SELECT AssignedFromSiteGuid, SITEGUID,* FROM map.tblEntityCompanyToSite
where CompanyToSiteGuid in (select CompanyToSiteGuid from AviationChildMappedEntityCompanies)


-- STEP: EntityCompanies assigned from Varec to Aviation
select  a.CompanyToSiteGuid,a.companyguid, p._masterrecordguid,a.siteguid, a.AssignedFromSiteGuid, p.id, a.createddate, a.updateddate
into  AviationEntityCompaniesAssignedFromVarec
from map.tblEntityCompanyToSite a
inner join tblCompanies p on a.Companyguid=p.Companyguid
and a.companyguid=p._masterrecordguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid-- parent
Inner Join tblSites d on d.SiteGuid = a.SiteGuid --child
where
a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

-- select * from AviationEntityCompaniesAssignedFromVarec

--- DELETE EntityCompanies assigned from Varec to Aviation

DELETE FROM map.tblEntityCompanyToSite
where CompanyToSiteGuid in (select CompanyToSiteGuid from AviationEntityCompaniesAssignedFromVarec)

-- validation -- CompanyEntities mapped to Varec only
select * FROM map.tblEntityCompanyToSite m
inner join 
AviationEntityCompaniesAssignedFromVarec v
on v.companyguid=m.companyguid
and v.assignedfromsiteguid=m.assignedfromsiteguid
and m.siteguid='6733898E-CFE0-4067-9404-7CEC984BC955'


select  a.CompanyToSiteGuid,a.companyguid, p._masterrecordguid,a.siteguid, a.AssignedFromSiteGuid, p.id, a.createddate, a.updateddate
from map.tblEntityCompanyToSite a
inner join tblCompanies p on a.Companyguid=p.Companyguid
and a.companyguid=p._masterrecordguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid-- parent
Inner Join tblSites d on d.SiteGuid = a.SiteGuid --child
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
OR a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'




--------------------------------------------------------------------------------------------
--- STEP:   map.tblCompanyCompanyToUserGroup CompanyCompanyToUserGroup Aviation Assignment - Map to Varec

select * from map.tblCompanyCompanyToUserGroup
where  siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'--Validation

UPDATE map.tblCompanyCompanyToUserGroup
SET  siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
where  siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

----------------------------------------------------------------------------------------------
--- STEP:  map.tblCompanyToRole CompanyToRole Aviation Assignment - Map to Varec
select * from map.tblCompanyToRole
where  siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' --Validation

UPDATE map.tblCompanyToRole
SET  siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
where  siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 



-----------------------------------------------------------------------------------------------------

