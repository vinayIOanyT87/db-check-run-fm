
CREATE FUNCTION [rpt].[udf_IsStringUniqueidentifier] (@ui nvarchar(50))  
RETURNS bit AS  
BEGIN

declare @uiToCheck nvarchar(50);

declare @formatCheck bit
declare @hexCheck1 bit
declare @hexCheck2 bit
declare @hexCheck3 bit
declare @hexCheck4 bit
declare @hexCheck5 bit
Set @formatCheck = 0;
Set @hexCheck1 = 1;
Set @hexCheck2 = 1;
Set @hexCheck3 = 1;
Set @hexCheck4 = 1;
Set @hexCheck5 = 1;
  
  /*3815bb6f-dfdb-4b92-bad6-968aa758afe0*/

if(substring(@ui,9,1)='-' and substring(@ui,14,1)='-' and substring(@ui,19,1)='-' and substring(@ui,24,1)='-' and len(@ui) = 36)
   Set @formatCheck = 1;

if substring(@ui,1,8) LIKE '%[^a-fA-F0-9]%'
	Set @hexCheck1 = 0;
if  substring(@ui,10,4) LIKE '%[^a-fA-F0-9]%'
	Set @hexCheck2 = 0;
if substring(@ui,15,4) LIKE '%[^a-fA-F0-9]%'
	Set @hexCheck3 = 0;
if substring(@ui,20,4) LIKE '%[^a-fA-F0-9]%'
	Set @hexCheck4 = 0;
if substring(@ui,25,12) LIKE '%[^a-fA-F0-9]%'
	Set @hexCheck5 = 0;

return @formatCheck & @hexCheck1 & @hexCheck2 & @hexCheck3 & @hexCheck4 & @hexCheck5;

END