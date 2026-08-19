USE [ConsolidatedDB]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

If exists (Select * From dbo.sysobjects Where id = object_id(N'dbo.[rpt_sp_ta_ReverseHierarchyReport]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
Drop Procedure dbo.[rpt_sp_ta_ReverseHierarchyReport]
GO



CREATE PROCEDURE  [dbo].[rpt_sp_ta_ReverseHierarchyReport] 
 /*=============================================
 Author:				URVI PATEL
 Create date:			5/15/2009
 Description:			Main SP for Reverse Hierarchy Report
 Version:				7.5.1.0
 Execution:
			EXEC [rpt_sp_ta_ReverseHierarchyReport] 1,1,2,'3122 - CITGO Petroleum Corp','52','3241','<ALL>','<ALL>',''  
			EXEC [rpt_sp_ta_ReverseHierarchyReport] 1,1,2,'3122 - CITGO Petroleum Corp','<ALL>','<ALL>','<ALL>','<ALL>','' 
Do Not Use This - EXEC [rpt_sp_ta_ReverseHierarchyReport] 1,1,2,'3122 - CITGO Petroleum Corp','9901','9902','<ALL>','<ALL>','' 

 Modification History:
	Date		by		Description
	5/15/2009	UP		New Stored Procedure
	12/10/2009	KF		Version 7.5.1.0
 =============================================*/

(
	@SiteIndex int ,
	@LoginSiteIndex int,
	@UserIndex int,
	@Manager nvarchar(30),
	@StockHolder nvarchar(30),
	@Shipper nvarchar(30),
	@BillTo nvarchar(30),
	@ShipTo nvarchar(30),
	@Load nvarchar(30)

)
AS

IF @Manager = '<ALL>' SET @Manager = null
IF @StockHolder = '<ALL>' SET @StockHolder = null
IF @Shipper = '<ALL>' SET @Shipper = null
IF @BillTo = '<ALL>' SET @BillTo = null
IF @ShipTo = '<ALL>' SET @ShipTo = null
IF @Load = '<ALL>' or @Load = '' SET @Load = null


	SELECT c.ID AS 'ManagerID', C.Companyindex,StockHolder,StockHolderIndex, StockHolderID,StockHolderType,Shipper,ShipperID,ShipperIndex,ShipperType,BillTo,BillToID,BillToIndex,BillToType,ShipTo, ShipToID,ShipToIndex,ShipToType,LoadID, LoadType
	FROM tblcompanies c
	JOIN
	(
				SELECT C.ID  AS 'StockHolder', StockHolderID = CASE WHEN  charindex('-',LEFT(c.name,10)) = 0 THEN  C.ID + '-' + C.Name ELSE  C.Name END,
						cm.assignedindex AS 'StockHolderIndex',cm.assignedtoindex,cm.type AS 'StockHolderType',Shipper,ShipperID,ShipperIndex,ShipperType,BillTo,BillToID,BillToIndex,BillToType, ShipTo,ShipToID,ShipToIndex,ShipToType,LOADID, LOADTYPE
				FROM tblcompanymap cm
				JOIN
				(
						SELECT  C.ID  As 'Shipper' ,ShipperID = CASE WHEN  charindex('-',LEFT(c.name,10)) = 0 THEN  C.ID + '-' + C.Name ELSE  C.Name END,
								cm.assignedindex AS 'ShipperIndex',cm.assignedtoindex, cm.type AS 'ShipperType',BillTo,BillToID,BillToIndex,BillToType,ShipTo, ShipToID,ShipToIndex,ShipToType,LoadID, LoadType
						FROM tblcompanymap cm
						JOIN
						(
								SELECT  C.ID AS 'BillTo', C.ID + '-' + C.Name AS 'BillToID',cm.[Index],cm.assignedtoindex , cm.assignedindex AS 'BillToIndex',cm.type As 'BillToType',ShipTo,ShipToID,ShipToIndex,ShipToType,LoadID, LoadType
								FROM tblcompanymap cm
								JOIN		
										(
											SELECT C.ID AS 'ShipTo', C.ID + '-' + C.Name AS 'ShipToID',cm.assignedtoindex ,cm.[Index], cm.assignedindex AS 'ShipToIndex', cm.Type AS 'ShipToType', LoadID.LoadID , LOADID.LoadType
											FROM tblcompanymap cm
											JOIN 
														(
														SELECT cm.assignedtoindex, ID AS LoadID ,Type AS LoadType
														FROM tblcompanymap cm 
														WHERE Type = 5 
														)LoadID
											ON cm.[index] = LoadID. assignedtoindex
											AND cm.type = 3 
											JOIN tblCompanies c
											ON c.companyindex = cm.assignedindex
										)ShipTo
								ON cm.[index] = shipto.assignedtoindex
								AND cm.type = 2 
								JOIN tblCompanies c
								ON c.companyindex = cm.assignedindex
						)BillTo
						ON  cm.[Index] = BillTo.assignedtoindex
						AND Type = 1
						JOIN tblCompanies c
						ON c.companyindex = cm.assignedindex
				)Shipper
				ON cm.[Index] = Shipper.assignedtoindex
				AND Type = 0
				JOIN tblCompanies c
				ON c.companyindex = cm.assignedindex
	)StockHolder
	ON c.[CompanyIndex] = StockHolder.assignedtoindex
	WHERE c.ID = isnull(@Manager,C.ID)
	AND StockHolderIndex = isnull(@StockHolder,StockHolderIndex)
	AND ShipperIndex = isnull(@Shipper,ShipperIndex)
	AND BillToIndex = isnull(@BillTo,BillToIndex)
	AND ShipToIndex = isnull(@ShipTo,ShipToIndex)
	AND (LoadID = isnull(@Load,LoadID) or LoadID = '')

ORDER BY C.ID, StockHolder, Shipper, BillTo, ShipTo, LoadID
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
 

GRANT EXECUTE ON dbo.[rpt_sp_ta_ReverseHierarchyReport] TO [public]
GO
