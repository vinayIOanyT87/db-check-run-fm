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
using FMBusinessObjects.DataObjects;
using OptomuxOPCObjectsLib;

namespace OPCWebApp.OptomuxOPCWebApp
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
		/// used in Optomux OPC.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys( SecurityClass security )
		{
			string [] keys = 
			{
				// Navigation
				"Optomux",
                "Controllers",
				
				// Ports
				"System",
				"Ports",
				"Ports Configuration",
				"Delete",
				"ID",
				"Edit",
				"Add",
				"Edit this item",
				"Delete this item",

				// Controllers
				"Controllers Configuration",
				"System",
				"Type",
				"Port",
				"IP Address",
				"Network",

				// Port
				"Port Configuration",
				"Baud",
				"OK",
				"Cancel",
				"Stop Bits",
				"Data Bits",
				"Parity",

				// Controller
				"Controller Configuration",
				"Serial Communications",
				"Network Communications",
				"Output",
				"Input",
				"I/O Module",
				"Address",
				"* Denotes Required Field",

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

			for ( int nLoop = 0; nLoop < keys.Length; ++nLoop )
			{
				keys[nLoop] = "Optomux|" + keys[nLoop];
			}

			// Add the Parity types
			PortClass port = new PortClass();

			string [] parityTypes = new string [ Enum.GetNames( typeof(OPTOMUX_PARITY) ).Length - 1 ];

			int index = 0;

			for ( OPTOMUX_PARITY parity = OPTOMUX_PARITY.OPTOMUX_PARITY_NONE; parity < OPTOMUX_PARITY.MAX_OPTOMUX_PARITY; parity++ )
			{
				parityTypes[index++] = "Optomux|" + port.ParityID( parity );
			}

			// Roll them all together

			string [] moreKeys = new string[ keys.Length + parityTypes.Length ];

			keys.CopyTo( moreKeys, 0 );
			parityTypes.CopyTo( moreKeys, keys.Length );

			return moreKeys;

		}

        
		//*************************************************************************
		// Accessors
		//*************************************************************************    

	}

}