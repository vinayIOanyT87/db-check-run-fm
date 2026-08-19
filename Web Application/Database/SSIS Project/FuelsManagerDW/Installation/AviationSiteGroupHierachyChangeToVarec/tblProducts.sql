
/********************* PRODUCTS  *******************************************
Note: 
- There are products created at Aviation with no Transactions with no EntityProductToSite assignments
- There are map.tblEntityProductToSite assignments from Varec to Aviation
-- There are map.tblEntityProductToSite assignments from Aviation to Child sites.

Fix - Map products assigned from Aviation and from Aviation>> childsites to Varec

– Tables: 
  i) map.tblEntityProductToSite
 ii) tblProducts

*******************************************************************************/

--STEP 1: map.tblEntityProductToSite assignments from Varec to Aviation (Delete mapping)
--select siteguid, * from tblsites
--select * from map.tblEntityProductToSite 
-- drop table ProductsAssignedFromAviationToChildSiteGroups

select a.ProductToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.productguid, a.assignedfromsiteguid ParentSiteGuid,  a.siteguid ChildSiteguid
into EntityProductToSiteVarecToAviation
from map.tblEntityProductToSite a
inner join tblproducts p on a.productguid=p.productguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where
a.AssignedFromSiteGuid='6733898E-CFE0-4067-9404-7CEC984BC955'
and a.siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 
order by 2 


--SELECT * FROM ProductsAssignedFromVarecToAviation
-- Delete Products Assigned from Varec To Aviation
DELETE FROM 
 map.tblEntityProductToSite 
 Where ProductToSiteGuid in 
(SELECT ProductToSiteGuid  FROM EntityProductToSiteVarecToAviation)
 

---VALIDATION
SELECT * FROM 
 map.tblEntityProductToSite 
WHERE siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'

/****************************************************************************************/

-- STEP 2: map.tblEntityProductToSite assignments from Aviation>>ChildSites to Varec>> ChildSites
-- drop table EntityProductToSiteAviationToChildSiteGroups

select a.ProductToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.productguid, a.assignedfromsiteguid ParentSiteGuid,  a.siteguid ChildSiteguid
into EntityProductToSiteAviationToChildSiteGroups
from map.tblEntityProductToSite a
inner join tblproducts p on a.productguid=p.productguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  
order by 2 


---select * from map.tblEntityProductToSite 
---select * from EntityProductToSiteAviationToChildSiteGroups


update  map.tblEntityProductToSite 
set assignedfromsiteguid='6733898E-CFE0-4067-9404-7CEC984BC955'
from  map.tblEntityProductToSite  e
inner join EntityProductToSiteAviationToChildSiteGroups p
on e.ProductToSiteGuid=p.ProductToSiteGuid



--VALIDATION

select e.assignedfromsiteguid, p.Parentsiteguid, e.siteguid, p.childsiteguid,*  from 
map.tblEntityProductToSite  e
inner join EntityProductToSiteAviationToChildSiteGroups p
on e.ProductToSiteGuid=p.ProductToSiteGuid


select a.ProductToSiteGuid, c.id ParentSiteID, d.id ChildSiteID, a.productguid, a.assignedfromsiteguid ParentSiteGuid,  a.siteguid ChildSiteguid
from map.tblEntityProductToSite a
inner join tblproducts p on a.productguid=p.productguid
Inner Join tblSites c on c.SiteGuid = a.AssignedFromSiteGuid
Inner Join tblSites d on d.SiteGuid = a.SiteGuid 
where a.AssignedFromSiteGuid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'  or
a.siteguid='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A' 
order by 2 


---------------------------------------------------------------------------------------------------

/**** dbo.tblproducts  *****/


-- STEP 1: products created at Aviation with no transactions and no map.tblEntityProductToSite site assignments ( Delete Products)
select s.id,s.siteguid, t.productguid,t.* from  tblproducts t
inner join tblsites s
on 
t.siteguid=s.siteguid
where  1=1
and s.id like 'Aviation' 
order by s.id



-- no transactions tied to this products
select * from tbltransactionlineitems where productguid in
(select t.productguid from  tblproducts t
inner join tblsites s
on 
t.siteguid=s.siteguid
where  1=1
and s.id like 'Aviation%' 
)

-- products not mapped to any child sites
select * from map.tblEntityProductToSite where
productguid in 
( select t.productguid from  tblproducts t
inner join tblsites s
on 
t.siteguid=s.siteguid
where  1=1
and s.id like 'Aviation' 
)


DELETE FROM 
tblproducts where  
productguid in 
( select t.productguid from  tblproducts t
inner join tblsites s
on 
t.siteguid=s.siteguid
where  1=1
and s.id like 'Aviation' 
)



-- VALIDATION 
SELECT * FROM 
tblproducts where  productguid in 
( select t.productguid from  tblproducts t
inner join tblsites s
on 
t.siteguid=s.siteguid
where  1=1
and s.id like 'Aviation' 
)


select * from tblproducts where siteguid ='4EEBC7AC-A5B2-405C-B743-B74338F3BC1A'
----------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------
