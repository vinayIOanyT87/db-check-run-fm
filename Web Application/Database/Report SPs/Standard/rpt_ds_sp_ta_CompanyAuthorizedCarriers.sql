USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_ds_sp_ta_CompanyAuthorizedCarriers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_ds_sp_ta_CompanyAuthorizedCarriers]
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_CompanyAuthorizedCarriers] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Dataset For Carriers LookUp
 Version:				7.5.1.0
 Execution:
		EXEC [rpt_ds_sp_ta_CompanyAuthorizedCarriers] 1,1,2, '<ALL>'
	
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

SELECT '<ALL>' AS 'Carrier', '<ALL>' AS Label
UNION
SELECT Convert(nvarchar(30),AssignedIndex)
		,CarrierID
FROM
(SELECT	DISTINCT AssignedIndex
				,SCAC.ID AS 'CarrierID'
				,C.ID AS 'ShipToID'
FROM  dbo.tblCompanies c with (nolock)
LEFT OUTER JOIN dbo.tblCompanyMap cm  with (nolock)
ON c.CompanyIndex = cm.AssignedToIndex
LEFT OUTER JOIN dbo.tblCompanies SCAC  with (nolock)
ON cm.assignedindex = SCAC.companyindex
WHERE Type = 4)carrier
WHERE(ShipToID = isnull(@ShipTo,ShipToID))
ORDER BY Label


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_CompanyAuthorizedCarriers] TO [public]
GO



