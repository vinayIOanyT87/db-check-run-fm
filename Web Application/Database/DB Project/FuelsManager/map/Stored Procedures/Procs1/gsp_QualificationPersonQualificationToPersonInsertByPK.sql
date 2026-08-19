CREATE PROCEDURE [map].[gsp_QualificationPersonQualificationToPersonInsertByPK]
(
		@QualificationPersonQualificationToPersonGuid uniqueidentifier=NULL OUTPUT
	,	@QualificationGuid uniqueidentifier=NULL
	,	@PersonnelGuid uniqueidentifier=NULL
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
	-- Stored procedure: [map].[gsp_QualificationPersonQualificationToPersonInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.7802767 -05:00
	-- Purpose: Insert into table [map].[tblQualificationPersonQualificationToPerson]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @QualificationPersonQualificationToPersonGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [map].[tblQualificationPersonQualificationToPerson] 
		(
			[QualificationPersonQualificationToPersonGuid]
		,	[QualificationGuid]
		,	[PersonnelGuid]
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
			@QualificationPersonQualificationToPersonGuid
		,	@QualificationGuid
		,	@PersonnelGuid
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
		FROM [map].[tblQualificationPersonQualificationToPerson]           
		WHERE QualificationPersonQualificationToPersonGuid=@QualificationPersonQualificationToPersonGuid;
	
 
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
						+ 'Procedure Name: gsp_QualificationPersonQualificationToPersonInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
