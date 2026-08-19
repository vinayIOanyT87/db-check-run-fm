CREATE PROCEDURE [dbo].[usp_GetNextManualBOLNumber]
	@SiteGuid uniqueidentifier,
	@UpdatedBy nvarchar(50),
	@UpdatedDate datetimeoffset
AS
	DECLARE @NextNumberTable AS Table (NextNumber int)
	DECLARE @NextNumber int

	UPDATE tblSites SET ManualBOLNextNumber = ManualBOLNextNumber + 1,
                                UpdatedDate = @UpdatedDate, UpdatedBy = @UpdatedBy
                                OUTPUT DELETED.ManualBOLNextNumber INTO @NextNumberTable (NextNumber)
                                WHERE SiteGuid = @SiteGuid

	SELECT NextNumber FROM @NextNumberTable

	RETURN 0
