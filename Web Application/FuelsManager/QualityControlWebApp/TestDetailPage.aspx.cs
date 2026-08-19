// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDetailPage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FuelsManager.QualityControlWebApp
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using FMCore;
	using FMWebApp;

	public partial class TestDetailPage : FMAutoSubmitFormBase
	{
		#region Methods

		protected void AppendCommand(object sender, EventArgs e)
		{
			try
			{
				if (this.RuleTypeRadioButtonList.SelectedValue == "Range")
				{
					if (string.IsNullOrEmpty(this.RangeFromTextBox.Text) || string.IsNullOrEmpty(this.RangeToTextbox.Text))
					{
						this.ErrorHandler(
							new Exception("You must enter a Range From and Range To value representing the range you wish to append"));
						return;
					}

					double rangeFromDouble;
					double rangeToDouble;

					if (double.TryParse(this.RangeFromTextBox.Text, out rangeFromDouble)
					    && double.TryParse(this.RangeToTextbox.Text, out rangeToDouble))
					{
						if (this.CurrentValueTextbox.Text.Trim().Length > 0)
						{
							this.CurrentValueTextbox.Text += ", ";
						}

						this.CurrentValueTextbox.Text += rangeFromDouble.ToString(CultureInfo.InvariantCulture) + ".."
						                                 + rangeToDouble.ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this.ErrorHandler(new ApplicationException("Range must be numeric."));
					}
				}
				else
				{
					var objAlphaNumeric = new Regex("[^a-zA-Z0-9]");
					string[] adjustRules = this.AdjustRuleTextbox.Text.Trim().Split(new[] { ',' });
					foreach (string adjustRule in adjustRules)
					{
						string trimmedAdjustRule = adjustRule.Trim();

						if (trimmedAdjustRule.Length < 1)
						{
							this.ErrorHandler(new Exception("You must enter the value that you wish to append"));
							break;
						}

						if (objAlphaNumeric.IsMatch(trimmedAdjustRule))
						{
							this.ErrorHandler(new Exception("Value must be alphanumeric."));
							break;
						}

						if (this.CurrentValueTextbox.Text.Trim().Length > 0)
						{
							this.CurrentValueTextbox.Text += ", ";
						}

						this.CurrentValueTextbox.Text += adjustRule.Trim();
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void CancelCommand(object sender, EventArgs e)
		{
			try
			{
				this.Redirect("TestsAndInspectionsForm.aspx");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void ClearAllCommand(object sender, EventArgs e)
		{
			try
			{
				this.CurrentValueTextbox.Text = string.Empty;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void NewCommand(object sender, EventArgs e)
		{
			try
			{
				if (this.CommitData())
				{
					this.Redirect("TestDetailPage.aspx");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Handles the Command event of the OK control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void OkCommand(object sender, EventArgs e)
		{
			try
			{
				if (this.CommitData())
				{
					this.Redirect("TestsAndInspectionsForm.aspx");
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// Raises the <see cref="OnInit" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected override void OnInit(EventArgs e)
		{
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			this.InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Handles the Load event of the Page control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		/// <exception cref="System.ApplicationException">Access denied.</exception>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS) && !this.Security.HasRight(RIGHT.VIEW_TEST_ITEMS))
				{
					throw new ApplicationException("Access denied.");
				}

				if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS))
				{
					this.New.Enabled = false;
					this.OK.Enabled = false;
				}

				if (this.Page.IsPostBack == false)
				{
					TestClass test;

					if (string.IsNullOrEmpty(this.Request.GetQueryOrFormValue("IdentityGuid")))
					{
						test = new TestClass();
					}
					else
					{
						Guid testGuid = Guid.Parse(this.Request.GetQueryOrFormValue("IdentityGuid"));
						test = FMChannelHelper.MakeCall<ITests, TestClass>(tests => tests.Get(this.Security, testGuid));

						// Make the Test Name read only if the Test is associated with any Test Sets.
						TestToTestSetMapCollectionClass testSetMap =
							FMChannelHelper.MakeCall<ITestToTestSetMaps, TestToTestSetMapCollectionClass>(
								testSetMaps => testSetMaps.EnumerateByTestGuid(this.Security, testGuid));

						this.TestNameTextbox.ReadOnly = testSetMap.Count != 0;

						if (!this.Security.HasRight(RIGHT.MODIFY_TEST_ITEMS)
						    || (this.Security.SiteGuid != test.SiteGuid && test.SiteGuid != Guid.Empty))
						{
							this.OK.Enabled = false;
							this.New.Enabled = false;
						}
					}

					this.ShowDLAEnergyControls();

					this.Session["Test"] = test;

					this.UpdateView();

					// Set the title label with a key field from the bound object appended
					if (test != null)
					{
						this.TestTitleLabel.Text = this.GetTitleLabelText(this.TestTitleLabel.Text, test.ID);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				this.Response.End();
			}
		}

		/// <summary>
		/// Handles the Command event of the Remove control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected void RemoveCommand(object sender, EventArgs e)
		{
			try
			{
				if (this.RuleTypeRadioButtonList.SelectedValue == "Range")
				{
					string str = ", " + this.CurrentValueTextbox.Text + ",";

					string[] vals = str.Split(new[] { ',' });

					if (string.IsNullOrEmpty(this.RangeFromTextBox.Text) || string.IsNullOrEmpty(this.RangeToTextbox.Text))
					{
						this.ErrorHandler(
							new Exception("You must enter a Range From and Range To value representing the range you wish to remove"));
						return;
					}

					double rangeFromDouble;
					double rangeToDouble;

					if (double.TryParse(this.RangeFromTextBox.Text, out rangeFromDouble)
					    && double.TryParse(this.RangeToTextbox.Text, out rangeToDouble))
					{
						for (int i = vals.Length - 1; i >= 0; i--)
						{
							string val = vals[i];
							var stringSeparators = new[] { ".." };
							string[] d = val.Split(stringSeparators, StringSplitOptions.None);
							if (d.Length == 2)
							{
								if (Convert.ToDouble(d[0]) == rangeFromDouble && Convert.ToDouble(d[1]) == rangeToDouble)
								{
									int inx = str.LastIndexOf(val, StringComparison.Ordinal);
									str = str.Substring(0, inx) + str.Substring(inx + val.Length);
									this.CurrentValueTextbox.Text = str.Replace(",,", ",");
									break;
								}
							}
						}
					}
					else
					{
						this.ErrorHandler(new Exception("Range must be numeric."));
					}
				}
				else
				{
					var objAlphaNumeric = new Regex("[^a-zA-Z0-9]");
					string[] adjustRules = this.AdjustRuleTextbox.Text.Trim().Split(new[] { ',' });
					foreach (string adjustRule in adjustRules)
					{
						string trimmedAdjustRule = adjustRule.Trim();

						if (trimmedAdjustRule.Length < 1)
						{
							this.ErrorHandler(new Exception("You must enter the value that you wish to remove"));
							break;
						}
						
						if (objAlphaNumeric.IsMatch(trimmedAdjustRule))
						{
							this.ErrorHandler(new Exception("Value must be alphanumeric."));
							break;
						}
						
						string str = adjustRule.Trim();
						int inx = this.CurrentValueTextbox.Text.LastIndexOf(str, StringComparison.Ordinal);
						if (inx >= 0)
						{
							int len = str.Length;
							if (inx == 0)
							{
								this.CurrentValueTextbox.Text = this.CurrentValueTextbox.Text.Substring(len);
							}
							else
							{
								this.CurrentValueTextbox.Text = this.CurrentValueTextbox.Text.Substring(0, inx - 1)
								                                + this.CurrentValueTextbox.Text.Substring(inx + len);
							}

							this.CurrentValueTextbox.Text = this.CurrentValueTextbox.Text.Replace(",,", ",");
						}
					}
				}

				if (this.CurrentValueTextbox.Text.Trim().StartsWith(","))
				{
					this.CurrentValueTextbox.Text = this.CurrentValueTextbox.Text.Trim().Substring(1).Trim();
				}

				if (this.CurrentValueTextbox.Text.Trim().EndsWith(","))
				{
					int inx = this.CurrentValueTextbox.Text.Trim().LastIndexOf(",", StringComparison.Ordinal);
					this.CurrentValueTextbox.Text = this.CurrentValueTextbox.Text.Substring(0, inx).Trim();
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void RuleTypeRadioButtonListSelectedIndexChanged(object sender, EventArgs e)
		{
			this.AdjustRuleTextbox.Enabled = this.RuleTypeRadioButtonList.SelectedValue == "Adjust";
			this.RangeFromTextBox.Enabled = this.RuleTypeRadioButtonList.SelectedValue == "Range";
			this.RangeToTextbox.Enabled = this.RuleTypeRadioButtonList.SelectedValue == "Range";
		}

		private bool CommitData()
		{
			try
			{
                SiteClass site = FMChannelHelper.MakeCall<ISites, SiteClass>(sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

				if (this.TestNameTextbox.Text == string.Empty)
				{
					var except = new ApplicationException("Test Name is a required field.");
					this.ErrorHandler(except);
					return false;
				}

				if (this.CurrentValueTextbox.Text == string.Empty)
				{
					var except = new ApplicationException("Current Value is a required field.");
					this.ErrorHandler(except);
					return false;
				}

				var test = (TestClass)this.Session["Test"];
				test.ID = this.TestNameTextbox.Text;
				test.MeasurementUnit = this.MeasurementUnitTextbox.Text;
				test.ValidationRule = this.CurrentValueTextbox.Text;
				test.TestCode = this.TestCodeTextbox.Text;
				test.TestMethod = this.TestMethodTextbox.Text;
				test.ProductID = this.ProductDropDownList.Text;

				double tmpSampleSize;

				if (!Double.TryParse(this.SampleSizeTextbox.Text, NumberStyles.Any, site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT), out tmpSampleSize))
				{
					throw new Exception("Sample Size must be numeric.");
				}

				test.SampleSize = (float)tmpSampleSize;

				if (test.IdentityGuid == Guid.Empty)
				{
					test.SiteGuid = this.Security.SiteGuid;
					FMChannelHelper.MakeCall<ITests>(tests => tests.Add(this.Security, test));
				}
				else
				{
					FMChannelHelper.MakeCall<ITests>(tests => tests.Modify(this.Security, test));
				}

				return true;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
				return false;
			}
		}

		/// <summary>
		///     Required method for Designer support - do not modify
		///     the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void UpdateView()
		{
			SiteClass site =
				FMChannelHelper.MakeCall<ISites, SiteClass>(
					sites => sites.Get(this.Security, this.Security.SiteGuid, false, false, false));

			var test = this.Session["Test"] as TestClass;
			if (test != null)
			{
				this.TestNameTextbox.Text = test.ID;
				this.MeasurementUnitTextbox.Text = test.MeasurementUnit;
				this.CurrentValueTextbox.Text = test.ValidationRule;
				this.SampleSizeTextbox.Text = test.SampleSize.ToString(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));
				this.TestCodeTextbox.Text = test.TestCode;
				this.TestMethodTextbox.Text = test.TestMethod;
			}

			this.ProductDropDownList.Items.Clear();

			var productList = FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(x => x.Enumerate(this.Security));
			foreach (ProductClass product in productList)
			{
				ProductDropDownList.Items.Add(product.ID);
			}
		}

		private void ShowDLAEnergyControls()
		{
			bool showControl = FMChannelHelper.MakeCall<IHardwareKey, bool>(x=>x.IsDescEnterpriseKey()) && Security.HasRight(RIGHT.CONFIGURE_DLA_TEST);
			this.TestCodeLabel.Visible = showControl;
			this.TestCodeTextbox.Visible = showControl;
			this.TestMethodLabel.Visible = showControl;
			this.TestMethodTextbox.Visible = showControl;
			this.ProductLabel.Visible = showControl;
			this.ProductDropDownList.Visible = showControl;
		}


		#endregion
	}
}