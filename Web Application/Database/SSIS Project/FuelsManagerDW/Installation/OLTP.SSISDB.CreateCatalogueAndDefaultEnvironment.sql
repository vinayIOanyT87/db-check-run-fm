DECLARE 	
    @folderId bigint,
    @environmentId bigint
 
--Drop Catalogue and Project
IF EXISTS(SELECT 1 FROM [SSISDB].[catalog].[folders] WHERE name = N'FuelsManagerDWSSISCatalogueProject')
BEGIN
       IF EXISTS(SELECT 1 FROM [SSISDB].[catalog].[projects] WHERE folder_id = (SELECT TOP 1 folder_id FROM [SSISDB].[catalog].[folders] WHERE name = N'FuelsManagerDWSSISCatalogueProject'))
       BEGIN
              EXEC [SSISDB].[catalog].[delete_project] @project_name=N'FuelsManagerDWSSIS', @folder_name=N'FuelsManagerDWSSISCatalogueProject'
       END

       EXEC [SSISDB].[catalog].[delete_folder] @folder_name=N'FuelsManagerDWSSISCatalogueProject'
END
--Re-create Catalogue. The Project itself will be created through the SSIS project deployment.
EXEC [SSISDB].[catalog].[create_folder] @folder_name=N'FuelsManagerDWSSISCatalogueProject', @folder_id=@folderId OUTPUT 
EXEC [SSISDB].[catalog].[set_folder_description] @folder_name=N'FuelsManagerDWSSISCatalogueProject', @folder_description=N'FuelsManager  DW SSIS Catalogue Project'
 
--Create the target Environment
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environments] WHERE folder_id = @folderId AND name = N'DefaultEnvironment')
    EXEC [SSISDB].[catalog].[create_environment] @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject'
 
--Create the Environment Variables for the target Environment
SET @environmentId = (SELECT environment_id FROM [SSISDB].[catalog].[environments] WHERE folder_id = @folderId and name = N'DefaultEnvironment')
 
DECLARE @var sql_variant
DECLARE @varDT DateTime
DECLARE @varInt Int
DECLARE @varBit bit
SET @var = N'Data Source=GHP9XV1\SOLEIL;Initial Catalog=FuelsManagerDWDB;Integrated Security=True;'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'CMAudit')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'CMAudit', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'Data Source=GHP9XV1\SOLEIL;Initial Catalog=FuelsManagerDWDB;Provider=SQLNCLI11.1;Integrated Security=SSPI;Auto Translate=False;'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'CMDestination')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'CMDestination', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'

SET @var = N'Data Source=GHP9XV1\SOLEIL;Initial Catalog=FuelsManagerDWSSAS;Provider=MSOLAP.8;Integrated Security=SSPI;Impersonation Level=Impersonate;'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'CMFMDWSSAS')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'CMFMDWSSAS', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'Data Source=GHP9XV1\SOLEIL;Initial Catalog=FuelsManagerDB;Provider=SQLNCLI11.1;Integrated Security=SSPI;Auto Translate=False;'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'CMSource')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'CMSource', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'Data Source=GHP9XV1\SOLEIL;Initial Catalog=FuelsManagerDWDB;Provider=SQLNCLI11.1;Integrated Security=SSPI;Auto Translate=False;'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'CMStaging')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'CMStaging', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'ProcessCubeNone'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'ProcessCubeMode')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'ProcessCubeMode', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @varDT = N'Jan  1 1900 12:00AM'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'ProcessRunDate')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'ProcessRunDate', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varDT, @data_type=N'DateTime'
 
SET @varInt = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'TransactionBatchExtractionSize')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'TransactionBatchExtractionSize', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varInt, @data_type=N'Int32'
 
SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailEnableSSL')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailEnableSSL', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'
 
SET @var = N''
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailPassword')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailPassword', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N''
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailSendCC')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailSendCC', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'Hansraj.Bapoo@varec.com'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailSendFrom')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailSendFrom', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N''
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailSendFromName')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailSendFromName', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'Hansraj.Bapoo@varec.com'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailSendTo')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailSendTo', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'smtp.leidos.com'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailServer')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailServer', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N'25'
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailServerPort')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailServerPort', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @var = N''
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'EmailUsername')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'EmailUsername', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@var, @data_type=N'String'
 
SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'RebuildIndexes')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'RebuildIndexes', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'

SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'RebuildIndexesOnAllPartitions')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'RebuildIndexesOnAllPartitions', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'

SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'ResetHistoricalStartDates')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'ResetHistoricalStartDates', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'

SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'SkipEntitySourceExtraction')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'SkipEntitySourceExtraction', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'

SET @varBit = 0
IF NOT EXISTS (SELECT 1 FROM [SSISDB].[catalog].[environment_variables] WHERE environment_id = @environmentId AND name = N'ExtractByInventoryDate')
			EXEC [SSISDB].[catalog].[create_environment_variable] @variable_name=N'ExtractByInventoryDate', @sensitive=0, @description=N'', @environment_name=N'DefaultEnvironment', @folder_name=N'FuelsManagerDWSSISCatalogueProject', @value=@varBit, @data_type=N'Boolean'
