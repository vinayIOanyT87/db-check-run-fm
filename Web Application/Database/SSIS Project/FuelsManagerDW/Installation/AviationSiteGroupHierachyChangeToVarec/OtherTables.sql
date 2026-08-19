
/******************************************************************************************************************
Tables with related Aviaition Siteguid Data

Fix: Map Aviation Records to Varec

Tables:
dbo.tblTransactionAliases
dbo.tblUserDataFieldTransactionAlias
dbo.tblUserDataFieldTransactionAliasLineItem
dbo.tblReportDetails
dbo.tblReportGroups
dbo.tblScheduleTerminalOperation
dbo.tblGeneralConfiguration
dbo.tblGroups
dbo.tblListViews
dbo.tblProcessVariableSite
dbo.tblUsers
map.tblUserToGroup


*****************************************************************************************************/
--  tblUserDataFieldTransactionAliasLineItem 
SELECT SITEGUID, * FROM dbo.tblUserDataFieldTransactionAliasLineItem where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


Update 
dbo.tblUserDataFieldTransactionAliasLineItem
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

--------------------------------------------------------------------------------------
--dbo.tblUserDataFieldTransactionAlias

SELECT SITEGUID, * FROM dbo.tblUserDataFieldTransactionAlias where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblUserDataFieldTransactionAlias
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

-------------------------------------------------------------------------------
--dbo.tblTransactionAliases

SELECT SITEGUID, * FROM dbo.tblTransactionAliases where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblTransactionAliases
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

-----------------------------------------------------------------------------
---dbo.tblReportDetails

SELECT SITEGUID, * FROM dbo.tblReportDetails
where siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblReportDetails
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
----------------------------------------------------------------------------

---dbo.tblReportGroups
SELECT SITEGUID, * FROM dbo.tblReportGroups
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblReportGroups
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
---------------------------------------------------------------------

--dbo.tblScheduleTerminalOperation
SELECT SITEGUID, * FROM dbo.tblScheduleTerminalOperation
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblScheduleTerminalOperation
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

-----------------------------------------------------------------------

--dbo.tblGeneralConfiguration
SELECT SITEGUID, * FROM dbo.tblGeneralConfiguration where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblGeneralConfiguration
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
----------------------------------------------------------------------
---dbo.tblGroups
SELECT SITEGUID, * FROM dbo.tblGroups where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblGroups
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
---------------------------------------------------------------

---dbo.tblListViews
SELECT SITEGUID, * FROM dbo.tblListViews where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblListViews
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
-----------------------------------------------------------------

--dbo.tblProcessVariableSite
SELECT SITEGUID, * FROM dbo.tblProcessVariableSite where siteguid
='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update 
dbo.tblProcessVariableSite
SET SITEGUID='6733898E-CFE0-4067-9404-7CEC984BC955'
where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
------------------------------------------------------------------


---************************************************************************************************
---************************************************************************************************
/**
dbo.tblUsers
map.tblUserToGroup

***************************************************************************************************/
---dbo.tblUsers
SELECT SITEGUID, * FROM dbo.tblUsers where siteguid=
'4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

Update dbo.tblUsers
set siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'
from dbo.tblUsers m
where userguid in 
(select userguid from dbo.tblUsers
where 
 siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')

--- validation
SELECT SITEGUID, * FROM dbo.tblUsers where siteguid=
'6733898E-CFE0-4067-9404-7CEC984BC955'


-----------------------------------------------------------------------

---map.tblUserToGroup 

select * into tblUserToGroup_Aviation
from map.tblUserToGroup where 
 siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


select * into tblUserToGroup_Varec
from map.tblUserToGroup where 
 siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955'

 select * from tblUserToGroup_Aviation
 select * from tblUserToGroup_Varec

 --duplicate usertogroup mappings for Aviation and Varec with same userguid and groupguid
 select a.siteguid, v.siteguid,* from tblUserToGroup_Aviation a
 inner join tblUserToGroup_Varec v
 on v.USERGUID =a.userguid
AND v.GROUPGUID=a.groupguid


--- DELETE AVIATION DUPLICATE USERGUID, GROUPGUID MAPPING

 DELETE FROM map.tblUserToGroup
 WHERE usertogroupguid in
 ( select a.usertogroupguid from tblUserToGroup_Aviation a
 inner join tblUserToGroup_Varec v
 on v.USERGUID =a.userguid
AND v.GROUPGUID=a.groupguid)

 DELETE FROM tblUserToGroup_Aviation
 WHERE usertogroupguid in
 ( select a.usertogroupguid from tblUserToGroup_Aviation a
 inner join tblUserToGroup_Varec v
 on v.USERGUID =a.userguid
AND v.GROUPGUID=a.groupguid)

-- UPDATE THE AVIATION MAPPINGS TO VAREC
update tblUserToGroup_Aviation set siteguid ='6733898E-CFE0-4067-9404-7CEC984BC955' 

 update map.tblUserToGroup
set siteguid =r.siteguid
from map.tblUserToGroup m
inner join tblUserToGroup_Aviation r
on m.usertogroupguid=r.usertogroupguid


-- VALIDATIONS 
select * from map.tblUserToGroup
WHERE SITEGUID='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
--------------------------------------------------------------------------------------------