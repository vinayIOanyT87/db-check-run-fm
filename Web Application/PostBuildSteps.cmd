@IF "%1"=="" GOTO :ERROR_NO_SOURCE
@IF "%2"=="" GOTO :ERROR_NO_DEST
@IF "%3"=="" GOTO :ERROR_NO_MODE

REM Thanks to our use of spaces in our folder structures which always throws crap off, apparently the XCOPY /EXCLUDE:<file> option can't handle spaces in the path such as "Web Application"
REM Solution, attempt to copy the exclusion files to the agent's root working folder which doesn't have spaces.
COPY "%1\Web Application\xcopy*.*" .\ /Y

ECHO "Removing existing folders"
for %%d in (%2) do rmdir "%%~d" /s /q

ECHO "Copy Shared Binaries"
MKDIR "%2\ObfuscatedWebRole"
XCOPY "%1\Web Application\Binaries" "%2\SupportBinaries" /E /R /Y /I /Q

ECHO "Copy Build Output to Binaries Folder"
ECHO "Web Apps"
ECHO "XCOPY FuelsManager"
MKDIR "%2\ObfuscatedWebRole\FuelsManager"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FuelsManager" "%2\ObfuscatedWebRole\FuelsManager" /E /R /Y /I /Q
ECHO "XCOPY FuelsManager web.config"
XCOPY "%1\Web Application\FuelsManager\web.config" "%2\ObfuscatedWebRole\FuelsManager" /E /R /Y /I /Q

ECHO "XCOPY OPCWeb"
MKDIR "%2\ObfuscatedWebRole\OPCWebApp"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\OPC\OPCWebApp" "%2\ObfuscatedWebRole\OPCWebApp" /E /R /Y /I /Q

ECHO "Web Services"
ECHO "FMBusinessServices Sequence"
MKDIR "%2\ObfuscatedWebRole\FMBusinessServices"

ECHO "XCOPY FMBusinessService Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMBusinessServices" "%2\ObfuscatedWebRole\FMBusinessServices" /E /R /Y /I /Q
ECHO "XCOPY FMBusinessServices web.config"
XCOPY "%1\Web Application\FMBusinessServices\web.config" "%2\ObfuscatedWebRole\FMBusinessServices" /E /R /Y /I /Q
ECHO "XCOPY ServiceHostingEnvironment.config"
XCOPY "%1\Web Application\FMBusinessServices\ServiceHostingEnvironment.config" "%2\ObfuscatedWebRole\FMBusinessServices" /E /R /Y /I /Q

ECHO "AccountingImportExport Sequence"
MKDIR "%2\ObfuscatedWebRole\AccountingImportExport"
ECHO "XCOPY AccountingImportExport"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\AccountingImportExport" "%2\ObfuscatedWebRole\AccountingImportExport" /E /R /Y /I /Q

ECHO "FMDataExchange Sequence"
MKDIR "%2\ObfuscatedWebRole\FMDataExchange"
ECHO "XCOPY FMDataExchange"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMDataExchange" "%2\ObfuscatedWebRole\FMDataExchange" /E /R /Y /I /Q

ECHO "FMDispatch Proxy Sequence"
MKDIR "%2\ObfuscatedWebRole\FMDispatchProxyServices"
ECHO "XCOPY FMDispatchProxyServices Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMDispatchProxyServices" "%2\ObfuscatedWebRole\FMDispatchProxyServices" /E /R /Y /I /Q
ECHO "XCOPY FMDispatchProxyServices web.config"
XCOPY "%1\Web Application\FMDispatchProxyServices\web.config" "%2\ObfuscatedWebRole\FMDispatchProxyServices" /E /R /Y /I /Q
ECHO "XCOPY ServiceHostingEnvironment.config"
XCOPY "%1\Web Application\FMDispatchProxyServices\ServiceHostingEnvironment.config" "%2\ObfuscatedWebRole\FMDispatchProxyServices" /E /R /Y /I /Q

ECHO "FMWebAPI Sequence"
MKDIR "%2\ObfuscatedWebRole\FMWebAPI"
ECHO "XCOPY FMWebAPI Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMWebAPI" "%2\ObfuscatedWebRole\FMWebAPI" /E /R /Y /I /Q
ECHO "XCOPY FMWebAPI web.config"
XCOPY "%1\Web Application\FMWebAPI\web.config" "%2\ObfuscatedWebRole\FMWebAPI" /E /R /Y /I /Q

