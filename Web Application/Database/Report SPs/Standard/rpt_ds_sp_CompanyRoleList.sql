USE ConsolidatedDB
GO
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_ds_sp_CompanyRoleList') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_ds_sp_CompanyRoleList
GO

create procedure [dbo].rpt_ds_sp_CompanyRoleList
 /*=============================================
 Author:	 			UNKNOWN
 Create date: 			
 Description: 	
 Version:				7.5.1.0		
 Modification History:
	Date		by		Description
	4/29/2009	UP		Rename from [fm_CompanyRoleList] to [rpt_ds_sp_CompanyRoleList]
 =============================================*/

(
	@SiteIndex int,
	@ShowAll bit
)

as 

begin

set nocount on

-- The following query returns company role definitions that are assigned to the Site
declare @CompanyRoleList table(	RoleName nvarchar (100), RoleIndex int)

-- only insert <All> when @ShowAll is set to true
if @ShowAll = 1 insert into @CompanyRoleList select '<All>', -999

-- insert each role into list
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Bill To'),'Bill To')), 3		--CUSTOMER_BILLTO = 3
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Carrier'),'Carrier')), 5		--CARRIER = 5
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Manager'),'Manager')), 0		--MANAGER = 0
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Owner'),'Owner')), 1			--OWNER = 1
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Ship To'),'Ship To')), 4		--CUSTOMER_SHIPTO = 4		
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Shipper'),'Shipper')), 2		--SHIPPER = 2		
insert into @CompanyRoleList select (isnull((select [value] from tbldatadictionaries where siteindex = @SiteIndex and [key] = 'Supplier'),'Supplier')), 6	--SUPPLIER = 6

-- final query
select * from @CompanyRoleList order by RoleName
end


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
GRANT EXECUTE ON dbo.[rpt_ds_sp_CompanyRoleList] TO [public]
GO

