/// <summary>
/// File name:	StopWatch.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2005.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Greg Kendall
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		2007-09-21		I. Orndorff				- Added SupplyOrderWebApp to APPNAMES												
///		
///		2008-08-21		I. Orndorff				- Added EntityImportExport to APPNAMES
///
///		2009-07-07		W.Gray					- Added LoadRackService to APPNAMES
///		
/// </summary>

using System;
using System.Security;

namespace LogClient
{
	[SecuritySafeCriticalAttribute]
	public class StopWatch : Logger
	{
		//*************************************************************************
		// Member variables
		//*************************************************************************    

		private string sActionName = "StopWatch";

		private System.DateTime startTime;
        
		public enum APPNAMES
		{
			OrderEntry,
			Accounting,
			AccountingBLL,
			EntityImportExport,
			LoggerImpl,
			FMWebApp,
			ConsolidatedBLL,
			SiteCache,
			ConsolidatedDataObjects,
			SupplyOrderWebApp,
			LoadRackService
		};

		//*************************************************************************
		// CTOR
		//*************************************************************************    

		public StopWatch( string AppName, string ActionName ) : base( AppName )
		{
			this.sActionName = ActionName;
			Start();
		}

		public StopWatch ( APPNAMES enumAppName, string ActionName ) : base ( enumAppName.ToString() )
		{
			this.sActionName = ActionName;
			Start();
		}

		//*************************************************************************
		// Member functions
		//*************************************************************************    

		public void Start ()
		{
			this.startTime = System.DateTime.Now;
		}


		public void Start ( string ActionName )
		{
			this.ActionName = ActionName;
			this.Start();
		}


		public void Stop ()
		{
			this.Perform( this.sActionName + " completed in " + this.ElapsedTime.ToString() + "." );
		}


		public void Split ()
		{
			this.Perform( this.sActionName + " split time is " + this.ElapsedTime.ToString() + "." );
		}


		//*************************************************************************
		// Accessors
		//*************************************************************************    

		public string ActionName 
		{
			get { return this.sActionName; }
			set { this.sActionName = value; }
		}

		public System.TimeSpan ElapsedTime
		{
			get { return new System.TimeSpan( System.DateTime.Now.Ticks - this.startTime.Ticks ); }
		}

	}

}
