CREATE PROCEDURE [dbo].[gsp_EquipmentQualityTagLogInsertByPK]
(
		@EquipmentQualityTagLogGuid uniqueidentifier=NULL OUTPUT
	,	@QualityTagName nvarchar(50)=NULL
	,	@EquipmentID nvarchar(50)=NULL
	,	@EquipmentType nvarchar(50)=NULL
	,	@TaggedDate datetimeoffset(7)=NULL
	,	@TaggedBy nvarchar(50)=NULL
	,	@Memo nvarchar(1000)=NULL
	,	@RemovedDate datetimeoffset(7)=NULL
	,	@RemovedBy nvarchar(255)=NULL
	,	@DeleteFlag bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@TagNumber int=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@EquipmentGuid uniqueidentifier=NULL
	,	@QualityTagGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_EquipmentQualityTagLogInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.1562767 -05:00
	-- Purpose: Insert into table [dbo].[tblEquipmentQualityTagLog]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @EquipmentQualityTagLogGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblEquipmentQualityTagLog] 
		(
			[EquipmentQualityTagLogGuid]
		,	[QualityTagName]
		,	[EquipmentID]
		,	[EquipmentType]
		,	[TaggedDate]
		,	[TaggedBy]
		,	[Memo]
		,	[RemovedDate]
		,	[RemovedBy]
		,	[DeleteFlag]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[TagNumber]
		,	[SiteGuid]
		,	[EquipmentGuid]
		,	[QualityTagGuid]
		)
		VALUES
		(
			@EquipmentQualityTagLogGuid
		,	@QualityTagName
		,	@EquipmentID
		,	@EquipmentType
		,	@TaggedDate
		,	@TaggedBy
		,	@Memo
		,	@RemovedDate
		,	@RemovedBy
		,	@DeleteFlag
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@TagNumber
		,	@SiteGuid
		,	@EquipmentGuid
		,	@QualityTagGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblEquipmentQualityTagLog]           
		WHERE EquipmentQualityTagLogGuid=@EquipmentQualityTagLogGuid;
	
 
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
						+ 'Procedure Name: gsp_EquipmentQualityTagLogInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
