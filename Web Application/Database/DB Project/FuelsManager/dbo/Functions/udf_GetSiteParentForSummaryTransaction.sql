GO

/****** Object:  UserDefinedFunction [dbo].[udf_GetSiteParentForSummaryTransaction]    Script Date: 10/4/2016 4:27:27 PM ******/

CREATE FUNCTION [dbo].[udf_GetSiteParentForSummaryTransaction]
(	
	@SiteGuid uniqueidentifier
)
RETURNS @SiteTable TABLE
(
	[SiteGuid] [uniqueidentifier]
)
AS
BEGIN

	DECLARE @ParentGuid UNIQUEIDENTIFIER;
	
	INSERT INTO @SiteTable VALUES (@SiteGuid);

	DECLARE CurSites CURSOR FOR SELECT SiteGuid FROM @SiteTable;

	OPEN CurSites;
	
	FETCH NEXT FROM CurSites INTO @ParentGuid;
	
	WHILE @@FETCH_STATUS = 0
		BEGIN
			INSERT INTO @SiteTable
				SELECT ChildSiteGuid 
				FROM map.tblSiteToSite 
				WHERE ParentSiteGuid = @ParentGuid AND ChildSiteGuid <> ParentSiteGuid;

			FETCH NEXT FROM CurSites INTO @ParentGuid;
		END;

	CLOSE CurSites;

	DEALLOCATE CurSites;

	RETURN;
END


GO


