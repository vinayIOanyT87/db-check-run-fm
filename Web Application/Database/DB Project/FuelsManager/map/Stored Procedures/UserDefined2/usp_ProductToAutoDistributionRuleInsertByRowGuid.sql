
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblProductToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@ProductToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblProductToAutoDistributionRule]
	( 
		ProductToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ProductGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ProductGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ProductToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END