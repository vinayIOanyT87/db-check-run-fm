USE [ConsolidatedDB]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_AllocationSummaryReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_AllocationSummaryReport
GO


CREATE PROCEDURE [dbo].rpt_sp_ta_AllocationSummaryReport

 /*=============================================
 Author:				URVI PATEL
 Create date:			6/17/2009
 Description:			
 Version:				7.5.1.0
 Execution:		
				EXEC rpt_sp_ta_AllocationSummaryReport 1,1,2,1,'<ALL>','<ALL>'

 Modification History:
	Date		by		Description
	6/17/2009	UP		New Stored Procedure  0 - StockHolder, 1-Shipper, 2- BillTo, 3- ShipTo
	12/9/2009	KF		Vesion 7.5.1.0
 =============================================*/

(

 	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@Type int,
	@AllocationGroup varchar(30),
	@Company varchar(30)

)
AS

IF @AllocationGroup = '<ALL>' SET  @AllocationGroup = NULL
IF @Company = '<ALL>' SET  @Company = NULL

DECLARE @VolumeUnits int
SET @VolumeUnits = (SELECT VolumeUnitIndex FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @SiteIndex)

DECLARE @VolumeDecimalPlaces int
SET @VolumeDecimalPlaces = (SELECT VolumeDecimalPlaces FROM tblSites with(nolock) WHERE tblSites.SiteIndex = @SiteIndex)

SELECT		Manager,
			tmpall.ID,
			Company,
			StockHolder,
			Shipper,
			BillTo,
			ShipTo,
			companymaptype,
			EffectiveDate,
			ExpirationDate, 
			AllocationGroupIndex,
			tblas.ID As 'AllocationGroup' ,
			LoadWarning,
			LoadDenial, 
			ContractNumber,
			LastAllocationResetDate ,
			Type = Case when tmpall.Type = 0 then 'Product'
								when tmpall.Type = 1 then 'Group'
								when tmpall.Type = 2 then 'All Products'
								when tmpall.Type = 3 then ''
								else ''
								END,

			productid ,
		--	Limit  ,
			Limit_Conv,
			Next,
			ResetPeriod =  Case when ResetPeriod = 0 then 'Day'
								when ResetPeriod = 1 then 'Week'
								when ResetPeriod = 2 then 'Month'
								when ResetPeriod = 3 then 'Year'
								else ''
								END,
			ResetMultiple,
			ResetMethod = Case when ResetMethod = 0 then 'Repeat'
								when ResetMethod = 1 then 'Balance'
								when ResetMethod = 2 then 'Next'
								when ResetMethod = 3 then 'Next-Balance'
								when ResetMethod = 4 then 'Book-Unavailable'
								when ResetMethod = 5 then  ''
								else ''
								END,
			ResetDate
FROM

