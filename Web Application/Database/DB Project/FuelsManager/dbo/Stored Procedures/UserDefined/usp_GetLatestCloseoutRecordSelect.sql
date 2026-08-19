


CREATE PROCEDURE [dbo].[usp_GetLatestCloseoutRecordSelect]
@SiteGuid UNIQUEIDENTIFIER, @CloseoutDate DATE, @ManagerCompanyGuid UNIQUEIDENTIFIER, @ProductGuid UNIQUEIDENTIFIER
AS 
BEGIN
	SET NOCOUNT ON

	declare @CloseoutDateVar		date
    declare @ProductGuidVar			uniqueidentifier 
    declare @SiteGuidVar            uniqueidentifier
    declare @ManagerCompanyGuidVar	uniqueidentifier
   
    Set @CloseoutDateVar = @CloseoutDate;
    Set @SiteGuidVar = @SiteGuid;
    Set @ManagerCompanyGuidVar = @ManagerCompanyGuid;
    Set @ProductGuidVar = @ProductGuid;
   
	SELECT TOP 1 CloseoutDate
	FROM tblCloseoutInventory c WITH(NOLOCK)
	WHERE c.SiteGuid     = @SiteGuidVar
	AND c.CloseoutDate < @CloseoutDateVar
	AND c.ManagerCompanyGuid  = @ManagerCompanyGuidVar
	AND c.ProductGuid  = @ProductGuidVar
	ORDER BY CloseoutDate DESC
END