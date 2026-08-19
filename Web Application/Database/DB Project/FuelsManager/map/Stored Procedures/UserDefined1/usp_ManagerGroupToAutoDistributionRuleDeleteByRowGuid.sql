
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [map].[tblManagerGroupToAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [map].[usp_ManagerGroupToAutoDistributionRuleDeleteByRowGuid]
	@ManagerGroupToAutoDistributionRuleGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [map].[tblManagerGroupToAutoDistributionRule]
			WHERE [ManagerGroupToAutoDistributionRuleGuid] = @ManagerGroupToAutoDistributionRuleGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of ManagerGroupToAutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [map].[tblManagerGroupToAutoDistributionRule] 
	WHERE [ManagerGroupToAutoDistributionRuleGuid] = @ManagerGroupToAutoDistributionRuleGuid; 
END