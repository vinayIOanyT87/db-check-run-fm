@ECHO Updating existing database with dacpacs

@ECHO Updating FuelsManagerDB Standard
@CALL SQLScript_UpdateFMDB.cmd FuelsManagerDB HH0G8V1\SQLSERVER2012 dacpacs\FuelsManagerDB.dacpac 
@ECHO Updating FuelsManagerDB Core
@CALL SQLScript_UpdateFMDB.cmd FuelsManagerDB HH0G8V1\SQLSERVER2012 dacpacs\FuelsManagerCore.dacpac
@ECHO Updating FuelsManagerDB for NSPA
@CALL SQLScript_UpdateFMDB.cmd FuelsManagerDB HH0G8V1\SQLSERVER2012 dacpacs\FuelsManagerNSPA.dacpac

