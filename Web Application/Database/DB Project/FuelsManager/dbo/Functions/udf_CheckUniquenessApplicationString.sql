

CREATE FUNCTION [dbo].[udf_CheckUniquenessApplicationString]
(@ApplicationStringGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(250), @LookupApplicationStringTypeIndex int)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	IF @LookupApplicationStringTypeIndex = 0
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityDotHazardousMessagesToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityDotHazardousMessagesToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 1
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityProductMessageToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityProductMessageToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 2
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityAllocationGroupToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityAllocationGroupToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 3
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityProductGroupToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityProductGroupToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 4
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityCompanyTypeToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityCompanyTypeToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 6
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityAlarmAndEventCategoryToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityAlarmAndEventCategoryToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 7
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityEmailAddressToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityEmailAddressToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 8
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityCompanyGroupToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityCompanyGroupToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 9
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityEntryMessageToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityEntryMessageToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 10
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityExitMessageToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityExitMessageToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 11
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityProcessVariableMessageToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityProcessVariableMessageToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 12
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityFootNoteToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityFootNoteToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 15
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityPointTemplateTypeToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityPointTemplateTypeToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END

	ELSE IF @LookupApplicationStringTypeIndex = 17
	BEGIN
		IF 0 < (SELECT COUNT(*) FROM [dbo].[tblApplicationString] e
		LEFT JOIN map.tblEntityPointCategoryToSite em1 ON em1.ApplicationStringGuid = e.ApplicationStringGuid
		RIGHT JOIN map.tblEntityPointCategoryToSite em2 ON em2.ApplicationStringGuid = @ApplicationStringGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE e.ApplicationStringGuid <> @ApplicationStringGuid
		AND ID = @ID
		AND LookupApplicationStringTypeIndex = @LookupApplicationStringTypeIndex)
			SET @Exists = 0
	END



	RETURN @Exists
END
