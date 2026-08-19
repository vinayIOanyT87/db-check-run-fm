CREATE PROCEDURE [dbo].[usp_UpdateOrInsertTransactionAliasFieldPlacementInformation]
	@TransactionAliasGuid uniqueidentifier,
	@PlacementInformation nvarchar(max),
	@CreatedBy nvarchar(100),
	@UpdatedBy nvarchar(100)
AS
	MERGE [dbo].[tblTransactionAliasFieldPlacementInformation] AS target
USING (SELECT 
	@TransactionAliasGuid, @PlacementInformation, 
	@CreatedBy,
	@UpdatedBy) 
	AS source(TransactionAliasGuid, PlacementInformation, CreatedBy, UpdatedBy)
	ON target.TransactionAliasGuid = source.TransactionAliasGuid
WHEN MATCHED THEN
UPDATE SET 
	PlacementInformation = source.PlacementInformation,
	UpdatedBy = source.UpdatedBy,
	UpdatedDate = GETDATE()
WHEN NOT MATCHED THEN
INSERT (TransactionAliasGuid, PlacementInformation, 
	CreatedBy, CreatedDate, 
	UpdatedBy, UpdatedDate)
	VALUES (TransactionAliasGuid, PlacementInformation,
	CreatedBy, GETDATE(),
	UpdatedBy, GETDATE());