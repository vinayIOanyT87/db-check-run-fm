CREATE PROCEDURE [dbo].[usp_ErrorHandler]

AS
BEGIN
	SET NOCOUNT ON

	DECLARE @ENumber			INT					SET @ENumber      = ISNULL(ERROR_NUMBER(),          -1)
	DECLARE @ESeverity		INT					SET @ESeverity    = ISNULL(ERROR_SEVERITY(),        -1)
	DECLARE @EState			INT					SET @EState       = ISNULL(ERROR_STATE(),            0)	IF @EState = 0 SET @EState = 42
	DECLARE @EProcedure		NVARCHAR(126)		SET @EProcedure   = ISNULL(ERROR_PROCEDURE(), N'{N/A}')
	DECLARE @ELine				INT					SET @ELine			= ISNULL(ERROR_LINE(),            -1)
	DECLARE @EMessageRecv	NVARCHAR(2048)		SET @EMessageRecv = ISNULL(ERROR_MESSAGE(),        N'')
	DECLARE @EMessageSent	NVARCHAR(440)		SET @EMessageSent = N''

	IF ERROR_PROCEDURE() IS NOT NULL   SET @EMessageSent = N'Error %d, Level %d, State %d, Procedure %s, Line %d, Message: '
	SET @EMessageSent = @EMessageSent + ERROR_MESSAGE()
	RAISERROR(@EMessageSent, @ESeverity, @EState, @ENumber, @ESeverity, @EState, @EProcedure, @ELine)
END