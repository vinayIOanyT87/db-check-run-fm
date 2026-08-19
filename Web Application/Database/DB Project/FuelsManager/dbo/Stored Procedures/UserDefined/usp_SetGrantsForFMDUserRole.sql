CREATE PROCEDURE [dbo].[usp_SetGrantsForFMDUserRole]

AS
BEGIN
	SET NOCOUNT ON
	
	-- Want to emit XML for Database.sqlpermissions file? 
	DECLARE @bEmitXml BIT
	SET @bEmitXml = 0
	
	-- Create 'database role' FMDUserRole if it does not exist. 
	IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'FMDUserRole' and type_desc = 'DATABASE_ROLE')
	BEGIN
		CREATE ROLE FMDUserRole
	END

	---------------------------------------------------------------------------- 
	-- Grant access to tables and views. 
	---------------------------------------------------------------------------- 

	DECLARE @sSql NVARCHAR(4000)
	DECLARE @sName sysname

	DECLARE cur CURSOR FOR
		SELECT name
		  FROM sys.objects
		 WHERE (type_desc = 'USER_TABLE' AND name LIKE 'tbl%' AND name NOT LIKE 'tbl%_Backup%')
			 OR (type_desc = 'VIEW'       AND name LIKE 'vw%')
			AND is_ms_shipped = 0

	OPEN cur
	FETCH NEXT FROM cur INTO @sName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @sSql = 'GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE ON ' + QUOTENAME(@sName) + ' TO FMDUserRole '
		--PRINT @sSql 
		EXEC sp_executesql @sSql


		-- Emit XML for Database Model. Done here to guarantee that the database 
		-- objects and permissions match. 
		IF @bEmitXml = 1
		BEGIN
			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>DELETE</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>INSERT</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>REFERENCES</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>SELECT</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>UPDATE</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

		END

		FETCH NEXT FROM cur INTO @sName
	END
	
	CLOSE cur
	DEALLOCATE cur


	---------------------------------------------------------------------------- 
	-- Grant access to scalar functions. 
	---------------------------------------------------------------------------- 

	DECLARE cur CURSOR FOR
		SELECT name
		  FROM sys.objects
		 WHERE type_desc IN ('SQL_SCALAR_FUNCTION' /* , 'CLR_SCALAR_FUNCTION' */  )
			AND is_ms_shipped = 0

	OPEN cur
	FETCH NEXT FROM cur INTO @sName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @sSql = 'GRANT EXECUTE, REFERENCES ON ' + QUOTENAME(@sName) + ' TO FMDUserRole '
		--PRINT @sSql 
		EXEC sp_executesql @sSql

		-- Emit XML for Database Model. Done here to guarantee that the database 
		-- objects and permissions match. 
		IF @bEmitXml = 1
		BEGIN
			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>EXECUTE</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

			PRINT '<PermissionStatement Action="GRANT">'
			PRINT '   <Permission>REFERENCES</Permission>'
			PRINT '   <Grantee>FMDUserRole</Grantee>'
			PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
			PRINT '   <Grantor>dbo</Grantor>'
			PRINT '</PermissionStatement>'
			PRINT ' '

		END

		FETCH NEXT FROM cur INTO @sName
	END
	
	CLOSE cur
	DEALLOCATE cur


	---------------------------------------------------------------------------- 
	-- Grant access to table-valued functions. 
	---------------------------------------------------------------------------- 

	DECLARE cur CURSOR FOR
		SELECT name
		  FROM sys.objects
		 WHERE type_desc IN ('SQL_TABLE_VALUED_FUNCTION', 'SQL_INLINE_TABLE_VALUED_FUNCTION' /* , 'CLR_TABLE_VALUED_FUNCTION' */  )
			AND is_ms_shipped = 0

	OPEN cur
	FETCH NEXT FROM cur INTO @sName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @sSql = 'GRANT REFERENCES, SELECT ON ' + QUOTENAME(@sName) + ' TO FMDUserRole '
		--PRINT @sSql 
		EXEC sp_executesql @sSql

		-- Emit XML for Database Model. Done here to guarantee that the database 
		-- objects and permissions match. 
		PRINT '<PermissionStatement Action="GRANT">'
		PRINT '   <Permission>REFERENCES</Permission>'
		PRINT '   <Grantee>FMDUserRole</Grantee>'
		PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
		PRINT '   <Grantor>dbo</Grantor>'
		PRINT '</PermissionStatement>'
		PRINT ' '

		PRINT '<PermissionStatement Action="GRANT">'
		PRINT '   <Permission>SELECT</Permission>'
		PRINT '   <Grantee>FMDUserRole</Grantee>'
		PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>'
		PRINT '   <Grantor>dbo</Grantor>'
		PRINT '</PermissionStatement>'
		PRINT ' '

		FETCH NEXT FROM cur INTO @sName
	END

	CLOSE cur
	DEALLOCATE cur


	---------------------------------------------------------------------------- 
	-- Grant access to stored procedures. 
	---------------------------------------------------------------------------- 

	DECLARE cur CURSOR FOR
		SELECT name
		  FROM sys.objects
		 WHERE type_desc IN ('SQL_STORED_PROCEDURE', 'CLR_STORED_PROCEDURE', 'EXTENDED_STORED_PROCEDURE')
			AND name LIKE 'fm%' OR name LIKE 'ASC_%'
			AND is_ms_shipped = 0

	OPEN cur
	FETCH NEXT FROM cur INTO @sName

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @sSql = 'GRANT EXECUTE ON ' + QUOTENAME(@sName) + ' TO FMDUserRole '
		--PRINT @sSql 
		EXEC sp_executesql @sSql

		-- Emit XML for Database Model. Done here to guarantee that the database 
		-- objects and permissions match. 
		PRINT '<PermissionStatement Action="GRANT">' 
		PRINT '   <Permission>EXECUTE</Permission>' 
		PRINT '   <Grantee>FMDUserRole</Grantee>' 
		PRINT '   <Object Name="' + @sName + '" Schema="dbo" Type="OBJECT"/>' 
		PRINT '   <Grantor>dbo</Grantor>'
		PRINT '</PermissionStatement>' 
		PRINT ' '

		FETCH NEXT FROM cur INTO @sName
	END

	CLOSE cur
	DEALLOCATE cur
END