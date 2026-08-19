/// <summary>
/// File name:	PIDXAuthorizationBase.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Ivan Orndorff
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		04-Feb-08	I.Orndorff		1.0.0 - Initial Revision.
///		
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMBusinessObjects.PIDXTransactions
{
	public abstract class PIDXAuthorizationBase
	{
		#region Private attributes
		private string responseType;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the default constructor for the PIDX Authorization base class.
		/// </summary>
		public PIDXAuthorizationBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties
		public string ResponseType
		{
			get { return this.responseType; }
			set { this.responseType = value; }
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize ( )
		{
			this.responseType = null;
		}
		#endregion
	}
}