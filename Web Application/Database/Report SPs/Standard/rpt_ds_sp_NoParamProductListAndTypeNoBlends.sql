USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_ds_sp_NoParamProductListAndTypeNoBlends') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_ds_sp_NoParamProductListAndTypeNoBlends
GO


CREATE PROCEDURE [dbo].rpt_ds_sp_NoParamProductListAndTypeNoBlends

 /*=============================================
 Author:		Urvi Patel
 Create date:	5/22/2009
 Description:	SP for Product List and Type - No Blends
 Version:				7.5.1.0
 Execution:
			EXEC rpt_ds_sp_NoParamProductListAndTypeNoBlends 1,1,1,1	
 Modification History:
	Date		by		Description
	

 =============================================*/
(
	@LoginSiteIndex int,
	@SiteIndex int,
	@ShowAll int,
	@Header int
)
AS



/* The following query returns Products that are either in the Site or the Login Site 
based on the site to entity mapping */

IF @Header = 0
BEGIN

SELECT -999 AS ProductIndex, '<All>' AS ProductID, '<All>' AS Description, '' AS ProductType
WHERE @ShowAll = 1 UNION

SELECT ProductIndex, ProductID, Description, ProductType
FROM tblProducts, (SELECT tblEntityToSiteMap.*,(SELECT SubTable.SiteIndex 
		   				FROM tblEntityToSiteMap SubTable 
				      		WHERE SubTable.TypeID = 'Products' AND 
						      SubTable.[Index] = tblEntityToSiteMap.[Index] AND 
						      SubTable.SiteIndex = @LoginSiteIndex) AS LoginSiteIndex
	 	   FROM tblEntityToSiteMap WHERE TypeID = 'Products' AND SiteIndex = @SiteIndex ) tblEntities 
WHERE 
     tblEntities.[Index] = tblProducts.ProductIndex 
AND (tblProducts.SiteIndex = @SiteIndex OR tblEntities.LoginSiteIndex = @LoginSiteIndex )
--AND ProductID = @Product
AND ProductType in(0,2)
AND (InhibitAccounting <>1)
ORDER BY ProductID

END

IF @Header = 1
BEGIN

SELECT -999 AS ProductIndex, '<All>' AS ProductID, '<All>' AS Description, '' AS ProductType
WHERE @ShowAll = 1 UNION

SELECT ProductIndex, ProductID, Description, ProductType
FROM tblProducts, (SELECT tblEntityToSiteMap.*,(SELECT SubTable.SiteIndex 
		   				FROM tblEntityToSiteMap SubTable 
				      		WHERE SubTable.TypeID = 'Products' AND 
						      SubTable.[Index] = tblEntityToSiteMap.[Index] AND 
						      SubTable.SiteIndex = @LoginSiteIndex) AS LoginSiteIndex
	 	   FROM tblEntityToSiteMap WHERE TypeID = 'Products' AND SiteIndex = @SiteIndex ) tblEntities 
WHERE 
     tblEntities.[Index] = tblProducts.ProductIndex 
AND (tblProducts.SiteIndex = @SiteIndex OR tblEntities.LoginSiteIndex = @LoginSiteIndex )
--AND ProductID = @Product
AND ProductType in(0,2)

ORDER BY ProductID
END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_ds_sp_NoParamProductListAndTypeNoBlends TO [public]
GO


