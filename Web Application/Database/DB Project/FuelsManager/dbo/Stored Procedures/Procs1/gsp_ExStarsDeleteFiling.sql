CREATE PROCEDURE [dbo].[gsp_ExStarsDeleteFiling]
  @ExStarsFilingsGuid		UNIQUEIDENTIFIER
AS
BEGIN
	if( exists ( 
		select 1 
		from [dbo].[tblExStarsFilings]
		where 1=1
		and ExStarsFilingsGuid = @ExStarsFilingsGuid
		and [ResponseLoaded] is not null and [ResponseLoaded] > '1/1/1900'))
	BEGIN
	DECLARE	@_ErrMessage NVARCHAR(2048)  
		SET @_ErrMessage = 'Error: Deleting a row from tblExStarsReportedErrors is not allowed if column ResponseLoaded has been set'  + CHAR(13)+CHAR(10)                 
		RAISERROR(@_ErrMessage,18,1); 
	END
	DELETE [dbo].[tblExStarsReportedErrors] where ExStarsFilingsGuid = @ExStarsFilingsGuid
	UPDATE [dbo].[tblExStarsFilings] 
	set [ReportType] = 'Replaced'
	where ExStarsFilingsGuid = @ExStarsFilingsGuid
END