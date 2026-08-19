/****** Object:  StoredProcedure [dbo].[usp_ArchiveAlarmAndEventLog]    Script Date: 11/12/2013 14:13:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveAlarmAndEventLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveAlarmAndEventLog]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveAlarmAndEventLog]    Script Date: 11/12/2013 14:13:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveAlarmAndEventLog]
(
		@BeginDate datetime,
		@EndDate datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveAlarmAndEventLog] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	INSERT INTO FuelsManagerDBArchive.dbo.tblAlarmAndEventLog 
	(
		 [AlarmAndEventLogGuid]
		,[SiteGuid]
		,[SequenceNumber]
		,[Source]
		,[Alarm]
		,[ID]
		,[AssociatedData]
		,[CategoryID]
		,[PriorityID]
		,[Acknowledged]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	)
	SELECT 
		 [AlarmAndEventLogGuid]
		,[SiteGuid]
		,[SequenceNumber]
		,[Source]
		,[Alarm]
		,[ID]
		,[AssociatedData]
		,[CategoryID]
		,[PriorityID]
		,[Acknowledged]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblAlarmAndEventLog
	WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblAlarmAndEventLog
	WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived AlarmAndEventLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveAlarmAndEventLog' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveAuditLog]    Script Date: 11/12/2013 14:14:18 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveAuditLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveAuditLog]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveAuditLog]    Script Date: 11/12/2013 14:14:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveAuditLog]
(
		@BeginDate datetime,
		@EndDate datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveAuditLog] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	INSERT INTO FuelsManagerDBArchive.dbo.tblAuditLog 
	(
		 [AuditLogGuid]
		,[SiteGuid]
		,[SessionID]
		,[ActionID]
		,[TypeID]
		,[ID]
		,[PropertyID]
		,[NewValue]
		,[OldValue]
		,[CreatedDate]
		,[CreatedBy]
		,[ParentTypeID]
		,[_RowVersion]
	)
	SELECT 
		 [AuditLogGuid]
		,[SiteGuid]
		,[SessionID]
		,[ActionID]
		,[TypeID]
		,[ID]
		,[PropertyID]
		,[NewValue]
		,[OldValue]
		,[CreatedDate]
		,[CreatedBy]
		,[ParentTypeID]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblAuditLog
	WHERE [CreatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblAuditLog
	WHERE [CreatedDate] BETWEEN @BeginDate AND @EndDate
	
	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived AuditLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;


	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveAuditLog' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveMaintenanceData]    Script Date: 11/12/2013 14:23:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveMaintenanceData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveMaintenanceData]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveMaintenanceData]    Script Date: 11/12/2013 14:23:25 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveMaintenanceData]
(
		@BeginDate datetime,
		@EndDate datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveMaintenanceData] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	INSERT INTO FuelsManagerDBArchive.dbo.tblMaintenanceReasons 
	(
		 [MaintenanceReasonGuid]
		,[ID]
		,[Description]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[DeletedFlag]
		,[_RowVersion]
	)
	SELECT 
		 [MaintenanceReasonGuid]
		,[ID]
		,[Description]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[DeletedFlag]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblMaintenanceReasons
	WHERE UpdatedDate BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblMaintenanceReasons 
	WHERE UpdatedDate BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	(SELECT GetDate(), 'Success' AS Status, 'Archived MaintenanceReasons.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info);

	INSERT INTO FuelsManagerDBArchive.dbo.tblEquipmentMaintenanceLog 
	(
		 [EquipmentMaintenanceLogGuid]
		,[SiteGuid]
		,[EquipmentGuid]
		,[MaintenanceReasonGuid]
		,[OperatorPersonnelGuid]
		,[EquipmentID]
		,[EquipmentType]
		,[OperatorID]
		,[MaintenanceReason]
		,[InServiceFlag]
		,[ChangeDate]
		,[EstReturnToServiceDate]
		,[WorkOrder]
		,[Memo]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	)
	SELECT 
		 [EquipmentMaintenanceLogGuid]
		,[SiteGuid]
		,[EquipmentGuid]
		,[MaintenanceReasonGuid]
		,[OperatorPersonnelGuid]
		,[EquipmentID]
		,[EquipmentType]
		,[OperatorID]
		,[MaintenanceReason]
		,[InServiceFlag]
		,[ChangeDate]
		,[EstReturnToServiceDate]
		,[WorkOrder]
		,[Memo]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblEquipmentMaintenanceLog 
	WHERE ChangeDate BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblEquipmentMaintenanceLog 
	WHERE ChangeDate BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	(SELECT GetDate(), 'Success' AS Status, 'Archived EquipmentMaintenanceLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info);

	INSERT INTO FuelsManagerDBArchive.dbo.tblTankMaintenanceLog 
	(
		 [TankMaintenanceLogGuid]
		,[TankGuid]
		,[SiteGuid]
		,[MaintenanceReasonGuid]
		,[OperatorPersonnelGuid]
		,[TankID]
		,[LookupVesselTypeIndex]
		,[VesselTypeIndex]
		,[VesselType]
		,[OperatorID]
		,[MaintenanceReason]
		,[InServiceFlag]
		,[ChangeDate]
		,[EstReturnToServiceDate]
		,[WorkOrder]
		,[Memo]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	)
	SELECT 
		 [TankMaintenanceLogGuid]
		,[TankGuid]
		,[SiteGuid]
		,[MaintenanceReasonGuid]
		,[OperatorPersonnelGuid]
		,[TankID]
		,[LookupVesselTypeIndex]
		,[LookupVesselTypeIndex]
		,[VesselType]
		,[OperatorID]
		,[MaintenanceReason]
		,[InServiceFlag]
		,[ChangeDate]
		,[EstReturnToServiceDate]
		,[WorkOrder]
		,[Memo]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblTankMaintenanceLog 
	WHERE ChangeDate BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTankMaintenanceLog 
	WHERE ChangeDate BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	(SELECT GetDate(), 'Success' AS Status, 'Archived TankMaintenanceLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info);

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveMaintenanceData' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveQualityData]    Script Date: 11/12/2013 14:23:56 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveQualityData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveQualityData]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveQualityData]    Script Date: 11/12/2013 14:23:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveQualityData]
(
		@BeginDate datetime,
		@EndDate datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveQualityData] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	INSERT INTO FuelsManagerDBArchive.dbo.tblEquipmentQualityTagLog 
	(
		 [EquipmentQualityTagLogGuid]
		,[SiteGuid]
		,[EquipmentGuid]
		,[QualityTagGuid]
		,[QualityTagName]
		,[EquipmentID]
		,[EquipmentType]
		,[TaggedDate]
		,[TaggedBy]
		,[Memo]
		,[RemovedDate]
		,[RemovedBy]
		,[DeleteFlag]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TagNumber]
		,[_RowVersion]
	)
	SELECT 
		 [EquipmentQualityTagLogGuid]
		,[SiteGuid]
		,[EquipmentGuid]
		,[QualityTagGuid]
		,[QualityTagName]
		,[EquipmentID]
		,[EquipmentType]
		,[TaggedDate]
		,[TaggedBy]
		,[Memo]
		,[RemovedDate]
		,[RemovedBy]
		,[DeleteFlag]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TagNumber]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblEquipmentQualityTagLog 
		WHERE [RemovedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblEquipmentQualityTagLog 
		WHERE [RemovedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived EquipmentQualityTagLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	INSERT INTO FuelsManagerDBArchive.dbo.tblTankQualityTagLog
	(
		 [TankQualityTagLogGuid]
		,[TankGuid]
		,[QualityTagGuid]
		,[SiteGuid]
		,[TankID]
		,[LookupVesselTypeIndex]
		,[VesselTypeIndex]
		,[VesselType]
		,[QualityTagName]
		,[TaggedDate]
		,[TaggedBy]
		,[Memo]
		,[RemovedDate]
		,[RemovedBy]
		,[DeleteFlag]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TagNumber]
		,[_RowVersion]
	)
	SELECT 
		 [TankQualityTagLogGuid]
		,[TankGuid]
		,[QualityTagGuid]
		,[SiteGuid]
		,[TankID]
		,[LookupVesselTypeIndex]
		,[LookupVesselTypeIndex]
		,[VesselType]
		,[QualityTagName]
		,[TaggedDate]
		,[TaggedBy]
		,[Memo]
		,[RemovedDate]
		,[RemovedBy]
		,[DeleteFlag]
		,[CreatedDate]
		,[CreatedBy]
		,[UpdatedDate]
		,[UpdatedBy]
		,[TagNumber]
		,[_RowVersion]
	FROM FuelsManagerDB.dbo.tblTankQualityTagLog 
		WHERE [RemovedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTankQualityTagLog 
		WHERE [RemovedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived TankQualityTagLog.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveQualityData' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTestAndTestSetResultsData]    Script Date: 11/12/2013 14:24:11 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveTestAndTestSetResultsData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveTestAndTestSetResultsData]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTestAndTestSetResultsData]    Script Date: 11/12/2013 14:24:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveTestAndTestSetResultsData]
(
		@BeginDate datetime,
		@EndDate datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveTestAndTestSetResultsData] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	INSERT INTO FuelsManagerDBArchive.dbo.tblTestSetEquipmentResults 
	(
	[ResultTimeStamp],
	[TestSetName],
	[Inspector],
	[Supervisor],
	[EquipmentID],
	[SampleNumber],
	[SampleSize],
	[IsRetest],
	[PreviousSampleNumber],
	[DocumentNumber],
	[Memo],
	[GallonsRepresented],
	[Override],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[TestSetEquipmentResultGuid],
	[SiteGuid],
	[LookupTestSetStatusIndex],
	[EquipmentGuid]
	)
	SELECT 
	[ResultTimeStamp],
	[TestSetName],
	[Inspector],
	[Supervisor],
	[EquipmentID],
	[SampleNumber],
	[SampleSize],
	[IsRetest],
	[PreviousSampleNumber],
	[DocumentNumber],
	[Memo],
	[GallonsRepresented],
	[Override],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[TestSetEquipmentResultGuid],
	[SiteGuid],
	[LookupTestSetStatusIndex],
	[EquipmentGuid]
	FROM FuelsManagerDB.dbo.tblTestSetEquipmentResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTestSetEquipmentResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived TestSetEquipmentResults.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	INSERT INTO FuelsManagerDBArchive.dbo.tblTestSetTankResults 
	(
	[ResultTimeStamp],
	[TestSetName],
	[Inspector],
	[Supervisor],
	[TankID],
	[SampleNumber],
	[SampleSize],
	[IsRetest],
	[PreviousSampleNumber],
	[DocumentNumber],
	[Memo],
	[GallonsRepresented],
	[Override],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[TestSetTankResultGuid],
	[SiteGuid],
	[LookupTestSetStatusIndex],
	[TankGuid]
	)
	SELECT 
	[ResultTimeStamp],
	[TestSetName],
	[Inspector],
	[Supervisor],
	[TankID],
	[SampleNumber],
	[SampleSize],
	[IsRetest],
	[PreviousSampleNumber],
	[DocumentNumber],
	[Memo],
	[GallonsRepresented],
	[Override],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[TestSetTankResultGuid],
	[SiteGuid],
	[LookupTestSetStatusIndex],
	[TankGuid]
	FROM FuelsManagerDB.dbo.tblTestSetTankResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTestSetTankResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived TestSetTankResults.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	INSERT INTO FuelsManagerDBArchive.dbo.tblTestEquipmentResults 
	(
	[TestName],
	[Measurement],
	[TestDate],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[PerformedBy],
	[Supervisor],
	[TestEquipmentResultGuid],
	[LookupTestSetStatusIndex],
	[TestSetEquipmentResultGuid]
	)
	SELECT 
	[TestName],
	[Measurement],
	[TestDate],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[PerformedBy],
	[Supervisor],
	[TestEquipmentResultGuid],
	[LookupTestSetStatusIndex],
	[TestSetEquipmentResultGuid]
	FROM FuelsManagerDB.dbo.tblTestEquipmentResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTestEquipmentResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT GetDate(), 'Success' AS Status, 'Archived TestEquipmentResults.Count = ' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	INSERT INTO FuelsManagerDBArchive.dbo.tblTestTankResults
	(
	[TestName],
	[Measurement],
	[TestDate],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[PerformedBy],
	[Supervisor],
	[TestTankResultGuid],
	[LookupTestSetStatusIndex],
	[TestSetTankResultGuid]
	) 
	SELECT 
	[TestName],
	[Measurement],
	[TestDate],
	[DeleteFlag],
	[CreatedDate],
	[CreatedBy],
	[UpdatedDate],
	[UpdatedBy],
	[PerformedBy],
	[Supervisor],
	[TestTankResultGuid],
	[LookupTestSetStatusIndex],
	[TestSetTankResultGuid]
	FROM FuelsManagerDB.dbo.tblTestTankResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	DELETE FROM FuelsManagerDB.dbo.tblTestTankResults 
		WHERE [UpdatedDate] BETWEEN @BeginDate AND @EndDate

	INSERT INTO #MSG (LogTime , Status , Info ) 
	SELECT  GetDate(), 'Success' AS Status, 'Archived TestTankResults.Count=' + CAST(@@ROWCOUNT AS nvarchar) AS Info;

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveTestAndTestSetResultsData' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTransaction]    Script Date: 11/12/2013 14:24:26 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveTransaction]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveTransaction]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTransaction]    Script Date: 11/12/2013 14:24:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveTransaction]
(
	@TransID NVARCHAR(64)	
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveTransaction] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @TransID: ID of the transaction record that is to be archived.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	IF EXISTS(SELECT * FROM FuelsManagerDBArchive.dbo.tblTransactions WHERE TransID = @TransID) 
		RETURN;

	DECLARE @TransactionGuid uniqueidentifier

	SELECT @TransactionGuid = TransactionGuid FROM FuelsManagerDB.dbo.tblTransactions WHERE TransID = @TransID 
	EXEC dbo.usp_InsertTransactionsToArchiveTable @TransID, @TransactionGuid

	DECLARE @assocTransID NVARCHAR(64);
	DECLARE AssociatedTransactionIDs_cursor CURSOR FOR
		SELECT linkedTransID FROM FuelsManagerDB.dbo.tblTransactionLinks WHERE @TransID IN (OriginalTransID, LinkedTransID) 
	
	OPEN AssociatedTransactionIDs_cursor 
	FETCH NEXT FROM AssociatedTransactionIDs_cursor INTO @assocTransID 
	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		EXEC dbo.usp_ArchiveTransaction @assocTransID  
		FETCH NEXT FROM AssociatedTransactionIDs_cursor INTO @assocTransID 
	END 
	CLOSE AssociatedTransactionIDs_cursor 
	DEALLOCATE AssociatedTransactionIDs_cursor; 
	
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLinks WHERE @TransID IN (OriginalTransID, LinkedTransID) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionSublineItems 
			WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM FuelsManagerDB.dbo.tblTransactionLineItems WHERE TransactionGuid = @TransactionGuid) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLineItemUserData 
			WHERE TransactionLineItemGuid IN (SELECT TransactionLineItemGuid FROM FuelsManagerDB.dbo.tblTransactionLineItems WHERE TransactionGuid = @TransactionGuid) 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionWeightReadings WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionNotes   WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionSignature  WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionUserData   WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionPIDX    WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionTransportLineItems  WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactionLineItems  WHERE TransactionGuid = @TransactionGuid 
	DELETE FROM FuelsManagerDB.dbo.tblTransactions    WHERE TransactionGuid = @TransactionGuid
	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveTransaction' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTransactions]    Script Date: 11/12/2013 14:24:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_ArchiveTransactions]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_ArchiveTransactions]
GO

/****** Object:  StoredProcedure [dbo].[usp_ArchiveTransactions]    Script Date: 11/12/2013 14:24:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_ArchiveTransactions]
(
		@start_date datetime,
		@end_date datetime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_ArchiveTransactions] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @start_date: Beginning of date range to archive records.
	-- 2. @end_date: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	CREATE TABLE #TransToBeArchived (TransID NVARCHAR(64) NOT NULL, InventoryDate smalldatetime, SiteGuid uniqueidentifier, ProductGuid uniqueidentifier,TransTypeID smallint);
	/*Populate #TransToBeArchived */
	EXEC dbo.usp_GetTransactionsToBeArchived @start_date, @end_date 
	/* Cursor for all archivable transactions. */
	DECLARE transactionIDs_cursor CURSOR FOR SELECT TransID, InventoryDate FROM #TransToBeArchived  

	BEGIN TRY
		DECLARE @TransID NVARCHAR(64);
		DECLARE @inventoryDate DateTime;
		DECLARE @ArchivedTransCount int 
		
		Set @ArchivedTransCount = 0
		/*
		 Archive one FM transactions per SQL transaction.
		*/
		OPEN transactionIDs_cursor;
		FETCH NEXT FROM transactionIDs_cursor INTO @TransID, @inventoryDate;
		WHILE @@FETCH_STATUS = 0  
		BEGIN   
		--	INSERT INTO #MSG (LogTime , Status , Info )
		--	(SELECT  GetDate(), 'Info', 'Archiving transaction ' + @TransID + '. InventoryDate is ' + 
		--	CONVERT(nvarchar, @inventoryDate,101));
			BEGIN TRY 
				EXEC dbo.usp_ArchiveTransaction @TransID 
				SET @ArchivedTransCount = @ArchivedTransCount + 1
			END TRY
			BEGIN CATCH
				INSERT INTO #MSG (LogTime , Status , Info )
				(SELECT  GetDate(), 'Error' AS Status, 'Failed to archive transaction ' + @TransID + '. ' + ISNULL(ERROR_MESSAGE(),'Unknown') AS Info);
			END CATCH      
			FETCH NEXT FROM transactionIDs_cursor INTO @TransID, @inventoryDate;  
		END 
		insert into #MSG (LogTime , Status , Info )
		(SELECT  GetDate(), 'Success' AS Status, 'Archived transaction count = ' + CAST(@ArchivedTransCount AS nvarchar) + '. ' AS Info);
	END TRY 
	BEGIN CATCH
		INSERT INTO #MSG (LogTime , Status , Info ) 
		(SELECT  GetDate(), 'Error' AS Status, 'Failed to archive Accounting tables. ' +  ERROR_MESSAGE() AS Info); 
	END CATCH
	BEGIN TRY
		CLOSE transactionIDs_cursor; 
		DEALLOCATE transactionIDs_cursor; 
	END TRY 
	BEGIN CATCH 
	END CATCH 

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_ArchiveTransactions' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_InsertTransactionsToArchiveTable]    Script Date: 11/12/2013 14:26:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_InsertTransactionsToArchiveTable]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_InsertTransactionsToArchiveTable]
GO

