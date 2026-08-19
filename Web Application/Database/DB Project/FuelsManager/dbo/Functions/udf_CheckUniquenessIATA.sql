

CREATE FUNCTION [dbo].[udf_CheckUniquenessIATA]
(@IATAGuid uniqueidentifier, @SiteGuid uniqueidentifier, @IATAID nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblIATA
	IF 0 < (SELECT COUNT(*) FROM tblIATA e
	LEFT JOIN map.tblEntityIATACodeToSite em1 ON em1.IATAGuid = e.IATAGuid
	RIGHT JOIN map.tblEntityIATACodeToSite em2 ON em2.IATAGuid = @IATAGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.IATAGuid <> @IATAGuid
	AND IATAID = @IATAID)
		SET @Exists = 0

	RETURN @Exists
END
