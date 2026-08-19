CREATE PROCEDURE [dbo].[gsp_EquipmentMaintenanceLogInsertByPK]
(
		@EquipmentMaintenanceLogGuid uniqueidentifier=NULL OUTPUT
	,	@EquipmentID nvarchar(50)=NULL
	,	@EquipmentType nvarchar(50)=NULL
	,	@OperatorID nvarchar(50)=NULL
	,	@MaintenanceReason nvarchar(50)=NULL
	,	@InServiceFlag tinyint=NULL
	,	@ChangeDate datetimeoffset(7)=NULL
	,	@EstReturnToServiceDate datetimeoffset(7)=NULL
	,	@WorkOrder nvarchar(20)=NULL
	,	@Memo nvarchar(1000)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@EquipmentGuid uniqueidentifier=NULL
	,	@MaintenanceReasonGuid uniqueidentifier=NULL
	,	@OperatorPersonnelGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EquipmentMaintenanceLogInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1532767 -05:00
	-- Purpose: Insert into table [dbo].[tblEquipmentMaintenanceLog]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @EquipmentMaintenanceLogGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblEquipmentMaintenanceLog] 
		(
			[EquipmentMaintenanceLogGuid]
		,	[EquipmentID]
		,	[EquipmentType]
		,	[OperatorID]
		,	[MaintenanceReason]
		,	[InServiceFlag]
		,	[ChangeDate]
		,	[EstReturnToServiceDate]
		,	[WorkOrder]
		,	[Memo]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[SiteGuid]
		,	[EquipmentGuid]
		,	[MaintenanceReasonGuid]
		,	[OperatorPersonnelGuid]
		)
		VALUES
		(
			@EquipmentMaintenanceLogGuid
		,	@EquipmentID
		,	@EquipmentType
		,	@OperatorID
		,	@MaintenanceReason
		,	@InServiceFlag
		,	@ChangeDate
		,	@EstReturnToServiceDate
		,	@WorkOrder
		,	@Memo
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@SiteGuid
		,	@EquipmentGuid
		,	@MaintenanceReasonGuid
		,	@OperatorPersonnelGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEquipmentMaintenanceLog]           
		WHERE EquipmentMaintenanceLogGuid=@EquipmentMaintenanceLogGuid;
	
 
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
						+ 'Procedure Name: gsp_EquipmentMaintenanceLogInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
