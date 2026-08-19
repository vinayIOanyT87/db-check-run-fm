USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_ds_sp_ta_CompanyAuthorizedProducts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_ds_sp_ta_CompanyAuthorizedProducts]
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_CompanyAuthorizedProducts] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Dataset for Products LookUp
 Version:				7.5.1.0
 Execution:
			EXEC [rpt_ds_sp_ta_CompanyAuthorizedProducts] 1,1,2,'<ALL>'
 Modification History:
	Date		by		Description
	5/8/2009	UP		
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@ShipTo nvarchar(30)
)
 AS
IF @ShipTo = '<ALL>' SET @ShipTo = null

SELECT '<ALL>' AS 'ProductID', '<ALL>' AS Label
UNION
SELECT distinct p.productid, p.productid AS Label
FROM dbo.tblCompanies C 
LEFT OUTER JOIN dbo.tblProductMap PM
ON c.CompanyIndex = PM.AssignedToIndex
LEFT OUTER JOIN dbo.tblProducts P
ON pm.AssignedIndex = P.ProductIndex
WHERE pm.TYPE = 6
and (C.ID = isnull(@ShipTo,C.ID))

ORDER BY ProductID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_CompanyAuthorizedProducts] TO [public]
GO



