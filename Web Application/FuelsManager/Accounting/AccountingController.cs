// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingController.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The Accounting Controller class is the base class to all the
//   accounting controllers.  It inhierits from the UIP process
//   contoller base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.Accounting
{
	/// <summary>
	/// The Accounting Controller class is the base class to all the
	///	accounting controllers.  It inhierits from the UIP process
	///	contoller base.
	/// </summary>
	public class AccountingController
	{
		/// <summary>
		/// Gets or sets the current state.
		/// </summary>
		protected Olduip State { get; set; }

		/// <summary>
		/// Default navigation routine.
		/// </summary>
		protected void Navigate()
		{
		}

		/// <summary>
		/// Container for keeping strack of the old navigated value.
		/// </summary>
		public class Olduip
		{
			/// <summary>
			/// Gets or sets the storage for navigated value.
			/// </summary>
			public string NavigateValue { get; set; }
		}
	}
}
