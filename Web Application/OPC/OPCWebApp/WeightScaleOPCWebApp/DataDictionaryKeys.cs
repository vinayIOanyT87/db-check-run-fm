/******************************************************************************
	FILE NAME:		DataDictionaryKeys.cs

	PURPOSE:			Data dictionary entries

	COMMENTS:

		Copyright (C) Leidos - Varec, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the expressed written consent of Leidos - Varec.

	AUTHOR(S):	Kendall

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using FMBusinessObjects.DataObjects;

namespace WeightScaleOPCWebApp
{
	/// <summary>
	/// DataDictionaryKeys implements the IDataDictionary interface
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
		/// used in WeightScale OPC.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys( SecurityClass security )
		{
			string [] Keys = 
			{
				// WeightScales
				"Weight Scales Configuration",
				"System",
				"Add",
				"Edit",
				"Edit this item",
				"Delete this item",
				"Delete",
				"ID",
				"Type",
				"Port",
                "WeightScales",

				// WeightScale
				"Weight Scale Configuration",
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
				Keys[nLoop] = "WeightScale|" + Keys[nLoop];
			}

			return Keys;

		}

        
		//*************************************************************************
		// Accessors
		//*************************************************************************    

	}

}