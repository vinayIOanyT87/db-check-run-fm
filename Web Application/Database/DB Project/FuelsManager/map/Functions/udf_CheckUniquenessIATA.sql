

CREATE FUNCTION [map].[udf_CheckUniquenessIATA]
(@IATAGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @IATAID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @IATAID = (SELECT IATAID FROM tblIATA e WHERE e.IATAGuid = @IATAGuid)
	IF 0 < (SELECT COUNT(*) FROM tblIATA e 
	RIGHT JOIN map.tblEntityIATACodeToSite em ON em.SiteGuid = @SiteGuid AND em.IATAGuid = e.IATAGuid 
	WHERE e.IATAGuid <> @IATAGuid
	AND e.IATAID = @IATAID)
		SET @Exists = 0

	RETURN @Exists
END
