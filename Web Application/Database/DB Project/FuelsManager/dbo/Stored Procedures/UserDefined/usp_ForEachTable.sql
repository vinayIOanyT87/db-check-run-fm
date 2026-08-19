CREATE PROCEDURE [dbo].[usp_ForEachTable]
@command1 NVARCHAR (2000), @replacechar NCHAR (1)=N'?', @command2 NVARCHAR (2000)=null, @command3 NVARCHAR (2000)=null, @whereand NVARCHAR (2000)=null, @precommand NVARCHAR (2000)=null, @postcommand NVARCHAR (2000)=null
AS
BEGIN

	-- Preprocessor won't replace within quotes so have to use STR().
	DECLARE @ch12MsCategory NVARCHAR(12)
	SET @ch12MsCategory = LTRIM(STR(CONVERT(INT, 0x0002)))

	IF @precommand IS NOT NULL
	BEGIN
		EXEC(@precommand)
	END

	-- Create the SELECT.
	DECLARE @retval INT

   EXEC(N''
		+ 'declare hCForEachTableLAL cursor global for '
		+ '   select ''['' + replace(schema_name(syso.schema_id), N'']'', N'']]'') + '']'' + ''.'' + ''['' + '
		+ '                  replace(object_name(o.id), N'']'', N'']]'') + '']'' '
		+ '     from dbo.sysobjects  o '
		+ '     join sys.all_objects syso '
		+ '       on o.id = syso.object_id '
		+ '    where objectproperty(o.id, N''IsUserTable'') = 1 '
		+ '      and o.category & ' + @ch12MsCategory + N' = 0 '
		+ '    order by o.name '
		+ @whereand)

	SET @retval = @@error
	
	IF @retval = 0
	BEGIN
		EXEC @retval = dbo.usp_foreach_worker @command1, @replacechar, @command2, @command3, 0
	END
	
	IF @retval = 0 AND @postcommand IS NOT NULL
	BEGIN
		EXEC(@postcommand)
	END

	RETURN @retval
END