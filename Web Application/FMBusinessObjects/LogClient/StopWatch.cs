// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopWatch.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StopWatch type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.LogClient
{
	using System;
	using System.Security;

	/// <summary>
	/// Stopwatch to help in timing sections of code and logging the results.
	/// </summary>
	[SecuritySafeCritical]
	public class StopWatch : Logger
	{
		// *************************************************************************
		// Member variables
		// *************************************************************************    
		#region Constants and Fields

		/// <summary>
		/// Holds the time the stopwatch was started.
		/// </summary>
		private DateTimeOffset startTime;

		#endregion

		// *************************************************************************
		// CTOR
		// *************************************************************************    
		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="StopWatch"/> class.
		/// </summary>
		/// <param name="appName">Name of the app.</param>
		/// <param name="actionName">Name of the action.</param>
		public StopWatch(string appName, string actionName)
			: base(appName)
		{
			this.ActionName = actionName;
			this.Start();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StopWatch"/> class.
		/// </summary>
		/// <param name="enumAppName">Name of the enum app.</param>
		/// <param name="actionName">Name of the action.</param>
		public StopWatch(Appnames enumAppName, string actionName)
			: base(enumAppName.ToString())
		{
			this.ActionName = actionName;
			this.Start();
		}

		#endregion

		#region Enums

		/// <summary>
		/// Application section name for segregating messages by application module.
		/// </summary>
		public enum Appnames
		{
			/// <summary>
			/// Order Entry
			/// </summary>
			OrderEntry,

			/// <summary>
			/// Accounting client
			/// </summary>
			Accounting,

			/// <summary>
			/// Accounting service layer (now FMBusinessServices)
			/// </summary>
			AccountingBLL,

			/// <summary>
			/// Entity Import Export
			/// </summary>
			EntityImportExport,

			/// <summary>
			/// Logger section
			/// </summary>
			LoggerImpl,

			/// <summary>
			/// FuelsManager Web App
			/// </summary>
			FMWebApp,

			/// <summary>
			/// Shared Components service layer (now FMBusinessServices)
			/// </summary>
			ConsolidatedBLL,

			/// <summary>
			/// Site cache 
			/// </summary>
			SiteCache,

			/// <summary>
			/// Shared components data (now FMBusinessObjects)
			/// </summary>
			ConsolidatedDataObjects,

			/// <summary>
			/// Supply order web application
			/// </summary>
			SupplyOrderWebApp,

			/// <summary>
			/// Load rack service
			/// </summary>
			LoadRackService,

			/// <summary>
			/// FuelsManager Web Application Global
			/// </summary>
			FuelsManagerGlobal,

		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the name of the action.
		/// </summary>
		/// <value>
		/// The name of the action.
		/// </value>
		public string ActionName { get; set; }

		/// <summary>
		/// Gets the elapsed time.
		/// </summary>
		public TimeSpan ElapsedTime
		{
			get
			{
				return DateTimeOffset.Now - this.startTime;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Gets the split time.
		/// </summary>
		public void Split()
		{
			this.Perform(this.ActionName + " split time is " + this.ElapsedTime.ToString() + ".");
		}

		/// <summary>
		/// Starts the stop watch.
		/// </summary>
		public void Start()
		{
			this.startTime = DateTimeOffset.Now;
		}

		/// <summary>
		/// Starts the stop watch and changes the action name.
		/// </summary>
		/// <param name="actionName">Name of the action.</param>
		public void Start(string actionName)
		{
			this.ActionName = actionName;
			this.Start();
		}

		/// <summary>
		/// Stops the stop watch and logs the timing information.
		/// </summary>
		public void Stop()
		{
			this.Perform(this.ActionName + " completed in " + this.ElapsedTime.ToString() + ".");
		}

		#endregion
	}
}