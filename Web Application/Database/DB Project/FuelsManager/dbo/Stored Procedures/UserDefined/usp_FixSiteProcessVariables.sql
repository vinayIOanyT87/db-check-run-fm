

CREATE PROCEDURE [dbo].[usp_FixSiteProcessVariables]

AS
BEGIN
	SET NOCOUNT ON

	DECLARE @SiteList TABLE(SiteGuid UNIQUEIDENTIFIER)

	INSERT INTO @SiteList
		SELECT SiteGuid
		 FROM tblSites
		WHERE SiteGroupFlag = CAST(0 AS bit)
		
	DECLARE @SiteGuid UNIQUEIDENTIFIER

	DECLARE SiteCursor CURSOR FOR
		SELECT * FROM @SiteList

	OPEN SiteCursor

	FETCH NEXT FROM SiteCursor INTO @SiteGuid

	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @PVGuid UNIQUEIDENTIFIER

		-- Check AlarmOutputPV 
		SELECT @PVGuid = OPCConnectionGuid
		  FROM [dbo].[tblProcessVariableSite]
		 WHERE LookupProcessVariableTypeIndex = 21
		   AND SiteGuid           = @SiteGuid
		   
		IF (@PVGuid IS NULL)
			INSERT INTO [dbo].[tblProcessVariableSite]
			(
				[LookupProcessVariableTypeIndex],
				[InstanceNumber],
				[OPCItemID],
				[SiteGuid],
				[DataType],
				[ServerEngineeringUnitsIndex],
				[Quality],
				[DateTimeStamp],
				[DataTypeEnabled],
				[Input],
				[InputEnabled],
				CreatedDate,
				CreatedBy,
				UpdatedDate,
				UpdatedBy				
			)		
		
			VALUES
			(
				21             ,
				0              ,
				''             ,
				@SiteGuid     ,
				11             ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				0              ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				'Administrator',
				SYSDATETIMEOFFSET()      ,
				'Administrator'
			)


		-- Check WatchDogOutputPV 
		SELECT @PVGuid = OPCConnectionGuid
		  FROM [dbo].[tblProcessVariableSite]
		 WHERE LookupProcessVariableTypeIndex = 22
		   AND SiteGuid           = @SiteGuid
		   
		IF (@PVGuid IS NULL)
			INSERT INTO [dbo].[tblProcessVariableSite]
			(
				[LookupProcessVariableTypeIndex]        ,
				InstanceNumber             ,
				OPCItemID                  ,
				[SiteGuid]                  ,
				DataType                   ,
				ServerEngineeringUnitsIndex,
				Quality                    ,
				DateTimeStamp              ,
				DataTypeEnabled            ,
				Input                      ,
				InputEnabled               ,
				CreatedDate                ,
				CreatedBy                  ,
				UpdatedDate                ,
				UpdatedBy
			)
			VALUES
			(
				22             ,
				0              ,
				''             ,
				@SiteGuid     ,
				11             ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				0              ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				'Administrator',
				SYSDATETIMEOFFSET()      ,
				'Administrator'
			)

		-- Check VRUSetpointOutputPV 
		SELECT @PVGuid = OPCConnectionGuid
		  FROM [dbo].[tblProcessVariableSite]
		 WHERE LookupProcessVariableTypeIndex = 23
		   AND SiteGuid           = @SiteGuid
		   
		IF (@PVGuid IS NULL)
			INSERT INTO [dbo].[tblProcessVariableSite]
			(
				[LookupProcessVariableTypeIndex]        ,
				InstanceNumber             ,
				OPCItemID                  ,
				[SiteGuid]                  ,
				DataType                   ,
				ServerEngineeringUnitsIndex,
				Quality                    ,
				[SIValue]					,
				DateTimeStamp              ,
				DataTypeEnabled            ,
				Input                      ,
				InputEnabled               ,
				CreatedDate                ,
				CreatedBy                  ,
				UpdatedDate                ,
				UpdatedBy
			)
			VALUES
			(
				23             ,
				0              ,
				''             ,
				@SiteGuid     ,
				5             ,
				243              ,
				192              ,
				0				,
				SYSDATETIMEOFFSET()      ,
				0              ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				'Administrator',
				SYSDATETIMEOFFSET()      ,
				'Administrator'
			)
			
		-- Check VRUDeadbandOutputPV 
		SELECT @PVGuid = OPCConnectionGuid
		  FROM [dbo].[tblProcessVariableSite]
		 WHERE LookupProcessVariableTypeIndex = 24
		   AND SiteGuid           = @SiteGuid
		   
		IF (@PVGuid IS NULL)
			INSERT INTO tblProcessVariableSite
			(
				[LookupProcessVariableTypeIndex]        ,
				InstanceNumber             ,
				OPCItemID                  ,
				[SiteGuid]                  ,
				DataType                   ,
				ServerEngineeringUnitsIndex,
				Quality                    ,
				[SIValue]					,
				DateTimeStamp              ,
				DataTypeEnabled            ,
				Input                      ,
				InputEnabled               ,
				CreatedDate                ,
				CreatedBy                  ,
				UpdatedDate                ,
				UpdatedBy
			)
			VALUES
			(
				24             ,
				0              ,
				''             ,
				@SiteGuid     ,
				5             ,
				234              ,
				192              ,
				0				,
				SYSDATETIMEOFFSET()      ,
				0              ,
				0              ,
				0              ,
				SYSDATETIMEOFFSET()      ,
				'Administrator',
				SYSDATETIMEOFFSET()      ,
				'Administrator'
			)


		FETCH NEXT FROM SiteCursor INTO @SiteGuid
	END

	-- Task 3904. Error changing API to Kg per l for site Commercial-AF. 
	-- These changes have also been added to the above. 
	UPDATE dbo.tblProcessVariableSite
		SET SIValue = 0,
			 ServerEngineeringUnitsIndex = 234,
			 Quality = 192
	 WHERE SIValue IS NULL
		AND [LookupProcessVariableTypeIndex] IN (23, 24)

END