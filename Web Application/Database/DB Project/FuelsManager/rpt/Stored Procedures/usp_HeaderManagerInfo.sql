CREATE PROCEDURE [rpt].[usp_HeaderManagerInfo]

 /*=============================================
 Author:	 			Paul Carpenter
 Create date: 			
 Description:			Pull Manager contact Info
 Execution:
		declare @LoginSiteGuid  UNIQUEIDENTIFIER=N'55bcb5db-57e6-4afc-b4f4-3029b55afd40'
		declare @UserGuid  UNIQUEIDENTIFIER =N'ae595738-2522-470b-93bf-94f99115544f'
		declare @ManagerGuid UNIQUEIDENTIFIER='C27988F0-980D-4715-8FFA-881744EB9A54'
		exec [rpt].[usp_HeaderManagerInfo] @LoginSiteGuid, @UserGuid , @ManagerGuid 
		
 Modification History:
	Date		by		Description
	2015-Jan-26 pcarpenter rewritten for FuelsManager 9.x
	
 =============================================*/
(
	@LoginSiteGuid uniqueidentifier,
	@UserGuid uniqueidentifier,
	@ManagerGuid uniqueidentifier
)
AS

DECLARE @ManagerRole int = 0;

Select top 1
	 ID as Manager
	,Address1 + ' ' + Address2 AS Address
	,City
	,State
	,Zip
	,Phone
	,case when City = '' then '' else City +', '+State + ' '+Zip + ' '+Phone end AS CityStateZipPhone
	,EmergencyContact
	,Name as CompanyName
	From tblCompanies c
	INNER JOIN map.tblCompanyToRole cr on c.companyGuid=cr.companyGuid 
	Where 1=1
		AND cr.LookupCompanyRoleIndex = @ManagerRole
		AND c.CompanyGuid = @ManagerGuid 
		--AND @ManagerGuid  in  ( SELECT CompanyGuid  FROM  [dbo].[udf_AuthorizedCompaniesGuid](@LoginSiteGuid, @UserGuid)) 	
	Order by ID
	