ECHO "FMErrorNotificationWebService Sequence"
MKDIR "%2\ObfuscatedWebRole\ErrorNotificationWebService"
ECHO "XCOPY ErrorNotificationWebService Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMErrorNotifications\ErrorNotificationWebService" "%2\ObfuscatedWebRole\ErrorNotificationWebService" /E /R /Y /I /Q
ECHO "XCOPY ErrorNotificationWebService web.config"
XCOPY "%1\Web Application\FMErrorNotifications\ErrorNotificationWebService\web.config" "%2\ObfuscatedWebRole\ErrorNotificationWebService" /E /R /Y /I /Q

ECHO "FMEnterpriseManagementProxyServices Sequence"
MKDIR "%2\ObfuscatedWebRole\FMEnterpriseManagementProxyServices"
ECHO "XCOPY FMEnterpriseManagementProxyServices Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMEnterpriseManagementProxyServices" "%2\ObfuscatedWebRole\FMEnterpriseManagementProxyServices" /E /R /Y /I /Q
ECHO "XCOPY FMEnterpriseManagementProxyServices web.config"
XCOPY "%1\Web Application\FMEnterpriseManagementProxyServices\web.config" "%2\ObfuscatedWebRole\FMEnterpriseManagementProxyServices" /E /R /Y /I /Q
ECHO "XCOPY ServiceHostingEnvironment.config"
XCOPY "%1\Web Application\FMEnterpriseManagementProxyServices\ServiceHostingEnvironment.config" "%2\ObfuscatedWebRole\FMEnterpriseManagementProxyServices" /E /R /Y /I /Q

ECHO "FMPointGroupReport Web Service Sequence"
MKDIR "%2\ObfuscatedWebRole\FMPointGroupReport"
ECHO "XCOPY FMPointGroupReport Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMPointGroupReport" "%2\ObfuscatedWebRole\FMPointGroupReport" /E /R /Y /I /Q
ECHO "XCOPY FMEnterpriseManagementProxyServices web.config"
XCOPY "%1\Web Application\FMPointGroupReport\web.config" "%2\ObfuscatedWebRole\FMPointGroupReport" /E /R /Y /I /Q


ECHO "Windows Services"
ECHO "FuelsManager Service Components"
ECHO "CreateDirectory (ObfuscatedFuelsManagerService)"
MKDIR "%2\ObfuscatedFuelsManagerService"
ECHO "XCOPY FuelsManagerService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FuelsManagerService\bin" "%2\ObfuscatedFuelsManagerService\bin" /E /R /Y /I /Q

ECHO "Synchronization Service Components"
ECHO "CreateDirectory (ObfuscatedFMSynchronizationService)"
MKDIR "%2\ObfuscatedFMSynchronizationService"
ECHO "XCOPY FMSynchronizationService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMSynchronizationService\bin" "%2\ObfuscatedFMSynchronizationService\bin" /E /R /Y /I /Q

ECHO "Point Server Components"
ECHO "CreateDirectory (ObfuscatedFuelsManagerPointService)"
MKDIR "%2\ObfuscatedFuelsManagerPointService"
ECHO "XCOPY FuelsManagerPointService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMPointService\bin" "%2\ObfuscatedFuelsManagerPointService\bin" /E /R /Y /I /Q

ECHO "FMOpcUaServerService Service Components"
ECHO "CreateDirectory (ObfuscatedFMOpcUaServerService)"
MKDIR "%2\ObfuscatedFMOpcUaServerService"
ECHO "XCOPY FMOpcUaServerService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMOpcUaServerService\bin" "%2\ObfuscatedFMOpcUaServerService\bin" /E /R /Y /I /Q

ECHO "FMExport Service Components"
ECHO "CreateDirectory (ObfuscatedFMExportService)"
MKDIR "%2\ObfuscatedFMExportService"
ECHO "XCOPY FMExport"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMExport\Binaries" "%2\ObfuscatedFMExportService\bin" /E /R /Y /I /Q

ECHO "FMExport Service Configuration"
ECHO "CreateDirectory (ObfuscatedFMExportServiceConfiguration)"
MKDIR "%2\ObfuscatedFMExportServiceConfiguration"
ECHO "XCOPY FMExport Service Configuration"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMExport\Binaries" "%2\ObfuscatedFMExportServiceConfiguration\bin" /E /R /Y /I /Q

