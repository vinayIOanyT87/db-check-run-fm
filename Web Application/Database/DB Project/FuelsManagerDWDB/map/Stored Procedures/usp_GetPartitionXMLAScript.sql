/*
	DROP PROCEDURE [map].[usp_GetPartitionXMLAScript]

	EXEC [map].[usp_GetPartitionXMLAScript] 20090101
	
*/
CREATE PROCEDURE [map].[usp_GetPartitionXMLAScript] @InventoryDate int
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [map].[usp_GetPartitionXMLAScript]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Identifies the correct SSAS Cube partition for a given Inventory Date year, and returns the XMLA script for that partition.
  -- Notes:
  -- 1. @InventoryDate. InventoryDate in the format YYYYMMDD.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    DECLARE @databaseID varchar(50)
    DECLARE @cubeID varchar(50)
    DECLARE @measureGroupID varchar(50)
    DECLARE @partitionID varchar(50)
    DECLARE @partitionName varchar(50)
    DECLARE @openStartdate int
    DECLARE @openEndDate int
    DECLARE @xmlaScriptTemplate nvarchar(2000)
	DECLARE @xmlaScriptTarget nvarchar(2000)

	DECLARE @tblXmlaScript TABLE
	(
		XmlaScript nvarchar(2000)
	)

    SELECT
      @openStartDate = 18000101
    SELECT
      @openEndDate = 30000101

    SELECT @xmlaScriptTemplate = SettingValue
    FROM dbo.tblSystemSettings
	WHERE SettingKey = 'PartitionSchemaTemplate'


	DECLARE TableCursor CURSOR FOR 
		SELECT DatabaseID, CubeID, MeasureGroupID, PartitionID, PartitionName
		FROM map.tblSSASPartitionToRangeCriteria
		WHERE @InventoryDate BETWEEN ISNULL(LowerRange, @openStartDate) AND ISNULL(UpperRange, @openEndDate)
	OPEN TableCursor 

		FETCH NEXT FROM TableCursor INTO @databaseID, @cubeID, @measureGroupID, @partitionID, @partitionName
 
		WHILE @@FETCH_STATUS = 0  
		BEGIN 
			IF (@partitionID IS NOT NULL)
			BEGIN
			  SET @xmlaScriptTarget = REPLACE(@xmlaScriptTemplate, 'PUTDATABASEIDHERE', @databaseID)
			  SET @xmlaScriptTarget = REPLACE(@xmlaScriptTarget, 'PUTCUBEIDHERE', @cubeID)
			  SET @xmlaScriptTarget = REPLACE(@xmlaScriptTarget, 'PUTMEASUREGROUPIDHERE', @measureGroupID)
			  SET @xmlaScriptTarget = REPLACE(@xmlaScriptTarget, 'PUTPARTITIONIDHERE', @partitionID)
			END

			INSERT INTO @tblXmlaScript
			(XmlaScript)
			VALUES (@xmlaScriptTarget)
		
			FETCH NEXT FROM TableCursor INTO @databaseID, @cubeID, @MeasureGroupID, @partitionID, @partitionName
		END 
	CLOSE TableCursor 
	DEALLOCATE TableCursor 

    SELECT XmlaScript From @tblXmlaScript

  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [map].[usp_GetPartitionXMLAScript]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
