using FMControls;
using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.ServiceRequests;

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Services;
using System.Timers;

namespace ADFWebApp
{
	public partial class WacOverride : AccountingAutoSubmitWebFormView, IDataDictionary
	{
		protected static string RETURN_PAGE = "WacSummary.aspx";
		protected bool readOnlyMode = false;
		protected bool saving = false;
		protected System.Timers.Timer dateTimeTimer;
		protected AccountingSite accountingSite = null;
		protected SiteTimeConverter converter = null;

		#region Data Dictionary
		string[] IDataDictionary.Keys(SecurityClass security)
		{
			string[] keys = {
								"Override Date",
								"Site",
								"Fuel Type",
								"Created By",
								"WAC Value",
								"Notes",
								"OK",
								"Cancel",
								"WAC Detail",
								"* Denotes Required Field"
							};

			return keys;
		}
		#endregion // Data Dictionary

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				// ensure there is a WacSummary context in the session
				WacSummaryContext context = Session[WacSummary.CONTEXT_SESSION_KEY] as WacSummaryContext;
				// setup accounting...
				base.security = Session["Security"] as SecurityClass;

				if (null == context || null == base.security)
				{
					Session["Security"] = null;
					throw new FMSessionInvalidException();
				}

				// create accounting site
				this.accountingSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(base.security, base.security.SiteGuid)
																);

				converter = new SiteTimeConverter(accountingSite.CurrentSite);

				// check security
				if (!base.security.HasRight(RIGHT.VIEW_WAC_HISTORY))
				{
					throw new System.AccessViolationException("Access Denied");
				}

				this.BindControls();

				if (!this.IsPostBack)
				{
					this.LoadCommon();

					if (Request.Params.Get("WacIndex") != null)
					{
						this.LoadReadOnly();
					}
					else
					{
						this.LoadNew();
					}
				}

				this.EnableDisableControls();

