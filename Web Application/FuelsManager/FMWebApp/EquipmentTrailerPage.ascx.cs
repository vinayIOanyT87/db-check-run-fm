/******************************************************************************

	FILE NAME:		EquipmentTrailerPage.ascx.cs


	PURPOSE:			Implementation of EquipmentTrailerPage


	COMMENTS:

		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002

		This file shall not be copied or reproduced in any form without
				the express written consent of Endress+Hauser.


	AUTHOR(S):	W. Gray


	VERSION:		1.0.0  Current version



	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for EquipmentTrailerPage.
	/// </summary>
	public partial class EquipmentTrailerPage : EquipmentPageBase
	{
		protected FMControls.FMLabel Label3;
	

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				if (this.Equipment == null || this.Equipment.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				{
					return;
				}
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		public void UpdateData()
		{
			if(this.Equipment.Type != EQUIPMENT_TYPE.TRAILER_TYPE)
				return;

		}

	}
}

