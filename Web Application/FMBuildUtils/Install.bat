echo off
set parm1=99
set parm2=99

IF (%1)==(/L) GOTO SET_PARMA
IF (%1)==(/l) GOTO SET_PARMA
IF (%1)==(-L) GOTO SET_PARMA
IF (%1)==(-l) GOTO SET_PARMA
GOTO END1
:SET_PARMA
set parm2=LOCAL
:END1

IF (%1)==(/O) GOTO SET_PARMB
IF (%1)==(/o) GOTO SET_PARMB
IF (%1)==(-O) GOTO SET_PARMB
IF (%1)==(-o) GOTO SET_PARMB
GOTO END2
:SET_PARMB
set parm1=OPC
:END2

IF (%2)==(/L) GOTO SET_PARMC
IF (%2)==(/l) GOTO SET_PARMC
IF (%2)==(-L) GOTO SET_PARMC
IF (%2)==(-l) GOTO SET_PARMC
GOTO END3
:SET_PARMC
set parm2=LOCAL
:END3

IF (%2)==(/O) GOTO SET_PARMD
IF (%2)==(/o) GOTO SET_PARMD
IF (%2)==(-O) GOTO SET_PARMD
IF (%2)==(-o) GOTO SET_PARMD
GOTO END4
:SET_PARMD
set parm1=OPC
:END4

:: Make a directory for component service files
IF (%parm2%)==(LOCAL) GOTO CREATE_COMPONENT_DIR
GOTO END
:CREATE_COMPONENT_DIR
set srcpath=C:\Varec Shared\Build_Fusion
mkdir "C:\Varec Shared\Build_Fusion\ComponentServices"
echo .
echo .
echo Do not forget to create the following MSI component services files manually:
echo %srcpath%\AccountingBLLInstall.MSI
echo %srcpath%\FMSharedComponentsInstall.MSI
echo %srcpath%\AcculoadOPCInstall.MSI
echo %srcpath%\ContrecOPCInstall.MSI
echo %srcpath%\OptomuxOPCInstall.MSI
echo %srcpath%\OsdpOPCInstall.MSI
echo %srcpath%\DanielOPCInstall.MSI
echo %srcpath%\WeightScaleOPCInstall.MSI
pause
:END

echo on
::Copy non-shared binaries to destination
set srcpath=C:\Varec Shared\Build_Fusion\FuelsManager
set srcs=
set srcs=%srcs% "%srcpath%\AccountingEOM.exe"
set srcs=%srcs% "%srcpath%\LoadRackService.exe"
set srcs=%srcs% "%srcpath%\LogService.exe"
set srcs=%srcs% "%srcpath%\TransactionArchivingService.exe"

set destpath="C:\Program Files\FuelsManager"
FOR %%f IN (%srcs%) DO xcopy /I /Y /Q %%f %destpath%

::Copy non-shared binaries to destination
set srcpath=C:\Varec Shared\Build_Fusion\FuelsManager\Online Documentation
set destpath="C:\Program Files\FuelsManager\Online Documentation"
xcopy /E /I /Y /Q %srcpath% %destpath%

::Copy OPC DLLs to the .../Program Files/FuelsManager directory
set srcpath=C:\Varec Shared\Build_Fusion\FuelsManagerVDir\Bin
set srcs=
set srcs=%srcs% "%srcpath%\AcculoadOPCObjects.dll"
set srcs=%srcs% "%srcpath%\AcculoadOPCServer.dll"
set srcs=%srcs% "%srcpath%\ContrecOPCObjects.dll"
set srcs=%srcs% "%srcpath%\ContrecOPCServer.dll"
set srcs=%srcs% "%srcpath%\OptomuxOPCObjects.dll"
set srcs=%srcs% "%srcpath%\OptomuxOPCServer.dll"
set srcs=%srcs% "%srcpath%\OsdpOPCObjects.dll"
set srcs=%srcs% "%srcpath%\OsdpOPCServer.dll"
set srcs=%srcs% "%srcpath%\WeightScaleOPCObjects.dll"
set srcs=%srcs% "%srcpath%\WeightScaleOPCServer.dll"
set srcs=%srcs% "%srcpath%\DanielOPCObjects.dll"
set srcs=%srcs% "%srcpath%\DanielOPCServer.dll"
set srcs=%srcs% "%srcpath%\DanielOPCObjects.dll"
set srcs=%srcs% "%srcpath%\DanielOPCServer.dll"

