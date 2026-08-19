 
CREATE PROCEDURE [map].[gsp_EntityPersonnelLicenseToSiteInsertByPK]
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
	-- Stored procedure: [map].[gsp_EntityPersonnelLicenseToSiteInsertByPK]
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.002 / 2013-12-30 14:44:52.5164393 -05:00
	-- Purpose: Insert into table [map].[tblEntityPersonnelLicenseToSite]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		SET @MappingGuid=NEWID();
		INSERT INTO [map].[tblEntityPersonnelLicenseToSite]
		(
			[QualificationGuid]
		,	[SiteGuid]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[PersonnelLicenseToSiteGuid]
		,	[AssignedFromSiteGuid]
		)
		VALUES
		(
			@EntityRecordGuid
		,	@AssignedToSiteGuid
		,	@CreatedDate
		,	@CreatedBy
		,	@CreatedDate
		,	@CreatedBy
		,	@MappingGuid
		,	@AssignedFromSiteGuid
		)
 
		SELECT @_RowVersion=_RowVersion
		FROM [map].[tblEntityPersonnelLicenseToSite]
		WHERE [PersonnelLicenseToSiteGuid] = @MappingGuid
 
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
		+ 'Procedure Name: gsp_EntityPersonnelLicenseToSiteInsertByPK' + CHAR(13)+CHAR(10)
		+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,18,1);
	END CATCH
	
END
