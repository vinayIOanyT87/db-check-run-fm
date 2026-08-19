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

CREATE VIEW [dbo].[vw_TableAndColumnName]
AS
      SELECT      sc.name as SchemaName
            ,     tb.name as TableName
            ,     dbo.udf_GetDisplayName(tb.name,1) AS TableDisplayName
            ,     cl.name as ColumnName
            ,     dbo.udf_GetDisplayName(cl.name,1) AS ColumnDisplayName
      FROM sys.tables tb
      INNER JOIN sys.schemas sc on tb.schema_id=sc.schema_id      
      INNER JOIN sys.columns cl ON tb.object_id=cl.object_id
      WHERE LEFT(cl.name,1) <> '_'
