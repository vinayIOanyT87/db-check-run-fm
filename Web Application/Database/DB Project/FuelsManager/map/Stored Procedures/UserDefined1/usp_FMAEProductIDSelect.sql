

/*
=============================================
Author: Ryan Hill
Create date: 03/19/2013
Description:
	Select product ID translations defined for records
	imported through the FMAE interface
=============================================
*/
CREATE PROCEDURE [map].[usp_FMAEProductIDSelect]
(
	@FMAEProductID NVARCHAR(30) = NULL,
	@FMAEProductIDMapGuid UNIQUEIDENTIFIER = NULL,
	@FMAEProductIDSearchFilter NVARCHAR(25) = NULL
)
AS
BEGIN
	IF (@FMAEProductID IS NOT NULL)
	BEGIN
		SELECT
			map.tblFMAEProductID.FMAEProductIDMapGuid,
			map.tblFMAEProductID.FMAEProductID,
			map.tblFMAEProductID.ProductGuid,
			tblProducts.ProductID,
			map.tblFMAEProductID.CreatedDate, 
			map.tblFMAEProductID.CreatedBy,
			map.tblFMAEProductID.UpdatedDate, 
			map.tblFMAEProductID.UpdatedBy 	
		FROM map.tblFMAEProductID 
		INNER JOIN tblProducts ON map.tblFMAEProductID.ProductGuid = tblProducts.ProductGuid 
		WHERE map.tblFMAEProductID.FMAEProductID = @FMAEProductID
	END
	ELSE IF (@FMAEProductIDMapGuid IS NOT NULL)
	BEGIN
		SELECT
			map.tblFMAEProductID.FMAEProductIDMapGuid,
			map.tblFMAEProductID.FMAEProductID,
			map.tblFMAEProductID.ProductGuid,
			tblProducts.ProductID,
			map.tblFMAEProductID.CreatedDate, 
			map.tblFMAEProductID.CreatedBy,
			map.tblFMAEProductID.UpdatedDate, 
			map.tblFMAEProductID.UpdatedBy 	
		FROM map.tblFMAEProductID
		INNER JOIN tblProducts ON map.tblFMAEProductID.ProductGuid = tblProducts.ProductGuid 
		WHERE map.tblFMAEProductID.FMAEProductIDMapGuid = @FMAEProductIDMapGuid
	END
	ELSE
	BEGIN
		IF (@FMAEProductIDSearchFilter IS NOT NULL AND @FMAEProductIDSearchFilter != '')
		BEGIN
			SELECT
				map.tblFMAEProductID.FMAEProductIDMapGuid,
				map.tblFMAEProductID.FMAEProductID,
				map.tblFMAEProductID.ProductGuid,
				tblProducts.ProductID,
				map.tblFMAEProductID.CreatedDate, 
				map.tblFMAEProductID.CreatedBy,
				map.tblFMAEProductID.UpdatedDate, 
				map.tblFMAEProductID.UpdatedBy 	
			FROM map.tblFMAEProductID
			INNER JOIN tblProducts ON map.tblFMAEProductID.ProductGuid = tblProducts.ProductGuid 
			WHERE map.tblFMAEProductID.FMAEProductID LIKE ('%' + @FMAEProductIDSearchFilter + '%')
		END
		ELSE
		BEGIN
			SELECT
				map.tblFMAEProductID.FMAEProductIDMapGuid,
				map.tblFMAEProductID.FMAEProductID,
				map.tblFMAEProductID.ProductGuid,
				tblProducts.ProductID,
				map.tblFMAEProductID.CreatedDate, 
				map.tblFMAEProductID.CreatedBy,
				map.tblFMAEProductID.UpdatedDate, 
				map.tblFMAEProductID.UpdatedBy 	
			FROM map.tblFMAEProductID
			INNER JOIN tblProducts ON map.tblFMAEProductID.ProductGuid = tblProducts.ProductGuid 
		END
	END
	
END

