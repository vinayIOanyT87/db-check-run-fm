use consolidateddb

delete from tblentitytositemap where typeid = 'Products' and [index] not in (SELECT ProductIndex FROM tblProducts)

