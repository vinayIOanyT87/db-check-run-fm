namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// Summary description for ProductAlarmsPage.
	/// </summary>
	public partial class ProductAlarmsPage : ProductPageBase
	{
		protected FMControls.FMLabel Label1;
		protected FMControls.FMLabel Label2;
		protected FMControls.FMLabel TemperatureUnitsLabel;
	
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				if (! this.Page.IsPostBack) 
				{
						  this.UpdateView();
						  this.SetFieldAccessibilityForChildRecordVersion();
				}
			}	
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{	 

		}
		#endregion

		public void UpdateView()
		{
			try
			{
				this.DensityHighLimitTextbox.Text = this.Product.DensityHighLimit;
				this.DensityLowLimitTextbox.Text = this.Product.DensityLowLimit;
				this.DensityHighMinusDeadbandTextbox.Text = this.Product.DensityHighMinusDeadband;
				this.DensityLowPlusDeadbandTextbox.Text = this.Product.DensityLowPlusDeadband;
				this.ApplyDensityLimitsCheckBox.Checked = Convert.ToBoolean(this.Product.ApplyDensityLimits);
				this.TemperatureHiHiLimitTextbox.Text = this.Product.TemperatureHiHiLimit;
				this.TemperatureHighLimitTextbox.Text = this.Product.TemperatureHighLimit;
				this.TemperatureLowLimitTextbox.Text = this.Product.TemperatureLowLimit;
				this.TemperatureLoLoLimitTextbox.Text = this.Product.TemperatureLoLoLimit;
				this.TemperatureDeadbandTextbox.Text = this.Product.TemperatureDeadband;
				this.ApplyTemperatureLimitsCheckBox.Checked = Convert.ToBoolean(this.Product.ApplyTemperatureLimits);
			}
			catch (Exception e)
			{
				string msg = "Product Alarms Page - " + e.Message;
				throw new Exception(msg);
			}
		}

		public void ValidateDataToUI()
		{
			string msg = string.Empty;

			try
			{
				var d = new SIDouble { Units = this.Product.DensityUnits };
				msg = "Product Alarms Page - Density High Limit : ";
				d.SIValue = this.Product._DensityHighLimit.SIValue;

				msg = "Product Alarms Page - Density Low Limit : ";
				d.SIValue = this.Product._DensityLowLimit.SIValue;
			}
			catch (Exception e)
			{
				throw new Exception(msg + e.Message);
			}
		}

		public void ValidateDataFromUI()
		{
			string msg = "";
			try
			{
				var d = new SIDouble { Units = this.Product.DensityUnits };

				msg = "Product Alarms Page - Density High Limit : ";
				d.Value = Double.Parse(this.DensityHighLimitTextbox.Text, this.Product._DensityHighLimit.Format);
				msg = "Product Alarms Page - Density Low Limit : ";
				d.Value = Double.Parse(this.DensityLowLimitTextbox.Text, this.Product._DensityLowLimit.Format);
				msg = "Product Alarms Page - Density High Limit Minus Limit : ";
				d.Value = Double.Parse(this.DensityHighMinusDeadbandTextbox.Text, this.Product._DensityHighLimit.Format);
				msg = "Product Alarms Page - Density Low Limit Plus Deadband Limit : ";
				d.Value = Double.Parse(this.DensityLowPlusDeadbandTextbox.Text, this.Product._DensityLowLimit.Format);

				d.Units = this.Product.TemperatureUnits;
				msg = "Product Alarms Page - Temperature HiHi Limit : ";
				d.Value = Double.Parse(this.TemperatureHiHiLimitTextbox.Text, this.Product._TemperatureHiHiLimit.Format);
				msg = "Product Alarms Page - Temperature High Limit : ";
				d.Value = Double.Parse(this.TemperatureHighLimitTextbox.Text, this.Product._TemperatureHighLimit.Format);
				msg = "Product Alarms Page - Temperature Low Limit : ";
				d.Value = Double.Parse(this.TemperatureLowLimitTextbox.Text, this.Product._TemperatureLowLimit.Format);
				msg = "Product Alarms Page - Temperature LoLo Limit : ";
				d.Value = Double.Parse(this.TemperatureLoLoLimitTextbox.Text, this.Product._TemperatureLoLoLimit.Format);
				msg = "Product Alarms Page - Temperature Deadband : ";
				d.Value = Double.Parse(this.TemperatureDeadbandTextbox.Text, this.Product._TemperatureHiHiLimit.Format);
			}
			catch (Exception e)
			{
				throw new Exception(msg + e.Message);
			}

		}

		public void UpdateData()
		{
			try
			{
				this.Product.DensityHighLimit=this.DensityHighLimitTextbox.Text;
				this.Product.DensityLowLimit=this.DensityLowLimitTextbox.Text;
				this.Product.ApplyDensityLimits=this.ApplyDensityLimitsCheckBox.Checked;
				this.Product.TemperatureHiHiLimit=this.TemperatureHiHiLimitTextbox.Text;
				this.Product.TemperatureHighLimit=this.TemperatureHighLimitTextbox.Text;
				this.Product.TemperatureLowLimit=this.TemperatureLowLimitTextbox.Text;
				this.Product.TemperatureLoLoLimit=this.TemperatureLoLoLimitTextbox.Text;
				this.Product.TemperatureDeadband=this.TemperatureDeadbandTextbox.Text;
				this.Product.ApplyTemperatureLimits=this.ApplyTemperatureLimitsCheckBox.Checked;
			}
			catch(Exception e)
			{
				string msg = "Product Alarms Page - " + e.Message;
				throw new Exception(msg);
			}
			
		}

		/// <summary>
		/// Handles the TextChanged event of the DensityHighLimitTextbox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void DensityHighLimitTextboxTextChanged(object sender, EventArgs e)
		{
			string oldVal = this.Product.DensityHighLimit;
			
			try
			{
				this.UpdateData();

				if (this.DensityHighMinusDeadbandTextbox.Text != this.Product.DensityHighMinusDeadband)
				{
					var d = new SIDouble()
						        {
							        Units = this.Product._DensityHighLimit.Units,
							        Value =
								        this.Product._DensityHighLimit.Value
								        - Double.Parse(this.DensityHighMinusDeadbandTextbox.Text, this.Product._DensityHighLimit.Format)
						        };
					this.Product.DensityDeadband = System.Convert.ToString(d.SIValue);
				}

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.DensityHighLimitTextbox.Text = oldVal;
				this.ErrorHandler(ex);
			}		
		}

		/// <summary>
		/// Handles the TextChanged event of the DensityLowLimitTextbox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void DensityLowLimitTextboxTextChanged(object sender, EventArgs e)
		{
			string oldVal = this.Product.DensityLowLimit;

			try
			{
				this.UpdateData();

				if (this.DensityLowPlusDeadbandTextbox.Text != this.Product.DensityLowPlusDeadband)
				{
					var d = new SIDouble()
						        {
							       Units = this.Product._DensityLowLimit.Units,
							       Value = 
										Double.Parse(this.DensityLowPlusDeadbandTextbox.Text, this.Product._DensityLowLimit.Format)
										- this.Product._DensityLowLimit.Value
						        };
					this.Product.DensityDeadband = System.Convert.ToString(d.SIValue);
				}

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.DensityLowLimitTextbox.Text = oldVal;
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Handles the TextChanged event of the DensityHighMinusDeadbandTextbox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void DensityHighMinusDeadbandTextboxTextChanged(object sender, EventArgs e)
		{

			string oldVal = this.Product.DensityHighMinusDeadband;

			try
			{
				this.UpdateData();
				if (this.DensityHighMinusDeadbandTextbox.Text != this.Product.DensityHighMinusDeadband)
				{
					var d = new SIDouble()
						        {
							        Units = this.Product._DensityHighLimit.Units,
							        Value =
								        this.Product._DensityHighLimit.Value
										- Double.Parse(this.DensityHighMinusDeadbandTextbox.Text, this.Product._DensityHighLimit.Format)
						        };
					this.Product.DensityDeadband = System.Convert.ToString(d.SIValue);
				}
				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.DensityHighMinusDeadbandTextbox.Text = oldVal;
				this.ErrorHandler(ex);
			}
			
		}

		/// <summary>
		/// Handles the TextChanged event of the DensityLowPlusDeadbandTextbox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
		protected void DensityLowPlusDeadbandTextboxTextChanged(object sender, EventArgs e)
		{
			string oldVal = this.Product.DensityLowPlusDeadband;

			try
			{
				this.UpdateData();
				if (this.DensityLowPlusDeadbandTextbox.Text != this.Product.DensityLowPlusDeadband)
				{
					var d = new SIDouble()
						        {
							        Units = this.Product._DensityLowLimit.Units,
							        Value =
								        Double.Parse(this.DensityLowPlusDeadbandTextbox.Text, this.Product._DensityLowLimit.Format)
								        - this.Product._DensityLowLimit.Value
						        };
					this.Product.DensityDeadband = System.Convert.ToString(d.SIValue);
				}

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.DensityLowPlusDeadbandTextbox.Text = oldVal;
				this.ErrorHandler(ex);
			}
		}

		private void SetFieldAccessibilityForChildRecordVersion()
		{
			 bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
			 if ((this.Product.IdentityGuid.Equals(Guid.Empty)
					|| (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
					|| (this.VersionSpecificFields == null)))
			 {
				  return;
			 }

			 this.DensityHighLimitTextbox.Enabled = (this.DensityHighLimitTextbox.Enabled && this.VersionSpecificFields.Contains("DensityHighLimit"));
			 this.DensityLowLimitTextbox.Enabled = (this.DensityLowLimitTextbox.Enabled && this.VersionSpecificFields.Contains("DensityLowLimit"));
			 this.DensityHighMinusDeadbandTextbox.Enabled = (this.DensityHighMinusDeadbandTextbox.Enabled && this.VersionSpecificFields.Contains("DensityDeadband"));
			 this.DensityLowPlusDeadbandTextbox.Enabled = (this.DensityLowPlusDeadbandTextbox.Enabled && this.VersionSpecificFields.Contains("DensityDeadband"));
			 this.ApplyDensityLimitsCheckBox.Enabled = (this.ApplyDensityLimitsCheckBox.Enabled && this.VersionSpecificFields.Contains("ApplyDensityLimits"));
			 this.TemperatureHiHiLimitTextbox.Enabled = (this.TemperatureHiHiLimitTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureHiHiLimit"));
			 this.TemperatureHighLimitTextbox.Enabled = (this.TemperatureHighLimitTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureHighLimit"));
			 this.TemperatureLowLimitTextbox.Enabled = (this.TemperatureLowLimitTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureLowLimit"));
			 this.TemperatureLoLoLimitTextbox.Enabled = (this.TemperatureLoLoLimitTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureLoLoLimit"));
			 this.TemperatureDeadbandTextbox.Enabled = (this.TemperatureDeadbandTextbox.Enabled && this.VersionSpecificFields.Contains("TemperatureDeadband"));
			 this.ApplyTemperatureLimitsCheckBox.Enabled = (this.ApplyTemperatureLimitsCheckBox.Enabled && this.VersionSpecificFields.Contains("ApplyTemperatureLimits"));
		}
	}
}
