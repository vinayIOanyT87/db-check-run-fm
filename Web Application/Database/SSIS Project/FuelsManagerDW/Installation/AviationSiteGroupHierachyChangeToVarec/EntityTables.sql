/******************************************************************************************************************
Entity Tables with related Aviaition Siteguid and AssignedFromToSiteGuid Data

Fix: Map Aviation Records to Varec

Tables:
map.tblEntityQuerySettingToSite
map.tblEntityListViewToSite
map.tblEntityListViewToSite
map.tblEntityReportConfigurationSettingsToSite
map.tblEntityTransactionAliasToSite
map.tblEntityUserDataToSite
map.tblEntityUserGroupToSite
map.tblEntityUserToSite
*****************************************************************************************************/

--- STEP: map.tblEntityQuerySettingToSite
---select * from map.tblEntityQuerySettingToSite
select * 
into EntityQuerySettingToSiteAviationAssignedToSiteGuid
from map.tblEntityQuerySettingToSite
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or MaptoSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or AssignedFromSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

select * from EntityQuerySettingToSiteAviationAssignedToSiteGuid


update map.tblEntityQuerySettingToSite
set AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
from  map.tblEntityQuerySettingToSite m
inner join EntityQuerySettingToSiteAviationAssignedToSiteGuid e
on e.querysettingtositeguid=m.querysettingtositeguid
where e.AssignedFromSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

