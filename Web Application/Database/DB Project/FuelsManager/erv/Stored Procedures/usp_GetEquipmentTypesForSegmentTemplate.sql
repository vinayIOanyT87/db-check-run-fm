
--EXEC usp_GetEquipmentTypesForSegmentTemplate '00000000-0000-0000-0000-000000000001'
	
CREATE PROCEDURE [erv].[usp_GetEquipmentTypesForSegmentTemplate]
(
	@SiteGuid uniqueidentifier=NULL
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_GetEquipmentTypesForSegmentTemplate] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 2012-08-14
	-- Description: Retrieve the set of Equipment Types that are to be supported for the Equipment Type filter defined on the Equipment segment record in tblEntitySegmentTemplate.
	-- Notes:
	-- 1. @SiteGuid parameter helps filter the values by SiteGuid. 
	--    When provided (i.e. not null), it filters the resultset to those equipment types that have been assigned to the site identified by the @SiteGuid.
	--	  It assumes that an Equipment Type owned by a site/sitegroup always has an entity-to-site assignment to the same site.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
 
	SELECT a.EquipmentTypeGuid, a.EqTypeName, b.SiteGuid
	FROM [dbo].[tblEquipmentTypes] a
	INNER JOIN [map].[tblEntityEquipmentTypeToSite] b
	ON b.EquipmentTypeGuid = a.EquipmentTypeGuid
	WHERE ((b.SiteGuid = @SiteGuid) OR (@SiteGuid IS NULL))
	UNION
	SELECT NULL EquipmentTypeGuid, NULL EqTypeName, @SiteGuid SiteGuid  --All one entry for a NULL Equipment Type. This corresponds to the case where the Equipment Type of an equipment is undefined (NULL).
	ORDER BY a.EqTypeName
 
END