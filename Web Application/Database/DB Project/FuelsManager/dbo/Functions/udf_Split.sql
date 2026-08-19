CREATE FUNCTION [dbo].[udf_Split]
(@ItemList NVARCHAR (4000), @delimiter CHAR (1))
RETURNS 
    @GeneratedTableName TABLE (
        [Item] NVARCHAR (50) NULL)
AS
BEGIN
--The script body was encrypted, and cannot be reproduced here.
    RETURN
END