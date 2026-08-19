CREATE PROCEDURE [dbo].[gsp_AutoDistributionRuleInsertByPK]
(
		@AutoDistributionRuleGuid uniqueidentifier=NULL OUTPUT
	,	@SiteGuid uniqueidentifier=NULL
	,	@RuleID nvarchar(50)=NULL
	,	@RuleDescription nvarchar(255)=NULL
	,	@RuleEnabled bit=NULL
	,	@DefaultEOM bit=NULL
	,	@TransactionAliasGuid uniqueidentifier=NULL
	,	@DefaultReasonCodeGuid uniqueidentifier=NULL
	,	@DefaultNotes nvarchar(1000)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_AutoDistributionRuleInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.0852767 -05:00
	-- Purpose: Insert into table [dbo].[tblAutoDistributionRule]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @AutoDistributionRuleGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblAutoDistributionRule] 
		(
			[AutoDistributionRuleGuid]
		,	[SiteGuid]
		,	[RuleID]
		,	[RuleDescription]
		,	[RuleEnabled]
		,	[DefaultEOM]
		,	[TransactionAliasGuid]
		,	[DefaultReasonCodeGuid]
		,	[DefaultNotes]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		)
		VALUES
		(
			@AutoDistributionRuleGuid
		,	@SiteGuid
		,	@RuleID
		,	@RuleDescription
		,	@RuleEnabled
		,	@DefaultEOM
		,	@TransactionAliasGuid
		,	@DefaultReasonCodeGuid
		,	@DefaultNotes
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblAutoDistributionRule]           
		WHERE AutoDistributionRuleGuid=@AutoDistributionRuleGuid;
	
 
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
						+ 'Procedure Name: gsp_AutoDistributionRuleInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
