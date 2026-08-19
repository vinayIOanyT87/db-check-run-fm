/// <summary>
/// File name:	FinanceDO.cs
/// Purpose:	The purpose the finance data object is to
///				store the attributes and supply SQL retrieve data.
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///	Author(s):	Richard R. Panachida
///	Version:	1.0.0  Current version
///	
///	Modification History:
///		Date:				By:						Reason:
///		----------		-------------------	----------------------------------
///		12/18/2008		V. Thompson				Updated GetAverageUnitPriceExecuteSQL function to calculate
///														average unit price using a start date and an end date
///		2009-03-03     Richard Panachida    Updated contain additional information for handling the standing offer.
///		                                    Defect 1696.
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

using FMBusinessObjects.ServiceRequests;

namespace FMBusinessObjects.DataObjects
{
	[DataContract]
   [Serializable]
	public class FinanceDO : DataObject
	{
		#region Private data members
		[DataMember]
		private double averageGrossUnitPrice;
		[DataMember]
		private double averageNetUnitPrice;
		[DataMember]
		private double currentStandingOfferPrice;
		[DataMember]
		private double mostRecentStandingOfferPrice;
		[DataMember]
		private bool containsCurrentStandingOfferPrice;
		[DataMember]
		private bool containsMostRecentStandingOfferPrice;
		[DataMember]
		private bool containsAverageUnitPrice;
		[DataMember]
		private string standingOfferID;
		[DataMember]
		private string infoMessage;
		[DataMember]
		private bool hasMessage;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Finance data object class.
		/// </summary>
		public FinanceDO()
		{
			this.Initialize();
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return true if there is an information message to
		/// display to the user.
		/// </summary>
		public bool HasMessage
		{
			get { return this.hasMessage; }
			private set { this.hasMessage = value; }
		}

		/// <summary>
		/// This property will set and get the information message.
		/// </summary>
		public string InfoMessage
		{
			get { return this.infoMessage; }
			set
			{
				if (string.IsNullOrEmpty(value) == true)
				{
					this.hasMessage = false;
				}
				else
				{
					this.hasMessage = true;
					this.infoMessage = value;
				}
			}
		}

		/// <summary>
		/// This property will return the average gross unit price.
		/// </summary>
		public double AverageGrossUnitPrice
		{
			get { return this.averageGrossUnitPrice; }
			set { this.averageGrossUnitPrice = value; }
		}

		/// <summary>
		/// This property will return the average net unit price.
		/// </summary>
		public double AverageNetUnitPrice
		{
			get { return this.averageNetUnitPrice; }
			set { this.averageNetUnitPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the current price list (aka standing offer) price for the 
		/// current period.
		/// </summary>
		public double CurrentStandingOfferPrice
		{
			get { return this.currentStandingOfferPrice; }
			set { this.currentStandingOfferPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the most recent price list (aka standing offer) price for a 
		/// previous period.
		/// </summary>
		public double MostRecentStandingOfferPrice
		{
			get { return this.mostRecentStandingOfferPrice; }
			set { this.mostRecentStandingOfferPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the current price list (aka standing offer) price flag.
		/// True indicates that there is a valid price list (aka standing offer) price.
		/// </summary>
		public bool ContainsCurrentStandingOfferPrice
		{
			get { return this.containsCurrentStandingOfferPrice; }
			set { this.containsCurrentStandingOfferPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the most recent price list (aka standing offer) price flag.
		/// True indicates that there is a valid price list (aka standing offer) price.
		/// </summary>
		public bool ContainsMostRecentStandingOfferPrice
		{
			get { return this.containsMostRecentStandingOfferPrice; }
			set { this.containsMostRecentStandingOfferPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the most average unit price flag.
		/// True indicates that there is a valid average unit price.
		/// </summary>
		public bool ContainsAverageUnitPrice
		{
			get { return this.containsAverageUnitPrice; }
			set { this.containsAverageUnitPrice = value; }
		}

		/// <summary>
		/// This property sets and gets the price list (aka standing offer) ID.
		/// </summary>
		public string StandingOfferID
		{
			get { return this.standingOfferID; }
			set { this.standingOfferID = value; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method will clear the information message.
		/// </summary>
		public void ClearInfoMessage()
		{
			this.hasMessage = false;
			this.infoMessage = "";
		}
		#endregion

		#region Public SQL methods
		/// <summary>
		/// This method will return a SQL execute string to retrieve average unit price.
		/// </summary>
		/// <param name="financeSR"></param>
		/// <returns></returns>
		public void GetAverateUnitPriceExecuteSQL(SqlCommand cmd, FinanceSR sr,
													DateTimeOffset startDate,
													DateTimeOffset endDate)
		{

			cmd.CommandText = "EXEC usp_AverageUnitPrice @SiteGuid, " +
						"@StartDate, " +
						"@EndDate, " +
						"@SupplierCompanyGuid, " +
						"@ProductGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@StartDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@EndDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@SupplierCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ProductGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = sr.SiteGuid;
			cmd.Parameters["@StartDate"].Value = startDate;
			cmd.Parameters["@EndDate"].Value = endDate;
			cmd.Parameters["@SupplierCompanyGuid"].Value = sr.SupplierCompanyGuid;
			cmd.Parameters["@ProductGuid"].Value = sr.ProductGuid;
		}

		#endregion

		#region Load methods
		/// <summary>
		/// This method will load the information in the data set to the
		/// object.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadAverateUnitPrice(System.Data.DataSet dataSet)
		{
			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				if (table.Rows.Count > 0)
				{
					System.Data.DataRow row = table.Rows[0];

					if ((row == null) || (row["AverageGrossUnitPrice"] == DBNull.Value))
					{
						this.averageNetUnitPrice = 0.0;
						this.ContainsAverageUnitPrice = false;
					}
					else
					{
						this.averageGrossUnitPrice = (double)row["AverageGrossUnitPrice"];
						this.ContainsAverageUnitPrice = true;

						if (row["AverageNetUnitPrice"] == DBNull.Value)
						{
							this.averageNetUnitPrice = 0.0;
							this.ContainsAverageUnitPrice = false;
						}
						else
						{
							this.averageNetUnitPrice = (double)row["AverageNetUnitPrice"];
							this.ContainsAverageUnitPrice = true;
						}
					}
				}
			}
		}
		#endregion

		#region Overrides
		public override string getDeleteCommand()
		{
			return null;
		}
		public override string getInsertCommand()
		{
			return null;
		}
		public override string getSelectCommand()
		{
			return null;
		}
		public override string getUpdateCommand()
		{
			return null;
		}
		#endregion Overrides

		#region Private methods
		/// <summary>
		/// This method initializes the finance data object to its initial state.
		/// </summary>
		private void Initialize()
		{
			this.averageGrossUnitPrice = 0.0;
			this.averageNetUnitPrice = 0.0;
			this.currentStandingOfferPrice = 0.0;
			this.mostRecentStandingOfferPrice = 0.0;
			this.containsCurrentStandingOfferPrice = false;
			this.containsMostRecentStandingOfferPrice = false;
			this.containsAverageUnitPrice = false;
			this.standingOfferID = "";
			this.infoMessage = "";
			this.hasMessage = false;
		}
		#endregion
	}
}
