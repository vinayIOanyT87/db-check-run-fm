// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountingServiceRequest.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Accounting service request base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Accounting service request base class.
	/// </summary>
    [Serializable]
	[DataContract]
	public class AccountingServiceRequest
	{
		/// <summary>
		/// Empty string numeric indicator
		/// </summary>
		[DataMember]
		private const int EmptyString = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountingServiceRequest"/> class. 
		/// This is the default constructor for the accounting service request base class.
		/// </summary>
		public AccountingServiceRequest()
		{
			this.SiteList = new ArrayList();
		}

		/// <summary>
		/// Gets or sets an array list of sites.
		/// </summary>
		[DataMember]
		public ArrayList SiteList { get; set; }

		/// <summary>
		/// Gets or sets the site information.
		/// </summary>
		[DataMember]
		public string Site { get; set; }

		/// <summary>
		/// Gets or sets the security object.
		/// </summary>
		[DataMember]
		public SecurityClass Security { get; set; }

		/// <summary>
		/// Gets or sets the current site Guid.
		/// </summary>
		[DataMember]
		public Guid CurrentSiteGuid { get; set; }

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// This method will add the site to a list of sites to be
		/// used as a criterion during a query.
		/// </summary>
		/// <param name="site">The ide of the site to add.</param>
		public void AddSiteToList( string site )
		{
			if ( ( site != null ) && ( site.Length > EmptyString ) )
			{
				this.SiteList.Add( site );
			}
		}
	}
}
