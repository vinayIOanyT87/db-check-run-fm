


CREATE FUNCTION [rpt].[udf_MonthlyJournalBaseTable] 
 ( 
        @SiteGuidParams nvarchar(max),
		@ManagerGuidParams nvarchar(max),
		@OwnerGuidParams nvarchar(max),
		@ProductGuid uniqueidentifier
 ) 
 RETURNS @OutputTable TABLE (  
							  SiteGuid uniqueidentifier, 
							  ManagerGuid uniqueidentifier, 
							  OwnerGuid uniqueidentifier) 
 AS 
 /********************************************************************************* 
 ** Description          : This function returns a table populated with a row for each string value in the space separated string 
 ** Assumptions          : None 
 ** Inputs               : @StringInput = the space separated string values 
 ** Outputs              : Single table  
 ** Output Rows          : One row per space separated string value 
 ** Return Values        : None 
 *********************************************************************************/ 
 BEGIN 
	 
	 --Populate the Site Table
	 DECLARE @SitesGuidTable TABLE (SiteGuid uniqueidentifier)
     SET @SiteGuidParams = RTRIM(LTRIM(@SiteGuidParams)) 
     insert into @SitesGuidTable
	 Select IdentityGuid from rpt.udf_StringListToGuidTable(@SiteGuidParams)

	 Delete from @SitesGuidTable where SiteGuid not in
	(
		Select SiteGuid from map.tblEntityProductToSite where ProductGuid = @ProductGuid
	)

	 --Populate the Site,Manager Table
	 DECLARE @ManagerGuidTable TABLE (SiteGuid uniqueidentifier, ManagerGuid uniqueidentifier)
     SET @ManagerGuidParams = RTRIM(LTRIM(@ManagerGuidParams)) 
     insert into @ManagerGuidTable
	 Select a.SiteGuid,b.IdentityGuid from 
	 @SitesGuidTable a,
	 rpt.udf_StringListToGuidTable(@ManagerGuidParams) b, 
	 map.tblEntityCompanyToSite c
	 where 
	 a.SiteGuid = c.SiteGuid and
	 c.CompanyGuid = b.IdentityGuid

	 --Populate the Site,Manager,Owner Table
	 DECLARE @OwnerGuidTable TABLE (SiteGuid uniqueidentifier,  ManagerGuid uniqueidentifier, OwnerGuid uniqueidentifier)
     SET @OwnerGuidParams = RTRIM(LTRIM(@OwnerGuidParams)) 
	 insert into @OwnerGuidTable
	 Select a.SiteGuid,a.ManagerGuid,b.IdentityGuid from 
	 @ManagerGuidTable a,
	 rpt.udf_StringListToGuidTable(@OwnerGuidParams) b,
	 map.tblEntityCompanyToSite d
	 where 
	 a.SiteGuid = d.SiteGuid and
	 d.CompanyGuid = b.IdentityGuid

	--Populate the Output Table
	 insert into @OutputTable
	 (SiteGuid,ManagerGuid,OwnerGuid) 
	 Select 
			DISTINCT b.SiteGuid, b.ManagerGuid,b.OwnerGuid
		from 
		@OwnerGuidTable b
		order by b.SiteGuid,b.ManagerGuid,b.OwnerGuid
     RETURN 
 END