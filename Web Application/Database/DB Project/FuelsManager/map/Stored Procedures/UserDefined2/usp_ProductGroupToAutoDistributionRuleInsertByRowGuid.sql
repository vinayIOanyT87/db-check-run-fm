
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblProductGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ProductGroupToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ProductGroupGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@ProductGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblProductGroupToAutoDistributionRule]
	( 
		ProductGroupToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ProductGroupGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ProductGroupGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ProductGroupToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END