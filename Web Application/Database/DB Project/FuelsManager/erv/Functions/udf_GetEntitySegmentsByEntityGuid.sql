

/*
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Equipment', NULL)
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Equipment', NULL)
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Equipment', '7EC5B639-5207-4DF9-8B2B-11167E9E248E')
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Equipment', '05C83626-004B-4097-A028-E343F4C856F5')
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Product', '80B08634-D356-4569-B9A2-CD36DF955BD0')
	SELECT * FROM [erv].[udf_GetEntitySegmentsByEntityGuid] ('Transaction_Alias', '0DC68ACA-11AD-4F43-AD2B-87609738C453')

*/

	CREATE FUNCTION [erv].[udf_GetEntitySegmentsByEntityGuid]
	(
		@EntityTypeId nvarchar(100), @EntityGuid uniqueidentifier
	)
	RETURNS @tblEntityRecords TABLE
	(
		FilterValueGuid uniqueidentifier NULL,
		EntitySegmentTemplateGuid uniqueidentifier NOT NULL
	)
	AS
	BEGIN
	/*
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[udf_GetEntitySegmentsByEntityGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to return the entity segments applicable to a given entity record
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityGuid: Entity record guid. It can be either the index of the specific child record version being examined, or the index of the master record version. 
	--                 This field is only required for equipments, where it is used to determine the EquipmentTypeGuid (FileterValueGuid). 
	-- 3. More than one entity segment can be returned for a given entity record if more than one segment (i.e. more than one filter field) has been defined for the same entity type.
	------------------------------------------------------------------------------------------------------
	*/

	
		IF (@EntityTypeId = 'Equipment')
		BEGIN
			INSERT @tblEntityRecords
			(FilterValueGuid, EntitySegmentTemplateGuid)
			SELECT  
				CASE 
					WHEN (b.FilterFieldName = 'EquipmentTypeGuid') THEN a.EquipmentTypeGuid 
					ELSE NULL 
				END, 
				b.EntitySegmentTemplateGuid 
			FROM [dbo].[tblEquipment] a
			INNER JOIN erv.tblEntitySegmentTemplate b
			ON b.EntityTypeId = @EntityTypeId
			WHERE ((a.EquipmentGuid = @EntityGuid) OR (@EntityGuid IS NULL))
		END
		ELSE IF ((@EntityTypeId = 'Product') OR (@EntityTypeId = 'Company') OR (@EntityTypeId = 'Transaction_Alias') OR (@EntityTypeId = 'Personnel'))
		BEGIN			
			INSERT @tblEntityRecords
			(FilterValueGuid, EntitySegmentTemplateGuid)
			SELECT  NULL, EntitySegmentTemplateGuid 
			FROM erv.tblEntitySegmentTemplate
			WHERE EntityTypeId = @EntityTypeId
		END
		RETURN;
	END