USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers]
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/8/2009
 Description:			Dataset For ShipTo Carriers LookUp
 Version:				7.5.1.0
 Modification History:
	Date		by		Description
	5/8/2009	UP		
	7/16/2009	UP		Rename from rpt_ds_sp_ta_CompanyAuthorizedCarriers to rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@ShipTo nvarchar(30)
)
 AS

IF @ShipTo = '<ALL>' SET @ShipTo = null

SELECT '<ALL>' AS 'Carrier', '<ALL>' AS Name, '<ALL>' AS Label, '(-1)' AS 'NameOrder'
UNION
SELECT DISTINCT Convert(nvarchar(30),AssignedIndex)
				,CarrierID
				,Convert(nvarchar(30),AssignedIndex) + ' - ' + Name AS 'Label'
				, Name  AS 'NameOrder'
				
FROM
(SELECT	DISTINCT AssignedIndex
				,SCAC.ID AS 'CarrierID'
				,C.Name AS 'Name'
				,C.ID AS 'ShipToID'
FROM  dbo.tblCompanies c with (nolock)
LEFT OUTER JOIN dbo.tblCompanyMap cm  with (nolock)
ON c.CompanyIndex = cm.AssignedToIndex
LEFT OUTER JOIN dbo.tblCompanies SCAC  with (nolock)
ON cm.assignedindex = SCAC.companyindex
WHERE Type = 4)carrier
WHERE(ShipToID = isnull(@ShipTo,ShipToID))
--ORDER BY Label
ORDER BY NameOrder

/*


EXEC [rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers] 1,1,2, '<ALL>'

*/
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_CompanyAuthorizedShipToCarriers] TO [public]
GO