ECHO "FMBackup Utility Components"
ECHO "CreateDirectory (ObfuscatedFMBackupUtility)"
MKDIR "%2\ObfuscatedFMBackupUtility"
ECHO "CreateDirectory (FMBackupUtility)"
MKDIR "%2\ObfuscatedFMBackupUtility\FMBackupUtility"
ECHO "CreateDirectory (FMBackupUtility)"
MKDIR "%2\ObfuscatedFMBackupUtility\FMBackupUtility\bin"
ECHO "CreateDirectory (FMBackupUtilityConfiguration)"
MKDIR "%2\ObfuscatedFMBackupUtility\FMBackupUtilityConfiguration"
ECHO "CreateDirectory (FMBackupUtilityConfiguration)"
MKDIR "%2\ObfuscatedFMBackupUtility\FMBackupUtilityConfiguration\bin"

ECHO "XCOPY FMBackupUtility"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMBackupUtility\FMBackupUtility\bin\Release" "%2\ObfuscatedFMBackupUtility\FMBackupUtility\bin" /E /R /Y /I /Q
ECHO "XCOPY FMBackupUtilityConfiguration"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMBackupUtility\FMBackupUtilityConfiguration\bin\Release" "%2\ObfuscatedFMBackupUtility\FMBackupUtilityConfiguration\bin" /E /R /Y /I /Q
ECHO "XCOPY Help File"
XCOPY "%1\Web Application\FMBackupUtility\Binary\*.chm" "%2\ObfuscatedFMBackupUtility\" /E /R /Y /I /Q

ECHO "Load Rack Components"
ECHO "CreateDirectory (ObfLoadRack)"
MKDIR "%2\ObfuscatedLoadRackService"
ECHO "XCOPY LoadRackService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\LoadRackComponents\LoadRackService\bin\%3" "%2\ObfuscatedLoadRackService\bin" /E /R /Y /I /Q

ECHO "Iridium Gss Service"
ECHO "CreateDirectory (ObfuscatedFMIridiumGssService)"
MKDIR "%2\ObfuscatedFMIridiumGssService"
ECHO "XCOPY Iridium Gss Service"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMIridiumGssService\bin\%3" "%2\ObfuscatedFMIridiumGssService\bin" /E /R /Y /I /Q

ECHO "OPC Server Components"
ECHO "CreateDirectory (ObfOPCComponents)"
MKDIR "%2\ObfuscatedOPCComponents"
ECHO "Accuload OPC"
ECHO "CreateDirectory (Accuload)"
MKDIR "%2\ObfuscatedOPCComponents\AcculoadOPC"
ECHO "XCOPY AcculoadOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\AcculoadOPC\AcculoadOPCObjects\%3" "%2\ObfuscatedOPCComponents\AcculoadOPC" /E /R /Y /I /Q
ECHO "XCOPY AcculoadOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\AcculoadOPC\AcculoadOPCServer\%3" "%2\ObfuscatedOPCComponents\AcculoadOPC" /E /R /Y /I /Q

ECHO "Optomux OPC"
ECHO "CreateDirectory (Optomux)"
MKDIR "%2\ObfuscatedOPCComponents\OptomuxOPC"
ECHO "XCOPY OptomuxOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\OptomuxOPC\OptomuxOPCObjects\%3" "%2\ObfuscatedOPCComponents\OptomuxOPC" /E /R /Y /I /Q
ECHO "XCOPY OptomuxOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\OptomuxOPC\OptomuxOPCServer\%3" "%2\ObfuscatedOPCComponents\OptomuxOPC" /E /R /Y /I /Q

ECHO "Osdp OPC"
ECHO "CreateDirectory (Osdp)"
MKDIR "%2\ObfuscatedOPCComponents\OsdpOPC"
ECHO "XCOPY OsdpOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\OsdpOPC\OsdpOPCObjects\%3" "%2\ObfuscatedOPCComponents\OsdpOPC" /E /R /Y /I /Q
ECHO "XCOPY OsdpOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\OsdpOPC\OsdpOPCServer\%3" "%2\ObfuscatedOPCComponents\OsdpOPC" /E /R /Y /I /Q

ECHO "Hid OPC"
ECHO "CreateDirectory (Hid)"
MKDIR "%2\ObfuscatedOPCComponents\HidOPC"
ECHO "XCOPY HidOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\HidOPC\HidOPCObjects\%3" "%2\ObfuscatedOPCComponents\HidOPC" /E /R /Y /I /Q
ECHO "XCOPY HidOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\HidOPC\HidOPCServer\%3" "%2\ObfuscatedOPCComponents\HidOPC" /E /R /Y /I /Q

