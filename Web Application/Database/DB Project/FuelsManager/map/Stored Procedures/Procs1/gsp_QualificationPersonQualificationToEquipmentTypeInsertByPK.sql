CREATE PROCEDURE [map].[gsp_QualificationPersonQualificationToEquipmentTypeInsertByPK]
(
		@QualificationPersonQualificationToEquipmentTypeGuid uniqueidentifier=NULL OUTPUT
	,	@QualificationGuid uniqueidentifier=NULL
	,	@EquipmentTypeGuid uniqueidentifier=NULL
	,	@Sequence int=NULL
	,	@Instructor nvarchar(50)=NULL
	,	@DateCompleted datetimeoffset(7)=NULL
	,	@DateDue datetimeoffset(7)=NULL
	,	@ExpirationDate datetimeoffset(7)=NULL
	,	@ID varchar(25)=NULL
	,	@Rating nvarchar(20)=NULL
	,	@HistoricalRecord bit=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_QualificationPersonQualificationToEquipmentTypeInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7782767 -05:00
	-- Purpose: Insert into table [map].[tblQualificationPersonQualificationToEquipmentType]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @QualificationPersonQualificationToEquipmentTypeGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblQualificationPersonQualificationToEquipmentType] 
		(
			[QualificationPersonQualificationToEquipmentTypeGuid]
		,	[QualificationGuid]
		,	[EquipmentTypeGuid]
		,	[Sequence]
		,	[Instructor]
		,	[DateCompleted]
		,	[DateDue]
		,	[ExpirationDate]
		,	[ID]
		,	[Rating]
		,	[HistoricalRecord]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@QualificationPersonQualificationToEquipmentTypeGuid
		,	@QualificationGuid
		,	@EquipmentTypeGuid
		,	@Sequence
		,	@Instructor
		,	@DateCompleted
		,	@DateDue
		,	@ExpirationDate
		,	@ID
		,	@Rating
		,	@HistoricalRecord
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [map].[tblQualificationPersonQualificationToEquipmentType]           
		WHERE QualificationPersonQualificationToEquipmentTypeGuid=@QualificationPersonQualificationToEquipmentTypeGuid;
	
 
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
						+ 'Procedure Name: gsp_QualificationPersonQualificationToEquipmentTypeInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
