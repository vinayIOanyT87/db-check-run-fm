USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriers]
GO


CREATE PROCEDURE  [dbo].[rpt_sp_ta_ShiptoCoAuthProductsAndCarriers] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Main SP for Company Authorized Products and Carriers Report
 Version:				7.5.1.0
 Execution:			
		EXEC [rpt_sp_ta_ShiptoCoAuthProductsAndCarriers] 1,1,2,'<ALL>','<ALL>','<ALL>'		

 Modification History:
	Date		by		Description
	5/8/2009	UP		New
	9/3/2009	KF		Rename file from rpt_sp_ta_CompanyAuthorizedProductsandCarriers 
						to rpt_sp_ta_ShiptoCoAuthProductsAndCarriers
	12/10/2009	KF		Version 7.5.1.0
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@ShipTo nvarchar(30),
	@Product nvarchar(30),
	@Carrier nvarchar(30)
)
 AS
IF @ShipTo = '<ALL>' SET @ShipTo = null
IF @Product = '<ALL>' SET @Product = null
IF @Carrier = '<ALL>' SET @Carrier = null

IF @Carrier is not null       
SELECT c.[ID] 
INTO #tmpCarriers
FROM  dbo.tblCompanies c with (nolock)
LEFT OUTER JOIN dbo.tblCompanyMap cm  with (nolock)
ON c.CompanyIndex = cm.AssignedToIndex
LEFT OUTER JOIN dbo.tblCompanies SCAC  with (nolock)
ON cm.assignedindex = SCAC.companyindex
WHERE Type = 4
AND (c.[ID] = isnull(@ShipTo,c.[ID]))
AND (AssignedIndex = isnull(@Carrier,AssignedIndex))

IF @Product is not null
SELECT c.ID AS [ID]
INTO #tmpProducts
FROM dbo.tblCompanies C with (nolock)
LEFT OUTER JOIN dbo.tblProductMap PM
ON c.CompanyIndex = PM.AssignedToIndex
LEFT OUTER JOIN dbo.tblProducts P with (nolock)
ON pm.AssignedIndex = P.ProductIndex
LEFT OUTER JOIN dbo.tblApplicationString A with (nolock)
ON A.[Index] = PM.AdditiveProfileIndex--AssignedToIndex
WHERE pm.TYPE = 6
AND (c.ID = isnull(@ShipTo,c.ID))
AND (ProductID= isnull(@Product,ProductID) )




SELECT distinct c.ID 'ShipToID' , C.Name  AS 'Label'
INTO #tmpShipTo
FROM dbo.tblCompanies C with (nolock)
WHERE (C.ID = isnull(@ShipTo,C.ID)) 
AND CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE Role=4)

IF @Carrier is not null and @Product is null     -- ShipTo has Carriers associated but no product 

	SELECT distinct ShipToID  AS 'ShipToID' , Label  AS 'Label'
	FROM #tmpShipTo
	WHERE  ShipToID IN (SELECT [ID] FROM #tmpCarriers)
				
		
ELSE IF @Carrier is  null and @Product is not null    -- ShipTo has Products associated but no Carriers

		SELECT distinct ShipToID  AS 'ShipToID' , Label  AS 'Label'
		FROM #tmpShipTo
		WHERE  ShipToID IN (SELECT [ID] FROM #tmpProducts)

ELSE IF @Carrier is  not null and @Product is not null  -- ShipTo has Products and Carriers
		SELECT distinct ShipToID  AS 'ShipToID' , Label  AS 'Label'
		FROM #tmpShipTo
		WHERE  ShipToID IN (SELECT [ID] FROM #tmpCarriers) AND 
				ShipToID IN (SELECT [ID] FROM #tmpProducts)
ELSE
		SELECT distinct ShipToID  AS 'ShipToID' , Label  AS 'Label'
		FROM #tmpShipTo


ORDER BY ShipToID

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_ShiptoCoAuthProductsAndCarriers] TO [public]
GO



