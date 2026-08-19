//******************************************************************************
//	FILE NAME:		ConsolidatedDA.cs
//	PURPOSE:			Database access support class for connecting to the
//						AviationDB database
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Chris Knight
//	VERSION:		1.0.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-------------- -------------------------------------------
//		04-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;

namespace ASCReporter
{
	/// <summary>
	/// Class to provide database access methods for the AviationDB database
	/// </summary>
	class ConsolidatedDA
	{
		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// currently does nothing
		/// </remarks>
		ConsolidatedDA ( )
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns a database connection string for the AviationDB
		/// logging in as the user specified
		/// </summary>
		/// <returns>Formatted connection string</returns>
		/// <remarks>
		/// This function creates a connection string intended to be
		/// used with SQL Server logins.  It does not support integrated Windows
		/// logins.
		/// </remarks>
		static public string ConnectionString
		{
			get
			{
				string strConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

				if (string.IsNullOrEmpty ( strConnectionString ) == true)
				{
					strConnectionString = "Persist Security Info=False;Integrated Security=SSPI;database=ConsolidatedDB;server=127.0.0.1;Connect Timeout=30";
				}

				SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder ( strConnectionString );
				connectionString.AsynchronousProcessing = true;
				return connectionString.ToString ( );
			}
		}
		#endregion
	}
}
