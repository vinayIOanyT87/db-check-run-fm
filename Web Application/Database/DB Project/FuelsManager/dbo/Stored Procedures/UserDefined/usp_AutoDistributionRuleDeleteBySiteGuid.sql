
-- ==================================================================================================================
-- Author:		Daniel Or
-- Create date:	5/19/2012
-- Description:	Delete a (group of) record(s) from the [dbo].[tblAutoDistributionRule]
-- ==================================================================================================================
CREATE PROCEDURE [dbo].[usp_AutoDistributionRuleDeleteBySiteGuid]
	@SiteGuid UNIQUEIDENTIFIER,
	@_RowVersion VARBINARY(8) = NULL
AS
BEGIN
	SET NOCOUNT ON; 
	IF @_RowVersion IS NOT NULL 
	BEGIN 
		 IF NOT EXISTS(SELECT 1 FROM [dbo].[tblAutoDistributionRule]
			WHERE [SiteGuid] = @SiteGuid AND [_RowVersion]=@_RowVersion) 
		 BEGIN 
			 RAISERROR('Attempted to delete a stale version of AutoDistributionRule.',16,1); 
			 RETURN; 
		END 
	END 
	DELETE [dbo].[tblAutoDistributionRule] 
	WHERE [SiteGuid] = @SiteGuid; 
END