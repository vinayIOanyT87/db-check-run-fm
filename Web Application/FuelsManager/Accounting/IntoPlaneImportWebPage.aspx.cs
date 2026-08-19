namespace FuelsManager.Accounting
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Text;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using FMDepedencyManager;
	using Unity;

	public partial class IntoPlaneImportWebPage : AccountingWebFormView
	{
		protected AccountingSite accountingSite;
		protected System.Globalization.DateTimeFormatInfo _dateFormat = System.Globalization.DateTimeFormatInfo.CurrentInfo;

		private void InitializeComponent()
		{
		}

		protected override void OnInit(EventArgs e)
		{
			this.InitializeComponent();
			this.CurrentSiteGuid = Guids.SiteAdminGuid;
			this.Initialize();
			base.OnInit(e);
		}

		/// <summary>
		///     This is the main entry point for the IntoPlaneImport page.  It is called by IIS.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				this.accountingSite =
					FMChannelHelper.MakeCall<IAccountingSites, AccountingSite>(
							x => x.LoadSiteInfo(this.security, this.security.SiteGuid));

				if (this.Page.IsPostBack == false)
				{
					this.titleLable.Text = DataDictionarySingleton.Get(this.security.SiteGuid, "Import IntoPlane Data");
					this.ImportButton.Enabled = false;
					this.fileUpload.Enabled = true;

					List<ProductRow> pr = new List<ProductRow>();
					this.ProductDataGrid.DataSource = pr;
					this.ProductDataGrid.DataBind();
				}
				else
				{
					if (this.fileUpload.Enabled && this.fileUpload.HasFile)
					{
						this.LoadFile();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private bool SetManagerList(IEnumerable<string> managers)
		{
			CompanyCollectionClass companies =
				 FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
				 x => x.EnumerateByRole(this.security, COMPANY_ROLE.MANAGER, false, false));

			this.managerList.Items.Clear();
			this.managerList.Items.Add("");
			foreach (CompanyClass company in companies)
			{
				if (managers.Contains(company.ID))
				{
					this.managerList.Items.Add(company.ID);
				}
			}

			return managerList.Items.Count > 1;
		}

		protected void ClearButton_Click(object sender, EventArgs e)
		{
			try
			{
				ClearForm();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}


		/// <summary>
		/// Parses the selected file and gets a list of all of the included products.
		/// A warning is given if an included product is not configured for the site.
		/// </summary>
		protected void LoadFile()
		{
			try
			{
				if (!this.fileUpload.HasFile || string.IsNullOrWhiteSpace(this.fileUpload.FileName))
				{
					throw new ApplicationException("No file selected.");
				}

				if (this.fileUpload.PostedFile.ContentType != "text/csv")
				{
					throw new ApplicationException("File selected must be a csv.");
				}

				string data = System.Text.ASCIIEncoding.ASCII.GetString(this.fileUpload.FileBytes);
				//save file data on server side to avoid uploading twice
				Session["FileData"] = data;
				var records = data.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList<string>();

				//remove header
				var headerFields = new IntoPlaneImportFields(new SortedList<string, int>());
				headerFields.ParseValues(records[0]);

				if (headerFields.ValidateHeader(out string message, out SortedList<string, int> headers))
				{
					records.RemoveAt(0);
				}
				else
				{
					throw new ApplicationException(message);
				}

				this.txtResults.Text = String.Empty;
				records = records.Where(r => !String.IsNullOrEmpty(r) && !r.All(c => c == ',')).ToList();

				StringBuilder errors = new StringBuilder();
				int managerColum = headers[IntoPlaneImportFieldNames.Manager];
				IEnumerable<string> managers = records.Select(x => x.Split(',')[managerColum]).Distinct();
				if (!SetManagerList(managers))
				{
					errors.AppendLine(String.Format("No valid managers found in file for site: {0}.", this.accountingSite.CurrentSiteName));
				}

				int productColum = headers[IntoPlaneImportFieldNames.Product];
				var products = records.Select(x => x.Split(',')[productColum]).Distinct();

				//Get products that are configured for this site.
				var productsProxy = FMServiceLocator.Container.Resolve<IProducts>();
				var configuredProducts = productsProxy.EnumerateBySite(this.security);

				//list of products in the file that are configured for the site to be bound to the DataGrid
				var prodRows = products.Where(p => configuredProducts.Any(x => x.ID == p)).Select(q => new ProductRow { Product = q, Vcf = "", Temp = "", Gravity = "" }).ToList<ProductRow>();

				//list of products in the file that are not configured for the site
				//alert about these

				var badProdRows = products.Except(configuredProducts.Select(cp => cp.ID));
				if (badProdRows.Count() > 0)
				{
					foreach (string s in badProdRows)
					{
						errors.AppendLine(String.Format("Product {0} not configured for site: {1}.", s, this.accountingSite.CurrentSiteName));
					}
				}

				this.ProductDataGrid.DataSource = prodRows;
				this.ProductDataGrid.DataBind();

				string loadResults = records.Count.ToString() + " transaction" + (records.Count > 1 ? "s" : String.Empty) + " found in import file." + Environment.NewLine;
				if (errors.Length > 0)
				{
					loadResults += errors.ToString() + Environment.NewLine;

					//Manage Control States
					this.fileUpload.Enabled = true;
					this.ImportButton.Enabled = false;
				}
				else
				{
					//Manage Control States
					this.fileUpload.Enabled = false;
					this.ImportButton.Enabled = true;
				}

				FMTBFilePath.Text = fileUpload.FileName;
				this.results.InnerHtml = "<p>" + loadResults + "</p>";

				this.uploadIcon.ImageUrl = "~/Content/icons/cyan-upload-icon.png";
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void UploadButton_Click(object sender, EventArgs e)
		{
			try
			{
				this.SubmitFile();

				//Manage Control States
				ImportButton.Enabled = false;

				Session["FileData"] = string.Empty;
				List<ProductRow> pr = new List<ProductRow>();
				ProductDataGrid.DataSource = pr;
				ProductDataGrid.DataBind();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ClearForm()
		{
			results.InnerHtml = string.Empty;
			FMTBFilePath.Text = "No file selected";
			Session["FileData"] = string.Empty;
			List<ProductRow> pr = new List<ProductRow>();
			ProductDataGrid.DataSource = pr;
			ProductDataGrid.DataBind();
			ImportButton.Enabled = false;
			fileUpload.Enabled = true;
			txtResults.Text = string.Empty;
			StartDate.Text = string.Empty;
			EndDate.Text = string.Empty;
			managerList.SelectedIndex = -1;
			uploadIcon.ImageUrl = "~/Content/icons/gray-upload-icon.png";
		}


		private void SubmitFile()
		{
			try
			{
				ProductDataGrid.Visible = true;
				IntoPlaneImportParametersDO paramDO = new IntoPlaneImportParametersDO();
				if (StartDate.Text.Length < 1)
				{
					throw new ApplicationException("Select a valid Start Date.");
				}
				else
				{
					paramDO.StartDateFilter = StartDate.DateTimeValue;
				}
				if (EndDate.Text.Length < 1)
				{
					throw new ApplicationException("Select a valid End Date.");
				}
				else
				{
					paramDO.EndDateFilter = EndDate.DateTimeValue;
				}

				paramDO.ManagerFilter = this.managerList.Text;
				if (string.IsNullOrWhiteSpace(paramDO.ManagerFilter))
				{
					throw new ApplicationException("Select a valid Manager.");
				}
				paramDO.UseTempGravVCFParam = false;

				foreach (DataGridItem itm in ProductDataGrid.Items)
				{
					string prod = ((TextBox)itm.FindControl("txtProduct")).Text;
					string vcf = ((TextBox)itm.FindControl("txtVcf")).Text;
					string temp = ((TextBox)itm.FindControl("txtTemperature")).Text;
					string grav = ((TextBox)itm.FindControl("txtGravity")).Text;

					if (!string.IsNullOrEmpty(vcf) || !string.IsNullOrEmpty(temp) || !string.IsNullOrEmpty(grav))
					{
						paramDO.UseTempGravVCFParam = true;
						IntoPlaneImportTempGravVcfParams vcfTempGrav = new IntoPlaneImportTempGravVcfParams();

						if (!string.IsNullOrEmpty(vcf) && (!string.IsNullOrEmpty(temp) || !string.IsNullOrEmpty(grav)))
						{
							//use entered Vcf in UI
							throw new ApplicationException("You can not enter Temperature or Gravity with Vcf");
						}

						if (!string.IsNullOrEmpty(vcf))
						{
							//use entered Vcf in UI
							double dVcf;
							if (Double.TryParse(vcf, out dVcf))
							{
								vcfTempGrav.VCF = dVcf;
							}
							else
							{
								//error: bad gravity.  full stop
								throw new ApplicationException("Please enter a valid value for the Vcf.");
							}
						}
						else
						{
							vcfTempGrav.VCF = 0;
							//use entered Temp/Grav
							if (string.IsNullOrEmpty(temp) || string.IsNullOrEmpty(grav))
							{
								//error: need both temp and grav.  full stop
								throw new ApplicationException("Please enter both a Temperature and Gravity.");
							}
							double dTemp;
							if (Double.TryParse(temp, out dTemp))
							{
								vcfTempGrav.Temperature = dTemp;
							}
							else
							{
								//error: bad temp. full stop.
								throw new ApplicationException("Please enter a valid value for the Temperature.");
							}

							double dGrav;
							if (Double.TryParse(grav, out dGrav))
							{
								vcfTempGrav.Gravity = dGrav;
							}
							else
							{
								//error: bad gravity. full stop.
								throw new ApplicationException("Please enter a valid value for the Gravity.");
							}
						}
						paramDO.AddTempGravityVCFParam(prod, vcfTempGrav);
					}
				}

				string data = Session["FileData"] as string;
				if (string.IsNullOrEmpty(data))
				{
					this.ClearForm();
					throw new ApplicationException("There is no data in this file.  Please select a file.");
				}

				string result = FMChannelHelper.MakeCall<IIntoPlaneImport, string>(
																	 x =>
																	 x.ImportData(this.accountingSite.Security, data, paramDO)
																);
				result = result.Replace(Environment.NewLine, "<br>");
				this.txtResults.Text = result;
				this.txtResults.Text += (string.IsNullOrEmpty(this.txtResults.Text) ? string.Empty : "<br>") + "Import Completed.";
			}
			catch (Exception ex)
			{
				throw new ApplicationException(ex.Message);
			}

		}
	}

	/// <summary>
	/// Object used to bind to ProductDataGrid
	/// maybe use anonymous type instead?
	/// </summary>
	public class ProductRow
	{
		public string Product { get; set; }
		public string Vcf { get; set; }
		public string Temp { get; set; }
		public string Gravity { get; set; }
	}
}