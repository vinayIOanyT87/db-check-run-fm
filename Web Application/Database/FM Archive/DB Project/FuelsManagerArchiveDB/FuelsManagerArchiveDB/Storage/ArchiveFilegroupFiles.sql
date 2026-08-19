/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveDataStart],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveDataStart.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_Start];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2014],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2014.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2014];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2015],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2015.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2015];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2016],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2016.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2016];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2017],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2017.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2017];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2018],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2018.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2018];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2019],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2019.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2019];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2020],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2020.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2020];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2021],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2021.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2021];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2022],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2022.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2022];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2023],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2023.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2023];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2024],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2024.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2024];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2025],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2025.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2025];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2026],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2026.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2026];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2027],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2027.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2027];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2028],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2028.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2028];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2029],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2029.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2029];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2030],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2030.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2030];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2031],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2031.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2031];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2032],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2032.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2032];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2033],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2033.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2033];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2034],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2034.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2034];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2035],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2035.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2035];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2036],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2036.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2036];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2037],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2037.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2037];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2038],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2038.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2038];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [ArchiveData2039],
		FILENAME = '$(FMArchiveDefaultDataPath)ArchiveData2039.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_Archive_2039];
GO
