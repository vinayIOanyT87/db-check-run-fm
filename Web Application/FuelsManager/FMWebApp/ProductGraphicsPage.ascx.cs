namespace FuelsManager.FMWebApp
{
	using System;

	using global::FMWebApp;

	public partial class ProductGraphicsPage : ProductPageBase
	{
		#region Public Methods and Operators
		/// <summary>
		/// This method is called by the main form to update the data from the page to
		/// the product object.
		/// </summary>
		public void UpdateData()
		{
			string fillColor = this.FillColorHexValue.Value;
			string patternColor = this.PatternColorHexValue.Value;
			int patternNumber = 1;

			if (string.IsNullOrEmpty(this.FillColorHexValue.Value))
			{
				// This default color a shade of blue is what the default color
				// is in draw.
				fillColor = "#99ccff";
			}

			if (string.IsNullOrEmpty(this.PatternColorHexValue.Value))
			{
				// The default pattern color is white.
				patternColor = "#ffffff";
			}

			if (string.IsNullOrEmpty(this.SelectedPatternNumber.Value) == false)
			{
				if (int.TryParse(this.SelectedPatternNumber.Value, out patternNumber) == false)
				{
					// Pattern number 1 is the default, which is no pattern.
					patternNumber = 1;
				}
			}

			this.Product.ProductColor = fillColor;
			this.Product.PatternColor = patternColor;
			this.Product.PatternNumber = patternNumber;
		}
		#endregion

		#region Event methods
		/// <summary>
		/// This is the main entry point into this page.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				this.FillColorHexValue.Value = "#99ccff";
				this.PatternColorHexValue.Value = "#ffffff";
				this.SelectedPatternNumber.Value = this.Product.PatternNumber.ToString();

                if (string.IsNullOrEmpty(this.Product.ProductColor) == false)
				{
					this.FillColorHexValue.Value = this.Product.ProductColor;
				}

				if (string.IsNullOrEmpty(this.Product.PatternColor) == false)
				{
					this.PatternColorHexValue.Value = this.Product.PatternColor;
				}
                this.SetFieldAccessibilityForChildRecordVersion();
            }
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		#endregion

		#region Private methods
		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			//this.LevelUnitsDropDownList.SelectedIndexChanged +=
			//	new System.EventHandler(this.LevelUnitsDropDownList_SelectedIndexChanged);
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            this.IsFillColorHexValueEnabled.Value = Convert.ToString(true);
            this.IsPatternColorHexValueEnabled.Value = Convert.ToString(true);
            this.IsSelectedPatternNumberEnabled.Value = Convert.ToString(true);

            bool currentSiteOwnsRecordVersion = (this.Product.SiteGuid == this.Security.SiteGuid);
            if ((this.Product.IdentityGuid.Equals(Guid.Empty)
                 || (currentSiteOwnsRecordVersion && this.Product.IdentityGuid.Equals(this.Product.MasterRecordGuid))
                 || (this.VersionSpecificFields == null)))
            {
                return;
            }

            this.IsFillColorHexValueEnabled.Value = (this.VersionSpecificFields.Contains("ProductColor")).ToString();
            this.IsPatternColorHexValueEnabled.Value = (this.VersionSpecificFields.Contains("PatternColor")).ToString();
            this.IsSelectedPatternNumberEnabled.Value = (this.VersionSpecificFields.Contains("PatternNumber")).ToString();

        }
        #endregion
    }
}