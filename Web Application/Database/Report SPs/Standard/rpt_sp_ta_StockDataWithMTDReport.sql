USE [ConsolidatedDB]
GO

SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.rpt_sp_ta_StockDataWithMTDReport') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.rpt_sp_ta_StockDataWithMTDReport
GO

CREATE PROCEDURE [dbo].[rpt_sp_ta_StockDataWithMTDReport]

/********************************************************************************************************
	
 Author:				UNKNOWN
 Create date:			
 Description:			Has sub stored procedure rpt_sp_ta_StockDataReport
 Version:				7.5.1.2
 Execution:
				Execute rpt_sp_ta_StockDataWithMTDReport '1/1/2010','1/30/2010','3427 - CITGO Petroleum Corp','<All>','<All>',1,2,1

 Modification History:

	Date			By			Description
	2/10/2010		KF			Standard Report
	2/12/2010		KF			Version change due to change in report

**************************************************************************************************************/


(
	@BeginDate smalldatetime,
	@EndDate smalldatetime,
	@Manager nvarchar(1000),
	@Owner nvarchar(1000),
	@Product nvarchar(1000),
	@SiteIndex int,
	@UserIndex int,
	@LoginSiteIndex int
)
as 

begin
set nocount on

-- find the beginning of the end month
declare @beginmonthdate datetime;
declare @day int;
select @day = DATEPART(day, @EndDate);
select @beginmonthdate = dateadd(day, (-@day)+1, @EndDate)

declare @TempTable table(	OwnerID nvarchar (30), OwnerName nvarchar (100), ProductID nvarchar(30), ProductType int, LoadRackDisplayText nvarchar(10), 
							[Begin Inventory] float, [Book Inventory] float, [Unavailable Inventory] float, [Available Inventory] float, 
							Adjustment float, BOL float, [Meter Closeout] float, Receipt float, Regrade float, Shipment float, Transfer float )

declare @StockDataReportFinal table(	OwnerID nvarchar (30), OwnerName nvarchar (100), ProductID nvarchar(30), ProductType int, LoadRackDisplayText nvarchar(10), 
										[Begin Inventory] float, [Book Inventory] float, [Unavailable Inventory] float, [Available Inventory] float, 
										Adjustment float, BOL float, [Meter Closeout] float, Receipt float, Regrade float, Shipment float, Transfer float, 
										[Begin Inventory MTD] float, [Book Inventory MTD] float, [Unavailable Inventory MTD] float, [Available Inventory MTD] float, 
										[Adjustment MTD] float, [BOL MTD] float, [Meter Closeout MTD] float, [Receipt MTD] float, [Regrade MTD] float, 
										[Shipment MTD] float, [Transfer MTD] float )

-- insert into temporary table first 
insert @TempTable exec rpt_sp_ta_StockDataReport @BeginDate, @EndDate, @Manager, @Owner, @Product, @SiteIndex, @UserIndex, @LoginSiteIndex

-- insert day totals in final table
insert into @StockDataReportFinal
	select	OwnerID, OwnerName, ProductID, ProductType, LoadRackDisplayText, [Begin Inventory], [Book Inventory], [Unavailable Inventory], [Available Inventory], 
			Adjustment, BOL, [Meter Closeout], Receipt, Regrade, Shipment, Transfer, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
	from @TempTable
delete from @TempTable

-- insert into temporary table second pass with new begin date
insert @TempTable exec rpt_sp_ta_StockDataReport @beginmonthdate, @EndDate, @Manager, @Owner, @Product, @SiteIndex, @UserIndex, @LoginSiteIndex

-- insert month to date totals in final table
update @StockDataReportFinal  
	set	[Begin Inventory MTD] = b.[Begin Inventory], 
		[Book Inventory MTD] = b.[Book Inventory], 
		[Unavailable Inventory MTD] = b.[Unavailable Inventory], 
		[Available Inventory MTD] = b.[Available Inventory], 
		[Adjustment MTD] = b.[Adjustment], 
		[BOL MTD] = b.[BOL], 
		[Meter Closeout MTD] = b.[Meter Closeout], 
		[Receipt MTD] = b.[Receipt], 
		[Regrade MTD] = b.[Regrade], 
		[Shipment MTD] = b.[Shipment], 
		[Transfer MTD] = b.[Transfer]
from @StockDataReportFinal a, @TempTable b
where a.OwnerID = b.OwnerID and a.ProductID = b.ProductID

-- final query
select * from @StockDataReportFinal
end




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.rpt_sp_ta_StockDataWithMTDReport TO [public]
GO

