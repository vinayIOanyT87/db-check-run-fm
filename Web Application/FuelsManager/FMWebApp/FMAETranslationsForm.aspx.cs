// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMAETranslationsForm.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
// The FMAE Translations Form allows a user to define translations for entities that are applied during the import of 
// legacy Aviation transactions to FuelsManager through the FMAE interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	/// The FMAE Translations Form allows a user to define translations for entities that are applied during the import of 
	/// legacy Aviation transactions to FuelsManager through the FMAE interface.
	/// 
	/// When entities are imported, the FMAE interface will examine the ID and see if there is a translation defined for that ID.
	/// If there is, the transaction record will be created with the entity specified for the translation.
	/// 
	/// This form is meant to only be visible at a site group level. The translations defined apply to the system as a whole and not any one
	/// site in particular.
	/// </summary>
	public partial class FMAETranslationsForm : FMFormBase, IMenuDiscovery
	{
		#region Form Properties

        /// <summary>
        /// When translations are imported or exported, these are the column names that should be in the csv file
        /// </summary>
        private readonly List<string> importExportColumnNames = new List<string> { "Entity Type", "FMAE ID", "Enterprise Entity ID" };

		/// <summary>
		/// The FMAE translations displayed on the page
		/// </summary>
		private List<FMAETranslation> SessionTranslations
		{
			get
			{
				if (this.Session["FMAETranslations"] is List<FMAETranslation>)
				{
					return this.Session["FMAETranslations"] as List<FMAETranslation>;
				}
				else
				{
					return new List<FMAETranslation>();
				}
			}
			set
			{
				this.Session.Add("FMAETranslations", value);
			}
		}

        /// <summary>
        /// Stores the text the user searched on when the Find button is pressed
        /// </summary>
        private string SessionFindTextBoxSearchString
        {
            get
            {
                if (this.Session["FMAEFindSearchString"] is string)
                {
                    return this.Session["FMAEFindSearchString"] as string;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                this.Session.Add("FMAEFindSearchString", value);
            }
        }

        #endregion

		/// <summary>
		/// When the page loads, get the security information and display the translations currently defined
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Not used</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();

                // Hide the import results controls if they are visible.
                // The import results should only display once for a user and shouldn't linger
                // after the import is complete
			    this.HideImportResultsControls();

			    if (!this.IsPostBack)
				{
                    this.SessionFindTextBoxSearchString = string.Empty;
					this.EntityTypeDropDownList.DataBind();
					this.UpdateView();
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		#region Form Grid Events

		/// <summary>
		/// When the user clicks the cancel edit button, either remove the translation from the list if it's a new one,
		/// or cancel the edits on the current translation
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Identifies the row the edit was cancelled for</param>
		protected void TranslationsGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			try
			{
				this.EnableControls(true);

				this.TranslationsGrid.EditIndex = -1;

                List<FMAETranslation> translations = this.SessionTranslations;

			    // Get the identity guid of the object associated with the row
                DataKey dataKey = this.TranslationsGrid.DataKeys[e.RowIndex];

                if (dataKey != null && dataKey.Value is Guid && (Guid)dataKey.Value == Guid.Empty)
                {
                    // If the translation is a new one, cancel should remove it from the list rather than just cancelling the edit
                    // Keep in mind that new translations are added to the end of the list
                    translations.RemoveAt(translations.Count - 1);                 
                }

                this.BindData(translations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user edits a row, disable the add button and edit the row
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Identifies the row being edited</param>
		protected void TranslationsGrid_RowEditing(object sender, GridViewEditEventArgs e)
		{
			try
			{
				this.EnableControls(false);
				this.TranslationsGrid.EditIndex = e.NewEditIndex;
				this.BindData(this.SessionTranslations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When a row is bound to the grid wire up the delete button and select the appropriate value in the entity drop down
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Identifies the row being bound</param>
		protected void TranslationsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			try
			{
				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					FMDeleteLinkButton deleteButton = e.Row.FindControl("DeleteButton") as FMDeleteLinkButton;
                    
					if (deleteButton != null)
					{
						deleteButton.CommandArgument = e.Row.RowIndex.ToString(CultureInfo.InvariantCulture);
					}

					if (e.Row.DataItem is FMAETranslation)
					{
						FMAETranslation translation = e.Row.DataItem as FMAETranslation;

						FMDropDownList entityDropDown = e.Row.FindControl("EnterpriseEntityDropDownList") as FMDropDownList;

						if (entityDropDown != null)
						{
							if (translation.EntityGuid != Guid.Empty)
							{
								entityDropDown.SelectedValue = translation.EntityGuid.ToString();
							}
						}

					}
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// Fires when the user presses the delete button and deletes a translation from the grid
		/// </summary>
		/// <param name="sender">not used</param>
		/// <param name="e">Identifies the row the user pressed delete on</param>
		protected void TranslationsGrid_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			try
			{
				if (e.CommandName.Equals("Delete", StringComparison.OrdinalIgnoreCase))
				{
					int rowIndex = Convert.ToInt32(e.CommandArgument);

                    // Get the identity guid of the record that the user pressed delete for
				    Guid identityGuidToDelete = Guid.Empty;
				    if (this.TranslationsGrid.DataKeys[rowIndex] != null && this.TranslationsGrid.DataKeys[rowIndex].Value is Guid)
				    {
				        identityGuidToDelete = (Guid)this.TranslationsGrid.DataKeys[rowIndex].Value;
				    }

				    List<FMAETranslation> translations = this.SessionTranslations;

                    // If the identity guid is set, delete the record from the database
                    if (identityGuidToDelete != Guid.Empty)
					{
                        FMAETranslation translationToDelete = translations.FirstOrDefault(translation => identityGuidToDelete == translation.IdentityGuid);
					    
                        if (translationToDelete != null)
					    {
					        FMChannelHelper.MakeCall<IFMAETranslations>(translationsClient => translationsClient.Purge(this.Security, translationToDelete));
					    }
					}

                    // Remove the record from the collection. We want to do this even with an empty identity guid
                    // since that indicates a new record that wasn't saved. 
                    translations.RemoveAll(translation => identityGuidToDelete == translation.IdentityGuid);

					this.EnableControls(true);
					this.TranslationsGrid.EditIndex = -1;

					this.SessionTranslations = translations;

					this.BindData(translations);
				}
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user saves edits on a row, use the information provided to update a translation record
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Identifies the row being saved</param>
		protected void TranslationsGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			try
			{			   
                // Get the identity guid of the record contained in the row updated by the user
                Guid identityGuidUpdated = Guid.Empty;
				if (this.TranslationsGrid.DataKeys[e.RowIndex] != null && this.TranslationsGrid.DataKeys[e.RowIndex].Value is Guid)
				{
				    identityGuidUpdated = (Guid)this.TranslationsGrid.DataKeys[e.RowIndex].Value;
				}

                List<FMAETranslation> translations = this.SessionTranslations;

				// Get the object we have associated with the row
                FMAETranslation translation = translations.FirstOrDefault(matchingTranslation => matchingTranslation.IdentityGuid == identityGuidUpdated);

				if (translation != null)
				{
                    // Get the row
                    GridViewRow row = this.TranslationsGrid.Rows[e.RowIndex];

					// Save the data the user entered
					string fmaeID = ((FMTextBox)row.Cells[1].Controls[1]).Text;//bds

					if (string.IsNullOrEmpty(fmaeID))
					{
						throw new ApplicationException("FMAE ID is required");
					}

					translation.ID = fmaeID;

					Guid entityGuid = Guid.Empty;
					string entityID = string.Empty;

					FMDropDownList entityDropDown = row.Cells[2].Controls[1] as FMDropDownList;//bds

					if (entityDropDown != null
						&& entityDropDown.SelectedItem != null
						&& !string.IsNullOrEmpty(entityDropDown.SelectedItem.Value))
					{
						string entityGuidString = entityDropDown.SelectedItem.Value;
						Guid.TryParse(entityGuidString, out entityGuid);

						entityID = entityDropDown.SelectedItem.Text;
					}

					if (entityGuid == Guid.Empty)
					{
						throw new ApplicationException("Enterprise entity is required");
					}

					translation.EntityGuid = entityGuid;
					translation.EntityID = entityID;

					if (translation.IdentityGuid == Guid.Empty)
					{
						translation.IdentityGuid = FMChannelHelper.MakeCall<IFMAETranslations, Guid>(translationsClient => translationsClient.Add(this.Security, translation));
					}
					else
					{
						FMChannelHelper.MakeCall<IFMAETranslations>(translationsClient => translationsClient.Modify(this.Security, translation));
					}
				}

				this.EnableControls(true);

				// Reset the edit index
				this.TranslationsGrid.EditIndex = -1;

                this.SessionTranslations = translations;

                // Bind data to the grid control
                this.BindData(translations);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		#endregion

		#region Form Control Events

		/// <summary>
		/// When the add button is clicked, add a new record to the grid of the appropriate translation type
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Not used</param>
		protected void AddButton_Click(object sender, EventArgs e)
		{
			try
			{
				FMAETranslationType translationType = FMAETranslationType.Unknown;

				if (this.EntityTypeDropDownList.SelectedItem != null && !string.IsNullOrEmpty(this.EntityTypeDropDownList.SelectedItem.Value))
				{
					Enum.TryParse(this.EntityTypeDropDownList.SelectedItem.Value, out translationType);
				}

				List<FMAETranslation> translations = this.SessionTranslations;

				if (translationType != FMAETranslationType.Unknown)
				{
					FMAETranslation translation = FMAETranslation.CreateTranslationObject(translationType);

					translations.Add(translation);
					this.EnableControls(false);

                    // The newly added row in the grid should be in edit.
                    // We must set the page index to the last page. 
                    // To calculate this, we divide the number of translations by the page size and round up. 
                    // We must subtract one since the page index is zero based.
				    this.TranslationsGrid.PageIndex = (int)Math.Ceiling((double)translations.Count / this.TranslationsGrid.PageSize) - 1;

                    // If there is no remainder when dividing by the page size
                    // Then the row added is the last record in the grid.
                    // Otherwise, the row added is the remainder when dividing the count of translations by the page size
                    // Keep in mind that the EditIndex is zero based, so we have to subtract one.
	                if (translations.Count % this.TranslationsGrid.PageSize == 0)
                    {
                        this.TranslationsGrid.EditIndex = this.TranslationsGrid.PageSize - 1;
                    }
                    else
                    {
                        this.TranslationsGrid.EditIndex = (translations.Count % this.TranslationsGrid.PageSize) - 1;
                    }
				}

				this.SessionTranslations = translations;
				this.BindData(translations);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

		/// <summary>
		/// When the user selects a different entity type in the entity type drop down, 
		/// display translations defined for that type of entity
		/// </summary>
		/// <param name="sender">Not used</param>
		/// <param name="e">Not used</param>
		protected void EntityTypeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				this.EnableControls(true);
				this.TranslationsGrid.EditIndex = -1;
			    this.TranslationsGrid.PageIndex = 0;

				this.UpdateView();
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}
		}

        /// <summary>
        /// Update the view when the user changes the grid page size
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void PageSizeDropDown_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.TranslationsGrid.EditIndex = -1;
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user changes the page, change the page and update the view
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">Identifies the page selected by the user.</param>
        protected void TranslationsGrid_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            try
            {
                this.EnableControls(true);
                this.TranslationsGrid.EditIndex = -1;
                this.TranslationsGrid.PageIndex = e.NewPageIndex;
                
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the find button, limit the results to those
        /// that contain the value the user typed into the find box
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void FindButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.SessionFindTextBoxSearchString = this.FindTextBox.Text;

                this.TranslationsGrid.PageIndex = 0;
                this.TranslationsGrid.EditIndex = -1;
                this.EnableControls(true);
                this.UpdateView(); 
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// When the user clicks the Show All button, 
        /// display all translations for the selected type of entity.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ShowAllButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                this.FindTextBox.Text = string.Empty;
                this.SessionFindTextBoxSearchString = string.Empty;

                this.TranslationsGrid.PageIndex = 0;
                this.TranslationsGrid.EditIndex = -1;
                this.EnableControls(true);
                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Export all of the translations to a CSV (comma-separated values) file.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ExportButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                // The entities that translation is supported for are the ones in the translation type enumeration, 
                // except for the "unknown" value.
                List<FMAETranslationType> translationTypes = Enum.GetValues(typeof(FMAETranslationType)).OfType<FMAETranslationType>().ToList<FMAETranslationType>();
                translationTypes.RemoveAll(translationType => translationType == FMAETranslationType.Unknown);

                List<FMAETranslation> translationsToExport = new List<FMAETranslation>();

                // Retrieve the translations of each type from the database
                foreach (FMAETranslationType translationType in translationTypes)
                {
                    FMAETranslationType blockScopedTranslationType = translationType;

                    translationsToExport.AddRange(
                        FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(
                            translationsClient => translationsClient.Enumerate(this.Security, blockScopedTranslationType)));
                }

                if (translationsToExport.Count == 0)
                {
                    throw new ApplicationException("No FMAE Translations found to export.");
                }

                // Build the export file.
                // First, add the column header names to the string.
                StringBuilder builder = new StringBuilder();
                builder.Append(string.Join(",", this.importExportColumnNames)).Append(Environment.NewLine);

                // Add the exported translations. We create one row per translation.
                List<string> rows = new List<string>();
                foreach (FMAETranslation translation in translationsToExport)
                {
                    List<string> values = new List<string> { translation.TranslationType.ToString(), translation.ID, translation.EntityID };
                    rows.Add(string.Join(",", values));
                }

                builder.Append(string.Join(Environment.NewLine, rows));

                // Write the file to the response. Tell the user that it's a CSV file.
                this.Response.Clear();
                this.Response.ContentType = "text/csv";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=FMAETranslationsExport.csv");
                this.Response.Write(builder.ToString());

                // Return the response.
                // The use of CompleteRequest() is preferred to Response.End() because Response.End() throws a ThreadAbortException.
                // However, we must set SuppressContent = true to avoid writing the page's html to the file as well.
                this.Response.Flush();
                this.Response.SuppressContent = true;
                this.Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

        /// <summary>
        /// Import translations from a CSV (comma-separated values) file.
        /// </summary>
        /// <param name="sender">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        protected void ImportButton_OnClick(object sender, EventArgs e)
        {
            try
            {
                if (this.Request.Files.AllKeys.Length == 0)
                {
                    return;
                }

                HttpPostedFile file = this.Request.Files[0];

                // The user must provide a file to import, and it must be a csv file
                if (string.IsNullOrEmpty(file.FileName) || file.ContentLength == 0)
                {
                    throw new ApplicationException("The name of the file to import must be provided.");
                }

                if (!file.FileName.EndsWith(".csv"))
                {
                    throw new ApplicationException("The file to import must be a .csv file.");
                }

                List<FMAETranslation> translationsToImport = new List<FMAETranslation>();
                List<string> importErrors = new List<string>();
                using (StreamReader reader = new StreamReader(file.InputStream))
                {
                    // The first line of the import file should be the column names
                    string columnNames = reader.ReadLine();
                    if (columnNames != string.Join(",", this.importExportColumnNames))
                    {
                        throw new Exception("The file selected does not appear to contain FMAE translations. The first row should contain the FMAE translation column names.");
                    }

                    // Read the entire file
                    while (!reader.EndOfStream)
                    {
                        // Each row in the file contains a translation's data
                        string translationsRow = reader.ReadLine();

                        if (!string.IsNullOrEmpty(translationsRow))
                        {
                            string[] translationsValues = translationsRow.Split(',');

                            // Something might be wrong with the csv file. Continue trying to process the next row.
                            if (translationsValues.Length < 3)
                            {
                                continue;
                            }
                           
                            // Create a translation of the provided type, set the values on the new object with those from the row, 
                            // and add the translation to the list of translations to import
                            string translationTypeString = translationsValues[0];
                            FMAETranslationType translationType = FMAETranslationType.Unknown;

                            if (string.IsNullOrWhiteSpace(translationTypeString))
                            {
                                importErrors.Add("Entity Type must be provided.");
                                continue;
                            }
                            else if (!Enum.TryParse(translationTypeString, true, out translationType) || translationType == FMAETranslationType.Unknown)
                            {
                                importErrors.Add("Unrecognized Entity Type: " + translationTypeString + ".");
                                continue;
                            }

                            FMAETranslation translation = FMAETranslation.CreateTranslationObject(translationType);

                            string fmaeID = translationsValues[1];
                            if (string.IsNullOrWhiteSpace(fmaeID))
                            {
                                importErrors.Add("FMAE ID must be provided.");
                                continue;
                            }

                            translation.ID = fmaeID;

                            string enterpriseEntityID = translationsValues[2];
                            if (string.IsNullOrWhiteSpace(enterpriseEntityID))
                            {
                                importErrors.Add("Enterprise Entity ID must be provided.");
                                continue;
                            }

                            translation.EntityID = enterpriseEntityID;

                            translationsToImport.Add(translation);
                        }
                    }
                }

                if (translationsToImport.Count > 0)
                {
                    // Perform the import
                    importErrors.AddRange(FMChannelHelper.MakeCall<IFMAETranslations, List<string>>(translationsClient => translationsClient.Import(this.Security, translationsToImport)));
                }
                else
                {
                    importErrors.Add("No valid translations found in the import file.");
                }

                // Display the import results TextBox and Label, and populate the TextBox with the errors found during the import process
                this.ImportResultsLabel.Visible = true;
                this.ImportResultsTextBox.Visible = true;

                if (importErrors.Count == 0)
                {
                    this.ImportResultsTextBox.Text = "Import successful!";
                }
                else
                {
                    this.ImportResultsTextBox.Text = string.Join(Environment.NewLine, importErrors.Distinct());
                }

                this.UpdateView();
            }
            catch (Exception ex)
            {
                this.ErrorHandler(ex);
            }
        }

		#endregion

		#region Form Control Population

		/// <summary>
		/// Get the types of entities FMAE translation is supported for. The values are displayed
		/// in a drop down box at the top of the form. When the user changes the value in the box, 
		/// we display the translations defined for that type of entity.
		/// </summary>
		/// <returns>The types of entities FMAE translation is supported for.</returns>
		protected ICollection EnumerateTranslationTypes()
		{
			List<FMAETranslationType> translationTypes = new List<FMAETranslationType>();

			try
			{
				// The entities translation is supported for are the ones in the enumeration, 
				// except for the "unknown" value.
				translationTypes = Enum.GetValues(typeof(FMAETranslationType)).OfType<FMAETranslationType>().ToList<FMAETranslationType>();
				translationTypes.RemoveAll(translationType => translationType == FMAETranslationType.Unknown);
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return translationTypes;
		}

		/// <summary>
		/// Get the entities to display in the drop down list.
		/// The entity selected represents the FuelsManager record the legacy value should 
		/// be translated to.
		/// </summary>
		/// <returns>Entities to display in the Enterprise Entity drop down list</returns>
		protected ICollection EnumerateEntities()
		{
			try
			{
				FMAETranslationType translationType = FMAETranslationType.Unknown;

				if (this.EntityTypeDropDownList.SelectedItem != null && !string.IsNullOrEmpty(this.EntityTypeDropDownList.SelectedItem.Value))
				{
					Enum.TryParse(this.EntityTypeDropDownList.SelectedItem.Value, out translationType);
				}

				if (translationType == FMAETranslationType.Company)
				{
				    CompanyCollectionClass companyList =
				        FMChannelHelper.MakeCall<ICompanies, CompanyCollectionClass>(
				            companies => companies.EnumerateExt(this.Security, false, false, false));

				    return companyList;
				}
				else if (translationType == FMAETranslationType.Product)
				{
				    ProductCollectionClass productList =
				        FMChannelHelper.MakeCall<IProducts, ProductCollectionClass>(products => products.Enumerate(this.Security));

				    return productList;
			    }
			}
			catch (Exception ex)
			{
				this.ErrorHandler(ex);
			}

			return null;
		}

		#endregion

		#region Form Methods

		/// <summary>
		/// Enable or disable controls on the screen.
		/// </summary>
		/// <param name="enable">True to enable, false to disable.</param>
		private void EnableControls(bool enable)
		{
			this.AddButton.Enabled = enable;
		    this.AddButtonTop.Enabled = enable;
		    this.ImportButton.Enabled = enable;
		    this.ExportButton.Enabled = enable;
		}

		/// <summary>
		/// Using the type of entity selected, retrieve any translations defined for that type from the database
		/// and display them on the grid
		/// </summary>
		private void UpdateView()
		{
			FMAETranslationType translationType = FMAETranslationType.Unknown;

			if (this.EntityTypeDropDownList.SelectedItem != null && !string.IsNullOrEmpty(this.EntityTypeDropDownList.SelectedItem.Value))
			{
				Enum.TryParse(this.EntityTypeDropDownList.SelectedItem.Value, out translationType);
			}

			List<FMAETranslation> translations = new List<FMAETranslation>();

			if (translationType != FMAETranslationType.Unknown)
			{
			    if (string.IsNullOrWhiteSpace(this.SessionFindTextBoxSearchString))
			    {
			        translations =
			            FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(
			                translationsClient => translationsClient.Enumerate(this.Security, translationType));
			    }
			    else
			    {
                    translations =
                        FMChannelHelper.MakeCall<IFMAETranslations, List<FMAETranslation>>(
                            translationsClient => translationsClient.EnumerateAndFilter(this.Security, translationType, this.SessionFindTextBoxSearchString));  
			    }
			}

			this.SessionTranslations = translations;
            this.PageSizeDropDown.SetPageSize(this.TranslationsGrid, translations.Count);

			this.BindData(translations);
		}

		/// <summary>
		/// Bind the translations provided to the grid
		/// </summary>
		/// <param name="translations">The translations to bind to the grid</param>
		private void BindData(List<FMAETranslation> translations)
		{
			this.TranslationsGrid.DataSource = translations;
			this.TranslationsGrid.DataBind();
		}

        /// <summary>
        /// Hide the Import Results label and text box if they are visible.
        /// </summary>
        private void HideImportResultsControls()
        {
            if (this.ImportResultsLabel.Visible)
            {
                this.ImportResultsLabel.Visible = false;
            }

            if (this.ImportResultsTextBox.Visible)
            {
                this.ImportResultsTextBox.Visible = false;
            }
        }

		#endregion

		#region FuelsManager Menu Support

		/// <summary>
		/// Gets a list of menu items that should be displayed for the current user.
		/// </summary>
		/// <param name="security">The security object of the current session</param>
		/// <param name="siteGroup">Whether the current logged-in site is a site group</param>
		/// <param name="options">Hardware key options</param>
		/// <returns>
		/// List of menu items to be displayed
		/// </returns>
		public List<FMMenuItem> GetMenuItems(SecurityClass security, bool siteGroup, ushort word1,ushort word2, ushort useNewLicenseKey, uint options)
		{
            if(useNewLicenseKey == 1)
            {
                //if ((word1 & 0x01) != 0x01)
                //    return null;
            }
            else
            {

            }
			List<FMMenuItem> items = new List<FMMenuItem>();

			if (!security.HasRight(RIGHT.MODIFY_SITES_AND_SITE_GROUPS))
			{
				return null;
			}

			// The menu item is only displayed if the current site is a site group and that site group is the one specified in tblConfigurationSetting. 
			bool display = false;

			if (siteGroup)
			{
                string siteGroupName = FMChannelHelper.MakeCall<IConfigurationSettings, string>(
                            configurationSettings => configurationSettings.GetKeyValueByKey(this.Security, ConfigurationSettingDOClass.Key_FMAETranslationsConfigurationSiteGroup));

				if (String.Compare(security.SiteID, siteGroupName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					display = true;
				}
			}

			if (display)
			{
				items.Add(new FMMenuItem()
				{
					MenuItemType = FMMenuItemType.CONFIG_OTHER_FMAE_INTERFACE_TRANSLATIONS,
					RootMenuName = "Configuration",
					CategoryName = "Other",
					ItemName = "FMAE Translations",
					NavigateUrl = "FMAETranslationsForm.aspx",
					ApplyDataDictionary = ApplyDataDictionary.Apply,
				});
			}

			return items;
		}

		#endregion

	}
}