/// <summary>
/// File name:	BOLProductBase.cs
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
///		10-Jul-08	I.Orndorff		1.0.1 - Remove "Math.Abs()" from "GrossDigit()". This
///											will make the assignment of gross and net values
///											consistent.
///		
/// </summary>
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.Constants;

namespace FMBusinessObjects.PIDXTransactions
{
	public abstract class BOLProductBase
	{
		#region Private attributes
		#endregion

		#region Protected attributes
		protected int blendOrAlterationIndicator;
		protected string productCode;
		protected double gross;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the BOL Product class.
		/// </summary>
		public BOLProductBase ( )
		{
			this.Initialize ( );
		}
		#endregion

		#region Properties

		public int BlendOrAlterationIndicatorDigit
		{
			get { return this.blendOrAlterationIndicator; }
			set { this.blendOrAlterationIndicator = Math.Abs(value); }
		}


		public double GrossDigit
		{
			get { return this.gross; }
			set { this.gross = value; }
		}

		#endregion

		#region Protected methods
		/// <summary>
		/// This method validates for the BOL Product common fields. It throws
		/// an exception if the validation fails.
		/// </summary>
		protected void Validate()
		{
			if(this.BlendOrAlterationIndicatorDigit == -99)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_012);
			}

			if(this.gross == -9999.0)
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_013);
			}


			if(string.IsNullOrEmpty(this.productCode))
			{
				throw new PIDXException(PIDXConstants.ERR_MSG_016);
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.productCode = null;
			this.blendOrAlterationIndicator = -99;
			this.gross = -9999.0;
		}
		#endregion

		#region Abstract methods
		/// <summary>
		/// This is an abstract method that forces implementation of the validation at the derived class.
		/// </summary>
		/// <returns></returns>
		public abstract void ValidateProduct();
		#endregion

	}
}