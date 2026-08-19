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
using AcculoadOPCObjectsLib;
using FMBusinessObjects.DataObjects;

namespace OPCWebApp.AcculoadOPCWebApp
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
		/// used in Accuload OPC.
		/// </summary>
		/// <param name="security"></param>
		/// <returns></returns>
		string[] IDataDictionary.Keys( SecurityClass security )
		{
			string [] keys = 
			{
				"{None}",

				// Presets
				"SmithMeter|Smith Meter",
				"SmithMeter|System",
				"SmithMeter|Add",
				"SmithMeter|Delete this item",
				"SmithMeter|Edit this item",
				"SmithMeter|Edit",
				"SmithMeter|Delete",
				"SmithMeter|ID",
                "SmithMeter|IP Address",
                "SmithMeter|Network",
                "SmithMeter|Type",
				"SmithMeter|Port",
				"SmithMeter|Presets Configuration",
                "SmithMeter|Presets",

				// Preset
				"SmithMeter|Preset Configuration",
				"SmithMeter|Arm",
				"SmithMeter|Products",
				"SmithMeter|Update this item",
				"SmithMeter|Cancel Edit on this item",
				"SmithMeter|Serial Communications",                
                "SmithMeter|Network Communications",                

				// Ports
				"SmithMeter|Ports Configuration",
                "SmithMeter|Ports",

				// Card Readers
				"SmithMeter|Card Readers Configuration",

				// Card Reader
				"SmithMeter|Card Reader Configuration",
				"SmithMeter|Address",
				"SmithMeter|OK",
				"SmithMeter|Cancel",
				"SmithMeter|* Denotes Required Field",
                "SmithMeter|Card Readers",

				// Port
				"SmithMeter|Baud",
				"SmithMeter|Stop Bits",
				"SmithMeter|Data Bits",
				"SmithMeter|Parity",
                "SmithMeter|Port Configuration",

				// PageSizeDropDown
				"SmithMeter|Show 10",
				"SmithMeter|Show 25",
				"SmithMeter|Show 50",
				"SmithMeter|Show 100",
				"SmithMeter|Show All",

				// Errors
				"SmithMeter|Invalid IP Address lower octet > 0 and <= 99",

				// SelectSystemModeSystem
				"Text",
				"List",

			};

			
			// Add the arm types
			ArmClass arm=new ArmClass();

			string [] armTypes = new string [ Enum.GetNames(typeof(ACCULOAD_ARM_TYPE)).Length - 1 ];

			int index = 0;

			for ( ACCULOAD_ARM_TYPE type = ACCULOAD_ARM_TYPE.STRAIGHT ; type < ACCULOAD_ARM_TYPE.MAX_ACCULOAD_ARM_TYPE; type++ )
			{
				armTypes[index++] = "SmithMeter|" + arm.TypeID( type );
			}

			// Add the Parity types
			PortClass port = new PortClass();

			string [] parityTypes = new string [ Enum.GetNames( typeof(ACCULOAD_PARITY) ).Length - 1 ];

			index = 0;

			for ( ACCULOAD_PARITY parity = ACCULOAD_PARITY.ACCULOAD_PARITY_NONE; parity < ACCULOAD_PARITY.MAX_ACCULOAD_PARITY; parity++ )
			{
				parityTypes[index++] = "SmithMeter|" + port.ParityID( parity );
			}

			// Roll them all together

			string [] moreKeys = new string[ keys.Length + armTypes.Length + parityTypes.Length ];

			keys.CopyTo( moreKeys, 0 );
			armTypes.CopyTo( moreKeys, keys.Length );
			parityTypes.CopyTo( moreKeys, keys.Length + armTypes.Length );

			return moreKeys;

		}
        
		//*************************************************************************
		// Accessors
		//*************************************************************************    

	}

}