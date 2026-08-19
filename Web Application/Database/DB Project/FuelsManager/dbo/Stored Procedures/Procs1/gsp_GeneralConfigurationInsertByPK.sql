CREATE PROCEDURE [dbo].[gsp_GeneralConfigurationInsertByPK]
(
		@GeneralConfigurationGuid uniqueidentifier=NULL OUTPUT
	,	@Method int=NULL
	,	@ConsortiumFlag bit=NULL
	,	@ShowDeletedTrxFlag bit=NULL
	,	@AllowUndeleteFlag bit=NULL
	,	@ReverseTrxDateMode nvarchar(15)=NULL
	,	@ForcedCloseout int=NULL
	,	@SecurityCode nvarchar(50)=NULL
	,	@AuthorizationCode nvarchar(50)=NULL
	,	@MeterTolerance float=NULL
	,	@CreatedBy udtUserID=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@SetBeginInventoryToZeroFlag bit=NULL
	,	@SiteGuid uniqueidentifier=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_GeneralConfigurationInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.2362767 -05:00
	-- Purpose: Insert into table [dbo].[tblGeneralConfiguration]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @GeneralConfigurationGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblGeneralConfiguration] 
		(
			[GeneralConfigurationGuid]
		,	[Method]
		,	[ConsortiumFlag]
		,	[ShowDeletedTrxFlag]
		,	[AllowUndeleteFlag]
		,	[ReverseTrxDateMode]
		,	[ForcedCloseout]
		,	[SecurityCode]
		,	[AuthorizationCode]
		,	[MeterTolerance]
		,	[CreatedBy]
		,	[CreatedDate]
		,	[UpdatedBy]
		,	[UpdatedDate]
		,	[SetBeginInventoryToZeroFlag]
		,	[SiteGuid]
		)
		VALUES
		(
			@GeneralConfigurationGuid
		,	@Method
		,	@ConsortiumFlag
		,	@ShowDeletedTrxFlag
		,	@AllowUndeleteFlag
		,	@ReverseTrxDateMode
		,	@ForcedCloseout
		,	@SecurityCode
		,	@AuthorizationCode
		,	@MeterTolerance
		,	@CreatedBy
		,	@CreatedDate
		,	@UpdatedBy
		,	@UpdatedDate
		,	@SetBeginInventoryToZeroFlag
		,	@SiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblGeneralConfiguration]           
		WHERE GeneralConfigurationGuid=@GeneralConfigurationGuid;
	
 
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
						+ 'Procedure Name: gsp_GeneralConfigurationInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
