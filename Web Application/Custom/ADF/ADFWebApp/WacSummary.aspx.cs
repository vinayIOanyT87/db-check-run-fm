using FMControls;
using Accounting;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.Exceptions;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;

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
using System.Globalization;

namespace ADFWebApp
{
	#region Context
	public class WacSummaryContext : BaseContext
	{
		public string WacSite { get; set; }
		public string WacFuelType { get; set; }
		public short DisplayPrecision { get; set; }

		public WacSummaryContext()
			: base()
		{
			this.WacSite = null;
			this.WacFuelType = null;
			this.DisplayPrecision = 6;
		}
	}
	#endregion // Context

	public partial class WacSummary : AccountingAutoSubmitWebFormView, IDataDictionary
	{
		public static string CONTEXT_SESSION_KEY = typeof(WacSummaryContext).ToString();
		protected static string GRID_SESSION_KEY = typeof(WacSummary).ToString() + ".ListView";
		protected WacSummaryContext context = null;
		protected SiteTimeConverter converter = null;

		#region Properties
		public short DisplayPrecision { get; set; }
		#endregion // Properties

		#region Data Dictionary
		string[] IDataDictionary.Keys(SecurityClass security)
		{
			string[] keys = 
			{
				"WAC Summary",
				"Start Date",
				"End Date",
				"Site",
				"Fuel Type",
				"Refresh",
				"Add",
				"Edit",
				"Date",
				"Value",
				"Source"
			};

			return keys;
		}
		#endregion // Data Dictionary

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				context = this.GetContext();

				// setup accounting...
				base.security = Session["Security"] as SecurityClass;
				if (base.security == null)
				{
					throw new FMSessionInvalidException();
				}

				context.AcctSite = FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
																	 x =>
																	 x.LoadSiteInfo(base.security, base.security.SiteGuid)
																);

				converter = new SiteTimeConverter(context.AcctSite.CurrentSite);

				Session[WacSummary.CONTEXT_SESSION_KEY] = context;

				// setup event handling...
				this.BindControls();

				// check security
				bool ok = this.SecurityProcessing();

				if (ok == false)
				{
					throw new System.AccessViolationException("Access Denied");
				}

				if (!this.IsPostBack)
				{
					// populate dropdowns
					this.BuildSiteDropDownCtrl();
					this.BuildFuelTypeDropDownCtrl();

					this.BuildDictionaryLabels();
					this.BuildInterfaceFromContext();

					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void BuildDictionaryLabels()
		{

			lblHeading.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "WAC Summary")
																);

