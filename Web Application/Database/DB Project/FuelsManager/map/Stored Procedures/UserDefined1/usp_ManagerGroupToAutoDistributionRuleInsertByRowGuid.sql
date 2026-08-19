
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Insert a record to the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleInsertByRowGuid] (
	@AutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@ManagerGroupGuid UNIQUEIDENTIFIER,
	@CreatedDate DATETIMEOFFSET,
	@CreatedBy udtUserID,
	@UpdatedDate DATETIMEOFFSET,
	@UpdatedBy udtUserID,
	@_RowVersion VARBINARY(8) = NULL,
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER
	 OUTPUT
) AS
BEGIN
	DECLARE @NewPrimaryKeyGuid UNIQUEIDENTIFIER
	SET @NewPrimaryKeyGuid = NEWID()
	INSERT INTO [map].[tblManagerGroupToAutoDistributionRule]
	( 
		ManagerGroupToAutoDistributionRuleGuid, AutoDistributionRuleGuid, ManagerGroupGuid, 
		CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	) VALUES ( 
		@NewPrimaryKeyGuid, @AutoDistributionRuleGuid, @ManagerGroupGuid, 
		@CreatedDate, @CreatedBy, @UpdatedDate, @UpdatedBy
	)
	SET @ManagerGroupToAutoDistributionRuleGuid = @NewPrimaryKeyGuid
END