 
CREATE PROCEDURE [map].[gsp_EntityListViewToSiteInsertByPK]
(
		@MappingGuid uniqueidentifier=NULL OUTPUT
	,	@EntityRecordGuid uniqueidentifier=NULL
	,	@AssignedFromSiteGuid uniqueidentifier=NULL
	,	@AssignedToSiteGuid uniqueidentifier=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[gsp_EntityListViewToSiteInsertByPK]
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.002 / 2013-12-30 14:44:52.5144389 -05:00
	-- Purpose: Insert into table [map].[tblEntityListViewToSite]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		SET @MappingGuid=NEWID();
		INSERT INTO [map].[tblEntityListViewToSite]
		(
			[ListViewToSiteGuid]
		,	[ListViewGuid]
		,	[SiteGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[AssignedFromSiteGuid]
		)
		VALUES
		(
			@MappingGuid
		,	@EntityRecordGuid
		,	@AssignedToSiteGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@CreatedDate
		,	@CreatedBy
		,	@AssignedFromSiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion
		FROM [map].[tblEntityListViewToSite]
		WHERE [ListViewToSiteGuid] = @MappingGuid
 
	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)
		, @_ErrNumber INT
		, @_ErrProcName NVARCHAR(126)
		, @_ErrLineNumber INT;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		IF(@_ErrNumber = 547 AND CHARINDEX('Uniqueness',@_ErrMessage,0) <> 0)
			RAISERROR('Operation would result in duplicate identifiers.',16,1);
		ELSE
		BEGIN
			SET @_ErrProcName= ERROR_PROCEDURE();
			SET @_ErrLineNumber = ERROR_LINE();
			SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)
			+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)
			+ 'Procedure Name: gsp_EntityListViewToSiteInsertByPK' + CHAR(13)+CHAR(10)
			+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
			RAISERROR(@_ErrMessage,18,1);
		END
	END CATCH
	
END
