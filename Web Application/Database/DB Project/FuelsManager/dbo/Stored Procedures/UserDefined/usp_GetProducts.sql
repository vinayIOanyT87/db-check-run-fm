
CREATE PROCEDURE [dbo].[usp_GetProducts]
(
	@TargetSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetProducts] 
	-- Author: Brian Main
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Product records that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Products that have been assigned to this site/sitegroup only
	-- 2. This stored procedure replaces the ProductClass.EnumerateSQL inline SQL.
	------------------------------------------------------------------------------------------------------
		
BEGIN TRY
IF(@TargetSiteGuid IS NOT NULL)
Begin
SELECT 
	A.*,
	B.ProductID 
FROM tblProducts A WITH(NOLOCK) 
INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) RP 
ON A.ProductGuid = RP.ProductGuid
Left Join 
( 
	select p._MasterRecordGuid, p.ProductID from tblProducts p WITH(NOLOCK) 
	inner join [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) rp 
	on p.ProductGuid = rp.ProductGuid
) B 
ON A.TrackingProductGuid = B._MasterRecordGuid
ORDER BY A.ProductID
End
Else
Begin
SELECT 
	A.*,
	B.ProductID 
FROM tblProducts A WITH(NOLOCK)
Left Join tblProducts B WITH(NOLOCK)
ON A.TrackingProductGuid = B._MasterRecordGuid
ORDER BY A.ProductID
End
END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_GetProducts' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    

	END