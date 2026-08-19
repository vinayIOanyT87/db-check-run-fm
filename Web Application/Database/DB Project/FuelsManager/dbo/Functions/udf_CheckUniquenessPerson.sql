

CREATE FUNCTION [dbo].[udf_CheckUniquenessPerson]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier, @PersonID nvarchar(50), @CardNumber nvarchar(30), @ShortCardNumber nvarchar(6))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblPerson
	IF 0 < (SELECT COUNT(*) FROM tblPersonnel e
	LEFT JOIN map.tblEntityPersonnelToSite em1 ON em1.PersonnelGuid = e._MasterRecordGuid
	RIGHT JOIN map.tblEntityPersonnelToSite em2 ON em2.PersonnelGuid = @_MasterRecordGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND (PersonID = @PersonID
	OR (ISNULL(CardNumber,'') <> ''
	AND ISNULL(@CardNumber,'') <> ''
	AND CardNumber = @CardNumber)
	OR (ISNULL(ShortCardNumber,'') <> ''
	AND ISNULL(@ShortCardNumber,'') <> ''
	AND ShortCardNumber = @ShortCardNumber)))
		SET @Exists = 0

	RETURN @Exists
END

