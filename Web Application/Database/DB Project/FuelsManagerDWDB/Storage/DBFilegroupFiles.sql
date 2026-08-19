/*
Do not change the database path or name variables.
Any sqlcmd variables will be properly substituted during 
build and deployment.
*/
ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactDataStart],
		FILENAME = '$(FMDWDefaultDataPath)FactDataStart.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_Start];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2000],
		FILENAME = '$(FMDWDefaultDataPath)FactData2000.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2000];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2001],
		FILENAME = '$(FMDWDefaultDataPath)FactData2001.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2001];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2002],
		FILENAME = '$(FMDWDefaultDataPath)FactData2002.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2002];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2003],
		FILENAME = '$(FMDWDefaultDataPath)FactData2003.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2003];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2004],
		FILENAME = '$(FMDWDefaultDataPath)FactData2004.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2004];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2005],
		FILENAME = '$(FMDWDefaultDataPath)FactData2005.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2005];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2006],
		FILENAME = '$(FMDWDefaultDataPath)FactData2006.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2006];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2007],
		FILENAME = '$(FMDWDefaultDataPath)FactData2007.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2007];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2008],
		FILENAME = '$(FMDWDefaultDataPath)FactData2008.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2008];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2009],
		FILENAME = '$(FMDWDefaultDataPath)FactData2009.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2009];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2010],
		FILENAME = '$(FMDWDefaultDataPath)FactData2010.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2010];
GO	

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2011],
		FILENAME = '$(FMDWDefaultDataPath)FactData2011.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2011];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2012],
		FILENAME = '$(FMDWDefaultDataPath)FactData2012.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2012];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2013],
		FILENAME = '$(FMDWDefaultDataPath)FactData2013.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2013];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2014],
		FILENAME = '$(FMDWDefaultDataPath)FactData2014.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2014];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2015],
		FILENAME = '$(FMDWDefaultDataPath)FactData2015.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2015];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2016],
		FILENAME = '$(FMDWDefaultDataPath)FactData2016.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2016];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2017],
		FILENAME = '$(FMDWDefaultDataPath)FactData2017.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2017];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2018],
		FILENAME = '$(FMDWDefaultDataPath)FactData2018.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2018];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2019],
		FILENAME = '$(FMDWDefaultDataPath)FactData2019.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2019];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2020],
		FILENAME = '$(FMDWDefaultDataPath)FactData2020.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2020];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2021],
		FILENAME = '$(FMDWDefaultDataPath)FactData2021.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2021];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2022],
		FILENAME = '$(FMDWDefaultDataPath)FactData2022.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2022];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2023],
		FILENAME = '$(FMDWDefaultDataPath)FactData2023.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2023];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2024],
		FILENAME = '$(FMDWDefaultDataPath)FactData2024.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2024];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2025],
		FILENAME = '$(FMDWDefaultDataPath)FactData2025.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2025];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2026],
		FILENAME = '$(FMDWDefaultDataPath)FactData2026.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2026];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2027],
		FILENAME = '$(FMDWDefaultDataPath)FactData2027.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2027];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2028],
		FILENAME = '$(FMDWDefaultDataPath)FactData2028.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2028];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2029],
		FILENAME = '$(FMDWDefaultDataPath)FactData2029.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2029];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2030],
		FILENAME = '$(FMDWDefaultDataPath)FactData2030.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2030];
GO

ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2031],
		FILENAME = '$(FMDWDefaultDataPath)FactData2031.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2031];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2032],
		FILENAME = '$(FMDWDefaultDataPath)FactData2032.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2032];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2033],
		FILENAME = '$(FMDWDefaultDataPath)FactData2033.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2033];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2034],
		FILENAME = '$(FMDWDefaultDataPath)FactData2034.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2034];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2035],
		FILENAME = '$(FMDWDefaultDataPath)FactData2035.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2035];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2036],
		FILENAME = '$(FMDWDefaultDataPath)FactData2036.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2036];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2037],
		FILENAME = '$(FMDWDefaultDataPath)FactData2037.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2037];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2038],
		FILENAME = '$(FMDWDefaultDataPath)FactData2038.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2038];
GO


ALTER DATABASE [$(DatabaseName)]
	ADD FILE
	(
		NAME = [FactData2039],
		FILENAME = '$(FMDWDefaultDataPath)FactData2039.ndf',
		SIZE = 5MB,
		MAXSIZE = UNLIMITED,
		FILEGROWTH = 2%
	)
	TO FILEGROUP [fg_FACT_2039];
GO