update map.tblEntityQuerySettingToSite
set SiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid in (select siteguid from map.tblEntityQuerySettingToSite
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or MaptoSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or AssignedFromSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')


-- VALIDATION
select m.siteguid, m.Maptositeguid, m.assignedfromsiteguid, e.siteguid, e.Maptositeguid, e.assignedfromsiteguid
from EntityQuerySettingToSiteAviationAssignedToSiteGuid e
inner join  map.tblEntityQuerySettingToSite m
on e.querysettingtositeguid=m.querysettingtositeguid

select *  from map.tblEntityQuerySettingToSite
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or MaptoSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or AssignedFromSiteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


--------------------------------------------------------------------------------------------------


---STEP:  map.tblEntityLedgerViewToSite 
--- tblEntityLedgerViewToSite  -- self assigned from Aviation to Aviation - updated to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'
select a.LedgerViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where  a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  


UPDATE 
map.tblEntityLedgerViewToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE LedgerViewToSiteGuid IN 
(select  a.LedgerViewToSiteGuid 
from 
map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
)


--- Child assignments from Aviation >> update AssignedFromSiteGuid to Varec
select a.LedgerViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  


UPDATE 
map.tblEntityLedgerViewToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE LedgerViewToSiteGuid IN 
(select LedgerViewToSiteGuid
from map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  )



---VALIDATION 
select a.LedgerViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
and a.siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'


select a.LedgerViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from 
map.tblEntityLedgerViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
and a.siteguid <>'6733898E-CFE0-4067-9404-7CEC984BC955'


select * from  map.tblEntityLedgerViewToSite
where AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'   
or siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 

-----------------------------------------------------------------------------------------------------------------------------
---STEP: map.tblEntityListViewToSite
-- select * from map.tblEntityListViewToSite
--- tblEntityLedgerViewToSite  -- self assigned from Aviation to Aviation - updated to Varec 

select a.ListViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from 
map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  


UPDATE 
map.tblEntityListViewToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE ListViewToSiteGuid IN 
(select a.ListViewToSiteGuid
from map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' )


--- tblEntityLedgerViewToSite child assignedfromsiteguid Aviation  - updated to Varec 
select a.ListViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  

UPDATE 
map.tblEntityListViewToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE ListViewToSiteGuid IN 
(select a.ListViewToSiteGuid
from map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')  

---VALIDATION
select a.ListViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' or
a.SITEGUID <>'6733898E-CFE0-4067-9404-7CEC984BC955'


select a.ListViewToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityListViewToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' or
a.SITEGUID ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

------------------------------------------------------------------------------------------------------
---STEP: map.tblEntityReportConfigurationSettingsToSite

--select * from map.tblEntityReportConfigurationSettingsToSite
--- Aviation self assigned -- map to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'

select a.ReportConfigurationSettingsToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityReportConfigurationSettingsToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SITEGUID ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



UPDATE 
map.tblEntityReportConfigurationSettingsToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE ReportConfigurationSettingsToSiteGuid IN 
(select a.ReportConfigurationSettingsToSiteGuid
from map.tblEntityReportConfigurationSettingsToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' )

UPDATE 
map.tblEntityReportConfigurationSettingsToSite
SET siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE ReportConfigurationSettingsToSiteGuid IN 
(select a.ReportConfigurationSettingsToSiteGuid
from map.tblEntityReportConfigurationSettingsToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' )


--- Validation
select a.ReportConfigurationSettingsToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityReportConfigurationSettingsToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' OR
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

---------------------------------------------------------------------------------------------- 

----------------------------------------------------------------------------------------------------------------

---- STEP: map.tblEntityTransactionAliasToSite

--- select * from map.tblEntityTransactionAliasToSite
--- EntityTransactionAliasToSite Aviation self assigned -- map to Varec 
select a.TransactionAliasToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SITEGUID ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



UPDATE 
map.tblEntityTransactionAliasToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE TransactionAliasToSiteGuid IN 
(select a.TransactionAliasToSiteGuid
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')   

--- EntityTransactionAliasToSite Aviation to Child site AssignedFromSiteGuid -- map to Varec 
select a.TransactionAliasToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


UPDATE 
map.tblEntityTransactionAliasToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE TransactionAliasToSiteGuid IN 
(select a.TransactionAliasToSiteGuid
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' )


--- Validation 

select a.TransactionAliasToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' OR
a.SITEGUID = '6733898E-CFE0-4067-9404-7CEC984BC955'


select a.TransactionAliasToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityTransactionAliasToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' OR
a.SITEGUID= '4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
--------------------------------------------------------------------------------------------------------------------

-------STEP: map.tblEntityUserDataToSite
--- select * from map.tblEntityUserDataToSite

--- EntityUserDataToSite Aviation self assigned -- map to Varec 
select a.UserDataToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.ownersiteguid 
from map.tblEntityUserDataToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.OwnerSiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.OwnerSiteGuid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



UPDATE 
map.tblEntityUserDataToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
Ownersiteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserDataToSiteGuid IN 
(select a.UserDataToSiteGuid
from 
map.tblEntityUserDataToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.OwnerSiteGuid 
where
a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.OwnerSiteGuid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')



--- Aviation to Child site AssignedFromSiteGuid -- map to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'
select a.UserDataToSiteGuid
from 
map.tblEntityUserDataToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.OwnerSiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


UPDATE 
map.tblEntityUserDataToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserDataToSiteGuid IN 
(select a.UserDataToSiteGuid
from 
map.tblEntityUserDataToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.OwnerSiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')



-- Validation
--select * from map.tblEntityUserDataToSite
select a.UserDataToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.ownersiteguid 
from map.tblEntityUserDataToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.OwnerSiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' or
a.OwnerSiteGuid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



---------------------------------------------------------------------------------------------------

--------------map.tblEntityUserGroupToSite

--- select * from map.tblEntityUserGroupToSite
--- EntityUserGroupToSite Aviation self assigned -- map to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'
select a.UserGroupToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.SiteGuid
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



UPDATE 
map.tblEntityUserGroupToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
SiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserGroupToSiteGuid IN 
(select a.UserGroupToSiteGuid
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')



--- EntityUserGroupToSiteAviation to Child site AssignedFromSiteGuid -- map to Varec 
select a.UserGroupToSiteGuid
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


UPDATE 
map.tblEntityUserGroupToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserGroupToSiteGuid IN 
(select a.UserGroupToSiteGuid
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')

-- Delete duplicate mapping 
DELETE FROM
map.tblEntityUserGroupToSite
WHERE UserGroupToSiteGuid IN 
(select a.UserGroupToSiteGuid
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' AND a.SITEGUID='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')

--- Validation 

select a.UserGroupToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' OR
a.SITEGUID <> '6733898E-CFE0-4067-9404-7CEC984BC955'


select a.UserGroupToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityUserGroupToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' OR
a.SITEGUID= '4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
--------------------------------------------------------------------------------------------------------------------

--------STEP: map.tblEntityUserToSite

--- select * from map.tblEntityUserToSite
--- EntityUserToSite Aviation self assigned -- map to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'

select a.UserToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.SiteGuid
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'



UPDATE 
map.tblEntityUserToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955',
SiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserToSiteGuid IN 
(select a.UserToSiteGuid
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' AND
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')



-- DELETE duplicate Varec Aviation mappings
DELETE FROM
map.tblEntityUserToSite
WHERE UserToSiteGuid IN 
(select a.UserToSiteGuid
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' AND
a.SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')

--- Aviation to Child site AssignedFromSiteGuid -- map to Varec '6733898E-CFE0-4067-9404-7CEC984BC955'
select a.UserToSiteGuid
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


UPDATE 
map.tblEntityUserToSite
SET AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE UserToSiteGuid IN 
(select a.UserToSiteGuid
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')


--- Validation 

select a.UserToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from 
map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955' or
a.SITEGUID = '6733898E-CFE0-4067-9404-7CEC984BC955'


select a.UserToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.assignedfromsiteguid ,  a.siteguid 
from map.tblEntityUserToSite a
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' or
a.SITEGUID= '4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
--------------------------------------------------------------------------------------------------------------------




