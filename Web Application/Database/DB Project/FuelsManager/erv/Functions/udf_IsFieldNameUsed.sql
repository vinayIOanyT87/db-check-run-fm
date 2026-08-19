



/*
	SELECT [erv].[udf_IsFieldNameUsed] ('23E3CCEC-2CCF-4653-A497-29FD15FAFCD4', 'HBEquipmentUserData01')
*/
CREATE FUNCTION [erv].[udf_IsFieldNameUsed]
(@EntitySegmentTemplateGuid uniqueidentifier, @FieldName nvarchar(100))
RETURNS bit
AS
BEGIN

------------------------------------------------------------------------------------------------------
-- Function: [erv].[udf_IsFieldNameUsed]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
-- Purpose: Function to verify if a name is already used either as an internal field name or as a UserDataField display name for an entity type.
-- Notes:
-- 1. @EntitySegmentTemplateGuid: Guid of an Entity Segment Template as defined in the Entity Segment Template table (erv.tblEntitySegmentTemplate)
-- 2. @FieldName: Name to be tested.
-- 3. An internal field is a field that is defined in the direct data table associated with the entity type (e.g. dbo.tblEquipment for the Equipment entity type).
-- 4. A UserDataField table is a dedicated UserDataField table for an entity type (e.g. dbo.tblUserDataFieldEquipment for the Equipment entity type).
------------------------------------------------------------------------------------------------------

	DECLARE @result bit
	SET @result = 0

	DECLARE @appTableName varchar(200)
	DECLARE @userDataFieldTableName varchar(200)
	DECLARE @entityTypeId varchar(100)

	SELECT @appTableName = AppTableName, @entityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

	IF EXISTS 
	(
		SELECT * FROM sys.columns
		WHERE object_id = OBJECT_ID(@appTableName)
		AND Name = @FieldName
	)	
		SET @result = 1
	ELSE
		SET @result = 0

	IF (@result = 0)
	BEGIN
		IF  (@entityTypeId = 'Equipment')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldEquipment]
				WHERE DisplayName = @FieldName
			)	
				SET @result = 1
		END
		ELSE IF  (@entityTypeId = 'Product')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldProduct]
				WHERE DisplayName = @FieldName
			)	
				SET @result = 1
		END
		ELSE IF  (@entityTypeId = 'Company')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldCompany]
				WHERE DisplayName = @FieldName
			)	
				SET @result = 1
		END
		ELSE IF  (@entityTypeId = 'Personnel')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldPersonnel]
				WHERE DisplayName = @FieldName
			)	
				SET @result = 1
		END
		ELSE IF  (@entityTypeId = 'Transaction_Alias')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM [dbo].[tblUserDataFieldTransactionAlias]
				WHERE DisplayName = @FieldName
			)	
				SET @result = 1
		END
	END

	RETURN @result         
END