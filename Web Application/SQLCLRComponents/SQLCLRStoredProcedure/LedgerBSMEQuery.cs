/// <summary>
///   File name:	LedgerBSMEQuery.cs
///   Purpose:	   The purpose of this class is to return ledger vertical data queries and results
///               for the BSME project.
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
///	yyyy-mm-dd		developer's name 		reason for the change
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class LedgerBSMEQuery : LedgerQueryBase
{
   #region Constructors
   /// <summary>
   /// This is the default for the Ledger Standard Query class.
   /// </summary>
   public LedgerBSMEQuery(	double volumeConversionFactor,
									int volumeDecimalPlaces,
									double massConversionFactor,
									int massDecimalPlaces,
									double currencyFactor,
									int currencyDecimalPlaces,
									double volumePackageSize,
									double massPackageSize,
									bool loadByWeight)
      : base(volumeConversionFactor, volumeDecimalPlaces,
				 massConversionFactor, massDecimalPlaces,
				 currencyFactor, currencyDecimalPlaces,
				 volumePackageSize, massPackageSize, loadByWeight)
   {
   }
   #endregion
}