				this.PopulateLabels();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void PopulateLabels()
		{
			lblHeading.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "WAC Detail")
																);

			labSite.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Site")
																);

			labProduct.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Fuel Type")
																);

			labLastEdit.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Created By")
																);

			labValue.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "WAC Value")
																);

			labNotes.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Notes")
																);

			btnOK.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "OK")
																);

			btnCancel.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Cancel")
																);

			lblOverrideDate.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Override Date")
																);

			lblRequiredFooter.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "* Denotes Required Field")
																);
		}

		protected void EnableDisableControls()
		{
			// always read only, otherwise this will be a security problem (will be populated by the session)
			overrideDateControl.Enabled = false;

			// these fields are enabled/disabled depending on which made we're running under
			tbWacValue.Enabled = !readOnlyMode;
			tbNotes.Enabled = !readOnlyMode;
			btnOK.Enabled = !readOnlyMode;

			// the last edit field is never editable by the user and is for display only, we use the user stored in security
			tbLastEdit.Enabled = false;
		}

		protected void LoadCommon()
		{
			// hmm maybe don't need this after all
		}

		protected void LoadReadOnly()
		{
			readOnlyMode = true;

			// retrieve the specified WAC
			string wacIndexStr = Request.Params.Get("WacIndex");
			int wacIndex = int.Parse(wacIndexStr);

			WeightedAverageCostClass wac = FMChannelHelper.MakeCall<IWeightedAverageCosts, WeightedAverageCostClass>(
																	 x =>
																	 x.GetByIndex(base.security, wacIndex)
																);
			if (null == wac)
			{
				base.ErrorHandler(new Exception("The request WAC(" + wacIndex + ") does not exist"));
				Close();
			}
			else
			{
				// for display, use date adjusted to date/time settings for the site
				DateTime displayDateTime = wac.CreatedDate;
				if (accountingSite != null)
				{
					displayDateTime = converter.ConvertToSiteTime(displayDateTime);
				}

				// fill date
				this.FillDate(displayDateTime);

				// fill site
				tbSite.Text = this.GetSiteName(wac.SiteGuid);

				// fill product
				tbFuelType.Text = this.GetProductName(wac.ProductGuid);

				// fill last edit
				tbLastEdit.Text = wac.CreatedBy;

				// fill wac value
				tbWacValue.Text = wac.WacValue.ToString("F6");

				// fill in notes
				tbNotes.Text = wac.Notes;
			}
		}

		protected void LoadNew()
		{
			readOnlyMode = false;

			// fill date
			this.FillDate(converter.ConvertToSiteTime(DateTime.UtcNow));

			// for others, we get the stuff from the sessions
			WacSummaryContext context = Session[WacSummary.CONTEXT_SESSION_KEY] as WacSummaryContext;
			if (null != context && null != base.security)
			{
				tbSite.Text = context.WacSite;
				tbFuelType.Text = context.WacFuelType;
				tbLastEdit.Text = base.security.UserID;
				tbWacValue.Text = "0.000000";
				tbNotes.Text = "";
			}
			else
			{

			}
		}

		protected delegate void UpdateDate(DateTime a_date);
		protected void FillDate(DateTime a_date)
		{
			//overrideDateControl.DateTime = a_date;

			WacSummaryContext context = Session[WacSummary.CONTEXT_SESSION_KEY] as WacSummaryContext;

			overrideDateControl.Text = context.AcctSite.FormatDateTime(a_date);

			//overrideDateControl.Text = a_date.ToString();
		}

		protected string GetSiteName(Guid a_guid)
		{
			SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 x =>
																	 x.Get(base.security, a_guid, false, false, false)
																);
			return site.ID;
		}

		protected string GetProductName(int a_index)
		{
			string result = null;

			ProductCollectionClass col = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(base.security)
																);
			foreach (ProductClass product in col)
			{
				if (product.Index == a_index)
				{
					result = product.ID;
					break;
				}
			}

			return result;
		}

		protected void BindControls()
		{
			this.btnCancel.Click += new EventHandler(btnCancel_Click);
			this.btnOK.Click += new EventHandler(btnOK_Click);
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			// prevent OK from being clicked again
			btnOK.Enabled = false;

			try
			{
				saving = this.Save();

				if (saving)
					this.Close();
				else
					btnOK.Enabled = true;
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		protected bool Save()
		{
			saving = true;

			// get the relevant values from the session as well
			WacSummaryContext context = Session[WacSummary.CONTEXT_SESSION_KEY] as WacSummaryContext;
			if (null == context || null == base.security)
			{
				base.ErrorHandler(new Exception("Invalid session"));
				base.DisplayErrorPage();
				return true; // continue to close
			}

			WeightedAverageCostDO wac = new WeightedAverageCostDO();
			wac.CreatedBy = base.security.UserID;
			wac.CreatedDate = DateTime.UtcNow;
			wac.IsManualOverride = true;
			wac.Notes = tbNotes.Text;
			wac.ProductGuid = FMChannelHelper.MakeCall<IProducts, Guid>(
																	 x =>
																	 x.GetMasterRecordGuid ( base.security, context.WacFuelType )
																);

			wac.SiteGuid = FMChannelHelper.MakeCall<ISites, Guid>(
																	 x =>
																	 x.GetIdentityGuid(base.security, context.WacSite)
																);

			wac.WacValue = double.Parse(tbWacValue.Text);
			wac.Source = "";
			wac.WeightedAverageCostGuid = Guid.Empty;

			// Set the Inventory Date to local date at midnight.
			DateTime invDate = DateTime.Now;
			wac.InventoryDate = new DateTime(invDate.Year, invDate.Month, invDate.Day, 0, 0, 0);

			// validate values
			if (wac.ProductGuid == Guid.Empty)
			{
				base.ErrorHandler(new Exception("Selected product no longer exists"));
				return true;
			}

			if (wac.SiteGuid == Guid.Empty)
			{
				base.ErrorHandler(new Exception("Selected site no longer exists"));
				return true;
			}

			if (wac.WacValue < 0.0)
			{
				base.ErrorHandler(new Exception("WAC value must be greater or equal to 0.0"));
				return false;
			}

			SaveWeightedAverageCostsSR sr = new SaveWeightedAverageCostsSR(base.security);
			sr.WeightedAverageCosts.Add(wac);

			CustomResultDO results = FMChannelHelper.MakeCall<ISaveWeightedAverageCostsProcessor, CustomResultDO>(
																	 x =>
																	 x.Process(sr)
																);

			if (results.Errors.Count > 0)
			{
				base.ErrorHandler(results.Errors[0]);
				return false;
			}

			return true;
		}

		protected void Close()
		{
			this.Redirect(WacOverride.RETURN_PAGE);
		}
	}
}
