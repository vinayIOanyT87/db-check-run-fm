/*
DROP PROCEDURE [dbo].[usp_GetPersonnelEnumerateBasic]

	EXEC [dbo].[usp_GetPersonnelEnumerateBasic] 'AD74B677-F294-4BF8-8861-30D6B424ADC6'

*/
CREATE PROCEDURE [dbo].[usp_GetPersonnelEnumerateBasic]
(
	@SiteGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetPersonnelEnumerateBasic] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Retrieve all Personnel records that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. SiteGuid: Limit results to Personnel that have been assigned to this site/sitegroup only
	-- 2. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	-- 3. The SiteGuid of the master record is included in the resultset to support the decryption of the Personnel.PINNumber in child record versions.
	------------------------------------------------------------------------------------------------------


	SET NOCOUNT ON

	SELECT b.PersonnelGuid, 
		b._MasterRecordGuid, 
		b.SiteGuid,
		b.PersonID,
		a.MasterSiteGuid
	FROM [erv].[udf_GetPersonnelRecordVersions] (@SiteGuid) a 
	INNER JOIN tblPersonnel b ON b.PersonnelGuid = a.PersonnelGuid			
		
END
