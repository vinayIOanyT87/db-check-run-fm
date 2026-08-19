
CREATE PROCEDURE [dbo].[usp_PopulateAccountingDB]

AS
SET NOCOUNT ON

	DECLARE @timestamp DATETIMEOFFSET(7);
	SET @TimeStamp = SYSDATETIMEOFFSET();

	DECLARE @UserName DATETIMEOFFSET(7);
	SET @UserName = @UserName;

	DECLARE @Site nvarchar (30);
	SET @Site = 'SiteAdmin';


	/***** Populate Standard XML Import Plugin type *****/
	INSERT INTO dbo.tblImportExportPlugins (PluginType, ConfigURL, RunURL, Import, Export)
	  VALUES ('Standard XML', '../StandardXMLImportExport/Setup.aspx', '../StandardXMLImportExport/Run.aspx', 1, 1);