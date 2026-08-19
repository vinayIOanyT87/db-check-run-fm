CREATE PROCEDURE [dbo].[gsp_ExStarsProductPriorInventoryInsert]
  @ManagerCompanyGuid	UNIQUEIDENTIFIER
, @SiteGuid				UNIQUEIDENTIFIER
, @TaxCode				NVARCHAR(10)
, @PriorInventoryExists BIT
, @UpdatedBy				[dbo].[udtUserID]

AS
BEGIN
	INSERT INTO [dbo].[tblExStarsProductPriorInventory](
		 [ManagerCompanyGuid]
		,[SiteGuid]
		,[TaxCode]
		,[PriorInventoryExists]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy])
		Values(
		  @ManagerCompanyGuid	
		, @SiteGuid				
		, @TaxCode 		 			
		, @PriorInventoryExists 
		, GETDATE()
		, @UpdatedBy	
		, GETDATE()
		, @UpdatedBy
		)		
END