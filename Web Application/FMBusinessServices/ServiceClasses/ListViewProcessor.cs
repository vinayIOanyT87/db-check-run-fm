/// <summary>
/// File name:	RequestProcessor.cs
/// Purpose:	The purpose of this class is to handle the requests for the
///				List Views. This processor interprets the request, processes
///				the request and returns the results.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		
/// </summary>
using System;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ServiceRequests;
using FMBusinessServices.DataAccessLayer;
using FMBusinessObjects.Exceptions;
using FMBusinessServices.InternalClasses;

namespace FMBusinessServices.ServiceClasses
{
	public class ListViewProcessorClass : IListViewProcessor
	{
		public ListViewProcessorClass()
		{
		}

		/// <summary>
		/// This method processes the request.
		/// </summary>
		/// <param name="inListViewSR"></param>
		/// <returns></returns>
		public ListViewDO Process(ListViewSR listViewSR)
		{
			ListViewsClass ListViews = new ListViewsClass();

			ListViewDO ldo;

			// For ledger views 
			Guid listViewGuid = listViewSR.ListViewGuid;

			if (listViewGuid == Guid.Empty)
			{
				listViewGuid = ListViews.GetIdentityGuid(listViewSR.Security, listViewSR.Type, listViewSR.TypeGuid);
			}

			if (listViewGuid != Guid.Empty)
			{
				ListViewClass ListView = ListViews.Get(listViewSR.Security, listViewSR.Type, listViewGuid);
				ldo = new ListViewDO(ListView);
			}
			else
			{
				ldo = new ListViewDO();
			}

			ldo.Site = listViewSR.Site;

			return ldo;
		}

	}
}