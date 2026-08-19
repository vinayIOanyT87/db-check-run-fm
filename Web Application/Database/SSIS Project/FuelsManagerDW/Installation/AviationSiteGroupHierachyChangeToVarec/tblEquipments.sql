
/*************************************************************************************************
ENTITY: EQUIPMENTS
Notes:
• Equipment: There are no equipment assigned directly from Aviation.

select siteguid, * from tblequipment
where SiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

• EquipmentTypeToSite: There are EntityEquipmentTypeToSite assignments to Aviation.

Fix: Map Equipment EntityEquipmentTypeToSite assignments to Aviation to Varec
Tables: 
- map.tblEntityEquipmentTypeToSite
- dbo.tblEquipmentTypes)
************************************************************************************************************/


-- STEP 1: Map tblEquipmentTypes Siteguid from Aviation To SiteAdmin

select siteguid,* from tblsites where id='SiteAdmin'
-- Equipments Assigned to Aviation from Aviation(self-assignment)

select * from [dbo].[tblEquipmentTypes] 
--drop table tblEquipmentTypes_remap

-- select * from tblEquipmentTypes_remap
select equipmentTypeguid, siteguid
into tblEquipmentTypes_remap
from [dbo].[tblEquipmentTypes] 



-- STEP: update tblEquipmentTypes Siteguid from Aviation To SiteAdmin
Update [dbo].[tblEquipmentTypes] 
set Siteguid='00000000-0000-0000-0000-000000000001'
where siteguid in (
select siteguid from tblEquipmentTypes_remap
where siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A')


---- VALIDATION
select equipmentTypeguid, siteguid from [dbo].[tblEquipmentTypes] 




--------------------------------------------------------------------------
-- STEP: Map EntityEquipmentTypeToSite Records from Aviation (self assignment) to SiteAdminToSiteAdmin


--select * from tblEntityEquipmentTypeToSite_AviationSelfAssigned
select * 
into tblEntityEquipmentTypeToSite_AviationSelfAssigned
from map.tblEntityEquipmentTypeToSite
where 
siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' and
assignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


-- 1) Create a new record for each equipmenttypeguid with AssignedFromSiteGuid='SiteAdmin' and Siteguid ='SiteAdmin'

INSERT INTO [map].[tblEntityEquipmentTypeToSite]
           (
           [EquipmentTypeGuid]
           ,[SiteGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
           ,[AssignedFromSiteGuid])
  SELECT 
      [EquipmentTypeGuid]
      ,'00000000-0000-0000-0000-000000000001'
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
        ,'00000000-0000-0000-0000-000000000001'
    FROM [dbo].[tblEntityEquipmentTypeToSite_AviationSelfAssigned]


	-- VALIDATION
		select * from map.tblEntityEquipmentTypeToSite where siteguid='00000000-0000-0000-0000-000000000001'


-- 2) Create a new EntityEquipmentTypeToSite record for each equipmenttypeguid with AssignedFromSiteGuid='SiteAdmin' and Siteguid ='Varec'
-- select siteguid, * from tblsites where id='Varec'

select * from map.tblEntityEquipmentTypeToSite
where 
siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' and
assignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


--- UPDATE SELF ASSIGNED RECORDS >> SITEGUID=VAREC AND ASSIGNEDTOSITEGUID=SITEADMIN

