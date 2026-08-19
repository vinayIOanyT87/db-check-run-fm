


-- The original query includes a filter that limits the results to Product that are also owned/assignedto @LoginSiteGuid in addition to being assigned to @SiteGuid, 
-- unless the Product is owned by @Siteguid itself. This filter logic was preserved as the code was modified to be RecordVersioning-aware.
-- EXEC [rpt].[usp_DsProductList] '46426312-E408-4AF8-85FD-338B622B32BF', 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421', 1
-- EXEC [rpt].[usp_DsProductList] 'DF5060D4-25E4-4F56-AE46-50C25331863E', '3D95FDFA-3D72-4E4B-9264-B8E068ECD364', 1

CREATE PROCEDURE [rpt].[usp_DsProductList]
@LoginSiteGuid UNIQUEIDENTIFIER, @SiteGuid UNIQUEIDENTIFIER, @ShowAll INT
AS
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
	SELECT '00000000-0000-0000-0000-000000000000' AS ProductGuid, '<All>' AS ProductID, '<All>' AS Description
	WHERE @ShowAll = 1 
	UNION
	SELECT b.ProductGuid, b.ProductID, b.Description
	FROM [erv].[udf_GetProductRecordVersions](@SiteGuid) a
	INNER JOIN tblProducts b WITH (NOLOCK)
	ON b.ProductGuid = a.ProductGuid
	LEFT OUTER JOIN map.tblEntityProductToSite c WITH (NOLOCK)
	ON c.ProductGuid = b._MasterRecordGuid
	WHERE 
	(
		(
			(c.SiteGuid = @LoginSiteGuid) 
			AND
			(b.ProductGuid <> b._MasterRecordGuid)
		)
		OR 
		(
			(b.SiteGuid = @SiteGuid)
			AND
			(b.ProductGuid = b._MasterRecordGuid)
		)
	)
	ORDER BY ProductID
	
	/*
	SELECT '00000000-0000-0000-0000-000000000000' AS ProductGuid, '<All>' AS ProductID, '<All>' AS Description
	WHERE @ShowAll = 1 
	UNION
	SELECT tblProducts.ProductGuid, tblProducts.ProductID, Description 
	FROM tblProducts (NoLock), 
	(
		SELECT map.tblEntityProductToSite.*,
		(
			SELECT SubTable.SiteGuid 
			FROM map.tblEntityProductToSite (NoLock) SubTable 
			WHERE SubTable.ProductGuid = map.tblEntityProductToSite.ProductGuid 
			AND SubTable.SiteGuid = @LoginSiteGuid
		) AS LoginSiteGuid
	 	FROM map.tblEntityProductToSite (NoLock) 
		WHERE SiteGuid = @SiteGuid 
	) tblEntities 
	WHERE tblEntities.ProductGuid = tblProducts.ProductGuid 
	AND (tblProducts.SiteGuid = @SiteGuid OR tblEntities.LoginSiteGuid = @LoginSiteGuid )
	ORDER BY ProductID
	*/