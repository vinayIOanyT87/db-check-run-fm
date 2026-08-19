

CREATE FUNCTION [map].[udf_CheckUniquenessPerson]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @PersonID nvarchar(50)
			, @CardNumber nvarchar(30)
			, @ShortCardNumber nvarchar(6)
			, @Exists bit
	SET @Exists = 1

	SET @PersonID = (SELECT PersonID FROM tblPersonnel e WHERE e.PersonnelGuid = @_MasterRecordGuid)
	SET @CardNumber = (SELECT CardNumber FROM tblPersonnel e WHERE e.PersonnelGuid = @_MasterRecordGuid)
	SET @ShortCardNumber = (SELECT ShortCardNumber FROM tblPersonnel e WHERE e.PersonnelGuid = @_MasterRecordGuid)
	IF 0 < (SELECT COUNT(*) FROM tblPersonnel e 
	RIGHT JOIN map.tblEntityPersonnelToSite em ON em.SiteGuid = @SiteGuid AND em.PersonnelGuid = e._MasterRecordGuid 
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