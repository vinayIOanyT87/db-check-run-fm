/******************************************************************************
	FILE NAME:		DataDictionaryKeys.cs

	PURPOSE:			Data dictionary entries

	COMMENTS:

		Copyright (C) SAIC - Varec, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of SAIC - Varec.

	AUTHOR(S):	Kendall

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using FMBusinessObjects.DataObjects;

namespace ScullyOPCWebApp
{
   /// <summary>
   /// DataDictionaryKeys implements the FMCommon.IDataDictionary interface
   /// so as to provide data dictionary support.  All language text is encapsulated here.
   /// </summary>
   public class DataDictionaryKeys : IDataDictionary
	{
		//*************************************************************************
		// Member variables
		//*************************************************************************    

		//*************************************************************************
		// CTOR
		//*************************************************************************    

		/// <summary>
		/// Creates a DataDictionaryKeys instance.
		/// </summary>
		public DataDictionaryKeys()
		{
		}

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		/// <summary>
		/// This function is responsible for returning any user-visual language string
		/// used in Scully OPC.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys( SecurityClass security )
		{
			string [] Keys = 
			{
				// Scullys
				"Scully Configuration",
				"System",
				"Add",
				"Edit",
				"Edit this item",
				"Delete this item",
				"Delete",
				"ID",
				"Port",
                "Scullys",

				// Scully
				"Scully Configuration",
				"Cancel",
				"OK",

				// PageSizeDropDown
				"Show 10",
				"Show 25",
				"Show 50",
				"Show 100",
				"Show All",

				// SelectSystemModeSystem
				"Text",
				"List",
			};

			for ( int nLoop = 0; nLoop < Keys.Length; ++nLoop )
			{
				Keys[nLoop] = "Scully|" + Keys[nLoop];
			}

			return Keys;

		}

        
		//*************************************************************************
		// Accessors
		//*************************************************************************    

	}

}