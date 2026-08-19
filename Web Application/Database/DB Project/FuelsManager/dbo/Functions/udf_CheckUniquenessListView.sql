

CREATE FUNCTION [dbo].[udf_CheckUniquenessListView]
(@ListViewGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(50), @LookupListViewStandardTypeIndex int)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblListView
	IF @LookupListViewStandardTypeIndex <> 1
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM tblListViews e
		LEFT JOIN map.tblEntityListViewToSite em1 ON em1.ListViewGuid = e.ListViewGuid
		RIGHT JOIN map.tblEntityListViewToSite em2 ON em2.ListViewGuid = @ListViewGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ListViewGuid <> @ListViewGuid
		AND ID = @ID)
			SET @Exists = 0
	END
	ELSE
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM tblListViews e
		LEFT JOIN map.tblEntityLedgerViewToSite em1 ON em1.ListViewGuid = e.ListViewGuid
		RIGHT JOIN map.tblEntityLedgerViewToSite em2 ON em2.ListViewGuid = @ListViewGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ListViewGuid <> @ListViewGuid
		AND ID = @ID)
			SET @Exists = 0
	END

	RETURN @Exists
END
