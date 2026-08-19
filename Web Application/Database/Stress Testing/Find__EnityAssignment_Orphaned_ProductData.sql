use consolidateddb

select count(*) as [Orphaned Product Map] from tblentitytositemap where typeid = 'Products' and [index] not in (SELECT ProductIndex FROM tblProducts)

