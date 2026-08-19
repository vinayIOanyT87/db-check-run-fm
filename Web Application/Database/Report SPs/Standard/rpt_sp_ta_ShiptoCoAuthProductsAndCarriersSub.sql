USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub]
GO


CREATE PROCEDURE  [dbo].[rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			SubReport SP For Ship To Company Authorized Products and Carriers
 Version:				7.5.1.0
 Execution:			
	EXEC [rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub] 1,1,2,'<ALL>','<ALL>',5481,1 
	

 Modification History:
	Date		by		Description
	5/8/2009	UP	
	9/3/2009	KF		Rename file from rpt_sp_ta_CompanyAuthorizedProductsandCarriersSub 
						to rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub
	12/10/2009	KF		Version 7.5.1.0
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@ShipTo nvarchar(30),
	@Product nvarchar(30),
	@Carrier nvarchar(30),
	@Header int
)
 AS

IF @ShipTo = '<ALL>' SET @ShipTo = null
IF @Product = '<ALL>' SET @Product = null
IF @Carrier = '<ALL>' SET @Carrier = null

SELECT	c.ID AS 'ShipToID'
		,p.productid,A.[ID]
		,Name 
		,ShipToProductID
		,AdditiveProfileIndex
		,PM.ShipToProductCode
		,PM.ShipToLoadRackDisplayText
		,c.CompanyIndex
INTO #tmpProducts
FROM dbo.tblCompanies C
LEFT OUTER JOIN dbo.tblProductMap PM
ON c.CompanyIndex = PM.AssignedToIndex
LEFT OUTER JOIN dbo.tblProducts P
ON pm.AssignedIndex = P.ProductIndex
LEFT OUTER JOIN dbo.tblApplicationString A with (nolock)
ON A.[Index] = PM.AdditiveProfileIndex--AssignedToIndex
WHERE pm.TYPE = 6
AND (c.ID = isnull(@ShipTo,c.ID))
AND (ProductID= isnull(@Product,ProductID))

ORDER BY ShipToID, ProductID


SELECT SCAC.ID AS 'CarrierID'
		,SCAC.name AS 'CarrierName'
		,c.[ID] AS 'ShipTo'
		,cm.AssignedIndex
INTO #tmpCarriers
FROM  dbo.tblCompanies c with (nolock)
LEFT OUTER JOIN dbo.tblCompanyMap cm  with (nolock)
ON c.CompanyIndex = cm.AssignedToIndex
LEFT OUTER JOIN dbo.tblCompanies SCAC  with (nolock)
ON cm.assignedindex = SCAC.companyindex
WHERE Type = 4
AND (c.[ID] = isnull(@ShipTo,c.[ID]))
AND (AssignedIndex = isnull(@Carrier,AssignedIndex))

IF @Header = 1
BEGIN
SELECT * FROM #tmpProducts
END
IF @Header = 2
BEGIN
SELECT * FROM #tmpCarriers
END 

DROP TABLE #tmpProducts
DROP TABLE #tmpCarriers

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriersSub] TO [public]
GO



