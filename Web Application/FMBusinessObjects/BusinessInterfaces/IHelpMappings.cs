///***************************************************************************
/// Module Name:	IHelpMappings
/// Author:			Andy Hush
/// Copyright (c) Varec, Inc. All rights reserved.
///***************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.BusinessInterfaces
{
	/// <summary>
	/// An interface for access to the HelpMappings service class
	/// </summary>
	[ServiceContract]
	public interface IHelpMappings
	{
		/// <summary>
		/// Retrieve a Dictionary of all the help mappings
		/// </summary>
		/// <param name="security">Security object</param>
		/// <returns>Dictionary of help mappings</returns>
		[OperationContract]
		HelpMappingDictionary GetDictionary(SecurityClass security);
	}
}
