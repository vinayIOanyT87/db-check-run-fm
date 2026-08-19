set parm1=99

IF (%1)==(/O) GOTO SET_PARM
IF (%1)==(/o) GOTO SET_PARM
IF (%1)==(-O) GOTO SET_PARM
IF (%1)==(-o) GOTO SET_PARM
GOTO END1
:SET_PARM
set parm1=OPC
:END1

::Shut down the web server first, as it sometimes holds on to a few files we want to delete.
net stop "World Wide Web Publishing Service"

::Remove NT services
net stop "FuelsManager Diagnostics"
net stop "FuelsManager Terminal Automation"
set cmd="%SystemRoot%\Microsoft.NET\framework\v2.0.50727\installutil"
%cmd% /u "C:\Program Files\FuelsManager\AccountingEOM.exe"
%cmd% /u "C:\Program Files\FuelsManager\LogService.exe"
%cmd% /u "C:\Program Files\FuelsManager\LoadRackService.exe"
%cmd% /u "C:\Program Files\FuelsManager\TransactionArchivingService.exe"

::Remove COM servers
set ServiceFiles=
set ServiceFiles=%ServiceFiles% ConsolidatedBLL.dll
set ServiceFiles=%ServiceFiles% ConsolidatedDAL.dll
set ServiceFiles=%ServiceFiles% AccountingBLL.dll
set ServiceFiles=%ServiceFiles% FinanceBLL.dll

set servicecmd="%SystemRoot%\Microsoft.NET\Framework\v2.0.50727\regsvcs"
set destpath=C:\Program Files\Common Files\FuelsManager Shared
FOR %%f IN (%ServiceFiles%) DO %servicecmd% /u "%destpath%\%%f"

::Remove OPC COM servers
IF (%parm1%)==(OPC) GOTO REMOVE_OPC
GOTO OPC_DONE
:REMOVE_OPC
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

set servicecmd="%SystemRoot%\system32\regsvr32"
set destpath=C:\Program Files\FuelsManager
FOR %%f IN (%ServiceFiles%) DO %servicecmd% /u /s "%destpath%\%%f"
:OPC_DONE

::Remove assemblies from the Global Assembly Cache
set GacFiles=
set GacFiles=%GacFiles% AccountingBLL
set GacFiles=%GacFiles% AccountingDAL
set GacFiles=%GacFiles% AccountingServices
set GacFiles=%GacFiles% ADFPriceCalculator
set GacFiles=%GacFiles% ADFTransactionCustomFields
set GacFiles=%GacFiles% amqmdnet
set GacFiles=%GacFiles% ConsolidatedBLL
set GacFiles=%GacFiles% ConsolidatedDAL
set GacFiles=%GacFiles% ConsolidatedDataObjects
set GacFiles=%GacFiles% EntityImportExport
set GacFiles=%GacFiles% FinanceBLL
set GacFiles=%GacFiles% FinanceDAL
set GacFiles=%GacFiles% FinanceDataObjects
set GacFiles=%GacFiles% FMCommon
set GacFiles=%GacFiles% FMControls
set GacFiles=%GacFiles% FMSecurityAuthentication
set GacFiles=%GacFiles% Helpers
set GacFiles=%GacFiles% Interop.AcctTx03
set GacFiles=%GacFiles% Interop.ConsolidatedUtilities
set GacFiles=%GacFiles% Interop.ConvertEngUnits
set GacFiles=%GacFiles% Interop.DataObjects
set GacFiles=%GacFiles% Interop.DataManager
set GacFiles=%GacFiles% Interop.DisplayServer
set GacFiles=%GacFiles% Interop.FMUtil
set GacFiles=%GacFiles% Interop.VolumeCorrection
set GacFiles=%GacFiles% LoadRackLibrary
set GacFiles=%GacFiles% LogClient
set GacFiles=%GacFiles% PIDXTransactions
set GacFiles=%GacFiles% PIDXCommunications
set GacFiles=%GacFiles% PriceCalculator
set GacFiles=%GacFiles% ReportingBLL
set GacFiles=%GacFiles% ReportingDAL
set GacFiles=%GacFiles% ReportingServices
set GacFiles=%GacFiles% ReserveLevelCalculator
set GacFiles=%GacFiles% VolumeCorrectionDotNet
set GacFiles=%GacFiles% SigPlusNET
set GacFiles=%GacFiles% XMLImport

set gaccmd="C:\Varec Shared\Build_Fusion\Utils\gacutil"
set destpath=C:\Program Files\Common Files\FuelsManager Shared
FOR %%f IN (%GacFiles%) DO %gaccmd% /u "%%~nf"

::Remove Shared program files
set SharedBinariesPath=C:\Program Files\Common Files\FuelsManager Shared
del /S /Q "%SharedBinariesPath%\*"

::Remove Virtual Directories
cscript %SystemRoot%\system32\iisvdir.vbs /delete "Default Web Site"/FuelsManager
cscript %SystemRoot%\system32\iisvdir.vbs /delete "Default Web Site"/AccountingImportExport
cscript %SystemRoot%\system32\iisvdir.vbs /delete "Default Web Site"/FMReport

::Remove non-shared program files
set destpath=C:\Program Files\FuelsManager
del "%destpath%\AccountingEOM.exe"
del "%destpath%\LoadRackService.exe"
del "%destpath%\LogClient.dll"
del "%destpath%\LogService.exe"
del "%destpath%\TransactionArchivingService.exe"
del "%destpath%\AcculoadOPCObjects.dll"
del "%destpath%\AcculoadOPCServer.dll"
del "%destpath%\ContrecOPCObjects.dll"
del "%destpath%\ContrecOPCServer.dll"
del "%destpath%\OptomuxOPCObjects.dll"
del "%destpath%\OptomuxOPCServer.dll"
del "%destpath%\OsdpOPCObjects.dll"
del "%destpath%\OsdpOPCServer.dll"
del "%destpath%\WeightScaleOPCObjects.dll"
del "%destpath%\WeightScaleOPCServer.dll"
del "%destpath%\DanielOPCObjects.dll"
del "%destpath%\DanielOPCServer.dll"
del /S /Q "%destpath%\FuelsManagerVDir\*"
rmdir /S /Q "%destpath%\FuelsManagerVDir"
del /S /Q "%destpath%\AccountingImportExportVDir\*"
rmdir /S /Q "%destpath%\AccountingImportExportVDir"

::Restart the web server
net start "World Wide Web Publishing Service"
