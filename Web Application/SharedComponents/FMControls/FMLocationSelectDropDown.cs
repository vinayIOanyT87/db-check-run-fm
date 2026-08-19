/******************************************************************************
	FILE NAME:		FMLocationSelectDropDown.cs
	PURPOSE:			Implementation of: FMLocationSelectDropDown

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	A. Coker
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		
*******************************************************************************/
using System;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

namespace FMControls
{
	/// <summary>
	/// Summary description for FMLocationSelectDropDown.
	/// Drop down containing IATA codes as destination locations.
	/// </summary>
	public class FMLocationSelectDropDown : FMDropDownList
	{

		#region Constructors
		public FMLocationSelectDropDown()
		{

		}
		#endregion

		#region Properties
		/// <summary>
		/// Given a location Guid, determines its index within the drop down list.
		/// </summary>
		public Guid SelectedLocationGuid
		{
			set
			{
				foreach (System.Web.UI.WebControls.ListItem item in this.Items)
				{
					if (item.Value == value.ToString())
					{

						base.SelectedIndex = this.Items.IndexOf(item);
						return;
					}
				}
			}
		}
		#endregion

		#region Protected Overridden Methods
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			if (this.DesignMode == false)
			{
				//
				// Set contents of drop down list.
				SecurityClass security = (SecurityClass)this.Page.Session["Security"];

				IATACodeCollectionClass iataCodeCollection = FMChannelHelper.MakeCall<IIATACodes, IATACodeCollectionClass>(
																	 x =>
																	 x.Enumerate(security)
																);

				this.Items.Add(new System.Web.UI.WebControls.ListItem(StandingOfferClass.LOCATION_NONE, Guid.Empty.ToString()));

				foreach (IATACodeClass iata in iataCodeCollection)
				{
					this.Items.Add(new System.Web.UI.WebControls.ListItem(iata.ID, iata.IdentityGuid.ToString()));
				}
			}
		}
		#endregion
	}
}
