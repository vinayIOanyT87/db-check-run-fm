CREATE PROCEDURE [dbo].[gsp_LoadArmsInsertByPK]
(
		@LoadArmGuid uniqueidentifier=NULL OUTPUT
	,	@LoadRackText nvarchar(9)=NULL
	,	@Enabled bit=NULL
	,	@SwingArm bit=NULL
	,	@BayAArmNumber int=NULL
	,	@BayBArmNumber int=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@LookupPresetTypeIndex int=NULL
	,	@BayAStationGuid uniqueidentifier=NULL
	,	@BayBStationGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_LoadArmsInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2662767 -05:00
	-- Purpose: Insert into table [dbo].[tblLoadArms]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @LoadArmGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblLoadArms] 
		(
			[LoadArmGuid]
		,	[LoadRackText]
		,	[Enabled]
		,	[SwingArm]
		,	[BayAArmNumber]
		,	[BayBArmNumber]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[LookupPresetTypeIndex]
		,	[BayAStationGuid]
		,	[BayBStationGuid]
		)
		VALUES
		(
			@LoadArmGuid
		,	@LoadRackText
		,	@Enabled
		,	@SwingArm
		,	@BayAArmNumber
		,	@BayBArmNumber
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@LookupPresetTypeIndex
		,	@BayAStationGuid
		,	@BayBStationGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblLoadArms]           
		WHERE LoadArmGuid=@LoadArmGuid;
	
 
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
						+ 'Procedure Name: gsp_LoadArmsInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
