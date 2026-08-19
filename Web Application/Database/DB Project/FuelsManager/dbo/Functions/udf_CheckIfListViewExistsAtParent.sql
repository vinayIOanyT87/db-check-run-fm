-- =======================================================================================================================================
-- Author:		Srinivasa Divyakolu
-- Create date: 07/27/2022
-- Description:	Recursively checks to see if a given list view by id exists at a given site or at its parent
-- =======================================================================================================================================
CREATE FUNCTION [dbo].[udf_CheckIfListViewExistsAtParent]
(
	-- Add the parameters for the function here
	@SiteGuid uniqueidentifier,
	@ListviewName nvarchar(100)

)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Exists INT
	DECLARE @ListViewGuid as UNIQUEIDENTIFIER
	DECLARE @ParentSiteGuid as UNIQUEIDENTIFIER

	SELECT @ListViewGuid = ListViewGuid FROM [dbo].[tblListViews] Where ID = @ListviewName and SiteGuid =  @SiteGuid
	
	IF @ListViewGuid IS NULL
	BEGIN
		SELECT @ParentSiteGuid = ParentSiteGuid FROM [map].[tblSiteToSite] Where ChildSiteGuid =  @SiteGuid
		
		IF @ParentSiteGuid <> @SiteGuid AND @ParentSiteGuid IS NOT NULL
			SET @Exists = [dbo].[udf_CheckIfListViewExistsAtParent] (@ParentSiteGuid, @ListviewName)
		ELSE
			RETURN 0
	END
	ELSE
		RETURN 1

	-- Return the result of the function
	RETURN @Exists

END
GO
