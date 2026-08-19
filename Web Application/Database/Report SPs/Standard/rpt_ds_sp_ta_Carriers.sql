USE ConsolidatedDB

GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_ds_sp_ta_Carriers') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_ds_sp_ta_Carriers
GO


CREATE PROCEDURE  [dbo].[rpt_ds_sp_ta_Carriers] 
 /*=============================================
 Author:				Kimberly Foote
 Create date:			7/16/2009
 Description:			Dataset For Carriers LookUp
 Version:				7.5.1.0
 Modification History:
	Date		by		Description
	7/16/2009	KF		NEW
	8/10/2009	KF		rtrim and ltrim label
	4/14/2011	Al		[Bug 22310] Added another JOIN to tblComapanyMap table
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int
)

 AS

		Select '' AS Carrier, '<All>' AS Label, '(-1)' AS 'NameOrder'

UNION ALL

		Select	distinct c.CompanyIndex 
				,rtrim(ltrim(c.Name)) AS Label
				,c.Name  AS 'NameOrder'
				--,P.PersonID AS Driver
		From  dbo.tblCompanies c with (nolock)
			LEFT JOIN dbo.tblCompanyMap m WITH (NOLOCK) ON
				m.AssignedIndex = c.CompanyIndex
			LEFT JOIN dbo.tblPersonnel p WITH (NOLOCK) ON
				m.AssignedToIndex = p.PersonIndex

		Where	
				c.SiteIndex = 1  
			and (c.Name <> '')
			and (P.PersonID is not null or P.PersonID <> '')
			and	m.[Type] = 15
		Order By NameOrder





/*
EXEC [rpt_ds_sp_ta_Carriers] 1,1,2
*/



GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_ds_sp_ta_Carriers] TO [public]
GO