UPDATE [map].[tblEntityEquipmentTypeToSite]
         SET [SiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955',
        [AssignedFromSiteGuid] ='00000000-0000-0000-0000-000000000001'
	 FROM [map].[tblEntityEquipmentTypeToSite] m
	 inner join [dbo].[tblEntityEquipmentTypeToSite_AviationSelfAssigned] e
on m.EquipmentTypeToSiteGuid =e.EquipmentTypeToSiteGuid
and m.siteguid=e.siteguid
and m.AssignedFromSiteGuid=e.AssignedFromSiteGuid
where 
m.siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' and
m.assignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


--- VALIDATION
	select * from map.tblEntityEquipmentTypeToSite where  [AssignedFromSiteGuid] ='00000000-0000-0000-0000-000000000001'
	AND [SiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955' AND EquipmentTypeToSiteGuid in 
	(select EquipmentTypeToSiteGuid from [dbo].[tblEntityEquipmentTypeToSite_AviationSelfAssigned])

---------------------------------------------------------------------------------------------------------------------
---STEP: UPDATE CHILD RECORDS WHERE ASSIGNEDFROMSITEGUID = AVIATION TO VAREC
--SELECT * FROM tblEntityEquipmentTypeToSite_AviationSelfAssigned
	
SELECT EQUIPMENTTYPEGUID ,* from map.tblEntityEquipmentTypeToSite where  [AssignedFromSiteGuid] ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
	AND EQUIPMENTTYPEGUID IN (	SELECT EQUIPMENTTYPEGUID --FROM tblEntityEquipmentTypeToSite_AviationSelfAssigned)
	from map.tblEntityEquipmentTypeToSite where  [AssignedFromSiteGuid] ='00000000-0000-0000-0000-000000000001'
	AND [SiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955' AND EquipmentTypeToSiteGuid in 
	(select EquipmentTypeToSiteGuid from [dbo].[tblEntityEquipmentTypeToSite_AviationSelfAssigned]))


	UPDATE map.tblEntityEquipmentTypeToSite 
	SET   [AssignedFromSiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955'
	 from map.tblEntityEquipmentTypeToSite where  [AssignedFromSiteGuid] ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
	AND EQUIPMENTTYPEGUID IN (	SELECT EQUIPMENTTYPEGUID --FROM tblEntityEquipmentTypeToSite_AviationSelfAssigned)
	from map.tblEntityEquipmentTypeToSite where  [AssignedFromSiteGuid] ='00000000-0000-0000-0000-000000000001'
	AND [SiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955' AND EquipmentTypeToSiteGuid in 
	(select EquipmentTypeToSiteGuid from [dbo].[tblEntityEquipmentTypeToSite_AviationSelfAssigned]))
      
--- VALIDATION 
SELECT * FROM 
map.tblEntityEquipmentTypeToSite 
WHERE  [AssignedFromSiteGuid] = '6733898E-CFE0-4067-9404-7CEC984BC955'

---------------------------------------------------------------------------------------------------------------------------


-- STEP:  EntityEquipmentTypeToSite RECORDS THAT ARE NOT ASSIGNED FROM VAREC


SELECT * FROM 
map.tblEntityEquipmentTypeToSite 
WHERE  [AssignedFromSiteGuid] NOT IN( '6733898E-CFE0-4067-9404-7CEC984BC955','00000000-0000-0000-0000-000000000001')



----ASIG Canada -  Update ASIG Canada siteguid to '1B393BC9-0AC5-470E-B810-486C0EE26F31' to siteadmin

SELECT * FROM tblEquipmentTypes_remap WHERE  Siteguid='1B393BC9-0AC5-470E-B810-486C0EE26F31'

SELECT *
INTO ASIGCanada  
FROM [dbo].[tblEquipmentTypes] WHERE  Siteguid='1B393BC9-0AC5-470E-B810-486C0EE26F31'


Update [dbo].[tblEquipmentTypes] 
set Siteguid='00000000-0000-0000-0000-000000000001'
where siteguid in (
select siteguid from ASIGCanada 
)


--- VALIDATION
SELECT * FROM tblEquipmentTypes
----------------------------------------------------------------------------------------------------------------------------------------------

-- STEP: NEW RECORDS FOR SELF ASSIGNED ASIGCANADA RECORDS Siteguid='1B393BC9-0AC5-470E-B810-486C0EE26F31'  TO SELFASSIGNED  SITEADMIN

select * 
into tblEntityEquipmentTypeToSite_ASIGCanada
from map.tblEntityEquipmentTypeToSite
where 
siteguid='1B393BC9-0AC5-470E-B810-486C0EE26F31' and
assignedFromSiteGuid='1B393BC9-0AC5-470E-B810-486C0EE26F31'


--SELECT * FROM tblEntityEquipmentTypeToSite_ASIGCanada

INSERT INTO [map].[tblEntityEquipmentTypeToSite]
           (
           [EquipmentTypeGuid]
           ,[SiteGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
           ,[AssignedFromSiteGuid])
  SELECT 
      [EquipmentTypeGuid]
      ,'00000000-0000-0000-0000-000000000001'
      ,[CreatedDate]
      ,[CreatedBy]
      ,[UpdatedDate]
      ,[UpdatedBy]
        ,'00000000-0000-0000-0000-000000000001'
    FROM tblEntityEquipmentTypeToSite_ASIGCanada

	-- VALIDATION
	SELECT * FROM [map].[tblEntityEquipmentTypeToSite]
	WHERE  [EquipmentTypeGuid] IN
	(  SELECT 
     [EquipmentTypeGuid]
       FROM tblEntityEquipmentTypeToSite_ASIGCanada)
	  AND ASSIGNEDFROMSITEGUID='00000000-0000-0000-0000-000000000001'



-- UPDATE ASIGCANADA SELF ASSIGNED EntityEquipmentTypeToSite RECORDS ASSIGNMENT  FROM TO Varec

--select * FROM  tblEntityEquipmentTypeToSite_ASIGCanada

UPDATE map.tblEntityEquipmentTypeToSite
SET assignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
WHERE EquipmentTypeToSiteGuid in 
(select EquipmentTypeToSiteGuid FROM  tblEntityEquipmentTypeToSite_ASIGCanada)



-- VALIDATION 

SELECT * FROM map.tblEntityEquipmentTypeToSite
WHERE EquipmentTypeToSiteGuid in 
(select EquipmentTypeToSiteGuid FROM  tblEntityEquipmentTypeToSite_ASIGCanada)


SELECT * FROM tblEquipmentTypes
where equipmentTypeGuid not in (
SELECT equipmentTypeGuid FROM map.tblEntityEquipmentTypeToSite
WHERE ASSIGNEDFROMSITEGUID NOT IN ('6733898E-CFE0-4067-9404-7CEC984BC955','00000000-0000-0000-0000-000000000001'))


-- should have no records
SELECT * FROM map.tblEntityEquipmentTypeToSite
WHERE ASSIGNEDFROMSITEGUID NOT IN ('6733898E-CFE0-4067-9404-7CEC984BC955','00000000-0000-0000-0000-000000000001')
and equipmentTypeGuid not in (SELECT equipmentTypeGuid FROM tblEquipmentTypes)


SELECT siteguid,ASSIGNEDFROMSITEGUID,* FROM map.tblEntityEquipmentTypeToSite
where ASSIGNEDFROMSITEGUID not in (SELECT ASSIGNEDFROMSITEGUID FROM map.tblEntityEquipmentTypeToSite
WHERE ASSIGNEDFROMSITEGUID IN ('6733898E-CFE0-4067-9404-7CEC984BC955','00000000-0000-0000-0000-000000000001'))



select ASSIGNEDFROMSITEGUID, SITEGUID, *  FROM map.tblEntityEquipmentTypeToSite  WHERE SITEGUID in
('6055425D-B05B-4EA7-A235-2B7B0BA7B711') or  ASSIGNEDFROMSITEGUID in ('6055425D-B05B-4EA7-A235-2B7B0BA7B711')

select * from map.tblEntityEquipmentTypeToSite where
ASSIGNEDFROMSITEGUID ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
or SITEGUID ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'


--**********************************************************************************************************************