set destpath="C:\Program Files\FuelsManager"
FOR %%f IN (%srcs%) DO xcopy /I /Y /Q %%f %destpath%

:: Copy shared binaries to destination
set srcpath=C:\Varec Shared\Build_Fusion\FuelsManager Shared
set srcs=
set srcs=%srcs% "%srcpath%\AccountingBLL.dll"
set srcs=%srcs% "%srcpath%\AccountingDAL.dll"
set srcs=%srcs% "%srcpath%\AccountingServices.dll"
set srcs=%srcs% "%srcpath%\ADFPriceCalculator.dll"
set srcs=%srcs% "%srcpath%\ADFTransactionCustomFields.dll"
set srcs=%srcs% "%srcpath%\amqmdnet.dll"
set srcs=%srcs% "%srcpath%\ConsolidatedBLL.dll"
set srcs=%srcs% "%srcpath%\ConsolidatedDAL.dll"
set srcs=%srcs% "%srcpath%\ConsolidatedDataObjects.dll"
set srcs=%srcs% "%srcpath%\ConsolidatedUtilities.dll"
set srcs=%srcs% "%srcpath%\ConvertEngUnitsU.dll"
set srcs=%srcs% "%srcpath%\EntityImportExport.dll"
set srcs=%srcs% "%srcpath%\FinanceBLL.dll"
set srcs=%srcs% "%srcpath%\FinanceDAL.dll"
set srcs=%srcs% "%srcpath%\FinanceDataObjects.dll"
set srcs=%srcs% "%srcpath%\FMCLRAssembly.dll"
set srcs=%srcs% "%srcpath%\FMCommon.dll"
set srcs=%srcs% "%srcpath%\FMControls.dll"
set srcs=%srcs% "%srcpath%\FMSecurityAuthentication.dll"
set srcs=%srcs% "%srcpath%\FMUtil.dll"
set srcs=%srcs% "%srcpath%\Helpers.dll"
set srcs=%srcs% "%srcpath%\gettransfields.dll"
set srcs=%srcs% "%srcpath%\ICSharpCode.SharpZipLib.dll"
set srcs=%srcs% "%srcpath%\Interop.AcctTx03.dll"
set srcs=%srcs% "%srcpath%\Interop.ConsolidatedUtilities.dll"
set srcs=%srcs% "%srcpath%\Interop.ConvertEngUnits.dll"
set srcs=%srcs% "%srcpath%\Interop.DataManager.dll"
set srcs=%srcs% "%srcpath%\Interop.DataObjects.dll"
set srcs=%srcs% "%srcpath%\Interop.DisplayServer.dll"
set srcs=%srcs% "%srcpath%\Interop.FMUtil.dll"
set srcs=%srcs% "%srcpath%\Interop.VolumeCorrection.dll"
set srcs=%srcs% "%srcpath%\LogClient.dll"
set srcs=%srcs% "%srcpath%\LoadRackLibrary.dll"
set srcs=%srcs% "%srcpath%\PIDXTransactions.dll"
set srcs=%srcs% "%srcpath%\PIDXCommunications.dll"
set srcs=%srcs% "%srcpath%\PriceCalculator.dll"
set srcs=%srcs% "%srcpath%\ReportingBLL.dll"
set srcs=%srcs% "%srcpath%\ReportingDAL.dll"
set srcs=%srcs% "%srcpath%\ReportingServices.dll"
set srcs=%srcs% "%srcpath%\ReserveLevelCalculator.dll"
set srcs=%srcs% "%srcpath%\SigPlusNET.dll"
set srcs=%srcs% "%srcpath%\VolumeCorrection.dll"
set srcs=%srcs% "%srcpath%\VolumeCorrectionDotNet.dll"
set srcs=%srcs% "%srcpath%\XMLImport.dll"

set SharedBinariesPath="C:\Program Files\Common Files\FuelsManager Shared"
FOR %%f IN (%srcs%) DO xcopy /I /Y /Q %%f %SharedBinariesPath%

::Copy web applications to destination
set srcpath="C:\Varec Shared\Build_Fusion\FuelsManagerVDir"
set destpath="C:\Program Files\FuelsManager\FuelsManagerVDir"
xcopy /E /I /Y /Q %srcpath% %destpath%

