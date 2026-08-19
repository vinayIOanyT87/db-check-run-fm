// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryPrinterFriendlyResults.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.QueryWriterWebApp
{
	using System;
    using System.Data;
    using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.UtilityObjects;
    using FMCore;

	using FuelsManager.FMWebApp;


	/// <summary>
	/// The query printer friendly results.
	/// </summary>
	public partial class QueryPrinterFriendlyResults : FMFormBaseAjax
	{
		#region Constants and Fields
		/// <summary>
		/// The query.
		/// </summary>
		private QueryClass query;

		/// <summary>
		/// The query defaults.
		/// </summary>
		private QueryDefaultClass queryDefaults;

		/// <summary>
		/// The site format info.
		/// </summary>
		private DateTimeFormatInfo siteFormatInfo;

		/// <summary>
		/// The query results.
		/// </summary>
		private DataView queryResults;
		#endregion

		#region Methods
		/// <summary>
		/// The generate body.
		/// </summary>
		protected void GenerateBody()
		{
			this.SetGridTemplate();
		}

		/// <summary>
		/// The page_ init.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Init(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

				SiteClass currentSite = FMChannelHelper.MakeCall<ISites, SiteClass>(
																	 sites => sites.Get(this.Security, this.Security.LoginSiteGuid, false, false, false));

				this.siteFormatInfo = currentSite.GetDateTimeFormatInfo();

				// Get the query defaults
				this.queryDefaults = FMChannelHelper.MakeCall<IQueryDefaults, QueryDefaultClass>(defaults => defaults.Enumerate(this.Security));

				// Get the query object out of session
				this.query = (QueryClass)this.Session[QueryDefinitionForm.QuerywriterQueryObject];

				this.UpdateView();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The page load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var table = this.Session[QueryResultsForm.QueryResultsAdditionalInfo] as DataTable;
				if (table != null)
				{
					this.AdditionalInformation.InnerHtml = QueryResultsForm.FormatAdditionalInformation(table);
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// The process functions.
		/// </summary>
		/// <param name="originalText">
		/// The original text.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		protected string ProcessFunctions(string originalText)
		{
			DateTimeOffset now = DateTimeOffset.Now;
			if (originalText.Contains("@TIME"))
			{
				originalText = originalText.Replace("@TIME", now.ToString("t", this.siteFormatInfo));
			}

			if (originalText.Contains("@SHORTDATE"))
			{
				originalText = originalText.Replace("@SHORTDATE", now.ToString("d", this.siteFormatInfo));
			}

			if (originalText.Contains("@LONGDATE"))
			{
				originalText = originalText.Replace("@LONGDATE", now.ToString("D", this.siteFormatInfo));
			}

			return originalText;
		}

		/// <summary>
		/// The results grid pre-render.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void ResultsGridPreRender(object sender, EventArgs e)
		{
			this.queryResults = new DataView();

			// Get the results table
			var table = this.Session[QueryResultsForm.QueryResultsDataTable] as DataTable;

			if (table != null)
			{
				this.queryResults.Table = table;	 
			}

			// You only need the following 2 lines of code if you are not 
			// using an ObjectDataSource of SqlDataSource
			this.ResultsGrid.DataSource = this.queryResults;
			this.ResultsGrid.DataBind();

			if (this.ResultsGrid.Rows.Count > 0)
			{
				// This replaces <td> with <th> and adds the scope attribute
				this.ResultsGrid.UseAccessibleHeader = true;

				// This will add the <thead> and <tbody> elements
				this.ResultsGrid.HeaderRow.TableSection = TableRowSection.TableHeader;

				// This adds the <tfoot> element. 
				// Remove if you don't have a footer row
				this.ResultsGrid.FooterRow.TableSection = TableRowSection.TableFooter;
			}
		}

		/// <summary>
		/// The set global footer.
		/// </summary>
		protected void SetGlobalFooter()
		{
			if (string.IsNullOrEmpty(this.queryDefaults.Footer) == false)
			{
				this.GlobalFooter.Text = this.ProcessFunctions(this.queryDefaults.Footer);
			}
			else
			{
				this.PrinterFriendResults.Rows.Remove(this.GlobalFooterRow);
			}
		}


        /// <summary>
        /// The set CUI markers visability.
        /// </summary>
        protected void SetCuiVisability()
        {
				bool displayCUIDataMark = Global.IsFdsIM || AppSettingsHelper.GetKeyValue<bool>("DisplayCUIDataMark", false); 

            HeaderCUIRow.Visible = displayCUIDataMark;
            FooterCUI.Visible = displayCUIDataMark;
        }

        /// <summary>
        /// The set global header.
        /// </summary>
        protected void SetGlobalHeader()
		{
			if (String.IsNullOrEmpty(this.queryDefaults.Header) == false)
			{
				this.GlobalHeader.Text = this.ProcessFunctions(this.queryDefaults.Header);
			}
			else
			{
				this.PrinterFriendResults.Rows.Remove(this.GlobalHeaderRow);
			}
		}

		/// <summary>
		/// The set grid template.
		/// </summary>
		protected void SetGridTemplate()
		{
			if (this.query.IncludeLineNumbers)
			{
				// Show the Line Number column, which should be the first one.
				this.ResultsGrid.Columns[0].Visible = true;
			}

			// Do the group fields
			foreach (QueryWriterField field in this.query.DataGroups)
			{
				this.ResultsGrid.Columns.Add(QueryResultsForm.CreateNewField(field, this.siteFormatInfo));
			}

			// Loop through the Query fields and add the columns
			foreach (QueryWriterField field in this.query.Fields)
			{
				this.ResultsGrid.Columns.Add(QueryResultsForm.CreateNewField(field, this.siteFormatInfo));
			}

			// Check the grouping level
			if (this.query.HasGroups)
			{
				this.ResultsGrid.GroupingDepth = this.query.DataGroups.Count;
			}
		}

		/// <summary>
		/// The set local footer.
		/// </summary>
		protected void SetLocalFooter()
		{
			if (this.query.Footer.DefaultIfNull("").NotEquals(""))
			{
				this.LocalFooter.Text = this.ProcessFunctions(this.query.Footer);
			}
			else
			{
				this.PrinterFriendResults.Rows.Remove(this.LocalFooterRow);
			}
		}

		/// <summary>
		/// The set local header.
		/// </summary>
		protected void SetLocalHeader()
		{
			if (this.query.Header.DefaultIfNull("").NotEquals(""))
			{
				this.LocalHeader.Text = this.ProcessFunctions(this.query.Header);
			}
			else
			{
				this.PrinterFriendResults.Rows.Remove(this.LocalHeaderRow);
			}
		}

		/// <summary>
		/// The set title.
		/// </summary>
		protected void SetTitle()
		{
			if (this.query.Title.DefaultIfNull("").NotEquals(""))
			{
				this.QueryTitle.Text = this.query.Title;
				this.Page.Title = this.query.Title;
			}
			else
			{
				this.PrinterFriendResults.Rows.Remove(this.QueryTitleRow);
			}
		}

		/// <summary>
		/// The update view.
		/// </summary>
		protected void UpdateView()
		{
			// Set the global header if necessary
			this.SetGlobalHeader();

			// Set the Query local header
			this.SetLocalHeader();

			// Set the title
			this.SetTitle();

			// Show the results body
			this.GenerateBody();

			// Set the Query local footer
			this.SetLocalFooter();

			// Set the Query global footer
			this.SetGlobalFooter();

			this.SetCuiVisability();

        }

		#endregion
	}
}