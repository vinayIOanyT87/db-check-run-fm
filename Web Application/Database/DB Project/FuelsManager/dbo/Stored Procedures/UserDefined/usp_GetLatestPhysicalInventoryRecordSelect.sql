


CREATE PROCEDURE [dbo].[usp_GetLatestPhysicalInventoryRecordSelect]
@SiteGuid UNIQUEIDENTIFIER, @InventoryDate DATE, @ManagerCompanyGuid UNIQUEIDENTIFIER, @ProductGuid UNIQUEIDENTIFIER, @TankGuid UNIQUEIDENTIFIER
AS 
BEGIN
	SET NOCOUNT ON

	declare @InventoryDateVar		date
    declare @ProductGuidVar			uniqueidentifier 
    declare @SiteGuidVar            uniqueidentifier
    declare @ManagerCompanyGuidVar	uniqueidentifier
    declare @TankGuidVar			uniqueidentifier
 
    Set @InventoryDateVar = @InventoryDate;
    Set @SiteGuidVar = @SiteGuid;
    Set @ManagerCompanyGuidVar = @ManagerCompanyGuid;
    Set @ProductGuidVar = @ProductGuid;
    Set @TankGuidVar = @TankGuid;


	SELECT TOP 1 InventoryDate
	FROM tblTransactionLineItems l WITH(NOLOCK)INNER JOIN tblTransactions t WITH(NOLOCK) ON l.TransactionGuid = t.TransactionGuid
	WHERE t.SiteGuid     = @SiteGuidVar
	AND t.InventoryDate < @InventoryDateVar
	AND t.ManagerCompanyGuid  = @ManagerCompanyGuidVar
	AND l.ProductGuid  = @ProductGuidVar
	AND t.DeleteFlag = cast(0 AS bit)
	AND (l.StorageLocationTankGuid = @TankGuidVar OR @TankGuidVar IS NULL)
	ORDER BY InventoryDate DESC
END