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
//namespace LedgerCore
//{
    using System;
    using LedgerCore;
    public class LRStoredProcedureClass
    {

        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void xsp_LedgerCalculator(DateTime inBeginDate,
                                               DateTime inEndDate,
                                               Guid     inProductGuid,
                                               Guid     inManagerGuid,
                                               Guid     inOwnerGuid,
                                               Guid     inSelectedSiteGuid,
                                               Guid     inUserGuid,
                                               int      inLedgerRequest,
                                               int      inReportLedger//,
                                               /*Guid     inTankGuid,
                                               int      inSystemEdition*/)
        {
            LRLedgerProcessor ledgerProcessor = new LRLedgerProcessor(LRLedgerProcessor.LedgerConnectionTypes.ClrConnection);
            ledgerProcessor.BeginDate = inBeginDate;
            ledgerProcessor.EndDate = inEndDate;
            ledgerProcessor.ProductGuid = inProductGuid;
            ledgerProcessor.ManagerGuid = inManagerGuid;
            ledgerProcessor.OwnerGuid = inOwnerGuid;
            ledgerProcessor.SiteGuid = inSelectedSiteGuid;
            ledgerProcessor.UserGuid = inUserGuid;
            //ledgerProcessor.TankGuid = inTankGuid;
            //ledgerProcessor.SystemEdition = (LRLedgerProcessor.SystemEditions)inSystemEdition;

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

            ledgerProcessor.GetLedgerProcessingResultDataSet();
        }
    }
//}