set srcpath="C:\Varec Shared\Build_Fusion\AccountingImportExportVDir"
set destpath="C:\Program Files\FuelsManager\AccountingImportExportVDir"
xcopy /E /I /Y /Q %srcpath% %destpath%

::Copy pre-existing (SCADA related) binaries to destination. These are not provided by the FuelsManager.sln build
::set srcpath=C:\FM7RuntimeBinaries
::set srcs=
::set srcs=%srcs% "%srcpath%\FMDisplayServer.exe"
::set srcs=%srcs% "%srcpath%\FMDataManager.exe"
::set srcs=%srcs% "%srcpath%\FMDataObjects.dll"
::set srcs=%srcs% "%srcpath%\FMDataManager.tlb"
::set srcs=%srcs% "%srcpath%\FMSystem.dll"
::set destpath="C:\Program Files\FuelsManager"
::FOR %%f IN (%srcs%) DO xcopy /I /Y /Q %%f %destpath%

::Create Virtual Directories
cscript %SystemRoot%\system32\iisvdir.vbs /create "Default Web Site" FuelsManager "C:\Program Files\FuelsManager\FuelsManagerVDir"
cscript %SystemRoot%\system32\iisvdir.vbs /create "Default Web Site" FMReport "C:\Program Files\FuelsManager\FuelsManagerVDir\FMReporting"
cscript %SystemRoot%\system32\iisvdir.vbs /create "Default Web Site" AccountingImportExport "C:\Program Files\FuelsManager\AccountingImportExportVDir"

::Call regsvr32 for SCADA related files
::"C:\Program Files\FuelsManager\FMDisplayServer.exe" /regserver
::"C:\Program Files\FuelsManager\FMDisplayServer.exe" /service
::"C:\Program Files\FuelsManager\FMDataManager.exe" /regserver
:: regsvr32 /s "C:\Program Files\FuelsManager\FMDataObjects.dll"

regsvr32 /s "C:\Program Files\Common Files\FuelsManager Shared\FMUtil.dll"
regsvr32 /s "C:\Program Files\Common Files\FuelsManager Shared\ConvertEngUnitsU.dll"
regsvr32 /s "C:\Program Files\Common Files\FuelsManager Shared\ConsolidatedUtilities.dll"

::Add assemblies to Global Assembly Cache
set GacFiles=
set GacFiles=%GacFiles% AccountingBLL.dll
set GacFiles=%GacFiles% AccountingDAL.dll
set GacFiles=%GacFiles% AccountingServices.dll
set GacFiles=%GacFiles% ADFPriceCalculator.dll
set GacFiles=%GacFiles% ADFTransactionCustomFields.dll
set GacFiles=%GacFiles% amqmdnet.dll
set GacFiles=%GacFiles% ConsolidatedBLL.dll
set GacFiles=%GacFiles% ConsolidatedDAL.dll
set GacFiles=%GacFiles% ConsolidatedDataObjects.dll
set GacFiles=%GacFiles% EntityImportExport.dll
set GacFiles=%GacFiles% FinanceBLL.dll
set GacFiles=%GacFiles% FinanceDAL.dll
set GacFiles=%GacFiles% FinanceDataObjects.dll
set GacFiles=%GacFiles% FMCommon.dll
set GacFiles=%GacFiles% FMControls.dll
set GacFiles=%GacFiles% FMSecurityAuthentication.dll
set GacFiles=%GacFiles% Helpers.dll
set GacFiles=%GacFiles% Interop.AcctTx03.dll
set GacFiles=%GacFiles% Interop.ConsolidatedUtilities.dll
set GacFiles=%GacFiles% Interop.ConvertEngUnits.dll
set GacFiles=%GacFiles% Interop.DataObjects.dll
set GacFiles=%GacFiles% Interop.DataManager.dll
set GacFiles=%GacFiles% Interop.DisplayServer.dll
set GacFiles=%GacFiles% Interop.FMUtil.dll
set GacFiles=%GacFiles% Interop.VolumeCorrection.dll
set GacFiles=%GacFiles% LoadRackLibrary.dll
set GacFiles=%GacFiles% LogClient.dll
set GacFiles=%GacFiles% PIDXTransactions.dll
set GacFiles=%GacFiles% PIDXCommunications.dll
set GacFiles=%GacFiles% PriceCalculator.dll
set GacFiles=%GacFiles% ReportingBLL.dll
set GacFiles=%GacFiles% ReportingDAL.dll
set GacFiles=%GacFiles% ReportingServices.dll
set GacFiles=%GacFiles% ReserveLevelCalculator.dll
set GacFiles=%GacFiles% SigPlusNET.dll
set GacFiles=%GacFiles% VolumeCorrectionDotNet.dll
set GacFiles=%GacFiles% XMLImport.dll