ECHO "Scully OPC"
ECHO "CreateDirectory (Scully)"
MKDIR "%2\ObfuscatedOPCComponents\ScullyOPC"
ECHO "XCOPY ScullyOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\ScullyOPC\ScullyOPCObjects\%3" "%2\ObfuscatedOPCComponents\ScullyOPC" /E /R /Y /I /Q
ECHO "XCOPY ScullyOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\ScullyOPC\ScullyOPCServer\%3" "%2\ObfuscatedOPCComponents\ScullyOPC" /E /R /Y /I /Q

ECHO "WeightScale OPC"
ECHO "CreateDirectory (WeightScale)"
MKDIR "%2\ObfuscatedOPCComponents\WeightScaleOPC"
ECHO "XCOPY WeightScaleOPCObjects"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\WeightScaleOPC\WeightScaleOPCObjects\%3" "%2\ObfuscatedOPCComponents\WeightScaleOPC" /E /R /Y /I /Q
ECHO "XCOPY WeightScaleOPCServer"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\WeightScaleOPC\WeightScaleOPCServer\%3" "%2\ObfuscatedOPCComponents\WeightScaleOPC" /E /R /Y /I /Q

ECHO "Dispatch Components"
ECHO "CreateDirectory (ObfDispatch)"
MKDIR "%2\ObfuscatedDispatchDesktopClient"
ECHO "XCOPY Dispatch Client"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\Dispatch Desktop\Clent\bin" "%2\ObfuscatedDispatchDesktopClient\bin" /E /R /Y /I /Q

ECHO "FMNotificationService Service Components"
ECHO "CreateDirectory (ObfuscatedFMNotificationService)"
MKDIR "%2\ObfuscatedFMNotificationService"
ECHO "CreateDirectory (ObfuscatedFMNotificationService\bin)"
MKDIR "%2\ObfuscatedFMNotificationService\bin"
ECHO "XCOPY FMNotificationService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMErrorNotifications\NotificationService\bin" "%2\ObfuscatedFMNotificationService\bin" /E /R /Y /I /Q

ECHO "FM Active Directory Manage Service"
ECHO "CreateDirectory (ObfFMActiveDirectoryManageService)"
MKDIR "%2\ObfuscatedFMActiveDirectoryManageService"
ECHO "XCOPY FMActiveDirectoryManageService"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FMActiveDirectoryManageService\bin\%3" "%2\ObfuscatedFMActiveDirectoryManageService\bin" /E /R /Y /I /Q

ECHO "Create FMTransactionExportService Drop Folder"
ECHO "MD FMTransactionExportService"
MKDIR "%2\FMTransactionExportService"
ECHO "XCOPY FMTransactionExportService"
XCOPY "%1\Web Application\FMTransactionExportService\bin\Release\*.*" "%2\FMTransactionExportService" /E /R /Y /I /Q


ECHO "Tools"
ECHO "Test Tools"

ECHO "CreateDirectory (TestTools)"
MKDIR "%2\TestTools"

ECHO "XCOPY All Test Tools"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\Test Tools" "%2\TestTools\" /E /R /Y /I /Q

ECHO "Utilities"

ECHO "CreateDirectory (Utilities)"
MKDIR "%2\Utilities"

ECHO "XCOPY FMToLegacyInterface"
XCOPY "%1\Web Application\Utilities\FMToLegacyInterface\Release" "%2\Utilities\" /E /R /Y /I /Q

ECHO "Graphics"

ECHO "CreateDirectory (Graphics)"
MKDIR "%2\Graphics"

ECHO "XCOPY Graphics"
XCOPY "%1\Web Application\Graphics" "%2\Graphics\" /E /R /Y /I /Q

ECHO "Automated Fuel Service Station"
ECHO "MVC Web App Extension"

ECHO "CreateObfuscatedSubDirectory"
MKDIR "%2\ObfuscatedWebRole\AFSSWebApp"

ECHO "CreateBINfolder"
MKDIR "%2\ObfuscatedWebRole\AFSSWebApp\bin"
ECHO "XCOPY Assemblies"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\Extension.AFSS\Afss.WebApp\bin\*.*" "%2\ObfuscatedWebRole\AFSSWebApp\bin" /E /R /Y /I /Q

