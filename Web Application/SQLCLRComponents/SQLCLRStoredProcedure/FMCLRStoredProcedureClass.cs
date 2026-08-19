/// <summary>
///   File name:	FMCLRStoredProcedureClass.cs
///   Purpose:	   The purpose of this class is to implement store procedure interfaces into the
///               CLR functionality.
///				
///   Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA,
///				   2000.  This file shall not be copied or reproduced in any form
///				   without the express written consent of Endress+Hauser.
///				
///	Author(s):	Richard Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///	Date:				By:						Reason:
///	----------		--------------------	----------------------------------
///	yyyy-mm-dd		Coder's name   		Reason for change
///
/// </summary>
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Server;

public class FMCLRStoredProcedureClass
{

   [Microsoft.SqlServer.Server.SqlProcedure]
   public static void xsp_LedgerCalculator(DateTime inBeginDate,
                                          DateTime inEndDate,
                                          int      inProductIndex,
                                          int      inManagerIndex,
                                          int      inOwnerIndex,
                                          int      inLoginSiteIndex,
                                          int      inSelectedSiteIndex,
                                          int      inUserIndex,
                                          int      inLedgerRequest,
                                          int      inReportLedger,
                                          int		inTankIndex,
                                          int      inSystemEdition)
   {
      CLRLedgerProcessor ledgerProcessor = new CLRLedgerProcessor();
      ledgerProcessor.BeginDate        = inBeginDate;
      ledgerProcessor.EndDate          = inEndDate;
      ledgerProcessor.ProductIndex     = inProductIndex;
      ledgerProcessor.ManagerIndex     = inManagerIndex;
      ledgerProcessor.OwnerIndex       = inOwnerIndex;
      ledgerProcessor.LoginSiteIndex   = inLoginSiteIndex;
      ledgerProcessor.SiteIndex        = inSelectedSiteIndex;
      ledgerProcessor.UserIndex        = inUserIndex;
      ledgerProcessor.TankIndex			= inTankIndex;
      ledgerProcessor.SystemEdition    = (CLRLedgerProcessor.SystemEditions) inSystemEdition;

      if ((inLedgerRequest <= 0) || (inLedgerRequest > 1))
      {
         ledgerProcessor.LedgerRequestInt = 0;
      }
      else
      {
         ledgerProcessor.LedgerRequestInt = inLedgerRequest;
      }

      if (inReportLedger == 1)
      {
         ledgerProcessor.ReportLedger = true;
      }
      else
      {
         ledgerProcessor.ReportLedger = false;
      }

      ledgerProcessor.StartLedgerProcessing();
   }
}