set gaccmd="C:\Varec Shared\Build_Fusion\Utils\gacutil"
set destpath=C:\Program Files\Common Files\FuelsManager Shared
FOR %%f IN (%GacFiles%) DO %gaccmd% /i "%destpath%\%%f"

::Register .NET COM Services
set srcpath=C:\Varec Shared\Build_Fusion\ComponentServices
IF (%parm2%)==(LOCAL) GOTO INSTALL_WITH_REGSRV1
   "%srcpath%\AccountingBLLInstall.MSI"
   "%srcpath%\FMSharedComponentsInstall.MSI"
   GOTO DONE1
:INSTALL_WITH_REGSRV1
   set ServiceFiles=
   set ServiceFiles=%ServiceFiles% ConsolidatedBLL.dll
   set ServiceFiles=%ServiceFiles% ConsolidatedDAL.dll
   set ServiceFiles=%ServiceFiles% AccountingBLL.dll
   set ServiceFiles=%ServiceFiles% FinanceBLL.dll
   set servicecmd="%windir%\Microsoft.NET\Framework\v2.0.50727\regsvcs"
   set destpath=C:\Program Files\Common Files\FuelsManager Shared
   FOR %%f IN (%ServiceFiles%) DO %servicecmd% "%destpath%\%%f"
:DONE1


:: Only register OPC objects if the user has requested to do so.
IF (%parm1%)==(OPC) GOTO REGOPC
GOTO DONE_OPC
:REGOPC
set srcpath=C:\Varec Shared\Build_Fusion\ComponentServices

:: If local is set, then regsrv32 the objects, otherwise install using MSI
IF (%parm2%)==(LOCAL) GOTO INSTALL_WITH_REGSRV2
   "%srcpath%\AcculoadOPCInstall.MSI"
   "%srcpath%\ContrecOPCInstall.MSI"
   "%srcpath%\OptomuxOPCInstall.MSI"
   "%srcpath%\OsdpOPCInstall.MSI"
   "%srcpath%\DanielOPCInstall.MSI"
   "%srcpath%\WeightScaleOPCInstall.MSI"
:INSTALL_WITH_REGSRV2
   set ServiceFiles=
   set ServiceFiles=%ServiceFiles% AcculoadOPCObjects.dll
   set ServiceFiles=%ServiceFiles% AcculoadOPCServer.dll
   set ServiceFiles=%ServiceFiles% ContrecOPCObjects.dll
   set ServiceFiles=%ServiceFiles% ContrecOPCServer.dll
   set ServiceFiles=%ServiceFiles% OptomuxOPCObjects.dll
   set ServiceFiles=%ServiceFiles% OptomuxOPCServer.dll
   set ServiceFiles=%ServiceFiles% OsdpOPCObjects.dll
   set ServiceFiles=%ServiceFiles% OsdpOPCServer.dll
   set ServiceFiles=%ServiceFiles% WeightScaleOPCObjects.dll
   set ServiceFiles=%ServiceFiles% WeightScaleOPCServer.dll
   set ServiceFiles=%ServiceFiles% DanielOPCObjects.dll
   set ServiceFiles=%ServiceFiles% DanielOPCServer.dll
   set ServiceFiles=%ServiceFiles% DanielOPCObjects.dll
   set ServiceFiles=%ServiceFiles% DanielOPCServer.dll
   set servicecmd="%windir%\system32\regsvr32"
   set destpath=C:\Program Files\FuelsManager
   FOR %%f IN (%ServiceFiles%) DO %servicecmd% /s "%destpath%\%%f"
:DONE_OPC

::Register NT Services
set installcmd="%windir%\Microsoft.NET\framework\v2.0.50727\installutil"
%installcmd% "C:\Program Files\FuelsManager\AccountingEOM.exe"
%installcmd% "C:\Program Files\FuelsManager\LoadRackService.exe"
%installcmd% "C:\Program Files\FuelsManager\LogService.exe"
%installcmd% "C:\Program Files\FuelsManager\TransactionArchivingService.exe"



