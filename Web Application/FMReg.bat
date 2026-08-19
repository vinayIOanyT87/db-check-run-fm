@echo off

set ConfigurationName=Debug
set gaccmd="C:\FuelsManager.net\Primus\Web Application\FMBuildUtils\GacUtil40\gacutil" /nologo /i
set svcscmd="C:\FuelsManager.net\Primus\Web Application\FMBuildUtils\regsvcs"

echo.

@echo on
regsvr32 /s Binaries\ConvertEngUnitsU.dll
regsvr32 /s Binaries\FMUtil.dll
regsvr32 /s Binaries\FMDataManagerps.dll
regsvr32 /s Binaries\FMDataObjects.dll
regsvr32 /s AcculoadOPC\AcculoadOPCServer\Debug\AcculoadOPCServer.dll
regsvr32 /s OptomuxOPC\OptomuxOPCServer\Debug\OptomuxOPCServer.dll
regsvr32 /s OsdpOPC\OsdpOPCServer\Debug\OsdpOPCServer.dll
regsvr32 /s WeightScaleOPC\WeightScaleOPCServer\Debug\WeightScaleOPCServer.dll
regsvr32 /s LoadRackComponents\LoadRackService\bin\Debug\LoadRackLibrary.dll
regsvr32 /s LoadRackComponents\LoadRackService\bin\Debug\LoadRackService.exe


%gaccmd% Binaries\Interop.AcctTx03.dll

%gaccmd% Binaries\Interop.ConvertEngUnits.dll

%gaccmd% Binaries\Interop.DataObjects.dll

%gaccmd% Binaries\Interop.DataManager.dll

%gaccmd% Binaries\Interop.DisplayServer.dll

%gaccmd% Binaries\Interop.FMInterfaces.dll

%gaccmd% Binaries\Interop.FMUtil.dll

%gaccmd% Binaries\Interop.VolumeCorrection.dll

echo.
echo Registering services...
%svcscmd% /quiet SharedComponents\ConsolidatedBLL\bin\ConsolidatedBLL.dll
%svcscmd% /quiet AccountingComponents\AccountingBLL\bin\AccountingBLL.dll
%svcscmd% /quiet WebTicketingComponents\WebTicketingBLL\bin\WebTicketingBLL.dll

echo.
echo Done
echo.
