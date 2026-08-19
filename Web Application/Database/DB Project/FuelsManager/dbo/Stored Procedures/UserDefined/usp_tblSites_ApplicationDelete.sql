
/****************************** usp_tblSites_ApplicationDelete ******************************/
CREATE PROCEDURE dbo.usp_tblSites_ApplicationDelete
	@SiteGuid UNIQUEIDENTIFIER
AS
BEGIN
	/*
		If you are creating a new entity(Product) and it referenses a parent(Site)
		you should have a tblEntity Table(tblProducts) and a tblParent table (tblSite)
		
		you have have usp_EntityDeleteApplicationByRowGuid (usp_ProductDeleteApplicationByRowGuid)

		and you should have usp_EntityDeleteApplicationByParentGuid.(usp_ProductDeleteApplicationBySiteGuid)
		you should add code here to call:
		EXEC usp_tblEntity_ParentDelete @SiteGuid
	*/
	-- Cascade Delete
	EXEC dbo.usp_AutoDistributionRuleDeleteApplicationBySiteGuid @SiteGuid 
	EXEC dbo.usp_tblAutoDistributionReasonCodes_SiteDelete @SiteGuid 
	EXEC map.usp_tblEntityAutoDistributionReasonCodeToSite_SiteDelete @SiteGuid 
	
	-- workaround for existing issue.
	DELETE FROM dbo.tblChangesQueue
	WHERE SiteGuid = @SiteGuid
	DELETE FROM dbo.tblSessions
	WHERE SiteGuid = @SiteGuid
	
	-- Delete Site Record
	EXEC dbo.usp_tblSites_Delete @SiteGuid 		
END