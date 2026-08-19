// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchTotalAndAveragePage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchTotalAndAveragePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    Partial definition of the DispatchTotalAndAveragePage class.  Provides functionality for the
	///    Dispatch Total And Average web page.
	/// </summary>
	public partial class DispatchTotalAndAveragePage : FMFormBase
	{
		#region Methods

		/// <summary>
		///    Closes the form and redirects client to previous page or FuelsManager home page.
		///    If a close button click was used to navigate to this page then the FuelsManager
		///    home page will be displayed when this page is closed.  Otherwise the previous
		///    page will be displayed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void CloseButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// If the menu bar was used to navigate to this page then the URL of the previous
				// page will be stored in the PreviousMenuItemUrl property.  If an open button
				// click was used to navigate to this page then the URL of the previous page
				// will be stored in the CurrentMenuItemUrl property.  The navigate action is
				// only provided on open and close button clicks.  A null or empty navigate
				// action indicates the menu bar was used to navigate to this page.
				var navigateAction = this.Session["NavigateAction"] as string;
				string redirectPageUrl;

				if (string.IsNullOrEmpty(navigateAction))
				{
					redirectPageUrl = this.ucFMMenuBar.PreviousMenuItemUrl;
				}
				else if (navigateAction == "openClick")
				{
					redirectPageUrl = this.ucFMMenuBar.CurrentMenuItemUrl;
				}
				else
				{
					redirectPageUrl = FMMenuBar.FuelsManagerHomePageUrl;
				}

				this.Redirect(redirectPageUrl + "?navigateAction=closeClick");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Called when a user selection value changes.  A list of transactions is retrieved based on the specified
		///    parameters used to build the dispatch transactions service request object.  The total and average value
		///    of the tranactions for the selected transaction field is computed and displayed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void OnSelectionValueChanged(object sender, EventArgs e)
		{
			try
			{
				bool useTimeFormat = this.rbResponseTime.Checked || this.rbFuelTime.Checked;
				var formatInfo = new NumberFormatInfo();

				if (!useTimeFormat)
				{
					SiteClass site =
						FMChannelHelper.MakeCall<ISites, SiteClass>(
							x => x.Get(this.Security, this.Security.SiteGuid, false, false, false));

					formatInfo = site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME);
				}

				double totalAmount = 0.0;
				double averageAmount = 0.0;
				long transactionCount = 0;

				var beginDate = (DateTime)this.Session["BeginDate"];
				var endDate = (DateTime)this.Session["EndDate"];
				var serviceRequest = new DispatchTransactionsSR
					{
						Security = this.Security,
						Site = this.Security.SiteID,
						CurrentSiteGuid = this.Security.SiteGuid,
						BeginDate = beginDate,
						EndDate = endDate
					};

				foreach (object control in this.transactionAliasCheckBoxList.Items)
				{
					var checkBox = control as ListItem;
					if (checkBox != null && checkBox.Selected)
					{
						serviceRequest.AliasNames.Add(checkBox.Value);
					}
				}

				// Set average and total to set if no aliases were selected.
				if (serviceRequest.AliasNames.Count == 0)
				{
					if (useTimeFormat)
					{
						this.averageTextBox.Text = averageAmount.ToString("F2");
						this.totalTextBox.Text = totalAmount.ToString("F2");
					}
					else
					{
						this.averageTextBox.Text = averageAmount.ToString(formatInfo);
						this.totalTextBox.Text = totalAmount.ToString(formatInfo);
					}
				}
				else
				{
					var transactionStatus = (string)this.Session["Status"];

					// The tabular view uses the empty string to represent a status selection of "{All}"
					if (string.IsNullOrWhiteSpace(transactionStatus) || transactionStatus == "{All}")
					{
						serviceRequest.Statuses.Add("Dispatched");
						serviceRequest.Statuses.Add("Arrived");
						serviceRequest.Statuses.Add("Started");
						serviceRequest.Statuses.Add("Stopped");
						serviceRequest.Statuses.Add("Completed");
					}
					else
					{
						serviceRequest.Statuses.Add(transactionStatus);
					}

					DispatchTransactionsDO transactionResults =
						FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(
							x => x.GetLineItems(serviceRequest));

					DataSet ds = transactionResults.Transactions;
					foreach (DataRow dr in ds.Tables[0].Rows)
					{
						// The following data row columns are used
						// Start Time =		FST
						// Stop Time =		TimeEnd
						// Arrival Time =	TimeIn
						// Departed Time =	TimeOut
						// Dispatch Time =	DispatchedDateTime
						// Quantity =		GrossQuantity
						// Variance =		Variance
						// status =			TransactionStatus
						if (this.rbQuantity.Checked)
						{
							var status = (TransactionStatus)Enum.Parse(typeof(TransactionStatus), (string)dr["LookupTransactionStatusIndex"]);
							if (status == TransactionStatus.Completed)
							{
								// calculate the quantities
								totalAmount += Convert.ToDouble(dr["GrossQuantity"].ToString());
								++transactionCount;
							}
						}
						else if (this.rbResponseTime.Checked)
						{
							// this is the time difference between requested time and arrival time
							if (dr["RequestedDateTime"].ToString().Length > 0 && dr["TimeIn"].ToString().Length > 0)
							{
								// calculate the response time in minutes
								var timeSpan = new TimeSpan();
								timeSpan = DateTimeOffset.Parse(dr["TimeIn"].ToString())
								           - DateTimeOffset.Parse(dr["RequestedDateTime"].ToString());
								totalAmount += timeSpan.TotalMinutes;
								++transactionCount;
							}
						}
						else if (this.rbFuelTime.Checked)
						{
							// this is the difference between start and stop time
							if (dr["FST"].ToString().Length > 0 && dr["TimeEnd"].ToString().Length > 0)
							{
								// calculate the response time in minutes
								var timeSpan = new TimeSpan();
								timeSpan = DateTimeOffset.Parse(dr["TimeEnd"].ToString()) - DateTimeOffset.Parse(dr["FST"].ToString());
								totalAmount += timeSpan.TotalMinutes;
								++transactionCount;
							}
						}
						else if (this.rbVariance.Checked)
						{
							if (dr["Variance"].ToString().Length > 0)
							{
								totalAmount += Convert.ToDouble(dr["Variance"].ToString());
								++transactionCount;
							}
						}
					}

					if (transactionCount != 0)
					{
						averageAmount = totalAmount / transactionCount;
					}

					if (useTimeFormat)
					{
						this.averageTextBox.Text = averageAmount.ToString("F2");
						this.totalTextBox.Text = totalAmount.ToString("F2");
					}
					else
					{
						this.averageTextBox.Text = averageAmount.ToString(formatInfo);
						this.totalTextBox.Text = totalAmount.ToString(formatInfo);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executes when the page is loaded.  Filter parameters for the transaction processer are extracted
		///    from the page request query string collection and stored in the session object for later use.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Page.IsPostBack)
				{
					this.Session["NavigateAction"] = this.Request.QueryString["navigateAction"];
					this.Session["BeginDate"] = this.ConvertToDateTime(this.Request.QueryString["beginDate"]);
					this.Session["EndDate"] = this.ConvertToDateTime(this.Request.QueryString["endDate"]);
					this.Session["Status"] = this.Request.QueryString["status"];

					TransactionAliasNameCollectionClass aliasNameClasses =
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasNameCollectionClass>(
							x => x.EnumerateDispatchAliasNames(this.Security));

					var checkBoxInfoMap = new Dictionary<string, bool>();

					foreach (TransactionAliasNameClass aliasNameClass in aliasNameClasses)
					{
						string aliasName = aliasNameClass.AliasName;
						bool defaultToChecked = aliasNameClass.TransTypeID == TransactionTypes.T3_PrimaryDefuel
							                    || aliasNameClass.TransTypeID == TransactionTypes.T4_SecondaryDefuel
							                    || aliasNameClass.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
							                    || aliasNameClass.TransTypeID == TransactionTypes.T6_SecondaryDisbursement;
						checkBoxInfoMap.Add(aliasName, defaultToChecked);
					}

					this.transactionAliasCheckBoxList.DataSource = checkBoxInfoMap.Keys;
					this.transactionAliasCheckBoxList.DataBind();

					foreach (object control in this.transactionAliasCheckBoxList.Items)
					{
						var checkBox = control as ListItem;
						if (checkBox != null)
						{
							string aliasName = checkBox.Text;
							checkBox.Selected = checkBoxInfoMap[aliasName];
							checkBox.Text = this.GetTranslatedText(aliasName);
						}
					}

					this.rbQuantity.Checked = true;
					this.OnSelectionValueChanged(null, null);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}
		#endregion

		private DateTime ConvertToDateTime(string inDate)
		{
			if (string.IsNullOrEmpty(inDate))
			{
				throw new Exception("Invalid date (null).");
			}

			const int QuoteMark = 8206;
			char[] charArray = inDate.ToCharArray();
			string strippedDate = String.Empty;

			// Strip the quote marks out of the string.
			foreach (char charValue in charArray)
			{
				if (charValue != QuoteMark)
				{
					strippedDate = strippedDate + charValue;
				}
			}

			string[] parts = strippedDate.Split('/');

			if (parts.Length < 3)
			{
				throw new Exception("Invalid date format mm/dd/yyy");
			}

			try
			{
				int month = Convert.ToInt32(parts[0]);
				int day = Convert.ToInt32(parts[1]);
				int year = Convert.ToInt32(parts[2]);

				DateTime newDateTime = new DateTime(year, month, day, 0, 0, 0);
				return newDateTime;
			}
			catch (Exception ex)
			{
				throw new Exception("Invalid date values. " + ex.Message);
			}
		}
	}
}