USE ConsolidatedDB
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_ds_sp_ProductListAdditives') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_ds_sp_ProductListAdditives

GO



CREATE PROCEDURE [dbo].rpt_ds_sp_ProductListAdditives
/*=============================================
 Author:	 			UNKNOWN
 Create date: 			Display Additive products only - Product type = 2
 Description: 	
 Version:				7.5.1.0
 Execution:		
			EXEC rpt_ds_sp_ProductListAdditives 1,1,1
			
 Modification History:
	Date		by		Description
	
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
AND ProductType = 2 
ORDER BY ProductID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.rpt_ds_sp_ProductListAdditives TO [public]
GO




