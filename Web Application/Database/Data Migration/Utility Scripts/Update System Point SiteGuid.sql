-- Script to update System point to Siteguid

USE FuelsManagerDB
GO

DECLARE @SiteGuid AS UNIQUEIDENTIFIER
DECLARE @ApplicationStringGuid AS UNIQUEIDENTIFIER

SET @SiteGuid = (
		SELECT SiteGuid
		FROM tblApplicationString
		WHERE ApplicationStringGuid = 'E78CD406-4C19-4978-8940-FA4E404E3E53' -- Tank
		)
SET @ApplicationStringGuid = '2DDEB3E0-545C-444B-B1BF-9CAB048F21B7' -- System

UPDATE tblApplicationString
SET SiteGuid = @SiteGuid
WHERE ApplicationStringGuid = @ApplicationStringGuid

UPDATE map.tblEntityPointTemplateTypeToSite
SET SiteGuid = @SiteGuid
WHERE ApplicationStringGuid = @ApplicationStringGuid
