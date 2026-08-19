CREATE PROCEDURE [map].[gsp_QualificationEquipmentTagAndLicenseToEquipmentInsertByPK]
(
		@QualificationEquipmentTagAndLicenseToEquipmentGuid uniqueidentifier=NULL OUTPUT
	,	@QualificationGuid uniqueidentifier=NULL
	,	@EquipmentGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_QualificationEquipmentTagAndLicenseToEquipmentInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7512767 -05:00
	-- Purpose: Insert into table [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @QualificationEquipmentTagAndLicenseToEquipmentGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblQualificationEquipmentTagAndLicenseToEquipment] 
		(
			[QualificationEquipmentTagAndLicenseToEquipmentGuid]
		,	[QualificationGuid]
		,	[EquipmentGuid]
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
			@QualificationEquipmentTagAndLicenseToEquipmentGuid
		,	@QualificationGuid
		,	@EquipmentGuid
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
		FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment]           
		WHERE QualificationEquipmentTagAndLicenseToEquipmentGuid=@QualificationEquipmentTagAndLicenseToEquipmentGuid;
	
 
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
						+ 'Procedure Name: gsp_QualificationEquipmentTagAndLicenseToEquipmentInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
