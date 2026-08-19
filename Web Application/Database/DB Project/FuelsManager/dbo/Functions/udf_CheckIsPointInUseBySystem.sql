CREATE FUNCTION [dbo].[udf_CheckIsPointInUseBySystem]
(
	@PointGuid UniqueIdentifier
)
RETURNS NVARCHAR(100)
AS
BEGIN
	DECLARE @IsMovement BIT = 0
	DECLARE @IsNode BIT = 0
	DECLARE @PointTemplateGuid UNIQUEIDENTIFIER = (SELECT PointTemplateGuid FROM [dbo].[tblPoint] WHERE PointGuid = @PointGuid)

	IF EXISTS (SELECT 1 FROM [map].[tblModuleToPointTemplate] WHERE PointTemplateGuid = @PointTemplateGuid AND ModuleGuid = 'E0024C94-0725-4423-9261-EDE9D84A6ACC')
		SET @IsMovement = 1
	ELSE IF EXISTS (SELECT 1 FROM [map].[tblModuleToPointTemplate] WHERE PointTemplateGuid = @PointTemplateGuid AND (ModuleGuid IN('26DE3166-5417-415C-9801-BB2E363D2447','F769E8AF-1F5F-4EC7-A2E5-58759EF79186','DB8313DD-E9BD-4BCF-8584-B3B6B33E827E')))
		SET @IsNode = 1

	IF @IsMovement = 1
	BEGIN
		DECLARE @Value NVARCHAR(MAX) = CONVERT(NVARCHAR(MAX), (SELECT pt.Value FROM [dbo].[tblPointTag] pt
										INNER JOIN [dbo].[tblPointTemplateTag] ptt ON ptt.PointTemplateTagGuid = pt.PointTemplateTagGuid
										WHERE PointGuid = @PointGuid AND ptt.WellKnownIdentityGuid = '0BC90D94-A42B-4C6F-8C99-60A18A5546AB'))

		IF '<MovementStatus>Inactive</MovementStatus>' <> @Value AND '<MovementStatus>Disabled</MovementStatus>' <> @Value
		BEGIN
			RETURN 'Attempt to Modify/Delete Point in use by the Movement System'
		END
	END
	ELSE IF @IsNode = 1
	BEGIN
		DECLARE @NodeGuidString VARCHAR(36) = CONVERT(varchar(36),LOWER(@PointGuid))

		IF EXISTS (SELECT 1 FROM [dbo].[tblPointProperty] pp
											INNER JOIN [dbo].[tblPoint] p ON p.PointGuid = pp.PointGuid
											INNER JOIN [dbo].[tblPointTag] pt ON pt.PointGuid = p.PointGuid 
											INNER JOIN [dbo].[tblPointTemplateTag] ptt ON ptt.PointTemplateTagGuid = pt.PointTemplateTagGuid
											WHERE ptt.WellKnownIdentityGuid = '0BC90D94-A42B-4C6F-8C99-60A18A5546AB'
											AND CONVERT(VARCHAR(MAX),pt.Value) <> '<MovementStatus>Inactive</MovementStatus>'
											AND CONVERT(VARCHAR(MAX),pt.Value) <> '<MovementStatus>Disabled</MovementStatus>'
											AND pp.ID = 'Movement Settings'
											AND pp.Value.exist('/MovementModuleSettings/MovementNodeDataList/MovementNodeData[MovementNodeGuid = sql:variable("@NodeGuidString")]') = 1)
		BEGIN
			RETURN 'Attempt to Modify/Delete Point in use by the Movement System'
		END
	END
	RETURN NULL
END
