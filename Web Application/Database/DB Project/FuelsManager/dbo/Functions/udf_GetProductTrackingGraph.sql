
CREATE FUNCTION [dbo].[udf_GetProductTrackingGraph](
	@ProductGuid uniqueidentifier
)
RETURNS @tblTrackingGraph TABLE
(
	ProductGuid uniqueidentifier
)
AS
BEGIN
	-- =================================================================================
	-- Author:		George C. Peters Iv
	-- Create Date:	1/30/2013
	-- Description:	Given a ProductGuid, all associated Tracking Product Guid
	--				references are iterated until all unique Product Guids have been 
	--				identified.
	-- =================================================================================
	DECLARE @MatchFound int

	; WITH ProductTrackingMap AS (
		SELECT ProductGuid
				,TrackingProductGuid
		FROM dbo.tblProducts
		WHERE TrackingProductGuid IS NOT NULL 
			OR ProductGuid <> TrackingProductGuid
	), 
	Recursive_CTE AS (
		SELECT
				ProductGuid
				,TrackingProductGuid
				,CONVERT(varchar(3000), ProductGuid) AS PreviousGuids
			FROM ProductTrackingMap
			WHERE ProductGuid = @ProductGuid
		UNION ALL
		SELECT child.ProductGuid
				,child.TrackingProductGuid
				,CONVERT(varchar(3000), (parent.PreviousGuids + ',' + CONVERT(varchar(128), child.ProductGuid))) AS PreviousGuids
		FROM Recursive_CTE parent 
			INNER JOIN dbo.tblProducts child 
				ON child.ProductGuid = parent.TrackingProductGuid
					AND child.ProductGuid NOT IN (SELECT * FROM dbo.udf_SplitString(parent.PreviousGuids, ',', 0))
	)
	INSERT INTO @tblTrackingGraph
		SELECT ProductGuid
		FROM Recursive_CTE

	RETURN;
END