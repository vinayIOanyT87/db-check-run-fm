
CREATE VIEW [dbo].[vw_ProductGroupProducts]
AS
SELECT		 a.SiteGuid,
			 a.[id]										AS ProductGroupID,
			 a.ApplicationStringGuid					AS ProductGroupGuid,
			 p.ProductID,
			 pm.ProductGuid							AS ProductGuid
	  FROM dbo.tblApplicationString					AS a
	  INNER JOIN map.tblProductToProductGroup	pm ON a.ApplicationStringGuid = pm.AssignedToApplicationStringGuid
	  INNER JOIN dbo.tblProducts p ON pm.ProductGuid = p.productGuid