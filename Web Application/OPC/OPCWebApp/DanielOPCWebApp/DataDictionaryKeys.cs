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

using System;
using DanielOPCObjectsLib;
using FMBusinessObjects.DataObjects;

namespace OPCWebApp.DanielOPCWebApp
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

	    //*************************************************************************
		// Member functions
		//*************************************************************************    

		/// <summary>
		/// This function is responsible for returning any user-visual language string
		/// used in Daniel OPC.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys( SecurityClass security )
		{
			string [] keys = 
			{
				"{None}",

				// Presets
				"Daniel|Daniel",
				"Daniel|System",
				"Daniel|Add",
				"Daniel|Delete this item",
				"Daniel|Edit this item",
				"Daniel|Edit",
				"Daniel|Delete",
				"Daniel|ID",
				"Daniel|Type",
				"Daniel|Port",
				"Daniel|Address",
                "Daniel|Presets",
				"Daniel|Presets Configuration",

				// Preset
				"Daniel|Preset Configuration",
				"Daniel|Products",
				"Daniel|Update this item",
				"Daniel|Cancel Edit on this item",

				// Ports
                "Daniel|Ports",
				"Daniel|Ports Configuration",

				// Port
				"Daniel|Baud",
				"Daniel|Stop Bits",
				"Daniel|Data Bits",
				"Daniel|Parity",

				// PageSizeDropDown
				"Daniel|Show 10",
				"Daniel|Show 25",
				"Daniel|Show 50",
				"Daniel|Show 100",
				"Daniel|Show All",

				// SelectSystemModeSystem
				"Text",
				"List",

			};

			

			// Add the Parity types
			PortClass port = new PortClass();

			string [] parityTypes = new string [ Enum.GetNames( typeof(DANLOAD_PARITY) ).Length - 1 ];

			int index = 0;

			for ( DANLOAD_PARITY parity = DANLOAD_PARITY.DANLOAD_PARITY_NONE; parity < DANLOAD_PARITY.MAX_DANLOAD_PARITY; parity++ )
			{
				parityTypes[index++] = "Daniel|" + port.ParityID( parity );
			}

			// Roll them all together

			string [] moreKeys = new string[ keys.Length + parityTypes.Length ];

			keys.CopyTo( moreKeys, 0 );
			parityTypes.CopyTo( moreKeys, keys.Length);

			return moreKeys;

		}
        
		//*************************************************************************
		// Accessors
		//*************************************************************************    

	}

}