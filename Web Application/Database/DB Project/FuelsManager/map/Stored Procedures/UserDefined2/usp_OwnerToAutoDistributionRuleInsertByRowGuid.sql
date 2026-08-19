
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblOwnerToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_OwnerToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@OwnerGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@OwnerToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblOwnerToAutoDistributionRule]
	( 
		OwnerToAutoDistributionRuleGuid, AutoDistributionRuleGuid, OwnerGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @OwnerGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @OwnerToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END