CREATE PROCEDURE [dbo].[gsp_ProcessVariableSiteInsertByPK]
(
		@ProcessVariableSiteGuid uniqueidentifier=NULL OUTPUT
	,	@LookupProcessVariableTypeIndex int=NULL
	,	@InstanceNumber int=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@OPCConnectionGuid uniqueidentifier=NULL
	,	@OPCItemID nvarchar(255)=NULL
	,	@DataType int=NULL
	,	@ServerEngineeringUnitsIndex int=NULL
	,	@Quality smallint=NULL
	,	@SIValue varbinary=NULL
	,	@LookupSIValueVariantTypeIndex int=NULL
	,	@DateTimeStamp datetimeoffset(7)=NULL
	,	@Maximum varbinary=NULL
	,	@LookupMaximumVariantTypeIndex int=NULL
	,	@Minimum varbinary=NULL
	,	@LookupMinimumVariantTypeIndex int=NULL
	,	@DataTypeEnabled bit=NULL
	,	@Input bit=NULL
	,	@InputEnabled bit=NULL
	,	@MessageApplicationStringGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_ProcessVariableSiteInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.3752767 -05:00
	-- Purpose: Insert into table [dbo].[tblProcessVariableSite]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @ProcessVariableSiteGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblProcessVariableSite] 
		(
			[ProcessVariableSiteGuid]
		,	[LookupProcessVariableTypeIndex]
		,	[InstanceNumber]
		,	[SiteGuid]
		,	[OPCConnectionGuid]
		,	[OPCItemID]
		,	[DataType]
		,	[ServerEngineeringUnitsIndex]
		,	[Quality]
		,	[SIValue]
		,	[LookupSIValueVariantTypeIndex]
		,	[DateTimeStamp]
		,	[Maximum]
		,	[LookupMaximumVariantTypeIndex]
		,	[Minimum]
		,	[LookupMinimumVariantTypeIndex]
		,	[DataTypeEnabled]
		,	[Input]
		,	[InputEnabled]
		,	[MessageApplicationStringGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@ProcessVariableSiteGuid
		,	@LookupProcessVariableTypeIndex
		,	@InstanceNumber
		,	@SiteGuid
		,	@OPCConnectionGuid
		,	@OPCItemID
		,	@DataType
		,	@ServerEngineeringUnitsIndex
		,	@Quality
		,	@SIValue
		,	@LookupSIValueVariantTypeIndex
		,	@DateTimeStamp
		,	@Maximum
		,	@LookupMaximumVariantTypeIndex
		,	@Minimum
		,	@LookupMinimumVariantTypeIndex
		,	@DataTypeEnabled
		,	@Input
		,	@InputEnabled
		,	@MessageApplicationStringGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblProcessVariableSite]           
		WHERE ProcessVariableSiteGuid=@ProcessVariableSiteGuid;
	
 
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
						+ 'Procedure Name: gsp_ProcessVariableSiteInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
