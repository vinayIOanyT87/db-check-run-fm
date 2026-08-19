USE ConsolidatedDB
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_ds_sp_ProductListNoBlendsNoInhibitAcc') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_ds_sp_ProductListNoBlendsNoInhibitAcc

GO


CREATE PROCEDURE [dbo].rpt_ds_sp_ProductListNoBlendsNoInhibitAcc
/*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description: 	
 Version:				7.5.1.0
 Execution:
			EXEC rpt_ds_sp_ProductListNoBlendsNoInhibitAcc 1,1,1	
	
 Modification History:
	Date		by		Description
	4/28/2009	UP		Rename from [fm_ProductList] to [rpt_ds_sp_ProductList]
	5/14/2009	KF		No Product Blends  - ProductType <> 1
	6/2/2009	UP		No Inhibit Accounting - Inhibit Accounting <> 1
 =============================================*/


(
	@LoginSiteIndex int,
	@SiteIndex int,
	@ShowAll int
)
AS

/* The following query returns Products that are either in the Site or the Login Site 
based on the site to entity mapping */
SELECT -999 AS ProductIndex, '<All>' AS ProductID, '<All>' AS Description
WHERE @ShowAll = 1 

UNION

SELECT ProductIndex, ProductID, Description 
FROM tblProducts, (SELECT tblEntityToSiteMap.*,(SELECT SubTable.SiteIndex 
		   				FROM tblEntityToSiteMap SubTable 
				      		WHERE SubTable.TypeID = 'Products' AND 
						      SubTable.[Index] = tblEntityToSiteMap.[Index] AND 
						      SubTable.SiteIndex = @LoginSiteIndex) AS LoginSiteIndex
	 	   FROM tblEntityToSiteMap WHERE TypeID = 'Products' AND SiteIndex = @SiteIndex ) tblEntities 
WHERE 
     tblEntities.[Index] = tblProducts.ProductIndex 
AND (tblProducts.SiteIndex = @SiteIndex OR tblEntities.LoginSiteIndex = @LoginSiteIndex)
AND ProductType in(0,2)
AND (InhibitAccounting <>1)

ORDER BY ProductID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.rpt_ds_sp_ProductListNoBlendsNoInhibitAcc TO [public]
GO
