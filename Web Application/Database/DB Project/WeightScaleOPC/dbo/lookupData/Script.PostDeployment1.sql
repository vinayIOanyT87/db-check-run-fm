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

MERGE INTO [tblWeightScaleBaudLookup] AS Target
USING (VALUES
(0, N'1200')
,(1, N'2400')
,(2, N'4800')
,(3, N'9600')
,(4, N'19200')
,(5, N'38400')
,(6, N'Max')
) AS Source ([BaudIndex], [BaudDescription])
ON (Target.[BaudIndex] = Source.[BaudIndex])
WHEN MATCHED AND (Target.[BaudDescription] <> Source.[BaudDescription]) THEN
	UPDATE SET 
				[BaudDescription] = Source.[BaudDescription]
WHEN NOT MATCHED THEN
	INSERT ([BaudIndex], [BaudDescription])
		VALUES (Source.[BaudIndex],Source.[BaudDescription])
;

MERGE INTO [tblWeightScaleDataBitsLookup] AS Target
USING (VALUES
(0, N'7')
,(1, N'8')
,(2, N'Max')
) AS Source ([DataBitsIndex], [DataBitsDescription])
ON (Target.[DataBitsIndex] = Source.[DataBitsIndex])
WHEN MATCHED AND (Target.[DataBitsDescription] <> Source.[DataBitsDescription]) THEN
	UPDATE SET 
				[DataBitsDescription] = Source.[DataBitsDescription]
WHEN NOT MATCHED THEN
	INSERT ([DataBitsIndex], [DataBitsDescription])
		VALUES (Source.[DataBitsIndex],Source.[DataBitsDescription])
;
MERGE INTO [tblWeightScaleParityLookup] AS Target
USING (VALUES
(0, N'None')
,(1, N'Even')
,(2, N'Odd')
,(3, N'Max')
) AS Source ([ParityIndex], [ParityDescription])
ON (Target.[ParityIndex] = Source.[ParityIndex])
WHEN MATCHED AND (Target.[ParityDescription] <> Source.[ParityDescription]) THEN
	UPDATE SET 
				[ParityDescription] = Source.[ParityDescription]
WHEN NOT MATCHED THEN
	INSERT ([ParityIndex], [ParityDescription])
		VALUES (Source.[ParityIndex],Source.[ParityDescription])
;
MERGE INTO [tblWeightScaleStopBitsLookup] AS Target
USING (VALUES
(0, N'1')
,(1, N'2')
,(2, N'Max')
) AS Source ([StopBitsIndex], [StopBitsDescription])
ON (Target.[StopBitsIndex] = Source.[StopBitsIndex])
WHEN MATCHED AND (Target.[StopBitsDescription] <> Source.[StopBitsDescription]) THEN
	UPDATE SET 
				[StopBitsDescription] = Source.[StopBitsDescription]
WHEN NOT MATCHED THEN
	INSERT ([StopBitsIndex], [StopBitsDescription])
		VALUES (Source.[StopBitsIndex],Source.[StopBitsDescription])
;
MERGE INTO [tblWeightScaleTypeLookup] AS Target
USING (VALUES
(0, N'Toledo 8142')
,(1, N'Fairbanks 90 164')
,(2, N'Brechbuhler UMC600')
,(3, N'Sipelaries ASCII')
,(4, N'Mettler Toledo SICS')
,(5, N'Rice Lake 720i')
) AS Source ([TypeIndex], [TypeDescription])
ON (Target.[TypeIndex] = Source.[TypeIndex])
WHEN MATCHED AND (Target.[TypeDescription] <> Source.[TypeDescription]) THEN
	UPDATE SET 
				[TypeDescription] = Source.[TypeDescription]
WHEN NOT MATCHED THEN
	INSERT ([TypeIndex], [TypeDescription])
		VALUES (Source.[TypeIndex],Source.[TypeDescription])
;