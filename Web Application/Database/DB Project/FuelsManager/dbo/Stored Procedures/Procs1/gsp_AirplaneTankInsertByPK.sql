CREATE PROCEDURE [dbo].[gsp_AirplaneTankInsertByPK]
(
		@TankGuid uniqueidentifier=NULL OUTPUT
	,	@Alias nvarchar(50)=NULL
	,	@AirlineTankId nvarchar(50)=NULL
	,	@Description nvarchar(250)=NULL
	,	@Capacity float=NULL
	,	@Position int=NULL
	,	@Location int=NULL
	,	@DisplayOrder int=NULL
	,	@EquipmentTypeGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AirplaneTankInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0032767 -05:00
	-- Purpose: Insert into table [dbo].[tblAirplaneTank]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @TankGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAirplaneTank] 
		(
			[TankGuid]
		,	[Alias]
		,	[AirlineTankId]
		,	[Description]
		,	[Capacity]
		,	[Position]
		,	[Location]
		,	[DisplayOrder]
		,	[EquipmentTypeGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@TankGuid
		,	@Alias
		,	@AirlineTankId
		,	@Description
		,	@Capacity
		,	@Position
		,	@Location
		,	@DisplayOrder
		,	@EquipmentTypeGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAirplaneTank]           
		WHERE TankGuid=@TankGuid;
	
 
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
						+ 'Procedure Name: gsp_AirplaneTankInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
