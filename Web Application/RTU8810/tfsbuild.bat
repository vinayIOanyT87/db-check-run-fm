cd rtu8810
del "..\RTUWebApi\bin\release\net461\publish" / S /Q
del dist /S /Q
CALL npm install
CALL npm uninstall --save-dev angular-cli
CALL npm install --save-dev @angular/cli@9.1.9
IF NOT [%1]==[] (npm version %1)
CALL ng build --prod
cd ../RTUWebApi
CALL dotnet publish --configuration Release