ECHO "CreateObfuscatedSubDirectory"
MKDIR "%2\ObfuscatedWebRole\AFSSWebApp\Areas"

ECHO "XCOPY MVC Areas"
XCOPY /EXCLUDE:"%1\xcopydeployexclusions_rice_mvcwebapp.txt" "%1\Web Application\Extension.AFSS\Afss.WebApp\Areas" "%2\ObfuscatedWebRole\AFSSWebApp\Areas" /E /R /Y /I /Q

ECHO "Web Services"
ECHO "External Fuel Station Service"
ECHO "CreateObfuscatedSubDirectory"
MKDIR "%2\ObfuscatedWebRole\GasboyBusinessServices"

ECHO "XCOPY Gasboy.BusinessServices Files"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\Extension.AFSS\Module.Gasboy\Gasboy.BusinessServices" "%2\ObfuscatedWebRole\GasboyBusinessServices" /E /R /Y /I /Q

ECHO "XCOPY Gasboy.BusinessServices web.config"
XCOPY "%1\Web Application\Extension.AFSS\Module.Gasboy\Gasboy.BusinessServices\web.config" "%2\ObfuscatedWebRole\GasboyBusinessServices" /E /R /Y /I /Q

ECHO "XCOPY ServiceHostingEnvironment.config"
XCOPY %1\Sources\Web Application\Extension.AFSS\Module.Gasboy\Gasboy.BusinessServices\ServiceHostingEnvironment.config" "%2\ObfuscatedWebRole\GasboyBusinessServices" /E /R /Y /I /Q

ECHO "SiteOmat web Service"
ECHO "XCOPY web config"
XCOPY "%1\Web Application\Extension.AFSS\AFSS.WebApp\web.config" "%2\ObfuscatedWebRole\AFSSWebApp" /E /R /Y /I /Q
ECHO "XCOPY SiteOmat ASMX"
XCOPY "%1\Web Application\Extension.AFSS\AFSS.WebApp\SiteOmatService.asmx" "%2\ObfuscatedWebRole\AFSSWebApp" /E /R /Y /I /Q


ECHO "Windows Services"
ECHO "Automated Fuel Service Station Service Components"
ECHO "CreateDirectory (ObfuscatedFMAFSSService)"
MKDIR "%2\ObfuscatedFMAFSSService"
ECHO "XCOPY Automated Fuel Service Station Service"
XCOPY /EXCLUDE:"%1\xcopydeployexclusions_service_process.txt" "%1\Web Application\Extension.AFSS\Afss.ServiceProcess\bin" "%2\ObfuscatedFMAFSSService\bin" /E /R /Y /I /Q

ECHO "Create FMLegacyMovementService Drop Folder"
ECHO "MD FMLegacyMovementService"
MKDIR "%2\FMLegacyMovementService"

ECHO "XCOPY FMLegacyMovementService"
XCOPY "%1\Web Application\FMLegacyMovementService\bin\Release\*.*" "%2\FMLegacyMovementService" /E /R /Y /I /Q

ECHO "Varec FCEE Service"
ECHO "CreateDirectory (ObfuscatedVarecFCEEService)"
MKDIR "%2\ObfuscatedVarecFCEEService"
ECHO "XCOPY Varec FCEE Service"
XCOPY /EXCLUDE:xcopydeploymentexclusions.txt "%1\Web Application\FCEEService\bin" "%2\ObfuscatedVarecFCEEService\bin" /E /R /Y /I /Q

************* OBFUSCATION PROCESS ****************  REPLACES THE PREVIOUSLY COPIED ASSEMBLIES IN THE BINARIES DROP FOLDER ********************
ECHO "Obfuscation Sequence"