/****** Object:  StoredProcedure [dbo].[usp_InsertTransactionsToArchiveTable]    Script Date: 11/12/2013 14:26:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_InsertTransactionsToArchiveTable]
(
	@TransID nvarchar(64),
	@TransactionGuid uniqueidentifier
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_InsertTransactionsToArchiveTable] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @TransID: Transaction Id of the transaction being inserted.
	-- 2. @TransactionGuid: Guid of the transaction being inserted.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY


	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactions (
	[TransID],
	[AliasName],
	[SubType],
	[Site],
	[TransReferenceID],
	[InventoryDate],
	[ShipToID],
	[ShipToCode],
	[SupplierID],
	[SupplierCode],
	[CreatedDate],
	[CreatedBy],
	[RequestedDeliveryDate],
	[UpdatedDate],
	[UpdatedBy],
	[TransDateTime],
	[TransVersion],
	[SCACCode],
	[CardNumber],
	[ShipmentNumber],
	[ShipperID],
	[ShipperCode],
	[OwnerID],
	[OwnerCode],
	[ManagerID],
	[ManagerCode],
	[CarrierID],
	[CarrierCode],
	[CarrierIndex],
	[ConjoinTransID],
	[ReversedTransID],
	[LinkedDocumentNumber],
	[ReversalType],
	[PONumber],
	[TimeIn],
	[TimeOut],
	[TimeEnd],
	[RoutingID],
	[TicketSource],
	[LoadID],
	[BillToID],
	[BillToCode],
	[DriverIdentificationNumber],
	[CreditAmount],
	[CardExpiration],
	[CardName],
	[CardType],
	[CashAmount],
	[RouteOriginationDate],
	[InternationalRouteIndicator],
	[PreviousRoutingID],
	[ShippingDocumentNumber],
	[DocumentNumber],
	[STD],
	[ETD],
	[STA],
	[ETA],
	[SFT],
	[FST],
	[EstimatedFuelingDuration],
	[DeleteFlag],
	[TicketMode],
	[DestinationRegistrationID1],
	[DestinationSerialNumber1],
	[DestinationEquipmentType1],
	[DestinationEquipmentModel1],
	[DestinationCompanyEquipmentID1],
	[DestinationRegistrationID2],
	[DestinationSerialNumber2],
	[DestinationEquipmentType2],
	[DestinationEquipmentModel2],
	[DestinationCompanyEquipmentID2],
	[DestinationRegistrationID3],
	[DestinationSerialNumber3],
	[DestinationEquipmentType3],
	[DestinationEquipmentModel3],
	[DestinationCompanyEquipmentID3],
	[SourceRegistrationID1],
	[SourceSerialNumber1],
	[SourceEquipmentType1],
	[SourceEquipmentModel1],
	[SourceCompanyEquipmentID1],
	[SourceRegistrationID2],
	[SourceSerialNumber2],
	[SourceEquipmentType2],
	[SourceEquipmentModel2],
	[SourceCompanyEquipmentID2],
	[SourceRegistrationID3],
	[SourceSerialNumber3],
	[SourceEquipmentType3],
	[SourceEquipmentModel3],
	[SourceCompanyEquipmentID3],
	[OperatorID],
	[EffectiveDate],
	[ExpirationDate],
	[ScheduledDate],
	[AutoComplete],
	[Flag01],
	[Flag02],
	[Flag03],
	[Flag04],
	[Flag05],
	[Flag06],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06],
	[ContactFirstName],
	[ContactSurname],
	[Date01],
	[Date02],
	[Date03],
	[Date04],
	[LegacyNumber],
	[Country],
	[ContactInfo],
	[AssociatedDocNumber],
	[AssociatedCLIN],
	[SubmittedToAccounting],
	[FuelCardID],
	[AssociatedTransportOrderNumber],
	[RequestedDateTime],
	[DispatchedDateTime],
	[ErrorFlag],
	[TransactionGuid],
	[SiteGuid],
	[LookupTransTypeIndex],
	[LookupTransactionStatusIndex],
	[LookupOriginApplicationIndex],
	[TransactionAliasGuid],
	[BillToCompanyGuid],
	[Destination1EquipmentGuid],
	[Destination2EquipmentGuid],
	[Destination3EquipmentGuid],
	[FinalStationIATAGuid],
	[FuelCardGuid],
	[ManagerCompanyGuid],
	[NextStationIATAGuid],
	[OperatorPersonnelGuid],
	[OriginStationIATAGuid],
	[OwnerCompanyGuid],
	[PreviousStationIATAGuid],
	[ShipperCompanyGuid],
	[ShipToCompanyGuid],
	[Source1EquipmentGuid],
	[Source2EquipmentGuid],
	[Source3EquipmentGuid],
	[SupplierCompanyGuid],
	[CarrierCompanyGuid],
	[ReasonCodeGuid],
	[OriginStationIATAID],
	[PreviousStationIATAID],
	[NextStationIATAID],
	[FinalStationIATAID],
	[OperatorName],
	[FuelAdditiveFlag],
	[IssuePoint],
	[IssuePointNumber],
	[RadioNumber]
	) 
	SELECT 
	[TransID],
	[AliasName],
	[SubType],
	[Site],
	[TransReferenceID],
	[InventoryDate],
	[ShipToID],
	[ShipToCode],
	[SupplierID],
	[SupplierCode],
	[CreatedDate],
	[CreatedBy],
	[RequestedDeliveryDate],
	[UpdatedDate],
	[UpdatedBy],
	[TransDateTime],
	[TransVersion],
	[SCACCode],
	[CardNumber],
	[ShipmentNumber],
	[ShipperID],
	[ShipperCode],
	[OwnerID],
	[OwnerCode],
	[ManagerID],
	[ManagerCode],
	[CarrierID],
	[CarrierCode],
	[CarrierIndex],
	[ConjoinTransID],
	[ReversedTransID],
	[LinkedDocumentNumber],
	[ReversalType],
	[PONumber],
	[TimeIn],
	[TimeOut],
	[TimeEnd],
	[RoutingID],
	[TicketSource],
	[LoadID],
	[BillToID],
	[BillToCode],
	[DriverIdentificationNumber],
	[CreditAmount],
	[CardExpiration],
	[CardName],
	[CardType],
	[CashAmount],
	[RouteOriginationDate],
	[InternationalRouteIndicator],
	[PreviousRoutingID],
	[ShippingDocumentNumber],
	[DocumentNumber],
	[STD],
	[ETD],
	[STA],
	[ETA],
	[SFT],
	[FST],
	[EstimatedFuelingDuration],
	[DeleteFlag],
	[TicketMode],
	[DestinationRegistrationID1],
	[DestinationSerialNumber1],
	[DestinationEquipmentType1],
	[DestinationEquipmentModel1],
	[DestinationCompanyEquipmentID1],
	[DestinationRegistrationID2],
	[DestinationSerialNumber2],
	[DestinationEquipmentType2],
	[DestinationEquipmentModel2],
	[DestinationCompanyEquipmentID2],
	[DestinationRegistrationID3],
	[DestinationSerialNumber3],
	[DestinationEquipmentType3],
	[DestinationEquipmentModel3],
	[DestinationCompanyEquipmentID3],
	[SourceRegistrationID1],
	[SourceSerialNumber1],
	[SourceEquipmentType1],
	[SourceEquipmentModel1],
	[SourceCompanyEquipmentID1],
	[SourceRegistrationID2],
	[SourceSerialNumber2],
	[SourceEquipmentType2],
	[SourceEquipmentModel2],
	[SourceCompanyEquipmentID2],
	[SourceRegistrationID3],
	[SourceSerialNumber3],
	[SourceEquipmentType3],
	[SourceEquipmentModel3],
	[SourceCompanyEquipmentID3],
	[OperatorID],
	[EffectiveDate],
	[ExpirationDate],
	[ScheduledDate],
	[AutoComplete],
	[Flag01],
	[Flag02],
	[Flag03],
	[Flag04],
	[Flag05],
	[Flag06],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06],
	[ContactFirstName],
	[ContactSurname],
	[Date01],
	[Date02],
	[Date03],
	[Date04],
	[LegacyNumber],
	[Country],
	[ContactInfo],
	[AssociatedDocNumber],
	[AssociatedCLIN],
	[SubmittedToAccounting],
	[FuelCardID],
	[AssociatedTransportOrderNumber],
	[RequestedDateTime],
	[DispatchedDateTime],
	[ErrorFlag],
	[TransactionGuid],
	[SiteGuid],
	[LookupTransTypeIndex],
	[LookupTransactionStatusIndex],
	[LookupOriginApplicationIndex],
	[TransactionAliasGuid],
	[BillToCompanyGuid],
	[Destination1EquipmentGuid],
	[Destination2EquipmentGuid],
	[Destination3EquipmentGuid],
	[FinalStationIATAGuid],
	[FuelCardGuid],
	[ManagerCompanyGuid],
	[NextStationIATAGuid],
	[OperatorPersonnelGuid],
	[OriginStationIATAGuid],
	[OwnerCompanyGuid],
	[PreviousStationIATAGuid],
	[ShipperCompanyGuid],
	[ShipToCompanyGuid],
	[Source1EquipmentGuid],
	[Source2EquipmentGuid],
	[Source3EquipmentGuid],
	[SupplierCompanyGuid],
	[CarrierCompanyGuid],
	[ReasonCodeGuid],
	[OriginStationIATAID],
	[PreviousStationIATAID],
	[NextStationIATAID],
	[FinalStationIATAID],
	[OperatorName],
	[FuelAdditiveFlag],
	[IssuePoint],
	[IssuePointNumber],
	[RadioNumber]
	FROM FuelsManagerDB.dbo.tblTransactions WHERE TransactionGuid = @TransactionGuid

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionLineItems 
	(
	[SequenceID],
	[MeterStart],
	[MeterStop],
	[GrossQuantity],
	[Temperature],
	[Vcf],
	[Density],
	[Product],
	[ProductCode],
	[ProductType],
	[ProductPrice],
	[CLIN],
	[NetQuantity],
	[ContractNumber],
	[DestinationRegistrationID],
	[DestinationSerialNumber],
	[DestinationEquipmentType],
	[DestinationEquipmentModel],
	[DestinationCompanyEquipmentID],
	[DestinationCompartmentID],
	[SourceRegistrationID],
	[SourceSerialNumber],
	[SourceEquipmentType],
	[SourceEquipmentModel],
	[SourceCompanyEquipmentID],
	[SourceCompartmentID],
	[MeterFactor],
	[LineItemSequenceNumber],
	[BatchNumber],
	[DocumentNumber],
	[LineFill],
	[BottomVolume],
	[NetCapacity],
	[Customs],
	[ArmNumber],
	[LineNumber],
	[OperatorID],
	[TankStatus],
	[MeterStartDateTime],
	[MeterStopDateTime],
	[Pit],
	[RequestedDateTime],
	[DispatchedDateTime],
	[AcknowledgedDateTime],
	[OnLocationTime],
	[ValidationDateTime],
	[CompletionDateTime],
	[ReceiptVariance],
	[DifferentialPressure],
	[LoadRackVariance],
	[RequestedBy],
	[FreezePoint],
	[DeleteFlag],
	[StorageLocationID],
	[MeterID],
	[AdditiveProfileID],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[PresetAmount],
	[EngineeringUnitsIndex],
	[CustomerProductName],
	[CustomerProductCode],
	[TransactionInventoryDate],
	[COAWaiver],
	[COANote],
	[COAID],
	[Tax1],
	[Tax2],
	[Tax3],
	[Tax4],
	[Tax5],
	[TransVersion],
	[LoadingLocationID],
	[ImproperAdditization],
	[BrokenBlend],
	[ContaminatePrompt],
	[CompartmentsPreviouslyLoaded],
	[CompartmentsEmpty],
	[Flag01],
	[Flag02],
	[Flag03],
	[Flag04],
	[Flag05],
	[Flag06],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06],
	[OdometerHours],
	[EndDeliveryDate],
	[RequestedDeliveryDate],
	[InvoiceNumber],
	[InvoiceLineNumber],
	[AlternativeGrossVolume],
	[AlternativeNetVolume],
	[AlternativeUnits],
	[TankLevel],
	[TankLevelUnits],
	[Date01],
	[Date02],
	[Date03],
	[Date04],
	[NonDomesticPrice],
	[CurrencyUnit],
	[ExchangeRate],
	[QualityTestNumber],
	[Odometer],
	[DeliveryLocation],
	[Variance],
	[PartialFill],
	[MassQuantity],
	[NetManualValueFlag],
	[MassManualValueFlag],
	[GrossManualValueFlag],
	[VcfManualValueFlag],
	[TransactionLineItemGuid],
	[LookupTransactionStatusIndex],
	[LookupQualityIndex],
	[StorageLocationTankGuid],
	[AdditiveProfileGuid],
	[DestinationCompartmentEquipmentGuid],
	[DestinationEquipmentGuid],
	[OperatorPersonnelGuid],
	[ProductGuid],
	[SourceCompartmentEquipmentGuid],
	[SourceEquipmentGuid],
	[TransactionGuid],
	[CurrencyGuid],
	[OrderReferenceTransactionLineItemGuid],
	[LoadingLocationStationGuid],
	[MeterGuid],
	[PackageManualValueFlag],
	[CleanLineItem],
	[CleanLineDeductItem],
	[CleanLineDeductQuantity],
	[CleanLinePackQuantity]
	) 
	SELECT 
	[SequenceID],
	[MeterStart],
	[MeterStop],
	[GrossQuantity],
	[Temperature],
	[Vcf],
	[Density],
	[Product],
	[ProductCode],
	[ProductType],
	[ProductPrice],
	[CLIN],
	[NetQuantity],
	[ContractNumber],
	[DestinationRegistrationID],
	[DestinationSerialNumber],
	[DestinationEquipmentType],
	[DestinationEquipmentModel],
	[DestinationCompanyEquipmentID],
	[DestinationCompartmentID],
	[SourceRegistrationID],
	[SourceSerialNumber],
	[SourceEquipmentType],
	[SourceEquipmentModel],
	[SourceCompanyEquipmentID],
	[SourceCompartmentID],
	[MeterFactor],
	[LineItemSequenceNumber],
	[BatchNumber],
	[DocumentNumber],
	[LineFill],
	[BottomVolume],
	[NetCapacity],
	[Customs],
	[ArmNumber],
	[LineNumber],
	[OperatorID],
	[TankStatus],
	[MeterStartDateTime],
	[MeterStopDateTime],
	[Pit],
	[RequestedDateTime],
	[DispatchedDateTime],
	[AcknowledgedDateTime],
	[OnLocationTime],
	[ValidationDateTime],
	[CompletionDateTime],
	[ReceiptVariance],
	[DifferentialPressure],
	[LoadRackVariance],
	[RequestedBy],
	[FreezePoint],
	[DeleteFlag],
	[StorageLocationID],
	[MeterID],
	[AdditiveProfileID],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[PresetAmount],
	[EngineeringUnitsIndex],
	[CustomerProductName],
	[CustomerProductCode],
	[TransactionInventoryDate],
	[COAWaiver],
	[COANote],
	[COAID],
	[Tax1],
	[Tax2],
	[Tax3],
	[Tax4],
	[Tax5],
	[TransVersion],
	[LoadingLocationID],
	[ImproperAdditization],
	[BrokenBlend],
	[ContaminatePrompt],
	[CompartmentsPreviouslyLoaded],
	[CompartmentsEmpty],
	[Flag01],
	[Flag02],
	[Flag03],
	[Flag04],
	[Flag05],
	[Flag06],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06],
	[OdometerHours],
	[EndDeliveryDate],
	[RequestedDeliveryDate],
	[InvoiceNumber],
	[InvoiceLineNumber],
	[AlternativeGrossVolume],
	[AlternativeNetVolume],
	[AlternativeUnits],
	[TankLevel],
	[TankLevelUnits],
	[Date01],
	[Date02],
	[Date03],
	[Date04],
	[NonDomesticPrice],
	[CurrencyUnit],
	[ExchangeRate],
	[QualityTestNumber],
	[Odometer],
	[DeliveryLocation],
	[Variance],
	[PartialFill],
	[MassQuantity],
	[NetManualValueFlag],
	[MassManualValueFlag],
	[GrossManualValueFlag],
	[VcfManualValueFlag],
	[TransactionLineItemGuid],
	[LookupTransactionStatusIndex],
	[LookupQualityIndex],
	[StorageLocationTankGuid],
	[AdditiveProfileGuid],
	[DestinationCompartmentEquipmentGuid],
	[DestinationEquipmentGuid],
	[OperatorPersonnelGuid],
	[ProductGuid],
	[SourceCompartmentEquipmentGuid],
	[SourceEquipmentGuid],
	[TransactionGuid],
	[CurrencyGuid],
	[OrderReferenceTransactionLineItemGuid],
	[LoadingLocationStationGuid],
	[MeterGuid],
	[PackageManualValueFlag],
	[CleanLineItem],
	[CleanLineDeductItem],
	[CleanLineDeductQuantity],
	[CleanLinePackQuantity]
	FROM FuelsManagerDB.dbo.tbltransactionLineitems WHERE TransactionGuid = @TransactionGuid; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionTransportLineItems 
	(
	[TransportOrderNumber],
	[TransVersion],
	[LocationName],
	[Address1],
	[Address2],
	[City],
	[State],
	[Zip],
	[POCName],
	[POCPhone],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionTransportLineItemGuid],
	[TransactionGuid]
	) 
	SELECT 
	[TransportOrderNumber],
	[TransVersion],
	[LocationName],
	[Address1],
	[Address2],
	[City],
	[State],
	[Zip],
	[POCName],
	[POCPhone],
	d.[CreatedBy],
	d.[CreatedDate],
	d.[UpdatedBy],
	d.[UpdatedDate],
	[TransactionTransportLineItemGuid],
	[TransactionGuid]
	FROM FuelsManagerDB.dbo.tblTransactionTransportLineItems d
	WHERE d.TransactionGuid = @TransactionGuid ; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionLineItemUserData 
	(
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData4],
	[UserData5],
	[UserData6],
	[UserData7],
	[UserData8],
	[UserData9],
	[UserData10],
	[UserData11],
	[UserData12],
	[UserData13],
	[UserData14],
	[UserData15],
	[UserData16],
	[UserData17],
	[UserData18],
	[UserData19],
	[UserData20],
	[UserData21],
	[UserData22],
	[UserData23],
	[UserData24],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionLineItemUserDataGuid],
	[TransactionLineItemGuid]
	) 
	SELECT 
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData4],
	[UserData5],
	[UserData6],
	[UserData7],
	[UserData8],
	[UserData9],
	[UserData10],
	[UserData11],
	[UserData12],
	[UserData13],
	[UserData14],
	[UserData15],
	[UserData16],
	[UserData17],
	[UserData18],
	[UserData19],
	[UserData20],
	[UserData21],
	[UserData22],
	[UserData23],
	[UserData24],
	d.[CreatedBy],
	d.[CreatedDate],
	d.[UpdatedBy],
	d.[UpdatedDate],
	[TransactionLineItemUserDataGuid],
	d.[TransactionLineItemGuid]
	FROM FuelsManagerDB.dbo.tblTransactionLineItemUserData d
	INNER JOIN FuelsManagerDB.dbo.tblTransactionLineItems l ON d.TransactionLineItemGuid = l.TransactionLineItemGuid
	WHERE l.TransactionGuid = @TransactionGuid ; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionSubLineItems
	(
	[SequenceID],
	[Product],
	[ProductCode],
	[ProductType],
	[GrossQuantity],
	[NetQuantity],
	[Vcf],
	[Density],
	[Temperature],
	[Customs],
	[ArmNumber],
	[LineNumber],
	[BatchNumber],
	[LineFill],
	[BottomVolume],
	[NetCapacity],
	[TankStatus],
	[MeterFactor],
	[MeterStart],
	[MeterStop],
	[MeterStopDateTime],
	[MeterStartDateTime],
	[FreezePoint],
	[DifferentialPressure],
	[DosageRate],
	[DeleteFlag],
	[PresetAmount],
	[StorageLocationID],
	[MeterID],
	[COAID],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionInventoryDate],
	[Tax1],
	[Tax2],
	[Tax3],
	[Tax4],
	[Tax5],
	[TransVersion],
	[ImproperAdditization],
	[BrokenBlend],
	[Flag01],
	[Flag02],
	[Flag03],
	[Flag04],
	[Flag05],
	[Flag06],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06],
	[Date01],
	[Date02],
	[Date03],
	[Date04],
	[MassQuantity],
	[NetManualValueFlag],
	[MassManualValueFlag],
	[GrossManualValueFlag],
	[VcfManualValueFlag],
	[TransactionSubLineItemGuid],
	[LookupTransactionStatusIndex],
	[LookupQualityIndex],
	[TransactionLineItemGuid],
	[ProductGuid],
	[TransactionGuid],
	[StorageLocationTankGuid],
	[MeterGuid],
	[PackageManualValueFlag],
	[CleanLineItem],
	[CleanLineDeductItem],
	[CleanLineDeductQuantity],
	[CleanLinePackQuantity]
	)
	SELECT 
	s.[SequenceID],
	s.[Product],
	s.[ProductCode],
	s.[ProductType],
	s.[GrossQuantity],
	s.[NetQuantity],
	s.[Vcf],
	s.[Density],
	s.[Temperature],
	s.[Customs],
	s.[ArmNumber],
	s.[LineNumber],
	s.[BatchNumber],
	s.[LineFill],
	s.[BottomVolume],
	s.[NetCapacity],
	s.[TankStatus],
	s.[MeterFactor],
	s.[MeterStart],
	s.[MeterStop],
	s.[MeterStopDateTime],
	s.[MeterStartDateTime],
	s.[FreezePoint],
	s.[DifferentialPressure],
	s.[DosageRate],
	s.[DeleteFlag],
	s.[PresetAmount],
	s.[StorageLocationID],
	s.[MeterID],
	s.[COAID],
	s.[CreatedBy],
	s.[CreatedDate],
	s.[UpdatedBy],
	s.[UpdatedDate],
	s.[TransactionInventoryDate],
	s.[Tax1],
	s.[Tax2],
	s.[Tax3],
	s.[Tax4],
	s.[Tax5],
	s.[TransVersion],
	s.[ImproperAdditization],
	s.[BrokenBlend],
	s.[Flag01],
	s.[Flag02],
	s.[Flag03],
	s.[Flag04],
	s.[Flag05],
	s.[Flag06],
	s.[Number01],
	s.[Number02],
	s.[Number03],
	s.[Number04],
	s.[Number05],
	s.[Number06],
	s.[Date01],
	s.[Date02],
	s.[Date03],
	s.[Date04],
	s.[MassQuantity],
	s.[NetManualValueFlag],
	s.[MassManualValueFlag],
	s.[GrossManualValueFlag],
	s.[VcfManualValueFlag],
	s.[TransactionSubLineItemGuid],
	s.[LookupTransactionStatusIndex],
	s.[LookupQualityIndex],
	s.[TransactionLineItemGuid],
	s.[ProductGuid],
	s.[TransactionGuid],
	s.[StorageLocationTankGuid],
	s.[MeterGuid],
	s.[PackageManualValueFlag],
	s.[CleanLineItem],
	s.[CleanLineDeductItem],
	s.[CleanLineDeductQuantity],
	s.[CleanLinePackQuantity]
	FROM FuelsManagerDB.dbo.tblTransactionSubLineItems s 
	INNER JOIN FuelsManagerDB.dbo.tblTransactionLineItems l ON s.TransactionLineItemGuid = l.TransactionLineItemGuid 
	WHERE l.TransactionGuid = @TransactionGuid ; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionUserData  
	(
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData4],
	[UserData5],
	[UserData6],
	[UserData7],
	[UserData8],
	[UserData9],
	[UserData10],
	[UserData11],
	[UserData12],
	[UserData13],
	[UserData14],
	[UserData15],
	[UserData16],
	[UserData17],
	[UserData18],
	[UserData19],
	[UserData20],
	[UserData21],
	[UserData22],
	[UserData23],
	[UserData24],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionUserDataGuid] ,
	[TransactionGuid] 
	)
	SELECT 
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData4],
	[UserData5],
	[UserData6],
	[UserData7],
	[UserData8],
	[UserData9],
	[UserData10],
	[UserData11],
	[UserData12],
	[UserData13],
	[UserData14],
	[UserData15],
	[UserData16],
	[UserData17],
	[UserData18],
	[UserData19],
	[UserData20],
	[UserData21],
	[UserData22],
	[UserData23],
	[UserData24],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionUserDataGuid] ,
	[TransactionGuid] 
	FROM FuelsManagerDB.dbo.tblTransactionUserData WHERE TransactionGuid = @TransactionGuid; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionLinks
	(
	[OriginalTransID],
	[LinkedTransID],
	[Level],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionLinkGuid],
	[SiteGuid],
	[LinkedTransactionLineItemGuid],
	[TransactionLineItemGuid]
		)
	SELECT 
	[OriginalTransID],
	[LinkedTransID],
	[Level],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionLinkGuid],
	[SiteGuid],
	[LinkedTransactionLineItemGuid],
	[TransactionLineItemGuid]
	FROM FuelsManagerDB.dbo.tblTransactionLinks   WHERE @TransID IN (OriginalTransID, LinkedTransID)

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionWeightReadings 
	(
	[CompartmentID],
	[BeginQuantityValue],
	[RequestedQuantityValue],
	[FinalQuantityValue],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransVersion],
	[TransactionWeightReadingGuid],
	[TransactionGuid],
	[FuelsManagerVersionNumber],
	[SourceVersionNumber],
	[HistoricalFlag]
	)
	SELECT
	[CompartmentID],
	[BeginQuantityValue],
	[RequestedQuantityValue],
	[FinalQuantityValue],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransVersion],
	[TransactionWeightReadingGuid],
	[TransactionGuid],
	[FuelsManagerVersionNumber],
	[SourceVersionNumber],
	[HistoricalFlag]
	FROM FuelsManagerDB.dbo.tblTransactionWeightReadings WHERE TransactionGuid = @TransactionGuid; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionNotes
	(
	[Notes],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[AdditionalInformation],
	[TransactionNoteGuid],
	[TransactionGuid]
	)
	SELECT 
	[Notes],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[AdditionalInformation],
	[TransactionNoteGuid],
	[TransactionGuid]
	FROM FuelsManagerDB.dbo.tblTransactionNotes WHERE TransactionGuid = @TransactionGuid; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionSignature
	(
	[Signature],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionSignatureGuid],
	[TransactionGuid]
	)
	SELECT 
	[Signature],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[TransactionSignatureGuid],
	[TransactionGuid]
	FROM FuelsManagerDB.dbo.tblTransactionSignature WHERE TransactionGuid = @TransactionGuid; 

	INSERT INTO FuelsManagerDBArchive.dbo.tblTransactionPIDX 
	(
	[AuthorizationNumber],
	[SentFlag],
	[DateSent],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[BrokenBlend],
	[TransactionPIDXGuid],
	[PIDXProfileGuid],
	[TransactionGuid],
	[CompanyPersonnelToShipToBillToGuid]
	) 
	SELECT 
	[AuthorizationNumber],
	[SentFlag],
	[DateSent],
	[CreatedBy],
	[CreatedDate],
	[UpdatedBy],
	[UpdatedDate],
	[BrokenBlend],
	[TransactionPIDXGuid],
	[PIDXProfileGuid],
	[TransactionGuid],
	[CompanyPersonnelToShipToBillToGuid]
	FROM FuelsManagerDB.dbo.tblTransactionPIDX WHERE TransactionGuid = @TransactionGuid; 

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_InsertTransactionsToArchiveTable' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_GetTransactionsToBeArchived]    Script Date: 11/12/2013 14:28:36 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_GetTransactionsToBeArchived]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_GetTransactionsToBeArchived]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetTransactionsToBeArchived]    Script Date: 11/12/2013 14:28:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_GetTransactionsToBeArchived]
(
	@BeginDate DateTime,
	@EndDate DateTime
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetTransactionsToBeArchived] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @BeginDate: Beginning of date range to archive records.
	-- 2. @EndDate: End of date range to archive records.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	/* Step 1 : Find transactions that are old enough to become candidates for archiving. */
	INSERT INTO #TransToBeArchived (TransID, InventoryDate, SiteGuid, ProductGuid, TransTypeID)
	(SELECT t.TransID, t.InventoryDate, t.SiteGuid, l.ProductGuid, t.LookupTransTypeIndex FROM
		FuelsManagerDB.dbo.tblTransactions t LEFT JOIN FuelsManagerDB.dbo.tblTransactionLineItems l ON t.TransactionGuid = l.TransactionGuid WHERE 
		t.InventoryDate BETWEEN @BeginDate AND @EndDate); 
	/* Step 2 : Filter out transactions that are not closed out. These can not be archived. Supply Order, Payment and Recovery are 
		excluded from close out.*/
	/*DELETE FROM #TransToBeArchived WHERE 
		InventoryDate > ISNULL((SELECT Max(CloseOutDate) FROM FuelsManagerDB.dbo.tblCloseoutInventory c WHERE 
		#TransToBeArchived.SiteGuid = c.SiteGuid AND c.ProductGuid = #TransToBeArchived.ProductGuid), '1/1/1900') AND 
		#TransToBeArchived.TransTypeID <> 18 AND -- Exclude from close out check: Bulk Purchase Order and Fuel Order 
		#TransToBeArchived.TransTypeID <> 21 AND -- Payment 
		#TransToBeArchived.TransTypeID <> 22 ;   -- Recovery 
	*/

	/* Step 3 : Filter out transactions that are associated to other transactions that are not yet selected as candidates for archiving.*/
	CREATE TABLE #AssociatedTransactionsNotReady (TransID NVARCHAR(64) NOT NULL); 
	WITH X (TransID1, TransID2 ) 
	AS ( 
		SELECT OriginalTransID, LinkedTransID FROM tblTransactionLinks
		UNION ALL 
		SELECT OriginalTransID, LinkedTransID FROM X JOIN tblTransactionLinks ON TransID1 = LinkedTransID 
	) 

	INSERT INTO #AssociatedTransactionsNotReady (TransID) 
	(SELECT TransID2 FROM X WHERE TransID1 NOT IN (SELECT TransID FROM  #TransToBeArchived) );
	WITH X (TransID1, TransID2 ) 
	AS ( 
		SELECT OriginalTransID, LinkedTransID FROM tblTransactionLinks
		UNION ALL 
		SELECT OriginalTransID, LinkedTransID FROM X JOIN tblTransactionLinks ON TransID1 = LinkedTransID 
	) 

	INSERT INTO #AssociatedTransactionsNotReady (TransID)
	(SELECT TransID1 FROM X WHERE TransID2 NOT IN (SELECT TransID FROM  #TransToBeArchived) );

	DELETE FROM #TransToBeArchived WHERE TransID IN (SELECT TransID FROM #AssociatedTransactionsNotReady);
	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_GetTransactionsToBeArchived' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

/****** Object:  StoredProcedure [dbo].[usp_SystemDataArchive]    Script Date: 11/12/2013 14:29:28 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usp_SystemDataArchive]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usp_SystemDataArchive]
GO

/****** Object:  StoredProcedure [dbo].[usp_SystemDataArchive]    Script Date: 11/12/2013 14:29:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE Procedure [dbo].[usp_SystemDataArchive]
(
		@start_date datetime,
		@end_date datetime,
		@data_selected nvarchar(64)
)

AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_SystemDataArchive] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-08-30 07:54:10.4470770 -10:00
	-- Purpose: Archive Data to FuelsManagerDBArchive
	-- Notes:
	-- 1. @start_date: Beginning of date range to archive records.
	-- 2. @end_date: End of date range to archive records.
	-- 3. @data_selected: A string identifying the type of data to be archived
	------------------------------------------------------------------------------------------------------
	BEGIN TRY

	/* SET NOCOUNT ON added to prevent extra result sets from 
	  interfering with SELECT statements.
	  */
	SET NOCOUNT ON; 

	CREATE TABLE #MSG (line int IDENTITY(1,1) NOT NULL, LogTime DateTime, Status nvarchar(32), Info nvarchar(4000)); 
	DECLARE @MSG nvarchar(MAX)

	INSERT INTO #MSG (LogTime , Status , Info )
	(Select GetDate(), 'Info' AS Status, 'Current UTC Date = ' + CONVERT(nvarchar, GETUTCDATE(), 101) + 
	' Archiving records between ' + CAST(@start_date AS nvarchar) + ' and ' + CAST(@end_date AS nvarchar) 
	AS Info); 

	IF UPPER(@data_selected) = 'ACCOUNTING'
	BEGIN
		EXEC dbo.usp_ArchiveTransactions @start_date, @end_date 
	END
	ELSE 
	BEGIN
		BEGIN TRY
			IF @data_selected = 'ALARM LOG'
			BEGIN	
				EXEC dbo.usp_ArchiveAlarmAndEventLog  @start_date, @end_date
			END
			
			IF @data_selected = 'AUDIT LOG'
			BEGIN	
				EXEC dbo.usp_ArchiveAuditLog  @start_date, @end_date
			END
			
			ELSE IF @data_selected = 'MAINTENANCE'
			BEGIN
				EXEC dbo.usp_ArchiveMaintenanceData @start_date ,@end_date
			END

			ELSE IF @data_selected = 'QUALITY CONTROL'
			BEGIN
				EXEC dbo.usp_ArchiveQualityData @start_date ,@end_date
				EXEC dbo.usp_ArchiveTestAndTestSetResultsData @start_date ,@end_date
			END

			IF @@ERROR != 0
			BEGIN
				SET @MSG = ERROR_MESSAGE()
				INSERT INTO #MSG (LogTime , Status , Info ) 
				(SELECT  GetDate(), 'Error' AS Status, @MSG AS Info); 
			END

		END TRY
		BEGIN CATCH

			SET @MSG = ERROR_MESSAGE()
			INSERT INTO #MSG (LogTime , Status , Info ) 
			(SELECT  GetDate(), 'Error' AS Status, @MSG AS Info); 
		
		END CATCH
	END

	SELECT * FROM #MSG

	END TRY
	BEGIN CATCH  
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [dbo].usp_SystemDataArchive' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,16,1);      
	END CATCH    
	
END     



GO

