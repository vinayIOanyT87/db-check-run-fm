del "..\RTUWebApi\bin\release\net461\publish" /S /Q
del dist /S /Q
CALL npm install
CALL npm uninstall --save-dev angular-cli
CALL npm install --save-dev @angular/cli@latest
CALL ng build --prod
cd ../RTUWebApi
CALL dotnet publish --configuration Release