ECHO "Perform Dotfuscation"
ECHO "Dotfuscate FMWebRoleFuelsManager" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FuelsManager\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\FuelsManager,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMWebRoleFuelsManager_v2010.xml
ECHO "Dotfuscate FMBusinessServices" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMBusinessServices\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\FMBusinessServices,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMWebRoleFMBusinessServices_v2010.xml
ECHO "Dotfuscator AccountingImportExport" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\AccountingImportExport\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\AccountingImportExport\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMWebRoleAccountingImportExport_v2010.xml
ECHO "Dotfuscate FMDataExchange" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMDataExchange\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\FMDataExchange\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMDataExchange_v2010.xml
ECHO "Dotfuscate FMSynchronizationService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMSynchronizationService\bin,ObfuscationOutputPath=%2\ObfuscatedFMSynchronizationService\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMSynchronizationService_v2010.xml
ECHO "Dotfuscate FuelsManagerService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FuelsManagerService\bin\Release,ObfuscationOutputPath=%2\ObfuscatedFuelsManagerService\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFuelsManagerService_v2010.xml
ECHO "Dotfuscate FMPointService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMPointService\bin,ObfuscationOutputPath=%2\ObfuscatedFuelsManagerPointService,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMPointService_v2010.xml
ECHO "Dotfuscate FMPointGroupReportService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMPointGroupReport\bin,ObfuscationOutputPath=%2\ObfuscatedFMPointGroupReportService,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMPointService_v2010.xml
ECHO "Dotfuscate FMExportService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMExport\FMExportService\bin\%3,ObfuscationOutputPath=%2\ObfuscatedFMExportService,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMExportService_v2010.xml
ECHO "Dotfuscate Dispatch Desktop" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\Dispatch Desktop\Clent\bin,ObfuscationOutputPath=%2\ObfuscatedDispatchDesktopClient,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationDispatchDesktopClient_v2010.xml
ECHO "Dotfuscator Azure FuelsManagerServiceWorkerRole" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FuelsManagerServiceWorkerRole\bin\%3,ObfuscationOutputPath=%2\ObfuscatedFuelsManagerServiceWorkerRole,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFuelsManagerServiceWorkerRole_v2010.xml
ECHO "Dotfuscate Azure FMExportServiceWorkerRole" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMExport\FMExportServiceWorkerRole\bin\%3,ObfuscationOutputPath=%2\ObfuscatedFMExportServiceWorkerRole,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMExportServiceWorkerRole_v2010.xml
ECHO "Dotfuscate FMAFSSService" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\Extension.AFSS\Afss.ServiceProcess\bin,ObfuscationOutputPath=%2\ObfuscatedFMAFSSService\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate FMBackupUtility" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMBackupUtility\FMBackupUtility\bin\Release,ObfuscationOutputPath=%2\ObfuscatedFMBackupUtility\FMBackupUtility\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate FMBackupUtility Configuration" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMBackupUtility\FMBackupUtilityConfiguration\bin\Release,ObfuscationOutputPath=%2\ObfuscatedFMBackupUtility\FMBackupUtilityConfiguration\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate Load Rack Service" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\LoadRackComponents\LoadRackService\bin\Release,ObfuscationOutputPath=%2\ObfuscatedLoadRackService\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate WebAFSSWebApp" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\Extension.AFSS\Afss.WebApp\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\AFSSWebApp,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate FMDispatchProxyServices" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMDispatchProxyServices\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\FMDispatchProxyServices,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate GasBoyBusinessServices" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\Extension.AFSS\Module.Gasboy\Gasboy.BusinessServices\bin,ObfuscationOutputPath=%2\ObfuscatedWebRole\GasboyBusinessServices,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationBigFileList_v2010.xml
ECHO "Dotfuscate FM Active Directory Manage Service" FileName="[DotfuscatorPathAndExe]"
REM XCOPY /p=ObfuscationInputPath=%1\Web Application\FMActiveDirectoryManageService\bin\Release,ObfuscationOutputPath=%2\ObfuscatedFMActiveDirectoryManageService\bin,StrongNameKeyPath=%1\Web Application\FMBusinessObjects /v" "%1\Web Application\ObfuscationFMActiveDirectoryManageService_v2010.xml


ECHO ************************ COPY REMAINING ARTIFACTS **********************************
ECHO "Create Additional Drop Folders"
ECHO "MD Database DACPAC"
MKDIR "%2\DatabaseDacpac"

ECHO "XCOPY FuelsManagerDWDB"
XCOPY "%1\Web Application\Database\DB Project\FuelsManagerDWDB\bin\Release\*.*" "%2\DatabaseDacpac" /E /R /Y /I /Q

ECHO "MD FMDataWarehouse SSIS"
MKDIR "%2\FMDataWarehouse\SSIS"
ECHO "MD FMDataWarehouse SSIS Installation"
MKDIR "%2\FMDataWarehouse\SSIS\Installation"
ECHO "XCOPY FMDataWarehouse SSIS"
XCOPY "%1\Web Application\Database\SSIS Project\FuelsManagerDW\Installation\*.*" "%2\FMDataWarehouse\SSIS\Installation" /E /R /Y /I /Q

