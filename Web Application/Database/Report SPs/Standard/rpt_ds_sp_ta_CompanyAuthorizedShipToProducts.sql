USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToProducts]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToProducts]
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_CompanyAuthorizedShipToProducts] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Dataset for ShipTo Products LookUp
 Version:				7.5.1.0
 Modification History:
	Date		by		Description
	5/8/2009	UP		
	6/12/2009	KF		Add with(nolock) to tables
	7/16/2009	UP		Rename from rpt_ds_sp_ta_CompanyAuthorizedProducts to rpt_ds_sp_ta_CompanyAuthorizedShipToProducts
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
SELECT distinct p.productid, LTRIM(RTRIM(Convert(nvarchar(7),p.productid))) AS Label
FROM dbo.tblCompanies C with(nolock)
LEFT OUTER JOIN dbo.tblProductMap PM with(nolock)
ON c.CompanyIndex = PM.AssignedToIndex
LEFT OUTER JOIN dbo.tblProducts P
ON pm.AssignedIndex = P.ProductIndex
WHERE pm.TYPE = 6
and (C.ID = isnull(@ShipTo,C.ID))

ORDER BY ProductID

/*

EXEC [rpt_ds_sp_ta_CompanyAuthorizedShipToProducts] 1,1,2,'<ALL>'

*/

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToProducts] TO [public]
GO



