USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipTo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipTo]
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_CompanyAuthorizedShipTo] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Dataset for ShipTo LookUp
 Version:				7.5.1.0
 Execution:
		EXEC [rpt_ds_sp_ta_CompanyAuthorizedShipTo] 1,1,2

 Modification History:
	Date		by		Description
	5/8/2009	UP		
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int
)
 AS

SELECT '<ALL>' AS 'ShipTo', '<ALL>' AS Label
UNION
SELECT DISTINCT c.ID AS 'ShipTo' ,  c.Name  AS 'Label'
FROM dbo.tblCompanies C
WHERE CompanyIndex IN (SELECT CompanyIndex FROM tblCompanyRoleMap WHERE Role=4)

ORDER BY ShipTo

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipTo] TO [public]
GO