			labStartDate.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Start Date")
																);
			;
			labEndDate.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "End Date")
																);

			labFuelType.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Fuel Type")
																);

			btnRefresh1.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Refresh")
																);

			btnAddTop.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Add")
																);

			btnAddBottom.Text = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Add")
																);


			WacGrid.Columns[0].HeaderText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "View")
																);

			WacGrid.Columns[1].HeaderText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Date")
																);

			WacGrid.Columns[2].HeaderText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Value")
																);

			WacGrid.Columns[3].HeaderText = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(base.security.SiteGuid, "Source")
																);

		}

		protected bool SecurityProcessing()
		{
			bool returnVal = false;

			if (null == Session["Security"] || null == base.security)
			{
				Session.RemoveAll();
				this.DisplayErrorPage();
			}
			else
			{
				this.btnAddTop.Enabled = security.HasRight(RIGHT.OVERRIDE_WAC);
				this.btnAddBottom.Enabled = security.HasRight(RIGHT.OVERRIDE_WAC);
				returnVal = security.HasRight(RIGHT.VIEW_WAC_HISTORY);
			}

			return returnVal;
		}

		protected void BindControls()
		{
			this.WacGrid.PageIndexChanged += new DataGridPageChangedEventHandler(WacGrid_PageIndexChanged);
			this.WacGrid.ItemDataBound += new DataGridItemEventHandler(WacGrid_ItemDataBound);
			this.btnAddTop.Command += new CommandEventHandler(AddButton_Command);
			this.btnAddBottom.Command += new CommandEventHandler(AddButton_Command);
			this.ddlSite.SelectedIndexChanged += new EventHandler(ddlSite_SelectedIndexChanged);
			this.ddlSite.AutoPostBack = true;
		}

		protected DataView BuildDataView(WeightedAverageCostCollectionClass wacCollection)
		{
			DataView result = null;

			WacSummaryContext context = this.GetContext();
			DateTimeFormatInfo formatInfo = context.AcctSite.CurrentSite.GetDateTimeFormatInfo();

			DataTable table = new DataTable();
			table.Columns.Add("Date", typeof(string));
			table.Columns.Add("Value", typeof(string));
			table.Columns.Add("Source", typeof(string));
			table.Columns.Add("Index", typeof(string));
			table.Columns.Add("DateValue", typeof(long));

			// JS20100311 generate our collection for filtering on changes
			WeightedAverageCostCollectionClass collection = new WeightedAverageCostCollectionClass();
			if (!this.radioListShow.SelectedValue.ToUpper().Equals("ALL"))
			{
				double lastWacValue = -1.0;

				for (int i = wacCollection.Count - 1; i >= 0; --i)
				{
					// Wallaby: if only showing changed (not all) then should compare the values as we
					// fill the data
					WeightedAverageCostClass wac = wacCollection[i];

					if (lastWacValue >= 0.0)
					{
						if (lastWacValue == wac.WacValue)
						{
							continue;
						}
					}

					lastWacValue = wac.WacValue;
					collection.Add(wac);
				}
			}
			else
			{
				collection = wacCollection;
			}

			// add one row per query in the collection in order (i.e. wacindex desc)
			foreach (WeightedAverageCostClass wac in collection)
			{
				DataRow row = table.NewRow();

				// convert UTC date to display date
				DateTime siteDateTime = converter.ConvertToSiteTime(wac.CreatedDate.UtcDateTime);

				row["Date"] = siteDateTime.ToString(formatInfo);
				row["Value"] = wac.WacValue.ToString("F" + context.DisplayPrecision);

				if (wac.IsManualOverride)
				{
					row["Source"] = "User Override";
				}
				else
				{
					row["Source"] = wac.Alias;
				}

				row["Index"] = wac.WeightedAverageCostGuid.ToString();
				row["DateValue"] = wac.CreatedDate.UtcDateTime.ToBinary();

				table.Rows.Add(row);
			}

			result = new DataView(table);

			return result;
		}

		#region LineItem Events
		protected void WacGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			try
			{
				this.WacGrid.CurrentPageIndex = e.NewPageIndex;

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void WacGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemIndex != -1)
			{
				LinkButton EditButton = (LinkButton)e.Item.FindControl("EditLinkButton");

				// Leave hard space zero length string
				DataRowView view = e.Item.DataItem as DataRowView;
				string wacIndexStr = (string)view.Row.ItemArray[3];

				if (wacIndexStr != null)
				{
					// get the actual WAC details to work out if it's an user override
					WeightedAverageCostClass wac = FMChannelHelper.MakeCall<IWeightedAverageCosts, WeightedAverageCostClass>(
                                                    x =>
                                                    x.GetByIndex(base.security, int.Parse(wacIndexStr))
                                                );

					// change the text on the edit button
					//EditButton.Text = "View";

					if (wac.IsManualOverride)
					{
						EditButton.Attributes["href"] = "javascript:EditWacItem('" + wac.WeightedAverageCostGuid + "')";
					}
					else
					{
						EditButton.Attributes["href"] = "TransactionDetail.aspx" +
								"?TransID=" + wac.Source +
								"&" + TransactionDetail.CUSTOM_REDIRECT_PARAM + "=WacSummary.aspx" +
								"&" + "disableAll=true";
					}
				}
			}
		}
		#endregion // LienITem Events

		protected void AddButton_Command(object source, EventArgs e)
		{
			this.BuildContextFromInterface();

			this.Redirect("WacOverride.aspx");
		}

		protected delegate Pair DelegateExtractName(object a_obj);

		#region Delegates for DelegateExtractName
		protected Pair ExtractSiteName(object obj)
		{
			string name = null;
			string value = null;

			SiteClass site = obj as SiteClass;

			if (null != site)
			{
				if (!site.SiteGroup)
				{
					name = site.ID;
					value = /*site.IdentityGuid*/BaseDataObject.DUMMY_INDEX.ToString();
				}
			}
			Pair result = null;

			if (name != null && value != null)
				result = new Pair(name, value);

			return result;
		}

		protected Pair ExtractProductName(object obj)
		{
			ProductClass product = obj as ProductClass;

			return new Pair(product.ID, product.Index.ToString());
		}
		#endregion // Delegates for DelegateExtractName

		protected void BuildSiteDropDownCtrl()
		{
			SiteCollectionClass collection = FMChannelHelper.MakeCall<ISites, SiteCollectionClass>(
																	 x =>
																	 x.EnumerateByParentSite(security, security.SiteGuid)
																);


			this.BuildDropDownCtrlSite(this.ddlSite, collection, new DelegateExtractName(ExtractSiteName));

			// select the one from context (if any)
			WacSummaryContext context = this.GetContext();

			if (context.WacSite != null)
			{
				this.ddlSite.SelectedIndex = this.FindIndex(context.WacSite, this.ddlSite);
			}
			else
			{
				this.BuildContextFromInterface();
			}
		}

		protected void ddlSite_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.BuildFuelTypeDropDownCtrl();
		}

		protected void BuildFuelTypeDropDownCtrl()
		{
			ProductCollectionClass collection = null;

			this.ddlFuelType.Items.Clear();

			if (this.ddlSite.SelectedItem == null)
			{
				collection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(base.security)
																);
			}
			else
			{
				// temporarily change security to grab the right list of items
				SecurityClass tempSecurity = base.security;

				int originalSiteIndex = base.security.SiteIndex;

				//int selectedSiteIndex = sites.GetIndex ( security, ddlSite.SelectedItem.Text );
				int selectedSiteIndex = EntityToSiteMapClass.DUMMY_INDEX;
				tempSecurity.SiteIndex = selectedSiteIndex;

				collection = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(
																	 x =>
																	 x.Enumerate(tempSecurity)
																);

				tempSecurity.SiteIndex = originalSiteIndex;
			}

			this.BuildDropDownCtrlProduct(this.ddlFuelType, collection, new DelegateExtractName(ExtractProductName));

			// select the fuel type from context (if any)
			WacSummaryContext context = this.GetContext();
			if (context.WacFuelType != null)
			{
				this.ddlFuelType.SelectedIndex = this.FindIndex(context.WacFuelType, this.ddlFuelType);
			}
			else
			{
				this.BuildContextFromInterface();
			}
		}

		protected int FindIndex(string text, DropDownList list)
		{
			for (int i = 0; i < list.Items.Count; ++i)
			{
				ListItem li = list.Items[i];

				if (li.Text.Equals(text))
				{
					return i;
				}
			}

			return 0;
		}

		protected void BuildDropDownCtrlProduct(DropDownList ctrl, ProductCollectionClass collection, DelegateExtractName extractor)
		{
			// add all the names to the drop down list data source
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Pair pair = extractor(enumerator.Current);
				if (null != pair)
				{
					ListItem li = new ListItem((string)pair.First, (string)pair.Second);
					ctrl.Items.Add(li);
				}
			}

			// default to first selection
			if (ctrl.Items.Count > 0)
			{
				ctrl.SelectedIndex = 0;
			}
		}


		protected void BuildDropDownCtrlSite(DropDownList ctrl, SiteCollectionClass collection, DelegateExtractName extractor)
		{
			// add all the names to the drop down list data source
			IEnumerator enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Pair pair = extractor(enumerator.Current);
				if (null != pair)
				{
					ListItem li = new ListItem((string)pair.First, (string)pair.Second);
					ctrl.Items.Add(li);
				}
			}

			// default to first selection
			if (ctrl.Items.Count > 0)
			{
				ctrl.SelectedIndex = 0;
			}
		}

		protected WacSummaryContext GetContext()
		{
			// check for existing context in session
			context = (WacSummaryContext)Session[WacSummary.CONTEXT_SESSION_KEY];

			if (null == context)
			{
				context = new WacSummaryContext();
			}

			return context;
		}

		protected void BuildInterfaceFromContext()
		{
			this.startDateControl.CurrentValue = converter.ConvertToSiteTime(context.StartDate);
			this.endDateControl.CurrentValue = converter.ConvertToSiteTime(context.EndDate);

			if (context.WacFuelType != null)
			{
				this.ddlFuelType.SelectedValue = context.WacFuelType;
			}

			if (context.WacSite != null)
			{
				this.ddlSite.SelectedValue = context.WacSite;
			}
		}

		protected void BuildContextFromInterface()
		{
			context.StartDate = converter.ConvertFromSiteTime(this.startDateControl.CurrentValue);
			context.EndDate = converter.ConvertFromSiteTime(this.endDateControl.CurrentValue);

			if (this.ddlSite.SelectedItem != null)
			{
				context.WacSite = this.ddlSite.SelectedItem.Text;
			}

			if (this.ddlFuelType.SelectedItem != null)
			{
				context.WacFuelType = this.ddlFuelType.SelectedItem.Text;
			}

			Session[WacSummary.CONTEXT_SESSION_KEY] = context;
		}

		protected WeightedAverageCostClass[] FilterWac(WeightedAverageCostCollectionClass wacCollection)
		{
			WeightedAverageCostClass[] result = new WeightedAverageCostClass[wacCollection.Count];

			int index = 0;

			foreach (WeightedAverageCostClass wac in wacCollection)
			{
				// check within date range, need to compare to utc date
				if (wac.CreatedDate < converter.ConvertFromSiteTime(this.startDateControl.CurrentValue) ||
					wac.CreatedDate > converter.ConvertFromSiteTime(this.endDateControl.CurrentValue))
				{
					continue;
				}

				// check matches site
				ListItem selected = this.ddlSite.SelectedItem;
				if (wac.SiteIndex != int.Parse(selected.Value))
				{
					continue;
				}

				// check matches fuel
				selected = this.ddlFuelType.SelectedItem;
				if (wac.ProductGuid != int.Parse(selected.Value))
				{
					continue;
				}

				// at this point, matches filter criteria
				result[index++] = wac;
			}

			return result;
		}

		protected void UpdateView()
		{
			try
			{
				// validate start and end dates
				if (this.startDateControl.CurrentValue.ToBinary() > this.endDateControl.CurrentValue.ToBinary())
				{
					this.startDateControl.Focus(); // focus to error

					base.ErrorHandler(new Exception("Start date must be prior to the end date"));
				}
				else
				{
					// get the necessary data for wac enumeration
					int productIndex = int.Parse(this.ddlFuelType.SelectedItem.Value);
					int siteIndex = int.Parse(this.ddlSite.SelectedItem.Value);


					// need to figure out actual start & end dates using site configuration
					DateTime utcStartDate = converter.ConvertFromSiteTime(this.startDateControl.CurrentValue);
					DateTime utcEndDate = converter.ConvertFromSiteTime(this.endDateControl.CurrentValue.AddDays(1.0));

					// perform the enumeration
					WeightedAverageCostCollectionClass wacCollection = 
						FMChannelHelper.MakeCall<IWeightedAverageCosts, WeightedAverageCostCollectionClass>(
								x =>
								x.EnumerateBySiteProductDate(base.security, siteIndex, productIndex, utcStartDate, utcEndDate)
						);

					DataView dv = this.BuildDataView(wacCollection);

					dv.Sort = "DateValue DESC";

					this.ddlPageSize.SetPageSize(this.WacGrid, wacCollection.Count);

					this.WacGrid.DataSource = dv;
					this.WacGrid.DataBind();

					// set page size
					int size = int.Parse(this.ddlPageSize.SelectedValue);
					if (size <= 0) // means all
					{
						this.WacGrid.PageSize = wacCollection.Count;
					}
					else
					{
						this.WacGrid.PageSize = size;
					}
				}
			}
			catch (Exception e)
			{
				base.ErrorHandler(e);
			}
		}

		protected void btnRefresh1_Click(object sender, EventArgs e)
		{
			try
			{
				// reset page to avoid invalid index messages, i.e. if viewer was refreshed to a fuel with only 1 page fuel type
				// and the current page index is 2 on the current.
				this.WacGrid.CurrentPageIndex = 0;

				this.BuildContextFromInterface();

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}

		protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				// reset to first page to avoid invalid page errors
				this.WacGrid.CurrentPageIndex = 0;

				this.UpdateView();
			}
			catch (Exception ex)
			{
				base.ErrorHandler(ex);
			}
		}
	}
}
