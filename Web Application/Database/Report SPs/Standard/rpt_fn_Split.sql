USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[rpt_fn_Split]') IS NOT NULL
   DROP FUNCTION [rpt_fn_Split]

GO

CREATE  FUNCTION [dbo].[rpt_fn_Split]
/******************************************************************************
Function: [Split]
Take a string of comma separated values and return
Version 7.5.1.0
Version History
7/29/2009	UP	Created Initial Stored Procedure
******************************************************************************/
(
  @ItemList NVARCHAR(4000),
  @delimiter CHAR(1)
)
  RETURNS @itemtable TABLE (Item NVARCHAR(50) )

AS

BEGIN
DECLARE @tempItemList NVARCHAR(4000)
SET @tempItemList = @ItemList

DECLARE @i INT
DECLARE @Item NVARCHAR(4000)

SET @tempItemList = REPLACE (@tempItemList, @delimiter + ' ', @delimiter)
SET @i = CHARINDEX(@delimiter, @tempItemList)

WHILE (LEN(@tempItemList) > 0)
BEGIN
IF @i = 0
SET @Item = @tempItemList
ELSE
SET @Item = LEFT(@tempItemList, @i - 1)

INSERT INTO @itemtable(Item) VALUES(@Item)

IF @i = 0
SET @tempItemList = ''
ELSE
SET @tempItemList = RIGHT(@tempItemList, LEN(@tempItemList) - @i)

SET @i = CHARINDEX(@delimiter, @tempItemList)
END
RETURN
END

/*******
Call up in stored procedure example
Notes:
@Owner parameter needs to be set to @Owner nvarchar(4000)
WHERE  @Owner IS NULL OR OwnerID IN (SELECT * FROM rpt_fn_Split(@Owner, ',')) 
RDL:
main stored procedure dataset parameter set 
Name: @Owner 
Value: =JOIN(Parameters!Owner.Value,",")

******/



