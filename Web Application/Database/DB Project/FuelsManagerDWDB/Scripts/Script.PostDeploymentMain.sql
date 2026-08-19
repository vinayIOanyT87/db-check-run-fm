/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r ..\dboData\dbo.tblSystemSettings.refdata.sql
:r ..\dboData\dbo.DimSystemInfo.refdata.sql
:r ..\dboData\dbo.DimUnitOfMeasure.refdata.sql
:r ..\mapData\map.tblSSASPartitionToRangeCriteria.refdata.sql
:r ..\dboData\dbo.tblPreRunMDXQueries.refdata.sql