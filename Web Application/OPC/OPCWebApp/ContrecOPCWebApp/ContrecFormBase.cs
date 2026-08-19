/******************************************************************************
	FILE NAME:		ContrecFormBase.cs

	PURPOSE:			ContrecFormBase

	COMMENTS:

		Copyright (C) Leidos - Varec, Inc. Norcross, GA, USA, 2007

		This file shall not be copied or reproduced in any form without
				the expressed written consent of Leidos - Varec.

	AUTHOR(S):	B. Schaal

	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:			Reason:
		---------	----------  -------------------------------------------

*******************************************************************************/

using System;
using System.Web.UI.WebControls;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using FuelsManager.FMWebApp;

namespace OPCWebApp.ContrecOPCWebApp
{
	/// <summary>
	/// Summary description for ContrecFormBase.
	/// </summary>
	public class ContrecFormBase : FMAutoSubmitFormBase
	{
		protected void UpdateDeleteButton ( LinkButton deleteButton )
		{
			if (deleteButton != null)
			{
				deleteButton.Enabled = this.Security.HasRight ( RIGHT.MODIFY_SITES_AND_SITE_GROUPS );
			}

		}

		protected void PageSizeDropDown_SelectedIndexChanged ( object source, EventArgs e )
		{
		    this.UpdateView ( );
		}

		protected virtual void UpdateView ( )
		{
		}

		protected string GetDictionaryText ( string key )
		{
			string altText = key;

			if (this.Session["UseDataDictionary"] == null || (bool) this.Session["UseDataDictionary"])
			{
			    var text = altText;
			    altText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => x.Get(this.Security.SiteGuid, text));
			}
			else
			{
				// If the data dictionary is not available or is turned off, we need to strip off the
				// compartmental text at the beginning of the key value
				int index = key.IndexOf ( "|", StringComparison.Ordinal );

				if (index >= 0)
				{
					altText = altText.Substring ( index + 1 );
				}

			}

			return altText;

		}

	}
}