ECHO "Copy Build to Binaries Folder"
ECHO "XCOPY dacpacs"
XCOPY "%1\Web Application\Database\DB Project\dacpacs\*.*" "%2\DatabaseDacpac" /E /R /Y /I /Q


ECHO "MD FMArchive"
MKDIR "%2\FMArchive"
ECHO "MD FuelsManagerArchiveDB"
MKDIR "%2\FMArchive\FuelsManagerArchiveDB"
ECHO "XCOPY FMArchive"
XCOPY "%1\Web Application\Database\FM Archive\DB Project\FuelsManagerArchiveDB\FuelsManagerArchiveDB\bin\Release\*.*" "%2\FMArchive\FuelsManagerArchiveDB" /E /R /Y /I /Q

ECHO "MD LiveDBComponents"
MKDIR "%2\FMArchive\LiveDBComponents"
ECHO "XCOPY FMArchive LiveDBComponents"
XCOPY "%1\Web Application\Database\FM Archive\LiveDBComponents\*.*" "%2\FMArchive\LiveDBComponents" /E /R /Y /I /Q

ECHO "MD FMArchive SSIS"
MKDIR "%2\FMArchive\SSIS"
ECHO "MD FMArchive SSIS Installation"
MKDIR "%2\FMArchive\SSIS\Installation"
ECHO "XCOPY FMArchive SSIS"
XCOPY "%1\Web Application\Database\FM Archive\SSIS\Installation\*.*" "%2\FMArchive\SSIS\Installation" /E /R /Y /I /Q

ECHO "MD HelpFiles"
MKDIR "%2\HelpFiles"
ECHO "*** HELP FILES ARE CURRENTLY NOT IN THE BITBUCKET REPOSITORY SO THIS BUILD DEFINITION IS UNABLE TO ACCESS THEM ***"
REM ECHO "Help Files"
REM ECHO "XCOPY Help Files"
REM XCOPY %1\FMHelp" "%2\HelpFiles" /E /R /Y /I /Q

ECHO "MD ReportFiles"
MKDIR "%2\ReportFiles"
ECHO "MD ReportFiles Standard"
MKDIR "%2\ReportFiles\StandardReports"
ECHO "*** STANDARD REPORTS ARE CURRENTLY NOT IN THE BITBUCKET REPOSITORY SO THIS BUILD DEFINITION IS UNABLE TO ACCESS THEM ***

REM ECHO "Report Files"
REM ECHO "Copy Standard Reports"
REM ECHO "XCOPY RDL files"
REM XCOPY "%1\StandardReports\*.rdl" "%2\ReportFiles\StandardReports\" /E /R /Y /I /Q
REM ECHO "XCOPY SQL files"
REM XCOPY "%1\StandardReports\*.sql" "%2\ReportFiles\StandardReports\" /E /R /Y /I /Q
REM ECHO "XCOPY PNG files"
REM XCOPY "%1\StandardReports\*.png" "%2\ReportFiles\StandardReports\" /E /R /Y /I /Q
REM ECHO "XCOPY JPG files"
REM XCOPY "%1\StandardReports\*.jpg" "%2\ReportFiles\StandardReports\" /E /R /Y /I /Q

REM ECHO "Copy Other Reports"
REM ECHO "XCOPY Other RDL files"
REM XCOPY "%1\Web Application\Projects\*.rdl" "%2\ReportFiles\" /E /R /Y /I /Q
REM ECHO "XCOPY Other SQL files"
REM XCOPY "%1\Web Application\Projects\*.sql" "%2\ReportFiles\" /E /R /Y /I /Q
REM ECHO "XCOPY PNG files"
REM XCOPY "%1\Web Application\Projects\*.png" "%2\ReportFiles\" /E /R /Y /I /Q
REM ECHO "XCOPY JPG files"
REM XCOPY "%1\Web Application\Projects\*.jpg" "%2\ReportFiles\" /E /R /Y /I /Q


:ERROR_NO_SOURCE
@ECHO.
@ECHO Please pass in the build source path
@ECHO.
GOTO :EXIT

:ERROR_NO_DEST
@ECHO.
@ECHO Please pass in the binary output path
@ECHO.
GOTO :EXIT

:EXIT