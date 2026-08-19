CREATE PROCEDURE [dbo].[usp_TransactionsGetEarliestInventoryDate]
	@SiteGuid UNIQUEIDENTIFIER,
	@ManagerCompanyGuid UNIQUEIDENTIFIER,
	@ProductGuid UNIQUEIDENTIFIER
AS
BEGIN
	SET NOCOUNT ON

	--Get the earliest inventory date for any transaction that has a site and manager equal to the parameters provided,
	--and where any associated line item or sub line item uses the product specified.
	SELECT MIN(InventoryDate) AS InventoryDate 
	FROM tblTransactions
	INNER JOIN tblTransactionLineItems ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
	LEFT JOIN tblTransactionSubLineItems ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
	WHERE SiteGuid = @SiteGuid AND ManagerCompanyGuid = @ManagerCompanyGuid AND (tblTransactionLineItems.ProductGuid = @ProductGuid OR tblTransactionSubLineItems.ProductGuid = @ProductGuid)

END