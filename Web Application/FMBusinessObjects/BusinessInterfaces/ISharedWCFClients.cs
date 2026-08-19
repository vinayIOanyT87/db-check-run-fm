///***************************************************************************
/// Module Name:  ISharedWCFChannels
/// Author:       Daniel Or
/// Copyright (c) Varec, Inc.  All rights reserved.
///***************************************************************************

namespace FMBusinessObjects.BusinessInterfaces
{
	using System;

	/// <summary>
	/// This interface is used by web page(mainly FMFormBase) to provide shared WCF channels
	/// to minimize the opening/closing of the channels.
	/// This will most likely be called from like controls, so they can get to the parent page
	/// </summary>
	public interface ISharedWCFChannels
	{
		/// <summary>
		/// Returns the DataDictionary proxy/channel
		/// </summary>
		IDataDictionariesClass DataDictionaryChannel { get;}

		/// <summary>
		/// Returns the Sites proxy/channel
		/// </summary>
		ISites SiteChannel { get; }
	}
}
