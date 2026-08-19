///<summary>
/// DataDictionaryKeys
///
/// Original Author: Greg Kendall
/// Revisions: See source control comments
///
/// (C) Copyright 2009 by Varec, Inc.  All rights reserved.
///
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///
///</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.DataObjects;

namespace ADFWebApp
{
	public class DataDictionaryKeys : IDataDictionary
	{
		/// <summary>
		/// Creates a DataDictionaryKeys instance.
		/// </summary>
		public DataDictionaryKeys ( )
		{
		}

		/// <summary>
		/// This function is responsible for returning any user-visual language string
		/// used in the module
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys ( SecurityClass security )
		{
			string[] Keys = 
				{
					// General FESS Fields (tree node, etc)
					"FESS",

					// Keys from FESS Select Associated Payments Dialog
					"Select Associated Payments",
					"Associated Payments",
					"Find String",
					"Find",
					"Show All",
					"OK",
					"Cancel",
					"Select All",
					"Clear All",
					"Selection",
					"Order Number",
					"Account Number",
					"Invoice Number",
					"Supplier",
					"Fuel Type",
				
					// Keys from FESS Summary Page
					"FESS Summary",
					"Add",
					"Edit",
					"PB No",
					"Fuel Type",
					"Discounts",
					"QTY Total",
					"AMT $ (for fuel)",
					"Excise Rate",
					"GST $",
					"Total $",
					"Supplier",
					"Refresh",
					"Acct Code",
					"Order #",
					"Rebate #",
					"Invoice #",
					"Print",

					// Keys from FESS Detail Page
					"Associate Payments",
					"FESS Detail",
					"OK",
					"Close",
					"* Denotes Required Field",
					"Qty (L)",
					"ExGST $",
					"GST $",
					"Total $",
					"Rebate #",
					"Remove",
					"Remove this item",
					"Are you sure you want to remove this item?",
					"Excise"

			};

			// Tag the keys as FESS only
			for (int Index = 0; Index < Keys.Length; ++Index)
			{
				Keys[Index] = "FESS|" + Keys[Index];
			}

			return Keys;
		}
	}
}
