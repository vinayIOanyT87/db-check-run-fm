CREATE PROCEDURE [dbo].[gsp_PointServiceInsertByPK]
(
		@PointServiceGuid uniqueidentifier=NULL OUTPUT
	,	@Hostname nvarchar(256)=NULL
	,	@LastPingTime datetimeoffset(7)=NULL
	,	@PingIntervalInSeconds int=NULL
	,	@HealthStatusIndex int=NULL
	,	@MaxNumberOfPoints int=NULL
	,	@PercentCpuUtilization float=NULL
	,	@PercentCpuUtilizationThrottleLevel float=NULL
	,	@PercentMemoryUtilization float=NULL
	,	@PercentMemoryUtilizationThrottleLevel float=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_ClusterIdx bigint=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_PointServiceInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2015-09-25 09:13:46.5371040 -10:00
	-- Purpose: Insert into table [dbo].[tblPointService]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @PointServiceGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblPointService] 
		(
			[PointServiceGuid]
		,	[Hostname]
		,	[LastPingTime]
		,	[PingIntervalInSeconds]
		,	[HealthStatusIndex]
		,	[MaxNumberOfPoints]
		,	[PercentCpuUtilization]
		,	[PercentCpuUtilizationThrottleLevel]
		,	[PercentMemoryUtilization]
		,	[PercentMemoryUtilizationThrottleLevel]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@PointServiceGuid
		,	@Hostname
		,	@LastPingTime
		,	@PingIntervalInSeconds
		,	@HealthStatusIndex
		,	@MaxNumberOfPoints
		,	@PercentCpuUtilization
		,	@PercentCpuUtilizationThrottleLevel
		,	@PercentMemoryUtilization
		,	@PercentMemoryUtilizationThrottleLevel
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblPointService]           
		WHERE PointServiceGuid=@PointServiceGuid;
	
 
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
						+ 'Procedure Name: gsp_PointServiceInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
