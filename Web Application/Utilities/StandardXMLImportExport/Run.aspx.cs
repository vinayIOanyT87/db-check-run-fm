using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using FM7Accounting;
using XMLImport;
using ConsolidatedDataObjects;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for Run.
	/// </summary>
	public class Run : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.Label FromDateLabel;
		protected System.Web.UI.WebControls.Label ToDateLabel;
		protected System.Web.UI.WebControls.Button ImportButton;
		protected System.Web.UI.HtmlControls.HtmlInputFile FileSelector;
		protected System.Web.UI.WebControls.Label FileLabel;

		protected string name;
		protected System.Web.UI.WebControls.Calendar FromCalendar;
		protected System.Web.UI.WebControls.Calendar ToCalendar;
		protected System.Web.UI.WebControls.TextBox FromTextBox;
		protected System.Web.UI.WebControls.TextBox ToTextBox;
		protected System.Web.UI.WebControls.Button FromButton;
		protected System.Web.UI.WebControls.Button ToButton;
		protected System.Web.UI.WebControls.CheckBox IgnoreDatesCheckBox;
		protected System.Web.UI.WebControls.Image FadeImage;
		protected string site;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			name = Request.Params["Name"];
			site = Request.Params["Site"];

			if(! IsPostBack)
			{
				this.FromCalendar.SelectedDate = System.DateTime.Now;
				this.ToCalendar.SelectedDate = System.DateTime.Now;

				this.FromTextBox.Text = this.FromCalendar.SelectedDate.ToShortDateString();
				this.ToTextBox.Text = this.ToCalendar.SelectedDate.ToShortDateString();

				ApplyDataDictionary();
			}
		}

		private void ApplyDataDictionary()
		{
			// Determine if the data dictionary shall be used.  True indicates that it
			// shall be used and false otherwise.
			bool useDataDictionary=false;
			if(Page.Session["UseDataDictionary"] == null
			|| (bool) Page.Session["UseDataDictionary"])
				useDataDictionary=true;

			DataDictionary dd = new DataDictionary(new AccountingSecurity().GetSecurity(Session["Token"] as string),useDataDictionary);

			this.IgnoreDatesCheckBox.Text = dd.getNameFromGlobalDictionary(this.IgnoreDatesCheckBox.Text);
			this.FromDateLabel.Text = dd.getNameFromGlobalDictionary(this.FromDateLabel.Text.Trim());
			this.ToDateLabel.Text = dd.getNameFromGlobalDictionary(this.ToDateLabel.Text);
			this.FromButton.Text = dd.getNameFromGlobalDictionary(this.FromButton.Text);
			this.ToButton.Text = dd.getNameFromGlobalDictionary(this.ToButton.Text);
			this.FileLabel.Text = dd.getNameFromGlobalDictionary(this.FileLabel.Text.Trim());
			this.ImportButton.Text = dd.getNameFromGlobalDictionary(this.ImportButton.Text);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.FromCalendar.SelectionChanged += new System.EventHandler(this.FromCalendar_SelectionChanged);
			this.ToCalendar.SelectionChanged += new System.EventHandler(this.ToCalendar_SelectionChanged);
			this.ImportButton.Click += new System.EventHandler(this.ImportButton_Click);
			this.FromTextBox.TextChanged += new System.EventHandler(this.FromTextBox_TextChanged);
			this.ToTextBox.TextChanged += new System.EventHandler(this.ToTextBox_TextChanged);
			this.FromButton.Click += new System.EventHandler(this.FromButton_Click);
			this.ToButton.Click += new System.EventHandler(this.ToButton_Click);
			this.IgnoreDatesCheckBox.CheckedChanged += new System.EventHandler(this.IgnoreDatesCheckBox_CheckedChanged);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void ImportButton_Click(object sender, System.EventArgs e)
		{
			if( (this.FileSelector.Value == null) || (this.FileSelector.Value == "") )
			{
				string alertScript = "<script language='JavaScript'>alert('Please select a file to upload.');</script>";
				Response.Write(alertScript);
				return;
			}

			AccountingSecurity accountingSecurity = new AccountingSecurity();
			accountingSecurity.GetSecurity(Session["Token"] as string);

			SetupProcessor setupProcessor = new SetupProcessor();
			ImportFilter filter = setupProcessor.GetConfiguration(name);

			if(this.IgnoreDatesCheckBox.Checked == false)
			{
				filter.FromDate = new Date();
				filter.FromDate.Value = this.FromCalendar.SelectedDate;
				filter.ToDate = new Date();
				filter.ToDate.Value = this.ToCalendar.SelectedDate;
			}

			Session.Add("ImportFilter", filter);
			Session.Add("ImportFileStream", this.FileSelector.PostedFile.InputStream);
			Response.Redirect("ImportResults.aspx");
		}

		private void IgnoreDatesCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.FromTextBox.Visible = true;
			this.FromButton.Visible = true;

			this.ToTextBox.Visible = true;
			this.ToButton.Visible = true;

			this.FromCalendar.Visible = false;
			this.ToCalendar.Visible = false;

			if(this.IgnoreDatesCheckBox.Checked == true)
			{
				this.FromTextBox.Text = "";
				this.ToTextBox.Text = "";

				this.FromDateLabel.Enabled = false;
				this.FromTextBox.Enabled = false;
				this.FromButton.Enabled = false;

				this.ToDateLabel.Enabled = false;
				this.ToTextBox.Enabled = false;
				this.ToButton.Enabled = false;
			}
			else
			{
				this.FromTextBox.Text = this.FromCalendar.SelectedDate.ToShortDateString();
				this.ToTextBox.Text = this.ToCalendar.SelectedDate.ToShortDateString();

				this.FromDateLabel.Enabled = true;
				this.FromTextBox.Enabled = true;
				this.FromButton.Enabled = true;

				this.ToDateLabel.Enabled = true;
				this.ToTextBox.Enabled = true;
				this.ToButton.Enabled = true;
			}
		}

		private void FromButton_Click(object sender, System.EventArgs e)
		{
			this.FromCalendar.Visible = true;
			this.FromTextBox.Visible = false;
			this.FromButton.Visible = false;
		}

		private void ToButton_Click(object sender, System.EventArgs e)
		{
			this.ToCalendar.Visible = true;
			this.ToTextBox.Visible = false;
			this.ToButton.Visible = false;
		}

		private void FromCalendar_SelectionChanged(object sender, System.EventArgs e)
		{
			this.FromCalendar.Visible = false;
			this.FromTextBox.Visible = true;
			this.FromButton.Visible = true;
			this.FromTextBox.Text = this.FromCalendar.SelectedDate.ToShortDateString();
		}

		private void ToCalendar_SelectionChanged(object sender, System.EventArgs e)
		{
			this.ToCalendar.Visible = false;
			this.ToTextBox.Visible = true;
			this.ToButton.Visible = true;
			this.ToTextBox.Text = this.ToCalendar.SelectedDate.ToShortDateString();
		}

		private void FromTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				FromCalendar.SelectedDate = System.DateTime.Parse(FromTextBox.Text);
			}
			catch(Exception)
			{
				FromTextBox.Text = FromCalendar.SelectedDate.ToShortDateString();
			}
		}

		private void ToTextBox_TextChanged(object sender, System.EventArgs e)
		{
			try
			{
				ToCalendar.SelectedDate = System.DateTime.Parse(ToTextBox.Text);
			}
			catch(Exception)
			{
				ToTextBox.Text = ToCalendar.SelectedDate.ToShortDateString();
			}
		}
	}
}
