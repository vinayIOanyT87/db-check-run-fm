
USE FuelsManagerDB
GO

IF EXISTS ( SELECT * FROM [rpt].sysobjects WHERE id = OBJECT_ID(N'[rpt].[nspa_GetSiteManagerList]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rpt].[nspa_GetSiteManagerList]
GO

--USE FuelsManagerDB
USE ConsolidatedDB
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[usp_Nspa_OverAllocationReport_FuelCard]'))
DROP PROCEDURE [rpt].[usp_Nspa_OverAllocationReport_FuelCard]
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[usp_Nspa_OverAllocationReport_Transaction]'))
DROP PROCEDURE [rpt].[usp_Nspa_OverAllocationReport_Transaction]
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[udf_Nspa_OverAllocation_Daily]'))
DROP FUNCTION [rpt].[udf_Nspa_OverAllocation_Daily]
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[udf_Nspa_OverAllocation_Monthly]'))
DROP FUNCTION [rpt].[udf_Nspa_OverAllocation_Monthly]
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[udf_Nspa_OverAllocation_Transaction]'))
DROP FUNCTION [rpt].[udf_Nspa_OverAllocation_Transaction]
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].[udf_Nspa_OverAllocation_FuelCards]'))
DROP FUNCTION [rpt].[udf_Nspa_OverAllocation_FuelCards]
GO

USE FuelsManagerDB
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].nspa_SitesFromSiteGroup') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rpt].nspa_SitesFromSiteGroup
GO

USE [FuelsManagerDB]
GO

/****** Object:  StoredProcedure [rpt].[usp_DsMonthYearList_LongMonth]    Script Date: 4/6/2014 2:58:55 PM ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[rpt].[usp_DsMonthYearList_LongMonth]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rpt].[usp_DsMonthYearList_LongMonth]
GO

/****** Object:  StoredProcedure [rpt].[usp_DsMonthYearList_LongMonth]    Script Date: 4/6/2014 2:58:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE [FuelsManagerDB]
GO

/****** Object:  StoredProcedure [rpt].[usp_NspaReceiptReport_Section1]    Script Date: 4/2/2014 11:35:15 AM ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[rpt].[usp_NspaReceiptReport_Section1]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
	DROP PROCEDURE [rpt].[usp_NspaReceiptReport_Section1]
GO

/****** Object:  StoredProcedure [rpt].[usp_NspaReceiptReport_Section1]    Script Date: 4/2/2014 11:35:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE [FuelsManagerDB]
GO

/****** Object:  StoredProcedure [rpt].[usp_NspaReceiptReport_Section2]    Script Date: 4/2/2014 11:37:15 AM ******/
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[rpt].[usp_NspaReceiptReport_Section2]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rpt].[usp_NspaReceiptReport_Section2]
GO

/****** Object:  StoredProcedure [rpt].[usp_NspaReceiptReport_Section2]    Script Date: 4/2/2014 11:37:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

USE FuelsManagerDB
GO

IF EXISTS ( SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[rpt].nspa_SitesFromSiteGroup') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [rpt].nspa_SitesFromSiteGroup
GO
