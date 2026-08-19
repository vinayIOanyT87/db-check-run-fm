
 #pragma warning disable 1587
/// <summary>
/// File name:	PIDXProductAuthorization.cs
/// Purpose:	
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA, 
///				2008.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Warren Gray
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:		By:				Reason:
///		----------	-------------	-------------------------------------------
///		
/// </summary>
/// 
#pragma warning restore 1587

namespace FMBusinessObjects.PIDXTransactions
{
    public class PIDXProductAuthorization
	{
		#region Private attributes
		private string productTypeIndicator;
		private string pidxProductOrFamily;
		private string authorizedVolume;
		private string unitOfMeasure;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the PIDXProductAuthorization class.
		/// </summary>
		public PIDXProductAuthorization()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		public string ProductTypeIndicator
		{
			get { return this.productTypeIndicator;}
			set { this.productTypeIndicator=value;}
		}

		public string PidxProductOrFamily
		{
			get { return this.pidxProductOrFamily; }
			set { this.pidxProductOrFamily = value; }
		}

		public string AuthorizedVolume
		{
			get { return this.authorizedVolume; }
			set { this.authorizedVolume = value; }
		}

		public string UnitOfMeasure
		{
			get { return this.unitOfMeasure; }
			set { this.unitOfMeasure = value; }
		}


		#endregion
		#region Private methods
		/// <summary>
		/// This method initializes the object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.productTypeIndicator = null;
			this.pidxProductOrFamily = null;
			this.authorizedVolume = null;
			this.unitOfMeasure = null;
		}
		#endregion


	}
}
