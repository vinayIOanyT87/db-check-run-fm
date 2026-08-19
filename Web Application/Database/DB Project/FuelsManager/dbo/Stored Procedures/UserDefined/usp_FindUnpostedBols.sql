-- =============================================
-- Author:		Sijuan Jiang
-- Create date: 10/28/2016
-- Description:	
-- =============================================
CREATE PROCEDURE usp_FindUnpostedBols 
	-- Add the parameters for the stored procedure here
	@SiteGuid UNIQUEIDENTIFIER, 
	@ManagerID nvarchar(30),
	@StartDate datetime,
	@EndDate datetime,
	@ProductID nvarchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @ProductGuid UNIQUEIDENTIFIER 
select @ProductGuid = _MasterRecordGuid from tblProducts where ProductID = @ProductID

declare @ManagerGuid UNIQUEIDENTIFIER
select @ManagerGuid = _MasterRecordGuid from tblCompanies where ID = @ManagerID

select t.transid, t.aliasname, t.documentnumber
from tblTransactions t inner join tblTransactionLineItems l on t.TransactionGuid = l.TransactionGuid
					left outer join tblTransactionSubLineItems s on l.TransactionLineItemGuid = s.TransactionLineItemGuid
where t.LookupTransTypeIndex = 5
	and t.SiteGuid = @SiteGuid
	and t.ManagerCompanyGuid = @ManagerGuid
	and not exists (select recordid from tblExportResultDetails where RecordID = t.TransID and Fail = 0) 
	and t.InventoryDate between @StartDate and @EndDate
	and (l.ProductGuid = @ProductGuid or s.ProductGuid = @ProductGuid)
	and t.DeleteFlag = 0 
	and t.LookupTransactionStatusIndex != 7 -- cancelled
	and t.LookupTransactionStatusIndex != 11 -- Posted 
	--and l.DeleteFlag = 0 
	--and (s.DeleteFlag = 0 or s.DeleteFlag is null) 
union
select t.transid, t.aliasname, t.documentnumber
from tblTransactions t inner join tblTransactionLineItems l on t.TransactionGuid = l.TransactionGuid
where t.LookupTransTypeIndex = 8
	and t.SiteGuid = @SiteGuid
	and t.ManagerCompanyGuid = @ManagerGuid
	and t.InventoryDate between @StartDate and @EndDate
	and l.ProductGuid = @ProductGuid
	and (t.transdatetime is null
		or t.PONumber is null
		or t.DocumentNumber is null
		or t.OwnerID is null
		or t.CarrierID is null
		or l.NetQuantity is null)
	and t.DeleteFlag = 0 
	and t.LookupTransactionStatusIndex != 7 -- cancelled
	and t.LookupTransactionStatusIndex != 11 -- Posted 
union
select t.transid, t.aliasname, t.documentnumber
from tblTransactions t inner join tblTransactionLineItems l on t.TransactionGuid = l.TransactionGuid
					inner join tblTransactionSubLineItems s on l.TransactionLineItemGuid = s.TransactionLineItemGuid
where t.LookupTransTypeIndex = 8
	and t.SiteGuid = @SiteGuid
	and t.ManagerCompanyGuid = @ManagerGuid
	and t.InventoryDate between @StartDate and @EndDate
	and s.ProductGuid = @ProductGuid
	and (t.transdatetime is null
		or t.PONumber is null
		or t.DocumentNumber is null
		or t.OwnerID is null
		or t.CarrierID is null
		or l.NetQuantity is null)
	and t.DeleteFlag = 0 
	and t.LookupTransactionStatusIndex != 7 -- cancelled
	and t.LookupTransactionStatusIndex != 11 -- Posted 
END