(
	select	manager.ID AS Manager,
			C.name  AS  [ID],
			C.name AS StockHolder,
			'' AS Shipper,
			'' AS BillTo,
			'' AS ShipTo,
			c.ID AS [Company],
			a.companymaptype,
			a.EffectiveDate,
			a.ExpirationDate, 
			AllocationGroupIndex,
			a.LoadWarning,
			a.LoadDenial, 
			a.ContractNumber,
			a.LastAllocationResetDate ,
			al.Type,-- e.typeid,
			p.productid ,
			al.Limit  ,
			IsNull(dbo.ConvertFromSIUnits(al.limit,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Limit_Conv,
			al.Next,
			al.ResetPeriod,
			al.ResetMultiple,
			al.ResetMethod, 
			al.ResetDate
from dbo.tblAllocations a with (nolock)
join dbo.tblAllocationLineItems AL  with (nolock)
on a.[Index] = AL.allocationindex
JOIN tblproducts p  with (nolock)
on al.assignedindex = p.productindex
JOIN tblCompanyMap cm  with (nolock)
ON cm.[index] = a.companymapindex
JOIN tblcompanies c  with (nolock)
ON cm.assignedindex = c.companyindex
JOIN  tblcompanies manager  with (nolock)
ON cm.assignedtoindex = manager.companyindex
WHERE a.companymaptype = 0   -- StockHolder

UNION ALL 

/*     Shipper */

select		Manager.ID AS Manager,
			stockholder.name + '->'+C.name  AS [ID], 
			stockholder.name AS StockHolder,
			C.name AS Shipper,
			'' AS BillTo,
			'' AS ShipTo,
			c.ID AS [Company],
			a.companymaptype,
		--	cm.type,
		--	stockholderindex.type,
			a.EffectiveDate,
			a.ExpirationDate, 
			AllocationGroupIndex,
			a.LoadWarning,
			a.LoadDenial, 
			a.ContractNumber,
			a.LastAllocationResetDate ,
			al.Type,-- e.typeid,
			p.productid ,
			al.Limit  ,
			IsNull(dbo.ConvertFromSIUnits(al.limit,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Limit_Conv,
			al.Next,
			al.ResetPeriod,
			al.ResetMultiple,
			al.ResetMethod, 
			al.ResetDate
from dbo.tblAllocations a  with (nolock)
join dbo.tblAllocationLineItems AL  with (nolock)
on a.[Index] = AL.allocationindex
JOIN tblproducts p  with (nolock)
on al.assignedindex = p.productindex
JOIN tblCompanyMap cm  with (nolock)
ON cm.[index] = a.companymapindex
JOIN tblcompanies c  with (nolock)
ON cm.assignedindex = c.companyindex
JOIN tblCompanyMap stockholderindex  with (nolock)
on cm.Assignedtoindex = stockholderindex.[index]
JOIN tblcompanies stockholder  with (nolock)
ON stockholderindex.assignedindex = stockholder.companyindex
JOIN  tblcompanies manager  with (nolock)
ON stockholderindex.assignedtoindex = manager.companyindex
WHERE a.companymaptype = 1    -- Shipper

UNION ALL

/*  BillTo  */


select		Manager.ID  AS Manager,
			--stockholder.name+'->'+ BillTo.name+'->'+C.name   AS [ID], 
			stockholder.name+'->'+ Shipper.name+'->'+C.name   AS [ID], 
			stockholder.name  AS StockHolder,
			Shipper.name AS Shipper,
			c.name AS BillTo,
			'' AS ShipTo,
			c.ID AS [Company],
			a.companymaptype,
		--	cm.type,
		--	stockholderindex.type,
			a.EffectiveDate,
			a.ExpirationDate, 
			AllocationGroupIndex,
			a.LoadWarning,
			a.LoadDenial, 
			a.ContractNumber,
			a.LastAllocationResetDate ,
			al.Type,-- e.typeid,
			p.productid ,
			al.Limit  ,
			IsNull(dbo.ConvertFromSIUnits(al.limit,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Limit_Conv,
			al.Next,
			al.ResetPeriod,
			al.ResetMultiple,
			al.ResetMethod, 
			al.ResetDate
from dbo.tblAllocations a  with (nolock)
join dbo.tblAllocationLineItems AL  with (nolock)
on a.[Index] = AL.allocationindex
JOIN tblproducts p  with (nolock)
on al.assignedindex = p.productindex
JOIN tblCompanyMap cm  with (nolock)
ON cm.[index] = a.companymapindex
JOIN tblcompanies c  with (nolock)
ON cm.assignedindex = c.companyindex
JOIN tblCompanyMap Shipperindex  with (nolock)
on cm.Assignedtoindex = Shipperindex.[index]
JOIN tblcompanies Shipper  with (nolock)
ON Shipperindex.assignedindex = Shipper.companyindex
JOIN tblCompanyMap stockholderindex  with (nolock)
on Shipperindex.Assignedtoindex = stockholderindex.[index]
JOIN tblcompanies stockholder  with (nolock)
ON stockholderindex.assignedindex = stockholder.companyindex
JOIN  tblcompanies manager  with (nolock)
ON stockholderindex.assignedtoindex = manager.companyindex
WHERE a.companymaptype = 2    -- BillTo

UNION ALL 

/* Ship TO */


select		Manager.ID  AS Manager,
			--stockholder.name+'->'+ BillTo.name+'->'+ ShipTo.name+'->'+   C.name AS [ID],
			stockholder.name+'->'+ Shipper.name+'->'+ BillTo.name+'->'+   C.name AS [ID],
			stockholder.name AS StockHolder,
			Shipper.name As Shipper,
			BillTo.name AS BillTo,
			c.name AS ShipTo,
			c.ID AS [Company],
			a.companymaptype,
		--	cm.type,
		--	stockholderindex.type,
			a.EffectiveDate,
			a.ExpirationDate, 
			AllocationGroupIndex,
			a.LoadWarning,
			a.LoadDenial, 
			a.ContractNumber,
			a.LastAllocationResetDate ,
			al.Type,-- e.typeid,
			p.productid ,
			al.Limit  ,
			IsNull(dbo.ConvertFromSIUnits(al.limit,@VolumeUnits,@VolumeDecimalPlaces),0.0) AS Limit_Conv,
			al.Next,
			al.ResetPeriod,
			al.ResetMultiple,
			al.ResetMethod, 
			al.ResetDate
from dbo.tblAllocations a  with (nolock)
join dbo.tblAllocationLineItems AL  with (nolock)
on a.[Index] = AL.allocationindex
JOIN tblproducts p  with (nolock)
on al.assignedindex = p.productindex
JOIN tblCompanyMap cm  with (nolock)
ON cm.[index] = a.companymapindex
JOIN tblcompanies c  with (nolock)
ON cm.assignedindex = c.companyindex
--JOIN tblCompanyMap BillToindex  with (nolock)
--on cm.Assignedtoindex = BillToindex.[index]
--JOIN tblcompanies BillTo  with (nolock)
--ON BillToindex.assignedindex = BillTo.companyindex
JOIN tblCompanyMap BillToindex  with (nolock)
on cm.Assignedtoindex = BillToindex.[index]
JOIN tblcompanies BillTo  with (nolock)
ON BillToindex.assignedindex = BillTo.companyindex

JOIN tblCompanyMap Shipperindex  with (nolock)
on BillToindex.Assignedtoindex = Shipperindex.[index]
JOIN tblcompanies Shipper  with (nolock)
ON Shipperindex.assignedindex = Shipper.companyindex
JOIN tblCompanyMap stockholderindex  with (nolock)
on Shipperindex.Assignedtoindex = stockholderindex.[index]
JOIN tblcompanies stockholder  with (nolock)
ON stockholderindex.assignedindex = stockholder.companyindex
JOIN  tblcompanies manager  with (nolock)
ON stockholderindex.assignedtoindex = manager.companyindex

WHERE a.companymaptype = 3    -- ShipTo

)tmpall
LEFT JOIN tblApplicationString tblas  with (nolock)
ON tmpall.AllocationGroupIndex = tblas.[Index] 

WHERE tmpall.companymaptype = @Type
AND ( isnull(tmpall.AllocationGroupIndex,-999) = isnull(@AllocationGroup,isnull(tmpall.AllocationGroupIndex,-999)) )
AND ( tmpall.company = isnull(@Company,tmpall.company) )

ORDER BY tmpall.ID, tmpall.productid

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_AllocationSummaryReport TO [public]
GO
