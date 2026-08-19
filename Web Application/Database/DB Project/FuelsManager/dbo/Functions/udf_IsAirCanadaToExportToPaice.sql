CREATE FUNCTION [dbo].[udf_IsAirCanadaToExportToPaice]
(
	 @TransID nvarchar (64)

)
RETURNS INT
AS
BEGIN
	DECLARE @RetVar int = 0;


	SELECT @RetVar = 1
	FROM tblTransactions
	WHERE TransID=@TransID
		AND OwnerCode IS NOT NULL
		AND ManagerID is NOT NULL
		AND OwnerCode <> ''
		AND ManagerID <> ''
		AND OwnerCode in ( 'AC','RS','ZX','ACRG')
		AND ManagerID in ('YH3 - Vopak','YQ2 - IMTT','YV4 - Westridge Pipeline','YV3 - Westridge');
	RETURN @RetVar

END
