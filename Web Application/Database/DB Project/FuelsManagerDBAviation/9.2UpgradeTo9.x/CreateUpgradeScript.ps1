If ($(Get-Module PathUtils -ListAvailable).Length -eq 0) {Install-Module -Name PathUtils -Scope CurrentUser -Force} #install path helper
Add-ToPath 'C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin' #add sqlpackage from ssms2017 temporarily to our path 
SqlPackage.exe /Action:Script `
/Profile:".\10.33.16.167.FuelsManagerDBAviation.publish.xml" `
/SourceFile:"..\..\dacpacs\FuelsManagerDB.dacpac" `
/OutputPath:".\MigrationScript1.sql" | Tee-Object -FilePath .\Migration.log

#this creates the aviation specific migration script
If ($(Get-Module PathUtils -ListAvailable).Length -eq 0) {Install-Module -Name PathUtils -Scope CurrentUser -Force} #install path helper
Add-ToPath 'C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin' #add sqlpackage from ssms2017 temporarily to our path 
SqlPackage.exe /Action:Script `
/Profile:".\10.33.16.212.FuelsManagerDBAviation.publish.xml" `
/SourceFile:"..\..\dacpacs\FuelsManagerAviation.dacpac" `
/OutputPath:".\MigrationScript2.sql" | Tee-Object -FilePath .\Migration.log

SqlPackage.exe /Action:Publish `
/Profile:".\10.33.16.167.FuelsManagerDBAviation.publish.xml" `
/SourceFile:"..\..\dacpacs\FuelsManagerAviation.dacpac"

If ($(Get-Module PathUtils -ListAvailable).Length -eq 0) {Install-Module -Name PathUtils -Scope CurrentUser -Force} #install path helper
Add-ToPath 'C:\Program Files (x86)\Microsoft SQL Server\140\DAC\bin' #add sqlpackage from ssms2017 temporarily to our path 
SqlPackage.exe /Action:Script `
/Profile:".\localhost.FuelsManagerDBAviation.publish.xml" `
/SourceFile:"..\..\dacpacs\FuelsManagerDB.dacpac" `
/OutputPath:".\MigrationScript3.sql" | Tee-Object -FilePath .\Migration.log