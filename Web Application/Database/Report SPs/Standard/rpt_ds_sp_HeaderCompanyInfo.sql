USE ConsolidatedDB
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[rpt_ds_sp_HeaderCompanyInfo]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[rpt_ds_sp_HeaderCompanyInfo]
GO

CREATE PROCEDURE [rpt_ds_sp_HeaderCompanyInfo]

 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description: 	
 Version:				7.5.1.0		
 Modification History:
	Date		by		Description
	4/28/2009	UP		Rename from [fm_HeaderCompanyInfo] to [rpt_ds_sp_HeaderCompanyInfo]
	
 =============================================*/
(
	@SiteIndex int
)
AS

SELECT	ID,
		Number,
		SPLCCode,
		Address1 + ' ' + Address2 AS Address,
		City,
		State,
		Zip,
		Phone,
		UserData1 AS EPANumber, 
        City +', '+State + ' '+Zip + ' '+Phone AS CityStateZipPhone,
        ID + '   Terminal ID: '+Number AS TerminalNumber,
		CompanyName = (SELECT [Name] from tblCompanies  --Get name from Manager Name field
						WHERE CompanyIndex =(select Top 1 CompanyIndex from dbo.tblCompanyRoleMap
											where [Role]=0 AND SiteIndex = @SiteIndex ))
FROM tblSites
WHERE SiteIndex = @SiteIndex

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.[rpt_ds_sp_HeaderCompanyInfo] TO [public]
GO

